using System;

namespace FrameDeltaInspector.Models
{
    public class Sequence
    {
        // Event to notify subscribers when the model is updated
        public event Action ModelUpdated;

        // Private fields
        private string _framePrefix;
        private string _frameExtension;
        private int _startFrameNumber;
        private int _endFrameNumber;
        private int _selectedFrameNumber;
        private int _frameNumberDigits;
        private string _framePath;

        // Properties with private setters
        public string FramePrefix
        {
            get => _framePrefix;
            private set
            {
                _framePrefix = value;
                OnModelUpdated();
            }
        }

        public string FrameExtension
        {
            get => _frameExtension;
            private set
            {
                _frameExtension = value;
                OnModelUpdated();
            }
        }

        public int StartFrameNumber
        {
            get => _startFrameNumber;
            private set
            {
                _startFrameNumber = value;
                OnModelUpdated();
            }
        }

        public int EndFrameNumber
        {
            get => _endFrameNumber;
            private set
            {
                _endFrameNumber = value;
                OnModelUpdated();
            }
        }

        public int SelectedFrameNumber
        {
            get => _selectedFrameNumber;
            private set
            {
                _selectedFrameNumber = value;
                OnModelUpdated();
            }
        }

        public int FrameNumberDigits
        {
            get => _frameNumberDigits;
            private set
            {
                _frameNumberDigits = value;
                OnModelUpdated();
            }
        }

        public string FramePath
        {
            get => _framePath;
            private set
            {
                _framePath = value;
                OnModelUpdated();
            }
        }

        // Constructor to initialize the model
        public Sequence(string framePrefix, string frameExtension, int startFrameNumber, int endFrameNumber, int selectedFrameNumber, int frameNumberDigits, string framePath)
        {
            _framePrefix = framePrefix;
            _frameExtension = frameExtension;
            _startFrameNumber = startFrameNumber;
            _endFrameNumber = endFrameNumber;
            _selectedFrameNumber = selectedFrameNumber;
            _frameNumberDigits = frameNumberDigits;
            _framePath = framePath;
        }

        // Method to call when the model is updated
        protected virtual void OnModelUpdated()
        {
            // Invoke the ModelUpdated event, notifying subscribers of the change
            ModelUpdated?.Invoke();
        }

        // Public setter methods to allow controlled updates and trigger model updates
        public void SetFramePrefix(string framePrefix)
        {
            FramePrefix = framePrefix;
        }

        public void SetFrameExtension(string frameExtension)
        {
            FrameExtension = frameExtension;
        }

        public void SetStartFrameNumber(int startFrameNumber)
        {
            StartFrameNumber = startFrameNumber;
        }

        public void SetEndFrameNumber(int endFrameNumber)
        {
            EndFrameNumber = endFrameNumber;
        }

        public void SetSelectedFrameNumber(int selectedFrameNumber)
        {
            SelectedFrameNumber = selectedFrameNumber;
        }

        public void SetFrameNumberDigits(int frameNumberDigits)
        {
            FrameNumberDigits = frameNumberDigits;
        }

        public void SetFramePath(string framePath)
        {
            FramePath = framePath;
        }
    }
}
