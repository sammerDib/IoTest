using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Client.Proxy.Probe
{
    /// <summary>
    /// Local service to supervise probe
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class ProbesSupervisor : ObservableObject, IProbeService, IProbeServiceCallback
    {
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private IMessenger _messenger;
        private DuplexServiceInvoker<IProbeService> _probeService;
        private IProbesFactory _probesFactory;

        public delegate void ObjectiveChangedHandler(ObjectiveResult newObjective);

        public event ObjectiveChangedHandler ObjectiveChangedEvent;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProbesSupervisor(ILogger<ProbesSupervisor> logger, IMessenger messenger)
        {
            _instanceContext = new InstanceContext(this);
            // Probe service
            _probeService = new DuplexServiceInvoker<IProbeService>(_instanceContext, "ANALYSEProbeService", ClassLocator.Default.GetInstance<SerilogLogger<IProbeService>>(), messenger, s => s.SubscribeToProbeChanges(), ClientConfiguration.GetServiceAddress(UnitySC.Shared.Data.Enum.ActorType.ANALYSE));
            _logger = logger;
            _messenger = messenger;
            _probesFactory = new ProbesVieModelFactory();

            // Subscribe to probe changes
            //_probeService.InvokeAndGetMessages(s => s.SubscribeToProbeChanges());

            CurrentProbeLise = ProbeLiseUp;
            CurrentProbe = ProbeLiseUp;
        }

        /// <summary>
        /// Subscribe to hardware changes
        /// </summary>
        public Response<VoidResult> SubscribeToProbeChanges()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _probeService.TryInvokeAndGetMessages(s => s.SubscribeToProbeChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Probe subscribe error");
            }

            return resp;
        }

        /// <summary>
        /// Unsubscribe to hardware changes
        /// </summary>
        public Response<VoidResult> UnsubscribeToProbeChanges()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _probeService.TryInvokeAndGetMessages(s => s.UnsubscribeToProbeChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Axes unsubscribe error");
            }

            return resp;
        }

        public void ProbeStateUpdatedCallback(DeviceState probeState, string probeId)
        {
            var probe = GetProbeFromID(probeId);
            if (probe == null)
                return;
            probe.State = probeState;
        }

        public void ProbeCalibrationResultsCallback(ProbeCalibResultsBase probeCalibrationResults)
        {
            if (probeCalibrationResults == null)
                return;

            var probe = GetProbeFromID(probeCalibrationResults.ProbeID);
            if (probe == null)
                return;
            probe.SetCalibrationResult(probeCalibrationResults);
        }

        // Virtual is needed for unit test, do not remove
        public virtual void ProbeNewObjectiveInUseCallback(ObjectiveResult currentObjective)
        {
            ObjectiveChangedEvent?.Invoke(currentObjective);

            if (currentObjective == null)
                return;

            foreach (var probe in Probes)
                probe.ObjectiveInUse = currentObjective;
        }

        // This function is virtual for the unit test. Do not remove it
        public virtual void ProbeRawMeasuresCallback(ProbeSignalBase probeRawSignal)
        {
            if (probeRawSignal == null)
                return;

            if (!(CurrentProbe is null))
            {
                CurrentProbe.SetRawSignal(probeRawSignal);
            }
        }

        private ProbeBaseVM GetProbeFromID(string probeID)
        {
            if (Probes == null)
            {
                return null;
            }
            return Probes.FirstOrDefault(p => p.Configuration.DeviceID == probeID);
        }

        public Response<List<IProbeConfig>> GetProbesConfig()
        {
            return _probeService.TryInvokeAndGetMessages(s => s.GetProbesConfig());
        }

        private List<ProbeBaseVM> _probes;

        public List<ProbeBaseVM> Probes

        {
            get
            {
                if (_probes == null)
                {

                    var probesConfig = GetProbesConfig()?.Result;
                    if (probesConfig != null)
                    {
                        _probes = new List<ProbeBaseVM>();
                        foreach (var config in probesConfig)
                        {
                            var newProbe = _probesFactory.Create(config, this);
                            _probes.Add(newProbe);
                        }
                    }
                }
                return _probes;
            }
            set
            {
                _probes = value;
            }
        }

        public ProbeLiseVM ProbeLiseUp => (ProbeLiseVM)Probes?.FirstOrDefault(p => (p is ProbeLiseVM) && (p as ProbeLiseVM).Configuration.ModulePosition == ModulePositions.Up);

        public ProbeLiseVM ProbeLiseDown => (ProbeLiseVM)Probes?.FirstOrDefault(p => (p is ProbeLiseVM) && (p as ProbeLiseVM).Configuration.ModulePosition == ModulePositions.Down);

        private ProbeLiseBaseVM _currentProbeLise;

        public ProbeLiseBaseVM CurrentProbeLise
        {
            get => _currentProbeLise; set { if (_currentProbeLise != value) { _currentProbeLise = value; OnPropertyChanged(); } }
        }

        private ProbeBaseVM _currentProbe;

        public ProbeBaseVM CurrentProbe
        {
            get => _currentProbe; set { if (_currentProbe != value) { _currentProbe = value; OnPropertyChanged(); } }
        }

        private bool _isEditingProbe = false;

        public bool IsEditingProbe
        {
            get => _isEditingProbe; set { if (_isEditingProbe != value) { _isEditingProbe = value; OnPropertyChanged(); } }
        }

        public void SetCurrentProbe(string probeID)
        {
            CurrentProbe = (ProbeBaseVM)Probes.FirstOrDefault(p => p.DeviceID == probeID);
            if (CurrentProbe is ProbeLiseBaseVM)
            {
                CurrentProbeLise = (ProbeLiseBaseVM)CurrentProbe;
            }
        }

        public Response<List<ObjectivesSelectorConfigBase>> GetObjectivesSelectorData()
        {
            return _probeService.TryInvokeAndGetMessages(s => s.GetObjectivesSelectorData());
        }

        private List<ObjectivesSelectorConfigBase> _objectivesSelectors;

        public List<ObjectivesSelectorConfigBase> ObjectivesSelectors
        {
            get
            {
                if (_objectivesSelectors == null)
                {
                    _objectivesSelectors = new List<ObjectivesSelectorConfigBase>();
                    var objectivesSelectors = GetObjectivesSelectorData()?.Result;
                    if (objectivesSelectors != null)
                        foreach (var objectivesSelector in objectivesSelectors)
                            _objectivesSelectors.Add(objectivesSelector);
                }
                return _objectivesSelectors;
            }
            set
            {
                _objectivesSelectors = value;
            }
        }

        public ObjectiveConfig GetOjectiveConfig(string objectiveId)
        {
            ObjectiveConfig objectiveConfig = null;

            var objectivesSelector = ObjectivesSelectors;
            foreach (var objectiveSelector in objectivesSelector)
            {
                objectiveConfig = objectiveSelector.FindObjective(objectiveId);
                if (!(objectiveConfig is null))
                    break;
            }

            return objectiveConfig;
        }

        public ProbeConfigBase GetProbeConfig(string probeId)
        {
            var probeConfig = ServiceLocator.ProbesSupervisor.GetProbesConfig()?.Result.FirstOrDefault(p => p.DeviceID == probeId);
            return (ProbeConfigBase)probeConfig;
        }

        public Response<List<IProbeResult>> DoMultipleMeasures(string probeID, IProbeInputParams inputParameters, int nbMeasuresWanted)
        {
            return _probeService.TryInvokeAndGetMessages(s => s.DoMultipleMeasures(probeID, inputParameters, nbMeasuresWanted));
        }

        public Response<IProbeResult> DoMeasure(string probeID, IProbeInputParams inputParameters)
        {
            return _probeService.TryInvokeAndGetMessages(s => s.DoMeasure(probeID, inputParameters));
        }

        public Response<bool> SingleAcquisition(string probeID, IProbeInputParams inputParameters)
        {
            return _probeService.TryInvokeAndGetMessages(s => s.SingleAcquisition(probeID, inputParameters));
        }

        public Response<bool> StartContinuousAcquisition(string probeID, IProbeInputParams inputParameters)
        {
            return _probeService.TryInvokeAndGetMessages(s => s.StartContinuousAcquisition(probeID, inputParameters));
        }

        public Response<bool> StopContinuousAcquisition(string probeID)
        {
            return _probeService.TryInvokeAndGetMessages(s => s.StopContinuousAcquisition(probeID));
        }

        public Response<bool> StartCalibration(string probeID, IProbeCalibParams probeCalibInputParameters, IProbeInputParams probeInputParameters)
        {
            return _probeService.TryInvokeAndGetMessages(s => s.StartCalibration(probeID, probeCalibInputParameters, probeInputParameters));
        }
        public Response<bool> IsCalibrated(string probeID, IProbeCalibParams probeCalibInputParameters, IProbeInputParams probeInputParameters)
        {
            return _probeService.TryInvokeAndGetMessages(s => s.IsCalibrated(probeID, probeCalibInputParameters, probeInputParameters));
        }

        public Response<VoidResult> CancelCalibration(string probeID)
        {
            return _probeService.TryInvokeAndGetMessages(s => s.CancelCalibration(probeID));
        }

        public bool SetObjectiveToUse(string newObjectiveToUseID, bool applyObjectiveOffset = true)
        {
            ObjectivesSelectorConfigBase associatedObjectivesSelector = null;
            // we search the objective selector that manages the Objective
            foreach (var objectivesSelector in ObjectivesSelectors)
            {
                if (objectivesSelector.Objectives.FirstOrDefault(o => o.DeviceID == newObjectiveToUseID) == null)
                    continue;
                associatedObjectivesSelector = objectivesSelector;
                break;
            }
            if (!(associatedObjectivesSelector is null))
                return SetNewObjectiveToUse(associatedObjectivesSelector.DeviceID, newObjectiveToUseID, applyObjectiveOffset)?.Result ?? false;

            return false;
        }

        public Response<bool> SetNewObjectiveToUse(string objectiveSelectorID, string newObjectiveToUseID, bool applyObjectiveOffset = true)
        {
            return _probeService.TryInvokeAndGetMessages(s => s.SetNewObjectiveToUse(objectiveSelectorID, newObjectiveToUseID, applyObjectiveOffset));
        }

        public Response<ObjectiveConfig> GetObjectiveInUse(string objectiveSelectorID)
        {
            return _probeService.TryInvokeAndGetMessages(s => s.GetObjectiveInUse(objectiveSelectorID));
        }

        public Response<bool> ResetObjectivesSelectors()
        {
            return _probeService.TryInvokeAndGetMessages(s => s.ResetObjectivesSelectors());
        }
    }
}
