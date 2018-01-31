using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace BildeTek
{
    /// <summary>
    /// The Inspektor is the main processing class. It can perform kernel convolution, sobel edge detection, conversion to greyscale and more.
    /// </summary>
    public static class Inspektor
    {
        /// <summary>
        /// Takes the image and runs the provided kernel over and returns the pixel data from the convolution.
        /// </summary>
        /// <param name="image">The image to be convolved.</param>
        /// <param name="kernel">The kernel to run over the image.</param>
        /// <returns>A byte[] of pixel colour values.</returns>
        public unsafe static byte[] Convolve(Bilde i, double[,] kernel, int radius = 0)
        {

            if (kernel.GetLength(0) != kernel.GetLength(1))
            {
                throw new Exception("Convolve only works with square kernels.");
            }

            if (kernel.GetLength(0) % 2 == 0)
            {
                throw new Exception("The kernel must have an odd length, else there is no centre pixel.");
            }

            Bitmap image = i.GetBitmap();
            PixelFormat pf = image.PixelFormat;

            switch (pf)
            {
                case PixelFormat.Format24bppRgb:
                    return Convolve24Bpp(i, kernel, radius);
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppRgb:
                    return Convolve32Bpp(i, kernel, radius);
                default:
                    throw new Exception(String.Format("The pixel format {0} is not supported.", pf.ToString()));
            }
        }

        /// <summary>
        /// Convolves a 24 bits per pixel image.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="kernel"></param>
        /// <returns>A byte[] of BGR values.</returns>
        private unsafe static byte[] Convolve24Bpp(Bilde i, double[,] kernel, int radius)
        {
            Bitmap image = i.GetBitmap();

            int width = image.Width;
            int height = image.Height;

            BitmapData imageData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, image.PixelFormat);

            

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();

            byte bitsPerPixel = (byte)((imageData.Stride / width) * 8);

            int bytes = imageData.Stride * height;

            byte[] dataOut = new byte[bytes];

            int kernelSize = kernel.GetLength(0);

            // as the kernel has already been checked by Convolve it's safe to continue

            // 24bpp is BGR not RGB
            byte b, g, r;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double bT = 0.0, gT = 0.0, rT = 0.0, kT = 0.0;
                    int location = y * imageData.Stride + x * bitsPerPixel / 8;

                    for (int v = 0; v < kernelSize; v++)
                    {
                        for (int u = 0; u < kernelSize; u++)
                        {
                            int cX = x + u - radius;
                            int cY = y + v - radius;

                            // We must make sure that the position isn't off the image
                            if (cX < 0 || cX > width - 1 || cY < 0 || cY > height - 1)
                            {
                                continue;
                            }

                            byte* pixel = scan0 + cY * imageData.Stride + cX * bitsPerPixel / 8;

                            b = *pixel;
                            g = *(pixel + 1);
                            r = *(pixel + 2);

                            bT += b * kernel[u, v];
                            gT += g * kernel[u, v];
                            rT += r * kernel[u, v];
                            kT += kernel[u, v];
                        }
                    }

                    b = (byte)(bT / kT + 0.5);
                    g = (byte)(gT / kT + 0.5);
                    r = (byte)(rT / kT + 0.5);

                    dataOut[location] = b;
                    dataOut[location + 1] = g;
                    dataOut[location + 2] = r;

                }
            }

            image.UnlockBits(imageData);

            return dataOut;

        }

        /// <summary>
        /// Convolves a 32 bits per pixel image.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="kernel"></param>
        /// <returns>A byte[] of ARGB pixel values.</returns>
        private unsafe static byte[] Convolve32Bpp(Bilde i, double[,] kernel, int radius)
        {
            Bitmap image = i.GetBitmap();

            int width = image.Width;
            int height = image.Height;

            BitmapData imageData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, image.PixelFormat);

            byte bitsPerPixel = (byte)((imageData.Stride / width) * 8);

            int bytes = imageData.Stride * height;

            byte[] dataOut = new byte[bytes];

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();

            int kernelSize = kernel.GetLength(0);

            // as the kernel has already been checked by Convolve it's safe to continue

            // 32bpp is ARGB
            byte a, r, g, b;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double bT = 0.0, gT = 0.0, rT = 0.0, kT = 0.0;
                    int location = y * imageData.Stride + x * bitsPerPixel / 8;

                    for (int v = 0; v < kernelSize; v++)
                    {
                        for (int u = 0; u < kernelSize; u++)
                        {
                            int cX = x + u - radius;
                            int cY = y + v - radius;

                            // We must make sure that the position isn't off the image
                            if (cX < 0 || cX > width - 1 || cY < 0 || cY > height - 1)
                            {
                                continue;
                            }

                            byte* pixel = scan0 + cY * imageData.Stride + cX * bitsPerPixel / 8;

                            b = *pixel;
                            g = *(pixel + 1);
                            r = *(pixel + 2);
                            a = *(pixel + 3);

                            bT += b * kernel[u, v];
                            gT += g * kernel[u, v];
                            rT += r * kernel[u, v];
                            kT += kernel[u, v];
                        }
                    }

                    b = (byte)(bT / kT + 0.5);
                    g = (byte)(gT / kT + 0.5);
                    r = (byte)(rT / kT + 0.5);

                    dataOut[location] = b; 
                    dataOut[location + 1] = g;
                    dataOut[location + 2] = r;
                    dataOut[location + 3] = 0xFF; // we ignore the alpha channel here

                }
            }

            image.UnlockBits(imageData);

            return dataOut;
        }


        public static byte[] ConvertToGreyScale(Bilde i)
        {
            Bitmap image = i.GetBitmap();
            PixelFormat pf = image.PixelFormat;

            switch (pf)
            {
                case PixelFormat.Format24bppRgb:
                    return ConvertToGreyScale24Bpp(i);
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppRgb:
                    return ConvertToGreyScale32Bpp(i);
                default:
                    throw new Exception(String.Format("The pixel format {0} is not supported.", pf.ToString()));
            }
        }

        private unsafe static byte[] ConvertToGreyScale24Bpp(Bilde i)
        {
            Bitmap image = i.GetBitmap();

            //BitmapData imageData = image.LockBits(new Rectangle)

            int width = image.Width;
            int height = image.Height;

            byte r, g, b;

            

            return new byte[9];
        }

        private unsafe static byte[] ConvertToGreyScale32Bpp(Bilde i)
        {
            Bitmap image = i.GetBitmap();

            int width = image.Width;
            int height = image.Height;

            byte[] imageData = i.GetBytes();

            byte[] greyOut = new byte[imageData.Length];

            byte r, g, b;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < i.Stride; x += 4)
                {
                    b = imageData[y * i.Stride + x];
                    g = imageData[y * i.Stride + x + 1];
                    r = imageData[y * i.Stride + x + 2];

                    byte grey = (byte)(b * .11 + g * .59 + r * .3);

                    greyOut[y * i.Stride + x] = grey;
                    greyOut[y * i.Stride + x + 1] = grey;
                    greyOut[y * i.Stride + x + 2] = grey;
                    greyOut[y * i.Stride + x + 3] = 0xFF;


                }
            }

            return greyOut;
        }
    }
}
