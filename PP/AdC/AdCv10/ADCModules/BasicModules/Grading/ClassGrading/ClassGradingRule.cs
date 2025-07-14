using ADCEngine;

namespace BasicModules.Grading.ClassGrading
{
    /// <summary>
    /// Rule for class grading
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public class ClassGradingRule : GradingRule, IValueComparer
    {
        /// <summary>
        /// Name of the defect class
        /// </summary>
        public string DefectClass { get; set; }

        public bool HasSameValue(object obj)
        {
            var comparator = obj as ClassGradingRule;
            return comparator != null
                && comparator.BiggerThan == BiggerThan
                && comparator.Criteria == Criteria
                && comparator.DefectClass == DefectClass
                && comparator.GradingMark == GradingMark;
        }
    }
}
