using System.Collections.Generic;
using System.Linq;

namespace UnitySC.PM.DMT.Service.Interface.Recipe
{
    public static class DMTDeflectometryMeasureHelper
    {
        public static List<int> CreatePeriodsListForRatio(int basePeriod, int screenWidth, int screenHeight, int ratio)
        {
            
            if (basePeriod > 0 && ratio > 0)
            {
                var periods = new List<int> {
                    basePeriod
                };
                if (ratio > 1)
                {
                    int newPeriod = basePeriod;
                    do
                    {
                        newPeriod *= ratio;
                        if (newPeriod < screenWidth * 10)
                            periods.Add(newPeriod);
                        else
                            break;
                    } while (!(newPeriod > screenHeight * 1.1));
                }
                return periods;
            }
            return new List<int>();
        }

        public static bool IsMultiPeriodFringeAvailable(List<int> periods, int screenWidth, int screenHeight)
        {
            int ratio = periods.Count == 1 ? 1 : periods[1] / periods[0];
            var periodsWithRatio = CreatePeriodsListForRatio(periods[0], screenWidth, screenHeight, ratio);
            return periodsWithRatio.SequenceEqual(periods);
        }
    }
}
