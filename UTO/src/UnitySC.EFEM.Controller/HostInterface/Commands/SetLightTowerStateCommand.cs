using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface.Converters;
using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the set light command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.5.3 oSTwr
    /// </summary>
    public class SetLightTowerStateCommand : BaseCommand
    {
        public SetLightTowerStateCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(
                Constants.Commands.SetLightTowerState,
                sender,
                eqFacade,
                logger,
                equipmentManager)
        {
        }

        protected override bool TreatOrder(Message message)
        {
            // Check number of parameters
            if (message.CommandParameters.Count != 1 || message.CommandParameters[0].Length != 4)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            // Check first parameter validity
            if (!int.TryParse(message.CommandParameters[0][0], out int _))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            if (!EnumHelpers.TryParseEnumValue(message.CommandParameters[0][0], out LightState redLightState))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            // Check second parameter validity
            if (!int.TryParse(message.CommandParameters[0][1], out int _))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            if (!EnumHelpers.TryParseEnumValue(message.CommandParameters[0][1], out LightState yellowLightState))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            // Check third parameter validity
            if (!int.TryParse(message.CommandParameters[0][2], out int _))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            if (!EnumHelpers.TryParseEnumValue(message.CommandParameters[0][2], out LightState greenLightState))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            // Check fourth parameter validity
            if (!int.TryParse(message.CommandParameters[0][3], out int _))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            if (!EnumHelpers.TryParseEnumValue(message.CommandParameters[0][3], out LightState blueLightState))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            // Send acknowledge response (i.e. command ok, will be performed)
            SendAcknowledge(message);

            // Start command asynchronously (no completion event)
            EquipmentManager.LightTower.DefineRedLightModeAsync(LightStateConverter.ToGui(redLightState));
            EquipmentManager.LightTower.DefineOrangeLightModeAsync(LightStateConverter.ToGui(yellowLightState));
            EquipmentManager.LightTower.DefineGreenLightModeAsync(LightStateConverter.ToGui(greenLightState));
            EquipmentManager.LightTower.DefineBlueLightModeAsync(LightStateConverter.ToGui(blueLightState));

            return true;
        }
    }
}
