using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;

namespace UnitySC.PM.ANA.Service.Core.Recipe
{
    public static class RecipeHelper
    {
        public static bool IsMeasureInWaferReferential(RecipeMeasure measure, List<MeasurePoint> points)
        {
            var measurePointId = measure.MeasurePointIds.First();
            var point = points.Find(p => p.Id == measurePointId);

            return !point.IsDiePosition;
        }
    }
}
