using System;
using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Core.PatternRec;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.AlignmentMarks
{
    public class AlignmentMarksFlow : FlowComponent<AlignmentMarksInput, AlignmentMarksResult, AlignmentMarksConfiguration>
    {
        private PatternRecFlow _patternRecFlow;

        public AlignmentMarksFlow(AlignmentMarksInput input, PatternRecFlow patternRec = null) : base(input, "AlignmentMarksFlow")
        {
            _patternRecFlow = patternRec != null ?
                patternRec :
                new PatternRecFlow(new PatternRecInput());
        }

        private class AlignmentMarkShift
        {
            public XYPosition ExpectedMarkPos { get; set; }// The pattern's original position
            public XYPosition ActualMarkPos { get; set; }// The current pattern position, compute thanks to patternRec
            public double Confidence { get; set; }
        }

        protected override void Process()
        {
            FindAngleAndShift();
        }

        private void FindAngleAndShift()
        {
            // Site 1 mark patternRec
            AlignmentMarkShift site1MarkShift = FindMarkShift(Input.Site1Images) ?? throw new Exception("Pattern not found on site 1");
            CheckCancellation();

            // Site 2 mark patternRec
            AlignmentMarkShift site2MarkShift = FindMarkShift(Input.Site2Images) ?? throw new Exception("Pattern not found on site 2");
            CheckCancellation();

            // Compute the angle from Expected to Actual
            Angle angle = MathTools.ComputeAngleFromTwoVectors(site1MarkShift.ExpectedMarkPos, site2MarkShift.ExpectedMarkPos, site1MarkShift.ActualMarkPos, site2MarkShift.ActualMarkPos);

            // Apply angle to the expected marks
            var waferCenter = new XYPosition(site1MarkShift.ExpectedMarkPos.Referential, 0, 0);
            XYPosition expectedMarkPosRotated = site1MarkShift.ExpectedMarkPos.Clone() as XYPosition;
            MathTools.ApplyAntiClockwiseRotation(angle, expectedMarkPosRotated, waferCenter);

            Result.ShiftX = (expectedMarkPosRotated.X - site1MarkShift.ActualMarkPos.X).Millimeters();
            Result.ShiftY = (expectedMarkPosRotated.Y - site1MarkShift.ActualMarkPos.Y).Millimeters();
            Result.RotationAngle = -angle;
            Result.Confidence = (site1MarkShift.Confidence + site2MarkShift.Confidence) / 2;

            Logger.Information($"{LogHeader} Calculated ShiftX: {Result.ShiftX}.");
            Logger.Information($"{LogHeader} Calculated ShiftY: {Result.ShiftY}.");
            Logger.Information($"{LogHeader} Calculated RotationAngle: {Result.RotationAngle}.");
        }

        private AlignmentMarkShift FindMarkShift(List<PositionWithPatternRec> images)
        {
            foreach (var image in images)
            {
                _patternRecFlow.Input = image.ConvertToPatternRecInput(Input.AutoFocusSettings);
                PatternRecResult patternRecResult = _patternRecFlow.Execute();
                CheckCancellation();

                if (patternRecResult.Status.State == FlowState.Success)
                {
                    XYPosition expectedMarkPos = image.Position.ToXYPosition();
                    var actualMarkPos = new XYPosition(expectedMarkPos.Referential, expectedMarkPos.X - patternRecResult.ShiftX.Millimeters, expectedMarkPos.Y - patternRecResult.ShiftY.Millimeters);
                    return new AlignmentMarkShift
                    {
                        ExpectedMarkPos = expectedMarkPos,
                        ActualMarkPos = actualMarkPos,
                        Confidence = patternRecResult.Confidence,
                    };
                }
            }
            return null;
        }
    }
}
