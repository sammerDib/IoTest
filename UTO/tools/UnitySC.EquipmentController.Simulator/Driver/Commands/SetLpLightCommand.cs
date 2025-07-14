using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class SetLpLightCommand : LoadPortCommand
    {
        #region Constructors

        public static SetLpLightCommand NewOrder(
            Constants.Port loadPort,
            bool loadLightState,
            bool unloadLightState,
            bool manualLightState,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]> 
                { 
                    new[] 
                    { 
                        ((int)loadPort).ToString(), 
                        loadLightState ? "1" : "0", 
                        unloadLightState ? "1" : "0", 
                        manualLightState ? "1" : "0"
                    }
                };

            return new SetLpLightCommand(
                Constants.CommandType.Order,
                loadPort,
                parameters,
                sender,
                eqFacade,
                logger);
        }

        private SetLpLightCommand(
            char commandType,
            Constants.Port loadPort,
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(commandType, Constants.Commands.SetLightOnLp, loadPort, parameters, sender, eqFacade, logger)
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
