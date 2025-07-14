using Agileo.Semi.Communication.Abstractions.E5;

namespace UnitySC.UTO.Controller.Remote.E5.DataItems
{
    /// <summary>
    /// A reserved text string to denote the format and application of the table. Text conforming to the
    /// requirements of OBJSPEC.
    /// </summary>
    public class TBLTYP : ComparableDataItemProxy<TBLTYP>
    {
        #region Format 20

        /// <summary>
        /// Implicit conversion of an <see cref="TBLTYP" /> to a <see cref="string" />.
        /// </summary>
        /// <param name="tblTyp">The TBLTYP to convert to string.</param>
        /// <returns>The <see cref="string" /> value of the TBLTYP.</returns>
        public static implicit operator string(TBLTYP tblTyp)
        {
            return tblTyp?.ValueTo<string>() ?? string.Empty;
        }

        /// <summary>
        /// Implicit conversion of a <see cref="string" /> to an <see cref="TBLTYP" />.
        /// </summary>
        /// <param name="value">The string to convert to an TBLTYP.</param>
        /// <returns>The <see cref="TBLTYP" /> with the input value.</returns>
        public static implicit operator TBLTYP(string value)
        {
            return DataItemFactory.NewDataItem<TBLTYP>(value);
        }

        #endregion

        public static TBLTYP Empty => DataItemFactory.NewDataItem<TBLTYP>();
    }
}
