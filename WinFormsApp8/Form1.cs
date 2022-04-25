using System.Drawing.Imaging;
using System.Drawing.Printing;

namespace WinFormsApp8
{
    public partial class Form1 : Form
    {
        Bitmap bitm;
        Color new_color;
        PrintDialog printDialog = new PrintDialog(); 
        PrintDocument printDocument = new PrintDocument();  
        ColorDialog colorDialog = new ColorDialog();
        SaveFileDialog save_dia = new SaveFileDialog();
        OpenFileDialog open_dia = new OpenFileDialog();
        Graphics graphics;
        bool p = false;
        Point p2, p1;
        int thickness = 1;
        Pen pen = new Pen(Color.Black, 1);
        int mode = 1;
        int x, y, gx, gy, zx, zy;
        public Form1()
        {
            WindowState = FormWindowState.Maximized;
            InitializeComponent();
            bitm = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            graphic_update(bitm);
        }

        //point_set method created to set and return mouse position on color pallete image.
        static Point point_Set(PictureBox picb, Point pnt) {return new Point((int)(pnt.X * 1f * picb.Width / picb.Width), (int)(pnt.Y * 1f * picb.Height / picb.Height));} 

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            zx = e.X;
            zy = e.Y;
            p1 = e.Location;
            p = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            pen.Width = thickness;
            if (p)
            {
                switch (mode)
                {
                    case 1:
                        p2 = e.Location;
                        graphics.DrawLine(pen, p2, p1);
                        p1 = p2;
                        break;
                    case 5:
                        graphics.DrawRectangle(pen, zx, zy, gx, gy);
                        break;
                    case 6:
                        graphics.DrawEllipse(pen, zx, zy, gx, gy);
                        break;
                    case 7:
                        p2 = e.Location;
                        graphics.DrawLine(pen, p1,p2);
                        p1 = p1;
                        break;
                    case 8:
                        p2 = e.Location;
                        pen.Color = Color.White;
                        graphics.DrawLine(pen, p2, p1);
                        p1 = p2;
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
            p = false;
            switch (mode)
            {
                case 2:
                    graphics.DrawRectangle(pen, zx, zy, gx, gy);
                    break;
                case 3:
                    graphics.DrawEllipse(pen, zx, zy, gx, gy);
                    break;
                case 9:
                    graphics.DrawLine(pen, zx, zy, x, y);
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mode = 1;
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
            mode = 3;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            mode = 7;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            mode = 5;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            mode = 6;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            mode = 8;
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
            mode = 9;
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
            mode = 2;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}