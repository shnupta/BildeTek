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
    /// Bilde is a Bitmap/Image wrapper for the BildeTek library.
    /// </summary>
    public class Bilde 
    {

        private Bitmap m_Image;
        public int BitsPerPixel
        {
            get
            {
                return GetBitsPerPixel();
            }
        }

        public int Height
        {
            get
            {
                return m_Image.Height;
            }
        }

        public int Width
        {
            get
            {
                return m_Image.Width;
            }
        }

        /// <summary>
        /// The number of bytes in of one row of pixels.
        /// </summary>
        public int Stride
        {
            get
            {
                return m_Image.Width * BitsPerPixel / 8;
            }
        }

        /// <summary>
        /// Basic constructor for a Bilde.
        /// </summary>
        /// <param name="filename">Path to the image file.</param>
        public Bilde(string filename)
        {
            m_Image = new Bitmap(filename);
        }

        /// <summary>
        /// Simple function to access the Bitmap type the current instance is wrapping.
        /// </summary>
        /// <returns>Bitmap image.</returns>
        public Bitmap GetBitmap()
        {
            return m_Image;
        }

        /// <summary>
        /// Gets the byte data for the pixels of the image.
        /// </summary>
        /// <returns>Returns a byte[] of all the (A)RGB values for the image. N.B. These may not be in RGB order.</returns>
        public unsafe byte[] GetBytes()
        {
            BitmapData imageData = m_Image.LockBits(new Rectangle(0, 0, m_Image.Width, m_Image.Height), ImageLockMode.ReadOnly, m_Image.PixelFormat);

            int bytes = imageData.Stride * m_Image.Height;

            int bitsPerPixel = (imageData.Stride / m_Image.Width) * 8;

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();

            byte[] dataOut = new byte[bytes];

            for (int y = 0; y < m_Image.Height; y++)
            {
                for (int x = 0; x < (m_Image.Width * (bitsPerPixel / 8)); x++)
                {
                    dataOut[y * imageData.Stride + x] = *(scan0 + y * imageData.Stride + x);
                }
            }

            m_Image.UnlockBits(imageData);

            return dataOut;

        }


        private int GetBitsPerPixel()
        {
            switch (m_Image.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    return 24;
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                    return 32;
                default:
                    throw new Exception(String.Format("Pixel format {0} is not supported.", m_Image.PixelFormat));
            }
        }


        public void LockBits(Rectangle rect, ImageLockMode ilm, PixelFormat pf)
        {
             
        }


    }
}
