using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace winLive
{
    public partial class Main : Form
    {
        public static Bitmap BM = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Cursor.Hide();

            Thread.Sleep(250);

            Graphics GH = Graphics.FromImage(BM as Image);
            GH.CopyFromScreen(0, 0, 0, 0, BM.Size);

            ScreenAnts form = new ScreenAnts((int)fpsNumericUpDown.Value, (int)redNumericUpDown.Value, (int)greenNumericUpDown.Value, (int)blueNumericUpDown.Value);
            form.ShowDialog();

            this.Show();
            Cursor.Show();
        }
    }
}