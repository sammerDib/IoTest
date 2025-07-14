using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.UI.Controls
{
    public class TextBoxUnit : TextBox
    {
        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register(nameof(Unit), typeof(string), typeof(TextBoxUnit), new PropertyMetadata(string.Empty));

        public Length LengthValue
        {
            get { return (Length)GetValue(LengthValueProperty); }
            set { SetValue(LengthValueProperty, value); }
        }

        public static readonly DependencyProperty LengthValueProperty =
            DependencyProperty.Register(nameof(LengthValue), typeof(Length), typeof(TextBoxUnit), new FrameworkPropertyMetadata(0.Micrometers(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, LengthValueChanged));

        private static void LengthValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TextBoxUnit).UpdateValueAndUnit(e.NewValue);
        }

        private void UpdateValueAndUnit(object newValue)
        {
            if (newValue is null)
                return;
            //Text = (newValue as Length).Value.ToString();
            InternalValue = (newValue as Length).Value;
            Unit = (newValue as Length).UnitSymbol;
        }

        public double InternalValue
        {
            get { return (double)GetValue(InternalValueProperty); }
            set { SetValue(InternalValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InternalValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InternalValueProperty =
            DependencyProperty.Register("InternalValue", typeof(double), typeof(TextBoxUnit), new PropertyMetadata(0d, InternalValueChanged));

        private static void InternalValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TextBoxUnit).UpdateValues(e.NewValue);
        }

        private void UpdateValues(object newValue)
        {
            if (!(LengthValue is null))
                LengthValue = new Length(InternalValue, LengthValue.Unit);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var valueBinding = GetBindingExpression(LengthValueProperty)?.ParentBinding;

            string path;
            if (!(valueBinding is null))
            {
                string stringFormat = valueBinding.StringFormat;
                path = "InternalValue";
                var internalValueBinding = new Binding(path)
                {
                    Source = this,
                    StringFormat = stringFormat,
                };

                SetBinding(TextBox.TextProperty, internalValueBinding);
            }
        }
    }
}
