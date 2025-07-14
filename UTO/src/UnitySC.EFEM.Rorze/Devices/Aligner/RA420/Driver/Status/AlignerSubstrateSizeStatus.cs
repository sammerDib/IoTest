using System;
using System.Globalization;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Status
{
    public class AlignerSubstrateSizeStatus : Equipment.Abstractions.Drivers.Common.Status
    {
        public AlignerSubstrateSizeStatus(AlignerSubstrateSizeStatus other)
        {
            Set(other);
        }

        public AlignerSubstrateSizeStatus(string messageStatusData)
        {
            var status = messageStatusData.Replace(":", string.Empty).Split('/');

            if (status.Length == 0)
            {
                throw new InvalidOperationException(
                    $"\"{status}\" is not a valid aligner substrate size status.");
            }

            if (uint.TryParse(
                    status[0],
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var substrateSize))
            {
                SubstrateSize = substrateSize;
            }
            else
            {
                throw new InvalidOperationException(
                    $"\"{status}\" is not a valid aligner substrate size status.");
            }
        }

        public uint SubstrateSize { get; internal set; }

        /// <summary>
        /// Create an object that it is a DEEP copy of current instance
        /// <returns>new object instance that it is a DEEP copy</returns>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new AlignerSubstrateSizeStatus(this);
        }

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        private void Set(AlignerSubstrateSizeStatus other)
        {
            lock (this)
            {
                SubstrateSize = other.SubstrateSize;
            }
        }
    }
}
