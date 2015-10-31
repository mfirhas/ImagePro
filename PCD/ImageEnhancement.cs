using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PCD
{
    public class ImageEnhancement
    {

        public byte[] ImageBytes;
        public int RowSizeBytes;
        public const int PixelDataSize = 32;

        public Bitmap Bitmap;

        private bool _IsLocked = false;
        public bool IsLocked
        {
            get
            {
                return _IsLocked;
            }
        }

        public ImageEnhancement(Bitmap bm)
        {
            Bitmap = bm;
        }

        private BitmapData m_BitmapData;

        public int Width
        {
            get
            {
                return Bitmap.Width;
            }
        }
        public int Height
        {
            get
            {
                return Bitmap.Height;
            }
        }

        public void GetPixel(int x, int y, out byte red, out byte green, out byte blue, out byte alpha)
        {
            int i = y * m_BitmapData.Stride + x * 4;
            blue = ImageBytes[i++];
            green = ImageBytes[i++];
            red = ImageBytes[i++];
            alpha = ImageBytes[i];
        }
        public void SetPixel(int x, int y, byte red, byte green, byte blue, byte alpha)
        {
            int i = y * m_BitmapData.Stride + x * 4;
            ImageBytes[i++] = blue;
            ImageBytes[i++] = green;
            ImageBytes[i++] = red;
            ImageBytes[i] = alpha;
        }
        public byte GetBlue(int x, int y)
        {
            int i = y * m_BitmapData.Stride + x * 4;
            return ImageBytes[i];
        }
        public void SetBlue(int x, int y, byte blue)
        {
            int i = y * m_BitmapData.Stride + x * 4;
            ImageBytes[i] = blue;
        }
        public byte GetGreen(int x, int y)
        {
            int i = y * m_BitmapData.Stride + x * 4;
            return ImageBytes[i + 1];
        }
        public void SetGreen(int x, int y, byte green)
        {
            int i = y * m_BitmapData.Stride + x * 4;
            ImageBytes[i + 1] = green;
        }
        public byte GetRed(int x, int y)
        {
            int i = y * m_BitmapData.Stride + x * 4;
            return ImageBytes[i + 2];
        }
        public void SetRed(int x, int y, byte red)
        {
            int i = y * m_BitmapData.Stride + x * 4;
            ImageBytes[i + 2] = red;
        }
        public byte GetAlpha(int x, int y)
        {
            int i = y * m_BitmapData.Stride + x * 4;
            return ImageBytes[i + 3];
        }
        
        public void LockBitmap()
        {
            if (IsLocked) return;

            Rectangle bounds = new Rectangle(
                0, 0, Bitmap.Width, Bitmap.Height);
            m_BitmapData = Bitmap.LockBits(bounds,
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            RowSizeBytes = m_BitmapData.Stride;

            int total_size = m_BitmapData.Stride * m_BitmapData.Height;
            ImageBytes = new byte[total_size];

            Marshal.Copy(m_BitmapData.Scan0, ImageBytes, 0, total_size);

            _IsLocked = true;
        }

        public void UnlockBitmap()
        {
            if (!IsLocked) return;

            int total_size = m_BitmapData.Stride * m_BitmapData.Height;
            Marshal.Copy(ImageBytes, 0, m_BitmapData.Scan0, total_size);

            Bitmap.UnlockBits(m_BitmapData);

            ImageBytes = null;
            m_BitmapData = null;

            _IsLocked = false;
        }

        
        

        
        

        
        

        

        

        #region "FilterStuff"

        public class Filter
        {
            public float[,] Kernel;
            public float Weight, Offset;

            
            public void Normalize()
            {
                Weight = 0;
                for (int row = 0; row <= Kernel.GetUpperBound(0); row++)
                {
                    for (int col = 0; col <= Kernel.GetUpperBound(1); col++)
                    {
                        Weight += Kernel[row, col];
                    }
                }
            }

            
            public void ZeroKernel()
            {
                float total = 0;
                for (int row = 0; row <= Kernel.GetUpperBound(0); row++)
                {
                    for (int col = 0; col <= Kernel.GetUpperBound(1); col++)
                    {
                        total += Kernel[row, col];
                    }
                }

                int row_mid = (int)(Kernel.GetUpperBound(0) / 2);
                int col_mid = (int)(Kernel.GetUpperBound(1) / 2);
                total -= Kernel[row_mid, col_mid];
                Kernel[row_mid, col_mid] = -total;
            }
        }

        public ImageEnhancement Clone()
        {
            bool was_locked = this.IsLocked;

            this.LockBitmap();

            ImageEnhancement result = (ImageEnhancement)this.MemberwiseClone();

            result.Bitmap = new Bitmap(this.Bitmap.Width, this.Bitmap.Height);
            result._IsLocked = false;

            if (!was_locked) this.UnlockBitmap();

            return result;
        }

        
        
        public static Filter LowFilter
        {
            get
            {
                Filter result = new Filter()
                {
                    Offset = 0,
                    Kernel = new float[,]
                    {
                        {1,  4,  7,  4, 1},
                        {4, 16, 26, 16, 4},
                        {7, 26, 41, 26, 7},
                        {4, 16, 26, 16, 4},
                        {1,  4,  7,  4, 1},
                    }
                };
                result.Normalize();
                return result;
            }
        }

        

        

        public static Filter HighPassFilter
        {
            get
            {
                return new Filter()
                {
                    Weight = 16,
                    Offset = 127,
                    Kernel = new float[,]
                    {
                        {-1, -2, -1},
                        {-2, 12, -2},
                        {-1, -2, -1},
                    }
                };
            }
        }

        public static Filter BlurFilter5x5Mean
        {
            get
            {
                Filter result = new Filter()
                {
                    Offset = 0,
                    Kernel = new float[,]
                    {
                        {0, 0, 0, 0, 0},
                        {0, 1, 1, 1, 0},
                        {0, 1, 0, 1, 0},
                        {0, 1, 1, 1, 0},
                        {0, 0, 0, 0, 0},
                    }
                };
                result.Normalize();
                return result;
            }
        }

        public ImageEnhancement ApplyFilter(Filter filter, bool lock_result)
        {
            ImageEnhancement result = this.Clone();

            bool was_locked = this.IsLocked;
            this.LockBitmap();
            result.LockBitmap();

            int xoffset = -(int)(filter.Kernel.GetUpperBound(1) / 2);
            int yoffset = -(int)(filter.Kernel.GetUpperBound(0) / 2);
            int xmin = -xoffset;
            int xmax = Bitmap.Width - filter.Kernel.GetUpperBound(1);
            int ymin = -yoffset;
            int ymax = Bitmap.Height - filter.Kernel.GetUpperBound(0);
            int row_max = filter.Kernel.GetUpperBound(0);
            int col_max = filter.Kernel.GetUpperBound(1);

            for (int x = xmin; x <= xmax; x++)
            {
                for (int y = ymin; y <= ymax; y++)
                {
                    
                    bool skip_pixel = false;

                    float red = 0, green = 0, blue = 0;
                    for (int row = 0; row <= row_max; row++)
                    {
                        for (int col = 0; col <= col_max; col++)
                        {
                            int ix = x + col + xoffset;
                            int iy = y + row + yoffset;
                            byte new_red, new_green, new_blue, new_alpha;
                            this.GetPixel(ix, iy, out new_red, out new_green, out new_blue, out new_alpha);

                            if (new_alpha == 0)
                            {
                                skip_pixel = true;
                                break;
                            }

                            red   += new_red   * filter.Kernel[row, col];
                            green += new_green * filter.Kernel[row, col];
                            blue  += new_blue  * filter.Kernel[row, col];
                        }
                        if (skip_pixel) break;
                    }

                    if (!skip_pixel)
                    {
                        
                        red = filter.Offset + red / filter.Weight;
                        if (red < 0) red = 0;
                        if (red > 255) red = 255;

                        green = filter.Offset + green / filter.Weight;
                        if (green < 0) green = 0;
                        if (green > 255) green = 255;

                        blue = filter.Offset + blue / filter.Weight;
                        if (blue < 0) blue = 0;
                        if (blue > 255) blue = 255;

                        result.SetPixel(x, y, (byte)red, (byte)green, (byte)blue,
                            this.GetAlpha(x, y));
                    }
                }
            }

            if (!lock_result) result.UnlockBitmap();
            if (!was_locked) this.UnlockBitmap();

            return result;

#endregion
        }

    }
}
