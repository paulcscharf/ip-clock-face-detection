using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

// This file was created for INFOIBV 2011/12 by Paul Scharf (3683745)

namespace INFOIBV
{
    /// <summary>
    /// RGB enumerator, used with RgbByteArrayBitmap
    /// </summary>
    enum RgbColor
    {
        Blue, Green, Red
    }

    /// <summary>
    /// Class representing an RGB bitmap but allows far faster manipulation due to providing direct access to the pixel data without using system calls
    /// </summary>
    class RgbByteArrayBitmap
    {
        byte[] rgbData;

        /// <summary>
        /// Width of the stored bitmap
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Height of the stored bitmap
        /// </summary>
        public int Height { get; private set; }


        /// <summary>
        /// Creates new instance with given width and height
        /// All pixel values will be 0
        /// </summary>
        public RgbByteArrayBitmap(int width, int height)
        {
            rgbData = new byte[width * height * 3];
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Creates new instance from a given array of bytes and specified width and height
        /// </summary>
        public RgbByteArrayBitmap(byte[] rgb, int width, int height)
        {
            rgbData = rgb;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// The total number of bytes in the pixeldata
        /// </summary>
        public int Length
        {
            get { return rgbData.Length; }
        }

        /// <summary>
        /// Gets or sets any byte in the pixeldata by index
        /// </summary>
        public byte this[int i]
        {
            get { return rgbData[i]; }
            set { rgbData[i] = value; }
        }

        /// <summary>
        /// Gets or sets any byte in the pxieldata by coordinates and colour
        /// </summary>
        public byte this[int x, int y, RgbColor c]
        {
            get { return rgbData[(x + y * Width) * 3 + ((int)c)]; }
            set { rgbData[(x + y * Width) * 3 + ((int)c)] = value; }
        }

        /// <summary>
        /// Gets or sets any 3-byte pixel in the pxieldata by coordinates
        /// </summary>
        public byte[] this[int x, int y]
        {
            get { int offset = (x + y * Width) * 3; byte[] bs = { rgbData[offset], rgbData[offset + 1], rgbData[offset + 2] }; return bs; }
            set { int offset = (x + y * Width) * 3; rgbData[offset] = value[0]; rgbData[offset + 1] = value[1]; rgbData[offset + 2] = value[2]; }
        }

        /// <summary>
        /// Creates a new instance from a provided bitmap
        /// </summary>
        public static RgbByteArrayBitmap FromBitmap(Bitmap bitmap)
        {
            int bytes = bitmap.Width * bitmap.Height * 3;
            byte[] rgbData = new byte[bytes];
            System.Drawing.Imaging.BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int scan0 = (int)bitmapData.Scan0;
            int stride = bitmapData.Stride;
            int w3 = bitmap.Width * 3;
            for (int i = 0; i < bitmap.Height; i++)
                System.Runtime.InteropServices.Marshal.Copy(new IntPtr(scan0 + stride * i), rgbData, w3 * i, w3);
            bitmap.UnlockBits(bitmapData);
            return new RgbByteArrayBitmap(rgbData, bitmap.Width, bitmap.Height);
        }

        /// <summary>
        /// Writes the pixel data into a bitmap and returns that
        /// </summary>
        public Bitmap ToBitmap()
        {
            Bitmap bitmap = new Bitmap(Width, Height);
            System.Drawing.Imaging.BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int scan0 = (int)bitmapData.Scan0;
            int stride = bitmapData.Stride;
            int w3 = Width * 3;
            for (int i = 0; i < bitmap.Height; i++)
                System.Runtime.InteropServices.Marshal.Copy(rgbData, w3 * i, new IntPtr(scan0 + stride * i), w3);
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        /// <summary>
        /// Creates an exact copy of the object
        /// </summary>
        /// <returns></returns>
        public RgbByteArrayBitmap Clone()
        {
            return new RgbByteArrayBitmap((byte[])rgbData.Clone(), Width, Height);
        }


        /// <summary>
        /// Converts the bitmap to a DoubleArrayBitmap, with channels for Hue, Saturation and Brightness
        /// </summary>
        public DoubleArrayBitmap ToHSV()
        {
            double[][] rgb = new double[3][];
            for (int j = 0; j < 3; j++)
                rgb[j] = new double[Width * Height];
            for (int i = 0; i < Width * Height; i++)
            {
                Color c = Color.FromArgb(rgbData[i * 3], rgbData[i * 3 + 1], rgbData[i * 3 + 2]);
                rgb[0][i] = c.GetHue() / 360;
                rgb[1][i] = c.GetSaturation();
                rgb[2][i] = c.GetBrightness();
            }
            return new DoubleArrayBitmap(rgb, Width, Height);
        }

        /// <summary>
        /// Bitwise AND operator. Takes two bitmaps and merges them using a bitwise AND on every pixel-value
        /// </summary>
        public static RgbByteArrayBitmap operator &(RgbByteArrayBitmap bitmap1, RgbByteArrayBitmap bitmap2)
        {
            byte[] rgb = new byte[bitmap1.Length];
            for (int i = 0; i < bitmap1.Length; i++)
                rgb[i] = (byte)(bitmap1[i] & bitmap2[i]);
            return new RgbByteArrayBitmap(rgb, bitmap1.Width, bitmap1.Height);
        }

    }

    /// <summary>
    /// Class representing an RGB bitmap but allows far faster manipulation due to providing direct access to the pixel data without using system calls
    /// A DoubleArrayBitmap can have any number of channels, though 1 or 3 should be most common. All pixel values are stored as floating point values
    /// </summary>
    class DoubleArrayBitmap
    {
        /// <summary>
        /// Array of channels
        /// </summary>
        public double[][] Channels;

        /// <summary>
        /// Width of the bitmap
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Height of the bitmap
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Creates a new instance with the given width, height and number of channels(default: 1)
        /// </summary>
        public DoubleArrayBitmap(int width, int height, int channs = 1)
        {
            Channels = new double[channs][];
            for (int i = 0; i < channs; i++)
                Channels[i] = new double[width * height];
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Creates a new instance from a given array of channels with a given width and height
        /// </summary>
        public DoubleArrayBitmap(double[][] rgb, int width, int height)
        {
            Channels = rgb;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Creates a new 3-channel instance copied its values from a provided byte bitmap
        /// </summary>
        public DoubleArrayBitmap(RgbByteArrayBitmap rgbBytes)
        {
            Width = rgbBytes.Width;
            Height = rgbBytes.Height;
            Channels = new double[3][];
            for (int j = 0; j < 3; j++)
                Channels[j] = new double[Width * Height];
            for (int i = 0; i < Width * Height; i++)
            {
                Channels[0][i] = rgbBytes[i * 3];
                Channels[1][i] = rgbBytes[i * 3 + 1];
                Channels[2][i] = rgbBytes[i * 3 + 2];
            }
        }

        /// <summary>
        /// The total amount of pixel values stored over all channels
        /// </summary>
        public int Length
        {
            get { return Channels.Length * Width * Height; }
        }

        /// <summary>
        /// Gets or sets a pixel value in any channel by their index
        /// </summary>
        public double this[int i]
        {
            get { return Channels[i % Channels.Length][i % (Width * Height)]; }
            set { Channels[i % Channels.Length][i % (Width * Height)] = value; }
        }

        /// <summary>
        /// Gets or sets a pixel value in a given channel with coordinates x and y
        /// </summary>
        public double this[int c, int x, int y]
        {
            get { return Channels[c][x + y * Width]; }
            set { Channels[c][x + y * Width] = value; }
        }

        /// <summary>
        /// Gets or sets a pixel value in a given channel by their index
        /// </summary>
        public double this[int c, int i]
        {
            get { return Channels[c][i]; }
            set { Channels[c][i] = value; }
        }

        /// <summary>
        /// Creates an exact copy of the object
        /// </summary>
        public DoubleArrayBitmap Clone()
        {
            double[][] cs = new double[Channels.Length][];
            for (int i = 0; i < Channels.Length; i++)
                cs[i] = (double[])Channels[i].Clone();
            return new DoubleArrayBitmap(cs, Width, Height);
        }

        /// <summary>
        /// Returns a byte version of the image with contrast adjusted to stretch from 0 to 255
        /// </summary>
        /// <returns></returns>
        public RgbByteArrayBitmap ToBytes()
        {
            double min = Channels[0][0];
            double max = Channels[0][0];
            for (int i = 0; i < Width * Height; i++)
                for (int j = 0; j < Channels.Length; j++)
                {
                    if (Channels[j][i] > max)
                        max = Channels[j][i];
                    else if (Channels[j][i] < min)
                        min = Channels[j][i];
                }
            double f = 255 / (max - min);

            byte[] rgbBytes = new byte[Width * Height * 3];

            if (Channels.Length == 3)
            {
                for (int i = 0; i < Width * Height; i++)
                    for (int j = 0; j < Channels.Length; j++)
                        rgbBytes[i * Channels.Length + j] = (byte)((Channels[j][i] - min) * f);
            }
            else
            {
                for (int i = 0; i < Width * Height; i++)
                {
                    byte b =  (byte)((Channels[0][i] - min) * f);
                    rgbBytes[i * 3] = b;
                    rgbBytes[i * 3 + 1] = b;
                    rgbBytes[i * 3 + 2] = b;
                }
            }

            return new RgbByteArrayBitmap(rgbBytes, Width, Height);
        }

        /// <summary>
        /// Adds the corresponding pixel values of two bitmaps of the same size
        /// </summary>
        public static DoubleArrayBitmap operator + (DoubleArrayBitmap bitmap1, DoubleArrayBitmap bitmap2)
        {
            if (bitmap1.Length != bitmap2.Length)
                return null;
            DoubleArrayBitmap result = new DoubleArrayBitmap(bitmap1.Width, bitmap1.Height, bitmap1.Channels.Length);
            for (int i = 0; i < bitmap1.Length; i++)
                result[i] = bitmap1[i] + bitmap2[i];
            return result;
        }

        /// <summary>
        /// Substracts the pixel values of one bitmap from another
        /// </summary>
        public static DoubleArrayBitmap operator -(DoubleArrayBitmap bitmap1, DoubleArrayBitmap bitmap2)
        {
            if (bitmap1.Length != bitmap2.Length)
                return null;
            DoubleArrayBitmap result = new DoubleArrayBitmap(bitmap1.Width, bitmap1.Height, bitmap1.Channels.Length);
            for (int i = 0; i < bitmap1.Length; i++)
                result[i] = bitmap1[i] - bitmap2[i];
            return result;
        }

        /// <summary>
        /// Multiplies the corresponding pixel values of two bitmaps
        /// </summary>
        public static DoubleArrayBitmap operator * (DoubleArrayBitmap bitmap1, DoubleArrayBitmap bitmap2)
        {
            if (bitmap1.Length != bitmap2.Length)
                return null;
            DoubleArrayBitmap result = new DoubleArrayBitmap(bitmap1.Width, bitmap1.Height, bitmap1.Channels.Length);
            for (int i = 0; i < bitmap1.Length; i++)
                result[i] = bitmap1[i] * bitmap2[2];
            return result;
        }

        /// <summary>
        /// Adds a constant to every pixel value
        /// </summary>
        public static DoubleArrayBitmap operator +(DoubleArrayBitmap bitmap1, double addand)
        {
            DoubleArrayBitmap result = new DoubleArrayBitmap(bitmap1.Width, bitmap1.Height, bitmap1.Channels.Length);
            for (int i = 0; i < bitmap1.Length; i++)
                result[i] = bitmap1[i] + addand;
            return result;
        }

        /// <summary>
        /// Substracts a constant from every pixel value
        /// </summary>
        public static DoubleArrayBitmap operator -(DoubleArrayBitmap bitmap1, double addand)
        {
            DoubleArrayBitmap result = new DoubleArrayBitmap(bitmap1.Width, bitmap1.Height, bitmap1.Channels.Length);
            for (int i = 0; i < bitmap1.Length; i++)
                result[i] = bitmap1[i] - addand;
            return result;
        }

        /// <summary>
        /// Multiplies every pixel value with a constant
        /// </summary>
        public static DoubleArrayBitmap operator * (DoubleArrayBitmap bitmap1, double factor)
        {
            DoubleArrayBitmap result = new DoubleArrayBitmap(bitmap1.Width, bitmap1.Height, bitmap1.Channels.Length);
            for (int i = 0; i < bitmap1.Length; i++)
                result[i] = bitmap1[i] * factor;
            return result;
        }

        /// <summary>
        /// Extracts the positive square root of all pixel values
        /// </summary>
        public void SqrtMe()
        {
            for (int i = 0; i < Width * Height; i++)
                for (int j = 0; j < Channels.Length; j++)
                    Channels[j][i] = Math.Sqrt(Channels[j][i]);
        }

        /// <summary>
        /// Squares all pixel values
        /// </summary>
        public void SquareMe()
        {
            for (int i = 0; i < Width * Height; i++)
                for (int j = 0; j < Channels.Length; j++)
                    Channels[j][i] = Channels[j][i] * Channels[j][i];
        }

        /// <summary>
        /// Changes the sign of all negative pixel values
        /// </summary>
        public void AbsMe()
        {
            for (int i = 0; i < Width * Height; i++)
                for (int j = 0; j < Channels.Length; j++)
                    Channels[j][i] = Math.Abs(Channels[j][i]);
        }

        /// <summary>
        /// Thresholds all pixel values by a given constant:
        /// All values smaller will be set to 0
        /// All values larger or equal will be set to 1
        /// </summary>
        public void ThresholdMe(double threshold = 0)
        {
            for (int i = 0; i < Width * Height; i++)
                for (int j = 0; j < Channels.Length; j++)
                {
                    if (Channels[j][i] >= threshold)
                        Channels[j][i] = 1;
                    else
                        Channels[j][i] = 0;
                }
        }

        /// <summary>
        /// Returns a new one-channel bitmap including the lengths of all "pixel-vectors"
        /// </summary>
        public DoubleArrayBitmap PixelLengths()
        {
            double[][] cs = new double[1][];
            cs[0] = new double[Width * Height];
            for (int i = 0; i < Width * Height; i++)
            {
                for (int j = 0; j < Channels.Length; j++)
                {
                    cs[0][i] += Channels[j][i] * Channels[j][i];
                }
                cs[0][i] = Math.Sqrt(cs[0][i]);
            }
            return new DoubleArrayBitmap(cs, Width, Height);
        }

        /// <summary>
        /// Returns a one-channel bitmap which is a copy of the specified channel
        /// </summary>
        public DoubleArrayBitmap ExtractChannel(int c)
        {
            double[][] cs = new double[1][];
            cs[0] = (double[])Channels[c].Clone();
            return new DoubleArrayBitmap(cs, Width, Height);
        }

        /// <summary>
        /// Returns a bitmap of half the width and height, with their pixels being averages of 2*2 pixels in the original
        /// </summary>
        public DoubleArrayBitmap ShrinkByTwo()
        {
            int w = Width / 2;
            int h = Height / 2;
            double[][] cs = new double[Channels.Length][];
            for (int j = 0; j < Channels.Length; j++)
            {
                cs[j] = new double[w * h];
                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        cs[j][x + y * w] = 0.25 * (this[j, x * 2, y * 2] + this[j, x * 2 + 1, y * 2] + this[j, x * 2 + 1, y * 2 + 1] + this[j, x * 2, y * 2 + 1]);
                    }
                }
            }
            return new DoubleArrayBitmap(cs, w, h);
        }

        /// <summary>
        /// Returns a bitmap with twice the width and height, with the original pixels simply being resized to 2*2 pixels
        /// </summary>
        /// <returns></returns>
        public DoubleArrayBitmap ExpandByTwo()
        {
            int w = Width * 2;
            int h = Height * 2;
            double[][] cs = new double[Channels.Length][];
            for (int j = 0; j < Channels.Length; j++)
            {
                cs[j] = new double[w * h];
                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        cs[j][x + y * w] = this[j, x / 2, y / 2];
                    }
                }
            }
            return new DoubleArrayBitmap(cs, w, h);
        }
    }

    
}
