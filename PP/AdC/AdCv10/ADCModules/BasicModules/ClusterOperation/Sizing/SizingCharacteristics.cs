using AdcBasicObjects;

namespace BasicModules.Sizing
{
    ///////////////////////////////////////////////////////////////////////
    // Liste des caractéristiques des clusters pour le Sizing
    ///////////////////////////////////////////////////////////////////////
    public static class SizingCharacteristics
    {
        public static Characteristic DefectMaxSize = new Characteristic(typeof(double), "DefectMaxSize");
        public static Characteristic TotalDefectSize = new Characteristic(typeof(double), "DefectTotalSize");
        public static Characteristic SizingType = new Characteristic(typeof(BasicModules.Sizing.eSizingType), "SizingType");
    }
}
