using System;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.AutofocusV2;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.ANA.Service.Core.Autofocus
{
    /// <summary>
    /// Compute the Z position for an objective to be on focus.
    /// Current objective of given lise probe is involved here.
    /// </summary>
    public class AutofocusFlow : FlowComponent<AutofocusInput, AutofocusResult, AutofocusConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private AutoFocusSettings _settings;
        private Shared.ImageOperators _imageOperator;

        public AutofocusFlow(AutofocusInput input) : base(input, "AutoFocusFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
        }

        protected override void Process()
        {
            _settings = Input.Settings;

            if ((_settings.Type == AutoFocusType.Lise) || (_settings.Type == AutoFocusType.LiseAndCamera))
            {
                CheckCancellation();
                AutofocusLise();
            }

            if ((_settings.Type == AutoFocusType.Camera) || (_settings.Type == AutoFocusType.LiseAndCamera))
            {
                // TODO : Remove this line when AFCamera for bottom camera is implemented
                CheckIfCameraIsUsable();
                CheckCancellation();
                AutofocusCamera();
            }

            Logger.Information($"{LogHeader} Autofocus was successful at Z position {Result.ZPosition} mm.");
        }

        private void AutofocusLise()
        {
            double LiseOffestX_mm = _settings.LiseOffsetX?.Millimeters ?? 0.0;
            double LiseOffestY_mm = _settings.LiseOffsetY?.Millimeters ?? 0.0;
            bool HasLiseOffset = (LiseOffestX_mm != 0.0) || (LiseOffestY_mm != 0.0);

            // move to lise offset position
            if (HasLiseOffset)
            {
                HardwareUtils.MoveIncremental(_hardwareManager.Axes, new XYZTopZBottomMove(LiseOffestX_mm, LiseOffestY_mm, double.NaN, double.NaN));
            }

            var afLiseInitialContext = new ContextsList(_settings.LiseAutoFocusContext);
            var afLiseInput = new AFLiseInput(afLiseInitialContext, _settings.ProbeId, _settings.LiseGain, _settings.LiseScanRange);
            var afLiseFlow = new AFLiseFlow(afLiseInput);
            afLiseFlow.CancellationToken = CancellationToken;

            var result = afLiseFlow.Execute();
            if (IsResultStateError(result, afLiseFlow.Name))
                throw new Exception("AF LISE Failure");

            Result.ZPosition = result.ZPosition;
            Result.QualityScore = result.QualityScore;

            // restore initial position
            if (HasLiseOffset)
            {
                HardwareUtils.MoveIncremental(_hardwareManager.Axes, new XYZTopZBottomMove(-LiseOffestX_mm, -LiseOffestY_mm, double.NaN, double.NaN));
            }

        }

        private void AutofocusCamera()
        {
            var AFCameraInitialContext = new ContextsList(_settings.ImageAutoFocusContext);
            var afCameraInput = new AFCameraInput(AFCameraInitialContext, _settings.CameraId, _settings.CameraScanRange, _settings.CameraScanRangeConfigured);
            afCameraInput.UseCurrentZPosition = _settings.UseCurrentZPosition;

            if (_settings.AutofocusModifiedLaplacien)
                _imageOperator = new Shared.ImageOperators(FocusMeasureMethod.SumOfModifiedLaplacian);

            var afV2CameraFlow = new AFV2CameraFlow(afCameraInput, null, null, null, _imageOperator);
            afV2CameraFlow.Configuration.MeasureNbToQualityScore = Configuration.AFCamera.MeasureNbToQualityScore;
            afV2CameraFlow.CancellationToken = CancellationToken;

            var result = afV2CameraFlow.Execute();
            if (IsResultStateError(result, afV2CameraFlow.Name))
                throw new Exception("AFV2 Camera Failure");

            Result.ZPosition = result.ZPosition;
            Result.ResultImage = result.ResultImage;
            Result.QualityScore = result.QualityScore;
        }

        private void CheckIfCameraIsUsable()
        {
            var cameraPosition = _hardwareManager.Cameras[_settings.CameraId].Config.ModulePosition;
            if (cameraPosition != ModulePositions.Up)
            {
                throw new NotImplementedException("Autofocus Camera works only for Top camera for now.");
            }
        }
    }
}
