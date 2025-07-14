using System;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.EquipmentController.Simulator.Driver.Statuses;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class GetPressureCommand : RorzeCommand
    {
        #region Constructors

        public static GetPressureCommand NewOrder(IMacroCommandSender sender, IEquipmentFacade eqFacade, ILogger logger)
        {
            return new GetPressureCommand(Constants.CommandType.Order, sender, eqFacade, logger);
        }

        private GetPressureCommand(
            char commandType,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(commandType, Constants.Commands.GetPressure, null, sender, eqFacade, logger)
        {
        }

        protected override bool TreatAck(Message message)
        {
            if (message.CommandParameters.Count != 1
                || message.CommandParameters[0].Length > 2)
                return false;


            try
            {
                double.TryParse(message.CommandParameters[0][0].Replace("mPa", string.Empty), out double pressure);
                var status = new PressureStatus(pressure);

                facade.SendEquipmentEvent((int)EventsToFacade.EfemPressureReceived, new StatusEventArgs<PressureStatus>(status));
                CommandComplete();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"{nameof(GetPressureCommand)} - Failed when trying to parse pressure statuses from equipment. Received status=\"{message.CommandParameters[0][0]}\".");
                return false;
            }
        }


        #endregion Constructors
    }
}
