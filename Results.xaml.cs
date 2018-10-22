using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Configuration;

namespace FileManager {
    /// <summary>
    /// Interaction logic for Results.xaml
    /// </summary>
    public partial class Results : Window {

        public Results() {
            InitializeComponent();
            BindGrid();
        }


        private void BindGrid() {

            /*
             select f.FileName, f.Length, d.DirectoryName, f.Checksum, p.DirectoryName ParentDir from [dbo].[File] f with (nolock) 
inner join dbo.Directory d with (nolock)  on d.DirectoryID = f.DirectoryID
inner join dbo.Directory p with (nolock)  on p.DirectoryID = d.ParentID
where [CHECKSUM] in (

  select [CHECKSUM]
  from [dbo].[File]
  group by [CHECKSUM]
  having count(*) > 1
)
order by f.Length desc, f.FileName, d.DirectoryName, p.DirectoryName
             */

            using (FileSystemLibraryEntities _db = new FileSystemLibraryEntities(ConfigurationManager.ConnectionStrings["FileSystemLibraryEntities"].ConnectionString)) {
                //var data = (from f in _db.Files
                //            join d in _db.Directories on f.DirectoryID equals d.DirectoryID
                //            join p in _db.Directories on d.DirectoryID equals p.ParentID
                //            where f.Checksum in (
                //                from fg in _db.Files
                //                group fg by fg.Checksum into g = Group

                //                )

                

                var group1 = from fg in _db.Files
                             where fg.Ignore == false
                             group fg.Checksum by fg.Checksum into grp
                             where grp.Count() > 1
                             select new { Count = grp.Count(), Checksum = grp.Key };

                List<FileData> data = (from f in _db.Files
                                       join d in _db.Directories on f.DirectoryID equals d.DirectoryID
                                       //join p in _db.Directories on d.DirectoryID equals p.ParentID
                                       join ch1 in group1 on f.Checksum equals ch1.Checksum
                                       where d.Ignore == false && f.Ignore == false
                                       orderby f.Length descending, f.FileName, f.DirectoryID
                                       select new FileData { FileID = f.FileID, Filename = f.FileName, Length = f.Length, DirectoryName = d.DirectoryName, Checksum = f.Checksum }).ToList<FileData>();
                //select new FileData { FileID = f.FileID, Filename = f.FileName, Length = f.Length, DirectoryName = d.DirectoryName, Checksum = f.Checksum, ParentName = p.DirectoryName }).ToList<FileData>();

                dataGrid1.ItemsSource = new ObservableCollection<FileData>(data);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e) {

            using (FileSystemLibraryEntities _db = new FileSystemLibraryEntities(ConfigurationManager.ConnectionStrings["FileSystemLibraryEntities"].ConnectionString)) {
                ObservableCollection<FileData> col = (ObservableCollection<FileData>)dataGrid1.ItemsSource;
                do {
                    FileData selectedItem = (FileData)dataGrid1.SelectedItem;
                    string fileName = selectedItem.DirectoryName + "\\" + selectedItem.Filename;
                    if (System.IO.File.Exists(fileName)) {
                        try {
                            DeleteFile(fileName);
                            var file = from f in _db.Files where f.FileID == selectedItem.FileID select f;
                            if (file.Count() > 0)
                                _db.Files.DeleteObject(file.First());
                            col.RemoveAt(dataGrid1.SelectedIndex);
                        } catch (Exception ex) {
                            MessageBox.Show(ex.ToString());
                            dataGrid1.SelectedIndex = -1;
                        }
                    } else {
                        //if file doesn't exist delete only from db
                        var file = from f in _db.Files where f.FileID == selectedItem.FileID select f;
                        if (file.Count() > 0)
                            _db.Files.DeleteObject(file.First());
                        col.RemoveAt(dataGrid1.SelectedIndex);
                    }
                } while (dataGrid1.SelectedIndex != -1);
                _db.SaveChanges();
                BindGrid();
            }
        }

        private void btnDelFolder_Click(object sender, RoutedEventArgs e) {
            if (dataGrid1.SelectedIndex != -1) {
                List<string> selectedFolders = new List<string>();
                foreach (var item in dataGrid1.SelectedItems) {
                    selectedFolders.Add(((FileData)item).DirectoryName);
                }
                //string dirName = ((FileData)dataGrid1.SelectedItem).DirectoryName;
                List<long> gridRowsToDelete = new List<long>();
                using (FileSystemLibraryEntities _db = new FileSystemLibraryEntities(ConfigurationManager.ConnectionStrings["FileSystemLibraryEntities"].ConnectionString)) {
                    foreach (FileData fileData in dataGrid1.ItemsSource) {
                        //if (fileData.DirectoryName.Equals(dirName)) {
                        if (selectedFolders.Contains(fileData.DirectoryName)) {
                            string fileName = fileData.DirectoryName + "\\" + fileData.Filename;
                            if (System.IO.File.Exists(fileName)) {
                                try {
                                    DeleteFile(fileName);
                                    var file = from f in _db.Files where f.FileID == fileData.FileID select f;
                                    if (file.Count() > 0)
                                        _db.Files.DeleteObject(file.First());
                                    gridRowsToDelete.Add(fileData.FileID);
                                } catch (Exception ex) {
                                    MessageBox.Show(ex.ToString());
                                }
                            }
                        }
                    }
                    _db.SaveChanges();
                    ObservableCollection<FileData> allGridItems = (ObservableCollection<FileData>)dataGrid1.ItemsSource;
                    List<FileData> itemsToDelete = (from c in allGridItems
                                                    join g in gridRowsToDelete on c.FileID equals g
                                                    select c).ToList<FileData>();
                    foreach (FileData item in itemsToDelete) {
                        allGridItems.Remove(item);
                    }
                }
            }
            BindGrid();
        }

        private void DeleteFile(string fileName) {
            //reseting hidden and readonly attributes
            System.IO.FileAttributes fa = System.IO.File.GetAttributes(fileName);
            if ((fa & System.IO.FileAttributes.ReadOnly) == System.IO.FileAttributes.ReadOnly ||
                (fa & System.IO.FileAttributes.Hidden) == System.IO.FileAttributes.Hidden) {
                System.IO.File.SetAttributes(fileName, System.IO.FileAttributes.Normal);
            }
            System.IO.File.Delete(fileName);
        }

        private void btnIgnoreSelected_Click(object sender, RoutedEventArgs e) {
            using (FileSystemLibraryEntities _db = new FileSystemLibraryEntities(ConfigurationManager.ConnectionStrings["FileSystemLibraryEntities"].ConnectionString)) {
                ObservableCollection<FileData> col = (ObservableCollection<FileData>)dataGrid1.ItemsSource;
                do {
                    FileData selectedItem = (FileData)dataGrid1.SelectedItem;

                        //if file doesn't exist delete only from db
                        List<File> file = (from f in _db.Files where f.FileID == selectedItem.FileID select f).ToList<File>();
                        if (file.Count() > 0)
                            (file[0]).Ignore = true;
                        col.RemoveAt(dataGrid1.SelectedIndex);
                    
                } while (dataGrid1.SelectedIndex != -1);
                _db.SaveChanges();
                BindGrid();
            }
        }

        private void btnIgnoreFolder_Click(object sender, RoutedEventArgs e) {
            if (dataGrid1.SelectedIndex != -1) {
                List<string> selectedFolders = new List<string>();
                foreach (var item in dataGrid1.SelectedItems) {
                    selectedFolders.Add(((FileData)item).DirectoryName);
                }
                //string dirName = ((FileData)dataGrid1.SelectedItem).DirectoryName;
                List<long> gridRowsToDelete = new List<long>();
                using (FileSystemLibraryEntities _db = new FileSystemLibraryEntities(ConfigurationManager.ConnectionStrings["FileSystemLibraryEntities"].ConnectionString)) {
                    foreach (string dirName in selectedFolders) {
                        List<Directory> dir = (from d in _db.Directories where d.DirectoryName == dirName select d).ToList<Directory>();
                        if (dir.Count > 0) {
                            dir[0].Ignore = true;
                        }
                    }
                    _db.SaveChanges();
                }
            }
            BindGrid();
        }

        private void btnResetIgnored_Click(object sender, RoutedEventArgs e) {
            using (FileSystemLibraryEntities _db = new FileSystemLibraryEntities(ConfigurationManager.ConnectionStrings["FileSystemLibraryEntities"].ConnectionString)) {
                List<Directory> dirs = (from d in _db.Directories where d.Ignore == true select d).ToList<Directory>();
                foreach (Directory dir in dirs) {
                    dir.Ignore = false;
                }
                List<File> files = (from f in _db.Files where f.Ignore == true select f).ToList<File>();
                foreach (File file in files) {
                    file.Ignore = false;
                }
                _db.SaveChanges();
            }
            BindGrid();
        }

    }

    class FileData {
        public string Checksum { get; set; }
        public string Filename { get; set; }
        public long? Length { get; set; }
        public string DirectoryName { get; set; }
        public long FileID { get; set; }
    }
}
