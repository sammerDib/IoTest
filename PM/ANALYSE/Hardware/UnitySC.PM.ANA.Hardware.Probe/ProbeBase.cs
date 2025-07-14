using System;
using System.Collections.Generic;
using System.Threading;


using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.ANA.Hardware.Probe
{
    public abstract class ProbeBase : IProbe
    {
        public ProbeBase(IProbeConfig config,ILogger logger)
        {
            Configuration = config;
            Family = DeviceFamily.Probe;
            Name = config.Name;
            DeviceID = config.DeviceID;
            State = new DeviceState(DeviceStatus.Unknown);
            Logger = logger;

        }

        public ILogger Logger { get; }

        public DeviceFamily Family { get; }
        public string Name { get; set; }
        public virtual string DeviceID { get; set; }
        public DeviceState State { get; set; }
        public virtual IProbeSignal LastRawSignal { get; set; }
        public virtual IProbeConfig Configuration { get; set; }

        public IProbeCalibrationManager CalibrationManager { get; set; }



#pragma warning disable CS0067 // Event not used. Not detected by the compiler

        public event StateChangedEventHandler OnStatusChanged;

        public abstract void Init();

        public abstract void Shutdown();

        public abstract IProbeSignal DoSingleAcquisition(IProbeInputParams inputParameters);

        public abstract IEnumerable<IProbeSignal> DoMultipleAcquisitions(IProbeInputParams inputParameters);

        public abstract IProbeResult DoMeasure(IProbeInputParams inputParameters);

        public abstract void StartContinuousAcquisition(IProbeInputParams inputParameters);

        public abstract void StopContinuousAcquisition();

    }
}
