using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.EFEM.Controller.HostInterface.Enums;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class SetLightTowerStateCommand: RorzeCommand
    {
        #region Constructors

        public static SetLightTowerStateCommand NewOrder(
            LightState redLightState,
            LightState orangeLightState,
            LightState greenLightState,
            LightState blueLightState,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]>(1)
            {
                new[]
                {
                    ((uint)redLightState).ToString(),
                    ((uint)orangeLightState).ToString(),
                    ((uint)greenLightState).ToString(),
                    ((uint)blueLightState).ToString()
                }
            };

            return new SetLightTowerStateCommand(parameters, sender, eqFacade, logger);
        }

        private SetLightTowerStateCommand(
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(Constants.CommandType.Order, Constants.Commands.SetLightTowerState, parameters, sender, eqFacade, logger)
        {
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatAck(Message message)
        {
            // Command is finished on ACK received.
            CommandComplete();
            return true;
        }

        #endregion RorzeCommand
    }
}
