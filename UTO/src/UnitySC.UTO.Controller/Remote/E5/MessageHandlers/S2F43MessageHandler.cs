using Agileo.Semi.Communication.Abstractions.E5;
using Agileo.Semi.Gem.Abstractions.E30;

using UnitySC.UTO.Controller.Remote.E5.MessageDescriptions;

namespace UnitySC.UTO.Controller.Remote.E5.MessageHandlers
{
    public class S2F43MessageHandler : E30MessageHandler
    {
        public S2F43MessageHandler() : base(2, 43)
        {
        }

        public S2F43MessageHandler(bool isOptional = false) : base(StreamAndFunction.New(2, 43), isOptional)
        {
        }

        public override MessageHandlerResult Handle(Message message)
        {
            var s2f43 = message.As<S2F43>();

            if (s2f43 == null)
            {
                return MessageHandlerResult.WithResponse(S2F44.SpoolingSetupRejected());
            }

            return MessageHandlerResult.WithResponse(
                !s2f43.Body.First().Any()
                    ? S2F44.AcknowledgeSpoolingSetupAccepted()
                    : S2F44.SpoolingSetupRejected());
        }
    }
}
