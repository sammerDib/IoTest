using System;
using System.Xml.Serialization;

namespace MergeContext.Context
{
    public class DMTCorrector : AdcTools.Serializable
    {
        public enum ImageType { None = -1, CurvatureX, CurvatureY, Amplitude, Brightfield };

        public SideCorrector FrontSideCorrector = new SideCorrector();
        public SideCorrector BackSideCorrector = new SideCorrector();

        ///////////////////////////////////////////////////////////////////
        //
        ///////////////////////////////////////////////////////////////////
        [Serializable]
        public class SideCorrector
        {
            public bool IsEnabled;

            [XmlIgnore] public EdgeFinderPositionCorrectorConfiguration edgeFinderPositionCorrectorConfiguration;
            public EdgeFinderPositionCorrectorConfiguration EdgeFinderPositionCorrectorConfiguration
            {
                // ça permet de ne pas sérialiser la structure quand elle ne sert pas
                get { if (IsEnabled) return edgeFinderPositionCorrectorConfiguration; else return null; }
                set { edgeFinderPositionCorrectorConfiguration = value; }
            }

            [XmlIgnore] public MilModelRotationCorrectorConfiguration milModelRotationCorrectorConfiguration;
            public MilModelRotationCorrectorConfiguration MilModelRotationCorrectorConfiguration
            {
                // ça permet de ne pas sérialiser la structure quand elle ne sert pas
                get { if (IsEnabled) return milModelRotationCorrectorConfiguration; else return null; }
                set { milModelRotationCorrectorConfiguration = value; }
            }

            [XmlIgnore] public MilModelPositionRotationCorrectorConfiguration milModelPositionRotationCorrectorConfiguration;
            public MilModelPositionRotationCorrectorConfiguration MilModelPositionRotationCorrectorConfiguration
            {
                // ça permet de ne pas sérialiser la structure quand elle ne sert pas
                get { if (IsEnabled) return milModelPositionRotationCorrectorConfiguration; else return null; }
                set { milModelPositionRotationCorrectorConfiguration = value; }
            }
        }

        ///////////////////////////////////////////////////////////////////
        //
        ///////////////////////////////////////////////////////////////////
        [Serializable]
        public class EdgeFinderPositionCorrectorConfiguration
        {
            [XmlAttribute] public ImageType ImageIndex = ImageType.Brightfield;
            [XmlAttribute] public int BinThreshold = 60;
            [XmlAttribute] public int BinThresholdUpperLimit = 255;
            [XmlAttribute] public int NbClosingIterations = 2;
            [XmlAttribute] public double RadiusTolerance = 3000;
            [XmlAttribute] public double WaferPositionTolerance = 3000;
            [XmlAttribute] public double FitCircleCoverageMin = 0.6;
            [XmlAttribute] public int FillGapDistance = 2;
            [XmlAttribute] public int Smoothness = 50;
            [XmlAttribute] public string EdgeFinderName;
            public byte[] EdgeFinder;
        }

        ///////////////////////////////////////////////////////////////////
        //
        ///////////////////////////////////////////////////////////////////
        [Serializable]
        public class MilModelRotationCorrectorConfiguration
        {
            [XmlAttribute] public ImageType ImageIndex = ImageType.Brightfield;
            [XmlAttribute] public int BinThreshold = 60;
            [XmlAttribute] public int BinThresholdUpperLimit = 255;
            [XmlAttribute] public int NbClosingIterations = 2;
            [XmlAttribute] public string ModelFinderName;
            public byte[] ModelFinder;
            [XmlAttribute] public double WaferAngleToleranceDegree = 5;

            [XmlIgnore]
            public double WaferAngleTolerance
            {
                get { return WaferAngleToleranceDegree / 180 * Math.PI; }
                set { WaferAngleToleranceDegree = value / Math.PI * 180; }
            }
        }

        ///////////////////////////////////////////////////////////////////
        //
        ///////////////////////////////////////////////////////////////////
        [Serializable]
        public class MilModelPositionRotationCorrectorConfiguration
        {
            [XmlAttribute] public ImageType ImageIndex = ImageType.Brightfield;
            [XmlAttribute] public int BinThreshold = 60;
            [XmlAttribute] public int BinThresholdUpperLimit = 255;
            [XmlAttribute] public int NbClosingIterations = 2;
            [XmlAttribute] public double WaferPositionTolerance = 3000;
            [XmlAttribute] public string ModelFinderName;
            public byte[] ModelFinder;
            [XmlAttribute] public double WaferAngleToleranceDegree = 5;

            [XmlIgnore]
            public double WaferAngleTolerance
            {
                get { return WaferAngleToleranceDegree / 180 * Math.PI; }
                set { WaferAngleToleranceDegree = value / Math.PI * 180; }
            }
        }

    }
}
