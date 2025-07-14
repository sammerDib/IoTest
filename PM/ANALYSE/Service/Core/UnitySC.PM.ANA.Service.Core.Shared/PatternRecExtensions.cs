using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Core.Shared
{
    public static class PatternRecExtensions
    {
        public static PatternRecInput ConvertToPatternRecInput(this PositionWithPatternRec patternRec, AutoFocusSettings autofocusSettings = null)
        {
            var runAutoFocus = !(autofocusSettings is null);

            return new PatternRecInput(patternRec.PatternRec, runAutoFocus, autofocusSettings)
            {
                InitialContext = patternRec.Context.AppendWithPositionContext(patternRec.Position)
            };
        }

        public static PatternRecInput ConvertToPatternRecInputForOtherPosition(this PositionWithPatternRec patternRec, XYZTopZBottomPosition position, AutoFocusSettings autofocusSettings = null)
        {
            var runAutofocus = !(autofocusSettings is null);

            return new PatternRecInput(patternRec.PatternRec, runAutofocus, autofocusSettings)
            {
                InitialContext = patternRec.Context.AppendWithPositionContext(position)
            };
        }

        public static PatternRecInput ConvertToPatternRecInputForOtherPosition(this PositionWithPatternRec patternRec, XYPosition position, AutoFocusSettings autofocusSettings = null)
        {
            var runAutofocus = !(autofocusSettings is null);

            return new PatternRecInput(patternRec.PatternRec, runAutofocus, autofocusSettings)
            {
                InitialContext = patternRec.Context.AppendWithPositionContext(position)
            };
        }
    }
}
