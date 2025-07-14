using System.Text;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.EventArgs
{
    /// <summary>
    /// Provides data about carrierId
    /// </summary>
    public class CarrierIdReceivedEventArgs : LoadPortEventArgs
    {
        /// <summary>
        /// Defines a CarrierIDReceivedArgs
        /// </summary>
        /// <param name="loadPortNumber">Is a target port number</param>
        /// <param name="carrierId">Carrier ID (16 byte) which was read. Can be omitted</param>
        /// <param name="isSucceed">Is Reading succeed. Can be omitted</param>
        public CarrierIdReceivedEventArgs(byte loadPortNumber, string carrierId = "", bool isSucceed = true)
            : base(loadPortNumber)
        {
            CarrierId = carrierId;
            IsSucceed = isSucceed;
        }

        /// <summary>
        /// Carrier Id which was read.
        /// </summary>
        public string CarrierId { get; set; }

        /// <summary>
        /// Is Reading succeed
        /// </summary>
        public bool IsSucceed { get; set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var build = new StringBuilder();

            build.AppendLine(base.ToString());
            build.AppendLine($"CarrierId: {CarrierId}");
            build.AppendLine($"IsSucceed: {IsSucceed}");

            return build.ToString();
        }
    }
}
