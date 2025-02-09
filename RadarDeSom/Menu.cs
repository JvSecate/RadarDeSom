using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace RadarDeSom
{
    public partial class Menu : Form
    {
        private Overlay overlay;
        public Menu()
        {
            this.InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (overlay == null || overlay.IsDisposed)
            {
                overlay = new Overlay
                {
                    ParentHandle = base.Handle
                };
                overlay.Show();
            }
            else
            {
                overlay.BringToFront();
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Texto", "aaaa", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel1.Text = string.Format("Versão: {0}", Assembly.GetExecutingAssembly().GetName().Version);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "settings.ini";
            Process.Start("notepad.exe", filePath);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (overlay != null && !overlay.IsDisposed)
            {
                overlay.Close();
            }
            base.OnFormClosing(e);
        }
    }
}
