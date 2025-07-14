using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.EdgeTrim;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosCppWrapper;

namespace UnitySC.PM.ANA.Service.Measure.Shared
{
    public class ProfileEdgeTrimAnalyserResultToEdgeTrimPointData : ProfileAnalyserResultToMeasureScanLine
    {
        private readonly EdgeTrimSettings _settings;

        public ProfileEdgeTrimAnalyserResultToEdgeTrimPointData(EdgeTrimSettings settings)
        {
            _settings = settings;
        }

        public EdgeTrimPointData Convert(Profile2d profile, ProfileEdgeTrimAnalyserResult result)
        {
            var edgeTrimPointData = new EdgeTrimPointData
            {
                RawProfileScan = ConvertProfile(profile, LengthUnit.Millimeter, LengthUnit.Micrometer)
            };

            edgeTrimPointData.Height = _settings.HeightCorrection.ApplyCorrection(
                new Length(result.GetStepHeight(), edgeTrimPointData.RawProfileScan.ZUnit));
            edgeTrimPointData.Width = _settings.WidthCorrection.ApplyCorrection(
                new Length(result.GetWidth(), edgeTrimPointData.RawProfileScan.XUnit));

            var (state, message) = ConvertStatusToStateMessage(result.GetStatus(), edgeTrimPointData.Height, edgeTrimPointData.Width);
            edgeTrimPointData.State = state;
            edgeTrimPointData.Message = message;

            return edgeTrimPointData;
        }

        private (MeasureState, string) ConvertStatusToStateMessage(ProfileAnalyserResult.Status status, Length height, Length width)
        {
            if (status == ProfileAnalyserResult.Status.Ok)
            {
                if (_settings.HeightTolerance.IsInTolerance(height, _settings.HeightTarget) && 
                    (!_settings.IsWidthMeasured ||
                    _settings.WidthTolerance.IsInTolerance(width, _settings.WidthTarget)))
                {
                    return (MeasureState.Success, null);
                }
                else
                {
                    return (MeasureState.Error, null);
                }
            }
            else
            {
                return ConvertStatusToStateMessage(status);
            }
        }
    }
}
