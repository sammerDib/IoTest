using System.Linq;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosCppWrapper;

namespace UnitySC.PM.ANA.Service.Measure.Shared
{
    public class ProfileAnalyserResultToMeasureScanLine
    {
        public RawProfile ConvertProfile(Profile2d profile, LengthUnit xUnit, LengthUnit yUnit)
        {
            return new RawProfile
            {
                XUnit = xUnit,
                ZUnit = yUnit,
                RawPoints = profile.Select(point => new RawProfilePoint
                {
                    X = point.X,
                    Z = point.Y
                }).ToList()
            };
        }

        public (MeasureState, string) ConvertStatusToStateMessage(ProfileAnalyserResult.Status status)
        {
            switch (status)
            {
                case ProfileAnalyserResult.Status.Ok:
                    return (MeasureState.Success, null);
                case ProfileAnalyserResult.Status.EmptyProfile:
                    return (MeasureState.NotMeasured, "Scan gave an empty profile");
                case ProfileAnalyserResult.Status.EmptyProfileNan:
                    return (MeasureState.NotMeasured, "All points were measured as NaN");
                case ProfileAnalyserResult.Status.ProfileTooSmallAfterStdDevFiltering:
                    return (MeasureState.NotMeasured, "Too many points were filtered out because they were too far of the average. Try to reduce Nb Std Dev filtering");
                default:
                    return (MeasureState.NotMeasured, "Unknown ProfileAnalyser cause");
            }
        }
        }
}
