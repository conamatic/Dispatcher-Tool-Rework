﻿using System;
using System.Windows.Forms;

namespace Dispatcher_Tool_Rework
{
    public partial class Add_Form : Form
    {
        public delegate void AddMultipleUpdateHandler(object sender, AddMultipleUpdateArgs e);
        public event AddMultipleUpdateHandler SettingsUpdated;

        public Add_Form()
        {
            InitializeComponent();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Save_Button_Click(object sender, EventArgs e)
        {
            SaveAndDispose();
        }

        private void Quantity_Input_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                SaveAndDispose();
            }
        }

        private void SaveAndDispose()
        {
            try
            {
                int newQuantity = Convert.ToInt32(Quantity_Input.Text);
                string newTemplate = Template_Input.Text;

                AddMultipleUpdateArgs args = new AddMultipleUpdateArgs(newQuantity, newTemplate);
                SettingsUpdated(this, args);
                this.Dispose();
            }
            catch (Exception AddNewException)
            {
                MessageBox.Show(AddNewException.Message);
            }
        }
    }

    public class AddMultipleUpdateArgs : System.EventArgs
    {
        private int mQuantity;
        private string mTemplate;

        public AddMultipleUpdateArgs(int NewQuantity, string NewTemplate)
        {
            this.mQuantity = NewQuantity;
            this.mTemplate = NewTemplate;
        }

        public int Quantity
        {
            get
            {
                return mQuantity;
            }
        }
        public string Template
        {
            get
            {
                return mTemplate;
            }
        }
    }
}
