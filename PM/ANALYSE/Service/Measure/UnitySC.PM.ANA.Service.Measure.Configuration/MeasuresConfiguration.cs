using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared;
using UnitySC.Shared.Configuration;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Measure.Configuration
{
    [XmlInclude(typeof(MeasureNanoTopoConfiguration))]
    [XmlInclude(typeof(MeasureTopoConfiguration))]
    [XmlInclude(typeof(MeasureTSVConfiguration))]
    [XmlInclude(typeof(MeasureXYCalibrationConfiguration))]
    [XmlInclude(typeof(MeasureBowConfiguration))]
    [XmlInclude(typeof(MeasureWarpConfiguration))]
    [XmlInclude(typeof(MeasureStepConfiguration))]
    [XmlInclude(typeof(MeasureEdgeTrimConfiguration))]
    [XmlInclude(typeof(MeasureTrenchConfiguration))]
    [XmlInclude(typeof(MeasureThicknessConfiguration))]

    public class MeasuresConfiguration
    {
        [XmlAttribute]
        public string Version { get; set; } = "1.0.0";

        private const string MeasuresConfigurationFileName = "MeasuresConfiguration.xml";

        public static MeasuresConfiguration Init(IPMServiceConfigurationManager configManager)
        {
            string measuresConfigurationFilePath = Path.Combine(configManager.ConfigurationFolderPath, MeasuresConfigurationFileName);
            MeasuresConfiguration measuresConfiguration = null;
            if (File.Exists(measuresConfigurationFilePath))
            {
                measuresConfiguration = XML.Deserialize<MeasuresConfiguration>(measuresConfigurationFilePath);
            }
            else
            {
                measuresConfiguration = new MeasuresConfiguration();
                measuresConfiguration.Measures = new List<MeasureConfigurationBase>();

                // Uncomment to add configuration in XML
                //var measureNanoTopoConfiguration = new MeasureNanoTopoConfiguration();
                //measureNanoTopoConfiguration.Acquisitions = new List<AcquisitionConfiguration>();
                //measureNanoTopoConfiguration.Acquisitions.Add(new AcquisitionConfiguration() { ImagesPerStep = 1, Resolution =NanoTopoAcquisitionResolution.Low });
                //measureNanoTopoConfiguration.Acquisitions.Add(new AcquisitionConfiguration() { ImagesPerStep = 5, Resolution = NanoTopoAcquisitionResolution.Low });
                //measureNanoTopoConfiguration.Acquisitions.Add(new AcquisitionConfiguration() { ImagesPerStep = 10, Resolution = NanoTopoAcquisitionResolution.Low });
                //measureNanoTopoConfiguration.Algos = new List<AlgoConfiguration>();
                //measureNanoTopoConfiguration.Algos.Add(new AlgoConfiguration() { Name= "Guided by quality", PhaseCalculation = PhaseCalculationAlgo.Hariharan, PhaseUnwrapping = Interface.Algo.PSIInput.PhaseUnwrappingAlgo.QualityGuidedByPseudoCorrelation, FactorBetweenWavelengthAndStepSize = 0.125, StepCount = 7 });
                //measureNanoTopoConfiguration.Algos.Add(new AlgoConfiguration() { Name = "Standard", PhaseCalculation = Interface.Algo.PSIInput.PhaseCalculationAlgo.Hariharan, PhaseUnwrapping = Interface.Algo.PSIInput.PhaseUnwrappingAlgo.Goldstein, FactorBetweenWavelengthAndStepSize = 0.125, StepCount = 7 });
                //measureNanoTopoConfiguration.MinCompatibleLightWavelength = 620.Nanometers();
                //measureNanoTopoConfiguration.MaxCompatibleLightWavelength = 750.Nanometers();
                //measuresConfiguration.Measures.Add(measureNanoTopoConfiguration);
                //XML.Serialize(measuresConfiguration, measuresConfigurationFilePath);
            }

            return measuresConfiguration;
        }

        public MeasureConfigurationBase GetMeasureConfiguration(MeasureType measureType)
        {
           switch (measureType) 
           { 
                case MeasureType.Bow:
                    return Measures.OfType<MeasureBowConfiguration>().SingleOrDefault();
                case MeasureType.NanoTopo:
                    return Measures.OfType<MeasureNanoTopoConfiguration>().SingleOrDefault();
                case MeasureType.TSV:
                    return Measures.OfType<MeasureTSVConfiguration>().SingleOrDefault();
                case MeasureType.Topography:
                    return Measures.OfType<MeasureTopoConfiguration>().SingleOrDefault();
                case MeasureType.XYCalibration:
                    return Measures.OfType<MeasureXYCalibrationConfiguration>().SingleOrDefault();
                case MeasureType.Warp:
                    return Measures.OfType<MeasureWarpConfiguration>().SingleOrDefault();
                case MeasureType.Step:
                    return Measures.OfType<MeasureStepConfiguration>().SingleOrDefault();
                case MeasureType.EdgeTrim:
                    return Measures.OfType<MeasureEdgeTrimConfiguration>().SingleOrDefault();
                case MeasureType.Trench:
                    return Measures.OfType<MeasureTrenchConfiguration>().SingleOrDefault();
                case MeasureType.Thickness:
                    return Measures.OfType<MeasureThicknessConfiguration>().SingleOrDefault();
                default:
                    throw new Exception($"The measure type {measureType} doesn't have an associated configuration");
            }
        }

        public List<MeasureConfigurationBase> Measures { get; set; }

        public List<MeasureType> AuthorizedMeasures { get; set; }

        public bool LocalBackupOfAllResults { get; set; }

        public uint MinuteBetweenTwoDualLiseCalibration { get; set; }
    }
}
