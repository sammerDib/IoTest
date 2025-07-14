using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class IconRun : InlineUIContainer
    {
        public IconRun() : base(new Icon
        {
            UseLayoutRounding = true,
            SnapsToDevicePixels = true,
        })
        {
            BaselineAlignment = BaselineAlignment.Subscript;
        }

        public IconRun(TextPointer pos) : base(new Icon
        {
            UseLayoutRounding = true,
            SnapsToDevicePixels = true,
        }, pos)
        {
            BaselineAlignment = BaselineAlignment.Subscript;
        }

        public new Icon Child => base.Child as Icon;

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            nameof(Data),
            typeof(PathGeometry),
            typeof(IconRun),
            new PropertyMetadata(default(PathGeometry), OnDataChanged));

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IconRun self)
            {
                self.Child.Data = e.NewValue as Geometry;
            }
        }

        public PathGeometry Data
        {
            get => (PathGeometry)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        #region Overrides of TextElement

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            switch (e.Property.Name)
            {
                case nameof(FontSize):
                    OnFontSizeChanged();
                    break;
            }
        }

        private void OnFontSizeChanged()
        {
            // [TLa] The multiplier coefficients have been defined arbitrarily by manual testing on the user interface because it is not possible to know what exact size the text takes.
            Child.Margin = new Thickness { Right = FontSize * 0.15 };
            Child.Height = FontSize;
            Child.Width = FontSize;
        }

        #endregion
    }
}
