using System.Collections.Generic;
using System.Linq;

using Agileo.Semi.Communication.Abstractions.E5;

namespace UnitySC.UTO.Controller.Remote.E5.DataItems
{
    /// <summary>Indicates success or failure.</summary>
    public class STRACK : ComparableDataItemProxy<STRACK>
    {
        public static STRACK SpoolingNotAllowedForStream
        {
            get
            {
                var item = DataItemFactory.NewDataItem<STRACK>((byte)1);
                item.Description = "Spooling Not Allowed For Stream";
                return item;
            }
        }

        public static STRACK StreamUnknown
        {
            get
            {
                var item = DataItemFactory.NewDataItem<STRACK>((byte)2);
                item.Description = "Stream Unknown";
                return item;
            }
        }

        public static STRACK UnknownFunctionSpecifiedForThisStream
        {
            get
            {
                var item = DataItemFactory.NewDataItem<STRACK>((byte)3);
                item.Description = "Unknown Function Specified For This Stream";
                return item;
            }
        }

        public static STRACK SecondaryFunctionSpecifiedForThisStream
        {
            get
            {
                var item = DataItemFactory.NewDataItem<STRACK>((byte)4);
                item.Description = "Secondary Function Specified For This Stream";
                return item;
            }
        }

        public static IEnumerable<STRACK> Values
        {
            get
            {
                yield return SpoolingNotAllowedForStream;
                yield return StreamUnknown;
                yield return UnknownFunctionSpecifiedForThisStream;
                yield return SecondaryFunctionSpecifiedForThisStream;
            }
        }

        #region Format 10

        /// <summary>Gets an <see cref="STRACK" /> from byte value.</summary>
        /// <param name="value">The byte value.</param>
        /// <returns>The corresponding <see cref="STRACK" />.</returns>
        public static STRACK FromByte(byte value)
        {
            return Values.FirstOrDefault(_ => _.ValueTo<byte>() == value);
        }

        /// <summary>
        /// Implicit conversion of a <see cref="byte" /> to a <see cref="STRACK" />.
        /// </summary>
        /// <param name="value">The byte to convert to a STRACK.</param>
        /// <returns>The <see cref="STRACK" /> with the input value.</returns>
        public static implicit operator STRACK(byte value)
        {
            return FromByte(value);
        }

        /// <summary>Gets a <see cref="byte" /> from an <see cref="STRACK" />.</summary>
        /// <param name="strAck">The STRACK to convert to byte.</param>
        /// <returns>The <see cref="byte" /> value of the STRACK.</returns>
        public static byte ToByte(STRACK strAck)
        {
            return strAck?.ValueTo<byte>() ?? default(byte);
        }

        /// <summary>
        /// Implicit conversion of a <see cref="STRACK" /> to a <see cref="byte" />.
        /// </summary>
        /// <param name="strAck">The STRACK to convert to byte.</param>
        /// <returns>The <see cref="byte" /> value of the STRACK.</returns>
        public static implicit operator byte(STRACK strAck)
        {
            return ToByte(strAck);
        }

        #endregion
    }
}
