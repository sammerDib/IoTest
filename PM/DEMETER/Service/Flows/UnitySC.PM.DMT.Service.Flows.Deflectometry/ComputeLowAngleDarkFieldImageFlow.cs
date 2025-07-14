using System.IO;

using UnitySC.PM.DMT.Service.Flows.Shared;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Implementation;

namespace UnitySC.PM.DMT.Service.Flows.Deflectometry
{
    public class ComputeLowAngleDarkFieldImageFlow : FlowComponent<ComputeLowAngleDarkFieldImageInput, ComputeLowAngleDarkFieldImageResult, ComputeLowAngleDarkFieldImageConfiguration>
    {
        public const int MaximumNumberOfSteps = 3;

        public ComputeLowAngleDarkFieldImageFlow(ComputeLowAngleDarkFieldImageInput input) : base(input, "ComputeLowAngleDarkFieldFlow")
        {
        }

        protected override void Process()
        {
            SetProgressMessage($"Starting {Input.Side}side {Name} for period {Input.Period}px");
            CheckCancellation();
            var rawDark =
                DeflectometryCalculations.ComputeBaseDark(Input.XResult.Dark, Input.YResult.Dark, Input.XResult.Mask);
            CheckCancellation();
            Result.DarkImage =
                DeflectometryCalculations.ApplyDarkDynamicsCoefficient(rawDark, Input.XResult.Mask, Input.DarkDynamic, Configuration.PercentageOfLowSaturation);
            Result.Fringe = Input.Fringe;
            Result.Period = Input.Period;
            SetProgressMessage($"Successfully computed Low angle dark-field image for period {Input.Period}px");
            if (Configuration.IsAnyReportEnabled())
            {
                CheckCancellation();
                using (var rawDarkMil = rawDark.ConvertToUSPImageMil(false))
                {
                    rawDarkMil.Save(Path.Combine(ReportFolder, $"dark_{Input.Period}px_raw.tif"));
                }
            }
        }
    }
}
