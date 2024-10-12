using System.IO;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FrameDeltaInspector.Models
{
    internal class DeltaRGBCompareFilter : CompareFilter
    {
        public override BitmapImage Compare(BitmapImage imageA, BitmapImage imageB)
        {
            // Ensure both images have the same dimensions
            if (imageA.PixelWidth != imageB.PixelWidth || imageA.PixelHeight != imageB.PixelHeight)
            {
                throw new ArgumentException("Images must have the same dimensions.");
            }

            // Create writable bitmaps to access pixel data
            WriteableBitmap prevBitmap = new WriteableBitmap(imageA);
            WriteableBitmap currBitmap = new WriteableBitmap(imageB);

            // Create a new writable bitmap for the delta frame
            int width = prevBitmap.PixelWidth;
            int height = prevBitmap.PixelHeight;
            WriteableBitmap deltaBitmap = new WriteableBitmap(width, height, imageA.DpiX, imageA.DpiY, PixelFormats.Bgra32, null);

            byte[] prevPixels = new byte[height * width * 4];
            byte[] currPixels = new byte[height * width * 4];
            byte[] deltaPixels = new byte[height * width * 4];

            prevBitmap.CopyPixels(prevPixels, width * 4, 0);
            currBitmap.CopyPixels(currPixels, width * 4, 0);

            // Calculate the delta
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    // Extract RGB values
                    byte rPrev = prevPixels[index * 4 + 2];
                    byte gPrev = prevPixels[index * 4 + 1];
                    byte bPrev = prevPixels[index * 4];
                    byte rCurr = currPixels[index * 4 + 2];
                    byte gCurr = currPixels[index * 4 + 1];
                    byte bCurr = currPixels[index * 4];

                    Color prevColor = Color.FromRgb(rPrev, gPrev, bPrev);
                    Color currColor = Color.FromRgb(rCurr, gCurr, bCurr);

                    double delta = CalculateRGBDelta(prevColor, currColor);

                    //byte deltaValue = (byte)Math.Round(255.0 * delta);
                    byte deltaValue = (byte)delta;
                    deltaPixels[index * 4] = deltaValue;
                    deltaPixels[index * 4 + 1] = deltaValue;
                    deltaPixels[index * 4 + 2] = deltaValue;
                    deltaPixels[index * 4 + 3] = 255;
                }
            }

            // Write pixel data to delta bitmap
            deltaBitmap.WritePixels(new Int32Rect(0, 0, width, height), deltaPixels, width * 4, 0);

            // Convert WriteableBitmap to BitmapImage
            BitmapImage resultImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                // Encode WriteableBitmap to PNG and write to MemoryStream
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(deltaBitmap));
                encoder.Save(stream);

                // Reset stream position and create BitmapImage from stream
                stream.Position = 0;
                resultImage.BeginInit();
                resultImage.StreamSource = stream;
                resultImage.CacheOption = BitmapCacheOption.OnLoad;
                resultImage.EndInit();
                resultImage.Freeze(); // Optional: make the image immutable
            }

            return resultImage;
        }

        public static double CalculateRGBDelta(Color color1, Color color2)
        {
            // Berechnen der Differenzen für jede Komponente
            int deltaR = color1.R - color2.R;
            int deltaG = color1.G - color2.G;
            int deltaB = color1.B - color2.B;

            // Berechnen der euklidischen Distanz der Differenzen
            double delta = Math.Sqrt(deltaR * deltaR + deltaG * deltaG + deltaB * deltaB);

            return delta;
        }

        public override string ToString()
        {
            return "RGB Delta";
        }
    }
}
