using System.Collections.Generic;
using System.Linq;

using Agileo.Semi.Communication.Abstractions.E5;

namespace UnitySC.UTO.Controller.Remote.E5.DataItems
{
    /// <summary>Indicates success or failure.</summary>
    public class RSPACK : ComparableDataItemProxy<RSPACK>
    {
        public static RSPACK AcknowledgeSpoolingSetupAccepted
        {
            get
            {
                var item = DataItemFactory.NewDataItem<RSPACK>((byte)0);
                item.Description = "Acknowledge, Spooling Setup Accepted";
                return item;
            }
        }

        public static RSPACK SpoolingSetupRejected
        {
            get
            {
                var item = DataItemFactory.NewDataItem<RSPACK>((byte)1);
                item.Description = "Spooling Setup Rejected";
                return item;
            }
        }

        public static IEnumerable<RSPACK> Values
        {
            get
            {
                yield return AcknowledgeSpoolingSetupAccepted;
                yield return SpoolingSetupRejected;
            }
        }

        #region Format 10

        /// <summary>Gets an <see cref="RSPACK" /> from byte value.</summary>
        /// <param name="value">The byte value.</param>
        /// <returns>The corresponding <see cref="RSPACK" />.</returns>
        public static RSPACK FromByte(byte value)
        {
            return Values.FirstOrDefault(_ => _.ValueTo<byte>() == value);
        }

        /// <summary>
        /// Implicit conversion of a <see cref="byte" /> to a <see cref="RSPACK" />.
        /// </summary>
        /// <param name="value">The byte to convert to a RSPACK.</param>
        /// <returns>The <see cref="RSPACK" /> with the input value.</returns>
        public static implicit operator RSPACK(byte value)
        {
            return FromByte(value);
        }

        /// <summary>Gets a <see cref="byte" /> from an <see cref="RSPACK" />.</summary>
        /// <param name="rspAck">The RSPACK to convert to byte.</param>
        /// <returns>The <see cref="byte" /> value of the RSPACK.</returns>
        public static byte ToByte(RSPACK rspAck)
        {
            return rspAck?.ValueTo<byte>() ?? default(byte);
        }

        /// <summary>
        /// Implicit conversion of a <see cref="RSPACK" /> to a <see cref="byte" />.
        /// </summary>
        /// <param name="rspAck">The RSPACK to convert to byte.</param>
        /// <returns>The <see cref="byte" /> value of the RSPACK.</returns>
        public static implicit operator byte(RSPACK rspAck)
        {
            return ToByte(rspAck);
        }

        #endregion
    }
}
