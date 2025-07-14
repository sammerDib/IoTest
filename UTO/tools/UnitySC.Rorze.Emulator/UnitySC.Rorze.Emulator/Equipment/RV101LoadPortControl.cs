using System;
using System.Collections.Generic;
using UnitySC.Rorze.Emulator.Controls.RV101;

namespace UnitySC.Rorze.Emulator.Equipment
{
    internal partial class Rv101LoadPortControl : BaseEquipmentControl.BaseEquipmentControl
    {
        #region Fields

        private string _internalMapping = "1010101010101";

        #endregion

        #region Constructor

        public Rv101LoadPortControl()
        {
            InitializeComponent();

            mappingTextBox.Text = _internalMapping;

            loadPortStatusWordSpyControl1.StatusChanged += LoadPortStatusWordSpyControl1_StatusChanged;
            rv101InputsOutputsControl1.StatusChanged += RV101InputsOutputsControl1_StatusChanged;
        }

        #endregion

        #region Properties

        public int InstanceId { get; set; }

        #endregion

        #region Event Handlers

        private void LoadPortStatusWordSpyControl1_StatusChanged(object sender, EventArgs e)
        {
            SendMessage($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");
        }

        private void RV101InputsOutputsControl1_StatusChanged(object sender, EventArgs e)
        {
            SendMessage($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");
        }

        private void MappingButton_Click(object sender, EventArgs e)
        {
            _internalMapping = mappingTextBox.Text;
        }

        private void FoupPresentPlacedButton_Click(object sender, EventArgs e)
        {
            //TODO What about 200mm & 150mm sensor presence?
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.PresenceMiddle, "1");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.PresenceLeft, "1");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.PresenceRight, "1");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierProperPlaced, "1");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierPresenceSensorOn, "1");
            rv101InputsOutputsControl1.UpdateStatus();
            SendMessage($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");
        }

        private void FoupPresentNoPlacedButton_Click(object sender, EventArgs e)
        {
            //TODO What about 200mm & 150mm sensor presence?
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.PresenceMiddle, "1");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.PresenceLeft, "0");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.PresenceRight, "0");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierProperPlaced, "0");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierPresenceSensorOn, "1");
            rv101InputsOutputsControl1.UpdateStatus();
            SendMessage($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");
        }

        private void FoupNoPresentPlacedButton_Click(object sender, EventArgs e)
        {
            //TODO What about 200mm & 150mm sensor presence?
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.PresenceMiddle, "0");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.PresenceLeft, "1");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.PresenceRight, "1");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierProperPlaced, "0");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierPresenceSensorOn, "1");
            rv101InputsOutputsControl1.UpdateStatus();
            SendMessage($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

        }

        private void FoupRemovedButton_Click(object sender, EventArgs e)
        {
            //TODO What about 200mm & 150mm sensor presence?
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.PresenceMiddle, "0");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.PresenceLeft, "0");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.PresenceRight, "0");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierProperPlaced, "0");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierPresenceSensorOn, "0");
            rv101InputsOutputsControl1.UpdateStatus();
            SendMessage($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");
        }

        #endregion

        #region Overrides

        public override void Clean()
        {
            loadPortStatusWordSpyControl1.StatusChanged -= LoadPortStatusWordSpyControl1_StatusChanged;
            rv101InputsOutputsControl1.StatusChanged -= RV101InputsOutputsControl1_StatusChanged;

            base.Clean();
        }

        protected override List<string> GetResponses(string toRespondeTo)
        {
            List<string> answers = new List<string>();
            if (!string.IsNullOrEmpty(toRespondeTo))
            {
                if (toRespondeTo.Contains(".INIT"))
                {
                    answers.Add($"aSTG{InstanceId}.INIT:\r");
                    answers.AddRange(PerformInit());

                }
                else if (toRespondeTo.Contains(".GWID"))
                {
                    answers.Add($"aSTG{InstanceId}.GWID:AUTO/CST1\r");
                }
                else if (toRespondeTo.Contains(".SWID"))
                {
                    answers.Add($"aSTG{InstanceId}.SWID:\r");
                }
                else if (toRespondeTo.Contains(".HOME"))
                {
                    var slot = byte.Parse(toRespondeTo.Substring(13, toRespondeTo.Length - 15));
                    answers.Add($"aSTG{InstanceId}.HOME:\r");
                    answers.AddRange(PerformHome(slot));
                }
                else if (toRespondeTo.Contains(".UCLM"))
                {
                    var param1 = -1;
                    var param2 = -1;

                    param1 = Int32.Parse(toRespondeTo[11].ToString());
                    if (toRespondeTo.Contains(","))
                    {
                        param2 = Int32.Parse(toRespondeTo[13].ToString());
                    }

                    answers.Add($"aSTG{InstanceId}.UCLM:\r");
                    answers.AddRange(PerformUnclamp(param1, param2));
                }
                else if (toRespondeTo.Contains(".CLMP")) //11
                {
                    var param1 = Int32.Parse(toRespondeTo[11].ToString());

                    answers.Add($"aSTG{InstanceId}.CLMP:\r");
                    answers.AddRange(PerformClamp(param1));
                }
                else if (toRespondeTo.Contains(".GMAP"))
                {
                    answers.Add($"aSTG{InstanceId}.GMAP:{_internalMapping}\r");
                }
                else if (toRespondeTo.Contains(".WMAP"))
                {
                    answers.Add($"aSTG{InstanceId}.WMAP:\r");
                }
                else if (toRespondeTo.Contains(".STIM"))
                {
                    answers.Add($"aSTG{InstanceId}.STIM:\r");
                }
                else if (toRespondeTo.Contains(".EVNT"))
                {
                    answers.Add($"aSTG{InstanceId}.EVNT:\r");
                    var evnt = int.Parse(toRespondeTo.Substring(11, 1));
                    switch (evnt)
                    {
                        case 1:
                            answers.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                            break;

                        case 2:
                            answers.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");
                            break;

                        case 3:
                            answers.Add($"eSTG{InstanceId}.GPOS:02005/01\r");
                            break;

                        case 4:
                            answers.Add($"eSTG{InstanceId}.GWID:AUTO/CST1\r");
                            break;
                    }
                }
                else if (toRespondeTo.Contains(".ORGN"))
                {
                    answers.Add($"aSTG{InstanceId}.ORGN:\r");
                    answers.AddRange(PerformOrigin());
                }
                else if (toRespondeTo.Contains(".STAT"))
                {
                    answers.Add($"aSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains(".GPIO"))
                {
                    answers.Add($"aSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains(".GPOS"))
                {
                    answers.Add($"aSTG{InstanceId}.GPOS:02005/01\r");
                }
                else if (toRespondeTo.Contains(".DPRM.GTDT"))
                {
                    answers.Add($"aSTG{InstanceId}.DPRM.GTDT:13\r");
                }
                else if (toRespondeTo.Contains(".DEQU.GTDT"))
                {
                    answers.Add($"aSTG{InstanceId}.DEQU.GTDT:5\r");
                }
                else if (toRespondeTo.Contains(".SPOT"))
                {
                    answers.Add($"aSTG{InstanceId}.SPOT:\r");
                }
                else if (toRespondeTo.Contains("STOP"))
                {
                    answers.Add($"aSTG{InstanceId}.STOP:\r");
                    loadPortStatusWordSpyControl1.SetStatus(Rv101Status.OperationMode, "2");
                    loadPortStatusWordSpyControl1.UpdateStatus();
                    answers.Add($"eTRB{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains("GVER"))
                {
                    answers.Add($"aSTG{InstanceId}.GVER:RORZE STD_STG RV101 Ver 1.00\r");
                }
            }

            return answers;
        }

        protected override void SendConnectedMessage()
        {
            SendMessage($"eSTG{InstanceId}.CNCT:");
        }

        #endregion

        #region Private Methods

        private List<string> PerformInit()
        {
            //TODO Need logs to confirm the sequence
            var initStrings = new List<string>();

            loadPortStatusWordSpyControl1.SetStatus(Rv101Status.OperationMode, "1");
            loadPortStatusWordSpyControl1.UpdateStatus();
            initStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            initStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterOpenInput, "0");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterOpenOutput, "0");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterOpenOutput2, "0");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterCloseInput, "1");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterCloseOutput, "1");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.Cover, "1");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CoverLock, "0");
            rv101InputsOutputsControl1.UpdateStatus();
            initStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            initStrings.Add($"eSTG{InstanceId}.GPOS:99/99\r");
            initStrings.Add($"eSTG{InstanceId}.GWID:AUTO/----\r");
            initStrings.Add($"eSTG{InstanceId}.GWID:AUTO/CST1\r");

            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.PreparationCompleted, "1");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.PreparationCompleted2, "1");
            rv101InputsOutputsControl1.UpdateStatus();
            initStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            loadPortStatusWordSpyControl1.SetStatus(Rv101Status.OperationStatus, "0");
            loadPortStatusWordSpyControl1.UpdateStatus();
            initStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            return initStrings;
        }

        private List<string> PerformOrigin()
        {
            //TODO Need logs to confirm the sequence
            var originStrings = new List<string>();

            loadPortStatusWordSpyControl1.SetStatus(Rv101Status.OperationStatus, "1");
            loadPortStatusWordSpyControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierClampCloseRight, "1");
            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierClampCloseLeft, "1");
            rv101InputsOutputsControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ClampRightOpen, "0");
            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ClampLeftOpen, "0");
            rv101InputsOutputsControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ClampRightClose, "1");
            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ClampLeftClose, "1");
            rv101InputsOutputsControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            originStrings.Add($"eSTG{InstanceId}.GPOS:99/00\r");

            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ProtrusionDetection, "0");
            rv101InputsOutputsControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ProtrusionDetection, "1");
            rv101InputsOutputsControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            originStrings.Add($"eSTG{InstanceId}.GPOS:00/00\r");
            originStrings.Add($"eSTG{InstanceId}.GPOS:00/01\r");

            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierClampCloseRight, "0");
            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierClampOpenRight, "1");
            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierClampCloseLeft, "0");
            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierClampOpenLeft, "1");
            rv101InputsOutputsControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ClampRightClose, "0");
            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ClampLeftClose, "0");
            rv101InputsOutputsControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ClampRightOpen, "1");
            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ClampLeftOpen, "1");
            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierClamp, "0");
            rv101InputsOutputsControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            loadPortStatusWordSpyControl1.SetStatus(Rv101Status.OperationStatus, "0");
            loadPortStatusWordSpyControl1.SetStatus(Rv101Status.OriginReturnCompletion, "1");
            loadPortStatusWordSpyControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            return originStrings;
        }

        private List<string> PerformUnclamp(int param1, int param2)
        {
            var unclampStrings = new List<string>();

            loadPortStatusWordSpyControl1.SetStatus(Rv101Status.OperationStatus, "1");
            loadPortStatusWordSpyControl1.UpdateStatus();
            unclampStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ClampMotionStart, "1");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ClampMovingDirection, "0");
            rv101InputsOutputsControl1.UpdateStatus();
            unclampStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ClampMotionStart, "0");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ClampMotionStart, "0");
            rv101InputsOutputsControl1.UpdateStatus();
            unclampStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.LaserStop, "1");
            //rv101InputsOutputsControl1.UpdateStatus();
            //unclampStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ProtrusionDetection, "0");
            rv101InputsOutputsControl1.UpdateStatus();
            unclampStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ProtrusionDetection, "1");
            rv101InputsOutputsControl1.UpdateStatus();
            unclampStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            unclampStrings.Add($"eSTG{InstanceId}.GPOS:00/01\r");

            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierClampCloseRight, "0");
            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierClampOpenRight, "1");
            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierClampCloseLeft, "0");
            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierClampOpenLeft, "1");
            //rv101InputsOutputsControl1.UpdateStatus();
            //unclampStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterOpenInput, "1");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterOpenOutput, "1");
            rv101InputsOutputsControl1.UpdateStatus();
            unclampStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            switch (param1)
            {
                case 0:
                    switch (param2)
                    {
                        case 0:
                            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CoverLock, "0");
                            break;
                        case 1:
                            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterCloseInput, "1");
                            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterCloseOutput, "1");

                            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterOpenInput, "0");
                            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterOpenOutput, "0");
                            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterOpenOutput2, "0");
                            break;
                    }
                    break;

                case 1:
                    rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CoverLock, "0");
                    break;

                case 2:
                    rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CoverLock, "0");

                    rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterCloseInput, "1");
                    rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterCloseOutput, "1");

                    rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterOpenInput, "0");
                    rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterOpenOutput, "0");
                    rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterOpenOutput2, "0");
                    break;

            }
            rv101InputsOutputsControl1.UpdateStatus();
            unclampStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");


            loadPortStatusWordSpyControl1.SetStatus(Rv101Status.OperationStatus, "0");
            loadPortStatusWordSpyControl1.UpdateStatus();
            unclampStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            return unclampStrings;
        }

        private List<string> PerformClamp(int param)
        {
            var clampStrings = new List<string>();

            loadPortStatusWordSpyControl1.SetStatus(Rv101Status.OperationStatus, "1");
            loadPortStatusWordSpyControl1.UpdateStatus();
            clampStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ClampMotionStart, "1");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ClampMovingDirection, "1");
            rv101InputsOutputsControl1.UpdateStatus();
            clampStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ClampMotionStart, "0");
            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ClampMotionStart, "0");
            rv101InputsOutputsControl1.UpdateStatus();
            clampStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ClampRightClose, "1");
            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ClampLeftClose, "1");
            //rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CarrierClamp, "1");
            //rv101InputsOutputsControl1.UpdateStatus();
            //clampStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ProtrusionDetection, "0");
            rv101InputsOutputsControl1.UpdateStatus();
            clampStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ProtrusionDetection, "1");
            rv101InputsOutputsControl1.UpdateStatus();
            clampStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            switch (param)
            {
                case 0:
                    rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterOpenInput, "1");
                    rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterOpenOutput, "1");
                    rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterOpenOutput2, "1");

                    rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterCloseInput, "0");
                    rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterCloseOutput, "0");
                    rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CoverLock, "1");
                    break;
                case 1:
                    rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.CoverLock, "1");
                    break;
                case 2:
                    rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterOpenInput, "1");
                    rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterOpenOutput, "1");
                    rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterOpenOutput2, "1");

                    rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterCloseInput, "0");
                    rv101InputsOutputsControl1.SetStatus(Rv101InputsOutputs.ShutterCloseOutput, "0");
                    break;
            }

            rv101InputsOutputsControl1.UpdateStatus();
            clampStrings.Add($"eSTG{InstanceId}.GPIO:{rv101InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            loadPortStatusWordSpyControl1.SetStatus(Rv101Status.OperationStatus, "0");
            loadPortStatusWordSpyControl1.UpdateStatus();
            clampStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            return clampStrings;
        }

        private List<string> PerformHome(byte slot)
        {
            var homeStrings = new List<string>();

            loadPortStatusWordSpyControl1.SetStatus(Rv101Status.OperationStatus, "1");
            loadPortStatusWordSpyControl1.UpdateStatus();
            homeStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            homeStrings.Add($"eSTG{InstanceId}.GPOS:020{slot:00}/01\r");

            loadPortStatusWordSpyControl1.SetStatus(Rv101Status.OperationStatus, "0");
            loadPortStatusWordSpyControl1.UpdateStatus();
            homeStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            return homeStrings;
        }

        #endregion
    }
}
