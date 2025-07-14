using System.Collections;
using System.Text;

using UnitySC.Rorze.Emulator.Common;

namespace UnitySC.Rorze.Emulator.Controls.Dio0
{
    public enum Dio0LayingPlanLoadPortInputsOutputs
    {
        PlacementSensorA,
        PlacementSensorB,
        PlacementSensorC,
        PlacementSensorD,
        WaferProtrudeSensor1,
        WaferProtrudeSensor2,
        WaferProtrudeSensor3
    }

    public partial class Dio0LayingPlanLoadPortControl : InputsOutputsControl
    {
        public Dio0LayingPlanLoadPortControl()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            paramDataGridView.Rows.Clear();

            paramDataGridView.Rows.Add(
                1,
                Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorA,
                "0: False/1: True",
                "1");
            paramDataGridView.Rows.Add(
                2,
                Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorB,
                "0: False/1: True",
                "1");
            paramDataGridView.Rows.Add(
                3,
                Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorC,
                "0: False/1: True",
                "1");
            paramDataGridView.Rows.Add(
                4,
                Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorD,
                "0: False/1: True",
                "1");
            paramDataGridView.Rows.Add(
                5,
                Dio0LayingPlanLoadPortInputsOutputs.WaferProtrudeSensor1,
                "0: False/1: True",
                "1");
            paramDataGridView.Rows.Add(
                6,
                Dio0LayingPlanLoadPortInputsOutputs.WaferProtrudeSensor2,
                "0: False/1: True",
                "1");
            paramDataGridView.Rows.Add(
                7,
                Dio0LayingPlanLoadPortInputsOutputs.WaferProtrudeSensor3,
                "0: False/1: True",
                "1");

            cbWaferSize.SelectedIndex = 0;
        }

        public string GetStatus(Dio0LayingPlanLoadPortInputsOutputs status)
        {
            return GetStatus((int)status);
        }

        public void SetStatus(Dio0LayingPlanLoadPortInputsOutputs status, string value)
        {
            SetStatus((int)status, value);
        }

        public override string GetConcatenatedStatuses()
        {
            var stringBuilder = new StringBuilder();

            var inputs = new BitArray(
                new[]
                {
                    GetStatus(Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorA) == "1",
                    GetStatus(Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorB) == "1",
                    GetStatus(Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorC) == "1",
                    GetStatus(Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorD) == "1",
                    GetStatus(Dio0LayingPlanLoadPortInputsOutputs.WaferProtrudeSensor1) == "1",
                    GetStatus(Dio0LayingPlanLoadPortInputsOutputs.WaferProtrudeSensor2) == "1",
                    GetStatus(Dio0LayingPlanLoadPortInputsOutputs.WaferProtrudeSensor3) == "1",
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                });

            var outputs = new BitArray(
                new[]
                {
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false
                });

            stringBuilder.Append(BitArrayToHexaString(outputs));
            stringBuilder.Append(BitArrayToHexaString(inputs));

            return stringBuilder.ToString();
        }

        public void SetOutputsFromConcatenatedString(string concatenatedStatus)
        {
            //No mask defined
            if (concatenatedStatus.Length == 4)
            {
                var outputs = new BitArray(RorzeHelpers.StringToByteArray(concatenatedStatus));
            }
            else
            {
                var status = concatenatedStatus.Split('/');
                var value = new BitArray(RorzeHelpers.StringToByteArray(status[0]));
                var mask = new BitArray(RorzeHelpers.StringToByteArray(status[0]));

                for (var iMask = 0; iMask < mask.Length; iMask++)
                {
                    if (!mask[iMask])
                    {
                        continue;
                    }

                    switch (iMask)
                    {
                        case 0:
                            SetStatus(
                                Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorA,
                                value[iMask]
                                    ? "1"
                                    : "0");
                            break;
                        case 1:
                            SetStatus(
                                Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorB,
                                value[iMask]
                                    ? "1"
                                    : "0");
                            break;
                        case 2:
                            SetStatus(
                                Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorC,
                                value[iMask]
                                    ? "1"
                                    : "0");
                            break;
                        case 3:
                            SetStatus(
                                Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorD,
                                value[iMask]
                                    ? "1"
                                    : "0");
                            break;
                        case 4:
                            SetStatus(
                                Dio0LayingPlanLoadPortInputsOutputs.WaferProtrudeSensor1,
                                value[iMask]
                                    ? "1"
                                    : "0");
                            break;
                        case 5:
                            SetStatus(
                                Dio0LayingPlanLoadPortInputsOutputs.WaferProtrudeSensor2,
                                value[iMask]
                                    ? "1"
                                    : "0");
                            break;
                        case 6:
                            SetStatus(
                                Dio0LayingPlanLoadPortInputsOutputs.WaferProtrudeSensor3,
                                value[iMask]
                                    ? "1"
                                    : "0");
                            break;
                    }
                }
            }
        }

        private void foupPresentPlacedButton_Click(object sender, System.EventArgs e)
        {
            switch ((string)cbWaferSize.SelectedItem)
            {
                case "200mm":
                    SetStatus(Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorC, "0");
                    SetStatus(Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorD, "0");
                    if (cbProtrusionError.Checked)
                    {
                        SetStatus(Dio0LayingPlanLoadPortInputsOutputs.WaferProtrudeSensor3, "0");
                    }
                    break;

                case "150mm":
                    SetStatus(Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorA, "0");
                    SetStatus(Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorC, "0");
                    if (cbProtrusionError.Checked)
                    {
                        SetStatus(Dio0LayingPlanLoadPortInputsOutputs.WaferProtrudeSensor2, "0");
                        SetStatus(Dio0LayingPlanLoadPortInputsOutputs.WaferProtrudeSensor3, "0");
                    }
                    break;

                case "100mm":
                    SetStatus(Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorB, "0");
                    SetStatus(Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorC, "0");
                    if (cbProtrusionError.Checked)
                    {
                        SetStatus(Dio0LayingPlanLoadPortInputsOutputs.WaferProtrudeSensor1, "0");
                        SetStatus(Dio0LayingPlanLoadPortInputsOutputs.WaferProtrudeSensor2, "0");
                        SetStatus(Dio0LayingPlanLoadPortInputsOutputs.WaferProtrudeSensor3, "0");
                    }
                    break;
            }

            UpdateStatus();
        }

        private void foupRemovedButton_Click(object sender, System.EventArgs e)
        {
            SetStatus(Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorA, "1");
            SetStatus(Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorB, "1");
            SetStatus(Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorC, "1");
            SetStatus(Dio0LayingPlanLoadPortInputsOutputs.PlacementSensorD, "1");
            SetStatus(Dio0LayingPlanLoadPortInputsOutputs.WaferProtrudeSensor1, "1");
            SetStatus(Dio0LayingPlanLoadPortInputsOutputs.WaferProtrudeSensor2, "1");
            SetStatus(Dio0LayingPlanLoadPortInputsOutputs.WaferProtrudeSensor3, "1");
            UpdateStatus();
        }
    }
}
