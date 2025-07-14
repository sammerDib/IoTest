using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.CalibFlow;
using UnitySC.PM.ANA.Service.Core.Dummy;
using UnitySC.PM.ANA.Service.Core.Referentials;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Measure.Shared;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.Format.Metro.Warp;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.MeasureCalibration
{
    public class ProbeCalibrationManagerLise : IProbeCalibrationManager
    {
        private bool FlowsAreSimulated => ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().FlowsAreSimulated;
        private readonly uint _minuteBetweenTwoDualLiseCalibration;
        private readonly AnaHardwareManager _hardwareManager;

        private string _probeId;
        private readonly List<CalibrationDualLiseFlowResult> _calibrations;
        private double _oldZTop;
        private double _oldZBottom;
        private bool _needLastCalibration;

        public CancellationToken CancellationToken { get; set; }

        public ProbeCalibrationManagerLise(string probeId, CancellationToken cancellationToken, uint minuteBetweenTwoDualLiseCalibration)
        {
            _probeId = probeId;
            _calibrations = new List<CalibrationDualLiseFlowResult>();
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            CancellationToken = cancellationToken;
            _oldZBottom = double.PositiveInfinity;
            _oldZTop = double.PositiveInfinity;
            _minuteBetweenTwoDualLiseCalibration = minuteBetweenTwoDualLiseCalibration;
        }

        // TODO this function should be private, it is public only for the tests but we probably should change the tests
        public (CalibrationDualLiseFlowResult, CalibrationDualLiseFlowResult) GetClosestsCalibrations(DateTime timestamp)
        {
            CalibrationDualLiseFlowResult firstCalibration = null;
            CalibrationDualLiseFlowResult secondCalibration = null;

            for (int i = 0; i < _calibrations.Count; i++)
            {
                if (timestamp >= _calibrations[i].Timestamp)
                {
                    firstCalibration = _calibrations[i];
                    if (i + 1 < _calibrations.Count)
                        secondCalibration = _calibrations[i + 1];
                }
            }

            return (firstCalibration, secondCalibration);
        }

        public IProbeCalibResult GetCalibration(bool createIfNeeded, string probeId, IProbeInputParams probeInputParams, PointPosition point, DieIndex die = null)
        {
            // TODO manage Create if needed

            // If a measure use a different probe, we expect that the ZPosition will be different
            if (!AreZPositionsEqual(point))
            {
                DoFirstCalibration(point, die); // First calibration for current measure
                _needLastCalibration = true;
                _oldZTop = point.ZTop;
                _oldZBottom = point.ZBottom;
            }

            if (IsCalibrationOutOfDate())
                DoCalibration();

            return _calibrations.Last().CalibResult;
        }

        public void DoLastCalibration()
        {
            // Same as DoCalibration(), used for readability
            DoCalibration();
        }

        private bool AreZPositionsEqual(PointPosition point)
        {
            var epsilon = 0.0005;  //mm
            return point.ZTop.Near(_oldZTop, epsilon) && point.ZBottom.Near(_oldZBottom, epsilon);
        }

        private void DoFirstCalibration(PointPosition point, DieIndex die)
        {
            DoCalibration();
        }

        private bool IsCalibrationOutOfDate()
        {
            return DateTime.UtcNow > GetLastCalibrationDate().AddMinutes(_minuteBetweenTwoDualLiseCalibration);
        }

        private DateTime GetLastCalibrationDate()
        {
            if (_calibrations.Count - 1 < 0)
                return default;

            return _calibrations[_calibrations.Count - 1].Timestamp;
        }

        private void DoCalibration()
        {
            ExecuteDualLiseCalibration();
        }

        private void ExecuteDualLiseCalibration()
        {
            string probeId = "ProbeLiseDouble";
            var probeConfig = HardwareUtils.GetProbeLiseFromID(_hardwareManager, probeId).Configuration;
            var config = probeConfig as ProbeDualLiseConfig;

            ProbeSample probeSample = new ProbeSample();
            var liseUpInput = new MeasureLiseInput(new ThicknessLiseInput(config.ProbeUpID, probeSample));
            var liseDownInput = new MeasureLiseInput(new ThicknessLiseInput(config.ProbeDownID, probeSample));

            GetProbeSampleCalibration(out double x, out double y, out var probeCalibration);

            var dualLiseCalibrationInput = new CalibrationDualLiseInput(probeConfig.DeviceID, liseUpInput, liseDownInput, probeCalibration, new XYPosition(new StageReferential(), x, y));

            var calibrationDualLiseFlow = FlowsAreSimulated ? new DualLiseCalibrationFlowDummy(dualLiseCalibrationInput) : new DualLiseCalibrationFlow(dualLiseCalibrationInput);
            calibrationDualLiseFlow.CancellationToken = CancellationToken;

            var calibrationResult = calibrationDualLiseFlow.Execute();
            if (calibrationResult.Status.State != FlowState.Success)
                throw new Exception("Dual Lise calibration failed.");

            _calibrations.Add(calibrationResult);
        }

        private void GetProbeSampleCalibration(out double x, out double y, out ProbeSample probeCalibration)
        {
            ANAChuckConfig chuckConfig = null;
            if (_hardwareManager.Chuck.Configuration is ANAChuckConfig anaChuckConfig)
                chuckConfig = anaChuckConfig;
            else
                throw new Exception("Dual Lise calibration failed. Bad chuck configuration.");
            var list = chuckConfig.ReferencesList;
            Length refThickness = 0.Micrometers();
            Length refTolerence = 0.Micrometers();
            x = 0.0;
            y = 0.0;
            double refRefractionIndex = 0.0;
            string name = "";
            //TODO Which reference shim should be used?? Information given by the user?? 100, 750, 1500, ou ref cam?
            //TODO Don't leave it hard
            foreach (var reference in list)
            {
                if (reference.ReferenceName == "REF 750UM-sori")
                {
                    name = reference.ReferenceName;
                    refThickness = reference.RefThickness;
                    refRefractionIndex = reference.RefRefrIndex;
                    refTolerence = reference.RefTolerance;
                    x = reference.PositionX.Millimeters;
                    y = reference.PositionY.Millimeters;
                    break;
                }
            }

            var probeSampleLayerCalibration = new ProbeSampleLayer()
            {
                Name = name,
                Thickness = refThickness,
                RefractionIndex = refRefractionIndex,
                Tolerance = new LengthTolerance(refTolerence.Micrometers, LengthToleranceUnit.Micrometer),
                IsMandatory = true,
            };
            var probeSampleList = new List<ProbeSampleLayer>() { probeSampleLayerCalibration };
            probeCalibration = new ProbeSample()
            {
                Name = "",
                Info = "",
                Layers = probeSampleList,
            };
        }

        public void ResetCalibrations()
        {
            _calibrations.Clear();
            _oldZTop = double.NaN;
            _oldZBottom = double.NaN;
            _needLastCalibration = false;
        }

        public void SetCalibration(string probeId, IProbeCalibResult probeCalibResult, IProbeInputParams probeInputParams, PointPosition point, DieIndex die = null)
        {
            throw new NotImplementedException();
        }

        public void RecipeExecutionStarted()
        {
            ResetCalibrations();
        }

        public void MeasureExecutionTerminated()
        {
            if (_calibrations.Count > 0)
                DoLastCalibration();
        }

        // Used to correct the measures at the end of the recipe execution
        public void CorrectMeasurePoint(MeasurePointDataResultBase measurePointDataResult)
        {
            if (measurePointDataResult is IMeasureAirGap pointDataWithAirGaps)
            {
                var (firstCalibration, secondCalibration) = GetClosestsCalibrations(measurePointDataResult.Timestamp);

                if (firstCalibration == null || secondCalibration == null)
                {
                    throw new Exception("Can not apply measure correction : at least one calibration is missing");
                }

                if (!firstCalibration.CalibResult.ZTopUsedForCalib.Near(secondCalibration.CalibResult.ZTopUsedForCalib, 0.001) || !firstCalibration.CalibResult.ZBottomUsedForCalib.Near(secondCalibration.CalibResult.ZBottomUsedForCalib, 0.001))
                {
                    throw new Exception("Can not apply measure correction : calibration done with different Z values");
                }

                // Apply airGap corrections
                if (pointDataWithAirGaps.AirGapUp != null)
                {
                    pointDataWithAirGaps.AirGapUp += MeasureCorrectionHelper.ComputeAirGapUpDelta(firstCalibration, secondCalibration, measurePointDataResult.Timestamp);
                }

                if (pointDataWithAirGaps.AirGapDown != null)
                {
                    pointDataWithAirGaps.AirGapDown += MeasureCorrectionHelper.ComputeAirGapDownDelta(firstCalibration, secondCalibration, measurePointDataResult.Timestamp);
                }
            }
        }
    }
}
