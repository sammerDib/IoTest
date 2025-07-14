using System.Collections.Generic;
using System.Linq;

using Agileo.Semi.Communication.Abstractions.E5;

namespace UnitySC.UTO.Controller.Remote.E5.DataItems
{
    /// <summary>
    /// Provides information about the table or parts of the table being transferred or requested.
    /// Enumerated.
    /// </summary>
    public class TBLCMD : ComparableDataItemProxy<TBLCMD>
    {
        /// <summary>Complete Table</summary>
        public static TBLCMD CompleteTable
        {
            get
            {
                var item = DataItemFactory.NewDataItem<TBLCMD>((byte)0);
                item.Description = "Complete Table";
                return item;
            }
        }

        /// <summary>New rows (add)</summary>
        public static TBLCMD NewRowsAdd
        {
            get
            {
                var item = DataItemFactory.NewDataItem<TBLCMD>((byte)1);
                item.Description = "New rows (add)";
                return item;
            }
        }

        /// <summary>New columns (append)</summary>
        public static TBLCMD NewColumnsAppend
        {
            get
            {
                var item = DataItemFactory.NewDataItem<TBLCMD>((byte)2);
                item.Description = "New columns (append)";
                return item;
            }
        }

        /// <summary>Replace existing rows</summary>
        public static TBLCMD ReplaceExistingRows
        {
            get
            {
                var item = DataItemFactory.NewDataItem<TBLCMD>((byte)3);
                item.Description = "Replace existing rows";
                return item;
            }
        }

        /// <summary>Replace existing columns</summary>
        public static TBLCMD ReplaceExistingColumns
        {
            get
            {
                var item = DataItemFactory.NewDataItem<TBLCMD>((byte)4);
                item.Description = "Replace existing columns";
                return item;
            }
        }

        public static IEnumerable<TBLCMD> Values
        {
            get
            {
                yield return CompleteTable;
                yield return NewRowsAdd;
                yield return NewColumnsAppend;
                yield return ReplaceExistingRows;
                yield return ReplaceExistingColumns;
            }
        }

        #region Format 51

        /// <summary>Gets an <see cref="TBLCMD" /> from byte value.</summary>
        /// <param name="value">The byte value.</param>
        /// <returns>The corresponding <see cref="TBLCMD" />.</returns>
        public static TBLCMD FromByte(byte value)
        {
            return Values.FirstOrDefault(_ => _.ValueTo<byte>() == value);
        }

        /// <summary>
        /// Implicit conversion of a <see cref="byte" /> to a <see cref="TBLCMD" />.
        /// </summary>
        /// <param name="value">The byte to convert to a TBLCMD.</param>
        /// <returns>The <see cref="TBLCMD" /> with the input value.</returns>
        public static implicit operator TBLCMD(byte value)
        {
            return FromByte(value);
        }

        /// <summary>Gets a <see cref="byte" /> from an <see cref="TBLCMD" />.</summary>
        /// <param name="tblCmd">The TBLCMD to convert to byte.</param>
        /// <returns>The <see cref="byte" /> value of the TBLCMD.</returns>
        public static byte ToByte(TBLCMD tblCmd)
        {
            return tblCmd?.ValueTo<byte>() ?? default(byte);
        }

        /// <summary>
        /// Implicit conversion of a <see cref="TBLCMD" /> to a <see cref="byte" />.
        /// </summary>
        /// <param name="tblCmd">The TBLCMD to convert to byte.</param>
        /// <returns>The <see cref="byte" /> value of the TBLCMD.</returns>
        public static implicit operator byte(TBLCMD tblCmd)
        {
            return ToByte(tblCmd);
        }

        #endregion
    }
}
