using System;
using System.Globalization;

using UnitySC.Equipment.Abstractions.Drivers.Common;

using static System.UInt64;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Statuses
{
    public class RC550GeneralIoStatus : Status
    {
        #region Enums

        /// <summary>
        /// Represent low order input digits.
        /// Input should be stored in a 96bits number.
        /// Therefore, it has been split into 2 part of 64bits.
        /// </summary>
        [Flags]
        public enum Inputs_LowOrder : UInt64
        {
            FAN1Rotating                           = 1UL << 8,
            FAN2Rotating                           = 1UL << 9,
            FAN3Rotating                           = 1UL << 10,
            FAN4Rotating                           = 1UL << 11,
            FAN5Rotating                           = 1UL << 12,
            FAN6Rotating                           = 1UL << 13,
            FAN7Rotating                           = 1UL << 14,
            FAN8Rotating                           = 1UL << 15,
            FAN9Rotating                           = 1UL << 16,
            FAN10Rotating                          = 1UL << 17,
            FAN11Rotating                          = 1UL << 18,
            FAN12Rotating                          = 1UL << 19,
            FAN13Rotating                          = 1UL << 20,
            FAN14Rotating                          = 1UL << 21,
            FAN15Rotating                          = 1UL << 22,
            FAN16Rotating                          = 1UL << 23,
            FAN17Rotating                          = 1UL << 24,
            FAN18Rotating                          = 1UL << 25,
            FAN19Rotating                          = 1UL << 26,
            FAN20Rotating                          = 1UL << 27,
            FAN1AlarmOccurred                      = 1UL << 28,
            FAN2AlarmOccurred                      = 1UL << 29,
            FAN3AlarmOccurred                      = 1UL << 30,
            FAN4AlarmOccurred                      = 1UL << 31,
            FAN5AlarmOccurred                      = 1UL << 32,
            FAN6AlarmOccurred                      = 1UL << 33,
            FAN7AlarmOccurred                      = 1UL << 34,
            FAN8AlarmOccurred                      = 1UL << 35,
            FAN9AlarmOccurred                      = 1UL << 36,
            FAN10AlarmOccurred                     = 1UL << 37,
            FAN11AlarmOccurred                     = 1UL << 38,
            FAN12AlarmOccurred                     = 1UL << 39,
            FAN13AlarmOccurred                     = 1UL << 40,
            FAN14AlarmOccurred                     = 1UL << 41,
            FAN15AlarmOccurred                     = 1UL << 42,
            FAN16AlarmOccurred                     = 1UL << 43,
            FAN17AlarmOccurred                     = 1UL << 44,
            FAN18AlarmOccurred                     = 1UL << 45,
            FAN19AlarmOccurred                     = 1UL << 46,
            FAN20AlarmOccurred                     = 1UL << 47,
            Sensor1_WithinUpperLimitThresholdValue = 1UL << 48,
            Sensor1_WithinLowerLimitThresholdValue = 1UL << 49,
            Sensor2_WithinUpperLimitThresholdValue = 1UL << 50,
            Sensor2_WithinLowerLimitThresholdValue = 1UL << 51,
            Sensor3_WithinUpperLimitThresholdValue = 1UL << 52,
            Sensor3_WithinLowerLimitThresholdValue = 1UL << 53,
            Sensor4_WithinUpperLimitThresholdValue = 1UL << 54,
            Sensor4_WithinLowerLimitThresholdValue = 1UL << 55,
            Sensor5_WithinUpperLimitThresholdValue = 1UL << 56,
            Sensor5_WithinLowerLimitThresholdValue = 1UL << 57,
            Sensor6_WithinUpperLimitThresholdValue = 1UL << 58,
            Sensor6_WithinLowerLimitThresholdValue = 1UL << 59,
            Sensor7_WithinUpperLimitThresholdValue = 1UL << 60,
            Sensor7_WithinLowerLimitThresholdValue = 1UL << 61,
            Sensor8_WithinUpperLimitThresholdValue = 1UL << 62,
            Sensor8_WithinLowerLimitThresholdValue = 1UL << 63
        }

        /// <summary>
        /// Represent high order input digits.
        /// Input should be stored in a 96bits number.
        /// Therefore, it has been split into 2 part of 64bits.
        /// </summary>
        [Flags]
        public enum Inputs_HighOrder : UInt64
        {
            Sensor9_WithinUpperLimitThresholdValue  = 1UL << (64 % 64),
            Sensor9_WithinLowerLimitThresholdValue  = 1UL << (65 % 64),
            Sensor10_WithinUpperLimitThresholdValue = 1UL << (66 % 64),
            Sensor10_WithinLowerLimitThresholdValue = 1UL << (67 % 64),
            Sensor11_WithinUpperLimitThresholdValue = 1UL << (68 % 64),
            Sensor11_WithinLowerLimitThresholdValue = 1UL << (69 % 64),
            Sensor12_WithinUpperLimitThresholdValue = 1UL << (70 % 64),
            Sensor12_WithinLowerLimitThresholdValue = 1UL << (71 % 64),
            ControllerDirectInput_IN0               = 1UL << (80 % 64),
            ControllerDirectInput_IN1               = 1UL << (81 % 64),
            ControllerDirectInput_IN2               = 1UL << (82 % 64),
            ControllerDirectInput_IN3               = 1UL << (83 % 64)
        }

        /// <summary>
        /// Represent low order output digits.
        /// Input should be stored in a 96bits number.
        /// Therefore, it has been split into 2 part of 64bits.
        /// </summary>
        [Flags]
        public enum Outputs_LowOrder : UInt64
        {
            SystemIsReady = 1UL << 0,

            BatchAlarmClear_1ShotOutput                 = 1UL << 5,
            Fan_OperationStop_AllUsingFans_1ShotOutput  = 1UL << 6,
            Fan_OperationStart_AllUsingFans_1ShotOutput = 1UL << 7,
            FAN1_OperationStart_1ShotOutput             = 1UL << 8,
            FAN2_OperationStart_1ShotOutput             = 1UL << 9,
            FAN3_OperationStart_1ShotOutput             = 1UL << 10,
            FAN4_OperationStart_1ShotOutput             = 1UL << 11,
            FAN5_OperationStart_1ShotOutput             = 1UL << 12,
            FAN6_OperationStart_1ShotOutput             = 1UL << 13,
            FAN7_OperationStart_1ShotOutput             = 1UL << 14,
            FAN8_OperationStart_1ShotOutput             = 1UL << 15,
            FAN9_OperationStart_1ShotOutput             = 1UL << 16,
            FAN10_OperationStart_1ShotOutput            = 1UL << 17,
            FAN11_OperationStart_1ShotOutput            = 1UL << 18,
            FAN12_OperationStart_1ShotOutput            = 1UL << 19,
            FAN13_OperationStart_1ShotOutput            = 1UL << 20,
            FAN14_OperationStart_1ShotOutput            = 1UL << 21,
            FAN15_OperationStart_1ShotOutput            = 1UL << 22,
            FAN16_OperationStart_1ShotOutput            = 1UL << 23,
            FAN17_OperationStart_1ShotOutput            = 1UL << 24,
            FAN18_OperationStart_1ShotOutput            = 1UL << 25,
            FAN19_OperationStart_1ShotOutput            = 1UL << 26,
            FAN20_OperationStart_1ShotOutput            = 1UL << 27,
            FAN1_AlarmClear_1ShotOutput                 = 1UL << 28,
            FAN2_AlarmClear_1ShotOutput                 = 1UL << 29,
            FAN3_AlarmClear_1ShotOutput                 = 1UL << 30,
            FAN4_AlarmClear_1ShotOutput                 = 1UL << 31,
            FAN5_AlarmClear_1ShotOutput                 = 1UL << 32,
            FAN6_AlarmClear_1ShotOutput                 = 1UL << 33,
            FAN7_AlarmClear_1ShotOutput                 = 1UL << 34,
            FAN8_AlarmClear_1ShotOutput                 = 1UL << 35,
            FAN9_AlarmClear_1ShotOutput                 = 1UL << 36,
            FAN10_AlarmClear_1ShotOutput                = 1UL << 37,
            FAN11_AlarmClear_1ShotOutput                = 1UL << 38,
            FAN12_AlarmClear_1ShotOutput                = 1UL << 39,
            FAN13_AlarmClear_1ShotOutput                = 1UL << 40,
            FAN14_AlarmClear_1ShotOutput                = 1UL << 41,
            FAN15_AlarmClear_1ShotOutput                = 1UL << 42,
            FAN16_AlarmClear_1ShotOutput                = 1UL << 43,
            FAN17_AlarmClear_1ShotOutput                = 1UL << 44,
            FAN18_AlarmClear_1ShotOutput                = 1UL << 45,
            FAN19_AlarmClear_1ShotOutput                = 1UL << 46,
            FAN20_AlarmClear_1ShotOutput                = 1UL << 47,
            FAN1_OperationStop_1ShotOutput              = 1UL << 48,
            FAN2_OperationStop_1ShotOutput              = 1UL << 49,
            FAN3_OperationStop_1ShotOutput              = 1UL << 50,
            FAN4_OperationStop_1ShotOutput              = 1UL << 51,
            FAN5_OperationStop_1ShotOutput              = 1UL << 52,
            FAN6_OperationStop_1ShotOutput              = 1UL << 53,
            FAN7_OperationStop_1ShotOutput              = 1UL << 54,
            FAN8_OperationStop_1ShotOutput              = 1UL << 55,
            FAN9_OperationStop_1ShotOutput              = 1UL << 56,
            FAN10_OperationStop_1ShotOutput             = 1UL << 57,
            FAN11_OperationStop_1ShotOutput             = 1UL << 58,
            FAN12_OperationStop_1ShotOutput             = 1UL << 59,
            FAN13_OperationStop_1ShotOutput             = 1UL << 60,
            FAN14_OperationStop_1ShotOutput             = 1UL << 61,
            FAN15_OperationStop_1ShotOutput             = 1UL << 62,
            FAN16_OperationStop_1ShotOutput             = 1UL << 63
        }

        /// <summary>
        /// Represent high order output digits.
        /// Input should be stored in a 96bits number.
        /// Therefore, it has been split into 2 part of 64bits.
        /// </summary>
        [Flags]
        public enum Outputs_HighOrder : UInt64
        {
            FAN17_OperationStop_1ShotOutput = 1UL << (64 % 64),
            FAN18_OperationStop_1ShotOutput = 1UL << (65 % 64),
            FAN19_OperationStop_1ShotOutput = 1UL << (66 % 64),
            FAN20_OperationStop_1ShotOutput = 1UL << (67 % 64)
        }

        #endregion Enums

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RC550GeneralIoStatus"/> class.
        /// <param name="other">Create a deep copy of <see cref="RC550GeneralIoStatus"/> instance</param>
        /// </summary>
        public RC550GeneralIoStatus(RC550GeneralIoStatus other)
        {
            Set(other);
        }

        public RC550GeneralIoStatus(string messageStatusData)
        {
            var statuses = messageStatusData.Replace(":", string.Empty).Split('/');

            // We expect 2 statuses with a 96 bits length <=> 96 / 4 = 24 hexadecimal digit per status
            if (statuses.Length != 2)
            {
                throw new ArgumentException(
                    $"{nameof(RC550GeneralIoStatus)} should be created by exactly 2 statuses: Input and Output.\n"
                    + $"Current number of statuses={statuses.Length}.");
            }

            if (statuses[0].Length != 24)
            {
                throw new ArgumentException(
                    $"{nameof(RC550GeneralIoStatus)} - Invalid input argument - Input number should have exactly 24 hexadecimal numbers.\n"
                    + $"Given input: \"{statuses[0]}\".");
            }

            if (statuses[1].Length != 24)
            {
                throw new ArgumentException(
                    $"{nameof(RC550GeneralIoStatus)} - Invalid output argument - Output number should have exactly 24 hexadecimal numbers.\n"
                    + $"Given output: \"{statuses[1]}\".");
            }

            // Get input low order digits
            var lowOrderInputString = statuses[0].Substring(12, 12);
            var lowOrderInput       = (Inputs_LowOrder)Parse(lowOrderInputString, NumberStyles.AllowHexSpecifier);

            FAN1Rotating                           = (lowOrderInput & Inputs_LowOrder.FAN1Rotating)                           != 0;
            FAN2Rotating                           = (lowOrderInput & Inputs_LowOrder.FAN2Rotating)                           != 0;
            FAN3Rotating                           = (lowOrderInput & Inputs_LowOrder.FAN3Rotating)                           != 0;
            FAN4Rotating                           = (lowOrderInput & Inputs_LowOrder.FAN4Rotating)                           != 0;
            FAN5Rotating                           = (lowOrderInput & Inputs_LowOrder.FAN5Rotating)                           != 0;
            FAN6Rotating                           = (lowOrderInput & Inputs_LowOrder.FAN6Rotating)                           != 0;
            FAN7Rotating                           = (lowOrderInput & Inputs_LowOrder.FAN7Rotating)                           != 0;
            FAN8Rotating                           = (lowOrderInput & Inputs_LowOrder.FAN8Rotating)                           != 0;
            FAN9Rotating                           = (lowOrderInput & Inputs_LowOrder.FAN9Rotating)                           != 0;
            FAN10Rotating                          = (lowOrderInput & Inputs_LowOrder.FAN10Rotating)                          != 0;
            FAN11Rotating                          = (lowOrderInput & Inputs_LowOrder.FAN11Rotating)                          != 0;
            FAN12Rotating                          = (lowOrderInput & Inputs_LowOrder.FAN12Rotating)                          != 0;
            FAN13Rotating                          = (lowOrderInput & Inputs_LowOrder.FAN13Rotating)                          != 0;
            FAN14Rotating                          = (lowOrderInput & Inputs_LowOrder.FAN14Rotating)                          != 0;
            FAN15Rotating                          = (lowOrderInput & Inputs_LowOrder.FAN15Rotating)                          != 0;
            FAN16Rotating                          = (lowOrderInput & Inputs_LowOrder.FAN16Rotating)                          != 0;
            FAN17Rotating                          = (lowOrderInput & Inputs_LowOrder.FAN17Rotating)                          != 0;
            FAN18Rotating                          = (lowOrderInput & Inputs_LowOrder.FAN18Rotating)                          != 0;
            FAN19Rotating                          = (lowOrderInput & Inputs_LowOrder.FAN19Rotating)                          != 0;
            FAN20Rotating                          = (lowOrderInput & Inputs_LowOrder.FAN20Rotating)                          != 0;
            FAN1AlarmOccurred                      = (lowOrderInput & Inputs_LowOrder.FAN1AlarmOccurred)                      != 0;
            FAN2AlarmOccurred                      = (lowOrderInput & Inputs_LowOrder.FAN2AlarmOccurred)                      != 0;
            FAN3AlarmOccurred                      = (lowOrderInput & Inputs_LowOrder.FAN3AlarmOccurred)                      != 0;
            FAN4AlarmOccurred                      = (lowOrderInput & Inputs_LowOrder.FAN4AlarmOccurred)                      != 0;
            FAN5AlarmOccurred                      = (lowOrderInput & Inputs_LowOrder.FAN5AlarmOccurred)                      != 0;
            FAN6AlarmOccurred                      = (lowOrderInput & Inputs_LowOrder.FAN6AlarmOccurred)                      != 0;
            FAN7AlarmOccurred                      = (lowOrderInput & Inputs_LowOrder.FAN7AlarmOccurred)                      != 0;
            FAN8AlarmOccurred                      = (lowOrderInput & Inputs_LowOrder.FAN8AlarmOccurred)                      != 0;
            FAN9AlarmOccurred                      = (lowOrderInput & Inputs_LowOrder.FAN9AlarmOccurred)                      != 0;
            FAN10AlarmOccurred                     = (lowOrderInput & Inputs_LowOrder.FAN10AlarmOccurred)                     != 0;
            FAN11AlarmOccurred                     = (lowOrderInput & Inputs_LowOrder.FAN11AlarmOccurred)                     != 0;
            FAN12AlarmOccurred                     = (lowOrderInput & Inputs_LowOrder.FAN12AlarmOccurred)                     != 0;
            FAN13AlarmOccurred                     = (lowOrderInput & Inputs_LowOrder.FAN13AlarmOccurred)                     != 0;
            FAN14AlarmOccurred                     = (lowOrderInput & Inputs_LowOrder.FAN14AlarmOccurred)                     != 0;
            FAN15AlarmOccurred                     = (lowOrderInput & Inputs_LowOrder.FAN15AlarmOccurred)                     != 0;
            FAN16AlarmOccurred                     = (lowOrderInput & Inputs_LowOrder.FAN16AlarmOccurred)                     != 0;
            FAN17AlarmOccurred                     = (lowOrderInput & Inputs_LowOrder.FAN17AlarmOccurred)                     != 0;
            FAN18AlarmOccurred                     = (lowOrderInput & Inputs_LowOrder.FAN18AlarmOccurred)                     != 0;
            FAN19AlarmOccurred                     = (lowOrderInput & Inputs_LowOrder.FAN19AlarmOccurred)                     != 0;
            FAN20AlarmOccurred                     = (lowOrderInput & Inputs_LowOrder.FAN20AlarmOccurred)                     != 0;
            Sensor1_WithinUpperLimitThresholdValue = (lowOrderInput & Inputs_LowOrder.Sensor1_WithinUpperLimitThresholdValue) != 0;
            Sensor1_WithinLowerLimitThresholdValue = (lowOrderInput & Inputs_LowOrder.Sensor1_WithinLowerLimitThresholdValue) != 0;
            Sensor2_WithinUpperLimitThresholdValue = (lowOrderInput & Inputs_LowOrder.Sensor2_WithinUpperLimitThresholdValue) != 0;
            Sensor2_WithinLowerLimitThresholdValue = (lowOrderInput & Inputs_LowOrder.Sensor2_WithinLowerLimitThresholdValue) != 0;
            Sensor3_WithinUpperLimitThresholdValue = (lowOrderInput & Inputs_LowOrder.Sensor3_WithinUpperLimitThresholdValue) != 0;
            Sensor3_WithinLowerLimitThresholdValue = (lowOrderInput & Inputs_LowOrder.Sensor3_WithinLowerLimitThresholdValue) != 0;
            Sensor4_WithinUpperLimitThresholdValue = (lowOrderInput & Inputs_LowOrder.Sensor4_WithinUpperLimitThresholdValue) != 0;
            Sensor4_WithinLowerLimitThresholdValue = (lowOrderInput & Inputs_LowOrder.Sensor4_WithinLowerLimitThresholdValue) != 0;
            Sensor5_WithinUpperLimitThresholdValue = (lowOrderInput & Inputs_LowOrder.Sensor5_WithinUpperLimitThresholdValue) != 0;
            Sensor5_WithinLowerLimitThresholdValue = (lowOrderInput & Inputs_LowOrder.Sensor5_WithinLowerLimitThresholdValue) != 0;
            Sensor6_WithinUpperLimitThresholdValue = (lowOrderInput & Inputs_LowOrder.Sensor6_WithinUpperLimitThresholdValue) != 0;
            Sensor6_WithinLowerLimitThresholdValue = (lowOrderInput & Inputs_LowOrder.Sensor6_WithinLowerLimitThresholdValue) != 0;
            Sensor7_WithinUpperLimitThresholdValue = (lowOrderInput & Inputs_LowOrder.Sensor7_WithinUpperLimitThresholdValue) != 0;
            Sensor7_WithinLowerLimitThresholdValue = (lowOrderInput & Inputs_LowOrder.Sensor7_WithinLowerLimitThresholdValue) != 0;
            Sensor8_WithinUpperLimitThresholdValue = (lowOrderInput & Inputs_LowOrder.Sensor8_WithinUpperLimitThresholdValue) != 0;
            Sensor8_WithinLowerLimitThresholdValue = (lowOrderInput & Inputs_LowOrder.Sensor8_WithinLowerLimitThresholdValue) != 0;

            // Get input high order digits
            var highOrderInputString = statuses[0].Substring(0, 12);
            var highOrderInput       = (Inputs_HighOrder)Parse(highOrderInputString, NumberStyles.AllowHexSpecifier);

            Sensor9_WithinUpperLimitThresholdValue  = (highOrderInput & Inputs_HighOrder.Sensor9_WithinUpperLimitThresholdValue)  != 0;
            Sensor9_WithinLowerLimitThresholdValue  = (highOrderInput & Inputs_HighOrder.Sensor9_WithinLowerLimitThresholdValue)  != 0;
            Sensor10_WithinUpperLimitThresholdValue = (highOrderInput & Inputs_HighOrder.Sensor10_WithinUpperLimitThresholdValue) != 0;
            Sensor10_WithinLowerLimitThresholdValue = (highOrderInput & Inputs_HighOrder.Sensor10_WithinLowerLimitThresholdValue) != 0;
            Sensor11_WithinUpperLimitThresholdValue = (highOrderInput & Inputs_HighOrder.Sensor11_WithinUpperLimitThresholdValue) != 0;
            Sensor11_WithinLowerLimitThresholdValue = (highOrderInput & Inputs_HighOrder.Sensor11_WithinLowerLimitThresholdValue) != 0;
            Sensor12_WithinUpperLimitThresholdValue = (highOrderInput & Inputs_HighOrder.Sensor12_WithinUpperLimitThresholdValue) != 0;
            Sensor12_WithinLowerLimitThresholdValue = (highOrderInput & Inputs_HighOrder.Sensor12_WithinLowerLimitThresholdValue) != 0;
            ControllerDirectInput_IN0               = (highOrderInput & Inputs_HighOrder.ControllerDirectInput_IN0)               != 0;
            ControllerDirectInput_IN1               = (highOrderInput & Inputs_HighOrder.ControllerDirectInput_IN1)               != 0;
            ControllerDirectInput_IN2               = (highOrderInput & Inputs_HighOrder.ControllerDirectInput_IN2)               != 0;
            ControllerDirectInput_IN3               = (highOrderInput & Inputs_HighOrder.ControllerDirectInput_IN3)               != 0;

            // Get input low order digits
            var lowOrderOutputString = statuses[1].Substring(12, 12);
            var lowOrderOutput       = (Outputs_LowOrder)Parse(lowOrderOutputString, NumberStyles.AllowHexSpecifier);

            SystemIsReady                               = (lowOrderOutput & Outputs_LowOrder.SystemIsReady)                               != 0;
            BatchAlarmClear_1ShotOutput                 = (lowOrderOutput & Outputs_LowOrder.BatchAlarmClear_1ShotOutput)                 != 0;
            Fan_OperationStop_AllUsingFans_1ShotOutput  = (lowOrderOutput & Outputs_LowOrder.Fan_OperationStop_AllUsingFans_1ShotOutput)  != 0;
            Fan_OperationStart_AllUsingFans_1ShotOutput = (lowOrderOutput & Outputs_LowOrder.Fan_OperationStart_AllUsingFans_1ShotOutput) != 0;
            FAN1_OperationStart_1ShotOutput             = (lowOrderOutput & Outputs_LowOrder.FAN1_OperationStart_1ShotOutput)             != 0;
            FAN2_OperationStart_1ShotOutput             = (lowOrderOutput & Outputs_LowOrder.FAN2_OperationStart_1ShotOutput)             != 0;
            FAN3_OperationStart_1ShotOutput             = (lowOrderOutput & Outputs_LowOrder.FAN3_OperationStart_1ShotOutput)             != 0;
            FAN4_OperationStart_1ShotOutput             = (lowOrderOutput & Outputs_LowOrder.FAN4_OperationStart_1ShotOutput)             != 0;
            FAN5_OperationStart_1ShotOutput             = (lowOrderOutput & Outputs_LowOrder.FAN5_OperationStart_1ShotOutput)             != 0;
            FAN6_OperationStart_1ShotOutput             = (lowOrderOutput & Outputs_LowOrder.FAN6_OperationStart_1ShotOutput)             != 0;
            FAN7_OperationStart_1ShotOutput             = (lowOrderOutput & Outputs_LowOrder.FAN7_OperationStart_1ShotOutput)             != 0;
            FAN8_OperationStart_1ShotOutput             = (lowOrderOutput & Outputs_LowOrder.FAN8_OperationStart_1ShotOutput)             != 0;
            FAN9_OperationStart_1ShotOutput             = (lowOrderOutput & Outputs_LowOrder.FAN9_OperationStart_1ShotOutput)             != 0;
            FAN10_OperationStart_1ShotOutput            = (lowOrderOutput & Outputs_LowOrder.FAN10_OperationStart_1ShotOutput)            != 0;
            FAN11_OperationStart_1ShotOutput            = (lowOrderOutput & Outputs_LowOrder.FAN11_OperationStart_1ShotOutput)            != 0;
            FAN12_OperationStart_1ShotOutput            = (lowOrderOutput & Outputs_LowOrder.FAN12_OperationStart_1ShotOutput)            != 0;
            FAN13_OperationStart_1ShotOutput            = (lowOrderOutput & Outputs_LowOrder.FAN13_OperationStart_1ShotOutput)            != 0;
            FAN14_OperationStart_1ShotOutput            = (lowOrderOutput & Outputs_LowOrder.FAN14_OperationStart_1ShotOutput)            != 0;
            FAN15_OperationStart_1ShotOutput            = (lowOrderOutput & Outputs_LowOrder.FAN15_OperationStart_1ShotOutput)            != 0;
            FAN16_OperationStart_1ShotOutput            = (lowOrderOutput & Outputs_LowOrder.FAN16_OperationStart_1ShotOutput)            != 0;
            FAN17_OperationStart_1ShotOutput            = (lowOrderOutput & Outputs_LowOrder.FAN17_OperationStart_1ShotOutput)            != 0;
            FAN18_OperationStart_1ShotOutput            = (lowOrderOutput & Outputs_LowOrder.FAN18_OperationStart_1ShotOutput)            != 0;
            FAN19_OperationStart_1ShotOutput            = (lowOrderOutput & Outputs_LowOrder.FAN19_OperationStart_1ShotOutput)            != 0;
            FAN20_OperationStart_1ShotOutput            = (lowOrderOutput & Outputs_LowOrder.FAN20_OperationStart_1ShotOutput)            != 0;
            FAN1_AlarmClear_1ShotOutput                 = (lowOrderOutput & Outputs_LowOrder.FAN1_AlarmClear_1ShotOutput)                 != 0;
            FAN2_AlarmClear_1ShotOutput                 = (lowOrderOutput & Outputs_LowOrder.FAN2_AlarmClear_1ShotOutput)                 != 0;
            FAN3_AlarmClear_1ShotOutput                 = (lowOrderOutput & Outputs_LowOrder.FAN3_AlarmClear_1ShotOutput)                 != 0;
            FAN4_AlarmClear_1ShotOutput                 = (lowOrderOutput & Outputs_LowOrder.FAN4_AlarmClear_1ShotOutput)                 != 0;
            FAN5_AlarmClear_1ShotOutput                 = (lowOrderOutput & Outputs_LowOrder.FAN5_AlarmClear_1ShotOutput)                 != 0;
            FAN6_AlarmClear_1ShotOutput                 = (lowOrderOutput & Outputs_LowOrder.FAN6_AlarmClear_1ShotOutput)                 != 0;
            FAN7_AlarmClear_1ShotOutput                 = (lowOrderOutput & Outputs_LowOrder.FAN7_AlarmClear_1ShotOutput)                 != 0;
            FAN8_AlarmClear_1ShotOutput                 = (lowOrderOutput & Outputs_LowOrder.FAN8_AlarmClear_1ShotOutput)                 != 0;
            FAN9_AlarmClear_1ShotOutput                 = (lowOrderOutput & Outputs_LowOrder.FAN9_AlarmClear_1ShotOutput)                 != 0;
            FAN10_AlarmClear_1ShotOutput                = (lowOrderOutput & Outputs_LowOrder.FAN10_AlarmClear_1ShotOutput)                != 0;
            FAN11_AlarmClear_1ShotOutput                = (lowOrderOutput & Outputs_LowOrder.FAN11_AlarmClear_1ShotOutput)                != 0;
            FAN12_AlarmClear_1ShotOutput                = (lowOrderOutput & Outputs_LowOrder.FAN12_AlarmClear_1ShotOutput)                != 0;
            FAN13_AlarmClear_1ShotOutput                = (lowOrderOutput & Outputs_LowOrder.FAN13_AlarmClear_1ShotOutput)                != 0;
            FAN14_AlarmClear_1ShotOutput                = (lowOrderOutput & Outputs_LowOrder.FAN14_AlarmClear_1ShotOutput)                != 0;
            FAN15_AlarmClear_1ShotOutput                = (lowOrderOutput & Outputs_LowOrder.FAN15_AlarmClear_1ShotOutput)                != 0;
            FAN16_AlarmClear_1ShotOutput                = (lowOrderOutput & Outputs_LowOrder.FAN16_AlarmClear_1ShotOutput)                != 0;
            FAN17_AlarmClear_1ShotOutput                = (lowOrderOutput & Outputs_LowOrder.FAN17_AlarmClear_1ShotOutput)                != 0;
            FAN18_AlarmClear_1ShotOutput                = (lowOrderOutput & Outputs_LowOrder.FAN18_AlarmClear_1ShotOutput)                != 0;
            FAN19_AlarmClear_1ShotOutput                = (lowOrderOutput & Outputs_LowOrder.FAN19_AlarmClear_1ShotOutput)                != 0;
            FAN20_AlarmClear_1ShotOutput                = (lowOrderOutput & Outputs_LowOrder.FAN20_AlarmClear_1ShotOutput)                != 0;
            FAN1_OperationStop_1ShotOutput              = (lowOrderOutput & Outputs_LowOrder.FAN1_OperationStop_1ShotOutput)              != 0;
            FAN2_OperationStop_1ShotOutput              = (lowOrderOutput & Outputs_LowOrder.FAN2_OperationStop_1ShotOutput)              != 0;
            FAN3_OperationStop_1ShotOutput              = (lowOrderOutput & Outputs_LowOrder.FAN3_OperationStop_1ShotOutput)              != 0;
            FAN4_OperationStop_1ShotOutput              = (lowOrderOutput & Outputs_LowOrder.FAN4_OperationStop_1ShotOutput)              != 0;
            FAN5_OperationStop_1ShotOutput              = (lowOrderOutput & Outputs_LowOrder.FAN5_OperationStop_1ShotOutput)              != 0;
            FAN6_OperationStop_1ShotOutput              = (lowOrderOutput & Outputs_LowOrder.FAN6_OperationStop_1ShotOutput)              != 0;
            FAN7_OperationStop_1ShotOutput              = (lowOrderOutput & Outputs_LowOrder.FAN7_OperationStop_1ShotOutput)              != 0;
            FAN8_OperationStop_1ShotOutput              = (lowOrderOutput & Outputs_LowOrder.FAN8_OperationStop_1ShotOutput)              != 0;
            FAN9_OperationStop_1ShotOutput              = (lowOrderOutput & Outputs_LowOrder.FAN9_OperationStop_1ShotOutput)              != 0;
            FAN10_OperationStop_1ShotOutput             = (lowOrderOutput & Outputs_LowOrder.FAN10_OperationStop_1ShotOutput)             != 0;
            FAN11_OperationStop_1ShotOutput             = (lowOrderOutput & Outputs_LowOrder.FAN11_OperationStop_1ShotOutput)             != 0;
            FAN12_OperationStop_1ShotOutput             = (lowOrderOutput & Outputs_LowOrder.FAN12_OperationStop_1ShotOutput)             != 0;
            FAN13_OperationStop_1ShotOutput             = (lowOrderOutput & Outputs_LowOrder.FAN13_OperationStop_1ShotOutput)             != 0;
            FAN14_OperationStop_1ShotOutput             = (lowOrderOutput & Outputs_LowOrder.FAN14_OperationStop_1ShotOutput)             != 0;
            FAN15_OperationStop_1ShotOutput             = (lowOrderOutput & Outputs_LowOrder.FAN15_OperationStop_1ShotOutput)             != 0;
            FAN16_OperationStop_1ShotOutput             = (lowOrderOutput & Outputs_LowOrder.FAN16_OperationStop_1ShotOutput)             != 0;

            // Get output high order digits
            var highOrderOutputString = statuses[1].Substring(0, 12);
            var highOrderOutput       = (Outputs_HighOrder)Parse(highOrderOutputString, NumberStyles.AllowHexSpecifier);

            FAN17_OperationStop_1ShotOutput = (highOrderOutput & Outputs_HighOrder.FAN17_OperationStop_1ShotOutput) != 0;
            FAN18_OperationStop_1ShotOutput = (highOrderOutput & Outputs_HighOrder.FAN18_OperationStop_1ShotOutput) != 0;
            FAN19_OperationStop_1ShotOutput = (highOrderOutput & Outputs_HighOrder.FAN19_OperationStop_1ShotOutput) != 0;
            FAN20_OperationStop_1ShotOutput = (highOrderOutput & Outputs_HighOrder.FAN20_OperationStop_1ShotOutput) != 0;
        }

        #endregion Constructors

        #region Properties

        #region Inputs

        public bool FAN1Rotating { get; internal set; }

        public bool FAN2Rotating { get; internal set; }

        public bool FAN3Rotating { get; internal set; }

        public bool FAN4Rotating { get; internal set; }

        public bool FAN5Rotating { get; internal set; }

        public bool FAN6Rotating { get; internal set; }

        public bool FAN7Rotating { get; internal set; }

        public bool FAN8Rotating { get; internal set; }

        public bool FAN9Rotating { get; internal set; }

        public bool FAN10Rotating { get; internal set; }

        public bool FAN11Rotating { get; internal set; }

        public bool FAN12Rotating { get; internal set; }

        public bool FAN13Rotating { get; internal set; }

        public bool FAN14Rotating { get; internal set; }

        public bool FAN15Rotating { get; internal set; }

        public bool FAN16Rotating { get; internal set; }

        public bool FAN17Rotating { get; internal set; }

        public bool FAN18Rotating { get; internal set; }

        public bool FAN19Rotating { get; internal set; }

        public bool FAN20Rotating { get; internal set; }

        public bool FAN1AlarmOccurred { get; internal set; }

        public bool FAN2AlarmOccurred { get; internal set; }

        public bool FAN3AlarmOccurred { get; internal set; }

        public bool FAN4AlarmOccurred { get; internal set; }

        public bool FAN5AlarmOccurred { get; internal set; }

        public bool FAN6AlarmOccurred { get; internal set; }

        public bool FAN7AlarmOccurred { get; internal set; }

        public bool FAN8AlarmOccurred { get; internal set; }

        public bool FAN9AlarmOccurred { get; internal set; }

        public bool FAN10AlarmOccurred { get; internal set; }

        public bool FAN11AlarmOccurred { get; internal set; }

        public bool FAN12AlarmOccurred { get; internal set; }

        public bool FAN13AlarmOccurred { get; internal set; }

        public bool FAN14AlarmOccurred { get; internal set; }

        public bool FAN15AlarmOccurred { get; internal set; }

        public bool FAN16AlarmOccurred { get; internal set; }

        public bool FAN17AlarmOccurred { get; internal set; }

        public bool FAN18AlarmOccurred { get; internal set; }

        public bool FAN19AlarmOccurred { get; internal set; }

        public bool FAN20AlarmOccurred { get; internal set; }

        public bool Sensor1_WithinUpperLimitThresholdValue { get; internal set; }

        public bool Sensor1_WithinLowerLimitThresholdValue { get; internal set; }

        public bool Sensor2_WithinUpperLimitThresholdValue { get; internal set; }

        public bool Sensor2_WithinLowerLimitThresholdValue { get; internal set; }

        public bool Sensor3_WithinUpperLimitThresholdValue { get; internal set; }

        public bool Sensor3_WithinLowerLimitThresholdValue { get; internal set; }

        public bool Sensor4_WithinUpperLimitThresholdValue { get; internal set; }

        public bool Sensor4_WithinLowerLimitThresholdValue { get; internal set; }

        public bool Sensor5_WithinUpperLimitThresholdValue { get; internal set; }

        public bool Sensor5_WithinLowerLimitThresholdValue { get; internal set; }

        public bool Sensor6_WithinUpperLimitThresholdValue { get; internal set; }

        public bool Sensor6_WithinLowerLimitThresholdValue { get; internal set; }

        public bool Sensor7_WithinUpperLimitThresholdValue { get; internal set; }

        public bool Sensor7_WithinLowerLimitThresholdValue { get; internal set; }

        public bool Sensor8_WithinUpperLimitThresholdValue { get; internal set; }

        public bool Sensor8_WithinLowerLimitThresholdValue { get; internal set; }

        public bool Sensor9_WithinUpperLimitThresholdValue { get; internal set; }

        public bool Sensor9_WithinLowerLimitThresholdValue { get; internal set; }

        public bool Sensor10_WithinUpperLimitThresholdValue { get; internal set; }

        public bool Sensor10_WithinLowerLimitThresholdValue { get; internal set; }

        public bool Sensor11_WithinUpperLimitThresholdValue { get; internal set; }

        public bool Sensor11_WithinLowerLimitThresholdValue { get; internal set; }

        public bool Sensor12_WithinUpperLimitThresholdValue { get; internal set; }

        public bool Sensor12_WithinLowerLimitThresholdValue { get; internal set; }

        public bool ControllerDirectInput_IN0 { get; internal set; }

        public bool ControllerDirectInput_IN1 { get; internal set; }

        public bool ControllerDirectInput_IN2 { get; internal set; }

        public bool ControllerDirectInput_IN3 { get; internal set; }

        #endregion Inputs

        #region Outputs

        public bool SystemIsReady { get; internal set; }

        public bool BatchAlarmClear_1ShotOutput { get; internal set; }

        public bool Fan_OperationStop_AllUsingFans_1ShotOutput { get; internal set; }

        public bool Fan_OperationStart_AllUsingFans_1ShotOutput { get; internal set; }

        public bool FAN1_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN2_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN3_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN4_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN5_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN6_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN7_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN8_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN9_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN10_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN11_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN12_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN13_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN14_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN15_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN16_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN17_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN18_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN19_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN20_OperationStart_1ShotOutput { get; internal set; }

        public bool FAN1_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN2_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN3_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN4_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN5_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN6_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN7_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN8_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN9_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN10_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN11_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN12_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN13_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN14_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN15_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN16_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN17_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN18_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN19_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN20_AlarmClear_1ShotOutput { get; internal set; }

        public bool FAN1_OperationStop_1ShotOutput { get; internal set; }

        public bool FAN2_OperationStop_1ShotOutput { get; internal set; }

        public bool FAN3_OperationStop_1ShotOutput { get; internal set; }

        public bool FAN4_OperationStop_1ShotOutput { get; internal set; }

        public bool FAN5_OperationStop_1ShotOutput { get; internal set; }

        public bool FAN6_OperationStop_1ShotOutput { get; internal set; }

        public bool FAN7_OperationStop_1ShotOutput { get; internal set; }

        public bool FAN8_OperationStop_1ShotOutput { get; internal set; }

        public bool FAN9_OperationStop_1ShotOutput { get; internal set; }

        public bool FAN10_OperationStop_1ShotOutput { get; internal set; }

        public bool FAN11_OperationStop_1ShotOutput { get; internal set; }

        public bool FAN12_OperationStop_1ShotOutput { get; internal set; }

        public bool FAN13_OperationStop_1ShotOutput { get; internal set; }

        public bool FAN14_OperationStop_1ShotOutput { get; internal set; }

        public bool FAN15_OperationStop_1ShotOutput { get; internal set; }

        public bool FAN16_OperationStop_1ShotOutput { get; internal set; }

        public bool FAN17_OperationStop_1ShotOutput { get; internal set; }

        public bool FAN18_OperationStop_1ShotOutput { get; internal set; }

        public bool FAN19_OperationStop_1ShotOutput { get; internal set; }

        public bool FAN20_OperationStop_1ShotOutput { get; internal set; }

        #endregion Outputs

        #endregion Properties

        #region Private Methods

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        private void Set(RC550GeneralIoStatus other = null)
        {
            lock (this)
            {
                if (other == null)
                {
                    FAN1Rotating                                = false;
                    FAN2Rotating                                = false;
                    FAN3Rotating                                = false;
                    FAN4Rotating                                = false;
                    FAN5Rotating                                = false;
                    FAN6Rotating                                = false;
                    FAN7Rotating                                = false;
                    FAN8Rotating                                = false;
                    FAN9Rotating                                = false;
                    FAN10Rotating                               = false;
                    FAN11Rotating                               = false;
                    FAN12Rotating                               = false;
                    FAN13Rotating                               = false;
                    FAN14Rotating                               = false;
                    FAN15Rotating                               = false;
                    FAN16Rotating                               = false;
                    FAN17Rotating                               = false;
                    FAN18Rotating                               = false;
                    FAN19Rotating                               = false;
                    FAN20Rotating                               = false;
                    FAN1AlarmOccurred                           = false;
                    FAN2AlarmOccurred                           = false;
                    FAN3AlarmOccurred                           = false;
                    FAN4AlarmOccurred                           = false;
                    FAN5AlarmOccurred                           = false;
                    FAN6AlarmOccurred                           = false;
                    FAN7AlarmOccurred                           = false;
                    FAN8AlarmOccurred                           = false;
                    FAN9AlarmOccurred                           = false;
                    FAN10AlarmOccurred                          = false;
                    FAN11AlarmOccurred                          = false;
                    FAN12AlarmOccurred                          = false;
                    FAN13AlarmOccurred                          = false;
                    FAN14AlarmOccurred                          = false;
                    FAN15AlarmOccurred                          = false;
                    FAN16AlarmOccurred                          = false;
                    FAN17AlarmOccurred                          = false;
                    FAN18AlarmOccurred                          = false;
                    FAN19AlarmOccurred                          = false;
                    FAN20AlarmOccurred                          = false;
                    Sensor1_WithinUpperLimitThresholdValue      = false;
                    Sensor1_WithinLowerLimitThresholdValue      = false;
                    Sensor2_WithinUpperLimitThresholdValue      = false;
                    Sensor2_WithinLowerLimitThresholdValue      = false;
                    Sensor3_WithinUpperLimitThresholdValue      = false;
                    Sensor3_WithinLowerLimitThresholdValue      = false;
                    Sensor4_WithinUpperLimitThresholdValue      = false;
                    Sensor4_WithinLowerLimitThresholdValue      = false;
                    Sensor5_WithinUpperLimitThresholdValue      = false;
                    Sensor5_WithinLowerLimitThresholdValue      = false;
                    Sensor6_WithinUpperLimitThresholdValue      = false;
                    Sensor6_WithinLowerLimitThresholdValue      = false;
                    Sensor7_WithinUpperLimitThresholdValue      = false;
                    Sensor7_WithinLowerLimitThresholdValue      = false;
                    Sensor8_WithinUpperLimitThresholdValue      = false;
                    Sensor8_WithinLowerLimitThresholdValue      = false;
                    Sensor9_WithinUpperLimitThresholdValue      = false;
                    Sensor9_WithinLowerLimitThresholdValue      = false;
                    Sensor10_WithinUpperLimitThresholdValue     = false;
                    Sensor10_WithinLowerLimitThresholdValue     = false;
                    Sensor11_WithinUpperLimitThresholdValue     = false;
                    Sensor11_WithinLowerLimitThresholdValue     = false;
                    Sensor12_WithinUpperLimitThresholdValue     = false;
                    Sensor12_WithinLowerLimitThresholdValue     = false;
                    ControllerDirectInput_IN0                   = false;
                    ControllerDirectInput_IN1                   = false;
                    ControllerDirectInput_IN2                   = false;
                    ControllerDirectInput_IN3                   = false;
                    SystemIsReady                               = false;
                    BatchAlarmClear_1ShotOutput                 = false;
                    Fan_OperationStop_AllUsingFans_1ShotOutput  = false;
                    Fan_OperationStart_AllUsingFans_1ShotOutput = false;
                    FAN1_OperationStart_1ShotOutput             = false;
                    FAN2_OperationStart_1ShotOutput             = false;
                    FAN3_OperationStart_1ShotOutput             = false;
                    FAN4_OperationStart_1ShotOutput             = false;
                    FAN5_OperationStart_1ShotOutput             = false;
                    FAN6_OperationStart_1ShotOutput             = false;
                    FAN7_OperationStart_1ShotOutput             = false;
                    FAN8_OperationStart_1ShotOutput             = false;
                    FAN9_OperationStart_1ShotOutput             = false;
                    FAN10_OperationStart_1ShotOutput            = false;
                    FAN11_OperationStart_1ShotOutput            = false;
                    FAN12_OperationStart_1ShotOutput            = false;
                    FAN13_OperationStart_1ShotOutput            = false;
                    FAN14_OperationStart_1ShotOutput            = false;
                    FAN15_OperationStart_1ShotOutput            = false;
                    FAN16_OperationStart_1ShotOutput            = false;
                    FAN17_OperationStart_1ShotOutput            = false;
                    FAN18_OperationStart_1ShotOutput            = false;
                    FAN19_OperationStart_1ShotOutput            = false;
                    FAN20_OperationStart_1ShotOutput            = false;
                    FAN1_AlarmClear_1ShotOutput                 = false;
                    FAN2_AlarmClear_1ShotOutput                 = false;
                    FAN3_AlarmClear_1ShotOutput                 = false;
                    FAN4_AlarmClear_1ShotOutput                 = false;
                    FAN5_AlarmClear_1ShotOutput                 = false;
                    FAN6_AlarmClear_1ShotOutput                 = false;
                    FAN7_AlarmClear_1ShotOutput                 = false;
                    FAN8_AlarmClear_1ShotOutput                 = false;
                    FAN9_AlarmClear_1ShotOutput                 = false;
                    FAN10_AlarmClear_1ShotOutput                = false;
                    FAN11_AlarmClear_1ShotOutput                = false;
                    FAN12_AlarmClear_1ShotOutput                = false;
                    FAN13_AlarmClear_1ShotOutput                = false;
                    FAN14_AlarmClear_1ShotOutput                = false;
                    FAN15_AlarmClear_1ShotOutput                = false;
                    FAN16_AlarmClear_1ShotOutput                = false;
                    FAN17_AlarmClear_1ShotOutput                = false;
                    FAN18_AlarmClear_1ShotOutput                = false;
                    FAN19_AlarmClear_1ShotOutput                = false;
                    FAN20_AlarmClear_1ShotOutput                = false;
                    FAN1_OperationStop_1ShotOutput              = false;
                    FAN2_OperationStop_1ShotOutput              = false;
                    FAN3_OperationStop_1ShotOutput              = false;
                    FAN4_OperationStop_1ShotOutput              = false;
                    FAN5_OperationStop_1ShotOutput              = false;
                    FAN6_OperationStop_1ShotOutput              = false;
                    FAN7_OperationStop_1ShotOutput              = false;
                    FAN8_OperationStop_1ShotOutput              = false;
                    FAN9_OperationStop_1ShotOutput              = false;
                    FAN10_OperationStop_1ShotOutput             = false;
                    FAN11_OperationStop_1ShotOutput             = false;
                    FAN12_OperationStop_1ShotOutput             = false;
                    FAN13_OperationStop_1ShotOutput             = false;
                    FAN14_OperationStop_1ShotOutput             = false;
                    FAN15_OperationStop_1ShotOutput             = false;
                    FAN16_OperationStop_1ShotOutput             = false;
                    FAN17_OperationStop_1ShotOutput             = false;
                    FAN18_OperationStop_1ShotOutput             = false;
                    FAN19_OperationStop_1ShotOutput             = false;
                    FAN20_OperationStop_1ShotOutput             = false;
                }
                else
                {
                    FAN1Rotating                            = other.FAN1Rotating;
                    FAN2Rotating                            = other.FAN2Rotating;
                    FAN3Rotating                            = other.FAN3Rotating;
                    FAN4Rotating                            = other.FAN4Rotating;
                    FAN5Rotating                            = other.FAN5Rotating;
                    FAN6Rotating                            = other.FAN6Rotating;
                    FAN7Rotating                            = other.FAN7Rotating;
                    FAN8Rotating                            = other.FAN8Rotating;
                    FAN9Rotating                            = other.FAN9Rotating;
                    FAN10Rotating                           = other.FAN10Rotating;
                    FAN11Rotating                           = other.FAN11Rotating;
                    FAN12Rotating                           = other.FAN12Rotating;
                    FAN13Rotating                           = other.FAN13Rotating;
                    FAN14Rotating                           = other.FAN14Rotating;
                    FAN15Rotating                           = other.FAN15Rotating;
                    FAN16Rotating                           = other.FAN16Rotating;
                    FAN17Rotating                           = other.FAN17Rotating;
                    FAN18Rotating                           = other.FAN18Rotating;
                    FAN19Rotating                           = other.FAN19Rotating;
                    FAN20Rotating                           = other.FAN20Rotating;
                    FAN1AlarmOccurred                       = other.FAN1AlarmOccurred;
                    FAN2AlarmOccurred                       = other.FAN2AlarmOccurred;
                    FAN3AlarmOccurred                       = other.FAN3AlarmOccurred;
                    FAN4AlarmOccurred                       = other.FAN4AlarmOccurred;
                    FAN5AlarmOccurred                       = other.FAN5AlarmOccurred;
                    FAN6AlarmOccurred                       = other.FAN6AlarmOccurred;
                    FAN7AlarmOccurred                       = other.FAN7AlarmOccurred;
                    FAN8AlarmOccurred                       = other.FAN8AlarmOccurred;
                    FAN9AlarmOccurred                       = other.FAN9AlarmOccurred;
                    FAN10AlarmOccurred                      = other.FAN10AlarmOccurred;
                    FAN11AlarmOccurred                      = other.FAN11AlarmOccurred;
                    FAN12AlarmOccurred                      = other.FAN12AlarmOccurred;
                    FAN13AlarmOccurred                      = other.FAN13AlarmOccurred;
                    FAN14AlarmOccurred                      = other.FAN14AlarmOccurred;
                    FAN15AlarmOccurred                      = other.FAN15AlarmOccurred;
                    FAN16AlarmOccurred                      = other.FAN16AlarmOccurred;
                    FAN17AlarmOccurred                      = other.FAN17AlarmOccurred;
                    FAN18AlarmOccurred                      = other.FAN18AlarmOccurred;
                    FAN19AlarmOccurred                      = other.FAN19AlarmOccurred;
                    FAN20AlarmOccurred                      = other.FAN20AlarmOccurred;
                    Sensor1_WithinUpperLimitThresholdValue  = other.Sensor1_WithinUpperLimitThresholdValue;
                    Sensor1_WithinLowerLimitThresholdValue  = other.Sensor1_WithinLowerLimitThresholdValue;
                    Sensor2_WithinUpperLimitThresholdValue  = other.Sensor2_WithinUpperLimitThresholdValue;
                    Sensor2_WithinLowerLimitThresholdValue  = other.Sensor2_WithinLowerLimitThresholdValue;
                    Sensor3_WithinUpperLimitThresholdValue  = other.Sensor3_WithinUpperLimitThresholdValue;
                    Sensor3_WithinLowerLimitThresholdValue  = other.Sensor3_WithinLowerLimitThresholdValue;
                    Sensor4_WithinUpperLimitThresholdValue  = other.Sensor4_WithinUpperLimitThresholdValue;
                    Sensor4_WithinLowerLimitThresholdValue  = other.Sensor4_WithinLowerLimitThresholdValue;
                    Sensor5_WithinUpperLimitThresholdValue  = other.Sensor5_WithinUpperLimitThresholdValue;
                    Sensor5_WithinLowerLimitThresholdValue  = other.Sensor5_WithinLowerLimitThresholdValue;
                    Sensor6_WithinUpperLimitThresholdValue  = other.Sensor6_WithinUpperLimitThresholdValue;
                    Sensor6_WithinLowerLimitThresholdValue  = other.Sensor6_WithinLowerLimitThresholdValue;
                    Sensor7_WithinUpperLimitThresholdValue  = other.Sensor7_WithinUpperLimitThresholdValue;
                    Sensor7_WithinLowerLimitThresholdValue  = other.Sensor7_WithinLowerLimitThresholdValue;
                    Sensor8_WithinUpperLimitThresholdValue  = other.Sensor8_WithinUpperLimitThresholdValue;
                    Sensor8_WithinLowerLimitThresholdValue  = other.Sensor8_WithinLowerLimitThresholdValue;
                    Sensor9_WithinUpperLimitThresholdValue  = other.Sensor9_WithinUpperLimitThresholdValue;
                    Sensor9_WithinLowerLimitThresholdValue  = other.Sensor9_WithinLowerLimitThresholdValue;
                    Sensor10_WithinUpperLimitThresholdValue = other.Sensor10_WithinUpperLimitThresholdValue;
                    Sensor10_WithinLowerLimitThresholdValue = other.Sensor10_WithinLowerLimitThresholdValue;
                    Sensor11_WithinUpperLimitThresholdValue = other.Sensor11_WithinUpperLimitThresholdValue;
                    Sensor11_WithinLowerLimitThresholdValue = other.Sensor11_WithinLowerLimitThresholdValue;
                    Sensor12_WithinUpperLimitThresholdValue = other.Sensor12_WithinUpperLimitThresholdValue;
                    Sensor12_WithinLowerLimitThresholdValue = other.Sensor12_WithinLowerLimitThresholdValue;
                    ControllerDirectInput_IN0               = other.ControllerDirectInput_IN0;
                    ControllerDirectInput_IN1               = other.ControllerDirectInput_IN1;
                    ControllerDirectInput_IN2               = other.ControllerDirectInput_IN2;
                    ControllerDirectInput_IN3               = other.ControllerDirectInput_IN3;

                    SystemIsReady                               = other.SystemIsReady;
                    BatchAlarmClear_1ShotOutput                 = other.BatchAlarmClear_1ShotOutput;
                    Fan_OperationStop_AllUsingFans_1ShotOutput  = other.Fan_OperationStop_AllUsingFans_1ShotOutput;
                    Fan_OperationStart_AllUsingFans_1ShotOutput = other.Fan_OperationStart_AllUsingFans_1ShotOutput;
                    FAN1_OperationStart_1ShotOutput             = other.FAN1_OperationStart_1ShotOutput;
                    FAN2_OperationStart_1ShotOutput             = other.FAN2_OperationStart_1ShotOutput;
                    FAN3_OperationStart_1ShotOutput             = other.FAN3_OperationStart_1ShotOutput;
                    FAN4_OperationStart_1ShotOutput             = other.FAN4_OperationStart_1ShotOutput;
                    FAN5_OperationStart_1ShotOutput             = other.FAN5_OperationStart_1ShotOutput;
                    FAN6_OperationStart_1ShotOutput             = other.FAN6_OperationStart_1ShotOutput;
                    FAN7_OperationStart_1ShotOutput             = other.FAN7_OperationStart_1ShotOutput;
                    FAN8_OperationStart_1ShotOutput             = other.FAN8_OperationStart_1ShotOutput;
                    FAN9_OperationStart_1ShotOutput             = other.FAN9_OperationStart_1ShotOutput;
                    FAN10_OperationStart_1ShotOutput            = other.FAN10_OperationStart_1ShotOutput;
                    FAN11_OperationStart_1ShotOutput            = other.FAN11_OperationStart_1ShotOutput;
                    FAN12_OperationStart_1ShotOutput            = other.FAN12_OperationStart_1ShotOutput;
                    FAN13_OperationStart_1ShotOutput            = other.FAN13_OperationStart_1ShotOutput;
                    FAN14_OperationStart_1ShotOutput            = other.FAN14_OperationStart_1ShotOutput;
                    FAN15_OperationStart_1ShotOutput            = other.FAN15_OperationStart_1ShotOutput;
                    FAN16_OperationStart_1ShotOutput            = other.FAN16_OperationStart_1ShotOutput;
                    FAN17_OperationStart_1ShotOutput            = other.FAN17_OperationStart_1ShotOutput;
                    FAN18_OperationStart_1ShotOutput            = other.FAN18_OperationStart_1ShotOutput;
                    FAN19_OperationStart_1ShotOutput            = other.FAN19_OperationStart_1ShotOutput;
                    FAN20_OperationStart_1ShotOutput            = other.FAN20_OperationStart_1ShotOutput;
                    FAN1_AlarmClear_1ShotOutput                 = other.FAN1_AlarmClear_1ShotOutput;
                    FAN2_AlarmClear_1ShotOutput                 = other.FAN2_AlarmClear_1ShotOutput;
                    FAN3_AlarmClear_1ShotOutput                 = other.FAN3_AlarmClear_1ShotOutput;
                    FAN4_AlarmClear_1ShotOutput                 = other.FAN4_AlarmClear_1ShotOutput;
                    FAN5_AlarmClear_1ShotOutput                 = other.FAN5_AlarmClear_1ShotOutput;
                    FAN6_AlarmClear_1ShotOutput                 = other.FAN6_AlarmClear_1ShotOutput;
                    FAN7_AlarmClear_1ShotOutput                 = other.FAN7_AlarmClear_1ShotOutput;
                    FAN8_AlarmClear_1ShotOutput                 = other.FAN8_AlarmClear_1ShotOutput;
                    FAN9_AlarmClear_1ShotOutput                 = other.FAN9_AlarmClear_1ShotOutput;
                    FAN10_AlarmClear_1ShotOutput                = other.FAN10_AlarmClear_1ShotOutput;
                    FAN11_AlarmClear_1ShotOutput                = other.FAN11_AlarmClear_1ShotOutput;
                    FAN12_AlarmClear_1ShotOutput                = other.FAN12_AlarmClear_1ShotOutput;
                    FAN13_AlarmClear_1ShotOutput                = other.FAN13_AlarmClear_1ShotOutput;
                    FAN14_AlarmClear_1ShotOutput                = other.FAN14_AlarmClear_1ShotOutput;
                    FAN15_AlarmClear_1ShotOutput                = other.FAN15_AlarmClear_1ShotOutput;
                    FAN16_AlarmClear_1ShotOutput                = other.FAN16_AlarmClear_1ShotOutput;
                    FAN17_AlarmClear_1ShotOutput                = other.FAN17_AlarmClear_1ShotOutput;
                    FAN18_AlarmClear_1ShotOutput                = other.FAN18_AlarmClear_1ShotOutput;
                    FAN19_AlarmClear_1ShotOutput                = other.FAN19_AlarmClear_1ShotOutput;
                    FAN20_AlarmClear_1ShotOutput                = other.FAN20_AlarmClear_1ShotOutput;
                    FAN1_OperationStop_1ShotOutput              = other.FAN1_OperationStop_1ShotOutput;
                    FAN2_OperationStop_1ShotOutput              = other.FAN2_OperationStop_1ShotOutput;
                    FAN3_OperationStop_1ShotOutput              = other.FAN3_OperationStop_1ShotOutput;
                    FAN4_OperationStop_1ShotOutput              = other.FAN4_OperationStop_1ShotOutput;
                    FAN5_OperationStop_1ShotOutput              = other.FAN5_OperationStop_1ShotOutput;
                    FAN6_OperationStop_1ShotOutput              = other.FAN6_OperationStop_1ShotOutput;
                    FAN7_OperationStop_1ShotOutput              = other.FAN7_OperationStop_1ShotOutput;
                    FAN8_OperationStop_1ShotOutput              = other.FAN8_OperationStop_1ShotOutput;
                    FAN9_OperationStop_1ShotOutput              = other.FAN9_OperationStop_1ShotOutput;
                    FAN10_OperationStop_1ShotOutput             = other.FAN10_OperationStop_1ShotOutput;
                    FAN11_OperationStop_1ShotOutput             = other.FAN11_OperationStop_1ShotOutput;
                    FAN12_OperationStop_1ShotOutput             = other.FAN12_OperationStop_1ShotOutput;
                    FAN13_OperationStop_1ShotOutput             = other.FAN13_OperationStop_1ShotOutput;
                    FAN14_OperationStop_1ShotOutput             = other.FAN14_OperationStop_1ShotOutput;
                    FAN15_OperationStop_1ShotOutput             = other.FAN15_OperationStop_1ShotOutput;
                    FAN16_OperationStop_1ShotOutput             = other.FAN16_OperationStop_1ShotOutput;
                    FAN17_OperationStop_1ShotOutput             = other.FAN17_OperationStop_1ShotOutput;
                    FAN18_OperationStop_1ShotOutput             = other.FAN18_OperationStop_1ShotOutput;
                    FAN19_OperationStop_1ShotOutput             = other.FAN19_OperationStop_1ShotOutput;
                    FAN20_OperationStop_1ShotOutput             = other.FAN20_OperationStop_1ShotOutput;
                }
            }
        }

        #endregion Private Methods

        #region Status Override

        public override object Clone()
        {
            return new RC550GeneralIoStatus(this);
        }

        #endregion Status Override
    }
}
