﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using BildeTek;
using System.Resources;
using System.Reflection;

namespace Tests
{
    class Program
    {

        public static ResourceManager rm = new ResourceManager("Tests.images", Assembly.GetExecutingAssembly());


        static unsafe void Main(string[] args)
        {
            Test24BppConvolve();
            TestGetBytes();
            TestConvertToGreyScale24Bpp();
            TestSobel();
            TestGaussian();
            TestCanny();

            Console.WriteLine("All tests complete.");

            Console.ReadLine();
        }

        private unsafe static void Test24BppConvolve()
        {
            Console.WriteLine("Starting Convolve24Bpp() Test");
            DateTime start = DateTime.Now;

            Bilde i = new Bilde(rm.GetString("bike_in"));
            string outputPath = rm.GetString("out_path") + "bike.convolve.jpg";

            Console.WriteLine("Created Bilde in {0}", DateTime.Now - start);

            double[,] kernel = Kernel.MeanBlur;

            byte[] convolvedData = Inspektor.Convolve(i, kernel);
            Console.WriteLine("Bilde has been convolved in {0}", DateTime.Now - start);

            BildeData imageData = i.LockBits(new Rectangle(0, 0, i.Width, i.Height), ImageLockMode.ReadWrite, i.PixelFormat);

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();

            for (int y = 0; y < i.Height; y++)
            {
                for (int x = 0; x < i.Width * 3; x++) // * 3 as 4 bytes per pixel
                {
                    *(scan0 + y * imageData.Stride + x) = convolvedData[y * imageData.Stride + x];
                }
            }

            i.UnlockBits(imageData);
            i.Save(outputPath);

            TimeSpan duration = DateTime.Now - start;

            Console.WriteLine("Total took {0} milliseconds.\n", Math.Round(duration.TotalMilliseconds));
        }

        private static void TestGetBytes()
        {
            Console.WriteLine("Starting GetBytes() Test");
            DateTime start = DateTime.Now;

            Bilde i = new Bilde(rm.GetString("buzzard_in"));
            byte[] b = i.GetBytes();

            TimeSpan duration = DateTime.Now - start;

            Console.WriteLine("Total took {0} milliseconds.\n", Math.Round(duration.TotalMilliseconds));
        }


        private unsafe static void TestConvertToGreyScale24Bpp()
        {
            Console.WriteLine("Starting ConvertToGreyScale24Bpp() Test");
            DateTime start = DateTime.Now;

            Bilde i = new Bilde(rm.GetString("car_in"));

            byte[] greyData = Inspektor.GetGreyBytesOnly(i);
            Console.WriteLine("Retrieved greyscale data in {0}", DateTime.Now - start);

            string outputPath = rm.GetString("out_path") + "car.greyscale.jpg";

            BildeData imageData = i.LockBits(new Rectangle(0, 0, i.Width, i.Height), ImageLockMode.ReadWrite, i.PixelFormat);

            int height = i.Height;
            int width = i.Width;
            int stride = i.Stride;
            int bitsperpixel = i.BitsPerPixel;

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();


            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * bitsperpixel / 8;
                    byte* px = scan0 + index;

                    byte colour = greyData[y * width + x];

                    *px = colour;
                    *(px + 1) = colour;
                    *(px + 2) = colour;
                }
            }

            i.UnlockBits(imageData);
            i.Save(outputPath);

            TimeSpan duration = DateTime.Now - start;

            Console.WriteLine("Total took {0} milliseconds.\n", Math.Round(duration.TotalMilliseconds));
        }

        private static unsafe void TestSobel()
        {
            Console.WriteLine("Starting Sobel() Test");
            DateTime start = DateTime.Now;

            Bilde i = new Bilde(rm.GetString("cat_in"));

            byte[] sobelData = Inspektor.Sobel(i);
            Console.WriteLine("Retrieved sobel data in {0}", DateTime.Now - start);

            string outputPath = rm.GetString("out_path") + "cat.sobel.jpg";

            BildeData imageData = i.LockBits(new Rectangle(0, 0, i.Width, i.Height), ImageLockMode.ReadWrite, i.PixelFormat);

            int height = i.Height;
            int width = i.Width;
            int stride = i.Stride;
            int bitsperpixel = i.BitsPerPixel;

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();


            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * bitsperpixel / 8;
                    byte* px = scan0 + index;

                    byte colour = sobelData[y * width + x];

                    *px = colour;
                    *(px + 1) = colour;
                    *(px + 2) = colour;
                }
            }

            i.UnlockBits(imageData);
            i.Save(outputPath);

            TimeSpan duration = DateTime.Now - start;

            Console.WriteLine("Total took {0} milliseconds.\n", Math.Round(duration.TotalMilliseconds));
        }

        private unsafe static void TestGaussian()
        {
            Console.WriteLine("Starting Gaussian() Test");
            DateTime start = DateTime.Now;

            Bilde i = new Bilde(rm.GetString("car_in"));
            string outputPath = rm.GetString("out_path") + "car.gaussian.jpg";

            Console.WriteLine("Created Bilde in {0}", DateTime.Now - start);

            double[,] kernel = Kernel.GaussianBlur;

            byte[] convolvedData = Inspektor.Convolve(i, kernel);
            Console.WriteLine("Bilde has been convolved in {0}", DateTime.Now - start);

            BildeData imageData = i.LockBits(new Rectangle(0, 0, i.Width, i.Height), ImageLockMode.ReadWrite, i.PixelFormat);

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();

            for (int y = 0; y < i.Height; y++)
            {
                for (int x = 0; x < i.Width * 3; x++) // * 3 as 4 bytes per pixel
                {
                    *(scan0 + y * imageData.Stride + x) = convolvedData[y * imageData.Stride + x];
                }
            }

            i.UnlockBits(imageData);
            i.Save(outputPath);

            TimeSpan duration = DateTime.Now - start;

            Console.WriteLine("Total took {0} milliseconds.\n", Math.Round(duration.TotalMilliseconds));
        }

        private static unsafe void TestCanny()
        {

            Console.WriteLine("Starting Canny() test.");

            DateTime start = DateTime.Now;

            Bilde i = new Bilde(rm.GetString("bike_in"));

            byte[] afterCanny = Inspektor.Canny(i);

            Console.WriteLine("Retrived canny data in {0}", DateTime.Now - start);

            string outputPath = (rm.GetString("out_path") + "bike.canny.jpg");

            BildeData imageData = i.LockBits(new Rectangle(0, 0, i.Width, i.Height), ImageLockMode.ReadWrite, i.PixelFormat);

            int height = i.Height;
            int width = i.Width;
            int stride = i.Stride;
            int bitsperpixel = i.BitsPerPixel;

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();


            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * bitsperpixel / 8;
                    byte* px = scan0 + index;

                    byte colour = afterCanny[y * width + x];

                    *px = colour;
                    *(px + 1) = colour;
                    *(px + 2) = colour;
                }
            }

            i.UnlockBits(imageData);
            i.Save(outputPath);

            TimeSpan duration = DateTime.Now - start;

            Console.WriteLine("Total took {0} milliseconds.\n", Math.Round(duration.TotalMilliseconds));
        }
    }
}
