using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Service.Interface
{
    public interface IProbe : IDevice
    {
        IProbeConfig Configuration { get; }

        IProbeSignal LastRawSignal { get; }

        IProbeCalibrationManager CalibrationManager { get; set; }

        void Init();

        void Shutdown();

        IProbeSignal DoSingleAcquisition(IProbeInputParams inputParameters);

        IEnumerable<IProbeSignal> DoMultipleAcquisitions(IProbeInputParams inputParameters);

        IProbeResult DoMeasure(IProbeInputParams inputParameters);

        void StartContinuousAcquisition(IProbeInputParams inputParameters);

        void StopContinuousAcquisition();
    }
}
