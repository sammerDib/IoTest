using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.Aligner.Resources;

namespace UnitySC.Equipment.Abstractions.Devices.Aligner.Conditions
{
    public class IsWaferPresent : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (context.Device is not Aligner aligner)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.DeviceNotAnAligner,
                    context.Device?.GetType().Name ?? "null"));
                return;
            }

            if (aligner.Location.Material == null || aligner.SubstrateDetectionError)
            {
                context.AddContextError(Messages.AlignerHaveNoMaterial);
            }
        }
    }
}
