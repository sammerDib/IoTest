using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface.Statuses;
using UnitySC.Equipment.Abstractions;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the get system input signal status command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.5.6 eSYSI
    /// </summary>
    public class SystemInputEvent : BaseCommand
    {
        public static SystemInputEvent NewEvent(
            SystemStatus status,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
        {
            var parameters = new List<string[]>(1) { new[] { status.ToString() } };
            var message    = new Message(Constants.CommandType.Event, Constants.Commands.GetSystemStatus, parameters);

            return new SystemInputEvent(message, sender, eqFacade, logger, equipmentManager);
        }

        private SystemInputEvent(
            Message message,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(message, sender, eqFacade, logger, equipmentManager)
        {
        }

        protected override bool TreatOrder(Message message)
        {
            throw new System.NotImplementedException();
        }
    }
}
