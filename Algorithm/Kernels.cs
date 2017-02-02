using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// This file was created for INFOIBV 2011/12 by Paul Scharf (3683745)

namespace INFOIBV
{
    /// <summary>
    /// This class includes functionality to perform morphological operations on DoubleArrayBitmaps
    /// </summary>
    class Kernel
    {
        protected const int elementX = 0, elementY = 1, elementF = 2;
        protected double defaultPixel = 0;

        /// <summary>
        /// Wether the kernel should always be applied normalized(with a total weight of 1)
        /// </summary>
        public bool Normalize = true;
        /// <summary>
        /// Wether pixels for which the kernel intersects the border should simply be set to the default
        /// </summary>
        public bool IgnoreBorders = true;
        /// <summary>
        /// A constant factor for all weigths
        /// </summary>
        public Double Factor = 1;

        protected LinkedList<int[]> elements = new LinkedList<int[]>();

        /// <summary>
        /// Adds an element with local coordinates x,y and specified weight f to the kernel
        /// (overrides any other element at x,y)
        /// </summary>
        public void AddElement(int x, int y, int f)
        {
            RemoveElementAt(x, y);
            int[] e = { x, y, f };
            elements.AddLast(e);
        }

        /// <summary>
        /// Removes any element at x,y
        /// </summary>
        public void RemoveElementAt(int x, int y)
        {
            LinkedListNode<int[]> node = elements.First;
            while (node != null)
            {
                if (node.Value[elementX] == x && node.Value[elementY] == y)
                {
                       elements.Remove(node);
                    break;
                }
                node = node.Next;
            }
        }

        /// <summary>
        /// Applies the kernel to a given bitmap
        /// </summary>
        public virtual DoubleArrayBitmap ApplyToBitmap(DoubleArrayBitmap bitmap)
        {
            // apply elements to all channels
            double[][] rgb = new double[bitmap.Channels.Length][];
            for (int i = 0; i < bitmap.Channels.Length; i++)
                rgb[i] = applyToChannel(bitmap.Channels[i], bitmap.Width, bitmap.Height);
            return new DoubleArrayBitmap(rgb, bitmap.Width, bitmap.Height);
        }

        /// <summary>
        /// Applies the kernel to a given channel
        /// </summary>
        protected virtual double[] applyToChannel(double[] input, int w, int h)
        {
            // apply elements to all pixels
            double[] output = new double[input.Length];
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    output[x + y * w] = applyElementsToPixel(input, x, y, w, h);
            return output;
        }

        /// <summary>
        /// Applies the kernel to a single pixel inside a channel
        /// </summary>
        protected virtual double applyElementsToPixel(double[] input, int x, int y, int w, int h)
        {
            int totalWeight = 0;
            double newPixel = defaultPixel;
            bool first = true;

            // for all elements in the kernel...
            foreach (int[] e in elements)
            {
                // calculate coordinates of element wit offset x,y
                int px = x + e[elementX];
                int py = y + e[elementY];
                // if element lies within channel
                if (px >= 0 && px < w && py >= 0 && py < h)
                {
                    // calculate index in channel for faster access
                    double pixel = input[px + py * w];
                    // apply element to pixel
                    pixel = applyElementToPixel(pixel, e[elementF]);
                    if (first)
                    {   // if this is the first pixel, do special operation
                        newPixel = mergePixelsFirst(pixel);
                        first = false;
                    }
                    else
                        // if is not first pixel, to normal operation
                        newPixel = mergePixels(newPixel, pixel);
                    // collect weight of applied elements for normilisation
                    totalWeight += Math.Abs(e[elementF]);
                }
                // if element outside of channel and IgnoreBorders is set, return default pixel
                else if (IgnoreBorders)
                {
                    return defaultPixel;
                }
            }
            // if no non-zero elements where applied, return default pixel
            if (totalWeight == 0)
            {
                return defaultPixel;
            }
            // apply cosntant factor
            newPixel *= Factor;
            // if set, normilise
            if (Normalize)
            {
                newPixel /= totalWeight;
            }
            // return new pixel
            return newPixel;
        }

        /// <summary>
        /// Applies one element to one pixel
        /// </summary>
        protected virtual double applyElementToPixel(double pixel, int f)
        {
            return pixel * f;
        }

        /// <summary>
        /// Applies possible special operation on first pixel
        /// </summary>
        protected virtual double mergePixelsFirst(double p)
        {
            return p;
        }

        /// <summary>
        /// Merges new pixel into existing one
        /// </summary>
        protected virtual double mergePixels(double p1, double p2)
        {
            return p1 + p2;
        }

        /// <summary>
        /// Adds a rectangle of elements width weight f to the kernel
        /// </summary>
        public void AddRectangleElements(int x_from, int y_from, int width, int height, int f)
        {
            for (int x = x_from; x < x_from + width; x++)
                for (int y = y_from; y < y_from + height; y++)
                    AddElement(x, y, f);
        }

        /// <summary>
        /// Adds a circle of elements around 0,0 with a given radius and weight f to the kernel
        /// </summary>
        public void AddCircleElements(double radius, int f)
        {
            int from = (int)Math.Ceiling(radius);
            for (int x = -from; x < from + 1; x++)
                for (int y = -from; y < from + 1; y++)
                    if (Math.Sqrt(x * x + y * y) <= radius)
                        AddElement(x, y, f);
        }

        /// <summary>
        /// Adds a circle of elements around a specified center and with a given radius and weight f to the kernel
        /// </summary>
        public void AddCircleElements(int c_x, int c_y, double radius, int f)
        {
            int from = (int)Math.Ceiling(radius);
            for (int x = -from; x < from + 1; x++)
                for (int y = -from; y < from + 1; y++)
                    if (Math.Sqrt(x * x + y * y) <= radius)
                        AddElement(c_x + x, c_y + y, f);
        }
    }


    /// <summary>
    /// This kernel takes the maximum of all convoluted pixels
    /// </summary>
    class KernelMax : Kernel
    {
        protected override double mergePixels(double sp1, double sp2)
        {
            return Math.Max(sp1, sp2);
        }
    }

    /// <summary>
    /// This kernel takes the minimum of all convoluted pixels
    /// </summary>
    class KernelMin : Kernel
    {
        protected override double mergePixels(double sp1, double sp2)
        {
            return Math.Min(sp1, sp2);
        }
    }

    /// <summary>
    /// This abstract class changes Kernel to allow for a wider variety of behaviours
    /// </summary>
    abstract class KernelListingPixels : Kernel
    {
        new public bool Normalize = false;

        protected override double applyElementsToPixel(double[] input, int x, int y, int w, int h)
        {
            int totalWeight = 0;
            // initialize list of pixels
            List<double> pixels = new List<double>(elements.Count);
            // for all elements in the kernel...
            foreach (int[] e in elements)
            {
                // calculate coordinates of element wit offset x,y
                int px = x + e[elementX];
                int py = y + e[elementY];
                // if element lies within channel
                if (px >= 0 && px < w && py >= 0 && py < h)
                {
                    // calculate index in channel for faster access
                    double pixel = input[px + py * w];
                    // apply element to pixel
                    pixel = applyElementToPixel(pixel, e[elementF]);
                    // add pixel to list, later to be merged
                    pixels.Add(pixel);
                    // collect weight of applied elements for normilisation
                    totalWeight += Math.Abs(e[elementF]);
                }
                // if element outside of channel and IgnoreBorders is set, return default pixel
                else if (IgnoreBorders)
                {
                    return defaultPixel;
                }
            }
            // if no non-zero elements where applied, return default pixel
            if (totalWeight == 0)
            {
                return defaultPixel;
            }
            // merge list into output pixel
            double newPixel = mergePixelList(pixels);
            // apply constant factor
            newPixel *= Factor;
            // if set, normilise
            if (Normalize)
            {
                newPixel /= totalWeight;
            }
            // return new pixel
            return newPixel;
        }


        /// <summary>
        /// This abstract mathod has to be overriden to merge the gathered list of pixels into one
        /// </summary>
        abstract protected double mergePixelList(List<double> pixels);
    }

    /// <summary>
    /// This kernel takes the median of all convoluted pixels
    /// </summary>
    class KernelMedian : KernelListingPixels
    {
        /// <summary>
        /// Overrides abstract method to sort pixel list and take median value
        /// </summary>
        protected override double mergePixelList(List<double> pixels)
        {
            pixels.Sort();
            return pixels[pixels.Count / 2];
        }
    }
}
