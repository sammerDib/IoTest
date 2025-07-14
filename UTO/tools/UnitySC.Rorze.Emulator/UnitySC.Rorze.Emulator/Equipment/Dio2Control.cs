using System.Collections.Generic;

using UnitySC.Rorze.Emulator.Common;
using UnitySC.Rorze.Emulator.Controls.Dio2;

namespace UnitySC.Rorze.Emulator.Equipment
{
    internal partial class Dio2Control : BaseEquipmentControl.BaseEquipmentControl
    {
        #region Constructor

        public Dio2Control()
        {
            InitializeComponent();

            dio2StatusWordSpyControl1.SetStatus(Dio2Status.OperationMode1, "1");
            dio2StatusWordSpyControl1.SetStatus(Dio2Status.OperationMode2, "1");
            dio2StatusWordSpyControl1.UpdateStatus();

            dio2InputsOutputsControl1.SetStatus(Dio2InputsOutputs.Pm1DoorOpened, "1");
            dio2InputsOutputsControl1.SetStatus(Dio2InputsOutputs.Pm1ReadyToLoadUnload, "1");
            dio2InputsOutputsControl1.UpdateStatus();

            dio2StatusWordSpyControl1.StatusChanged += Dio2StatusWordSpyControl1_StatusChanged;
            dio2InputsOutputsControl1.StatusChanged += Dio2InputsOutputsControl1_StatusChanged;
        }

        #endregion

        #region Overrides

        public override void Clean()
        {
            dio2StatusWordSpyControl1.StatusChanged -= Dio2StatusWordSpyControl1_StatusChanged;
            dio2InputsOutputsControl1.StatusChanged -= Dio2InputsOutputsControl1_StatusChanged;

            base.Clean();
        }

        protected override List<string> GetResponses(string toRespondeTo)
        {
            List<string> answers = new List<string>();
            if (!string.IsNullOrEmpty(toRespondeTo))
            {
                if (toRespondeTo.Contains(".STIM"))
                {
                    answers.Add("aDIO2.STIM:\r");
                }
                else if (toRespondeTo.Contains(".EVNT"))
                {
                    answers.Add("aDIO2.EVNT:\r");
                    var eventParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var evnt = int.Parse(eventParameters[0]);
                    var isActive = int.Parse(eventParameters[1]);
                    if (isActive == 1)
                    {
                        switch (evnt)
                        {
                            case 0:
                                answers.Add($"eDIO2.STAT:{dio2StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                                answers.Add($"eDIO2.GDIO:00/{dio2InputsOutputsControl1.GetConcatenatedStatuses()}\r");
                                break;
                            case 1:
                                answers.Add($"eDIO2.STAT:{dio2StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                                break;

                            case 2:
                                answers.Add($"eDIO2.GDIO:00/{dio2InputsOutputsControl1.GetConcatenatedStatuses()}\r");
                                break;
                        }
                    }
                }
                else if (toRespondeTo.Contains(".STAT"))
                {
                    answers.Add($"aDIO2.STAT:{dio2StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains("GDIO"))
                {
                    answers.Add($"aDIO2.GDIO:00/{dio2InputsOutputsControl1.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains(".RSTA"))
                {
                    answers.Add("aDIO2.RSTA:\r");

                    dio2StatusWordSpyControl1.SetStatus(Dio2Status.CommandProcessing, "1");
                    dio2StatusWordSpyControl1.UpdateStatus();
                    answers.Add($"eDIO2.STAT:{dio2StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

                    dio2StatusWordSpyControl1.SetStatus(Dio2Status.CommandProcessing, "0");
                    dio2StatusWordSpyControl1.UpdateStatus();
                    answers.Add($"eDIO2.STAT:{dio2StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains("SDOU"))
                {
                    answers.Add("aDIO2.SDOU:\r");

                    var sdouParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);

                    dio2InputsOutputsControl1.SetOutputsFromConcatenatedString(sdouParameters[1]);
                    dio2InputsOutputsControl1.UpdateStatus();
                    answers.Add($"eDIO2.GDIO:00/{dio2InputsOutputsControl1.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains("GVER"))
                {
                    answers.Add($"aDIO2.GVER:RORZE STD_DIO DIO2 Ver 1.00\r");
                }
            }

            return answers;
        }

        #endregion

        #region Event Handlers

        private void Dio2InputsOutputsControl1_StatusChanged(object sender, System.EventArgs e)
        {
            SendMessage($"eDIO2.GDIO:00/{dio2InputsOutputsControl1.GetConcatenatedStatuses()}\r");
        }

        private void Dio2StatusWordSpyControl1_StatusChanged(object sender, System.EventArgs e)
        {
            SendMessage($"eDIO2.STAT:{dio2StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
        }

        #endregion

        #region Public

        public void ManageDoor(int pmNumber, bool isOpen)
        {
            switch (pmNumber)
            {
                case 1:
                    if (dio2InputsOutputsControl1.GetStatus(Dio2InputsOutputs.Pm1DoorOpened)
                        != isOpen.ToString())
                    {
                        dio2InputsOutputsControl1.SetStatus(Dio2InputsOutputs.Pm1DoorOpened, isOpen ? "1" : "0");
                        dio2InputsOutputsControl1.SetStatus(Dio2InputsOutputs.Pm1ReadyToLoadUnload, isOpen ? "1" : "0");
                        dio2InputsOutputsControl1.UpdateStatus();
                        SendMessage($"eDIO2.GDIO:00/{dio2InputsOutputsControl1.GetConcatenatedStatuses()}\r");
                    }
                    break;
                case 2:
                    if (dio2InputsOutputsControl1.GetStatus(Dio2InputsOutputs.Pm2DoorOpened)
                        != isOpen.ToString())
                    {
                        dio2InputsOutputsControl1.SetStatus(Dio2InputsOutputs.Pm2DoorOpened, isOpen ? "1" : "0");
                        dio2InputsOutputsControl1.SetStatus(Dio2InputsOutputs.Pm2ReadyToLoadUnload, isOpen ? "1" : "0");
                        dio2InputsOutputsControl1.UpdateStatus();
                        SendMessage($"eDIO2.GDIO:00/{dio2InputsOutputsControl1.GetConcatenatedStatuses()}\r");
                    }
                    break;
                case 3:
                    if (dio2InputsOutputsControl1.GetStatus(Dio2InputsOutputs.Pm3DoorOpened)
                        != isOpen.ToString())
                    {
                        dio2InputsOutputsControl1.SetStatus(Dio2InputsOutputs.Pm3DoorOpened, isOpen ? "1" : "0");
                        dio2InputsOutputsControl1.SetStatus(Dio2InputsOutputs.Pm3ReadyToLoadUnload, isOpen ? "1" : "0");
                        dio2InputsOutputsControl1.UpdateStatus();
                        SendMessage($"eDIO2.GDIO:00/{dio2InputsOutputsControl1.GetConcatenatedStatuses()}\r");
                    }
                    break;
            }
        }
        #endregion
    }
}
