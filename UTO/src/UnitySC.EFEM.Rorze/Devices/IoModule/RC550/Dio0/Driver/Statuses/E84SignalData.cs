using System;
using System.Globalization;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Statuses
{
    public class E84SignalData : SignalData
    {
        #region Fields

        private readonly uint _baseIoModuleIdIds;

        #endregion Fields

        #region Enums

        [Flags]
        public enum Input : UInt16
        {
            Valid  = 1 << 0,
            Cs_0   = 1 << 1,
            Cs_1   = 1 << 2,
            Tr_Req = 1 << 4,
            Busy   = 1 << 5,
            Compt  = 1 << 6,
            Cont   = 1 << 7,
        }

        [Flags]
        public enum Output : UInt16
        {
            L_Req   = 1 << 0,
            U_Req   = 1 << 1,
            Ready   = 1 << 3,
            Ho_Avbl = 1 << 2, //second module
            Es      = 1 << 3, //second module
        }

        #endregion Enums

        #region Properties

        #region Inputs

        public bool I_Valid { get; private set; }

        public bool I_Cs_0 { get; private set; }

        public bool I_Cs_1 { get; private set; }

        public bool I_Tr_Req { get; private set; }

        public bool I_Busy { get; private set; }

        public bool I_Compt { get; private set; }

        public bool I_Cont { get; private set; }

        #endregion Inputs

        #region Outputs

        public bool? O_L_Req { get; set; }

        public bool? O_U_Req { get; set; }

        public bool? O_Ready { get; set; }

        public bool? O_Ho_Avbl { get; set; }

        public bool? O_Es { get; set; }

        #endregion Outputs

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Define an empty <see cref="E84SignalData"/>.
        /// Typical use would be for initializing output signal of IO card.
        /// Use it with initializer to set output signal.
        /// </summary>
        public E84SignalData()
            : base(16, 16)
        {
        }

        /// <summary>
        /// Define an empty <see cref="E84SignalData"/>.
        /// Typical use would be for initializing output signal of IO card.
        /// Use it with initializer to set output signal.
        /// </summary>
        public E84SignalData(uint baseIoModuleId, uint ioModuleId)
            : base(16, 16)
        {
            _baseIoModuleIdIds = baseIoModuleId;
            IoModuleNo         = ioModuleId;
        }

        /// <summary>
        /// Define a <see cref="E84SignalData"/> from a string received from equipment.
        /// </summary>
        /// <exception cref="ArgumentException"> if the <see cref="signalAsString"/> has not the expected length.</exception>
        public E84SignalData(uint baseIoModuleId, uint ioModuleId, string signalAsString)
            : base(16, 16)
        {
            _baseIoModuleIdIds = baseIoModuleId;
            IoModuleNo         = ioModuleId;

            // Expected signal is an hexadecimal number with a fixed length.
            if (signalAsString.Length != (NbInputBits + NbOutputBits) / 4)
            {
                throw new ArgumentException("Argument has not the expected length.\n"
                                            + $"Expected: {(NbInputBits + NbOutputBits) / 4}\n"
                                            + $"Obtained: {signalAsString.Length}");
            }

            var input = signalAsString.Substring(0, 4);
            var inputFlags = (Input)uint.Parse(input, NumberStyles.AllowHexSpecifier);

            if (_baseIoModuleIdIds == IoModuleNo)
            {
                I_Valid  = (inputFlags & Input.Valid)  != 0;
                I_Cs_0   = (inputFlags & Input.Cs_0)   != 0;
                I_Cs_1   = (inputFlags & Input.Cs_1)   != 0;
                I_Tr_Req = (inputFlags & Input.Tr_Req) != 0;
                I_Busy   = (inputFlags & Input.Busy)   != 0;
                I_Compt  = (inputFlags & Input.Compt)  != 0;
                I_Cont   = (inputFlags & Input.Cont)   != 0;
            }

            var output      = signalAsString.Substring(4);
            var outputFlags = (Output)uint.Parse(output, NumberStyles.AllowHexSpecifier);

            if (_baseIoModuleIdIds == IoModuleNo)
            {
                O_L_Req = (outputFlags & Output.L_Req) != 0;
                O_U_Req = (outputFlags & Output.U_Req) != 0;
                O_Ready = (outputFlags & Output.Ready) != 0;
            }
            else
            {
                O_Ho_Avbl = (outputFlags & Output.Ho_Avbl) != 0;
                O_Es      = (outputFlags & Output.Es) != 0;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FanDetectionSignalData"/> class.
        /// <param name="other">Create a deep copy of <see cref="FanDetectionSignalData"/> instance</param>
        /// </summary>
        private E84SignalData(E84SignalData other)
            : base(other.NbInputBits, other.NbOutputBits)
        {
            Set(other);
        }

        #endregion Constructors

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
            if (_baseIoModuleIdIds == IoModuleNo)
            {
                inputAsUint += I_Valid  ? (uint)Input.Valid  : 0u;
                inputAsUint += I_Cs_0   ? (uint)Input.Cs_0   : 0u;
                inputAsUint += I_Cs_1   ? (uint)Input.Cs_1   : 0u;
                inputAsUint += I_Tr_Req ? (uint)Input.Tr_Req : 0u;
                inputAsUint += I_Busy   ? (uint)Input.Busy   : 0u;
                inputAsUint += I_Compt  ? (uint)Input.Compt  : 0u;
                inputAsUint += I_Cont   ? (uint)Input.Cont   : 0u;
            }

            return inputAsUint.ToString("X4");
        }

        /// <inheritdoc cref="GetSignalOutput"/>
        public override string GetSignalOutput()
        {
            var outputAsUint = 0u;

            if (_baseIoModuleIdIds == IoModuleNo)
            {
                outputAsUint += O_L_Req != null && (bool)O_L_Req ? (uint)Output.L_Req : 0u;
                outputAsUint += O_U_Req != null && (bool)O_U_Req ? (uint)Output.U_Req : 0u;
                outputAsUint += O_Ready != null && (bool)O_Ready ? (uint)Output.Ready : 0u;
            }
            else
            {
                outputAsUint += O_Ho_Avbl != null && (bool)O_Ho_Avbl ? (uint)Output.Ho_Avbl : 0u;
                outputAsUint += O_Es != null && (bool)O_Es ? (uint)Output.Es : 0u;
            }

            return outputAsUint.ToString("X4");
        }

        /// <inheritdoc cref="GetOutputMask"/>
        public override string GetOutputMask()
        {
            var maskAsUint = 0u;

            if (_baseIoModuleIdIds == IoModuleNo)
            {
                maskAsUint += O_L_Req != null ? (uint)Output.L_Req : 0u;
                maskAsUint += O_U_Req != null ? (uint)Output.U_Req : 0u;
                maskAsUint += O_Ready != null ? (uint)Output.Ready : 0u;
            }
            else
            {
                maskAsUint += O_Ho_Avbl != null ? (uint)Output.Ho_Avbl : 0u;
                maskAsUint += O_Es != null ? (uint)Output.Es : 0u;
            }

            return maskAsUint.Equals(0u) ? null : maskAsUint.ToString("X4");
        }

        #endregion Signal Data

        #region IClonable

        public override object Clone()
        {
            return new E84SignalData(this);
        }

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        private void Set(E84SignalData other = null)
        {
            lock (this)
            {
                if (other == null)
                {
                    I_Valid  = false;
                    I_Cs_0   = false;
                    I_Cs_1   = false;
                    I_Tr_Req = false;
                    I_Busy   = false;
                    I_Compt  = false;
                    I_Cont   = false;

                    O_L_Req   = null;
                    O_U_Req   = null;
                    O_Ready   = null;
                    O_Ho_Avbl = null;
                    O_Es      = null;
                }
                else
                {
                    I_Valid  = other.I_Valid;
                    I_Cs_0   = other.I_Cs_0;
                    I_Cs_1   = other.I_Cs_1;
                    I_Tr_Req = other.I_Tr_Req;
                    I_Busy   = other.I_Busy;
                    I_Compt  = other.I_Compt;
                    I_Cont   = other.I_Cont;

                    O_L_Req   = other.O_L_Req;
                    O_U_Req   = other.O_U_Req;
                    O_Ready   = other.O_Ready;
                    O_Ho_Avbl = other.O_Ho_Avbl;
                    O_Es      = other.O_Es;
                }
            }
        }

        #endregion IClonable
    }
}
