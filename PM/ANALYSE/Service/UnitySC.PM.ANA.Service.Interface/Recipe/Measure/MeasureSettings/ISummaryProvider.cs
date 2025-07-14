using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings
{
    public interface ISummaryProvider
    {
        List<string> GetProbesUsed();

        List<string> GetObjectivesUsed();

        List<string> GetLightsUsed();

        AutoFocusType? GetAutoFocusTypeUsed();

    }
}
