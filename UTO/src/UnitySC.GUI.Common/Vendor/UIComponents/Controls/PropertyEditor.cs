using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class PropertyEditor : ContentControl
    {
        #region Apply Default Style

        static PropertyEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(PropertyEditor),
                new FrameworkPropertyMetadata(typeof(PropertyEditor)));
        }

        #endregion

        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(
            nameof(PropertyName),
            typeof(object),
            typeof(PropertyEditor),
            new PropertyMetadata(default(object)));

        [Category("Main")]
        public object PropertyName
        {
            get => GetValue(PropertyNameProperty);
            set => SetValue(PropertyNameProperty, value);
        }

        public static readonly DependencyProperty PropertyDescriptionProperty = DependencyProperty.Register(
            nameof(PropertyDescription),
            typeof(string),
            typeof(PropertyEditor),
            new PropertyMetadata(default(string)));

        [Category("Main")]
        public string PropertyDescription
        {
            get => (string)GetValue(PropertyDescriptionProperty);
            set => SetValue(PropertyDescriptionProperty, value);
        }

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            nameof(Orientation),
            typeof(Orientation),
            typeof(PropertyEditor),
            new PropertyMetadata(Orientation.Vertical));

        [Category("Main")]
        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public static readonly DependencyProperty ContentPaddingProperty = DependencyProperty.Register(
            nameof(ContentPadding),
            typeof(Thickness),
            typeof(PropertyEditor),
            new PropertyMetadata(default(Thickness)));

        [Category("Main")]
        public Thickness ContentPadding
        {
            get => (Thickness)GetValue(ContentPaddingProperty);
            set => SetValue(ContentPaddingProperty, value);
        }

        public static readonly DependencyProperty HeaderPaddingProperty = DependencyProperty.Register(
            nameof(HeaderPadding),
            typeof(Thickness),
            typeof(PropertyEditor),
            new PropertyMetadata(default(Thickness)));

        [Category("Main")]
        public Thickness HeaderPadding
        {
            get => (Thickness)GetValue(HeaderPaddingProperty);
            set => SetValue(HeaderPaddingProperty, value);
        }
    }
}
