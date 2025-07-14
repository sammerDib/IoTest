namespace UnitySC.PM.DMT.Service.Interface.RecipeService
{
    public enum RecipeCheckError
    {
        SideIncompatibility,
        NotCompatiblePerspectiveCalibration,
        BrightFieldColorNotCompatible,
        BrightFieldApplyUniformity,
        NotCompatibleDeflectometryOutputs,
        NotCompatibleDeflectometryStandardFringe,
        NotCompatibleDeflectometryMultiFringe,
        NotAvailableMeasure,
        NotAvailableExposureMatchingCalibration
    }
}
