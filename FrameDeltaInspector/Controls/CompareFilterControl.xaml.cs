using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FrameDeltaInspector.Controls
{
    public partial class CompareFilterControl : UserControl
    {
        public ObservableCollection<UserControl> Filters { get; set; }
        public event EventHandler SelectionChanged;

        public CompareFilterControl()
        {
            InitializeComponent();
            Filters = new ObservableCollection<UserControl>();
            ListBoxFilters.ItemsSource = Filters;

            ListBoxFilters.SelectionChanged += ListBoxFilters_SelectionChanged;
            UpdateButtons();
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            var filterControl = new SelectCompareFilterControl();
            filterControl.ComboBoxFilter.SelectionChanged += new SelectionChangedEventHandler(Event_filterControl_SelectionChanged);
            Filters.Add(filterControl);
            ListBoxFilters.SelectedItem = filterControl;
            UpdateButtons();
            UpdateMoveButtons();
        }

        private void Event_filterControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSelectionChanged();
        }

        protected virtual void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Button_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxFilters.SelectedItem is UserControl selectedControl)
            {
                Filters.Remove(selectedControl);
            }
            UpdateButtons();
            UpdateMoveButtons();
        }

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxFilters.SelectedItem is SelectCompareFilterControl selectedFilter)
            {
                // Hier deine Bearbeitungslogik für den ausgewählten Filter implementieren
                MessageBox.Show("Edit button clicked for selected filter!");
            }
        }

        private void ListBoxFilters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtons();
            UpdateMoveButtons();
        }

        private void UpdateButtons()
        {
            Button_Remove.IsEnabled = ListBoxFilters.SelectedItem != null;
            Button_Edit.IsEnabled = ListBoxFilters.SelectedItem != null;
        }

        private void UpdateMoveButtons()
        {
            foreach (var control in Filters)
            {
                if (control is SelectCompareFilterControl selectCompareFilterControl)
                {
                    selectCompareFilterControl.UpdateMoveButtons();
                }
            }
        }
    }
}
