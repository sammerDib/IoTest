using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace UnitySC.Shared.UI.Controls.ZoomboxImage
{
    /// <summary>
    /// Interaction logic for ZoomboxWithImageList.xaml
    /// </summary>
    public partial class ZoomboxWithImageList : UserControl
    {
        //=================================================================
        // Constructeur
        //=================================================================
        public ZoomboxWithImageList()
        {
            InitializeComponent();
        }

        //=================================================================
        // Dependency properties
        //=================================================================
        public ObservableCollection<ImageVM> ItemsSource
        {
            get { return (ObservableCollection<ImageVM>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<ImageVM>), typeof(ZoomboxWithImageList), new PropertyMetadata(null));
    }
}