using System;

using UnitySC.PM.ANA.Service.Core.PatternRec;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Tools;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Geometry;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.WaferMap
{
    public struct DieAndStreetLength
    {
        public DieAndStreetLength(Length die, Length street, double confidence)
        {
            Die = die;
            Street = street;
            Confidence = confidence;
        }

        public Length Die;
        public Length Street;
        public double Confidence;

        public Length Pitch
        {
            get
            {
                return Die + Street;
            }
        }
    }

    public struct RotationValue
    {
        public RotationValue(Angle angle, double confidence)
        {
            Angle = angle;
            Confidence = confidence;
        }

        public Angle Angle;
        public double Confidence;
    }

    public struct PatternRecognitionParams
    {
        public PatternRecognitionParams(PatternRecFlow patternRecFlow, PositionWithPatternRec topLeftCorner, AutoFocusSettings autofocusSettings)
        {
            PatternRecFlow = patternRecFlow;
            TopLeftCorner = topLeftCorner;
            AutoFocusSettings = autofocusSettings;
        }

        public PatternRecFlow PatternRecFlow;
        public PositionWithPatternRec TopLeftCorner;
        public AutoFocusSettings AutoFocusSettings;
    }

    public static class WaferMapUtils
    {
        public static RotationValue FindApproximateDieAngle(PatternRecognitionParams patternRecParams, Length dieAndPitchWidth)
        {
            try
            {
                int dieNbToMove = 1;

                XYPosition approximateTopLeftCornerOfDieOnRight = new XYPosition(
                    new WaferReferential(),
                    patternRecParams.TopLeftCorner.Position.X + dieNbToMove * dieAndPitchWidth.Millimeters,
                    patternRecParams.TopLeftCorner.Position.Y);

                patternRecParams.PatternRecFlow.Input = patternRecParams.TopLeftCorner.ConvertToPatternRecInputForOtherPosition(approximateTopLeftCornerOfDieOnRight, patternRecParams.AutoFocusSettings); ;
                var patternRecResultAtDieOnRight = ComputePatternRec(patternRecParams.PatternRecFlow);

                XYPosition topLeftCornerOfDieOnRight = new XYPosition(
                    new WaferReferential(),
                    approximateTopLeftCornerOfDieOnRight.X + patternRecResultAtDieOnRight.ShiftX.Millimeters,
                    approximateTopLeftCornerOfDieOnRight.Y + patternRecResultAtDieOnRight.ShiftY.Millimeters);

                var angle = MathTools.ComputeAngleFromTwoPositions(patternRecParams.TopLeftCorner.Position.ToXYPosition(), topLeftCornerOfDieOnRight);

                return new RotationValue(angle, patternRecResultAtDieOnRight.Confidence);
            }
            catch
            {
                throw new Exception("Unable to estimate die angle");
            }
        }

        public static Tuple<XYPosition, double> FindTopLeftCornerOfDieAtRight(PatternRecognitionParams patternRecParams, Length approximateDieWidth, Length cameraImageWidth, double overlap)
        {
            var stepSizeX = cameraImageWidth.Millimeters * (1 - overlap);
            int nbStep = 0;

            var approximateTopRightCornerOfRefDie = new XYPosition(
                new WaferReferential(),
                patternRecParams.TopLeftCorner.Position.X + approximateDieWidth.Millimeters,
                patternRecParams.TopLeftCorner.Position.Y);
            var researchPosition = (XYPosition)approximateTopRightCornerOfRefDie.Clone();

            double halfDieWidth = approximateDieWidth.Millimeters / 2;
            double maxPosistionX = approximateTopRightCornerOfRefDie.X + halfDieWidth;

            while (researchPosition.X < maxPosistionX)
            {
                try
                {
                    patternRecParams.PatternRecFlow.Input = patternRecParams.TopLeftCorner.ConvertToPatternRecInputForOtherPosition(researchPosition, patternRecParams.AutoFocusSettings);
                    var patternRecResultAtTopRight = ComputePatternRec(patternRecParams.PatternRecFlow);
                    var topLeftCornerOfDieAtRight = new XYPosition(
                        new WaferReferential(),
                        approximateTopRightCornerOfRefDie.X + patternRecResultAtTopRight.ShiftX.Millimeters + nbStep * stepSizeX,
                        approximateTopRightCornerOfRefDie.Y + patternRecResultAtTopRight.ShiftY.Millimeters);

                    return new Tuple<XYPosition, double>(topLeftCornerOfDieAtRight, patternRecResultAtTopRight.Confidence);
                }
                catch
                {
                    researchPosition.X += stepSizeX;
                    nbStep++;
                }
            }
            throw new Exception("Top left corner of die on the right not found.");
        }

        public static Tuple<XYPosition, double> FindTopLeftCornerOfDieAtBottom(PatternRecognitionParams patternRecParams, Length approximateDieHeight, Length cameraImageHeight, double overlap)
        {
            var stepSizeY = cameraImageHeight.Millimeters * (1 - overlap);
            int nbStep = 0;

            var approximateBottomLeftCornerOfRefDie = new XYPosition(
               new WaferReferential(),
               patternRecParams.TopLeftCorner.Position.X,
               patternRecParams.TopLeftCorner.Position.Y - approximateDieHeight.Millimeters);
            var researchPosition = (XYPosition)approximateBottomLeftCornerOfRefDie.Clone();

            double halfDieHeight = approximateDieHeight.Millimeters / 2;
            double maxPosistionY = approximateBottomLeftCornerOfRefDie.Y + halfDieHeight;

            while (researchPosition.Y < maxPosistionY)
            {
                try
                {
                    patternRecParams.PatternRecFlow.Input = patternRecParams.TopLeftCorner.ConvertToPatternRecInputForOtherPosition(researchPosition, patternRecParams.AutoFocusSettings);
                    var patternRecResultAtBottomLeft = ComputePatternRec(patternRecParams.PatternRecFlow);
                    var topLeftCornerOfDieAtBottom = new XYPosition(
                        new WaferReferential(),
                        approximateBottomLeftCornerOfRefDie.X + patternRecResultAtBottomLeft.ShiftX.Millimeters,
                        approximateBottomLeftCornerOfRefDie.Y + patternRecResultAtBottomLeft.ShiftY.Millimeters - nbStep * stepSizeY);

                    return new Tuple<XYPosition, double>(topLeftCornerOfDieAtBottom, patternRecResultAtBottomLeft.Confidence);
                }
                catch
                {
                    researchPosition.Y -= stepSizeY;
                    nbStep++;
                }
            }
            throw new Exception("Top left corner of die below not found.");
        }

        public static Tuple<XYPosition, double> FindTopLeftCornerOfDieOnSameLine(PatternRecognitionParams patternRecParams, int dieNbToMove, Length dieHorizontalPitch, Angle approximateDieAngle)
        {
            try
            {
                XYPosition approximateTopLeftCornerOfLastDieOnX = new XYPosition(
                    new WaferReferential(),
                    patternRecParams.TopLeftCorner.Position.X + dieNbToMove * dieHorizontalPitch.Millimeters * Math.Cos(approximateDieAngle.Radians),
                    patternRecParams.TopLeftCorner.Position.Y + dieNbToMove * dieHorizontalPitch.Millimeters * Math.Sin(approximateDieAngle.Radians));

                patternRecParams.PatternRecFlow.Input = patternRecParams.TopLeftCorner.ConvertToPatternRecInputForOtherPosition(approximateTopLeftCornerOfLastDieOnX, patternRecParams.AutoFocusSettings);
                var patternRecResultAtLastDieOnX = ComputePatternRec(patternRecParams.PatternRecFlow);

                XYPosition topLeftCornerOfLastDieOnX = new XYPosition(
                    new WaferReferential(),
                    approximateTopLeftCornerOfLastDieOnX.X + patternRecResultAtLastDieOnX.ShiftX.Millimeters,
                    approximateTopLeftCornerOfLastDieOnX.Y + patternRecResultAtLastDieOnX.ShiftY.Millimeters);

                double confidence = patternRecResultAtLastDieOnX.Confidence;

                return new Tuple<XYPosition, double>(topLeftCornerOfLastDieOnX, confidence);
            }
            catch
            {
                throw new Exception("Unable to accurately estimate the die with because the top left corner of the rightmost die can't be found.");
            }
        }

        public static Tuple<XYPosition, double> FindTopLeftCornerOfDieOnSameColumn(PatternRecognitionParams patternRecParams, int dieNbToMove, Length dieVerticalPitch, Angle approximateDieAngle)
        {
            try
            {
                XYPosition approximateTopLeftCornerOfLastDieOnY = new XYPosition(
                new WaferReferential(),
                patternRecParams.TopLeftCorner.Position.X - dieNbToMove * dieVerticalPitch.Millimeters * Math.Sin(approximateDieAngle.Radians),
                patternRecParams.TopLeftCorner.Position.Y + dieNbToMove * dieVerticalPitch.Millimeters * Math.Cos(approximateDieAngle.Radians));

                patternRecParams.PatternRecFlow.Input = patternRecParams.TopLeftCorner.ConvertToPatternRecInputForOtherPosition(approximateTopLeftCornerOfLastDieOnY, patternRecParams.AutoFocusSettings);
                var patternRecResultAtLastDieOnY = ComputePatternRec(patternRecParams.PatternRecFlow);

                XYPosition topLeftCornerOfLastDieOnY = new XYPosition(
                    new WaferReferential(),
                    approximateTopLeftCornerOfLastDieOnY.X + patternRecResultAtLastDieOnY.ShiftX.Millimeters,
                    approximateTopLeftCornerOfLastDieOnY.Y + patternRecResultAtLastDieOnY.ShiftY.Millimeters);

                double confidence = patternRecResultAtLastDieOnY.Confidence;

                return new Tuple<XYPosition, double>(topLeftCornerOfLastDieOnY, confidence);
            }
            catch
            {
                throw new Exception("Unable to accurately estimate the die height because the top left corner of the lowest die can't be found.");
            }
        }

        private static int MaxDieNbToWaferEdge(XYPosition initialTopLeftDieCornerPosition, Length diePitch, WaferDimensionalCharacteristic waferCharacteristics, Length outerRadiusExclusionLength)
        {
            var usableWaferRadius = waferCharacteristics.Diameter.Millimeters / 2 - outerRadiusExclusionLength.Millimeters;
            // Upper bound of distance to wafer edge is the distance to center plus usable radius (triangle inequality)
            double upperBoundDistanceToWaferEdge = usableWaferRadius + Math.Sqrt(Math.Pow(initialTopLeftDieCornerPosition.X, 2) + Math.Pow(initialTopLeftDieCornerPosition.Y, 2));
            return Convert.ToInt32(Math.Floor(upperBoundDistanceToWaferEdge / diePitch.Millimeters));
        }

        //TODOCle: refactor last 2 parameters by using a common WaferMapInput for wafermap and dieandstreetsize (after mergin AFInput branch).
        public static int FindUsableDieNumberAtRight(XYPosition initialTopLeftDieCornerPosition, Length dieHorizontalPitch, Length dieVerticalPitch, Angle approximateDieAngle, WaferDimensionalCharacteristic waferCharacteristics, Length outerRadiusExclusionLength)
        {
            int maxDieNb = MaxDieNbToWaferEdge(initialTopLeftDieCornerPosition, dieHorizontalPitch, waferCharacteristics, outerRadiusExclusionLength);
            var wafer = WaferUtils.CreateWaferShapeFromWaferCharacteristics(waferCharacteristics, outerRadiusExclusionLength);
            for (int i = 1; i < maxDieNb; i++)
            {
                var topRightDieCorner = new XYPosition(
                    new WaferReferential(),
                    initialTopLeftDieCornerPosition.X + (i + 1) * dieHorizontalPitch.Millimeters,
                    initialTopLeftDieCornerPosition.Y);

                MathTools.ApplyAntiClockwiseRotation(approximateDieAngle, topRightDieCorner, initialTopLeftDieCornerPosition.ToXYPosition());

                var bottomRightDieCorner = new XYPosition(
                    new WaferReferential(),
                    initialTopLeftDieCornerPosition.X + (i + 1) * dieHorizontalPitch.Millimeters,
                    initialTopLeftDieCornerPosition.Y - dieVerticalPitch.Millimeters);

                MathTools.ApplyAntiClockwiseRotation(approximateDieAngle, bottomRightDieCorner, initialTopLeftDieCornerPosition.ToXYPosition());

                if (!wafer.IsInside(new PointUnits(topRightDieCorner.X.Millimeters(), topRightDieCorner.Y.Millimeters())) ||
                    !wafer.IsInside(new PointUnits(bottomRightDieCorner.X.Millimeters(), bottomRightDieCorner.Y.Millimeters())))
                {
                    return i - 1;
                }
            }

            return maxDieNb - 1;
        }

        //TODOCle: refactor last 2 parameters by using a common WaferMapInput for wafermap and dieandstreetsize (after mergin AFInput branch).
        public static int FindUsableDieNumberAtBottom(XYPosition initialTopLeftDieCornerPosition, Length dieHorizontalPitch, Length dieVerticalPitch, Angle approximateDieAngle, WaferDimensionalCharacteristic waferCharacteristics, Length outerRadiusExclusionLength)
        {
            int maxDieNb = MaxDieNbToWaferEdge(initialTopLeftDieCornerPosition, dieVerticalPitch, waferCharacteristics, outerRadiusExclusionLength);
            var wafer = WaferUtils.CreateWaferShapeFromWaferCharacteristics(waferCharacteristics, outerRadiusExclusionLength);
            for (int i = 1; i < maxDieNb; i++)
            {
                var bottomLeftDieCorner = new XYPosition(
                    new WaferReferential(),
                    initialTopLeftDieCornerPosition.X,
                    initialTopLeftDieCornerPosition.Y - (i + 1) * dieVerticalPitch.Millimeters);

                MathTools.ApplyAntiClockwiseRotation(approximateDieAngle, bottomLeftDieCorner, initialTopLeftDieCornerPosition.ToXYPosition());

                var bottomRightDieCorner = new XYPosition(
                    new WaferReferential(),
                    initialTopLeftDieCornerPosition.X + dieHorizontalPitch.Millimeters,
                    initialTopLeftDieCornerPosition.Y - (i + 1) * dieVerticalPitch.Millimeters);

                MathTools.ApplyAntiClockwiseRotation(approximateDieAngle, bottomRightDieCorner, initialTopLeftDieCornerPosition.ToXYPosition());

                if (!wafer.IsInside(new PointUnits(bottomLeftDieCorner.X.Millimeters(), bottomLeftDieCorner.Y.Millimeters())) ||
                    !wafer.IsInside(new PointUnits(bottomRightDieCorner.X.Millimeters(), bottomRightDieCorner.Y.Millimeters())))
                {
                    return i - 1;
                }
            }

            return maxDieNb - 1;
        }

        public static PatternRecResult ComputePatternRec(PatternRecFlow patternRecFlow)
        {
            var subResult = patternRecFlow.Execute();
            if (subResult.Status.State != FlowState.Success)
            {
                throw new Exception("Pattern recognition failed.");
            }

            return subResult;
        }
    }
}
