using System.Collections;
using System.Linq;
using System.Text;

using UnitySC.Rorze.Emulator.Common;

namespace UnitySC.Rorze.Emulator.Controls.Dio2
{
    public enum Dio2InputsOutputs
    {
        Pm1DoorOpened,
        Pm1ReadyToLoadUnload,
        Pm2DoorOpened,
        Pm2ReadyToLoadUnload,
        Pm3DoorOpened,
        Pm3ReadyToLoadUnload,
        RobotArmNotExtendedInPm1,
        RobotArmNotExtendedInPm2,
        RobotArmNotExtendedInPm3,
    }

    public partial class Dio2InputsOutputsControl : InputsOutputsControl
    {
        public Dio2InputsOutputsControl()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            paramDataGridView.Rows.Clear();

            paramDataGridView.Rows.Add(1, Dio2InputsOutputs.Pm1DoorOpened, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(2, Dio2InputsOutputs.Pm1ReadyToLoadUnload, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(3, Dio2InputsOutputs.Pm2DoorOpened, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(4, Dio2InputsOutputs.Pm2ReadyToLoadUnload, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(5, Dio2InputsOutputs.Pm3DoorOpened, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(6, Dio2InputsOutputs.Pm3ReadyToLoadUnload, "0: False/1: True", "0");

            paramDataGridView.Rows.Add(7, Dio2InputsOutputs.RobotArmNotExtendedInPm1, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(8, Dio2InputsOutputs.RobotArmNotExtendedInPm2, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(9, Dio2InputsOutputs.RobotArmNotExtendedInPm3, "0: False/1: True", "0");
        }

        public string GetStatus(Dio2InputsOutputs status)
        {
            return GetStatus((int)status);
        }

        public void SetStatus(Dio2InputsOutputs status, string value)
        {
            SetStatus((int)status, value);
        }

        public override string GetConcatenatedStatuses()
        {
            StringBuilder stringBuilder = new StringBuilder();

            BitArray inputs = new BitArray(new[]
            {
                GetStatus(Dio2InputsOutputs.Pm1DoorOpened) == "1",
                GetStatus(Dio2InputsOutputs.Pm1ReadyToLoadUnload) == "1",
                GetStatus(Dio2InputsOutputs.Pm2DoorOpened) == "1",
                GetStatus(Dio2InputsOutputs.Pm2ReadyToLoadUnload) == "1",
                GetStatus(Dio2InputsOutputs.Pm3DoorOpened) == "1",
                GetStatus(Dio2InputsOutputs.Pm3ReadyToLoadUnload) == "1",
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

            BitArray outputs = new BitArray(new[]
            {
                GetStatus(Dio2InputsOutputs.RobotArmNotExtendedInPm1) == "1",
                GetStatus(Dio2InputsOutputs.RobotArmNotExtendedInPm2) == "1",
                GetStatus(Dio2InputsOutputs.RobotArmNotExtendedInPm3) == "1",
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

            stringBuilder.Append(BitArrayToHexaString(inputs));
            stringBuilder.Append(BitArrayToHexaString(outputs));

            return stringBuilder.ToString();
        }

        public void SetOutputsFromConcatenatedString(string concatenatedStatus)
        {
            //No mask defined
            if (concatenatedStatus.Length == 4)
            {
                BitArray outputs = new BitArray(RorzeHelpers.StringToByteArray(concatenatedStatus));

                SetStatus(Dio2InputsOutputs.RobotArmNotExtendedInPm1, outputs[0] ? "1" : "0");
                SetStatus(Dio2InputsOutputs.RobotArmNotExtendedInPm2, outputs[1] ? "1" : "0");
                SetStatus(Dio2InputsOutputs.RobotArmNotExtendedInPm3, outputs[2] ? "1" : "0");
            }
            else
            {
                var status = concatenatedStatus.Split('/');
                var value = new BitArray(RorzeHelpers.StringToByteArray(status[0]).Reverse().ToArray());
                var mask = new BitArray(RorzeHelpers.StringToByteArray(status[1]).Reverse().ToArray());

                for (int iMask = 0; iMask < mask.Length; iMask++)
                {
                    if (!mask[iMask])
                    {
                        continue;
                    }

                    switch (iMask)
                    {
                        case 0:
                            SetStatus(Dio2InputsOutputs.RobotArmNotExtendedInPm1, value[iMask] ? "1" : "0");
                            break;
                        case 1:
                            SetStatus(Dio2InputsOutputs.RobotArmNotExtendedInPm2, value[iMask] ? "1" : "0");
                            break;
                        case 2:
                            SetStatus(Dio2InputsOutputs.RobotArmNotExtendedInPm3, value[iMask] ? "1" : "0");
                            break;
                    }
                }
            }
        }
    }
}
