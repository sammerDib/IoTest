using UnitsNet;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Simulation;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Simulation
{
    public class Dio0SimulationData : SimulationData
    {
        #region Constructor

        public Dio0SimulationData(Dio0 dio0)
            : base(dio0)
        {
        }

        #endregion

        #region Properties

        private bool _drvAlarm;

        public bool I_DvrAlarm
        {
            get => _drvAlarm;
            set => SetAndRaiseIfChanged(ref _drvAlarm, value, nameof(I_DvrAlarm));
        }

        #region FFU

        private RotationalSpeed _fanSpeed;

        public RotationalSpeed FanSpeed
        {
            get => _fanSpeed;
            set => SetAndRaiseIfChanged(ref _fanSpeed, value, nameof(FanSpeed));
        }

        private bool _fanDetection1;

        public bool I_FANDetection1
        {
            get => _fanDetection1;
            set => SetAndRaiseIfChanged(ref _fanDetection1, value, nameof(I_FANDetection1));
        }

        private bool _fanDetection2;

        public bool I_FANDetection2
        {
            get => _fanDetection2;
            set => SetAndRaiseIfChanged(ref _fanDetection2, value, nameof(I_FANDetection2));
        }

        #endregion FFU

        #region GPIO

        #region Inputs

        private bool _fan1Rotating;

        public bool I_FAN1Rotating
        {
            get => _fan1Rotating;
            set => SetAndRaiseIfChanged(ref _fan1Rotating, value, nameof(I_FAN1Rotating));
        }

        private bool _fan2Rotating;

        public bool I_FAN2Rotating
        {
            get => _fan2Rotating;
            set => SetAndRaiseIfChanged(ref _fan2Rotating, value, nameof(I_FAN2Rotating));
        }

        private bool _fan3Rotating;

        public bool I_FAN3Rotating
        {
            get => _fan3Rotating;
            set => SetAndRaiseIfChanged(ref _fan3Rotating, value, nameof(I_FAN3Rotating));
        }

        private bool _fan4Rotating;

        public bool I_FAN4Rotating
        {
            get => _fan4Rotating;
            set => SetAndRaiseIfChanged(ref _fan4Rotating, value, nameof(I_FAN4Rotating));
        }

        private bool _fan5Rotating;

        public bool I_FAN5Rotating
        {
            get => _fan5Rotating;
            set => SetAndRaiseIfChanged(ref _fan5Rotating, value, nameof(I_FAN5Rotating));
        }

        private bool _fan6Rotating;

        public bool I_FAN6Rotating
        {
            get => _fan6Rotating;
            set => SetAndRaiseIfChanged(ref _fan6Rotating, value, nameof(I_FAN6Rotating));
        }

        private bool _fan7Rotating;

        public bool I_FAN7Rotating
        {
            get => _fan7Rotating;
            set => SetAndRaiseIfChanged(ref _fan7Rotating, value, nameof(I_FAN7Rotating));
        }

        private bool _fan8Rotating;

        public bool I_FAN8Rotating
        {
            get => _fan8Rotating;
            set => SetAndRaiseIfChanged(ref _fan8Rotating, value, nameof(I_FAN8Rotating));
        }

        private bool _fan9Rotating;

        public bool I_FAN9Rotating
        {
            get => _fan9Rotating;
            set => SetAndRaiseIfChanged(ref _fan9Rotating, value, nameof(I_FAN9Rotating));
        }

        private bool _fan10Rotating;

        public bool I_FAN10Rotating
        {
            get => _fan10Rotating;
            set => SetAndRaiseIfChanged(ref _fan10Rotating, value, nameof(I_FAN10Rotating));
        }

        private bool _fan11Rotating;

        public bool I_FAN11Rotating
        {
            get => _fan11Rotating;
            set => SetAndRaiseIfChanged(ref _fan11Rotating, value, nameof(I_FAN11Rotating));
        }

        private bool _fan12Rotating;

        public bool I_FAN12Rotating
        {
            get => _fan12Rotating;
            set => SetAndRaiseIfChanged(ref _fan12Rotating, value, nameof(I_FAN12Rotating));
        }

        private bool _fan13Rotating;

        public bool I_FAN13Rotating
        {
            get => _fan13Rotating;
            set => SetAndRaiseIfChanged(ref _fan13Rotating, value, nameof(I_FAN13Rotating));
        }

        private bool _fan14Rotating;

        public bool I_FAN14Rotating
        {
            get => _fan14Rotating;
            set => SetAndRaiseIfChanged(ref _fan14Rotating, value, nameof(I_FAN14Rotating));
        }

        private bool _fan15Rotating;

        public bool I_FAN15Rotating
        {
            get => _fan15Rotating;
            set => SetAndRaiseIfChanged(ref _fan15Rotating, value, nameof(I_FAN15Rotating));
        }

        private bool _fan16Rotating;

        public bool I_FAN16Rotating
        {
            get => _fan16Rotating;
            set => SetAndRaiseIfChanged(ref _fan16Rotating, value, nameof(I_FAN16Rotating));
        }

        private bool _fan17Rotating;

        public bool I_FAN17Rotating
        {
            get => _fan17Rotating;
            set => SetAndRaiseIfChanged(ref _fan17Rotating, value, nameof(I_FAN17Rotating));
        }

        private bool _fan18Rotating;

        public bool I_FAN18Rotating
        {
            get => _fan18Rotating;
            set => SetAndRaiseIfChanged(ref _fan18Rotating, value, nameof(I_FAN18Rotating));
        }

        private bool _fan19Rotating;

        public bool I_FAN19Rotating
        {
            get => _fan19Rotating;
            set => SetAndRaiseIfChanged(ref _fan19Rotating, value, nameof(I_FAN19Rotating));
        }

        private bool _fan20Rotating;

        public bool I_FAN20Rotating
        {
            get => _fan20Rotating;
            set => SetAndRaiseIfChanged(ref _fan20Rotating, value, nameof(I_FAN20Rotating));
        }

        private bool _fan1AlarmOccured;

        public bool I_FAN1AlarmOccurred
        {
            get => _fan1AlarmOccured;
            set => SetAndRaiseIfChanged(ref _fan1AlarmOccured, value, nameof(I_FAN1AlarmOccurred));
        }

        private bool _fan2AlarmOccured;

        public bool I_FAN2AlarmOccurred
        {
            get => _fan2AlarmOccured;
            set => SetAndRaiseIfChanged(ref _fan2AlarmOccured, value, nameof(I_FAN2AlarmOccurred));
        }

        private bool _fan3AlarmOccured;

        public bool I_FAN3AlarmOccurred
        {
            get => _fan3AlarmOccured;
            set => SetAndRaiseIfChanged(ref _fan3AlarmOccured, value, nameof(I_FAN3AlarmOccurred));
        }

        private bool _fan4AlarmOccured;

        public bool I_FAN4AlarmOccurred
        {
            get => _fan4AlarmOccured;
            set => SetAndRaiseIfChanged(ref _fan4AlarmOccured, value, nameof(I_FAN4AlarmOccurred));
        }

        private bool _fan5AlarmOccured;

        public bool I_FAN5AlarmOccurred
        {
            get => _fan5AlarmOccured;
            set => SetAndRaiseIfChanged(ref _fan5AlarmOccured, value, nameof(I_FAN5AlarmOccurred));
        }

        private bool _fan6AlarmOccured;

        public bool I_FAN6AlarmOccurred
        {
            get => _fan6AlarmOccured;
            set => SetAndRaiseIfChanged(ref _fan6AlarmOccured, value, nameof(I_FAN6AlarmOccurred));
        }

        private bool _fan7AlarmOccured;

        public bool I_FAN7AlarmOccurred
        {
            get => _fan7AlarmOccured;
            set => SetAndRaiseIfChanged(ref _fan7AlarmOccured, value, nameof(I_FAN7AlarmOccurred));
        }

        private bool _fan8AlarmOccured;

        public bool I_FAN8AlarmOccurred
        {
            get => _fan8AlarmOccured;
            set => SetAndRaiseIfChanged(ref _fan8AlarmOccured, value, nameof(I_FAN8AlarmOccurred));
        }

        private bool _fan9AlarmOccured;

        public bool I_FAN9AlarmOccurred
        {
            get => _fan9AlarmOccured;
            set => SetAndRaiseIfChanged(ref _fan9AlarmOccured, value, nameof(I_FAN9AlarmOccurred));
        }

        private bool _fan10AlarmOccured;

        public bool I_FAN10AlarmOccurred
        {
            get => _fan10AlarmOccured;
            set
                => SetAndRaiseIfChanged(
                    ref _fan10AlarmOccured,
                    value,
                    nameof(I_FAN10AlarmOccurred));
        }

        private bool _fan11AlarmOccured;

        public bool I_FAN11AlarmOccurred
        {
            get => _fan11AlarmOccured;
            set
                => SetAndRaiseIfChanged(
                    ref _fan11AlarmOccured,
                    value,
                    nameof(I_FAN11AlarmOccurred));
        }

        private bool _fan12AlarmOccured;

        public bool I_FAN12AlarmOccurred
        {
            get => _fan12AlarmOccured;
            set
                => SetAndRaiseIfChanged(
                    ref _fan12AlarmOccured,
                    value,
                    nameof(I_FAN12AlarmOccurred));
        }

        private bool _fan13AlarmOccured;

        public bool I_FAN13AlarmOccurred
        {
            get => _fan13AlarmOccured;
            set
                => SetAndRaiseIfChanged(
                    ref _fan13AlarmOccured,
                    value,
                    nameof(I_FAN13AlarmOccurred));
        }

        private bool _fan14AlarmOccured;

        public bool I_FAN14AlarmOccurred
        {
            get => _fan14AlarmOccured;
            set
                => SetAndRaiseIfChanged(
                    ref _fan14AlarmOccured,
                    value,
                    nameof(I_FAN14AlarmOccurred));
        }

        private bool _fan15AlarmOccured;

        public bool I_FAN15AlarmOccurred
        {
            get => _fan15AlarmOccured;
            set
                => SetAndRaiseIfChanged(
                    ref _fan15AlarmOccured,
                    value,
                    nameof(I_FAN15AlarmOccurred));
        }

        private bool _fan16AlarmOccured;

        public bool I_FAN16AlarmOccurred
        {
            get => _fan16AlarmOccured;
            set
                => SetAndRaiseIfChanged(
                    ref _fan16AlarmOccured,
                    value,
                    nameof(I_FAN16AlarmOccurred));
        }

        private bool _fan17AlarmOccured;

        public bool I_FAN17AlarmOccurred
        {
            get => _fan17AlarmOccured;
            set
                => SetAndRaiseIfChanged(
                    ref _fan17AlarmOccured,
                    value,
                    nameof(I_FAN17AlarmOccurred));
        }

        private bool _fan18AlarmOccured;

        public bool I_FAN18AlarmOccurred
        {
            get => _fan18AlarmOccured;
            set
                => SetAndRaiseIfChanged(
                    ref _fan18AlarmOccured,
                    value,
                    nameof(I_FAN18AlarmOccurred));
        }

        private bool _fan19AlarmOccured;

        public bool I_FAN19AlarmOccurred
        {
            get => _fan19AlarmOccured;
            set
                => SetAndRaiseIfChanged(
                    ref _fan19AlarmOccured,
                    value,
                    nameof(I_FAN19AlarmOccurred));
        }

        private bool _fan20AlarmOccured;

        public bool I_FAN20AlarmOccurred
        {
            get => _fan20AlarmOccured;
            set
                => SetAndRaiseIfChanged(
                    ref _fan20AlarmOccured,
                    value,
                    nameof(I_FAN20AlarmOccurred));
        }

        private bool _sensor1WithinUpperLimitThresholdValue;

        public bool I_Sensor1_WithinUpperLimitThresholdValue
        {
            get => _sensor1WithinUpperLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor1WithinUpperLimitThresholdValue,
                    value,
                    nameof(I_Sensor1_WithinUpperLimitThresholdValue));
        }

        private bool _sensor1WithinLowerLimitThresholdValue;

        public bool I_Sensor1_WithinLowerLimitThresholdValue
        {
            get => _sensor1WithinLowerLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor1WithinLowerLimitThresholdValue,
                    value,
                    nameof(I_Sensor1_WithinLowerLimitThresholdValue));
        }

        private bool _sensor2WithinUpperLimitThresholdValue;

        public bool I_Sensor2_WithinUpperLimitThresholdValue
        {
            get => _sensor2WithinUpperLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor2WithinUpperLimitThresholdValue,
                    value,
                    nameof(I_Sensor2_WithinUpperLimitThresholdValue));
        }

        private bool _sensor2WithinLowerLimitThresholdValue;

        public bool I_Sensor2_WithinLowerLimitThresholdValue
        {
            get => _sensor2WithinLowerLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor2WithinLowerLimitThresholdValue,
                    value,
                    nameof(I_Sensor2_WithinLowerLimitThresholdValue));
        }

        private bool _sensor3WithinUpperLimitThresholdValue;

        public bool I_Sensor3_WithinUpperLimitThresholdValue
        {
            get => _sensor3WithinUpperLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor3WithinUpperLimitThresholdValue,
                    value,
                    nameof(I_Sensor3_WithinUpperLimitThresholdValue));
        }

        private bool _sensor3WithinLowerLimitThresholdValue;

        public bool I_Sensor3_WithinLowerLimitThresholdValue
        {
            get => _sensor3WithinLowerLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor3WithinLowerLimitThresholdValue,
                    value,
                    nameof(I_Sensor3_WithinLowerLimitThresholdValue));
        }

        private bool _sensor4WithinUpperLimitThresholdValue;

        public bool I_Sensor4_WithinUpperLimitThresholdValue
        {
            get => _sensor4WithinUpperLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor4WithinUpperLimitThresholdValue,
                    value,
                    nameof(I_Sensor4_WithinUpperLimitThresholdValue));
        }

        private bool _sensor4WithinLowerLimitThresholdValue;

        public bool I_Sensor4_WithinLowerLimitThresholdValue
        {
            get => _sensor4WithinLowerLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor4WithinLowerLimitThresholdValue,
                    value,
                    nameof(I_Sensor4_WithinLowerLimitThresholdValue));
        }

        private bool _sensor5WithinUpperLimitThresholdValue;

        public bool I_Sensor5_WithinUpperLimitThresholdValue
        {
            get => _sensor5WithinUpperLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor5WithinUpperLimitThresholdValue,
                    value,
                    nameof(I_Sensor5_WithinUpperLimitThresholdValue));
        }

        private bool _sensor5WithinLowerLimitThresholdValue;

        public bool I_Sensor5_WithinLowerLimitThresholdValue
        {
            get => _sensor5WithinLowerLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor5WithinLowerLimitThresholdValue,
                    value,
                    nameof(I_Sensor5_WithinLowerLimitThresholdValue));
        }

        private bool _sensor6WithinUpperLimitThresholdValue;

        public bool I_Sensor6_WithinUpperLimitThresholdValue
        {
            get => _sensor6WithinUpperLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor6WithinUpperLimitThresholdValue,
                    value,
                    nameof(I_Sensor6_WithinUpperLimitThresholdValue));
        }

        private bool _sensor6WithinLowerLimitThresholdValue;

        public bool I_Sensor6_WithinLowerLimitThresholdValue
        {
            get => _sensor6WithinLowerLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor6WithinLowerLimitThresholdValue,
                    value,
                    nameof(I_Sensor6_WithinLowerLimitThresholdValue));
        }

        private bool _sensor7WithinUpperLimitThresholdValue;

        public bool I_Sensor7_WithinUpperLimitThresholdValue
        {
            get => _sensor7WithinUpperLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor7WithinUpperLimitThresholdValue,
                    value,
                    nameof(I_Sensor7_WithinUpperLimitThresholdValue));
        }

        private bool _sensor7WithinLowerLimitThresholdValue;

        public bool I_Sensor7_WithinLowerLimitThresholdValue
        {
            get => _sensor7WithinLowerLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor7WithinLowerLimitThresholdValue,
                    value,
                    nameof(I_Sensor7_WithinLowerLimitThresholdValue));
        }

        private bool _sensor8WithinUpperLimitThresholdValue;

        public bool I_Sensor8_WithinUpperLimitThresholdValue
        {
            get => _sensor8WithinUpperLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor8WithinUpperLimitThresholdValue,
                    value,
                    nameof(I_Sensor8_WithinUpperLimitThresholdValue));
        }

        private bool _sensor8WithinLowerLimitThresholdValue;

        public bool I_Sensor8_WithinLowerLimitThresholdValue
        {
            get => _sensor8WithinLowerLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor8WithinLowerLimitThresholdValue,
                    value,
                    nameof(I_Sensor8_WithinLowerLimitThresholdValue));
        }

        private bool _sensor9WithinUpperLimitThresholdValue;

        public bool I_Sensor9_WithinUpperLimitThresholdValue
        {
            get => _sensor9WithinUpperLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor9WithinUpperLimitThresholdValue,
                    value,
                    nameof(I_Sensor9_WithinUpperLimitThresholdValue));
        }

        private bool _sensor9WithinLowerLimitThresholdValue;

        public bool I_Sensor9_WithinLowerLimitThresholdValue
        {
            get => _sensor9WithinLowerLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor9WithinLowerLimitThresholdValue,
                    value,
                    nameof(I_Sensor9_WithinLowerLimitThresholdValue));
        }

        private bool _sensor10WithinUpperLimitThresholdValue;

        public bool I_Sensor10_WithinUpperLimitThresholdValue
        {
            get => _sensor10WithinUpperLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor10WithinUpperLimitThresholdValue,
                    value,
                    nameof(I_Sensor10_WithinUpperLimitThresholdValue));
        }

        private bool _sensor10WithinLowerLimitThresholdValue;

        public bool I_Sensor10_WithinLowerLimitThresholdValue
        {
            get => _sensor10WithinLowerLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor10WithinLowerLimitThresholdValue,
                    value,
                    nameof(I_Sensor10_WithinLowerLimitThresholdValue));
        }

        private bool _sensor11WithinUpperLimitThresholdValue;

        public bool I_Sensor11_WithinUpperLimitThresholdValue
        {
            get => _sensor11WithinUpperLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor11WithinUpperLimitThresholdValue,
                    value,
                    nameof(I_Sensor11_WithinUpperLimitThresholdValue));
        }

        private bool _sensor11WithinLowerLimitThresholdValue;

        public bool I_Sensor11_WithinLowerLimitThresholdValue
        {
            get => _sensor11WithinLowerLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor11WithinLowerLimitThresholdValue,
                    value,
                    nameof(I_Sensor11_WithinLowerLimitThresholdValue));
        }

        private bool _sensor12WithinUpperLimitThresholdValue;

        public bool I_Sensor12_WithinUpperLimitThresholdValue
        {
            get => _sensor12WithinUpperLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor12WithinUpperLimitThresholdValue,
                    value,
                    nameof(I_Sensor12_WithinUpperLimitThresholdValue));
        }

        private bool _sensor12WithinLowerLimitThresholdValue;

        public bool I_Sensor12_WithinLowerLimitThresholdValue
        {
            get => _sensor12WithinLowerLimitThresholdValue;
            set
                => SetAndRaiseIfChanged(
                    ref _sensor12WithinLowerLimitThresholdValue,
                    value,
                    nameof(I_Sensor12_WithinLowerLimitThresholdValue));
        }

        private bool _controllerDirectInputIn0;

        public bool I_ControllerDirectInput_IN0
        {
            get => _controllerDirectInputIn0;
            set
                => SetAndRaiseIfChanged(
                    ref _controllerDirectInputIn0,
                    value,
                    nameof(I_ControllerDirectInput_IN0));
        }

        private bool _controllerDirectInputIn1;

        public bool I_ControllerDirectInput_IN1
        {
            get => _controllerDirectInputIn1;
            set
                => SetAndRaiseIfChanged(
                    ref _controllerDirectInputIn1,
                    value,
                    nameof(I_ControllerDirectInput_IN1));
        }

        private bool _controllerDirectInputIn2;

        public bool I_ControllerDirectInput_IN2
        {
            get => _controllerDirectInputIn2;
            set
                => SetAndRaiseIfChanged(
                    ref _controllerDirectInputIn2,
                    value,
                    nameof(I_ControllerDirectInput_IN2));
        }

        private bool _controllerDirectInputIn3;

        public bool I_ControllerDirectInput_IN3
        {
            get => _controllerDirectInputIn3;
            set
                => SetAndRaiseIfChanged(
                    ref _controllerDirectInputIn3,
                    value,
                    nameof(I_ControllerDirectInput_IN3));
        }

        #endregion Inputs

        #region Outputs

        private bool _systemIsReady;
        public bool O_SystemIsReady
        {
            get => _systemIsReady;
            set => SetAndRaiseIfChanged(ref _systemIsReady, value, nameof(O_SystemIsReady));
        }

        private bool _batchAlarmClear1ShotOutput;
        public bool O_BatchAlarmClear_1ShotOutput
        {
            get => _batchAlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _batchAlarmClear1ShotOutput,
                    value,
                    nameof(O_BatchAlarmClear_1ShotOutput));
        }

        private bool _fanOperationStopAllUsingFans1ShotOutput;
        public bool O_Fan_OperationStop_AllUsingFans_1ShotOutput
        {
            get => _fanOperationStopAllUsingFans1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fanOperationStopAllUsingFans1ShotOutput,
                    value,
                    nameof(O_Fan_OperationStop_AllUsingFans_1ShotOutput));
        }

        private bool _fanOperationStartAllUsingFans1ShotOutput;
        public bool O_Fan_OperationStart_AllUsingFans_1ShotOutput
        {
            get => _fanOperationStartAllUsingFans1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fanOperationStartAllUsingFans1ShotOutput,
                    value,
                    nameof(O_Fan_OperationStart_AllUsingFans_1ShotOutput));
        }

        private bool _fan1OperationStart1ShotOutput;
        public bool O_FAN1_OperationStart_1ShotOutput
        {
            get => _fan1OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan1OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN1_OperationStart_1ShotOutput));
        }

        private bool _fan2OperationStart1ShotOutput;
        public bool O_FAN2_OperationStart_1ShotOutput
        {
            get => _fan2OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan2OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN2_OperationStart_1ShotOutput));
        }

        private bool _fan3OperationStart1ShotOutput;
        public bool O_FAN3_OperationStart_1ShotOutput
        {
            get => _fan3OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan3OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN3_OperationStart_1ShotOutput));
        }

        private bool _fan4OperationStart1ShotOutput;
        public bool O_FAN4_OperationStart_1ShotOutput
        {
            get => _fan4OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan4OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN4_OperationStart_1ShotOutput));
        }

        private bool _fan5OperationStart1ShotOutput;
        public bool O_FAN5_OperationStart_1ShotOutput
        {
            get => _fan5OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan5OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN5_OperationStart_1ShotOutput));
        }

        private bool _fan6OperationStart1ShotOutput;
        public bool O_FAN6_OperationStart_1ShotOutput
        {
            get => _fan6OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan6OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN6_OperationStart_1ShotOutput));
        }

        private bool _fan7OperationStart1ShotOutput;
        public bool O_FAN7_OperationStart_1ShotOutput
        {
            get => _fan7OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan7OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN7_OperationStart_1ShotOutput));
        }

        private bool _fan8OperationStart1ShotOutput;
        public bool O_FAN8_OperationStart_1ShotOutput
        {
            get => _fan8OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan8OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN8_OperationStart_1ShotOutput));
        }

        private bool _fan9OperationStart1ShotOutput;
        public bool O_FAN9_OperationStart_1ShotOutput
        {
            get => _fan9OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan9OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN9_OperationStart_1ShotOutput));
        }

        private bool _fan10OperationStart1ShotOutput;
        public bool O_FAN10_OperationStart_1ShotOutput
        {
            get => _fan10OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan10OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN10_OperationStart_1ShotOutput));
        }

        private bool _fan11OperationStart1ShotOutput;
        public bool O_FAN11_OperationStart_1ShotOutput
        {
            get => _fan11OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan11OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN11_OperationStart_1ShotOutput));
        }

        private bool _fan12OperationStart1ShotOutput;
        public bool O_FAN12_OperationStart_1ShotOutput
        {
            get => _fan12OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan12OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN12_OperationStart_1ShotOutput));
        }

        private bool _fan13OperationStart1ShotOutput;
        public bool O_FAN13_OperationStart_1ShotOutput
        {
            get => _fan13OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan13OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN13_OperationStart_1ShotOutput));
        }

        private bool _fan14OperationStart1ShotOutput;
        public bool O_FAN14_OperationStart_1ShotOutput
        {
            get => _fan14OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan14OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN14_OperationStart_1ShotOutput));
        }

        private bool _fan15OperationStart1ShotOutput;
        public bool O_FAN15_OperationStart_1ShotOutput
        {
            get => _fan15OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan15OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN15_OperationStart_1ShotOutput));
        }

        private bool _fan16OperationStart1ShotOutput;
        public bool O_FAN16_OperationStart_1ShotOutput
        {
            get => _fan16OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan16OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN16_OperationStart_1ShotOutput));
        }

        private bool _fan17OperationStart1ShotOutput;
        public bool O_FAN17_OperationStart_1ShotOutput
        {
            get => _fan17OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan17OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN17_OperationStart_1ShotOutput));
        }

        private bool _fan18OperationStart1ShotOutput;
        public bool O_FAN18_OperationStart_1ShotOutput
        {
            get => _fan18OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan18OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN18_OperationStart_1ShotOutput));
        }

        private bool _fan19OperationStart1ShotOutput;
        public bool O_FAN19_OperationStart_1ShotOutput
        {
            get => _fan19OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan19OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN19_OperationStart_1ShotOutput));
        }

        private bool _fan20OperationStart1ShotOutput;
        public bool O_FAN20_OperationStart_1ShotOutput
        {
            get => _fan20OperationStart1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan20OperationStart1ShotOutput,
                    value,
                    nameof(O_FAN20_OperationStart_1ShotOutput));
        }

        private bool _fan1AlarmClear1ShotOutput;
        public bool O_FAN1_AlarmClear_1ShotOutput
        {
            get => _fan1AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan1AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN1_AlarmClear_1ShotOutput));
        }

        private bool _fan2AlarmClear1ShotOutput;
        public bool O_FAN2_AlarmClear_1ShotOutput
        {
            get => _fan2AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan2AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN2_AlarmClear_1ShotOutput));
        }

        private bool _fan3AlarmClear1ShotOutput;
        public bool O_FAN3_AlarmClear_1ShotOutput
        {
            get => _fan3AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan3AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN3_AlarmClear_1ShotOutput));
        }

        private bool _fan4AlarmClear1ShotOutput;
        public bool O_FAN4_AlarmClear_1ShotOutput
        {
            get => _fan4AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan4AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN4_AlarmClear_1ShotOutput));
        }

        private bool _fan5AlarmClear1ShotOutput;
        public bool O_FAN5_AlarmClear_1ShotOutput
        {
            get => _fan5AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan5AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN5_AlarmClear_1ShotOutput));
        }

        private bool _fan6AlarmClear1ShotOutput;
        public bool O_FAN6_AlarmClear_1ShotOutput
        {
            get => _fan6AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan6AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN6_AlarmClear_1ShotOutput));
        }

        private bool _fan7AlarmClear1ShotOutput;
        public bool O_FAN7_AlarmClear_1ShotOutput
        {
            get => _fan7AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan7AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN7_AlarmClear_1ShotOutput));
        }

        private bool _fan8AlarmClear1ShotOutput;
        public bool O_FAN8_AlarmClear_1ShotOutput
        {
            get => _fan8AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan8AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN8_AlarmClear_1ShotOutput));
        }

        private bool _fan9AlarmClear1ShotOutput;
        public bool O_FAN9_AlarmClear_1ShotOutput
        {
            get => _fan9AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan9AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN9_AlarmClear_1ShotOutput));
        }

        private bool _fan10AlarmClear1ShotOutput;
        public bool O_FAN10_AlarmClear_1ShotOutput
        {
            get => _fan10AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan10AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN10_AlarmClear_1ShotOutput));
        }

        private bool _fan11AlarmClear1ShotOutput;
        public bool O_FAN11_AlarmClear_1ShotOutput
        {
            get => _fan11AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan11AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN11_AlarmClear_1ShotOutput));
        }

        private bool _fan12AlarmClear1ShotOutput;
        public bool O_FAN12_AlarmClear_1ShotOutput
        {
            get => _fan12AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan12AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN12_AlarmClear_1ShotOutput));
        }

        private bool _fan13AlarmClear1ShotOutput;
        public bool O_FAN13_AlarmClear_1ShotOutput
        {
            get => _fan13AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan13AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN13_AlarmClear_1ShotOutput));
        }

        private bool _fan14AlarmClear1ShotOutput;
        public bool O_FAN14_AlarmClear_1ShotOutput
        {
            get => _fan14AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan14AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN14_AlarmClear_1ShotOutput));
        }

        private bool _fan15AlarmClear1ShotOutput;
        public bool O_FAN15_AlarmClear_1ShotOutput
        {
            get => _fan15AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan15AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN15_AlarmClear_1ShotOutput));
        }

        private bool _fan16AlarmClear1ShotOutput;
        public bool O_FAN16_AlarmClear_1ShotOutput
        {
            get => _fan16AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan16AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN16_AlarmClear_1ShotOutput));
        }

        private bool _fan17AlarmClear1ShotOutput;
        public bool O_FAN17_AlarmClear_1ShotOutput
        {
            get => _fan17AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan17AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN17_AlarmClear_1ShotOutput));
        }

        private bool _fan18AlarmClear1ShotOutput;
        public bool O_FAN18_AlarmClear_1ShotOutput
        {
            get => _fan18AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan18AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN18_AlarmClear_1ShotOutput));
        }

        private bool _fan19AlarmClear1ShotOutput;
        public bool O_FAN19_AlarmClear_1ShotOutput
        {
            get => _fan19AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan19AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN19_AlarmClear_1ShotOutput));
        }

        private bool _fan20AlarmClear1ShotOutput;
        public bool O_FAN20_AlarmClear_1ShotOutput
        {
            get => _fan20AlarmClear1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan20AlarmClear1ShotOutput,
                    value,
                    nameof(O_FAN20_AlarmClear_1ShotOutput));
        }

        private bool _fan1OperationStop1ShotOutput;
        public bool O_FAN1_OperationStop_1ShotOutput
        {
            get => _fan1OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan1OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN1_OperationStop_1ShotOutput));
        }

        private bool _fan2OperationStop1ShotOutput;
        public bool O_FAN2_OperationStop_1ShotOutput
        {
            get => _fan2OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan2OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN2_OperationStop_1ShotOutput));
        }

        private bool _fan3OperationStop1ShotOutput;
        public bool O_FAN3_OperationStop_1ShotOutput
        {
            get => _fan3OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan3OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN3_OperationStop_1ShotOutput));
        }

        private bool _fan4OperationStop1ShotOutput;
        public bool O_FAN4_OperationStop_1ShotOutput
        {
            get => _fan4OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan4OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN4_OperationStop_1ShotOutput));
        }

        private bool _fan5OperationStop1ShotOutput;
        public bool O_FAN5_OperationStop_1ShotOutput
        {
            get => _fan5OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan5OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN5_OperationStop_1ShotOutput));
        }

        private bool _fan6OperationStop1ShotOutput;
        public bool O_FAN6_OperationStop_1ShotOutput
        {
            get => _fan6OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan6OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN6_OperationStop_1ShotOutput));
        }

        private bool _fan7OperationStop1ShotOutput;
        public bool O_FAN7_OperationStop_1ShotOutput
        {
            get => _fan7OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan7OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN7_OperationStop_1ShotOutput));
        }

        private bool _fan8OperationStop1ShotOutput;
        public bool O_FAN8_OperationStop_1ShotOutput
        {
            get => _fan8OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan8OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN8_OperationStop_1ShotOutput));
        }

        private bool _fan9OperationStop1ShotOutput;
        public bool O_FAN9_OperationStop_1ShotOutput
        {
            get => _fan9OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan9OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN9_OperationStop_1ShotOutput));
        }

        private bool _fan10OperationStop1ShotOutput;
        public bool O_FAN10_OperationStop_1ShotOutput
        {
            get => _fan10OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan10OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN10_OperationStop_1ShotOutput));
        }

        private bool _fan11OperationStop1ShotOutput;
        public bool O_FAN11_OperationStop_1ShotOutput
        {
            get => _fan11OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan11OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN11_OperationStop_1ShotOutput));
        }

        private bool _fan12OperationStop1ShotOutput;
        public bool O_FAN12_OperationStop_1ShotOutput
        {
            get => _fan12OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan12OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN12_OperationStop_1ShotOutput));
        }

        private bool _fan13OperationStop1ShotOutput;
        public bool O_FAN13_OperationStop_1ShotOutput
        {
            get => _fan13OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan13OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN13_OperationStop_1ShotOutput));
        }

        private bool _fan14OperationStop1ShotOutput;
        public bool O_FAN14_OperationStop_1ShotOutput
        {
            get => _fan14OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan14OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN14_OperationStop_1ShotOutput));
        }

        private bool _fan15OperationStop1ShotOutput;
        public bool O_FAN15_OperationStop_1ShotOutput
        {
            get => _fan15OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan15OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN15_OperationStop_1ShotOutput));
        }

        private bool _fan16OperationStop1ShotOutput;
        public bool O_FAN16_OperationStop_1ShotOutput
        {
            get => _fan16OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan16OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN16_OperationStop_1ShotOutput));
        }

        private bool _fan17OperationStop1ShotOutput;
        public bool O_FAN17_OperationStop_1ShotOutput
        {
            get => _fan17OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan17OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN17_OperationStop_1ShotOutput));
        }

        private bool _fan18OperationStop1ShotOutput;
        public bool O_FAN18_OperationStop_1ShotOutput
        {
            get => _fan18OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan18OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN18_OperationStop_1ShotOutput));
        }

        private bool _fan19OperationStop1ShotOutput;
        public bool O_FAN19_OperationStop_1ShotOutput
        {
            get => _fan19OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan19OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN19_OperationStop_1ShotOutput));
        }

        private bool _fan20OperationStop1ShotOutput;
        public bool O_FAN20_OperationStop_1ShotOutput
        {
            get => _fan20OperationStop1ShotOutput;
            set
                => SetAndRaiseIfChanged(
                    ref _fan20OperationStop1ShotOutput,
                    value,
                    nameof(O_FAN20_OperationStop_1ShotOutput));
        }

        #endregion Outputs

        #endregion GPIO

        #region E84

        #region Inputs

        #region LP1

        private bool _lp1Valid;
        public bool I_Lp1_Valid
        {
            get => _lp1Valid;
            set => SetAndRaiseIfChanged(ref _lp1Valid, value, nameof(I_Lp1_Valid));
        }

        private bool _lp1Cs0;
        public bool I_Lp1_Cs_0
        {
            get => _lp1Cs0;
            set => SetAndRaiseIfChanged(ref _lp1Cs0, value, nameof(I_Lp1_Cs_0));
        }

        private bool _lp1Cs1;
        public bool I_Lp1_Cs_1
        {
            get => _lp1Cs1;
            set => SetAndRaiseIfChanged(ref _lp1Cs1, value, nameof(I_Lp1_Cs_1));
        }

        private bool _lp1TrReq;
        public bool I_Lp1_Tr_Req
        {
            get => _lp1TrReq;
            set => SetAndRaiseIfChanged(ref _lp1TrReq, value, nameof(I_Lp1_Tr_Req));
        }

        private bool _lp1Busy;
        public bool I_Lp1_Busy
        {
            get => _lp1Busy;
            set => SetAndRaiseIfChanged(ref _lp1Busy, value, nameof(I_Lp1_Busy));
        }

        private bool _lp1Compt;
        public bool I_Lp1_Compt
        {
            get => _lp1Compt;
            set => SetAndRaiseIfChanged(ref _lp1Compt, value, nameof(I_Lp1_Compt));
        }

        private bool _lp1Cont;
        public bool I_Lp1_Cont
        {
            get => _lp1Cont;
            set => SetAndRaiseIfChanged(ref _lp1Cont, value, nameof(I_Lp1_Cont));
        }

        #endregion LP1

        #region LP2

        private bool _lp2Valid;
        public bool I_Lp2_Valid
        {
            get => _lp2Valid;
            set => SetAndRaiseIfChanged(ref _lp2Valid, value, nameof(I_Lp2_Valid));
        }

        private bool _lp2Cs0;
        public bool I_Lp2_Cs_0
        {
            get => _lp2Cs0;
            set => SetAndRaiseIfChanged(ref _lp2Cs0, value, nameof(I_Lp2_Cs_0));
        }

        private bool _lp2Cs1;
        public bool I_Lp2_Cs_1
        {
            get => _lp2Cs1;
            set => SetAndRaiseIfChanged(ref _lp2Cs1, value, nameof(I_Lp2_Cs_1));
        }

        private bool _lp2TrReq;
        public bool I_Lp2_Tr_Req
        {
            get => _lp2TrReq;
            set => SetAndRaiseIfChanged(ref _lp2TrReq, value, nameof(I_Lp2_Tr_Req));
        }

        private bool _lp2Busy;
        public bool I_Lp2_Busy
        {
            get => _lp2Busy;
            set => SetAndRaiseIfChanged(ref _lp2Busy, value, nameof(I_Lp2_Busy));
        }

        private bool _lp2Compt;
        public bool I_Lp2_Compt
        {
            get => _lp2Compt;
            set => SetAndRaiseIfChanged(ref _lp2Compt, value, nameof(I_Lp2_Compt));
        }

        private bool _lp2Cont;
        public bool I_Lp2_Cont
        {
            get => _lp2Cont;
            set => SetAndRaiseIfChanged(ref _lp2Cont, value, nameof(I_Lp2_Cont));
        }

        #endregion LP2

        #region LP3

        private bool _lp3Valid;
        public bool I_Lp3_Valid
        {
            get => _lp3Valid;
            set => SetAndRaiseIfChanged(ref _lp3Valid, value, nameof(I_Lp3_Valid));
        }

        private bool _lp3Cs0;
        public bool I_Lp3_Cs_0
        {
            get => _lp3Cs0;
            set => SetAndRaiseIfChanged(ref _lp3Cs0, value, nameof(I_Lp3_Cs_0));
        }

        private bool _lp3Cs1;
        public bool I_Lp3_Cs_1
        {
            get => _lp3Cs1;
            set => SetAndRaiseIfChanged(ref _lp3Cs1, value, nameof(I_Lp3_Cs_1));
        }

        private bool _lp3TrReq;
        public bool I_Lp3_Tr_Req
        {
            get => _lp3TrReq;
            set => SetAndRaiseIfChanged(ref _lp3TrReq, value, nameof(I_Lp3_Tr_Req));
        }

        private bool _lp3Busy;
        public bool I_Lp3_Busy
        {
            get => _lp3Busy;
            set => SetAndRaiseIfChanged(ref _lp3Busy, value, nameof(I_Lp3_Busy));
        }

        private bool _lp3Compt;
        public bool I_Lp3_Compt
        {
            get => _lp3Compt;
            set => SetAndRaiseIfChanged(ref _lp3Compt, value, nameof(I_Lp3_Compt));
        }

        private bool _lp3Cont;
        public bool I_Lp3_Cont
        {
            get => _lp3Cont;
            set => SetAndRaiseIfChanged(ref _lp3Cont, value, nameof(I_Lp3_Cont));
        }

        #endregion LP3

        #region LP4

        private bool _lp4Valid;
        public bool I_Lp4_Valid
        {
            get => _lp4Valid;
            set => SetAndRaiseIfChanged(ref _lp4Valid, value, nameof(I_Lp4_Valid));
        }

        private bool _lp4Cs0;
        public bool I_Lp4_Cs_0
        {
            get => _lp4Cs0;
            set => SetAndRaiseIfChanged(ref _lp4Cs0, value, nameof(I_Lp4_Cs_0));
        }

        private bool _lp4Cs1;
        public bool I_Lp4_Cs_1
        {
            get => _lp4Cs1;
            set => SetAndRaiseIfChanged(ref _lp4Cs1, value, nameof(I_Lp4_Cs_1));
        }

        private bool _lp4TrReq;
        public bool I_Lp4_Tr_Req
        {
            get => _lp4TrReq;
            set => SetAndRaiseIfChanged(ref _lp4TrReq, value, nameof(I_Lp4_Tr_Req));
        }

        private bool _lp4Busy;
        public bool I_Lp4_Busy
        {
            get => _lp4Busy;
            set => SetAndRaiseIfChanged(ref _lp4Busy, value, nameof(I_Lp4_Busy));
        }

        private bool _lp4Compt;
        public bool I_Lp4_Compt
        {
            get => _lp4Compt;
            set => SetAndRaiseIfChanged(ref _lp4Compt, value, nameof(I_Lp4_Compt));
        }

        private bool _lp4Cont;
        public bool I_Lp4_Cont
        {
            get => _lp4Cont;
            set => SetAndRaiseIfChanged(ref _lp4Cont, value, nameof(I_Lp4_Cont));
        }

        #endregion LP4

        #region LP5

        private bool _lp5Valid;
        public bool I_Lp5_Valid
        {
            get => _lp5Valid;
            set => SetAndRaiseIfChanged(ref _lp5Valid, value, nameof(I_Lp5_Valid));
        }

        private bool _lp5Cs0;
        public bool I_Lp5_Cs_0
        {
            get => _lp5Cs0;
            set => SetAndRaiseIfChanged(ref _lp5Cs0, value, nameof(I_Lp5_Cs_0));
        }

        private bool _lp5Cs1;
        public bool I_Lp5_Cs_1
        {
            get => _lp5Cs1;
            set => SetAndRaiseIfChanged(ref _lp5Cs1, value, nameof(I_Lp5_Cs_1));
        }

        private bool _lp5TrReq;
        public bool I_Lp5_Tr_Req
        {
            get => _lp5TrReq;
            set => SetAndRaiseIfChanged(ref _lp5TrReq, value, nameof(I_Lp5_Tr_Req));
        }

        private bool _lp5Busy;
        public bool I_Lp5_Busy
        {
            get => _lp5Busy;
            set => SetAndRaiseIfChanged(ref _lp5Busy, value, nameof(I_Lp5_Busy));
        }

        private bool _lp5Compt;
        public bool I_Lp5_Compt
        {
            get => _lp5Compt;
            set => SetAndRaiseIfChanged(ref _lp5Compt, value, nameof(I_Lp5_Compt));
        }

        private bool _lp5Cont;
        public bool I_Lp5_Cont
        {
            get => _lp5Cont;
            set => SetAndRaiseIfChanged(ref _lp5Cont, value, nameof(I_Lp5_Cont));
        }

        #endregion LP5

        #region LP6

        private bool _lp6Valid;
        public bool I_Lp6_Valid
        {
            get => _lp6Valid;
            set => SetAndRaiseIfChanged(ref _lp6Valid, value, nameof(I_Lp6_Valid));
        }

        private bool _lp6Cs0;
        public bool I_Lp6_Cs_0
        {
            get => _lp6Cs0;
            set => SetAndRaiseIfChanged(ref _lp6Cs0, value, nameof(I_Lp6_Cs_0));
        }

        private bool _lp6Cs1;
        public bool I_Lp6_Cs_1
        {
            get => _lp6Cs1;
            set => SetAndRaiseIfChanged(ref _lp6Cs1, value, nameof(I_Lp6_Cs_1));
        }

        private bool _lp6TrReq;
        public bool I_Lp6_Tr_Req
        {
            get => _lp6TrReq;
            set => SetAndRaiseIfChanged(ref _lp6TrReq, value, nameof(I_Lp6_Tr_Req));
        }

        private bool _lp6Busy;
        public bool I_Lp6_Busy
        {
            get => _lp6Busy;
            set => SetAndRaiseIfChanged(ref _lp6Busy, value, nameof(I_Lp6_Busy));
        }

        private bool _lp6Compt;
        public bool I_Lp6_Compt
        {
            get => _lp6Compt;
            set => SetAndRaiseIfChanged(ref _lp6Compt, value, nameof(I_Lp6_Compt));
        }

        private bool _lp6Cont;
        public bool I_Lp6_Cont
        {
            get => _lp6Cont;
            set => SetAndRaiseIfChanged(ref _lp6Cont, value, nameof(I_Lp6_Cont));
        }

        #endregion LP6

        #region LP7

        private bool _lp7Valid;
        public bool I_Lp7_Valid
        {
            get => _lp7Valid;
            set => SetAndRaiseIfChanged(ref _lp7Valid, value, nameof(I_Lp7_Valid));
        }

        private bool _lp7Cs0;
        public bool I_Lp7_Cs_0
        {
            get => _lp7Cs0;
            set => SetAndRaiseIfChanged(ref _lp7Cs0, value, nameof(I_Lp7_Cs_0));
        }

        private bool _lp7Cs1;
        public bool I_Lp7_Cs_1
        {
            get => _lp7Cs1;
            set => SetAndRaiseIfChanged(ref _lp7Cs1, value, nameof(I_Lp7_Cs_1));
        }

        private bool _lp7TrReq;
        public bool I_Lp7_Tr_Req
        {
            get => _lp7TrReq;
            set => SetAndRaiseIfChanged(ref _lp7TrReq, value, nameof(I_Lp7_Tr_Req));
        }

        private bool _lp7Busy;
        public bool I_Lp7_Busy
        {
            get => _lp7Busy;
            set => SetAndRaiseIfChanged(ref _lp7Busy, value, nameof(I_Lp7_Busy));
        }

        private bool _lp7Compt;
        public bool I_Lp7_Compt
        {
            get => _lp7Compt;
            set => SetAndRaiseIfChanged(ref _lp7Compt, value, nameof(I_Lp7_Compt));
        }

        private bool _lp7Cont;
        public bool I_Lp7_Cont
        {
            get => _lp7Cont;
            set => SetAndRaiseIfChanged(ref _lp7Cont, value, nameof(I_Lp7_Cont));
        }

        #endregion LP7

        #region LP8

        private bool _lp8Valid;
        public bool I_Lp8_Valid
        {
            get => _lp8Valid;
            set => SetAndRaiseIfChanged(ref _lp8Valid, value, nameof(I_Lp8_Valid));
        }

        private bool _lp8Cs0;
        public bool I_Lp8_Cs_0
        {
            get => _lp8Cs0;
            set => SetAndRaiseIfChanged(ref _lp8Cs0, value, nameof(I_Lp8_Cs_0));
        }

        private bool _lp8Cs1;
        public bool I_Lp8_Cs_1
        {
            get => _lp8Cs1;
            set => SetAndRaiseIfChanged(ref _lp8Cs1, value, nameof(I_Lp8_Cs_1));
        }

        private bool _lp8TrReq;
        public bool I_Lp8_Tr_Req
        {
            get => _lp8TrReq;
            set => SetAndRaiseIfChanged(ref _lp8TrReq, value, nameof(I_Lp8_Tr_Req));
        }

        private bool _lp8Busy;
        public bool I_Lp8_Busy
        {
            get => _lp8Busy;
            set => SetAndRaiseIfChanged(ref _lp8Busy, value, nameof(I_Lp8_Busy));
        }

        private bool _lp8Compt;
        public bool I_Lp8_Compt
        {
            get => _lp8Compt;
            set => SetAndRaiseIfChanged(ref _lp8Compt, value, nameof(I_Lp8_Compt));
        }

        private bool _lp8Cont;
        public bool I_Lp8_Cont
        {
            get => _lp8Cont;
            set => SetAndRaiseIfChanged(ref _lp8Cont, value, nameof(I_Lp8_Cont));
        }

        #endregion LP8

        #endregion Inputs

        #region Outputs

        #region LP1

        private bool _lp1LReq;
        public bool O_Lp1_L_Req
        {
            get => _lp1LReq;
            set => SetAndRaiseIfChanged(ref _lp1LReq, value, nameof(O_Lp1_L_Req));
        }

        private bool _lp1UReq;
        public bool O_Lp1_U_Req
        {
            get => _lp1UReq;
            set => SetAndRaiseIfChanged(ref _lp1UReq, value, nameof(O_Lp1_U_Req));
        }

        private bool _lp1Ready;
        public bool O_Lp1_Ready
        {
            get => _lp1Ready;
            set => SetAndRaiseIfChanged(ref _lp1Ready, value, nameof(O_Lp1_Ready));
        }

        private bool _lp1HoAvbl;
        public bool O_Lp1_Ho_Avbl
        {
            get => _lp1HoAvbl;
            set => SetAndRaiseIfChanged(ref _lp1HoAvbl, value, nameof(O_Lp1_Ho_Avbl));
        }

        private bool _lp1Es;
        public bool O_Lp1_Es
        {
            get => _lp1Es;
            set => SetAndRaiseIfChanged(ref _lp1Es, value, nameof(O_Lp1_Es));
        }

        #endregion LP1

        #region LP2

        private bool _lp2LReq;
        public bool O_Lp2_L_Req
        {
            get => _lp2LReq;
            set => SetAndRaiseIfChanged(ref _lp2LReq, value, nameof(O_Lp2_L_Req));
        }

        private bool _lp2UReq;
        public bool O_Lp2_U_Req
        {
            get => _lp2UReq;
            set => SetAndRaiseIfChanged(ref _lp2UReq, value, nameof(O_Lp2_U_Req));
        }

        private bool _lp2Ready;
        public bool O_Lp2_Ready
        {
            get => _lp2Ready;
            set => SetAndRaiseIfChanged(ref _lp2Ready, value, nameof(O_Lp2_Ready));
        }

        private bool _lp2HoAvbl;
        public bool O_Lp2_Ho_Avbl
        {
            get => _lp2HoAvbl;
            set => SetAndRaiseIfChanged(ref _lp2HoAvbl, value, nameof(O_Lp2_Ho_Avbl));
        }

        private bool _lp2Es;
        public bool O_Lp2_Es
        {
            get => _lp2Es;
            set => SetAndRaiseIfChanged(ref _lp1Es, value, nameof(O_Lp2_Es));
        }

        #endregion LP2

        #region LP3

        private bool _lp3LReq;
        public bool O_Lp3_L_Req
        {
            get => _lp3LReq;
            set => SetAndRaiseIfChanged(ref _lp3LReq, value, nameof(O_Lp3_L_Req));
        }

        private bool _lp3UReq;
        public bool O_Lp3_U_Req
        {
            get => _lp3UReq;
            set => SetAndRaiseIfChanged(ref _lp3UReq, value, nameof(O_Lp3_U_Req));
        }

        private bool _lp3Ready;
        public bool O_Lp3_Ready
        {
            get => _lp3Ready;
            set => SetAndRaiseIfChanged(ref _lp3Ready, value, nameof(O_Lp3_Ready));
        }

        private bool _lp3HoAvbl;
        public bool O_Lp3_Ho_Avbl
        {
            get => _lp3HoAvbl;
            set => SetAndRaiseIfChanged(ref _lp3HoAvbl, value, nameof(O_Lp3_Ho_Avbl));
        }

        private bool _lp3Es;
        public bool O_Lp3_Es
        {
            get => _lp3Es;
            set => SetAndRaiseIfChanged(ref _lp1Es, value, nameof(O_Lp3_Es));
        }

        #endregion LP3

        #region LP4

        private bool _lp4LReq;
        public bool O_Lp4_L_Req
        {
            get => _lp4LReq;
            set => SetAndRaiseIfChanged(ref _lp4LReq, value, nameof(O_Lp4_L_Req));
        }

        private bool _lp4UReq;
        public bool O_Lp4_U_Req
        {
            get => _lp4UReq;
            set => SetAndRaiseIfChanged(ref _lp4UReq, value, nameof(O_Lp4_U_Req));
        }

        private bool _lp4Ready;
        public bool O_Lp4_Ready
        {
            get => _lp4Ready;
            set => SetAndRaiseIfChanged(ref _lp4Ready, value, nameof(O_Lp4_Ready));
        }

        private bool _lp4HoAvbl;
        public bool O_Lp4_Ho_Avbl
        {
            get => _lp4HoAvbl;
            set => SetAndRaiseIfChanged(ref _lp4HoAvbl, value, nameof(O_Lp4_Ho_Avbl));
        }

        private bool _lp4Es;
        public bool O_Lp4_Es
        {
            get => _lp4Es;
            set => SetAndRaiseIfChanged(ref _lp1Es, value, nameof(O_Lp4_Es));
        }

        #endregion LP4

        #region LP5

        private bool _lp5LReq;
        public bool O_Lp5_L_Req
        {
            get => _lp5LReq;
            set => SetAndRaiseIfChanged(ref _lp5LReq, value, nameof(O_Lp5_L_Req));
        }

        private bool _lp5UReq;
        public bool O_Lp5_U_Req
        {
            get => _lp5UReq;
            set => SetAndRaiseIfChanged(ref _lp5UReq, value, nameof(O_Lp5_U_Req));
        }

        private bool _lp5Ready;
        public bool O_Lp5_Ready
        {
            get => _lp5Ready;
            set => SetAndRaiseIfChanged(ref _lp5Ready, value, nameof(O_Lp5_Ready));
        }

        private bool _lp5HoAvbl;
        public bool O_Lp5_Ho_Avbl
        {
            get => _lp5HoAvbl;
            set => SetAndRaiseIfChanged(ref _lp5HoAvbl, value, nameof(O_Lp5_Ho_Avbl));
        }

        private bool _lp5Es;
        public bool O_Lp5_Es
        {
            get => _lp5Es;
            set => SetAndRaiseIfChanged(ref _lp1Es, value, nameof(O_Lp5_Es));
        }

        #endregion LP5

        #region LP6

        private bool _lp6LReq;
        public bool O_Lp6_L_Req
        {
            get => _lp6LReq;
            set => SetAndRaiseIfChanged(ref _lp6LReq, value, nameof(O_Lp6_L_Req));
        }

        private bool _lp6UReq;
        public bool O_Lp6_U_Req
        {
            get => _lp6UReq;
            set => SetAndRaiseIfChanged(ref _lp6UReq, value, nameof(O_Lp6_U_Req));
        }

        private bool _lp6Ready;
        public bool O_Lp6_Ready
        {
            get => _lp6Ready;
            set => SetAndRaiseIfChanged(ref _lp6Ready, value, nameof(O_Lp6_Ready));
        }

        private bool _lp6HoAvbl;
        public bool O_Lp6_Ho_Avbl
        {
            get => _lp6HoAvbl;
            set => SetAndRaiseIfChanged(ref _lp6HoAvbl, value, nameof(O_Lp6_Ho_Avbl));
        }

        private bool _lp6Es;
        public bool O_Lp6_Es
        {
            get => _lp6Es;
            set => SetAndRaiseIfChanged(ref _lp1Es, value, nameof(O_Lp6_Es));
        }

        #endregion LP6

        #region LP7

        private bool _lp7LReq;
        public bool O_Lp7_L_Req
        {
            get => _lp7LReq;
            set => SetAndRaiseIfChanged(ref _lp7LReq, value, nameof(O_Lp7_L_Req));
        }

        private bool _lp7UReq;
        public bool O_Lp7_U_Req
        {
            get => _lp7UReq;
            set => SetAndRaiseIfChanged(ref _lp7UReq, value, nameof(O_Lp7_U_Req));
        }

        private bool _lp7Ready;
        public bool O_Lp7_Ready
        {
            get => _lp7Ready;
            set => SetAndRaiseIfChanged(ref _lp7Ready, value, nameof(O_Lp7_Ready));
        }

        private bool _lp7HoAvbl;
        public bool O_Lp7_Ho_Avbl
        {
            get => _lp7HoAvbl;
            set => SetAndRaiseIfChanged(ref _lp7HoAvbl, value, nameof(O_Lp7_Ho_Avbl));
        }

        private bool _lp7Es;
        public bool O_Lp7_Es
        {
            get => _lp7Es;
            set => SetAndRaiseIfChanged(ref _lp1Es, value, nameof(O_Lp7_Es));
        }

        #endregion LP7

        #region LP8

        private bool _lp8LReq;
        public bool O_Lp8_L_Req
        {
            get => _lp8LReq;
            set => SetAndRaiseIfChanged(ref _lp8LReq, value, nameof(O_Lp8_L_Req));
        }

        private bool _lp8UReq;
        public bool O_Lp8_U_Req
        {
            get => _lp8UReq;
            set => SetAndRaiseIfChanged(ref _lp8UReq, value, nameof(O_Lp8_U_Req));
        }

        private bool _lp8Ready;
        public bool O_Lp8_Ready
        {
            get => _lp8Ready;
            set => SetAndRaiseIfChanged(ref _lp8Ready, value, nameof(O_Lp8_Ready));
        }

        private bool _lp8HoAvbl;
        public bool O_Lp8_Ho_Avbl
        {
            get => _lp8HoAvbl;
            set => SetAndRaiseIfChanged(ref _lp8HoAvbl, value, nameof(O_Lp8_Ho_Avbl));
        }

        private bool _lp8Es;
        public bool O_Lp8_Es
        {
            get => _lp8Es;
            set => SetAndRaiseIfChanged(ref _lp1Es, value, nameof(O_Lp8_Es));
        }

        #endregion LP8

        #endregion Outputs

        #endregion E84

        #endregion
    }
}
