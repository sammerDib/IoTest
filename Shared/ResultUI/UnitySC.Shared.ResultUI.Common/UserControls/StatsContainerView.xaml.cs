using System.Windows;

using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Common.UserControls
{
    /// <summary>
    /// Interaction logic for TsvStatsContainerView.xaml
    /// </summary>
    public partial class StatsContainerView
    {
        private const string NullValue = "-";

        public StatsContainerView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            nameof(Header), typeof(string), typeof(StatsContainerView), new PropertyMetadata(default(string)));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty StatsContainerProperty = DependencyProperty.Register(
            nameof(StatsContainer), typeof(IStatsContainer), typeof(StatsContainerView), new PropertyMetadata(default(IStatsContainer), OnDisplaySettingChanged));

        public IStatsContainer StatsContainer
        {
            get { return (IStatsContainer)GetValue(StatsContainerProperty); }
            set { SetValue(StatsContainerProperty, value); }
        }

        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(
            nameof(Unit), typeof(LengthUnit), typeof(StatsContainerView), new PropertyMetadata(LengthUnit.Micrometer, OnDisplaySettingChanged));

        public LengthUnit Unit
        {
            get { return (LengthUnit)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public static readonly DependencyProperty UnitSymbolProperty = DependencyProperty.Register(
            nameof(UnitSymbol), typeof(string), typeof(StatsContainerView), new PropertyMetadata(default(string), OnDisplaySettingChanged));

        public string UnitSymbol
        {
            get { return (string)GetValue(UnitSymbolProperty); }
            set { SetValue(UnitSymbolProperty, value); }
        }

        public static readonly DependencyProperty DigitsProperty = DependencyProperty.Register(
            nameof(Digits), typeof(int), typeof(StatsContainerView), new PropertyMetadata(3, OnDisplaySettingChanged));
        
        public int Digits
        {
            get { return (int)GetValue(DigitsProperty); }
            set { SetValue(DigitsProperty, value); }
        }

        private static void OnDisplaySettingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StatsContainerView self)
            {
                if (string.IsNullOrWhiteSpace(self.UnitSymbol))
                {
                    self.Max = LengthToStringConverter.ConvertToString(self.StatsContainer?.Max, self.Digits, true, NullValue, self.Unit);
                    self.Min = LengthToStringConverter.ConvertToString(self.StatsContainer?.Min, self.Digits, true, NullValue, self.Unit);
                    self.Delta = LengthToStringConverter.ConvertToString(self.StatsContainer?.Delta, self.Digits, true, NullValue, self.Unit);
                    self.Mean = LengthToStringConverter.ConvertToString(self.StatsContainer?.Mean, self.Digits, true, NullValue, self.Unit);
                    self.StdDev = LengthToStringConverter.ConvertToString(self.StatsContainer?.StdDev, self.Digits, true, NullValue, self.Unit);
                    self.Median = LengthToStringConverter.ConvertToString(self.StatsContainer?.Median, self.Digits, true, NullValue, self.Unit);
                }
                else
                {
                    self.Max = LengthToStringConverter.ConvertToString(self.StatsContainer?.Max, self.Digits, self.UnitSymbol);
                    self.Min = LengthToStringConverter.ConvertToString(self.StatsContainer?.Min, self.Digits, self.UnitSymbol);
                    self.Delta = LengthToStringConverter.ConvertToString(self.StatsContainer?.Delta, self.Digits, self.UnitSymbol);
                    self.Mean = LengthToStringConverter.ConvertToString(self.StatsContainer?.Mean, self.Digits, self.UnitSymbol);
                    self.StdDev = LengthToStringConverter.ConvertToString(self.StatsContainer?.StdDev, self.Digits, self.UnitSymbol);
                    self.Median = LengthToStringConverter.ConvertToString(self.StatsContainer?.Median, self.Digits, self.UnitSymbol);
                }
            }
        }

        #region Values

        public static readonly DependencyPropertyKey MaxPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Max), typeof(string), typeof(StatsContainerView), new FrameworkPropertyMetadata(NullValue,
                FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty MaxProperty = MaxPropertyKey.DependencyProperty;

        public string Max
        {
            get { return (string)GetValue(MaxProperty); }
            protected set { SetValue(MaxPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey MinPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Min), typeof(string), typeof(StatsContainerView), new FrameworkPropertyMetadata(NullValue,
                FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty MinProperty = MinPropertyKey.DependencyProperty;

        public string Min
        {
            get { return (string)GetValue(MinProperty); }
            protected set { SetValue(MinPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey DeltaPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Delta), typeof(string), typeof(StatsContainerView), new FrameworkPropertyMetadata(NullValue,
                FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty DeltaProperty = DeltaPropertyKey.DependencyProperty;

        public string Delta
        {
            get { return (string)GetValue(DeltaProperty); }
            protected set { SetValue(DeltaPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey MeanPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Mean), typeof(string), typeof(StatsContainerView), new FrameworkPropertyMetadata(NullValue,
                FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty MeanProperty = MeanPropertyKey.DependencyProperty;

        public string Mean
        {
            get { return (string)GetValue(MeanProperty); }
            protected set { SetValue(MeanPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey StdDevPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(StdDev), typeof(string), typeof(StatsContainerView), new FrameworkPropertyMetadata(NullValue,
                FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty StdDevProperty = StdDevPropertyKey.DependencyProperty;

        public string StdDev
        {
            get { return (string)GetValue(StdDevProperty); }
            protected set { SetValue(StdDevPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey MedianPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Median), typeof(string), typeof(StatsContainerView), new FrameworkPropertyMetadata(NullValue,
                FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty MedianProperty = MedianPropertyKey.DependencyProperty;

        public string Median
        {
            get { return (string)GetValue(MedianProperty); }
            protected set { SetValue(MedianPropertyKey, value); }
        }

        #endregion
    }
}
