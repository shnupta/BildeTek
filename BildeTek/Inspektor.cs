﻿using System;
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

            PixelFormat pf = i.PixelFormat;

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

            int width = i.Width;
            int height = i.Height;

            BildeData imageData = i.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, i.PixelFormat);

            

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();

            byte bitsPerPixel = (byte)i.BitsPerPixel;

            int bytes = imageData.Stride * height;

            int stride = i.Stride;

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
                    int location = y * stride + x * bitsPerPixel / 8;

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

                            byte* pixel = scan0 + cY * stride + cX * bitsPerPixel / 8;

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

            i.UnlockBits(imageData);

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

            int width = i.Width;
            int height = i.Height;

            BildeData imageData = i.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, i.PixelFormat);

            byte bitsPerPixel = (byte)i.BitsPerPixel;

            int bytes = imageData.Stride * height;

            int stride = i.Stride;

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
                    int location = y * stride + x * bitsPerPixel / 8;

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

                            byte* pixel = scan0 + cY * stride + cX * bitsPerPixel / 8;

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

            i.UnlockBits(imageData);

            return dataOut;
        }

        /// <summary>
        /// Converts the provided image to a greyscale image of the same pixel format.
        /// </summary>
        /// <param name="i"></param>
        /// <returns>A byte[] of the colour channel values.</returns>
        public static byte[] ConvertToGreyScale(Bilde i)
        {
            PixelFormat pf = i.PixelFormat;

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

        /// <summary>
        /// Converts the image into greyscale, returning only the value of each pixel (rather than a separate colour for each colour channel).
        /// </summary>
        /// <param name="i"></param>
        /// <returns>A byte[] of the colours represented if the image were 8bpp.</returns>
        public static byte[] GetGreyBytesOnly(Bilde i)
        {
            PixelFormat pf = i.PixelFormat;

            switch (pf)
            {
                case PixelFormat.Format24bppRgb:
                    return GetGreyBytesOnly24Bpp(i);
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppRgb:
                    return GetGreyBytesOnly32Bpp(i);
                default:
                    throw new Exception(String.Format("The pixel format {0} is not supported.", pf.ToString()));
            }
        }


        private unsafe static byte[] GetGreyBytesOnly24Bpp(Bilde i)
        {
            BildeData imageData = i.LockBits(new Rectangle(0, 0, i.Width, i.Height), ImageLockMode.ReadOnly, i.PixelFormat);

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();

            int width = i.Width;
            int height = i.Height;

            int stride = i.Stride;
            int bitsperpixel = i.BitsPerPixel;

            byte[] greyOut = new byte[i.Width * i.Height];

            byte r, g, b, grey;
            int index;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    index = y * stride + x * bitsperpixel / 8;

                    byte* data = scan0 + index;

                    b = *data;
                    g = *(data + 1);
                    r = *(data + 2);

                    grey = (byte)(b * 0.11 + g * 0.59 + r * 0.3);

                    greyOut[y * width + x] = grey;
                
                }
            }

            i.UnlockBits(imageData);

            return greyOut;
        }


        private unsafe static byte[] GetGreyBytesOnly32Bpp(Bilde i)
        {
            BildeData imageData = i.LockBits(new Rectangle(0, 0, i.Width, i.Height), ImageLockMode.ReadOnly, i.PixelFormat);

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();

            int width = i.Width;
            int height = i.Height;

            int stride = i.Stride;
            int bitsperpixel = i.BitsPerPixel;

            byte[] greyOut = new byte[i.Width * i.Height];

            byte r, g, b, grey;
            int index;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    index = y * stride + x * bitsperpixel / 8;

                    byte* data = scan0 + index;

                    b = *data;
                    g = *(data + 1);
                    r = *(data + 2);

                    grey = (byte)(b * 0.11 + g * 0.59 + r * 0.3);

                    greyOut[y * width + x] = grey;

                }
            }

            i.UnlockBits(imageData);

            return greyOut;
        }




        /// <summary>
        /// Converts the Bilde into a 24Bpp greyscale image.
        /// </summary>
        /// <param name="i"></param>
        /// <returns>A byte[] of RGB values.</returns>
        private unsafe static byte[] ConvertToGreyScale24Bpp(Bilde i)
        {
            BildeData imageData = i.LockBits(new Rectangle(0, 0, i.Width, i.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            int width = i.Width;
            int height = i.Height;

            int stride = i.Stride;
            int bitsperpixel = i.BitsPerPixel;

            byte[] greyOut = new byte[height * i.Stride];

            byte r, g, b;

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * bitsperpixel / 8;
                    byte* px = scan0 + index;

                    b = *px;
                    g = *(px + 1);
                    r = *(px + 2);

                    byte grey = (byte)(b * .11 + g * .59 + r * .3);

                    greyOut[index] = grey;
                    greyOut[index + 1] = grey;
                    greyOut[index + 2] = grey;
                }
            }

            i.UnlockBits(imageData);

            return greyOut;
        }

        /// <summary>
        /// Converts the Bilde into a 32Bpp greyscale image.
        /// </summary>
        /// <param name="i"></param>
        /// <returns>A byte[] of ARGB values.</returns>
        private unsafe static byte[] ConvertToGreyScale32Bpp(Bilde i) /////////////////////////// WAY TO SLOW, FIX THIS
        {
            BildeData imageData = i.LockBits(new Rectangle(0, 0, i.Width, i.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            int width = i.Width;
            int height = i.Height;
            int stride = i.Stride;
            int bitsperpixel = i.BitsPerPixel;

            byte[] greyOut = new byte[height * i.Stride];

            byte r, g, b;

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * bitsperpixel / 8;
                    byte* px = scan0 + index;

                    b = *px;
                    g = *(px + 1);
                    r = *(px + 2);

                    byte grey = (byte)(b * .11 + g * .59 + r * .3);

                    greyOut[index] = grey;
                    greyOut[index + 1] = grey;
                    greyOut[index + 2] = grey;
                    greyOut[index + 3] = 0xFF; // alpha channel
                }
            }

            i.UnlockBits(imageData);

            return greyOut;
        }


        public static byte[] Sobel(Bilde i)
        {
            PixelFormat pf = i.PixelFormat;

            switch (pf)
            {
                case PixelFormat.Format24bppRgb:
                    return Sobel24Bpp(i);
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppRgb:
                    return new byte[9]; // implement 32bpp option
                default:
                    throw new Exception(String.Format("The pixel format {0} is not supported.", pf.ToString()));
            }
        }


        private unsafe static byte[] Sobel24Bpp(Bilde i)
        {
            int width = i.Width;
            int height = i.Height;

            BildeData imageData = i.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, i.PixelFormat);

            int bitsPerPixel = i.BitsPerPixel;
            int stride = i.Stride;

            byte[] greyData = GetGreyBytesOnly(i);

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();

            byte r, g, b;

            // Buffers
            byte[] buffer = new byte[9];
            double[] magnitude = new double[width * height]; // Stores the magnitude of the edge response
            double[] orientation = new double[width * height]; // Stores the angle of the edge at that location


            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    // the first of 3 bgr colour bytes
                    byte* px = scan0 + y * stride + x * bitsPerPixel / 8;

                    b = *(px);
                    g = *(px + 1);
                    r = *(px + 2);

                    int index = y * width + x;

                    // 3x3 window around (x,y)
                    buffer[0] = greyData[index - width - 1];
                    buffer[1] = greyData[index - width];
                    buffer[2] = greyData[index - width + 1];
                    buffer[3] = greyData[index - 1];
                    buffer[4] = greyData[index];
                    buffer[5] = greyData[index + 1];
                    buffer[6] = greyData[index + width - 1];
                    buffer[7] = greyData[index + width];
                    buffer[8] = greyData[index + width + 1];

                    // Sobel horizontal and vertical response
                    double dx = buffer[2] + 2 * buffer[5] + buffer[8] - buffer[0] - 2 * buffer[3] - buffer[6];
                    double dy = buffer[6] + 2 * buffer[7] + buffer[8] - buffer[0] - 2 * buffer[1] - buffer[2];

                    magnitude[index] = Math.Sqrt(dx * dx + dy * dy); // 1141 is approximately the max sobel response, we will normalise later anyway

                    // Directional orientation
                    orientation[index] = Math.Atan2(dy, dx) + Math.PI; // Angle is in radians, now from 0 - 2PI. 

                }
            }


            ////////////////////////////////////////////////////////////////////////////// FIX

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte* px = scan0 + y * imageData.Stride + x * bitsPerPixel / 8;

                    b = *(px);
                    g = *(px + 1);
                    r = *(px + 2);

                    int index = y * width + x;

                    *px = (byte)magnitude[index];
                    *(px + 1) = (byte)magnitude[index];
                    *(px + 2) = (byte)magnitude[index];
                }
            }
            // unlock the image
            i.UnlockBits(imageData);



        }
    }
}
