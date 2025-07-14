using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using FluentAssertions;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.MeasureCalibration;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.MeasureCalibration
{

    [TestClass]
    public class MeasureCalibrationManagerLiseHFTest : TestWithMockedHardware<MeasureCalibrationManagerLiseHFTest>, ITestWithAxes, ITestWithProbeLise, ITestWithChuck, ITestWithCamera
    {
        #region parameters

        private readonly uint _minuteBetweenTwoDualLiseCalibration = 10;
        private readonly CancellationToken _cancellationToken = new CancellationToken();
        public Mock<IAxes> SimulatedAxes { get; set; }
        public List<string> SimpleProbesLise { get; set; }
        public string LiseUpId { get; set; }
        public string LiseBottomId { get; set; }
        public string LiseHFId { get; set; }
        public string DualLiseId { get; set; }
        public double DefaultGain { get; set; }
        public Mock<ProbeLise> FakeLiseUp { get; set; }
        public Mock<ProbeLise> FakeLiseBottom { get; set; }
        public Mock<ProbeLise> FakeLiseHF{ get; set; }

        public Mock<IProbeDualLise> FakeDualLise { get; set; }
        public string ObjectiveUpId { get; set; }
        public string ObjectiveBottomId { get; set; }
        public Length PixelSizeX { get; set; }
        public Length PixelSizeY { get; set; }
        public Mock<ITestChuck> SimulatedChuck { get; set; }
        public string CameraUpId { get; set; }
        public string CameraBottomId { get; set; }
        public Mock<CameraBase> SimulatedCameraUp { get; set; }
        public Mock<CameraBase> SimulatedCameraBottom { get; set; }
        public double ThicknessThresholdInTheAir { get; set; }

        #endregion parameters

        [TestMethod]
        public void CalibrationManager_IsExpired_should_return_true()
        {

            //Arrange
            var mockConf = Mock.Of<ProbeLiseHFConfig>(c => c.CalibrationValidityPeriodMinutes == 10);
            var calibrationManagerLiseHF = new ProbeCalibrationManagerLiseHF(mockConf, _cancellationToken);
            Type type = typeof(ProbeCalibrationManagerLiseHF);
            MethodInfo method = type.GetMethod("IsExpired", BindingFlags.NonPublic | BindingFlags.Instance);

            DateTime date = new DateTime(2025, 1, 1, 12, 0, 0);

            var calibParam = new CalibrationLiseHFResult() { Timestamp= date };
            object[] parameters = { calibParam };

            //Act
            bool res = (bool)method.Invoke(calibrationManagerLiseHF, parameters);

            //Assert
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void CalibrationManager_IsExpired_should_return_false_when_period_less_zero()
        {
            //Arrange
            var mockConf=Mock.Of<ProbeLiseHFConfig>(c=>c.CalibrationValidityPeriodMinutes==-1);
            var calibrationManagerLiseHF = new ProbeCalibrationManagerLiseHF(mockConf, _cancellationToken);
            Type type = typeof(ProbeCalibrationManagerLiseHF);
            MethodInfo method = type.GetMethod("IsExpired", BindingFlags.NonPublic | BindingFlags.Instance);

            DateTime date = new DateTime(2025, 1, 1, 12, 0, 0);
            var calibParam = new CalibrationLiseHFResult() { Timestamp = date };

            object[] parameters = { calibParam };

            //Act
            bool res = (bool)method.Invoke(calibrationManagerLiseHF, parameters);

            //Assert
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void CalibrationManager_IsExpired_should_return_false_when_calibration_date_is_less_than_time_param()
        {
            //Arrange
            var mockConf=Mock.Of<ProbeLiseHFConfig>(c=>c.CalibrationValidityPeriodMinutes==10);
            var calibrationManagerLiseHF = new ProbeCalibrationManagerLiseHF(mockConf, _cancellationToken);
            Type type = typeof(ProbeCalibrationManagerLiseHF);
            MethodInfo method = type.GetMethod("IsExpired", BindingFlags.NonPublic | BindingFlags.Instance);

            DateTime date = DateTime.UtcNow.AddMinutes(-5);
            var calibParam = new CalibrationLiseHFResult() { Timestamp= date };


            object[] parameters = { calibParam };

            //Act
            bool res = (bool)method.Invoke(calibrationManagerLiseHF, parameters);

            //Assert
            Assert.IsFalse(res);

        }

        [TestMethod]
        public void CalibrationManager_GetCalibration_should_return_null_when_not_found_and_createIfNeeded_False()
        {
            //Arrange
            var probeId = "ProbeLiseHF";
            var mockConf = Mock.Of<ProbeLiseHFConfig>(c => c.CalibrationValidityPeriodMinutes == 10);

            var calibrationManagerLiseHF = new ProbeCalibrationManagerLiseHF(mockConf, _cancellationToken);
            var paramInput = Mock.Of<IProbeInputParams>();
            var pointPos = Mock.Of<PointPosition>();
            
            //Act
            var res = calibrationManagerLiseHF.GetCalibration(false,probeId,paramInput,pointPos);

            //Assert
            Assert.IsNull(res);
        }

    }
}

