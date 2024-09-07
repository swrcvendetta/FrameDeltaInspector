using FrameDeltaInspector.Models;
using System.Windows.Controls;

namespace FrameDeltaInspector.Controls
{
    /// <summary>
    /// Interaction logic for SequencePropertyControl.xaml
    /// </summary>
    public partial class SequencePropertyControl : UserControl
    {
        Sequence Sequence;
        public SequencePropertyControl()
        {
            InitializeComponent();
            txtBox_path.TextChanged += new TextChangedEventHandler(Event_txtBox_path_TextChanged);
            txtBox_prefix.TextChanged += new TextChangedEventHandler(Event_txtBox_prefix_TextChanged);
            txtBox_digits.TextChanged += new TextChangedEventHandler(Event_txtBox_digits_TextChanged);
            txtBox_extension.TextChanged += new TextChangedEventHandler(Event_txtBox_extension_TextChanged);
            txtBox_start.TextChanged += new TextChangedEventHandler(Event_txtBox_start_TextChanged);
            txtBox_end.TextChanged += new TextChangedEventHandler(Event_txtBox_end_TextChanged);
            txtBox_selected.TextChanged += new TextChangedEventHandler(Event_txtBox_selected_TextChanged);
        }

        public void SetSequence(Sequence sequence)
        {
            Sequence = sequence;
            UpdateView();
        }

        private void UpdateView()
        {
            txtBox_path.Text = Sequence.FramePath;
            txtBox_prefix.Text = Sequence.FramePrefix;
            txtBox_digits.Text = Sequence.FrameNumberDigits.ToString();
            txtBox_extension.Text = Sequence.FrameExtension;
            txtBox_start.Text = Sequence.StartFrameNumber.ToString();
            txtBox_end.Text = Sequence.EndFrameNumber.ToString();
            txtBox_selected.Text = Sequence.SelectedFrameNumber.ToString();
        }

        private void Event_txtBox_selected_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(Sequence != null)
            {
                Sequence.SetSelectedFrameNumber(int.Parse(txtBox_selected.Text));
            }
        }

        private void Event_txtBox_end_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Sequence != null)
            {
                Sequence.SetEndFrameNumber(int.Parse(txtBox_end.Text));
            }
        }

        private void Event_txtBox_start_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Sequence != null)
            {
                Sequence.SetStartFrameNumber(int.Parse(txtBox_start.Text));
            }
        }

        private void Event_txtBox_extension_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Sequence != null)
            {
                Sequence.SetFrameExtension(txtBox_extension.Text);
            }
        }

        private void Event_txtBox_digits_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Sequence != null)
            {
                Sequence.SetFrameNumberDigits(int.Parse(txtBox_digits.Text));
            }
        }

        private void Event_txtBox_prefix_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Sequence != null)
            {
                Sequence.SetFramePrefix(txtBox_prefix.Text);
            }
        }

        private void Event_txtBox_path_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Sequence != null)
            {
                Sequence.SetFramePath(txtBox_path.Text);
            }
        }
    }
}
