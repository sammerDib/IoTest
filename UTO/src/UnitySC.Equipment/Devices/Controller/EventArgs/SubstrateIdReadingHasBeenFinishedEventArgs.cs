using System;
using System.Text;

namespace UnitySC.Equipment.Devices.Controller.EventArgs
{
    public class SubstrateIdReadingHasBeenFinishedEventArgs : System.EventArgs
    {
        /// <summary>
        /// reading status
        /// </summary>
        public bool IsSuccess{ get; }

        /// <summary>
        /// Id of the substrate
        /// </summary>
        public string SubstrateId { get; }

        /// <summary>
        /// Id acquired by the reader
        /// </summary>
        public string AcquiredId { get; }

        public SubstrateIdReadingHasBeenFinishedEventArgs(bool isSuccess, string substrateId, string acquiredId)
        {
            IsSuccess = isSuccess;
            SubstrateId = substrateId;
            AcquiredId = acquiredId;
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
            build.AppendLine(FormattableString.Invariant($"IsSuccess = {IsSuccess}"));
            build.AppendLine(FormattableString.Invariant($"SubstrateId = {SubstrateId}"));
            build.AppendLine(FormattableString.Invariant($"AcquiredId = {AcquiredId}"));
            return build.ToString();
        }
    }
}
