using System;

namespace AdvancedModules.ClassificationMultiLayer
{
    [Serializable]
    public enum DefectTestType
    {
        DefectClassNotUsed,
        DoNotTest,
        DefectMustBePresent,
        DefectMustNotBePresent,
    }
}
