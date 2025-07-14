using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using System.Windows.Markup;
using System.Windows.Media;

using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Interface;

namespace UnitySC.PM.ANA.Client.Controls
{
    /// <summary>
    /// Interaction logic for ProbeLiseHFGraph.xaml
    /// </summary>
    public partial class ProbeLiseHFGraph : UserControl, INotifyPropertyChanged
    {
        public ProbeLiseHFGraph()
        {
            InitializeComponent();
        }

        #region Properties
        public ProbeLiseHFVM ProbeLiseHF
        {
            get { return (ProbeLiseHFVM)GetValue(ProbeLiseProperty); }
            set { SetValue(ProbeLiseProperty, value); }
        }

        public static readonly DependencyProperty ProbeLiseProperty =
            DependencyProperty.Register(nameof(ProbeLiseHF), typeof(ProbeLiseHFVM), typeof(ProbeLiseHFGraph), new PropertyMetadata(null, OnProbeLiseHFChanged));

        private static void OnProbeLiseHFChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ProbeLiseHFVM)
                (d as ProbeLiseHFGraph).StartAcquisitionIfNeeded();
            else
                (d as ProbeLiseHFGraph).StopContinuousAcquisition(e.OldValue as ProbeLiseHFVM);
        }

        public bool EnableMouseInteraction
        {
            get { return (bool)GetValue(EnableMouseInteractionProperty); }
            set { SetValue(EnableMouseInteractionProperty, value); }
        }

        public static readonly DependencyProperty EnableMouseInteractionProperty =
            DependencyProperty.Register(nameof(EnableMouseInteraction), typeof(bool), typeof(ProbeLiseHFGraph), new PropertyMetadata(false));

        public bool DisplayAxes
        {
            get { return (bool)GetValue(DisplayAxesProperty); }
            set { SetValue(DisplayAxesProperty, value); }
        }

        public static readonly DependencyProperty DisplayAxesProperty =
            DependencyProperty.Register(nameof(DisplayAxes), typeof(bool), typeof(ProbeLiseHFGraph), new PropertyMetadata(false));

        public bool DisplaySaturationLine
        {
            get { return (bool)GetValue(DisplaySaturationLineProperty); }
            set { SetValue(DisplaySaturationLineProperty, value); }
        }

        public static readonly DependencyProperty DisplaySaturationLineProperty =
            DependencyProperty.Register(nameof(DisplaySaturationLine), typeof(bool), typeof(ProbeLiseHFGraph), new PropertyMetadata(true));

        public bool DisplayAnnotations
        {
            get { return (bool)GetValue(DisplayAnnotationsProperty); }
            set { SetValue(DisplayAnnotationsProperty, value); }
        }

        public static readonly DependencyProperty DisplayAnnotationsProperty =
            DependencyProperty.Register(nameof(DisplayAnnotations), typeof(bool), typeof(ProbeLiseHFGraph), new PropertyMetadata(true));

        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(nameof(BackgroundColor), typeof(Color), typeof(ProbeLiseHFGraph), new PropertyMetadata(Colors.Gray));

        public bool IsAcquiring
        {
            get { return (bool)GetValue(IsAcquiringProperty); }
            set { SetValue(IsAcquiringProperty, value); }
        }

        public static readonly DependencyProperty IsAcquiringProperty =
            DependencyProperty.Register(nameof(IsAcquiring), typeof(bool), typeof(ProbeLiseHFGraph), new PropertyMetadata(false, OnIsAcquiringChanged));

        private static void OnIsAcquiringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d as ProbeLiseHFGraph).IsVisible)
                return;
            bool isAcquiring = (bool)e.NewValue;
            if (isAcquiring)
                (d as ProbeLiseHFGraph).StartContinuousAcquisition();
            else
                (d as ProbeLiseHFGraph).StopContinuousAcquisition();
        }

        public bool IsCalibrationRequired
        {
            get { return (bool)GetValue(IsCalibrationRequiredProperty); }
            set { SetValue(IsCalibrationRequiredProperty, value); }
        }

        public static readonly DependencyProperty IsCalibrationRequiredProperty =
            DependencyProperty.Register(nameof(IsCalibrationRequired), typeof(bool), typeof(ProbeLiseHFGraph), new PropertyMetadata(false));

        public double BandBegin
        {
            get { return (double)GetValue(BandBeginProperty); }
            set { SetValue(BandBeginProperty, value); }
        }

        public static readonly DependencyProperty BandBeginProperty =
            DependencyProperty.Register(nameof(BandBegin), typeof(double), typeof(ProbeLiseHFGraph), new PropertyMetadata(0.0));

        public double BandEnd
        {
            get { return (double)GetValue(BandEndProperty); }
            set { SetValue(BandEndProperty, value); }
        }

        public static readonly DependencyProperty BandEndProperty =
            DependencyProperty.Register(nameof(BandEnd), typeof(double), typeof(ProbeLiseHFGraph), new PropertyMetadata(0.0));
        #endregion
       
        private void StartAcquisitionIfNeeded()
        {
            if (IsAcquiring)
                StartContinuousAcquisition();
        }

        private void StopContinuousAcquisition()
        {
            if (ProbeLiseHF is null)
                return;
            ProbeLiseHF.StopContinuousAcquisition();
        }

        private void StopContinuousAcquisition(ProbeLiseHFVM probeLiseHF)
        {
            if (probeLiseHF is null)
                return;
            probeLiseHF.StopContinuousAcquisition();
        }

        private void StartContinuousAcquisition()
        {
            if (ProbeLiseHF is null)
                return;

            ProbeLiseHF.StartContinuousAcquisition();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        private void ProbeLiseHFGraphUC_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                StartAcquisitionIfNeeded();
            else
                StopContinuousAcquisition();
        }


    }

    [ValueConversion(typeof(int), typeof(Color))]
    public class SaturationLevelToColorConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var saturationLevel = values[0] is int ? (int)values[0] : 0;

            var probe = values[1] is ProbeLiseHFVM ? (ProbeLiseHFVM)values[1] : null;

            if (probe != null)
            {
                if (saturationLevel < (probe.Configuration as ProbeLiseHFConfig).LowSaturationThreshold)
                    return new SolidColorBrush(Colors.Orange);

                if (saturationLevel > (probe.Configuration as ProbeLiseHFConfig).HighSaturationThreshold)
                    return new SolidColorBrush(Colors.DarkRed);
                return new SolidColorBrush(Colors.Green);
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    [ValueConversion(typeof(double), typeof(Color))]
    public class QualityLevelToColorConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var qualityLevel = values[0] is double ? (double)values[0] : 0.0;

            var probe = values[1] is ProbeLiseHFVM ? (ProbeLiseHFVM)values[1] : null;

            if (probe != null)
            { 
                if (qualityLevel > (probe.Configuration as ProbeLiseHFConfig).HighQualityThreshold)
                     return new SolidColorBrush(Colors.Green);

                if (qualityLevel < (probe.Configuration as ProbeLiseHFConfig).LowQualityThreshold)
                    return new SolidColorBrush(Colors.DarkRed);

                return new SolidColorBrush(Colors.Orange);
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
