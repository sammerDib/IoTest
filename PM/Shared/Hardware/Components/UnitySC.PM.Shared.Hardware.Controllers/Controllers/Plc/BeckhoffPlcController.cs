using System;
using System.Collections.Generic;
using System.Threading;

using CommunityToolkit.Mvvm.Messaging;

//using TcEventLoggerAdsProxyLib;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Plc
{
    public class BeckhoffPlcController : PlcController
    {

        private OpcController _opcController;
        private OpcControllerConfig _opcControllerConfig;
        private IGlobalStatusServer _globalStatus;
        private bool _listenToSmokeDetector;
     
        //private TcEventLogger _tcEventlogger;
        private const int LangId = 1031;

        private int _cntVaccumDetectMsgInit = 0;

        private enum CX5140Cmds
        { PlcRestart, PlcReboot, CustomCommand, TriggerOutEmitSignal, SmokeDetectorResetAlarm, RaisePropertiesChangedBase }

        private enum EFeedbackMsgPLC
        {
            SmokeDetectedMsg = 0,
            SmokeDetectorFaultMsg,
            VacuumDetectorMsg,
            AmsNetIdMsg
        }

        public BeckhoffPlcController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
            ILogger logger) : base(opcControllerConfig, globalStatusServer, logger)
        {
            _opcControllerConfig = opcControllerConfig;
            _opcController = new OpcController(opcControllerConfig, logger, new DeliverMessagesDelegate(DeliverMessages));

            _listenToSmokeDetector = !((_opcControllerConfig as BeckhoffPlcControllerConfig)?.MuteSmokeDetector ?? false);

            _globalStatus = ClassLocator.Default.GetInstance<IGlobalStatusServer>();

            ConnectEventLogger();
        }

        private void ConnectEventLogger()
        {
            // GVA : A garder pour remonter les logs provenant du PLC
            //_tcEventlogger = new TcEventLogger();
            //_tcEventlogger.MessageSent += OnMessageSent;

            string hostname;
            if (_opcControllerConfig.IsSimulated)
            {
                hostname = "localhost";
            }
            else
            {
                hostname = _opcControllerConfig.OpcCom.Hostname;
            }

            try
            {
                //_tcEventlogger.Connect(hostname);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "Connection to event logger failed");
            }
        }

        public override void Init(List<Message> initErrors)
        {
            _opcController.Init(initErrors);
            if (!_listenToSmokeDetector)
                Logger.Information("Mute ModeSD ON");
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

        public void TriggerUpdateEvent()
        {
            _opcController.SetMethodValueAsync(CX5140Cmds.RaisePropertiesChangedBase.ToString());
        }

        public override void Restart()
        {
            _opcController.SetMethodValueAsync(CX5140Cmds.PlcRestart.ToString());
        }

        public override void Reboot()
        {
            _opcController.SetMethodValueAsync(CX5140Cmds.PlcReboot.ToString());
        }

        public override void CustomCommand(string custom)
        {
            _opcController.SetMethodValueAsync(CX5140Cmds.CustomCommand.ToString(), custom);
            Thread.Sleep(1000);
        }

        public void DeliverMessages(string msgName, object value)
        {
            try
            {
                EFeedbackMsgPLC index = 0;
                EFeedbackMsgPLC.TryParse(msgName, out index);

                switch (index)
                {
                    case EFeedbackMsgPLC.SmokeDetectedMsg:
                        Boolean.TryParse((string)value, out var smokeDetected);
                        if (smokeDetected && _listenToSmokeDetector)
                        {
                            string message = $"Smoke detected";
                            _globalStatus.SetGlobalStatus(new GlobalStatus(new Message(ErrorID.SmokeDetected, MessageLevel.Error, message)));
                            Logger.Error(message);
                        }
                        break;

                    case EFeedbackMsgPLC.SmokeDetectorFaultMsg:
                        Boolean.TryParse((string)value, out var smokeDetectorISFaulty);
                        if (smokeDetectorISFaulty)
                        {
                            Logger.Error($"Smoke detector is faulty");
                        }
                        break;

                    case EFeedbackMsgPLC.VacuumDetectorMsg:
                        Boolean.TryParse((string)value, out var vacuumIsReady);
                        if (!vacuumIsReady)
                        {
                            if (_cntVaccumDetectMsgInit > 0)
                            {
                                string message = $"Vacuum is not ready";
                                Logger.Error(message);
                            }
                            else
                            {
                                // ignore first init
                                ++_cntVaccumDetectMsgInit;
                            }
                        }
                        break;

                    case EFeedbackMsgPLC.AmsNetIdMsg:
                        if (!String.IsNullOrWhiteSpace((string)value))
                        {
                            Messenger.Send(new AmsNetIdMessage() { AmsNetId = (string)value });
                        }
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

        public override void StartTriggerOutEmitSignal(double pulseDuration_ms = 1)
        {
            _opcController.SetMethodValueAsync(CX5140Cmds.TriggerOutEmitSignal.ToString(), pulseDuration_ms);
        }

        public override void SmokeDetectorResetAlarm()
        {
            _globalStatus.SetGlobalStatus(new GlobalStatus(new Message(MessageLevel.Information, "Reset smoke alarm")));
            _opcController.SetMethodValueAsync(CX5140Cmds.SmokeDetectorResetAlarm.ToString());
        }        

        /*
        private void OnMessageSent(TcMessage message)
        {
            LogEventLevel eventLevel = (LogEventLevel)message.EventId;

            switch (eventLevel)
            {
                case LogEventLevel.Verbose:
                    _logger.Verbose(message.GetText(LangId));
                    break;

                case LogEventLevel.Debug:
                    _logger.Debug(message.GetText(LangId));
                    break;

                case LogEventLevel.Information:
                    _logger.Information(message.GetText(LangId));
                    break;

                case LogEventLevel.Warning:
                    _logger.Warning(message.GetText(LangId));
                    break;

                case LogEventLevel.Error:
                    _logger.Error(message.GetText(LangId));
                    break;

                case LogEventLevel.Fatal:
                    // TODO gva : define exception
                    //_logger.Fatal(message.GetText(LangId));
                    break;

                default:
                    throw new ArgumentException("Plc unknown event message. EventLevel= " + eventLevel.ToString());
            }
        }
        */
    }
}
