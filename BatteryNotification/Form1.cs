using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace BatteryNotification
{
    public partial class Form1 : Form
    {
        private Timer timer;
        private ContextMenuStrip contextMenu;

        public Form1()
        {
            InitializeComponent();
            notifyIcon1.Visible = true;
            notifyIcon1.MouseClick += NotifyIcon1_MouseClick;
            contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Exit", null, Exit_Click);
            notifyIcon1.ContextMenuStrip = contextMenu;

            timer = new Timer();
            timer.Interval = 60000; // Update every 60 seconds
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
            var powerStatus = SystemInformation.PowerStatus;
            int batteryPercentage = (int)(powerStatus.BatteryLifePercent * 100);
            notifyIcon1.Text = $"Battery: {batteryPercentage}%";
            notifyIcon1.Icon = GenerateBatteryIcon(batteryPercentage);
        }

        private Icon GenerateBatteryIcon(int percentage)
        {
            Bitmap bitmap = new Bitmap(18, 18); // Create a slightly larger bitmap to be scaled down
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Transparent);
                //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias; // Enable anti-aliasing
                string text = percentage.ToString();
                Font font = new Font("Tahoma", 7, FontStyle.Regular);
                SizeF textSize = g.MeasureString(text, font);
                float x = (16 - textSize.Width) / 2;
                float y = (16 - textSize.Height) / 2;
                g.DrawString(text, font, Brushes.White, new PointF(x, y));
            }
            return Icon.FromHandle(bitmap.GetHicon());
        }

        private void UpdateIcon()
        {
            //TODO: Add icons for AC power and battery power
            //notifyIcon1.Icon = IsOnACPower() ? acIcon : batteryIcon;
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

        private void Exit_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
