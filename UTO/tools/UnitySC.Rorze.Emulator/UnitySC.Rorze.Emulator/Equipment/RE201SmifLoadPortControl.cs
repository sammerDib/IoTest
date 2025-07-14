using System;
using System.Collections.Generic;
using UnitySC.Rorze.Emulator.Controls.RE201;

namespace UnitySC.Rorze.Emulator.Equipment
{
    internal partial class Re201SmifLoadPortControl : BaseEquipmentControl.BaseEquipmentControl
    {
        #region Fields

        private string _internalMapping = "1010101010101";

        #endregion

        #region Constructor

        public Re201SmifLoadPortControl()
        {
            InitializeComponent();

            mappingTextBox.Text = _internalMapping;

            loadPortStatusWordSpyControl1.StatusChanged += LoadPortStatusWordSpyControl1_StatusChanged;
            rE201InputsOutputsControl1.StatusChanged += RE201InputsOutputsControl1_StatusChanged;
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

        private void RE201InputsOutputsControl1_StatusChanged(object sender, EventArgs e)
        {
            SendMessage($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");
        }

        private void MappingButton_Click(object sender, EventArgs e)
        {
            _internalMapping = mappingTextBox.Text;
        }

        private void FoupPresentPlacedButton_Click(object sender, EventArgs e)
        {
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierPresenceMiddle, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierPresenceLeft, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierPresenceRight, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ProtrusionDetection, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierProperPlaced, "1");
            rE201InputsOutputsControl1.UpdateStatus();
            SendMessage($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");
        }

        private void FoupPresentNoPlacedButton_Click(object sender, EventArgs e)
        {
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierPresenceMiddle, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierPresenceLeft, "0");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierPresenceRight, "0");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ProtrusionDetection, "0");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierProperPlaced, "0");
            rE201InputsOutputsControl1.UpdateStatus();
            SendMessage($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");
        }

        private void FoupNoPresentPlacedButton_Click(object sender, EventArgs e)
        {
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierPresenceMiddle, "0");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierPresenceLeft, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierPresenceRight, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ProtrusionDetection, "0");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierProperPlaced, "0");
            rE201InputsOutputsControl1.UpdateStatus();
            SendMessage($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

        }

        private void FoupRemovedButton_Click(object sender, EventArgs e)
        {
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierPresenceMiddle, "0");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierPresenceLeft, "0");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierPresenceRight, "0");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ProtrusionDetection, "0");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierProperPlaced, "0");
            rE201InputsOutputsControl1.UpdateStatus();
            SendMessage($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");
        }

        #endregion

        #region Overrides

        public override void Clean()
        {
            loadPortStatusWordSpyControl1.StatusChanged -= LoadPortStatusWordSpyControl1_StatusChanged;
            rE201InputsOutputsControl1.StatusChanged -= RE201InputsOutputsControl1_StatusChanged;

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
                    answers.Add($"aSTG{InstanceId}.GWID:CST1\r");
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
                    answers.Add($"aSTG{InstanceId}.UCLM:\r");
                    answers.AddRange(PerformUnclamp());
                }
                else if (toRespondeTo.Contains(".CLMP"))
                {
                    answers.Add($"aSTG{InstanceId}.CLMP:\r");
                    answers.AddRange(PerformClamp());
                }
                else if (toRespondeTo.Contains(".GMAP"))
                {
                    answers.Add($"aSTG{InstanceId}.GMAP:{_internalMapping}\r");
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
                            answers.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");
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
                    answers.Add($"aSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains(".GPOS"))
                {
                    answers.Add($"aSTG{InstanceId}.GPOS:02005/01\r");
                }
                else if (toRespondeTo.Contains(".DPRM.GTDT"))
                {
                    answers.Add($"aSTG{InstanceId}.DPRM.GTDT:13\r");
                }
                else if (toRespondeTo.Contains("GVER"))
                {
                    answers.Add($"aSTG{InstanceId}.GVER:RORZE STD_STG RE201 Ver 1.00\r");
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
            var initStrings = new List<string>();

            initStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.MotionProhibited, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.LaserStop, "1");
            rE201InputsOutputsControl1.UpdateStatus();
            initStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierOpen, "0");
            rE201InputsOutputsControl1.UpdateStatus();
            initStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");


            initStrings.Add($"eSTG{InstanceId}.GPOS:99/99\r");
            initStrings.Add($"eSTG{InstanceId}.GWID:AUTO/----\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.MotionProhibited, "0");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ClampRightOpen, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ClampLeftOpen, "1");
            rE201InputsOutputsControl1.UpdateStatus();
            initStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");


            initStrings.Add($"eSTG{InstanceId}.GWID:AUTO/CST1\r");

            loadPortStatusWordSpyControl1.SetStatus(Re201Status.OperationMode, "1");
            loadPortStatusWordSpyControl1.UpdateStatus();
            initStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");


            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.PreparationCompleted, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.PreparationCompletedSignalNotConnected, "1");
            initStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            loadPortStatusWordSpyControl1.SetStatus(Re201Status.OperationMode, "1");
            loadPortStatusWordSpyControl1.UpdateStatus();
            initStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            return initStrings;
        }

        private List<string> PerformOrigin()
        {
            var originStrings = new List<string>();

            loadPortStatusWordSpyControl1.SetStatus(Re201Status.OperationStatus, "1");
            loadPortStatusWordSpyControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierClampCloseRight, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierClampCloseLeft, "1");
            rE201InputsOutputsControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ClampRightOpen, "0");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ClampLeftOpen, "0");
            rE201InputsOutputsControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ClampRightClose, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ClampLeftClose, "1");
            rE201InputsOutputsControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            originStrings.Add($"eSTG{InstanceId}.GPOS:99/00\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ProtrusionDetection, "0");
            rE201InputsOutputsControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ProtrusionDetection, "1");
            rE201InputsOutputsControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            originStrings.Add($"eSTG{InstanceId}.GPOS:00/00\r");
            originStrings.Add($"eSTG{InstanceId}.GPOS:00/01\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierClampCloseRight, "0");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierClampOpenRight, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierClampCloseLeft, "0");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierClampOpenLeft, "1");
            rE201InputsOutputsControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ClampRightClose, "0");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ClampLeftClose, "0");
            rE201InputsOutputsControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ClampRightOpen, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ClampLeftOpen, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierClamp, "0");
            rE201InputsOutputsControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            loadPortStatusWordSpyControl1.SetStatus(Re201Status.OperationStatus, "0");
            loadPortStatusWordSpyControl1.SetStatus(Re201Status.OriginReturnCompletion, "1");
            loadPortStatusWordSpyControl1.UpdateStatus();
            originStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            return originStrings;
        }

        private List<string> PerformUnclamp()
        {
            var unclampStrings = new List<string>();

            loadPortStatusWordSpyControl1.SetStatus(Re201Status.OperationStatus, "1");
            loadPortStatusWordSpyControl1.UpdateStatus();
            unclampStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierOpen, "0");
            rE201InputsOutputsControl1.UpdateStatus();
            unclampStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.LaserStop, "1");
            rE201InputsOutputsControl1.UpdateStatus();
            unclampStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ProtrusionDetection, "0");
            rE201InputsOutputsControl1.UpdateStatus();
            unclampStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ProtrusionDetection, "1");
            rE201InputsOutputsControl1.UpdateStatus();
            unclampStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            unclampStrings.Add($"eSTG{InstanceId}.GPOS:00/01\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierClampCloseRight, "0");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierClampOpenRight, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierClampCloseLeft, "0");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierClampOpenLeft, "1");
            rE201InputsOutputsControl1.UpdateStatus();
            unclampStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ClampRightClose, "0");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ClampLeftClose, "0");
            rE201InputsOutputsControl1.UpdateStatus();
            unclampStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ClampRightOpen, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ClampLeftOpen, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierClamp, "0");
            rE201InputsOutputsControl1.UpdateStatus();
            unclampStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            loadPortStatusWordSpyControl1.SetStatus(Re201Status.OperationStatus, "0");
            loadPortStatusWordSpyControl1.UpdateStatus();
            unclampStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            return unclampStrings;
        }

        private List<string> PerformClamp()
        {
            var clampStrings = new List<string>();

            loadPortStatusWordSpyControl1.SetStatus(Re201Status.OperationStatus, "1");
            loadPortStatusWordSpyControl1.UpdateStatus();
            clampStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierClampCloseRight, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierClampCloseLeft, "1");
            rE201InputsOutputsControl1.UpdateStatus();
            clampStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ClampRightOpen, "0");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ClampLeftOpen, "0");
            rE201InputsOutputsControl1.UpdateStatus();
            clampStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ClampRightClose, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ClampLeftClose, "1");
            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierClamp, "1");
            rE201InputsOutputsControl1.UpdateStatus();
            clampStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ProtrusionDetection, "0");
            rE201InputsOutputsControl1.UpdateStatus();
            clampStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.ProtrusionDetection, "1");
            rE201InputsOutputsControl1.UpdateStatus();
            clampStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.SubstrateDetection, "1");
            rE201InputsOutputsControl1.UpdateStatus();
            clampStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rE201InputsOutputsControl1.SetStatus(Re201InputsOutputs.CarrierOpen, "1");
            rE201InputsOutputsControl1.UpdateStatus();
            clampStrings.Add($"eSTG{InstanceId}.GPIO:{rE201InputsOutputsControl1.GetConcatenatedStatuses()}\r");

            loadPortStatusWordSpyControl1.SetStatus(Re201Status.OperationStatus, "0");
            loadPortStatusWordSpyControl1.UpdateStatus();
            clampStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            return clampStrings;
        }

        private List<string> PerformHome(byte slot)
        {
            var homeStrings = new List<string>();

            loadPortStatusWordSpyControl1.SetStatus(Re201Status.OperationStatus, "1");
            loadPortStatusWordSpyControl1.UpdateStatus();
            homeStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            homeStrings.Add($"eSTG{InstanceId}.GPOS:020{slot:00}/01\r");

            loadPortStatusWordSpyControl1.SetStatus(Re201Status.OperationStatus, "0");
            loadPortStatusWordSpyControl1.UpdateStatus();
            homeStrings.Add($"eSTG{InstanceId}.STAT:{loadPortStatusWordSpyControl1.GetConcatenatedStatuses()}\r");

            return homeStrings;
        }

        #endregion
    }
}
