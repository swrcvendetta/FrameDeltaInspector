using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FrameDeltaInspector
{
    public partial class MainWindow : Window
    {
        public string FramePrefix { get; private set; }
        public string FrameExtension { get; private set; }
        public int StartFrameNumber { get; private set; }
        public int EndFrameNumber { get; private set; }
        public int SelectedFrameNumber { get; private set; }
        public int FrameNumberDigits { get; private set; }
        public string FramePath { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        // Event handler for "Datei" -> "Öffnen"
        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Multiselect = false  // Only allow selecting one file initially
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedFilePath = dialog.FileName;
                string directoryPath = Path.GetDirectoryName(selectedFilePath);
                string selectedFileName = Path.GetFileNameWithoutExtension(selectedFilePath);
                string fileExtension = Path.GetExtension(selectedFilePath); // Save the file extension

                // Extract prefix and frame number using regex
                var match = Regex.Match(selectedFileName, @"^(.*?)(\d+)$");
                if (match.Success)
                {
                    string prefix = match.Groups[1].Value;
                    int selectedFrameNumber = int.Parse(match.Groups[2].Value);

                    // Determine the number of digits in the frame number
                    int frameNumberDigits = match.Groups[2].Value.Length;

                    // Get all images in the directory with the same prefix and numeric suffix
                    var imageFiles = Directory.GetFiles(directoryPath, "*.jpg")
                                              .Concat(Directory.GetFiles(directoryPath, "*.jpeg"))
                                              .Concat(Directory.GetFiles(directoryPath, "*.png"))
                                              .Where(file => Regex.IsMatch(Path.GetFileNameWithoutExtension(file), $@"^{Regex.Escape(prefix)}\d+$"))
                                              .OrderBy(file =>
                                              {
                                                  var numMatch = Regex.Match(Path.GetFileNameWithoutExtension(file), @"(\d+)$");
                                                  return numMatch.Success ? int.Parse(numMatch.Value) : int.MaxValue;
                                              })
                                              .ToList();

                    if (imageFiles.Count > 0)
                    {
                        // Find start and end frames
                        string startFramePath = imageFiles.First();
                        string endFramePath = imageFiles.Last();
                        int startFrameNumber = int.Parse(Regex.Match(Path.GetFileNameWithoutExtension(startFramePath), @"(\d+)$").Value);
                        int endFrameNumber = int.Parse(Regex.Match(Path.GetFileNameWithoutExtension(endFramePath), @"(\d+)$").Value);

                        // Store the prefix, start, end frame details, file extension, frame number length, and all frame paths
                        FramePrefix = prefix;
                        FrameExtension = fileExtension; // Save the file extension
                        StartFrameNumber = startFrameNumber;
                        EndFrameNumber = endFrameNumber;
                        SelectedFrameNumber = selectedFrameNumber;
                        FrameNumberDigits = frameNumberDigits; // Save the number of digits

                        // Store the paths of all frames
                        FramePath = directoryPath;

                        // Set slider limits and current frame
                        FrameSlider.Minimum = StartFrameNumber;
                        FrameSlider.Maximum = EndFrameNumber;
                        FrameSlider.Value = selectedFrameNumber;

                        // Update UI with the initial frame
                        UpdateUI();
                    }
                }
                else
                {
                    MessageBox.Show("Ungültiges Dateiformat. Bitte wählen Sie eine Bildsequenz mit einem numerischen Suffix.");
                }
            }
        }


        // Event handler for "Datei" -> "Schließen"
        // This closes/unloads the current image sequence but does not exit the application
        private void MenuItem_Close_Click(object sender, RoutedEventArgs e)
        {
            CurrentFrameImage.Source = null; // Clear the current frame image
            DeltaFrameImage.Source = null;   // Clear the delta frame image
            FrameSlider.Value = 0;
            FrameLabel.Text = "Frame 0 of 0"; // Reset the label
        }

        // Event handler for "Datei" -> "Beenden"
        // This closes the application
        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Exit the application
        }

        // Method to load the image on demand
        private BitmapImage LoadFrame(int frameNumber)
        {
            // Ensure the frame number is properly formatted
            string fileName = $"{FramePrefix}{frameNumber.ToString($"D{FrameNumberDigits}")}{FrameExtension}"; // Append the file extension and format frame number

            // Use the directory of the first frame path for loading images
            string directoryPath = FramePath;
            string filePath = Path.Combine(directoryPath, fileName);

            if (File.Exists(filePath))
            {
                return new BitmapImage(new Uri(filePath));
            }
            return null;
        }


        // Updates the current frame and delta frame
        private void UpdateUI()
        {
            //if (frames.Count == 0) return;

            // Load and display the current frame
            CurrentFrameImage.Source = LoadFrame((int)FrameSlider.Value);

            // Load and display the delta frame
            if ((int)FrameSlider.Value > StartFrameNumber)
            {
                var previousFrame = LoadFrame((int)FrameSlider.Value - 1);
                var currentFrame = LoadFrame((int)FrameSlider.Value);
                if (previousFrame != null && currentFrame != null)
                {
                    BitmapImage deltaFrame = CalculateDelta(previousFrame, currentFrame);
                    DeltaFrameImage.Source = deltaFrame;
                }
            }
            else
            {
                // If no previous frame exists, just clear the delta image
                DeltaFrameImage.Source = null;
            }

            FrameLabel.Text = $"Frame {(int)FrameSlider.Value} of {EndFrameNumber}";
        }

        // Calculates the difference between two frames (this would be implemented to suit your needs)
        private BitmapImage CalculateDelta(BitmapImage previousFrame, BitmapImage currentFrame)
        {
            // Ensure both images have the same dimensions
            if (previousFrame.PixelWidth != currentFrame.PixelWidth || previousFrame.PixelHeight != currentFrame.PixelHeight)
            {
                throw new ArgumentException("Images must have the same dimensions.");
            }

            // Create writable bitmaps to access pixel data
            WriteableBitmap prevBitmap = new WriteableBitmap(previousFrame);
            WriteableBitmap currBitmap = new WriteableBitmap(currentFrame);

            // Create a new writable bitmap for the delta frame
            int width = prevBitmap.PixelWidth;
            int height = prevBitmap.PixelHeight;
            WriteableBitmap deltaBitmap = new WriteableBitmap(width, height, previousFrame.DpiX, previousFrame.DpiY, PixelFormats.Bgra32, null);

            //int bytesPerPixel = (prevBitmap.Format.BitsPerPixel + 7) / 8;
            //int stride = width * bytesPerPixel;
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
                    //double delta = CalculateHSVDelta(prevColor, currColor);
                    
                    //byte deltaValue = (byte)Math.Round(255.0 * delta);
                    byte deltaValue = (byte)delta;
                    deltaPixels[index * 4] = deltaValue;
                    deltaPixels[index * 4 + 1] = deltaValue;
                    deltaPixels[index * 4 + 2] = deltaValue;
                    deltaPixels[index * 4 + 3] = 255;

                    /*
                    Vector3 curr = new Vector3(rCurr, gCurr, bCurr);
                    Vector3 prev = new Vector3(rPrev, gPrev, bPrev);
                    curr = Vector3.Normalize(curr);
                    prev = Vector3.Normalize(prev);
                    float dp = Vector3.Dot(prev, curr);

                    // Calculate dot product
                    //int dotProduct = rPrev * rCurr + gPrev * gCurr + bPrev * bCurr;
                    //double dp = dotProduct / (double)(255 * 255 * 3); // Normalize
                    float delta = 1.0f - (dp + 1.0f) / 2.0f;
                    
                    float h = 0.0f;
                    float s = 0.0f;
                    float v = 0.0f;

                    // Calculate delta value and clamp it to [0, 255]
                    byte deltaValue = (byte)Math.Round(255.0f * delta);
                    deltaPixels[index * 4] = deltaValue;
                    deltaPixels[index * 4 + 1] = deltaValue;
                    deltaPixels[index * 4 + 2] = deltaValue;
                    deltaPixels[index * 4 + 3] = 255;
                    
                    if (delta > 0.0f)
                    {
                        h = 360.0f * delta;
                        s = 1.0f;
                        v = 1.0f;
                        var (rOut, gOut, bOut) = HsvToRgb(h, s, v);
                        deltaPixels[index * 4] = bOut;
                        deltaPixels[index * 4 + 1] = gOut;
                        deltaPixels[index * 4 + 2] = rOut;
                    }
                    */
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

        // Event handler for when the slider value changes
        private void FrameSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateUI();
        }

        // Converts HSV to RGB
        public static (byte R, byte G, byte B) HsvToRgb(double h, double s, double v)
        {
            double c = v * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = v - c;

            double r = 0, g = 0, b = 0;

            if (0 <= h && h < 60) {
                r = c;
                g = x;
                b = 0;
            }
            else if (60 <= h && h < 120){
                r = x;
                g = c;
                b = 0;
            }
            else if (120 <= h && h < 180){
                r = 0;
                g = c;
                b = x;
            }
            else if (180 <= h && h < 240){
                r = 0;
                g = x;
                b = c;
            }
            else if (240 <= h && h < 300){
                r = x;
                g = 0;
                b = c;
            }
            else if (300 <= h && h < 360){
                r = c;
                g = 0;
                b = x;
            }

            return ((byte)((r + m) * 255), (byte)((g + m) * 255), (byte)((b + m) * 255));
        }

        public static (double H, double S, double V) RgbToHsv(byte r, byte g, byte b)
        {
            double rNorm = r / 255.0;
            double gNorm = g / 255.0;
            double bNorm = b / 255.0;

            double max = Math.Max(Math.Max(rNorm, gNorm), bNorm);
            double min = Math.Min(Math.Min(rNorm, gNorm), bNorm);
            double delta = max - min;

            double h = 0;
            double s = (max == 0) ? 0 : delta / max;
            double v = max;

            if (delta > 0)
            {
                if (max == rNorm)
                    h = (gNorm - bNorm) / delta + (gNorm < bNorm ? 6 : 0);
                else if (max == gNorm)
                    h = (bNorm - rNorm) / delta + 2;
                else
                    h = (rNorm - gNorm) / delta + 4;
                h /= 6;
            }

            return (h * 360, s, v);
        }
        
        // Rotates RGB color by a given hue value
        public static Color RotateColorHue(Color color, double hueDegrees)
        {
            // Convert Color to RGB values
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;

            // Convert RGB to HSV
            var (h, s, v) = RgbToHsv(r, g, b);

            // Adjust hue
            h = (h + hueDegrees) % 360;
            if (h < 0) h += 360;

            // Convert back to RGB
            var (rOut, gOut, bOut) = HsvToRgb(h, s, v);

            return Color.FromRgb(rOut, gOut, bOut);
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

        private double CalculateHSVDelta(Color color1, Color color2)
        {
            var hsv1 = RGBtoHSV(color1);
            var hsv2 = RGBtoHSV(color2);

            // Hue Delta: Compute the shortest distance on the color wheel
            double hueDelta = Math.Min(Math.Abs(hsv1.Hue - hsv2.Hue), 360 - Math.Abs(hsv1.Hue - hsv2.Hue));

            // Saturation and Value Deltas
            double saturationDelta = Math.Abs(hsv1.Saturation - hsv2.Saturation);
            double valueDelta = Math.Abs(hsv1.Value - hsv2.Value);

            // Compute combined delta
            return Math.Sqrt(hueDelta * hueDelta + saturationDelta * saturationDelta + valueDelta * valueDelta);
        }

        private (double Hue, double Saturation, double Value) RGBtoHSV(Color color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            double hue = 0;
            if (delta != 0)
            {
                if (max == r)
                {
                    hue = (60 * ((g - b) / delta) + 360) % 360;
                }
                else if (max == g)
                {
                    hue = (60 * ((b - r) / delta) + 120) % 360;
                }
                else
                {
                    hue = (60 * ((r - g) / delta) + 240) % 360;
                }
            }

            double saturation = max == 0 ? 0 : (delta / max);
            double value = max;

            return (hue, saturation, value);
        }
    }
}
