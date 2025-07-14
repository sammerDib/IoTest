using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.UI.Controls
{
    public class ToleranceUnitsDictionnary : Dictionary<LengthToleranceUnit, string>
    { }

    /// <summary>
    /// Interaction logic for LengthToleranceInput.xaml
    /// </summary>
    public partial class LengthToleranceInput : UserControl
    {
        public LengthToleranceInput()
        {
            InitializeComponent();
            UpdateTextInfo();
            UpdateAvailableUnits();
        }

        public LengthTolerance ToleranceValue
        {
            get { return (LengthTolerance)GetValue(ToleranceValueProperty); }
            set { SetValue(ToleranceValueProperty, value); }
        }

        public static readonly DependencyProperty ToleranceValueProperty =
            DependencyProperty.Register(nameof(ToleranceValue), typeof(LengthTolerance), typeof(LengthToleranceInput), new PropertyMetadata(new LengthTolerance(0, LengthToleranceUnit.Micrometer), ToleranceValueChanged));

        private static void ToleranceValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LengthToleranceInput).UpdateValueAndUnit(e.NewValue);
        }

        private void UpdateValueAndUnit(object newValue)
        {
            //if (ToleranceValue == newValue)
            //    return;
            if (newValue is null)
                return;

            if ((newValue as LengthTolerance).Value != Value)
                Value = (newValue as LengthTolerance).Value;
            if ((newValue as LengthTolerance).Unit != Unit)
                Unit = (newValue as LengthTolerance).Unit;
            UpdateTextInfo();
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(LengthToleranceInput), new PropertyMetadata(double.NaN, ValueChanged));

        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LengthToleranceInput).UpdateValue(e.NewValue);
        }

        private void UpdateValue(object newValue)
        {
            if (Unit is null)
                return;
            if (ToleranceValue.Value != Value)
                ToleranceValue = new LengthTolerance(Value, (LengthToleranceUnit)Unit);
            UpdateTextInfo();
        }

        public LengthToleranceUnit? Unit
        {
            get { return (LengthToleranceUnit?)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register(nameof(Unit), typeof(LengthToleranceUnit?), typeof(LengthToleranceInput), new PropertyMetadata(null, UnitChanged));

        private static void UnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LengthToleranceInput).UpdateUnit(e.NewValue);
        }

        public bool IsPercentageAvailable
        {
            get { return (bool)GetValue(IsPercentageAvailableProperty); }
            set { SetValue(IsPercentageAvailableProperty, value); }
        }

        public static readonly DependencyProperty IsPercentageAvailableProperty =
            DependencyProperty.Register(nameof(IsPercentageAvailable), typeof(bool), typeof(LengthToleranceInput), new PropertyMetadata(true, AvailableUnitsChanged));

        public bool IsMicroMeterAvailable
        {
            get { return (bool)GetValue(IsMicroMeterAvailableProperty); }
            set { SetValue(IsMicroMeterAvailableProperty, value); }
        }

        public static readonly DependencyProperty IsMicroMeterAvailableProperty =
            DependencyProperty.Register(nameof(IsMicroMeterAvailable), typeof(bool), typeof(LengthToleranceInput), new PropertyMetadata(false, AvailableUnitsChanged));

        public bool IsNanoMeterAvailable
        {
            get { return (bool)GetValue(IsNanoMeterAvailableProperty); }
            set { SetValue(IsNanoMeterAvailableProperty, value); }
        }

        public static readonly DependencyProperty IsNanoMeterAvailableProperty =
            DependencyProperty.Register(nameof(IsNanoMeterAvailable), typeof(bool), typeof(LengthToleranceInput), new PropertyMetadata(false, AvailableUnitsChanged));

        public bool IsMilliMeterAvailable
        {
            get { return (bool)GetValue(IsMilliMeterAvailableProperty); }
            set { SetValue(IsMilliMeterAvailableProperty, value); }
        }

        public static readonly DependencyProperty IsMilliMeterAvailableProperty =
            DependencyProperty.Register(nameof(IsMilliMeterAvailable), typeof(bool), typeof(LengthToleranceInput), new PropertyMetadata(false, AvailableUnitsChanged));

        private static void AvailableUnitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LengthToleranceInput).UpdateAvailableUnits();
        }

        private void UpdateAvailableUnits()
        {
            var newAvailableUnits = new Dictionary<LengthToleranceUnit, string>();

            if (IsPercentageAvailable) newAvailableUnits.Add(LengthToleranceUnit.Percentage, "%");
            if (IsMicroMeterAvailable) newAvailableUnits.Add(LengthToleranceUnit.Micrometer, "µm");
            if (IsNanoMeterAvailable) newAvailableUnits.Add(LengthToleranceUnit.Nanometer, "nm");
            if (IsMilliMeterAvailable) newAvailableUnits.Add(LengthToleranceUnit.Millimeter, "mm");
            AvailableUnits = newAvailableUnits;
        }

        private Dictionary<LengthToleranceUnit, string> AvailableUnits
        {
            get { return (Dictionary<LengthToleranceUnit, string>)GetValue(AvailableUnitsProperty); }
            set { SetValue(AvailableUnitsProperty, value); }
        }

        public static readonly DependencyProperty AvailableUnitsProperty =
          DependencyProperty.Register(nameof(AvailableUnits), typeof(Dictionary<LengthToleranceUnit, string>), typeof(LengthToleranceInput), new PropertyMetadata(new Dictionary<LengthToleranceUnit, string>()));

        public double EditBoxWidth
        {
            get { return (double)GetValue(EditBoxWidthProperty); }
            set { SetValue(EditBoxWidthProperty, value); }
        }

        public static readonly DependencyProperty EditBoxWidthProperty =
            DependencyProperty.Register(nameof(EditBoxWidth), typeof(double), typeof(LengthToleranceInput), new PropertyMetadata(100d));

        public double UnitWidth
        {
            get { return (double)GetValue(UnitWidthProperty); }
            set { SetValue(UnitWidthProperty, value); }
        }

        public static readonly DependencyProperty UnitWidthProperty =
            DependencyProperty.Register(nameof(UnitWidth), typeof(double), typeof(LengthToleranceInput), new PropertyMetadata(60d));

        private LengthUnit GetLengthUnit(LengthToleranceUnit lengthToleranceUnit)
        {
            switch (lengthToleranceUnit)
            {
                case LengthToleranceUnit.Percentage:
                    return LengthUnit.Undefined;

                case LengthToleranceUnit.Micrometer:
                    return LengthUnit.Micrometer;

                case LengthToleranceUnit.Nanometer:
                    return LengthUnit.Nanometer;

                case LengthToleranceUnit.Millimeter:
                    return LengthUnit.Millimeter;

                default:
                    return LengthUnit.Undefined;
            }
        }

        private void UpdateUnit(object newValue)
        {
            if (double.IsNaN(Value))
                return;

            if (Unit is null)
                return;

            Length currentTolerance;
            // we retrieve the current tolerance value
            if (ToleranceValue.Unit == LengthToleranceUnit.Percentage)
                currentTolerance = new Length(ToleranceValue.Value * Target.Micrometers / 100, LengthUnit.Micrometer);
            else
                currentTolerance = new Length(ToleranceValue.Value, GetLengthUnit(ToleranceValue.Unit));

            switch (Unit)
            {
                case LengthToleranceUnit.Percentage:
                    if (Target.Micrometers == 0)
                        return;
                    else
                        Value = currentTolerance.Micrometers * 100 / Target.Micrometers;
                    break;

                default:
                    Value = currentTolerance.ToUnit(GetLengthUnit((LengthToleranceUnit)Unit)).Value;
                    break;
            }

            ToleranceValue = new LengthTolerance(Value, (LengthToleranceUnit)Unit);
            UpdateTextInfo();
        }

        public Length Target
        {
            get { return (Length)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register(nameof(Target), typeof(Length), typeof(LengthToleranceInput), new PropertyMetadata(new Length(0, UnitySC.Shared.Tools.Units.LengthUnit.Micrometer), TargetChanged));

        private static void TargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LengthToleranceInput).UpdateTextInfo();
        }

        private void UpdateTextInfo()
        {
            string newTextInfo = string.Empty;

            switch (Unit)
            {
                case LengthToleranceUnit.Percentage:
                    newTextInfo = $"+/- {Target.Value * Value / 100:F2} {Target.UnitSymbol}";
                    break;

                case LengthToleranceUnit.Micrometer:
                    break;

                default:
                    break;
            }

            TextInfo.Text = newTextInfo;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //var textBox = Template.FindName("ValueTextBox", this) as TextBox;

            var toleranceValueBinding = GetBindingExpression(ToleranceValueProperty)?.ParentBinding;

            string stringFormat = string.Empty;
            if (!(toleranceValueBinding is null))
            {
                stringFormat = toleranceValueBinding.StringFormat;
            }
            else
            {
                var valueBinding = GetBindingExpression(ValueProperty)?.ParentBinding;
                if (!(valueBinding is null))
                    stringFormat = valueBinding.StringFormat;
            }

            var internalValueBinding = new Binding(nameof(Value))
            {
                Source = this,
                StringFormat = stringFormat
            };
            if (!(ValueTextBox is null))
                ValueTextBox.SetBinding(TextBox.TextProperty, internalValueBinding);
        }
    }
}
