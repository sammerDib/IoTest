using System;
using System.IO;
using System.Threading;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.DistanceSensor;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;

namespace UnitySC.PM.EME.Service.Core.Flows.AutoFocus
{
    public class GetZFocusFlow : FlowComponent<GetZFocusInput, GetZFocusResult, GetZFocusConfiguration>
    {
        private readonly PhotoLumAxes _motionAxes;
        private readonly DistanceSensorBase _hardwareManagerDistanceSensor;
        private readonly IReferentialManager _referentialManager;

        public GetZFocusFlow(GetZFocusInput input) : base(input, "GetZFocusFlow")
        {
            _referentialManager = ClassLocator.Default.GetInstance<IReferentialManager>();
            var hardwareManager = ClassLocator.Default.GetInstance<EmeHardwareManager>();
            if (hardwareManager.MotionAxes is PhotoLumAxes motionAxes)
            {
                _motionAxes = motionAxes;
            }
            else
            {
                throw new Exception("MotionAxes should be PhotoLumAxes");
            }

            _hardwareManagerDistanceSensor = hardwareManager.DistanceSensor;
        }

        private GetZFocusConfiguration _configuration;
        public new GetZFocusConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                    _configuration = base.Configuration;
                return _configuration;
            }
            set
            {
                _configuration = value;
            }
        }

        protected override void Process()
        {                      
            double currentZPosition = Configuration.StartZScan;
            var initialPosition = _motionAxes.GetPosition().ToXYZPosition();
            try
            {      
                var stabilizationTime = 200;
                var report = new GetZFocusReport();
                MoveToZPosition(currentZPosition, true, stabilizationTime);
                double currentDistance = _hardwareManagerDistanceSensor.GetDistanceSensorHeight();
                report.AddToReport(currentZPosition, currentDistance);

                double zMin = Configuration.MinZScan;
                double zMax = Configuration.MaxZScan;

                for (int i = 0; i < Configuration.MaximumIterations && !IsInFocus(currentDistance); i++)
                {
                    CheckCancellation();
                    if (!IsDistanceInAllowedRange(currentDistance))
                    {
                        zMin = currentZPosition;
                        currentZPosition = (currentZPosition + zMax) / 2;
                        MoveToZPosition(currentZPosition, true, stabilizationTime);
                        currentDistance = _hardwareManagerDistanceSensor.GetDistanceSensorHeight();
                        report.AddToReport(currentZPosition, currentDistance);
                    }
                    else
                    {
                        double newZPosition = currentDistance < Input.TargetDistanceSensor
                            ? (currentZPosition + zMin) / 2
                            : (currentZPosition + zMax) / 2;
                        MoveToZPosition(newZPosition, true, stabilizationTime);
                        double newDistance = _hardwareManagerDistanceSensor.GetDistanceSensorHeight();
                        report.AddToReport(currentZPosition, currentDistance);

                        currentZPosition = EstimateZFocus(currentZPosition, currentDistance, newZPosition, newDistance);
                        currentZPosition = Math.Min(currentZPosition, zMax);
                        currentZPosition = Math.Max(currentZPosition, zMin);

                        MoveToZPosition(currentZPosition, true, stabilizationTime);
                        currentDistance = _hardwareManagerDistanceSensor.GetDistanceSensorHeight();
                        report.AddToReport(currentZPosition, currentDistance);
                    }
                }

                if (Configuration.IsAnyReportEnabled())
                {
                    report.Save(Path.Combine(ReportFolder, "measurements.csv"));
                }

                if (!IsInFocus(currentDistance))
                {
                    throw new InvalidOperationException(
                        $"Failed to reach the target height within {Configuration.MaximumIterations} attempts.");
                }
                initialPosition.Z = currentZPosition;
            }
            catch (Exception ex)
            {
                Logger.Error($"{LogHeader} Error during the Get ZFocus Flow : {ex.Message}");
                throw;
            }
            finally
            {
                Result = GetZFocusResult.Success(currentZPosition);
                _referentialManager.SetSettings(new StageReferentialSettings() { EnableDistanceSensorOffset = false });
                _motionAxes.GoToPosition(initialPosition);
            }
        }

        private bool IsInFocus(double currentDistance)
        {
            return Configuration.Tolerance.IsInTolerance(currentDistance, Input.TargetDistanceSensor);
        }

        private bool IsDistanceInAllowedRange(double distance)
        {
            var tolerance = new Tolerance(10, ToleranceUnit.Percentage);
            return !tolerance.IsInTolerance(distance, Configuration.MaximumDistance);
        }

        private void MoveToZPosition(double zPosition, bool applyDistanceSensorOffset, int stabilizationTime = 0)
        {
            var xyPosition = _motionAxes.GetPosition().ToXYPosition();
            var newPosition = new XYZPosition(xyPosition.Referential, xyPosition.X, xyPosition.Y, zPosition);
            newPosition = (XYZPosition)_referentialManager.ConvertTo(newPosition, ReferentialTag.Stage);            
            _referentialManager.SetSettings(new StageReferentialSettings() { EnableDistanceSensorOffset = applyDistanceSensorOffset });
            _motionAxes.GoToPosition(newPosition);
            _motionAxes.WaitMotionEnd(3000);
            if (stabilizationTime != 0)
            {
                Thread.Sleep(stabilizationTime);
            }
            CheckCancellation();
        }

        private double EstimateZFocus(double z1, double d1, double z2, double d2)
        {
            double targetDistance = Input.TargetDistanceSensor;
            double zFocus = (targetDistance - d1) / (d2 - d1) * (z2 - z1) + z1;
            return zFocus;
        }
    }
}
