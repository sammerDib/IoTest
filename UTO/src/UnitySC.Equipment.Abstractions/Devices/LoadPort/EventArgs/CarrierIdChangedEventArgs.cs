using System.Text;

using Agileo.SemiDefinitions;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort
{
    /// <summary>
    /// Contains information about Carrier ID.
    /// </summary>
    public class CarrierIdChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CarrierIdChangedEventArgs"/> class.
        /// </summary>
        /// <param name="carrierId">The carrier ID.</param>
        /// <param name="status">The status associated to command executed to get the carrier ID.</param>
        /// <param name="errorMessage">Error message explaining why read failed.</param>
        public CarrierIdChangedEventArgs(
            string carrierId,
            CommandStatusCode status,
            string errorMessage = "")
        {
            CarrierId = carrierId;
            Status = status;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets the carrier identifier.
        /// </summary>
        public string CarrierId { get; }

        /// <summary>
        /// Gets the status associated to command executed to get the carrier identifier.
        /// </summary>
        public CommandStatusCode Status { get; }

        /// <summary>
        /// Gets the error message if <see cref="Status"/> is not <see cref="CommandStatusCode.Ok"/>.
        /// </summary>
        public string ErrorMessage { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            var build = new StringBuilder();
            build.Append('[').Append(nameof(CarrierIdChangedEventArgs)).AppendLine("]");
            build.Append(nameof(CarrierId)).Append(" = ").AppendLine(CarrierId);
            build.Append(nameof(Status)).Append(" = ").Append(Status).AppendLine();
            return build.ToString();
        }
    }
}
