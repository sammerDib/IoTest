using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Screens
{
    public class DummyScreenController : DensitronDM430GNScreenController
    {
        private readonly ILogger _logger;

        public DummyScreenController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger) :
            base(opcControllerConfig, globalStatusServer, logger)
        {
            _logger = logger;
            BacklightValue = 100;
            BrightnessValue = 50;
            ContrastValue = 50;
        }

        public override void Init(List<Message> initErrors)
        {
            Logger.Information("Init ScreenController as dummy");
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

        public override async Task PowerOnAsync()
        {
            //TODO??
        }

        public override async Task PowerOffAsync()
        {
        }

        public override void TriggerUpdateEvent()
        {
        }
    }
}
