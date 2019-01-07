using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManagerWF
{
    public partial class DialogBox : Form
    {
        public string TextBox { get => textBox1.Text; }

        public DialogBox(string text, string button1Name, string button2Name)
        {
            InitializeComponent();
            this.Name = text;
            button1.Text = button1Name;
            button2.Text = button2Name;
        }
    }
}
