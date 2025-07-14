namespace AdcBasicObjects
{
    // use mainly for 3D treatments (such as bump/heuiht measurements) need to be shared by basic / advanced / and any aspecifcs modules 
    public static class Cluster3DCharacteristics
    {
        // Metrology Characterization
        public static Characteristic HeightAverage { get; private set; }
        public static Characteristic HeightStdDev { get; private set; }
        public static Characteristic HeightMin { get; private set; }
        public static Characteristic HeightMax { get; private set; }
        public static Characteristic Coplanarity { get; private set; }
        public static Characteristic SubstrateCoplanarity { get; private set; }
        public static Characteristic Height { get; private set; } // pour cluster3Dhm (obsolete ?)

        public static Characteristic isFailure { get; private set; }
        public static Characteristic isHeightFailure { get; private set; }
        public static Characteristic isCoplaFailure { get; private set; }
        public static Characteristic isSubCoplaFailure { get; private set; }
        public static Characteristic isMeasureMissingFailure { get; private set; }
        public static Characteristic isMeasureHeightFailure { get; private set; }

        // Specifc Bare wafer
        public static Characteristic BareHeightAverage { get; private set; }
        public static Characteristic BareHeightStdDev { get; private set; }
        public static Characteristic BareHeightMin { get; private set; }
        public static Characteristic BareHeightMax { get; private set; }
        public static Characteristic NaNsCount { get; private set; }

        internal static void Init()
        {
            HeightAverage = new Characteristic(typeof(double), "HeightAverage");
            HeightStdDev = new Characteristic(typeof(double), "HeightStdDev");
            HeightMin = new Characteristic(typeof(double), "HeightMin");
            HeightMax = new Characteristic(typeof(double), "HeightMax");
            Coplanarity = new Characteristic(typeof(double), "Coplanarity");
            SubstrateCoplanarity = new Characteristic(typeof(double), "SubstrateCoplanarity");
            Height = new Characteristic(typeof(double), "Height"); // pour cluster3Dhm (obsolete ?)
            isFailure = new Characteristic(typeof(bool), "isFail");
            isHeightFailure = new Characteristic(typeof(bool), "isHeightFail");
            isCoplaFailure = new Characteristic(typeof(bool), "isCoplaFail");
            isSubCoplaFailure = new Characteristic(typeof(bool), "isSubCoplaFail");
            isMeasureMissingFailure = new Characteristic(typeof(bool), "isMeasureMissingFail");
            isMeasureHeightFailure = new Characteristic(typeof(bool), "isMeasureHeightFail");
            BareHeightAverage = new Characteristic(typeof(double), "BareHeightAverage");
            BareHeightStdDev = new Characteristic(typeof(double), "BareHeightStdDev");
            BareHeightMin = new Characteristic(typeof(double), "BareHeightMin");
            BareHeightMax = new Characteristic(typeof(double), "BareHeightMax");
            NaNsCount = new Characteristic(typeof(double), "NaNsCount");
        }
    }

    public static class Blob3DCharacteristics
    {
        public static Characteristic HeightMicron { get; private set; }
        public static Characteristic SubstrateHeightMicron { get; private set; }
        public static Characteristic FailureType { get; private set; }

        internal static void Init()
        {
            HeightMicron = new Characteristic(typeof(double), "Heightblob");
            SubstrateHeightMicron = new Characteristic(typeof(double), "SubstrateHeightblob");
            FailureType = new Characteristic(typeof(double), "FailureType");
        }
    }
}
