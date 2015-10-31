using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace PCD
{
    public class Sampling
    {

        public Bitmap Sample(Bitmap image, Rectangle rectangle, Int32 samplingSize)
        {
            Bitmap pixelated = new System.Drawing.Bitmap(image.Width, image.Height);


            using (Graphics graphics = System.Drawing.Graphics.FromImage(pixelated)) graphics.DrawImage(image, new System.Drawing.Rectangle(0, 0, image.Width, image.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);



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
    }
}
