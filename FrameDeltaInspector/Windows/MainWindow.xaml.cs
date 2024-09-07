using FrameDeltaInspector.Models;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace FrameDeltaInspector.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Sequence SequenceA;
        Sequence SequenceB;
        Sequence Selected;
        public MainWindow()
        {
            InitializeComponent();
            SequenceASelectFrame.MouseDown += new System.Windows.Input.MouseButtonEventHandler(Event_SequenceASelectFrame_MouseDown);
            SequenceBSelectFrame.MouseDown += new System.Windows.Input.MouseButtonEventHandler(Event_SequenceBSelectFrame_MouseDown);
        }

        private void Event_SequenceBSelectFrame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Selected = SequenceB;
            sequencePropertyControl.SetSequence(Selected);
        }

        private void Event_SequenceASelectFrame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Selected = SequenceA;
            sequencePropertyControl.SetSequence(Selected);
        }

        private void MenuItem_Add_Click(object sender, RoutedEventArgs e)
        {
            Sequence seq = OpenSequence();
            Debug.WriteLine(seq.ToString());
            if(seq != null)
            {
                if (SequenceA == null)
                    SetSequenceA(seq);
                else
                    SetSequenceB(seq);
            }
        }

        private void SetSequenceA(Sequence seq)
        {
            SequenceA = seq;
            SequenceASelectFrame.SetSequence(SequenceA);
            SequenceAFrame.SetSequence(SequenceA);
        }

        private void SetSequenceB(Sequence seq)
        {
            SequenceB = seq;
            SequenceBSelectFrame.SetSequence(SequenceB);
            SequenceBFrame.SetSequence(SequenceB);
        }

        private void MenuItem_Close_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FrameSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //UpdateUI();
        }

        private Sequence OpenSequence()
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

                        Sequence sequence = new Sequence(prefix, fileExtension, startFrameNumber, endFrameNumber, selectedFrameNumber, frameNumberDigits, directoryPath);

                        // Set slider limits and current frame
                        /*
                        FrameSlider.Minimum = StartFrameNumber;
                        FrameSlider.Maximum = EndFrameNumber;
                        FrameSlider.Value = selectedFrameNumber;
                        */

                        return sequence;
                    }
                }
                else
                {
                    MessageBox.Show("Invalid file format. Please select an iamge sequence with a numbered suffix.");
                }
            }
            return null;
        }
    }
}
