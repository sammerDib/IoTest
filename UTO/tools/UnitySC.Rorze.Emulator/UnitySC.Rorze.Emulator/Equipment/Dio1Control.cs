using System.Collections.Generic;

using UnitySC.Rorze.Emulator.Common;
using UnitySC.Rorze.Emulator.Controls.Dio1;

namespace UnitySC.Rorze.Emulator.Equipment
{
    internal partial class Dio1Control : BaseEquipmentControl.BaseEquipmentControl
    {
        #region Constructor

        public Dio1Control()
        {
            InitializeComponent();

            dio1StatusWordSpyControl1.SetStatus(Dio1Status.OperationMode1, "1");
            dio1StatusWordSpyControl1.SetStatus(Dio1Status.OperationMode2, "1");
            dio1StatusWordSpyControl1.UpdateStatus();

            dio1InputsOutputsControl1.SetStatus(Dio1InputsOutputs.DoorStatus,"1");
            dio1InputsOutputsControl1.SetStatus(Dio1InputsOutputs.PressureSensorVac,"1");
            dio1InputsOutputsControl1.SetStatus(Dio1InputsOutputs.PressureSensorAir,"1");
            dio1InputsOutputsControl1.UpdateStatus();

            dio1StatusWordSpyControl1.StatusChanged += Dio1StatusWordSpyControl1_StatusChanged;
            dio1InputsOutputsControl1.StatusChanged += Dio1InputsOutputsControl1_StatusChanged;
        }

        #endregion

        #region Overrides

        public override void Clean()
        {
            dio1StatusWordSpyControl1.StatusChanged -= Dio1StatusWordSpyControl1_StatusChanged;
            dio1InputsOutputsControl1.StatusChanged -= Dio1InputsOutputsControl1_StatusChanged;

            base.Clean();
        }

        protected override List<string> GetResponses(string toRespondeTo)
        {
            List<string> answers = new List<string>();
            if (!string.IsNullOrEmpty(toRespondeTo))
            {
                if (toRespondeTo.Contains(".STIM"))
                {
                    answers.Add("aDIO1.STIM:\r");
                }
                else if (toRespondeTo.Contains(".EVNT"))
                {
                    answers.Add("aDIO1.EVNT:\r");
                    var eventParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var evnt = int.Parse(eventParameters[0]);
                    var isActive = int.Parse(eventParameters[1]);
                    if (isActive == 1)
                    {
                        switch (evnt)
                        {
                            case 0:
                                answers.Add($"eDIO1.STAT:{dio1StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                                answers.Add($"eDIO1.GDIO:00/{dio1InputsOutputsControl1.GetConcatenatedStatuses()}\r");
                                break;
                            case 1:
                                answers.Add($"eDIO1.STAT:{dio1StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                                break;

                            case 2:
                                answers.Add($"eDIO1.GDIO:00/{dio1InputsOutputsControl1.GetConcatenatedStatuses()}\r");
                                break;
                        }
                    }
                }
                else if (toRespondeTo.Contains(".STAT"))
                {
                    answers.Add($"aDIO1.STAT:{dio1StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains("GDIO"))
                {
                    answers.Add($"aDIO1.GDIO:00/{dio1InputsOutputsControl1.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains(".RSTA"))
                {
                    answers.Add("aDIO1.RSTA:\r");

                    dio1StatusWordSpyControl1.SetStatus(Dio1Status.CommandProcessing, "1");
                    dio1StatusWordSpyControl1.UpdateStatus();
                    answers.Add($"eDIO1.STAT:{dio1StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

                    dio1StatusWordSpyControl1.SetStatus(Dio1Status.CommandProcessing, "0");
                    dio1StatusWordSpyControl1.UpdateStatus();
                    answers.Add($"eDIO1.STAT:{dio1StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains("SDOU"))
                {
                    answers.Add("aDIO1.SDOU:\r");
                    var sdouParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);

                    dio1InputsOutputsControl1.SetOutputsFromConcatenatedString(sdouParameters[1]);
                    dio1InputsOutputsControl1.UpdateStatus();
                    answers.Add($"eDIO1.GDIO:00/{dio1InputsOutputsControl1.GetConcatenatedStatuses()}\r");

                }
                else if (toRespondeTo.Contains("GVER"))
                {
                    answers.Add($"aDIO1.GVER:RORZE STD_DIO DIO1 Ver 1.00\r");
                }
            }

            return answers;
        }

        #endregion

        #region Event Handlers

        private void Dio1InputsOutputsControl1_StatusChanged(object sender, System.EventArgs e)
        {
            SendMessage($"eDIO1.GDIO:00/{dio1InputsOutputsControl1.GetConcatenatedStatuses()}\r");
        }

        private void Dio1StatusWordSpyControl1_StatusChanged(object sender, System.EventArgs e)
        {
            SendMessage($"eDIO1.STAT:{dio1StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
        }
        #endregion
    }
}
