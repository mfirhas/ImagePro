using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace PCD
{
    /// <summary>
    /// Defining Structure for Complex Data type  N=R+Ii
    /// </summary>
    struct COMPLEX
    {
        public double real, imag;
        public COMPLEX(double x, double y)
        {
            real = x;
            imag = y;
        }
        public float Magnitude()
        {
            return ((float)Math.Sqrt(real * real + imag * imag));
        }
        public float Phase()
        {
            return ((float)Math.Atan(imag / real));
        }
    }

    class FFT
    {
        public Bitmap Obj;               
        public Bitmap FourierPlot;       
        public Bitmap PhasePlot;        

        public int[,] GreyImage;         
        public float[,] FourierMagnitude;
        public float[,] FourierPhase;

        float[,] FFTLog;                 
        float[,] FFTPhaseLog;            
        public int[,] FFTNormalized;     
        public int[,] FFTPhaseNormalized;
        int nx, ny;                      
        int Width, Height;
        COMPLEX[,] Fourier;              
        public COMPLEX[,] FFTShifted;    
        public COMPLEX[,] Output;       
        public COMPLEX[,] FFTNormal;     

       
        public FFT(Bitmap Input)
        {
            Obj = Input;
            Width = nx = Input.Width;
            Height = ny = Input.Height;
            ReadImage();
        }
        
        public FFT(int[,] Input)
        {
            GreyImage = Input;
            Width = nx = Input.GetLength(0);
            Height = ny = Input.GetLength(1);
        }
        
        public FFT(COMPLEX[,] Input)
        {
            nx = Width = Input.GetLength(0);
            ny = Height = Input.GetLength(1);
            Fourier = Input;

        }
        
        private void ReadImage()
        {
            int i, j;
            GreyImage = new int[Width, Height];  //[Row,Column]
            Bitmap image = Obj;
            BitmapData bitmapData1 = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                                     ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* imagePointer1 = (byte*)bitmapData1.Scan0;

                for (i = 0; i < bitmapData1.Height; i++)
                {
                    for (j = 0; j < bitmapData1.Width; j++)
                    {
                        GreyImage[j, i] = (int)((imagePointer1[0] + imagePointer1[1] + imagePointer1[2]) / 3.0);
                        //4 bytes per pixel
                        imagePointer1 += 4;
                    }//end for j
                    //4 bytes per pixel
                    imagePointer1 += bitmapData1.Stride - (bitmapData1.Width * 4);
                }//end for i
            }//end unsafe
            image.UnlockBits(bitmapData1);
            return;
        }
        public Bitmap Displayimage()
        {
            int i, j;
            Bitmap image = new Bitmap(Width, Height);
            BitmapData bitmapData1 = image.LockBits(new Rectangle(0, 0, Width, Height),
                                     ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {

                byte* imagePointer1 = (byte*)bitmapData1.Scan0;

                for (i = 0; i < bitmapData1.Height; i++)
                {
                    for (j = 0; j < bitmapData1.Width; j++)
                    {
                        imagePointer1[0] = (byte)GreyImage[j, i];
                        imagePointer1[1] = (byte)GreyImage[j, i];
                        imagePointer1[2] = (byte)GreyImage[j, i];
                        imagePointer1[3] = (byte)255;
                        //4 bytes per pixel
                        imagePointer1 += 4;
                    }//end for j

                    //4 bytes per pixel
                    imagePointer1 += (bitmapData1.Stride - (bitmapData1.Width * 4));
                }//end for i
            }//end unsafe
            image.UnlockBits(bitmapData1);
            return image;// col;
        }
        public Bitmap Displayimage(int[,] image)
        {
            int i, j;
            Bitmap output = new Bitmap(image.GetLength(0), image.GetLength(1));
            BitmapData bitmapData1 = output.LockBits(new Rectangle(0, 0, image.GetLength(0), image.GetLength(1)),
                                     ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* imagePointer1 = (byte*)bitmapData1.Scan0;
                for (i = 0; i < bitmapData1.Height; i++)
                {
                    for (j = 0; j < bitmapData1.Width; j++)
                    {
                        imagePointer1[0] = (byte)image[j, i];
                        imagePointer1[1] = (byte)image[j, i];
                        imagePointer1[2] = (byte)image[j, i];
                        imagePointer1[3] = 255;
                        //4 bytes per pixel
                        imagePointer1 += 4;
                    }//end for j
                    //4 bytes per pixel
                    imagePointer1 += (bitmapData1.Stride - (bitmapData1.Width * 4));
                }//end for i
            }//end unsafe
            output.UnlockBits(bitmapData1);
            return output;// col;

        }
        
        public void ForwardFFT()
        {
            int i, j;
            Fourier = new COMPLEX[Width, Height];
            Output = new COMPLEX[Width, Height];
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    Fourier[i, j].real = (double)GreyImage[i, j];
                    Fourier[i, j].imag = 0;
                }
            Output = FFT2D(Fourier, nx, ny, 1);
            return;
        }
        
        public void FFTShift()
        {
            int i, j;
            FFTShifted = new COMPLEX[nx, ny];

            for (i = 0; i <= (nx / 2) - 1; i++)
                for (j = 0; j <= (ny / 2) - 1; j++)
                {
                    FFTShifted[i + (nx / 2), j + (ny / 2)] = Output[i, j];
                    FFTShifted[i, j] = Output[i + (nx / 2), j + (ny / 2)];
                    FFTShifted[i + (nx / 2), j] = Output[i, j + (ny / 2)];
                    FFTShifted[i, j + (nx / 2)] = Output[i + (nx / 2), j];
                }

            return;
        }
        
        public void RemoveFFTShift()
        {
            int i, j;
            FFTNormal = new COMPLEX[nx, ny];

            for (i = 0; i <= (nx / 2) - 1; i++)
                for (j = 0; j <= (ny / 2) - 1; j++)
                {
                    FFTNormal[i + (nx / 2), j + (ny / 2)] = FFTShifted[i, j];
                    FFTNormal[i, j] = FFTShifted[i + (nx / 2), j + (ny / 2)];
                    FFTNormal[i + (nx / 2), j] = FFTShifted[i, j + (ny / 2)];
                    FFTNormal[i, j + (nx / 2)] = FFTShifted[i + (nx / 2), j];
                }
            return;
        }
        
        public void FFTPlot(COMPLEX[,] Output)
        {
            int i, j;
            float max;

            FFTLog = new float[nx, ny];
            FFTPhaseLog = new float[nx, ny];

            FourierMagnitude = new float[nx, ny];
            FourierPhase = new float[nx, ny];

            FFTNormalized = new int[nx, ny];
            FFTPhaseNormalized = new int[nx, ny];

            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FourierMagnitude[i, j] = Output[i, j].Magnitude();
                    FourierPhase[i, j] = Output[i, j].Phase();
                    FFTLog[i, j] = (float)Math.Log(1 + FourierMagnitude[i, j]);
                    FFTPhaseLog[i, j] = (float)Math.Log(1 + Math.Abs(FourierPhase[i, j]));
                }
            max = FFTLog[0, 0];
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    if (FFTLog[i, j] > max)
                        max = FFTLog[i, j];
                }
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FFTLog[i, j] = FFTLog[i, j] / max;
                }
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FFTNormalized[i, j] = (int)(2000 * FFTLog[i, j]);
                }
            FourierPlot = Displayimage(FFTNormalized);

            FFTPhaseLog[0, 0] = 0;
            max = FFTPhaseLog[1, 1];
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    if (FFTPhaseLog[i, j] > max)
                        max = FFTPhaseLog[i, j];
                }
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FFTPhaseLog[i, j] = FFTPhaseLog[i, j] / max;
                }
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FFTPhaseNormalized[i, j] = (int)(255 * FFTPhaseLog[i, j]);
                }
            PhasePlot = Displayimage(FFTPhaseNormalized);


        }
        
        public void FFTPlot()
        {
            int i, j;
            float max;
            FFTLog = new float[nx, ny];
            FFTPhaseLog = new float[nx, ny];

            FourierMagnitude = new float[nx, ny];
            FourierPhase = new float[nx, ny];

            FFTNormalized = new int[nx, ny];
            FFTPhaseNormalized = new int[nx, ny];

            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FourierMagnitude[i, j] = Output[i, j].Magnitude();
                    FourierPhase[i, j] = Output[i, j].Phase();
                    FFTLog[i, j] = (float)Math.Log(1 + FourierMagnitude[i, j]);
                    FFTPhaseLog[i, j] = (float)Math.Log(1 + Math.Abs(FourierPhase[i, j]));
                }
            max = FFTLog[0, 0];
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    if (FFTLog[i, j] > max)
                        max = FFTLog[i, j];
                }
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FFTLog[i, j] = FFTLog[i, j] / max;
                }
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FFTNormalized[i, j] = (int)(1000 * FFTLog[i, j]);
                }
            FourierPlot = Displayimage(FFTNormalized);


            max = FFTPhaseLog[0, 0];
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    if (FFTPhaseLog[i, j] > max)
                        max = FFTPhaseLog[i, j];
                }
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FFTPhaseLog[i, j] = FFTPhaseLog[i, j] / max;
                }
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    FFTPhaseNormalized[i, j] = (int)(2000 * FFTLog[i, j]);
                }
            PhasePlot = Displayimage(FFTPhaseNormalized);


        }
        
        public void InverseFFT()
        {
            int i, j;

            Output = new COMPLEX[nx, ny];
            Output = FFT2D(Fourier, nx, ny, -1);

            Obj = null;  
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    GreyImage[i, j] = (int)Output[i, j].Magnitude();

                }
            Obj = Displayimage(GreyImage);
            return;

        }
        
        public void InverseFFT(COMPLEX[,] Fourier)
        {
            int i, j;

            Output = new COMPLEX[nx, ny];
            Output = FFT2D(Fourier, nx, ny, -1);


           
            for (i = 0; i <= Width - 1; i++)
                for (j = 0; j <= Height - 1; j++)
                {
                    GreyImage[i, j] = (int)Output[i, j].Magnitude();

                }
            Obj = Displayimage(GreyImage);
            return;

        }
        
        public COMPLEX[,] FFT2D(COMPLEX[,] c, int nx, int ny, int dir)
        {
            int i, j;
            int m;
            double[] real;
            double[] imag;
            COMPLEX[,] output;
            output = c; 
            real = new double[nx];
            imag = new double[nx];

            for (j = 0; j < ny; j++)
            {
                for (i = 0; i < nx; i++)
                {
                    real[i] = c[i, j].real;
                    imag[i] = c[i, j].imag;
                }
                m = (int)Math.Log((double)nx, 2);
                FFT1D(dir, m, ref real, ref imag);

                for (i = 0; i < nx; i++)
                {
                    //  c[i,j].real = real[i];
                    //  c[i,j].imag = imag[i];
                    output[i, j].real = real[i];
                    output[i, j].imag = imag[i];
                }
            }
            real = new double[ny];
            imag = new double[ny];

            for (i = 0; i < nx; i++)
            {
                for (j = 0; j < ny; j++)
                {
                    //real[j] = c[i,j].real;
                    //imag[j] = c[i,j].imag;
                    real[j] = output[i, j].real;
                    imag[j] = output[i, j].imag;
                }
                m = (int)Math.Log((double)ny, 2);
                FFT1D(dir, m, ref real, ref imag);
                for (j = 0; j < ny; j++)
                {
                    //c[i,j].real = real[j];
                    //c[i,j].imag = imag[j];
                    output[i, j].real = real[j];
                    output[i, j].imag = imag[j];
                }
            }

            // return(true);
            return (output);
        }
        
        private void FFT1D(int dir, int m, ref double[] x, ref double[] y)
        {
            long nn, i, i1, j, k, i2, l, l1, l2;
            double c1, c2, tx, ty, t1, t2, u1, u2, z;
            nn = 1;
            for (i = 0; i < m; i++)
                nn *= 2;
            i2 = nn >> 1;
            j = 0;
            for (i = 0; i < nn - 1; i++)
            {
                if (i < j)
                {
                    tx = x[i];
                    ty = y[i];
                    x[i] = x[j];
                    y[i] = y[j];
                    x[j] = tx;
                    y[j] = ty;
                }
                k = i2;
                while (k <= j)
                {
                    j -= k;
                    k >>= 1;
                }
                j += k;
            }
            c1 = -1.0;
            c2 = 0.0;
            l2 = 1;
            for (l = 0; l < m; l++)
            {
                l1 = l2;
                l2 <<= 1;
                u1 = 1.0;
                u2 = 0.0;
                for (j = 0; j < l1; j++)
                {
                    for (i = j; i < nn; i += l2)
                    {
                        i1 = i + l1;
                        t1 = u1 * x[i1] - u2 * y[i1];
                        t2 = u1 * y[i1] + u2 * x[i1];
                        x[i1] = x[i] - t1;
                        y[i1] = y[i] - t2;
                        x[i] += t1;
                        y[i] += t2;
                    }
                    z = u1 * c1 - u2 * c2;
                    u2 = u1 * c2 + u2 * c1;
                    u1 = z;
                }
                c2 = Math.Sqrt((1.0 - c1) / 2.0);
                if (dir == 1)
                    c2 = -c2;
                c1 = Math.Sqrt((1.0 + c1) / 2.0);
            }
            if (dir == 1)
            {
                for (i = 0; i < nn; i++)
                {
                    x[i] /= (double)nn;
                    y[i] /= (double)nn;

                }
            }



            //  return(true) ;
            return;
        }

    }
}
