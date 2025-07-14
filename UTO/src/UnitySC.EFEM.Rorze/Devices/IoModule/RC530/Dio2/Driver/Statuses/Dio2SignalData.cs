using System;
using System.ComponentModel;
using System.Globalization;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2.Driver.Statuses
{
    public class Dio2SignalData : SignalData
    {
        #region Enums

        [Flags]
        public enum Input : UInt16
        {
            [Description("DOPM1")]                    PM1_DoorOpened        = 1 << 0,
            [Description("REPM1 - OFF: Arm disable")] PM1_ReadyToLoadUnload = 1 << 1,
            [Description("DOPM2")]                    PM2_DoorOpened        = 1 << 2,
            [Description("REPM2 - OFF: Arm disable")] PM2_ReadyToLoadUnload = 1 << 3,
            [Description("DOPM3")]                    PM3_DoorOpened        = 1 << 4,
            [Description("REPM3 - OFF: Arm disable")] PM3_ReadyToLoadUnload = 1 << 5
        }

        [Flags]
        public enum Output : UInt16
        {
            [Description("ANEPM1")] RobotArmNotExtended_PM1 = 1 << 0,
            [Description("ANEPM2")] RobotArmNotExtended_PM2 = 1 << 1,
            [Description("ANEPM3")] RobotArmNotExtended_PM3 = 1 << 2
        }

        #endregion Enums

        #region Constructors

        /// <summary>
        /// Define an empty <see cref="SignalData"/>.
        /// Typical use would be for initializing output signal of IO card.
        /// Use it with initializer to set output signal.
        /// </summary>
        public Dio2SignalData()
            : base(16, 16)
        {
        }

        public Dio2SignalData(string signalAsString)
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

            PM1_DoorOpened        = (inputFlags & Input.PM1_DoorOpened)        != 0;
            PM1_ReadyToLoadUnload = (inputFlags & Input.PM1_ReadyToLoadUnload) != 0;
            PM2_DoorOpened        = (inputFlags & Input.PM2_DoorOpened)        != 0;
            PM2_ReadyToLoadUnload = (inputFlags & Input.PM2_ReadyToLoadUnload) != 0;
            PM3_DoorOpened        = (inputFlags & Input.PM3_DoorOpened)        != 0;
            PM3_ReadyToLoadUnload = (inputFlags & Input.PM3_ReadyToLoadUnload) != 0;

            var output      = parsedData[1].Substring(4);
            var outputFlags = (Output)uint.Parse(output, NumberStyles.AllowHexSpecifier);

            RobotArmNotExtended_PM1 = (outputFlags & Output.RobotArmNotExtended_PM1) != 0;
            RobotArmNotExtended_PM2 = (outputFlags & Output.RobotArmNotExtended_PM2) != 0;
            RobotArmNotExtended_PM3 = (outputFlags & Output.RobotArmNotExtended_PM3) != 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dio2SignalData"/> class.
        /// <param name="other">Create a deep copy of <see cref="Dio2SignalData"/> instance</param>
        /// </summary>
        private Dio2SignalData(Dio2SignalData other)
            : base(other.NbInputBits, other.NbOutputBits)
        {
            Set(other);
        }

        #endregion Constructors

        #region Properties

        #region Inputs

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

            inputAsUint += PM1_DoorOpened        ? (uint)Input.PM1_DoorOpened        : 0u;
            inputAsUint += PM1_ReadyToLoadUnload ? (uint)Input.PM1_ReadyToLoadUnload : 0u;
            inputAsUint += PM2_DoorOpened        ? (uint)Input.PM2_DoorOpened        : 0u;
            inputAsUint += PM2_ReadyToLoadUnload ? (uint)Input.PM2_ReadyToLoadUnload : 0u;
            inputAsUint += PM3_DoorOpened        ? (uint)Input.PM3_DoorOpened        : 0u;
            inputAsUint += PM3_ReadyToLoadUnload ? (uint)Input.PM3_ReadyToLoadUnload : 0u;

            return inputAsUint.ToString("X4");
        }

        /// <inheritdoc cref="GetSignalOutput"/>
        public override string GetSignalOutput()
        {
            var outputAsUint = 0u;

            outputAsUint += RobotArmNotExtended_PM1 != null && (bool)RobotArmNotExtended_PM1 ? (uint)Output.RobotArmNotExtended_PM1 : 0u;
            outputAsUint += RobotArmNotExtended_PM2 != null && (bool)RobotArmNotExtended_PM2 ? (uint)Output.RobotArmNotExtended_PM2 : 0u;
            outputAsUint += RobotArmNotExtended_PM3 != null && (bool)RobotArmNotExtended_PM3 ? (uint)Output.RobotArmNotExtended_PM3 : 0u;

            return outputAsUint.ToString("X4");
        }

        /// <inheritdoc cref="GetOutputMask"/>
        public override string GetOutputMask()
        {
            var maskAsUint = 0u;

            maskAsUint += RobotArmNotExtended_PM1 != null ? (uint)Output.RobotArmNotExtended_PM1 : 0u;
            maskAsUint += RobotArmNotExtended_PM2 != null ? (uint)Output.RobotArmNotExtended_PM2 : 0u;
            maskAsUint += RobotArmNotExtended_PM3 != null ? (uint)Output.RobotArmNotExtended_PM3 : 0u;

            return maskAsUint.Equals(0u) ? null : maskAsUint.ToString("X4");
        }

        #endregion Signal Data

        #region IClonable

        public override object Clone()
        {
            return new Dio2SignalData(this);
        }

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        private void Set(Dio2SignalData other = null)
        {
            lock (this)
            {
                if (other == null)
                {
                    PM1_DoorOpened        = false;
                    PM1_ReadyToLoadUnload = false;
                    PM2_DoorOpened        = false;
                    PM2_ReadyToLoadUnload = false;
                    PM3_DoorOpened        = false;
                    PM3_ReadyToLoadUnload = false;

                    RobotArmNotExtended_PM1 = null;
                    RobotArmNotExtended_PM2 = null;
                    RobotArmNotExtended_PM3 = null;
                }
                else
                {
                    PM1_DoorOpened        = other.PM1_DoorOpened;
                    PM1_ReadyToLoadUnload = other.PM1_ReadyToLoadUnload;
                    PM2_DoorOpened        = other.PM2_DoorOpened;
                    PM2_ReadyToLoadUnload = other.PM2_ReadyToLoadUnload;
                    PM3_DoorOpened        = other.PM3_DoorOpened;
                    PM3_ReadyToLoadUnload = other.PM3_ReadyToLoadUnload;

                    RobotArmNotExtended_PM1 = other.RobotArmNotExtended_PM1;
                    RobotArmNotExtended_PM2 = other.RobotArmNotExtended_PM2;
                    RobotArmNotExtended_PM3 = other.RobotArmNotExtended_PM3;
                }
            }
        }

        #endregion IClonable
    }
}
