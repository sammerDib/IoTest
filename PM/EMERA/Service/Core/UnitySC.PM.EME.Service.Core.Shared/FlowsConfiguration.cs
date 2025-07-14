using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using Serilog;

using UnitySC.PM.EME.Service.Interface;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Core.Shared
{
    [DataContract]
    [XmlInclude(typeof(AutoFocusCameraConfiguration))]
    [XmlInclude(typeof(PatternRecConfiguration))]
    [XmlInclude(typeof(AxisOrthogonalityConfiguration))]
    [XmlInclude(typeof(MultiSizeChuckConfiguration))]
    [XmlInclude(typeof(VignettingConfiguration))]
    [XmlInclude(typeof(PixelSizeComputationConfiguration))]
    [XmlInclude(typeof(DistortionConfiguration))]
    [XmlInclude(typeof(AutoExposureConfiguration))]
    [XmlInclude(typeof(GetZFocusConfiguration))]
    [XmlInclude(typeof(DistanceSensorCalibrationConfiguration))]
    public class FlowsConfiguration : IFlowsConfiguration
    {
        public static FlowsConfiguration Init(IEMEServiceConfigurationManager configManager)
        {
            if (!File.Exists(configManager.FlowsConfigurationFilePath))
            {
                string errorMessage = $"Flows configuration file could not be found in path {configManager.FlowsConfigurationFilePath}";
                ClassLocator.Default.GetInstance<ILogger>().Error(errorMessage);
                throw new Exception(errorMessage);
            }
            
            var flowsConfiguration = XML.Deserialize<FlowsConfiguration>(configManager.FlowsConfigurationFilePath);
            SetWriteReportConfiguration(configManager.AllFlowsReportMode, flowsConfiguration);
            return flowsConfiguration;
        }

        [XmlAttribute]
        public string Version { get; set; } = "1.0.0";

        [DataMember]
        public double ImageScale { get; set; } = 0.2;

        [DataMember]
        public List<FlowConfigurationBase> Flows { get; set; }

        private static void SetWriteReportConfiguration(FlowReportConfiguration allFlowReportConfiguration, FlowsConfiguration flowsConfiguration)
        {
            foreach (var flow in flowsConfiguration.Flows.Where(flow => flow.WriteReportMode < allFlowReportConfiguration))
            {
                flow.WriteReportMode = allFlowReportConfiguration;
            }
        }
    }
}
