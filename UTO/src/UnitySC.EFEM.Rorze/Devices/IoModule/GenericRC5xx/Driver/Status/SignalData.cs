namespace UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status
{
    /// <summary>
    /// Aim to provide parse and build features to read and write signal data in Rorze EFEM RC5xx IO cards.
    /// </summary>
    public abstract class SignalData : Equipment.Abstractions.Drivers.Common.Status
    {
        protected SignalData(uint nbInputBits, uint nbOutputBits)
        {
            NbInputBits  = nbInputBits;
            NbOutputBits = nbOutputBits;
        }

        public uint NbInputBits { get; }

        public uint NbOutputBits { get; }

        public uint IoModuleNo { get; protected set; }

        /// <summary>
        /// Concatenate all <see cref="SignalData"/> properties into a single signal recognized by RORZE hardware.
        /// </summary>
        public abstract string GetSignal();

        /// <summary>
        /// Concatenate all <see cref="SignalData"/> input properties into a single signal recognized by RORZE hardware.
        /// </summary>
        public abstract string GetSignalInput();

        /// <summary>
        /// Concatenate all <see cref="SignalData"/> output properties into a single signal recognized by RORZE hardware.
        /// </summary>
        public abstract string GetSignalOutput();

        /// <summary>
        /// Indicates which output properties have null values and format them as a mask.
        /// </summary>
        public abstract string GetOutputMask();
    }
}
