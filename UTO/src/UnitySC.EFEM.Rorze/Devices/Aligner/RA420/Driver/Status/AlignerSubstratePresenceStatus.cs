using System;
using System.Linq;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Status
{
    public class AlignerSubstratePresenceStatus : Equipment.Abstractions.Drivers.Common.Status
    {
        public AlignerSubstratePresenceStatus(AlignerSubstratePresenceStatus other)
        {
            Set(other);
        }

        public AlignerSubstratePresenceStatus(string messageStatusData)
        {
            var status = messageStatusData.Replace(":", string.Empty).Split('/');

            if (status.Length == 0)
            {
                throw new InvalidOperationException(
                    $"\"{status}\" is not a valid aligner substrate presence status.");
            }

            var noPresence = status[0].All(c => c == '0');
            var presence   = status[0].All(c => c == '9');

            if (noPresence != !presence)
                throw new InvalidOperationException(
                    $"\"{status}\" is not a valid aligner substrate presence status.");

            IsSubstratePresent = presence;
        }

        public bool IsSubstratePresent { get; internal set; }

        /// <summary>
        /// Create an object that it is a DEEP copy of current instance
        /// <returns>new object instance that it is a DEEP copy</returns>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new AlignerSubstratePresenceStatus(this);
        }

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        private void Set(AlignerSubstratePresenceStatus other)
        {
            lock (this)
            {
                IsSubstratePresent = other != null && other.IsSubstratePresent;
            }
        }
    }
}
