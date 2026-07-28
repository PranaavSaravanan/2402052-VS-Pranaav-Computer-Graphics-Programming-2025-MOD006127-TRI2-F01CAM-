using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

namespace DoubleBuffering
{
    public partial class Flicker : Form
    {
        Rectangle rect;
        int x = 0;
        int y = 200;
        int dx = 1;
        int dy = 1;

        Bitmap backBuffer;
        Graphics bufferGraphics;

        public Flicker()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.Width = 400;
            this.Height = 400;

            rect = new Rectangle(x, y, 50, 50);

            // back buffer sized to the form; every frame is drawn here first, off-screen
            backBuffer = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            bufferGraphics = Graphics.FromImage(backBuffer);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Pen blackPen = new Pen(Color.Black);
            Brush blackBrush = new SolidBrush(Color.Black);
            Brush whiteBrush = new SolidBrush(Color.White);
            Font myFont = new System.Drawing.Font("Helvetica", 9);

            while (true)
            {
                bufferGraphics.FillRectangle(whiteBrush, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
                rect.Location = new Point(x, y);
                bufferGraphics.DrawRectangle(blackPen, rect);
                bufferGraphics.DrawString("Moving rectangle", myFont, blackBrush, 150, 150);

                // finished frame copied to the screen in one go, instead of drawing each piece directly
                e.Graphics.DrawImage(backBuffer, 0, 0);

                // reverse direction on hitting an edge, giving the right-angle bounce
                if (x <= 0 || x + rect.Width >= this.ClientSize.Width) dx = -dx;
                if (y <= 0 || y + rect.Height >= this.ClientSize.Height) dy = -dy;

                x += dx;
                y += dy;

                Thread.Sleep(10);
            }
        }
    }
}