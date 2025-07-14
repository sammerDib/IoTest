namespace AdcBasicObjects
{
    // use mainly for 2D treatments (such as bump/Diameter offset measurements) need to be shared by basic / advanced / and any aspecifcs modules 
    public static class Cluster2DCharacteristics
    {
        public static Characteristic DiameterAverage { get; private set; }
        public static Characteristic DiameterStdDev { get; private set; }
        public static Characteristic DiameterMin { get; private set; }
        public static Characteristic DiameterMax { get; private set; }

        public static Characteristic OffsetAverage { get; private set; }
        public static Characteristic OffsetStdDev { get; private set; }
        public static Characteristic OffsetMin { get; private set; }
        public static Characteristic OffsetMax { get; private set; }

        public static Characteristic isFailure { get; private set; }
        public static Characteristic isDiameterFailure { get; private set; }
        public static Characteristic isMeasureMissingFailure { get; private set; }
        public static Characteristic isMeasureDiameterFailure { get; private set; }
        public static Characteristic isMeasureOffsetFailure { get; private set; }

        internal static void Init()
        {
            DiameterAverage = new Characteristic(typeof(double), "DiameterAverage");
            DiameterStdDev = new Characteristic(typeof(double), "DiameterStdDev");
            DiameterMin = new Characteristic(typeof(double), "DiameterMin");
            DiameterMax = new Characteristic(typeof(double), "DiameterMax");
            OffsetAverage = new Characteristic(typeof(double), "OffsetAverage");
            OffsetStdDev = new Characteristic(typeof(double), "OffsetStdDev");
            OffsetMin = new Characteristic(typeof(double), "OffsetMin");
            OffsetMax = new Characteristic(typeof(double), "OffsetMax");
            isFailure = new Characteristic(typeof(bool), "isFail");
            isDiameterFailure = new Characteristic(typeof(bool), "isDiameterFail");
            isMeasureMissingFailure = new Characteristic(typeof(bool), "isMeasureMissingFail");
            isMeasureDiameterFailure = new Characteristic(typeof(bool), "isMeasureDiameterFail");
            isMeasureOffsetFailure = new Characteristic(typeof(bool), "isMeasureOffsetFail");
        }
    }

    public static class Blob2DCharacteristics
    {
        public static Characteristic Diameter { get; private set; }
        public static Characteristic OffsetPos { get; private set; }
        public static Characteristic DeltaTargetX { get; private set; }
        public static Characteristic DeltaTargetY { get; private set; }
        public static Characteristic isMissing { get; private set; }
        public static Characteristic FailureType { get; private set; }

        internal static void Init()
        {
            Diameter = new Characteristic(typeof(double), "Diameterblob");
            OffsetPos = new Characteristic(typeof(double), "OffsetPos");
            DeltaTargetX = new Characteristic(typeof(double), "DeltaTargetX");
            DeltaTargetY = new Characteristic(typeof(double), "DeltaTargetY");
            isMissing = new Characteristic(typeof(double), "isMissing");
            FailureType = new Characteristic(typeof(double), "FailureType");
        }
    }
}
