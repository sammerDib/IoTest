using System;
using System.Collections.Generic;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Tools;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Measure.AutofocusTracker
{
    public class MeasureAutofocusTracker
    {
        private List<FocusPosition> _topFocusPositons = new List<FocusPosition>();
        private List<FocusPosition> _bottomFocusPositons = new List<FocusPosition>();
        private readonly CalibrationManager _calibrationManager;
        private readonly AnaHardwareManager _hardwareManager;

        public MeasureAutofocusTracker()
        {
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
        }

        public void Reset()
        {
            _topFocusPositons.Clear();
            _bottomFocusPositons.Clear();
        }

        public void SaveAutofocusResult(AutoFocusSettings afSettings, XYZTopZBottomPosition measurePosition)
        {
            // Retrieve ObjectiveId used for Autofocus. We prioritize the camera Autofocus if both are used because the result is more accurate.
            string objectiveId = null;
            if ((afSettings.Type == AutoFocusType.Camera) || (afSettings.Type == AutoFocusType.LiseAndCamera))
            {
                objectiveId = (afSettings.ImageAutoFocusContext as TopImageAcquisitionContext)?.TopObjectiveContext?.ObjectiveId;
            }
            else if (afSettings.Type == AutoFocusType.Lise)
            {
                objectiveId = afSettings.LiseAutoFocusContext?.ObjectiveId;
            }

            if (objectiveId == null || measurePosition == null)
                return;

            var afResultContext = new FocusPosition(measurePosition);

            // Save focus position found
            var probe = HardwareUtils.GetProbeLiseFromID(_hardwareManager, afSettings.ProbeId);
            if (probe.Configuration.ModulePosition == ModulePositions.Up)
            {
                var objectiveCalibration = _calibrationManager.GetObjectiveCalibration(objectiveId);
                afResultContext.Position.ZTop += objectiveCalibration.ZOffsetWithMainObjective.Millimeters;
                _topFocusPositons.Add(afResultContext);
            }
            else if (probe.Configuration.ModulePosition == ModulePositions.Down)
            {
                var objectiveCalibration = _calibrationManager.GetObjectiveCalibration(objectiveId);
                afResultContext.Position.ZBottom += objectiveCalibration.ZOffsetWithMainObjective.Millimeters;
                _bottomFocusPositons.Add(afResultContext);
            }
        }

        public XYZTopZBottomPosition GetCorrectedAutofocusPosition(XYZTopZBottomPosition measurePosition, AutoFocusSettings afSettings)
        {
            // Retrieve ObjectiveId used for Autofocus. We prioritize the LISE Autofocus if both are used because it will be run first
            string objectiveId = null;
            if ((afSettings.Type == AutoFocusType.Lise) || (afSettings.Type == AutoFocusType.LiseAndCamera))
            {
                objectiveId = afSettings.LiseAutoFocusContext?.ObjectiveId;
            }
            else if (afSettings.Type == AutoFocusType.Camera)
            {
                objectiveId = (afSettings.ImageAutoFocusContext as TopImageAcquisitionContext)?.TopObjectiveContext?.ObjectiveId;
            }

            var probe = HardwareUtils.GetProbeLiseFromID(_hardwareManager, afSettings.ProbeId);
            var afResultContext = GetClosestAutofocus(measurePosition, probe);
            if (afResultContext == null || objectiveId == null)
                return null;

            var wantedObjectiveCalib = _calibrationManager.GetObjectiveCalibration(objectiveId);
            if (wantedObjectiveCalib == null)
                return null;

            // TODO? : For now, simply retrieve the closest saved point. We could also estimate the current position
            // according a reference plan computed from all saved point.
            var correctedPosition = new XYZTopZBottomPosition(measurePosition.Referential, measurePosition.X, measurePosition.Y, measurePosition.ZTop, measurePosition.ZBottom);
            if (probe.Configuration.ModulePosition == ModulePositions.Up)
            {
                correctedPosition.ZTop = afResultContext.Position.ZTop - wantedObjectiveCalib.ZOffsetWithMainObjective.Millimeters;
            }
            else if (probe.Configuration.ModulePosition == ModulePositions.Down)
            {
                correctedPosition.ZBottom = afResultContext.Position.ZBottom - wantedObjectiveCalib.ZOffsetWithMainObjective.Millimeters;
            }
            return correctedPosition;
        }

        private FocusPosition GetClosestAutofocus(XYZTopZBottomPosition measurePosition, IProbeLise probe)
        {
            List<FocusPosition> focusPositions = null;
            if (probe.Configuration.ModulePosition == ModulePositions.Up)
            {
                focusPositions = _topFocusPositons;
            }
            else if (probe.Configuration.ModulePosition == ModulePositions.Down)
            {
                focusPositions = _bottomFocusPositons;
            }

            if (measurePosition == null || focusPositions == null)
                return null;

            double minDistance = double.MaxValue;
            FocusPosition afResult = null;
            foreach (var afResultContext in focusPositions)
            {
                try
                {
                    var distance = MathTools.LineLength(measurePosition.ToXYPosition(), afResultContext.Position.ToXYPosition()).Millimeters;
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        afResult = afResultContext;
                    }
                }
                catch { /* Nothing to do : just continue */ }
            }
            return afResult;
        }
    }
}
