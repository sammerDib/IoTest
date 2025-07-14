using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class InitializeCommand : RorzeCommand
    {
        #region Constructors

        public static InitializeCommand NewOrder(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            return new InitializeCommand(Constants.CommandType.Order, null, sender, eqFacade, logger);
        }

        public static InitializeCommand NewOrder(
            Constants.Unit unit,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]> { new[] { ((int)unit).ToString() } };

            return new InitializeCommand(Constants.CommandType.Order, parameters, sender, eqFacade, logger);
        }

        private InitializeCommand(
            char commandType,
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(commandType, Constants.Commands.Initialize, parameters, sender, eqFacade, logger)
        {
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatEvent(Message message)
        {
            // TODO Check received parameters to determine if success or failure

            if (Command.CommandType == Constants.CommandType.Order)
            {
                CommandComplete();
            }

            return true;
        }

        #endregion RorzeCommand
    }
}
