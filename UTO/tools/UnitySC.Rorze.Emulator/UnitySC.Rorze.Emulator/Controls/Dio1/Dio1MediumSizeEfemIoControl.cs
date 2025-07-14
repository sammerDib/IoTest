using System.Collections;
using System.Linq;
using System.Text;

using UnitySC.Rorze.Emulator.Common;

namespace UnitySC.Rorze.Emulator.Controls.Dio1
{
    public enum Dio1MediumSizeInputsOutputs
    {
        MaintenanceSwitch,
        PressureSensorVac,
        LedPushButton,
        PressureSensorIonAir,
        IonizerAlarm,
        LightCurtain,
        Pm1DoorOpened,
        Pm1ReadyToLoadUnload,
        Pm2DoorOpened,
        Pm2ReadyToLoadUnload,
        Pm3DoorOpened,
        Pm3ReadyToLoadUnload,
        RobotArmNotExtendedInPm1,
        RobotArmNotExtendedInPm2,
        RobotArmNotExtendedInPm3,
        SignalTowerLightningRed,
        SignalTowerLightningYellow,
        SignalTowerLightningGreen,
        SignalTowerLightningBlue,
        SignalTowerBlinkingRed,
        SignalTowerBlinkingYellow,
        SignalTowerBlinkingGreen,
        SignalTowerBlinkingBlue,
        SignalTowerBuzzer1,
        SignalTowerBuzzer2,
        SignalTowerPowerSupply,
    }

    public partial class Dio1MediumSizeEfemIoControl : InputsOutputsControl
    {
        public Dio1MediumSizeEfemIoControl()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            paramDataGridView.Rows.Clear();

            paramDataGridView.Rows.Add(1, Dio1MediumSizeInputsOutputs.MaintenanceSwitch, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(2, Dio1MediumSizeInputsOutputs.PressureSensorVac, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(3, Dio1MediumSizeInputsOutputs.LedPushButton, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(4, Dio1MediumSizeInputsOutputs.PressureSensorIonAir, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(5, Dio1MediumSizeInputsOutputs.IonizerAlarm, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(6, Dio1MediumSizeInputsOutputs.LightCurtain, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(7, Dio1MediumSizeInputsOutputs.Pm1DoorOpened, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(8, Dio1MediumSizeInputsOutputs.Pm1ReadyToLoadUnload, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(9, Dio1MediumSizeInputsOutputs.Pm2DoorOpened, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(10, Dio1MediumSizeInputsOutputs.Pm2ReadyToLoadUnload, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(11, Dio1MediumSizeInputsOutputs.Pm3DoorOpened, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(12, Dio1MediumSizeInputsOutputs.Pm3ReadyToLoadUnload, "0: False/1: True", "0");

            paramDataGridView.Rows.Add(13, Dio1MediumSizeInputsOutputs.RobotArmNotExtendedInPm1, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(14, Dio1MediumSizeInputsOutputs.RobotArmNotExtendedInPm2, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(15, Dio1MediumSizeInputsOutputs.RobotArmNotExtendedInPm3, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(16, Dio1MediumSizeInputsOutputs.SignalTowerLightningRed, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(17, Dio1MediumSizeInputsOutputs.SignalTowerLightningYellow, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(18, Dio1MediumSizeInputsOutputs.SignalTowerLightningGreen, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(19, Dio1MediumSizeInputsOutputs.SignalTowerLightningBlue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(20, Dio1MediumSizeInputsOutputs.SignalTowerBlinkingRed, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(21, Dio1MediumSizeInputsOutputs.SignalTowerBlinkingYellow, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(22, Dio1MediumSizeInputsOutputs.SignalTowerBlinkingGreen, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(23, Dio1MediumSizeInputsOutputs.SignalTowerBlinkingBlue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(24, Dio1MediumSizeInputsOutputs.SignalTowerBuzzer1, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(25, Dio1MediumSizeInputsOutputs.SignalTowerBuzzer2, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(26, Dio1MediumSizeInputsOutputs.SignalTowerPowerSupply, "0: False/1: True", "0");
        }

        public string GetStatus(Dio1MediumSizeInputsOutputs status)
        {
            return GetStatus((int)status);
        }

        public void SetStatus(Dio1MediumSizeInputsOutputs status, string value)
        {
            SetStatus((int)status, value);
        }

        public override string GetConcatenatedStatuses()
        {
            StringBuilder stringBuilder = new StringBuilder();

            BitArray inputs = new BitArray(new[]
            {
                GetStatus(Dio1MediumSizeInputsOutputs.MaintenanceSwitch) == "1",
                false,
                GetStatus(Dio1MediumSizeInputsOutputs.PressureSensorVac) == "1",
                false,
                GetStatus(Dio1MediumSizeInputsOutputs.LedPushButton) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.PressureSensorIonAir) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.IonizerAlarm) == "1",
                false,
                GetStatus(Dio1MediumSizeInputsOutputs.LightCurtain) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.Pm1DoorOpened) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.Pm1ReadyToLoadUnload) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.Pm2DoorOpened) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.Pm2ReadyToLoadUnload) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.Pm3DoorOpened) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.Pm3ReadyToLoadUnload) == "1",
                false,
            });

            BitArray outputs = new BitArray(new[]
            {
                GetStatus(Dio1MediumSizeInputsOutputs.RobotArmNotExtendedInPm1) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.RobotArmNotExtendedInPm2) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.RobotArmNotExtendedInPm3) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.SignalTowerLightningRed) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.SignalTowerLightningYellow) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.SignalTowerLightningGreen) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.SignalTowerLightningBlue) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.SignalTowerBlinkingRed) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.SignalTowerBlinkingYellow) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.SignalTowerBlinkingGreen) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.SignalTowerBlinkingBlue) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.SignalTowerBuzzer1) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.SignalTowerBuzzer2) == "1",
                GetStatus(Dio1MediumSizeInputsOutputs.SignalTowerPowerSupply) == "1",
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

                SetStatus(Dio1MediumSizeInputsOutputs.RobotArmNotExtendedInPm1, outputs[0] ? "1" : "0");
                SetStatus(Dio1MediumSizeInputsOutputs.RobotArmNotExtendedInPm2, outputs[1] ? "1" : "0");
                SetStatus(Dio1MediumSizeInputsOutputs.RobotArmNotExtendedInPm3, outputs[2] ? "1" : "0");
                SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerLightningRed, outputs[3] ? "1" : "0");
                SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerLightningYellow, outputs[4] ? "1" : "0");
                SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerLightningGreen, outputs[5] ? "1" : "0");
                SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerLightningBlue, outputs[6] ? "1" : "0");
                SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerBlinkingRed, outputs[7] ? "1" : "0");
                SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerBlinkingYellow, outputs[8] ? "1" : "0");
                SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerBlinkingGreen, outputs[9] ? "1" : "0");
                SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerBlinkingBlue, outputs[10] ? "1" : "0");
                SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerBuzzer1, outputs[11] ? "1" : "0");
                SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerBuzzer2, outputs[12] ? "1" : "0");
                SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerPowerSupply, outputs[13] ? "1" : "0");
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
                            SetStatus(Dio1MediumSizeInputsOutputs.RobotArmNotExtendedInPm1, value[iMask] ? "1" : "0");
                            break;
                        case 1:
                            SetStatus(Dio1MediumSizeInputsOutputs.RobotArmNotExtendedInPm2, value[iMask] ? "1" : "0");
                            break;
                        case 2:
                            SetStatus(Dio1MediumSizeInputsOutputs.RobotArmNotExtendedInPm3, value[iMask] ? "1" : "0");
                            break;
                        case 3:
                            SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerLightningRed, value[iMask] ? "1" : "0");
                            break;
                        case 4:
                            SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerLightningYellow, value[iMask] ? "1" : "0");
                            break;
                        case 5:
                            SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerLightningGreen, value[iMask] ? "1" : "0");
                            break;
                        case 6:
                            SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerLightningBlue, value[iMask] ? "1" : "0");
                            break;
                        case 7:
                            SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerBlinkingRed, value[iMask] ? "1" : "0");
                            break;
                        case 8:
                            SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerBlinkingYellow, value[iMask] ? "1" : "0");
                            break;
                        case 9:
                            SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerBlinkingGreen, value[iMask] ? "1" : "0");
                            break;
                        case 10:
                            SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerBlinkingBlue, value[iMask] ? "1" : "0");
                            break;
                        case 11:
                            SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerBuzzer1, value[iMask] ? "1" : "0");
                            break;
                        case 12:
                            SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerBuzzer2, value[iMask] ? "1" : "0");
                            break;
                        case 13:
                            SetStatus(Dio1MediumSizeInputsOutputs.SignalTowerPowerSupply, value[iMask] ? "1" : "0");
                            break;
                    }
                }
            }
        }
    }
}
