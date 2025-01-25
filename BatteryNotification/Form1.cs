using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace BatteryNotification
{
    public partial class Form1 : Form
    {
        private Timer timer;
        private ContextMenuStrip contextMenu;
        private const string MessageBoxCaption = "Battery Notification";

        public Form1()
        {
            InitializeComponent();
            notifyIcon1.Visible = true;
            notifyIcon1.MouseClick += NotifyIcon1_MouseClick;
            contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("About...", null, About_Click);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Exit", null, Exit_Click);
            notifyIcon1.ContextMenuStrip = contextMenu;

            timer = new Timer();
            timer.Interval = 10000; // Update every 10 seconds
            timer.Tick += Timer_Tick;
            timer.Start();
            UpdateBatteryPercentage();

            // Hide the window on startup
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateBatteryPercentage();
        }

        private void UpdateBatteryPercentage()
        {
            String batteryMessage = "";
            if (IsOnACPower())
            {
                batteryMessage = "(plugged in)";
            }
            else
            {
                batteryMessage = "(unplugged)";
            }
            var powerStatus = SystemInformation.PowerStatus;
            int batteryPercentage = (int)(powerStatus.BatteryLifePercent * 100);
            notifyIcon1.Text = $"Battery: {batteryPercentage}% available {batteryMessage}";
            notifyIcon1.Icon = GenerateBatteryIcon(batteryPercentage);
        }

        private Icon GenerateBatteryIcon(int percentage)
        {
            int size = 24; // Size in pixels
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                Rectangle circleArea = new Rectangle(0, 0, size, size);
                if (IsOnACPower())
                {
                    g.FillEllipse(Brushes.RoyalBlue, circleArea);
                }
                else
                {
                    g.FillEllipse(Brushes.Gray, circleArea);
                }
                //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias; // Enable anti-aliasing
                string text = percentage.ToString();
                Font font = new Font("Tahoma", 9, FontStyle.Regular);
                SizeF textSize = g.MeasureString(text, font);
                float x = (size - textSize.Width) / 2;
                float y = (size - textSize.Height) / 2;
                g.DrawString(text, font, Brushes.White, new PointF(x, y));
            }
            return Icon.FromHandle(bitmap.GetHicon());
        }

        public static bool IsOnACPower()
        {
            PowerStatus powerStatus = SystemInformation.PowerStatus;
            return powerStatus.PowerLineStatus == PowerLineStatus.Online;
        }

        private void NotifyIcon1_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenu.Show(Cursor.Position);
            }
        }

        private void About_Click(object? sender, EventArgs e)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                Version version = entryAssembly.GetName().Version ?? new Version(1, 0, 0, 0);
                string versionString = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
                MessageBox.Show($"Battery Notification {versionString}\n\nCopyright (c) 2025 Bryan Candiliere\n\nLicensed under the MIT License.", MessageBoxCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Battery Notification\n\nCopyright (c) 2025 Bryan Candiliere\n\nLicensed under the MIT License.", MessageBoxCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Exit_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
