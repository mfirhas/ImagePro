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
    public class Warping
    {
        public byte[] ImageBytes;
        public int RowSizeBytes;
        public const int PixelDataSize = 32;
        
         
        public Bitmap Bitmap;

        private bool m_IsLocked = false;
        public bool IsLocked
        {
            get
            {
                return m_IsLocked;
            }
        }

        public Warping(Bitmap bm)
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

        #region ga perlu(mungkin nanti perlu)
        //public byte GetBlue(int x, int y)
        //{
        //    int i = y * m_BitmapData.Stride + x * 4;
        //    return ImageBytes[i];
        //}
        //public void SetBlue(int x, int y, byte blue)
        //{
        //    int i = y * m_BitmapData.Stride + x * 4;
        //    ImageBytes[i] = blue;
        //}
        //public byte GetGreen(int x, int y)
        //{
        //    int i = y * m_BitmapData.Stride + x * 4;
        //    return ImageBytes[i + 1];
        //}
        //public void SetGreen(int x, int y, byte green)
        //{
        //    int i = y * m_BitmapData.Stride + x * 4;
        //    ImageBytes[i + 1] = green;
        //}
        //public byte GetRed(int x, int y)
        //{
        //    int i = y * m_BitmapData.Stride + x * 4;
        //    return ImageBytes[i + 2];
        //}
        //public void SetRed(int x, int y, byte red)
        //{
        //    int i = y * m_BitmapData.Stride + x * 4;
        //    ImageBytes[i + 2] = red;
        //}
        //public byte GetAlpha(int x, int y)
        //{
        //    int i = y * m_BitmapData.Stride + x * 4;
        //    return ImageBytes[i + 3];
        //}
        //public void SetAlpha(int x, int y, byte alpha)
        //{
        //    int i = y * m_BitmapData.Stride + x * 4;
        //    ImageBytes[i + 3] = alpha;
        //}
        #endregion

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

            m_IsLocked = true;
        }

        public void UnlockBitmap()
        {
            if (!IsLocked) return;

            int total_size = m_BitmapData.Stride * m_BitmapData.Height;
            Marshal.Copy(ImageBytes, 0, m_BitmapData.Scan0, total_size);

            Bitmap.UnlockBits(m_BitmapData);

            ImageBytes = null;
            m_BitmapData = null;

            m_IsLocked = false;
        }

        #region ga perlu(mungkin nanti perlu)
        //public void Average()
        //{
        //    bool was_locked = IsLocked;
        //    LockBitmap();

        //    for (int y = 0; y < Height; y++)
        //    {
        //        for (int x = 0; x < Width; x++)
        //        {
        //            byte red, green, blue, alpha;
        //            GetPixel(x, y, out red, out green, out blue, out alpha);
        //            byte gray = (byte)((red + green + blue) / 3);
        //            SetPixel(x, y, gray, gray, gray, alpha);
        //        }
        //    }

        //    if (!was_locked) UnlockBitmap();
        //}

        //public void Grayscale()
        //{
        //    bool was_locked = IsLocked;
        //    LockBitmap();

        //    for (int y = 0; y < Height; y++)
        //    {
        //        for (int x = 0; x < Width; x++)
        //        {
        //            byte red, green, blue, alpha;
        //            GetPixel(x, y, out red, out green, out blue, out alpha);
        //            byte gray = (byte)(0.3 * red + 0.5 * green + 0.2 * blue);
        //            SetPixel(x, y, gray, gray, gray, alpha);
        //        }
        //    }

        //    if (!was_locked) UnlockBitmap();
        //}

        //public void ClearRed()
        //{
        //    bool was_locked = IsLocked;
        //    LockBitmap();

        //    for (int y = 0; y < Height; y++)
        //    {
        //        for (int x = 0; x < Width; x++)
        //        {
        //            SetRed(x, y, 0);
        //        }
        //    }

        //    if (!was_locked) UnlockBitmap();
        //}

        //public void ClearGreen()
        //{
        //    bool was_locked = IsLocked;
        //    LockBitmap();

        //    for (int y = 0; y < Height; y++)
        //    {
        //        for (int x = 0; x < Width; x++)
        //        {
        //            SetGreen(x, y, 0);
        //        }
        //    }

        //    if (!was_locked) UnlockBitmap();
        //}

        //public void ClearBlue()
        //{
        //    bool was_locked = IsLocked;
        //    LockBitmap();

        //    for (int y = 0; y < Height; y++)
        //    {
        //        for (int x = 0; x < Width; x++)
        //        {
        //            SetBlue(x, y, 0);
        //        }
        //    }

        //    if (!was_locked) UnlockBitmap();
        //}

        //public void Invert()
        //{
        //    bool was_locked = IsLocked;
        //    LockBitmap();

        //    for (int y = 0; y < Height; y++)
        //    {
        //        for (int x = 0; x < Width; x++)
        //        {
        //            byte red = (byte)(255 - GetRed(x, y));
        //            byte green = (byte)(255 - GetGreen(x, y));
        //            byte blue = (byte)(255 - GetBlue(x, y));
        //            byte alpha = GetAlpha(x, y);
        //            SetPixel(x, y, red, green, blue, alpha);
        //        }
        //    }

        //    if (!was_locked) UnlockBitmap();
        //}
        #endregion

        #region "FilterStuff"

        #region ga perlu(mungkin nanti perlu)
        // A public class to represent filters.
        public class Filter
        {
            public float[,] Kernel;
            public float Weight, offset;

            // Set the filter's weight equal to the sum
            // of the kernel's values.
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

            // Set the valeu of the center kernel coefficient
            // so the kernel has a zero total.
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
        #endregion

        
        public Warping Clone()
        {
            
            bool was_locked = this.IsLocked;

            this.LockBitmap();

            Warping result = (Warping)this.MemberwiseClone();

            result.Bitmap = new Bitmap(this.Bitmap.Width, this.Bitmap.Height);
            result.m_IsLocked = false;

            if (!was_locked) this.UnlockBitmap();

            return result;
        }

        
        #endregion // FilterStuff

        #region Warping Methods

        public enum WarpOperations
        {
            Identity,
            FishEye,
            Twist,
            Wave,
            SmallTop,
            Wiggles,
            DoubleWave,
        }

        public Warping Warp(WarpOperations warp_op, bool lock_result)
        {
            Warping result = this.Clone();

            bool was_locked = this.IsLocked;
            this.LockBitmap();
            result.LockBitmap();

            WarpImage(this, result, warp_op);

            if (!lock_result) result.UnlockBitmap();
            if (!was_locked) this.UnlockBitmap();

            return result;
        }

        private static void WarpImage(Warping bm_src, Warping bm_dest, WarpOperations warp_op)
        {
            double xmid = bm_dest.Width / 2.0;
            double ymid = bm_dest.Height / 2.0;
            double rmax = bm_dest.Width * 0.75;

            int ix_max = bm_src.Width - 2;
            int iy_max = bm_src.Height - 2;

            double x0, y0;
            for (int y1 = 0; y1 < bm_dest.Height; y1++)
            {
                for (int x1 = 0; x1 < bm_dest.Width; x1++)
                {
                    MapPixel(warp_op, xmid, ymid, rmax, x1, y1, out x0, out y0);

                    int ix0 = (int)x0;
                    int iy0 = (int)y0;

                    if ((ix0 < 0) || (ix0 > ix_max) ||
                        (iy0 < 0) || (iy0 > iy_max))
                    {
                        bm_dest.SetPixel(x1, y1, 255, 255, 255, 255);
                    }
                    else
                    {
                        double dx0 = x0 - ix0;
                        double dy0 = y0 - iy0;
                        double dx1 = 1 - dx0;
                        double dy1 = 1 - dy0;

                        byte r00, g00, b00, a00, r01, g01, b01, a01,
                             r10, g10, b10, a10, r11, g11, b11, a11;
                        bm_src.GetPixel(ix0, iy0, out r00, out g00, out b00, out a00);
                        bm_src.GetPixel(ix0, iy0 + 1, out r01, out g01, out b01, out a01);
                        bm_src.GetPixel(ix0 + 1, iy0, out r10, out g10, out b10, out a10);
                        bm_src.GetPixel(ix0 + 1, iy0 + 1, out r11, out g11, out b11, out a11);

                        int r = (int)(
                            r00 * dx1 * dy1 + r01 * dx1 * dy0 +
                            r10 * dx0 * dy1 + r11 * dx0 * dy0);
                        int g = (int)(
                            g00 * dx1 * dy1 + g01 * dx1 * dy0 +
                            g10 * dx0 * dy1 + g11 * dx0 * dy0);
                        int b = (int)(
                            b00 * dx1 * dy1 + b01 * dx1 * dy0 +
                            b10 * dx0 * dy1 + b11 * dx0 * dy0);
                        int a = (int)(
                            a00 * dx1 * dy1 + a01 * dx1 * dy0 +
                            a10 * dx0 * dy1 + a11 * dx0 * dy0);
                        bm_dest.SetPixel(x1, y1, (byte)r, (byte)g, (byte)b, (byte)a);
                    }
                }
            }
        }

        private static void MapPixel(WarpOperations warp_op, double xmid, double ymid, double rmax, int x1, int y1, out double x0, out double y0)
        {
            const double pi_over_2 = Math.PI / 2.0;
            const double K = 100.0;
            const double offset = -pi_over_2;
            double dx, dy, r1, r2;

            switch (warp_op)
            {
                case WarpOperations.Identity:
                    x0 = x1;
                    y0 = y1;
                    break;
                case WarpOperations.FishEye:
                    dx = x1 - xmid;
                    dy = y1 - ymid;
                    r1 = Math.Sqrt(dx * dx + dy * dy);
                    if (r1 == 0)
                    {
                        x0 = xmid;
                        y0 = ymid;
                    }
                    else
                    {
                        r2 = rmax / 2 * (1 / (1 - r1 / rmax) - 1);
                        x0 = dx * r2 / r1 + xmid;
                        y0 = dy * r2 / r1 + ymid;
                    }
                    break;
                case WarpOperations.Twist:
                    dx = x1 - xmid;
                    dy = y1 - ymid;
                    r1 = Math.Sqrt(dx * dx + dy * dy);
                    if (r1 == 0)
                    {
                        x0 = 0;
                        y0 = 0;
                    }
                    else
                    {
                        double theta = Math.Atan2(dx, dy) - r1 / K - offset;
                        x0 = r1 * Math.Cos(theta);
                        y0 = r1 * Math.Sin(theta);
                    }
                    x0 = x0 + xmid;
                    y0 = y0 + ymid;
                    break;
                case WarpOperations.Wave:
                    x0 = x1;
                    y0 = y1 - 10 * (Math.Sin(x1 / 50.0 * Math.PI) + 1) + 5;
                    break;
                case WarpOperations.SmallTop:
                    dx = xmid - x1;
                    dx = dx * ymid * 2 / (y1 + 1);
                    x0 = xmid - dx;
                    y0 = y1;
                    break;
                case WarpOperations.Wiggles:
                    dx = xmid - x1;
                    dx = dx * (Math.Sin(y1 / 50.0 * Math.PI) / 2 + 1.5);
                    x0 = xmid - dx;
                    y0 = y1;
                    break;
                case WarpOperations.DoubleWave:
                    x0 = x1 - 10 * (Math.Sin(y1 / 50.0 * Math.PI) + 1) + 5;
                    y0 = y1 - 10 * (Math.Sin(x1 / 50.0 * Math.PI) + 1) + 5;
                    break;
                default:    
                    x0 = 2 * xmid - x1;
                    y0 = 2 * ymid - y1;
                    break;
            }
        }

        #endregion Warping Methods
    }
}
