using System.Collections;
using System.Text;

namespace UnitySC.Rorze.Emulator.Controls.Dio0
{
    public enum Dio0InputsOutputs
    {
        Fan1Rotating,
        Fan2Rotating,
        Fan3Rotating,
        Fan4Rotating,
        Fan5Rotating,
        Fan6Rotating,
        Fan7Rotating,
        Fan8Rotating,
        Fan9Rotating,
        Fan10Rotating,
        Fan11Rotating,
        Fan12Rotating,
        Fan13Rotating,
        Fan14Rotating,
        Fan15Rotating,
        Fan16Rotating,
        Fan17Rotating,
        Fan18Rotating,
        Fan19Rotating,
        Fan20Rotating,
        Fan1AlarmOccurred,
        Fan2AlarmOccurred,
        Fan3AlarmOccurred,
        Fan4AlarmOccurred,
        Fan5AlarmOccurred,
        Fan6AlarmOccurred,
        Fan7AlarmOccurred,
        Fan8AlarmOccurred,
        Fan9AlarmOccurred,
        Fan10AlarmOccurred,
        Fan11AlarmOccurred,
        Fan12AlarmOccurred,
        Fan13AlarmOccurred,
        Fan14AlarmOccurred,
        Fan15AlarmOccurred,
        Fan16AlarmOccurred,
        Fan17AlarmOccurred,
        Fan18AlarmOccurred,
        Fan19AlarmOccurred,
        Fan20AlarmOccurred,
        Sensor1WithinUpperLimitThresholdValue,
        Sensor1WithinLowerLimitThresholdValue,
        Sensor2WithinUpperLimitThresholdValue,
        Sensor2WithinLowerLimitThresholdValue,
        Sensor3WithinUpperLimitThresholdValue,
        Sensor3WithinLowerLimitThresholdValue,
        Sensor4WithinUpperLimitThresholdValue,
        Sensor4WithinLowerLimitThresholdValue,
        Sensor5WithinUpperLimitThresholdValue,
        Sensor5WithinLowerLimitThresholdValue,
        Sensor6WithinUpperLimitThresholdValue,
        Sensor6WithinLowerLimitThresholdValue,
        Sensor7WithinUpperLimitThresholdValue,
        Sensor7WithinLowerLimitThresholdValue,
        Sensor8WithinUpperLimitThresholdValue,
        Sensor8WithinLowerLimitThresholdValue,
        Sensor9WithinUpperLimitThresholdValue,
        Sensor9WithinLowerLimitThresholdValue,
        Sensor10WithinUpperLimitThresholdValue,
        Sensor10WithinLowerLimitThresholdValue,
        Sensor11WithinUpperLimitThresholdValue,
        Sensor11WithinLowerLimitThresholdValue,
        Sensor12WithinUpperLimitThresholdValue,
        Sensor12WithinLowerLimitThresholdValue,
        ControllerDirectInputIn0,
        ControllerDirectInputIn1,
        ControllerDirectInputIn2,
        ControllerDirectInputIn3,
        SystemIsReady,
        BatchAlarmClear1ShotOutput,
        FanOperationStopAllUsingFans1ShotOutput,
        FanOperationStartAllUsingFans1ShotOutput,
        Fan1Operation1ShotOutput,
        Fan2Operation1ShotOutput,
        Fan3Operation1ShotOutput,
        Fan4Operation1ShotOutput,
        Fan5Operation1ShotOutput,
        Fan6Operation1ShotOutput,
        Fan7Operation1ShotOutput,
        Fan8Operation1ShotOutput,
        Fan9Operation1ShotOutput,
        Fan10Operation1ShotOutput,
        Fan11Operation1ShotOutput,
        Fan12Operation1ShotOutput,
        Fan13Operation1ShotOutput,
        Fan14Operation1ShotOutput,
        Fan15Operation1ShotOutput,
        Fan16Operation1ShotOutput,
        Fan17Operation1ShotOutput,
        Fan18Operation1ShotOutput,
        Fan19Operation1ShotOutput,
        Fan20Operation1ShotOutput,
        Fan1AlarmClear1ShotOutput,
        Fan2AlarmClear1ShotOutput,
        Fan3AlarmClear1ShotOutput,
        Fan4AlarmClear1ShotOutput,
        Fan5AlarmClear1ShotOutput,
        Fan6AlarmClear1ShotOutput,
        Fan7AlarmClear1ShotOutput,
        Fan8AlarmClear1ShotOutput,
        Fan9AlarmClear1ShotOutput,
        Fan10AlarmClear1ShotOutput,
        Fan11AlarmClear1ShotOutput,
        Fan12AlarmClear1ShotOutput,
        Fan13AlarmClear1ShotOutput,
        Fan14AlarmClear1ShotOutput,
        Fan15AlarmClear1ShotOutput,
        Fan16AlarmClear1ShotOutput,
        Fan17AlarmClear1ShotOutput,
        Fan18AlarmClear1ShotOutput,
        Fan19AlarmClear1ShotOutput,
        Fan20AlarmClear1ShotOutput,
        Fan1OperationStop1ShotOutput,
        Fan2OperationStop1ShotOutput,
        Fan3OperationStop1ShotOutput,
        Fan4OperationStop1ShotOutput,
        Fan5OperationStop1ShotOutput,
        Fan6OperationStop1ShotOutput,
        Fan7OperationStop1ShotOutput,
        Fan8OperationStop1ShotOutput,
        Fan9OperationStop1ShotOutput,
        Fan10OperationStop1ShotOutput,
        Fan11OperationStop1ShotOutput,
        Fan12OperationStop1ShotOutput,
        Fan13OperationStop1ShotOutput,
        Fan14OperationStop1ShotOutput,
        Fan15OperationStop1ShotOutput,
        Fan16OperationStop1ShotOutput,
        Fan17OperationStop1ShotOutput,
        Fan18OperationStop1ShotOutput,
        Fan19OperationStop1ShotOutput,
        Fan20OperationStop1ShotOutput,
    }

    public partial class Dio0InputsOutputsControl : InputsOutputsControl
    {
        public Dio0InputsOutputsControl()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            paramDataGridView.Rows.Clear();

            paramDataGridView.Rows.Add(1, Dio0InputsOutputs.Fan1Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(2, Dio0InputsOutputs.Fan2Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(3, Dio0InputsOutputs.Fan3Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(4, Dio0InputsOutputs.Fan4Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(5, Dio0InputsOutputs.Fan5Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(6, Dio0InputsOutputs.Fan6Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(7, Dio0InputsOutputs.Fan7Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(8, Dio0InputsOutputs.Fan8Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(9, Dio0InputsOutputs.Fan9Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(10, Dio0InputsOutputs.Fan10Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(11, Dio0InputsOutputs.Fan11Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(12, Dio0InputsOutputs.Fan12Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(13, Dio0InputsOutputs.Fan13Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(14, Dio0InputsOutputs.Fan14Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(15, Dio0InputsOutputs.Fan15Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(16, Dio0InputsOutputs.Fan16Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(17, Dio0InputsOutputs.Fan17Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(18, Dio0InputsOutputs.Fan18Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(19, Dio0InputsOutputs.Fan19Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(20, Dio0InputsOutputs.Fan20Rotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(21, Dio0InputsOutputs.Fan1AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(22, Dio0InputsOutputs.Fan2AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(23, Dio0InputsOutputs.Fan3AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(24, Dio0InputsOutputs.Fan4AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(25, Dio0InputsOutputs.Fan5AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(26, Dio0InputsOutputs.Fan6AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(27, Dio0InputsOutputs.Fan7AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(28, Dio0InputsOutputs.Fan8AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(29, Dio0InputsOutputs.Fan9AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(30, Dio0InputsOutputs.Fan10AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(31, Dio0InputsOutputs.Fan11AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(32, Dio0InputsOutputs.Fan12AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(33, Dio0InputsOutputs.Fan13AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(34, Dio0InputsOutputs.Fan14AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(35, Dio0InputsOutputs.Fan15AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(36, Dio0InputsOutputs.Fan16AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(37, Dio0InputsOutputs.Fan17AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(38, Dio0InputsOutputs.Fan18AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(39, Dio0InputsOutputs.Fan19AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(40, Dio0InputsOutputs.Fan20AlarmOccurred, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(41, Dio0InputsOutputs.Sensor1WithinUpperLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(42, Dio0InputsOutputs.Sensor1WithinLowerLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(43, Dio0InputsOutputs.Sensor2WithinUpperLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(44, Dio0InputsOutputs.Sensor2WithinLowerLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(45, Dio0InputsOutputs.Sensor3WithinUpperLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(46, Dio0InputsOutputs.Sensor3WithinLowerLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(47, Dio0InputsOutputs.Sensor4WithinUpperLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(48, Dio0InputsOutputs.Sensor4WithinLowerLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(49, Dio0InputsOutputs.Sensor5WithinUpperLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(50, Dio0InputsOutputs.Sensor5WithinLowerLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(51, Dio0InputsOutputs.Sensor6WithinUpperLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(52, Dio0InputsOutputs.Sensor6WithinLowerLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(53, Dio0InputsOutputs.Sensor7WithinUpperLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(54, Dio0InputsOutputs.Sensor7WithinLowerLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(55, Dio0InputsOutputs.Sensor8WithinUpperLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(56, Dio0InputsOutputs.Sensor8WithinLowerLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(57, Dio0InputsOutputs.Sensor9WithinUpperLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(58, Dio0InputsOutputs.Sensor9WithinLowerLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(59, Dio0InputsOutputs.Sensor10WithinUpperLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(60, Dio0InputsOutputs.Sensor10WithinLowerLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(61, Dio0InputsOutputs.Sensor11WithinUpperLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(62, Dio0InputsOutputs.Sensor11WithinLowerLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(63, Dio0InputsOutputs.Sensor12WithinUpperLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(64, Dio0InputsOutputs.Sensor12WithinLowerLimitThresholdValue, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(65, Dio0InputsOutputs.ControllerDirectInputIn0, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(66, Dio0InputsOutputs.ControllerDirectInputIn1, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(67, Dio0InputsOutputs.ControllerDirectInputIn2, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(68, Dio0InputsOutputs.ControllerDirectInputIn3, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(69, Dio0InputsOutputs.SystemIsReady, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(70, Dio0InputsOutputs.BatchAlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(71, Dio0InputsOutputs.FanOperationStopAllUsingFans1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(72, Dio0InputsOutputs.FanOperationStartAllUsingFans1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(73, Dio0InputsOutputs.Fan1Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(74, Dio0InputsOutputs.Fan2Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(75, Dio0InputsOutputs.Fan3Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(76, Dio0InputsOutputs.Fan4Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(77, Dio0InputsOutputs.Fan5Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(78, Dio0InputsOutputs.Fan6Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(79, Dio0InputsOutputs.Fan7Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(80, Dio0InputsOutputs.Fan8Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(81, Dio0InputsOutputs.Fan9Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(82, Dio0InputsOutputs.Fan10Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(83, Dio0InputsOutputs.Fan11Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(84, Dio0InputsOutputs.Fan12Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(85, Dio0InputsOutputs.Fan13Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(86, Dio0InputsOutputs.Fan14Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(87, Dio0InputsOutputs.Fan15Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(88, Dio0InputsOutputs.Fan16Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(89, Dio0InputsOutputs.Fan17Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(90, Dio0InputsOutputs.Fan18Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(91, Dio0InputsOutputs.Fan19Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(92, Dio0InputsOutputs.Fan20Operation1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(93, Dio0InputsOutputs.Fan1AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(94, Dio0InputsOutputs.Fan2AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(95, Dio0InputsOutputs.Fan3AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(96, Dio0InputsOutputs.Fan4AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(97, Dio0InputsOutputs.Fan5AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(98, Dio0InputsOutputs.Fan6AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(99, Dio0InputsOutputs.Fan7AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(100, Dio0InputsOutputs.Fan8AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(101, Dio0InputsOutputs.Fan9AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(102, Dio0InputsOutputs.Fan10AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(103, Dio0InputsOutputs.Fan11AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(104, Dio0InputsOutputs.Fan12AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(105, Dio0InputsOutputs.Fan13AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(106, Dio0InputsOutputs.Fan14AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(107, Dio0InputsOutputs.Fan15AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(108, Dio0InputsOutputs.Fan16AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(109, Dio0InputsOutputs.Fan17AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(110, Dio0InputsOutputs.Fan18AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(111, Dio0InputsOutputs.Fan19AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(112, Dio0InputsOutputs.Fan20AlarmClear1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(113, Dio0InputsOutputs.Fan1OperationStop1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(114, Dio0InputsOutputs.Fan2OperationStop1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(115, Dio0InputsOutputs.Fan3OperationStop1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(116, Dio0InputsOutputs.Fan4OperationStop1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(117, Dio0InputsOutputs.Fan5OperationStop1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(118, Dio0InputsOutputs.Fan6OperationStop1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(119, Dio0InputsOutputs.Fan7OperationStop1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(120, Dio0InputsOutputs.Fan8OperationStop1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(121, Dio0InputsOutputs.Fan9OperationStop1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(122, Dio0InputsOutputs.Fan10OperationStop1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(123, Dio0InputsOutputs.Fan11OperationStop1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(124, Dio0InputsOutputs.Fan12OperationStop1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(125, Dio0InputsOutputs.Fan13OperationStop1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(126, Dio0InputsOutputs.Fan14OperationStop1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(127, Dio0InputsOutputs.Fan15OperationStop1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(128, Dio0InputsOutputs.Fan16OperationStop1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(129, Dio0InputsOutputs.Fan17OperationStop1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(130, Dio0InputsOutputs.Fan18OperationStop1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(131, Dio0InputsOutputs.Fan19OperationStop1ShotOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(132, Dio0InputsOutputs.Fan20OperationStop1ShotOutput, "0: False/1: True", "0");
        }

        public string GetStatus(Dio0InputsOutputs status)
        {
            return GetStatus((int)status);
        }

        public void SetStatus(Dio0InputsOutputs status, string value)
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
                false,
                false,
                false,
                false,
                false,
                false,
                GetStatus(Dio0InputsOutputs.Fan1Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan2Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan3Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan4Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan5Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan6Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan7Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan8Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan9Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan10Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan11Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan12Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan13Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan14Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan15Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan16Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan17Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan18Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan19Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan20Rotating) == "1",
                GetStatus(Dio0InputsOutputs.Fan1AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Fan2AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Fan3AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Fan4AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Fan5AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Fan6AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Fan7AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Fan8AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Fan9AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Fan10AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Fan11AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Fan12AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Fan13AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Fan14AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Fan15AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Fan16AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Fan17AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Fan18AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Fan19AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Fan20AlarmOccurred) == "1",
                GetStatus(Dio0InputsOutputs.Sensor1WithinUpperLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor1WithinLowerLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor2WithinUpperLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor2WithinLowerLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor3WithinUpperLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor3WithinLowerLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor4WithinUpperLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor4WithinLowerLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor5WithinUpperLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor5WithinLowerLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor6WithinUpperLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor6WithinLowerLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor7WithinUpperLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor7WithinLowerLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor8WithinUpperLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor8WithinLowerLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor9WithinUpperLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor9WithinLowerLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor10WithinUpperLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor10WithinLowerLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor11WithinUpperLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor11WithinLowerLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor12WithinUpperLimitThresholdValue) == "1",
                GetStatus(Dio0InputsOutputs.Sensor12WithinLowerLimitThresholdValue) == "1",
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                GetStatus(Dio0InputsOutputs.ControllerDirectInputIn0) == "1",
                GetStatus(Dio0InputsOutputs.ControllerDirectInputIn1) == "1",
                GetStatus(Dio0InputsOutputs.ControllerDirectInputIn2) == "1",
                GetStatus(Dio0InputsOutputs.ControllerDirectInputIn3) == "1",
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

            BitArray outputs = new BitArray(new[]
            {
                GetStatus(Dio0InputsOutputs.SystemIsReady) == "1",
                false,
                false,
                false,
                false,
                GetStatus(Dio0InputsOutputs.BatchAlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.FanOperationStopAllUsingFans1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.FanOperationStartAllUsingFans1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan1Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan2Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan3Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan4Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan5Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan6Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan7Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan8Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan9Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan10Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan11Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan12Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan13Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan14Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan15Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan16Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan17Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan18Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan9Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan20Operation1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan1AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan2AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan3AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan4AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan5AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan6AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan7AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan8AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan9AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan10AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan11AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan12AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan13AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan14AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan15AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan16AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan17AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan18AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan19AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan20AlarmClear1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan1OperationStop1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan2OperationStop1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan3OperationStop1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan4OperationStop1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan5OperationStop1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan6OperationStop1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan7OperationStop1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan8OperationStop1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan9OperationStop1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan10OperationStop1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan11OperationStop1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan12OperationStop1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan13OperationStop1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan14OperationStop1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan15OperationStop1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan16OperationStop1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan17OperationStop1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan18OperationStop1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan19OperationStop1ShotOutput) == "1",
                GetStatus(Dio0InputsOutputs.Fan20OperationStop1ShotOutput) == "1",
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
            });

            stringBuilder.Append(BitArrayToHexaString(inputs));
            stringBuilder.Append("/");
            stringBuilder.Append(BitArrayToHexaString(outputs));

            return stringBuilder.ToString();
        }
    }
}
