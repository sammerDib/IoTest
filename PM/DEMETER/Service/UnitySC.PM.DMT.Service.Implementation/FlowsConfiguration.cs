using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.Service.Implementation
{
    [Serializable]
    [XmlInclude(typeof(AutoExposureConfiguration))]
    [XmlInclude(typeof(SaveImageConfiguration))]
    [XmlInclude(typeof(CorrectorConfiguration))]
    [XmlInclude(typeof(AcquirePhaseImagesForPeriodAndDirectionConfiguration))]
    [XmlInclude(typeof(ComputePhaseMapAndMaskForPeriodAndDirectionConfiguration))]
    [XmlInclude(typeof(ComputeRawCurvatureMapForPeriodAndDirectionConfiguration))]
    [XmlInclude(typeof(AdjustCurvatureDynamicsForRawCurvatureMapConfiguration))]
    [XmlInclude(typeof(ComputeLowAngleDarkFieldImageConfiguration))]
    [XmlInclude(typeof(CurvatureDynamicsCalibrationConfiguration))]
    [XmlInclude(typeof(ComputeUnwrappedPhaseMapForDirectionConfiguration))]
    [XmlInclude(typeof(GlobalTopoSystemCalibrationConfiguration))]
    [XmlInclude(typeof(GlobalTopoCameraCalibrationConfiguration))]
    [XmlInclude(typeof(SystemUniformityCalibrationConfiguration))]
    [XmlInclude(typeof(ComputeNanoTopoConfiguration))]
    public class FlowsConfiguration : IFlowsConfiguration
    {
        public List<FlowConfigurationBase> Flows { get; set; }

        public static FlowsConfiguration Init(IDMTServiceConfigurationManager configurationManager)
        {
            FlowsConfiguration flowsConfiguration;
            string fileName = configurationManager.FlowsConfigurationFilePath;
            if (File.Exists(fileName))
            {
                flowsConfiguration = XML.Deserialize<FlowsConfiguration>(fileName);
            }
            else
            {
                flowsConfiguration = new FlowsConfiguration { Flows = new List<FlowConfigurationBase>() };
            }

            SetWriteReportConfiguration(configurationManager.AllFlowsReportMode, flowsConfiguration);

            return flowsConfiguration;
        }

        private static void SetWriteReportConfiguration(
            FlowReportConfiguration allFlowReportConfiguration,
            FlowsConfiguration flowsConfiguration)
        {
            foreach (var flow in flowsConfiguration.Flows)
            {
                if (flow.WriteReportMode < allFlowReportConfiguration)
                {
                    flow.WriteReportMode = allFlowReportConfiguration;
                }
            }
        }

        public T GetConfiguration<T>() where T : FlowConfigurationBase
        {
            return Flows.OfType<T>().SingleOrDefault();
        }
    }
}
