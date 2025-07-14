using System;

using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Spectrometer
{
    public abstract class SpectrometerBase : DeviceBase
    {
        protected SpectrometerBase(Status.Service.Interface.IGlobalStatusServer globalStatusServer, ILogger logger) : base(globalStatusServer, logger)
        {
        }

        public SpectrometerConfig Configuration { get; set; }

        public virtual void Init(SpectrometerConfig config)
        {
            if (!(config is SpectrometerConfig))
                throw new Exception("Invalid spectrometer configuration");

            Name = config.Name;
            DeviceID = config.DeviceID;
            Configuration = config;

            Logger.Information("Init the device " + config.DeviceID);
        }

        public abstract SpectroSignal DoMeasure(SpectrometerParamBase param, bool isSilent = false);

        public abstract SpectroSignal GetLastMeasure();

        public abstract void StartContinuousAcquisition(SpectrometerParamBase param);

        public abstract void StopContinuousAcquisition(); 

    }
}
