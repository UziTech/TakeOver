using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scramble
{
    public partial class save : Form
    {
        public string Letter;
        public save(string s)
        {
            InitializeComponent();
            Letter = s;
            textBox1.Text = s;
            textBox1.SelectAll();
            textBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Letter = textBox1.Text;
            Close();
        }
    }
}
