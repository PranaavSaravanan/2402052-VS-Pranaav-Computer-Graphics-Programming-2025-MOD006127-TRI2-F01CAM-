using System;
using System.Drawing;
using System.Windows.Forms;

namespace LineAndPoints
{
    public partial class LineAndPointsForm : Form
    {
        Point[] points;
        int[] lineStart;
        int[] lineEnd;

        public LineAndPointsForm()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.Width = 550;
            this.Height = 350;
            this.BackColor = Color.White;

            points = new Point[]
            {
                new Point(30, 30),
                new Point(450, 30),
                new Point(30, 250),
                new Point(450, 250),
                new Point(280, 30),
                new Point(280, 170),
                new Point(280, 110),
                new Point(450, 110),
                new Point(30, 170),
                new Point(450, 170)
            };
            lineStart = new int[] { 0, 0, 1, 9, 2, 4, 6, 8 };
            lineEnd = new int[] { 1, 2, 7, 3, 3, 5, 7, 9 };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen blackPen = new Pen(Color.Black);

            for (int i = 0; i < lineStart.Length; i++)
            {
                g.DrawLine(blackPen, points[lineStart[i]], points[lineEnd[i]]);
            }
        }
    }
}