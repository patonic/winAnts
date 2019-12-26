using RazorGDIControlWF;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace winLive
{
    public partial class ScreenAnts : Form
    {
        private int setFPS = 60;
        private int redAnt = 10;
        private int greenAnt = 10;
        private int blueAnt = 10;
        private int antsPerFrame = 1;

        private Thread renderthread;
        private System.Timers.Timer fpstimer;
        private int fps;

        private bool first = true;

        private Ants ants;

        public ScreenAnts(int fps, int red, int green, int blue)
        {
            InitializeComponent();

            this.Text = "";
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            SetStyle(ControlStyles.DoubleBuffer, false);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.Opaque, true);

            setFPS = fps;
            redAnt = red;
            greenAnt = green;
            blueAnt = blue;
        }

        private void ScreenAnts_Shown(object sender, EventArgs e)
        {
            fpstimer = new System.Timers.Timer(1000);
            fpstimer.Elapsed += (sender1, args) =>
            {
                Update(delegate
                {
                    if (fps <= setFPS)
                        antsPerFrame -= 1 + (int)((setFPS - fps) * 0.05);
                    else
                        antsPerFrame += 1 + (int)((fps - setFPS) * 0.5);
                    if (antsPerFrame <= 0)
                        antsPerFrame = 1;
                    Text = "FPS: " + fps + "; SPF: " + antsPerFrame;
                    fps = 0;
                });
            };
            fpstimer.Start();

            renderthread = new Thread(() =>
            {
                while (true)
                    Render();
            });
            renderthread.Start();
        }

        public void Update(MethodInvoker callback)
        {
            if (IsDisposed || Disposing)
                return;

            try
            {
                if (this.InvokeRequired)
                    this.Invoke(callback);
                else
                    callback();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Render()
        {
            if (first)
            {
                razorPainterWFCtl1.RazorGFX.DrawImage((Bitmap)Main.BM.Clone(), 0, 0);
                razorPainterWFCtl1.RazorPaint();
                ants = new Ants(redAnt, greenAnt, blueAnt, razorPainterWFCtl1.RazorBMP.Width, razorPainterWFCtl1.RazorBMP.Height);
                first = false;
            }
            else
                lock (razorPainterWFCtl1.RazorLock)
                {
                    ants.magic(ref razorPainterWFCtl1, antsPerFrame);
                    razorPainterWFCtl1.RazorPaint();
                }
            fps++;
        }

        private void ScreenLive_FormClosing(object sender, FormClosingEventArgs e)
        {
            renderthread.Abort();
        }

        private class Ants
        {
            private static Random random = new Random();

            public enum color { blue = 0, green = 1, red = 2 };

            public enum direction { none = 0, up = 1, right = 2, down = 3, left = 4 }

            private Ant[] ants;

            public struct Ant
            {
                public color color;
                public direction direction;
                public int x, y;

                public Ant(color color, int width, int height, int x = -1, int y = -1, direction direction = direction.none)
                {
                    this.color = color;
                    if (x != -1)
                        this.x = x;
                    else
                        this.x = random.Next(0, width - 1);
                    if (y != -1)
                        this.y = y;
                    else
                        this.y = random.Next(0, height - 1);
                    if (direction != direction.none)
                        this.direction = direction;
                    else
                        this.direction = (direction)random.Next(1, 5);
                }
            }

            public Ants(int redCount, int greenCount, int blueCount, int width, int height)
            {
                ants = new Ant[redCount + greenCount + blueCount];
                uint allTry = 0;
                for (int i = 0; i < ants.Length; i++)
                {
                    allTry++;
                    color color = (color)random.Next(3);
                    switch (color)
                    {
                        case color.blue:
                            {
                                if (blueCount-- > 0)
                                    ants[i] = new Ant(color, width, height);
                                else
                                    i--;
                            }
                            break;

                        case color.green:
                            {
                                if (greenCount-- > 0)
                                    ants[i] = new Ant(color, width, height);
                                else
                                    i--;
                            }
                            break;

                        case color.red:
                            {
                                if (redCount-- > 0)
                                    ants[i] = new Ant(color, width, height);
                                else
                                    i--;
                            }
                            break;
                    }
                }
            }

            public void magic(ref RazorPainterWFCtl image, int count = 1)
            {
                unsafe
                {
                    int width = image.Width;
                    int height = image.Height;
                    BitmapData bitmapData = image.RazorBMP.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, image.RazorBMP.PixelFormat);

                    int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(image.RazorBMP.PixelFormat) / 8;
                    int heightInPixels = bitmapData.Height;
                    int widthInBytes = bitmapData.Width * bytesPerPixel;
                    byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

                    for (int magicCount = 0; magicCount < count; magicCount++)
                    {
                        #region обработка

                        Parallel.For(0, ants.Length / 100 + 1, (int outerI) =>
                        {
                            for (int currI = outerI * 100; currI < (outerI + 1) * 100 && currI < ants.Length; currI++)
                            {
                                byte* currentLine = PtrFirstPixel + (ants[currI].y * bitmapData.Stride);
                                int x = ants[currI].x * bytesPerPixel;

                                color min = color.blue;
                                color max = color.blue;

                                for (int i = 1; i < 3; i++)
                                {
                                    if (currentLine[x + (int)min] > currentLine[x + i])
                                        min = (color)i;
                                    if (currentLine[x + (int)max] < currentLine[x + i])
                                        max = (color)i;
                                }

                                if (max == ants[currI].color)
                                {
                                    byte swap = currentLine[x + (int)max];
                                    currentLine[x + (int)max] = currentLine[x + (int)min];
                                    currentLine[x + (int)min] = swap;

                                    switch (ants[currI].direction)
                                    {
                                        case direction.up:
                                            ants[currI].direction = direction.left;
                                            ants[currI].x--;
                                            break;

                                        case direction.right:
                                            ants[currI].direction = direction.up;
                                            ants[currI].y--;
                                            break;

                                        case direction.down:
                                            ants[currI].direction = direction.right;
                                            ants[currI].x++;
                                            break;

                                        case direction.left:
                                            ants[currI].direction = direction.down;
                                            ants[currI].y++;
                                            break;
                                    }
                                }
                                else
                                {
                                    byte swap = currentLine[x + (int)ants[currI].color];
                                    currentLine[x + (int)ants[currI].color] = currentLine[x + (int)max];
                                    currentLine[x + (int)max] = swap;

                                    switch (ants[currI].direction)
                                    {
                                        case direction.up:
                                            ants[currI].direction = direction.right;
                                            ants[currI].x++;
                                            break;

                                        case direction.right:
                                            ants[currI].direction = direction.down;
                                            ants[currI].y++;
                                            break;

                                        case direction.down:
                                            ants[currI].direction = direction.left;
                                            ants[currI].x--;
                                            break;

                                        case direction.left:
                                            ants[currI].direction = direction.up;
                                            ants[currI].y--;
                                            break;
                                    }
                                }
                                if (ants[currI].x < 0)
                                    ants[currI].x = width - 1;
                                if (ants[currI].x >= width)
                                    ants[currI].x = 0;
                                if (ants[currI].y < 0)
                                    ants[currI].y = height - 1;
                                if (ants[currI].y >= height)
                                    ants[currI].y = 0;
                            }
                        });

                        #endregion обработка
                    }

                    image.RazorBMP.UnlockBits(bitmapData);
                }
            }
        }
    }
}