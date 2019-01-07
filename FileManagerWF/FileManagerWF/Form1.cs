using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace FileManagerWF
{
    public partial class Form1 : Form, IFormData
    {
        private FileManager             _fileManager;
        private DialogBox               _dlgBox;
        private SearchDialogBox         _searchDlgBox;
        private DirectoryInfoDialogBox  _dirInfoDlgBox;
        private FileInfoDialogBox       _fileInfoDlgBox;

        public Form1()
        {
            InitializeComponent();
            _dlgBox = new DialogBox("Введите имя", "Отмена", "Подтвердить");

            listView1.GotFocus += LeftListViewGotFocus;
            listView2.GotFocus += RightListViewGotFocus;

            listView1.ItemActivate += ListViewDoubleClick;
            listView2.ItemActivate += ListViewDoubleClick;
            
            _fileManager = new FileManager(listView1, listView2, imageList1, imageList2, imageList3, imageList4);

            textBox1.Text = _fileManager.LeftDirectory.FullName;
            textBox2.Text = _fileManager.RightDirectory.FullName;

            comboBox1.Items.AddRange(_fileManager.Drives.Disks.ToArray());
            comboBox2.Items.AddRange(_fileManager.Drives.Disks.ToArray());

            comboBox1.SelectedIndex = comboBox2.SelectedIndex = 0;

            comboBox1.SelectedIndexChanged += ComboBox1SelectedValueChanged;
            comboBox2.SelectedIndexChanged += ComboBox2SelectedValueChanged;
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            textBox1.Text = _fileManager.LeftDirectory.FullName;
            textBox2.Text = _fileManager.RightDirectory.FullName;

            label1.Text = $"{_fileManager.Drives.Disks[comboBox1.SelectedIndex].AvailableFreeSpace / Math.Pow(1024, 3): 0.00} ГБ / " +
                          $"{_fileManager.Drives.Disks[comboBox1.SelectedIndex].TotalSize / Math.Pow(1024, 3): 0.00} ГБ";

            label2.Text = $"{_fileManager.Drives.Disks[comboBox2.SelectedIndex].AvailableFreeSpace / Math.Pow(1024, 3): 0.00} ГБ / " +
                          $"{_fileManager.Drives.Disks[comboBox2.SelectedIndex].TotalSize / Math.Pow(1024, 3): 0.00} ГБ";
        }

        private void ListViewDoubleClick(object sender, EventArgs e)
        {
            _fileManager.ItemDoubleClick(sender);
            UpdateLabels();
        }

        private void LeftListViewGotFocus(object sender, EventArgs e)
        {
            _fileManager.Section = Section.Left;
            Directory.SetCurrentDirectory(_fileManager.LeftDirectory.FullName);
        }

        private void RightListViewGotFocus(object sender, EventArgs e)
        {
            _fileManager.Section = Section.Right;
            Directory.SetCurrentDirectory(_fileManager.RightDirectory.FullName);
        }

        private void ChangeViewMode(object sender, EventArgs e)
        {
            _fileManager.ChangeViewMode(((sender as ToolStripMenuItem).Tag as string));
        }

        private void OpenNotepad(object sender, EventArgs e)
        {
            Process.Start("notepad.exe");
        }

        private void RefreshFiles(object sender, EventArgs e)
        {
            if (_fileManager.LeftDirectory.FullName == _fileManager.RightDirectory.FullName)
            {
                _fileManager.SetUpListView(Section.Left);
                _fileManager.SetUpListView(Section.Right);
            }
            else
            {
                _fileManager.SetUpListView(_fileManager.Section);
            }
        }

        private void ComboBox1SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                _fileManager.ChangeDirectory(_fileManager.Drives.Disks[comboBox1.SelectedIndex].Name, Section.Left);
                UpdateLabels();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ComboBox2SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                _fileManager.ChangeDirectory(_fileManager.Drives.Disks[comboBox2.SelectedIndex].Name, Section.Right);
                UpdateLabels();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CreateFolder(object sender, EventArgs e)
        {
            if (!(_dlgBox.ShowDialog() == DialogResult.OK))
                return;

            try
            {
                _fileManager.CreateFolder(_dlgBox.TextBox);
                RefreshFiles(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateFile(object sender, EventArgs e)
        {
            if (!(_dlgBox.ShowDialog() == DialogResult.OK))
                return;

            try
            {
                _fileManager.CreateFile(_dlgBox.TextBox);
                RefreshFiles(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteFiles(object sender, EventArgs e)
        {
            if (!(MessageBox.Show("Вы действительно хотите удалить эти объекты?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes))
                return;

            try
            {
                _fileManager.DeleteFiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CopyFiles(object sender, EventArgs e)
        {
            _fileManager.CopyFiles();
        }

        private void CutFiles(object sender, EventArgs e)
        {
            _fileManager.CutFiles();
        }

        private void PasteFiles(object sender, EventArgs e)
        {
            _fileManager.PasteFiles(_fileManager.Section);
            _fileManager.SetUpListView(Section.Right);
            _fileManager.SetUpListView(Section.Left);
        }

        private void SearchFile(object sender, EventArgs e)
        {
            try
            {
                _searchDlgBox = new SearchDialogBox(this);
                _searchDlgBox.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ChangeDirectory(string newPath)
        {
            _fileManager.ChangeDirectory(newPath, _fileManager.Section);
        }

        private void InfoDialogBox(object sender, EventArgs e)
        {
            var tmp = _fileManager.GetSelectedItem();

            if (tmp is FileInfo)
            {
                _fileInfoDlgBox = new FileInfoDialogBox(tmp as FileInfo);
                _fileInfoDlgBox.Show();

                /*if (!(_fileInfoDlgBox.ShowDialog() == DialogResult.OK))
                    return;*/
            }
            else if (tmp is DirectoryInfo)
            {
                _dirInfoDlgBox = new DirectoryInfoDialogBox(tmp as DirectoryInfo);
                _dirInfoDlgBox.Show();

                /*if (!(_dirInfoDlgBox.ShowDialog() == DialogResult.OK))
                    return;*/
            }
        }

        private void ItemDragEvent(object sender, ItemDragEventArgs e)
        {
            var path = _fileManager.GetSelectedItemsPath();
            (sender as ListView).DoDragDrop(path, DragDropEffects.Copy);
        }

        private void DragEnterEvent(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void DragDropEvent(object sender, DragEventArgs e)
        {
            string[] path;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                path = e.Data.GetData(DataFormats.FileDrop) as String[];              
            }
            else
            {
                path = e.Data.GetData(typeof(string[])) as String[];
            }

            if (!(MessageBox.Show($"Вы действительно хотите скопировать {path.Length} элементов?", "Копирование", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
                return;

            _fileManager.SetFilesToBuffer(path);

            if ((sender as ListView).Name == "listView1")
                _fileManager.PasteFiles(Section.Left);
            else if ((sender as ListView).Name == "listView2")
                _fileManager.PasteFiles(Section.Right);

            _fileManager.SetUpListView(Section.Right);
            _fileManager.SetUpListView(Section.Left);
        }
    }
}
