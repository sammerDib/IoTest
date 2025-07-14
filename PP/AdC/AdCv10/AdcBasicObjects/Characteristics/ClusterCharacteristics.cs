using System.Drawing;

namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    // List des caracteristiques des clusters
    ///////////////////////////////////////////////////////////////////////
    public static class ClusterCharacteristics
    {
        public static Characteristic Area { get; private set; }
        public static Characteristic Barycenter { get; private set; }
        public static Characteristic SymetricDispersion { get; private set; }
        public static Characteristic FillingValue { get; private set; }
        public static Characteristic Breadth { get; private set; }
        public static Characteristic Compactness { get; private set; }
        public static Characteristic ConvexPerimeter { get; private set; }
        public static Characteristic Elongation { get; private set; }
        public static Characteristic EulerNumber { get; private set; }
        public static Characteristic Perimeter { get; private set; }
        public static Characteristic Roughness { get; private set; }
        public static Characteristic Length { get; private set; }
        public static Characteristic AxisPrincipalAngle { get; private set; }
        public static Characteristic AxisSecondaryAngle { get; private set; }
        public static Characteristic RadialPosition { get; private set; }
        public static Characteristic AbsolutePosition { get; private set; }
        public static Characteristic SurroundingRectangleArea { get; private set; }
        public static Characteristic RealDiameter { get; private set; }
        public static Characteristic RealHeight { get; private set; }
        public static Characteristic RealWidth { get; private set; }
        public static Characteristic BlobAverageGreyLevel { get; private set; }
        public static Characteristic BlobMaxGreyLevel { get; private set; }
        public static Characteristic ClusterAverageGreyLevel { get; private set; }
        public static Characteristic ClusterMaxGreyLevel { get; private set; }
        public static Characteristic BlobMinGreyLevel { get; private set; }
        public static Characteristic BlobStandardDev { get; private set; }
        public static Characteristic ClusterMinGreyLevel { get; private set; }
        public static Characteristic ClusterStandardDev { get; private set; }
        public static Characteristic BlobCount { get; private set; }
        public static Characteristic RatioVertical { get; private set; }
        public static Characteristic SumLevel { get; private set; }
        public static Characteristic AnglePosition { get; private set; }
        public static Characteristic PSLValue { get; private set; }
        public static Characteristic PSLMaxValue { get; private set; }
        //public static Characteristic reportType { get; private set; } // 0 = psl, 1 = area

        internal static void Init()
        {
            Area = new Characteristic(typeof(double), "Area");
            Barycenter = new Characteristic(typeof(double), "Barycenter");
            SymetricDispersion = new Characteristic(typeof(double), "SymetricDispersion");
            FillingValue = new Characteristic(typeof(double), "FillingValue");
            Breadth = new Characteristic(typeof(double), "Breadth");
            Compactness = new Characteristic(typeof(double), "Compactness");
            ConvexPerimeter = new Characteristic(typeof(double), "ConvexPerimeter");
            Elongation = new Characteristic(typeof(double), "Elongation");
            EulerNumber = new Characteristic(typeof(double), "EulerNumber");
            Perimeter = new Characteristic(typeof(double), "Perimeter");
            Roughness = new Characteristic(typeof(double), "Roughness");
            Length = new Characteristic(typeof(double), "Length");
            AxisPrincipalAngle = new Characteristic(typeof(double), "AxisPrincipalAngle");
            AxisSecondaryAngle = new Characteristic(typeof(double), "AxisSecondaryAngle");
            RadialPosition = new Characteristic(typeof(double), "RadialPosition");
            AbsolutePosition = new Characteristic(typeof(RectangleF), "AbsolutePosition");
            SurroundingRectangleArea = new Characteristic(typeof(double), "SurroundingRectangleArea");
            RealDiameter = new Characteristic(typeof(double), "RealDiameter");
            RealHeight = new Characteristic(typeof(double), "RealHeight");
            RealWidth = new Characteristic(typeof(double), "RealWidth");
            BlobAverageGreyLevel = new Characteristic(typeof(double), "BlobAverageGreyLevel");
            BlobMaxGreyLevel = new Characteristic(typeof(double), "BlobMaxGreyLevel");
            ClusterAverageGreyLevel = new Characteristic(typeof(double), "ClusterAverageGreyLevel");
            ClusterMaxGreyLevel = new Characteristic(typeof(double), "ClusterMaxGreyLevel");
            BlobMinGreyLevel = new Characteristic(typeof(double), "BlobMinGreyLevel");
            BlobStandardDev = new Characteristic(typeof(double), "BlobStandardDev");
            ClusterMinGreyLevel = new Characteristic(typeof(double), "CusterMinGreyLevel");
            ClusterStandardDev = new Characteristic(typeof(double), "ClusterStandardDev");
            BlobCount = new Characteristic(typeof(double), "BlobCount");
            RatioVertical = new Characteristic(typeof(double), "RatioVertical");
            SumLevel = new Characteristic(typeof(double), "SumLevel");
            AnglePosition = new Characteristic(typeof(double), "AnglePosition");
            PSLValue = new Characteristic(typeof(double), "ClusterPSLValue");
            PSLMaxValue = new Characteristic(typeof(double), "ClusterMaxPSLValue");
            //reportType = new Characteristic(typeof(int), "reportType");
        }
    }
}
