using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace PCD
{
    public partial class Form1 : Form
    {
        //Variables
        private Image image;
        private Bitmap gambar;
        private NumericUpDown sudut;
        //variable to imgRGB
        public Image imgRGB;
        public Image imgRGBResult;
        //public static class Utilities
        private Bitmap filteringImage = null;
        private bool _selecting;
        private Rectangle cropSize;
        private bool Clicked = false;
        private int[] histo;
        private byte[] hasil;
        //private IColorQuantizer quantizer;

        public Form1()
        {
            InitializeComponent();
        }

        #region PICTURE BOX SET GET
        public PictureBox original2
        {
            get { return original; }
            set { original = value; }
        }

        public PictureBox result2
        {
            get { return result; }
            set { result = value; }
        }
        #endregion

        private void zoomToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void fastFourierTransformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FastFourierTransform fft = new FastFourierTransform(new Bitmap(original.Image));
            fft.Show();
        }

        #region Load Image
        private void LoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.Title = "Pilih Gambar";
            Bitmap image;

            if (f.ShowDialog() == DialogResult.OK)
            {
                string ImagePath = f.FileName.ToString();
                original.ImageLocation = ImagePath;
                imgRGB = original.Image;
            }
        }
        #endregion

        #region SAVE IMAGE
        private void SaveImage_Click(object sender, EventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.AddExtension = true;
            if (sv.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                switch (Path.GetExtension(sv.FileName).ToUpper())
                {
                    case ".BMP":
                        result.Image.Save(sv.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case ".JPG":
                        result.Image.Save(sv.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".PNG":
                        result.Image.Save(sv.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        private void ExitProgram_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region GRAYSCALE
        private void Grayscale_Click(object sender, EventArgs e)
        {
            

            if (original.Image != null)
            {
                Image img;
                img = original.Image;
                Bitmap pict = new Bitmap(img);
                try
                {
                    //Bitmap b;
                    CovertToGray(pict);
                    result.Image = pict;
                }
                catch { }
            }
            else
            {
                MessageBox.Show("Masukan Gambar Terlebih Dahulu!","Incomplete Procedure Detected",MessageBoxButtons.OK);
            }
            
        }

        public static bool CovertToGray(Bitmap b)
        {
            // GDI+ return format is BGR, NOT RGB. 
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = bmData.Stride; // bytes in a row 3*b.Width
            System.IntPtr Scan0 = bmData.Scan0;
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte red, green, blue;
                int nOffset = stride - b.Width * 3;
                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        blue = p[0];
                        green = p[1];
                        red = p[2];
                        p[0] = p[1] = p[2] = (byte)(.299 * red
                            + .587 * green + .114 * blue);
                        p += 3;
                    }
                    p += nOffset;
                }
            }
            b.UnlockBits(bmData);
            return true;
        }
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            
            panel1.AutoScroll = true;
            panel2.AutoScroll = true;
            original.SizeMode = PictureBoxSizeMode.AutoSize;
            result.SizeMode = PictureBoxSizeMode.AutoSize;
        }

        #region Sampling
        private void Sampling_Click(object sender, EventArgs e)
        {
            if (original.Image != null)
            {
                Image img = original.Image;
                Bitmap bm = new Bitmap(img);
                result.Image = Sample(bm, new Rectangle(0, 0, bm.Width, bm.Height), 10);
            }
            else
            {
                MessageBox.Show("Harap Input gambar terlebih dahulu!","Incomplete Procedure Detected",MessageBoxButtons.OK);
            }
        }

        private static Bitmap Sample(Bitmap image, Rectangle rectangle, Int32 samplingSize)
        {
            Bitmap pixelated = new System.Drawing.Bitmap(image.Width, image.Height);

            
            using (Graphics graphics = System.Drawing.Graphics.FromImage(pixelated)) graphics.DrawImage(image, new System.Drawing.Rectangle(0, 0, image.Width, image.Height),new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);


            // look at every pixel in the rectangle while making sure we're within the image bounds
            for (Int32 xx = rectangle.X; xx < rectangle.X + rectangle.Width && xx < image.Width; xx += samplingSize)
            {
                for (Int32 yy = rectangle.Y; yy < rectangle.Y + rectangle.Height && yy < image.Height; yy += samplingSize)
                {
                    Int32 offsetX = samplingSize / 2;
                    Int32 offsetY = samplingSize / 2;

                    
                    while (xx + offsetX >= image.Width) offsetX--;
                    while (yy + offsetY >= image.Height) offsetY--;

                    Color pixel = pixelated.GetPixel(xx + offsetX, yy + offsetY);

                    for (Int32 x = xx; x < xx + samplingSize && x < image.Width; x++)
                        for (Int32 y = yy; y < yy + samplingSize && y < image.Height; y++)
                            pixelated.SetPixel(x, y, pixel);
                }
            }

            return pixelated;
        }
        #endregion


        #region Quantization 1 bit
        private void bitToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Bitmap img = (Bitmap)original.Image;
            //Bitmap bm;


            if (img.PixelFormat != PixelFormat.Format32bppPArgb)
            {

                Bitmap temp = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppPArgb);

                Graphics g = Graphics.FromImage(temp);

                g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);

                img.Dispose();

                g.Dispose();

                img = temp;

            }

            this.original.Image = img;



            BitmapData bmdo = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);

            Bitmap bm = new Bitmap(this.original.Image.Width, this.original.Image.Height, PixelFormat.Format1bppIndexed);

            BitmapData bmdn = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);



            //for diagnostics

            DateTime dt = DateTime.Now;



            //scan through the pixels Y by X

            int x, y;

            for (y = 0; y < img.Height; y++)
            {

                for (x = 0; x < img.Width; x++)
                {

                    //generate the address of the colour pixel

                    int index = y * bmdo.Stride + (x * 4);

                    //check its brightness

                    if (Color.FromArgb(Marshal.ReadByte(bmdo.Scan0, index + 2),

                                    Marshal.ReadByte(bmdo.Scan0, index + 1),

                                    Marshal.ReadByte(bmdo.Scan0, index)).GetBrightness() > 0.5f)

                        this.SetIndexedPixel(x, y, bmdn, true); //set it if its bright.

                }

            }



            //tidy up

            bm.UnlockBits(bmdn);

            img.UnlockBits(bmdo);



            //show the time taken to do the conversion

            TimeSpan ts = dt - DateTime.Now;

            System.Diagnostics.Trace.WriteLine("Conversion time was:" + ts.ToString());



            //display the 1bpp image.

            this.result.Image = bm;
        }

        protected void SetIndexedPixel(int x, int y, BitmapData bmd, bool pixel)
        {

            int index = y * bmd.Stride + (x >> 3);

            byte p = Marshal.ReadByte(bmd.Scan0, index);

            byte mask = (byte)(0x80 >> (x & 0x7));

            if (pixel)

                p |= mask;

            else

                p &= (byte)(mask ^ 0xff);

            Marshal.WriteByte(bmd.Scan0, index, p);

        }
        #endregion

        private void reUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (result.Image != null)
            {
                original.Image = result.Image;
                result.Image = null;
            }
            else
            {
                MessageBox.Show("Belum ada Gambar yang diolah", "Inclomplete Procedure Detected", MessageBoxButtons.OK);
            }
        }

        #region RGB Filtering//BULLSHIT CODE
        private void RGB_Click(object sender, EventArgs e)
        {
            //RGB_Filtering rgb = new RGB_Filtering();
            //rgb.Visible = true;
            //if (rgb.pressed == true)
            //{
            //    Image img;
            //    img = original.Image;
            //    Decimal num1 = rgb.numeric1;
            //    Decimal num2 = rgb.numeric2;
            //    String wrn = rgb.warnaPilihan;

            //    Bitmap pict = new Bitmap(img);
            //    //result.Image = img;
            //    int x, y;

            //    for (x = 0; x < img.Width; x++)
            //    {
            //        for (y = 0; y < img.Height; y++)
            //        {
            //            Color pixelColor = pict.GetPixel(x, y);
            //            if (wrn == "Red")
            //            {
            //                Color newColor = Color.FromArgb(pixelColor.R, Convert.ToInt32(num1), Convert.ToInt32(num2));
            //                pict.SetPixel(x, y, newColor);
            //            }
            //            else if (wrn == "Green")
            //            {
            //                Color newColor = Color.FromArgb(Convert.ToInt32(num1), pixelColor.G, Convert.ToInt32(num2));
            //                pict.SetPixel(x, y, newColor);
            //            }
            //            else
            //            {
            //                Color newColor = Color.FromArgb(Convert.ToInt32(num1), Convert.ToInt32(num2), pixelColor.B);
            //                pict.SetPixel(x, y, newColor);
            //            }


            //            //pict.SetPixel(x, y, newColor);
            //        }
            //    }

            //    result.Image = pict;
            //}

        }
        #endregion

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {
           
        }

        #region RGBRed gagal
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //int green = Convert.ToInt32(RedGreen.Text);
            //Image img = original.Image;
            //Bitmap pict = new Bitmap(img);
            //int x, y;
            //if (RedGreen.Focused && e.KeyChar)
            //{
            //    for (x = 0; x < img.Width; x++)
            //    {
            //        for (y = 0; y < img.Height; y++)
            //        {
            //            Color pixelColor = pict.GetPixel(x, y);

            //            Color newColor = Color.FromArgb(pixelColor.R, Convert.ToInt32(RedGreen.Text), Convert.ToInt32(RedGreen.Text));
            //            pict.SetPixel(x, y, newColor);

            //        }
            //    }
            //}

            //result.Image = pict;
        }
        #endregion // 




        #region RGB Red
        private void RedGreen_KeyDown(object sender, KeyEventArgs e)
        {
            //int green = Convert.ToInt32(RedGreen.Text);
            Image img = original.Image;
            Bitmap pict = new Bitmap(img);
            int x, y;
            if (e.KeyCode == Keys.Enter)
            {
                for (x = 0; x < img.Width; x++)
                {
                    for (y = 0; y < img.Height; y++)
                    {
                        Color pixelColor = pict.GetPixel(x, y);

                        Color newColor = Color.FromArgb(pixelColor.R, Convert.ToInt32(RedGreen.Text), Convert.ToInt32(RedBlue.Text));
                        pict.SetPixel(x, y, newColor);

                    }
                }
            }

            result.Image = pict;
        }

        private void RedBlue_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void RedBlue_KeyDown(object sender, KeyEventArgs e)
        {
            Image img = original.Image;
            Bitmap pict = new Bitmap(img);
            int x, y;
            if (e.KeyCode == Keys.Enter)
            {
                for (x = 0; x < img.Width; x++)
                {
                    for (y = 0; y < img.Height; y++)
                    {
                        Color pixelColor = pict.GetPixel(x, y);

                        Color newColor = Color.FromArgb(pixelColor.R, Convert.ToInt32(RedGreen.Text), Convert.ToInt32(RedBlue.Text));
                        pict.SetPixel(x, y, newColor);

                    }
                }
            }

            result.Image = pict;
        }
        #endregion

        #region RGB Green
        private void GreenRed_KeyDown(object sender, KeyEventArgs e)
        {
            Image img = original.Image;
            Bitmap pict = new Bitmap(img);
            int x, y;
            if (e.KeyCode == Keys.Enter)
            {
                for (x = 0; x < img.Width; x++)
                {
                    for (y = 0; y < img.Height; y++)
                    {
                        Color pixelColor = pict.GetPixel(x, y);

                        Color newColor = Color.FromArgb(Convert.ToInt32(GreenRed.Text),pixelColor.G, Convert.ToInt32(GreenBlue.Text));
                        pict.SetPixel(x, y, newColor);

                    }
                }
            }

            result.Image = pict;
        }

        private void GreenBlue_KeyDown(object sender, KeyEventArgs e)
        {
            Image img = original.Image;
            Bitmap pict = new Bitmap(img);
            int x, y;
            if (e.KeyCode == Keys.Enter)
            {
                for (x = 0; x < img.Width; x++)
                {
                    for (y = 0; y < img.Height; y++)
                    {
                        Color pixelColor = pict.GetPixel(x, y);

                        Color newColor = Color.FromArgb(Convert.ToInt32(GreenRed.Text), pixelColor.G, Convert.ToInt32(GreenBlue.Text));
                        pict.SetPixel(x, y, newColor);

                    }
                }
            }

            result.Image = pict;
        }
        #endregion

        #region RGB Blue
        private void BlueRed_KeyDown(object sender, KeyEventArgs e)
        {
            Image img = original.Image;
            Bitmap pict = new Bitmap(img);
            int x, y;
            if (e.KeyCode == Keys.Enter)
            {
                for (x = 0; x < img.Width; x++)
                {
                    for (y = 0; y < img.Height; y++)
                    {
                        Color pixelColor = pict.GetPixel(x, y);

                        Color newColor = Color.FromArgb(Convert.ToInt32(BlueRed.Text), Convert.ToInt32(BlueGreen.Text), pixelColor.B);
                        pict.SetPixel(x, y, newColor);

                    }
                }
            }

            result.Image = pict;
        }

        private void BlueGreen_KeyDown(object sender, KeyEventArgs e)
        {
            Image img = original.Image;
            Bitmap pict = new Bitmap(img);
            int x, y;
            if (e.KeyCode == Keys.Enter)
            {
                for (x = 0; x < img.Width; x++)
                {
                    for (y = 0; y < img.Height; y++)
                    {
                        Color pixelColor = pict.GetPixel(x, y);

                        Color newColor = Color.FromArgb(Convert.ToInt32(BlueRed.Text), Convert.ToInt32(BlueGreen.Text), pixelColor.B);
                        pict.SetPixel(x, y, newColor);

                    }
                }
            }

            result.Image = pict;
        }
        #endregion 

        #region ZOOM
        private void ZoomInEnter_KeyDown(object sender, KeyEventArgs e)
        {
            Image img;
            img = original.Image;

            Bitmap pict = new Bitmap(img);

            //Double zoomin= Convert.ToDouble(ZoomInEnter.Text);
            if (e.KeyCode == Keys.Enter)
            {
                if (pict != null)
                {
                    Bitmap bmp = new Bitmap(img, Convert.ToInt32(original.Width * Convert.ToDouble(ZoomInEnter.Text)), Convert.ToInt32(original.Height * Convert.ToDouble(ZoomInEnter.Text)));
                    Graphics g = Graphics.FromImage(bmp);
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    result.Image = bmp;
                    img = result.Image;

                }
                else
                {
                    MessageBox.Show("Load gambar terlebih dahulu","Incomplete Procedure Detected",MessageBoxButtons.OK);
                }
            }
            
        }

        private void ZoomOutEnter_KeyDown(object sender, KeyEventArgs e)
        {
            Image img;
            img = original.Image;

            Bitmap pict = new Bitmap(img);

            //Double zoomin= Convert.ToDouble(ZoomInEnter.Text);
            if (e.KeyCode == Keys.Enter)
            {
                if (pict != null)
                {
                    Bitmap bmp = new Bitmap(img, Convert.ToInt32(original.Width / Convert.ToDouble(ZoomOutEnter.Text)), Convert.ToInt32(original.Height / Convert.ToDouble(ZoomOutEnter.Text)));
                    Graphics g = Graphics.FromImage(bmp);
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    result.Image = bmp;
                    img = result.Image;

                }
                else
                {
                    MessageBox.Show("Load gambar terlebih dahulu", "Incomplete Procedure Detected", MessageBoxButtons.OK);
                }
            }
        }
        #endregion

        #region ROTATE
        private void rightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image img;
            img = original.Image;
            Bitmap pict = new Bitmap(img);

            if (original.Image != null)
            {
                pict.RotateFlip(RotateFlipType.Rotate90FlipNone);
                result.Image = pict;
            }
            else
            {
                MessageBox.Show("Load Gambar Terlebih dahulu","Incomplete Procedure Detected",MessageBoxButtons.OK);
            }
            
        }

        private void leftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image img;
            img = original.Image;
            Bitmap pict = new Bitmap(img);

            if (original.Image != null)
            {
                pict.RotateFlip(RotateFlipType.Rotate270FlipNone);
                result.Image = pict;
            }
            else
            {
                MessageBox.Show("Load Gambar Terlebih dahulu", "Incomplete Procedure Detected", MessageBoxButtons.OK);
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Image img;
            img = original.Image;
            Bitmap pict = new Bitmap(img);

            if (original.Image != null)
            {
                pict.RotateFlip(RotateFlipType.Rotate180FlipNone);
                result.Image = pict;
            }
            else
            {
                MessageBox.Show("Load Gambar Terlebih dahulu", "Incomplete Procedure Detected", MessageBoxButtons.OK);
            }
        }
        #endregion

        #region FLIP
        private void verticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image img;
            img = original.Image;
            Bitmap pict = new Bitmap(img);
            pict.RotateFlip(RotateFlipType.RotateNoneFlipY);
            result.Image = pict;
        }

        private void horizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image img;
            img = original.Image;
            Bitmap pict = new Bitmap(img);
            pict.RotateFlip(RotateFlipType.RotateNoneFlipX);
            result.Image = pict;
        }
        #endregion

        #region CROP
        private void Crop_Click(object sender, EventArgs e)
        {
            Clicked = true;
        }
        private void original_MouseDown(object sender, MouseEventArgs e)
        {
            string message = "Jika ingin crop gambar, klik tombol 'Crop' terlebih dahulu";
            string caption = "Error Detected in Input";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result;

            //result = MessageBox.Show(message, caption, buttons);
            if (Clicked)
            {
                if (e.Button == MouseButtons.Left)
                {
                    _selecting = true;
                    cropSize = new Rectangle(new Point(e.X, e.Y), new Size());
                }
            }
            else
            {
                MessageBox.Show(message, caption, buttons);
            }
        }

        private void original_MouseMove(object sender, MouseEventArgs e)
        {
            if (_selecting)
            {
                cropSize.Width = e.X - cropSize.X;
                cropSize.Height = e.Y - cropSize.Y;

                original.Refresh();
            }
        }

        private void original_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _selecting)
            {
                //Image img = original.Image.Crop(cropSize);
                Image img = CropImage(original.Image,cropSize);

                //result.Image = img.Fit2PictureBox(original);
                result.Image = img;
                _selecting = false;
            }
        }

        private void original_Paint(object sender, PaintEventArgs e)
        {
            if (_selecting)
            {
                Pen pen = Pens.Red;
                e.Graphics.DrawRectangle(pen, cropSize);
            }
        }

        public static Image CropImage(Image image, Rectangle selection)
        {
            //Dengan cara meng cloning gambar yang berada dalam area rectangle
            Bitmap bmp = image as Bitmap;


            if (bmp == null)
                throw new ArgumentException("Gambar Belum Di Load");


            Bitmap cropBmp = bmp.Clone(selection, bmp.PixelFormat);


            image.Dispose();

            return cropBmp;
        }
        #endregion

        #region WARP
        private void DisplayWarpedImage(Warping.WarpOperations warp_op)
        {
            Bitmap bm = new Bitmap(original.Image);
            this.Cursor = Cursors.WaitCursor;
            DateTime start_time = DateTime.Now;

            // Make a Warping object.
            Warping bm32 = new Warping(bm);

            // Apply the warping operation.
            Warping new_bm32 = bm32.Warp(warp_op, false);

            // Display the result.
            result.Image = new_bm32.Bitmap;

            DateTime stop_time = DateTime.Now;
            this.Cursor = Cursors.Default;

            TimeSpan elapsed_time = stop_time - start_time;
            //lblElapsed.Text = elapsed_time.TotalSeconds.ToString("0.000000");
        }

        private void fishEyeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayWarpedImage(Warping.WarpOperations.FishEye);
        }

        private void twistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayWarpedImage(Warping.WarpOperations.Twist);
        }

        private void waveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayWarpedImage(Warping.WarpOperations.Wave);
        }

        private void smallTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayWarpedImage(Warping.WarpOperations.SmallTop);
        }

        private void wiggleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayWarpedImage(Warping.WarpOperations.Wiggles);
        }

        private void doubleWaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayWarpedImage(Warping.WarpOperations.DoubleWave);
        }

        #endregion

        #region HISTOGRAM 
        private void HistRed_Click(object sender, EventArgs e)
        {
            int temp = 0, baris = 0, data = 0, data2 = 0, temp2 = 0, baris2 = 0;

            Histogram Hg = new Histogram();

            Bitmap citra = new Bitmap(original.Image);

            //int[] histo;
            //mendapat nilai tiap RGB

            int[] histogramRed = Hg.HistogramAngkaRed(citra);

            int[] histogramGreen = Hg.HistogramAngkaGreen(citra);

            int[] histogramBlue = Hg.HistogramAngkaBlue(citra);




            listBox1.Items.Clear();



            while (baris < histogramRed.Length)
            {

                temp = histogramRed[baris];

                if (data < temp)
                {

                    data = temp;

                }

                baris++;

            }



            int y = data / 250;



            for (int i = 0; i <= 255; i++)
            {

                listBox1.Items.Add(i + "=" + histogramRed[i]);

                Label lb = new Label();



                int x = Convert.ToInt32(histogramRed[i]);

                lb.Height = x / y;

                lb.Width = 1;

                lb.BackColor = Color.Red;

                //bkin label sbg grapik hist nya. edit posisinya nilainya saja

                //lb.Location = new Point(675 + i, 400 - lb.Height);
                lb.Location = new Point(876 + i, 331 - lb.Height);
                this.Controls.Add(lb);
                //panelHist.Controls.Add(lb);
                lb.ResetText();




            }



            int banyakPixel = citra.Width * citra.Height;

            byte[] hasilR = Hg.HasilHistEqArr(histogramRed, banyakPixel);

            byte[] hasilG = Hg.HasilHistEqArr(histogramGreen, banyakPixel);

            byte[] hasilB = Hg.HasilHistEqArr(histogramBlue, banyakPixel);

            listBox2.Items.Clear();



            //searching nilai tertinggi terlebih dahulu (sequential search)



            while (baris2 < hasilR.Length)
            {

                temp2 = hasilR[baris2];

                if (data2 < temp2)
                {

                    data2 = temp2;

                }

                baris2++;

            }



            //Execute2.Text = data2.ToString();



            int z = data2 / 250;



            for (int i = 0; i <= 255; i++)
            {

                listBox2.Items.Add(i + "=" + hasilR[i]);

                Label lb = new Label();

                int x = Convert.ToInt32(hasilR[i]);



                lb.Height = x / y;

                lb.Width = 1;

                lb.BackColor = Color.Red;

                //lb.Location = new Point(675 + i, 500 - lb.Height);
                lb.Location = new Point(877 + i, 430 - lb.Height);
                this.Controls.Add(lb);
                //panelHist.Controls.Add(lb);






            }

            Hg.HistogramBar(citra, hasilR, hasilG, hasilB);

            //Hg.HistogramBar(citra, HasilRGB(), HasilRGB(), HasilRGB()); 
            result.Image = citra;
        }

        private void HistGreen_Click(object sender, EventArgs e)
        {
            int temp = 0, baris = 0, data = 0, data2 = 0, temp2 = 0, baris2 = 0;

            Histogram Hg = new Histogram();

            Bitmap citra = new Bitmap(original.Image);

            //int[] histo;
            //mendapat nilai tiap RGB

            int[] histogramRed = Hg.HistogramAngkaRed(citra);

            int[] histogramGreen = Hg.HistogramAngkaGreen(citra);

            int[] histogramBlue = Hg.HistogramAngkaBlue(citra);




            listBox1.Items.Clear();



            while (baris < histogramGreen.Length)
            {

                temp = histogramGreen[baris];

                if (data < temp)
                {

                    data = temp;

                }

                baris++;

            }



            int y = data / 250;



            for (int i = 0; i <= 255; i++)
            {

                listBox1.Items.Add(i + "=" + histogramGreen[i]);

                Label lb = new Label();



                int x = Convert.ToInt32(histogramGreen[i]);

                lb.Height = x / y;

                lb.Width = 1;

                lb.BackColor = Color.Green;

                //bkin label sbg grapik hist nya. edit posisinya nilainya saja

                //lb.Location = new Point(675 + i, 400 - lb.Height);
                lb.Location = new Point(876 + i, 331 - lb.Height);
                this.Controls.Add(lb);
                //panelHist.Controls.Add(lb);
                lb.ResetText();




            }



            int banyakPixel = citra.Width * citra.Height;

            byte[] hasilR = Hg.HasilHistEqArr(histogramRed, banyakPixel);

            byte[] hasilG = Hg.HasilHistEqArr(histogramGreen, banyakPixel);

            byte[] hasilB = Hg.HasilHistEqArr(histogramBlue, banyakPixel);

            listBox2.Items.Clear();



            //searching nilai tertinggi terlebih dahulu (sequential search)



            while (baris2 < hasilG.Length)
            {

                temp2 = hasilG[baris2];

                if (data2 < temp2)
                {

                    data2 = temp2;

                }

                baris2++;

            }



            //Execute2.Text = data2.ToString();



            int z = data2 / 250;



            for (int i = 0; i <= 255; i++)
            {

                listBox2.Items.Add(i + "=" + hasilG[i]);

                Label lb = new Label();

                int x = Convert.ToInt32(hasilG[i]);



                lb.Height = x / y;

                lb.Width = 1;

                lb.BackColor = Color.Green;

                //lb.Location = new Point(675 + i, 500 - lb.Height);
                lb.Location = new Point(877 + i, 430 - lb.Height);
                this.Controls.Add(lb);
                //panelHist.Controls.Add(lb);






            }

            Hg.HistogramBar(citra, hasilR, hasilG, hasilB);

            //Hg.HistogramBar(citra, HasilRGB(), HasilRGB(), HasilRGB()); 
            result.Image = citra;
        }

        private void HistBlue_Click(object sender, EventArgs e)
        {
            int temp = 0, baris = 0, data = 0, data2 = 0, temp2 = 0, baris2 = 0;

            Histogram Hg = new Histogram();

            Bitmap citra = new Bitmap(original.Image);

            //int[] histo;
            //mendapat nilai tiap RGB

            int[] histogramRed = Hg.HistogramAngkaRed(citra);

            int[] histogramGreen = Hg.HistogramAngkaGreen(citra);

            int[] histogramBlue = Hg.HistogramAngkaBlue(citra);




            listBox1.Items.Clear();



            while (baris < histogramBlue.Length)
            {

                temp = histogramBlue[baris];

                if (data < temp)
                {

                    data = temp;

                }

                baris++;

            }



            int y = data / 250;



            for (int i = 0; i <= 255; i++)
            {

                listBox1.Items.Add(i + "=" + histogramBlue[i]);

                Label lb = new Label();



                int x = Convert.ToInt32(histogramBlue[i]);

                lb.Height = x / y;

                lb.Width = 1;

                lb.BackColor = Color.Blue;

                //bkin label sbg grapik hist nya. edit posisinya nilainya saja

                //lb.Location = new Point(675 + i, 400 - lb.Height);
                lb.Location = new Point(876 + i, 331 - lb.Height);
                this.Controls.Add(lb);
                //panelHist.Controls.Add(lb);
                lb.ResetText();




            }



            int banyakPixel = citra.Width * citra.Height;

            byte[] hasilR = Hg.HasilHistEqArr(histogramRed, banyakPixel);

            byte[] hasilG = Hg.HasilHistEqArr(histogramGreen, banyakPixel);

            byte[] hasilB = Hg.HasilHistEqArr(histogramBlue, banyakPixel);

            listBox2.Items.Clear();



            //searching nilai tertinggi terlebih dahulu (sequential search)



            while (baris2 < hasilB.Length)
            {

                temp2 = hasilB[baris2];

                if (data2 < temp2)
                {

                    data2 = temp2;

                }

                baris2++;

            }



            //Execute2.Text = data2.ToString();



            int z = data2 / 250;



            for (int i = 0; i <= 255; i++)
            {

                listBox2.Items.Add(i + "=" + hasilB[i]);

                Label lb = new Label();

                int x = Convert.ToInt32(hasilB[i]);



                lb.Height = x / y;

                lb.Width = 1;

                lb.BackColor = Color.Blue;

                //lb.Location = new Point(675 + i, 500 - lb.Height);
                lb.Location = new Point(877 + i, 430 - lb.Height);
                this.Controls.Add(lb);
                //panelHist.Controls.Add(lb);






            }

            Hg.HistogramBar(citra, hasilR, hasilG, hasilB);

            //Hg.HistogramBar(citra, HasilRGB(), HasilRGB(), HasilRGB()); 
            result.Image = citra;
        }
        #endregion

        #region Convolution
        private void Convolution_Click(object sender, EventArgs e)
        {
            if (original.Image == null)
            {
                MessageBox.Show("Load Gambar Terlebih Dahulu","Incomplete Procedure Detected",MessageBoxButtons.OK);
            }
            else
            {
                ConvMask mask = new ConvMask();
                //Convolution cv = new Convolution();
                mask.setAll(1);
                result.Image = Konvolusi.ApplyConv(new Bitmap(original.Image), mask);
            }
        }
        #endregion


        #region MEDIAN FILTER
        private void median3x3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap source = null;
            Bitmap resultpict = null;
            
            source = new Bitmap(original.Image);
            


            if (original.Image == null)
            {
                MessageBox.Show("Mohon Inputkan gambar dulu", "Incomplete Procedure Detected", MessageBoxButtons.OK);

            }
            else
            {
                    resultpict = source.ApplyMedianFilter(3);
                    result.Image = resultpict;
            }
        }

        private void median5x5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap source = null;
            Bitmap resultpict = null;

            source = new Bitmap(original.Image);



            if (original.Image == null)
            {
                MessageBox.Show("Mohon Inputkan gambar dulu", "Incomplete Procedure Detected", MessageBoxButtons.OK);

            }
            else
            {
                resultpict = source.ApplyMedianFilter(5);
                result.Image = resultpict;
            }
        }

        private void median5x5ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Bitmap source = null;
            Bitmap resultpict = null;

            source = new Bitmap(original.Image);



            if (original.Image == null)
            {
                MessageBox.Show("Mohon Inputkan gambar dulu", "Incomplete Procedure Detected", MessageBoxButtons.OK);

            }
            else
            {
                resultpict = source.ApplyMedianFilter(7);
                result.Image = resultpict;
            }
        }
        #endregion

        #region MEAN FILTER 
        private void mean3x3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image img = original.Image;
            Bitmap bm = new Bitmap(img);
            MeanFilter.BlurType tipe = MeanFilter.BlurType.Mean3x3;
            result.Image = bm.ImageBlurFilter(tipe);
        }

        private void mean5x5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image img = original.Image;
            Bitmap bm = new Bitmap(img);
            MeanFilter.BlurType tipe = MeanFilter.BlurType.Mean5x5;
            result.Image = bm.ImageBlurFilter(tipe);
        }

        private void mean7x7ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image img = original.Image;
            Bitmap bm = new Bitmap(img);
            MeanFilter.BlurType tipe = MeanFilter.BlurType.Mean7x7;
            result.Image = bm.ImageBlurFilter(tipe);
        }
        #endregion

        #region INVERT IMAGE
        private void invertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (original.Image != null)
            {
                Image img = original.Image;
                Bitmap bmap = (Bitmap)img.Clone();
                Bitmap bmapR;

                if (original.Image != null)
                {
                    Color col;
                    for (int i = 0; i < bmap.Width; i++)
                    {
                        for (int j = 0; j < bmap.Height; j++)
                        {
                            col = bmap.GetPixel(i, j);
                            bmap.SetPixel(i, j, Color.FromArgb(255 - col.R, 255 - col.G, 255 - col.B));
                        }
                        //bmapR = bmap;
                    }
                }
                result.Image = bmap;
            }
            else
            {
                MessageBox.Show("Masukkan Gambar Terlebih Dahulu!","Incomplete Procedure Detected",MessageBoxButtons.OK);
            }
        }
        #endregion

        private void stretchImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            original.SizeMode = PictureBoxSizeMode.StretchImage;
            result.SizeMode = PictureBoxSizeMode.StretchImage;
            
        }

        #region Differential Sharpening
        private void spatialToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Image img = original.Image;
            Bitmap bm = new Bitmap(img);
            result.Image = sharpen(bm);
        }

        public static Bitmap sharpen(Bitmap image)
        {
            Bitmap sharpenImage = new Bitmap(image.Width, image.Height);

            int filterWidth = 3;
            int filterHeight = 3;
            int w = image.Width;
            int h = image.Height;

            double[,] filter = new double[filterWidth, filterHeight];

            filter[0, 0] = filter[0, 1] = filter[0, 2] = filter[1, 0] = filter[1, 2] = filter[2, 0] = filter[2, 1] = filter[2, 2] = -1;
            filter[1, 1] = 9;

            double factor = 1.0;
            double bias = 0.0;

            Color[,] result = new Color[image.Width, image.Height];

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    double red = 0.0, green = 0.0, blue = 0.0;

                    
                    Color imageColor = image.GetPixel(x, y);
                    

                    for (int filterX = 0; filterX < filterWidth; filterX++)
                    {
                        for (int filterY = 0; filterY < filterHeight; filterY++)
                        {
                            int imageX = (x - filterWidth / 2 + filterX + w) % w;
                            int imageY = (y - filterHeight / 2 + filterY + h) % h;

                            
                            Color imageColors = image.GetPixel(imageX, imageY);
                            

                            red += imageColors.R * filter[filterX, filterY];
                            green += imageColors.G * filter[filterX, filterY];
                            blue += imageColors.B * filter[filterX, filterY];
                        }
                        int r = Math.Min(Math.Max((int)(factor * red + bias), 0), 255);
                        int g = Math.Min(Math.Max((int)(factor * green + bias), 0), 255);
                        int b = Math.Min(Math.Max((int)(factor * blue + bias), 0), 255);

                        result[x, y] = Color.FromArgb(r, g, b);
                    }
                }
            }
            for (int i = 0; i < w; ++i)
            {
                for (int j = 0; j < h; ++j)
                {
                    sharpenImage.SetPixel(i, j, result[i, j]);
                }
            }
            return sharpenImage;
        }

        #endregion


        #region EDGE DETECTION
        private void laplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (original.Image != null)
            {
                Image img = original.Image;
                Bitmap bmp = new Bitmap(img);
                MeanFilter.BlurType tipe = MeanFilter.BlurType.Laplace;
                result.Image = MeanFilter.ImageBlurFilter(bmp, tipe);
            }
            else
            {
                MessageBox.Show("Input Gambar Terlebih Dahulu!","Incomplete Procedure Detected",MessageBoxButtons.OK);
            }
        }

        private void robertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (original.Image != null)
            {
                Image img = original.Image;
                Bitmap bmp = new Bitmap(img);
                MeanFilter.BlurType tipe = MeanFilter.BlurType.Robert;
                result.Image = MeanFilter.ImageBlurFilter(bmp,tipe);
            }
            else
            {
                MessageBox.Show("Input Gambar Terlebih Dahulu!", "Incomplete Procedure Detected", MessageBoxButtons.OK);
            }
        }

        private void prewitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (original.Image != null)
            {
                Image img = original.Image;
                Bitmap bmp = new Bitmap(img);
                MeanFilter.BlurType tipe = MeanFilter.BlurType.Prewit;
                result.Image = MeanFilter.ImageBlurFilter(bmp, tipe);
            }
            else
            {
                MessageBox.Show("Input Gambar Terlebih Dahulu!", "Incomplete Procedure Detected", MessageBoxButtons.OK);
            }
        }

        private void sobelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (original.Image != null)
            {
                Image img = original.Image;
                Bitmap bmp = new Bitmap(img);
                MeanFilter.BlurType tipe = MeanFilter.BlurType.Sobel;
                result.Image = MeanFilter.ImageBlurFilter(bmp, tipe);
            }
            else
            {
                MessageBox.Show("Input Gambar Terlebih Dahulu!", "Incomplete Procedure Detected", MessageBoxButtons.OK);
            }
        }

        private void freiChanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (original.Image != null)
            {
                Image img = original.Image;
                Bitmap bmp = new Bitmap(img);
                MeanFilter.BlurType tipe = MeanFilter.BlurType.FreiChan;
                result.Image = MeanFilter.ImageBlurFilter(bmp, tipe);
            }
            else
            {
                MessageBox.Show("Input gambar terlebih dahulu!","Incomplete Procedure Detected",MessageBoxButtons.OK);
            }
        }

        #endregion

        #region THRESHOLDING
        private void thresholdingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image img = original.Image;
            Bitmap bmp = new Bitmap(img);
            result.Image= ThresholdGS(bmp,90);
        }

        public Image ThresholdGS(Bitmap bmp, float thresh)
        {
            //Bitmap b = new Bitmap(_image);

            for (int i = 0; i < bmp.Height; ++i)
            {
                for (int j = 0; j < bmp.Width; ++j)
                {
                    Color c = bmp.GetPixel(j, i);

                    double magnitude = 1 / 3d * (c.B + c.G + c.R);

                    if (magnitude < thresh)
                    {
                        bmp.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    }
                    else
                    {
                        bmp.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                    }
                }
            }

            return bmp;
        }
        #endregion

        #region REGION GROWING
        private void regionGrowingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (original.Image != null)
            {
                Image img = original.Image;
                Bitmap bmp = new Bitmap(img);
                result.Image = regionGrowing(bmp);
            }
            else
            {
                MessageBox.Show("Input gambar terlebih dahulu","Incomplete Procedure Detected",MessageBoxButtons.OK);
            }
        }

        private Bitmap regionGrowing(Bitmap b)
        {
            byte minq = 50;
            byte maxq = 100;
            Bitmap EditImage = b;

            BitmapData bmData = EditImage.LockBits(new Rectangle(0, 0, EditImage.Width, EditImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            int bitsPerPixels = stride / EditImage.Width;

            int[,] arraynilai = new int[EditImage.Width + 1, EditImage.Height + 1];

            unsafe
            {
                byte* pos;
                byte* scan0 = (byte*)(bmData.Scan0.ToPointer());

                for (int j = 0; j < bmData.Height; j++)
                {
                    pos = scan0 + stride * j;
                    for (int i = 0; i < bmData.Width; i++)
                    {
                        *pos = (byte)(255 - *pos);
                        
                        if ((pos[i] <= maxq && pos[i] >= minq))
                            arraynilai[i, j] = 1;
                        else
                            arraynilai[i, j] = 0;
                        pos += bitsPerPixels;
                    }
                }
                for (int j = 1; j < bmData.Height; j++)
                {
                    pos = scan0 + stride * j;
                    for (int i = 1; i < bmData.Width; i++)
                    {
                        *pos = (byte)(255 - *pos);
                        int rc1 = arraynilai[i - 1, j - 1];
                        int rc2 = arraynilai[i, j - 1];
                        int rc3 = arraynilai[i + 1, j - 1];
                        int rc4 = arraynilai[i + 1, j];
                        int rc5 = arraynilai[i + 1, j + 1];
                        int rc6 = arraynilai[i, j + 1];
                        int rc7 = arraynilai[i - 1, j + 1];
                        int rc8 = arraynilai[i - 1, j];
                        int tot = rc1 + rc2 + rc3 + rc4 + rc5 + rc6 + rc7 + rc8;
                        if (tot < 8 && tot > 0)
                        {
                            pos[i] = pos[i + 1] = pos[i + 2] = 255;
                        }
                        pos += bitsPerPixels;
                    }
                }
            }
            EditImage.UnlockBits(bmData);

            return EditImage;
        }
            
 
        #endregion 

        #region DILATION
        private void dilationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (original.Image != null)
            {
                //Dilasi_Erosi_Open_Close d = new Dilasi_Erosi_Open_Close();
                //Bitmap bmp;
                Image img;
                img = original.Image;
                Bitmap bmp = new Bitmap(img);
                //result.Image = Dilasi_Erosi_Open_Close.Dilate(bmp);
                result.Image = Dilasi_Erosi_Open_Close.DilateAndErodeFilter(bmp,5,Dilasi_Erosi_Open_Close.MorphologyType.Dilation);
            }
            else
            {
                MessageBox.Show("","",MessageBoxButtons.OK);
            }
        }

        

        private void erosiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(original.Image != null)
            {
                Image img = original.Image;
                Bitmap bmp = new Bitmap(img);
                result.Image = Dilasi_Erosi_Open_Close.DilateAndErodeFilter(bmp, 5, Dilasi_Erosi_Open_Close.MorphologyType.Erosion);

            }
            else
            {
                MessageBox.Show("", "", MessageBoxButtons.OK);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (original.Image != null)
            {
                Image img = original.Image;
                Bitmap bmp = new Bitmap(img);
                result.Image = Dilasi_Erosi_Open_Close.OpenMorphologyFilter(bmp, 5);

            }
            else
            {
                MessageBox.Show("", "", MessageBoxButtons.OK);
            }
        }

       

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (original.Image != null)
            {
                Image img = original.Image;
                Bitmap bmp = new Bitmap(img);
                result.Image = Dilasi_Erosi_Open_Close.CloseMorphologyFilter(bmp, 5);

            }
            else
            {
                MessageBox.Show("", "", MessageBoxButtons.OK);
            }
        }

        #endregion


    }
}
