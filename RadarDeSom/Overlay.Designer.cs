using System.Windows.Forms;

namespace RadarDeSom
{
	public partial class Overlay : global::System.Windows.Forms.Form
	{
		protected override void Dispose(bool disposing)
		{
			bool flag = disposing && this.components != null;
			if (flag)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		protected override CreateParams CreateParams
		{
			get
			{
				const int csNoclose = 0x200;

				var cp = base.CreateParams;
				cp.ClassStyle |= csNoclose;
				return cp;
			}
		}
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Overlay));
            RadarBox = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)RadarBox).BeginInit();
            SuspendLayout();
            // 
            // RadarBox
            // 
            RadarBox.Dock = DockStyle.Fill;
            RadarBox.Location = new Point(0, 0);
            RadarBox.Margin = new Padding(0);
            RadarBox.Name = "RadarBox";
            RadarBox.Size = new Size(200, 174);
            RadarBox.TabIndex = 0;
            RadarBox.TabStop = false;
            // 
            // Overlay
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(200, 174);
            Controls.Add(RadarBox);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Location = new Point(150, 150);
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Overlay";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "SoundRadar Overlay";
            TopMost = true;
            Load += Overlay_Load;
            ((System.ComponentModel.ISupportInitialize)RadarBox).EndInit();
            ResumeLayout(false);
        }

        private global::System.ComponentModel.IContainer components = null;

		private global::System.Windows.Forms.PictureBox RadarBox;
	}
}
