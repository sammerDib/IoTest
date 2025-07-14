using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.UI.Controls
{
    public class TextBoxAngleUnit : TextBox
    {
        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register(nameof(Unit), typeof(string), typeof(TextBoxAngleUnit), new PropertyMetadata(string.Empty));

        public Angle AngleValue
        {
            get { return (Angle)GetValue(AngleValueProperty); }
            set { SetValue(AngleValueProperty, value); }
        }

        public static readonly DependencyProperty AngleValueProperty =
            DependencyProperty.Register(nameof(AngleValue), typeof(Angle), typeof(TextBoxAngleUnit), new FrameworkPropertyMetadata(0.Degrees(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AngleValueChanged));

        private static void AngleValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TextBoxAngleUnit).UpdateAngleValueAndUnit(e.NewValue);
        }

        private void UpdateAngleValueAndUnit(object newValue)
        {
            if (newValue is null)
                return;
            InternalValue = (newValue as Angle).Value;
            Unit = (newValue as Angle).UnitSymbol;
        }

        public double InternalValue
        {
            get { return (double)GetValue(InternalValueProperty); }
            set { SetValue(InternalValueProperty, value); }
        }

        public static readonly DependencyProperty InternalValueProperty =
            DependencyProperty.Register("InternalValue", typeof(double), typeof(TextBoxAngleUnit), new PropertyMetadata(0d, InternalValueChanged));

        private static void InternalValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TextBoxAngleUnit).UpdateValues(e.NewValue);
        }

        private void UpdateValues(object newValue)
        {
            if (!(AngleValue is null))
                AngleValue = new Angle(InternalValue, AngleValue.Unit);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            var valueBinding = GetBindingExpression(AngleValueProperty)?.ParentBinding;

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
