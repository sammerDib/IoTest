using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Profile1D;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Shared
{
    [DataContract]
    [XmlInclude(typeof(AFCameraConfiguration))]
    [XmlInclude(typeof(AFLiseConfiguration))]
    [XmlInclude(typeof(AirGapLiseConfiguration))]
    [XmlInclude(typeof(AlignmentMarksConfiguration))]
    [XmlInclude(typeof(AutolightConfiguration))]
    [XmlInclude(typeof(BareWaferAlignmentConfiguration))]
    [XmlInclude(typeof(BareWaferAlignmentImageConfiguration))]
    [XmlInclude(typeof(CheckPatternRecConfiguration))]
    [XmlInclude(typeof(DieAndStreetSizesConfiguration))]
    [XmlInclude(typeof(EllipseCriticalDimensionConfiguration))]
    [XmlInclude(typeof(CircleCriticalDimensionConfiguration))]
    [XmlInclude(typeof(CircleMetroCDConfiguration))]
    [XmlInclude(typeof(ImagePreprocessingConfiguration))]
    [XmlInclude(typeof(MeasureDualLiseConfiguration))]
    [XmlInclude(typeof(MeasureLiseConfiguration))]
    [XmlInclude(typeof(MultipleMeasuresLiseConfiguration))]
    [XmlInclude(typeof(PatternRecConfiguration))]
    [XmlInclude(typeof(PSIConfiguration))]
    [XmlInclude(typeof(VSIConfiguration))]
    [XmlInclude(typeof(TSVConfiguration))]
    [XmlInclude(typeof(TSVDepthConfiguration))]
    [XmlInclude(typeof(WaferMapConfiguration))]
    [XmlInclude(typeof(Profile1DConfiguration))]
    public class FlowsConfiguration : IFlowsConfiguration
    {
        public static FlowsConfiguration Init(IPMServiceConfigurationManager configManager)
        {
            FlowsConfiguration flowsConfiguration;
            if (File.Exists(configManager.FlowsConfigurationFilePath))
            {
                flowsConfiguration = XML.Deserialize<FlowsConfiguration>(configManager.FlowsConfigurationFilePath);
            }
            else
            {
                flowsConfiguration = new FlowsConfiguration();
                flowsConfiguration.Flows = new List<FlowConfigurationBase>();

                // Uncomment to generate file with defaut values
                /*flowConfiguration.Flows.Add(new AFCameraConfiguration());
                flowConfiguration.Flows.Add(new AFLiseConfiguration());
                flowConfiguration.Flows.Add(new AutolightConfiguration());
                flowConfiguration.Flows.Add(new PatternRecConfiguration());
                flowConfiguration.Flows.Add(new BareWaferAlignmentConfiguration());
                XML.Serialize(flowConfiguration, path);*/
            }
            setWriteReportConfiguration(configManager.AllFlowsReportMode, flowsConfiguration);

            return flowsConfiguration;
        }

        [XmlAttribute]
        public string Version { get; set; } = "1.0.0";

        [DataMember]
        public List<FlowConfigurationBase> Flows { get; set; }

        private static void setWriteReportConfiguration(FlowReportConfiguration allFlowReportConfiguration, FlowsConfiguration flowsConfiguration)
        {
            foreach(var flow in flowsConfiguration.Flows)
            {
                if (flow.WriteReportMode < allFlowReportConfiguration)
                {
                    flow.WriteReportMode = allFlowReportConfiguration;
                }
            }
        }
    }
}
