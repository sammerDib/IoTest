using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Rfids
{
    public class DummyRfidController : RfidController
    {
        private readonly ILogger _logger;
        private OpcControllerConfig _config;

        public DummyRfidController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger) :
            base(opcControllerConfig, globalStatusServer, logger)
        {
            _logger = logger;
            _config = opcControllerConfig;
        }

        public override void Init(List<Message> initErrors)
        {
            Logger.Information("Init RfidController as dummy");
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

        public override void TriggerUpdateEvent()
        {
        }

        public void ReadTag()
        {
            _logger.Information("Read tag ok");
        }

        public string GetTag()
        {
            return "";
        }
    }
}
