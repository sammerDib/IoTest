using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UnitySC.PM.DMT.Modules.Settings.ViewModel;

namespace UnitySC.PM.DMT.Modules.Settings.View
{
    /// <summary>
    /// Interaction logic for FocusOverlay.xaml
    /// </summary>
    public partial class FocusOverlay : UserControl
    {
        public FocusOverlay()
        {
            InitializeComponent();
        }

        public List<FocusDataVM> FocusOverlayItems
        {
            get { return (List<FocusDataVM>)GetValue(FocusOverlayItemsProperty); }
            set { SetValue(FocusOverlayItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FocusDataItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FocusOverlayItemsProperty =
            DependencyProperty.Register("FocusOverlayItems", typeof(List<FocusDataVM>), typeof(FocusOverlay), new PropertyMetadata(null));

    }







}
