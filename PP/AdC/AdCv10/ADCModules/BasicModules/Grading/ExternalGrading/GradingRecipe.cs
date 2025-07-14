using System.Collections.Generic;

namespace BasicModules.Grading.ExternalGrading
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class GradingRecipe
    {
        /// <summary>
        /// List of grading rules
        /// </summary>
        public List<Rule> Rules { get; set; }
    }

    [System.Reflection.Obfuscation(Exclude = true)]
    public class Rule : GradingRule
    {
        /// <summary>
        /// RoughBin numbed used for the rule
        /// </summary>
        public int RoughBinNumber { get; set; }
    }
}
