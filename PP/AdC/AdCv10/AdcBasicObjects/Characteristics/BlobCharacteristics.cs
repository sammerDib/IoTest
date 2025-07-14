namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    // List des caracteristiques spécifiques aux blobs
    ///////////////////////////////////////////////////////////////////////
    public static class BlobCharacteristics
    {
        public static Characteristic BOX_X_MAX { get; private set; }
        public static Characteristic BOX_X_MIN { get; private set; }
        public static Characteristic BOX_Y_MAX { get; private set; }
        public static Characteristic BOX_Y_MIN { get; private set; }
        public static Characteristic CENTER_OF_GRAVITY_X { get; private set; }
        public static Characteristic CENTER_OF_GRAVITY_Y { get; private set; }
        public static Characteristic PSLValue { get; private set; }
        public static Characteristic MicronArea { get; private set; }
        public static Characteristic REAL_PX_AREA { get; private set; }
        public static Characteristic sizeX { get; private set; }
        public static Characteristic sizeY { get; private set; }
        public static Characteristic pslµsize { get; private set; }

        internal static void Init()
        {
            BOX_X_MAX = new Characteristic(typeof(double), "BOX_X_MAX");
            BOX_X_MIN = new Characteristic(typeof(double), "BOX_X_MIN");
            BOX_Y_MAX = new Characteristic(typeof(double), "BOX_Y_MAX");
            BOX_Y_MIN = new Characteristic(typeof(double), "BOX_Y_MIN");
            CENTER_OF_GRAVITY_X = new Characteristic(typeof(double), "CENTER_OF_GRAVITY_X");
            CENTER_OF_GRAVITY_Y = new Characteristic(typeof(double), "CENTER_OF_GRAVITY_Y");
            PSLValue = new Characteristic(typeof(double), "BlobPSLValue");
            MicronArea = new Characteristic(typeof(double), "MicronArea");
            REAL_PX_AREA = new Characteristic(typeof(double), "REAL_PX_AREA");
            sizeX = new Characteristic(typeof(double), "sizeX");
            sizeY = new Characteristic(typeof(double), "sizeY");
            pslµsize = new Characteristic(typeof(double), "pslµsize");
        }
    }
}
