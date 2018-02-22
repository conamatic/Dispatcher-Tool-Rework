﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dispatcher_Tool_Rework
{
    public partial class Settings : Form
    {
        public delegate void SettingsUpdateHandler(object sender, SettingsUpdateArgs e);
        public event SettingsUpdateHandler SettingsUpdated;

        public Settings(int AttemptedRows, int AttemptedCols)
        {
            InitializeComponent();
            Rows_Input.Text = AttemptedRows.ToString();
            Columns_Input.Text = AttemptedCols.ToString();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Save_Button_Click(object sender, EventArgs e)
        {
            try
            {
                int newRows = (String.IsNullOrEmpty(Rows_Input.Text)) ? 0 : Convert.ToInt32(Rows_Input.Text);
                int newCols = (String.IsNullOrEmpty(Columns_Input.Text)) ? 0 : Convert.ToInt32(Columns_Input.Text);

                SettingsUpdateArgs args = new SettingsUpdateArgs(newRows, newCols);
                SettingsUpdated(this, args);
                this.Dispose();
            }
            catch (Exception AddNewException)
            {
                MessageBox.Show(AddNewException.Message);
            }
        }
    }

    public class SettingsUpdateArgs : System.EventArgs
    {
        private int newRows;
        private int newCols;

        public SettingsUpdateArgs(int Rows, int Cols)
        {
            this.newRows = Rows;
            this.newCols = Cols;
        }

        public int Rows
        {
            get
            {
                return newRows;
            }
        }
        public int Cols
        {
            get
            {
                return newCols;
            }
        }
    }

}
