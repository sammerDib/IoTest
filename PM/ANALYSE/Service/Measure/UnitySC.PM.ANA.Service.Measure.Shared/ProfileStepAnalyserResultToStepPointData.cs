using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Step;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosCppWrapper;

namespace UnitySC.PM.ANA.Service.Measure.Shared
{
    public class ProfileStepAnalyserResultToStepPointData : ProfileAnalyserResultToMeasureScanLine
    {
        private readonly StepSettings _settings;

        public ProfileStepAnalyserResultToStepPointData(StepSettings settings) 
        { 
            _settings = settings;
        }

        public StepPointData Convert(Profile2d profile, ProfileStepAnalyserResult result)
        {
            var stepPointData = new StepPointData();

            stepPointData.RawProfileScan = ConvertProfile(profile, _settings.ScanSize.Unit, LengthUnit.Micrometer);

            stepPointData.StepHeight = new Length(result.GetStepHeight(), stepPointData.RawProfileScan.ZUnit);

            var (state, message) = ConvertStatusToStateMessage(result.GetStatus(), stepPointData.StepHeight);
            stepPointData.State = state;
            stepPointData.Message = message;

            return stepPointData;
        }

        private Tuple<MeasureState, string> ConvertStatusToStateMessage(ProfileAnalyserResult.Status status, Length height)
        {
            if(status == ProfileAnalyserResult.Status.Ok)
            {
                MeasureState state;
                if (_settings.ToleranceHeight.IsInTolerance(height, _settings.TargetHeight))
                {
                    state = MeasureState.Success;
                }
                else
                {
                    state = MeasureState.Error;
                }
                return Tuple.Create(state, Properties.Resources.ResourceManager.GetString(state.ToString()));
            }
            else
            {
                return Tuple.Create(MeasureState.NotMeasured, Properties.Resources.ResourceManager.GetString(status.ToString()));
            }
        }
    }
}
