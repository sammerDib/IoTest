using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.ANA.Service.Core.PatternRec;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.WaferMap
{
    public class CheckPatternRecFlow : FlowComponent<CheckPatternRecInput, CheckPatternRecResult, CheckPatternRecConfiguration>
    {
        private PatternRecFlow _patternRecFlow;

        public CheckPatternRecFlow(CheckPatternRecInput input, PatternRecFlow patternRec = null) : base(input, "CheckPatternRecFlow")
        {
            _patternRecFlow = patternRec != null ?
                patternRec :
                new PatternRecFlow(new PatternRecInput());
        }

        protected override void Process()
        {
            Result.Gamma = Input.PositionWithPatternRec.PatternRec.Gamma;
            Result.SingleResults = ComputeAllPatternRec(Input.ValidationPositions, Result.Gamma);
            Result.Succeeded = CheckSinglesResult(Input.ValidationPositions, Result.SingleResults);
        }

        private List<PatternRecResult> ComputeAllPatternRec(List<XYZTopZBottomPosition> positions, double gamma)
        {
            var subResultForEachPosition = new List<PatternRecResult>();

            foreach (var position in positions)
            {
                CheckCancellation();

                _patternRecFlow.Input = Input.PositionWithPatternRec.ConvertToPatternRecInputForOtherPosition(position);
                _patternRecFlow.Input.Data.Gamma = gamma;
                var resultAtPos = _patternRecFlow.Execute();

                subResultForEachPosition.Add(resultAtPos);
            }

            return subResultForEachPosition;
        }

        private bool CheckSinglesResult(List<XYZTopZBottomPosition> validationPositions, List<PatternRecResult> patternRecResult)
        {
            var allResultAreValid = true;
            if (patternRecResult.Count != validationPositions.Count)
            {
                throw new Exception($"The number of positions must be equal to the number of results to check the validity of these results compared to their position.");
            }

            int resultNb = patternRecResult.Count;
            for (int i = 0; i < resultNb; i++)
            {
                var expectedShiftX = new Length(Input.PositionWithPatternRec.Position.X - validationPositions.ElementAt(i).X, LengthUnit.Millimeter);
                var expectedShiftY = new Length(Input.PositionWithPatternRec.Position.Y - validationPositions.ElementAt(i).Y, LengthUnit.Millimeter);

                var resultAtPos = patternRecResult.ElementAt(i);

                bool validResult = resultAtPos.Status.State == FlowState.Success;
                allResultAreValid &= validResult;

                // Shifts in the result may be null (with an invalid result for example)
                if (!(resultAtPos.ShiftX is null))
                {
                    bool validShiftX = resultAtPos.ShiftX.Millimeters.Near(expectedShiftX.Millimeters, Input.Tolerance.Millimeters);
                    resultAtPos.ShiftX -= expectedShiftX;
                    allResultAreValid &= validShiftX;
                }
                else
                {
                    allResultAreValid = false;
                }

                if (!(resultAtPos.ShiftY is null))
                {
                    bool validShiftY = resultAtPos.ShiftY.Millimeters.Near(expectedShiftY.Millimeters, Input.Tolerance.Millimeters);
                    allResultAreValid &= validShiftY;
                    resultAtPos.ShiftY -= expectedShiftY;
                }
                else
                {
                    allResultAreValid = false;
                }
            }

            return allResultAreValid;
        }
    }
}
