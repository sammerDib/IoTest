using System.Collections;
using System.Linq;
using System.Text;

using UnitySC.Rorze.Emulator.Common;

namespace UnitySC.Rorze.Emulator.Controls.Dio1
{
    public enum Dio1InputsOutputs
    {
        PressureSensorVac,
        PressureSensorAir,
        LedPushButton,
        PressureSensorIonAir,
        IonizerAlarm,
        Rv201Interlock,
        MaintenanceSwitch,
        DriverPower,
        DoorStatus,
        TpMode,
        LightCurtain,
        OcrWaferReaderLimitSensor1,
        OcrWaferReaderLimitSensor2,
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
        OcrWaferReaderValve1,
        OcrWaferReaderValve2
    }

    public partial class Dio1InputsOutputsControl : InputsOutputsControl
    {
        public Dio1InputsOutputsControl()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            paramDataGridView.Rows.Clear();

            paramDataGridView.Rows.Add(1, Dio1InputsOutputs.PressureSensorVac, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(2, Dio1InputsOutputs.PressureSensorAir, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(3, Dio1InputsOutputs.LedPushButton, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(4, Dio1InputsOutputs.PressureSensorIonAir, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(5, Dio1InputsOutputs.IonizerAlarm, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(6, Dio1InputsOutputs.Rv201Interlock, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(7, Dio1InputsOutputs.MaintenanceSwitch, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(8, Dio1InputsOutputs.DriverPower, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(9, Dio1InputsOutputs.DoorStatus, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(10, Dio1InputsOutputs.TpMode, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(11, Dio1InputsOutputs.LightCurtain, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(12, Dio1InputsOutputs.OcrWaferReaderLimitSensor1, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(13, Dio1InputsOutputs.OcrWaferReaderLimitSensor2, "0: False/1: True", "0");

            paramDataGridView.Rows.Add(14, Dio1InputsOutputs.SignalTowerLightningRed, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(15, Dio1InputsOutputs.SignalTowerLightningYellow, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(16, Dio1InputsOutputs.SignalTowerLightningGreen, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(17, Dio1InputsOutputs.SignalTowerLightningBlue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(18, Dio1InputsOutputs.SignalTowerBlinkingRed, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(19, Dio1InputsOutputs.SignalTowerBlinkingYellow, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(20, Dio1InputsOutputs.SignalTowerBlinkingGreen, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(21, Dio1InputsOutputs.SignalTowerBlinkingBlue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(22, Dio1InputsOutputs.SignalTowerBuzzer1, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(23, Dio1InputsOutputs.SignalTowerBuzzer2, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(24, Dio1InputsOutputs.OcrWaferReaderValve1, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(25, Dio1InputsOutputs.OcrWaferReaderValve2, "0: False/1: True", "0");
        }

        public string GetStatus(Dio1InputsOutputs status)
        {
            return GetStatus((int)status);
        }

        public void SetStatus(Dio1InputsOutputs status, string value)
        {
            SetStatus((int)status, value);
        }

        public override string GetConcatenatedStatuses()
        {
            StringBuilder stringBuilder = new StringBuilder();

            BitArray inputs = new BitArray(new[]
            {
                false,
                false,
                GetStatus(Dio1InputsOutputs.PressureSensorVac) == "1",
                GetStatus(Dio1InputsOutputs.PressureSensorAir) == "1",
                GetStatus(Dio1InputsOutputs.LedPushButton) == "1",
                GetStatus(Dio1InputsOutputs.PressureSensorIonAir) == "1",
                GetStatus(Dio1InputsOutputs.IonizerAlarm) == "1",
                GetStatus(Dio1InputsOutputs.Rv201Interlock) == "1",
                GetStatus(Dio1InputsOutputs.MaintenanceSwitch) == "1",
                GetStatus(Dio1InputsOutputs.DriverPower) == "1",
                GetStatus(Dio1InputsOutputs.DoorStatus) == "1",
                GetStatus(Dio1InputsOutputs.TpMode) == "1",
                GetStatus(Dio1InputsOutputs.LightCurtain) == "1",
                false,
                GetStatus(Dio1InputsOutputs.OcrWaferReaderLimitSensor1) == "1",
                GetStatus(Dio1InputsOutputs.OcrWaferReaderLimitSensor2) == "1",
            });

            BitArray outputs = new BitArray(new[]
            {
                GetStatus(Dio1InputsOutputs.SignalTowerLightningRed) == "1",
                GetStatus(Dio1InputsOutputs.SignalTowerLightningYellow) == "1",
                GetStatus(Dio1InputsOutputs.SignalTowerLightningGreen) == "1",
                GetStatus(Dio1InputsOutputs.SignalTowerLightningBlue) == "1",
                GetStatus(Dio1InputsOutputs.SignalTowerBlinkingRed) == "1",
                GetStatus(Dio1InputsOutputs.SignalTowerBlinkingYellow) == "1",
                GetStatus(Dio1InputsOutputs.SignalTowerBlinkingGreen) == "1",
                GetStatus(Dio1InputsOutputs.SignalTowerBlinkingBlue) == "1",
                GetStatus(Dio1InputsOutputs.SignalTowerBuzzer1) == "1",
                GetStatus(Dio1InputsOutputs.SignalTowerBuzzer2) == "1",
                GetStatus(Dio1InputsOutputs.OcrWaferReaderValve1) == "1",
                GetStatus(Dio1InputsOutputs.OcrWaferReaderValve2) == "1",
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

                SetStatus(Dio1InputsOutputs.SignalTowerLightningRed, outputs[0] ? "1" : "0");
                SetStatus(Dio1InputsOutputs.SignalTowerLightningYellow, outputs[1] ? "1" : "0");
                SetStatus(Dio1InputsOutputs.SignalTowerLightningGreen, outputs[2] ? "1" : "0");
                SetStatus(Dio1InputsOutputs.SignalTowerLightningBlue, outputs[3] ? "1" : "0");
                SetStatus(Dio1InputsOutputs.SignalTowerBlinkingRed, outputs[4] ? "1" : "0");
                SetStatus(Dio1InputsOutputs.SignalTowerBlinkingYellow, outputs[5] ? "1" : "0");
                SetStatus(Dio1InputsOutputs.SignalTowerBlinkingGreen, outputs[6] ? "1" : "0");
                SetStatus(Dio1InputsOutputs.SignalTowerBlinkingBlue, outputs[7] ? "1" : "0");
                SetStatus(Dio1InputsOutputs.SignalTowerBuzzer1, outputs[8] ? "1" : "0");
                SetStatus(Dio1InputsOutputs.SignalTowerBuzzer2, outputs[9] ? "1" : "0");
                SetStatus(Dio1InputsOutputs.OcrWaferReaderValve1, outputs[10] ? "1" : "0");
                SetStatus(Dio1InputsOutputs.OcrWaferReaderValve2, outputs[11] ? "1" : "0");
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
                            SetStatus(Dio1InputsOutputs.SignalTowerLightningRed, value[iMask] ? "1" : "0");
                            break;
                        case 1:
                            SetStatus(Dio1InputsOutputs.SignalTowerLightningYellow, value[iMask] ? "1" : "0");
                            break;
                        case 2:
                            SetStatus(Dio1InputsOutputs.SignalTowerLightningGreen, value[iMask] ? "1" : "0");
                            break;
                        case 3:
                            SetStatus(Dio1InputsOutputs.SignalTowerLightningBlue, value[iMask] ? "1" : "0");
                            break;
                        case 4:
                            SetStatus(Dio1InputsOutputs.SignalTowerBlinkingRed, value[iMask] ? "1" : "0");
                            break;
                        case 5:
                            SetStatus(Dio1InputsOutputs.SignalTowerBlinkingYellow, value[iMask] ? "1" : "0");
                            break;
                        case 6:
                            SetStatus(Dio1InputsOutputs.SignalTowerBlinkingGreen, value[iMask] ? "1" : "0");
                            break;
                        case 7:
                            SetStatus(Dio1InputsOutputs.SignalTowerBlinkingBlue, value[iMask] ? "1" : "0");
                            break;
                        case 8:
                            SetStatus(Dio1InputsOutputs.SignalTowerBuzzer1, value[iMask] ? "1" : "0");
                            break;
                        case 9:
                            SetStatus(Dio1InputsOutputs.SignalTowerBuzzer2, value[iMask] ? "1" : "0");
                            break;
                        case 10:
                            SetStatus(Dio1InputsOutputs.OcrWaferReaderValve1, value[iMask] ? "1" : "0");
                            break;
                        case 11:
                            SetStatus(Dio1InputsOutputs.OcrWaferReaderValve2, value[iMask] ? "1" : "0");
                            break;
                    }
                }
            }
        }
    }
}
