using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.ANA.Client.Proxy.Probe
{
    public abstract class ProbeBaseVM : ViewModelBaseExt
    {
        protected IProbeService _probeSupervisor;


        protected bool IsContinuousAcquisitionInProgress = false;

        public ProbeBaseVM(IProbeService probeSupervisor, string probeId)
        {
            _probeSupervisor = probeSupervisor;
            DeviceID = probeId;
            State = new DeviceState(DeviceStatus.Unknown);
        }

        public IProbeConfig Configuration { get; set; }

        public string DeviceID { get; set; }

        public string Name
        {
            get { return Configuration?.Name; }
            set { if (Configuration != null) Configuration.Name = value; }
        }

        private DeviceState _state;

        public DeviceState State
        {
            get
            {
                return _state;
            }
            set
            {
                if (_state == value)
                {
                    return;
                }
                _state = value;
                OnPropertyChanged();
                UpdateAllCanExecutes();
            }
        }

#pragma warning disable CS0067 // Event not used. Not detected by the compiler

        public event StateChangedEventHandler OnStatusChanged;

        public ObjectiveResult ObjectiveInUse { get; set; }

        public bool DoSingleAcquisition(IProbeInputParams inputParameters)
        {
            var success = _probeSupervisor.SingleAcquisition(DeviceID, inputParameters)?.Result;
            return success ?? false;
        }

        public bool StartContinuousAcquisition(IProbeInputParams inputParameters)
        {
            var success = _probeSupervisor.StartContinuousAcquisition(DeviceID, inputParameters)?.Result;
            if (success != null && success == true)
            {
                State = new DeviceState(DeviceStatus.Busy);
                IsContinuousAcquisitionInProgress = true;
            }
            return success ?? false;
        }

        public bool StopContinuousAcquisition()
        {
            var success = _probeSupervisor.StopContinuousAcquisition(DeviceID)?.Result;
            if (success != null && success == true)
            {
                State = new DeviceState(DeviceStatus.Ready);
                IsContinuousAcquisitionInProgress = false;
            }
            return success ?? false;
        }

        public List<IProbeResult> DoMultipleMeasures(IProbeInputParams inputParameters, int nbMeasuresWanted)
        {
            return _probeSupervisor.DoMultipleMeasures(DeviceID, inputParameters, nbMeasuresWanted)?.Result;
        }

        public IProbeResult DoMeasure(IProbeInputParams inputParameters)
        {
            return _probeSupervisor.DoMeasure(DeviceID, inputParameters)?.Result;
        }

        public bool StartCalibration(IProbeCalibParams probeCalibParams, IProbeInputParams probeInputParameters)
        {
            State = new DeviceState(DeviceStatus.Busy);
            var result = _probeSupervisor.StartCalibration(DeviceID, probeCalibParams, probeInputParameters)?.Result;
            State = new DeviceState(DeviceStatus.Ready);
            return result ?? false;
        }

        public ObjectiveConfig ObjectiveSelectorGetPos(string objectiveSelectorID)
        {
            return _probeSupervisor.GetObjectiveInUse(objectiveSelectorID)?.Result;
        }

        public abstract void SetRawSignal(ProbeSignalBase rawSignal);
        public abstract void SetCalibrationResult(ProbeCalibResultsBase probeCalibrationResults);
    }
}
