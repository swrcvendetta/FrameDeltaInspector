using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Diagnostics;
using FrameDeltaInspector.Models;

namespace FrameDeltaInspector.Controls
{
    public partial class SelectCompareFilterControl : UserControl
    {
        public SelectCompareFilterControl()
        {
            InitializeComponent();
            ComboBoxFilter.Items.Add(new CompareFilter());
            ComboBoxFilter.Items.Add(new DeltaRGBCompareFilter());
            UpdateMoveButtons();
        }

        public CompareFilter GetCompareFilter()
        {
            return ComboBoxFilter.SelectedItem as CompareFilter;
        }

        private void Button_MoveUp_Click(object sender, RoutedEventArgs e)
        {
            var parentListBox = FindParentListBox(this);
            if (parentListBox != null && parentListBox.ItemsSource is ObservableCollection<UserControl> filters)
            {
                int currentIndex = filters.IndexOf(this);
                if (currentIndex > 0)
                {
                    var item = filters[currentIndex];
                    filters.RemoveAt(currentIndex);
                    filters.Insert(currentIndex - 1, item);
                    parentListBox.SelectedItem = item;
                }
            }
            UpdateMoveButtons();
        }

        private void Button_MoveDown_Click(object sender, RoutedEventArgs e)
        {
            var parentListBox = FindParentListBox(this);
            if (parentListBox != null && parentListBox.ItemsSource is ObservableCollection<UserControl> filters)
            {
                int currentIndex = filters.IndexOf(this);
                if (currentIndex < filters.Count - 1)
                {
                    var item = filters[currentIndex];
                    filters.RemoveAt(currentIndex);
                    filters.Insert(currentIndex + 1, item);
                    parentListBox.SelectedItem = item;
                }
            }
            UpdateMoveButtons();
        }

        public void UpdateMoveButtons()
        {
            var parentListBox = FindParentListBox(this);
            if (parentListBox != null && parentListBox.ItemsSource is ObservableCollection<UserControl> filters)
            {
                int currentIndex = filters.IndexOf(this);
                button_MoveUp.IsEnabled = currentIndex > 0;
                button_MoveDown.IsEnabled = currentIndex < filters.Count - 1;
            }
        }

        private ListBox FindParentListBox(DependencyObject child)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is ListBox))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as ListBox;
        }
    }
}
