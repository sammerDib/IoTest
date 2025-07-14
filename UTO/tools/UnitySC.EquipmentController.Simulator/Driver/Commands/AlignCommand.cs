using System.Collections.Generic;
using System.Globalization;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitsNet;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class AlignCommand: RorzeCommand
    {
        public static AlignCommand NewOrder(
            Angle angle,
            AlignType alignType,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var normalizedAngle = angle.Degrees % 360;
            if (normalizedAngle < 0)
                normalizedAngle += 360;

            var parameters = new List<string[]>
            {
                new [] 
                {
                    Constants.Aligner.ToString(),
                    normalizedAngle.ToString("F2", CultureInfo.InvariantCulture),
                    ((int)alignType).ToString()
                }
            };

            return new AlignCommand(parameters, sender, eqFacade, logger);
        }

        private AlignCommand(
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger) 
            :base(Constants.CommandType.Order, Constants.Commands.Align, parameters, sender, eqFacade, logger)
        {
        }

        protected override bool TreatEvent(Message message)
        {
            // If not an "order" command, we have no reason to treat the event
            if (Command.CommandType != Constants.CommandType.Order)
            {
                return false;
            }

            // Expected events have one parameter group of at least one argument
            // Success: eALGN:1
            // Error:   eALGN:2_<Level>_<ErrCode>_<Msg>
            // TMPError:eALGN:2 TODO: while errors are not managed by EFEM Controller. Must update condition.
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
