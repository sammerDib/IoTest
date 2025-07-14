using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.Shared.Test
{
    [TestClass]
    public class ConfigurationManagerTest
    {
        #region Test section : SERVICE

        [TestMethod]
        public void Service_Configuration_Expect_default_values_when_args_is_null()
        {
            var configurationManager = new PMServiceConfigurationManager(null);
            Assert.IsFalse(configurationManager.HardwaresAreSimulated, "Hardware should be not simulated");
            Assert.IsFalse(configurationManager.FlowsAreSimulated, "Flows should be not simulated");
            Assert.AreEqual(FlowReportConfiguration.NeverWrite, configurationManager.AllFlowsReportMode, "Flows report should be NeverWrite");
            Assert.AreEqual(Environment.MachineName, configurationManager.ConfigurationName, "Configuration name should be machine name");
        }

        [TestMethod]
        public void Service_Configuration_Expect_input_values_with_full_args()
        {
            var args = "--config NST5 --simulateHardware --simulateFlow --reportAllFlow".Split(' ');
            var configurationManager = new PMServiceConfigurationManager(args);
            Assert.IsTrue(configurationManager.HardwaresAreSimulated, "Hardware should be simulated");
            Assert.IsTrue(configurationManager.FlowsAreSimulated, "Flows should be simulated");
            Assert.AreEqual(FlowReportConfiguration.AlwaysWrite, configurationManager.AllFlowsReportMode, "Flows report should be AlwaysWrite");
            Assert.AreEqual("NST5", configurationManager.ConfigurationName, "Configuration name should be NST5");
        }

        [TestMethod]
        public void Service_Configuration_Expect_input_values_with_full_args_re()
        {
            var args = "--config NST5 --simulateHardware --simulateFlow --reportOnlyError".Split(' ');
            var configurationManager = new PMServiceConfigurationManager(args);
            Assert.IsTrue(configurationManager.HardwaresAreSimulated, "Hardware should be simulated");
            Assert.IsTrue(configurationManager.FlowsAreSimulated, "Flows should be simulated");
            Assert.AreEqual(FlowReportConfiguration.WriteOnError, configurationManager.AllFlowsReportMode, "Flows report should be WriteOnError");
            Assert.AreEqual("NST5", configurationManager.ConfigurationName, "Configuration name should be NST5");
        }

        [TestMethod]
        public void Service_Configuration_Expect_input_values_with_alias_args()
        {
            var args = "-c NST7 -sh -sf -rf".Split(' ');
            var configurationManager = new PMServiceConfigurationManager(args);
            Assert.IsTrue(configurationManager.HardwaresAreSimulated, "Hardware should be simulated");
            Assert.IsTrue(configurationManager.FlowsAreSimulated, "Flows should be simulated");
            Assert.AreEqual(FlowReportConfiguration.AlwaysWrite, configurationManager.AllFlowsReportMode, "Flows report should be AlwaysWrite");
            Assert.AreEqual("NST7", configurationManager.ConfigurationName, "Configuration name should be NST7");
        }

        [TestMethod]
        public void Service_Configuration_Expect_input_values_with_alias_args_re()
        {
            var args = "-c NST7 -sh -sf -re".Split(' ');
            var configurationManager = new PMServiceConfigurationManager(args);
            Assert.IsTrue(configurationManager.HardwaresAreSimulated, "Hardware should be simulated");
            Assert.IsTrue(configurationManager.FlowsAreSimulated, "Flows should be simulated");
            Assert.AreEqual(FlowReportConfiguration.WriteOnError, configurationManager.AllFlowsReportMode, "Flows report should be WriteOnError");
            Assert.AreEqual("NST7", configurationManager.ConfigurationName, "Configuration name should be NST7");
        }

        [TestMethod]
        public void Service_Configuration_Expect_Exception_with_bad_args()
        {
            var args = "-badArgs".Split(' ');
            Assert.ThrowsException<Exception>(() => new PMServiceConfigurationManager(args), "Bad args should raise exception");
        }


        [TestMethod]
        public void Service_Configuration_Expect_AllFlowReports_WriteOnError_when_reportOnlyError_flag_is_used()
        {
            string[] args = { "--reportOnlyError" };
            var configurationManager = new PMServiceConfigurationManager(args);
            Assert.AreEqual(FlowReportConfiguration.WriteOnError, configurationManager.AllFlowsReportMode, "Flows report should be WriteOnError");
        }

        [TestMethod]
        public void Service_Configuration_Expect_AllFlowReports_WriteOnError_when_re_flag_is_used()
        {
            string[] args = { "-re" };
            var configurationManager = new PMServiceConfigurationManager(args);
            Assert.AreEqual(FlowReportConfiguration.WriteOnError, configurationManager.AllFlowsReportMode, "Flows report should be WriteOnError");
        }

        #endregion
        #region Test section : CLIENT

        [TestMethod]
        public void Client_Configuration_Expect_default_values_when_args_is_null()
        {
            var configurationManager = new ClientConfigurationManager(null);
            Assert.AreEqual(Environment.MachineName, configurationManager.ConfigurationName, "Configuration name should be machine name");
        }

        [TestMethod]
        public void Client_Configuration_Expect_input_values_with_full_args()
        {
            var args = "--config NST5".Split(' ');
            var configurationManager = new ClientConfigurationManager(args);
            Assert.AreEqual("NST5", configurationManager.ConfigurationName, "Configuration name should be NST5");
        }

        [TestMethod]
        public void Client_Configuration_Expect_input_values_with_alias_args()
        {
            var args = "-c NST7".Split(' ');
            var configurationManager = new ClientConfigurationManager(args);
            Assert.AreEqual("NST7", configurationManager.ConfigurationName, "Configuration name should be NST7");
        }

        [TestMethod]
        public void Client_Configuration_Expect_Exception_with_bad_args()
        {
            var args = "-badArgs".Split(' ');
            Assert.ThrowsException<ArgumentException>(() => new ClientConfigurationManager(args), "Bad args should raise exception");
        }

        [TestMethod]
        public void Client_Configuration_Expect_Exception_with_no_args()
        {
            var args = "".Split();
            Assert.ThrowsException<ArgumentException>(() => new ClientConfigurationManager(args), "No args should raise exception");
        }

        #endregion
    }
}
