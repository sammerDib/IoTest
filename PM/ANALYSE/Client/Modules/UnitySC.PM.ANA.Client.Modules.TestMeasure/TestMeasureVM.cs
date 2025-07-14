using System;
using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Client.Modules.TestMeasure.ViewModel;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Modules.TestMeasure
{
    public class TestMeasureVM : ObservableObject, IMenuContentViewModel
    {
        public bool IsEnabled => true;

        public List<IWizardNavigationItem> Measures { get; private set; }
        private IWizardNavigationItem _selectedMeasure;
        private IDialogOwnerService _dialogService;
        private IMessenger _messenger;
        private ProbesSupervisor _probeSupervisor;
        private CamerasSupervisor _camerasSupervisor;

        public ProbeLiseVM ProbeLise => (GetCurrentProbe() is ProbeLiseVM probeLise) ? probeLise : null;

        private ProbeBaseVM GetCurrentProbe()
        {
            var probes = _probeSupervisor.Probes;
            var position = _camerasSupervisor.Camera.Configuration.ModulePosition;
            var probeLise = probes.FirstOrDefault(p =>
                (p is ProbeLiseVM vm) && vm.Configuration.ModulePosition == position);
            return probeLise;
        }

        public IWizardNavigationItem SelectedMeasure
        {
            get => _selectedMeasure;
            set => SetProperty(ref _selectedMeasure, value);
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public List<SpecificPositions> AvailablePositions
        {
            get => new List<SpecificPositions>
            {
                SpecificPositions.PositionChuckCenter,
                SpecificPositions.PositionHome,
                SpecificPositions.PositionManualLoad,
                SpecificPositions.PositionPark
            };
        }

        public SpecificPositions DefaultSpecificPosition
        {
            get => SpecificPositions.PositionChuckCenter;
        }

        public TestMeasureVM()
        {
            Measures = new List<IWizardNavigationItem>
            {
                new TSVMeasureVM(this),
                new NanoTopoMeasureVM(this),
                new TopographyMeasureVM(this),
                new ThicknessMeasureVM(this),
                new StepMeasureVM(this),
                new EdgeTrimMeasureVM(this),
                new TrenchMeasureVM(this),
            };
            SelectedMeasure = Measures.First();
            _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();
            _probeSupervisor = ClassLocator.Default.GetInstance<ProbesSupervisor>();
            _camerasSupervisor = ClassLocator.Default.GetInstance<CamerasSupervisor>();
        }

        public void StartMeasure(MeasureSettingsBase measureConfigurationBase)
        {
            try
            {
                IsBusy = true;

                // AxesSupervisor.GetCurrentPosition return the position in motor referential with application of StageToMotor converter corrections.
                // In test measure module, we don't want this correction because we are working in stage referential only.
                // So we apply correction the other way (MotorToStage).
                var anaPosition = (AnaPosition)ServiceLocator.AxesSupervisor.GetCurrentPosition()?.Result;
                var anaPositionStageReferential = ServiceLocator.ReferentialSupervisor
                    .ConvertTo(anaPosition, ReferentialTag.Stage)?.Result;
                var measurePoint = new Service.Interface.Recipe.Measure.MeasurePoint(0,
                    anaPositionStageReferential.ToXYZTopZBottomPosition(), false);

                ServiceLocator.MeasureSupervisor.StartMeasure(measureConfigurationBase, measurePoint, null);
            }
            catch (Exception ex)
            {
                IsBusy = false;
                _dialogService.ShowException(ex, "Start measure error");
            }
        }

        public bool CanClose()
        {
            ServiceLocator.CamerasSupervisor.StopAllStreaming();
            ServiceLocator.MeasureSupervisor.MeasureProgressChangedEvent -=
                MeasureSupervisor_MeasureProgressChangedEvent;
            ServiceLocator.MeasureSupervisor.MeasureResultChangedEvent -= MeasureSupervisor_MeasureResultChangedEvent;

            return true;
        }

        public void Refresh()
        {
            ServiceLocator.MeasureSupervisor.MeasureProgressChangedEvent +=
                MeasureSupervisor_MeasureProgressChangedEvent;
            ServiceLocator.MeasureSupervisor.MeasureResultChangedEvent += MeasureSupervisor_MeasureResultChangedEvent;

            _camerasSupervisor.ApplyObjectiveOffset = true;
            _camerasSupervisor.ObjectiveChangedEvent += CamerasSupervisor_ObjectiveChangedEvent;

            ProbeLise.IsAcquiring = true;
        }

        private void CamerasSupervisor_ObjectiveChangedEvent(string objectiveID)
        {
            OnPropertyChanged(nameof(ProbeLise));
        }

        private void MeasureSupervisor_MeasureResultChangedEvent(MeasurePointResult result, string resultFolderPath,
            DieIndex dieIndex)
        {
            IsBusy = false;
            var messageLevel = (result.State == MeasureState.Error || result.State == MeasureState.NotMeasured)
                ? MessageLevel.Error
                : MessageLevel.Information;
            _messenger.Send(new Message(messageLevel, $"[MeasurePoint] {result}"));
        }

        private void MeasureSupervisor_MeasureProgressChangedEvent(MeasurePointProgress progress)
        {
            _messenger.Send(new Message(MessageLevel.Information, $"[MeasureProgress] {progress.Message}"));
        }
    }
}
