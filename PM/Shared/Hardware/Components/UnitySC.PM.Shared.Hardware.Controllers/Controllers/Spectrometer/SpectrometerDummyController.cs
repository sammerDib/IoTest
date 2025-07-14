using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer;
using UnitySC.PM.Shared.Hardware.Spectrometer;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Spectrometer
{
    public class SpectrometerDummyController : ControllerBase
    {
        private readonly SpectrometerAvantesControllerConfig _config;

        public short NbScan { get; set; } = 1;
        public bool IsTriggered { get; set; } = false;

        public SpectrometerDummyController(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger) : base(controllerConfig, globalStatusServer, logger)
        {
            _config = (SpectrometerAvantesControllerConfig)controllerConfig;
        }

        public override void Connect()
        {
            switch (_config.CommunicationMode)
            {
                case CommunicationMode.Ethernet:
                    //devicePort = 256;
                    break;

                case CommunicationMode.USB:
                    //devicePort = 0;
                    break;
            }
        }

        public override void Connect(string deviceId)
        {
            Connect();
        }

        public override void Disconnect()
        {
        }

        public override void Disconnect(string deviceID)
        {
        }

        public override void Init(List<Message> initErrors)
        {
            if (!(_config is SpectrometerAvantesControllerConfig))
                throw new Exception("Invalid spectrometer controller configuration");

            Name = _config.Name;
            DeviceID = _config.DeviceID;
            Logger.Information("Init SpectrometerController as dummy");
            Logger.Information("Init the device " + _config.DeviceID);
            Connect();
        }

        public override bool ResetController()
        {
            return true;
        }

        public SpectroSignal DoMeasure(SpectrometerParamBase param, bool isSilent)
        {
            int nbPixels = 4096;
            var dummySignal = new SpectroSignal();
            dummySignal.Wave = new List<double>(nbPixels);
            dummySignal.RawValues = new List<double>(nbPixels);
            var rand = new Random();
            for (int i = 0; i < nbPixels; i++)
            {
                dummySignal.Wave.Add(i/8 + 20.0);
                dummySignal.RawValues.Add(50.0*Math.Sin(2*Math.PI/800.0) + (10.0 * rand.NextDouble() - 5.0));
            }
            return dummySignal;
         }
    }
}
