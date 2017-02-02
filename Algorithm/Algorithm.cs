using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

// This file was created for INFOIBV 2011/12 by Paul Scharf (3683745)

namespace INFOIBV
{
    /// <summary>
    /// This is the abstract class grouping all algorithms processing images in this application
    /// </summary>
    abstract class Algorithm
    {
        private ProgressBar pBar;
        private PictureBox pBox;

        /// <summary>
        /// Give the algorithm a picture box, so it can display progress information
        /// </summary>
        /// <param name="pictureBox"></param>
        public void GivePreviewPictureBox(PictureBox pictureBox)
        {
            pBox = pictureBox;
        }


        /// <summary>
        /// give the algorithm a progress bar, to show progress information
        /// </summary>
        public void GiveProgressBar(ProgressBar progressBar)
        {
            pBar = progressBar;
        }

        /// <summary>
        /// Executes the algorithm on the given input image with given parameters
        /// Returns an object of class Output, containing output images
        /// </summary>
        virtual public Output Execute(Bitmap input, bool debug = false, bool showSteps = false)
        {
            Output o = new Output();
            o.Add(input, "original image");
            return o;
        }

        /// <summary>
        /// Internal method to set the maximum value of the given progressbar
        /// </summary>
        protected void progressBarSetMax(int max)
        {
            if (pBar != null)
                pBar.Maximum = max;
        }

        /// <summary>
        /// Internal method to advance the progressbar by one step
        /// </summary>
        protected void progressBarStep()
        {
            if (pBar != null)
                pBar.PerformStep();
        }

        /// <summary>
        /// Internal progressbar to display an image as preview
        /// </summary>
        protected void setPreviewBitmap(Bitmap b)
        {
            setPreviewBitmap(b, null);
        }

        /// <summary>
        /// Internal progressbar to display an image as preview, invluding a caption
        /// </summary>
        protected void setPreviewBitmap(Bitmap b, string caption)
        {
            setPreviewBitmap(b, caption, Color.White);
        }

        /// <summary>
        /// Internal progressbar to display an image as preview, invluding a caption in a given colour
        /// </summary>
        protected void setPreviewBitmap(Bitmap b, string caption, Color colour)
        {
            if (pBox != null)
            {
                if (caption != null)
                {
                    Graphics g = Graphics.FromImage(b);
                    g.DrawString(caption, new Font("Arial", 10, FontStyle.Regular), new SolidBrush(colour), new Point(2, 2));
                }
                pBox.Image = (Image)b;
                pBox.Refresh();
            }
        }

        /// <summary>
        /// Name of the algorithm
        /// </summary>
        public override string ToString()
        {
            return "unnamed Algorithm";
        }
    }
}
