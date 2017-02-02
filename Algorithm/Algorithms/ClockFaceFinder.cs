using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

// This file was created for INFOIBV 2011/12 by Paul Scharf (3683745)

namespace INFOIBV
{
    /// <summary>
    /// This Algorithm searches for clock faces in pictures
    /// </summary>
    class ClockFaceFinderAlgorithm : Algorithm
    {
        /// <summary>
        /// Applies the algorithm to a bitmap
        /// </summary>
        public override Output Execute(Bitmap input, bool debug = false, bool showSteps = false)
        {
            // create an output object to later add output (and debug) images to
            Output o = new Output();
            // convert the input to an RgbByteArrayBitmap for faster processing
            RgbByteArrayBitmap rgb = RgbByteArrayBitmap.FromBitmap((input));

            // set preview image, anouncing that processing has begone
            setPreviewBitmap((Bitmap)input.Clone(), "processing..", Color.Red);

            // calculate the number of radii later tested and their relative spacing(f)
            int min_r = 10;
            int radii = Math.Min(Math.Min(rgb.Width, rgb.Height) / 4, 40);
            double f = Math.Max(1, (Math.Min(rgb.Width, rgb.Height) / 6.0 - min_r) / radii);

            // set progressbar maximum
            progressBarSetMax(radii * 2);

            // create a dilation kernel
            Kernel max = new KernelMax();
            max.AddCircleElements(2.5, 1);
            
            // create a median kernel
            Kernel median = new KernelMedian();
            median.IgnoreBorders = false;
            median.AddCircleElements(1.5, 1);

            // create a simple averaging kernel
            Kernel smooth = new Kernel();
            smooth.AddCircleElements(1.5, 1);
            smooth.IgnoreBorders = false;

            // create a x-derivative kernel
            Kernel dx = new Kernel();
            dx.AddElement(-1, 0, 1);
            dx.AddElement(1, 0, -1);

            // create a y-derivative kernel
            Kernel dy = new Kernel();
            dy.AddElement(0, -1, 1);
            dy.AddElement(0, 1, -1);

            // set up some variables
            DoubleArrayBitmap result;
            DoubleArrayBitmap resultDx;
            DoubleArrayBitmap resultDy;

            // convert byte array bitmap to hsv
            result = rgb.ToHSV();

            // extract brightness channel
            DoubleArrayBitmap brightness = result.ExtractChannel(2);

            // if preview is enabled, show the brightness channel
            if (showSteps)
                setPreviewBitmap(brightness.ToBytes().ToBitmap(), "grey values", Color.Red);

            // taking median and smoothing brightness to reduce noise
            brightness = median.ApplyToBitmap(brightness);
            brightness = smooth.ApplyToBitmap(brightness);

            // show progress
            if (showSteps)
                setPreviewBitmap(brightness.ToBytes().ToBitmap(), "reduced noise", Color.Red);

            // add brightness image to debug output
            if (debug)
                o.Add(brightness.ToBytes().ToBitmap(), "debug: brightness");

            // calculate derivatives
            resultDx = dx.ApplyToBitmap(brightness);
            resultDy = dy.ApplyToBitmap(brightness);

            // square derivatives
            resultDx.SquareMe();
            resultDy.SquareMe();

            // calculate gradient
            DoubleArrayBitmap gradient = resultDx + resultDy;
            gradient.SqrtMe();

            // show progress
            if (showSteps)
                setPreviewBitmap(gradient.ToBytes().ToBitmap(), "gradient");

            // dilate gradient slightly, improves detection
            gradient = max.ApplyToBitmap(gradient);

            // show progress
            if (showSteps)
                setPreviewBitmap(gradient.ToBytes().ToBitmap(), "dilated gradient");

            // add gradient to debug output
            if (debug)
                o.Add(gradient.ToBytes().ToBitmap(), "debug: gradient");

            // reduce size of gradient image, making detection faster by a factor of 4
            gradient = gradient.ShrinkByTwo();


            // initializing array to hold detected features for different radii
            DoubleArrayBitmap[] steps = new DoubleArrayBitmap[radii];

            // detect features for the different radii
            for (int r = 0; r < radii; r++)
            {
                // update progressbar
                progressBarStep();
                // create 'special' 24-point clockface kernel(see makeClockFaceKernel-method)
                Kernel clockface = makeClockFaceKernel(min_r + (int)(r * f));
                // apply the kernel to detect features
                steps[r] = clockface.ApplyToBitmap(gradient);
                // square the resulting image to make both negative and positive features stand out
                steps[r].SquareMe();

                // show progress
                if (showSteps)
                    setPreviewBitmap(steps[r].ExpandByTwo().ToBytes().ToBitmap(), "pattern detection " + r + "/" + radii);
            }

            // find maximum over all features in xyr-space
            double maximum = 0;
            for (int r = 0; r < radii; r++)
            {
                for (int i = 0; i < gradient.Width * gradient.Height; i++)
                {
                    if (steps[r][0, i] > maximum)
                        maximum = steps[r][0, i];
                }
            }

            // initialize another array of bitmaps to hold the final smoothed features
            DoubleArrayBitmap[] steps_final = new DoubleArrayBitmap[radii];

            // create a new bitmap to sum up all steps_final for debug output
            if (debug)
                result = new DoubleArrayBitmap(gradient.Width, gradient.Height);

            // initialize 3d-array to hold thresholded object information extracted from detected features
            bool[,,] xyrSpace = new bool[gradient.Width, gradient.Height, radii];

            // for all radii...
            for (int r = 0; r < radii; r++)
            {
                int a = Math.Max(r - 2, 0);
                int b = Math.Min(r + 2, radii);
                steps_final[r] = new DoubleArrayBitmap(gradient.Width, gradient.Height);

                // sum up detected features over up to 5 r-layers ( smoothes features )
                for (int x = 0; x < gradient.Width * gradient.Height; x++)
                    for (int i = a; i < b; i++)
                        steps_final[r][0, x] += steps[i][0, x];

                // theshold r-layer if needed for preview/debug-output
                if (showSteps || debug)
                    steps_final[r].ThresholdMe(maximum);

                // show smoothed and thresholded preview of r-layer
                if (showSteps)
                    setPreviewBitmap(steps_final[r].ExpandByTwo().ToBytes().ToBitmap(), "threshold " + r + "/" + radii);

                // threshold r-layer and store in xyrSpace as boolean
                for (int x = 0; x < gradient.Width; x++)
                    for (int y = 0; y < gradient.Height; y++)
                        xyrSpace[x, y, r] = steps_final[r][0, x, y] > maximum;

                // add thresholded r-layer to debug output
                if (debug)
                {
                    result += steps_final[r];
                    o.Add(steps_final[r].ExpandByTwo().ToBytes().ToBitmap(), "debug: radius " + (min_r + (int)(r * f)).ToString());
                }
                // update progress bar
                progressBarStep();
            }
            // debug output sum of thresholded r-layers
            if (debug)
                o.Add(result.ExpandByTwo().ToBytes().ToBitmap(), "debug: sum(all radii)"); // debug

            // boolean-xyrSpace is filled now!

            // initialzed a list of detected clock faces
            ClockFaceList clList = new ClockFaceList();
            //clList.AllowOverlap = true; // enabling this will detect many smaller features that are not really clock faces

            // search through xyrSpace from large to small radius
            for (int r = radii - 1; r >= 0; r--)
                for (int x = 0; x < gradient.Width; x++)
                    for (int y = 0; y < gradient.Height; y++)
                    {
                        if (xyrSpace[x, y, r])
                        {
                            // if object is found, extract and add to list
                            int[] center = extractObject(ref xyrSpace, x, y, r);
                            // perform some funny computations to offset previous resolution change and take into acount radius factor f
                            clList.Add(center[0] * 2, center[1] * 2, (int)((min_r + center[2] * f) * 2 * 1.1));
                        }
                    }


            // create final output bitmaps
            // "output" shows all detected clock faces in different gray values on black
            // "mask" shows the original image masked with the detected clock faces, leaving everything else black
            Bitmap output = new Bitmap(input.Width, input.Height);
            Graphics g = Graphics.FromImage(output);
            Bitmap mask = new Bitmap(input);
            Graphics g_m = Graphics.FromImage(mask);
            g.Clear(Color.Black);
            g_m.Clear(Color.Black);
            int clId = clList.ClockFaces.Count;
            foreach (ClockFace cl in clList.ClockFaces)
            {
                int c = clId * 255 / clList.ClockFaces.Count;
                Brush b = new SolidBrush(Color.FromArgb(c, c, c));
                Point p = new Point(cl.X - cl.R, cl.Y - cl.R);
                Size s = new Size(cl.R * 2, cl.R * 2);
                g.FillEllipse(b, new Rectangle(p, s));
                b = new SolidBrush(Color.White);
                g_m.FillEllipse(b, new Rectangle(p, s));
                clId--;
            }

            // add final outputs to output list
            o.Add(output, "detected clock faces");
            // also perform bitwise AND operation with input bitmap here to get masked image
            o.Add((RgbByteArrayBitmap.FromBitmap(mask) & rgb).ToBitmap(), "masked input");

            return o;
        }

        /// <summary>
        /// Extracts an object that has a pixel at x,y,r from the xyrSpace
        /// All pixels belonging to the object are set to 0 and the center of mass of the object is returned
        /// </summary>
        private int[] extractObject(ref bool[,,] xyrSpace, int x, int y, int r)
        {
            // the method basically finds all connected pixels using a simple "flood-fill"-type algorithm
            int[] l = new int[3] { xyrSpace.GetLength(0), xyrSpace.GetLength(1), xyrSpace.GetLength(2) };
            int count = 0;
            int[] total = new int[3];
            Queue<int[]> queue = new Queue<int[]>();
            queue.Enqueue(new int[3] {x, y, r});
            while (queue.Count > 0)
            {
                int[] p = queue.Dequeue();
                if (p[0] >= 0 && p[1] >= 0 && p[2] >= 0 && p[0] < l[0] && p[1] < l[1] && p[2] < l[2])
                    if (xyrSpace[p[0], p[1], p[2]])
                    {
                        queue.Enqueue(new int[3] { p[0] + 1, p[1], p[2] });
                        queue.Enqueue(new int[3] { p[0], p[1] + 1, p[2] });
                        queue.Enqueue(new int[3] { p[0] - 1, p[1], p[2] });
                        queue.Enqueue(new int[3] { p[0], p[1] - 1, p[2] });
                        queue.Enqueue(new int[3] { p[0], p[1], p[2] + 1 });
                        queue.Enqueue(new int[3] { p[0], p[1], p[2] - 1});

                        xyrSpace[p[0], p[1], p[2]] = false;

                        // adds coordinates of extracted pixel to a total count
                        total[0] += p[0];
                        total[1] += p[1];
                        total[2] += p[2];
                        count++;
                    }
            }
            // devides total coordinates by amoutn of pixels extracted to get center of mass
            total[0] /= count;
            total[1] /= count;
            total[2] /= count;
            return total;
        }

        /// <summary>
        /// This mathod creates a kernel optimized to detect clock faces of a given radius(at a given offset angle)
        /// </summary>
        private Kernel makeClockFaceKernel(double radius, double offset = 0)
        {
            Kernel k = new Kernel();
            //k.IgnoreBorders = false;
            int sign = 1;
            double b = Math.PI * 1/36.0 * 0.5;
            for (int i = 0; i < 24; i++)
            {
                // the kernel includes 24 bunches of elements, with alternating sine
                // this detects the alternating bright and dark spaces formed by the
                // gradient of the numerals/dots around the clock face
                double a = Math.PI * (i+offset) / 12.0;
                int x = (int)(Math.Cos(a) * radius);
                int y = (int)(Math.Sin(a) * radius);
                k.AddElement(x, y, 2 *sign);
                k.AddElement(x, y, 2 * sign);
                k.AddElement(x, y, 2 * sign);

                x = (int)(Math.Cos(a + b) * radius);
                y = (int)(Math.Sin(a + b) * radius);
                k.AddElement(x, y, sign);

                x = (int)(Math.Cos(a - b) * radius);
                y = (int)(Math.Sin(a - b) * radius);
                k.AddElement(x, y, sign);

                sign = -sign;
            }
            return k;
        }

        /// <summary>
        /// The name of the algorithm
        /// </summary>
        public override string ToString()
        {
            return "Find Clock Faces";
        }
    }

    /// <summary>
    /// A simple class to keep a list of clock faces and has a flag to only add new ones that are not overlapping others
    /// </summary>
    class ClockFaceList
    {
        /// <summary>
        /// The actuall list of clock faces
        /// </summary>
        public LinkedList<ClockFace> ClockFaces = new LinkedList<ClockFace>();

        /// <summary>
        /// Wether to allow overlapping
        /// </summary>
        public bool AllowOverlap = false;

        /// <summary>
        /// Adds a new clock face
        /// </summary>
        public bool Add(int x, int y, int r)
        {
            ClockFace cl = new ClockFace(x, y, r);
            if (!AllowOverlap)
                foreach (ClockFace cl2 in ClockFaces)
                {
                    if (cl.DistanceTo(cl2) < (cl.R + cl2.R))
                    {
                        return false;
                    }
                }
            ClockFaces.AddLast(cl);
            return true;
        }
    }

    /// <summary>
    /// Class representing a clock face
    /// </summary>
    class ClockFace
    {
        /// <summary>
        /// X coordinate
        /// </summary>
        public int X {get; private set;}

        /// <summary>
        /// Y coordinate
        /// </summary>
        public int Y {get; private set;}

        /// <summary>
        /// Radius of clock face
        /// </summary>
        public int R {get; private set;}

        /// <summary>
        /// Constructs a new clock face
        /// </summary>
        public ClockFace(int x, int y, int r)
        {
            X = x;
            Y = y;
            R = r;
        }

        /// <summary>
        /// Calculates the distance to another clock face
        /// </summary>
        public double DistanceTo(ClockFace c)
        {
            return Math.Sqrt((c.X - X) * (c.X - X) + (c.Y - Y) * (c.Y - Y));
        }
    }
}
