using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace PCD
{
    public class Histogram
    {
        public int[] HistogramAngkaRed(Bitmap b)
        {


            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;

            System.IntPtr Scan0 = bmData.Scan0;

            int[] data = new int[b.Width * b.Height];

            unsafe
            {

                byte* p = (byte*)(void*)Scan0;



                int nOffset = stride - b.Width * 3;

                for (int y = 0; y < b.Height; ++y)
                {

                    for (int x = 0; x < b.Width; ++x)
                    {



                        data[p[0]] += 1;



                        p += 3;

                    }

                    p += nOffset;

                }



            }

            b.UnlockBits(bmData);

            return data;

        }



        public int[] HistogramAngkaGreen(Bitmap b)
        {


            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),

                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;

            System.IntPtr Scan0 = bmData.Scan0;

            int[] data = new int[b.Width * b.Height];

            unsafe
            {

                byte* p = (byte*)(void*)Scan0;



                int nOffset = stride - b.Width * 3;

                for (int y = 0; y < b.Height; ++y)
                {

                    for (int x = 0; x < b.Width; ++x)
                    {



                        data[p[1]] += 1;



                        p += 3;

                    }

                    p += nOffset;

                }



            }

            b.UnlockBits(bmData);

            return data;

        }



        public int[] HistogramAngkaBlue(Bitmap b)
        {


            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),

                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;

            System.IntPtr Scan0 = bmData.Scan0;

            int[] data = new int[b.Width * b.Height];

            unsafe
            {

                byte* p = (byte*)(void*)Scan0;



                int nOffset = stride - b.Width * 3;

                for (int y = 0; y < b.Height; ++y)
                {

                    for (int x = 0; x < b.Width; ++x)
                    {



                        data[p[2]] += 1;



                        p += 3;

                    }

                    p += nOffset;

                }



            }

            b.UnlockBits(bmData);

            return data;

        }



        public byte[] HasilHistEqArr(int[] data, int bykPixel)
        {

            byte[] hasil = new byte[data.Length];

            double sigmaNj = 0;

            double sigmaNjPerN = 0;

            double hasilPecahan = 0;

            for (int i = 0; i <= 255; i++)
            {

                sigmaNj = Convert.ToDouble(sigmaNj + data[i]);

                sigmaNjPerN = sigmaNj / bykPixel;

                hasilPecahan = sigmaNjPerN * 255;

                hasil[i] = Convert.ToByte(Math.Round(hasilPecahan));

            }

            return hasil;

        }



        public void HistogramBar(Bitmap b, byte[] dataR, byte[] dataG, byte[] dataB)
        {


            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),

                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;

            System.IntPtr Scan0 = bmData.Scan0;

            unsafe
            {

                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - b.Width * 3;



                int bykData = b.Width * b.Height;

                for (int y = 0; y < b.Height; ++y)
                {

                    for (int x = 0; x < b.Width; ++x)
                    {

                        p[0] = dataR[p[0]];

                        p[1] = dataG[p[1]];

                        p[2] = dataB[p[2]];

                        p[2] = p[1] = p[0];



                        p += 3;

                    }

                    p += nOffset;

                }



            }

            b.UnlockBits(bmData);

            //return true;

        }



        public bool DiagramHistogram(Bitmap b, int[] hisRed)
        {


            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),

                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;

            System.IntPtr Scan0 = bmData.Scan0;

            unsafe
            {

                byte* p = (byte*)(void*)Scan0;

                int sRed, sGreen, sBlue;

                int nOffset = stride - b.Width * 3;

                for (int y = 0; y < b.Height; ++y)
                {

                    for (int x = 0; x < b.Width; ++x)
                    {



                        p += 3;

                    }

                    p += nOffset;

                }

            }

            b.UnlockBits(bmData);

            return true;

        }
    }
}
