using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
//using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
//using AForge;
//using AForge.Math;
//using AForge.Imaging;
//using AForge.Imaging.Filters;
//using AForge.Imaging.Textures;
//using AForge.Imaging.Filters;

namespace PCD
{
    public partial class FastFourierTransform : Form
    {
        Bitmap InputImage;
        Bitmap SelectedImage;  
        Bitmap bmp;  
        public Point current;
        Color mlinecolor;
        FFT ImgFFT;
        public int rec_width, rec_height;
        public int scale = 25; 
        public int WindowSize = 256;  
        public FastFourierTransform(Bitmap bmp2)
        {
            InitializeComponent();
            mlinecolor = Color.Red;

            ImageInput.SizeMode = PictureBoxSizeMode.Normal;
            scale = Convert.ToInt32(scalepercentage.Text);
            rec_width = rec_height = (int)(512 * ((float)scale / 100));
            InputImage = new Bitmap(bmp2);  
            ImageInput.SizeMode = PictureBoxSizeMode.AutoSize;
            ImageInput.Image = ScaleByPercent((Image)InputImage, Convert.ToInt32(scalepercentage.Text));
        }

        

        static Image ScaleByPercent(Image imgPhoto, int Percent)
        {
            float nPercent = ((float)Percent / 100);
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.DrawImage(imgPhoto,
            new Rectangle(destX, destY, destWidth, destHeight),
            new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
            GraphicsUnit.Pixel);
            grPhoto.Dispose();
            return bmPhoto;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (FourierMag.Image == null)
            {
                MessageBox.Show("Tidak ada gambar yang akan disimpan ","Incomplete Procedure Detected!",MessageBoxButtons.OK);
            }
            else
            {
                SaveFileDialog sv = new SaveFileDialog();
                sv.AddExtension = true;
                if (sv.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    switch (Path.GetExtension(sv.FileName).ToUpper())
                    {
                        case ".BMP":
                            FourierMag.Image.Save(sv.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                            break;
                        case ".JPG":
                            FourierMag.Image.Save(sv.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;
                        case ".PNG":
                            FourierMag.Image.Save(sv.FileName, System.Drawing.Imaging.ImageFormat.Png);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void ImageInput_MouseMove(object sender, MouseEventArgs e)
        {
            toolTip1.SetToolTip(ImageInput, e.X.ToString() + ", " + e.Y.ToString());
            Pen ppen = new Pen(mlinecolor, 1);
            Graphics g;
            ImageInput.Refresh();
            try
            {
                g = ImageInput.CreateGraphics();
                Rectangle rec = new Rectangle(e.X, e.Y, (int)(WindowSize * Convert.ToInt32(scalepercentage.Text) / 100), (int)(WindowSize * Convert.ToInt32(scalepercentage.Text) / 100));
                g.DrawRectangle(ppen, rec);
                current.X = e.X;
                current.Y = e.Y;
                ppen.Color = Color.Red;
                g.DrawLine(ppen, ImageInput.Width / 2, ImageInput.Top, ImageInput.Width / 2, ImageInput.Height);
                g.DrawLine(ppen, 0, ImageInput.Height / 2, ImageInput.Width, ImageInput.Height / 2);
                ppen.Color = Color.LightBlue;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void selectImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int x, y, width, height;
            
            try
            {
                Bitmap temp = (Bitmap)InputImage.Clone();
                width = height = (int)(WindowSize * Convert.ToInt32(scalepercentage.Text) / 100);
                bmp = new Bitmap(width, height, InputImage.PixelFormat);

                x = (int)((float)current.X * (100 / Convert.ToDouble(scalepercentage.Text)));
                y = (int)((float)current.Y * (100 / Convert.ToDouble(scalepercentage.Text)));
                width = height = (int)(rec_width * (100 / (float)scale));
                if (width > WindowSize)
                {
                    width = height = WindowSize;
                }

                Rectangle area = new Rectangle(x, y, width, height);
                bmp = (Bitmap)InputImage.Clone(area, InputImage.PixelFormat);
                SelectedImage = bmp;
            }
            catch (System.OutOfMemoryException ex)
            {
                MessageBox.Show("Select Area Inside Image only : " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void previewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImSelected.Image = (Image)SelectedImage;
           
            ImSelected.Invalidate();
        }


        private void FastFourierTransform_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ImgFFT = new FFT(bmp);

            ImgFFT.ForwardFFT();
            ImgFFT.FFTShift();
            ImgFFT.FFTPlot(ImgFFT.FFTShifted);
            FourierMag.Image = (Image)ImgFFT.FourierPlot;
        }

        

        

        

        
        

        


        

    }
}
