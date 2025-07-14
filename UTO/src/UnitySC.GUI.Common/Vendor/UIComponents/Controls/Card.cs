using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    /// <summary>
    /// Material uses six levels of elevation, each with a corresponding dp value.
    /// These values are named for their relative distance above the UIâs surface: 0, +1, +2, +3, +4, and +5.
    /// An elementâs resting state can be on levels 0 to +3, while levels +4 and +5 are reserved for user-interacted states such as hover and dragged.
    /// </summary>
    public enum CardElevation
    {
        Level0,
        Level1,
        Level2,
        Level3,
        Level4,
        Level5,
    }

    public class Card : ContentControl
    {
        static Card()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Card), new FrameworkPropertyMetadata(typeof(Card)));
        }

        public Card()
        {
            InitializeClipMask();
        }

        /// <summary>
        /// The creation of a clip mask allows to cut the visual rendering at the limits of the card
        /// by taking into account the rounded edges and the thickness of the borders.
        /// </summary>
        private void InitializeClipMask()
        {
            var borderClip = new Border
            {
                Background = Brushes.Black,
                BorderBrush = Brushes.Transparent,
                SnapsToDevicePixels = true,
            };

            borderClip.SetBinding(Border.CornerRadiusProperty, new Binding
            {
                Mode = BindingMode.OneWay,
                Path = new PropertyPath(nameof(CornerRadius)),
                Source = this
            });
            borderClip.SetBinding(Border.HeightProperty, new Binding
            {
                Mode = BindingMode.OneWay,
                Path = new PropertyPath(nameof(ActualHeight)),
                Source = this
            });
            borderClip.SetBinding(Border.WidthProperty, new Binding
            {
                Mode = BindingMode.OneWay,
                Path = new PropertyPath(nameof(ActualWidth)),
                Source = this
            });
            borderClip.SetBinding(Border.BorderThicknessProperty, new Binding
            {
                Mode = BindingMode.OneWay,
                Path = new PropertyPath(nameof(BorderThickness)),
                Source = this
            });

            ClipMask = new VisualBrush(borderClip);
        }
        
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius), typeof(CornerRadius), typeof(Card), new FrameworkPropertyMetadata(new CornerRadius(0), FrameworkPropertyMetadataOptions.AffectsMeasure));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty IsShadowEnabledProperty = DependencyProperty.Register(
            nameof(IsShadowEnabled), typeof(bool), typeof(Card), new PropertyMetadata(default(bool)));

        public bool IsShadowEnabled
        {
            get { return (bool)GetValue(IsShadowEnabledProperty); }
            set { SetValue(IsShadowEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsSurfaceEnabledProperty = DependencyProperty.Register(
            nameof(IsSurfaceEnabled), typeof(bool), typeof(Card), new PropertyMetadata(default(bool)));

        public bool IsSurfaceEnabled
        {
            get { return (bool)GetValue(IsSurfaceEnabledProperty); }
            set { SetValue(IsSurfaceEnabledProperty, value); }
        }

        public static readonly DependencyProperty ElevationProperty = DependencyProperty.Register(
            nameof(Elevation), typeof(CardElevation), typeof(Card), new PropertyMetadata(CardElevation.Level0));

        public CardElevation Elevation
        {
            get { return (CardElevation)GetValue(ElevationProperty); }
            set { SetValue(ElevationProperty, value); }
        }

        public static readonly DependencyProperty SurfaceProperty = DependencyProperty.Register(
            nameof(Surface), typeof(Brush), typeof(Card), new PropertyMetadata(default(Brush)));

        /// <summary>
        /// While background color remains consistent, surface color is not static.
        /// Surfaces at elevation levels +1 to +5 are tinted via color overlays based on the primary color, such as app bars or menus.The addition of a grade from +1 to +5 introduces tonal variation to the surface baseline.
        /// Tonal surfaces provide a few benefits:
        /// - Simulate the effect of elevation to create differentiation amongst competing content areas.
        /// - Establish contrast for accessibility benefits.
        /// - Create visual interest and soften transitions between interactive elements.
        /// </summary>
        public Brush Surface
        {
            get { return (Brush)GetValue(SurfaceProperty); }
            set { SetValue(SurfaceProperty, value); }
        }

        public static readonly DependencyProperty ClipContentProperty = DependencyProperty.Register(
            nameof(ClipContent), typeof(bool), typeof(Card), new PropertyMetadata(default(bool)));

        public bool ClipContent
        {
            get { return (bool)GetValue(ClipContentProperty); }
            set { SetValue(ClipContentProperty, value); }
        }

        public static readonly DependencyProperty ClipMaskProperty = DependencyProperty.Register(
            nameof(ClipMask),
            typeof(object),
            typeof(Card),
            new PropertyMetadata(default(object)));

        public object ClipMask
        {
            get { return GetValue(ClipMaskProperty); }
            set { SetValue(ClipMaskProperty, value); }
        }
    }
}
