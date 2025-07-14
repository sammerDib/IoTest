using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class AlignmentFocusStepVM : AlignmentStepBaseVM, IDisposable
    {
        private TaskCompletionSource<bool> _taskExecuteAuto;

        public AlignmentFocusStepVM(RecipeAlignmentVM recipeAlignmentVM) : base(recipeAlignmentVM)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            PropertyChanged += AlignmentFocusVM_PropertyChanged;
            ClassLocator.Default.GetInstance<AlgosSupervisor>().AFLiseChangedEvent += RecipeAlignmentVM_AFLiseChangedEvent;
        }

        private void AlignmentFocusVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsInAutomaticMode))
            {
                if (!IsInAutomaticMode)
                    AreParametersUsed = false;
            }

            if (e.PropertyName == nameof(StepState))
            {
                OnPropertyChanged(nameof(IsAcquiringProbeSignal));
            }
        }

        public ProbeLiseVM ProbeLise => (GetProbeForAutoFocus() is ProbeLiseVM probeLise) ? probeLise : null;
        public bool IsAcquiringProbeSignal => AreParametersUsed || StepState == StepStates.InProgress;

        private bool _areParametersUsed = false;

        public bool AreParametersUsed
        {
            get => _areParametersUsed;
            set
            {
                if (_areParametersUsed != value)
                {
                    _areParametersUsed = value;
                    RecipeAlignment.StopAutoAlignment.Execute(null);
                    ParametersChanged = true;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsAcquiringProbeSignal));
                }
            }
        }

        private bool _parametersChanged = false;

        private bool ParametersChanged
        {
            get => _parametersChanged;
            set
            {
                if (_parametersChanged != value)
                {
                    _parametersChanged = value;
                    if (_parametersChanged)
                        RecipeAlignment.IsModified = true;
                    UpdateAllCanExecutes();
                    OnPropertyChanged();
                }
            }
        }

        private double _gainParameter = 1.8;

        public double GainParameter
        {
            get => _gainParameter;
            set
            {
                if (_gainParameter != value)
                {
                    _gainParameter = value;
                    ParametersChanged = true;
                    OnPropertyChanged();
                }
            }
        }

        public double MaxGain => (GetProbeForAutoFocus() is ProbeLiseVM probelise) ? (probelise.Configuration as ProbeConfigurationLiseVM).MaximumGain : 100;

        public double MinGain => (GetProbeForAutoFocus() is ProbeLiseVM probelise) ? (probelise.Configuration as ProbeConfigurationLiseVM).MinimumGain : 100;

        public double MaxZTop => ServiceLocator.AxesSupervisor?.AxesVM?.ConfigurationAxisZTop?.PositionMax.Millimeters ?? 0;

        private double _zMinParameter = 10;

        public double ZMinParameter
        {
            get => _zMinParameter;
            set
            {
                if (_zMinParameter != value)
                {
                    ParametersChanged = true;
                    _zMinParameter = value;
                    if (_zMinParameter > _zMaxParameter)
                        ZMaxParameter = _zMinParameter;
                    OnPropertyChanged();
                }
            }
        }

        private double _zMaxParameter = 10;

        public double ZMaxParameter
        {
            get => _zMaxParameter;
            set
            {
                if (_zMaxParameter != value)
                {
                    ParametersChanged = true;
                    _zMaxParameter = value;
                    if (_zMaxParameter < _zMinParameter)
                        ZMinParameter = _zMaxParameter;
                    OnPropertyChanged();
                }
            }
        }

        private double _resultZPosition = double.NaN;

        public double ResultZPosition
        {
            get => _resultZPosition; set { if (_resultZPosition != value) { _resultZPosition = value; OnPropertyChanged(); } }
        }

        private string _objectiveUsed = "";

        public string ObjectiveIdUsed
        {
            get => _objectiveUsed; set { if (_objectiveUsed != value) { _objectiveUsed = value; OnPropertyChanged(); } }
        }

        private AutoRelayCommand _submitParameters;

        public AutoRelayCommand SubmitParameters
        {
            get
            {
                return _submitParameters ?? (_submitParameters = new AutoRelayCommand(
                    () =>
                    {
                        ParametersChanged = false;
                        // "_ =" to disable warning CS4014
                        _ = RecipeAlignment.ExecuteAutoAlignment();
                    },
                    () => { return ParametersChanged && (StepState != StepStates.InProgress); }
                ));
            }
        }

        public async Task<bool> ExecuteAutoAsync()
        {
            StepState = StepStates.InProgress;
            ResultZPosition = double.NaN;
            _taskExecuteAuto = new TaskCompletionSource<bool>();
            var probeToUse = GetProbeForAutoFocus();

            if (probeToUse is null)
            {
                StepState = StepStates.Error;
                // TODO display the error somewhere
                Logger.Error("Unable to get a Probe Lise UP");
            }

            AFLiseInput afLiseInput = new AFLiseInput(probeToUse.DeviceID);

            var cameraSupervisor = ClassLocator.Default.GetInstance<CamerasSupervisor>();
            var objectiveId = cameraSupervisor.MainObjective.DeviceID;
            afLiseInput.InitialContext = new ObjectiveContext(objectiveId);
            ObjectiveIdUsed = objectiveId;

            if (AreParametersUsed)
            {
                afLiseInput.ZPosScanRange = new ScanRange(ZMinParameter, ZMaxParameter);
                afLiseInput.Gain = GainParameter;
            }

            ClassLocator.Default.GetInstance<AlgosSupervisor>().StartAFLise(afLiseInput);

            ParametersChanged = false;
            return await _taskExecuteAuto.Task;
        }

        private static ProbeBaseVM GetProbeForAutoFocus()
        {
            return ServiceLocator.ProbesSupervisor.ProbeLiseUp;
        }

        private void RecipeAlignmentVM_AFLiseChangedEvent(AFLiseResult afLiseResult)
        {
            if (_taskExecuteAuto is null)
                return;

            if (_taskExecuteAuto.Task.Status == TaskStatus.RanToCompletion)
                return;
            StepState = GetStepStateFromFlowState(afLiseResult.Status.State);

            ErrorMessage = (afLiseResult.Status.State == FlowState.Error) ? afLiseResult.Status.Message : string.Empty;

            if (afLiseResult.Status.State == FlowState.Success)
            {
                Score = (int)(afLiseResult.QualityScore * 100);
                ResultZPosition = afLiseResult.ZPosition;
                _taskExecuteAuto.TrySetResult(true);
                return;
            }
            if (afLiseResult.Status.State == FlowState.Error)
                _taskExecuteAuto.TrySetResult(false);
        }

        internal void StopExecutionAuto()
        {
            if ((IsInAutomaticMode) && (StepState == StepStates.InProgress))
            {
                Task.Run(() => ClassLocator.Default.GetInstance<AlgosSupervisor>().CancelAFLise());

                _taskExecuteAuto.TrySetResult(false);
                StepState = StepStates.Error;
                ErrorMessage = "The Auto-Focus has been canceled";
            }
        }

        public void Dispose()
        {
            PropertyChanged -= AlignmentFocusVM_PropertyChanged;
            ClassLocator.Default.GetInstance<AlgosSupervisor>().AFLiseChangedEvent -= RecipeAlignmentVM_AFLiseChangedEvent;
        }

        protected override Task SubmitManualSettings()
        {
            ResultZPosition = ServiceLocator.AxesSupervisor.AxesVM.Position.ZTop;
            StepState = StepStates.Done;
            return Task.CompletedTask;
        }
    }
}
