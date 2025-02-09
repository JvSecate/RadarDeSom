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
			global::System.ComponentModel.ComponentResourceManager resources = new global::System.ComponentModel.ComponentResourceManager(typeof(global::RadarDeSom.Overlay));
			this.RadarBox = new global::System.Windows.Forms.PictureBox();
			((global::System.ComponentModel.ISupportInitialize)this.RadarBox).BeginInit();
			base.SuspendLayout();
			int yPosRadar = 50;
			this.RadarBox.Location = new global::System.Drawing.Point(12, yPosRadar);
			this.RadarBox.Name = "RadarBox";
			this.RadarBox.Size = new global::System.Drawing.Size(150, 150);
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(512, 512);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.None;
			base.Icon = (global::System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "Overlay";
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.Manual;
			base.Location = new global::System.Drawing.Point(150, 150);
			this.Text = "SoundRadar Overlay";
			base.TopMost = true;
			base.Controls.Add(this.RadarBox);
			base.Load += new global::System.EventHandler(this.Overlay_Load);
			((global::System.ComponentModel.ISupportInitialize)this.RadarBox).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::System.ComponentModel.IContainer components = null;

		private global::System.Windows.Forms.PictureBox RadarBox;
	}
}
