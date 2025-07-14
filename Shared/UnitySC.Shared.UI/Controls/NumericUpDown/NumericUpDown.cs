using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

using UnitySC.Shared.UI.Extensions;

namespace UnitySC.Shared.UI.Controls
{
    public abstract class NumericUpDown<T> : Control where T : struct, IComparable<T>
    {
        private RepeatButton _upButton;
        private RepeatButton _downButton;

        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown<T>), new FrameworkPropertyMetadata(typeof(NumericUpDown<T>)));
        }

        #region DependencyProperties

        public ImageSource ImageDown
        {
            get { return (ImageSource)GetValue(ImageDownProperty); }
            set { SetValue(ImageDownProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageDownProperty =
            DependencyProperty.Register(nameof(ImageDown), typeof(ImageSource), typeof(NumericUpDown<T>), new PropertyMetadata(null));

        public ImageSource ImageUp
        {
            get { return (ImageSource)GetValue(ImageUpProperty); }
            set { SetValue(ImageUpProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageUpProperty =
            DependencyProperty.Register(nameof(ImageUp), typeof(ImageSource), typeof(NumericUpDown<T>), new PropertyMetadata(null));

        public Geometry ImageGeometryDown
        {
            get { return (Geometry)GetValue(ImageGeometryDownProperty); }
            set { SetValue(ImageGeometryDownProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageGeometry.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageGeometryDownProperty =
            DependencyProperty.Register(nameof(ImageGeometryDown), typeof(Geometry), typeof(NumericUpDown<T>), new PropertyMetadata(null));

        public Geometry ImageGeometryUp
        {
            get { return (Geometry)GetValue(ImageGeometryUpProperty); }
            set { SetValue(ImageGeometryUpProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageGeometry.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageGeometryUpProperty =
            DependencyProperty.Register(nameof(ImageGeometryUp), typeof(Geometry), typeof(NumericUpDown<T>), new PropertyMetadata(null));

        public Brush ImageGeometryBrushDown
        {
            get { return (Brush)GetValue(ImageGeometryBrushDownProperty); }
            set { SetValue(ImageGeometryBrushDownProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageGeometry.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageGeometryBrushDownProperty =
            DependencyProperty.Register(nameof(ImageGeometryBrushDown), typeof(Brush), typeof(NumericUpDown<T>), new PropertyMetadata(null));

        public Brush ImageGeometryBrushUp
        {
            get { return (Brush)GetValue(ImageGeometryBrushUpProperty); }
            set { SetValue(ImageGeometryBrushUpProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageGeometry.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageGeometryBrushUpProperty =
            DependencyProperty.Register(nameof(ImageGeometryBrushUp), typeof(Brush), typeof(NumericUpDown<T>), new PropertyMetadata(null));

        public T Maximum
        {
            get { return (T)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(T), typeof(NumericUpDown<T>), new FrameworkPropertyMetadata(OnLimitsChanged));

        public T Minimum
        {
            get { return (T)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(T), typeof(NumericUpDown<T>), new FrameworkPropertyMetadata(OnLimitsChanged));

        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register(nameof(MaxLength), typeof(int), typeof(NumericUpDown<T>), new PropertyMetadata(10));

        public T Step
        {
            get { return (T)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Step.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register(nameof(Step), typeof(T), typeof(NumericUpDown<T>), new PropertyMetadata(null));

        public MaskType InputMask
        {
            get { return (MaskType)GetValue(InputMaskProperty); }
            set { SetValue(InputMaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputMask.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputMaskProperty =
            DependencyProperty.Register(nameof(InputMask), typeof(MaskType), typeof(NumericUpDown<T>), new PropertyMetadata());

        public T InternalValue
        {
            get { return (T)GetValue(InternalValueProperty); }
            set { SetValue(InternalValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InternalValueProperty =
            DependencyProperty.Register(nameof(InternalValue), typeof(T), typeof(NumericUpDown<T>), new FrameworkPropertyMetadata(OnInternalValueChanged, OnInternalCoerceValue) { BindsTwoWayByDefault = true });

        public T Value
        {
            get { return (T)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(T), typeof(NumericUpDown<T>), new FrameworkPropertyMetadata(OnValueChanged) { BindsTwoWayByDefault = true });

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var value = (T)e.NewValue;
            if ((d as NumericUpDown<T>).InternalValue.CompareTo(value) != 0)
                (d as NumericUpDown<T>).InternalValue = value;
        }

        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Unit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register(nameof(Unit), typeof(string), typeof(NumericUpDown<T>), new PropertyMetadata(null));

        #endregion DependencyProperties

        private static void OnLimitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var value = (T)e.NewValue;

            (d as NumericUpDown<T>).LimitsChanged();
            (d as NumericUpDown<T>).UpdateButtonsStatus();
        }

        protected virtual void LimitsChanged()
        {
            InternalValue = Value;
        }

        private static object OnInternalCoerceValue(DependencyObject d, object baseValue)
        {
            var min = (d as NumericUpDown<T>).Minimum;
            var max = (d as NumericUpDown<T>).Maximum;


            var val = (T)baseValue;
            if (max.CompareTo(min) <= 0)
                return baseValue;

            if (val.CompareTo(min) < 0) return min;
            if (val.CompareTo(max) > 0) return max;
            return baseValue;
        }

        private static void OnInternalValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var value = (T)e.NewValue;
            if ((d as NumericUpDown<T>).Value.CompareTo(value) != 0)
            {
                (d as NumericUpDown<T>).Value = value;
                (d as NumericUpDown<T>).ValueChanged(value);

            }
            (d as NumericUpDown<T>).UpdateButtonsStatus();
        }

        protected virtual void ValueChanged(T value)
        {
            UpdateButtonsStatus();
        }

        private void UpdateButtonsStatus()
        {
            if ((_upButton == null) || _downButton == null)
                return;
            _upButton.IsEnabled = InternalValue.CompareTo(Maximum) < 0;
            _downButton.IsEnabled = InternalValue.CompareTo(Minimum) > 0;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _upButton = Template.FindName("Part_UpButton", this) as RepeatButton;
            _downButton = Template.FindName("Part_DownButton", this) as RepeatButton;
            _upButton.Click += _UpButton_Click;
            _downButton.Click += _DownButton_Click;

            var textBox = Template.FindName("Part_TextBox", this) as TextBox;

            var valueBinding = GetBindingExpression(ValueProperty)?.ParentBinding;
            var internalValueBinding = new Binding(nameof(InternalValue))
            {
                Source = this,
                StringFormat = valueBinding?.StringFormat
            };
            // Bind the new data source to the myText TextBlock control's Text dependency property.
            textBox.SetBinding(TextBox.TextProperty, internalValueBinding);
            UpdateButtonsStatus();
        }

        private void _DownButton_Click(object sender, RoutedEventArgs e)
        {
            InternalValue = DecrementValue(InternalValue, Step);
        }

        private void _UpButton_Click(object sender, RoutedEventArgs e)
        {
            InternalValue = IncrementValue(InternalValue, Step);
        }

        protected abstract T IncrementValue(T value, T increment);

        protected abstract T DecrementValue(T value, T increment);
    }
}
