using System.Collections.Generic;
using System.Linq;

using Agileo.Semi.Communication.Abstractions.E5;

namespace UnitySC.UTO.Controller.Remote.E5.DataItems
{
    /// <summary>Indicates success or failure.</summary>
    public class TBLACK : ComparableDataItemProxy<TBLACK>
    {
        public static TBLACK Success
        {
            get
            {
                var item = DataItemFactory.NewDataItem<TBLACK>((byte)0);
                item.Description = "Success";
                return item;
            }
        }

        public static TBLACK Failure
        {
            get
            {
                var item = DataItemFactory.NewDataItem<TBLACK>((byte)1);
                item.Description = "Failure";
                return item;
            }
        }

        public static IEnumerable<TBLACK> Values
        {
            get
            {
                yield return Success;
                yield return Failure;
            }
        }

        #region Format 51

        /// <summary>Gets an <see cref="TBLACK" /> from byte value.</summary>
        /// <param name="value">The byte value.</param>
        /// <returns>The corresponding <see cref="TBLACK" />.</returns>
        public static TBLACK FromByte(byte value)
        {
            return Values.FirstOrDefault(_ => _.ValueTo<byte>() == value);
        }

        /// <summary>
        /// Implicit conversion of a <see cref="byte" /> to a <see cref="TBLACK" />.
        /// </summary>
        /// <param name="value">The byte to convert to a TBLACK.</param>
        /// <returns>The <see cref="TBLACK" /> with the input value.</returns>
        public static implicit operator TBLACK(byte value)
        {
            return FromByte(value);
        }

        /// <summary>Gets a <see cref="byte" /> from an <see cref="TBLACK" />.</summary>
        /// <param name="tblAck">The TBLACK to convert to byte.</param>
        /// <returns>The <see cref="byte" /> value of the TBLACK.</returns>
        public static byte ToByte(TBLACK tblAck)
        {
            return tblAck?.ValueTo<byte>() ?? default(byte);
        }

        /// <summary>
        /// Implicit conversion of a <see cref="TBLACK" /> to a <see cref="byte" />.
        /// </summary>
        /// <param name="tblAck">The TBLACK to convert to byte.</param>
        /// <returns>The <see cref="byte" /> value of the TBLACK.</returns>
        public static implicit operator byte(TBLACK tblAck)
        {
            return ToByte(tblAck);
        }

        #endregion
    }
}
