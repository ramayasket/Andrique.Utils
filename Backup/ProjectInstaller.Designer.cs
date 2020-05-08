namespace FileSync
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.FileSyncServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.FileSyncServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // FileSyncServiceProcessInstaller
            // 
            this.FileSyncServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.FileSyncServiceProcessInstaller.Password = null;
            this.FileSyncServiceProcessInstaller.Username = null;
            // 
            // FileSyncServiceInstaller
            // 
            this.FileSyncServiceInstaller.Description = "File Synchronization Service";
            this.FileSyncServiceInstaller.DisplayName = "File Synchronization Service";
            this.FileSyncServiceInstaller.ServiceName = "FileSynchronization";
            this.FileSyncServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.FileSyncServiceProcessInstaller,
            this.FileSyncServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller FileSyncServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller FileSyncServiceInstaller;
    }
}