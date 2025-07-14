using System.Linq;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Format.Metro;

namespace UnitySC.PM.ANA.Service.Measure.Shared
{
    internal static class MeasurePointResultExtensions
    {
        public static void FillMeasurePointStateFromData(this MeasurePointResult result, MeasureSettingsBase measureSettings)
        {
            if (result.AreAllDataSuccess())
                result.State = MeasureState.Success;
            else if (result.IsAtLeastOneDataNotMeasured())
                result.State = MeasureState.NotMeasured;
            else if (result.IsAtLeastOneDataSuccess())
                result.State = MeasureState.Partial;
            else
                result.State = MeasureState.Error;
        }

        private static bool AreAllDataSuccess(this MeasurePointResult result)
        {
            return result.Datas.TrueForAll((data) => data.State == MeasureState.Success);
        }

        private static bool IsAtLeastOneDataNotMeasured(this MeasurePointResult result)
        {
            return result.Datas.Any((data) => data.State == MeasureState.NotMeasured);
        }

        private static bool IsAtLeastOneDataSuccess(this MeasurePointResult result)
        {
            return result.Datas.Any((data) => data.State == MeasureState.Success);
        }
    }

    internal static class MeasureResultExtensions
    {
        public static void FillMeasureDiesStateFromData(this MeasureResultBase result)
        {
            if (result.Dies is null)
                return;

            foreach (var die in result.Dies)
            {
                if (die.AreAllPointsSuccess())
                    die.State = GlobalState.Success;
                else if (die.IsAtLeastOnePointNotMeasured() || die.IsAtLeastOnePointSuccess())
                    die.State = GlobalState.Partial;
                else
                    die.State = GlobalState.Error;
            }
        }

        private static bool AreAllPointsSuccess(this MeasureDieResult result)
        {
            return result.Points.TrueForAll((data) => data.State == MeasureState.Success);
        }

        private static bool IsAtLeastOnePointNotMeasured(this MeasureDieResult result)
        {
            return result.Points.Any((data) => data.State == MeasureState.NotMeasured);
        }

        private static bool IsAtLeastOnePointSuccess(this MeasureDieResult result)
        {
            return result.Points.Any((data) => data.State == MeasureState.Success);
        }
    }
}
