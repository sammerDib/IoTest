using System;
using System.Collections.Generic;
using System.Threading;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Plc
{
    public class BeckhoffPlcDummyController : PlcController
    {
        private readonly ILogger _logger;

        private const int LangId = 1031;

        private enum CX5140Cmds
        { PlcRestart, PlcReboot, CustomCommand }

        private enum EFeedbackMsgPLC
        {
            SmokeDetectedMsg = 0
        }

        public BeckhoffPlcDummyController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
            ILogger logger) : base(opcControllerConfig, globalStatusServer, logger)
        {
            _logger = logger;

            ConnectEventLogger();
        }

        private void ConnectEventLogger()
        {
        }

        public override void Init(List<Message> initErrors)
        {
            Logger.Information("Init BeckhoffPlcController as dummy");
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

        public override void Restart()
        {
        }

        public override void Reboot()
        {
        }

        public override void CustomCommand(string custom)
        {
        }

        public override void StartTriggerOutEmitSignal(double pulseDuration_ms = 1)
        {
            _logger.Information("Plc dummy StartTriggerOutEmitSignal");
        }

        public override void SmokeDetectorResetAlarm()
        {
            _logger.Information("Plc dummy SmokeDetectorResetAlarm");
        }
    }
}
