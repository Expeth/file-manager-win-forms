using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManagerWF
{
    public partial class FileInfoDialogBox : Form
    {
        public string FileName { get => textBox1.Text; }
        private FileInfo _fileInfo;

        public FileInfoDialogBox(FileInfo fileInfo)
        {
            InitializeComponent();
            _fileInfo = fileInfo;

            pictureBox1.BackgroundImage = Icon.ExtractAssociatedIcon(fileInfo.FullName).ToBitmap();
            textBox1.Text = _fileInfo.Name;
            locationLabel.Text = Path.GetDirectoryName(_fileInfo.FullName);
            createdLabel.Text = _fileInfo.CreationTime.ToLongDateString();
            editedLabel.Text = _fileInfo.LastWriteTime.ToLongDateString();
            openedLabel.Text = _fileInfo.LastAccessTime.ToLongDateString();
            typeLabel.Text = Path.GetExtension(fileInfo.FullName);
            sizeLabel.Text = $"{fileInfo.Length / Math.Pow(1024, 2) : 0.00} МБ.";

            if (_fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                checkBox1.CheckState = CheckState.Checked;

            if (_fileInfo.Attributes.HasFlag(FileAttributes.ReadOnly))
                checkBox2.CheckState = CheckState.Checked;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
