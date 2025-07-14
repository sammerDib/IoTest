using System.ServiceModel;

using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.ProbeLiseHF;
using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Service.Interface
{
    [ServiceKnownType(typeof(ProbeLiseSignal))]
    [ServiceKnownType(typeof(ProbeLiseHFSignal))]
    [ServiceKnownType(typeof(ProbeDualLiseCalibResult))]
    [ServiceKnownType(typeof(ProbeLiseHFCalibResult))]

    [ServiceContract]
    public interface IProbeServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void ProbeStateUpdatedCallback(DeviceState probeState, string deviceID);

        [OperationContract(IsOneWay = true)]
        void ProbeRawMeasuresCallback(ProbeSignalBase probeRawSignal);

        [OperationContract(IsOneWay = true)]
        void ProbeCalibrationResultsCallback(ProbeCalibResultsBase probeCalibrationResults);

        [OperationContract(IsOneWay = true)]
        void ProbeNewObjectiveInUseCallback(ObjectiveResult currentObject);
    }
}
