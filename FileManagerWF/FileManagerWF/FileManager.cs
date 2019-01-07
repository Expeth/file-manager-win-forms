using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Security.AccessControl;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace FileManagerWF
{
    public class FileManager
    {
        public Drives Drives { get; set; }
        public Section Section { get; set; }

        public string LeftPath { get; set; }
        public string RightPath { get; set; }

        public DirectoryInfo LeftDirectory { get; set; }
        public DirectoryInfo RightDirectory { get; set; }

        private ListView _leftListView;
        private ListView _rightListView;

        // icons
        private List<ImageList> _leftImagelist;
        private List<ImageList> _rightImagelist;

        private Buffer _buffer;

        public FileManager(ListView left,          ListView right, 
                           ImageList leftImg,      ImageList rightImg, 
                           ImageList leftImgLarge, ImageList rightImgLarge)
        {
            _buffer = new Buffer();
            Drives = new Drives();
            LeftPath = RightPath = Drives.Disks[0].Name;
            Section = Section.Left;
            Directory.SetCurrentDirectory(LeftPath);

            _leftListView = left;
            _rightListView = right;

            //---------------------------------------------
            _leftImagelist = new List<ImageList>(2);
            _rightImagelist = new List<ImageList>(2);

            _leftImagelist.Add(leftImg);
            _leftImagelist.Add(leftImgLarge);

            _rightImagelist.Add(rightImg);
            _rightImagelist.Add(rightImgLarge);
            //---------------------------------------------

            LeftDirectory = new DirectoryInfo(LeftPath);
            RightDirectory = new DirectoryInfo(RightPath);

            SetUpListView(Section.Left);
            SetUpListView(Section.Right);
        }

        public void SetUpListView(Section s)
        {
            var imgList   = s == Section.Left ? _leftImagelist : _rightImagelist;
            var listView  = s == Section.Left ? _leftListView  : _rightListView;
            var directory = s == Section.Left ? LeftDirectory  : RightDirectory;

            imgList[0].Images.Clear();
            imgList[1].Images.Clear();
            listView.Items.Clear();

            // Arrow up
            int i = 0;
            if (directory.Parent != null)
            {
                listView.Items.Add(new ListViewItem("..", i++) { Tag = "Folder" });
                imgList[0].Images.Add(Images.arrowUp);
                imgList[1].Images.Add(Images.arrowUp);
            }

            int index = i;
            // Directories
            if (directory.GetDirectories().Length > 0)
            {
                imgList[0].Images.Add(Images.folder);
                imgList[1].Images.Add(Images.folder);

                foreach (var dir in directory.GetDirectories())
                {
                    listView.Items.Add(dir.Name, index);
                    listView.Items[i].Tag = "Folder";
                    listView.Items[i].SubItems.Add("");
                    listView.Items[i].SubItems.Add("<Папка>");
                    listView.Items[i++].SubItems.Add(dir.CreationTime.ToShortDateString());
                }
                index++;
            }
           
            // Files
            foreach (var file in directory.GetFiles())
            {
                imgList[0].Images.Add(Icon.ExtractAssociatedIcon(file.FullName).ToBitmap());
                imgList[1].Images.Add(Icon.ExtractAssociatedIcon(file.FullName).ToBitmap());

                listView.Items.Add(file.Name, index++);
                listView.Items[i].Tag = "File";
                listView.Items[i].SubItems.Add(file.Extension);
                listView.Items[i].SubItems.Add(file.Length.ToString());
                listView.Items[i++].SubItems.Add(file.CreationTime.ToShortDateString());
            }
        }

        public void ChangeViewMode(string mode)
        {
            var tmp = Section == Section.Left ? _leftListView : _rightListView;
            tmp.View = (View)Enum.Parse(typeof(View), mode);
        }

        public void ItemDoubleClick(object sender)
        {
            var tmp = sender as ListView;
            try
            {
                if ((string)tmp.FocusedItem.Tag == "File")
                {
                    Process.Start(tmp.FocusedItem.Text);
                }
                else if ((string)tmp.FocusedItem.Tag == "Folder")
                {
                    ChangeDirectory(tmp.FocusedItem.Text, Section);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ChangeDirectory(string newPath, Section s)
        {
            var tmp = new DirectoryInfo(newPath);
            tmp.GetFiles();
            
            if (s == Section.Left)
                LeftDirectory = tmp;
            else
                RightDirectory = tmp;

            Directory.SetCurrentDirectory(newPath);
            SetUpListView(s);
        }

        public void CreateFolder(string name)
        {
            if (name.Length < 1)
                throw new ArgumentException("Слишком короткое название!");
            else if (Directory.Exists(name))
                throw new ArgumentException("Папка с таким именем уже существует!");

            Directory.CreateDirectory(name);
        }

        public void CreateFile(string name)
        {
            if (name.Length < 1)
                throw new ArgumentException("Слишком короткое название!");
            else if (File.Exists(name))
                throw new ArgumentException("Файл с таким именем уже существует!");

            FileStream fs = new FileStream(name, FileMode.CreateNew, FileAccess.Write, FileShare.Inheritable);
            fs.Close();
        }
        
        public void DeleteFiles()
        {
            var listView = Section == Section.Left ? _leftListView : _rightListView;
            bool samePath = RightDirectory.FullName == LeftDirectory.FullName ? true : false;

            int index;
            foreach (ListViewItem item in listView.SelectedItems)
            {
                if (item.Text == "..")
                    continue;
                else if ((item.Tag as string) == "Folder")
                    Directory.Delete(item.Text, true);
                else if ((item.Tag as string) == "File")
                    File.Delete(item.Text);

                if (samePath)
                {
                    index = _leftListView.Items.IndexOf(item);
                    _leftListView.Items.RemoveAt(index);
                    _rightListView.Items.RemoveAt(index);
                }
                else
                    listView.Items.Remove(item);
            }
        }

        public object GetSelectedItem()
        {
            var listView = Section == Section.Left ? _leftListView : _rightListView;
            var sourcePath = Section == Section.Left ? LeftDirectory.FullName : RightDirectory.FullName;

            if (listView.SelectedItems.Count == 0)
                return null;
            var item = listView.SelectedItems[0];

            if ((item.Tag as string) == "Folder")
                return new DirectoryInfo(sourcePath + "\\" + item.Text);
            else if ((item.Tag as string) == "File")
                return new FileInfo(sourcePath + "\\" + item.Text);

            return null;
        }

        public string[] GetSelectedItemsPath()
        {
            var listView = Section == Section.Left ? _leftListView : _rightListView;
            var sourcePath = Section == Section.Left ? LeftDirectory.FullName : RightDirectory.FullName;
            var path = new List<string>();

            foreach (ListViewItem item in listView.SelectedItems)
            {
                if (item.Text == "..")
                    continue;
                path.Add(sourcePath + "\\" + item.Text);
            }

            return path.ToArray();
        }

        public void CopyFiles()
        {
            _buffer.Type = TransferType.Copy;
            SetFilesToBuffer();
        }

        public void PasteFiles(Section s)
        {
            var listView = s == Section.Left ? _leftListView : _rightListView;
            var sourcePath = s == Section.Left ? LeftDirectory.FullName : RightDirectory.FullName;

            string dir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(sourcePath);

            if (_buffer.Type == TransferType.Copy)
            {
                foreach (var item in _buffer.GetFiles())
                {
                    try { File.Copy(item, Path.GetFileName(item)); }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }

                foreach (var item in _buffer.GetFolders())
                {
                    try { CopyFolder(new DirectoryInfo(item), Directory.GetCurrentDirectory()); }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                    finally { Directory.SetCurrentDirectory(sourcePath); }
                }
            }
            else
            {
                foreach (var item in _buffer.GetFiles())
                {
                    try { File.Move(item, Path.GetFileName(item)); }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }

                foreach (var item in _buffer.GetFolders())
                {
                    try { Directory.Move(item, Path.GetFileName(item)); }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
            }
            Directory.SetCurrentDirectory(dir);
        }

        public void CutFiles()
        {
            _buffer.Type = TransferType.Cut;
            SetFilesToBuffer();
        }

        public void SetFilesToBuffer(string[] path)
        {
            _buffer.Clear();
            _buffer.Type = TransferType.Copy;

            foreach (var item in path)
            {
                if (File.Exists(item))
                    _buffer.AddFile(item);
                else if (Directory.Exists(item))
                    _buffer.AddFolder(item);
            }
        }

        private void SetFilesToBuffer()
        {
            _buffer.Clear();
            var listView = Section == Section.Left ? _leftListView : _rightListView;
            var sourcePath = Section == Section.Left ? LeftDirectory.FullName : RightDirectory.FullName;

            foreach (ListViewItem item in listView.SelectedItems)
            {
                if ((item.Tag as string) == "Folder")
                    _buffer.AddFolder(sourcePath + "\\" + item.Text);
                else if ((item.Tag as string) == "File")
                    _buffer.AddFile(sourcePath + "\\" + item.Text);
            }
        }

        private void CopyFolder(DirectoryInfo from, string to)
        {
            try
            {
                Directory.SetCurrentDirectory(to);
                Directory.CreateDirectory(from.Name);
                Directory.SetCurrentDirectory(from.Name);

                ArrayList current = new ArrayList();
                current.AddRange(from.GetFiles());
                current.AddRange(from.GetDirectories());

                foreach (var i in current)
                {
                    if (i is FileInfo)
                    {
                        (i as FileInfo).CopyTo($"{Directory.GetCurrentDirectory()}\\{(i as FileInfo).Name}");
                    }
                    else
                    {
                        CopyFolder(i as DirectoryInfo, Directory.GetCurrentDirectory());
                        Directory.SetCurrentDirectory("..");
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
