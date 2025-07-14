using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Service.Interface
{
    public interface IProbeServiceCallbackProxy
    {
        void ProbeRawMeasuresCallback(ProbeSignalBase probeSignal);

        void ProbeStateUpdatedCallback(DeviceState newProbeState, string deviceID);

        void ProbeCalibrationResultsCallback(ProbeCalibResultsBase probeCalibrationResults);

        void ProbeNewObjectiveInUseCallback(ObjectiveResult currentObjective);
    }
}
