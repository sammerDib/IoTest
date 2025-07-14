using System.Collections.Generic;
using System.Linq;

using Agileo.Recipes.Annotations;
using Agileo.Semi.Communication.Abstractions.E173.Model;
using Agileo.Semi.Communication.Abstractions.E5;

using UnitySC.UTO.Controller.Remote.E5.DataItems;

namespace UnitySC.UTO.Controller.Remote.E5.MessageDescriptions
{
    /// <summary>S13,F16 Table Data (TD)</summary>
    /// <remarks>This message is used to return data from the requested table.</remarks>
    public class S13F16 : Message
    {
        /// <summary>Initializes a new instance of <see cref="S13F16" /> message.</summary>
        [PublicAPI]
        protected S13F16()
            : base(13, 16)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="S13F16" /> message from a <see cref="SECSMessage" />
        /// model.
        /// </summary>
        /// <param name="model">The SECSMessage model.</param>
        /// <remarks>
        /// This constructor is required by extension method <see cref="MessageExtensions.As{T}" />.
        /// </remarks>
        [UsedImplicitly]
        protected S13F16(SECSMessage model)
            : base(13, 16, model)
        {
        }

        #region Properties

        public TBLTYP TableType => Body.First().ElementAt<TBLTYP>(0);

        public TBLID TableID => Body.First().ElementAt<TBLID>(1);

        /// <summary>Gets the list of attributes.</summary>
        public IEnumerable<ATTRIBUTE> Attributes
            => Body.First().ElementAt(2).Where<ATTRIBUTE>().DataItems.OfType<ATTRIBUTE>();

        public IEnumerable<COLHDR> ColumnDefinitions
            => Body.First().ElementAt(3).Where<COLHDR>().DataItems.OfType<COLHDR>();

        /// <summary>Gets the table ack</summary>
        public TBLACK TableAck => Body.First().ElementAt(5).ElementAt<TBLACK>(0);

        /// <summary>Gets the list of error information.</summary>
        public IEnumerable<ERROR> Errors
            => Body.First()
                .ElementAt(1)
                .ElementAt(1)
                .RecursiveWhere<ERROR>()
                .DataItems.OfType<ERROR>();

        #endregion

        #region Build methods

        /// <summary>
        /// Initializes a new instance of <see cref="S13F16" /> message with success.
        /// </summary>
        /// <param name="tableType">The table type.</param>
        /// <param name="tableId">The table ID.</param>
        /// <returns>A new <see cref="S13F16" /> message.</returns>
        public static S13F16 Success(TBLTYP tableType, TBLID tableId)
        {
            return New(tableType, tableId, TBLACK.Success);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="S13F16" /> message with failure.
        /// </summary>
        /// <param name="tableType">The table type.</param>
        /// <param name="tableId">The table ID.</param>
        /// <returns>A new <see cref="S13F16" /> message.</returns>
        public static S13F16 Failure(TBLTYP tableType, TBLID tableId)
        {
            return New(tableType, tableId, TBLACK.Failure);
        }

        /// <summary>Initializes a new instance of <see cref="S13F16" /> message.</summary>
        /// <param name="tableType">The table type.</param>
        /// <param name="tableId">The table ID.</param>
        /// <param name="tableAck">The table ack.</param>
        /// <returns>A new <see cref="S13F16" /> message.</returns>
        public static S13F16 New(TBLTYP tableType, TBLID tableId, TBLACK tableAck)
        {
            var message = new S13F16();
            message.Body.AddList(
                it =>
                {
                    it.AddItem(tableType);
                    it.AddItem(tableId);
                    it.AddList("TABLEATTRIBUTES");
                    it.AddList("COLUMNDEFINITIONS");
                    it.AddList("ROWDEFINITIONS");
                    it.AddList(
                        ackList =>
                        {
                            ackList.AddItem(tableAck);
                            ackList.AddList("ERRORS");
                        });
                });
            return message;
        }

        /// <summary>Adds attribute to the message</summary>
        /// <param name="attributeId">The attribute identifier.</param>
        /// <param name="attributeData">The attribute data.</param>
        /// <returns>This instance of <see cref="S13F16" /> message.</returns>
        public S13F16 WithAttribute(ATTRID attributeId, ATTRDATA attributeData)
        {
            return InternalAddAttribute(attributeId, attributeData);
        }

        /// <summary>Adds column definition to the message</summary>
        /// <param name="columnDefinition">The column definition</param>
        /// <returns>This instance of <see cref="S13F16" /> message.</returns>
        public S13F16 WithColumnDefinition(COLHDR columnDefinition)
        {
            var columnDefinitions = (DataList)Body.First().ElementAt(3);
            columnDefinitions.AddItem(columnDefinition);
            return this;
        }

        /// <summary>Adds row definition to the message</summary>
        /// <param name="tableElements">The table elements</param>
        /// <returns>This instance of <see cref="S13F16" /> message.</returns>
        public S13F16 WithRowDefinition(IEnumerable<TBLELT> tableElements)
        {
            var rowDefinitions = (DataList)Body.First().ElementAt(4);
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

        /// <summary>Adds errors to the message</summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorText">The error text.</param>
        /// <returns>This instance of <see cref="S13F16" /> message.</returns>
        public S13F16 AddError(ERRCODE errorCode, ERRTEXT errorText)
        {
            var errors = (DataList)Body.First().ElementAt(5).ElementAt(1);
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

        #region Private methods

        /// <summary>Adds attribute to the message</summary>
        /// <param name="attributeId">The attribute identifier.</param>
        /// <param name="attributeData">The attribute data.</param>
        /// <returns>This instance of <see cref="S13F16" /> message.</returns>
        private S13F16 InternalAddAttribute(ATTRID attributeId, ATTRDATA attributeData)
        {
            var attribute = Attributes.FirstOrDefault(attr => attr.AttributeId == attributeId);

            if (attribute == null)
            {
                var attributes = (DataList)Body.First().ElementAt(2);
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
