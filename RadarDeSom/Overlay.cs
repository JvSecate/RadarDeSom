using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using IniParser;
using IniParser.Model;
using NAudio.CoreAudioApi;

namespace RadarDeSom
{
    public partial class Overlay : Form
    {
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        private DateTime[] lastHighlightedTimestamps = new DateTime[12];

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public bool ModoMovimento { get; set; } = false;

        private volatile bool _isRunning = true;
        private Thread _workerThread;

        public Overlay()
        {
            this.InitializeComponent();
            this.ClientSize = RadarBox.Size;
            RadarBox.Location = new Point(0, 0);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _isRunning = false;

            if (_workerThread != null && _workerThread.IsAlive)
            {
                _workerThread.Join(1000); // max wait 1 second
            }

            _device?.Dispose();
            _enumerator?.Dispose();

            base.OnFormClosing(e);

            Environment.Exit(0);  // hard kill
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (ModoMovimento && e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0xA1, 0x2, 0); // 0xA1 = WM_NCLBUTTONDOWN, 0x2 = HTCAPTION
            }

            base.OnMouseDown(e);
        }

        private void Overlay_Load(object sender, EventArgs e)
        {
            base.TransparencyKey = Color.Turquoise;
            this.BackColor = Color.Turquoise;
            int initialStyle = Overlay.GetWindowLong(base.Handle, -20);
            Overlay.SetWindowLong(base.Handle, -20, initialStyle | 524288 | 32);
            base.WindowState = FormWindowState.Maximized;
            base.TopMost = true;
            base.Opacity = 0.7;
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(AppDomain.CurrentDomain.BaseDirectory + "settings.ini");
            this.RadarBox.Location = new System.Drawing.Point(int.Parse(data["basic"]["positionX"]), int.Parse(data["basic"]["positionY"]));
            this._multiplier = int.Parse(data["basic"]["multiplier"]);
            this._updateRate = int.Parse(data["basic"]["updateRate"]);
            this._delay = int.Parse(data["basic"]["delay"]);
            this._sectionAmount = int.Parse(data["sectionHighlights"]["sectionAmount"]);
            this._highlightDurationSeconds = int.Parse(data["sectionHighlights"]["highlightDurationSeconds"]);
            this._highlightSoundThreshold = int.Parse(data["sectionHighlights"]["highlightSoundThreshold"]);
            _workerThread = new Thread(new ThreadStart(this.Loop));
            _workerThread.Start();
        }

        //Loop Inicial
        private void Loop()
        {
            this._enumerator = new MMDeviceEnumerator();
            this._device = this._enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            bool flag = this._device.AudioMeterInformation.PeakValues.Count < 8;
            if (flag)
            {
                if (MessageBox.Show("O dispositivo de áudio utilizado não é 7.1. Deseja continuar para o modo estéreo?\nThe audio device in use is not 7.1. Do you want to continue in stereo mode?", "Audio 7.1 Não Detectado!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    StereoOverlay();
                }
                else
                {
                    Application.Exit();
                }
            }
            else
            {
                SurroundOverlay();
            }
        }

        //Calcula a posição do radar para dispositivos Estereos
        private void StereoOverlay()
        {
            const int history = 100;
            float[] left = new float[history];
            float[] right = new float[history];

            int idx = 0;

            Graphics grp = Graphics.FromImage(this._radar);
            grp.FillRectangle(Brushes.Black, 0, 0, this._radar.Width, this._radar.Height);

            for (; ; )
            {
                left[idx] = this._device.AudioMeterInformation.PeakValues[0];
                right[idx] = this._device.AudioMeterInformation.PeakValues[1];
                idx = (idx + 1) % 100;

                float tempFive = left[(idx + (history - this._delay)) % history] * (float)this._multiplier;
                float tempSix = right[(idx + (history - this._delay)) % history] * (float)this._multiplier;


                float x = 75f - tempFive + tempSix;
                float y = 75f;
                bool flag2 = y < 10f;
                if (flag2)
                {
                    y = 10f;
                }
                bool flag3 = x < 10f;
                if (flag3)
                {
                    x = 10f;
                }
                bool flag4 = y > 140f;
                if (flag4)
                {
                    y = 140f;
                }
                bool flag5 = x > 140f;
                if (flag5)
                {
                    x = 140f;
                }
                this.CreateRadarTriangle((int)x, (int)y, (int)idx);
                Thread.Sleep(this._updateRate);
            }
        }

        //Calcula a posição do radar para dispositivos Surrounds 7.1
        private void SurroundOverlay()
        {
            const int history = 100;
            float[] leftTop = new float[history];
            float[] rightTop = new float[history];
            float[] leftBottom = new float[history];
            float[] rightBottom = new float[history];
            float[] left = new float[history];
            float[] right = new float[history];

            int idx = 0;

            Graphics grp = Graphics.FromImage(this._radar);
            grp.FillRectangle(Brushes.Black, 0, 0, this._radar.Width, this._radar.Height);

            while (_isRunning)
            {
                leftTop[idx] = this._device.AudioMeterInformation.PeakValues[0];
                rightTop[idx] = this._device.AudioMeterInformation.PeakValues[1];
                leftBottom[idx] = this._device.AudioMeterInformation.PeakValues[4];
                rightBottom[idx] = this._device.AudioMeterInformation.PeakValues[5];
                left[idx] = this._device.AudioMeterInformation.PeakValues[6];
                right[idx] = this._device.AudioMeterInformation.PeakValues[7];
                idx = (idx + 1) % 100;


                float tempOne = leftTop[(idx + (history - this._delay)) % history] * (float)this._multiplier;
                float tempTwo = rightTop[(idx + (history - this._delay)) % history] * (float)this._multiplier;
                float tempThree = leftBottom[(idx + (history - this._delay)) % history] * (float)this._multiplier;
                float tempFour = rightBottom[(idx + (history - this._delay)) % history] * (float)this._multiplier;
                float tempFive = left[(idx + (history - this._delay)) % history] * (float)this._multiplier;
                float tempSix = right[(idx + (history - this._delay)) % history] * (float)this._multiplier;


                float x = 75f - tempOne + tempTwo - tempFive + tempSix;
                float y = 75f - tempOne - tempTwo;
                x = x - tempThree + tempFour;
                y = y + tempThree + tempFour;
                bool flag2 = y < 10f;
                if (flag2)
                {
                    y = 10f;
                }
                bool flag3 = x < 10f;
                if (flag3)
                {
                    x = 10f;
                }
                bool flag4 = y > 140f;
                if (flag4)
                {
                    y = 140f;
                }
                bool flag5 = x > 140f;
                if (flag5)
                {
                    x = 140f;
                }
                this.CreateRadarTriangle((int)x, (int)y, (int)idx);
                Thread.Sleep(this._updateRate);
            }
        }
        private void CreateRadarTriangle(int x, int y, int idx)
        {

            Graphics grp = Graphics.FromImage(this._radar);

            Point[][] triangles = new Point[this._sectionAmount][];
            Point center = new Point(75, 75);
            double angleStep = 360.0 / this._sectionAmount;
            double overlapAngle = 0;

            for (int i = 0; i < this._sectionAmount; i++)
            {
                double angle1 = Math.PI / 180 * ((i * angleStep) - overlapAngle);
                double angle2 = Math.PI / 180 * (((i + 1) * angleStep) + overlapAngle);

                Point vertex1 = new Point(center.X + (int)(150 * Math.Cos(angle1)), center.Y + (int)(150 * Math.Sin(angle1)));
                Point vertex2 = new Point(center.X + (int)(150 * Math.Cos(angle2)), center.Y + (int)(150 * Math.Sin(angle2)));

                triangles[i] = new Point[] { center, GetIntersectionPoint(center, vertex1), GetIntersectionPoint(center, vertex2) };
            }

            for (int i = 0; i < this._sectionAmount; i++)
            {
                if ((DateTime.Now - lastHighlightedTimestamps[i]).TotalSeconds > this._highlightDurationSeconds)
                {
                    grp.FillPolygon(Brushes.Black, triangles[i]);
                }
                grp.DrawLine(Pens.MintCream, triangles[i][0], triangles[i][1]);
                grp.DrawLine(Pens.MintCream, triangles[i][0], triangles[i][2]);
            }

            int section = -1;
            for (int i = 0; i < this._sectionAmount; i++)
            {
                if (IsPointInTriangle(new Point(x, y), triangles[i][0], triangles[i][1], triangles[i][2]))
                {
                    section = i;
                    break;
                }
            }

            double normalized_x = (75f - x) % 75;
            double normalized_y = (75f - y) % 75;
            double magnitude = Math.Sqrt((normalized_x * normalized_x) + (normalized_y * normalized_y));
            if (section != -1 && magnitude >= this._highlightSoundThreshold)
            {
                grp.FillPolygon(Brushes.Green, triangles[section]);
                lastHighlightedTimestamps[section] = DateTime.Now;

            }


            grp.FillRectangle(Brushes.Red, x - 5, y - 5, 10, 10);

            if (RadarBox != null &&
                !RadarBox.IsDisposed &&
                RadarBox.IsHandleCreated)
            {
                try
                {
                    RadarBox.Invoke(new MethodInvoker(delegate ()
                    {
                        RadarBox.Image = _radar;
                    }));
                }
                catch (InvalidAsynchronousStateException)
                {
                    this.Close();
                }
            }
        }
        private Point GetIntersectionPoint(Point p1, Point p2)
        {
            float t;
            if (p2.X - p1.X != 0)
            {
                t = Math.Min(Math.Max((-150 - p1.X) / (float)(p2.X - p1.X), (300 - p1.X) / (float)(p2.X - p1.X)), 1);
            }
            else
            {
                t = Math.Min(Math.Max((-150 - p1.Y) / (float)(p2.Y - p1.Y), (300 - p1.Y) / (float)(p2.Y - p1.Y)), 1);
            }

            return new Point((int)(p1.X + t * (p2.X - p1.X)), (int)(p1.Y + t * (p2.Y - p1.Y)));
        }

        private bool IsPointInTriangle(Point pt, Point v1, Point v2, Point v3)
        {
            float d1, d2, d3;
            bool has_neg, has_pos;

            d1 = Sign(pt, v1, v2);
            d2 = Sign(pt, v2, v3);
            d3 = Sign(pt, v3, v1);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }

        private float Sign(Point p1, Point p2, Point p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }
        private MMDeviceEnumerator _enumerator;
        private MMDevice _device;

        private int _multiplier = 500;
        private int _updateRate = 50;

        private int _sectionAmount = 12;
        private int _highlightDurationSeconds = 3;
        private int _highlightSoundThreshold = 50;
        private int _delay = 5;

        private readonly Bitmap _radar = new Bitmap(150, 150);
        public IntPtr ParentHandle;
    }
}
