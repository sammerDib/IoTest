using System.Globalization;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Devices.Controller.Resources;

namespace UnitySC.Equipment.Devices.Controller
{
    public class CheckCarrierOpened : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            var controller = context.Device as Controller;
            if (controller == null)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.InvalidDeviceType,
                    nameof(Controller),
                    context.Device?.GetType().Name ?? "null"));
                return;
            }

            if (!(context.GetArgument("loadPort") is ILoadPort))
            {
                context.AddContextError(Messages.IncorrectLoadPortSelected);
                return;
            }

            var expectedLoadPort = (Abstractions.Devices.LoadPort.LoadPort)context.GetArgument("loadPort");

            if (expectedLoadPort == null)
            {
                context.AddContextError(Messages.LoadPortNotAvailable);
                return;
            }

            if (expectedLoadPort.PhysicalState != LoadPortState.Open && !expectedLoadPort.Configuration.CloseDoorAfterRobotAction)
            {
                context.AddContextError(Messages.CarrierNotOpened);
            }
        }
    }
}
