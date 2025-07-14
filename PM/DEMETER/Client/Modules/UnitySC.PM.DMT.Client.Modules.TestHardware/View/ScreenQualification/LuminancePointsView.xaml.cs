using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using UnitySC.Shared.UI.Controls;

using Xceed.Wpf.Toolkit;

namespace UnitySC.PM.DMT.Client.Modules.TestHardware.View
{
    /// <summary>
    /// Logique d'interaction pour LuminanceCalibrationMultiPointView.xaml
    /// </summary>
    public partial class LuminancePointsView
    {
        public LuminancePointsView()
        {
            InitializeComponent();
        }


        public static Visual GetDescendantByType(Visual element, Type type, string name)
        {
            if (element == null) return null;
            if (element.GetType() == type)
            {
                FrameworkElement fe = element as FrameworkElement;
                if (fe != null)
                {
                    if (fe.Name == name)
                    {
                        return fe;
                    }
                }
            }
            Visual foundElement = null;
            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();
            for (int i = 0;
                i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType(visual, type, name);
                if (foundElement != null)
                    break;
            }
            return foundElement;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemContainerGenerator generator = this.listview.ItemContainerGenerator;
            ListViewItem selectedItem = (ListViewItem)generator.ContainerFromIndex(listview.SelectedIndex);
            TextBoxUnit tbFind = GetDescendantByType(selectedItem, typeof(TextBoxUnit), "luminance") as TextBoxUnit;
            if (tbFind != null)
            {
                tbFind.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(delegate ()
                {
                    tbFind.Focus();
                }));
            }
        }

        private void luminance_GotFocus(object sender, RoutedEventArgs e)
        {
            //listview.SelectedItem = (sender as TextBoxUnit).DataContext;
            listview.SelectedItem = (sender as DecimalUpDown).DataContext; 
        }
    }
}
