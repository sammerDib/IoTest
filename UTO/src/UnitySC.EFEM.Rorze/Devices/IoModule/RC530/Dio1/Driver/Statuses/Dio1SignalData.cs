using System;
using System.ComponentModel;
using System.Globalization;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1.Driver.Statuses
{
    public class Dio1SignalData : SignalData
    {
        #region Enums

        [Flags]
        // Integer length must be the same as NbInputBits
        public enum Input : UInt16
        {
            [Description("VACS1")]   PressureSensor_VAC         = 1 << 2,
            [Description("AIRS1")]   PressureSensor_AIR         = 1 << 3,
            [Description("LEDPB")]   Led_PushButton             = 1 << 4,
            [Description("AIRS2")]   PressureSensor_ION_AIR     = 1 << 5,
            [Description("IONAL1")]  Ionizer1Alarm              = 1 << 6,
            [Description("---")]     RV201Interlock             = 1 << 7,
            [Description("MODE SW")] MaintenanceSwitch          = 1 << 8,
            [Description("MC2,MC3")] DriverPower                = 1 << 9,
            [Description("RYD")]     DoorStatus                 = 1 << 10,
            [Description("RYTP")]    TPMode                     = 1 << 11,
            [Description("LC")]      LightCurtain               = 1 << 12,
            [Description("OCRLS1")]  OCRWaferReaderLimitSensor1 = 1 << 14,
            [Description("OCRLS2")]  OCRWaferReaderLimitSensor2 = 1 << 15
        }

        [Flags]
        // Integer length must be the same as NbOutputBits
        public enum Output : UInt16
        {
            [Description("Signal Tower (S.T.)")] SignalTower_LightningRed    = 1 << 0,
            [Description("Signal Tower (S.T.)")] SignalTower_LightningYellow = 1 << 1,
            [Description("Signal Tower (S.T.)")] SignalTower_LightningGreen  = 1 << 2,
            [Description("Signal Tower (S.T.)")] SignalTower_LightningBlue   = 1 << 3,
            [Description("Signal Tower (S.T.)")] SignalTower_BlinkingRed     = 1 << 4,
            [Description("Signal Tower (S.T.)")] SignalTower_BlinkingYellow  = 1 << 5,
            [Description("Signal Tower (S.T.)")] SignalTower_BlinkingGreen   = 1 << 6,
            [Description("Signal Tower (S.T.)")] SignalTower_BlinkingBlue    = 1 << 7,
            [Description("Signal Tower (S.T.)")] SignalTower_Buzzer1         = 1 << 8,
            [Description("Signal Tower (S.T.)")] SignalTower_Buzzer2         = 1 << 9,
            [Description("OCRVAL1")]             OCRWaferReaderValve1        = 1 << 10,
            [Description("OCRVAL2")]             OCRWaferReaderValve2        = 1 << 11
        }

        #endregion Enums

        #region Constructors

        /// <summary>
        /// Define an empty <see cref="SignalData"/>.
        /// Typical use would be for initializing output signal of IO card.
        /// Use it with initializer to set output signal.
        /// </summary>
        public Dio1SignalData()
            : base(16, 16)
        {
        }

        /// <summary>
        /// Define a <see cref="SignalData"/> from a string received from equipment.
        /// </summary>
        /// <exception cref="ArgumentException"> if the <see cref="signalAsString"/> has not the expected length.</exception>
        public Dio1SignalData(string signalAsString)
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

            PressureSensor_VAC         = (inputFlags & Input.PressureSensor_VAC)         != 0;
            PressureSensor_AIR         = (inputFlags & Input.PressureSensor_AIR)         != 0;
            Led_PushButton             = (inputFlags & Input.Led_PushButton)             != 0;
            PressureSensor_ION_AIR     = (inputFlags & Input.PressureSensor_ION_AIR)     != 0;
            Ionizer1Alarm              = (inputFlags & Input.Ionizer1Alarm)              != 0;
            RV201Interlock             = (inputFlags & Input.RV201Interlock)             != 0;
            MaintenanceSwitch          = (inputFlags & Input.MaintenanceSwitch)          != 0;
            DriverPower                = (inputFlags & Input.DriverPower)                != 0;
            DoorStatus                 = (inputFlags & Input.DoorStatus)                 != 0;
            TPMode                     = (inputFlags & Input.TPMode)                     != 0;
            LightCurtain               = (inputFlags & Input.LightCurtain)               != 0;
            OCRWaferReaderLimitSensor1 = (inputFlags & Input.OCRWaferReaderLimitSensor1) != 0;
            OCRWaferReaderLimitSensor2 = (inputFlags & Input.OCRWaferReaderLimitSensor2) != 0;

            var output      = parsedData[1].Substring(4);
            var outputFlags = (Output)uint.Parse(output, NumberStyles.AllowHexSpecifier);

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
            OCRWaferReaderValve1        = (outputFlags & Output.OCRWaferReaderValve1)        != 0;
            OCRWaferReaderValve2        = (outputFlags & Output.OCRWaferReaderValve2)        != 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dio1SignalData"/> class.
        /// <param name="other">Create a deep copy of <see cref="Dio1SignalData"/> instance</param>
        /// </summary>
        private Dio1SignalData(Dio1SignalData other)
            : base(other.NbInputBits, other.NbOutputBits)
        {
            Set(other);
        }

        #endregion Constructors

        #region Properties

        #region Inputs

        /// <summary>
        /// Pressure sensor (VAC)
        /// Setting P1.
        /// ON: Exceed a setting range.
        /// Name: VACS1
        /// </summary>
        public bool PressureSensor_VAC { get; private set; }

        /// <summary>
        /// Pressure sensor (AIR)
        /// Setting P1.
        /// ON: Exceed a setting range.
        /// Name: AIRS1
        /// </summary>
        public bool PressureSensor_AIR { get; private set; }

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
        /// RV201 Interlock
        /// OFF: Failure
        /// Name: ---
        /// </summary>
        public bool RV201Interlock { get; private set; }

        /// <summary>
        /// Maintenance switch
        /// ON: Selected "RUN"
        /// Name: MODE SW
        /// </summary>
        public bool MaintenanceSwitch { get; private set; }

        /// <summary>
        /// Driver power
        /// ON: Supplied
        /// Name: MC2, MC3
        /// </summary>
        public bool DriverPower { get; private set; }

        /// <summary>
        /// DOOR Status
        /// OFF: Door Open
        /// Name: RYD
        /// </summary>
        public bool DoorStatus { get; private set; }

        /// <summary>
        /// T/P Mode
        /// ON: KEY SW/Maint and T_KEY SW/ON
        /// Name: RYTP
        /// </summary>
        public bool TPMode { get; private set; }

        /// <summary>
        /// Light curtain
        /// ON: Obstructed
        /// Name: LC
        /// </summary>
        public bool LightCurtain { get; private set; }

        /// <summary>
        /// OCR Wafer Reader limit sensor 1
        /// ON: At detection
        /// Name: OCRLS1
        /// </summary>
        public bool OCRWaferReaderLimitSensor1 { get; private set; }

        /// <summary>
        /// OCR Wafer Reader limit sensor 2
        /// ON: At detection
        /// Name: OCRLS2
        /// </summary>
        public bool OCRWaferReaderLimitSensor2 { get; private set; }

        #endregion Inputs

        #region Outputs

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
        /// OCR Wafer Reader Valve 1
        /// ON: Output
        /// Name: OCRVAL1
        /// </summary>
        public bool? OCRWaferReaderValve1 { get; set; }

        /// <summary>
        /// OCR Wafer Reader Valve 2
        /// ON: Output
        /// Name: OCRVAL2
        /// </summary>
        public bool? OCRWaferReaderValve2 { get; set; }

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

            inputAsUint += PressureSensor_VAC         ? (uint)Input.PressureSensor_VAC         : 0u;
            inputAsUint += PressureSensor_AIR         ? (uint)Input.PressureSensor_AIR         : 0u;
            inputAsUint += Led_PushButton             ? (uint)Input.Led_PushButton             : 0u;
            inputAsUint += PressureSensor_ION_AIR     ? (uint)Input.PressureSensor_ION_AIR     : 0u;
            inputAsUint += Ionizer1Alarm              ? (uint)Input.Ionizer1Alarm              : 0u;
            inputAsUint += RV201Interlock             ? (uint)Input.RV201Interlock             : 0u;
            inputAsUint += MaintenanceSwitch          ? (uint)Input.MaintenanceSwitch          : 0u;
            inputAsUint += DriverPower                ? (uint)Input.DriverPower                : 0u;
            inputAsUint += DoorStatus                 ? (uint)Input.DoorStatus                 : 0u;
            inputAsUint += TPMode                     ? (uint)Input.TPMode                     : 0u;
            inputAsUint += LightCurtain               ? (uint)Input.LightCurtain               : 0u;
            inputAsUint += OCRWaferReaderLimitSensor1 ? (uint)Input.OCRWaferReaderLimitSensor1 : 0u;
            inputAsUint += OCRWaferReaderLimitSensor2 ? (uint)Input.OCRWaferReaderLimitSensor2 : 0u;

            return inputAsUint.ToString("X4");
        }

        /// <inheritdoc cref="GetSignalOutput"/>
        public override string GetSignalOutput()
        {
            var outputAsUint = 0u;

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
            outputAsUint += OCRWaferReaderValve1        != null && (bool)OCRWaferReaderValve1        ? (uint)Output.OCRWaferReaderValve1        : 0u;
            outputAsUint += OCRWaferReaderValve2        != null && (bool)OCRWaferReaderValve2        ? (uint)Output.OCRWaferReaderValve2        : 0u;

            return outputAsUint.ToString("X4");
        }

        /// <inheritdoc cref="GetOutputMask"/>
        public override string GetOutputMask()
        {
            var maskAsUint = 0u;

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
            maskAsUint += OCRWaferReaderValve1        != null ? (uint)Output.OCRWaferReaderValve1        : 0u;
            maskAsUint += OCRWaferReaderValve2        != null ? (uint)Output.OCRWaferReaderValve2        : 0u;

            return maskAsUint.Equals(0u) ? null : maskAsUint.ToString("X4");
        }

        #endregion Signal Data

        #region IClonable

        public override object Clone()
        {
            return new Dio1SignalData(this);
        }

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        private void Set(Dio1SignalData other = null)
        {
            lock (this)
            {
                if (other == null)
                {
                    PressureSensor_VAC         = false;
                    PressureSensor_AIR         = false;
                    Led_PushButton             = false;
                    PressureSensor_ION_AIR     = false;
                    Ionizer1Alarm              = false;
                    RV201Interlock             = false;
                    MaintenanceSwitch          = false;
                    DriverPower                = false;
                    DoorStatus                 = false;
                    TPMode                     = false;
                    LightCurtain               = false;
                    OCRWaferReaderLimitSensor1 = false;
                    OCRWaferReaderLimitSensor2 = false;

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
                    OCRWaferReaderValve1        = null;
                    OCRWaferReaderValve2        = null;
                }
                else
                {
                    PressureSensor_VAC         = other.PressureSensor_VAC;
                    PressureSensor_AIR         = other.PressureSensor_AIR;
                    Led_PushButton             = other.Led_PushButton;
                    PressureSensor_ION_AIR     = other.PressureSensor_ION_AIR;
                    Ionizer1Alarm              = other.Ionizer1Alarm;
                    RV201Interlock             = other.RV201Interlock;
                    MaintenanceSwitch          = other.MaintenanceSwitch;
                    DriverPower                = other.DriverPower;
                    DoorStatus                 = other.DoorStatus;
                    TPMode                     = other.TPMode;
                    LightCurtain               = other.LightCurtain;
                    OCRWaferReaderLimitSensor1 = other.OCRWaferReaderLimitSensor1;
                    OCRWaferReaderLimitSensor2 = other.OCRWaferReaderLimitSensor2;

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
                    OCRWaferReaderValve1        = other.OCRWaferReaderValve1;
                    OCRWaferReaderValve2        = other.OCRWaferReaderValve2;
                }
            }
        }

        #endregion IClonable
    }
}
