using System.Globalization;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Vendor.JobDefinition;
using UnitySC.Equipment.Devices.Controller.Resources;

namespace UnitySC.Equipment.Devices.Controller
{
    public class IsJobResumable : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (context.Device is not Controller controller)
            {
                context.AddContextError(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Messages.DeviceNotController,
                        context.Device?.GetType().Name ?? "null"));
                return;
            }

            var expectedJobName = context.GetArgument("jobName").ToString();

            if (controller.Jobs.FirstOrDefault(j => j.Name == expectedJobName) is not { } job)
            {
                context.AddContextError(
                    string.Format(CultureInfo.InvariantCulture, Messages.NoJobRunning));
                return;
            }

            if (job.Status != JobStatus.Paused)
            {
                context.AddContextError(
                    string.Format(CultureInfo.InvariantCulture, Messages.CurrentJobNotResumable));
            }
        }
    }
}
