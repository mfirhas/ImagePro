using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Data;


namespace PCD
{
    public class Konvolusi
    {
        public Konvolusi()
        {

        }

        public static Bitmap ApplyConv(Bitmap b, ConvMask m)
        {
            if (m.factor == 0)
                return b;

            Bitmap OutPutImage;
            OutPutImage = (Bitmap)b.Clone();
            BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //BitmapData copyData=b.LockBits(new Rectangle(0,0,OutPutImage.Width,OutPutImage.Height),ImageLockMode.ReadWrite,tengahFormat.Format24bppRgb);
            BitmapData copyData = OutPutImage.LockBits(new Rectangle(0, 0, OutPutImage.Width, OutPutImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bData.Stride;
            int stride2 = stride * 2;

            System.IntPtr ptr = bData.Scan0;
            System.IntPtr ptrOutPut = copyData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)ptr;
                byte* pOutPut = (byte*)(void*)ptrOutPut;

                int noffset = stride - b.Width * 3;
                int nWidth = b.Width - 2;
                int nHeight = b.Height - 2;
                int ntengah;

                for (int x = 0; x < nWidth; x++)
                    for (int y = 0; y < nHeight; y++)
                    {
                        
                        ntengah = (((pOutPut[2] * m.atasKiri) + (pOutPut[5] * m.atasTengah) + (pOutPut[8] * m.atasKanan) +
                            (pOutPut[stride + 2] * m.tengahKiri) + (pOutPut[stride + 5] * m.tengah) + (pOutPut[stride + 8] * m.tengahKanan)
                            + (pOutPut[stride2 + 2] * m.bawahKiri) + (pOutPut[stride2 + 5] * m.bawahTengah) + (pOutPut[stride2 + 8] * m.bawahKanan)) / m.factor) + m.offset;

                        if (ntengah < 0) ntengah = 0;
                        if (ntengah > 255) ntengah = 255;
                        pOutPut[5 + stride] = (byte)ntengah;
                        ntengah = ((((pOutPut[1] * m.atasKiri) + (pOutPut[4] * m.atasTengah) +
                            (pOutPut[7] * m.atasKanan) + (pOutPut[1 + stride] * m.tengahKiri) +
                            (pOutPut[4 + stride] * m.tengah) + (pOutPut[7 + stride] * m.tengahKanan) +
                            (pOutPut[1 + stride2] * m.bawahKiri) +
                            (pOutPut[4 + stride2] * m.bawahTengah) +
                            (pOutPut[7 + stride2] * m.bawahKanan))
                            / m.factor) + m.offset);

                        if (ntengah < 0) ntengah = 0;
                        if (ntengah > 255) ntengah = 255;
                        p[4 + stride] = (byte)ntengah;

                        ntengah = ((((pOutPut[0] * m.atasKiri) + (pOutPut[3] * m.atasTengah) +
                            (pOutPut[6] * m.atasKanan) + (pOutPut[0 + stride] * m.tengahKiri) +
                            (pOutPut[3 + stride] * m.tengah) +
                            (pOutPut[6 + stride] * m.tengahKanan) +
                            (pOutPut[0 + stride2] * m.bawahKiri) +
                            (pOutPut[3 + stride2] * m.bawahTengah) +
                            (pOutPut[6 + stride2] * m.bawahKanan))
                            / m.factor) + m.offset);

                        if (ntengah < 0) ntengah = 0;
                        if (ntengah > 255) ntengah = 255;
                        p[3 + stride] = (byte)ntengah;

                        p += 3;
                        pOutPut += 3;
                    }

                p += noffset;
                pOutPut += noffset;
            }

            b.UnlockBits(bData);
            OutPutImage.UnlockBits(copyData);

            return OutPutImage;
        }
    }
}
