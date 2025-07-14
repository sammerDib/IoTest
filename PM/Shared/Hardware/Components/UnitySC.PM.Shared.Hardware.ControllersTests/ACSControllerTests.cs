using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.ControllersTests
{
    [TestClass]
    public class ACSControllerTests
    {
        public Mock<ILogger> _logger_Mock = new Mock<ILogger>();

        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        public void Test_ConstructorExceptions()
        {
            ACSController acsCtrl = null;
            ILogger logger = _logger_Mock.Object;

            // Test Logger null
            ACSControllerConfig acsCtrlConfig = GetConfigurationStandard();
            Exception exceptionRaised = Assert.ThrowsException<Exception>(() =>
            {
                acsCtrl = new ACSController(acsCtrlConfig, null, null);
            });
            Assert.IsTrue(exceptionRaised.Message == "Invalid Logger (NULL)!");

            // Test DeviceID
            acsCtrlConfig = GetConfigurationStandard();
            acsCtrlConfig.DeviceID = "";
            exceptionRaised = Assert.ThrowsException<Exception>(() =>
            {
                acsCtrl = new ACSController(acsCtrlConfig, null, logger);
            });
            Assert.IsTrue(exceptionRaised.Message == "Invalid DeviceID for controller in configuration");

            // Test Name
            acsCtrlConfig = GetConfigurationStandard();
            acsCtrlConfig.Name = "";
            exceptionRaised = Assert.ThrowsException<Exception>(() =>
            {
                acsCtrl = new ACSController(acsCtrlConfig, null, logger);
            });
            Assert.IsTrue(exceptionRaised.Message == "Invalid Name for controller in configuration");

            // Test Axis parameters
            acsCtrlConfig = GetConfigurationStandard();
            ACSAxisIDLink newLink = new ACSAxisIDLink();
            newLink.ACSID = "Invalid parameter";
            newLink.AxisID = "Z";
            acsCtrlConfig.ACSAxisIDLinks.Add(newLink);

            exceptionRaised = Assert.ThrowsException<Exception>(() =>
            {
                acsCtrl = new ACSController(acsCtrlConfig, null, logger);
            });
            Assert.IsTrue(exceptionRaised.Message == "[ACSMotion]Bad axis [Z] parameters in configuration. Controller creation failed !");

            // Test Configuration type
            ControllerConfig acsCtrlConfig2 = new ControllerConfig();
            acsCtrlConfig2.DeviceID = "ACSMotion";
            acsCtrlConfig2.Name = "ACSMotion controller";
            exceptionRaised = Assert.ThrowsException<Exception>(() =>
            {
                acsCtrl = new ACSController(acsCtrlConfig2, null, logger);
            });
            Assert.IsTrue(exceptionRaised.Message == "[]Bad controller configuration type. Controller creation failed !");

            // Test configuration null
            exceptionRaised = Assert.ThrowsException<Exception>(() =>
            {
                acsCtrl = new ACSController(null, null, logger);
            });
            Assert.IsTrue(exceptionRaised.Message == "Invalid Controller configuration (NULL)");
        }

        [TestMethod]
        public void Test_Constructor()
        {
            ACSController acsCtrl = null;
            ILogger logger = _logger_Mock.Object;

            // Test build ok
            ACSControllerConfig acsCtrlConfig = GetConfigurationStandard();
            acsCtrlConfig = GetConfigurationStandard();
            acsCtrl = new ACSController(acsCtrlConfig, null, logger);

            Assert.IsTrue(acsCtrl.ACSID.Count == 4);
            Assert.IsTrue(acsCtrl.ChuckConfiguration == null);
            Assert.IsTrue(acsCtrl.AxesList.Count == 0);
            Assert.IsTrue(acsCtrl.ControllerConfiguration != null);
            Assert.IsTrue(acsCtrl.Family == DeviceFamily.Controller);
            Assert.IsTrue(acsCtrl.DeviceID == "ACSMotion");
            Assert.IsTrue(acsCtrl.Name == "ACSMotion controller");
            Assert.IsTrue(acsCtrl.State.Status == DeviceStatus.Unknown);
            Assert.IsTrue(acsCtrl.State.StatusMessage == null);
        }

        public ACSControllerConfig GetConfigurationStandard()
        {
            ACSControllerConfig acsCtrlConfig = new ACSControllerConfig();
            acsCtrlConfig.EthernetCom.IP = "127.0.0.1";
            acsCtrlConfig.EthernetCom.Port = 3000;
            acsCtrlConfig.ControllerTypeName = "";
            acsCtrlConfig.DeviceID = "ACSMotion";
            acsCtrlConfig.IsEnabled = true;
            acsCtrlConfig.Name = "ACSMotion controller";

            acsCtrlConfig.ACSAxisIDLinks = new List<ACSAxisIDLink>();
            ACSAxisIDLink newLink = new ACSAxisIDLink();
            newLink.ACSID = "ACSC_AXIS_2";
            newLink.AxisID = "X";
            acsCtrlConfig.ACSAxisIDLinks.Add(newLink);
            newLink = new ACSAxisIDLink();
            newLink.ACSID = "ACSC_AXIS_0";
            newLink.AxisID = "Y";
            acsCtrlConfig.ACSAxisIDLinks.Add(newLink);
            newLink = new ACSAxisIDLink();
            newLink.ACSID = "ACSC_AXIS_5";
            newLink.AxisID = "ZTop";
            acsCtrlConfig.ACSAxisIDLinks.Add(newLink);
            newLink = new ACSAxisIDLink();
            newLink.ACSID = "ACSC_AXIS_4";
            newLink.AxisID = "ZBottom";
            acsCtrlConfig.ACSAxisIDLinks.Add(newLink);

            return acsCtrlConfig;
        }
    }
}
