using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

namespace XORDrawing
{
    public partial class XORDrawingForm : Form
    {
        Rectangle aRect;
        Rectangle anEllipse;
        Rectangle moving;
        int x = 0, y = 0;
        Graphics g;

        public XORDrawingForm()
        {
            InitializeComponent();
            aRect = new Rectangle(100, 100, 200, 200);
            anEllipse = new Rectangle(150, 150, 200, 100);
            moving = new Rectangle(x, y, 10, 10);

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.Width = 500;
            this.Height = 500;
            this.BackColor = Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            g = e.Graphics;
            Brush redBrush = new SolidBrush(Color.Purple);
            g.FillRectangle(redBrush, aRect);
            Brush greenBrush = new SolidBrush(Color.LightPink);
            g.FillEllipse(greenBrush, anEllipse);

            // moves the square by drawing then re-drawing in XOR mode, which erases it before the next step
            while (x < 500)
            {
                moving.Location = this.PointToScreen(new Point(x, y));
                ControlPaint.FillReversibleRectangle(moving, Color.Red);
                Thread.Sleep(10);
                ControlPaint.FillReversibleRectangle(moving, Color.Red);

                x++;
                y++;
            }
        }
    }

    public class XORDemo
    {
        public static void Main()
        {
            Application.Run(new XORDrawingForm());
        }
    }
}