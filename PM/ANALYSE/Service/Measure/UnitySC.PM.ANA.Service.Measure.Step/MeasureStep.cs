using System;
using System.Linq;
using System.Threading;

using UnitySC.PM.ANA.Service.Core.Dummy;
using UnitySC.PM.ANA.Service.Core.Profile1D;
using UnitySC.PM.ANA.Service.Core.Step;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Profile1D;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Shared;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Step;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySCSharedAlgosCppWrapper;

namespace UnitySC.PM.ANA.Service.Measure.Step
{
    public class MeasureStep : MeasureBase<StepSettings, StepPointResult>
    {
        private readonly MeasureStepConfiguration _measureConfig;

        public MeasureStep() : base(ClassLocator.Default.GetInstance<ILogger<MeasureStep>>())
        {
            _measureConfig = MeasuresConfiguration?.Measures.OfType<MeasureStepConfiguration>().SingleOrDefault();
            if (_measureConfig is null)
            {
                throw new Exception("Step measure configuration is missing");
            }
        }

        protected override MeasurePointDataResultBase Process(StepSettings measureSettings,
            MeasureContext measureContext,
            CancellationToken cancelToken)
        {
            var flowResult = ExecuteFlow(measureSettings, cancelToken);
            return AnalyseProfile(flowResult.Profile, measureSettings);
        }

        private StepPointData AnalyseProfile(Profile2d profile, StepSettings settings)
        {
            var parameters = new ProfileStepAnalyserParameters(
                settings.StepKind == StepKind.Up ? 
                    ProfileStepAnalyserParameters.KindStep.Up : 
                    ProfileStepAnalyserParameters.KindStep.Down,
                settings.TargetHeight.Micrometers,
                settings.ToleranceHeight.GetAbsoluteTolerance(settings.TargetHeight).Micrometers
            );
            var algoResult = new ProfileStepAnalyser(parameters).Process(profile) as ProfileStepAnalyserResult;

            return new ProfileStepAnalyserResultToStepPointData(settings).Convert(profile, algoResult);
        }

        private Profile1DFlowResult ExecuteFlow(StepSettings measureSettings, CancellationToken cancelToken)
        {
            var startPosition = new XYPosition(measureSettings.Point.Referential)
            { 
                X = measureSettings.Point.X - Math.Cos(measureSettings.ScanOrientation.Radians) * measureSettings.ScanSize.Millimeters / 2.0, 
                Y = measureSettings.Point.Y - Math.Sin(measureSettings.ScanOrientation.Radians) * measureSettings.ScanSize.Millimeters / 2.0,
            };
            var endPosition = new XYPosition(measureSettings.Point.Referential)
            {
                X = measureSettings.Point.X + Math.Cos(measureSettings.ScanOrientation.Radians) * measureSettings.ScanSize.Millimeters / 2.0,
                Y = measureSettings.Point.Y + Math.Sin(measureSettings.ScanOrientation.Radians) * measureSettings.ScanSize.Millimeters / 2.0,
            };

            var input = new Profile1DInput
            {
                LiseData = new LiseInput(
                    probeId: measureSettings.ProbeId,
                    nbAveraging: _measureConfig.NbAveragingLise),
                Speed = measureSettings.Speed,
                StartPosition = startPosition,
                EndPosition = endPosition,
            };

            var flow = FlowsAreSimulated ? new Profile1DFlowDummy(input) : new Profile1DFlow(input)
            {
                CancellationToken = cancelToken
            };

            flow.Execute();
            return flow.Result;
        }


        protected override MeasureToolsBase GetMeasureToolsInternal(StepSettings measureSettings)
        {
            throw new System.NotImplementedException();
        }

        protected override MeasurePointDataResultBase CreateNotMeasuredPointData(StepSettings measureSettings, Exception ex)
        {
            return new StepPointData
            {
                State = MeasureState.NotMeasured,
                Message = ex.Message
            };
        }

        public override MeasureResultBase CreateMetroMeasureResult(MeasureSettingsBase measureSettings)
        {
            var result = new StepResult();
            return result;
        }

        public override MeasureType MeasureType => MeasureType.Step;
    }
}
