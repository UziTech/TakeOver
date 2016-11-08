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
    public partial class Form2 : Form
    {
        #region Global Variables
        int _i = 0;
        int _j = 0;
        int[,,] _lettersArray;
        #endregion
        public Form2(int[,,] array)
        {
            _lettersArray = array;
            InitializeComponent();
            ShowButton();
        }
        private void buttonNext_Click(object sender, EventArgs e)
        {
            _j++;
            if (_j == 5)
            {
                _j = 0;
                _i++;
                if (_i == 5)
                    _i = 0;
            }
            ShowButton();
        }
        private void buttonPrev_Click(object sender, EventArgs e)
        {
            _j--;
            if (_j == -1)
            {
                _j = 4;
                _i--;
                if (_i == -1)
                    _i = 4;
            }
            ShowButton();
        }
        private void ShowButton()
        {
            label1.Text = "button[" + _i + ", " + _j + "]";
            for (int i = 0; i < 26; i++)
            {
                if (_lettersArray[_i, _j, i] > 0)
                {
                    label[i].Visible = true;
                }
                else
                {
                    label[i].Visible = false;
                }
            }
        }
    }
}
