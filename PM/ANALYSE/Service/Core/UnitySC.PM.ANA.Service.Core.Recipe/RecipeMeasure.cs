using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Shared;

namespace UnitySC.PM.ANA.Service.Core.Recipe
{
 
        public class RecipeMeasure
        {
            public IMeasure Measure { get; set; }
            public MeasureSettingsBase Settings { get; set; }
            public HashSet<int> MeasurePointIds { get; set; }
        }

}
