using System.Collections.Generic;

using Agileo.Recipes.Annotations;
using Agileo.Semi.Communication.Abstractions.E173.Model;
using Agileo.Semi.Communication.Abstractions.E5;

using UnitySC.UTO.Controller.Remote.E5.DataItems;

namespace UnitySC.UTO.Controller.Remote.E5.MessageDescriptions
{
    /// <summary>S2,F44 Reset Spooling Acknowledge (RSA)</summary>
    /// <remarks>
    /// Acknowledge or error
    /// </remarks>
    public class S2F44 : Message
    {
        /// <summary>Initializes a new instance of <see cref="S2F44" /> message.</summary>
        [PublicAPI]
        protected S2F44()
            : base(2, 44)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="S2F44" /> message from a <see cref="SECSMessage" />
        /// model.
        /// </summary>
        /// <param name="model">The SECSMessage model.</param>
        /// <remarks>
        /// This constructor is required by extension method <see cref="MessageExtensions.As{T}" />.
        /// </remarks>
        [UsedImplicitly]
        protected S2F44(SECSMessage model)
            : base(2, 44, model)
        {
        }

        #region Properties

        public RSPACK ResetSpoolingAcknowledge => Body.First().ElementAt<RSPACK>(0);

        #endregion

        #region Build Methods

        /// <summary>
        /// Initializes a new instance of <see cref="S2F44" /> message with acknowledge spooling setup accepted.
        /// </summary>
        /// <returns>A new <see cref="S2F44" /> message.</returns>
        public static S2F44 AcknowledgeSpoolingSetupAccepted()
        {
            return New(RSPACK.AcknowledgeSpoolingSetupAccepted);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="S2F44" /> message with spooling setup rejected.
        /// </summary>
        /// <returns>A new <see cref="S2F44" /> message.</returns>
        public static S2F44 SpoolingSetupRejected()
        {
            return New(RSPACK.SpoolingSetupRejected);
        }

        /// <summary>Initializes a new instance of <see cref="S2F44" /> message.</summary>
        /// <param name="rspAck">The reset spooling ack.</param>
        /// <returns>A new <see cref="S2F44" /> message.</returns>
        public static S2F44 New(RSPACK rspAck)
        {
            var message = new S2F44();
            message.Body.AddList(
                body =>
                {
                    body.AddItem(rspAck);
                    body.AddList("STRERRORS");
                });

            return message;
        }

        /// <summary>Adds errors to the message</summary>
        /// <param name="strId">The stream ID.</param>
        /// <param name="strAck">The stream ack.</param>
        /// <param name="fcnIds">The function ids.</param>
        /// <returns>This instance of <see cref="S13F14" /> message.</returns>
        public S2F44 AddError(STRID strId, STRACK strAck, IEnumerable<FCNID> fcnIds)
        {
            var errors = (DataList)Body.First().ElementAt(1);
            errors.AddList(
                "STRERROR",
                er =>
                {
                    er.AddItem(strId);
                    er.AddItem(strAck);
                    er.AddList(
                        fcnList =>
                        {
                            foreach (var fcn in fcnIds)
                            {
                                fcnList.AddItem(fcn);
                            }
                        });
                });
            return this;
        }

        #endregion
    }
}
