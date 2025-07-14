using System;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.EFEM.Controller.HostInterface.Statuses;
using UnitySC.EquipmentController.Simulator.Driver.Statuses;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class GetGeneralStatusesCommand: RorzeCommand
    {
        #region Constructors

        public static GetGeneralStatusesCommand NewOrder(IMacroCommandSender sender, IEquipmentFacade eqFacade, ILogger logger)
        {
            return new GetGeneralStatusesCommand(Constants.CommandType.Order, sender, eqFacade, logger);
        }

        public static GetGeneralStatusesCommand NewEvent(IMacroCommandSender sender, IEquipmentFacade eqFacade, ILogger logger)
        {
            return new GetGeneralStatusesCommand(Constants.CommandType.Event, sender, eqFacade, logger);
        }

        private GetGeneralStatusesCommand(
            char commandType,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(commandType, Constants.Commands.GetGeneralStatuses, null, sender, eqFacade, logger)
        {
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatAck(Message message)
        {
            if (message.CommandParameters.Count != 1
                || message.CommandParameters[0].Length != 1)
                return false;

            try
            {
                var status = new GeneralStatus(message.CommandParameters[0][0]);

                facade.SendEquipmentEvent((int)EventsToFacade.GeneralStatusReceived, new StatusEventArgs<GeneralStatus>(status));
                CommandComplete();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"{nameof(GetGeneralStatusesCommand)} - Failed when trying to parse general statuses from equipment. Received status=\"{message.CommandParameters[0][0]}\".");
                return false;
            }
        }


        protected override bool TreatEvent(Message message)
        {
            if (message.CommandParameters.Count != 1
                || message.CommandParameters[0].Length != 1)
                return false;

            try
            {
                var status = new GeneralStatus(message.CommandParameters[0][0]);
                
                facade.SendEquipmentEvent((int)EventsToFacade.GeneralStatusReceived, new StatusEventArgs<GeneralStatus>(status));

                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"{nameof(GetGeneralStatusesCommand)} - Failed when trying to parse general statuses from equipment. Received status=\"{message.CommandParameters[0][0]}\".");
                return false;
            }
        }

        #endregion RorzeCommand
    }
}
