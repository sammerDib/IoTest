using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Ffus
{
    public class DummyFfuController : FfuController
    {
        private readonly ILogger _logger;

        public DummyFfuController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger) : 
            base(opcControllerConfig, globalStatusServer, logger)
        {
            _logger = logger;
        }

        public override void Init(List<Message> initErrors)
        {
            Logger.Information("Init FfuController as dummy");
        }

        public override bool ResetController()
        {
            throw new NotImplementedException();
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

        public override void PowerOn()
        {
        }

        public override void PowerOff()
        {
        }

        public override void SetSpeed(ushort speedPercent)
        {
        }

        public override void CustomCommand(string custom)
        {
        }

        public override void TriggerUpdateEvent()
        {
        }

        public Dictionary<string, ushort> GetDefaultFfuValues()
        {
            return new Dictionary<string, ushort>();
        }
    }
}
