using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// This file was created for INFOIBV 2011/12 by Paul Scharf (3683745)

namespace INFOIBV
{
    /// <summary>
    /// This algorithm inverts the RGB channels of any given bitmap
    /// </summary>
    class NegativeAlgorithm : Algorithm
    {
        public override Output Execute(System.Drawing.Bitmap input, bool debug = false, bool showSteps = false)
        {
            Output o = new Output();
            RgbByteArrayBitmap rgb = RgbByteArrayBitmap.FromBitmap((input));
            //progressBarSetMax(100); // we are so fast, we dont need no progress bar(we are also not showing debug or steps)
            //int step = rgb.Length / 100;
            for (int i = 0; i < rgb.Length; i++)
            {
                rgb[i] = (byte)(255 - rgb[i]);
                //if ((i % step) == 0)
                //    progressBarStep();
            }
            o.Add(rgb.ToBitmap(), "negative");
            return o;
        }

        /// <summary>
        /// Name of the algorithm
        /// </summary>
        public override string ToString()
        {
            return "Negative";
        }
    }
}
