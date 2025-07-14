using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.ControllersTests
{
    [TestCategory("RequiresHardware")]
    [TestClass]
    public class PIE709ControllerTests
    {
        public PIE709Controller Controller;
        public Mock<PIE709ControllerApiWrapper> API_Mock;

        public Mock<IGlobalStatusServer> GlobalStatusServer_Mock = new Mock<IGlobalStatusServer>();
        public Mock<ILogger> Logger_Mock = new Mock<ILogger>();

        [TestInitialize]
        public void BeforeEach()
        {
            var controllerConfig = new PIE709ControllerConfig()
            {
                Name = "PIE709Controller-Test",
                DeviceID = "PIE709-Test",
            };

            API_Mock = new Mock<PIE709ControllerApiWrapper>(controllerConfig);
            API_Mock.SetReturnsDefault((int)OperationResult.Success); // default return value: any method call returns a success (1)

            Controller = new PIE709Controller(controllerConfig, GlobalStatusServer_Mock.Object, Logger_Mock.Object, API_Mock.Object);
        }

        [TestMethod]
        public void When_ControllerHasMoreThanOneAxis_ItThrowsAnException()
        {
            // Given
            var pointlessAxis = new PiezoAxis(new PiezoAxisConfig(), null);
            var twoAxisList = new List<IAxis>() { pointlessAxis, pointlessAxis };

            // Then
            Assert.ThrowsException<Exception>(() => Controller.AxesList = twoAxisList);
        }

        [TestMethod]
        public void When_ControllerAPICallReturnsAFailResult_ApiErrorHandlerThrowsAnException()
        {
            // Given
            int failedOperationResult = (int)OperationResult.Fail;

            // Then
            Assert.ThrowsException<Exception>(() => Controller.Api.ErrorHandler(failedOperationResult));
        }

        [DataTestMethod]
        [DataRow("Z", "Z")]
        [DataRow("1", "1")]
        [DataRow("X\n", "X")]
        public void When_qSAI_ReturnsAOneCharAxisName_GetAxisNameReturnsAValidName(string apiResponse, string expectedName)
        {
            // Given
            API_Mock.Setup(api => api.qSAI(It.IsAny<StringBuilder>()))
                    .Callback<StringBuilder>((axisNames) => axisNames.Append(apiResponse));

            // When
            string axisName = Controller.GetAxisName();

            // Then
            Assert.AreEqual(expectedName, axisName);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("X\nY\n")]
        public void When_qSAI_ReturnsZeroOrMoreThanOneAxisName_GetAxisNameThrowsAnException(string apiResponse)
        {
            // Given
            API_Mock.Setup(api => api.qSAI(It.IsAny<StringBuilder>()))
                    .Callback<StringBuilder>((axisNames) => axisNames.Append(apiResponse));

            // Then
            Assert.ThrowsException<Exception>(() => Controller.GetAxisName());
        }

        [TestMethod]
        public void When_APIFailsToConnect_ConnectThrowsAnException()
        {
            // Given
            API_Mock.Setup(api => api.ConnectionID).Returns(-1);

            // Then
            Assert.ThrowsException<Exception>(() => Controller.Connect());
        }

        [TestMethod]
        public void When_CallingGetVelocity_ItReturnsTheRightValue()
        {
            // Given
            double expectedVelocity = 1234.5;
            API_Mock.Setup(api => api.qVEL(It.IsAny<double[]>()))
                    .Callback<double[]>((vel) => vel[0] = expectedVelocity * 1000);

            // When
            double velocity = Controller.GetSpeed();

            //Then
            Assert.AreEqual(expectedVelocity, velocity);
        }

        [TestMethod]
        public void When_CallingSetSpeed_ServoModeShouldBeTurnOnIfItWasOff()
        {
            // Given
            API_Mock.Setup(api => api.qSVO(It.IsAny<int[]>()))
                    .Callback<int[]>((servoMode) => servoMode[0] = (int)ServoMode.Off);

            // When
            Controller.SetSpeed(10);

            // Then
            API_Mock.Verify(apiCall => apiCall.SVO(new int[] { (int)ServoMode.On }), Times.Once);
        }

        [DataTestMethod]
        [DataRow(15, 10, 15)]
        [DataRow(10, 10, 10)]
        [DataRow(05, 10, 10)]
        public void When_CallingGetPositionMin_AxisConfigValuesShouldTakePrecedenceOverThoseReturnedByTheController(double axisPositionMin_um, double controllerPositionMin_um, double expectedPositionMin_um)
        {
            // Given
            Controller.AxesList = new List<IAxis>()
            {
                new PiezoAxis(new PiezoAxisConfig() { PositionMin = axisPositionMin_um.Micrometers() }, Logger_Mock.Object),
            };

            API_Mock.Setup(api => api.qTMN(It.IsAny<double[]>()))
                    .Callback<double[]>((minPositions) => minPositions[0] = controllerPositionMin_um);

            // When
            double minPosition_um = Controller.GetPositionMin().Micrometers;

            // Then
            Assert.AreEqual(expectedPositionMin_um, minPosition_um);
        }

        [DataTestMethod]
        [DataRow(85, 90, 85)]
        [DataRow(90, 90, 90)]
        [DataRow(95, 90, 90)]
        public void When_CallingGetMaxPosition_AxisConfigValuesShouldTakePrecedenceOverThoseReturnedByTheController(double axisMaxPosition_um, double controllerMaxPosition_um, double expectedMaxPosition_um)
        {
            // Given
            Controller.AxesList = new List<IAxis>()
            {
                new PiezoAxis(new PiezoAxisConfig() { PositionMax = axisMaxPosition_um.Micrometers() }, Logger_Mock.Object),
            };

            API_Mock.Setup(api => api.qTMX(It.IsAny<double[]>()))
                    .Callback<double[]>((maxPositions) => maxPositions[0] = controllerMaxPosition_um);

            // When
            double maxPosition_um = Controller.GetPositionMax().Micrometers;

            // Then
            Assert.AreEqual(expectedMaxPosition_um, maxPosition_um);
        }
    }
}
