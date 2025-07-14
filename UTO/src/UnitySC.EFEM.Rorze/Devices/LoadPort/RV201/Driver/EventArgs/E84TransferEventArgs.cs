using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.EventArgs;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.EventArgs
{
    public class E84TransferEventArgs : LoadPortEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="E84TransferEventArgs"/> class.
        /// </summary>
        /// <param name="loadPortNumber">The load port number.</param>
        /// <param name="transferRequest">The complete transfer request.</param>
        public E84TransferEventArgs(byte loadPortNumber, E84TransferRequest transferRequest)
            : base(loadPortNumber)
        {
            TransferRequest = transferRequest;
        }

        /// <summary>
        /// Gets the complete transfer request data.
        /// </summary>
        public E84TransferRequest TransferRequest { get; }
    }
}
