using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.Controls
{
    public class NumericUpDown : Control
    {
        private RepeatButton _upButton;
        private RepeatButton _downButton;
        public static readonly DependencyProperty MaximumProperty;
        public static readonly DependencyProperty MinimumProperty;
        public static readonly DependencyProperty MaxLengthProperty;
        public static readonly DependencyProperty ValueProperty;
        public static readonly DependencyProperty StepProperty;
        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
            MaximumProperty = DependencyProperty.Register("Maximum", typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(10));
            MinimumProperty = DependencyProperty.Register("Minimum", typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(0));
            MaxLengthProperty = DependencyProperty.Register("MaxLength", typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(10));

            StepProperty = DependencyProperty.Register("StepValue", typeof(int), typeof(NumericUpDown), new FrameworkPropertyMetadata(5));
            ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(NumericUpDown), new FrameworkPropertyMetadata(0));
        }
        #region DpAccessior
        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetCurrentValue(ValueProperty, value); }
        }
        public int StepValue
        {
            get { return (int)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }
        #endregion
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _upButton = Template.FindName("Part_UpButton", this) as RepeatButton;
            _downButton = Template.FindName("Part_DownButton", this) as RepeatButton;
            _upButton.Click += _UpButton_Click;
            _downButton.Click += _DownButton_Click;
        }

        void _DownButton_Click(object sender, RoutedEventArgs e)
        {
            if (Value > Minimum)
            {
                Value -= StepValue;
                if (Value < Minimum)
                    Value = Minimum;
            }
        }

        void _UpButton_Click(object sender, RoutedEventArgs e)
        {
            if (Value < Maximum)
            {
                Value += StepValue;
                if (Value > Maximum)
                    Value = Maximum;

            }
        }
    }
}
