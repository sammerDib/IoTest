using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.PlcScreen;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

using Workstation.ServiceModel.Ua;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Screens
{
    public class DensitronDM430GNScreenOpcMultiParams : IOpcMultiParams
    {
        public bool ScreenOn { get; set; }
        public short BacklightValue { get; set; }
        public short BrightnessValue { get; set; }
        public short ContrastValue { get; set; }
        public int SharpnessValue { get; set; }

        public DensitronDM430GNScreenOpcMultiParams(bool screenOn, short backlight, short brightness, short contrast, int sharpness)
        {
            ScreenOn = screenOn;
            BacklightValue = backlight;
            BrightnessValue = brightness;
            ContrastValue = contrast;
            SharpnessValue = sharpness;
        }

        public Variant[] ToVariantArray()
        {
            // the order ann value type is important for OPC InputArgument from OpcController.SetMethodValueAsync
            return new Variant[] { new Variant(true),
                new Variant(ScreenOn), new Variant(BacklightValue), new Variant(BrightnessValue), new Variant(ContrastValue), new Variant(SharpnessValue) };
        }
    }

    public class DensitronDM430GNScreenController : ScreenController
    {

        private OpcController _opcController;

        public short BacklightValue { get; set; }
        public short BrightnessValue { get; set; }
        public short ContrastValue { get; set; }
        public int SharpnessValue { get; set; }
        public double TemperatureValue { get; set; }
        public int FanRPMValue { get; private set; }

        private enum EScreenCmds
        {
            PowerOn, PowerOff, SetBacklight, SetBrightness, SetContrast, SetDefaultValue, SetFanStep,
            SetSharpness, FanAutoOn, CustomCommand, RaisePropertiesChanged, RestoreParameters
        }

        private enum EFeedbackMsgScreen
        {
            State = 0,
            StateMsg = 1,
            StatusMsg = 2,
            IsAliveMsg = 3,
            PowerStateMsg = 10,
            BacklightMsg,
            BrightnessMsg,
            ContrastMsg,
            FanAutoMsg,
            FanStepMsg,
            FanRpmMsg,
            TemperatureMsg,
            CustomMsg = 50
        }

        public DensitronDM430GNScreenController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
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

        public override async Task PowerOnAsync()
        {
            await _opcController.SetMethodValueAsync(EScreenCmds.PowerOn.ToString());
        }

        public override async Task PowerOffAsync()
        {
            await _opcController.SetMethodValueAsync(EScreenCmds.PowerOff.ToString());
        }

        public async Task SetBacklightAsync(short value_InPercent)
        {
            await _opcController.SetMethodValueAsync(EScreenCmds.SetBacklight.ToString(), value_InPercent);
        }

        public async Task SetBrightnessAsync(short value_InPercent)
        {
            await _opcController.SetMethodValueAsync(EScreenCmds.SetBrightness.ToString(), value_InPercent);
        }

        public async Task SetSharpnessAsync(DisplayControlStep step)
        {
            await _opcController.SetMethodValueAsync(EScreenCmds.SetSharpness.ToString(), ((int)step));
        }

        public async Task SetContrastAsync(short value_InPercent)
        {
            await _opcController.SetMethodValueAsync(EScreenCmds.SetContrast.ToString(), value_InPercent);
        }

        public async Task SetDefaultValueAsync()
        {
            await _opcController.SetMethodValueAsync(EScreenCmds.SetDefaultValue.ToString());
        }

        public async Task SetFanStepAsync(DisplayControlStep step)
        {
            await _opcController.SetMethodValueAsync(EScreenCmds.SetFanStep.ToString(), ((int)step));
        }

        public async Task FanAutoOn(bool autOn)
        {
            await _opcController.SetMethodValueAsync(EScreenCmds.FanAutoOn.ToString(), autOn);
        }

        public void CustomCommand(string cmd)
        {
            _opcController.SetMethodValueAsync(EScreenCmds.CustomCommand.ToString(), cmd);
        }

        public override void TriggerUpdateEvent()
        {
            _opcController.SetMethodValueAsync(EScreenCmds.RaisePropertiesChanged.ToString());
        }

        public async Task RestoreParameters(IOpcMultiParams multiParams)
        {
            await _opcController.SetMethodValueAsync(EScreenCmds.RestoreParameters.ToString(), multiParams);
        }

        public void DeliverMessages(string msgName, object value)
        {
            string[] fullMessages = new string[3];
            EFeedbackMsgScreen index = 0;
            try
            {
                if (!EFeedbackMsgScreen.TryParse(msgName, out index))
                    Logger.Error($"{ControllerConfig.DeviceID} - unable to parse enum value: <{msgName}>");

                Side side = Side.Unknown;
                if (index != EFeedbackMsgScreen.State)
                {
                    if (String.IsNullOrWhiteSpace((string)value))
                        return;

                    fullMessages = ((string)value).Split(';');
                    side = (fullMessages.First() == Side.Front.ToString()) ? Side.Front : Side.Back;
                }

                switch (index)
                {
                    case EFeedbackMsgScreen.State:
                        if ((int)value >= 0)
                        {
                            State = new DeviceState((DeviceStatus)(int)value);
                            Messenger.Send(new StateMessage() { Side = side, State = State });
                        }
                        break;

                    case EFeedbackMsgScreen.StateMsg:
                        if (int.TryParse((string)value, out var state))
                        {
                            State = new DeviceState((DeviceStatus)state);
                            Messenger.Send(new StateMessage() { Side = side, State = State });
                        }
                        break;

                    case EFeedbackMsgScreen.StatusMsg:
                        if (!String.IsNullOrWhiteSpace((string)value))
                        {
                            Messenger.Send(new StatusMessage() { Side = side, Status = (string)value });
                        }
                        break;

                    case EFeedbackMsgScreen.IsAliveMsg:
                        if (int.TryParse((string)value, out var isAlive))
                        {
                            _opcController.NewMeterSubscription = isAlive;
                        }
                        break;

                    case EFeedbackMsgScreen.PowerStateMsg:
                        if (!String.IsNullOrWhiteSpace(fullMessages.Last()))
                        {
                            bool powerOn = fullMessages.Last().Contains("ON");
                            Messenger.Send(new PowerStateMessage() { Side = side, PowerState = powerOn });
                        }
                        break;

                    case EFeedbackMsgScreen.BacklightMsg:
                        if (Double.TryParse(fullMessages.Last(), out var backlight))
                        {
                            BacklightValue = (short)backlight;
                            Messenger.Send(new BacklightMessage() { Side = side, Backlight = backlight });
                        }
                        break;

                    case EFeedbackMsgScreen.BrightnessMsg:
                        if (Double.TryParse(fullMessages.Last(), out var brightness))
                        {
                            BrightnessValue = (short)brightness;
                            Messenger.Send(new BrightnessMessage() { Side = side, Brightness = brightness });
                        }
                        break;

                    case EFeedbackMsgScreen.ContrastMsg:
                        if (Double.TryParse(fullMessages.Last(), out var contrast))
                        {
                            ContrastValue = (short)contrast;
                            Messenger.Send(new ContrastMessage() { Side = side, Contrast = contrast });
                        }
                        break;

                    case EFeedbackMsgScreen.FanStepMsg:
                        if (Int32.TryParse(fullMessages.Last(), out var fanStep))
                        {
                            Messenger.Send(new FanStepMessage() { Side = side, FanStep = fanStep });
                        }
                        break;

                    case EFeedbackMsgScreen.FanRpmMsg:
                        if (Int32.TryParse(fullMessages.Last(), out var fanRpm))
                        {
                            FanRPMValue = fanRpm;
                            Messenger.Send(new FanRpmMessage() { Side = side, FanRpm = fanRpm });
                        }
                        break;

                    case EFeedbackMsgScreen.TemperatureMsg:
                        if (Double.TryParse(fullMessages.Last(), out var temperature))
                        {
                            TemperatureValue = temperature;
                            Messenger.Send(new TemperatureMessage() { Side = side, Temperature = temperature });
                        }
                        break;

                    case EFeedbackMsgScreen.FanAutoMsg:
                        if (!String.IsNullOrWhiteSpace(fullMessages.Last()))
                        {
                            bool fanAuto = fullMessages.Last().Contains("ON");

                            Messenger.Send(new FanAutoMessage() { Side = side, FanAuto = fanAuto });
                        }
                        break;

                    case EFeedbackMsgScreen.CustomMsg:
                        if (!String.IsNullOrWhiteSpace(fullMessages.Last()))
                        {
                            Messenger.Send(new CustomMessage() { Side = side, Custom = fullMessages.Last() });
                        }
                        break;

                    default:
                        Logger.Warning($"{ControllerConfig.DeviceID} - Unknown message: {msgName} {index}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{ControllerConfig.DeviceID} - {ex.Message}: {(string)value} {index}");
            }
        }
    }
}
