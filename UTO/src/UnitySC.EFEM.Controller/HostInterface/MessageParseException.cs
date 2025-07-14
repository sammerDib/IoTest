using System;
using System.Text;

using Agileo.Drivers;

namespace UnitySC.EFEM.Controller.HostInterface
{
    public class MessageParseException : Exception
    {
        #region Constructors

        public MessageParseException(
            Error error,
            Message parsedMessage = null,
            string message = null,
            Exception innerException = null)
            : base(message ?? string.Empty, innerException)
        {
            Error = error ?? throw new ArgumentNullException(nameof(error));
            ParsedMessage = parsedMessage;
        }

        #endregion Constructors

        #region Properties

        public Error Error { get; }

        public Message ParsedMessage { get; }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());
            builder.AppendLine("---------------------");
            builder.AppendLine($"Parsed Message: {(ParsedMessage == null ? "<none>" : ParsedMessage.ToString())}");
            builder.AppendLine("Parse error:");
            builder.AppendLine(Error.ToString());
            return base.ToString();
        }

        #endregion Methods
    }
}
