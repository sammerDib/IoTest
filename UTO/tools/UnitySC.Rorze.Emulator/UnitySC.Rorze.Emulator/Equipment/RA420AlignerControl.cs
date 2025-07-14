using System.Collections.Generic;

using UnitySC.Rorze.Emulator.Common;
using UnitySC.Rorze.Emulator.Controls.RA420;
using UnitySC.Rorze.Emulator.Controls.RR75x;

namespace UnitySC.Rorze.Emulator.Equipment
{
    internal partial class RA420AlignerControl : BaseEquipmentControl.BaseEquipmentControl
    {
        #region Fields

        private bool _waferPresence;
        private string _waferId = "000000";
        private int _substrateSize;

        #endregion

        #region Constructor

        public RA420AlignerControl()
        {
            InitializeComponent();

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.ExhaustFanRotating, "1");
            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.AlignerReadyToOperate, "1");
            rA420InputsOutputsControl.UpdateStatus();

            ra420StatusWordSpyControl1.SetStatus(Ra420Status.OperationMode, "1");
            ra420StatusWordSpyControl1.UpdateStatus();

            rA420InputsOutputsControl.StatusChanged += RA420InputsOutputsControl_StatusChanged;
            ra420StatusWordSpyControl1.StatusChanged += Ra420StatusWordSpyControl1_StatusChanged;
        }

        #endregion

        public override void Clean()
        {
            rA420InputsOutputsControl.StatusChanged -= RA420InputsOutputsControl_StatusChanged;
            ra420StatusWordSpyControl1.StatusChanged -= Ra420StatusWordSpyControl1_StatusChanged;

            base.Clean();
        }

        #region Overrides

        protected override List<string> GetResponses(string toRespondeTo)
        {
            List<string> answers = new List<string>();
            if (!string.IsNullOrEmpty(toRespondeTo))
            {
                if (toRespondeTo.Contains(".STIM"))
                {
                    answers.Add("aALN1.STIM:\r");
                }
                else if (toRespondeTo.Contains(".EVNT"))
                {
                    answers.Add("aALN1.EVNT:\r");
                    var eventParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var evnt = int.Parse(eventParameters[0]);
                    var isActive = int.Parse(eventParameters[1]);
                    if (isActive == 1)
                    {
                        switch (evnt)
                        {
                            case 0:
                                answers.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                                answers.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");
                                answers.Add("eALN1.GPOS:99/99/99/99\r");
                                answers.Add($"eALN1.GWID:{_waferId}\r");
                                break;
                            case 1:
                                answers.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                                break;

                            case 2:
                                answers.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");
                                break;

                            case 3:
                                answers.Add("eALN1.GPOS:99/99/99/99\r");
                                break;

                            case 4:
                                answers.Add($"eALN1.GWID:{_waferId}\r");
                                break;
                        }
                    }
                }
                else if (toRespondeTo.Contains(".STAT"))
                {
                    answers.Add($"aALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains(".GPIO"))
                {
                    answers.Add($"aALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains("GPOS"))
                {
                    answers.Add("aALN1.GPOS:99/99/99/99\r");
                }
                else if (toRespondeTo.Contains("GWID"))
                {
                    answers.Add($"aALN1.GWID:{_waferId}\r");
                }
                else if (toRespondeTo.Contains(".RSTA"))
                {
                    answers.Add("aALN1.RSTA:\r");

                    ra420StatusWordSpyControl1.SetStatus(Ra420Status.CommandProcessing, "1");
                    ra420StatusWordSpyControl1.UpdateStatus();
                    answers.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

                    ra420StatusWordSpyControl1.SetStatus(Ra420Status.CommandProcessing, "0");
                    ra420StatusWordSpyControl1.UpdateStatus();
                    answers.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains("SSIZ"))
                {
                    answers.Add("aALN1.SSIZ:\r");
                    _substrateSize = int.Parse(toRespondeTo.Substring(11, 1));
                }
                else if (toRespondeTo.Contains("GSIZ"))
                {
                    answers.Add($"aALN1.GSIZ:{_substrateSize}\r");
                }
                else if (toRespondeTo.Contains("ALGN"))
                {
                    answers.Add("aALN1.ALGN:\r");
                    answers.AddRange(PerformAlign());
                }
                else if (toRespondeTo.Contains("CLMP"))
                {
                    answers.Add("aALN1.CLMP:\r");
                    answers.AddRange(PerformClamp());
                }
                else if (toRespondeTo.Contains("UCLM"))
                {
                    var position = int.Parse(toRespondeTo.Substring(11, 1));
                    answers.Add("aALN1.UCLM:\r");
                    answers.AddRange(PerformUnclamp(position));
                }
                else if (toRespondeTo.Contains("HOME"))
                {
                    answers.Add("aALN1.HOME:\r");
                    answers.AddRange(PerformHome());
                }
                else if (toRespondeTo.Contains("INIT"))
                {
                    answers.Add("aALN1.INIT:\r");
                    answers.AddRange(PerformInit());
                }
                else if (toRespondeTo.Contains("ORGN"))
                {
                    answers.Add("aALN1.ORGN:\r");
                    answers.AddRange(PerformOrigin());
                }
                else if (toRespondeTo.Contains("STOP"))
                {
                    answers.Add("aALN1.STOP:\r");
                    ra420StatusWordSpyControl1.SetStatus(Ra420Status.CommandProcessing, "1");
                    ra420StatusWordSpyControl1.UpdateStatus();
                    answers.Add(
                        $"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                    ra420StatusWordSpyControl1.SetStatus(Ra420Status.CommandProcessing, "0");
                    ra420StatusWordSpyControl1.UpdateStatus();
                    answers.Add(
                        $"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                    ra420StatusWordSpyControl1.SetStatus(Ra420Status.OperationStatus, "0");
                    ra420StatusWordSpyControl1.UpdateStatus();
                    answers.Add(
                        $"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains("PAUS"))
                {
                    answers.Add("aALN1.PAUS:\r");
                    ra420StatusWordSpyControl1.SetStatus(Ra420Status.CommandProcessing, "1");
                    ra420StatusWordSpyControl1.UpdateStatus();
                    answers.Add(
                        $"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                    ra420StatusWordSpyControl1.SetStatus(Ra420Status.CommandProcessing, "0");
                    ra420StatusWordSpyControl1.UpdateStatus();
                    answers.Add(
                        $"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                    ra420StatusWordSpyControl1.SetStatus(Ra420Status.OperationStatus, "2");
                    ra420StatusWordSpyControl1.UpdateStatus();
                    answers.Add(
                        $"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains("GVER"))
                {
                    answers.Add($"aALN1.GVER:RORZE STD_ALN RA420 Ver 1.00\r");
                }
            }

            return answers;
        }

        protected override void SendConnectedMessage()
        {
            SendMessage("eALN1.CNCT:");
        }

        #endregion

        #region Event Handlers

        private void Ra420StatusWordSpyControl1_StatusChanged(object sender, System.EventArgs e)
        {
            SendMessage($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
        }

        private void RA420InputsOutputsControl_StatusChanged(object sender, System.EventArgs e)
        {
            SendMessage($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");
        }

        #endregion

        #region Private Methods

        private List<string> PerformAlign()
        {
            var alignStrings = new List<string>();

            ra420StatusWordSpyControl1.SetStatus(Ra420Status.OperationStatus, "1");
            ra420StatusWordSpyControl1.UpdateStatus();
            alignStrings.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            alignStrings.Add("eALN1.GPOS:99/99/01/00\r");
            alignStrings.Add("eALN1.GPOS:01/00/02/00\r");
            alignStrings.Add("eALN1.GPOS:01/00/99/00\r");
            alignStrings.Add("eALN1.GPOS:99/99/99/00\r");
            alignStrings.Add("eALN1.GPOS:01/00/02/00\r");
            alignStrings.Add("eALN1.GPOS:01/00/02/99\r");
            alignStrings.Add("eALN1.GPOS:99/99/02/99\r");
            alignStrings.Add("eALN1.GPOS:00/99/02/99\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SpindleSolenoidValveChuckingOff, "1");
            rA420InputsOutputsControl.UpdateStatus();
            alignStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor1, "0");
            rA420InputsOutputsControl.UpdateStatus();
            alignStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor2, "0");
            rA420InputsOutputsControl.UpdateStatus();
            alignStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SpindleSolenoidValveChuckingOff, "0");
            rA420InputsOutputsControl.UpdateStatus();
            alignStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            alignStrings.Add("eALN1.GPOS:00/99/99/99\r");
            alignStrings.Add("eALN1.GPOS:99/99/99/99\r");
            alignStrings.Add("eALN1.GPOS:01/00/99/99\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SpindleSolenoidValveChuckingOn, "1");
            rA420InputsOutputsControl.UpdateStatus();
            alignStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor2, _waferPresence ? "1" : "0");
            rA420InputsOutputsControl.UpdateStatus();
            alignStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor1, _waferPresence ? "1" : "0");
            rA420InputsOutputsControl.UpdateStatus();
            alignStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            alignStrings.Add("eALN1.GPOS:99/99/99/99\r");
            alignStrings.Add("eALN1.GPOS:99/00/01/99\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SpindleSolenoidValveChuckingOn, "0");
            rA420InputsOutputsControl.UpdateStatus();
            alignStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.AlignmentComplete, "1");
            rA420InputsOutputsControl.UpdateStatus();
            alignStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            alignStrings.Add("eALN1.GPOS:99/00/05/00\r"); //old eALN1.GPOS:99/00/01/00\r

            ra420StatusWordSpyControl1.SetStatus(Ra420Status.OperationStatus, "0");
            ra420StatusWordSpyControl1.UpdateStatus();
            alignStrings.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            return alignStrings;
        }

        private List<string> PerformClamp()
        {
            var clampStrings = new List<string>();

            ra420StatusWordSpyControl1.SetStatus(Ra420Status.OperationStatus, "1");
            ra420StatusWordSpyControl1.UpdateStatus();
            clampStrings.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            clampStrings.Add("eALN1.GPOS:99/00/99/00\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SpindleSolenoidValveChuckingOn, "1");
            rA420InputsOutputsControl.UpdateStatus();
            clampStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor2, _waferPresence ? "1" : "0");
            rA420InputsOutputsControl.UpdateStatus();
            clampStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor1, _waferPresence ? "1" : "0");
            rA420InputsOutputsControl.UpdateStatus();
            clampStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            clampStrings.Add("eALN1.GPOS:99/99/99/00\r");
            clampStrings.Add("eALN1.GPOS:99/00/01/00\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SpindleSolenoidValveChuckingOn, "0");
            rA420InputsOutputsControl.UpdateStatus();
            clampStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            ra420StatusWordSpyControl1.SetStatus(Ra420Status.OperationStatus, "0");
            ra420StatusWordSpyControl1.UpdateStatus();
            clampStrings.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            return clampStrings;
        }

        private List<string> PerformUnclamp(int position)
        {
            var unclampStrings = new List<string>();

            ra420StatusWordSpyControl1.SetStatus(Ra420Status.OperationStatus, "1");
            ra420StatusWordSpyControl1.UpdateStatus();
            unclampStrings.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SpindleSolenoidValveChuckingOff, "1");
            rA420InputsOutputsControl.UpdateStatus();
            unclampStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor1, "0");
            rA420InputsOutputsControl.UpdateStatus();
            unclampStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor2, "0");
            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.AlignmentComplete, "0");
            rA420InputsOutputsControl.UpdateStatus();
            unclampStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SpindleSolenoidValveChuckingOff, "0");
            rA420InputsOutputsControl.UpdateStatus();
            unclampStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            unclampStrings.Add("eALN1.GPOS:99/00/99/00\r");
            unclampStrings.Add("eALN1.GPOS:99/99/99/00\r");
            unclampStrings.Add($"eALN1.GPOS:99/00/0{position}/00\r");

            ra420StatusWordSpyControl1.SetStatus(Ra420Status.OperationStatus, "0");
            ra420StatusWordSpyControl1.UpdateStatus();
            unclampStrings.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            return unclampStrings;
        }

        private List<string> PerformHome()
        {
            var homeStrings = new List<string>();

            ra420StatusWordSpyControl1.SetStatus(Ra420Status.OperationStatus, "1");
            ra420StatusWordSpyControl1.UpdateStatus();
            homeStrings.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            homeStrings.Add("eALN1.GPOS:99/00/00/00\r");
            homeStrings.Add("eALN1.GPOS:99/99/00/00\r");
            homeStrings.Add("eALN1.GPOS:01/00/00/00\r");

            ra420StatusWordSpyControl1.SetStatus(Ra420Status.OperationStatus, "0");
            ra420StatusWordSpyControl1.UpdateStatus();
            homeStrings.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            return homeStrings;
        }

        private List<string> PerformInit()
        {
            var initStrings = new List<string>();

            ra420StatusWordSpyControl1.SetStatus(Ra420Status.OperationMode, "0");
            ra420StatusWordSpyControl1.UpdateStatus();
            initStrings.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            ra420StatusWordSpyControl1.SetStatus(Ra420Status.CommandProcessing, "1");
            ra420StatusWordSpyControl1.UpdateStatus();
            initStrings.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            ra420StatusWordSpyControl1.SetStatus(Ra420Status.OriginSearchCompleteFlag, "0");
            ra420StatusWordSpyControl1.UpdateStatus();
            initStrings.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            ra420StatusWordSpyControl1.SetStatus(Ra420Status.CommandProcessing, "0");
            ra420StatusWordSpyControl1.UpdateStatus();
            initStrings.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            ra420StatusWordSpyControl1.SetStatus(Ra420Status.OperationMode, "1");
            ra420StatusWordSpyControl1.UpdateStatus();
            initStrings.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            return initStrings;
        }

        private List<string> PerformOrigin()
        {
            var originStrings = new List<string>();

            ra420StatusWordSpyControl1.SetStatus(Ra420Status.OperationStatus, "1");
            ra420StatusWordSpyControl1.UpdateStatus();
            originStrings.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SpindleSolenoidValveChuckingOn, "1");
            rA420InputsOutputsControl.UpdateStatus();
            originStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor1, _waferPresence ? "1" : "0");
            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor2, _waferPresence ?"1" : "0");
            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetection, _waferPresence ? "1" : "0");
            rA420InputsOutputsControl.UpdateStatus();
            originStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            _waferId = "999999";
            originStrings.Add($"eALN1.GWID:{_waferId}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SpindleSolenoidValveChuckingOn, "0");
            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SpindleSolenoidValveChuckingOff, "1");
            rA420InputsOutputsControl.UpdateStatus();
            originStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor1, "0");
            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor2, "0");
            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetection, "0");
            rA420InputsOutputsControl.UpdateStatus();
            originStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            _waferId = "000000";
            originStrings.Add($"eALN1.GWID:{_waferId}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SpindleSolenoidValveChuckingOff, "0");
            rA420InputsOutputsControl.UpdateStatus();
            originStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SpindleSolenoidValveChuckingOn, "1");
            rA420InputsOutputsControl.UpdateStatus();
            originStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            originStrings.Add("eALN1.GPOS:01/00/99/00\r");
            originStrings.Add("eALN1.GPOS:99/99/99/00\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor1, _waferPresence ? "1" : "0");
            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor2, _waferPresence ? "1" : "0");
            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetection, _waferPresence ? "1" : "0");
            rA420InputsOutputsControl.UpdateStatus();
            originStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            _waferId = "999999";
            originStrings.Add($"eALN1.GWID:{_waferId}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SpindleSolenoidValveChuckingOn, "0");
            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SpindleSolenoidValveChuckingOff, "1");
            rA420InputsOutputsControl.UpdateStatus();
            originStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor1, "0");
            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor2, "0");
            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetection, "0");
            rA420InputsOutputsControl.UpdateStatus();
            originStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            _waferId = "000000";
            originStrings.Add($"eALN1.GWID:{_waferId}\r");

            ra420StatusWordSpyControl1.SetStatus(Ra420Status.OriginSearchCompleteFlag, "1");
            ra420StatusWordSpyControl1.UpdateStatus();
            originStrings.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            originStrings.Add("eALN1.GPOS:00/00/00/00\r");

            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SpindleSolenoidValveChuckingOff, "0");
            rA420InputsOutputsControl.UpdateStatus();
            originStrings.Add($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");

            ra420StatusWordSpyControl1.SetStatus(Ra420Status.OperationStatus, "0");
            ra420StatusWordSpyControl1.UpdateStatus();
            originStrings.Add($"eALN1.STAT:{ra420StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            return originStrings;
        }

        #endregion

        #region Public Methods

        public void PlaceWafer()
        {
            _waferPresence = true;
            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor1, "1");
            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor2, "1");
            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetection, "1");
            rA420InputsOutputsControl.UpdateStatus();
            SendMessage($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");
        }

        public void RemoveWafer()
        {
            _waferPresence = false;
            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor1, "0");
            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetectionSensor2, "0");
            rA420InputsOutputsControl.SetStatus(Ra420InputsOutputs.SubstrateDetection, "0");
            rA420InputsOutputsControl.UpdateStatus();
            SendMessage($"eALN1.GPIO:{rA420InputsOutputsControl.GetConcatenatedStatuses()}\r");
        }

        #endregion
    }
}
