using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;

namespace Word.Forms
{
    public partial class ColorPickerForm : Form
    {
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point lpPoint);
        private Timer timer;
        private IKeyboardMouseEvents globalHook;

        public ColorPickerForm()
        {
            InitializeComponent();
        }

        private void GlobalHook_MouseDownExt(object sender, MouseEventExtArgs e)
        {
            if (!timer.Enabled) return;
            timer.Stop();

            Cursor.Current = Cursors.Default;

            Color pickedColor;

            // Marshal everything that touches UI or Word COM to the main thread
            Invoke((MethodInvoker)(() =>
            {
                // Read BackColor safely
                pickedColor = pictureBox_colorPreview.BackColor;
                var hexColor = $"#{pickedColor.R:X2}{pickedColor.G:X2}{pickedColor.B:X2}";
                Clipboard.SetText(hexColor);
                textBox_HexColor.Text = hexColor;
            }));
        }

        private Color GetColorAt(Point point)
        {
            using (var bmp = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(point, Point.Empty, new Size(1, 1));
                return bmp.GetPixel(0, 0);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            GetCursorPos(out var pos);
            var c = GetColorAt(pos);
            pictureBox_colorPreview.BackColor = c;
        }

        private void button_ColorPicker_Click(object sender, EventArgs e)
        {
            if (timer == null)
            {
                timer = new Timer { Interval = 10 };
                timer.Tick += Timer_Tick;
            }

            if (globalHook == null)
            {
                globalHook = Hook.GlobalEvents();
                globalHook.MouseDownExt += GlobalHook_MouseDownExt;
            }

            timer.Start();
        }

        private void ColorPickerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop the timer
            if (timer != null && timer.Enabled)
                timer.Stop();

            // Unhook and dispose the global mouse hook
            if (globalHook != null)
            {
                globalHook.MouseDownExt -= GlobalHook_MouseDownExt;
                globalHook.Dispose();
                globalHook = null;
            }
        }
    }
}
