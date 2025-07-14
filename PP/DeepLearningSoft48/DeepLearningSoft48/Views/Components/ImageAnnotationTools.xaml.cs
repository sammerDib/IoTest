using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DeepLearningSoft48.Views.Components
{
    /// <summary>
    /// Interaction logic for ImageAnnotationTools.xaml
    /// </summary>
    public partial class ImageAnnotationTools : UserControl
    {
        public ImageAnnotationTools()
        {
            InitializeComponent();
        }

        private void SizeButton_ClickButton(object sender, RoutedEventArgs routedEventArgs)
        {
            //do stuff (eg activate drop down)

            if (!PopUp.IsOpen)
            {
                PopUp.IsOpen = true;
                Mouse.Capture(this, CaptureMode.SubTree);
                AddHandler();
            }
            else
                PopUp.IsOpen = !PopUp.IsOpen;

        }

        private void AddHandler()
        {
            AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(HandleClickOutsideOfControl), true);
            AddHandler(Selector.SelectedEvent, new RoutedEventHandler(ListBoxItem_Selected), true);
        }

        private void HandleClickOutsideOfControl(object sender, MouseButtonEventArgs e)
        {
            //do stuff (eg close drop down)
            if (PopUp.IsOpen)
            {
                SizeButton.IsChecked = false;
                PopUp.IsOpen = false;
            }
            ReleaseMouseCapture();
        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            PopUp.IsOpen = false;
            SizeButton.IsChecked = false;
            ReleaseMouseCapture();
        }
    }
}
