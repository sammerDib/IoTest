using System.Linq;

using Agileo.Semi.Communication.Abstractions.E5;
using Agileo.Semi.Gem.Abstractions.E30;

using UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager;
using UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem;
using UnitySC.UTO.Controller.Remote.E5.DataItems;
using UnitySC.UTO.Controller.Remote.E5.MessageDescriptions;

namespace UnitySC.UTO.Controller.Remote.E5.MessageHandlers
{
    public class S13F14MessageHandler : E30MessageHandler
    {
        private readonly ToolControlManager _toolControlManager;

        public S13F14MessageHandler(ToolControlManager toolControlManager)
            : base(13, 14)
        {
            _toolControlManager = toolControlManager;
        }

        public override MessageHandlerResult Handle(Message message)
        {
            var s13F14 = message.As<S13F14>();

            var ack = s13F14.TableAck;
            var errors = s13F14.Errors.ToList();

            var secsErrorList = new SecsErrorList();
            foreach (var error in errors)
            {
                secsErrorList.Add(new SecsError(error.ErrorCode, error.ErrorText));
            }

            _toolControlManager?.SendDataSetAck(
                new TableDataResponse(ack != TBLACK.Success) { Errors = secsErrorList });

            return MessageHandlerResult.NoResponse();
        }
    }
}
