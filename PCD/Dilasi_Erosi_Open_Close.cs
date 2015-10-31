using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace PCD
{
    public static class Dilasi_Erosi_Open_Close
    {

        public static Bitmap CopyToSquareCanvas(this Bitmap sourceBitmap, int canvasWidthLenght)
        {
            float ratio = 1.0f;
            int maxSide = sourceBitmap.Width > sourceBitmap.Height ?
                          sourceBitmap.Width : sourceBitmap.Height;

            ratio = (float)maxSide / (float)canvasWidthLenght;

            Bitmap bitmapResult = (sourceBitmap.Width > sourceBitmap.Height ?
                                    new Bitmap(canvasWidthLenght, (int)(sourceBitmap.Height / ratio))
                                    : new Bitmap((int)(sourceBitmap.Width / ratio), canvasWidthLenght));

            using (Graphics graphicsResult = Graphics.FromImage(bitmapResult))
            {
                graphicsResult.CompositingQuality = CompositingQuality.HighQuality;
                graphicsResult.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsResult.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphicsResult.DrawImage(sourceBitmap,
                                        new Rectangle(0, 0,
                                            bitmapResult.Width, bitmapResult.Height),
                                        new Rectangle(0, 0,
                                            sourceBitmap.Width, sourceBitmap.Height),
                                            GraphicsUnit.Pixel);
                graphicsResult.Flush();
            }

            return bitmapResult;
        }
        public static Bitmap Dilate(Bitmap SrcImage)
        {
            
            Bitmap tempbmp = new Bitmap(SrcImage.Width, SrcImage.Height);

            BitmapData SrcData = SrcImage.LockBits(new Rectangle(0, 0, SrcImage.Width, SrcImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            BitmapData DestData = tempbmp.LockBits(new Rectangle(0, 0, tempbmp.Width, tempbmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            byte[,] sElement = new byte[5, 5] { 
												{0,0,1,0,0},
												{0,1,1,1,0},
												{1,1,1,1,1},
												{0,1,1,1,0},
												{0,0,1,0,0}
											  };

            int size = 5;
            int ir, jr;
            int radius = size / 2;
            byte max, clrValue;

            unsafe
            {
                for (int colm = radius; colm < DestData.Height - radius; colm++)
                {
                    byte* ptr = (byte*)SrcData.Scan0 + (colm * SrcData.Stride);
                    byte* dstPtr = (byte*)DestData.Scan0 + (colm * SrcData.Stride);

                    for (int row = radius; row < DestData.Width - radius; row++)
                    {
                        max = 0;
                        clrValue = 0;

                        for (int eleColm = 0; eleColm < 5; eleColm++)
                        {
                            ir = eleColm - radius;
                            byte* tempPtr = (byte*)SrcData.Scan0 + ((colm + ir) * SrcData.Stride);

                            for (int eleRow = 0; eleRow < 5; eleRow++)
                            {
                                jr = eleRow - radius;

                                clrValue = (byte)((tempPtr[row * 3 + jr] + tempPtr[row * 3 + jr + 1] + tempPtr[row * 3 + jr + 2]) / 3);

                                if (max < clrValue)
                                {
                                    if (sElement[eleColm, eleRow] != 0)
                                        max = clrValue;
                                }

                            }
                        }

                        dstPtr[0] = dstPtr[1] = dstPtr[2] = max;

                        ptr += 3;
                        dstPtr += 3;
                    }
                }
            }

            SrcImage.UnlockBits(SrcData);
            tempbmp.UnlockBits(DestData);

            return tempbmp;
        }

        public static Bitmap OpenMorphologyFilter(this Bitmap sourceBitmap,int matrixSize,bool applyBlue = true,bool applyGreen = true,bool applyRed = true)
        {
            Bitmap resultBitmap = sourceBitmap.DilateAndErodeFilter(matrixSize,
                                                        MorphologyType.Erosion,
                                               applyBlue, applyGreen, applyRed);

            resultBitmap = resultBitmap.DilateAndErodeFilter(matrixSize,
                                                MorphologyType.Dilation,
                                               applyBlue, applyGreen, applyRed);

            return resultBitmap;
        }

        public static Bitmap CloseMorphologyFilter(this Bitmap sourceBitmap,
                                                   int matrixSize,
                                                   bool applyBlue = true,
                                                   bool applyGreen = true,
                                                   bool applyRed = true)
        {
            Bitmap resultBitmap = sourceBitmap.DilateAndErodeFilter(matrixSize,
                                                        MorphologyType.Dilation,
                                                applyBlue, applyGreen, applyRed);

            resultBitmap = resultBitmap.DilateAndErodeFilter(matrixSize,
                                                 MorphologyType.Erosion,
                                                applyBlue, applyGreen, applyRed);

            return resultBitmap;
        }

        public static Bitmap DilateAndErodeFilter(this Bitmap sourceBitmap,
                                                int matrixSize,
                                                MorphologyType morphType,
                                                bool applyBlue = true,
                                                bool applyGreen = true,
                                                bool applyRed = true)
        {
            BitmapData sourceData =
                       sourceBitmap.LockBits(new Rectangle(0, 0,
                       sourceBitmap.Width, sourceBitmap.Height),
                       ImageLockMode.ReadOnly,
                       PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride *
                                          sourceData.Height];

            byte[] resultBuffer = new byte[sourceData.Stride *
                                           sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0,
                                       pixelBuffer.Length);

            sourceBitmap.UnlockBits(sourceData);

            int filterOffset = (matrixSize - 1) / 2;
            int calcOffset = 0;

            int byteOffset = 0;

            byte blue = 0;
            byte green = 0;
            byte red = 0;

            byte morphResetValue = 0;

            if (morphType == MorphologyType.Erosion)
            {
                morphResetValue = 255;
            }

            for (int offsetY = filterOffset; offsetY <
                sourceBitmap.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX <
                    sourceBitmap.Width - filterOffset; offsetX++)
                {
                    byteOffset = offsetY *
                                 sourceData.Stride +
                                 offsetX * 4;

                    blue = morphResetValue;
                    green = morphResetValue;
                    red = morphResetValue;

                    if (morphType == MorphologyType.Dilation)
                    {
                        for (int filterY = -filterOffset;
                            filterY <= filterOffset; filterY++)
                        {
                            for (int filterX = -filterOffset;
                                filterX <= filterOffset; filterX++)
                            {
                                calcOffset = byteOffset +
                                             (filterX * 4) +
                                (filterY * sourceData.Stride);

                                if (pixelBuffer[calcOffset] > blue)
                                {
                                    blue = pixelBuffer[calcOffset];
                                }

                                if (pixelBuffer[calcOffset + 1] > green)
                                {
                                    green = pixelBuffer[calcOffset + 1];
                                }

                                if (pixelBuffer[calcOffset + 2] > red)
                                {
                                    red = pixelBuffer[calcOffset + 2];
                                }
                            }
                        }
                    }
                    else if (morphType == MorphologyType.Erosion)
                    {
                        for (int filterY = -filterOffset;
                            filterY <= filterOffset; filterY++)
                        {
                            for (int filterX = -filterOffset;
                                filterX <= filterOffset; filterX++)
                            {

                                calcOffset = byteOffset +
                                             (filterX * 4) +
                                (filterY * sourceData.Stride);

                                if (pixelBuffer[calcOffset] < blue)
                                {
                                    blue = pixelBuffer[calcOffset];
                                }

                                if (pixelBuffer[calcOffset + 1] < green)
                                {
                                    green = pixelBuffer[calcOffset + 1];
                                }

                                if (pixelBuffer[calcOffset + 2] < red)
                                {
                                    red = pixelBuffer[calcOffset + 2];
                                }
                            }
                        }
                    }

                    if (applyBlue == false)
                    {
                        blue = pixelBuffer[byteOffset];
                    }

                    if (applyGreen == false)
                    {
                        green = pixelBuffer[byteOffset + 1];
                    }

                    if (applyRed == false)
                    {
                        red = pixelBuffer[byteOffset + 2];
                    }

                    resultBuffer[byteOffset] = blue;
                    resultBuffer[byteOffset + 1] = green;
                    resultBuffer[byteOffset + 2] = red;
                    resultBuffer[byteOffset + 3] = 255;
                }
            }

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width,
                                             sourceBitmap.Height);

            BitmapData resultData =
                       resultBitmap.LockBits(new Rectangle(0, 0,
                       resultBitmap.Width, resultBitmap.Height),
                       ImageLockMode.WriteOnly,
                       PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0,
                                       resultBuffer.Length);

            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        public enum MorphologyType
        {
            Dilation,
            Erosion
        }
    }
}
