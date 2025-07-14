using System.Collections.Generic;
using System.Linq;

using Agileo.Recipes.Annotations;
using Agileo.Semi.Communication.Abstractions.E173.Model;
using Agileo.Semi.Communication.Abstractions.E5;

using UnitySC.UTO.Controller.Remote.E5.DataItems;

namespace UnitySC.UTO.Controller.Remote.E5.MessageDescriptions
{
    /// <summary>S13,F13 Table Data Send (TDS)</summary>
    /// <remarks>
    /// This message allows the host and the equipment to exchange predefined datasets in a tabular format.
    /// The first element of every row is used to reference that row for all other elements. If S13,F13 is
    /// Multi-block, it must be preceded by the S13,F11/S13,F12 Inquire/Grant transaction.
    /// </remarks>
    public class S13F13 : Message
    {
        /// <summary>Initializes a new instance of <see cref="S13F13" /> message.</summary>
        [PublicAPI]
        protected S13F13()
            : base(13, 13)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="S13F13" /> message from a <see cref="SECSMessage" />
        /// model.
        /// </summary>
        /// <param name="model">The SECSMessage model.</param>
        /// <remarks>
        /// This constructor is required by extension method <see cref="MessageExtensions.As{T}" />.
        /// </remarks>
        [UsedImplicitly]
        protected S13F13(SECSMessage model)
            : base(13, 13, model)
        {
        }

        #region Properties

        public DATAID DataID => Body.First().ElementAt<DATAID>(0);

        public OBJSPEC ObjSpec => Body.First().ElementAt<OBJSPEC>(1);

        public TBLTYP TableType => Body.First().ElementAt<TBLTYP>(2);

        public TBLID TableID => Body.First().ElementAt<TBLID>(3);

        public TBLCMD TableCmd => Body.First().ElementAt<TBLCMD>(4);

        /// <summary>Gets the list of attributes.</summary>
        public IEnumerable<ATTRIBUTE> Attributes
            => Body.First().ElementAt(5).Where<ATTRIBUTE>().DataItems.OfType<ATTRIBUTE>();

        public IEnumerable<COLHDR> ColumnDefinitions
            => Body.First().ElementAt(6).Where<COLHDR>().DataItems.OfType<COLHDR>();

        #endregion

        #region Build methods

        /// <summary>
        /// Initializes a new instance of <see cref="S13F13" /> message. If OBJSPEC is a zero-length item, then
        /// thoe owner of the table is the receiver of the message. If r is zero, any existing table definition
        /// of the given type and id is to be deleted. Otherwise, c1 may not be zero, and the value of c1 shall
        /// be less than or equal to the value of c.
        /// </summary>
        /// <param name="dataId">The  data ID.</param>
        /// <param name="objSpec">The objet ID.</param>
        /// <param name="tableType">The table type.</param>
        /// <param name="tableId">The table ID.</param>
        /// <param name="tableCmd">The table command.</param>
        /// <returns>A new <see cref="S13F13" /> message.</returns>
        public static S13F13 New(
            DATAID dataId,
            OBJSPEC objSpec,
            TBLTYP tableType,
            TBLID tableId,
            TBLCMD tableCmd)
        {
            var message = new S13F13();
            message.Body.AddList(
                it =>
                {
                    it.AddItem(dataId);
                    it.AddItem(objSpec);
                    it.AddItem(tableType);
                    it.AddItem(tableId);
                    it.AddItem(tableCmd);
                    it.AddList("TABLEATTRIBUTES");
                    it.AddList("COLUMNDEFINITIONS");
                    it.AddList("ROWDEFINITIONS");
                });
            return message;
        }

        /// <summary>Adds attribute to the message</summary>
        /// <param name="attributeId">The attribute identifier.</param>
        /// <param name="attributeData">The attribute data.</param>
        /// <returns>This instance of <see cref="S13F13" /> message.</returns>
        public S13F13 WithAttribute(ATTRID attributeId, ATTRDATA attributeData)
        {
            return InternalAddAttribute(attributeId, attributeData);
        }

        /// <summary>Adds column definition to the message</summary>
        /// <param name="columnDefinition">The column definition</param>
        /// <returns>This instance of <see cref="S13F13" /> message.</returns>
        public S13F13 WithColumnDefinition(COLHDR columnDefinition)
        {
            var columnDefinitions = (DataList)Body.First().ElementAt(6);
            columnDefinitions.AddItem(columnDefinition);
            return this;
        }

        /// <summary>Adds row definition to the message</summary>
        /// <param name="tableElements">The table elements</param>
        /// <returns>This instance of <see cref="S13F13" /> message.</returns>
        public S13F13 WithRowDefinition(IEnumerable<TBLELT> tableElements)
        {
            var rowDefinitions = (DataList)Body.First().ElementAt(7);
            rowDefinitions.AddList(
                list =>
                {
                    foreach (var tableElement in tableElements)
                    {
                        list.AddItem(tableElement);
                    }
                });

            return this;
        }

        #endregion

        #region Private methods

        /// <summary>Adds attribute to the message</summary>
        /// <param name="attributeId">The attribute identifier.</param>
        /// <param name="attributeData">The attribute data.</param>
        /// <returns>This instance of <see cref="S13F13" /> message.</returns>
        private S13F13 InternalAddAttribute(ATTRID attributeId, ATTRDATA attributeData)
        {
            var attribute = Attributes.FirstOrDefault(attr => attr.AttributeId == attributeId);

            if (attribute == null)
            {
                var attributes = (DataList)Body.First().ElementAt(5);
                attributes.AddList(
                    nameof(ATTRIBUTE),
                    att =>
                    {
                        att.AddItem(attributeId);
                        att.AddItem(attributeData);
                    });
            }
            else
            {
                attribute.SetValue(attributeData);
            }

            return this;
        }

        #endregion
    }
}
