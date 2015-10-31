using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace PCD
{
    public class Quantization
    {
        public  Bitmap ConvertTo8bppFormat(Bitmap image)
        {

            Bitmap destImage = new Bitmap(
                image.Width,
                image.Height,
                PixelFormat.Format8bppIndexed
                );

            BitmapData bitmapData = destImage.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadWrite,
                destImage.PixelFormat
                );

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color color = image.GetPixel(i, j);

                    byte index = GetSimilarColor(destImage.Palette, color);

                    WriteBitmapData(i, j, index, bitmapData, 8);
                }
            }

            destImage.UnlockBits(bitmapData);

            return destImage;
        }

        public Bitmap ConvertTo4bppFormat(Bitmap image)
        {

            Bitmap destImage = new Bitmap(
                image.Width,
                image.Height,
                PixelFormat.Format4bppIndexed
                );

            BitmapData bitmapData = destImage.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadWrite,
                destImage.PixelFormat
                );

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color color = image.GetPixel(i, j);

                    byte index = GetSimilarColor(destImage.Palette, color);

                    WriteBitmapData2(i, j, index, bitmapData, 4);
                }
            }

            destImage.UnlockBits(bitmapData);

            return destImage;
        }

        
        private static byte GetSimilarColor(ColorPalette palette, Color color)
        {

            byte minDiff = byte.MaxValue;
            byte index = 0;

            for (int i = 0; i < palette.Entries.Length - 1; i++)
            {

                byte currentDiff = GetMaxDiff(color, palette.Entries[i]);

                if (currentDiff < minDiff)
                {
                    minDiff = currentDiff;
                    index = (byte)i;
                }
            }

            return index;
        }

        
        private static byte GetMaxDiff(Color a, Color b)
        {

            byte bDiff = System.Convert.ToByte(
                Math.Abs((short)(a.B - b.B)));

            byte gDiff = System.Convert.ToByte(
                Math.Abs((short)(a.G - b.G)));

            byte rDiff = System.Convert.ToByte(
                Math.Abs((short)(a.R - b.R)));

            return Math.Max(rDiff, Math.Max(bDiff, gDiff));
        }

         
        private static void WriteBitmapData(
            int i,
            int j,
            byte index,
            BitmapData data,
            int pixelSize)
        {

            double entry = pixelSize / 8;

            IntPtr realByteAddr = new IntPtr(System.Convert.ToInt32(
                                  data.Scan0.ToInt32() +
                                  (j * data.Stride) + i * entry));

            byte[] dataToCopy = new byte[] { index };

            Marshal.Copy(dataToCopy, 0, realByteAddr,
                          dataToCopy.Length);
        }

        private static void WriteBitmapData2(
            int i,
            int j,
            byte index,
            BitmapData data,
            int pixelSize)
        {

            double entry = pixelSize / 4;

            IntPtr realByteAddr = new IntPtr(System.Convert.ToInt32(
                                  data.Scan0.ToInt32() +
                                  (j * data.Stride) + i * entry));

            byte[] dataToCopy = new byte[] { index };

            Marshal.Copy(dataToCopy, 0, realByteAddr,
                          dataToCopy.Length);
        }

    }
}
