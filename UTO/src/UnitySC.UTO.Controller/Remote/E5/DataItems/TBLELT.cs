using System;
using System.Linq;

using Agileo.Semi.Communication.Abstractions.E5;

namespace UnitySC.UTO.Controller.Remote.E5.DataItems
{
    /// <summary>
    /// Table element. The first table element in a row is used to identify the row.
    /// </summary>
    public class TBLELT : ComparableDataItemProxy<TBLELT>
    {
        #region Format 00

        /// <summary>
        /// Implicit conversion of an <see cref="TBLELT" /> to a <see cref="DataList" />.
        /// </summary>
        /// <param name="tblElt">The TBLELT to convert to DataList.</param>
        /// <returns>The <see cref="DataList" /> value of the TBLELT.</returns>
        public static implicit operator DataList(TBLELT tblElt)
        {
            return tblElt.AdaptTo<DataList>();
        }

        /// <summary>
        /// Implicit conversion of a <see cref="DataList" /> to an <see cref="TBLELT" />.
        /// </summary>
        /// <param name="value">The DataList to convert to an TBLELT.</param>
        /// <returns>The <see cref="TBLELT" /> with the input value.</returns>
        public static implicit operator TBLELT(DataList value)
        {
            var tblElt = DataItemFactory.NewDataItem<TBLELT>(value);

            //tblElt.Item = value;
            //tblElt.DataItemName = nameof(TBLELT);
            return tblElt;
        }

        /// <summary>
        /// Implicit conversion of an <see cref="TBLELT" /> to a <see cref="DataArray" />.
        /// </summary>
        /// <param name="tblElt">The TBLELT to convert to DataArray.</param>
        /// <returns>The <see cref="DataArray" /> value of the TBLELT.</returns>
        public static implicit operator DataArray(TBLELT tblElt)
        {
            return tblElt.AdaptTo<DataArray>();
        }

        /// <summary>
        /// Implicit conversion of a <see cref="DataArray" /> to an <see cref="TBLELT" />.
        /// </summary>
        /// <param name="value">The DataArray to convert to an TBLELT.</param>
        /// <returns>The <see cref="TBLELT" /> with the input value.</returns>
        public static implicit operator TBLELT(DataArray value)
        {
            var tblElt = DataItemFactory.NewDataItem<TBLELT>(value);

            //tblElt.Item = value;
            //tblElt.DataItemName = nameof(TBLELT);
            return tblElt;
        }

        /// <summary>
        /// Implicit conversion of an <see cref="TBLELT" /> to a <see cref="DataValue" />.
        /// </summary>
        /// <param name="tblElt">The TBLELT to convert to DataValue.</param>
        /// <returns>The <see cref="DataValue" /> value of the TBLELT.</returns>
        public static implicit operator DataValue(TBLELT tblElt)
        {
            return tblElt.AdaptTo<DataArray>()?.DataValues.FirstOrDefault();
        }

        /// <summary>
        /// Implicit conversion of a <see cref="DataValue" /> to an <see cref="TBLELT" />.
        /// </summary>
        /// <param name="value">The DataValue to convert to an TBLELT.</param>
        /// <returns>The <see cref="TBLELT" /> with the input value.</returns>
        public static implicit operator TBLELT(DataValue value)
        {
            var tblElt = DataItemFactory.NewDataItem<TBLELT>(value);
            return tblElt;
        }

        #endregion

        #region Format 11

        /// <summary>
        /// Implicit conversion of an <see cref="TBLELT" /> to a <see cref="bool" />.
        /// </summary>
        /// <param name="tblElt">The TBLELT to convert to bool.</param>
        /// <returns>The <see cref="bool" /> value of the TBLELT.</returns>
        public static implicit operator bool(TBLELT tblElt)
        {
            return tblElt?.ValueTo<bool>() ?? default(bool);
        }

        /// <summary>
        /// Implicit conversion of a <see cref="bool" /> to an <see cref="TBLELT" />.
        /// </summary>
        /// <param name="value">The bool to convert to an TBLELT.</param>
        /// <returns>The <see cref="TBLELT" /> with the input value.</returns>
        public static implicit operator TBLELT(bool value)
        {
            return DataItemFactory.NewDataItem<TBLELT>(value);
        }

        #endregion

        #region Format 20

        /// <summary>
        /// Implicit conversion of an <see cref="TBLELT" /> to a <see cref="string" />.
        /// </summary>
        /// <param name="tblElt">The TBLELT to convert to string.</param>
        /// <returns>The <see cref="string" /> value of the TBLELT.</returns>
        public static implicit operator string(TBLELT tblElt)
        {
            return tblElt?.ValueTo<string>() ?? string.Empty;
        }

        /// <summary>
        /// Implicit conversion of a <see cref="string" /> to an <see cref="TBLELT" />.
        /// </summary>
        /// <param name="value">The string to convert to an TBLELT.</param>
        /// <returns>The <see cref="TBLELT" /> with the input value.</returns>
        public static implicit operator TBLELT(string value)
        {
            return DataItemFactory.NewDataItem<TBLELT>(value);
        }

        #endregion

        #region Format 3()

        /// <summary>
        /// Implicit conversion of an <see cref="TBLELT" /> to a <see cref="sbyte" />.
        /// </summary>
        /// <param name="tblElt">The TBLELT to convert to sbyte.</param>
        /// <returns>The <see cref="sbyte" /> value of the TBLELT.</returns>
        public static implicit operator sbyte(TBLELT tblElt)
        {
            return tblElt?.ValueTo<sbyte>() ?? default(sbyte);
        }

        /// <summary>
        /// Implicit conversion of a <see cref="sbyte" /> to an <see cref="TBLELT" />.
        /// </summary>
        /// <param name="value">The sbyte to convert to an TBLELT.</param>
        /// <returns>The <see cref="TBLELT" /> with the input value.</returns>
        public static implicit operator TBLELT(sbyte value)
        {
            return DataItemFactory.NewDataItem<TBLELT>(value);
        }

        /// <summary>
        /// Implicit conversion of an <see cref="TBLELT" /> to a <see cref="short" />.
        /// </summary>
        /// <param name="tblElt">The TBLELT to convert to short.</param>
        /// <returns>The <see cref="short" /> value of the TBLELT.</returns>
        public static implicit operator short(TBLELT tblElt)
        {
            return tblElt?.ValueTo<short>() ?? default(short);
        }

        /// <summary>
        /// Implicit conversion of an <see cref="short" /> to an <see cref="TBLELT" />.
        /// </summary>
        /// <param name="value">The short to convert to an TBLELT.</param>
        /// <returns>The <see cref="TBLELT" /> with the input value.</returns>
        public static implicit operator TBLELT(short value)
        {
            return DataItemFactory.NewDataItem<TBLELT>(value);
        }

        /// <summary>
        /// Implicit conversion of an <see cref="TBLELT" /> to a <see cref="int" />.
        /// </summary>
        /// <param name="tblElt">The TBLELT to convert to int.</param>
        /// <returns>The <see cref="int" /> value of the TBLELT.</returns>
        public static implicit operator int(TBLELT tblElt)
        {
            return tblElt?.ValueTo<int>() ?? default(int);
        }

        /// <summary>
        /// Implicit conversion of an <see cref="int" /> to an <see cref="TBLELT" />.
        /// </summary>
        /// <param name="value">The int to convert to an TBLELT.</param>
        /// <returns>The <see cref="TBLELT" /> with the input value.</returns>
        public static implicit operator TBLELT(int value)
        {
            return DataItemFactory.NewDataItem<TBLELT>(value);
        }

        /// <summary>
        /// Implicit conversion of an <see cref="TBLELT" /> to a <see cref="long" />.
        /// </summary>
        /// <param name="tblElt">The TBLELT to convert to long.</param>
        /// <returns>The <see cref="long" /> value of the TBLELT.</returns>
        public static implicit operator long(TBLELT tblElt)
        {
            return tblElt?.ValueTo<long>() ?? default(long);
        }

        /// <summary>
        /// Implicit conversion of an <see cref="long" /> to an <see cref="TBLELT" />.
        /// </summary>
        /// <param name="value">The long to convert to an TBLELT.</param>
        /// <returns>The <see cref="TBLELT" /> with the input value.</returns>
        public static implicit operator TBLELT(long value)
        {
            return DataItemFactory.NewDataItem<TBLELT>(value);
        }

        #endregion

        #region Format 4()

        /// <summary>
        /// Implicit conversion of an <see cref="TBLELT" /> to a <see cref="float" />.
        /// </summary>
        /// <param name="tblElt">The TBLELT to convert to float.</param>
        /// <returns>The <see cref="float" /> value of the TBLELT.</returns>
        public static implicit operator float(TBLELT tblElt)
        {
            return tblElt?.ValueTo<float>() ?? default(float);
        }

        /// <summary>
        /// Implicit conversion of a <see cref="float" /> to an <see cref="TBLELT" />.
        /// </summary>
        /// <param name="value">The float to convert to an TBLELT.</param>
        /// <returns>The <see cref="TBLELT" /> with the input value.</returns>
        public static implicit operator TBLELT(float value)
        {
            return DataItemFactory.NewDataItem<TBLELT>(value);
        }

        /// <summary>
        /// Implicit conversion of an <see cref="TBLELT" /> to a <see cref="double" />.
        /// </summary>
        /// <param name="tblElt">The TBLELT to convert to double.</param>
        /// <returns>The <see cref="double" /> value of the TBLELT.</returns>
        public static implicit operator double(TBLELT tblElt)
        {
            return tblElt?.ValueTo<double>() ?? default(double);
        }

        /// <summary>
        /// Implicit conversion of an <see cref="double" /> to an <see cref="TBLELT" />.
        /// </summary>
        /// <param name="value">The double to convert to an TBLELT.</param>
        /// <returns>The <see cref="TBLELT" /> with the input value.</returns>
        public static implicit operator TBLELT(double value)
        {
            return DataItemFactory.NewDataItem<TBLELT>(value);
        }

        #endregion

        #region Format 5()

        /// <summary>
        /// Implicit conversion of an <see cref="TBLELT" /> to a <see cref="byte" />.
        /// </summary>
        /// <param name="tblElt">The TBLELT to convert to byte.</param>
        /// <returns>The <see cref="byte" /> value of the TBLELT.</returns>
        public static implicit operator byte(TBLELT tblElt)
        {
            return tblElt?.ValueTo<byte>() ?? default(byte);
        }

        /// <summary>
        /// Implicit conversion of a <see cref="byte" /> to an <see cref="TBLELT" />.
        /// </summary>
        /// <param name="value">The byte to convert to an TBLELT.</param>
        /// <returns>The <see cref="TBLELT" /> with the input value.</returns>
        public static implicit operator TBLELT(byte value)
        {
            return DataItemFactory.NewDataItem<TBLELT>(value);
        }

        /// <summary>
        /// Implicit conversion of an <see cref="TBLELT" /> to a <see cref="ushort" />.
        /// </summary>
        /// <param name="tblElt">The TBLELT to convert to ushort.</param>
        /// <returns>The <see cref="ushort" /> value of the TBLELT.</returns>
        public static implicit operator ushort(TBLELT tblElt)
        {
            return tblElt?.ValueTo<ushort>() ?? default(ushort);
        }

        /// <summary>
        /// Implicit conversion of an <see cref="ushort" /> to an <see cref="TBLELT" />.
        /// </summary>
        /// <param name="value">The ushort to convert to an TBLELT.</param>
        /// <returns>The <see cref="TBLELT" /> with the input value.</returns>
        public static implicit operator TBLELT(ushort value)
        {
            return DataItemFactory.NewDataItem<TBLELT>(value);
        }

        /// <summary>
        /// Implicit conversion of an <see cref="TBLELT" /> to a <see cref="uint" />.
        /// </summary>
        /// <param name="tblElt">The TBLELT to convert to uint.</param>
        /// <returns>The <see cref="uint" /> value of the TBLELT.</returns>
        public static implicit operator uint(TBLELT tblElt)
        {
            return tblElt?.ValueTo<uint>() ?? default(uint);
        }

        /// <summary>
        /// Implicit conversion of an <see cref="uint" /> to an <see cref="TBLELT" />.
        /// </summary>
        /// <param name="value">The uint to convert to an TBLELT.</param>
        /// <returns>The <see cref="TBLELT" /> with the input value.</returns>
        public static implicit operator TBLELT(uint value)
        {
            return DataItemFactory.NewDataItem<TBLELT>(value);
        }

        /// <summary>
        /// Implicit conversion of an <see cref="TBLELT" /> to a <see cref="ulong" />.
        /// </summary>
        /// <param name="tblElt">The TBLELT to convert to ulong.</param>
        /// <returns>The <see cref="ulong" /> value of the TBLELT.</returns>
        public static implicit operator ulong(TBLELT tblElt)
        {
            return tblElt?.ValueTo<ulong>() ?? default(ulong);
        }

        /// <summary>
        /// Implicit conversion of an <see cref="ulong" /> to an <see cref="TBLELT" />.
        /// </summary>
        /// <param name="value">The ulong to convert to an TBLELT.</param>
        /// <returns>The <see cref="TBLELT" /> with the input value.</returns>
        public static implicit operator TBLELT(ulong value)
        {
            return DataItemFactory.NewDataItem<TBLELT>(value);
        }

        #endregion

        /// <summary>
        /// Converts a generic <see cref="DataItem" /> to a <see cref="TBLELT" /> data item.
        /// </summary>
        /// <param name="value">The DataItem to convert to a TBLELT.</param>
        /// <returns>The <see cref="TBLELT" /> with the DataItem value.</returns>
        public static TBLELT FromDataItem(DataItem value)
        {
            // Needs to extract DataItem encapsulated into value because value can be a DataItemProxy.
            // /!\ Avoids to have a DataItemProxy into another DataItemProxy.
            var dataItem = value.AdaptTo<DataList>() ?? (DataItem)value.AdaptTo<DataArray>();

            if (dataItem == null)
            {
                throw new ArgumentException(
                    "Unable to convert DataItem value to TBLELT. Specified value must be a DataList or DataArray or a DataItemProxy containing a DataList or DataArray",
                    nameof(value));
            }

            // Encapsulates DataItem value into TBLELT DataItemProxy.
            var valueAsTblElt = DataItemFactory.NewDataItem<TBLELT>(dataItem);

            //TODO Check if .Item and .DataItemName are well feed with line before
            //valueAsTblElt.Item = dataItem;
            //valueAsTblElt.DataItemName = nameof(TBLELT);
            return valueAsTblElt;
        }
    }
}
