using System;
using System.Globalization;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Enums;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Statuses
{
    /// <summary>
    /// Define IO statuses available for SB019-1 IO card.
    /// </summary>
    public class FanDetectionSignalData : SignalData
    {
        #region Enums

        [Flags]
        public enum Input : UInt16
        {
            FanDetection1 = 1 << 0,
            FanDetection2 = 1 << 1,
            DvrAlarm      = 1 << 2
        }

        #endregion Enums

        #region Constructors

        /// <summary>
        /// Define an empty <see cref="FanDetectionSignalData"/>.
        /// Typical use would be for initializing output signal of IO card.
        /// Use it with initializer to set output signal.
        /// </summary>
        public FanDetectionSignalData()
            : base(16, 16)
        {
        }

        public FanDetectionSignalData(string signalAsString)
            : this((uint)IoModuleIds.RC550_HCL3_ID0, signalAsString)
        {
        }

        /// <summary>
        /// Define a <see cref="FanDetectionSignalData"/> from a string received from equipment.
        /// </summary>
        /// <exception cref="ArgumentException"> if the <see cref="signalAsString"/> has not the expected length.</exception>
        public FanDetectionSignalData(uint ioModuleId, string signalAsString)
            : base(16, 16)
        {
            // Expected signal is an hexadecimal number with a fixed length.
            if (signalAsString.Length != (NbInputBits + NbOutputBits) / 4)
            {
                throw new ArgumentException("Argument has not the expected length.\n"
                                            + $"Expected: {(NbInputBits + NbOutputBits) / 4}\n"
                                            + $"Obtained: {signalAsString.Length}");
            }

            var input      = signalAsString.Substring(0, 4);
            var inputFlags = (Input)uint.Parse(input, NumberStyles.AllowHexSpecifier);

            FanDetection1 = (inputFlags & Input.FanDetection1) != 0;
            FanDetection2 = (inputFlags & Input.FanDetection2) != 0;
            DvrAlarm      = (inputFlags & Input.DvrAlarm)      != 0;
            IoModuleNo    = ioModuleId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FanDetectionSignalData"/> class.
        /// <param name="other">Create a deep copy of <see cref="FanDetectionSignalData"/> instance</param>
        /// </summary>
        private FanDetectionSignalData(FanDetectionSignalData other)
            : base(other.NbInputBits, other.NbOutputBits)
        {
            Set(other);
        }

        #endregion Cosntructors

        #region Properties

        public bool FanDetection1 { get; private set; }

        public bool FanDetection2 { get; private set; }

        public bool DvrAlarm { get; private set; }

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

            inputAsUint += FanDetection1 ? (uint)Input.FanDetection1 : 0u;
            inputAsUint += FanDetection2 ? (uint)Input.FanDetection2 : 0u;
            inputAsUint += DvrAlarm      ? (uint)Input.DvrAlarm      : 0u;

            return inputAsUint.ToString("X4");
        }

        /// <inheritdoc cref="GetSignalOutput"/>
        public override string GetSignalOutput()
        {
            // No output is defined for this IO Signal Data.
            return 0u.ToString("X2");
        }

        /// <inheritdoc cref="GetOutputMask"/>
        public override string GetOutputMask()
        {
            // No output is defined for this IO Signal Data.
            return 0u.ToString("X2");
        }

        #endregion Signal Data

        #region IClonable

        public override object Clone()
        {
            return new FanDetectionSignalData(this);
        }

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        private void Set(FanDetectionSignalData other = null)
        {
            lock (this)
            {
                if (other == null)
                {
                    FanDetection1 = false;
                    FanDetection2 = false;
                    DvrAlarm      = false;
                }
                else
                {
                    FanDetection1 = other.FanDetection1;
                    FanDetection2 = other.FanDetection2;
                    DvrAlarm      = other.DvrAlarm;
                }
            }
        }

        #endregion IClonable
    }
}
