using System;
using System.Text;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Devices.Efem.Enums;

namespace UnitySC.EFEM.Controller.HostInterface.Statuses
{
    public class GeneralStatus
    {
        private const int NbSpeedLevels = 20;
        private const int SpeedIncrementCst = 100 / NbSpeedLevels;

        #region Constructors

        public GeneralStatus(
            OperationMode operationMode,
            RobotStatus robotStatus, Ratio robotSpeed,
            LoadPortStatus loadPortStatus1, bool isLoadPort1CarrierPresent,
            LoadPortStatus loadPortStatus2, bool isLoadPort2CarrierPresent,
            LoadPortStatus loadPortStatus3, bool isLoadPort3CarrierPresent,
            LoadPortStatus loadPortStatus4, bool isLoadPort4CarrierPresent,
            AlignerStatus alignerStatus, bool isAlignerCarrierPresent,
            bool safetyDoorSensor,
            bool vacuumSensor,
            bool airSensor)
        {
            OperationMode             = operationMode;
            RobotStatus               = robotStatus;
            RobotSpeed                = robotSpeed;
            LoadPortStatus1           = loadPortStatus1;
            IsLoadPort1CarrierPresent = isLoadPort1CarrierPresent;
            LoadPortStatus2           = loadPortStatus2;
            IsLoadPort2CarrierPresent = isLoadPort2CarrierPresent;
            LoadPortStatus3           = loadPortStatus3;
            IsLoadPort3CarrierPresent = isLoadPort3CarrierPresent;
            LoadPortStatus4           = loadPortStatus4;
            IsLoadPort4CarrierPresent = isLoadPort4CarrierPresent;
            AlignerStatus             = alignerStatus;
            IsAlignerCarrierPresent   = isAlignerCarrierPresent;
            SafetyDoorSensor          = safetyDoorSensor;
            VacuumSensor              = vacuumSensor;
            AirSensor                 = airSensor;
        }

        public GeneralStatus(string status)
        {
            var statusData = status.Replace(":", string.Empty);

            if (statusData.Length != 16)
                throw new InvalidOperationException(
                    $"Error while parsing general status: \"{statusData}\" has not the expected length.");

            var index = 0;
            OperationMode             = (OperationMode)Enum.Parse(typeof(OperationMode), statusData[index++].ToString());
            RobotStatus               = (RobotStatus)Enum.Parse(typeof(RobotStatus), statusData[index++].ToString());
            RobotSpeed                = Ratio.FromPercent((uint)ToInt(statusData[index++]) + 1) * SpeedIncrementCst;
            LoadPortStatus1           = (LoadPortStatus)Enum.Parse(typeof(LoadPortStatus), statusData[index++].ToString());
            IsLoadPort1CarrierPresent = int.Parse(statusData[index++].ToString()) == 1;
            LoadPortStatus2           = (LoadPortStatus)Enum.Parse(typeof(LoadPortStatus), statusData[index++].ToString());
            IsLoadPort2CarrierPresent = int.Parse(statusData[index++].ToString()) == 1;
            LoadPortStatus3           = (LoadPortStatus)Enum.Parse(typeof(LoadPortStatus), statusData[index++].ToString());
            IsLoadPort3CarrierPresent = int.Parse(statusData[index++].ToString()) == 1;
            LoadPortStatus4           = (LoadPortStatus)Enum.Parse(typeof(LoadPortStatus), statusData[index++].ToString());
            IsLoadPort4CarrierPresent = int.Parse(statusData[index++].ToString()) == 1;
            AlignerStatus             = (AlignerStatus)Enum.Parse(typeof(AlignerStatus), statusData[index++].ToString());
            IsAlignerCarrierPresent   = int.Parse(statusData[index++].ToString()) == 1;
            SafetyDoorSensor          = int.Parse(statusData[index++].ToString()) == 1;
            VacuumSensor              = int.Parse(statusData[index++].ToString()) == 1;
            AirSensor                 = int.Parse(statusData[index].ToString()) == 1;
        }

        #endregion Constructors

        #region Properties

        public OperationMode OperationMode { get; internal set; }

        public RobotStatus RobotStatus { get; internal set; }

        public Ratio RobotSpeed { get; internal set; }

        public LoadPortStatus LoadPortStatus1 { get; internal set; }

        public bool IsLoadPort1CarrierPresent { get; internal set; }

        public LoadPortStatus LoadPortStatus2 { get; internal set; }

        public bool IsLoadPort2CarrierPresent { get; internal set; }

        public LoadPortStatus LoadPortStatus3 { get; internal set; }

        public bool IsLoadPort3CarrierPresent { get; internal set; }

        public LoadPortStatus LoadPortStatus4 { get; internal set; }

        public bool IsLoadPort4CarrierPresent { get; internal set; }

        public AlignerStatus AlignerStatus { get; internal set; }

        public bool IsAlignerCarrierPresent { get; internal set; }

        public bool SafetyDoorSensor { get; internal set; }

        public bool VacuumSensor { get; internal set; }

        public bool AirSensor { get; internal set; }

        #endregion Properties

        public override string ToString()
        {
            var res = new StringBuilder(16);

            res.Append(((int)OperationMode).ToString());
            res.Append(((int)RobotStatus).ToString());
            res.Append(ToMajChar((uint)((RobotSpeed.Percent / SpeedIncrementCst) - 1)));
            res.Append(((int)LoadPortStatus1).ToString());
            res.Append(IsLoadPort1CarrierPresent ? '1' : '0');
            res.Append(((int)LoadPortStatus2).ToString());
            res.Append(IsLoadPort2CarrierPresent ? '1' : '0');
            res.Append(((int)LoadPortStatus3).ToString());
            res.Append(IsLoadPort3CarrierPresent ? '1' : '0');
            res.Append(((int)LoadPortStatus4).ToString());
            res.Append(IsLoadPort4CarrierPresent ? '1' : '0');
            res.Append(((int)AlignerStatus).ToString());
            res.Append(IsAlignerCarrierPresent ? '1' : '0');
            res.Append(SafetyDoorSensor ? '1' : '0');
            res.Append(VacuumSensor? '1' : '0');
            res.Append(AirSensor ? '1' : '0');

            return res.ToString();
        }

        /// <summary>
        /// Converts a char into a numerical value:
        /// - '0' to '9' return an int from 0 to 9
        /// - 'A' to 'Z' return an int from 10 to 35 (case insensitive)
        /// - everything else return -1
        /// </summary>
        private static int ToInt(char input)
        {
            input = char.ToUpperInvariant(input);

            if ('0' <= input && input <= '9')
                return input - '0';
            if ('A' <= input && input <= 'Z')
                return input - 'A' + 0xA;
            return -1;
        }

        /// <summary>
        /// Converts a numerical value into a char:
        /// - 0 to 9 return a char from '0' to '9'
        /// - 10 to 35 return a char from 'A' to 'Z'
        /// - everything else return '?'
        /// </summary>
        private static char ToMajChar(uint input)
        {
            if (input < 10)
                return input.ToString()[0];
            if (input < 36)
                return (char)(input - 10 + 'A');
            return '?';
        }
    }
}
