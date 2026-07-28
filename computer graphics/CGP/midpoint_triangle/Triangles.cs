using System;
using System.Drawing;
using System.Windows.Forms;

namespace CGPproject
{
    public partial class Triangles : Form
    {
        public Triangles()
        {
           
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.Width = 600;
            this.Height = 600;
            this.BackColor = Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen blackPen = new Pen(Color.Black);

            PointF p1 = new PointF(100, 100);
            PointF p2 = new PointF(500, 100);
            PointF p3 = new PointF(300, 446);

            while (Distance(p1, p2) >= 1)
            {
                DrawTriangle(g, blackPen, p1, p2, p3);
                PointF[] mids = FindMidpoints(p1, p2, p3);
                p1 = mids[0];
                p2 = mids[1];
                p3 = mids[2];
            }
        }

        // called repeatedly from OnPaint until the triangle shrinks below 1 pixel
        private PointF[] FindMidpoints(PointF p1, PointF p2, PointF p3)
        {
            PointF m1 = new PointF((p1.X + p2.X) / 2f, (p1.Y + p2.Y) / 2f);
            PointF m2 = new PointF((p2.X + p3.X) / 2f, (p2.Y + p3.Y) / 2f);
            PointF m3 = new PointF((p3.X + p1.X) / 2f, (p3.Y + p1.Y) / 2f);
            return new PointF[] { m1, m2, m3 };
        }

        private void DrawTriangle(Graphics g, Pen pen, PointF p1, PointF p2, PointF p3)
        {
            g.DrawLine(pen, p1, p2);
            g.DrawLine(pen, p2, p3);
            g.DrawLine(pen, p3, p1);
        }

        private double Distance(PointF a, PointF b)
        {
            double dx = b.X - a.X;
            double dy = b.Y - a.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static void Main()
        {
            Application.Run(new Triangles());
        }
    }
}