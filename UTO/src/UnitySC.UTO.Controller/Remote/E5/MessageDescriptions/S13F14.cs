using System.Collections.Generic;
using System.Linq;

using Agileo.Recipes.Annotations;
using Agileo.Semi.Communication.Abstractions.E173.Model;
using Agileo.Semi.Communication.Abstractions.E5;

using UnitySC.UTO.Controller.Remote.E5.DataItems;

namespace UnitySC.UTO.Controller.Remote.E5.MessageDescriptions
{
    /// <summary>S13,F14 Table Data Acknowledge (TDA)</summary>
    /// <remarks>
    /// This message is used to acknowledge the receipt of a table and to indicate any errors.
    /// </remarks>
    public class S13F14 : Message
    {
        /// <summary>Initializes a new instance of <see cref="S13F14" /> message.</summary>
        [PublicAPI]
        protected S13F14()
            : base(13, 14)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="S13F14" /> message from a <see cref="SECSMessage" />
        /// model.
        /// </summary>
        /// <param name="model">The SECSMessage model.</param>
        /// <remarks>
        /// This constructor is required by extension method <see cref="MessageExtensions.As{T}" />.
        /// </remarks>
        [UsedImplicitly]
        protected S13F14(SECSMessage model)
            : base(13, 14, model)
        {
        }

        #region Properties

        /// <summary>Gets the table ack</summary>
        public TBLACK TableAck => Body.First().ElementAt<TBLACK>(0);

        /// <summary>Gets the list of error information.</summary>
        public IEnumerable<ERROR> Errors
            => Body.First().ElementAt(1).RecursiveWhere<ERROR>().DataItems.OfType<ERROR>();

        #endregion

        #region Build methods

        /// <summary>
        /// Initializes a new instance of <see cref="S13F14" /> message with success.
        /// </summary>
        /// <returns>A new <see cref="S13F14" /> message.</returns>
        public static S13F14 Success()
        {
            return New(TBLACK.Success);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="S13F14" /> message with failure.
        /// </summary>
        /// <returns>A new <see cref="S13F14" /> message.</returns>
        public static S13F14 Failure()
        {
            return New(TBLACK.Failure);
        }

        /// <summary>Initializes a new instance of <see cref="S13F14" /> message.</summary>
        /// <param name="tableAck">The table ack.</param>
        /// <returns>A new <see cref="S13F14" /> message.</returns>
        public static S13F14 New(TBLACK tableAck)
        {
            var message = new S13F14();
            message.Body.AddList(
                it =>
                {
                    it.AddItem(tableAck);
                    it.AddList("ERRORS");
                });

            return message;
        }

        /// <summary>Adds errors to the message</summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorText">The error text.</param>
        /// <returns>This instance of <see cref="S13F14" /> message.</returns>
        public S13F14 AddError(ERRCODE errorCode, ERRTEXT errorText)
        {
            var errors = (DataList)Body.First().ElementAt(1);
            errors.AddList(
                nameof(ERROR),
                er =>
                {
                    er.AddItem(errorCode);
                    er.AddItem(errorText);
                });
            return this;
        }

        #endregion
    }
}
