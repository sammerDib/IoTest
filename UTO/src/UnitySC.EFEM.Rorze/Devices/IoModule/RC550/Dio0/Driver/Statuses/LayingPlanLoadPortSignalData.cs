using System;
using System.Globalization;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Statuses
{
    public class LayingPlanLoadPortSignalData : SignalData
    {
        #region Fields

        private readonly uint _baseIoModuleIdIds;

        #endregion

        #region Enums

        [Flags]
        public enum Input : UInt16
        {
            PlacementSensorA = 1 << 0,
            PlacementSensorB = 1 << 1,
            PlacementSensorC = 1 << 2,
            PlacementSensorD = 1 << 3,
            WaferProtrudeSensor1 = 1 << 4,
            WaferProtrudeSensor2 = 1 << 5,
            WaferProtrudeSensor3 = 1 << 6
        }

        #endregion Enums

        #region Inputs

        public bool I_PlacementSensorA { get; private set; }

        public bool I_PlacementSensorB { get; private set; }

        public bool I_PlacementSensorC { get; private set; }

        public bool I_PlacementSensorD { get; private set; }

        public bool I_WaferProtrudeSensor1 { get; private set; }

        public bool I_WaferProtrudeSensor2 { get; private set; }

        public bool I_WaferProtrudeSensor3 { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Define an empty <see cref="LayingPlanLoadPortSignalData" />. Typical use would be for initializing
        /// output signal of IO card. Use it with initializer to set output signal.
        /// </summary>
        public LayingPlanLoadPortSignalData()
            : base(16, 16)
        {
        }

        /// <summary>
        /// Define an empty <see cref="LayingPlanLoadPortSignalData" />. Typical use would be for initializing
        /// output signal of IO card. Use it with initializer to set output signal.
        /// </summary>
        public LayingPlanLoadPortSignalData(uint baseIoModuleId, uint ioModuleId)
            : base(16, 16)
        {
            _baseIoModuleIdIds = baseIoModuleId;
            IoModuleNo = ioModuleId;
        }

        /// <summary>
        /// Define a <see cref="LayingPlanLoadPortSignalData" /> from a string received from equipment.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// if the <see cref="signalAsString" /> has not the expected
        /// length.
        /// </exception>
        public LayingPlanLoadPortSignalData(
            uint baseIoModuleId,
            uint ioModuleId,
            string signalAsString)
            : base(16, 16)
        {
            _baseIoModuleIdIds = baseIoModuleId;
            IoModuleNo = ioModuleId;

            // Expected signal is an hexadecimal number with a fixed length.
            if (signalAsString.Length != (NbInputBits + NbOutputBits) / 4)
            {
                throw new ArgumentException(
                    "Argument has not the expected length.\n"
                    + $"Expected: {(NbInputBits + NbOutputBits) / 4}\n"
                    + $"Obtained: {signalAsString.Length}");
            }

            var input = signalAsString.Substring(4, 4);
            var inputFlags = (Input)uint.Parse(input, NumberStyles.AllowHexSpecifier);

            if (_baseIoModuleIdIds == IoModuleNo)
            {
                I_PlacementSensorA = (inputFlags & Input.PlacementSensorA) != 0;
                I_PlacementSensorB = (inputFlags & Input.PlacementSensorB) != 0;
                I_PlacementSensorC = (inputFlags & Input.PlacementSensorC) != 0;
                I_PlacementSensorD = (inputFlags & Input.PlacementSensorD) != 0;
                I_WaferProtrudeSensor1 = (inputFlags & Input.WaferProtrudeSensor1) != 0;
                I_WaferProtrudeSensor2 = (inputFlags & Input.WaferProtrudeSensor2) != 0;
                I_WaferProtrudeSensor3 = (inputFlags & Input.WaferProtrudeSensor3) != 0;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayingPlanLoadPortSignalData" /> class.
        /// <param name="other">
        /// Create a deep copy of <see cref="LayingPlanLoadPortSignalData" /> instance
        /// </param>
        /// </summary>
        private LayingPlanLoadPortSignalData(LayingPlanLoadPortSignalData other)
            : base(other.NbInputBits, other.NbOutputBits)
        {
            Set(other);
        }

        #endregion Constructors

        #region Signal Data

        /// <inheritdoc cref="GetSignal" />
        public override string GetSignal()
        {
            return GetSignalInput() + GetSignalOutput();
        }

        /// <inheritdoc cref="GetSignalInput" />
        public override string GetSignalInput()
        {
            var inputAsUint = 0u;
            if (_baseIoModuleIdIds == IoModuleNo)
            {
                inputAsUint += I_PlacementSensorA
                    ? (uint)Input.PlacementSensorA
                    : 0u;
                inputAsUint += I_PlacementSensorB
                    ? (uint)Input.PlacementSensorB
                    : 0u;
                inputAsUint += I_PlacementSensorC
                    ? (uint)Input.PlacementSensorC
                    : 0u;
                inputAsUint += I_PlacementSensorD
                    ? (uint)Input.PlacementSensorD
                    : 0u;
                inputAsUint += I_WaferProtrudeSensor1
                    ? (uint)Input.WaferProtrudeSensor1
                    : 0u;
                inputAsUint += I_WaferProtrudeSensor2
                    ? (uint)Input.WaferProtrudeSensor2
                    : 0u;
                inputAsUint += I_WaferProtrudeSensor3
                    ? (uint)Input.WaferProtrudeSensor3
                    : 0u;
            }

            return inputAsUint.ToString("X4");
        }

        /// <inheritdoc cref="GetSignalOutput" />
        public override string GetSignalOutput()
        {
            var outputAsUint = 0u;

            return outputAsUint.ToString("X4");
        }

        /// <inheritdoc cref="GetOutputMask" />
        public override string GetOutputMask()
        {
            var maskAsUint = 0u;

            return maskAsUint.Equals(0u)
                ? null
                : maskAsUint.ToString("X4");
        }

        #endregion Signal Data

        #region IClonable

        public override object Clone()
        {
            return new LayingPlanLoadPortSignalData(this);
        }

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        private void Set(LayingPlanLoadPortSignalData other = null)
        {
            lock (this)
            {
                if (other == null)
                {
                    I_PlacementSensorA = false;
                    I_PlacementSensorB = false;
                    I_PlacementSensorC = false;
                    I_PlacementSensorD = false;
                    I_WaferProtrudeSensor1 = false;
                    I_WaferProtrudeSensor2 = false;
                    I_WaferProtrudeSensor3 = false;
                }
                else
                {
                    I_PlacementSensorA = other.I_PlacementSensorA;
                    I_PlacementSensorB = other.I_PlacementSensorB;
                    I_PlacementSensorC = other.I_PlacementSensorC;
                    I_PlacementSensorD = other.I_PlacementSensorD;
                    I_WaferProtrudeSensor1 = other.I_WaferProtrudeSensor1;
                    I_WaferProtrudeSensor2 = other.I_WaferProtrudeSensor2;
                    I_WaferProtrudeSensor3 = other.I_WaferProtrudeSensor3;
                }
            }
        }

        #endregion IClonable
    }
}
