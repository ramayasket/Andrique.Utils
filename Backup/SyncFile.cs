using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Resources;
using System.Reflection;
using System.Web;
using System.IO;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Timers;
using System.Collections.Specialized;
using System.Configuration;

namespace FileSync
{
    class SyncFile
    {
       
        static Configuration oConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        static string domain = ConfigurationManager.AppSettings["remoteDomain"]; 
        static string srcRoot = ConfigurationManager.AppSettings["localFolder"];
        static string rmtRoot = ConfigurationManager.AppSettings["remoteFolder"]; 
        static string userID = ConfigurationManager.AppSettings["userName"];
        static string password = null;
        static string driveName;
        static Timer myTimer = new Timer();
        static bool connected;        


        static EventLog EventLog;

        public SyncFile(EventLog log)
        {
            EventLog = log;
            myTimer.Elapsed += new ElapsedEventHandler(unmapDrive);
            myTimer.Interval = 5000;

            handlePassword();
        }

        private void handlePassword()
        {
            //if the variable has been assigned value, nothing to do
            if (password != null) return;
            //get value from app.config
            string pwd = oConfig.AppSettings.Settings["Password"].Value;
            //check if it is encrypted. Assume plain text password is no longer than 50 characters.
            if (pwd.Length < 50)
            {
                password = pwd;
                encryptPassword(pwd);
            }
            else
                password = decryptPassword(pwd);
        }

        private void encryptPassword(string pwd)
        {
            oConfig.AppSettings.Settings["Password"].Value = SecurePassword.EncryptString(SecurePassword.ToSecureString(pwd));
            oConfig.Save(ConfigurationSaveMode.Modified);

        }
        private string decryptPassword(string pwd)
        {
            return SecurePassword.ToInsecureString(SecurePassword.DecryptString(pwd));
        }

        public void copyFile(System.IO.FileSystemEventArgs e)
        {
            myTimer.Stop();
            if(mapDrive())
            {
                try
                {
                    string source = e.FullPath;
                    bool exists = System.IO.Directory.Exists(source);
                    string dest = source.Replace(srcRoot, driveName);

                    if (exists)
                    {
                        Directory.CreateDirectory(dest);
                    }
                    else
                    {
                        File.Copy(source, dest, true);
                    }
                    
                }
                catch (Exception ioe)
                {
                    EventLog.WriteEntry("FileSync Copy File failed: " + ioe.Message);
                }
                
                myTimer.Start();
            }

        }

        public void renameFile(System.IO.RenamedEventArgs e)
        {
            myTimer.Stop();

            if (mapDrive())
            {
                try
                {
                    string source = e.OldFullPath.Replace(srcRoot, rmtRoot);
                    string dest = e.FullPath.Replace(srcRoot, rmtRoot);
                    bool exists = System.IO.Directory.Exists(source);
                    if (exists)
                    {
                        Directory.Move(source, dest);
                    }
                    else
                    {
                        File.Move(source, dest);
                    }
                }
                catch (Exception ioe)
                {
                    EventLog.WriteEntry("FileSync Rename File failed: " + ioe.Message);
                }
                
                myTimer.Start();
            }
        }

        public void deleteFile(System.IO.FileSystemEventArgs e)
        {
            myTimer.Stop();

            if (mapDrive())
            {
                try
                {
                    string dest = e.FullPath.Replace(srcRoot, rmtRoot);
                    bool exists = System.IO.Directory.Exists(dest);
                    if (exists)
                    {
                        Directory.Delete(dest, true);
                    }
                    else
                    {
                        File.Delete(dest);
                    }
                }
                catch (Exception ioe)
                {
                    EventLog.WriteEntry("FileSync Delete File failed: " + ioe.Message);
                }
                

                myTimer.Start();
            }

        }

   

        private bool mapDrive()
        {
            if (connected) return true;

            driveName = FindNextAvailableDriveLetter() + ":";
            NetWorkDrive oNetDrive = new NetWorkDrive();

            try
            {
                //set propertys
                oNetDrive.Force = false;
                oNetDrive.Persistent = true;
                oNetDrive.LocalDrive = driveName;
                oNetDrive.PromptForCredentials = false;
                oNetDrive.ShareName = rmtRoot;
                oNetDrive.SaveCredentials = true;
                
                oNetDrive.MapDrive(userID, password);
                
                connected = true;
                EventLog.WriteEntry(driveName + " drive mapped");
            }
            catch (Exception err)
            {
                EventLog.WriteEntry("FileSync map drive failed: " + err.Message + " use " + userID + "/" + password);
                return false;               
            }
            finally
            {
                oNetDrive = null;
            }

            return true;
        }

        public static void unmapDrive(object source, ElapsedEventArgs e)
        {
            NetWorkDrive oNetDrive = new NetWorkDrive();
            try
            {
                //unmap the drive
                oNetDrive.Force = false;
                oNetDrive.LocalDrive = driveName;
                oNetDrive.UnMapDrive();
                connected = false;
                EventLog.WriteEntry(driveName + " drive unmapped");
            }
            catch (Exception err)
            {
                EventLog.WriteEntry("FileSync unmap drive failed: " + err.Message);
            }
            finally
            {
                oNetDrive = null;
            }
            myTimer.Stop();
        }

        public static string FindNextAvailableDriveLetter()
        {
            // build a string collection representing the alphabet
            StringCollection alphabet = new StringCollection();

            int lowerBound = Convert.ToInt16('a');
            int upperBound = Convert.ToInt16('z');
            for (int i = upperBound; i > lowerBound; i--)
            {
                char driveLetter = (char)i;
                alphabet.Add(driveLetter.ToString());
            }

            // get all current drives
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                alphabet.Remove(drive.Name.Substring(0, 1).ToLower());
            }

            if (alphabet.Count > 0)
            {
                return alphabet[0];
            }
            else
            {
                throw (new ApplicationException("No drives available."));
            }
        }
    }
}
