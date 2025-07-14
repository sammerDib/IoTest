using Agileo.Semi.Communication.Abstractions.E5;

namespace UnitySC.UTO.Controller.Remote.E5.DataItems
{
    /// <summary>Table identifier. Text conforming to the requirements of OBJSPEC.</summary>
    public class TBLID : ComparableDataItemProxy<TBLID>
    {
        #region Format 20

        /// <summary>
        /// Implicit conversion of an <see cref="TBLID" /> to a <see cref="string" />.
        /// </summary>
        /// <param name="tblId">The TBLID to convert to string.</param>
        /// <returns>The <see cref="string" /> value of the TBLID.</returns>
        public static implicit operator string(TBLID tblId)
        {
            return tblId?.ValueTo<string>() ?? string.Empty;
        }

        /// <summary>
        /// Implicit conversion of a <see cref="string" /> to an <see cref="TBLID" />.
        /// </summary>
        /// <param name="value">The string to convert to an TBLID.</param>
        /// <returns>The <see cref="TBLID" /> with the input value.</returns>
        public static implicit operator TBLID(string value)
        {
            return DataItemFactory.NewDataItem<TBLID>(value);
        }

        #endregion

        public static TBLID Empty => DataItemFactory.NewDataItem<TBLID>();
    }
}
