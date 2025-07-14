using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Compatibility;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.Controls
{
    /// <summary>
    /// Interaction logic for ProbeLiseGraph.xaml
    /// </summary>
    public partial class ProbeLiseGraph : UserControl, INotifyPropertyChanged
    {
        public ProbeLiseGraph()
        {
            InitializeComponent();
        }

        public ProbeLiseBaseVM ProbeLise
        {
            get { return (ProbeLiseBaseVM)GetValue(ProbeLiseProperty); }
            set { SetValue(ProbeLiseProperty, value); }
        }

        public static readonly DependencyProperty ProbeLiseProperty =
            DependencyProperty.Register(nameof(ProbeLise), typeof(ProbeLiseBaseVM), typeof(ProbeLiseGraph), new PropertyMetadata(null, OnProbeLiseChanged));

        private static void OnProbeLiseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ProbeLiseGraph).IsDualLise = (e.NewValue is ProbeLiseDoubleVM);
            (d as ProbeLiseGraph).StartAcquisitionIfNeeded();
        }

        public bool DisplaySelectedPeaks
        {
            get { return (bool)GetValue(DisplaySelectedPeaksProperty); }
            set { SetValue(DisplaySelectedPeaksProperty, value); }
        }

        public static readonly DependencyProperty DisplaySelectedPeaksProperty =
            DependencyProperty.Register(nameof(DisplaySelectedPeaks), typeof(bool), typeof(ProbeLiseGraph), new PropertyMetadata(false));

        public bool DisplayDiscarderPeaks
        {
            get { return (bool)GetValue(DisplayDiscarderPeaksProperty); }
            set { SetValue(DisplayDiscarderPeaksProperty, value); }
        }

        public static readonly DependencyProperty DisplayDiscarderPeaksProperty =
            DependencyProperty.Register(nameof(DisplayDiscarderPeaks), typeof(bool), typeof(ProbeLiseGraph), new PropertyMetadata(false));

        public bool EnableMouseInteraction
        {
            get { return (bool)GetValue(EnableMouseInteractionProperty); }
            set { SetValue(EnableMouseInteractionProperty, value); }
        }

        public static readonly DependencyProperty EnableMouseInteractionProperty =
            DependencyProperty.Register(nameof(EnableMouseInteraction), typeof(bool), typeof(ProbeLiseGraph), new PropertyMetadata(false));

        public bool DisplayAxes
        {
            get { return (bool)GetValue(DisplayAxesProperty); }
            set { SetValue(DisplayAxesProperty, value); }
        }

        public static readonly DependencyProperty DisplayAxesProperty =
            DependencyProperty.Register(nameof(DisplayAxes), typeof(bool), typeof(ProbeLiseGraph), new PropertyMetadata(false));

        public bool DisplaySaturationLine
        {
            get { return (bool)GetValue(DisplaySaturationLineProperty); }
            set { SetValue(DisplaySaturationLineProperty, value); }
        }

        public static readonly DependencyProperty DisplaySaturationLineProperty =
            DependencyProperty.Register(nameof(DisplaySaturationLine), typeof(bool), typeof(ProbeLiseGraph), new PropertyMetadata(true));

        public bool DisplayAnnotations
        {
            get { return (bool)GetValue(DisplayAnnotationsProperty); }
            set { SetValue(DisplayAnnotationsProperty, value); }
        }

        public static readonly DependencyProperty DisplayAnnotationsProperty =
            DependencyProperty.Register(nameof(DisplayAnnotations), typeof(bool), typeof(ProbeLiseGraph), new PropertyMetadata(true));

        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(nameof(BackgroundColor), typeof(Color), typeof(ProbeLiseGraph), new PropertyMetadata(Colors.Gray));

        public bool IsAcquiring
        {
            get { return (bool)GetValue(IsAcquiringProperty); }
            set { SetValue(IsAcquiringProperty, value); }
        }

        public static readonly DependencyProperty IsAcquiringProperty =
            DependencyProperty.Register(nameof(IsAcquiring), typeof(bool), typeof(ProbeLiseGraph), new PropertyMetadata(false, OnIsAcquiringChanged));

        private static void OnIsAcquiringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool isAcquiring = (bool)e.NewValue;
            if (isAcquiring)
                (d as ProbeLiseGraph).StartContinuousAcquisition();
            else
                (d as ProbeLiseGraph).StopContinuousAcquisition();
        }

        public double Gain
        {
            get { return (double)GetValue(GainProperty); }
            set { SetValue(GainProperty, value); }
        }

        public static readonly DependencyProperty GainProperty =
            DependencyProperty.Register(nameof(Gain), typeof(double), typeof(ProbeLiseGraph), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender, OnGainChanged)
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

        private static void OnGainChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((e.NewValue is double newGain) && (newGain==0))
                return;
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                (d as ProbeLiseGraph).StartContinuousAcquisition();
            }));
        }

        public double GainUp
        {
            get { return (double)GetValue(GainUpProperty); }
            set { SetValue(GainUpProperty, value); }
        }

        public static readonly DependencyProperty GainUpProperty =
            DependencyProperty.Register(nameof(GainUp), typeof(double), typeof(ProbeLiseGraph), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender, OnGainChanged)
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

        public double GainDown
        {
            get { return (double)GetValue(GainDownProperty); }
            set { SetValue(GainDownProperty, value); }
        }

        public static readonly DependencyProperty GainDownProperty =
            DependencyProperty.Register(nameof(GainDown), typeof(double), typeof(ProbeLiseGraph), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender, OnGainChanged)
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

        public bool DisplayGainSelector
        {
            get { return (bool)GetValue(DisplayGainSelectorProperty); }
            set { SetValue(DisplayGainSelectorProperty, value); }
        }

        public static readonly DependencyProperty DisplayGainSelectorProperty =
            DependencyProperty.Register(nameof(DisplayGainSelector), typeof(bool), typeof(ProbeLiseGraph), new PropertyMetadata(true));

        private void StartAcquisitionIfNeeded()
        {
            if (IsAcquiring)
                StartContinuousAcquisition();
        }

        private void StopContinuousAcquisition()
        {
            if (ProbeLise is null)
                return;
            ProbeLise.StopContinuousAcquisition();
        }

        private SingleLiseInputParams CreateProbeInputParametersLise(double gain)
        {
            var tolerance = new LengthTolerance(10, LengthToleranceUnit.Nanometer);
            var probeSampleLayerMeasured = new ProbeSampleLayer(750.Nanometers(), tolerance, 1.4621);
            var probeLayers = new List<ProbeSampleLayer>() { probeSampleLayerMeasured };
            ProbeSample sample = new ProbeSample(probeLayers, "REF 750UM", "SampleInfo");

            double qualityThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            double detectionThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            int nbMeasures = 16;
            var probeInputParameters = new SingleLiseInputParams(sample, gain, qualityThreshold, detectionThreshold, nbMeasures);

            return probeInputParameters;
        }

        private DualLiseInputParams CreateProbeInputParametersDualLise(double gainUp, double gainDown, ModulePositions probePosition)
        {
            var tolerance = new LengthTolerance(10, LengthToleranceUnit.Nanometer);
            var probeSampleLayerMeasured = new ProbeSampleLayer(750.Nanometers(), tolerance, 1.4621);
            var probeLayers = new List<ProbeSampleLayer>() { probeSampleLayerMeasured };
            ProbeSample sample = new ProbeSample(probeLayers, "REF 750UM", "SampleInfo");

            ((ProbeLiseDoubleVM)ProbeLise).IsAcquisitionForProbeUp = (probePosition == ModulePositions.Up);

            double qualityThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            double detectionThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            int nbMeasures = 16;

            var probeUpParams = new SingleLiseInputParams(sample, gainUp, qualityThreshold, detectionThreshold, nbMeasures);
            var probeDownParams = new SingleLiseInputParams(sample, gainDown, qualityThreshold, detectionThreshold, nbMeasures);

            var probeInputParameters = new DualLiseInputParams(sample, probeUpParams, probeDownParams);

            var configLiseDouble = ((ProbeLiseDoubleVM)ProbeLise).Configuration as ProbeConfigurationLiseDoubleVM;
            if (probePosition == ModulePositions.Up)
            {
                probeInputParameters.CurrentProbeAcquisition = configLiseDouble.ProbeUp.DeviceID;
                probeInputParameters.CurrentProbeModule = configLiseDouble.ProbeUp.ModulePosition;
            }
            else
            {
                probeInputParameters.CurrentProbeAcquisition = configLiseDouble.ProbeDown.DeviceID;
                probeInputParameters.CurrentProbeModule = configLiseDouble.ProbeDown.ModulePosition;
            }

            return probeInputParameters;
        }

        private void StartContinuousAcquisition()
        {
            if (ProbeLise is null)
                return;

            if (IsDualLise)
            {
                ModulePositions probePosition = (ModulePositions)SelectedDualProbePosition;
                ProbeLise.StartContinuousAcquisition(CreateProbeInputParametersDualLise(GainUp,GainDown, probePosition));
            }
            else
            {
                ProbeLise.StartContinuousAcquisition(CreateProbeInputParametersLise(Gain));
            }
        }

        private bool _isDualLise = false;

        public bool IsDualLise
        {
            get => _isDualLise; set { if (_isDualLise != value) { _isDualLise = value; OnPropertyChanged(nameof(IsDualLise)); } }
        }

        private int _selectedDualProbePosition;

        public int SelectedDualProbePosition
        {
            get { return _selectedDualProbePosition; }
            set
            {
                _selectedDualProbePosition = value;
                StartAcquisitionIfNeeded();
                OnPropertyChanged(nameof(SelectedDualProbePosition));
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        private void ProbeLiseGraphUC_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;
            if (!isVisible)
                StopContinuousAcquisition();
            else if (!IsAcquiring)
            {
                IsAcquiring = true;
                //StartContinuousAcquisition();
            }
        }
    }
}
