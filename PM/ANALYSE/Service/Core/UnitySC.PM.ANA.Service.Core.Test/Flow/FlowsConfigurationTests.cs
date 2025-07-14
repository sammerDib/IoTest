using System;
using System.IO;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Core.Flow.Test
{
    [TestClass]
    public class FlowsConfigurationTests
    {
        [TestMethod]
        public void Expect_Init_to_return_an_empty_flows_list_when_given_an_incorrect_path()
        {
            // Given
            var configMock = new Mock<IPMServiceConfigurationManager>();
            configMock.SetupGet(configManager => configManager.AllFlowsReportMode).Returns(FlowReportConfiguration.NeverWrite);
            configMock.SetupGet(configManager => configManager.FlowsConfigurationFilePath).Returns("");

            // When
            var flowsConfiguration = FlowsConfiguration.Init(configMock.Object);

            // Then
            flowsConfiguration.Flows.Should().BeEmpty();
        }

        [TestMethod]
        public void Expect_Init_to_correctly_deserialize_flows_configurations()
        {
            // SetUp
            string flowConfigurationPath = prepareFlowsConfigurationTempFile();

            // Given
            var configMock = new Mock<IPMServiceConfigurationManager>();
            configMock.SetupGet(configManager => configManager.AllFlowsReportMode).Returns(FlowReportConfiguration.NeverWrite);
            configMock.SetupGet(configManager => configManager.FlowsConfigurationFilePath).Returns(flowConfigurationPath);

            // When
            var flowsConfiguration = FlowsConfiguration.Init(configMock.Object);

            // Then
            flowsConfiguration.Flows.Should().HaveCount(3).And.SatisfyRespectively(
                flow =>
                {
                    flow.GetType().Should().Be(typeof(AirGapLiseConfiguration));
                    flow.WriteReportMode.Should().Be(FlowReportConfiguration.AlwaysWrite);
                },
                flow =>
                {
                    flow.GetType().Should().Be(typeof(AlignmentMarksConfiguration));
                    flow.WriteReportMode.Should().Be(FlowReportConfiguration.WriteOnError);
                },
                flow =>
                {
                    flow.GetType().Should().Be(typeof(AutolightConfiguration));
                    flow.WriteReportMode.Should().Be(FlowReportConfiguration.NeverWrite);
                }
            );

            // TearDown
            File.Delete(flowConfigurationPath);
        }

        [TestMethod]
        public void Expect_Init_to_correctly_overwrite_flows_configuration_when_config_is_AlwaysWrite()
        {
            // SetUp
            string flowConfigurationPath = prepareFlowsConfigurationTempFile();

            // Given
            var configMock = new Mock<IPMServiceConfigurationManager>();
            configMock.SetupGet(configManager => configManager.AllFlowsReportMode).Returns(FlowReportConfiguration.AlwaysWrite);
            configMock.SetupGet(configManager => configManager.FlowsConfigurationFilePath).Returns(flowConfigurationPath);

            // When
            var flowsConfiguration = FlowsConfiguration.Init(configMock.Object);

            // Then
            flowsConfiguration.Flows.Should().AllSatisfy(flow => flow.WriteReportMode.Should().Be(FlowReportConfiguration.AlwaysWrite));

            // TearDown
            File.Delete(flowConfigurationPath);
        }

        [TestMethod]
        public void Expect_Init_to_correctly_overwrite_flows_configuration_when_config_is_WriteOnError()
        {
            // SetUp
            string flowConfigurationPath = prepareFlowsConfigurationTempFile();

            // Given
            var configMock = new Mock<IPMServiceConfigurationManager>();
            configMock.SetupGet(configManager => configManager.AllFlowsReportMode).Returns(FlowReportConfiguration.WriteOnError);
            configMock.SetupGet(configManager => configManager.FlowsConfigurationFilePath).Returns(flowConfigurationPath);

            // When
            var flowsConfiguration = FlowsConfiguration.Init(configMock.Object);

            // Then
            flowsConfiguration.Flows.Should().SatisfyRespectively(
                flow =>
                {
                    flow.GetType().Should().Be(typeof(AirGapLiseConfiguration));
                    flow.WriteReportMode.Should().Be(FlowReportConfiguration.AlwaysWrite);
                },
                flow =>
                {
                    flow.GetType().Should().Be(typeof(AlignmentMarksConfiguration));
                    flow.WriteReportMode.Should().Be(FlowReportConfiguration.WriteOnError);
                },
                flow =>
                {
                    flow.GetType().Should().Be(typeof(AutolightConfiguration));
                    flow.WriteReportMode.Should().Be(FlowReportConfiguration.WriteOnError);
                }
            );

            // TearDown
            File.Delete(flowConfigurationPath);
        }

        private static string prepareFlowsConfigurationTempFile()
        {
            var flowConfigurationPath = Path.Combine(Path.GetTempPath(), "FlowConfiguration-" + Guid.NewGuid() + ".xml");
            using (StreamWriter sw = File.AppendText(flowConfigurationPath))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sw.WriteLine("<FlowsConfiguration Version=\"1.0.0\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
                sw.WriteLine("  <Flows>");
                sw.WriteLine("    <FlowConfigurationBase xsi:type=\"AirGapLiseConfiguration\">");
                sw.WriteLine("      <WriteReportMode>AlwaysWrite</WriteReportMode>");
                sw.WriteLine("    </FlowConfigurationBase>");
                sw.WriteLine("    <FlowConfigurationBase xsi:type=\"AlignmentMarksConfiguration\">");
                sw.WriteLine("      <WriteReportMode>WriteOnError</WriteReportMode>");
                sw.WriteLine("    </FlowConfigurationBase>");
                sw.WriteLine("    <FlowConfigurationBase xsi:type=\"AutolightConfiguration\">");
                sw.WriteLine("      <WriteReportMode>NeverWrite</WriteReportMode>");
                sw.WriteLine("    </FlowConfigurationBase>");
                sw.WriteLine("  </Flows>");
                sw.WriteLine("</FlowsConfiguration>");
            }

            return flowConfigurationPath;
        }
    }
}
