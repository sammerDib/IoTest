using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Service.Core.Flows.AutoFocus;
using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.PM.EME.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.EME.Service.Core.Test.Flow
{
    [TestClass]
    public class GetZFocusTest : TestWithMockedHardware<GetZFocusTest>, ITestWithPhotoLumAxes, ITestWithDistanceSensor
    {
        private GetZFocusConfiguration _getZFocusConfiguration = new GetZFocusConfiguration { StartZScan = 7.0, MaxZScan = 8.0, MinZScan = 6.0 };
        public Mock<PhotoLumAxes> SimulatedMotionAxes { get; set; }

        [TestMethod]
        public void ShouldBeInError_WhenDistanceIsOutOfRange()
        {
            // Arrange
            var getZFocusInput = new GetZFocusInput();
            getZFocusInput.TargetDistanceSensor = 7600;
            var flow = new GetZFocusFlow(getZFocusInput);
            flow.Configuration = _getZFocusConfiguration;
            
            var mockedDistances = new List<double>
            {
                10000,
                10000,
                10000,
                10000,
                10000,
                10000,
                10000,
                10000,
                10000,
                10000,
                7000.0,
                8000.0
            };
            
            SetupDistanceSensorMockSequence(mockedDistances);
            
            // Act
            var result = flow.Execute();
            
            // Assert
            Assert.AreEqual(FlowState.Error, result.Status.State);
            Assert.AreEqual("Failed to reach the target height within 10 attempts.", result.Status.Message);
        }

        [TestMethod]
        public void ShouldBeInError_WhenDistanceTargetIsNotFound()
        {
            // Arrange
            var getZFocusInput = new GetZFocusInput();
            getZFocusInput.TargetDistanceSensor = 7600;
            var flow = new GetZFocusFlow(getZFocusInput);
            flow.Configuration = _getZFocusConfiguration;
            
            var mockedDistances = new List<double>
            {
                9000,
                7000,
                8000,
                5600,
                4200,
                8500,
                6300,
                8600,
                6400,
                8000,
                5600,
                5600
            };
            
            SetupDistanceSensorMockSequence(mockedDistances);

            // Act
            var result = flow.Execute();

            // Assert
            Assert.AreEqual(FlowState.Error, result.Status.State);
            Assert.AreEqual("Failed to reach the target height within 10 attempts.", result.Status.Message);
        }

        [TestMethod]
        public void ShouldBeInError_WhenFocusIsOutsideOfAllowedRange()
        {
            // Arrange
            var getZFocusInput = new GetZFocusInput { TargetDistanceSensor = 3000 };
            var flow = new GetZFocusFlow(getZFocusInput);
            flow.Configuration = _getZFocusConfiguration;

            var mockedDistances = new List<double>
            {
                7000,
                6000,
                5000,
                7000,
                5000,
                7000,
                5000,
                7000,
                5000,
                7000,
                5000,
                7000
            };
            
            SetupDistanceSensorMockSequence(mockedDistances);

            // Act
            var result = flow.Execute();

            // Assert
            Assert.AreEqual(FlowState.Error, result.Status.State);
            Assert.AreEqual("Failed to reach the target height within 10 attempts.", result.Status.Message);
        }
        
        
        private static void SetupDistanceSensorMockSequence(List<double> mockedDistances)
        {
            var sequentialResult = TestWithDistanceSensorHelper.s_distanceSensorMock.SetupSequence(distanceSensor => distanceSensor.GetDistanceSensorHeight());
            foreach (double value in mockedDistances.ToList())
            {
                sequentialResult.Returns(value);
            }
        }
    }
}
