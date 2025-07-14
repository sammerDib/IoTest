using System;
using System.Text;

namespace UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs
{
    /// <summary>
    /// Aims to provide formatted statuses
    /// </summary>
    public class StatusEventArgs<T> : System.EventArgs where T : Status
    {
        /// <summary>
        /// Gets the source name.
        /// </summary>
        public string SourceName { get; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        public T Status { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusEventArgs{T}" /> class.
        /// </summary>
        /// <param name="sourceName">The name of source which sent the event.</param>
        /// <param name="status">The status.</param>
        public StatusEventArgs(string sourceName, T status)
        {
            SourceName = sourceName;
            Status = status;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder build = new StringBuilder();
            build.AppendLine(FormattableString.Invariant($"SourceName = {SourceName}"));
            build.AppendLine(Status.ToString());
            return build.ToString();
        }
    }
}
