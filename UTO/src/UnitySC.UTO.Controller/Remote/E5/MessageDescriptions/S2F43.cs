using System.Collections.Generic;
using System.Linq;

using Agileo.Recipes.Annotations;
using Agileo.Semi.Communication.Abstractions.E173.Model;
using Agileo.Semi.Communication.Abstractions.E5;

using UnitySC.UTO.Controller.Remote.E5.DataItems;

namespace UnitySC.UTO.Controller.Remote.E5.MessageDescriptions
{
    /// <summary>S2,F43 Reset Spooling Streams and Functions (RSSF)</summary>
    /// <remarks>
    /// This message allows the host to select specific streams and functions to be spooled whenever spooling is active
    /// </remarks>
    public class S2F43 : Message
    {
        /// <summary>Initializes a new instance of <see cref="S2F43" /> message.</summary>
        [PublicAPI]
        protected S2F43()
            : base(2, 43)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="S2F43" /> message from a <see cref="SECSMessage" />
        /// model.
        /// </summary>
        /// <param name="model">The SECSMessage model.</param>
        /// <remarks>
        /// This constructor is required by extension method <see cref="MessageExtensions.As{T}" />.
        /// </remarks>
        [UsedImplicitly]
        protected S2F43(SECSMessage model)
            : base(2, 43, model)
        {
        }

        #region Properties

        public IEnumerable<STRFCNIDS> StreamFunctions
            => Body.First().ElementAt(0).Where<STRFCNIDS>().DataItems.OfType<STRFCNIDS>();

        #endregion
    }
}
