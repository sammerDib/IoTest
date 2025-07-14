using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Laser.LaserQuantum;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Laser
{
    public class SMD12LaserDummyController : LaserController
    {
        public SMD12LaserDummyController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
            ILogger logger) : base(opcControllerConfig, globalStatusServer, logger)
        {
        }

        public override void Init(List<Message> initErrors)
        {
            Logger.Information("Init SMD12LaserController as dummy");
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

        public override void ReadPower()
        {
            throw new NotImplementedException();
        }

        public override void SetPower(double power)
        {
        }

        public void SetCurrent(double current)
        {
        }

        public override void TriggerUpdateEvent()
        {
        }

        public override void CustomCommand(string custom)
        {
        }
    }
}
