using System;
using System.Drawing;
using System.Windows.Forms;

namespace Rotation
{
    public partial class TransformsForm : Form
    {
        public TransformsForm()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.Width = 500;
            this.Height = 500;
            this.BackColor = Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen blackPen = new Pen(Color.Black);
            Font myFont = new Font("Helvetica", 9);
            Brush blackWriter = new SolidBrush(Color.Black);

            PointF[] square = new PointF[]
            {
                new PointF(100, 100),
                new PointF(200, 100),
                new PointF(200, 200),
                new PointF(100, 200)
            };

            g.DrawPolygon(blackPen, square);
            g.DrawString("before rotation", myFont, blackWriter, 100, 205);

            PointF centre = new PointF(150, 150);
            float angle = 30f;
            PointF[] rotatedSquare = Tmatrix.matrixRotate(square, angle, centre);

            g.DrawPolygon(blackPen, rotatedSquare);
            g.DrawString("after " + angle + " degree rotation", myFont, blackWriter, 220, 100);
        }

        public static void Main()
        {
            Application.Run(new TransformsForm());
        }
    }
}