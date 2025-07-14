using System.Collections.Generic;

namespace UnitySC.Equipment.Abstractions.Vendor.Communication
{
    /// <summary>
    /// Contains information about the error that occurred while driving the device.
    /// </summary>
    public class ErrorOccurredEventArgs
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ErrorOccurredEventArgs"/>.
        /// </summary>
        /// <param name="error">The error that occurred while driving the device.</param>
        /// <param name="cmd">
        /// Name of the command that generated the error.
        /// By default, an empty string is used indicating that the error is self-initiated by the device.
        /// </param>
        /// <param name="sourceCategory">
        /// Indicates from which kind of device the error came.
        /// </param>
        /// <param name="sourcePort">
        /// Indicates the precise port of the source of error (in case there is several devices of same type but on different port).
        /// </param>
        public ErrorOccurredEventArgs(Error error, string cmd = "", string sourceCategory = "", byte sourcePort = 0)
        {
            Error = error;
            CommandInError = cmd;
            SourceCategory = sourceCategory;
            SourcePort = sourcePort;
        }

        /// <summary>
        /// The error object contains details about the error.
        /// </summary>
        public Error Error { get; }

        /// <summary>
        /// Indicates which command generated the error.
        /// </summary>
        public string CommandInError { get; }

        /// <summary>
        /// Indicates from which kind of device the error came.
        /// </summary>
        public string SourceCategory { get; }

        /// <summary>
        /// Indicates the precise port of the source of error (in case there is several devices of same type but on different port).
        /// </summary>
        public byte SourcePort { get; }
    }

    /// <summary>
    /// Contains information about an error that can occured during communication with a device.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// Ensure that no property is null.
        /// </summary>
        public Error()
        {
            Code = 0;
            Description = string.Empty;
            Cause = string.Empty;
            Sources = new List<string>();
        }

        /// <summary>
        /// Code that identifies the error.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Brief description to clarify the error code.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Possible cause for the error.
        /// </summary>
        public string Cause { get; set; }

        /// <summary>
        /// List of devices that could generate this error (Robot, Aligner, LightTower, ...).
        /// </summary>
        public List<string> Sources { get; set; }
    }
}
