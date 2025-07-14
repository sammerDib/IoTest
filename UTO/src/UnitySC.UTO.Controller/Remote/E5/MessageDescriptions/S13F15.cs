using System.Collections.Generic;
using System.Linq;

using Agileo.Recipes.Annotations;
using Agileo.Semi.Communication.Abstractions.E173.Model;
using Agileo.Semi.Communication.Abstractions.E5;

using UnitySC.UTO.Controller.Remote.E5.DataItems;

namespace UnitySC.UTO.Controller.Remote.E5.MessageDescriptions
{
    /// <summary>S13,F15 Table Data Request (TDR)</summary>
    /// <remarks>
    /// This messages allows the host or the equipment to request part of all of a specific table. Either
    /// specific columns or specific rows may be requested, but not both at the same time. If S13,F15 is
    /// Multi-block, it must be preceded by the S13,F11/S13,F12 Inquire/Grant transaction.
    /// </remarks>
    public class S13F15 : Message
    {
        /// <summary>Initializes a new instance of <see cref="S13F15" /> message.</summary>
        [PublicAPI]
        protected S13F15()
            : base(13, 15)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="S13F15" /> message from a <see cref="SECSMessage" />
        /// model.
        /// </summary>
        /// <param name="model">The SECSMessage model.</param>
        /// <remarks>
        /// This constructor is required by extension method <see cref="MessageExtensions.As{T}" />.
        /// </remarks>
        [UsedImplicitly]
        protected S13F15(SECSMessage model)
            : base(13, 15, model)
        {
        }

        #region Properties

        public DATAID DataID => Body.First().ElementAt<DATAID>(0);

        public OBJSPEC ObjSpec => Body.First().ElementAt<OBJSPEC>(1);

        public TBLTYP TableType => Body.First().ElementAt<TBLTYP>(2);

        public TBLID TableID => Body.First().ElementAt<TBLID>(3);

        public TBLCMD TableCmd => Body.First().ElementAt<TBLCMD>(4);

        public IEnumerable<COLHDR> ColumnDefinitions
            => Body.First().ElementAt(5).Where<COLHDR>().DataItems.OfType<COLHDR>();

        public IEnumerable<TBLELT> TableElements
            => Body.First().ElementAt(6).Where<TBLELT>().DataItems.OfType<TBLELT>();

        #endregion

        #region Build methods

        /// <summary>
        /// Initializes a new instance of <see cref="S13F15" /> message to request all rows. If OBJSPEC is a
        /// zero-length item, then thoe owner of the table is the receiver of the message. If r is zero, any
        /// existing table definition of the given type and id is to be deleted. Otherwise, c1 may not be zero,
        /// and the value of c1 shall be less than or equal to the value of c.
        /// </summary>
        /// <param name="dataId">The  data ID.</param>
        /// <param name="objSpec">The objet ID.</param>
        /// <param name="tableType">The table type.</param>
        /// <param name="tableId">The table ID.</param>
        /// <param name="tableCmd">The table command.</param>
        /// <returns>A new <see cref="S13F15" /> message.</returns>
        public S13F15 RequestAllRows(
            DATAID dataId,
            OBJSPEC objSpec,
            TBLTYP tableType,
            TBLID tableId,
            TBLCMD tableCmd)
        {
            return New(dataId, objSpec, tableType, tableId, tableCmd);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="S13F15" /> message to request specified columns. If
        /// OBJSPEC is a zero-length item, then thoe owner of the table is the receiver of the message. If r is
        /// zero, any existing table definition of the given type and id is to be deleted. Otherwise, c1 may
        /// not be zero, and the value of c1 shall be less than or equal to the value of c.
        /// </summary>
        /// <param name="dataId">The  data ID.</param>
        /// <param name="objSpec">The objet ID.</param>
        /// <param name="tableType">The table type.</param>
        /// <param name="tableId">The table ID.</param>
        /// <param name="tableCmd">The table command.</param>
        /// <param name="columnDefinitions">The column definitions</param>
        /// <returns>A new <see cref="S13F15" /> message.</returns>
        public S13F15 RequestSpecifiedColumns(
            DATAID dataId,
            OBJSPEC objSpec,
            TBLTYP tableType,
            TBLID tableId,
            TBLCMD tableCmd,
            IEnumerable<COLHDR> columnDefinitions)
        {
            var message = New(dataId, objSpec, tableType, tableId, tableCmd);
            foreach (var col in columnDefinitions)
            {
                message.WithColumnDefinition(col);
            }

            return message;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="S13F15" /> message to request specified rows. If OBJSPEC
        /// is a zero-length item, then thoe owner of the table is the receiver of the message. If r is zero,
        /// any existing table definition of the given type and id is to be deleted. Otherwise, c1 may not be
        /// zero, and the value of c1 shall be less than or equal to the value of c.
        /// </summary>
        /// <param name="dataId">The  data ID.</param>
        /// <param name="objSpec">The objet ID.</param>
        /// <param name="tableType">The table type.</param>
        /// <param name="tableId">The table ID.</param>
        /// <param name="tableCmd">The table command.</param>
        /// <param name="tableElements">The table elements</param>
        /// <returns>A new <see cref="S13F15" /> message.</returns>
        public S13F15 RequestSpecifiedColumns(
            DATAID dataId,
            OBJSPEC objSpec,
            TBLTYP tableType,
            TBLID tableId,
            TBLCMD tableCmd,
            IEnumerable<TBLELT> tableElements)
        {
            var message = New(dataId, objSpec, tableType, tableId, tableCmd);
            foreach (var tableElement in tableElements)
            {
                message.WithTableElement(tableElement);
            }

            return message;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="S13F15" /> message. If OBJSPEC is a zero-length item, then
        /// thoe owner of the table is the receiver of the message. If r is zero, any existing table definition
        /// of the given type and id is to be deleted. Otherwise, c1 may not be zero, and the value of c1 shall
        /// be less than or equal to the value of c.
        /// </summary>
        /// <param name="dataId">The  data ID.</param>
        /// <param name="objSpec">The objet ID.</param>
        /// <param name="tableType">The table type.</param>
        /// <param name="tableId">The table ID.</param>
        /// <param name="tableCmd">The table command.</param>
        /// <returns>A new <see cref="S13F15" /> message.</returns>
        public static S13F15 New(
            DATAID dataId,
            OBJSPEC objSpec,
            TBLTYP tableType,
            TBLID tableId,
            TBLCMD tableCmd)
        {
            var message = new S13F15();
            message.Body.AddList(
                it =>
                {
                    it.AddItem(dataId);
                    it.AddItem(objSpec);
                    it.AddItem(tableType);
                    it.AddItem(tableId);
                    it.AddItem(tableCmd);
                    it.AddList("COLUMNDEFINITIONS");
                    it.AddList("ROWDEFINITIONS");
                });
            return message;
        }

        /// <summary>Adds column definition to the message</summary>
        /// <param name="columnDefinition">The column definition</param>
        /// <returns>This instance of <see cref="S13F15" /> message.</returns>
        public S13F15 WithColumnDefinition(COLHDR columnDefinition)
        {
            var columnDefinitions = (DataList)Body.First().ElementAt(5);
            columnDefinitions.AddItem(columnDefinition);
            return this;
        }

        /// <summary>Adds table element to the message</summary>
        /// <param name="tableElement">The table element</param>
        /// <returns>This instance of <see cref="S13F15" /> message.</returns>
        public S13F15 WithTableElement(TBLELT tableElement)
        {
            var tableElements = (DataList)Body.First().ElementAt(6);
            tableElements.AddItem(tableElement);
            return this;
        }

        #endregion
    }
}
