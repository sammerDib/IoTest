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
    public class SelectableLiseVM : SelectableProbeVM
    {

        public SelectableLiseVM()
        {
            // The default gain is the current gain
            if (ServiceLocator.ProbesSupervisor.CurrentProbeLise is ProbeLiseVM currentProbeLiseVM)
                _liseGain = currentProbeLiseVM.InputParametersLise.Gain;
        }

        private ProbeWithObjectivesMaterial _liseMaterial => ProbeMaterial as ProbeWithObjectivesMaterial;

        protected override void UpdateMaterial()
        {
            foreach (var compatibleObjectiveId in _liseMaterial.CompatibleObjectives)
            {
                var objectiveConfig = ServiceLocator.ProbesSupervisor.GetOjectiveConfig(compatibleObjectiveId);

                if (!(objectiveConfig is null))
                    Objectives.Add(objectiveConfig);
            }

            if (SelectedObjective is null)
                SelectedObjective = Objectives.FirstOrDefault();
        }

        public override void SetProbeSettings(ProbeSettings probeSettings)
        {
            if (probeSettings is SingleLiseSettings singleLiseSettings)
            {
                LiseGain = singleLiseSettings.LiseGain;
                if (singleLiseSettings?.ProbeObjectiveContext != null)
                    SelectedObjective = Objectives.FirstOrDefault(o => o.DeviceID == singleLiseSettings.ProbeObjectiveContext.ObjectiveId);
            }
        }

        public override ProbeSettings GetProbeSettings()
        {
            var newProbeSettings = new SingleLiseSettings();
            newProbeSettings.ProbeId = ProbeMaterial.ProbeId;
            newProbeSettings.LiseGain = LiseGain;
            newProbeSettings.ProbeObjectiveContext = new ObjectiveContext
            {
                ObjectiveId = SelectedObjective?.DeviceID,
            };

            return newProbeSettings;
        }

        public override void SetAsCurrentProbe()
        {
            ServiceLocator.CamerasSupervisor.Objective = SelectedObjective;

            (ServiceLocator.ProbesSupervisor.CurrentProbeLise as ProbeLiseVM).InputParametersLise.Gain = LiseGain;
        }

        public override void UnsetAsCurrentProbe()
        {
            LiseGain = (ServiceLocator.ProbesSupervisor.CurrentProbeLise as ProbeLiseVM).InputParametersLise.Gain;
        }

        public override List<ObjectiveConfig> GetTopObjectives()
        {
            return Objectives.ToList();
        }

        private double _liseGain = 1;

        public double LiseGain
        {
            get => _liseGain; set { if (_liseGain != value) { _liseGain = value; OnPropertyChanged(); } }
        }

        private ObservableCollection<ObjectiveConfig> _objectives;

        public ObservableCollection<ObjectiveConfig> Objectives
        {
            get
            {
                if (_objectives is null)
                    _objectives = new ObservableCollection<ObjectiveConfig>();
                return _objectives;
            }
            set
            {
                if (_objectives == value)
                {
                    return;
                }
                _objectives = value;
                OnPropertyChanged();
            }
        }


    }
}
