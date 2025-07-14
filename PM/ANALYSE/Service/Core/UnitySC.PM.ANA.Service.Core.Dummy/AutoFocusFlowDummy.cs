using System;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Autofocus;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class AutofocusFlowDummy : AutofocusFlow
    {
        private readonly AnaHardwareManager _hardwareManager;
        private XYPositionContext _positionContext;
        private AutoFocusSettings _settings;

        public AutofocusFlowDummy(AutofocusInput input) : base(input)
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _settings = Input.Settings;
        }

        protected override void Process()
        {
            _positionContext = GetPositionContext();

            if ((_settings.Type == AutoFocusType.Lise) || (_settings.Type == AutoFocusType.LiseAndCamera))
            {
                CheckCancellation();
                AutofocusLise();
            }

            if ((_settings.Type == AutoFocusType.Camera) || (_settings.Type == AutoFocusType.LiseAndCamera))
            {
                CheckCancellation();
                AutofocusCamera();
            }

            // Go back to initial position if needed
            var newPositionContext = GetPositionContext();
            if (!_positionContext.Position.Near(newPositionContext.Position))
            {
                _hardwareManager.Axes.GotoPosition(_positionContext.Position, AxisSpeed.Normal);
            }

            Logger.Information($"{LogHeader} Autofocus was successful at Z position {Result.ZPosition} mm.");
        }

        private XYPositionContext GetPositionContext()
        {
            var afPosition = HardwareUtils.GetAxesPosition(_hardwareManager.Axes).ToXYZTopZBottomPosition();
            var afPositionXY = new XYPosition(afPosition.Referential, afPosition.X, afPosition.Y);
            return new XYPositionContext(afPositionXY);
        }

        private void AutofocusLise()
        {
            var positionContextWithOffset = new XYPositionContext((XYPosition)_positionContext.Position.Clone());

            positionContextWithOffset.Position.X += _settings.LiseOffsetX?.Millimeters ?? 0;
            positionContextWithOffset.Position.Y += _settings.LiseOffsetY?.Millimeters ?? 0;


            var afLiseInitialContext = new ContextsList(_settings.LiseAutoFocusContext, positionContextWithOffset);
            var afLiseInput = new AFLiseInput(afLiseInitialContext, _settings.ProbeId, _settings.LiseGain, _settings.LiseScanRange);
            var afLiseFlow = new AFLiseFlowDummy(afLiseInput);
            afLiseFlow.CancellationToken = CancellationToken;

            var result = afLiseFlow.Execute();
            if (IsResultStateError(result, afLiseFlow.Name))
                throw new Exception("AF LISE Failure");

            Result.ZPosition = result.ZPosition;
            Result.QualityScore = result.QualityScore;
        }

        private void AutofocusCamera()
        {
            var positionContextWithOffset = new XYPositionContext((XYPosition)_positionContext.Position.Clone());

            if (_settings.Type == AutoFocusType.LiseAndCamera)
            {
                positionContextWithOffset.Position.X += _settings.LiseOffsetX?.Millimeters ?? 0;
                positionContextWithOffset.Position.Y += _settings.LiseOffsetY?.Millimeters ?? 0;
            }
            var AFCameraInitialContext = new ContextsList(_settings.ImageAutoFocusContext, positionContextWithOffset);
            var afCameraInput = new AFCameraInput(AFCameraInitialContext, _settings.CameraId, _settings.CameraScanRange, _settings.CameraScanRangeConfigured);
            afCameraInput.UseCurrentZPosition = _settings.UseCurrentZPosition;

            var afV2CameraFlow = new AFV2CameraFlowDummy(afCameraInput);
            afV2CameraFlow.Configuration.MeasureNbToQualityScore = Configuration.AFCamera.MeasureNbToQualityScore;
            afV2CameraFlow.CancellationToken = CancellationToken;

            var result = afV2CameraFlow.Execute();
            if (IsResultStateError(result, afV2CameraFlow.Name))
                throw new Exception("AFV2 Camera Failure");

            Result.ZPosition = result.ZPosition;
            Result.ResultImage = result.ResultImage;
            Result.QualityScore = result.QualityScore;
        }
    }
}
