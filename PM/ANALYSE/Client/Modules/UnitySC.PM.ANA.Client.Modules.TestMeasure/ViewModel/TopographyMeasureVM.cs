using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.ANA.Client.Modules.TestMeasure.ViewModel
{
    public class TopographyMeasureVM : ObservableObject, IWizardNavigationItem, IDisposable
    {
        public string Name { get; set; } = "Topography";
        public bool IsEnabled { get; set; } = true;
        public bool IsMeasure { get; set; } = false;
        public bool IsValidated { get; set; } = false;
        private readonly string _lightId;
        private readonly TestMeasureVM _testMeasureVM;
        private readonly Length _heightVariationEnlarging;

        public IEnumerable<string> Algos { get; private set; }

        public TopographyMeasureVM(TestMeasureVM testMeasureVM)
        {
            var topoMeasureConfig = (MeasureTopoConfiguration)(ServiceLocator.MeasureSupervisor.GetMeasureConfiguration(MeasureType.Topography)?.Result);
            _heightVariationEnlarging = topoMeasureConfig?.VSIMarginConstant;
            _stepSize = new LengthVM(topoMeasureConfig?.VSIStepSize);

            _testMeasureVM = testMeasureVM;
            _lightId = ServiceLocator.MeasureSupervisor.GetMeasureLightIds(MeasureType.Topography)?.Result.FirstOrDefault();
            ComputeStartPosition();
            // We should handle property unsubscribe, event in testMeasure/testAlgo/TestHardware VMs
            //ServiceLocator.AxesSupervisor.AxesVM.Status.PropertyChanged += AxesVM_PropertyChanged;
        }

        private void AxesVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Update position when a move is end
            if (e.PropertyName == "IsMoving" && (sender as Proxy.Axes.Models.Status).IsMoving == false)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => ComputeStartPosition()));
            }
        }

        private LengthVM _heightVariation = new LengthVM(10.Micrometers());

        public LengthVM HeightVariation
        {
            get => _heightVariation;
            set { if (_heightVariation != value) { _heightVariation = value; OnPropertyChanged(); } }
        }

        public double HeightVariationValue
        {
            get => HeightVariation.Value;
            set
            {
                if (HeightVariation.Value != value)
                {
                    HeightVariation.Value = value;
                    ComputeStartPosition();
                    OnPropertyChanged();
                }
            }
        }

        private LengthTolerance _heightTolerance = new LengthTolerance(1, LengthToleranceUnit.Micrometer);

        public LengthTolerance HeightTolerance
        {
            get => _heightTolerance;
            set { if (_heightTolerance != value) { _heightTolerance = value; OnPropertyChanged(); } }
        }

        public double HeightToleranceValue
        {
            get => HeightTolerance.Value;
            set
            {
                if (HeightTolerance.Value != value)
                {
                    HeightTolerance.Value = value;
                    ComputeStartPosition();
                    OnPropertyChanged();
                }
            }
        }

        private LengthVM _stepSize;

        public LengthVM StepSize
        {
            get => _stepSize;
            set { if (_stepSize != value) { _stepSize = value; OnPropertyChanged(); } }
        }

        public double StepSizeValue
        {
            get => StepSize.Value;
            set
            {
                if (StepSize.Value != value)
                {
                    StepSize.Value = value;
                    ComputeStartPosition();
                    OnPropertyChanged();
                }
            }
        }

        private SurfacesInFocus _surfaceInFocus = SurfacesInFocus.Unknown;

        public SurfacesInFocus SurfaceInFocus
        {
            get
            {
                return _surfaceInFocus;
            }
            set
            {
                if (_surfaceInFocus == value)
                {
                    return;
                }

                _surfaceInFocus = value;
                ComputeStartPosition();
                OnPropertyChanged(nameof(SurfaceInFocus));
            }
        }

        private LengthVM _piezoStartPosition;

        public LengthVM PiezoStartPosition
        {
            get => _piezoStartPosition;
            set { if (_piezoStartPosition != value) { _piezoStartPosition = value; OnPropertyChanged(); } }
        }

        private LengthVM _piezoEndPosition;

        public LengthVM PiezoEndPosition
        {
            get => _piezoEndPosition;
            set { if (_piezoEndPosition != value) { _piezoEndPosition = value; OnPropertyChanged(); } }
        }

        private int _stepCount;

        public int StepCount
        {
            get => _stepCount;
            set { if (_stepCount != value) { _stepCount = value; OnPropertyChanged(); } }
        }

        public void Dispose()
        {
            ServiceLocator.AxesSupervisor.AxesVM.PropertyChanged -= AxesVM_PropertyChanged;
        }

        private AutoRelayCommand _startVSICommand;

        public AutoRelayCommand StartVSICommand
        {
            get
            {
                return _startVSICommand ?? (_startVSICommand = new AutoRelayCommand(
              () =>
              {
                  ComputeStartPosition();
                  var context = ServiceLocator.ContextSupervisor.GetTopImageAcquisitionContext()?.Result;
                  var topographySettings = new TopographySettings()
                  {
                      Name = "Topo Test",
                      IsActive = true,
                      MeasurePoints = new List<int> { 0, 1 },
                      NbOfRepeat = 1,
                      CameraId = ServiceLocator.CamerasSupervisor.Camera.Configuration.DeviceID,
                      ObjectiveId = ServiceLocator.CamerasSupervisor.Objective.DeviceID,
                      LightId = _lightId,
                      SurfacesInFocus = SurfaceInFocus,
                      HeightVariation = HeightVariation.Length,
                      ScanMargin = HeightTolerance.GetAbsoluteTolerance(HeightVariation.Length),
                      MeasureContext = context
                  };
                  _testMeasureVM.StartMeasure(topographySettings);
              },
              () => { return true; }));
            }
        }

        private void ComputeStartPosition()
        {
            var currentZPos = ServiceLocator.AxesSupervisor.GetXYZTopZBottomPosition()?.ZTop;
            if (currentZPos != null)
            {
                var startPos = currentZPos?.Millimeters();
                var endPos = currentZPos?.Millimeters();

                if (_surfaceInFocus == SurfacesInFocus.Top)
                {
                    startPos -= HeightVariation.Length + HeightTolerance.GetAbsoluteTolerance(HeightVariation.Length) + _heightVariationEnlarging;
                    endPos += HeightTolerance.GetAbsoluteTolerance(HeightVariation.Length) + _heightVariationEnlarging;
                }
                else if (_surfaceInFocus == SurfacesInFocus.Bottom)
                {
                    startPos -= HeightTolerance.GetAbsoluteTolerance(HeightVariation.Length) + _heightVariationEnlarging;
                    endPos += HeightVariation.Length + HeightTolerance.GetAbsoluteTolerance(HeightVariation.Length) + _heightVariationEnlarging;
                }
                else if (_surfaceInFocus == SurfacesInFocus.Unknown)
                {
                    startPos -= HeightVariation.Length + HeightTolerance.GetAbsoluteTolerance(HeightVariation.Length) + _heightVariationEnlarging;
                    endPos += HeightVariation.Length + HeightTolerance.GetAbsoluteTolerance(HeightVariation.Length) + _heightVariationEnlarging;
                }

                PiezoStartPosition = new LengthVM(startPos);
                PiezoEndPosition = new LengthVM(endPos);
                StepCount = (int)(Math.Abs(endPos.Micrometers - startPos.Micrometers) / StepSize.Length.Micrometers);
            }
        }
    }
}
