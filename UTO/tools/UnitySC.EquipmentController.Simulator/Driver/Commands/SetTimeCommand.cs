using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class SetTimeCommand : RorzeCommand
    {
        #region constructors
        public static SetTimeCommand NewOrder(
            string time,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]>
            {
                new[]
                {
                    time
                }
            };

            return new SetTimeCommand(parameters, sender, eqFacade, logger);
        }

        private SetTimeCommand(
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger) :
            base(Constants.CommandType.Order, Constants.Commands.SetTime, parameters, sender, eqFacade, logger)
        {
        }


        #endregion

        #region RorzeCommand

        protected override bool TreatAck(Message message)
        {
            CommandComplete();
            return true;
        }

        #endregion
    }
}
