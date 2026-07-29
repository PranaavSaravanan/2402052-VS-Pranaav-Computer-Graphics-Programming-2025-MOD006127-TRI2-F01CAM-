using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GrafPack
{
    public partial class GrafPackForm : Form
    {
        // set by the Create sub-menu
        private enum CreateMode { None, Square, Circle, Triangle }
        private CreateMode createMode = CreateMode.None;
        private int clicksNeeded = 0;
        private List<Point> pendingClicks = new List<Point>();

        // The single source  for every shape on screen.
        // select / move / rotate / delete works off this list.
        private List<Shape> shapes = new List<Shape>();
        private Shape selectedShape = null;
        private int selectedIndex = -1;

        /* Square/Circle are created by drag rather than
        separate clicks, with a live dashed preview while the mouse moves. */
        private bool isDragging = false;
        private Point dragStart;
        private Point dragCurrent;
        private bool suppressNextClick = false;

        // dragging an existing shape directly, instead of typing dx,dy into the Move prompt
        private bool isDraggingShape = false;
        private Point lastDragPoint;

        public GrafPackForm()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;

            MainMenu mainMenu = new MainMenu();

            MenuItem createItem = new MenuItem();
            MenuItem squareItem = new MenuItem();
            MenuItem circleItem = new MenuItem();
            MenuItem triangleItem = new MenuItem();

            MenuItem selectItem = new MenuItem();

            MenuItem transformItem = new MenuItem();
            MenuItem moveItem = new MenuItem();
            MenuItem rotateItem = new MenuItem();
            MenuItem reflectHItem = new MenuItem();
            MenuItem reflectVItem = new MenuItem();

            MenuItem deleteItem = new MenuItem();
            MenuItem exitItem = new MenuItem();

            createItem.Text = "&Create";
            squareItem.Text = "&Square";
            circleItem.Text = "&Circle";
            triangleItem.Text = "&Triangle";
            selectItem.Text = "&Select";
            transformItem.Text = "&Transform";
            moveItem.Text = "&Move";
            rotateItem.Text = "&Rotate";
            reflectHItem.Text = "Reflect &Horizontal";
            reflectVItem.Text = "Reflect &Vertical";
            deleteItem.Text = "&Delete";
            exitItem.Text = "E&xit";

            mainMenu.MenuItems.Add(createItem);
            createItem.MenuItems.Add(squareItem);
            createItem.MenuItems.Add(circleItem);
            createItem.MenuItems.Add(triangleItem);

            mainMenu.MenuItems.Add(selectItem);

            mainMenu.MenuItems.Add(transformItem);
            transformItem.MenuItems.Add(moveItem);
            transformItem.MenuItems.Add(rotateItem);
            transformItem.MenuItems.Add(reflectHItem);
            transformItem.MenuItems.Add(reflectVItem);

            mainMenu.MenuItems.Add(deleteItem);
            mainMenu.MenuItems.Add(exitItem);

            squareItem.Click += (s, e) => BeginCreate(CreateMode.Square, 2);
            circleItem.Click += (s, e) => BeginCreate(CreateMode.Circle, 2);
            triangleItem.Click += (s, e) => BeginCreate(CreateMode.Triangle, 3);

            selectItem.Click += new EventHandler(this.cycleSelect);
            moveItem.Click += new EventHandler(this.moveSelected);
            rotateItem.Click += new EventHandler(this.rotateSelected);
            reflectHItem.Click += new EventHandler(this.reflectHorizontal);
            reflectVItem.Click += new EventHandler(this.reflectVertical);
            deleteItem.Click += new EventHandler(this.deleteSelected);
            exitItem.Click += (s, e) => this.Close();

            this.Menu = mainMenu;
            this.MouseDown += mouseDown;
            this.MouseMove += mouseMove;
            this.MouseUp += mouseUp;
            this.MouseClick += mouseClick;
            this.Paint += formPaint;
        }



        private void BeginCreate(CreateMode mode, int clicksRequired)
        {
            createMode = mode;
            clicksNeeded = clicksRequired;
            pendingClicks.Clear();

            if (mode == CreateMode.Triangle)
                MessageBox.Show("Click 3 points on the canvas to create a Triangle");
            else
                MessageBox.Show("Click and drag on the canvas to create a " + mode);
        }

        // Square/Circle start here on mouse , otherwise a click on any shape selects and starts dragging it
        private void mouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (createMode == CreateMode.Square || createMode == CreateMode.Circle)
            {
                dragStart = new Point(e.X, e.Y);
                dragCurrent = dragStart;
                isDragging = true;
                return;
            }

            if (createMode == CreateMode.None)
            {
                Point pt = new Point(e.X, e.Y);
                for (int i = 0; i < shapes.Count; i++)
                {
                    if (shapes[i].HitTestVertex(pt) || shapes[i].Contains(pt))
                    {
                        SelectShapeAt(i);
                        isDraggingShape = true;
                        lastDragPoint = pt;
                        break;
                    }
                }
            }
        }

        // updates live as the mouse moves, so formPaint can draw a preview / the shape follows the cursor
        private void mouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                dragCurrent = new Point(e.X, e.Y);
                this.Invalidate();
                return;
            }

            if (isDraggingShape)
            {
                Point current = new Point(e.X, e.Y);
                selectedShape.Move(current.X - lastDragPoint.X, current.Y - lastDragPoint.Y);
                lastDragPoint = current;
                this.Invalidate();
            }
        }

        //  finalizes into a real Shape on release, or ends a shape being dragged
        private void mouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (isDragging)
            {
                Shape newShape = createMode == CreateMode.Square
                    ? new Square(dragStart, dragCurrent)
                    : (Shape)new Circle(dragStart, dragCurrent);

                shapes.Add(newShape);
                isDragging = false;
                createMode = CreateMode.None;

                /* MouseClick fires after MouseUp for the same click - without
                this flag it would immediately re-select the shape we just drew,
                 since the release is exactly on one of its vertices. */
                suppressNextClick = true;
                this.Invalidate();
                return;
            }

            if (isDraggingShape)
            {
                isDraggingShape = false;
                suppressNextClick = true;
            }
        }

        private void mouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (suppressNextClick)
            {
                suppressNextClick = false;
                return;
            }

            /* Triangle keeps the click approach dragging
             doesn't map naturally onto three points the way it does for a
            square or a circle. */
            if (createMode == CreateMode.Triangle)
            {
                pendingClicks.Add(new Point(e.X, e.Y));
                if (pendingClicks.Count == clicksNeeded)
                {
                    shapes.Add(new Triangle(pendingClicks[0], pendingClicks[1], pendingClicks[2]));
                    createMode = CreateMode.None;
                    pendingClicks.Clear();
                    this.Invalidate();
                }
                return;
            }

            // treats the click as select by vertex, then fall back to anywhere inside the shape

            if (createMode == CreateMode.None)
            {
                for (int i = 0; i < shapes.Count; i++)
                {
                    if (shapes[i].HitTestVertex(new Point(e.X, e.Y)))
                    {
                        SelectShapeAt(i);
                        return;
                    }
                }
                for (int i = 0; i < shapes.Count; i++)
                {
                    if (shapes[i].Contains(new Point(e.X, e.Y)))
                    {
                        SelectShapeAt(i);
                        return;
                    }
                }
            }
        }

        // Selection 

        private void cycleSelect(object sender, EventArgs e)
        {
            if (shapes.Count == 0)
            {
                MessageBox.Show("No shapes to select yet.");
                return;
            }
            int next = (selectedIndex + 1) % shapes.Count;
            SelectShapeAt(next);
        }

        private void SelectShapeAt(int index)
        {
            foreach (Shape s in shapes) s.IsSelected = false;
            selectedIndex = index;
            selectedShape = shapes[index];
            selectedShape.IsSelected = true;
            this.Invalidate();
        }

        // Transform and Delete 

        private void moveSelected(object sender, EventArgs e)
        {
            if (selectedShape == null) { MessageBox.Show("Select a shape first."); return; }

            string input = Prompt.ShowDialog("Enter movement as dx,dy (e.g. 30,-15):", "Move Shape");
            string[] parts = input.Split(',');
            if (parts.Length == 2
                && int.TryParse(parts[0].Trim(), out int dx)
                && int.TryParse(parts[1].Trim(), out int dy))
            {
                selectedShape.Move(dx, dy);
                this.Invalidate();
            }
        }

        private void rotateSelected(object sender, EventArgs e)
        {
            if (selectedShape == null) { MessageBox.Show("Select a shape first."); return; }

            string input = Prompt.ShowDialog("Enter rotation angle in degrees (e.g. 45):", "Rotate Shape");
            if (double.TryParse(input.Trim(), out double angle))
            {
                selectedShape.Rotate(angle);
                this.Invalidate();
            }
        }

        private void reflectHorizontal(object sender, EventArgs e)
        {
            if (selectedShape == null) { MessageBox.Show("Select a shape first."); return; }
            selectedShape.Reflect(true);
            this.Invalidate();
        }

        private void reflectVertical(object sender, EventArgs e)
        {
            if (selectedShape == null) { MessageBox.Show("Select a shape first."); return; }
            selectedShape.Reflect(false);
            this.Invalidate();
        }

        private void deleteSelected(object sender, EventArgs e)
        {
            if (selectedShape == null) { MessageBox.Show("Select a shape first."); return; }
            shapes.Remove(selectedShape);
            selectedShape = null;
            selectedIndex = -1;
            this.Invalidate();
        }

        //Drawing 

        private void formPaint(object sender, PaintEventArgs e)
        {
            Pen normalPen = new Pen(Color.Black, 1);
            Pen selectedPen = new Pen(Color.Red, 2);
            foreach (Shape s in shapes)
            {
                s.Draw(e.Graphics, s.IsSelected ? selectedPen : normalPen);
            }

            if (isDragging)
            {
                Pen previewPen = new Pen(Color.Gray, 1) { DashStyle = DashStyle.Dash };
                Shape preview = createMode == CreateMode.Square
                    ? new Square(dragStart, dragCurrent)
                    : (Shape)new Circle(dragStart, dragCurrent);
                preview.Draw(e.Graphics, previewPen);
            }
        }
    }


    abstract class Shape
    {
        protected List<Point> vertices = new List<Point>();
        public bool IsSelected { get; set; } = false;

        public virtual void Draw(Graphics g, Pen pen)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                Point p1 = vertices[i];
                Point p2 = vertices[(i + 1) % vertices.Count];
                g.DrawLine(pen, p1.X, p1.Y, p2.X, p2.Y);
            }
        }

        public virtual void Move(int dx, int dy)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] = new Point(vertices[i].X + dx, vertices[i].Y + dy);
            }
        }


        public virtual void Rotate(double angleDegrees)
        {
            Point centre = GetCentroid();
            double radians = angleDegrees * Math.PI / 180.0;
            double cosA = Math.Cos(radians);
            double sinA = Math.Sin(radians);

            for (int i = 0; i < vertices.Count; i++)
            {
                double xShift = vertices[i].X - centre.X;
                double yShift = vertices[i].Y - centre.Y;

                double xNew = xShift * cosA - yShift * sinA;
                double yNew = xShift * sinA + yShift * cosA;

                vertices[i] = new Point((int)Math.Round(centre.X + xNew), (int)Math.Round(centre.Y + yNew));
            }
        }

        // mirrors every vertex across the shape's own centroid
        public virtual void Reflect(bool horizontal)
        {
            Point centre = GetCentroid();

            for (int i = 0; i < vertices.Count; i++)
            {
                if (horizontal)
                {
                    int xNew = centre.X - (vertices[i].X - centre.X);
                    vertices[i] = new Point(xNew, vertices[i].Y);
                }
                else
                {
                    int yNew = centre.Y - (vertices[i].Y - centre.Y);
                    vertices[i] = new Point(vertices[i].X, yNew);
                }
            }
        }

        public Point GetCentroid()
        {
            long sumX = 0, sumY = 0;
            foreach (Point p in vertices) { sumX += p.X; sumY += p.Y; }
            return new Point((int)(sumX / vertices.Count), (int)(sumY / vertices.Count));
        }

        // Point-in-polygon test (ray casting)
        public bool Contains(Point testPt)
        {
            bool inside = false;
            int n = vertices.Count;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                Point vi = vertices[i];
                Point vj = vertices[j];
                if (((vi.Y > testPt.Y) != (vj.Y > testPt.Y)) &&
                    (testPt.X < (vj.X - vi.X) * (double)(testPt.Y - vi.Y) / (vj.Y - vi.Y) + vi.X))
                {
                    inside = !inside;
                }
            }
            return inside;
        }

        // Selection by clicking near a vertex (spec's alternative select method)
        public bool HitTestVertex(Point testPt)
        {
            const int tolerance = 8;
            foreach (Point v in vertices)
            {
                double dist = Math.Sqrt(Math.Pow(v.X - testPt.X, 2) + Math.Pow(v.Y - testPt.Y, 2));
                if (dist <= tolerance) return true;
            }
            return false;
        }
    }

    class Square : Shape
    {
        // Square defined by two opposite corners - here it just populates the shared vertex list

        public Square(Point keyPt, Point oppPt)
        {
            double xDiff = oppPt.X - keyPt.X;
            double yDiff = oppPt.Y - keyPt.Y;
            double xMid = (oppPt.X + keyPt.X) / 2.0;
            double yMid = (oppPt.Y + keyPt.Y) / 2.0;

            vertices.Add(keyPt);
            vertices.Add(new Point((int)(xMid + yDiff / 2), (int)(yMid - xDiff / 2)));
            vertices.Add(oppPt);
            vertices.Add(new Point((int)(xMid - yDiff / 2), (int)(yMid + xDiff / 2)));
        }
    }

    class Triangle : Shape
    {
        // Triangle defined by three clicked points 
        public Triangle(Point p1, Point p2, Point p3)
        {
            vertices.Add(p1);
            vertices.Add(p2);
            vertices.Add(p3);
        }
    }

    class Circle : Shape
    {
        /* Circle defined by a centre point and a point on its circumference
        (radius = distance between them)*/
        private const int Segments = 60;

        public Circle(Point centre, Point onCircumference)
        {
            double dx = onCircumference.X - centre.X;
            double dy = onCircumference.Y - centre.Y;
            double radius = Math.Sqrt(dx * dx + dy * dy);

            for (int i = 0; i < Segments; i++)
            {
                double theta = 2 * Math.PI * i / Segments;
                int x = (int)(centre.X + radius * Math.Cos(theta));
                int y = (int)(centre.Y + radius * Math.Sin(theta));
                vertices.Add(new Point(x, y));
            }
        }
    }


    static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 320,
                Height = 150,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };
            Label textLabel = new Label() { Left = 20, Top = 20, Text = text, Width = 270, Height = 20 };
            TextBox inputBox = new TextBox() { Left = 20, Top = 45, Width = 260 };
            Button confirmation = new Button()
            {
                Text = "OK",
                Left = 200,
                Width = 80,
                Top = 75,
                DialogResult = DialogResult.OK
            };
            confirmation.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? inputBox.Text : "";
        }
    }


    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new GrafPackForm());
        }
    }
}