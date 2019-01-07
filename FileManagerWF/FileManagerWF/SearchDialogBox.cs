using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManagerWF
{
    public partial class SearchDialogBox : Form
    {
        private Searcher _searcher;
        private IFormData _parentForm;

        public SearchDialogBox(IFormData f)
        {
            InitializeComponent();
            button1.Enabled = false;
            _searcher = new Searcher();
            _parentForm = f;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            _searcher.Clear();

            StartSearch();
        }

        private async void StartSearch()
        {
            await Task.Factory.StartNew(()=> {

                try
                {
                    _searcher.Search(new System.IO.DirectoryInfo(textBox1.Text), textBox2.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                if (_searcher.Results.Count == 0)
                {
                    MessageBox.Show("Ничего не найдено!", "Поиск", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                listBox1.Items.AddRange(_searcher.Results.ToArray());
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!(folderBrowserDialog1.ShowDialog() == DialogResult.OK))
                return;

            textBox1.Text = folderBrowserDialog1.SelectedPath;
            button1.Enabled = true;
        }

        private void SearchDialogBox_Load(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            var item = listBox1.SelectedItem as string;

            if (File.Exists(item))
            {
                Process.Start(item);
            }
            else if (Directory.Exists(item))
            {
                try
                {
                    _parentForm.ChangeDirectory(item);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.Close();
            }
        }
    }
}
