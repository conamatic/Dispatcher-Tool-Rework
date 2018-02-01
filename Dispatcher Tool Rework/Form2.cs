using System;
using System.Windows.Forms;

namespace Dispatcher_Tool_Rework
{
    public partial class Form2 : Form
    {
        public delegate void SettingsUpdateHandler(object sender, SettingsUpdateArgs e);
        public event SettingsUpdateHandler SettingsUpdated;

        public Form2()
        {
            InitializeComponent();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Save_Button_Click(object sender, EventArgs e)
        {
            try
            {
                int newQuantity = Convert.ToInt32(Quantity_Input.Text);
                string newTemplate = Template_Input.Text;

                SettingsUpdateArgs args = new SettingsUpdateArgs(newQuantity, newTemplate);
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
        private int mQuantity;
        private string mTemplate;

        public SettingsUpdateArgs(int NewQuantity, string NewTemplate)
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
