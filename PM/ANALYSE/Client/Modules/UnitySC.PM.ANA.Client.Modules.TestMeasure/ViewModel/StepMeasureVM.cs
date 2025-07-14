using System;
using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.ViewModel;
using UnitySC.PM.ANA.Service.Core.Step;

namespace UnitySC.PM.ANA.Client.Modules.TestMeasure.ViewModel
{
    public class StepMeasureVM : ObservableObject, IWizardNavigationItem, IDisposable
    {
        public string Name { get; set; } = "Step";
        public bool IsEnabled { get; set; } = true;
        public bool IsMeasure { get; set; } = false;
        public bool IsValidated { get; set; } = false;
        private ProbesSupervisor _probeSupervisor;
        private AxesSupervisor _axesSupervisor;

        private readonly TestMeasureVM _testMeasureVM;

        private XYPosition _point = new XYPosition(new MotorReferential());
        private LengthVM _scanSize = new LengthVM(1, LengthUnit.Millimeter);
        private double _scanOrientation;
        private double _speed = 0.1;
        private LengthVM _targetHeight = new LengthVM(10.Micrometers());
        private LengthTolerance _tolerance = new LengthTolerance(1, LengthToleranceUnit.Micrometer);
        private StepKind _stepKind = StepKind.Up;

        public XYPosition Point
        {
            get => _point;
            set => SetProperty(ref _point, value);
        }

        public LengthVM ScanSize
        {
            get => _scanSize;
            set => SetProperty(ref _scanSize, value);
        }

        public double ScanOrientation
        {
            get => _scanOrientation;
            set => SetProperty(ref _scanOrientation, value);
        }

        public double Speed
        {
            get => _speed;
            set => SetProperty(ref _speed, value);
        }

        public LengthVM TargetHeight
        {
            get => _targetHeight;
            set => SetProperty(ref _targetHeight, value);
        }

        public LengthTolerance Tolerance
        {
            get => _tolerance;
            set => SetProperty(ref _tolerance, value);
        }

        public StepKind StepKind
        {
            get => _stepKind;
            set => SetProperty(ref _stepKind, value);
        }

        public StepMeasureVM(TestMeasureVM testMeasureVM)
        {
            _testMeasureVM = testMeasureVM;
            _probeSupervisor = ClassLocator.Default.GetInstance<ProbesSupervisor>();
            _axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        private AutoRelayCommand _setPointCommand;

        private AutoRelayCommand _startCommand;

        public AutoRelayCommand StartCommand
        {
            get
            {
                return _startCommand ?? (_startCommand = new AutoRelayCommand(
                    () =>
                    {
                        var probe = _probeSupervisor.Probes.FirstOrDefault(p =>
                            (p is ProbeLiseVM) &&
                            (p as ProbeLiseVM).Configuration.ModulePosition == ModulePositions.Up);
                        string probeID = probe.DeviceID;
                        var settings = new StepSettings()
                        {
                            Name = "Step",
                            ProbeId = probeID,
                            // IsActive = true,
                            // MeasurePoints = new List<int> { 0, 1 },
                            // NbOfRepeat = 1,
                            Point = Point,
                            ScanSize = ScanSize.Length,
                            ScanOrientation = new Angle(ScanOrientation, AngleUnit.Degree),
                            Speed = new Speed(Speed),
                            TargetHeight = TargetHeight.Length,
                            ToleranceHeight = Tolerance,
                            StepKind = StepKind,
                        };
                        _testMeasureVM.StartMeasure(settings);
                    },
                    () => { return true; }));
            }
        }

        public AutoRelayCommand SetPointCommand
        {
            get
            {
                return _setPointCommand ?? (_setPointCommand = new AutoRelayCommand(() =>
                {
                    Point = new XYPosition(new MotorReferential(), _axesSupervisor.AxesVM.Position.X, _axesSupervisor.AxesVM.Position.Y);
                }));
            }
        }
    }
}
