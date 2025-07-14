using System;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.PatternRec;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Tools;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.WaferMap
{
    public class DieAndStreetSizesFlow : FlowComponent<DieAndStreetSizesInput, DieAndStreetSizesResult, DieAndStreetSizesConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly PatternRecFlow _patternRecFlow;

        public DieAndStreetSizesFlow(DieAndStreetSizesInput input, PatternRecFlow patternRec = null) : base(input, "DieAndStreetSizesFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _patternRecFlow = patternRec ?? new PatternRecFlow(new PatternRecInput());
        }

        protected override void Process()
        {
            var patternRecognitionParams = new PatternRecognitionParams(
                patternRecFlow: _patternRecFlow,
                topLeftCorner: Input.TopLeftCorner,
                autofocusSettings: Input.AutoFocusSettings);

            // Find a first approximation of dies dimensions thanks to the neighboring dies
            var approximateWidth = FindApproximateDieWidth(patternRecognitionParams);
            var approximateHeight = FindApproximateDieHeight(patternRecognitionParams);
            var approximateRotation = WaferMapUtils.FindApproximateDieAngle(patternRecognitionParams, approximateWidth.Pitch);

            Logger.Debug($"{LogHeader} Approximate die size : ( width =  {approximateWidth.Die.Millimeters} mm ; height =  {approximateHeight.Die.Millimeters} mm ).");
            Logger.Debug($"{LogHeader} Approximate street size : ( width =  {approximateWidth.Street.Millimeters} mm ; height =  {approximateHeight.Street.Millimeters} mm ).");
            Logger.Debug($"{LogHeader} Approximated rotation angle: {approximateRotation.Angle.Degrees} degrees.");

            if (approximateWidth.Die.Millimeters == 0 || approximateHeight.Die.Millimeters == 0)
            {
                throw new Exception("Die height and die width must not be zero, check the bottom right corner position.");
            }

            // Specifies the dimensions of the dies thanks to the distant dies
            var outerRadiusExclusionLength = Input.EdgeExclusion + Configuration.AdditionalEdgeExclusion;
            int usableDieNumberAtRight = WaferMapUtils.FindUsableDieNumberAtRight(Input.TopLeftCorner.Position.ToXYPosition(), approximateWidth.Pitch, approximateHeight.Pitch, approximateRotation.Angle, Input.Wafer, outerRadiusExclusionLength);
            int usableDieNumberAtBottom = WaferMapUtils.FindUsableDieNumberAtBottom(Input.TopLeftCorner.Position.ToXYPosition(), approximateWidth.Pitch, approximateHeight.Pitch, approximateRotation.Angle, Input.Wafer, outerRadiusExclusionLength);
            if (usableDieNumberAtRight <= 1 || usableDieNumberAtBottom <= 1)
            {
                Logger.Debug($"{LogHeader} Die size : ( width =  {approximateWidth.Die.Millimeters} mm ; height =  {approximateHeight.Die.Millimeters} mm ).");
                Logger.Debug($"{LogHeader} Street size : ( width =  {approximateWidth.Street.Millimeters} mm ; height =  {approximateHeight.Street.Millimeters} mm ).");

                Result.Confidence = (approximateWidth.Confidence + approximateRotation.Confidence + approximateHeight.Confidence) / 3;
                Result.DieDimensions = new DieDimensionalCharacteristic(dieWidth: approximateWidth.Die, dieHeight: approximateHeight.Die, streetWidth: approximateWidth.Street, streetHeight: approximateHeight.Street, dieAngle: approximateRotation.Angle);
                return;
            }

            var lastDieOnXPositionData = WaferMapUtils.FindTopLeftCornerOfDieOnSameLine(patternRecognitionParams, usableDieNumberAtRight, approximateWidth.Pitch, approximateRotation.Angle);
            var lastDieOnXPosition = lastDieOnXPositionData.Item1;
            double lastDieOnXPositionConfidence = lastDieOnXPositionData.Item2;
            Logger.Information($"{LogHeader} Position of the upper left corner of the last die on the x axis : ( x= {lastDieOnXPosition.X} mm ; y= {lastDieOnXPosition.Y} mm ; confidence= {lastDieOnXPositionConfidence}.");

            var lastDieOnYPositionData = WaferMapUtils.FindTopLeftCornerOfDieOnSameColumn(patternRecognitionParams, -usableDieNumberAtBottom, approximateHeight.Pitch, approximateRotation.Angle);
            var lastDieOnYPosition = lastDieOnYPositionData.Item1;
            double lastDieOnYPositionConfidence = lastDieOnYPositionData.Item2;
            Logger.Information($"{LogHeader} Position of the upper left corner of the last die on the y axis : ( x= {lastDieOnYPosition.X} mm ; y= {lastDieOnYPosition.Y} mm ; confidence= {lastDieOnYPositionConfidence}.");

            var dieHorizontalPitch = (Math.Sqrt(Math.Pow(Input.TopLeftCorner.Position.X - lastDieOnXPosition.X, 2) + Math.Pow(Input.TopLeftCorner.Position.Y - lastDieOnXPosition.Y, 2)) / usableDieNumberAtRight).Millimeters();
            var dieVerticalPitch = (Math.Sqrt(Math.Pow(Input.TopLeftCorner.Position.X - lastDieOnYPosition.X, 2) + Math.Pow(Input.TopLeftCorner.Position.Y - lastDieOnYPosition.Y, 2)) / usableDieNumberAtBottom).Millimeters();
            var dieWidth = dieHorizontalPitch - approximateWidth.Street;
            var dieHeight = dieVerticalPitch - approximateHeight.Street;

            Logger.Debug($"{LogHeader} Die size : ( width =  {dieWidth.Millimeters} mm ; height =  {dieHeight.Millimeters} mm ).");
            Logger.Debug($"{LogHeader} Street size : ( width =  {approximateWidth.Street.Millimeters} mm ; height =  {approximateHeight.Street.Millimeters} mm ).");

            var dieGridAngle = MathTools.ComputeAngleFromTwoPositions(Input.TopLeftCorner.Position.ToXYPosition(), lastDieOnXPosition);
            Logger.Information($"{LogHeader} Calculated rotation angle: {dieGridAngle} radian.");

            Result.Confidence = (approximateWidth.Confidence + approximateHeight.Confidence + approximateRotation.Confidence + lastDieOnXPositionConfidence + lastDieOnYPositionConfidence) / 5;
            Result.DieDimensions = new DieDimensionalCharacteristic(dieWidth: dieWidth, dieHeight: dieHeight, streetWidth: approximateWidth.Street, streetHeight: approximateHeight.Street, dieAngle: dieGridAngle);
        }

        private DieAndStreetLength FindApproximateDieWidth(PatternRecognitionParams patternRecognitionParams)
        {
            try
            {
                var approximateDieWidth = Math.Abs(Input.BottomRightCorner.Position.X - Input.TopLeftCorner.Position.X).Millimeters();
                var cameraImageWidth = HardwareUtils.GetCameraFieldOfViewWidth(_hardwareManager, Input.CameraID);
                var overlap = Configuration.OverlapForNextDieResearch;

                var topLeftCornerOfDieAtRightData = WaferMapUtils.FindTopLeftCornerOfDieAtRight(patternRecognitionParams, approximateDieWidth, cameraImageWidth, overlap);
                var topLeftCornerOfDieAtRightPos = topLeftCornerOfDieAtRightData.Item1;
                var topLeftCornerOfDieAtRightConfidence = topLeftCornerOfDieAtRightData.Item2;

                var topRightCornerOfRefDie = MathTools.OrthogonalProjectionOfPointOntoLine(Input.TopLeftCorner.Position.ToXYPosition(), topLeftCornerOfDieAtRightPos, Input.BottomRightCorner.Position.ToXYPosition());

                Length die = MathTools.LineLength(Input.TopLeftCorner.Position, topRightCornerOfRefDie);
                Length streetSize = 0.Millimeters();
                if (!IsStreetSizeWidthNegative(topRightCornerOfRefDie, topLeftCornerOfDieAtRightPos))
                {
                    streetSize = MathTools.LineLength(topLeftCornerOfDieAtRightPos, topRightCornerOfRefDie);
                }

                var dieAndStreetWidth = new DieAndStreetLength(
                    die: die,
                    street: streetSize,
                    confidence: topLeftCornerOfDieAtRightConfidence);

                return dieAndStreetWidth;
            }
            catch
            {
                throw new Exception("Unable to estimate the die width because the top left corner of die on the right can't be found.");
            }
        }

        private DieAndStreetLength FindApproximateDieHeight(PatternRecognitionParams patternRecognitionParams)
        {
            try
            {
                var approximateDieHeight = Math.Abs(Input.BottomRightCorner.Position.Y - Input.TopLeftCorner.Position.Y).Millimeters();
                var cameraImageHeight = HardwareUtils.GetCameraFieldOfViewHeight(_hardwareManager, Input.CameraID);
                var overlap = Configuration.OverlapForNextDieResearch;

                var topLeftCornerOfDieAtBottomData = WaferMapUtils.FindTopLeftCornerOfDieAtBottom(patternRecognitionParams, approximateDieHeight, cameraImageHeight, overlap);
                var topLeftCornerOfDieAtBottomPos = topLeftCornerOfDieAtBottomData.Item1;
                var topLeftCornerOfDieAtBottomConfidence = topLeftCornerOfDieAtBottomData.Item2;

                var bottomLeftCornerOfRefDie = MathTools.OrthogonalProjectionOfPointOntoLine(Input.TopLeftCorner.Position.ToXYPosition(), topLeftCornerOfDieAtBottomPos, Input.BottomRightCorner.Position.ToXYPosition());

                Length die = MathTools.LineLength(Input.TopLeftCorner.Position, bottomLeftCornerOfRefDie);
                Length streetSize = 0.Millimeters();
                if (!IsStreetSizeHeightNegative(bottomLeftCornerOfRefDie, topLeftCornerOfDieAtBottomPos))
                {
                    streetSize = MathTools.LineLength(topLeftCornerOfDieAtBottomPos, bottomLeftCornerOfRefDie);
                }

                var dieAndStreetHeight = new DieAndStreetLength(
                    die: die,
                    street: streetSize,
                    confidence: topLeftCornerOfDieAtBottomConfidence);

                return dieAndStreetHeight;
            }
            catch
            {
                throw new Exception("Unable to estimate the die height because the top left corner of die below can't be found.");
            }
        }

        private bool IsStreetSizeWidthNegative(XYPosition topRightCornerOfRefDie, XYPosition topLeftCornerOfDieAtRightPos)
        {
            return topRightCornerOfRefDie.X > topLeftCornerOfDieAtRightPos.X;
        }

        private bool IsStreetSizeHeightNegative(XYPosition bottomLeftCornerOfRefDie, XYPosition topLeftCornerOfDieAtBottomPos)
        {
            return bottomLeftCornerOfRefDie.Y < topLeftCornerOfDieAtBottomPos.Y;
        }
    }
}
