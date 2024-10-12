using FrameDeltaInspector.Models;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FrameDeltaInspector.Controls
{
    /// <summary>
    /// Interaction logic for SequenceControl.xaml
    /// </summary>
    public partial class SequenceControl : UserControl
    {
        Sequence Sequence;
        public SequenceControl()
        {
            InitializeComponent();
        }

        public void SetSequence(Sequence sequence)
        {
            Sequence = sequence;
            sequence.ModelUpdated += () =>
            {
                UpdateView();
            };
            UpdateView();
        }

        public Sequence GetSequence()
        {
            return Sequence;
        }

        private void UpdateView()
        {
            // Load and display the current frame
            FrameImage.Source = LoadFrame(Sequence.SelectedFrameNumber);
        }

        private BitmapImage LoadFrame(int frameNumber)
        {
            // Ensure the frame number is properly formatted
            string fileName = $"{Sequence.FramePrefix}{frameNumber.ToString($"D{Sequence.FrameNumberDigits}")}{Sequence.FrameExtension}"; // Append the file extension and format frame number

            // Use the directory of the first frame path for loading images
            string directoryPath = Sequence.FramePath;
            string filePath = Path.Combine(directoryPath, fileName);

            if (File.Exists(filePath))
            {
                return new BitmapImage(new Uri(filePath));
            }
            return null;
        }
    }
}
