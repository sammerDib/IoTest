using System;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.EFEM.Controller.HostInterface.Statuses;
using UnitySC.EquipmentController.Simulator.Driver.Statuses;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class SystemInputEvent: RorzeCommand
    {
        #region Constructors

        public static SystemInputEvent NewEvent(IMacroCommandSender sender, IEquipmentFacade eqFacade, ILogger logger)
        {
            return new SystemInputEvent(Constants.CommandType.Event, sender, eqFacade, logger);
        }

        private SystemInputEvent(
            char commandType,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(commandType, Constants.Commands.GetSystemStatus, null, sender, eqFacade, logger)
        {
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatEvent(Message message)
        {
            if (message.CommandParameters.Count != 1
                || message.CommandParameters[0].Length != 1)
                return false;

            try
            {
                var status = new SystemStatus(message.CommandParameters[0][0]);

                facade.SendEquipmentEvent((int)EventsToFacade.SystemStatusReceived,
                    new StatusEventArgs<SystemStatus>(status));

                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"{nameof(SystemStatus)} - Failed when trying to parse system statuses from equipment. Received status=\"{message.CommandParameters[0][0]}\".");
                return false;
            }
        }

        #endregion RorzeCommand
    }
}
