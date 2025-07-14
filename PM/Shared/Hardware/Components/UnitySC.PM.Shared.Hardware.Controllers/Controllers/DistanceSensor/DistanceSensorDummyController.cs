using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.DistanceSensor
{
    public class DistanceSensorDummyController : DistanceSensorController
    {
        private readonly ILogger _logger;

        public DistanceSensorDummyController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
            ILogger logger) : base(opcControllerConfig, globalStatusServer, logger)
        {
            _logger = logger;
        }

        public override void Init(List<Message> initErrors)
        {
            Logger.Information("Init DistanceSensorController as dummy");
        }

        public override bool ResetController()
        {
            return true;
        }

        public override void Connect()
        {
        }

        public override void Connect(string deviceId)
        {
        }

        public override void Disconnect()
        {
        }

        public override void Disconnect(string deviceID)
        {
        }

        public override double GetDistanceSensorHeight()
        {
            return 8600;
        }

        public override void TriggerUpdateEvent()
        {
        }

        public override void CustomCommand(string custom)
        {
        }
    }
}
