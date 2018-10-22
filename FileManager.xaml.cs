using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace FileManager {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FileManagerW : Window {

        private FileSystemLibraryEntities _db = null;
        private System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed();
        private long fileCount = 1;
        private BackgroundWorker _backgroundWorkerGo = null;
        private BackgroundWorker _backgroundWorkerDeleteEmpty = null;

        private string[] excludeFolders = null;
        private string[] excludeFiles = null;

        private ManualResetEvent reset = new ManualResetEvent(true);

        public FileManagerW() {
            InitializeComponent();

            txtFolderName.Text = ConfigurationManager.AppSettings["folder"];

            excludeFolders = ConfigurationManager.AppSettings["excludeFolders"].Split(',');
            excludeFiles = ConfigurationManager.AppSettings["excludeFiles"].Split(',');

            _backgroundWorkerGo = new BackgroundWorker();
            _backgroundWorkerDeleteEmpty = new BackgroundWorker();
            // Set up the Background Worker Events
            _backgroundWorkerGo.WorkerSupportsCancellation = true;
            _backgroundWorkerGo.WorkerReportsProgress = true;
            _backgroundWorkerGo.DoWork += DoWorkHashFiles;
            _backgroundWorkerGo.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
            _backgroundWorkerGo.ProgressChanged += _backgroundWorker_ReportProgress;

            _backgroundWorkerDeleteEmpty.WorkerSupportsCancellation = true;
            _backgroundWorkerDeleteEmpty.WorkerReportsProgress = true;
            _backgroundWorkerDeleteEmpty.DoWork += DoWorkDeleteEmptyFolders;
            _backgroundWorkerDeleteEmpty.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
            _backgroundWorkerDeleteEmpty.ProgressChanged += _backgroundWorker_ReportProgress;



        }

        /// <summary>
        /// Run the Background Worker which checksums the files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGo_Click(object sender, RoutedEventArgs e) {
            if (!_backgroundWorkerGo.IsBusy &&
                !_backgroundWorkerDeleteEmpty.IsBusy) {
                btnStop.IsEnabled = true;
                btnDelEmptyFolders.IsEnabled = false;
                btnGo.IsEnabled = false;
                btnResetData.IsEnabled = false;
                btnResults.IsEnabled = false;
                btnPause.IsEnabled = true;

                fileCount = 1;
                _backgroundWorkerGo.RunWorkerAsync();
            }
        }

        private void DoWorkHashFiles(object sender, DoWorkEventArgs e) {

            DirectoryInfo di = new DirectoryInfo(SetRootFolder());

            using (_db = new FileSystemLibraryEntities(ConfigurationManager.ConnectionStrings["FileSystemLibraryEntities"].ConnectionString)) {
                ProcessDirectory(di);
            }

            if (_backgroundWorkerGo.CancellationPending) {
                e.Cancel = true;
            }

        }

        private string SetRootFolder() {
            string folderName = ((string)Dispatcher.Invoke((Func<String>)(() => (txtFolderName.Text ?? "?").ToString()))).Trim();

            if (folderName.LastIndexOf("\\") == (folderName.Length - 1))
                folderName = folderName.Substring(0, txtFolderName.Text.Length - 1);

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(SetFolderText), folderName);

            string rootFolder = ConfigurationManager.AppSettings["folder"];
            //persist folder name in config file
            if (!rootFolder.Equals(folderName, StringComparison.InvariantCultureIgnoreCase)) {
                Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cfg.AppSettings.Settings.Remove("folder");
                cfg.AppSettings.Settings.Add("folder", folderName);
                cfg.Save();
                ConfigurationManager.RefreshSection(cfg.AppSettings.SectionInformation.SectionName);
            }

            return folderName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _backgroundWorker_ReportProgress(object sender, ProgressChangedEventArgs e) {
            lblCounter.Content = e.ProgressPercentage.ToString();
            stBarStatus.Content = e.UserState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Cancelled) {
                stBarStatus.Content = "Cancelled";
            } else if (e.Error != null) {
                stBarStatus.Content = "Exception Thrown";
                txtMessages.Text += e.Error.ToString();
            } else {
                stBarStatus.Content = "Ready...";

            }
            btnStop.IsEnabled = false;
            btnGo.IsEnabled = true;
            btnDelEmptyFolders.IsEnabled = true;
            btnResetData.IsEnabled = true;
            btnResults.IsEnabled = true;
            btnPause.IsEnabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirInfo"></param>
        private void ProcessDirectory(DirectoryInfo dirInfo) {
            if (_backgroundWorkerGo.CancellationPending) {
                return;
            } else {
                if (!excludeFolders.Contains(dirInfo.Name)) {
                    //find directoryID, or save the new directory and get the new id
                    List<Directory> dirs = (from dir
                                                in _db.Directories
                                            where dir.DirectoryName.Equals(dirInfo.FullName)
                                            select dir).ToList<Directory>();
                    long directoryID = -1;
                    if (dirs.Count == 0) {
                        Directory newDir = new Directory();
                        newDir.DirectoryName = dirInfo.FullName;
                        long parentID = GetParentDirectory(dirInfo.FullName);
                        if (parentID != -1) {
                            newDir.ParentID = parentID;
                        }
                        _db.AddToDirectories(newDir);
                        _db.SaveChanges();
                        directoryID = newDir.DirectoryID;
                    } else {
                        directoryID = dirs.First().DirectoryID;
                    }

                    //now check if the new directory is an existing directory's parent
                    List<Directory> children = (from dir
                                            in _db.Directories
                                                where
                                                dir.ParentID == null &&
                                                dir.DirectoryName.Length > dirInfo.FullName.Length &&
                                                dir.DirectoryName.Substring(dirInfo.FullName.Length).IndexOf("\\") == -1
                                                select dir).ToList<Directory>();
                    foreach (Directory kid in children) {
                        kid.ParentID = directoryID;
                    }
                    _db.SaveChanges();

                    //record the files
                    ProcessFiles(dirInfo.GetFiles(), directoryID);

                    //recurse to child folders
                    foreach (DirectoryInfo dirInf in dirInfo.GetDirectories()) {
                        reset.WaitOne();
                        ProcessDirectory(dirInf);
                    }
                }
            }
        }

        private long GetParentDirectory(string directoryName) {
            if (directoryName.Length == 0)
                return -1;

            int lastPos = directoryName.LastIndexOf("\\");
            if (lastPos == -1)
                return -1; // top drive letter reached
            string parentName = directoryName.Substring(0, lastPos);
            List<Directory> dirs = (from dir
                                        in _db.Directories
                                    where dir.DirectoryName.Equals(parentName)
                                    select dir).ToList<Directory>();
            if (dirs.Count == 0) {
                return -1;
            } else {
                return dirs.First().DirectoryID;
            }

        }

        private void ProcessFiles(FileInfo[] filesInfo, long directoryID) {
            foreach (FileInfo fileInfo in filesInfo) {
                try {
                    if (!excludeFiles.Contains(fileInfo.Name)) {
                        //show the file being processed on the status bar
                        _backgroundWorkerGo.ReportProgress((int)fileCount, fileInfo.FullName);

                        //look if the file exists
                        List<File> files = (from file
                                                in _db.Files
                                            where file.FileName.Equals(fileInfo.Name)
                                            && file.DirectoryID == directoryID
                                            select file).ToList<File>();
                        File newFile = null;
                        if (files.Count == 0)
                            newFile = new File();
                        else
                            newFile = files.First();
                        try {
                            newFile.Checksum = GetChecksum(fileInfo);
                        } catch (Exception ex) {
                            newFile.ErrorMessage = ex.Message;
                        }
                        newFile.CreationTime = fileInfo.CreationTime;
                        newFile.CreationTimeUTC = fileInfo.CreationTimeUtc;
                        newFile.DirectoryID = directoryID;
                        newFile.FileName = fileInfo.Name;
                        newFile.Length = fileInfo.Length;

                        if (files.Count == 0)
                            _db.AddToFiles(newFile);

                        _db.SaveChanges();
                        fileCount++;
                    }
                } catch (Exception ex) {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(AddMessage), ex.ToString() + Environment.NewLine);
                } finally {
                }
            }
        }

        private string GetChecksum(FileInfo fileInfo) {
            string checkSum = null;
            FileStream fs = null;
            try {
                fs = System.IO.File.OpenRead(fileInfo.FullName);
                sha1.ComputeHash(fs);
                fs.Close();
                checkSum = Convert.ToBase64String(sha1.Hash);
            } catch (Exception ex) {
                throw ex;
            } finally {
                try { fs.Close(); } catch { }
            }
            return checkSum;
        }

        private void SetFolderText(string folderName) {
            txtFolderName.Text = folderName;
        }

        private void AddMessage(string message) {
            AddMessage(message, false);
        }

        private void AddMessage(string message, bool replace) {
            if (replace) {
                txtMessages.Text = message;
            } else {
                txtMessages.Text = message + Environment.NewLine + txtMessages.Text;
            }
        }

        private void ToggleButtonState(Button button, bool value) {
            button.IsEnabled = value;
        }

        private void SetStatusBarMessage(string message) {
            stBarStatus.Content = message;
        }

        private void btnResults_Click(object sender, RoutedEventArgs e) {
            Results window = new Results();
            window.Show();
        }

        private void btnResetData_Click(object sender, RoutedEventArgs e) {
            MessageBoxResult userResponse = MessageBox.Show("This will empty the database content." + Environment.NewLine + "You will need to scan the filestructure again, which can take a long time." + Environment.NewLine + "Are you sure?", "Think before you click!", MessageBoxButton.OKCancel);
            if (userResponse == MessageBoxResult.OK) {
                using (_db = new FileSystemLibraryEntities(ConfigurationManager.ConnectionStrings["FileSystemLibraryEntities"].ConnectionString)) {
                    _db.ResetData();
                }
                lblCounter.Content = "0";
                stBarStatus.Content = "Ready...";
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e) {
            if (_backgroundWorkerGo != null) {
                _backgroundWorkerGo.CancelAsync();
                if (btnPause.Content.Equals("Resume")) {
                    btnPause.Content = "Pause";
                    reset.Set();
                }
            }
        }

        private void btnPause_Click(object sender, RoutedEventArgs e) {
            if (btnPause.Content.Equals("Pause")) {
                btnPause.Content = "Resume";
                reset.Reset();
            } else {
                btnPause.Content = "Pause";
                reset.Set();
            }
        }

        private void btnDelEmptyFolders_Click(object sender, RoutedEventArgs e) {
            if (!_backgroundWorkerGo.IsBusy &&
                !_backgroundWorkerDeleteEmpty.IsBusy) {
                btnStop.IsEnabled = true;
                btnDelEmptyFolders.IsEnabled = false;
                btnGo.IsEnabled = false;
                btnResetData.IsEnabled = false;
                btnResults.IsEnabled = false;
                btnPause.IsEnabled = true;

                fileCount = 1;
                _backgroundWorkerDeleteEmpty.RunWorkerAsync();
            }
        }

        private void DoWorkDeleteEmptyFolders(object sender, DoWorkEventArgs e) {

            DirectoryInfo dirInfo = new DirectoryInfo(SetRootFolder());
            if (dirInfo.Exists) {
                fileCount = 0;
                DeleteEmptyFolders(dirInfo);
                if (_backgroundWorkerDeleteEmpty.CancellationPending) {
                    e.Cancel = true;
                }
                //at the end, delete the root folder too if it's empty
                dirInfo.Refresh();
                if ((dirInfo.GetDirectories().Count() == 0) && (dirInfo.GetFiles().Count() == 0)) {
                    try {
                        DeleteFolder(dirInfo);
                        fileCount++;
                        _backgroundWorkerDeleteEmpty.ReportProgress((int)fileCount, dirInfo.FullName);
                    } catch (Exception ex) {
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(AddMessage), ex.ToString());
                    }
                }
            } else {
                //txtMessages.Text = "Missing folder " + dirInfo.FullName + ".";
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string, bool>(AddMessage), "Missing folder " + dirInfo.FullName + ".", true);
            }
        }

        private void DeleteEmptyFolders(DirectoryInfo directoryInfo) {
            bool foundEmptyFolder = false;

            do {
                reset.WaitOne();
                DirectoryInfo[] dirInfoSet = directoryInfo.GetDirectories();
                foundEmptyFolder = false;

                foreach (DirectoryInfo dir in dirInfoSet) {
                    if (!_backgroundWorkerDeleteEmpty.CancellationPending) {
                        reset.WaitOne();
                        try {
                            if (dir.GetDirectories().Count() > 0) {
                                //recursivity here
                                DeleteEmptyFolders(dir);
                            } else {
                                if (dir.GetFiles().Count() == 0) {
                                    DeleteFolder(dir);
                                    foundEmptyFolder = true;
                                    //show the file being processed on the status bar
                                    fileCount++;
                                    _backgroundWorkerDeleteEmpty.ReportProgress((int)fileCount, dir.FullName);
                                }
                            }
                        } catch (Exception ex) {
                            //txtMessages.Text += ex.ToString() + Environment.NewLine;
                            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string, bool>(AddMessage), ex.ToString() + Environment.NewLine, true);
                        }
                    } else {
                        break;
                    }
                }
            } while (foundEmptyFolder);
        }

        private void DeleteFolder(DirectoryInfo directoryInfo) {
            if ((directoryInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden ||
                (directoryInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                directoryInfo.Attributes = FileAttributes.Normal;
            }
            directoryInfo.Delete();
        }
    }
}
