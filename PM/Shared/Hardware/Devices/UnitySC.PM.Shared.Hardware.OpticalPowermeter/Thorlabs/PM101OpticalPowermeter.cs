using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Controller;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.OpticalPowermeter;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

using Workstation.ServiceModel.Ua;

namespace UnitySC.PM.Shared.Hardware.OpticalPowermeter
{
    public class PM101OpticalPowermeter : OpticalPowermeterBase
    {
        private const double NOTIFICATION_INTERVAL = 20;

        private enum PM101Cmds
        {
            RaisePropertiesChanged,
            CustomCommand,
            EnableAutoRange,
            RangesVariation,
            StartDarkAdjust,
            CancelDarkAdjust,
            SetResponsivity
        }

        private enum EFeedbackMsgPM101
        {
            State = 1,
            PowerMsg = 10,
            CurrentMsg,
            IdentifierMsg,
            ResponsivityMsg,
            DarkOffsetMsg,
            DarkAdjustStateMsg,
            CustomMsg,
            SensorTypeMsg,
            WavelengthMsg,
            BeamDiameterMsg,
            SensorAttenuationMsg,
            WavelengthRangeMsg,
            MaxPowerMsg,
            MinPowerMsg,
            CalibMsg
        }

        private NotificationTemplate<IOpticalPowermeterServiceCallback> _opticalPowermeterCallback = new NotificationTemplate<IOpticalPowermeterServiceCallback>();

        private PM101OpticalPowermeterConfig _opticalPowermeterConfig;

        public List<string> Wavelengths { get; } = new List<string>();

        public PM101OpticalPowermeter(string name, string id, ILogger logger)
        {
            Name = name;
            DeviceID = id;
            Logger = logger;
        }

        public override void Init(OpticalPowermeterConfig config)
        {
            if (!(config is PM101OpticalPowermeterConfig))
                throw new Exception("Invalid configuration type");

            _opticalPowermeterConfig = (PM101OpticalPowermeterConfig)config;
            Configuration = config;

            foreach (string wavelength in _opticalPowermeterConfig.Wavelengths)
            {
                Wavelengths.Add(wavelength);
            }

            Logger.Information("Init the device PM101OpticalPowermeter  " + Configuration.DeviceID);
        }

        public override void Connect()
        {
            throw new NotImplementedException();
        }

        private void DeliverMessages(string msgName, object value)
        {
            string[] fullMessages = new string[3];
            EFeedbackMsgPM101 index = 0;
            EFeedbackMsgPM101.TryParse(msgName, out index);
            PowerIlluminationFlow flow = PowerIlluminationFlow.Unknown;

            if (index != EFeedbackMsgPM101.State)
            {
                if (String.IsNullOrWhiteSpace((string)value))
                    return;

                fullMessages = ((string)value).Split(';');
                flow = (fullMessages.First() == PowerIlluminationFlow.HS.ToString()) ? PowerIlluminationFlow.HS : PowerIlluminationFlow.HT;
            }

            switch (index)
            {
                case EFeedbackMsgPM101.State:
                    State = new DeviceState((DeviceStatus)(int)value);
                    break;

                case EFeedbackMsgPM101.PowerMsg:
                    double power = Double.TryParse(fullMessages[1], out var tempPower) ? tempPower : -1;
                    double powerCal_mW = Double.TryParse(fullMessages[2], out var tempPowerCal_mW) ? tempPowerCal_mW : -1;
                    double rFactor = Double.TryParse(fullMessages[3], out var tempRFactor) ? tempRFactor : -1;

                    if (power != -1 && powerCal_mW != -1 && rFactor != -1)
                        Messenger.Send(new PowerMessage() { Flow = flow, Power = power, PowerCal_mW = powerCal_mW, RFactor = rFactor });
                    break;

                case EFeedbackMsgPM101.CurrentMsg:
                    double current_mA = Double.TryParse(fullMessages.Last(), out var tempCurrent) ? tempCurrent : 0;
                    Messenger.Send(new CurrentMessage() { Flow = flow, Current_mA = current_mA });
                    break;

                case EFeedbackMsgPM101.IdentifierMsg:
                    Messenger.Send(new IdentifierMessage() { Flow = flow, Identifier = fullMessages.Last() });
                    break;

                case EFeedbackMsgPM101.SensorTypeMsg:
                    Messenger.Send(new SensorTypeMessage() { Flow = flow, SensorType = fullMessages.Last() });
                    break;

                case EFeedbackMsgPM101.WavelengthMsg:
                    uint wavelength = UInt32.TryParse(fullMessages.Last(), out var tempWavelength) ? tempWavelength : 0;
                    Messenger.Send(new WavelengthMessage() { Flow = flow, Wavelength = wavelength });
                    break;

                case EFeedbackMsgPM101.ResponsivityMsg:
                    double responsivity = Double.TryParse(fullMessages.Last(), out var tempResponsivity) ? tempResponsivity : 0;
                    Messenger.Send(new ResponsivityMessage() { Flow = flow, Responsivity = responsivity });
                    break;

                case EFeedbackMsgPM101.DarkOffsetMsg:
                    double darkOffset = Double.TryParse(fullMessages.Last(), out var tempdarkOffset) ? tempdarkOffset : 0;
                    Logger.Information("Powermeter - DarkOffsetMsg:" + darkOffset);
                    Messenger.Send(new DarkOffsetMessage() { Flow = flow, DarkOffset = darkOffset });
                    break;

                case EFeedbackMsgPM101.DarkAdjustStateMsg:
                    Messenger.Send(new DarkAdjustStateMessage() { Flow = flow, DarkAdjustState = fullMessages.Last() });
                    break;

                case EFeedbackMsgPM101.CustomMsg:
                    Messenger.Send(new CustomMessage() { Flow = flow, Custom = fullMessages.Last() });
                    break;

                case EFeedbackMsgPM101.BeamDiameterMsg:
                    uint beamDiameter = UInt32.TryParse(fullMessages.Last(), out var tempBeamDiameter) ? tempBeamDiameter : 0;
                    Messenger.Send(new BeamDiameterMessage() { Flow = flow, BeamDiameter = beamDiameter });
                    break;

                case EFeedbackMsgPM101.SensorAttenuationMsg:
                    uint sensorAttenuation = UInt32.TryParse(fullMessages.Last(), out var tempSensorAtt) ? tempSensorAtt : 0;
                    Messenger.Send(new SensorAttenuationMessage() { Flow = flow, SensorAttenuation = sensorAttenuation });
                    break;

                case EFeedbackMsgPM101.WavelengthRangeMsg:
                    double wavelengthRange = Double.TryParse(fullMessages.Last(), out var tempWavelengthRange) ? tempWavelengthRange : 0;
                    Messenger.Send(new WavelengthRangeMessage() { Flow = flow, WavelengthRange = wavelengthRange });
                    break;

                case EFeedbackMsgPM101.MaxPowerMsg:
                    double maxPower = Double.TryParse(fullMessages.Last(), out var tempMaxPower) ? tempMaxPower : 0;
                    Messenger.Send(new MaxPowerMessage() { Flow = flow, MaximumPower = maxPower });
                    break;

                case EFeedbackMsgPM101.MinPowerMsg:
                    double minPower = Double.TryParse(fullMessages.Last(), out var tempMinPower) ? tempMinPower : 0;
                    Messenger.Send(new MinPowerMessage() { Flow = flow, MinimumPower = minPower });
                    break;

                case EFeedbackMsgPM101.CalibMsg:
                    double rFactorS = Double.TryParse(fullMessages[1], out var tempRFactorS) ? tempRFactorS : -1;
                    double rFactorP = Double.TryParse(fullMessages[2], out var tempRFactorP) ? tempRFactorP : -1;
                    Messenger.Send(new RFactorsCalibMessage() { Flow = flow, RFactorS = rFactorS, RFactorP = rFactorP });
                    break;

                default:
                    Logger.Warning(string.Format("EFeedbackMsgPM101 - {0} Unknown message : {1}", flow.ToString(), msgName));
                    break;
            }
        }

        public override void TriggerUpdateEvent()
        {
            throw new NotImplementedException();
        }

        public override void CustomCommand(string customCmd)
        {
            throw new NotImplementedException();
        }

        private void UpdateAvailableWavelengths(List<string> wavelengths)
        {
            throw new NotImplementedException();
        }

        public override void EnableAutoRange(bool activate)
        {
            throw new NotImplementedException();
        }

        public override void RangesVariation(string range)
        {
            throw new NotImplementedException();
        }

        public override void StartDarkAdjust()
        {
            throw new NotImplementedException();
        }

        public override void CancelDarkAdjust()
        {
            throw new NotImplementedException();
        }

        public override void EditResponsivity(double responsivity_mA_W)
        {
            throw new NotImplementedException();
        }

        private string ConvertRange(string range)
        {
            var unit = new string[] { "W", "mW", "µW" };

            string[] someArray = range.Split(new char[] { ' ' });
            var index = Array.FindIndex(unit, va => va == someArray[1]);
            double pow = Math.Pow(10, -3 * index);

            return (Double.Parse(someArray[0]) * pow).ToString();
        }
    }
}
