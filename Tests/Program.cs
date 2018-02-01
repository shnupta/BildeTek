using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using BildeTek;

namespace Tests
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            //Test24BppConvolve();
            //TestGetBytes();
            TestConvertToGreyScale24Bpp();

            Console.WriteLine("All tests complete.");

            Console.ReadLine();
        }

        private unsafe static void Test24BppConvolve()
        {
            Console.WriteLine("Starting Convolve24Bpp() Test");
            DateTime start = DateTime.Now;

            Bilde i = new Bilde(@"N:\My Documents\Computer Science\Other Coding\butterfly.jpg");
            string outputPath = (@"N:\My Documents\Computer Science\Other Coding\butterflyBilde.conv.out.jpg");

            Console.WriteLine("Created Bilde in {0}", DateTime.Now - start);

            double[,] kernel = new double[,] { { 0, 0, 0, 1, 0, 0, 0 },
                                               { 0, 1, 1, 1, 1, 1, 0 },
                                               { 0, 1, 1, 1, 1, 1, 0 },
                                               { 1, 1, 1, 1, 1, 1, 1 },
                                               { 0, 1, 1, 1, 1, 1, 0 },
                                               { 0, 1, 1, 1, 1, 1, 0 },
                                               { 0, 0, 0, 1, 0, 0, 0 } };

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

            Console.WriteLine("Total took {0}\n", DateTime.Now - start);
        }

        private static void TestGetBytes()
        {
            Console.WriteLine("Starting GetBytes() Test");
            DateTime start = DateTime.Now;

            Bilde i = new Bilde(@"N:\My Documents\Computer Science\Other Coding\butterfly.jpg");
            byte[] b = i.GetBytes();

            TimeSpan duration = DateTime.Now - start;

            Console.WriteLine("Total took {0} milliseconds.\n", Math.Round(duration.TotalMilliseconds));
        }


        private unsafe static void TestConvertToGreyScale24Bpp()
        {
            Console.WriteLine("Starting ConvertToGreyScale24Bpp() Test");
            DateTime start = DateTime.Now;

            Bilde i = new Bilde(@"N:\My Documents\Computer Science\Other Coding\butterfly.jpg");

            byte[] greyData = Inspektor.ConvertToGreyScale(i);
            Console.WriteLine("Retrieved greyscale data in {0}", DateTime.Now - start);

            string outputPath = (@"N:\My Documents\Computer Science\Other Coding\butterflyBilde.grey.out.jpg");

            BildeData imageData = i.LockBits(new Rectangle(0, 0, i.Width, i.Height), ImageLockMode.ReadWrite, i.PixelFormat);

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();


            for (int y = 0; y < i.Height; y++)
            {
                for (int x = 0; x < i.Width; x++)
                {
                    int index = y * imageData.Stride + x * i.BitsPerPixel / 8;
                    byte* px = scan0 + index;

                    byte colour = greyData[index];

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
