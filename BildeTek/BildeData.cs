using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;


namespace BildeTek
{
    public class BildeData
    {
        public BitmapData m_ImageData
        {
            get; set;
        }

        public BildeData()
        {
        }

        public int Height
        {
            get
            {
                return m_ImageData.Height;
            }
        }

        public int Width
        {
            get
            {
                return m_ImageData.Width;
            }
        }

        public int Stride
        {
            get
            {
                return m_ImageData.Stride;
            }
        }

        public PixelFormat PixelFormat
        {
            get
            {
                return m_ImageData.PixelFormat;
            }
        }

        public IntPtr Scan0
        {
            get
            {
                return m_ImageData.Scan0;
            }
        }

        public int Reserved
        {
            get
            {
                return m_ImageData.Reserved;
            }
        }
    }
}
