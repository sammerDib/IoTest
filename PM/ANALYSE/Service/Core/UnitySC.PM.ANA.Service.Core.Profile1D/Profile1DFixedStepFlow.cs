using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Profile1D;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Tools;
using UnitySC.Shared.Tools;

using UnitySCSharedAlgosCppWrapper;

using static UnitySC.PM.ANA.Hardware.Probe.Lise.LiseSignalAcquisition;

namespace UnitySC.PM.ANA.Service.Core.Profile1D
{
    public class Profile1DFixedStepFlow : FlowComponent<Profile1DFixedStepInput, Profile1DFlowResult, Profile1DConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;

        private IProbeLise _probe;

        public Profile1DFixedStepFlow(Profile1DFixedStepInput input) : base(input, "Profile1DFixedStepFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
        }

        protected override void Process()
        {
            Result.Profile = new UnitySCSharedAlgosCppWrapper.Profile2d();
            _probe = HardwareUtils.GetProbeLiseFromID(_hardwareManager, Input.LiseData.ProbeID);

            double totalDistance = MathTools.LineLength(Input.StartPosition, Input.EndPosition).Millimeters;
            int nbPoint = (int) Math.Floor(1 + (totalDistance / Input.StepLength.Millimeters));

            double incrementX = (Input.EndPosition.X - Input.StartPosition.X) / (nbPoint - 1);
            double incrementY = (Input.EndPosition.Y - Input.StartPosition.Y) / (nbPoint - 1);

            var acquisitionParams = new LiseAcquisitionParams(Input.LiseData.Gain, Input.LiseData.NbAveraging);

            var probeLiseConfig = HardwareUtils.GetProbeLiseConfigFromID(_hardwareManager, Input.LiseData.ProbeID);
            var analysisParams = new LiseSignalAnalysisParams(probeLiseConfig.Lag, probeLiseConfig.DetectionCoef,
                probeLiseConfig.PeakInfluence);

            for (double iPoint=0; iPoint<nbPoint-1; ++iPoint)
            {
                CancellationToken.ThrowIfCancellationRequested();
                
                var nextPosition = new XYPosition{
                    Referential = Input.StartPosition.Referential,
                    X = Input.StartPosition.X + (iPoint * incrementX),
                    Y = Input.StartPosition.Y + (iPoint * incrementY),
                };
                _hardwareManager.Axes.GotoPosition(nextPosition, AxisSpeed.Fast);
                _hardwareManager.Axes.WaitMotionEnd(Configuration.MotionTimeout);

                if (AcquireAirGap(acquisitionParams, analysisParams) is double airgap)
                {
                    Result.Profile.Add(new Point2d(iPoint * Input.StepLength.Millimeters, airgap));
                }
            }

            _hardwareManager.Axes.GotoPosition(Input.EndPosition, AxisSpeed.Fast);
            _hardwareManager.Axes.WaitMotionEnd(Configuration.MotionTimeout);
            if (AcquireAirGap(acquisitionParams, analysisParams) is double lastAirgap)
            {
                Result.Profile.Add(new Point2d(totalDistance, lastAirgap));
            }

            ConvertAirGapsToHeight(Result.Profile);
        }

        private double? AcquireAirGap(LiseAcquisitionParams acquisitionParams, LiseSignalAnalysisParams analysisParams)
        {
            double airGap = LiseMeasurement.DoAirGap(_probe, acquisitionParams, analysisParams).AirGap.Micrometers;
            if (airGap == 0.0)
            {
                return null;
            }

            return airGap;
        }

        private void ConvertAirGapsToHeight(Profile2d airGaps)
        {
            double yMax = airGaps.Max(point => point.Y);
            foreach (var point in airGaps)
            {
                point.Y = -point.Y + yMax;
            }
        }
    }
}
