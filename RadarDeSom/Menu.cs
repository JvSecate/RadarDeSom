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

        private void button4_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", "https://github.com/JvSecate/RadarDeSom/blob/master/README.md");
        }

        private bool radarTravado = true;
        private void button3_Click_1(object sender, EventArgs e)
        {
            if (overlay == null || overlay.IsDisposed)
            {
                MessageBox.Show("O radar ainda não foi iniciado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            radarTravado = !radarTravado;
            overlay.ModoMovimento = !radarTravado;

            if (radarTravado)
            {
                //MessageBox.Show(string.Format("a {0}, b {1}", overlay.Width, overlay.Height), "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                button3.Text = "Mover";
                overlay.FormBorderStyle = FormBorderStyle.None;
                overlay.TopMost = true;
            }
            else
            {
                button3.Text = "Fixar";
                overlay.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                overlay.WindowState = FormWindowState.Normal;
                overlay.Width = 167;
                overlay.Height = 190;

                overlay.TopMost = false;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
