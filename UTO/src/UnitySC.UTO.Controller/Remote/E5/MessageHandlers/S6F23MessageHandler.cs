using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agileo.Semi.Communication.Abstractions.E5;
using Agileo.Semi.Communication.Abstractions.E5.MessageDescriptions;
using Agileo.Semi.Gem.Abstractions.E30;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace UnitySC.UTO.Controller.Remote.E5.MessageHandlers
{
    public class S6F23MessageHandler : E30MessageHandler
    {
        public S6F23MessageHandler() : base(6, 23)
        {
        }

        public S6F23MessageHandler(bool isOptional = false) : base(StreamAndFunction.New(6, 23), isOptional)
        {
        }

        public override MessageHandlerResult Handle(Message message)
        {
            var s6f23 = message.As<S6F23>();

            if (s6f23 == null)
            {
                return MessageHandlerResult.WithResponse(S6F24.DeniedBusyTryLater());
            }

            if (Equals(s6f23.Rsdc, RSDC.PurgeSpooledMessages))
            {
                return MessageHandlerResult.WithResponse(S6F24.OK());
            }

            if (Equals(s6f23.Rsdc, RSDC.TransmitSpooledMessages))
            {
                return MessageHandlerResult.WithResponse(S6F24.DeniedSpooledDataDoesNotExist());
            }

            return MessageHandlerResult.WithResponse(S6F24.DeniedBusyTryLater());
        }
    }
}
