using System.Globalization;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Devices.Controller.Resources;

namespace UnitySC.Equipment.Devices.Controller
{
    public class CheckAlignerEmpty : CSharpCommandConditionBehavior
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

            var aligner = controller.AllDevices<Abstractions.Devices.Aligner.Aligner>().FirstOrDefault();

            if (aligner == null)
            {
                context.AddContextError(Messages.AlignerNotAvailable);
                return;
            }

            if (aligner.Location.Material != null)
            {
                context.AddContextError(Messages.AlignerNotEmpty);
            }
        }
    }
}
