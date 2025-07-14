using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.Modules.TestMeasure.ViewModel
{
    public class TSVMeasureVM : ObservableObject, IWizardNavigationItem, IDisposable
    {
        public string Name { get; set; } = "TSV";
        public bool IsEnabled { get; set; } = true;
        public bool IsMeasure { get; set; } = false;
        public bool IsValidated { get; set; } = false;
        private TestMeasureVM _testMeasureVM;

        private LengthVM _depthTarget = new LengthVM(10.Micrometers());

        public LengthVM DepthTarget
        {
            get => _depthTarget; set { if (_depthTarget != value) { _depthTarget = value; OnPropertyChanged(); } }
        }

        private LengthVM _depthOffset = new LengthVM(0.Micrometers());

        public LengthVM DepthOffset
        {
            get => _depthOffset; set { if (_depthOffset != value) { _depthOffset = value; OnPropertyChanged(); } }
        }

        private LengthTolerance _depthTolerance = new LengthTolerance(1, LengthToleranceUnit.Micrometer);

        public LengthTolerance DepthTolerance
        {
            get => _depthTolerance; set { if (_depthTolerance != value) { _depthTolerance = value; OnPropertyChanged(); } }
        }

        private LengthVM _lengthTarget = new LengthVM(10.Micrometers());

        public LengthVM LengthTarget
        {
            get => _lengthTarget; set { if (_lengthTarget != value) { _lengthTarget = value; OnPropertyChanged(); } }
        }

        private LengthVM _lengthOffset = new LengthVM(0.Micrometers());

        public LengthVM LengthOffset
        {
            get => _lengthOffset; set { if (_lengthOffset != value) { _lengthOffset = value; OnPropertyChanged(); } }
        }

        private LengthTolerance _lengthTolerance = new LengthTolerance(1, LengthToleranceUnit.Micrometer);

        public LengthTolerance LengthTolerance
        {
            get => _lengthTolerance; set { if (_lengthTolerance != value) { _lengthTolerance = value; OnPropertyChanged(); } }
        }

        private LengthVM _widthTarget = new LengthVM(10.Micrometers());

        public LengthVM WidthTarget
        {
            get => _widthTarget; set { if (_widthTarget != value) { _widthTarget = value; OnPropertyChanged(); } }
        }

        private LengthVM _widthOffset = new LengthVM(0.Micrometers());

        public LengthVM WidthOffset
        {
            get => _widthOffset; set { if (_widthOffset != value) { _widthOffset = value; OnPropertyChanged(); } }
        }

        private LengthTolerance _widthTolerance = new LengthTolerance(1, LengthToleranceUnit.Micrometer);

        public LengthTolerance WidthTolerance
        {
            get => _widthTolerance; set { if (_widthTolerance != value) { _widthTolerance = value; OnPropertyChanged(); } }
        }

        public TSVMeasureVM(TestMeasureVM testMeasureVM)
        {
            _testMeasureVM = testMeasureVM;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        private AutoRelayCommand _startTSVCommand;

        public AutoRelayCommand StartTSVCommand
        {
            get
            {
                return _startTSVCommand ?? (_startTSVCommand = new AutoRelayCommand(
                () =>
                {
                    var tsvSettings = new TSVSettings()
                    {
                        Name = "mon tsv",
                        IsActive = true,
                        MeasurePoints = new List<int> { 0, 1 },
                        NbOfRepeat = 1,
                        Strategy = Service.Interface.Algo.TSVAcquisitionStrategy.Standard,
                        Precision = Service.Interface.Algo.TSVMeasurePrecision.Fast,
                        DepthTarget = DepthTarget.Length,
                        DepthTolerance = DepthTolerance,
                        LengthTarget = LengthTarget.Length,
                        LengthTolerance = LengthTolerance,
                        WidthTarget = WidthTarget.Length,
                        WidthTolerance = WidthTolerance,
                        CameraId = ServiceLocator.CamerasSupervisor.Camera.Configuration.DeviceID,
                        // TODO should use real probe settings
                        Probe = new ProbeSettings(),
                        EllipseDetectionTolerance = 1.Micrometers(),
                        Shape = UnitySC.Shared.Format.Metro.TSV.TSVShape.Elipse, //note de rti : ou trouver la nature TSV shape ici ?
                        ROI = null //new CenteredRegionOfInterest { Width = null, Height = null, OffsetX = null, OffsetY = null }
                    };

                    _testMeasureVM.StartMeasure(tsvSettings);
                },
                () => { return true; }));
            }
        }
    }
}
