using System.Globalization;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Devices.Controller.Resources;

namespace UnitySC.Equipment.Devices.Controller
{
    public class CheckLoadPortReady : CSharpCommandConditionBehavior
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

            var expectedLoadPort = (LoadPort)context.GetArgument("loadPort");

            if (expectedLoadPort == null)
            {
                context.AddContextError(Messages.LoadPortNotAvailable);
                return;
            }

            if (expectedLoadPort.State != OperatingModes.Idle)
            {
                context.AddContextError(Messages.LoadPortNotIdle);
            }
        }
    }
}
