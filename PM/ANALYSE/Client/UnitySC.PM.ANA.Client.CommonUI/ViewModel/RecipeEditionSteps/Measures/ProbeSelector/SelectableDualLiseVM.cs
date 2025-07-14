using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.ProbeSelector
{
    public class SelectableDualLiseVM : SelectableProbeVM
    {

        public SelectableDualLiseVM()
        {
            // The default gain is the current gain
            switch (ServiceLocator.ProbesSupervisor.CurrentProbeLise)
            {
                case ProbeLiseVM currentProbeLiseVM:
                    DualLiseUpGain = currentProbeLiseVM.InputParametersLise.Gain;
                    DualLiseDownGain = currentProbeLiseVM.InputParametersLise.Gain;
                    break;

                case ProbeLiseDoubleVM currentProbeLiseDoubleVM:
                    DualLiseUpGain = currentProbeLiseDoubleVM.InputParametersLiseDouble.ProbeUpParams.Gain;
                    DualLiseDownGain = currentProbeLiseDoubleVM.InputParametersLiseDouble.ProbeDownParams.Gain;
                    break;

                default:
                    break;
            }
        }
        private DualProbeWithObjectivesMaterial _dualLiseMaterial => ProbeMaterial as DualProbeWithObjectivesMaterial;

        protected override void UpdateMaterial()
        {
            foreach (var compatibleUpObjectiveId in _dualLiseMaterial.UpProbe.CompatibleObjectives)
            {
                var objectiveConfig = ServiceLocator.ProbesSupervisor.GetOjectiveConfig(compatibleUpObjectiveId);

                if (!(objectiveConfig is null))
                    ObjectivesDualUp.Add(objectiveConfig);
            }

            foreach (var compatibleDownObjectiveId in _dualLiseMaterial.DownProbe.CompatibleObjectives)
            {
                var objectiveConfig = ServiceLocator.ProbesSupervisor.GetOjectiveConfig(compatibleDownObjectiveId);

                if (!(objectiveConfig is null))
                    ObjectivesDualDown.Add(objectiveConfig);
            }

            if (SelectedObjectiveDualUp is null)
                SelectedObjectiveDualUp = ObjectivesDualUp.FirstOrDefault();
            if (SelectedObjectiveDualDown is null)
                SelectedObjectiveDualDown = ObjectivesDualDown.FirstOrDefault();
        }

        public override void SetProbeSettings(ProbeSettings probeSettings)
        {
            if (probeSettings is DualLiseSettings dualLiseSettings)
            {
                DualLiseUpGain = dualLiseSettings.LiseUp.LiseGain;
                DualLiseDownGain = dualLiseSettings.LiseDown.LiseGain;
                SelectedObjectiveDualUp = ObjectivesDualUp.FirstOrDefault(o => o.DeviceID == dualLiseSettings.LiseUp.ProbeObjectiveContext.ObjectiveId);
                SelectedObjectiveDualDown = ObjectivesDualDown.FirstOrDefault(o => o.DeviceID == dualLiseSettings.LiseDown.ProbeObjectiveContext.ObjectiveId);
            }
        }

        public override ProbeSettings GetProbeSettings()
        {
            var newProbeSettings = new DualLiseSettings();
            newProbeSettings.ProbeId = ProbeMaterial.ProbeId;
            newProbeSettings.LiseUp = new SingleLiseSettings();
            newProbeSettings.LiseUp.ProbeId = _dualLiseMaterial.UpProbe.ProbeId;
            newProbeSettings.LiseUp.LiseGain = DualLiseUpGain;
            newProbeSettings.LiseUp.ProbeObjectiveContext = new ObjectiveContext
            {
                ObjectiveId = SelectedObjectiveDualUp?.DeviceID,
            };

            newProbeSettings.LiseDown = new SingleLiseSettings();
            newProbeSettings.LiseDown.ProbeId = _dualLiseMaterial.DownProbe.ProbeId;
            newProbeSettings.LiseDown.LiseGain = DualLiseDownGain;
            newProbeSettings.LiseDown.ProbeObjectiveContext = new ObjectiveContext
            {
                ObjectiveId = SelectedObjectiveDualDown?.DeviceID,
            };
            return newProbeSettings;
        }

        public override void SetAsCurrentProbe()
        {
            (ServiceLocator.ProbesSupervisor.CurrentProbeLise as ProbeLiseDoubleVM).InputParametersLiseDouble.ProbeUpParams.Gain = DualLiseUpGain;
            (ServiceLocator.ProbesSupervisor.CurrentProbeLise as ProbeLiseDoubleVM).InputParametersLiseDouble.ProbeDownParams.Gain = DualLiseDownGain;
        }

        public override void UnsetAsCurrentProbe()
        {
            DualLiseUpGain = (ServiceLocator.ProbesSupervisor.CurrentProbeLise as ProbeLiseDoubleVM).InputParametersLiseDouble.ProbeUpParams.Gain;
            DualLiseDownGain = (ServiceLocator.ProbesSupervisor.CurrentProbeLise as ProbeLiseDoubleVM).InputParametersLiseDouble.ProbeDownParams.Gain;
        }

        public override List<ObjectiveConfig> GetTopObjectives()
        {
            return ObjectivesDualUp.ToList();
        }

        private double _dualLiseUpGain = 1;

        public double DualLiseUpGain
        {
            get => _dualLiseUpGain; set { if (_dualLiseUpGain != value) { _dualLiseUpGain = value; OnPropertyChanged(); } }
        }

        private double _dualLiseDownGain = 1;

        public double DualLiseDownGain
        {
            get => _dualLiseDownGain; set { if (_dualLiseDownGain != value) { _dualLiseDownGain = value; OnPropertyChanged(); } }
        }

        private ObservableCollection<ObjectiveConfig> _objectivesDualUp;

        public ObservableCollection<ObjectiveConfig> ObjectivesDualUp
        {
            get
            {
                if (_objectivesDualUp is null)
                    _objectivesDualUp = new ObservableCollection<ObjectiveConfig>();
                return _objectivesDualUp;
            }
            set
            {
                if (_objectivesDualUp == value)
                {
                    return;
                }
                _objectivesDualUp = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ObjectiveConfig> _objectivesDualDown;

        public ObservableCollection<ObjectiveConfig> ObjectivesDualDown
        {
            get
            {
                if (_objectivesDualDown is null)
                    _objectivesDualDown = new ObservableCollection<ObjectiveConfig>();
                return _objectivesDualDown;
            }
            set
            {
                if (_objectivesDualDown == value)
                {
                    return;
                }
                _objectivesDualDown = value;
                OnPropertyChanged();
            }
        }

        private ObjectiveConfig _selectedObjectiveDualUp;

        public ObjectiveConfig SelectedObjectiveDualUp
        {
            get => _selectedObjectiveDualUp;
            set
            {
                if (_selectedObjectiveDualUp != value)
                {
                    _selectedObjectiveDualUp = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObjectiveConfig _selectedObjectiveDualDown;

        public ObjectiveConfig SelectedObjectiveDualDown
        {
            get => _selectedObjectiveDualDown;
            set
            {
                if (_selectedObjectiveDualDown != value)
                {
                    _selectedObjectiveDualDown = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
