using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;
using MODI;

namespace Scramble
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public partial class Form1 : Form
    {
        #region Global Variables
        int[,,] _letterArray = new int[5,5,26];
        Bitmap _desktop;
        Font _qu = new Font("Microsoft Sans Serif", 19);
        Font _norm = new Font("Microsoft Sans Serif", 28);
        bool _done = true;
        bool _start = false;
        bool _E5 = true;
        int _speed = 80;
        RegistryKey _regKey = Registry.CurrentUser;
        private bool _capturing;
        public int handle;
        public bool captured = false;
        public TakeOver to;
        private Cursor _cursorDefault;
        private Cursor _cursorFinder = Cursor.Current;
        private IntPtr _hPreviousWindow;
        #endregion
        public Form1()
        {
            InitializeComponent();
            sc = new ScreenCapture();
            handlePtr = new IntPtr(handle);
            timer2.Interval = _speed;
            _cursorFinder = new Cursor("Finder.cur");
            _cursorDefault = Cursor.Current;
            handle = 0;
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private void Form1_Activated(object sender, System.EventArgs e)
        {
            this.Focus();
        }
        private void button_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 'A' || (e.KeyChar > 'Z' && e.KeyChar < 'a') || e.KeyChar > 'z') && e.KeyChar != '')
            {
                MessageBox.Show("Letters A - Z Only", "Invalid Character");
                return;
            }
            //stopToolStripMenuItem.Text = "Start";
            timer2.Enabled = false;
            _done = true;
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                {
                    if (listBox1.Items.Count == 0 && sender.Equals(this.button[i, j]))
                    {
                        if (button[i, j].Text == "Q" && e.KeyChar.ToString().ToUpper() == "U")
                        {
                            button[i, j].Font = _qu;
                            button[i, j].Text = "Qu";
                            if (j == 4)
                            {
                                if (i == 4)
                                {
                                    button[0, 0].Focus();
                                }
                                else
                                {
                                    button[i + 1, 0].Focus();
                                }
                            }
                            else if (j == 3 && !_E5)
                            {
                                if (i == 3)
                                {
                                    button[0, 0].Focus();
                                }
                                else
                                {
                                    button[i + 1, 0].Focus();
                                }
                            }
                            else
                            {
                                button[i, j + 1].Focus();
                            }
                        }
                        else if (e.KeyChar == '')
                        {
                            if (j == 0)
                            {
                                if (_E5)
                                {
                                    if (i == 0)
                                    {
                                        button[4, 4].Focus();
                                    }
                                    else
                                    {
                                        button[i - 1, 4].Focus();
                                    }
                                }
                                else
                                {
                                    if (i == 0)
                                    {
                                        button[3, 3].Focus();
                                    }
                                    else
                                    {
                                        button[i - 1, 3].Focus();
                                    }
                                }
                            }
                            else
                            {
                                button[i, j - 1].Focus();
                            }
                        }
                        else
                        {
                            button[i, j].Font = _norm;
                            button[i, j].Text = e.KeyChar.ToString().ToUpper();
                            int k = Convert.ToByte(Convert.ToChar(button[i,j].Text)) - 65;
                            if (k >= 0 && k < 26)
                                _letterArray[i, j, k]++;
                            if (j == 4)
                            {
                                if (i == 4)
                                {
                                    button[0, 0].Focus();
                                }
                                else
                                {
                                    button[i + 1, 0].Focus();
                                }
                            }
                            else if (j == 3 && !_E5)
                            {
                                if (i == 3)
                                {
                                    button[0, 0].Focus();
                                }
                                else
                                {
                                    button[i + 1, 0].Focus();
                                }
                            }
                            else
                            {
                                button[i, j + 1].Focus();
                            }
                        }
                        return;
                    }
                }
        }
        private void button_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                if (stopToolStripMenuItem.Text == "Stop" && listBox1.Items.Count == 0)
                {
                    if (button[1, 3].Text == "")
                        CheckListsWT();
                    else if (_E5)
                        CheckLists(7);
                    else
                        CheckLists(6);
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                    for (int j = 0; j < 5; j++)
                    {
                        if (sender.Equals(this.button[i, j]))
                        {
                            if (e.KeyValue == 40)//down
                            {
                                if ((!_E5 && i == 3) || i == 4)
                                    if (j == 0)
                                        listBox1.Focus();
                                    else
                                        button[0, j - 1].Focus();
                                else
                                    if (j == 0)
                                        if (_E5)
                                            button[i, 4].Focus();
                                        else
                                            button[i, 3].Focus();
                                    else
                                        button[i + 1, j - 1].Focus();
                            }
                            else if (e.KeyValue == 38)//up
                            {
                                if ((!_E5 && j == 3) || j == 4)
                                    if (i == 0)
                                        listBox1.Focus();
                                    else
                                        button[i, 0].Focus();
                                else
                                    if (i == 0)
                                        if (_E5)
                                            button[4, j + 1].Focus();
                                        else
                                            button[3, j + 1].Focus();
                                    else
                                        button[i - 1, j + 1].Focus();
                            }
                            else if (e.KeyValue == 37)//left
                            {
                                if (j == 0)
                                    if ((!_E5 && i == 3) || i == 4)
                                        listBox1.Focus();
                                    else
                                        button[i + 1, j].Focus();
                            }
                            else if (e.KeyValue == 39)//Right
                            {
                                if ((!_E5 && j == 3) || j == 4)
                                    if (i == 0)
                                        listBox1.Focus();
                                    else
                                        button[i - 1, j].Focus();
                            }
                            return;
                        }
                    }
            }
        }
        private void button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (sender == button[i, j])
                    {
                        if (button[i, j].Text != "")
                        {
                            int k = Convert.ToByte(Convert.ToChar(button[i, j].Text.Substring(0, 1))) - 65;
                            _letterArray[i, j, k]--;
                        }
                        return;
                    }
                }
            }
        }
        private void CheckLists(int size)
        {
            string[,] grid = new string[size, size];
            for (int i = 1; i < 5; i++)
                for (int j = 1; j < 5; j++)
                {
                    grid[i, j] = button[i - 1, j - 1].Text;
                }
            if (size == 7)
            {
                for (int i = 1; i < 6; i++)
                {
                    grid[i, 5] = button[i - 1, 4].Text;
                    grid[5, i] = button[4, i - 1].Text;
                }
            }
            StreamReader words = new StreamReader("wordList.txt");
            int var;
            bool q1;
            bool q2;
            bool q3;
            bool q4;
            bool q5;
            bool q6;
            bool q7;
            bool q8;
            bool q9;
            bool q10;
            bool q11;
            bool q12;
            bool q13;
            bool q14;
            string word;
            while (!words.EndOfStream)
            {
            found:
                if (!words.EndOfStream)
                    word = words.ReadLine().ToUpper();
                else
                    break;
                var = 0;
                q1 = false;
                q2 = false;
                q3 = false;
                q4 = false;
                q5 = false;
                q6 = false;
                q7 = false;
                q8 = false;
                q9 = false;
                q10 = false;
                q11 = false;
                q12 = false;
                q13 = false;
                q14 = false;
                string letter1 = word.Substring(0, 1);
                for (int row1 = 1; row1 < size - 1; row1++)
                    for (int col1 = 1; col1 < size - 1; col1++)
                    {
                        if (q1)
                        {
                            var--;
                            q1 = false;
                        }
                        if (grid[row1, col1] == letter1 || (grid[row1, col1] == "Qu" && letter1 == "Q" && word.Substring(1, 1) == "U"))
                        {
                            if (grid[row1, col1] == "Qu" && letter1 == "Q" && word.Substring(1, 1) == "U")
                            {
                                var++;
                                q1 = true;
                            }
                            string letter2 = word.Substring(var + 1, 1);
                            for (int row2 = -1; row2 < 2; row2++)
                            {
                                for (int col2 = -1; col2 < 2; col2++)
                                {
                                    if (q2)
                                    {
                                        var--;
                                        q2 = false;
                                    }
                                    if (grid[row1 + row2, col1 + col2] == letter2 && (row2 != 0 || col2 != 0) || (grid[row1 + row2, col1 + col2] == "Qu" && letter2 == "Q" && word.Length > var + 2 && word.Substring(var + 2, 1) == "U"))
                                    {
                                        if (grid[row1 + row2, col1 + col2] == "Qu" && letter2 == "Q" && word.Substring(var + 2, 1) == "U")
                                        {
                                            var++;
                                            q2 = true;
                                        }
                                        if (word.Length > var + 2)
                                        {
                                            string letter3 = word.Substring(var + 2, 1);
                                            for (int row3 = -1; row3 < 2; row3++)
                                            {
                                                for (int col3 = -1; col3 < 2; col3++)
                                                {
                                                    if (q3)
                                                    {
                                                        var--;
                                                        q3 = false;
                                                    }
                                                    if (grid[row1 + row2 + row3, col1 + col2 + col3] == letter3 && (row3 != 0 || col3 != 0) && (row2 + row3 != 0 || col2 + col3 != 0) || (grid[row1 + row2 + row3, col1 + col2 + col3] == "Qu" && letter3 == "Q" && word.Length > var + 3 && word.Substring(var + 3, 1) == "U"))
                                                    {
                                                        if (grid[row1 + row2 + row3, col1 + col2 + col3] == "Qu" && letter3 == "Q" && word.Substring(var + 3, 1) == "U")
                                                        {
                                                            var++;
                                                            q3 = true;
                                                        }
                                                        if (word.Length > var + 3)
                                                        {
                                                            string letter4 = word.Substring(var + 3, 1);
                                                            for (int row4 = -1; row4 < 2; row4++)
                                                            {
                                                                for (int col4 = -1; col4 < 2; col4++)
                                                                {
                                                                    if (q4)
                                                                    {
                                                                        var--;
                                                                        q4 = false;
                                                                    }
                                                                    if (grid[row1 + row2 + row3 + row4, col1 + col2 + col3 + col4] == letter4 && (row4 != 0 || col4 != 0) && (row3 + row4 != 0 || col3 + col4 != 0) && (row2 + row3 + row4 != 0 || col2 + col3 + col4 != 0) || (grid[row1 + row2 + row3 + row4, col1 + col2 + col3 + col4] == "Qu" && letter4 == "Q" && word.Length > var + 4 && word.Substring(var + 4, 1) == "U"))
                                                                    {
                                                                        if (grid[row1 + row2 + row3 + row4, col1 + col2 + col3 + col4] == "Qu" && letter4 == "Q" && word.Substring(var + 4, 1) == "U")
                                                                        {
                                                                            var++;
                                                                            q4 = true;
                                                                        }
                                                                        if (word.Length > var + 4)
                                                                        {
                                                                            string letter5 = word.Substring(var + 4, 1);
                                                                            for (int row5 = -1; row5 < 2; row5++)
                                                                            {
                                                                                for (int col5 = -1; col5 < 2; col5++)
                                                                                {
                                                                                    if (q5)
                                                                                    {
                                                                                        var--;
                                                                                        q5 = false;
                                                                                    }
                                                                                    if (grid[row1 + row2 + row3 + row4 + row5, col1 + col2 + col3 + col4 + col5] == letter5 && (row5 != 0 || col5 != 0) && (row4 + row5 != 0 || col4 + col5 != 0) && (row3 + row4 + row5 != 0 || col3 + col4 + col5 != 0) && (row2 + row3 + row4 + row5 != 0 || col2 + col3 + col4 + col5 != 0) || (grid[row1 + row2 + row3 + row4 + row5, col1 + col2 + col3 + col4 + col5] == "Qu" && letter5 == "Q" && word.Length > var + 5 && word.Substring(var + 5, 1) == "U"))
                                                                                    {
                                                                                        if (grid[row1 + row2 + row3 + row4 + row5, col1 + col2 + col3 + col4 + col5] == "Qu" && letter5 == "Q" && word.Substring(var + 5, 1) == "U")
                                                                                        {
                                                                                            var++;
                                                                                            q5 = true;
                                                                                        }
                                                                                        if (word.Length > var + 5)
                                                                                        {
                                                                                            string letter6 = word.Substring(var + 5, 1);
                                                                                            for (int row6 = -1; row6 < 2; row6++)
                                                                                            {
                                                                                                for (int col6 = -1; col6 < 2; col6++)
                                                                                                {
                                                                                                    if (q6)
                                                                                                    {
                                                                                                        var--;
                                                                                                        q6 = false;
                                                                                                    }
                                                                                                    if (grid[row1 + row2 + row3 + row4 + row5 + row6, col1 + col2 + col3 + col4 + col5 + col6] == letter6 && (row6 != 0 || col6 != 0) && (row5 + row6 != 0 || col5 + col6 != 0) && (row4 + row5 + row6 != 0 || col4 + col5 + col6 != 0) && (row3 + row4 + row5 + row6 != 0 || col3 + col4 + col5 + col6 != 0) && (row2 + row3 + row4 + row5 + row6 != 0 || col2 + col3 + col4 + col5 + col6 != 0) || (grid[row1 + row2 + row3 + row4 + row5 + row6, col1 + col2 + col3 + col4 + col5 + col6] == "Qu" && letter6 == "Q" && word.Length > var + 6 && word.Substring(var + 6, 1) == "U"))
                                                                                                    {
                                                                                                        if (grid[row1 + row2 + row3 + row4 + row5 + row6, col1 + col2 + col3 + col4 + col5 + col6] == "Qu" && letter6 == "Q" && word.Substring(var + 6, 1) == "U")
                                                                                                        {
                                                                                                            var++;
                                                                                                            q6 = true;
                                                                                                        }
                                                                                                        if (word.Length > var + 6)
                                                                                                        {
                                                                                                            string letter7 = word.Substring(var + 6, 1);
                                                                                                            for (int row7 = -1; row7 < 2; row7++)
                                                                                                            {
                                                                                                                for (int col7 = -1; col7 < 2; col7++)
                                                                                                                {
                                                                                                                    if (q7)
                                                                                                                    {
                                                                                                                        var--;
                                                                                                                        q7 = false;
                                                                                                                    }
                                                                                                                    if (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7, col1 + col2 + col3 + col4 + col5 + col6 + col7] == letter7 && (row7 != 0 || col7 != 0) && (row6 + row7 != 0 || col6 + col7 != 0) && (row5 + row6 + row7 != 0 || col5 + col6 + col7 != 0) && (row4 + row5 + row6 + row7 != 0 || col4 + col5 + col6 + col7 != 0) && (row3 + row4 + row5 + row6 + row7 != 0 || col3 + col4 + col5 + col6 + col7 != 0) && (row2 + row3 + row4 + row5 + row6 + row7 != 0 || col2 + col3 + col4 + col5 + col6 + col7 != 0) || (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7, col1 + col2 + col3 + col4 + col5 + col6 + col7] == "Qu" && letter7 == "Q" && word.Length > var + 7 && word.Substring(var + 7, 1) == "U"))
                                                                                                                    {
                                                                                                                        if (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7, col1 + col2 + col3 + col4 + col5 + col6 + col7] == "Qu" && letter7 == "Q" && word.Substring(var + 7, 1) == "U")
                                                                                                                        {
                                                                                                                            var++;
                                                                                                                            q7 = true;
                                                                                                                        }
                                                                                                                        if (word.Length > var + 7)
                                                                                                                        {
                                                                                                                            string letter8 = word.Substring(var + 7, 1);
                                                                                                                            for (int row8 = -1; row8 < 2; row8++)
                                                                                                                            {
                                                                                                                                for (int col8 = -1; col8 < 2; col8++)
                                                                                                                                {
                                                                                                                                    if (q8)
                                                                                                                                    {
                                                                                                                                        var--;
                                                                                                                                        q8 = false;
                                                                                                                                    }
                                                                                                                                    if (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8] == letter8 && (row8 != 0 || col8 != 0) && (row7 + row8 != 0 || col7 + col8 != 0) && (row6 + row7 + row8 != 0 || col6 + col7 + col8 != 0) && (row5 + row6 + row7 + row8 != 0 || col5 + col6 + col7 + col8 != 0) && (row4 + row5 + row6 + row7 + row8 != 0 || col4 + col5 + col6 + col7 + col8 != 0) && (row3 + row4 + row5 + row6 + row7 + row8 != 0 || col3 + col4 + col5 + col6 + col7 + col8 != 0) && (row2 + row3 + row4 + row5 + row6 + row7 + row8 != 0 || col2 + col3 + col4 + col5 + col6 + col7 + col8 != 0) || (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8] == "Qu" && letter8 == "Q" && word.Length > var + 8 && word.Substring(var + 8, 1) == "U"))
                                                                                                                                    {
                                                                                                                                        if (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8] == "Qu" && letter8 == "Q" && word.Substring(var + 8, 1) == "U")
                                                                                                                                        {
                                                                                                                                            var++;
                                                                                                                                            q8 = true;
                                                                                                                                        }
                                                                                                                                        if (word.Length > var + 8)
                                                                                                                                        {
                                                                                                                                            string letter9 = word.Substring(var + 8, 1);
                                                                                                                                            for (int row9 = -1; row9 < 2; row9++)
                                                                                                                                            {
                                                                                                                                                for (int col9 = -1; col9 < 2; col9++)
                                                                                                                                                {
                                                                                                                                                    if (q9)
                                                                                                                                                    {
                                                                                                                                                        var--;
                                                                                                                                                        q9 = false;
                                                                                                                                                    }
                                                                                                                                                    if (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9] == letter9 && (row9 != 0 || col9 != 0) && (row8 + row9 != 0 || col8 + col9 != 0) && (row7 + row8 + row9 != 0 || col7 + col8 + col9 != 0) && (row6 + row7 + row8 + row9 != 0 || col6 + col7 + col8 + col9 != 0) && (row5 + row6 + row7 + row8 + row9 != 0 || col5 + col6 + col7 + col8 + col9 != 0) && (row4 + row5 + row6 + row7 + row8 + row9 != 0 || col4 + col5 + col6 + col7 + col8 + col9 != 0) && (row3 + row4 + row5 + row6 + row7 + row8 + row9 != 0 || col3 + col4 + col5 + col6 + col7 + col8 + col9 != 0) && (row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 != 0 || col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 != 0) || (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9] == "Qu" && letter9 == "Q" && word.Length > var + 9 && word.Substring(var + 9, 1) == "U"))
                                                                                                                                                    {
                                                                                                                                                        if (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9] == "Qu" && letter9 == "Q" && word.Substring(var + 9, 1) == "U")
                                                                                                                                                        {
                                                                                                                                                            var++;
                                                                                                                                                            q9 = true;
                                                                                                                                                        }
                                                                                                                                                        if (word.Length > var + 9)
                                                                                                                                                        {
                                                                                                                                                            string letter10 = word.Substring(var + 9, 1);
                                                                                                                                                            for (int row10 = -1; row10 < 2; row10++)
                                                                                                                                                            {
                                                                                                                                                                for (int col10 = -1; col10 < 2; col10++)
                                                                                                                                                                {
                                                                                                                                                                    if (q10)
                                                                                                                                                                    {
                                                                                                                                                                        var--;
                                                                                                                                                                        q10 = false;
                                                                                                                                                                    }
                                                                                                                                                                    if (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10] == letter10 && (row10 != 0 || col10 != 0) && (row9 + row10 != 0 || col9 + col10 != 0) && (row8 + row9 + row10 != 0 || col8 + col9 + col10 != 0) && (row7 + row8 + row9 + row10 != 0 || col7 + col8 + col9 + col10 != 0) && (row6 + row7 + row8 + row9 + row10 != 0 || col6 + col7 + col8 + col9 + col10 != 0) && (row5 + row6 + row7 + row8 + row9 + row10 != 0 || col5 + col6 + col7 + col8 + col9 + col10 != 0) && (row4 + row5 + row6 + row7 + row8 + row9 + row10 != 0 || col4 + col5 + col6 + col7 + col8 + col9 + col10 != 0) && (row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 != 0 || col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 != 0) && (row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 != 0 || col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 != 0) || (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10] == "Qu" && letter10 == "Q" && word.Length > var + 10 && word.Substring(var + 10, 1) == "U"))
                                                                                                                                                                    {
                                                                                                                                                                        if (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10] == "Qu" && letter10 == "Q" && word.Substring(var + 10, 1) == "U")
                                                                                                                                                                        {
                                                                                                                                                                            var++;
                                                                                                                                                                            q10 = true;
                                                                                                                                                                        }
                                                                                                                                                                        if (word.Length > var + 10)
                                                                                                                                                                        {
                                                                                                                                                                            string letter11 = word.Substring(var + 10, 1);
                                                                                                                                                                            for (int row11 = -1; row11 < 2; row11++)
                                                                                                                                                                            {
                                                                                                                                                                                for (int col11 = -1; col11 < 2; col11++)
                                                                                                                                                                                {
                                                                                                                                                                                    if (q11)
                                                                                                                                                                                    {
                                                                                                                                                                                        var--;
                                                                                                                                                                                        q11 = false;
                                                                                                                                                                                    }
                                                                                                                                                                                    if (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11] == letter11 && (row11 != 0 || col11 != 0) && (row10 + row11 != 0 || col10 + col11 != 0) && (row9 + row10 + row11 != 0 || col9 + col10 + col11 != 0) && (row8 + row9 + row10 + row11 != 0 || col8 + col9 + col10 + col11 != 0) && (row7 + row8 + row9 + row10 + row11 != 0 || col7 + col8 + col9 + col10 + col11 != 0) && (row6 + row7 + row8 + row9 + row10 + row11 != 0 || col6 + col7 + col8 + col9 + col10 + col11 != 0) && (row5 + row6 + row7 + row8 + row9 + row10 + row11 != 0 || col5 + col6 + col7 + col8 + col9 + col10 + col11 != 0) && (row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 != 0 || col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 != 0) && (row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 != 0 || col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 != 0) && (row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 != 0 || col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 != 0) || (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11] == "Qu" && letter11 == "Q" && word.Length > var + 11 && word.Substring(var + 11, 1) == "U"))
                                                                                                                                                                                    {
                                                                                                                                                                                        if (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11] == "Qu" && letter11 == "Q" && word.Substring(var + 11, 1) == "U")
                                                                                                                                                                                        {
                                                                                                                                                                                            var++;
                                                                                                                                                                                            q11 = true;
                                                                                                                                                                                        }
                                                                                                                                                                                        if (word.Length > var + 11)
                                                                                                                                                                                        {
                                                                                                                                                                                            string letter12 = word.Substring(var + 11, 1);
                                                                                                                                                                                            for (int row12 = -1; row12 < 2; row12++)
                                                                                                                                                                                            {
                                                                                                                                                                                                for (int col12 = -1; col12 < 2; col12++)
                                                                                                                                                                                                {
                                                                                                                                                                                                    if (q12)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        var--;
                                                                                                                                                                                                        q12 = false;
                                                                                                                                                                                                    }
                                                                                                                                                                                                    if (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12] == letter12 && (row12 != 0 || col12 != 0) && (row11 + row12 != 0 || col11 + col12 != 0) && (row10 + row11 + row12 != 0 || col10 + col11 + col12 != 0) && (row9 + row10 + row11 + row12 != 0 || col9 + col10 + col11 + col12 != 0) && (row8 + row9 + row10 + row11 + row12 != 0 || col8 + col9 + col10 + col11 + col12 != 0) && (row7 + row8 + row9 + row10 + row11 + row12 != 0 || col7 + col8 + col9 + col10 + col11 + col12 != 0) && (row6 + row7 + row8 + row9 + row10 + row11 + row12 != 0 || col6 + col7 + col8 + col9 + col10 + col11 + col12 != 0) && (row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 != 0 || col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 != 0) && (row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 != 0 || col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 != 0) && (row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 != 0 || col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 != 0) && (row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 != 0 || col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 != 0) || (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12] == "Qu" && letter12 == "Q" && word.Length > var + 12 && word.Substring(var + 12, 1) == "U"))
                                                                                                                                                                                                    {
                                                                                                                                                                                                        if (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12] == "Qu" && letter12 == "Q" && word.Substring(var + 12, 1) == "U")
                                                                                                                                                                                                        {
                                                                                                                                                                                                            var++;
                                                                                                                                                                                                            q12 = true;
                                                                                                                                                                                                        }
                                                                                                                                                                                                        if (word.Length > var + 12)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            string letter13 = word.Substring(var + 12, 1);
                                                                                                                                                                                                            for (int row13 = -1; row13 < 2; row13++)
                                                                                                                                                                                                            {
                                                                                                                                                                                                                for (int col13 = -1; col13 < 2; col13++)
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    if (q13)
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        var--;
                                                                                                                                                                                                                        q13 = false;
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                    if (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13] == letter13 && (row13 != 0 || col13 != 0) && (row12 + row13 != 0 || col12 + col13 != 0) && (row11 + row12 + row13 != 0 || col11 + col12 + col13 != 0) && (row10 + row11 + row12 + row13 != 0 || col10 + col11 + col12 + col13 != 0) && (row9 + row10 + row11 + row12 + row13 != 0 || col9 + col10 + col11 + col12 + col13 != 0) && (row8 + row9 + row10 + row11 + row12 + row13 != 0 || col8 + col9 + col10 + col11 + col12 + col13 != 0) && (row7 + row8 + row9 + row10 + row11 + row12 + row13 != 0 || col7 + col8 + col9 + col10 + col11 + col12 + col13 != 0) && (row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13 != 0 || col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13 != 0) && (row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13 != 0 || col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13 != 0) && (row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13 != 0 || col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13 != 0) && (row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13 != 0 || col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13 != 0) && (row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13 != 0 || col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13 != 0) || (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13] == "Qu" && letter13 == "Q" && word.Length > var + 13 && word.Substring(var + 13, 1) == "U"))
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        if (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13] == "Qu" && letter13 == "Q" && word.Substring(var + 13, 1) == "U")
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            var++;
                                                                                                                                                                                                                            q13 = true;
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                        if (word.Length > var + 13)
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            string letter14 = word.Substring(var + 13, 1);
                                                                                                                                                                                                                            for (int row14 = -1; row14 < 2; row14++)
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                for (int col14 = -1; col14 < 2; col14++)
                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                    if (q14)
                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                        var--;
                                                                                                                                                                                                                                        q14 = false;
                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                    if (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13 + row14, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13 + col14] == letter14 && (row14 != 0 || col14 != 0) && (row13 + row14 != 0 || col13 + col14 != 0) && (row12 + row13 + row14 != 0 || col12 + col13 + col14 != 0) && (row11 + row12 + row13 + row14 != 0 || col11 + col12 + col13 + col14 != 0) && (row10 + row11 + row12 + row13 + row14 != 0 || col10 + col11 + col12 + col13 + col14 != 0) && (row9 + row10 + row11 + row12 + row13 + row14 != 0 || col9 + col10 + col11 + col12 + col13 + col14 != 0) && (row8 + row9 + row10 + row11 + row12 + row13 + row14 != 0 || col8 + col9 + col10 + col11 + col12 + col13 + col14 != 0) && (row7 + row8 + row9 + row10 + row11 + row12 + row13 + row14 != 0 || col7 + col8 + col9 + col10 + col11 + col12 + col13 + col14 != 0) && (row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13 + row14 != 0 || col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13 + col14 != 0) && (row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13 + row14 != 0 || col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13 + col14 != 0) && (row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13 + row14 != 0 || col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13 + col14 != 0) && (row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13 + row14 != 0 || col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13 + col14 != 0) && (row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13 + row14 != 0 || col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13 + col14 != 0) || (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13 + row14, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13 + col14] == "Qu" && word.Length > var + 14 && letter14 == "Q" && word.Substring(var + 14, 1) == "U"))
                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                        if (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13 + row14, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13 + col14] == "Qu" && letter14 == "Q" && word.Substring(var + 14, 1) == "U")
                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                            var++;
                                                                                                                                                                                                                                            q14 = true;
                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                        if (word.Length > var + 14)
                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                            string letter15 = word.Substring(var + 14, 1);
                                                                                                                                                                                                                                            for (int row15 = -1; row15 < 2; row15++)
                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                for (int col15 = -1; col15 < 2; col15++)
                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                    if (grid[row1 + row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13 + row14 + row15, col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13 + col14 + col15] == letter15 && (row15 != 0 || col15 != 0) && (row14 + row15 != 0 || col14 + col15 != 0) && (row13 + row14 + row15 != 0 || col13 + col14 + col15 != 0) && (row12 + row13 + row14 + row15 != 0 || col12 + col13 + col14 + col15 != 0) && (row11 + row12 + row13 + row14 + row15 != 0 || col11 + col12 + col13 + col14 + col15 != 0) && (row10 + row11 + row12 + row13 + row14 + row15 != 0 || col10 + col11 + col12 + col13 + col14 + col15 != 0) && (row9 + row10 + row11 + row12 + row13 + row14 + row15 != 0 || col9 + col10 + col11 + col12 + col13 + col14 + col15 != 0) && (row8 + row9 + row10 + row11 + row12 + row13 + row14 + row15 != 0 || col8 + col9 + col10 + col11 + col12 + col13 + col14 + col15 != 0) && (row7 + row8 + row9 + row10 + row11 + row12 + row13 + row14 + row15 != 0 || col7 + col8 + col9 + col10 + col11 + col12 + col13 + col14 + col15 != 0) && (row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13 + row14 + row15 != 0 || col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13 + col14 + col15 != 0) && (row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13 + row14 + row15 != 0 || col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13 + col14 + col15 != 0) && (row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13 + row14 + row15 != 0 || col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13 + col14 + col15 != 0) && (row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13 + row14 + row15 != 0 || col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13 + col14 + col15 != 0) && (row2 + row3 + row4 + row5 + row6 + row7 + row8 + row9 + row10 + row11 + row12 + row13 + row14 + row15 != 0 || col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12 + col13 + col14 + col15 != 0))
                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                        listBox1.Items.Add(word);
                                                                                                                                                                                                                                                        listBox1.Update();
                                                                                                                                                                                                                                                        goto found;
                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                            listBox1.Items.Add(word);
                                                                                                                                                                                                                                            listBox1.Update();
                                                                                                                                                                                                                                            goto found;
                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                }
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                        else
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            listBox1.Items.Add(word);
                                                                                                                                                                                                                            listBox1.Update();
                                                                                                                                                                                                                            goto found;
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                }
                                                                                                                                                                                                            }
                                                                                                                                                                                                        }
                                                                                                                                                                                                        else
                                                                                                                                                                                                        {
                                                                                                                                                                                                            listBox1.Items.Add(word);
                                                                                                                                                                                                            listBox1.Update();
                                                                                                                                                                                                            goto found;
                                                                                                                                                                                                        }
                                                                                                                                                                                                    }
                                                                                                                                                                                                }
                                                                                                                                                                                            }
                                                                                                                                                                                        }
                                                                                                                                                                                        else
                                                                                                                                                                                        {
                                                                                                                                                                                            listBox1.Items.Add(word);
                                                                                                                                                                                            listBox1.Update();
                                                                                                                                                                                            goto found;
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                }
                                                                                                                                                                            }
                                                                                                                                                                        }
                                                                                                                                                                        else
                                                                                                                                                                        {
                                                                                                                                                                            listBox1.Items.Add(word);
                                                                                                                                                                            listBox1.Update();
                                                                                                                                                                            goto found;
                                                                                                                                                                        }
                                                                                                                                                                    }
                                                                                                                                                                }
                                                                                                                                                            }
                                                                                                                                                        }
                                                                                                                                                        else
                                                                                                                                                        {
                                                                                                                                                            listBox1.Items.Add(word);
                                                                                                                                                            listBox1.Update();
                                                                                                                                                            goto found;
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                        }////
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            listBox1.Items.Add(word);
                                                                                                                                            listBox1.Update();
                                                                                                                                            goto found;
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                }
                                                                                                                            }
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            listBox1.Items.Add(word);
                                                                                                                            listBox1.Update();
                                                                                                                            goto found;
                                                                                                                        }
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            listBox1.Items.Add(word);
                                                                                                            listBox1.Update();
                                                                                                            goto found;
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            listBox1.Items.Add(word);
                                                                                            listBox1.Update();
                                                                                            goto found;
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            listBox1.Items.Add(word);
                                                                            listBox1.Update();
                                                                            goto found;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            listBox1.Items.Add(word);
                                                            listBox1.Update();
                                                            goto found;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            listBox1.Items.Add(word);
                                            listBox1.Update();
                                            goto found;
                                        }
                                    }
                                }
                            }
                        }
                    }
            }
            if (stopToolStripMenuItem.Text == "Stop")
            {
                _done = false;
                timer2.Enabled = true;
            }
            words.Close();
        }
        private void CheckListsWT()
        {
            string[] grid = new string[7];
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 5; j++)
                {
                    if (i * 5 + j > 6)
                    {
                        break;
                    }
                    else
                    {
                        grid[i * 5 + j] = button[i, j].Text;
                    }
                }
            StreamReader words = new StreamReader("wordList.txt");
            int var;
            bool q1;
            bool q2;
            bool q3;
            bool q4;
            bool q5;
            bool q6;
            bool q7;
            string word;
            while (!words.EndOfStream)
            {
            found:
                if (!words.EndOfStream)
                    word = words.ReadLine().ToUpper();
                else
                    break;
                var = 0;
                q1 = false;
                q2 = false;
                q3 = false;
                q4 = false;
                q5 = false;
                q6 = false;
                q7 = false;
                string letter1 = word.Substring(0, 1);
                for (int let1 = 0; let1 < 7; let1++)
                {
                    if (q1)
                    {
                        var--;
                        q1 = false;
                    }
                    if (grid[let1] == letter1 || (grid[let1] == "Qu" && letter1 == "Q" && word.Substring(1, 1) == "U"))
                    {
                        if (grid[let1] == "Qu" && letter1 == "Q" && word.Substring(1, 1) == "U")
                        {
                            var++;
                            q1 = true;
                        }
                        string letter2 = word.Substring(var + 1, 1);
                        for (int let2 = 0; let2 < 7; let2++)
                        {
                            if (q2)
                            {
                                var--;
                                q2 = false;
                            }
                            if (grid[let2] == letter2 && let2 != let1 || (grid[let2] == "Qu" && letter2 == "Q" && word.Length > var + 2 && word.Substring(var + 2, 1) == "U"))
                            {
                                if (grid[let2] == "Qu" && letter2 == "Q" && word.Substring(var + 2, 1) == "U")
                                {
                                    var++;
                                    q2 = true;
                                }
                                if (word.Length > var + 2)
                                {
                                    string letter3 = word.Substring(var + 2, 1);
                                    for (int let3 = 0; let3 < 7; let3++)
                                    {
                                        if (q3)
                                        {
                                            var--;
                                            q3 = false;
                                        }
                                        if (grid[let3] == letter3 && let3 != let1 && let3 != let2 || (grid[let3] == "Qu" && letter3 == "Q" && word.Length > var + 3 && word.Substring(var + 3, 1) == "U"))
                                        {
                                            if (grid[let3] == "Qu" && letter3 == "Q" && word.Substring(var + 3, 1) == "U")
                                            {
                                                var++;
                                                q3 = true;
                                            }
                                            if (word.Length > var + 3)
                                            {
                                                string letter4 = word.Substring(var + 3, 1);
                                                for (int let4 = 0; let4 < 7; let4++)
                                                {
                                                    if (q4)
                                                    {
                                                        var--;
                                                        q4 = false;
                                                    }
                                                    if (grid[let4] == letter4 && let4 != let1 && let4 != let2 && let4 != let3 || (grid[let4] == "Qu" && letter4 == "Q" && word.Length > var + 4 && word.Substring(var + 4, 1) == "U"))
                                                    {
                                                        if (grid[let4] == "Qu" && letter4 == "Q" && word.Substring(var + 4, 1) == "U")
                                                        {
                                                            var++;
                                                            q4 = true;
                                                        }
                                                        if (word.Length > var + 4)
                                                        {
                                                            string letter5 = word.Substring(var + 4, 1);
                                                            for (int let5 = 0; let5 < 7; let5++)
                                                            {
                                                                if (q5)
                                                                {
                                                                    var--;
                                                                    q5 = false;
                                                                }
                                                                if (grid[let5] == letter5 && let5 != let1 && let5 != let2 && let5 != let3 && let5 != let4 || (grid[let5] == "Qu" && letter5 == "Q" && word.Length > var + 5 && word.Substring(var + 5, 1) == "U"))
                                                                {
                                                                    if (grid[let5] == "Qu" && letter5 == "Q" && word.Substring(var + 5, 1) == "U")
                                                                    {
                                                                        var++;
                                                                        q5 = true;
                                                                    }
                                                                    if (word.Length > var + 5)
                                                                    {
                                                                        string letter6 = word.Substring(var + 5, 1);
                                                                        for (int let6 = 0; let6 < 7; let6++)
                                                                        {
                                                                            if (q6)
                                                                            {
                                                                                var--;
                                                                                q6 = false;
                                                                            }
                                                                            if (grid[let6] == letter6 && let6 != let1 && let6 != let2 && let6 != let3 && let6 != let4 && let6 != let5 || (grid[let6] == "Qu" && letter6 == "Q" && word.Length > var + 6 && word.Substring(var + 6, 1) == "U"))
                                                                            {
                                                                                if (grid[let6] == "Qu" && letter6 == "Q" && word.Substring(var + 6, 1) == "U")
                                                                                {
                                                                                    var++;
                                                                                    q6 = true;
                                                                                }
                                                                                if (word.Length > var + 6)
                                                                                {
                                                                                    string letter7 = word.Substring(var + 6, 1);
                                                                                    for (int let7 = 0; let7 < 7; let7++)
                                                                                    {
                                                                                        if (q7)
                                                                                        {
                                                                                            var--;
                                                                                            q7 = false;
                                                                                        }
                                                                                        if (grid[let7] == letter7 && let7 != let1 && let7 != let2 && let7 != let3 && let7 != let4 && let7 != let5 && let7 != let6 || (grid[let7] == "Qu" && letter7 == "Q" && word.Length > var + 7 && word.Substring(var + 7, 1) == "U"))
                                                                                        {
                                                                                            if (grid[let7] == "Qu" && letter7 == "Q" && word.Substring(var + 7, 1) == "U")
                                                                                            {
                                                                                                var++;
                                                                                                q7 = true;
                                                                                            }
                                                                                            if (word.Length > var + 7)
                                                                                            {
                                                                                                string letter8 = word.Substring(var + 7, 1);
                                                                                                for (int let8 = 0; let8 < 7; let8++)
                                                                                                {
                                                                                                    if (grid[let8] == letter8 && let8 != let1 && let8 != let2 && let8 != let3 && let8 != let4 && let8 != let5 && let8 != let6 && let8 != let7)
                                                                                                    {
                                                                                                        listBox1.Items.Add(word);
                                                                                                        listBox1.Update();
                                                                                                        goto found;

                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                listBox1.Items.Add(word);
                                                                                                listBox1.Update();
                                                                                                goto found;
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    listBox1.Items.Add(word);
                                                                                    listBox1.Update();
                                                                                    goto found;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        listBox1.Items.Add(word);
                                                                        listBox1.Update();
                                                                        goto found;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            listBox1.Items.Add(word);
                                                            listBox1.Update();
                                                            goto found;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                listBox1.Items.Add(word);
                                                listBox1.Update();
                                                goto found;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    listBox1.Items.Add(word);
                                    listBox1.Update();
                                    goto found;
                                }
                            }
                        }
                    }
                }
            }
            if (stopToolStripMenuItem.Text == "Stop")
            {
                _done = false;
                timer2.Enabled = true;
            }
            words.Close();
        }
        private void x4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                button[i, 4].Enabled = false;
                button[4, i].Enabled = false;
                button[i, 4].Text = "";
                button[4, i].Text = "";
            }
            _E5 = false;
        }
        private void x5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                button[i, 4].Enabled = true;
                button[4, i].Enabled = true;
            }
            _E5 = true;
        }
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    button[i, j].Text = "";
                }
            }
            stopToolStripMenuItem.Text = "Start";
            _start = false;
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SendKeys.Send("{Enter}");
            if (!_done)
            {
                SendKeys.SendWait(listBox1.SelectedItem.ToString());
                timer2.Enabled = true;
            }
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            if (listBox1.SelectedIndex + 1 < listBox1.Items.Count)
            {
                if (!_done && stopToolStripMenuItem.Text == "Stop")
                {
                    if (_start)
                    {
                        SendKeys.SendWait(listBox1.SelectedItem.ToString());
                        _start = false;
                    }
                    listBox1.SetSelected(listBox1.SelectedIndex + 1, true);

                }
            }
            else
            {
                if (listBox1.Items.Count > 0)
                {
                    _done = true;
                    listBox1.SetSelected(0, true);
                    stopToolStripMenuItem.Text = "Start";
                }
            }
        }
        private void stopToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && listBox1.Items.Count == 0 && autoToolStripMenuItem.Text == "Automatic" && stopToolStripMenuItem.Text == "Start")
                this.CaptureMouse(true);
            else if(stopToolStripMenuItem.Text == "Stop")
            {
                stopToolStripMenuItem.Text = "Start";
                timer2.Enabled = false;
                _done = true;
            }
            else if (listBox1.Items.Count != 0)
            {
                stopToolStripMenuItem.Text = "Stop";
                System.Threading.Thread.Sleep(2000);
                timer2.Enabled = true;
                _done = false;
            }
            else if (autoToolStripMenuItem.Text == "Manual")
            {
                stopToolStripMenuItem.Text = "Stop";

            }
        }
        private void stopToolStripMenuItem_MouseUp()
        {
            if (stopToolStripMenuItem.Text == "Start")
            {
                stopToolStripMenuItem.Text = "Stop";
                if (listBox1.Items.Count > 0)
                {
                    System.Threading.Thread.Sleep(2000);
                    _start = true;
                    _done = false;
                    timer2.Enabled = true;
                }
                else if (autoToolStripMenuItem.Text == "Automatic")
                {
                    getLetters();
                    //getGrid();
                }
            }
        }
        private void autoToolStripMenuItem_MouseDown(object sender, EventArgs e)
        {
            if (autoToolStripMenuItem.Text == "Automatic")
                autoToolStripMenuItem.Text = "Manual";
            else
                autoToolStripMenuItem.Text = "Automatic";
        }
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.ShowDialog();
        }
        private void listBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            button[0, 0].Focus();
        }
        private void speedToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _speed--;
                if (_speed < 1)
                {
                    _speed = 1;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                _speed++;
            }
            speedToolStripMenuItem.Text = "Speed = " + _speed;
            timer2.Interval = _speed;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _regKey.SetValue("Speed", _speed);
        }
        private void getLetters()
        {
            Bitmap img = null;
            int spacingX, spacingY;
            if (_E5)
            {
                spacingX = 50;
                spacingY = 50;
            }
            else
            {
                spacingX = 62;
                spacingY = 62;
            }
            for (int r = 0; r < ((_E5) ? 5 : 4); r++)
            {
                for (int c = 0; c < ((_E5) ? 5 : 4); c++)
                {
                    img = _desktop.Clone(new Rectangle(8 + spacingX * c, 41 + spacingY * r, spacingX, spacingY), System.Drawing.Imaging.PixelFormat.DontCare);
                    //img = _desktop.Clone(new Rectangle(2, 41, 253, 247), System.Drawing.Imaging.PixelFormat.DontCare);
                    for (int x = 0; x < img.Width; x++)
                    {
                        for (int y = 0; y < img.Height; y++)
                        {
                            if (img.GetPixel(x, y).R < 50 && img.GetPixel(x, y).R != 0)
                                img.SetPixel(x, y, Color.Black);
                            else if (img.GetPixel(x, y).R >= 50)
                                img.SetPixel(x, y, Color.White);
                        }
                    }
                    button[r, c].Text = GetChar(img, r, c);
                    if (button[r, c].Text == "Qu")
                    {
                        button[r, c].Font = _qu;
                    }
                    else
                    {
                        button[r, c].Font = _norm;
                    }
                }
            }
            SortLetters();
        }
        private void SortLetters()
        {

            StreamReader fileR = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Letters\\letters.txt");
            string[] sa = fileR.ReadToEnd().Split('\n');
            //vv sort function vv
            string temp;
            for (int i = 0; i < sa.Length - 1; i++)
            {
                temp = sa[i];
                for(int j = i - 1; j > -2; j--)
                {
                    if (j > -1 && temp.ToCharArray(0, 1)[0] < sa[j].ToCharArray(0, 1)[0])
                    {
                        sa[j + 1] = sa[j];
                    }
                    else
                    {
                        sa[j + 1] = temp;
                        break;
                    }
                }
            }
            //^^ sort function ^^
            fileR.Close();
            StreamWriter fileW = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Letters\\letters.txt", false);
            foreach (string s in sa)
            {
                if (s != "")
                    fileW.WriteLine(s.Split('\r')[0]);
            }
            fileW.Close();
        }
        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2(_letterArray);
            f.Show();
        }
        /// <summary>
        /// Processes window messages sent to the Spy Window
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                /*
                 * stop capturing events as soon as the user releases the left mouse button
                 * */
                case (int)Win32.WindowMessages.WM_LBUTTONUP:
                    this.CaptureMouse(false);
                    if (this.handle != 0)
                    {
                        this.captured = true;
                        to = new TakeOver(this.handle);
                    }
                    break;
                /*
                 * handle all the mouse movements
                 * */
                case (int)Win32.WindowMessages.WM_MOUSEMOVE:
                    this.HandleMouseMovements();
                    break;
            };

            base.WndProc(ref m);
        }
        /// <summary>
        /// Captures or releases the mouse
        /// </summary>
        /// <param name="captured"></param>
        private void CaptureMouse(bool captured)
        {
            // if we're supposed to capture the window
            if (captured)
            {
                // capture the mouse movements and send them to ourself
                Win32.SetCapture(this.Handle);

                // set the mouse cursor to our finder cursor
                Cursor.Current = _cursorFinder;

                // change the image to the finder gone image
            }
            // otherwise we're supposed to release the mouse capture
            else
            {
                // so release it
                Win32.ReleaseCapture();

                // put the default cursor back
                Cursor.Current = _cursorDefault;
                _desktop = sc.CaptureWindow(new IntPtr(handle));
                // change the image back to the finder at home image
                int wi = _desktop.Width;
                int he = _desktop.Height;
                if (wi == 634 && (he == 650 || he == 800))
                {
                    stopToolStripMenuItem_MouseUp();
                }
                else
                    MessageBox.Show("Not the right window");
                // and finally refresh any window that we were highlighting
                if (_hPreviousWindow != IntPtr.Zero)
                {
                    WindowHighlighter.Refresh(_hPreviousWindow);
                    _hPreviousWindow = IntPtr.Zero;
                }
            }

            // save our capturing state
            _capturing = captured;
        }
        /// <summary>
        /// Handles all mouse move messages sent to the Spy Window
        /// </summary>
        private void HandleMouseMovements()
        {
            // if we're not capturing, then bail out
            if (!_capturing)
                return;

            // capture the window under the cursor's position
            IntPtr hWnd = Win32.WindowFromPoint(Cursor.Position);

            // if the window we're over, is not the same as the one before, and we had one before, refresh it
            if (_hPreviousWindow != IntPtr.Zero && _hPreviousWindow != hWnd)
                WindowHighlighter.Refresh(_hPreviousWindow);

            // if we didn't find a window.. that's pretty hard to imagine. lol
            if (hWnd == IntPtr.Zero)
            {

            }
            else
            {
                // save the window we're over
                _hPreviousWindow = hWnd;

                this.handle = hWnd.ToInt32();
                // highlight the window
                WindowHighlighter.Highlight(hWnd);
            }
        }
        private string GetChar(Bitmap pic, int row, int col)
        {
            Point topLeft = new Point(0, 0), topRight = new Point(0, 0), bottomLeft = new Point(0, 0), bottomRight = new Point(0, 0);
            int blackPixels = 0;
            for (int h = 0; h < pic.Height; h++)
            {
                for (int w = 0; w < pic.Width; w++)
                {
                    if (pic.GetPixel(w, h).R < 100)
                    {
                        topLeft.X = w;
                        topLeft.Y = h;
                        break;
                    }
                }
                if (!topLeft.Equals(new Point(0, 0)))
                    break;
            } 
            for (int h = 0; h < pic.Height; h++)
            {
                for (int w = pic.Width - 1; w >= 0; w--)
                {
                    if (pic.GetPixel(w, h).R < 100)
                    {
                        topRight.X = w;
                        topRight.Y = h;
                        break;
                    }
                }
                if (!topRight.Equals(new Point(0, 0)))
                    break;
            } 
            for (int h = pic.Height - 1; h >= 0; h--)
            {
                for (int w = 0; w < pic.Width; w++)
                {
                    if (pic.GetPixel(w, h).R < 100)
                    {
                        bottomLeft.X = w;
                        bottomLeft.Y = h;
                        break;
                    }
                }
                if (!bottomLeft.Equals(new Point(0, 0)))
                    break;
            }
            for (int h = pic.Height - 1; h >= 0; h--)
            {
                for (int w = pic.Width - 1; w >= 0; w--)
                {
                    if (pic.GetPixel(w, h).R < 100)
                    {
                        bottomRight.X = w;
                        bottomRight.Y = h;
                        break;
                    }
                }
                if (!bottomRight.Equals(new Point(0, 0)))
                    break;
            }
            for (int h = 0; h < pic.Height; h++)
            {
                for (int w = 0; w < pic.Width; w++)
                {
                    if (pic.GetPixel(w, h).R < 100)
                    {
                        blackPixels++;
                    }
                }
            }
            int top = topRight.X - topLeft.X;
            int bottom = bottomRight.X - bottomLeft.X;
            string c;
            if (top == 1)
            {
                if (bottom == 1)
                {
                    c = "I";
                }
                else if (bottom == 3 || bottom == 4)
                {
                    c = "J";
                }
                else
                {
                    c = "L";
                }
            }
            else if (top == 2)
            {
                c = "A";
            }
            else if (top == 4)
            {
                if (bottom == 4)
                {
                    c = "O";
                }
                else
                {
                    c = "S";
                }
            }
            else if (top == 5)
            {
                if (bottom == 0)
                {
                    c = "Qu";
                }
                else if (blackPixels < 100)
                {
                    c = "C";
                }
                else
                {
                    c = "G";
                }
            }
            else if (top == 7)
            {
                if (blackPixels < 111)
                {
                    c = "D";
                }
                else
                {
                    c = "B";
                }
            }
            else if (top == 8)
            {
                if (bottom == 1)
                {
                    c = "P";
                }
                else if (blackPixels < 112)
                {
                    c = "D";
                }
                else
                {
                    c = "B";
                }
            }
            else if (top == 9)
            {
                c = "R";
            }
            else if (top == 12)
            {
                if (bottom == 1)
                {
                    c = "F";
                }
                else
                {
                    c = "Z";
                }
            }
            else if (top == 13)
            {
                if (bottom == 13)
                {
                    c = "E";
                }
                else
                {
                    c = "Z";
                }
            }
            else if (top == 14)
            {
                if (bottom == 1)
                {
                    c = "T";
                }
                else if (bottom == 5 || bottom == 6)
                {
                    c = "U";
                }
                else if (bottom == 16)
                {
                    c = "X";
                }
                else if (blackPixels < 100)
                {
                    if (blackPixels == 91)
                    {
                        c = "H";
                    }
                    else
                    {
                        c = "K";
                    }
                }
                else
                {
                    c = "N";
                }
            }
            else if (top == 16)
            {
                if (blackPixels < 65)
                {
                    c = "Y";
                }
                else
                {
                    c = "V";
                }
            }
            else if (top == 17)
            {
                c = "V";
            }
            else if (top == 18)
            {
                c = "M";
            }
            else
            {
                c = "W";
            }
            /*
I1 1
J1 3
L1 11
A2 16
O4 4
S4 5
QU5 0
 C5 5 86
 G5 5 MORE 105
B7 8
P8 1
D8 8
R9 16
F12 1
z12 14
E13 13
T14 1
U14 5
 H14 14 91
 N14 14 MORE 118
X14 16
 Y16 1 62
 V17,16 1 74
M18 18
W24 14
    */
            c = debugPics(c, pic, topLeft, topRight, bottomLeft, bottomRight, blackPixels, row, col);
            return c;
        }
        private string debugPics(string c, Bitmap pic, Point topLeft, Point topRight, Point bottomLeft, Point bottomRight, int blackPixels, int row, int col)
        {
            /*save s = new save(c);
            s.ShowDialog();
            c = s.Letter.ToUpper();
            if (c == "QU")
            {
                c = "Qu";
            }*/
            OCR o = new OCR(pic);
            string st = c.ToUpper() + ((_E5) ? "5" : "4") + "r" + row + "c" + col;
            while (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Letters\\" + st + "r" + row + "c" + col + ".bmp"))
            {
                pic.Save(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Letters\\" + st + "r" + row + "c" + col + ".bmp");
            }
            StreamWriter fileW = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Letters\\letters.txt", true);
            fileW.WriteLine(st + " topLeft:" + topLeft.X + ", " + topLeft.Y + "  topRight:" + topRight.X + ", " + topRight.Y + " bottomLeft:" + bottomLeft.X + ", " + bottomLeft.Y + " bottomRight:" + bottomRight.X + ", " + bottomRight.Y + " blackPixels:" + blackPixels + " " + o.String);
            fileW.Close();
            return c;
        }
    }
}
