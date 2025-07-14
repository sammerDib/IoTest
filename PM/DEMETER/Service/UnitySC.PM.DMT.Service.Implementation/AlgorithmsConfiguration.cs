using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Service.Implementation
{
    /// <summary>
    ///     Définition du fichier XML de configuration des algorithmes
    /// </summary>
    [Serializable]
    public class AlgorithmsConfiguration
    {

        [XmlElement("CurvatureDynamicsCalibration")]
        public List<CurvatureDynamicsCalibrationConfiguration> CurvatureDynamicsCalibrationConfigurations;

        [XmlElement("DMTCalTransform")]
        public List<DMTCalTransform> DMTCalTransforms;

        [XmlElement("ExposureTimeCalibration")]
        public List<ExposureCalibrationSetting> ExposureCalibration;

        public ImageConfig Image = new ImageConfig();

        [Serializable]
        public class CurvatureDynamicsCalibrationConfiguration
        {
            [XmlAttribute]
            public Side Side;
            
            [XmlAttribute]
            public bool IsEnabled;
        }

        [Serializable]
        public class DMTCalTransform
        {
            [XmlAttribute] 
            public bool IsEnabled;

            [XmlAttribute] 
            public Side Side;
        }

        [Serializable]
        public class ExposureCalibrationSetting
        {
            [XmlAttribute] 
            public bool IsEnabled;

            [XmlAttribute] 
            public Side Side;
        }

        [Serializable]
        public class ImageConfig
        {
            /// <summary>
            ///     SaturationThresholdPercent, en secondes
            /// </summary>
            [XmlAttribute] 
            public double SaturationThresholdPercent = 0.5;
        }
    }
}
