using System.Collections.Generic;
using System.Text;

using UnitySC.Rorze.Emulator.Common;
using UnitySC.Rorze.Emulator.Controls.Dio0;

namespace UnitySC.Rorze.Emulator.Equipment
{
    internal partial class Dio0Control : BaseEquipmentControl.BaseEquipmentControl
    {
        #region Fields

        private int _fan1Speed;
        private int _fan2Speed;

        private int _pressure = 0;

        #endregion

        #region Constructor

        public Dio0Control()
        {
            InitializeComponent();

            dio0StatusWordSpyControl1.SetStatus(Dio0Status.OperationMode1, "1");
            dio0StatusWordSpyControl1.SetStatus(Dio0Status.OperationMode2, "1");
            dio0StatusWordSpyControl1.UpdateStatus();

            dio0StatusWordSpyControl1.StatusChanged += Dio0StatusWordSpyControl1_StatusChanged;
            dio0InputsOutputsControl1.StatusChanged += Dio0InputsOutputsControl1_StatusChanged;
            dio0FanDetectionControl1.StatusChanged += Dio0FanDetectionControl1_StatusChanged;
            dio0LayingPlanLoadPortControl1.StatusChanged += Dio0LayingPlanLoadPortControl1_StatusChanged;
            dio0LayingPlanLoadPortControl2.StatusChanged += Dio0LayingPlanLoadPortControl2_StatusChanged;
            dio0LayingPlanLoadPortControl3.StatusChanged += Dio0LayingPlanLoadPortControl3_StatusChanged;
            dio0LayingPlanLoadPortControl4.StatusChanged += Dio0LayingPlanLoadPortControl4_StatusChanged;
            dio0E84LpModule1Control1.StatusChanged += Dio0E84LpModule1Control1_StatusChanged;
            dio0E84LpModule1Control2.StatusChanged += Dio0E84LpModule1Control2_StatusChanged;
            dio0E84LpModule2Control1.StatusChanged += Dio0E84LpModule2Control1_StatusChanged;
            dio0E84LpModule2Control2.StatusChanged += Dio0E84LpModule2Control2_StatusChanged;
        }

        #endregion

        #region Overrides

        public override void Clean()
        {
            dio0StatusWordSpyControl1.StatusChanged -= Dio0StatusWordSpyControl1_StatusChanged;
            dio0InputsOutputsControl1.StatusChanged -= Dio0InputsOutputsControl1_StatusChanged;
            dio0FanDetectionControl1.StatusChanged -= Dio0FanDetectionControl1_StatusChanged;
            dio0LayingPlanLoadPortControl1.StatusChanged -= Dio0LayingPlanLoadPortControl1_StatusChanged;
            dio0LayingPlanLoadPortControl2.StatusChanged -= Dio0LayingPlanLoadPortControl2_StatusChanged;
            dio0LayingPlanLoadPortControl3.StatusChanged -= Dio0LayingPlanLoadPortControl3_StatusChanged;
            dio0LayingPlanLoadPortControl4.StatusChanged -= Dio0LayingPlanLoadPortControl4_StatusChanged;
            dio0E84LpModule1Control1.StatusChanged -= Dio0E84LpModule1Control1_StatusChanged;
            dio0E84LpModule1Control2.StatusChanged -= Dio0E84LpModule1Control2_StatusChanged;
            dio0E84LpModule2Control1.StatusChanged -= Dio0E84LpModule2Control1_StatusChanged;
            dio0E84LpModule2Control2.StatusChanged -= Dio0E84LpModule2Control2_StatusChanged;

            base.Clean();
        }

        protected override List<string> GetResponses(string toRespondeTo)
        {
            List<string> answers = new List<string>();
            if (!string.IsNullOrEmpty(toRespondeTo))
            {
                if (toRespondeTo.Contains(".STIM"))
                {
                    answers.Add("aDIO0.STIM:\r");
                }
                else if (toRespondeTo.Contains(".EVNT"))
                {
                    answers.Add("aDIO0.EVNT:\r");
                    var eventParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var evnt = int.Parse(eventParameters[0]);
                    var isActive = int.Parse(eventParameters[1]);
                    if (isActive == 1)
                    {
                        switch (evnt)
                        {
                            case 0:
                                answers.Add($"eDIO0.STAT:{dio0StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                                answers.Add($"eDIO0.GPIO:{dio0InputsOutputsControl1.GetConcatenatedStatuses()}\r");
                                answers.Add($"eDIO0.GDIO:008/{dio0FanDetectionControl1.GetConcatenatedStatuses()}\r");
                                answers.Add($"eDIO0.GDIO:004/{dio0LayingPlanLoadPortControl1.GetConcatenatedStatuses()}\r");
                                answers.Add($"eDIO0.GDIO:005/{dio0LayingPlanLoadPortControl2.GetConcatenatedStatuses()}\r");
                                answers.Add($"eDIO0.GDIO:006/{dio0LayingPlanLoadPortControl3.GetConcatenatedStatuses()}\r");
                                answers.Add($"eDIO0.GDIO:007/{dio0LayingPlanLoadPortControl4.GetConcatenatedStatuses()}\r");
                                answers.Add(
                                    $"eDIO0.GDIO:400/{dio0E84LpModule1Control1.GetConcatenatedStatuses()}/{dio0E84LpModule2Control1.GetConcatenatedStatuses()}/{dio0E84LpModule1Control2.GetConcatenatedStatuses()}/{dio0E84LpModule2Control2.GetConcatenatedStatuses()}\r");
                                answers.Add($"eDIO0.GREV:01|+{_fan1Speed:000000000}/02|+{_fan2Speed:00000000}\r");
                                answers.Add($"eDIO0.GPRS:01|-{_pressure:0000000}\r");
                                break;
                            case 1:
                                answers.Add($"eDIO0.STAT:{dio0StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                                break;

                            case 2:
                                answers.Add($"eDIO0.GPIO:{dio0InputsOutputsControl1.GetConcatenatedStatuses()}\r");
                                break;

                            case 3:
                                answers.Add(
                                    $"eDIO0.GDIO:400/{dio0E84LpModule1Control1.GetConcatenatedStatuses()}/{dio0E84LpModule2Control1.GetConcatenatedStatuses()}/{dio0E84LpModule1Control2.GetConcatenatedStatuses()}/{dio0E84LpModule2Control2.GetConcatenatedStatuses()}\r");
                                break;

                            case 4:
                                answers.Add($"eDIO0.GREV:01|+{_fan1Speed:000000000}/02|+{_fan2Speed:00000000}\r");
                                break;

                            case 5:
                                answers.Add($"eDIO0.GPRS:01|-{_pressure:0000000}\r");
                                break;
                        }
                    }
                }
                else if (toRespondeTo.Contains(".STAT"))
                {
                    answers.Add($"aDIO0.STAT:{dio0StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains(".GPIO"))
                {
                    answers.Add($"aDIO0.GPIO:{dio0InputsOutputsControl1.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains("GDIO"))
                {
                    var gdioParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var ioModule = gdioParameters[0];
                    switch (ioModule)
                    {
                        case "004":
                            answers.Add($"aDIO0.GDIO:004/{dio0LayingPlanLoadPortControl1.GetConcatenatedStatuses()}\r");
                            break;

                        case "005":
                            answers.Add($"aDIO0.GDIO:005/{dio0LayingPlanLoadPortControl2.GetConcatenatedStatuses()}\r");
                            break;

                        case "006":
                            answers.Add($"aDIO0.GDIO:006/{dio0LayingPlanLoadPortControl3.GetConcatenatedStatuses()}\r");
                            break;

                        case "007":
                            answers.Add($"aDIO0.GDIO:007/{dio0LayingPlanLoadPortControl4.GetConcatenatedStatuses()}\r");
                            break;

                        case "008":
                            answers.Add($"aDIO0.GDIO:008/{dio0FanDetectionControl1.GetConcatenatedStatuses()}\r");
                            break;

                        case "400":
                            answers.Add($"aDIO0.GDIO:400/{dio0E84LpModule1Control1.GetConcatenatedStatuses()}\r");
                            break;

                        case "401":
                            answers.Add($"aDIO0.GDIO:401/{dio0E84LpModule2Control1.GetConcatenatedStatuses()}\r");
                            break;

                        case "402":
                            answers.Add($"aDIO0.GDIO:402/{dio0E84LpModule1Control2.GetConcatenatedStatuses()}\r");
                            break;

                        case "403":
                            answers.Add($"aDIO0.GDIO:403/{dio0E84LpModule2Control2.GetConcatenatedStatuses()}\r");
                            break;
                    }
                }
                else if (toRespondeTo.Contains("GREV"))
                {
                    answers.Add($"aDIO0.GREV:01|+{_fan1Speed:000000000}/02|+{_fan2Speed:00000000}\r");
                }
                else if (toRespondeTo.Contains("GPRS"))
                {
                    answers.Add($"aDIO0.GPRS:01|-{_pressure:0000000}\r");
                }
                else if (toRespondeTo.Contains(".RSTA"))
                {
                    answers.Add("aDIO0.RSTA:\r");

                    dio0StatusWordSpyControl1.SetStatus(Dio0Status.CommandProcessing, "1");
                    dio0StatusWordSpyControl1.UpdateStatus();
                    answers.Add($"eDIO0.STAT:{dio0StatusWordSpyControl1.GetConcatenatedStatuses()}\r");

                    dio0StatusWordSpyControl1.SetStatus(Dio0Status.CommandProcessing, "0");
                    dio0StatusWordSpyControl1.UpdateStatus();
                    answers.Add($"eDIO0.STAT:{dio0StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains("MOVE"))
                {
                    answers.Add("aDIO0.MOVE:\r");
                    var moveParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    switch (moveParameters[0])
                    {
                        case "0":
                            //All fans
                            _fan1Speed = int.Parse(moveParameters[1]);
                            _fan2Speed = int.Parse(moveParameters[1]);
                            break;

                        case "1":
                            _fan1Speed = int.Parse(moveParameters[1]);
                            break;

                        case "2":
                            _fan2Speed = int.Parse(moveParameters[1]);
                            break;
                    }
                    answers.Add($"eDIO0.GREV:01|+{_fan1Speed:000000000}/02|+{_fan2Speed:00000000}\r");
                }
                else if (toRespondeTo.Contains("SDOU"))
                {
                    answers.Add("aDIO0.SDOU:\r");
                    var sdouParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var moduleId = int.Parse(sdouParameters[0]);
                    var answer = new StringBuilder();
                    answer.Append($"eDIO0.GDIO:{moduleId}");
                    for (int iParameter = 1; iParameter < sdouParameters.Length; iParameter++)
                    {
                        switch (moduleId)
                        {
                            case 400:
                                dio0E84LpModule1Control1.SetOutputsFromConcatenatedString(sdouParameters[iParameter]);
                                dio0E84LpModule1Control1.UpdateStatus();
                                answer.Append($"/{dio0E84LpModule1Control1.GetConcatenatedStatuses()}");
                                break;

                            case 401:
                                dio0E84LpModule2Control1.SetOutputsFromConcatenatedString(sdouParameters[iParameter]);
                                dio0E84LpModule1Control1.UpdateStatus();
                                answer.Append($"/{dio0E84LpModule2Control1.GetConcatenatedStatuses()}");
                                break;

                            case 402:
                                dio0E84LpModule1Control2.SetOutputsFromConcatenatedString(sdouParameters[iParameter]);
                                dio0E84LpModule1Control1.UpdateStatus();
                                answer.Append($"/{dio0E84LpModule1Control2.GetConcatenatedStatuses()}");
                                break;

                            case 403:
                                dio0E84LpModule2Control2.SetOutputsFromConcatenatedString(sdouParameters[iParameter]);
                                dio0E84LpModule1Control1.UpdateStatus();
                                answer.Append($"/{dio0E84LpModule2Control2.GetConcatenatedStatuses()}");
                                break;
                        }

                        moduleId++;
                    }

                    answer.Append("\r");
                    answers.Add(answer.ToString());
                }
                else if (toRespondeTo.Contains("STOP"))
                {
                    answers.Add("aDIO0.STOP:\r");
                    var stopParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    switch (stopParameters[0])
                    {
                        case "0":
                            //All fans
                            _fan1Speed = 0;
                            _fan2Speed = 0;
                            break;

                        case "1":
                            _fan1Speed = 0;
                            break;

                        case "2":
                            _fan2Speed = 0;
                            break;
                    }
                    answers.Add($"eDIO0.GREV:01|+{_fan1Speed:000000000}/02|+{_fan2Speed:00000000}\r");
                }
                else if (toRespondeTo.Contains("GVER"))
                {
                    answers.Add($"aDIO0.GVER:RORZE STD_DIO DIO0 Ver 1.00\r");
                }
            }

            return answers;
        }

        protected override void SendConnectedMessage()
        {
            SendMessage("eDIO0.CNCT:");
        }

        #endregion

        #region Event Handlers

        private void Dio0InputsOutputsControl1_StatusChanged(object sender, System.EventArgs e)
        {
            SendMessage($"eDIO0.GPIO:{dio0InputsOutputsControl1.GetConcatenatedStatuses()}\r");
        }

        private void Dio0StatusWordSpyControl1_StatusChanged(object sender, System.EventArgs e)
        {
            SendMessage($"eDIO0.STAT:{dio0StatusWordSpyControl1.GetConcatenatedStatuses()}\r");
        }

        private void Dio0FanDetectionControl1_StatusChanged(object sender, System.EventArgs e)
        {
            SendMessage($"eDIO1.GDIO:008/{dio0FanDetectionControl1.GetConcatenatedStatuses()}\r");
        }

        private void Dio0E84LpModule1Control1_StatusChanged(object sender, System.EventArgs e)
        {
            SendMessage($"eDIO1.GDIO:400/{dio0E84LpModule1Control1.GetConcatenatedStatuses()}\r");
        }

        private void Dio0E84LpModule1Control2_StatusChanged(object sender, System.EventArgs e)
        {
            SendMessage($"eDIO1.GDIO:402/{dio0E84LpModule1Control2.GetConcatenatedStatuses()}\r");
        }

        private void Dio0E84LpModule2Control1_StatusChanged(object sender, System.EventArgs e)
        {
            SendMessage($"eDIO1.GDIO:401/{dio0E84LpModule2Control1.GetConcatenatedStatuses()}\r");
        }

        private void Dio0E84LpModule2Control2_StatusChanged(object sender, System.EventArgs e)
        {
            SendMessage($"eDIO1.GDIO:403/{dio0E84LpModule2Control2.GetConcatenatedStatuses()}\r");
        }

        private void Dio0LayingPlanLoadPortControl4_StatusChanged(object sender, System.EventArgs e)
        {
            SendMessage(
                $"eDIO0.GDIO:007/{dio0LayingPlanLoadPortControl4.GetConcatenatedStatuses()}\r");
        }

        private void Dio0LayingPlanLoadPortControl3_StatusChanged(object sender, System.EventArgs e)
        {
            SendMessage(
                $"eDIO0.GDIO:006/{dio0LayingPlanLoadPortControl3.GetConcatenatedStatuses()}\r");
        }

        private void Dio0LayingPlanLoadPortControl2_StatusChanged(object sender, System.EventArgs e)
        {
            SendMessage(
                $"eDIO0.GDIO:005/{dio0LayingPlanLoadPortControl2.GetConcatenatedStatuses()}\r");
        }

        private void Dio0LayingPlanLoadPortControl1_StatusChanged(object sender, System.EventArgs e)
        {
            SendMessage(
                $"eDIO0.GDIO:004/{dio0LayingPlanLoadPortControl1.GetConcatenatedStatuses()}\r");
        }

        #endregion
    }
}
