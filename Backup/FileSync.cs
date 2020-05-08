using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Configuration;

// Using System.Threading because the event that will
// be raised from the monitor is comming from a different
// thread that will run on your code.
using System.Threading;


namespace FileSync
{
    public partial class FileSync : ServiceBase
    {
        FileSystemWatcher _watchFolder = new FileSystemWatcher();
        SyncFile fileSync;

        static string srcRoot = ConfigurationManager.AppSettings["localFolder"];

        public FileSync()
        {
            InitializeComponent();
            fileSync = new SyncFile(EventLog);
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("File Synchronization Service Started");
            // This is the path we want to monitor
            _watchFolder.Path = srcRoot;

            // Make sure you use the OR on each Filter because we need to monitor
            // all of those activities

            _watchFolder.IncludeSubdirectories = true;
            _watchFolder.NotifyFilter = System.IO.NotifyFilters.DirectoryName;

            _watchFolder.NotifyFilter = _watchFolder.NotifyFilter | System.IO.NotifyFilters.FileName;
            _watchFolder.NotifyFilter = _watchFolder.NotifyFilter | System.IO.NotifyFilters.Attributes;

            // Now hook the triggers(events) to our handler (eventRaised)
            _watchFolder.Changed += new FileSystemEventHandler(eventChangeRaised);
            _watchFolder.Created += new FileSystemEventHandler(eventCreateRaised);
            _watchFolder.Deleted += new FileSystemEventHandler(eventDeleteRaised);

            // Occurs when a file or directory is renamed in the specific path
            _watchFolder.Renamed += new System.IO.RenamedEventHandler(eventRenameRaised);

            // And at last.. We connect our EventHandles to the system API (that is all
            // wrapped up in System.IO)
            try
            {
                _watchFolder.EnableRaisingEvents = true;
            }
            catch (ArgumentException ex)
            {
                EventLog.WriteEntry("File Synchronization Service not start: " + ex.Message);
                //abortAcitivityMonitoring();
            }
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("File Synchronization Service Stoped");
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }



        /// <summary>
        /// Triggerd when an event is raised from the folder acitivty monitoring.
        /// All types exists in System.IO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">containing all data send from the event that got executed.</param>
        private void eventChangeRaised(object sender, System.IO.FileSystemEventArgs e)
        {
            fileSync.copyFile(e);
        }

        /// <summary>
        /// create new file in remote folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventCreateRaised(object sender, System.IO.FileSystemEventArgs e)
        {
            fileSync.copyFile(e);
        }

        /// <summary>
        /// delete file in remeote folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventDeleteRaised(object sender, System.IO.FileSystemEventArgs e)
        {
            fileSync.deleteFile(e);
        }

        /// <summary>
        /// When a folder or file is renamed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void eventRenameRaised(object sender, System.IO.RenamedEventArgs e)
        {
            fileSync.renameFile(e);
        }
    }
}
