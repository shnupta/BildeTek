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
            Test24BppConvolve();
            TestGetBytes();
            TestConvertToGreyScale24Bpp();

            Console.WriteLine("All tests complete.");

            Console.ReadLine();
        }

        private unsafe static void Test24BppConvolve()
        {
            Console.WriteLine("Starting 24Bpp Convolution Test");
            DateTime start = DateTime.Now;

            Bilde i = new Bilde(@"C:\Users\Casey\Downloads\rubik.jpg");
            string outputPath = (@"C:\Users\Casey\Downloads\rubik.conv.jpg");

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

            Bitmap image = i.GetBitmap();
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width * 3; x++) // * 3 as 4 bytes per pixel
                {
                    *(scan0 + y * imageData.Stride + x) = convolvedData[y * imageData.Stride + x];
                }
            }

            image.UnlockBits(imageData);
            image.Save(outputPath);

            Console.WriteLine("Total took {0}\n", DateTime.Now - start);
        }

        private static void TestGetBytes()
        {
            Console.WriteLine("Starting GetBytes() Test");
            DateTime start = DateTime.Now;

            Bilde i = new Bilde(@"C:\Users\Casey\Downloads\rubik.jpg");
            byte[] b = i.GetBytes();

            Console.WriteLine("Total took {0}\n", DateTime.Now - start);
        }


        private unsafe static void TestConvertToGreyScale24Bpp()
        {
            Console.WriteLine("Starting ConvertToGreyScale24Bpp() Test");
            DateTime start = DateTime.Now;

            Bilde i = new Bilde(@"C:\Users\Casey\Downloads\rubik.jpg");

            byte[] greyData = Inspektor.ConvertToGreyScale(i);
            Console.WriteLine("Retrieved greyscale data in {0}", DateTime.Now - start);

            Bitmap image = i.GetBitmap();

            string outputPath = (@"C:\Users\Casey\Downloads\rubikNEW.grey.jpg");

            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();


            for (int y = 0; y < i.Height; y++)
            {
                for (int x = 0; x < i.Stride; x++)
                {
                    *(scan0 + y * i.Stride + x) = greyData[y * i.Stride + x];
                }
            }

            image.UnlockBits(imageData);
            image.Save(outputPath);

            Console.WriteLine("Total took {0}\n", DateTime.Now - start);
        }
    }
}
