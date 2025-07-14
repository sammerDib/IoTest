using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Laser.LaserQuantum;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Laser
{
    public class SMD12LaserController : LaserController
    {
        private readonly ILogger Logger;

        private OpcController _opcController;

        private enum SMD12Cmds
        { PowerOn, PowerOff, SetPower, SetCurrent, RaisePropertiesChanged, CustomCommand }

        private enum EFeedbackMsgSMD12
        {
            State = 1,
            PowerMsg = 10,
            InterlockStatusMsg,
            LaserTemperatureMsg,
            PsuTemperatureMsg,
            IdMsg,
            CustomMsg
        }

        public SMD12LaserController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
            ILogger logger) : base(opcControllerConfig, globalStatusServer, logger)
        {
            _opcController = new OpcController(opcControllerConfig, logger, new DeliverMessagesDelegate(DeliverMessages));
        }

        public override void Init(List<Message> initErrors)
        {
            _opcController.Init(initErrors);
        }

        public override bool ResetController()
        {
            try
            {
                if (_opcController != null)
                {
                    Disconnect();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override void Connect()
        {
            _opcController.Connect();
        }

        public override void Connect(string deviceId)
        {
            Connect();
        }

        public override void Disconnect()
        {
            _opcController.Disconnect();
        }

        public override void Disconnect(string deviceID)
        {
            Disconnect();
        }

        public void DeliverMessages(string msgName, object value)
        {
            try
            {
                EFeedbackMsgSMD12 index = 0;
                EFeedbackMsgSMD12.TryParse(msgName, out index);

                switch (index)
                {
                    case EFeedbackMsgSMD12.State:
                        State = new DeviceState((DeviceStatus)(int)value);
                        break;

                    case EFeedbackMsgSMD12.PowerMsg:
                        double power = Double.TryParse((string)value, out var tempPower) ? tempPower : -1.0;
                        if (power != -1.0)
                            Messenger.Send(new PowerMessage() { Power = power });
                        break;

                    case EFeedbackMsgSMD12.InterlockStatusMsg:
                        string sSMsg = (string)value;
                        if (!String.IsNullOrWhiteSpace(sSMsg))
                            Messenger.Send(new InterlockStatusMessage() { InterlockStatus = sSMsg });
                        break;

                    case EFeedbackMsgSMD12.LaserTemperatureMsg:
                        double laserTemperature = Double.TryParse((string)value, out var tempLaserTemperature) ? tempLaserTemperature : -1.0;
                        if (laserTemperature != -1.0)
                            Messenger.Send(new LaserTemperatureMessage() { LaserTemperature = laserTemperature });
                        break;

                    case EFeedbackMsgSMD12.PsuTemperatureMsg:
                        double psuTemperature = Double.TryParse((string)value, out var tempPsuTemperature) ? tempPsuTemperature : -1.0;
                        if (psuTemperature != -1.0)
                            Messenger.Send(new PsuTemperatureMessage() { PsuTemperature = psuTemperature });
                        break;

                    case EFeedbackMsgSMD12.IdMsg:
                        string sId = (string)value;
                        if (!String.IsNullOrWhiteSpace(sId))
                            Messenger.Send(new IdMessage() { Id = sId });
                        break;

                    case EFeedbackMsgSMD12.CustomMsg:
                        string sCMsg = (string)value;
                        if (!String.IsNullOrWhiteSpace(sCMsg))
                            Messenger.Send(new CustomMessage() { Custom = sCMsg });
                        break;

                    default:
                        Logger.Warning($"{ControllerConfig.DeviceID} - Unknown message: {msgName}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{ControllerConfig.DeviceID} - {ex.Message}: {(string)value}");
            }
        }

        public override void PowerOn()
        {
            _opcController.SetMethodValueAsync(SMD12Cmds.PowerOn.ToString());
        }

        public override void PowerOff()
        {
            _opcController.SetMethodValueAsync(SMD12Cmds.PowerOff.ToString());
        }

        public override void ReadPower()
        {
            throw new NotImplementedException();
        }

        public override void SetPower(double power)
        {
            _opcController.SetMethodValueAsync(SMD12Cmds.SetPower.ToString(), power);
        }

        public void SetCurrent(double current)
        {
            _opcController.SetMethodValueAsync(SMD12Cmds.SetCurrent.ToString(), current);
        }

        public override void TriggerUpdateEvent()
        {
            _opcController.SetMethodValueAsync(SMD12Cmds.RaisePropertiesChanged.ToString());
        }

        public override void CustomCommand(string custom)
        {
            _opcController.SetMethodValueAsync(SMD12Cmds.CustomCommand.ToString(), custom);
        }
    }
}
