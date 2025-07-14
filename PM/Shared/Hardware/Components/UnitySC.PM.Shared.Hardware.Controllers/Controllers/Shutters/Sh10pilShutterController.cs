using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Shutter;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Shutters
{
    public class Sh10pilShutterController : ShutterController
    {

        private OpcController _opcController;

        private enum EShutterCmds
        { ManualOpen, ManualClose, RaisePropertiesChanged }

        private enum EFeedbackMsgShutter
        {
            State = 1,
            ShutterIrisPositionMsg = 10
        }

        public Sh10pilShutterController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
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

        public override void OpenIris()
        {
            _opcController.SetMethodValueAsync(EShutterCmds.ManualOpen.ToString());
        }

        public override void CloseIris()
        {
            _opcController.SetMethodValueAsync(EShutterCmds.ManualClose.ToString());
        }

        public override void TriggerUpdateEvent()
        {
            _opcController.SetMethodValueAsync(EShutterCmds.RaisePropertiesChanged.ToString());
        }

        public void DeliverMessages(string msgName, object value)
        {
            try
            {
                EFeedbackMsgShutter index = 0;
                EFeedbackMsgShutter.TryParse(msgName, out index);

                switch (index)
                {
                    case EFeedbackMsgShutter.State:
                        State = new DeviceState((DeviceStatus)(int)value);
                        break;

                    case EFeedbackMsgShutter.ShutterIrisPositionMsg:
                        string sMsg = (string)value;
                        if (!String.IsNullOrWhiteSpace(sMsg))
                            Messenger.Send(new ShutterIrisPositionMessage() { ShutterIrisPosition = sMsg });
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
    }
}
