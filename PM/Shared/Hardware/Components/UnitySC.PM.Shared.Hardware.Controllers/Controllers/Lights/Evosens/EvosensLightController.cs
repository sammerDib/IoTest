using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Light;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Light
{
    public class EvosensLightController : LightController, IPowerLightController
    {
        protected new EvosensLightControllerConfig ControllerConfig => (EvosensLightControllerConfig)base.ControllerConfig;
        private OpcController _opcController;
        private string[] _lightMessage = new string[1];

        public Dictionary<string, LightSourceMessage> LightSourceMessages { get; set; } = new Dictionary<string, LightSourceMessage>();

        private enum EFeedbackMsgEvosensLight
        {
            State = 0,
            StateMsg = 1,
            StatusMsg = 2,
            IsAliveMsg = 3,
            SwitchOnMsg = 10,
            PowerMsg,
            IntensityMsg,
            TemperatureMsg,
            LightSourcesMsg,
            TimeLightSourceMsg,
            CustomMsg
        }

        private enum EvosensLightCmds
        {
            SetSwitchOn, GetSwitchOn, SetPower, GetPower, SetIntensity, GetIntensity, GetTimeLightSource, GetLightSources,
            CustomCommand, RaisePropertiesChanged
        }

        public EvosensLightController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(opcControllerConfig, globalStatusServer, logger)
        {
            _opcController = new OpcController(opcControllerConfig, logger, new DeliverMessagesDelegate(DeliverMessages));
        }

        public override void Init(List<Message> initErrors)
        {
            _opcController.Init(initErrors);
            InitLightSources();
        }

        public override void Connect()
        {
            _opcController.Connect();
        }

        public override void Connect(string deviceID)
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

        public override double GetIntensity(string lightID)
        {
            throw new NotImplementedException();
        }

        public override void SetIntensity(string lightID, double intensity)
        {
            _opcController.SetMethodValueAsync(EvosensLightCmds.SetIntensity.ToString(), lightID, intensity);
        }

        public void InitLightSources()
        {
            _opcController.SetMethodValueAsync(EvosensLightCmds.GetLightSources.ToString());
        }

        public void SetPower(string lightID, double powerInPercent)
        {
            _opcController.SetMethodValueAsync(EvosensLightCmds.SetPower.ToString(), lightID, powerInPercent);
        }

        public double GetPower(string lightID)
        {
            return LightSourceMessages[lightID].Power;
        }

        public void SwitchOn(string lightID, bool powerOn)
        {
            _opcController.SetMethodValueAsync(EvosensLightCmds.SetSwitchOn.ToString(), lightID, powerOn);
        }

        public void RefreshLightSource(string lightID)
        {
            _opcController.SetMethodValueAsync(EvosensLightCmds.RaisePropertiesChanged.ToString(), lightID);
        }

        public void RefreshPower(string lightID)
        {
            _opcController.SetMethodValueAsync(EvosensLightCmds.GetPower.ToString(), lightID);
        }

        public void RefreshSwitchOn(string lightID)
        {
            _opcController.SetMethodValueAsync(EvosensLightCmds.GetSwitchOn.ToString(), lightID);
        }

        private void DeliverMessages(string msgName, object value)
        {
            string[] fullMessages = new string[3];
            EFeedbackMsgEvosensLight index = 0;
            try
            {
                if (!EFeedbackMsgEvosensLight.TryParse(msgName, out index))
                    Logger.Error($"{ControllerConfig.DeviceID} - unable to parse enum value: <{msgName}>");

                if (index != EFeedbackMsgEvosensLight.State)
                {
                    if (String.IsNullOrWhiteSpace((string)value))
                        return;

                    fullMessages = ((string)value).Split('=');
                }

                switch (index)
                {
                    case EFeedbackMsgEvosensLight.State:
                        if ((int)value >= 0)
                        {
                            if (State?.Status != (DeviceStatus)(int)value)
                            {
                                State = new DeviceState((DeviceStatus)(int)value);
                                Messenger.Send(new StateMessage() { State = State });
                            }
                        }
                        break;

                    case EFeedbackMsgEvosensLight.StateMsg:
                        if (int.TryParse((string)value, out var state))
                        {
                            if (State?.Status != (DeviceStatus)state)
                            {
                                State = new DeviceState((DeviceStatus)(int)value);
                                Messenger.Send(new StateMessage() { State = State });
                            }
                        }
                        break;

                    case EFeedbackMsgEvosensLight.IsAliveMsg:
                        if (int.TryParse((string)value, out var isAlive))
                        {
                            _opcController.NewMeterSubscription = isAlive;
                        }
                        break;

                    case EFeedbackMsgEvosensLight.SwitchOnMsg:
                        if (!String.IsNullOrWhiteSpace(fullMessages[1]))
                        {
                            if (SplitsLightSourceBValue(fullMessages[1]))
                            {
                                if (LightSourceMessages.ContainsKey(_lightMessage[0]))
                                {
                                    var switchOn = Boolean.Parse(_lightMessage[1]);
                                    LightSourceMessages[_lightMessage[0]].SwitchOn = switchOn;
                                    Messenger.Send(new LightSourceMessage() { SwitchOn = switchOn });
                                }
                            }
                            else
                            {
                                //_logger.Warning($"{ControllerConfig.DeviceID} - {EFeedbackMsgEvosensLight.SwitchOnMsg}: {(string)value}");
                            }
                        }
                        break;

                    case EFeedbackMsgEvosensLight.PowerMsg:
                        if (!String.IsNullOrWhiteSpace(fullMessages[1]))
                        {
                            if (SplitsLightSourceDValue(fullMessages[1]))
                            {
                                if (LightSourceMessages.ContainsKey(_lightMessage[0]))
                                {
                                    var power = Double.Parse(_lightMessage[1]);
                                    LightSourceMessages[_lightMessage[0]].Power = power;
                                    Messenger.Send(new LightSourceMessage() { Power = power });
                                    Logger.Information($"Power: {fullMessages[1]}");
                                }
                            }
                            else
                            {
                                //_logger.Warning($"{ControllerConfig.DeviceID} - {EFeedbackMsgEvosensLight.PowerMsg}: {(string)value}");
                            }
                        }
                        break;

                    case EFeedbackMsgEvosensLight.IntensityMsg:
                        if (!String.IsNullOrWhiteSpace(fullMessages[1]))
                        {
                            if (SplitsLightSourceDValue(fullMessages[1]))
                            {
                                if (LightSourceMessages.ContainsKey(_lightMessage[0]))
                                {
                                    var intensity = Double.Parse(_lightMessage[1]);
                                    LightSourceMessages[_lightMessage[0]].Intensity = intensity;
                                    Messenger.Send(new LightSourceMessage() { Intensity = intensity });
                                }
                            }
                            else
                            {
                              //  _logger.Warning($"{ControllerConfig.DeviceID} - {EFeedbackMsgEvosensLight.IntensityMsg}: {(string)value}");
                            }
                        }
                        break;

                    case EFeedbackMsgEvosensLight.TemperatureMsg:
                        if (!String.IsNullOrWhiteSpace(fullMessages[1]))
                        {
                            if (SplitsLightSourceDValue(fullMessages[1]))
                            {
                                if (LightSourceMessages.ContainsKey(_lightMessage[0]))
                                {
                                    var temperature = Double.Parse(_lightMessage[1]);
                                    LightSourceMessages[_lightMessage[0]].Temperature = temperature;
                                    Messenger.Send(new LightSourceMessage() { Temperature = temperature });
                                }
                            }
                            else
                            {
                                //_logger.Warning($"{ControllerConfig.DeviceID} - {EFeedbackMsgEvosensLight.TemperatureMsg}: {(string)value}");
                            }
                        }

                        /*if (!Double.TryParse((string)value, out var temperature))
                        {
                            //var position = (SlitDoorPosition)Enum.Parse(typeof(SlitDoorPosition), (string)value);
                            //Messenger.Send(new SlitDoorPositionMessage() { SlitDoorPosition = position });
                        }*/
                        break;

                    case EFeedbackMsgEvosensLight.LightSourcesMsg:
                        if (!String.IsNullOrWhiteSpace(fullMessages[1]))
                        {
                            LightSourceMessages.Clear();

                            String[] messages = (fullMessages[1]).Split(',');
                            foreach (string message in messages)
                            {
                                LightSourceMessages.Add(message, new LightSourceMessage() { LightID = message });
                            }
                        }
                        break;

                    case EFeedbackMsgEvosensLight.TimeLightSourceMsg:

                        break;

                    case EFeedbackMsgEvosensLight.CustomMsg:

                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{ControllerConfig.DeviceID} - {ex.Message}: {(string)value} {index}");
            }
        }

        private bool SplitsLightSourceDValue(string text)
        {
            bool isValid = false;

            _lightMessage = text.Split('-');
            if (_lightMessage.Length == 2)
            {
                string numericString = Regex.Replace(_lightMessage[1], @"[^\d.-]", "");
                if (Double.TryParse(numericString, NumberStyles.Any, new CultureInfo("en-US"), out var value))
                {
                    _lightMessage[1] = value.ToString();
                    isValid = true;
                }
            }
            return isValid;
        }

        private bool SplitsLightSourceBValue(string text)
        {
            bool isValid = false;
            _lightMessage = text.Split('-');
            if (_lightMessage.Length == 2)
            {
                if (_lightMessage[1] == ">ON")
                    _lightMessage[1] = true.ToString();
                else
                    _lightMessage[1] = false.ToString();

                isValid = true;
            }
            return isValid;
        }
    }
}
