using Agileo.EquipmentModeling;

using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Simulation;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0
{
    public partial class Dio0 : ISimDevice
    {
        #region Properties

        protected internal Dio0SimulationData SimulationData { get; private set; }

        #endregion

        #region ISimDevice

        public ISimDeviceView SimDeviceView
            => new Dio0SimulatorUserControl() { DataContext = SimulationData };

        #endregion ISimDevice

        #region Commands

        protected virtual void InternalSimulateSetFfuSpeed(
            UnitsNet.RotationalSpeed setPoint,
            Tempomat tempomat)
        {
            SimulationData.FanSpeed = setPoint;
        }

        #endregion

        #region Private Methods

        private void SetUpSimulatedMode()
        {
            SimulationData = new Dio0SimulationData(this);
            SimulationData.PropertyChanged += SimulationData_PropertyChanged;
        }

        private void DisposeSimulatedMode()
        {
            if (SimulationData != null)
            {
                SimulationData.PropertyChanged -= SimulationData_PropertyChanged;
                SimulationData = null;
            }
        }

        #endregion

        #region Event Handlers

        private void SimulationData_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SimulationData.I_DvrAlarm):
                    I_DvrAlarm = SimulationData.I_DvrAlarm;
                    break;
                case nameof(SimulationData.FanSpeed):
                    FanSpeed = SimulationData.FanSpeed;
                    break;
                case nameof(SimulationData.I_FANDetection1):
                    I_FANDetection1 = SimulationData.I_FANDetection1;
                    break;
                case nameof(SimulationData.I_FANDetection2):
                    I_FANDetection2 = SimulationData.I_FANDetection2;
                    break;
                case nameof(SimulationData.I_FAN1Rotating):
                    I_FAN1Rotating = SimulationData.I_FAN1Rotating;
                    break;
                case nameof(SimulationData.I_FAN2Rotating):
                    I_FAN2Rotating = SimulationData.I_FAN2Rotating;
                    break;
                case nameof(SimulationData.I_FAN3Rotating):
                    I_FAN3Rotating = SimulationData.I_FAN3Rotating;
                    break;
                case nameof(SimulationData.I_FAN4Rotating):
                    I_FAN4Rotating = SimulationData.I_FAN4Rotating;
                    break;
                case nameof(SimulationData.I_FAN5Rotating):
                    I_FAN5Rotating = SimulationData.I_FAN5Rotating;
                    break;
                case nameof(SimulationData.I_FAN6Rotating):
                    I_FAN6Rotating = SimulationData.I_FAN6Rotating;
                    break;
                case nameof(SimulationData.I_FAN7Rotating):
                    I_FAN7Rotating = SimulationData.I_FAN7Rotating;
                    break;
                case nameof(SimulationData.I_FAN8Rotating):
                    I_FAN8Rotating = SimulationData.I_FAN8Rotating;
                    break;
                case nameof(SimulationData.I_FAN9Rotating):
                    I_FAN9Rotating = SimulationData.I_FAN9Rotating;
                    break;
                case nameof(SimulationData.I_FAN10Rotating):
                    I_FAN10Rotating = SimulationData.I_FAN10Rotating;
                    break;
                case nameof(SimulationData.I_FAN11Rotating):
                    I_FAN11Rotating = SimulationData.I_FAN11Rotating;
                    break;
                case nameof(SimulationData.I_FAN12Rotating):
                    I_FAN12Rotating = SimulationData.I_FAN12Rotating;
                    break;
                case nameof(SimulationData.I_FAN13Rotating):
                    I_FAN13Rotating = SimulationData.I_FAN13Rotating;
                    break;
                case nameof(SimulationData.I_FAN14Rotating):
                    I_FAN14Rotating = SimulationData.I_FAN14Rotating;
                    break;
                case nameof(SimulationData.I_FAN15Rotating):
                    I_FAN15Rotating = SimulationData.I_FAN15Rotating;
                    break;
                case nameof(SimulationData.I_FAN16Rotating):
                    I_FAN16Rotating = SimulationData.I_FAN16Rotating;
                    break;
                case nameof(SimulationData.I_FAN17Rotating):
                    I_FAN17Rotating = SimulationData.I_FAN17Rotating;
                    break;
                case nameof(SimulationData.I_FAN18Rotating):
                    I_FAN18Rotating = SimulationData.I_FAN18Rotating;
                    break;
                case nameof(SimulationData.I_FAN19Rotating):
                    I_FAN19Rotating = SimulationData.I_FAN19Rotating;
                    break;
                case nameof(SimulationData.I_FAN20Rotating):
                    I_FAN20Rotating = SimulationData.I_FAN20Rotating;
                    break;
                case nameof(SimulationData.I_FAN1AlarmOccurred):
                    I_FAN1AlarmOccurred = SimulationData.I_FAN1AlarmOccurred;
                    break;
                case nameof(SimulationData.I_FAN2AlarmOccurred):
                    I_FAN2AlarmOccurred = SimulationData.I_FAN2AlarmOccurred;
                    break;
                case nameof(SimulationData.I_FAN3AlarmOccurred):
                    I_FAN3AlarmOccurred = SimulationData.I_FAN3AlarmOccurred;
                    break;
                case nameof(SimulationData.I_FAN4AlarmOccurred):
                    I_FAN4AlarmOccurred = SimulationData.I_FAN4AlarmOccurred;
                    break;
                case nameof(SimulationData.I_FAN5AlarmOccurred):
                    I_FAN5AlarmOccurred = SimulationData.I_FAN5AlarmOccurred;
                    break;
                case nameof(SimulationData.I_FAN6AlarmOccurred):
                    I_FAN6AlarmOccurred = SimulationData.I_FAN6AlarmOccurred;
                    break;
                case nameof(SimulationData.I_FAN7AlarmOccurred):
                    I_FAN7AlarmOccurred = SimulationData.I_FAN7AlarmOccurred;
                    break;
                case nameof(SimulationData.I_FAN8AlarmOccurred):
                    I_FAN8AlarmOccurred = SimulationData.I_FAN8AlarmOccurred;
                    break;
                case nameof(SimulationData.I_FAN9AlarmOccurred):
                    I_FAN9AlarmOccurred = SimulationData.I_FAN9AlarmOccurred;
                    break;
                case nameof(SimulationData.I_FAN10AlarmOccurred):
                    I_FAN10AlarmOccurred = SimulationData.I_FAN10AlarmOccurred;
                    break;
                case nameof(SimulationData.I_FAN11AlarmOccurred):
                    I_FAN11AlarmOccurred = SimulationData.I_FAN11AlarmOccurred;
                    break;
                case nameof(SimulationData.I_FAN12AlarmOccurred):
                    I_FAN12AlarmOccurred = SimulationData.I_FAN12AlarmOccurred;
                    break;
                case nameof(SimulationData.I_FAN13AlarmOccurred):
                    I_FAN13AlarmOccurred = SimulationData.I_FAN13AlarmOccurred;
                    break;
                case nameof(SimulationData.I_FAN14AlarmOccurred):
                    I_FAN14AlarmOccurred = SimulationData.I_FAN14AlarmOccurred;
                    break;
                case nameof(SimulationData.I_FAN15AlarmOccurred):
                    I_FAN15AlarmOccurred = SimulationData.I_FAN15AlarmOccurred;
                    break;
                case nameof(SimulationData.I_FAN16AlarmOccurred):
                    I_FAN16AlarmOccurred = SimulationData.I_FAN16AlarmOccurred;
                    break;
                case nameof(SimulationData.I_FAN17AlarmOccurred):
                    I_FAN17AlarmOccurred = SimulationData.I_FAN17AlarmOccurred;
                    break;
                case nameof(SimulationData.I_FAN18AlarmOccurred):
                    I_FAN18AlarmOccurred = SimulationData.I_FAN18AlarmOccurred;
                    break;
                case nameof(SimulationData.I_FAN19AlarmOccurred):
                    I_FAN19AlarmOccurred = SimulationData.I_FAN19AlarmOccurred;
                    break;
                case nameof(SimulationData.I_FAN20AlarmOccurred):
                    I_FAN20AlarmOccurred = SimulationData.I_FAN20AlarmOccurred;
                    break;
                case nameof(SimulationData.I_Sensor1_WithinUpperLimitThresholdValue):
                    I_Sensor1_WithinUpperLimitThresholdValue =
                        SimulationData.I_Sensor1_WithinUpperLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor1_WithinLowerLimitThresholdValue):
                    I_Sensor1_WithinLowerLimitThresholdValue =
                        SimulationData.I_Sensor1_WithinLowerLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor2_WithinUpperLimitThresholdValue):
                    I_Sensor2_WithinUpperLimitThresholdValue =
                        SimulationData.I_Sensor2_WithinUpperLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor2_WithinLowerLimitThresholdValue):
                    I_Sensor2_WithinLowerLimitThresholdValue =
                        SimulationData.I_Sensor2_WithinLowerLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor3_WithinUpperLimitThresholdValue):
                    I_Sensor3_WithinUpperLimitThresholdValue =
                        SimulationData.I_Sensor3_WithinUpperLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor3_WithinLowerLimitThresholdValue):
                    I_Sensor3_WithinLowerLimitThresholdValue =
                        SimulationData.I_Sensor3_WithinLowerLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor4_WithinUpperLimitThresholdValue):
                    I_Sensor4_WithinUpperLimitThresholdValue =
                        SimulationData.I_Sensor4_WithinUpperLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor4_WithinLowerLimitThresholdValue):
                    I_Sensor4_WithinLowerLimitThresholdValue =
                        SimulationData.I_Sensor4_WithinLowerLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor5_WithinUpperLimitThresholdValue):
                    I_Sensor5_WithinUpperLimitThresholdValue =
                        SimulationData.I_Sensor5_WithinUpperLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor5_WithinLowerLimitThresholdValue):
                    I_Sensor5_WithinLowerLimitThresholdValue =
                        SimulationData.I_Sensor5_WithinLowerLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor6_WithinUpperLimitThresholdValue):
                    I_Sensor6_WithinUpperLimitThresholdValue =
                        SimulationData.I_Sensor6_WithinUpperLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor6_WithinLowerLimitThresholdValue):
                    I_Sensor6_WithinLowerLimitThresholdValue =
                        SimulationData.I_Sensor6_WithinLowerLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor7_WithinUpperLimitThresholdValue):
                    I_Sensor7_WithinUpperLimitThresholdValue =
                        SimulationData.I_Sensor7_WithinUpperLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor7_WithinLowerLimitThresholdValue):
                    I_Sensor7_WithinLowerLimitThresholdValue =
                        SimulationData.I_Sensor7_WithinLowerLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor8_WithinUpperLimitThresholdValue):
                    I_Sensor8_WithinUpperLimitThresholdValue =
                        SimulationData.I_Sensor8_WithinUpperLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor8_WithinLowerLimitThresholdValue):
                    I_Sensor8_WithinLowerLimitThresholdValue =
                        SimulationData.I_Sensor8_WithinLowerLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor9_WithinUpperLimitThresholdValue):
                    I_Sensor9_WithinUpperLimitThresholdValue =
                        SimulationData.I_Sensor9_WithinUpperLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor9_WithinLowerLimitThresholdValue):
                    I_Sensor9_WithinLowerLimitThresholdValue =
                        SimulationData.I_Sensor9_WithinLowerLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor10_WithinUpperLimitThresholdValue):
                    I_Sensor10_WithinUpperLimitThresholdValue =
                        SimulationData.I_Sensor10_WithinUpperLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor10_WithinLowerLimitThresholdValue):
                    I_Sensor10_WithinLowerLimitThresholdValue =
                        SimulationData.I_Sensor10_WithinLowerLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor11_WithinUpperLimitThresholdValue):
                    I_Sensor11_WithinUpperLimitThresholdValue =
                        SimulationData.I_Sensor11_WithinUpperLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor11_WithinLowerLimitThresholdValue):
                    I_Sensor11_WithinLowerLimitThresholdValue =
                        SimulationData.I_Sensor11_WithinLowerLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor12_WithinUpperLimitThresholdValue):
                    I_Sensor12_WithinUpperLimitThresholdValue =
                        SimulationData.I_Sensor12_WithinUpperLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_Sensor12_WithinLowerLimitThresholdValue):
                    I_Sensor12_WithinLowerLimitThresholdValue =
                        SimulationData.I_Sensor12_WithinLowerLimitThresholdValue;
                    break;
                case nameof(SimulationData.I_ControllerDirectInput_IN0):
                    I_ControllerDirectInput_IN0 = SimulationData.I_ControllerDirectInput_IN0;
                    break;
                case nameof(SimulationData.I_ControllerDirectInput_IN1):
                    I_ControllerDirectInput_IN1 = SimulationData.I_ControllerDirectInput_IN1;
                    break;
                case nameof(SimulationData.I_ControllerDirectInput_IN2):
                    I_ControllerDirectInput_IN2 = SimulationData.I_ControllerDirectInput_IN2;
                    break;
                case nameof(SimulationData.I_ControllerDirectInput_IN3):
                    I_ControllerDirectInput_IN3 = SimulationData.I_ControllerDirectInput_IN3;
                    break;
                case nameof(SimulationData.O_SystemIsReady):
                    O_SystemIsReady = SimulationData.O_SystemIsReady;
                    break;
                case nameof(SimulationData.O_BatchAlarmClear_1ShotOutput):
                    O_BatchAlarmClear_1ShotOutput = SimulationData.O_BatchAlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_Fan_OperationStop_AllUsingFans_1ShotOutput):
                    O_Fan_OperationStop_AllUsingFans_1ShotOutput =
                        SimulationData.O_Fan_OperationStop_AllUsingFans_1ShotOutput;
                    break;
                case nameof(SimulationData.O_Fan_OperationStart_AllUsingFans_1ShotOutput):
                    O_Fan_OperationStart_AllUsingFans_1ShotOutput =
                        SimulationData.O_Fan_OperationStart_AllUsingFans_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN1_OperationStart_1ShotOutput):
                    O_FAN1_OperationStart_1ShotOutput =
                        SimulationData.O_FAN1_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN2_OperationStart_1ShotOutput):
                    O_FAN2_OperationStart_1ShotOutput =
                        SimulationData.O_FAN2_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN3_OperationStart_1ShotOutput):
                    O_FAN3_OperationStart_1ShotOutput =
                        SimulationData.O_FAN3_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN4_OperationStart_1ShotOutput):
                    O_FAN4_OperationStart_1ShotOutput =
                        SimulationData.O_FAN4_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN5_OperationStart_1ShotOutput):
                    O_FAN5_OperationStart_1ShotOutput =
                        SimulationData.O_FAN5_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN6_OperationStart_1ShotOutput):
                    O_FAN6_OperationStart_1ShotOutput =
                        SimulationData.O_FAN6_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN7_OperationStart_1ShotOutput):
                    O_FAN7_OperationStart_1ShotOutput =
                        SimulationData.O_FAN7_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN8_OperationStart_1ShotOutput):
                    O_FAN8_OperationStart_1ShotOutput =
                        SimulationData.O_FAN8_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN9_OperationStart_1ShotOutput):
                    O_FAN9_OperationStart_1ShotOutput =
                        SimulationData.O_FAN9_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN10_OperationStart_1ShotOutput):
                    O_FAN10_OperationStart_1ShotOutput =
                        SimulationData.O_FAN10_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN11_OperationStart_1ShotOutput):
                    O_FAN11_OperationStart_1ShotOutput =
                        SimulationData.O_FAN11_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN12_OperationStart_1ShotOutput):
                    O_FAN12_OperationStart_1ShotOutput =
                        SimulationData.O_FAN12_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN13_OperationStart_1ShotOutput):
                    O_FAN13_OperationStart_1ShotOutput =
                        SimulationData.O_FAN13_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN14_OperationStart_1ShotOutput):
                    O_FAN14_OperationStart_1ShotOutput =
                        SimulationData.O_FAN14_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN15_OperationStart_1ShotOutput):
                    O_FAN15_OperationStart_1ShotOutput =
                        SimulationData.O_FAN15_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN16_OperationStart_1ShotOutput):
                    O_FAN16_OperationStart_1ShotOutput =
                        SimulationData.O_FAN16_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN17_OperationStart_1ShotOutput):
                    O_FAN17_OperationStart_1ShotOutput =
                        SimulationData.O_FAN17_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN18_OperationStart_1ShotOutput):
                    O_FAN18_OperationStart_1ShotOutput =
                        SimulationData.O_FAN18_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN19_OperationStart_1ShotOutput):
                    O_FAN19_OperationStart_1ShotOutput =
                        SimulationData.O_FAN19_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN20_OperationStart_1ShotOutput):
                    O_FAN20_OperationStart_1ShotOutput =
                        SimulationData.O_FAN20_OperationStart_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN1_AlarmClear_1ShotOutput):
                    O_FAN1_AlarmClear_1ShotOutput = SimulationData.O_FAN1_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN2_AlarmClear_1ShotOutput):
                    O_FAN2_AlarmClear_1ShotOutput = SimulationData.O_FAN2_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN3_AlarmClear_1ShotOutput):
                    O_FAN3_AlarmClear_1ShotOutput = SimulationData.O_FAN3_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN4_AlarmClear_1ShotOutput):
                    O_FAN4_AlarmClear_1ShotOutput = SimulationData.O_FAN4_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN5_AlarmClear_1ShotOutput):
                    O_FAN5_AlarmClear_1ShotOutput = SimulationData.O_FAN5_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN6_AlarmClear_1ShotOutput):
                    O_FAN6_AlarmClear_1ShotOutput = SimulationData.O_FAN6_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN7_AlarmClear_1ShotOutput):
                    O_FAN7_AlarmClear_1ShotOutput = SimulationData.O_FAN7_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN8_AlarmClear_1ShotOutput):
                    O_FAN8_AlarmClear_1ShotOutput = SimulationData.O_FAN8_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN9_AlarmClear_1ShotOutput):
                    O_FAN9_AlarmClear_1ShotOutput = SimulationData.O_FAN9_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN10_AlarmClear_1ShotOutput):
                    O_FAN10_AlarmClear_1ShotOutput = SimulationData.O_FAN10_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN11_AlarmClear_1ShotOutput):
                    O_FAN11_AlarmClear_1ShotOutput = SimulationData.O_FAN11_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN12_AlarmClear_1ShotOutput):
                    O_FAN12_AlarmClear_1ShotOutput = SimulationData.O_FAN12_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN13_AlarmClear_1ShotOutput):
                    O_FAN13_AlarmClear_1ShotOutput = SimulationData.O_FAN13_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN14_AlarmClear_1ShotOutput):
                    O_FAN14_AlarmClear_1ShotOutput = SimulationData.O_FAN14_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN15_AlarmClear_1ShotOutput):
                    O_FAN15_AlarmClear_1ShotOutput = SimulationData.O_FAN15_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN16_AlarmClear_1ShotOutput):
                    O_FAN16_AlarmClear_1ShotOutput = SimulationData.O_FAN16_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN17_AlarmClear_1ShotOutput):
                    O_FAN17_AlarmClear_1ShotOutput = SimulationData.O_FAN17_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN18_AlarmClear_1ShotOutput):
                    O_FAN18_AlarmClear_1ShotOutput = SimulationData.O_FAN18_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN19_AlarmClear_1ShotOutput):
                    O_FAN19_AlarmClear_1ShotOutput = SimulationData.O_FAN19_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN20_AlarmClear_1ShotOutput):
                    O_FAN20_AlarmClear_1ShotOutput = SimulationData.O_FAN20_AlarmClear_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN1_OperationStop_1ShotOutput):
                    O_FAN1_OperationStop_1ShotOutput =
                        SimulationData.O_FAN1_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN2_OperationStop_1ShotOutput):
                    O_FAN2_OperationStop_1ShotOutput =
                        SimulationData.O_FAN2_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN3_OperationStop_1ShotOutput):
                    O_FAN3_OperationStop_1ShotOutput =
                        SimulationData.O_FAN3_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN4_OperationStop_1ShotOutput):
                    O_FAN4_OperationStop_1ShotOutput =
                        SimulationData.O_FAN4_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN5_OperationStop_1ShotOutput):
                    O_FAN5_OperationStop_1ShotOutput =
                        SimulationData.O_FAN5_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN6_OperationStop_1ShotOutput):
                    O_FAN6_OperationStop_1ShotOutput =
                        SimulationData.O_FAN6_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN7_OperationStop_1ShotOutput):
                    O_FAN7_OperationStop_1ShotOutput =
                        SimulationData.O_FAN7_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN8_OperationStop_1ShotOutput):
                    O_FAN8_OperationStop_1ShotOutput =
                        SimulationData.O_FAN8_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN9_OperationStop_1ShotOutput):
                    O_FAN9_OperationStop_1ShotOutput =
                        SimulationData.O_FAN9_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN10_OperationStop_1ShotOutput):
                    O_FAN10_OperationStop_1ShotOutput =
                        SimulationData.O_FAN10_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN11_OperationStop_1ShotOutput):
                    O_FAN11_OperationStop_1ShotOutput =
                        SimulationData.O_FAN11_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN12_OperationStop_1ShotOutput):
                    O_FAN12_OperationStop_1ShotOutput =
                        SimulationData.O_FAN12_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN13_OperationStop_1ShotOutput):
                    O_FAN13_OperationStop_1ShotOutput =
                        SimulationData.O_FAN13_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN14_OperationStop_1ShotOutput):
                    O_FAN14_OperationStop_1ShotOutput =
                        SimulationData.O_FAN14_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN15_OperationStop_1ShotOutput):
                    O_FAN15_OperationStop_1ShotOutput =
                        SimulationData.O_FAN15_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN16_OperationStop_1ShotOutput):
                    O_FAN16_OperationStop_1ShotOutput =
                        SimulationData.O_FAN16_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN17_OperationStop_1ShotOutput):
                    O_FAN17_OperationStop_1ShotOutput =
                        SimulationData.O_FAN17_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN18_OperationStop_1ShotOutput):
                    O_FAN18_OperationStop_1ShotOutput =
                        SimulationData.O_FAN18_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN19_OperationStop_1ShotOutput):
                    O_FAN19_OperationStop_1ShotOutput =
                        SimulationData.O_FAN19_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.O_FAN20_OperationStop_1ShotOutput):
                    O_FAN20_OperationStop_1ShotOutput =
                        SimulationData.O_FAN20_OperationStop_1ShotOutput;
                    break;
                case nameof(SimulationData.I_Lp1_Valid):
                    I_Lp1_Valid = SimulationData.I_Lp1_Valid;
                    break;
                case nameof(SimulationData.I_Lp1_Cs_0):
                    I_Lp1_Cs_0 = SimulationData.I_Lp1_Cs_0;
                    break;
                case nameof(SimulationData.I_Lp1_Cs_1):
                    I_Lp1_Cs_1 = SimulationData.I_Lp1_Cs_1;
                    break;
                case nameof(SimulationData.I_Lp1_Tr_Req):
                    I_Lp1_Tr_Req = SimulationData.I_Lp1_Tr_Req;
                    break;
                case nameof(SimulationData.I_Lp1_Busy):
                    I_Lp1_Busy = SimulationData.I_Lp1_Busy;
                    break;
                case nameof(SimulationData.I_Lp1_Compt):
                    I_Lp1_Compt = SimulationData.I_Lp1_Compt;
                    break;
                case nameof(SimulationData.I_Lp1_Cont):
                    I_Lp1_Cont = SimulationData.I_Lp1_Cont;
                    break;
                case nameof(SimulationData.O_Lp1_L_Req):
                    O_Lp1_L_Req = SimulationData.O_Lp1_L_Req;
                    break;
                case nameof(SimulationData.O_Lp1_U_Req):
                    O_Lp1_U_Req = SimulationData.O_Lp1_U_Req;
                    break;
                case nameof(SimulationData.O_Lp1_Ready):
                    O_Lp1_Ready = SimulationData.O_Lp1_Ready;
                    break;
                case nameof(SimulationData.O_Lp1_Ho_Avbl):
                    O_Lp1_Ho_Avbl = SimulationData.O_Lp1_Ho_Avbl;
                    break;
                case nameof(SimulationData.O_Lp1_Es):
                    O_Lp1_Es = SimulationData.O_Lp1_Es;
                    break;
                case nameof(SimulationData.I_Lp2_Valid):
                    I_Lp2_Valid = SimulationData.I_Lp2_Valid;
                    break;
                case nameof(SimulationData.I_Lp2_Cs_0):
                    I_Lp2_Cs_0 = SimulationData.I_Lp2_Cs_0;
                    break;
                case nameof(SimulationData.I_Lp2_Cs_1):
                    I_Lp2_Cs_1 = SimulationData.I_Lp2_Cs_1;
                    break;
                case nameof(SimulationData.I_Lp2_Tr_Req):
                    I_Lp2_Tr_Req = SimulationData.I_Lp2_Tr_Req;
                    break;
                case nameof(SimulationData.I_Lp2_Busy):
                    I_Lp2_Busy = SimulationData.I_Lp2_Busy;
                    break;
                case nameof(SimulationData.I_Lp2_Compt):
                    I_Lp2_Compt = SimulationData.I_Lp2_Compt;
                    break;
                case nameof(SimulationData.I_Lp2_Cont):
                    I_Lp2_Cont = SimulationData.I_Lp2_Cont;
                    break;
                case nameof(SimulationData.O_Lp2_L_Req):
                    O_Lp2_L_Req = SimulationData.O_Lp2_L_Req;
                    break;
                case nameof(SimulationData.O_Lp2_U_Req):
                    O_Lp2_U_Req = SimulationData.O_Lp2_U_Req;
                    break;
                case nameof(SimulationData.O_Lp2_Ready):
                    O_Lp2_Ready = SimulationData.O_Lp2_Ready;
                    break;
                case nameof(SimulationData.O_Lp2_Ho_Avbl):
                    O_Lp2_Ho_Avbl = SimulationData.O_Lp2_Ho_Avbl;
                    break;
                case nameof(SimulationData.O_Lp2_Es):
                    O_Lp2_Es = SimulationData.O_Lp2_Es;
                    break;
                case nameof(SimulationData.I_Lp3_Valid):
                    I_Lp3_Valid = SimulationData.I_Lp3_Valid;
                    break;
                case nameof(SimulationData.I_Lp3_Cs_0):
                    I_Lp3_Cs_0 = SimulationData.I_Lp3_Cs_0;
                    break;
                case nameof(SimulationData.I_Lp3_Cs_1):
                    I_Lp3_Cs_1 = SimulationData.I_Lp3_Cs_1;
                    break;
                case nameof(SimulationData.I_Lp3_Tr_Req):
                    I_Lp3_Tr_Req = SimulationData.I_Lp3_Tr_Req;
                    break;
                case nameof(SimulationData.I_Lp3_Busy):
                    I_Lp3_Busy = SimulationData.I_Lp3_Busy;
                    break;
                case nameof(SimulationData.I_Lp3_Compt):
                    I_Lp3_Compt = SimulationData.I_Lp3_Compt;
                    break;
                case nameof(SimulationData.I_Lp3_Cont):
                    I_Lp3_Cont = SimulationData.I_Lp3_Cont;
                    break;
                case nameof(SimulationData.O_Lp3_L_Req):
                    O_Lp3_L_Req = SimulationData.O_Lp3_L_Req;
                    break;
                case nameof(SimulationData.O_Lp3_U_Req):
                    O_Lp3_U_Req = SimulationData.O_Lp3_U_Req;
                    break;
                case nameof(SimulationData.O_Lp3_Ready):
                    O_Lp3_Ready = SimulationData.O_Lp3_Ready;
                    break;
                case nameof(SimulationData.O_Lp3_Ho_Avbl):
                    O_Lp3_Ho_Avbl = SimulationData.O_Lp3_Ho_Avbl;
                    break;
                case nameof(SimulationData.O_Lp3_Es):
                    O_Lp3_Es = SimulationData.O_Lp3_Es;
                    break;
                case nameof(SimulationData.I_Lp4_Valid):
                    I_Lp4_Valid = SimulationData.I_Lp4_Valid;
                    break;
                case nameof(SimulationData.I_Lp4_Cs_0):
                    I_Lp4_Cs_0 = SimulationData.I_Lp4_Cs_0;
                    break;
                case nameof(SimulationData.I_Lp4_Cs_1):
                    I_Lp4_Cs_1 = SimulationData.I_Lp4_Cs_1;
                    break;
                case nameof(SimulationData.I_Lp4_Tr_Req):
                    I_Lp4_Tr_Req = SimulationData.I_Lp4_Tr_Req;
                    break;
                case nameof(SimulationData.I_Lp4_Busy):
                    I_Lp4_Busy = SimulationData.I_Lp4_Busy;
                    break;
                case nameof(SimulationData.I_Lp4_Compt):
                    I_Lp4_Compt = SimulationData.I_Lp4_Compt;
                    break;
                case nameof(SimulationData.I_Lp4_Cont):
                    I_Lp4_Cont = SimulationData.I_Lp4_Cont;
                    break;
                case nameof(SimulationData.O_Lp4_L_Req):
                    O_Lp4_L_Req = SimulationData.O_Lp4_L_Req;
                    break;
                case nameof(SimulationData.O_Lp4_U_Req):
                    O_Lp4_U_Req = SimulationData.O_Lp4_U_Req;
                    break;
                case nameof(SimulationData.O_Lp4_Ready):
                    O_Lp4_Ready = SimulationData.O_Lp4_Ready;
                    break;
                case nameof(SimulationData.O_Lp4_Ho_Avbl):
                    O_Lp4_Ho_Avbl = SimulationData.O_Lp4_Ho_Avbl;
                    break;
                case nameof(SimulationData.O_Lp4_Es):
                    O_Lp4_Es = SimulationData.O_Lp4_Es;
                    break;
                case nameof(SimulationData.I_Lp5_Valid):
                    I_Lp5_Valid = SimulationData.I_Lp5_Valid;
                    break;
                case nameof(SimulationData.I_Lp5_Cs_0):
                    I_Lp5_Cs_0 = SimulationData.I_Lp5_Cs_0;
                    break;
                case nameof(SimulationData.I_Lp5_Cs_1):
                    I_Lp5_Cs_1 = SimulationData.I_Lp5_Cs_1;
                    break;
                case nameof(SimulationData.I_Lp5_Tr_Req):
                    I_Lp5_Tr_Req = SimulationData.I_Lp5_Tr_Req;
                    break;
                case nameof(SimulationData.I_Lp5_Busy):
                    I_Lp5_Busy = SimulationData.I_Lp5_Busy;
                    break;
                case nameof(SimulationData.I_Lp5_Compt):
                    I_Lp5_Compt = SimulationData.I_Lp5_Compt;
                    break;
                case nameof(SimulationData.I_Lp5_Cont):
                    I_Lp5_Cont = SimulationData.I_Lp5_Cont;
                    break;
                case nameof(SimulationData.O_Lp5_L_Req):
                    O_Lp5_L_Req = SimulationData.O_Lp5_L_Req;
                    break;
                case nameof(SimulationData.O_Lp5_U_Req):
                    O_Lp5_U_Req = SimulationData.O_Lp5_U_Req;
                    break;
                case nameof(SimulationData.O_Lp5_Ready):
                    O_Lp5_Ready = SimulationData.O_Lp5_Ready;
                    break;
                case nameof(SimulationData.O_Lp5_Ho_Avbl):
                    O_Lp5_Ho_Avbl = SimulationData.O_Lp5_Ho_Avbl;
                    break;
                case nameof(SimulationData.O_Lp5_Es):
                    O_Lp5_Es = SimulationData.O_Lp5_Es;
                    break;
                case nameof(SimulationData.I_Lp6_Valid):
                    I_Lp6_Valid = SimulationData.I_Lp6_Valid;
                    break;
                case nameof(SimulationData.I_Lp6_Cs_0):
                    I_Lp6_Cs_0 = SimulationData.I_Lp6_Cs_0;
                    break;
                case nameof(SimulationData.I_Lp6_Cs_1):
                    I_Lp6_Cs_1 = SimulationData.I_Lp6_Cs_1;
                    break;
                case nameof(SimulationData.I_Lp6_Tr_Req):
                    I_Lp6_Tr_Req = SimulationData.I_Lp6_Tr_Req;
                    break;
                case nameof(SimulationData.I_Lp6_Busy):
                    I_Lp6_Busy = SimulationData.I_Lp6_Busy;
                    break;
                case nameof(SimulationData.I_Lp6_Compt):
                    I_Lp6_Compt = SimulationData.I_Lp6_Compt;
                    break;
                case nameof(SimulationData.I_Lp6_Cont):
                    I_Lp6_Cont = SimulationData.I_Lp6_Cont;
                    break;
                case nameof(SimulationData.O_Lp6_L_Req):
                    O_Lp6_L_Req = SimulationData.O_Lp6_L_Req;
                    break;
                case nameof(SimulationData.O_Lp6_U_Req):
                    O_Lp6_U_Req = SimulationData.O_Lp6_U_Req;
                    break;
                case nameof(SimulationData.O_Lp6_Ready):
                    O_Lp6_Ready = SimulationData.O_Lp6_Ready;
                    break;
                case nameof(SimulationData.O_Lp6_Ho_Avbl):
                    O_Lp6_Ho_Avbl = SimulationData.O_Lp6_Ho_Avbl;
                    break;
                case nameof(SimulationData.O_Lp6_Es):
                    O_Lp6_Es = SimulationData.O_Lp6_Es;
                    break;
                case nameof(SimulationData.I_Lp7_Valid):
                    I_Lp7_Valid = SimulationData.I_Lp7_Valid;
                    break;
                case nameof(SimulationData.I_Lp7_Cs_0):
                    I_Lp7_Cs_0 = SimulationData.I_Lp7_Cs_0;
                    break;
                case nameof(SimulationData.I_Lp7_Cs_1):
                    I_Lp7_Cs_1 = SimulationData.I_Lp7_Cs_1;
                    break;
                case nameof(SimulationData.I_Lp7_Tr_Req):
                    I_Lp7_Tr_Req = SimulationData.I_Lp7_Tr_Req;
                    break;
                case nameof(SimulationData.I_Lp7_Busy):
                    I_Lp7_Busy = SimulationData.I_Lp7_Busy;
                    break;
                case nameof(SimulationData.I_Lp7_Compt):
                    I_Lp7_Compt = SimulationData.I_Lp7_Compt;
                    break;
                case nameof(SimulationData.I_Lp7_Cont):
                    I_Lp7_Cont = SimulationData.I_Lp7_Cont;
                    break;
                case nameof(SimulationData.O_Lp7_L_Req):
                    O_Lp7_L_Req = SimulationData.O_Lp7_L_Req;
                    break;
                case nameof(SimulationData.O_Lp7_U_Req):
                    O_Lp7_U_Req = SimulationData.O_Lp7_U_Req;
                    break;
                case nameof(SimulationData.O_Lp7_Ready):
                    O_Lp7_Ready = SimulationData.O_Lp7_Ready;
                    break;
                case nameof(SimulationData.O_Lp7_Ho_Avbl):
                    O_Lp7_Ho_Avbl = SimulationData.O_Lp7_Ho_Avbl;
                    break;
                case nameof(SimulationData.O_Lp7_Es):
                    O_Lp7_Es = SimulationData.O_Lp7_Es;
                    break;
                case nameof(SimulationData.I_Lp8_Valid):
                    I_Lp8_Valid = SimulationData.I_Lp8_Valid;
                    break;
                case nameof(SimulationData.I_Lp8_Cs_0):
                    I_Lp8_Cs_0 = SimulationData.I_Lp8_Cs_0;
                    break;
                case nameof(SimulationData.I_Lp8_Cs_1):
                    I_Lp8_Cs_1 = SimulationData.I_Lp8_Cs_1;
                    break;
                case nameof(SimulationData.I_Lp8_Tr_Req):
                    I_Lp8_Tr_Req = SimulationData.I_Lp8_Tr_Req;
                    break;
                case nameof(SimulationData.I_Lp8_Busy):
                    I_Lp8_Busy = SimulationData.I_Lp8_Busy;
                    break;
                case nameof(SimulationData.I_Lp8_Compt):
                    I_Lp8_Compt = SimulationData.I_Lp8_Compt;
                    break;
                case nameof(SimulationData.I_Lp8_Cont):
                    I_Lp8_Cont = SimulationData.I_Lp8_Cont;
                    break;
                case nameof(SimulationData.O_Lp8_L_Req):
                    O_Lp8_L_Req = SimulationData.O_Lp8_L_Req;
                    break;
                case nameof(SimulationData.O_Lp8_U_Req):
                    O_Lp8_U_Req = SimulationData.O_Lp8_U_Req;
                    break;
                case nameof(SimulationData.O_Lp8_Ready):
                    O_Lp8_Ready = SimulationData.O_Lp8_Ready;
                    break;
                case nameof(SimulationData.O_Lp8_Ho_Avbl):
                    O_Lp8_Ho_Avbl = SimulationData.O_Lp8_Ho_Avbl;
                    break;
                case nameof(SimulationData.O_Lp8_Es):
                    O_Lp8_Es = SimulationData.O_Lp8_Es;
                    break;
            }

            Alarm = I_FAN1AlarmOccurred
                    || I_FAN2AlarmOccurred
                    || I_FAN3AlarmOccurred
                    || I_FAN4AlarmOccurred
                    || I_FAN5AlarmOccurred
                    || I_FAN6AlarmOccurred
                    || I_FAN7AlarmOccurred
                    || I_FAN8AlarmOccurred
                    || I_FAN9AlarmOccurred
                    || I_FAN10AlarmOccurred
                    || I_FAN11AlarmOccurred
                    || I_FAN12AlarmOccurred
                    || I_FAN13AlarmOccurred
                    || I_FAN14AlarmOccurred
                    || I_FAN15AlarmOccurred
                    || I_FAN16AlarmOccurred
                    || I_FAN17AlarmOccurred
                    || I_FAN18AlarmOccurred
                    || I_FAN19AlarmOccurred
                    || I_FAN20AlarmOccurred;
        }

        #endregion
    }
}
