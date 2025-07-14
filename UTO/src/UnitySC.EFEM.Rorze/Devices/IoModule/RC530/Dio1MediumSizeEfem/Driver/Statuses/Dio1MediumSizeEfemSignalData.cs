using System;
using System.ComponentModel;
using System.Globalization;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1MediumSizeEfem.Driver.Statuses
{
    public class Dio1MediumSizeEfemSignalData : SignalData
    {
        #region Enums

        [Flags]
        // Integer length must be the same as NbInputBits
        public enum Input : UInt16
        {
            [Description("MODE SW")]                  MaintenanceSwitch          = 1 << 0,
            [Description("VACS1")]                    PressureSensor_VAC         = 1 << 2,
            [Description("LEDPB")]                    Led_PushButton             = 1 << 4,
            [Description("AIRS2")]                    PressureSensor_ION_AIR     = 1 << 5,
            [Description("IONAL1")]                   Ionizer1Alarm              = 1 << 6,
            [Description("LC")]                       LightCurtain               = 1 << 8,
            [Description("DOPM1")]                    PM1_DoorOpened             = 1 << 9,
            [Description("REPM1 - OFF: Arm disable")] PM1_ReadyToLoadUnload      = 1 << 10,
            [Description("DOPM2")]                    PM2_DoorOpened             = 1 << 11,
            [Description("REPM2 - OFF: Arm disable")] PM2_ReadyToLoadUnload      = 1 << 12,
            [Description("DOPM3")]                    PM3_DoorOpened             = 1 << 13,
            [Description("REPM3 - OFF: Arm disable")] PM3_ReadyToLoadUnload      = 1 << 14
        }

        [Flags]
        // Integer length must be the same as NbOutputBits
        public enum Output : UInt16
        {
            [Description("ANEPM1")]              RobotArmNotExtended_PM1     = 1 << 0,
            [Description("ANEPM2")]              RobotArmNotExtended_PM2     = 1 << 1,
            [Description("ANEPM3")]              RobotArmNotExtended_PM3     = 1 << 2,
            [Description("Signal Tower (S.T.)")] SignalTower_LightningRed    = 1 << 3,
            [Description("Signal Tower (S.T.)")] SignalTower_LightningYellow = 1 << 4,
            [Description("Signal Tower (S.T.)")] SignalTower_LightningGreen  = 1 << 5,
            [Description("Signal Tower (S.T.)")] SignalTower_LightningBlue   = 1 << 6,
            [Description("Signal Tower (S.T.)")] SignalTower_BlinkingRed     = 1 << 7,
            [Description("Signal Tower (S.T.)")] SignalTower_BlinkingYellow  = 1 << 8,
            [Description("Signal Tower (S.T.)")] SignalTower_BlinkingGreen   = 1 << 9,
            [Description("Signal Tower (S.T.)")] SignalTower_BlinkingBlue    = 1 << 10,
            [Description("Signal Tower (S.T.)")] SignalTower_Buzzer1         = 1 << 11,
            [Description("Signal Tower (S.T.)")] SignalTower_Buzzer2         = 1 << 12,
            [Description("Signal Tower PS")]     SignalTowerPowerSupply      = 1 << 13,
        }

        #endregion Enums

        #region Constructors

        /// <summary>
        /// Define an empty <see cref="SignalData"/>.
        /// Typical use would be for initializing output signal of IO card.
        /// Use it with initializer to set output signal.
        /// </summary>
        public Dio1MediumSizeEfemSignalData()
            : base(16, 16)
        {
        }

        /// <summary>
        /// Define a <see cref="SignalData"/> from a string received from equipment.
        /// </summary>
        /// <exception cref="ArgumentException"> if the <see cref="signalAsString"/> has not the expected length.</exception>
        public Dio1MediumSizeEfemSignalData(string signalAsString)
            : base(16, 16)
        {
            var parsedData = signalAsString.Split('/');

            // There must be at least the I/O module No. and one I/O status in the received data.
            if (parsedData.Length != 2)
            {
                throw new ArgumentException(
                    "Invalid format of argument.\n"
                    + "Format is \"00/********\" where '*' is a one-digit hexadecimal number.\n"
                    + $"Received data is {signalAsString}.");
            }

            // The I/O module No. must be an integer
            if (!uint.TryParse(parsedData[0], out var ioModuleId))
            {
                throw new ArgumentException(
                    "The I/O module No. must be an integer.\n"
                    + "Format is \"00/********\" where '*' is a one-digit hexadecimal number.\n"
                    + $"Received data is {signalAsString}.");
            }

            IoModuleNo = ioModuleId;

            // Expected signal is an hexadecimal number with a fixed length.
            if (parsedData[1].Length != (NbInputBits + NbOutputBits) / 4)
            {
                throw new ArgumentException("Argument has not the expected length.\n"
                                            + $"Expected: {(NbInputBits + NbOutputBits) / 4}\n"
                                            + $"Obtained: {parsedData[1].Length}");
            }

            var input      = parsedData[1].Substring(0, 4);
            var inputFlags = (Input)uint.Parse(input, NumberStyles.AllowHexSpecifier);

            MaintenanceSwitch = (inputFlags & Input.MaintenanceSwitch)                   != 0;
            PressureSensor_VAC         = (inputFlags & Input.PressureSensor_VAC)         != 0;
            Led_PushButton             = (inputFlags & Input.Led_PushButton)             != 0;
            PressureSensor_ION_AIR     = (inputFlags & Input.PressureSensor_ION_AIR)     != 0;
            Ionizer1Alarm              = (inputFlags & Input.Ionizer1Alarm)              != 0;
            LightCurtain               = (inputFlags & Input.LightCurtain)               != 0;
            PM1_DoorOpened             = (inputFlags & Input.PM1_DoorOpened)             != 0;
            PM1_ReadyToLoadUnload      = (inputFlags & Input.PM1_ReadyToLoadUnload)      != 0;
            PM2_DoorOpened             = (inputFlags & Input.PM2_DoorOpened)             != 0;
            PM2_ReadyToLoadUnload      = (inputFlags & Input.PM2_ReadyToLoadUnload)      != 0;
            PM3_DoorOpened             = (inputFlags & Input.PM3_DoorOpened)             != 0;
            PM3_ReadyToLoadUnload      = (inputFlags & Input.PM3_ReadyToLoadUnload)      != 0;

            var output      = parsedData[1].Substring(4);
            var outputFlags = (Output)uint.Parse(output, NumberStyles.AllowHexSpecifier);

            RobotArmNotExtended_PM1     = (outputFlags & Output.RobotArmNotExtended_PM1)     != 0;
            RobotArmNotExtended_PM2     = (outputFlags & Output.RobotArmNotExtended_PM2)     != 0;
            RobotArmNotExtended_PM3     = (outputFlags & Output.RobotArmNotExtended_PM3)     != 0;
            SignalTower_LightningRed    = (outputFlags & Output.SignalTower_LightningRed)    != 0;
            SignalTower_LightningYellow = (outputFlags & Output.SignalTower_LightningYellow) != 0;
            SignalTower_LightningGreen  = (outputFlags & Output.SignalTower_LightningGreen)  != 0;
            SignalTower_LightningBlue   = (outputFlags & Output.SignalTower_LightningBlue)   != 0;
            SignalTower_BlinkingRed     = (outputFlags & Output.SignalTower_BlinkingRed)     != 0;
            SignalTower_BlinkingYellow  = (outputFlags & Output.SignalTower_BlinkingYellow)  != 0;
            SignalTower_BlinkingGreen   = (outputFlags & Output.SignalTower_BlinkingGreen)   != 0;
            SignalTower_BlinkingBlue    = (outputFlags & Output.SignalTower_BlinkingBlue)    != 0;
            SignalTower_Buzzer1         = (outputFlags & Output.SignalTower_Buzzer1)         != 0;
            SignalTower_Buzzer2         = (outputFlags & Output.SignalTower_Buzzer2)         != 0;
            SignalTowerPowerSupply      = (outputFlags & Output.SignalTowerPowerSupply)      != 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dio1MediumSizeEfemSignalData"/> class.
        /// <param name="other">Create a deep copy of <see cref="Dio1MediumSizeEfemSignalData"/> instance</param>
        /// </summary>
        private Dio1MediumSizeEfemSignalData(Dio1MediumSizeEfemSignalData other)
            : base(other.NbInputBits, other.NbOutputBits)
        {
            Set(other);
        }

        #endregion Constructors

        #region Properties

        #region Inputs

        /// <summary>
        /// Maintenance switch
        /// ON: Selected "RUN"
        /// Name: MODE SW
        /// </summary>
        public bool MaintenanceSwitch { get; private set; }

        /// <summary>
        /// Pressure sensor (VAC)
        /// Setting P1.
        /// ON: Exceed a setting range.
        /// Name: VACS1
        /// </summary>
        public bool PressureSensor_VAC { get; private set; }

        /// <summary>
        /// LED Push button
        /// ON: At action
        /// Name: LEDPB
        /// </summary>
        public bool Led_PushButton { get; private set; }

        /// <summary>
        /// Pressure sensor (ON AIR)
        /// Setting P1
        /// ON: Exceed a setting range.
        /// Name: AIRS2
        /// </summary>
        public bool PressureSensor_ION_AIR { get; private set; }

        /// <summary>
        /// IONIZER 1 Alarm
        /// OFF: Failure
        /// Name: IONAL1
        /// </summary>
        public bool Ionizer1Alarm { get; private set; }

        /// <summary>
        /// Light curtain
        /// ON: Obstructed
        /// Name: LC
        /// </summary>
        public bool LightCurtain { get; private set; }

        /// <summary>
        /// PM1 Door opened.
        /// OFF: Door closed.
        /// Name: DOPM1
        /// </summary>
        public bool PM1_DoorOpened { get; private set; }

        /// <summary>
        /// PM1 Ready to load/unload.
        /// OFF: Arm disable.
        /// Name: REPM1
        /// </summary>
        public bool PM1_ReadyToLoadUnload { get; private set; }

        /// <summary>
        /// PM2 Door opened.
        /// OFF: Door closed.
        /// Name: DOPM2
        /// </summary>
        public bool PM2_DoorOpened { get; private set; }

        /// <summary>
        /// PM2 Ready to load/unload.
        /// OFF: Arm disable.
        /// Name: REPM2
        /// </summary>
        public bool PM2_ReadyToLoadUnload { get; private set; }

        /// <summary>
        /// PM3 Door opened.
        /// OFF: Door closed.
        /// Name: DOPM3
        /// </summary>
        public bool PM3_DoorOpened { get; private set; }

        /// <summary>
        /// PM3 Ready to load/unload.
        /// OFF: Arm disable.
        /// Name: REPM3
        /// </summary>
        public bool PM3_ReadyToLoadUnload { get; private set; }

        #endregion Inputs

        #region Outputs

        /// <summary>
        /// Robot arm not extended PM1.
        /// ON: Not extended.
        /// Name: ANEPM1
        /// </summary>
        public bool? RobotArmNotExtended_PM1 { get; internal set; }

        /// <summary>
        /// Robot arm not extended PM2.
        /// ON: Not extended.
        /// Name: ANEPM2
        /// </summary>
        public bool? RobotArmNotExtended_PM2 { get; internal set; }

        /// <summary>
        /// Robot arm not extended PM3.
        /// ON: Not extended.
        /// Name: ANEPM3
        /// </summary>
        public bool? RobotArmNotExtended_PM3 { get; internal set; }

        /// <summary>
        /// Signal tower
        /// Lightning: Red
        /// Name: S.T.
        /// </summary>
        public bool? SignalTower_LightningRed { get; set; }

        /// <summary>
        /// Signal tower
        /// Lightning: Yellow
        /// Name: S.T.
        /// </summary>
        public bool? SignalTower_LightningYellow { get; set; }

        /// <summary>
        /// Signal tower
        /// Lightning: Green
        /// Name: S.T.
        /// </summary>
        public bool? SignalTower_LightningGreen { get; set; }

        /// <summary>
        /// Signal tower
        /// Lightning: Blue
        /// Name: S.T.
        /// </summary>
        public bool? SignalTower_LightningBlue { get; set; }

        /// <summary>
        /// Signal tower
        /// Blinking: Red
        /// Name: S.T.
        /// </summary>
        public bool? SignalTower_BlinkingRed { get; set; }

        /// <summary>
        /// Signal tower
        /// Blinking: Yellow
        /// Name: S.T.
        /// </summary>
        public bool? SignalTower_BlinkingYellow { get; set; }

        /// <summary>
        /// Signal tower
        /// Blinking: Green
        /// Name: S.T.
        /// </summary>
        public bool? SignalTower_BlinkingGreen { get; set; }

        /// <summary>
        /// Signal tower
        /// Blinking: Blue
        /// Name: S.T.
        /// </summary>
        public bool? SignalTower_BlinkingBlue { get; set; }

        /// <summary>
        /// Signal tower
        /// Buzzer 1 continuous sound
        /// Name: S.T.
        /// </summary>
        public bool? SignalTower_Buzzer1 { get; set; }

        /// <summary>
        /// Signal tower
        /// Buzzer 2 continuous sound
        /// Name: S.T.
        /// </summary>
        public bool? SignalTower_Buzzer2 { get; set; }

        /// <summary>
        /// Signal tower power supply
        /// ON: Output
        /// Name: S.T.
        /// </summary>
        public bool? SignalTowerPowerSupply { get; set; }

        #endregion Outputs

        #endregion Properties

        #region Signal Data

        /// <inheritdoc cref="GetSignal"/>
        public override string GetSignal()
        {
            return GetSignalInput() + GetSignalOutput();
        }

        /// <inheritdoc cref="GetSignalInput"/>
        public override string GetSignalInput()
        {
            var inputAsUint = 0u;

            inputAsUint += MaintenanceSwitch          ? (uint)Input.MaintenanceSwitch          : 0u;
            inputAsUint += PressureSensor_VAC         ? (uint)Input.PressureSensor_VAC         : 0u;
            inputAsUint += Led_PushButton             ? (uint)Input.Led_PushButton             : 0u;
            inputAsUint += PressureSensor_ION_AIR     ? (uint)Input.PressureSensor_ION_AIR     : 0u;
            inputAsUint += Ionizer1Alarm              ? (uint)Input.Ionizer1Alarm              : 0u;
            inputAsUint += LightCurtain               ? (uint)Input.LightCurtain               : 0u;
            inputAsUint += PM1_DoorOpened             ? (uint)Input.PM1_DoorOpened             : 0u;
            inputAsUint += PM1_ReadyToLoadUnload      ? (uint)Input.PM1_ReadyToLoadUnload      : 0u;
            inputAsUint += PM2_DoorOpened             ? (uint)Input.PM2_DoorOpened             : 0u;
            inputAsUint += PM2_ReadyToLoadUnload      ? (uint)Input.PM2_ReadyToLoadUnload      : 0u;
            inputAsUint += PM3_DoorOpened             ? (uint)Input.PM3_DoorOpened             : 0u;
            inputAsUint += PM3_ReadyToLoadUnload      ? (uint)Input.PM3_ReadyToLoadUnload      : 0u;

            return inputAsUint.ToString("X4");
        }

        /// <inheritdoc cref="GetSignalOutput"/>
        public override string GetSignalOutput()
        {
            var outputAsUint = 0u;

            outputAsUint += RobotArmNotExtended_PM1     != null && (bool)RobotArmNotExtended_PM1     ? (uint)Output.RobotArmNotExtended_PM1     : 0u;
            outputAsUint += RobotArmNotExtended_PM2     != null && (bool)RobotArmNotExtended_PM2     ? (uint)Output.RobotArmNotExtended_PM2     : 0u;
            outputAsUint += RobotArmNotExtended_PM3     != null && (bool)RobotArmNotExtended_PM3     ? (uint)Output.RobotArmNotExtended_PM3     : 0u;
            outputAsUint += SignalTower_LightningRed    != null && (bool)SignalTower_LightningRed    ? (uint)Output.SignalTower_LightningRed    : 0u;
            outputAsUint += SignalTower_LightningYellow != null && (bool)SignalTower_LightningYellow ? (uint)Output.SignalTower_LightningYellow : 0u;
            outputAsUint += SignalTower_LightningGreen  != null && (bool)SignalTower_LightningGreen  ? (uint)Output.SignalTower_LightningGreen  : 0u;
            outputAsUint += SignalTower_LightningBlue   != null && (bool)SignalTower_LightningBlue   ? (uint)Output.SignalTower_LightningBlue   : 0u;
            outputAsUint += SignalTower_BlinkingRed     != null && (bool)SignalTower_BlinkingRed     ? (uint)Output.SignalTower_BlinkingRed     : 0u;
            outputAsUint += SignalTower_BlinkingYellow  != null && (bool)SignalTower_BlinkingYellow  ? (uint)Output.SignalTower_BlinkingYellow  : 0u;
            outputAsUint += SignalTower_BlinkingGreen   != null && (bool)SignalTower_BlinkingGreen   ? (uint)Output.SignalTower_BlinkingGreen   : 0u;
            outputAsUint += SignalTower_BlinkingBlue    != null && (bool)SignalTower_BlinkingBlue    ? (uint)Output.SignalTower_BlinkingBlue    : 0u;
            outputAsUint += SignalTower_Buzzer1         != null && (bool)SignalTower_Buzzer1         ? (uint)Output.SignalTower_Buzzer1         : 0u;
            outputAsUint += SignalTower_Buzzer2         != null && (bool)SignalTower_Buzzer2         ? (uint)Output.SignalTower_Buzzer2         : 0u;
            outputAsUint += SignalTowerPowerSupply        != null && (bool)SignalTowerPowerSupply    ? (uint)Output.SignalTowerPowerSupply      : 0u;

            return outputAsUint.ToString("X4");
        }

        /// <inheritdoc cref="GetOutputMask"/>
        public override string GetOutputMask()
        {
            var maskAsUint = 0u;

            maskAsUint += RobotArmNotExtended_PM1     != null ? (uint)Output.RobotArmNotExtended_PM1     : 0u;
            maskAsUint += RobotArmNotExtended_PM2     != null ? (uint)Output.RobotArmNotExtended_PM2     : 0u;
            maskAsUint += RobotArmNotExtended_PM3     != null ? (uint)Output.RobotArmNotExtended_PM3     : 0u;
            maskAsUint += SignalTower_LightningRed    != null ? (uint)Output.SignalTower_LightningRed    : 0u;
            maskAsUint += SignalTower_LightningYellow != null ? (uint)Output.SignalTower_LightningYellow : 0u;
            maskAsUint += SignalTower_LightningGreen  != null ? (uint)Output.SignalTower_LightningGreen  : 0u;
            maskAsUint += SignalTower_LightningBlue   != null ? (uint)Output.SignalTower_LightningBlue   : 0u;
            maskAsUint += SignalTower_BlinkingRed     != null ? (uint)Output.SignalTower_BlinkingRed     : 0u;
            maskAsUint += SignalTower_BlinkingYellow  != null ? (uint)Output.SignalTower_BlinkingYellow  : 0u;
            maskAsUint += SignalTower_BlinkingGreen   != null ? (uint)Output.SignalTower_BlinkingGreen   : 0u;
            maskAsUint += SignalTower_BlinkingBlue    != null ? (uint)Output.SignalTower_BlinkingBlue    : 0u;
            maskAsUint += SignalTower_Buzzer1         != null ? (uint)Output.SignalTower_Buzzer1         : 0u;
            maskAsUint += SignalTower_Buzzer2         != null ? (uint)Output.SignalTower_Buzzer2         : 0u;
            maskAsUint += SignalTowerPowerSupply      != null ? (uint)Output.SignalTowerPowerSupply      : 0u;

            return maskAsUint.Equals(0u) ? null : maskAsUint.ToString("X4");
        }

        #endregion Signal Data

        #region IClonable

        public override object Clone()
        {
            return new Dio1MediumSizeEfemSignalData(this);
        }

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        private void Set(Dio1MediumSizeEfemSignalData other = null)
        {
            lock (this)
            {
                if (other == null)
                {
                    MaintenanceSwitch          = false;
                    PressureSensor_VAC         = false;
                    Led_PushButton             = false;
                    PressureSensor_ION_AIR     = false;
                    Ionizer1Alarm              = false;
                    LightCurtain               = false;
                    PM1_DoorOpened             = false;
                    PM1_ReadyToLoadUnload      = false;
                    PM2_DoorOpened             = false;
                    PM2_ReadyToLoadUnload      = false;
                    PM3_DoorOpened             = false;
                    PM3_ReadyToLoadUnload      = false;

                    RobotArmNotExtended_PM1     = null;
                    RobotArmNotExtended_PM2     = null;
                    RobotArmNotExtended_PM3     = null;
                    SignalTower_LightningRed    = null;
                    SignalTower_LightningYellow = null;
                    SignalTower_LightningGreen  = null;
                    SignalTower_LightningBlue   = null;
                    SignalTower_BlinkingRed     = null;
                    SignalTower_BlinkingYellow  = null;
                    SignalTower_BlinkingGreen   = null;
                    SignalTower_BlinkingBlue    = null;
                    SignalTower_Buzzer1         = null;
                    SignalTower_Buzzer2         = null;
                    SignalTowerPowerSupply      = null;
                }
                else
                {
                    MaintenanceSwitch          = other.MaintenanceSwitch;
                    PressureSensor_VAC         = other.PressureSensor_VAC;
                    Led_PushButton             = other.Led_PushButton;
                    PressureSensor_ION_AIR     = other.PressureSensor_ION_AIR;
                    Ionizer1Alarm              = other.Ionizer1Alarm;
                    LightCurtain               = other.LightCurtain;
                    PM1_DoorOpened             = other.PM1_DoorOpened;
                    PM1_ReadyToLoadUnload      = other.PM1_ReadyToLoadUnload;
                    PM2_DoorOpened             = other.PM2_DoorOpened;
                    PM2_ReadyToLoadUnload      = other.PM2_ReadyToLoadUnload;
                    PM3_DoorOpened             = other.PM3_DoorOpened;
                    PM3_ReadyToLoadUnload      = other.PM3_ReadyToLoadUnload;

                    RobotArmNotExtended_PM1     = other.RobotArmNotExtended_PM1;
                    RobotArmNotExtended_PM2     = other.RobotArmNotExtended_PM2;
                    RobotArmNotExtended_PM3     = other.RobotArmNotExtended_PM3;
                    SignalTower_LightningRed    = other.SignalTower_LightningRed;
                    SignalTower_LightningYellow = other.SignalTower_LightningYellow;
                    SignalTower_LightningGreen  = other.SignalTower_LightningGreen;
                    SignalTower_LightningBlue   = other.SignalTower_LightningBlue;
                    SignalTower_BlinkingRed     = other.SignalTower_BlinkingRed;
                    SignalTower_BlinkingYellow  = other.SignalTower_BlinkingYellow;
                    SignalTower_BlinkingGreen   = other.SignalTower_BlinkingGreen;
                    SignalTower_BlinkingBlue    = other.SignalTower_BlinkingBlue;
                    SignalTower_Buzzer1         = other.SignalTower_Buzzer1;
                    SignalTower_Buzzer2         = other.SignalTower_Buzzer2;
                    SignalTowerPowerSupply      = other.SignalTowerPowerSupply;
                }
            }
        }

        #endregion IClonable
    }
}
