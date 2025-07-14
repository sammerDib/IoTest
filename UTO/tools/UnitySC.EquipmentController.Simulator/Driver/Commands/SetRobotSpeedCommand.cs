using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class SetRobotSpeedCommand: RorzeCommand
    {
        /// <param name="speed">The new robot speed. Validity range is: 5% ~ 9:50%, A:55% ~ J:100%.</param>
        /// <param name="sender"><inheritdoc cref="sender"/></param>
        /// <param name="eqFacade"><inheritdoc cref="eqFacade"/></param>
        /// <param name="logger"><inheritdoc cref="logger"/></param>
        public SetRobotSpeedCommand(
            char speed,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger) :
            base(
                Constants.CommandType.Order, 
                Constants.Commands.SetRobotSpeed, 
                new List<string[]> { new[] { speed.ToString() } },
                sender, 
                eqFacade, 
                logger)
        {
        }

        protected override bool TreatEvent(Message message)
        {
            // If not an "order" command, we have no reason to treat the event
            if (Command.CommandType != Constants.CommandType.Order)
            {
                return false;
            }

            // Expected events have one parameter group of at least two arguments
            // Success: eSSPD:1
            // Error:   eSSPD:2_<Level>_<ErrCode>_<Msg>
            // TMPError:eSSPD:2 TODO: while errors are not managed by EFEM Controller. Must update condition.
            if (message.CommandParameters.Count != 1 || message.CommandParameters[0].Length != 1)
            {
                return false;
            }

            // Parse argument to get command status
            if (!int.TryParse(message.CommandParameters[0][0], out int result))
            {
                return false;
            }

            switch (result)
            {
                case (int)Constants.EventResult.Success:
                    // TODO Notify driver about command success
                    break;
                case (int)Constants.EventResult.Error:
                    // TODO Notify driver about command failure
                    break;
            }

            CommandComplete();
            return true;
        }
    }
}
