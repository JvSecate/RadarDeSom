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

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel1.Text = string.Format("Version: {0}", 
                Assembly.GetExecutingAssembly().GetName().Version);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.ini");
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", "https://github.com/JvSecate/RadarDeSom/blob/master/README.md");
        }

        private bool radarTravado = true;
        private void button3_Click(object sender, EventArgs e)
        {
            if (overlay == null || overlay.IsDisposed)
            {
                MessageBox.Show("O radar ainda não foi iniciado.\nThe radar has not been started yet.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            radarTravado = !radarTravado;
            overlay.ModoMovimento = !radarTravado;

            if (radarTravado)
            {
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
            if (overlay != null) overlay.Close();
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Desenvolvido para o projeto: Representação Visual De Som Espacial Em Jogos Para Deficientes Auditivos.\r\n\nDeveloped for the project: Visual Representation of spatial sound in games for the hearing impaired\r\n\nDesenvolvedores | Developers: \r\n     João Victor do Nascimento Secate\r\n     Sofia de Moura Feitosa Bressa\r\n\nOrientador do projeto | Project Advisor:  \r\n     Rogério Pinto Alexandre\r\n\nBaseado em | Based on:\r\n     Canetis Radar\r\n", "Credits", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
