using System;
using System.Xml.Serialization;

namespace MergeContext.Context
{
    ///////////////////////////////////////////////////////////////////
    /// <summary>
    /// Paramètres du correcteur Edge.
    /// NB: la définition des Pads/Fingers est dans la configuration
    ///     de la chambre.
    /// </summary>
    ///////////////////////////////////////////////////////////////////
    public class EdgeCorrector : AdcTools.Serializable
    {
        public SensorCorrector[] SensorCorrectors = new SensorCorrector[5];

        public EdgeCorrector()
        {
            SensorCorrectors[0] = new SensorCorrector() { Sensor = "TOP SURFACE" };
            SensorCorrectors[1] = new SensorCorrector() { Sensor = "TOP BEVEL" };
            SensorCorrectors[2] = new SensorCorrector() { Sensor = "APEX" };
            SensorCorrectors[3] = new SensorCorrector() { Sensor = "BOTTOM BEVEL" };
            SensorCorrectors[4] = new SensorCorrector() { Sensor = "BOTTOM SURFACE" };
        }

        ///////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sous-classe contenant la description des corrections pour une caméra
        /// </summary>
        ///////////////////////////////////////////////////////////////////
        [Serializable]
        public class SensorCorrector
        {
            [XmlAttribute] public string Sensor;
            [XmlAttribute] public bool SearchNotch = false;
            [XmlAttribute] public bool IsMetalNotch = false;
            [XmlAttribute] public bool SearchPad = false;
            [XmlAttribute] public bool SearchFinger = false;

            [XmlIgnore] public StandardSearchParameters standardNotchParameters = new StandardSearchParameters();
            public StandardSearchParameters StandardNotchParameters
            {
                // ça permet de ne pas sérialiser la structure quand elle ne sert pas
                get { if (SearchNotch && !IsMetalNotch) return standardNotchParameters; else return null; }
                set { standardNotchParameters = value; }
            }

            [XmlIgnore] public StandardSearchParameters padParameters = new StandardSearchParameters();
            public StandardSearchParameters PadParameters
            {
                // ça permet de ne pas sérialiser la structure quand elle ne sert pas
                get { if (SearchPad) return padParameters; else return null; }
                set { padParameters = value; }
            }

            [XmlIgnore] public StandardSearchParameters fingerParameters = new StandardSearchParameters();
            public StandardSearchParameters FingerParameters
            {
                // ça permet de ne pas sérialiser la structure quand elle ne sert pas
                get { if (SearchFinger) return fingerParameters; else return null; }
                set { fingerParameters = value; }
            }

            [XmlIgnore] public MetalNotchParameters metalNotchParameters = new MetalNotchParameters();
            public MetalNotchParameters MetalNotchParameters
            {
                // ça permet de ne pas sérialiser la structure quand elle ne sert pas
                get { if (SearchNotch && IsMetalNotch) return metalNotchParameters; else return null; }
                set { metalNotchParameters = value; }
            }
        }

        ///////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sous-classe contenant les paramètres pour la recherche du 
        /// Notch/Pad/finger selon la méthode standard.
        /// </summary>
        ///////////////////////////////////////////////////////////////////
        [Serializable]
        public class StandardSearchParameters
        {
            /// <summary> Ratio de la moyenne calculée en dessous de laquelle on considère qu'il s'agit d'un notch/pad/finger </summary>
            [XmlAttribute] public double Ratio = 0.5;

            /// <summary> Tolérence sur la taille du notch/pad/finger </summary>
            [XmlAttribute] public double SizeTolerance = 0.5;

            /// <summary> Tolérence sur la position du notch/pad/finger, en ° </summary>
            [XmlAttribute] public double PositionToleranceDegree = 3;
            [XmlIgnore]
            public double PositionTolerance
            {
                get { return PositionToleranceDegree / 180 * Math.PI; }
                set { PositionToleranceDegree = value / Math.PI * 180; }
            }
        }

        ///////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sous-classe contenant les paramètres pour la recherche du Notch métallisé
        /// </summary>
        ///////////////////////////////////////////////////////////////////
        [Serializable]
        public class MetalNotchParameters
        {
            /// <summary> Tolérence sur la position du notch, en ° </summary>
            [XmlAttribute] public double NotchPositionToleranceDegree = 3;
            [XmlIgnore]
            public double NotchPositionTolerance
            {
                get { return NotchPositionToleranceDegree / 180 * Math.PI; }
                set { NotchPositionToleranceDegree = value / Math.PI * 180; }
            }

            [XmlAttribute] public int LargeSearchStep = 128;
            [XmlAttribute] public int FineSearchStep = 16;

            [XmlAttribute] public bool UsePreFiltering = true;
            [XmlAttribute] public int Smoothing = 85;

            [XmlAttribute] public bool AutosetPeakHeight = true;
            [XmlAttribute] public float MinPeakHeight = 20.0f;

            [XmlAttribute] public int WINDOWMINMAWSIGNSIZE = 8; // Quel joli nom ! ça vient de la V8

            [XmlAttribute] public int NeighborPeakDistance = 50;
            [XmlAttribute] public float CoherenceSigma = 70.0f;
        }
    }
}
