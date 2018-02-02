using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace Dispatcher_Tool_Rework
{
    public partial class Main_Form : Form
    {
        List<Panel> Panel_List = new List<Panel>();
        bool formOpen = false;
        int counter = 1;
        Form form3;

        public Main_Form()
        {
            InitializeComponent();

            Cef.Initialize();
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                ToolStripItem ScreenItem = runToolStripMenuItem.DropDownItems.Add(Screen.AllScreens[i].DeviceName);
                ScreenItem.Tag = i;
                ScreenItem.Click += new EventHandler(ScreenMenuClick);
            }
        }

        void New_Panel(string template, bool locked)
        {
            Panel panel = new Panel()
            {
                Width = 440,
                Height = 40,

                AutoSize = true,

                Name = "Panel_" + counter.ToString(),
                Tag = counter,

                Top = (45 * counter) - 15,
                Left = 20
            };
            Label Counter_Label = new Label();
            Counter_Label.Left = 0;
            Counter_Label.Top = 8;
            Counter_Label.Width = 30;
            Counter_Label.Text = counter.ToString();

            panel.Controls.Add(Counter_Label);


            TextBox text_input = new TextBox();
            text_input.Left = 40;
            text_input.Top = 0;
            text_input.Width = 350;
            text_input.Font = new Font(text_input.Font.FontFamily, 15);

            text_input.Text = template;
            text_input.Name = "Text_Input";

            if (locked)
            {
                text_input.ReadOnly = true;
            }

            panel.Controls.Add(text_input);

            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem menuRemove = new ToolStripMenuItem("Remove");
            menuRemove.Click += new EventHandler(RemovePanel);
            menu.Items.Add(menuRemove);

            panel.ContextMenuStrip = menu;

            counter++;
            Panel_List.Add(panel);
            this.Controls.Add(panel);
        }

        void OpenNewScreen(int screen)
        {
            form3 = new Form();
            form3.Controls.Clear();
            Screen screenToUse = Screen.AllScreens[screen];

            form3.FormBorderStyle = FormBorderStyle.None;
            form3.WindowState = FormWindowState.Maximized;
            form3.BackColor = Color.Black;
            form3.FormClosed += new FormClosedEventHandler(Form_Closing);

            form3.StartPosition = FormStartPosition.Manual;
            form3.Location = screenToUse.Bounds.Location;


            int cols = 0;
            int rows = 0;
            int total = Panel_List.Count;

            foreach (Panel panel in Panel_List)
            {
                List<TextBox> text_input = panel.Controls.OfType<TextBox>().ToList();

                int ScreenH = screenToUse.Bounds.Height;
                int ScreenW = screenToUse.Bounds.Width;

                string text_in = text_input[0].Text;

                ChromiumWebBrowser browser = new ChromiumWebBrowser(text_input[0].Text);
                browser.Dock = DockStyle.None;

                Label Address = new Label();
                Address.Text = text_input[0].Text;
                Address.Height = 20;
                Address.Font = new Font(Address.Font.FontFamily, 14);
                Address.ForeColor = Color.White;
                Address.Tag = browser;
                Address.Cursor = Cursors.Hand;
                Address.AutoSize = true;

                Address.Click += new EventHandler(Label_Clicked);

                if (Panel_List.Count > 1)
                {
                    decimal notRoundTotal = (decimal)total / 2;

                    if (ScreenH < ScreenW)
                    {
                        ScreenH /= 2;
                        browser.ClientSize = new Size(Convert.ToInt32(ScreenW / Math.Ceiling(notRoundTotal)), ScreenH - 20);
                        browser.Location = new Point(Convert.ToInt32(ScreenW / Math.Ceiling(notRoundTotal)) * cols, ScreenH * rows + 20);

                        Address.Location = new Point(Convert.ToInt32(ScreenW / Math.Ceiling(notRoundTotal)) * cols, ScreenH * rows);
                        Address.Width = ScreenW / (total / 2);

                        cols++;
                        if (cols == Math.Ceiling(notRoundTotal))
                        {
                            cols = 0;
                            rows++;
                        }
                    }
                    else
                    {
                        browser.ClientSize = new Size(ScreenW / 2, Convert.ToInt32(ScreenH / Math.Ceiling(notRoundTotal)) - 20);
                        browser.Location = new Point(ScreenW * cols / 2, Convert.ToInt32(ScreenH / Math.Ceiling(notRoundTotal)) * rows + 20);

                        Address.Location = new Point(ScreenW * cols / 2, Convert.ToInt32(ScreenH / Math.Ceiling(notRoundTotal)) * rows);
                        Address.Width = ScreenW / 2;

                        cols++;
                        if (cols == 2)
                        {
                            cols = 0;
                            rows++;
                        }
                    }

                    form3.Controls.Add(Address);
                    form3.Controls.Add(browser);
                }
                else
                {
                    browser.ClientSize = new Size(ScreenW, ScreenH);
                    form3.Controls.Add(browser);
                }
            }

            form3.Show();
            formOpen = true;
        }

        void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            New_Panel(null, false);
        }

        void addMultipleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 Add_Form = new Form2();
            Add_Form.SettingsUpdated += new Form2.SettingsUpdateHandler(addMultipleToolStripMenuItem_ButtonClicked);
            Add_Form.Show();
        }

        void addMultipleToolStripMenuItem_ButtonClicked(object sender, SettingsUpdateArgs Panels)
        {
            for (int i = 0; i < Panels.Quantity; i++)
            {
                New_Panel(Panels.Template, false);
            }
        }

        void RemovePanel(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            ContextMenuStrip menu = (ContextMenuStrip)menuItem.GetCurrentParent();
            Panel panel = (Panel)menu.SourceControl;
            this.Controls.Remove(panel);
            Panel_List.Remove(panel);
            counter--;
            foreach (Panel proceeding in Panel_List.Where(p => Convert.ToInt32(p.Tag) > Convert.ToInt32(panel.Tag)))
            {
                List<Label> label = proceeding.Controls.OfType<Label>().ToList();
                label[0].Text = (Convert.ToInt32(label[0].Text) - 1).ToString();
                proceeding.Tag = Convert.ToInt32(proceeding.Tag) - 1;
                proceeding.Top = proceeding.Top - 45;
            }
        }

        void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Import_File_Dialog.ShowDialog() == DialogResult.OK)
            {
                this.Controls.OfType<Panel>().ToList().ForEach(panel => this.Controls.Remove(panel));
                counter = 1;
                Panel_List.Clear();
                bool lock_input = false;

                try
                {
                    List<string> All_Lines = File.ReadAllLines(Import_File_Dialog.FileName).ToList();
                    if (All_Lines[0] == "locked")
                    {
                        lock_input = true;
                        All_Lines.RemoveAt(0);
                    }


                    foreach (string line in All_Lines)
                    {
                        New_Panel(line, lock_input);
                    }
                }
                catch (Exception FileImportEx)
                {
                    MessageBox.Show(FileImportEx.Message);
                }
            }
        }

        void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Panel_List.Count != 0)
            {
                DialogResult ExportResult = MessageBox.Show("Lock this configuration?", "Error", MessageBoxButtons.YesNoCancel);
                if (ExportResult != DialogResult.Cancel)
                {
                    if (Export_Config_Dialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            using (StreamWriter writer = new StreamWriter(Export_Config_Dialog.OpenFile()))
                            {
                                if (ExportResult == DialogResult.Yes)
                                {
                                    writer.WriteLine("locked");
                                    foreach (TextBox text in Panel_List.SelectMany(t => t.Controls.OfType<TextBox>()))
                                    {
                                        text.ReadOnly = true;
                                    }
                                }

                                foreach (TextBox text in Panel_List.SelectMany(t => t.Controls.OfType<TextBox>()))
                                {
                                    writer.WriteLine(text.Text);
                                }
                            }
                        }
                        catch (Exception FileExportEx)
                        {
                            MessageBox.Show(FileExportEx.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("What's the point of exporting nothing?");
            }
        }

        void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Panel panel in Panel_List)
            {
                this.Controls.Remove(panel);
                counter--;
            }
            Panel_List.Clear();
        }

        void ScreenMenuClick(object sender, EventArgs e)
        {
            if (!formOpen)
            {
                ToolStripItem screen_item = (ToolStripItem)sender;
                OpenNewScreen(Convert.ToInt32(screen_item.Tag));

                MenuStrip strip = screen_item.GetCurrentParent() as MenuStrip;

            }
            else
            {
                MessageBox.Show("Only one window supported");
            }
        }

        void Label_Clicked(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            ChromiumWebBrowser browser = (ChromiumWebBrowser)label.Tag;

            browser.Reload();
        }

        void Form_Closing(object sender, FormClosedEventArgs e)
        {
            try
            {
                formOpen = false;
            }
            catch (Exception CloseException)
            {
                MessageBox.Show(CloseException.Message);
            }
        }

        private void Main_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            Cef.Shutdown();
            this.Dispose();
        }
    }
}
