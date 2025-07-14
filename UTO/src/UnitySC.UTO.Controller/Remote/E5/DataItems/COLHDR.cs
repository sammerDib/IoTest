using Agileo.Semi.Communication.Abstractions.E5;

namespace UnitySC.UTO.Controller.Remote.E5.DataItems
{
    /// <summary>Text description of contents of TBLELT. 1-20 characters.</summary>
    public class COLHDR : ComparableDataItemProxy<COLHDR>
    {
        #region Format 20

        /// <summary>
        /// Implicit conversion of an <see cref="COLHDR" /> to a <see cref="string" />.
        /// </summary>
        /// <param name="colHdr">The COLHDR to convert to string.</param>
        /// <returns>The <see cref="string" /> value of the COLHDR.</returns>
        public static implicit operator string(COLHDR colHdr)
        {
            return colHdr?.ValueTo<string>() ?? string.Empty;
        }

        /// <summary>
        /// Implicit conversion of a <see cref="string" /> to an <see cref="COLHDR" />.
        /// </summary>
        /// <param name="value">The string to convert to an COLHDR.</param>
        /// <returns>The <see cref="COLHDR" /> with the input value.</returns>
        public static implicit operator COLHDR(string value)
        {
            return DataItemFactory.NewDataItem<COLHDR>(value);
        }

        #endregion

        /// <summary>Empty <see cref="COLHDR" /> instance.</summary>
        public static COLHDR Empty => DataItemFactory.NewDataItem<COLHDR>();
    }
}
