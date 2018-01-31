using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BildeTek
{
    /// <summary>
    /// Kernel is a collection of commonly used kernels for convolution. This is simply used as a quick way to setup a convolution.
    /// </summary>
    public static class Kernel
    {
        public static double[,] LensBlur
        {
            get
            {
                return new double[,] 
                { 
                    { 0, 0, 0, 1, 0, 0, 0 },
                    { 0, 1, 1, 1, 1, 1, 0 },
                    { 0, 1, 1, 1, 1, 1, 0 },
                    { 1, 1, 1, 1, 1, 1, 1 },
                    { 0, 1, 1, 1, 1, 1, 0 },
                    { 0, 1, 1, 1, 1, 1, 0 },
                    { 0, 0, 0, 1, 0, 0, 0 }
                };
            }
        }

        public static double[,] MeanBlur
        {
            get
            {
                return new double[,]
                {
                    { 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1 }
                };
            }
        }

        public static double[,] SobelX
        {
            get
            {
                return new double[,]
                {
                    {1, 0, -1 },
                    {2, 0, -2 },
                    {1, 0, -1 }
                };
            }
        }

        public static double[,] SobelY
        {
            get
            {
                return new double[,]
                {
                    {1, 2, 1 },
                    {0, 0, 0 },
                    {-1, -2, -1 }
                };
            }
        }
    }
}
