using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Format.Metro;

namespace UnitySC.PM.ANA.Service.Measure.Shared
{
    // Interface implemented by all the Measures that provide Data Collections (DVIDs)
    public interface IMeasureDCProvider
    {
        // Retrieves a list of DCPointMeasureData according to a MeasureResultBase
        List<DCPointMeasureData> GetDCResultBase(MeasureResultBase measureResult, MeasureSettingsBase measureSettings);

        // Retrieves a list of DCPointMeasureData according to a MeasurePointResult
        List<DCPointMeasureData> GetDCResult(MeasurePointResult measurePointResult, MeasureSettingsBase measureSettings, int siteId, int? dieRow = null, int? dieCol = null);

        // Retrieves a list of DCData according to a MeasureResultBase and its stats
        List<DCData> GetDCWaferStatistics(MeasureResultBase measureResult, MeasureSettingsBase measureSettings);

        // Retrieves a list of DCDieStatistics according to a MeasureResultBase and its stats and dies
        List<DCDieStatistics> GetDCDiesStatistics(MeasureResultBase measureResult, MeasureSettingsBase measureSettings);
    }
}
