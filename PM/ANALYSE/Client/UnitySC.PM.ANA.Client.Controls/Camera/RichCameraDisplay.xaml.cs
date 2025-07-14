using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UnitySC.PM.ANA.Client.Controls.Camera
{
    /// <summary>
    /// Interaction logic for RichCameraDisplay.xaml
    /// </summary>
    public partial class RichCameraDisplay : UserControl
    {
        public RichCameraDisplay()
        {
            InitializeComponent();
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            webHelpBrowser.Refresh();
        }

        public bool UsePixelUnit
        {
            get { return (bool)GetValue(UsePixelUnitProperty); }
            set { SetValue(UsePixelUnitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UsePixelUnit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UsePixelUnitProperty =
            DependencyProperty.Register("UsePixelUnit", typeof(bool), typeof(RichCameraDisplay), new PropertyMetadata(false));

        public bool MoveIsEnabled
        {
            get { return (bool)GetValue(MoveIsEnabledProperty); }
            set { SetValue(MoveIsEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MoveIsEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MoveIsEnabledProperty =
            DependencyProperty.Register("MoveIsEnabled", typeof(bool), typeof(RichCameraDisplay), new PropertyMetadata(true));

        public bool IsAutoNormaliseSelectorVisible
        {
            get { return (bool)GetValue(IsAutoNormaliseSelectorVisibleProperty); }
            set { SetValue(IsAutoNormaliseSelectorVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAutoNormaliseSelectorVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAutoNormaliseSelectorVisibleProperty =
            DependencyProperty.Register("IsAutoNormaliseSelectorVisible", typeof(bool), typeof(RichCameraDisplay), new PropertyMetadata(false));

        public bool IsCameraSelectionVisible
        {
            get { return (bool)GetValue(IsCameraSelectionVisibleProperty); }
            set { SetValue(IsCameraSelectionVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCameraSelectionVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCameraSelectionVisibleProperty =
            DependencyProperty.Register("IsCameraSelectionVisible", typeof(bool), typeof(RichCameraDisplay), new PropertyMetadata(true));

        public bool IsStartButtonVisible
        {
            get { return (bool)GetValue(IsStartButtonVisibleProperty); }
            set { SetValue(IsStartButtonVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsStartButtonVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsStartButtonVisibleProperty =
            DependencyProperty.Register("IsStartButtonVisible", typeof(bool), typeof(RichCameraDisplay), new PropertyMetadata(true));

        #region ROI

        // Roi size in pixels on the full image
        public Size RoiSize
        {
            get { return (Size)GetValue(RoiSizeProperty); }
            set { SetValue(RoiSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RoiRect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RoiSizeProperty =
            DependencyProperty.Register(nameof(RoiSize), typeof(Size), typeof(RichCameraDisplay), new PropertyMetadata(new Size(0, 0)));

        // Roi Rect on the full image
        public Rect RoiRect
        {
            get { return (Rect)GetValue(RoiRectProperty); }
            set { SetValue(RoiRectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RoiRect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RoiRectProperty =
            DependencyProperty.Register(nameof(RoiRect), typeof(Rect), typeof(RichCameraDisplay), new PropertyMetadata(new Rect(0, 0, 0, 0)));

        public bool IsRoiSelectorVisible
        {
            get { return (bool)GetValue(IsRoiSelectorVisibleProperty); }
            set { SetValue(IsRoiSelectorVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRoiRectVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRoiSelectorVisibleProperty =
            DependencyProperty.Register(nameof(IsRoiSelectorVisible), typeof(bool), typeof(RichCameraDisplay), new PropertyMetadata(false));

        public bool IsCenteredROI
        {
            get { return (bool)GetValue(IsCenteredROIProperty); }
            set { SetValue(IsCenteredROIProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CenteredRoiIsEnabledProperty.  This enables centred roi...
        public static readonly DependencyProperty IsCenteredROIProperty =
            DependencyProperty.Register("IsCenteredROI", typeof(bool), typeof(RichCameraDisplay), new PropertyMetadata(true));

        #endregion ROI
    }
}
