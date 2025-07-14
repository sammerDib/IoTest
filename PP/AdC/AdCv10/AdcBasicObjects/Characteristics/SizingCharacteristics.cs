namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    // Liste des caractéristiques des clusters pour le Sizing
    ///////////////////////////////////////////////////////////////////////
    public static class SizingCharacteristics
    {
        public static Characteristic DefectMaxSize { get; private set; }
        public static Characteristic TotalDefectSize { get; private set; }
        public static Characteristic SizingType { get; private set; }
        public static Characteristic sizeX { get; private set; }
        public static Characteristic sizeY { get; private set; }
        public static Characteristic DefectArea { get; private set; }
        public static Characteristic DSize { get; private set; }

        internal static void Init()
        {
            DefectMaxSize = new Characteristic(typeof(double), "DefectMaxSize");
            TotalDefectSize = new Characteristic(typeof(double), "TotalDefectSize");
            SizingType = new Characteristic(typeof(double), "SizingType");
            sizeX = new Characteristic(typeof(double), "sizeX");
            sizeY = new Characteristic(typeof(double), "sizeY");
            DefectArea = new Characteristic(typeof(double), "DefectArea");
            DSize = new Characteristic(typeof(double), "DSize");
        }
    }
}

