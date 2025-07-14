using Agileo.EquipmentModeling;

using UnitsNet;
using UnitsNet.Units;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Conditions;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Enums;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0
{
    /// <summary>
    /// Define all statuses needed by DIO0.
    /// </summary>
    [Device]
    public interface IDio0 : IGenericRC5xx
    {
        #region Statuses

        [Status]
        IoModuleInError IoModuleInErrorDescription { get; }

        [Status]
        ErrorCode ErrorDescription { get; }

        [Status(Documentation = "Mean of all pressure sensors", IsLoggingActivated = false)]
        Pressure MeanPressure { get; }

        [Status]
        bool I_DvrAlarm { get; }

        #region FFU

        [Status(Category = "FFU", Documentation = "Average value of all fans connected to FFU")]
        [Unit(RotationalSpeedUnit.RevolutionPerMinute)]
        RotationalSpeed FanSpeed { get; }

        [Status(Category = "FFU")]
        bool I_FANDetection1 { get; }

        [Status(Category = "FFU")]
        bool I_FANDetection2 { get; }

        #endregion FFU

        #region GPIO

        #region Inputs

        [Status(Category = "General Inputs")]
        bool I_FAN1Rotating { get; }

        [Status(Category = "General Inputs")]

        bool I_FAN2Rotating { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN3Rotating { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN4Rotating { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN5Rotating { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN6Rotating { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN7Rotating { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN8Rotating { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN9Rotating { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN10Rotating { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN11Rotating { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN12Rotating { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN13Rotating { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN14Rotating { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN15Rotating { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN16Rotating { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN17Rotating { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN18Rotating { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN19Rotating { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN20Rotating { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN1AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN2AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN3AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN4AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN5AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN6AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN7AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN8AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN9AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN10AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN11AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN12AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN13AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN14AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN15AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN16AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN17AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN18AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN19AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_FAN20AlarmOccurred { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor1_WithinUpperLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor1_WithinLowerLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor2_WithinUpperLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor2_WithinLowerLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor3_WithinUpperLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor3_WithinLowerLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor4_WithinUpperLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor4_WithinLowerLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor5_WithinUpperLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor5_WithinLowerLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor6_WithinUpperLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor6_WithinLowerLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor7_WithinUpperLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor7_WithinLowerLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor8_WithinUpperLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor8_WithinLowerLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor9_WithinUpperLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor9_WithinLowerLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor10_WithinUpperLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor10_WithinLowerLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor11_WithinUpperLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor11_WithinLowerLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor12_WithinUpperLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_Sensor12_WithinLowerLimitThresholdValue { get; }

        [Status(Category = "General Inputs")]
        bool I_ControllerDirectInput_IN0 { get; }

        [Status(Category = "General Inputs")]
        bool I_ControllerDirectInput_IN1 { get; }

        [Status(Category = "General Inputs")]
        bool I_ControllerDirectInput_IN2 { get; }

        [Status(Category = "General Inputs")]
        bool I_ControllerDirectInput_IN3 { get; }

        #endregion Inputs

        #region Outputs

        [Status(Category = "General Outputs")]
        bool O_SystemIsReady { get; }

        [Status(Category = "General Outputs")]
        bool O_BatchAlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs")]
        bool O_Fan_OperationStop_AllUsingFans_1ShotOutput { get; }

        [Status(Category = "General Outputs")]
        bool O_Fan_OperationStart_AllUsingFans_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN1_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN2_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN3_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN4_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN5_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN6_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN7_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN8_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN9_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN10_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN11_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN12_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN13_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN14_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN15_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN16_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN17_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN18_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN19_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN20_OperationStart_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN1_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN2_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN3_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN4_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN5_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN6_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN7_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN8_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN9_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN10_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN11_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN12_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN13_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN14_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN15_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN16_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN17_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN18_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN19_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN20_AlarmClear_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN1_OperationStop_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN2_OperationStop_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN3_OperationStop_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN4_OperationStop_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN5_OperationStop_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN6_OperationStop_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN7_OperationStop_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN8_OperationStop_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN9_OperationStop_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN10_OperationStop_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN11_OperationStop_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN12_OperationStop_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN13_OperationStop_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN14_OperationStop_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN15_OperationStop_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN16_OperationStop_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN17_OperationStop_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN18_OperationStop_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN19_OperationStop_1ShotOutput { get; }

        [Status(Category = "General Outputs", IsLoggingActivated = false)]
        bool O_FAN20_OperationStop_1ShotOutput { get; }

        #endregion Outputs

        #endregion GPIO

        #region E84

        #region Inputs

        #region LP1

        [Status(Category = "E84")]
        bool I_Lp1_Valid { get; }

        [Status(Category = "E84")]
        bool I_Lp1_Cs_0 { get; }

        [Status(Category = "E84")]
        bool I_Lp1_Cs_1 { get; }

        [Status(Category = "E84")]
        bool I_Lp1_Tr_Req { get; }

        [Status(Category = "E84")]
        bool I_Lp1_Busy { get; }

        [Status(Category = "E84")]
        bool I_Lp1_Compt { get; }

        [Status(Category = "E84")]
        bool I_Lp1_Cont { get; }

        #endregion LP1

        #region LP2

        [Status(Category = "E84")]
        bool I_Lp2_Valid { get; }

        [Status(Category = "E84")]
        bool I_Lp2_Cs_0 { get; }

        [Status(Category = "E84")]
        bool I_Lp2_Cs_1 { get; }

        [Status(Category = "E84")]
        bool I_Lp2_Tr_Req { get; }

        [Status(Category = "E84")]
        bool I_Lp2_Busy { get; }

        [Status(Category = "E84")]
        bool I_Lp2_Compt { get; }

        [Status(Category = "E84")]
        bool I_Lp2_Cont { get; }

        #endregion LP2

        #region LP3

        [Status(Category = "E84")]
        bool I_Lp3_Valid { get; }

        [Status(Category = "E84")]
        bool I_Lp3_Cs_0 { get; }

        [Status(Category = "E84")]
        bool I_Lp3_Cs_1 { get; }

        [Status(Category = "E84")]
        bool I_Lp3_Tr_Req { get; }

        [Status(Category = "E84")]
        bool I_Lp3_Busy { get; }

        [Status(Category = "E84")]
        bool I_Lp3_Compt { get; }

        [Status(Category = "E84")]
        bool I_Lp3_Cont { get; }

        #endregion LP3

        #region LP4

        [Status(Category = "E84")]
        bool I_Lp4_Valid { get; }

        [Status(Category = "E84")]
        bool I_Lp4_Cs_0 { get; }

        [Status(Category = "E84")]
        bool I_Lp4_Cs_1 { get; }

        [Status(Category = "E84")]
        bool I_Lp4_Tr_Req { get; }

        [Status(Category = "E84")]
        bool I_Lp4_Busy { get; }

        [Status(Category = "E84")]
        bool I_Lp4_Compt { get; }

        [Status(Category = "E84")]
        bool I_Lp4_Cont { get; }

        #endregion LP4

        #region LP5

        [Status(Category = "E84")]
        bool I_Lp5_Valid { get; }

        [Status(Category = "E84")]
        bool I_Lp5_Cs_0 { get; }

        [Status(Category = "E84")]
        bool I_Lp5_Cs_1 { get; }

        [Status(Category = "E84")]
        bool I_Lp5_Tr_Req { get; }

        [Status(Category = "E84")]
        bool I_Lp5_Busy { get; }

        [Status(Category = "E84")]
        bool I_Lp5_Compt { get; }

        [Status(Category = "E84")]
        bool I_Lp5_Cont { get; }

        #endregion LP5

        #region LP6

        [Status(Category = "E84")]
        bool I_Lp6_Valid { get; }

        [Status(Category = "E84")]
        bool I_Lp6_Cs_0 { get; }

        [Status(Category = "E84")]
        bool I_Lp6_Cs_1 { get; }

        [Status(Category = "E84")]
        bool I_Lp6_Tr_Req { get; }

        [Status(Category = "E84")]
        bool I_Lp6_Busy { get; }

        [Status(Category = "E84")]
        bool I_Lp6_Compt { get; }

        [Status(Category = "E84")]
        bool I_Lp6_Cont { get; }

        #endregion LP6

        #region LP7

        [Status(Category = "E84")]
        bool I_Lp7_Valid { get; }

        [Status(Category = "E84")]
        bool I_Lp7_Cs_0 { get; }

        [Status(Category = "E84")]
        bool I_Lp7_Cs_1 { get; }

        [Status(Category = "E84")]
        bool I_Lp7_Tr_Req { get; }

        [Status(Category = "E84")]
        bool I_Lp7_Busy { get; }

        [Status(Category = "E84")]
        bool I_Lp7_Compt { get; }

        [Status(Category = "E84")]
        bool I_Lp7_Cont { get; }

        #endregion LP7

        #region LP8

        [Status(Category = "E84")]
        bool I_Lp8_Valid { get; }

        [Status(Category = "E84")]
        bool I_Lp8_Cs_0 { get; }

        [Status(Category = "E84")]
        bool I_Lp8_Cs_1 { get; }

        [Status(Category = "E84")]
        bool I_Lp8_Tr_Req { get; }

        [Status(Category = "E84")]
        bool I_Lp8_Busy { get; }

        [Status(Category = "E84")]
        bool I_Lp8_Compt { get; }

        [Status(Category = "E84")]
        bool I_Lp8_Cont { get; }

        #endregion LP8

        #endregion Inputs

        #region Outputs

        #region LP1

        [Status(Category = "E84")]
        bool O_Lp1_L_Req { get; }

        [Status(Category = "E84")]
        bool O_Lp1_U_Req { get; }

        [Status(Category = "E84")]
        bool O_Lp1_Ready { get; }

        [Status(Category = "E84")]
        bool O_Lp1_Ho_Avbl { get; }

        [Status(Category = "E84")]
        bool O_Lp1_Es { get; }

        #endregion LP1

        #region LP2

        [Status(Category = "E84")]
        bool O_Lp2_L_Req { get; }

        [Status(Category = "E84")]
        bool O_Lp2_U_Req { get; }

        [Status(Category = "E84")]
        bool O_Lp2_Ready { get; }

        [Status(Category = "E84")]
        bool O_Lp2_Ho_Avbl { get; }

        [Status(Category = "E84")]
        bool O_Lp2_Es { get; }

        #endregion LP2

        #region LP3

        [Status(Category = "E84")]
        bool O_Lp3_L_Req { get; }

        [Status(Category = "E84")]
        bool O_Lp3_U_Req { get; }

        [Status(Category = "E84")]
        bool O_Lp3_Ready { get; }

        [Status(Category = "E84")]
        bool O_Lp3_Ho_Avbl { get; }

        [Status(Category = "E84")]
        bool O_Lp3_Es { get; }

        #endregion LP3

        #region LP4

        [Status(Category = "E84")]
        bool O_Lp4_L_Req { get; }

        [Status(Category = "E84")]
        bool O_Lp4_U_Req { get; }

        [Status(Category = "E84")]
        bool O_Lp4_Ready { get; }

        [Status(Category = "E84")]
        bool O_Lp4_Ho_Avbl { get; }

        [Status(Category = "E84")]
        bool O_Lp4_Es { get; }

        #endregion LP4

        #region LP5

        [Status(Category = "E84")]
        bool O_Lp5_L_Req { get; }

        [Status(Category = "E84")]
        bool O_Lp5_U_Req { get; }

        [Status(Category = "E84")]
        bool O_Lp5_Ready { get; }

        [Status(Category = "E84")]
        bool O_Lp5_Ho_Avbl { get; }

        [Status(Category = "E84")]
        bool O_Lp5_Es { get; }

        #endregion LP5

        #region LP6

        [Status(Category = "E84")]
        bool O_Lp6_L_Req { get; }

        [Status(Category = "E84")]
        bool O_Lp6_U_Req { get; }

        [Status(Category = "E84")]
        bool O_Lp6_Ready { get; }

        [Status(Category = "E84")]
        bool O_Lp6_Ho_Avbl { get; }

        [Status(Category = "E84")]
        bool O_Lp6_Es { get; }

        #endregion LP6

        #region LP7

        [Status(Category = "E84")]
        bool O_Lp7_L_Req { get; }

        [Status(Category = "E84")]
        bool O_Lp7_U_Req { get; }

        [Status(Category = "E84")]
        bool O_Lp7_Ready { get; }

        [Status(Category = "E84")]
        bool O_Lp7_Ho_Avbl { get; }

        [Status(Category = "E84")]
        bool O_Lp7_Es { get; }

        #endregion LP7

        #region LP8

        [Status(Category = "E84")]
        bool O_Lp8_L_Req { get; }

        [Status(Category = "E84")]
        bool O_Lp8_U_Req { get; }

        [Status(Category = "E84")]
        bool O_Lp8_Ready { get; }

        [Status(Category = "E84")]
        bool O_Lp8_Ho_Avbl { get; }

        [Status(Category = "E84")]
        bool O_Lp8_Es { get; }

        #endregion LP8

        #endregion Outputs

        #endregion E84

        #region Laying Plan Load Ports

        #region Inputs

        #region Load Port 1

        [Status(Category = "Load Ports")]
        bool PlacementSensorALoadPort1 { get; }

        [Status(Category = "Load Ports")]
        bool PlacementSensorBLoadPort1 { get; }

        [Status(Category = "Load Ports")]
        bool PlacementSensorCLoadPort1 { get; }

        [Status(Category = "Load Ports")]
        bool PlacementSensorDLoadPort1 { get; }

        [Status(Category = "Load Ports")]
        bool WaferProtrudeSensor1LoadPort1 { get; }

        [Status(Category = "Load Ports")]
        bool WaferProtrudeSensor2LoadPort1 { get; }

        [Status(Category = "Load Ports")]
        bool WaferProtrudeSensor3LoadPort1 { get; }

        #endregion

        #region Load Port 2

        [Status(Category = "Load Ports")]
        bool PlacementSensorALoadPort2 { get; }

        [Status(Category = "Load Ports")]
        bool PlacementSensorBLoadPort2 { get; }

        [Status(Category = "Load Ports")]
        bool PlacementSensorCLoadPort2 { get; }

        [Status(Category = "Load Ports")]
        bool PlacementSensorDLoadPort2 { get; }

        [Status(Category = "Load Ports")]
        bool WaferProtrudeSensor1LoadPort2 { get; }

        [Status(Category = "Load Ports")]
        bool WaferProtrudeSensor2LoadPort2 { get; }

        [Status(Category = "Load Ports")]
        bool WaferProtrudeSensor3LoadPort2 { get; }

        #endregion

        #region Load Port 3

        [Status(Category = "Load Ports")]
        bool PlacementSensorALoadPort3 { get; }

        [Status(Category = "Load Ports")]
        bool PlacementSensorBLoadPort3 { get; }

        [Status(Category = "Load Ports")]
        bool PlacementSensorCLoadPort3 { get; }

        [Status(Category = "Load Ports")]
        bool PlacementSensorDLoadPort3 { get; }

        [Status(Category = "Load Ports")]
        bool WaferProtrudeSensor1LoadPort3 { get; }

        [Status(Category = "Load Ports")]
        bool WaferProtrudeSensor2LoadPort3 { get; }

        [Status(Category = "Load Ports")]
        bool WaferProtrudeSensor3LoadPort3 { get; }

        #endregion

        #region Load Port 4

        [Status(Category = "Load Ports")]
        bool PlacementSensorALoadPort4 { get; }

        [Status(Category = "Load Ports")]
        bool PlacementSensorBLoadPort4 { get; }

        [Status(Category = "Load Ports")]
        bool PlacementSensorCLoadPort4 { get; }

        [Status(Category = "Load Ports")]
        bool PlacementSensorDLoadPort4 { get; }

        [Status(Category = "Load Ports")]
        bool WaferProtrudeSensor1LoadPort4 { get; }

        [Status(Category = "Load Ports")]
        bool WaferProtrudeSensor2LoadPort4 { get; }

        [Status(Category = "Load Ports")]
        bool WaferProtrudeSensor3LoadPort4 { get; }

        #endregion

        #endregion

        #endregion

        [Status]
        bool Alarm { get; }

        #endregion Statuses

        #region Commands

        [Command(Documentation = "Range is 300 ~ 1650 rpm, or 0 to stop FFU")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsFfuSpeedInRange))]
        void SetFfuSpeed([Unit(RotationalSpeedUnit.RevolutionPerMinute)] RotationalSpeed setPoint);

        #endregion Commands
    }
}
