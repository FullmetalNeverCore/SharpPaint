using System.Drawing.Imaging;
using System.Drawing.Printing;

namespace WinFormsApp8
{
    public partial class SharpPaint : Form
    {
        Bitmap bitm;
        Bitmap bitm2;
        Color new_color;
        PrintDialog printDialog = new PrintDialog();
        PrintDocument printDocument = new PrintDocument();
        ColorDialog colorDialog = new ColorDialog();
        SaveFileDialog save_dia = new SaveFileDialog();
        OpenFileDialog open_dia = new OpenFileDialog();
        private Point startPoint = new Point();
        private Point endPoint = new Point();
        public static bool DrawLine = true;
        int SprayRadius = 15;
        private Rectangle rectangle = new Rectangle();
        Graphics graphics;
        private Random rand = new Random();
        Graphics phantom_graphics;
        bool p = false;
        Point p2, p1;
        int thickness = 1;
        Pen pen = new Pen(Color.Black, 1);
        EnumaratedModes mode = EnumaratedModes.Pen;
        int test = 0;
        int x, y, gx, gy, zx, zy;
        List<Bitmap> bitmaps = new List<Bitmap>();
        public SharpPaint()
        {
            InitializeComponent();
            this.Text = "SharpPaint";
            this.AutoScroll = true;
            this.Controls.Add(pictureBox1);
            bitm = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            bitm2 = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            graphic_update(bitm);
        }

        //point_set method created to set and return mouse position on color pallete image.
        static Point point_Set(PictureBox picb, Point pnt) { return new Point((int)(pnt.X * 1f * picb.Width / picb.Width), (int)(pnt.Y * 1f * picb.Height / picb.Height)); }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            zx = e.X;
            zy = e.Y;
            startPoint.X = zx;
            startPoint.Y = zy;
            endPoint = startPoint;
            p1 = e.Location;
            p = true;
            Bitmap clone = bitm.Clone(new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height), bitm.PixelFormat);
            bitmaps.Add(bitm);
            switch (mode)
            {
                case EnumaratedModes.Line:
                    endPoint = new Point(e.X, e.Y);
                    textBox3.Text = "Mouse";
                    ControlPaint.DrawReversibleLine(pictureBox1.PointToScreen(startPoint), pictureBox1.PointToScreen(endPoint), Color.Black);
                    ControlPaint.DrawReversibleLine(pictureBox1.PointToScreen(startPoint), pictureBox1.PointToScreen(endPoint), Color.Black);
                    break;
            }
        }

        private async void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            pen.Width = thickness;
            if (p)
            {
                switch (mode)
                {
                    case EnumaratedModes.Pen:
                        p2 = e.Location; 
                        graphics.DrawLine(pen, p2, p1);
                        p1 = p2;
                        break;
                    case EnumaratedModes.HyperRectangle:
                        graphics.DrawRectangle(pen, zx, zy, gx, gy);
                        break;
                    case EnumaratedModes.HyperEllipse:
                        graphics.DrawEllipse(pen, zx, zy, gx, gy);
                        break;
                    case EnumaratedModes.Raycaster:
                        p2 = e.Location;
                        graphics.DrawLine(pen, p1, p2);
                        p1 = p1;
                        break;
                    case EnumaratedModes.Eraser:
                        p2 = e.Location;
                        pen.Color = (mode == EnumaratedModes.Eraser) ? Color.White : Color.Black;
                        graphics.DrawLine(pen, p2, p1);
                        p1 = p2;
                        pen.Color = Color.Black;
                        break;
                    case EnumaratedModes.Line:
                        endPoint = new Point(e.X, e.Y);
                        ControlPaint.DrawReversibleLine(pictureBox1.PointToScreen(startPoint), pictureBox1.PointToScreen(endPoint), Color.Black);
                        ControlPaint.DrawReversibleLine(pictureBox1.PointToScreen(startPoint), pictureBox1.PointToScreen(endPoint), Color.Black);
                        break;
                    case EnumaratedModes.Spray:
                        for (int i = 0; i < 100; ++i)
                        {
                            double theta = rand.NextDouble() * (Math.PI * 2);
                            double r = rand.NextDouble() * SprayRadius;
                            double x = e.X + Math.Cos(theta) * r;
                            double y = e.Y + Math.Sin(theta) * r;
                            graphics.DrawEllipse(pen, new Rectangle((int)x - 1, (int)y - 1, 1, 1));
                        }
                        break;
                    case EnumaratedModes.Rectangle:
                        gx = x - zx;
                        gy = y - zy;
                        Rectangle rect = new Rectangle(zx, zy, gx, gy);
                        ControlPaint.DrawReversibleFrame(pictureBox1.RectangleToScreen(rect), Color.Black, FrameStyle.Dashed);
                        break;
                }
            }
            x = e.X;
            y = e.Y;
            gx = e.X - zx;
            gy = e.Y - zy;
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            pen.Width = thickness;
            gx = x - zx;
            gy = y - zy;
            textBox2.Text = zx.ToString();
            textBox3.Text = zy.ToString();
            endPoint = new Point(e.X, e.Y);
            p = false;
            switch (mode)
            {
                case EnumaratedModes.Rectangle:
                    if ((gx < 0) && (gy > 0))
                    {
                        textBox3.Text = "Scen1";
                        graphics.DrawRectangle(pen, zx, zy, -gx, gy);
                    }
                    else if((gy < 0) && (gx > 0))
                    {
                        textBox3.Text = "Scen2";
                        graphics.DrawRectangle(pen, zx, zy, gx, -gy);
                    }
                    else if((gx < 0) && (gy < 0))
                    {
                        textBox3.Text = "Scen3";
                        graphics.DrawRectangle(pen, zx, zy, -gx, -gy);
                    }
                    else
                    {
                        textBox3.Text = "Scen4";
                        graphics.DrawRectangle(pen, zx, zy, gx, gy);
                    }
                    break;
                case EnumaratedModes.Ellipse:
                    graphics.DrawEllipse(pen, zx, zy, gx, gy);
                    break;
                case EnumaratedModes.Line:
                    graphics.DrawLine(pen, zx, zy, x, y);
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mode = EnumaratedModes.Pen;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            Point pt = point_Set(pictureBox2, e.Location);
            picture_Color.BackColor = ((Bitmap)pictureBox2.Image).GetPixel(pt.X, pt.Y);
            pen.Color = picture_Color.BackColor;

        }

        private void picture_Color_Click(object sender, EventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bitm = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphic_update(bitm);
        }

        private void graphic_update(Bitmap bitm,int bt = 0)
        {
            graphics = Graphics.FromImage(bitm);
            if(bt != 1) graphics.Clear(Color.White);
            pictureBox1.Refresh();
            pictureBox1.Image = bitm;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            mode = EnumaratedModes.Ellipse;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            mode = EnumaratedModes.Raycaster;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            mode = EnumaratedModes.HyperRectangle;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            mode = EnumaratedModes.HyperEllipse;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            mode = EnumaratedModes.Eraser;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                thickness = Convert.ToInt32(textBox1.Text);
            }
            catch
            {
                thickness = 1;
                textBox1.Clear();
            };
        }

        private void button9_Click(object sender, EventArgs e)
        {
            mode = EnumaratedModes.Line;
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap clone = bitm.Clone(new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height), bitm.PixelFormat);
            printDocument.PrintPage += bitmap_printing;
            printDialog.Document = printDocument;
            switch (printDialog.ShowDialog())
            {
                case DialogResult.OK:
                    printDocument.Print();
                    break;
            }
        }
        private void bitmap_printing(object sender,PrintPageEventArgs e)
        {
            pictureBox1.DrawToBitmap(bitm,new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));
            e.Graphics.DrawImage(bitm, 0, 0);
            bitm.Dispose();

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            mode = EnumaratedModes.Spray;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            try { SprayRadius = Convert.ToInt32(textBox4.Text); }
            catch { SprayRadius = 15; }
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            switch (mode)
            {
                case EnumaratedModes.Line:
                    ControlPaint.DrawReversibleLine(pictureBox1.PointToScreen(startPoint), pictureBox1.PointToScreen(endPoint), Color.Black);
                    ControlPaint.DrawReversibleLine(pictureBox1.PointToScreen(startPoint), pictureBox1.PointToScreen(endPoint), Color.Black);
                    break;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            open_dia.Filter = "Image(*.png)|*.png|(*.*|*.*";
            switch (open_dia.ShowDialog())
            {
                case DialogResult.OK:
                    bitm = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                    bitm = new Bitmap(Image.FromFile(open_dia.FileName));
                    graphic_update(bitm,1);
                    break;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save_dia.Filter = "Image(*.png)|*.png|(*.*|*.*";
            switch (save_dia.ShowDialog())
            {
                case DialogResult.OK:
                    Bitmap clone = bitm.Clone(new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height), bitm.PixelFormat);
                    clone.Save(save_dia.FileName,ImageFormat.Png);
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            colorDialog.ShowDialog();
            picture_Color.BackColor = colorDialog.Color;
            pen.Color = colorDialog.Color;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mode = EnumaratedModes.Rectangle;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}