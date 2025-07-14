using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.EdgeTrim;
using UnitySC.Shared.Format.Metro.Step;
using UnitySC.Shared.Format.Metro.Trench;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosCppWrapper;

namespace UnitySC.PM.ANA.Service.Measure.Shared
{
    public class ProfileTrenchAnalyserResultToTrenchPointData : ProfileAnalyserResultToMeasureScanLine
    {
        private readonly TrenchSettings _settings;

        public ProfileTrenchAnalyserResultToTrenchPointData(TrenchSettings settings)
        {
            _settings = settings;
        }

        public TrenchPointData Convert(Profile2d profile, ProfileTrenchAnalyserResult result)
        {
            var trenchPointData = new TrenchPointData
            {
                RawProfileScan = ConvertProfile(profile, LengthUnit.Millimeter, LengthUnit.Micrometer)
            };

            trenchPointData.Depth = _settings.DepthCorrection.ApplyCorrection(
                new Length(result.GetDepth(), trenchPointData.RawProfileScan.ZUnit));
            trenchPointData.Width = _settings.WidthCorrection.ApplyCorrection(
                new Length(result.GetWidth(), trenchPointData.RawProfileScan.XUnit));

            var (state, message) = ConvertStatusToStateMessage(result.GetStatus(), trenchPointData.Depth, trenchPointData.Width);
            trenchPointData.State = state;
            trenchPointData.Message = message;

            return trenchPointData;
        }

        private (MeasureState, string) ConvertStatusToStateMessage(ProfileAnalyserResult.Status status, Length depth, Length width)
        {
            if (status == ProfileAnalyserResult.Status.Ok)
            {
                if (_settings.DepthTolerance.IsInTolerance(depth, _settings.DepthTarget) &&
                    (!_settings.IsWidthMeasured ||
                    _settings.WidthTolerance.IsInTolerance(width, _settings.WidthTarget)))
                {
                    return (MeasureState.Success, "Success");
                }
                else
                {
                    return (MeasureState.Error, "Measure is outside target +/- tolerance");
                }
            }
            else
            {
                return ConvertStatusToStateMessage(status);
            }
        }
    }
}
