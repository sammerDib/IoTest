using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Proxy.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Proxy.Test
{
    [TestClass]
    public class UnitTestAxes
    {
        #region Fields

        private readonly double _positionningAccuracy = 0.001; // mm
        private static int _waitingEndMove = 3000; // ms
        private static AxesConfig _axesConfig;
        private static ChuckBaseConfig _chuckConfig;

        private static string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).AbsolutePath).Replace("%20", " ");
        }

        protected static Mock<IDialogOwnerService> dialogServiceMock = new Mock<IDialogOwnerService>();

        #endregion Fields

        [ClassInitialize]
        public static void InitTestSuite(TestContext testContext)
        {
            var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
            var chuckSupervisor = ClassLocator.Default.GetInstance<ChuckSupervisor>();
            _axesConfig = axesSupervisor.GetAxesConfiguration()?.Result;
            _chuckConfig = chuckSupervisor.GetChuckConfiguration()?.Result;
            if (_axesConfig.IsSimulated)
                _waitingEndMove = 4000;
            else
                _waitingEndMove = 4000;

            Thread.Sleep(4000); // Wait for ACS controller to park LOH and UOH at start

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
            var axesState = axesSupervisor.GetCurrentState()?.Result;

            if (axesState.Landed == true)
                axesSupervisor.StopLanding();

            axesState = axesSupervisor.GetCurrentState()?.Result;

            axesSupervisor.GoToHome(AxisSpeed.Fast);
            axesSupervisor.WaitMotionEnd(30000);
        }

        [TestMethod]
        public void TestGotoPosition()
        {
            var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();

            var destination = new XYZTopZBottomPosition(new StageReferential(), 10, 15, 3, 2.01);
            axesSupervisor.GotoPosition(destination, AxisSpeed.Fast);

            Thread.Sleep(_waitingEndMove);

            var pos = (XYZTopZBottomPosition)axesSupervisor.GetCurrentPosition()?.Result;

            // Attention here we do not take into account the objectives calibration and the XY calibration
            // which leads to a shift in the results
            // if the current calibrations are simulated, for example, it is possible that this will lead to
            // failures in positioning accuracy of this UNIT tests
            Assert.IsTrue(pos.X.Near(10, _positionningAccuracy));
            Assert.IsTrue(pos.Y.Near(15, _positionningAccuracy));
            Assert.IsTrue(pos.ZTop.Near(3, _positionningAccuracy));
            Assert.IsTrue(pos.ZBottom.Near(2.01, _positionningAccuracy));
        }

        [TestMethod]
        public void TestGotoPositionCustomSpeedAccel()
        {
            var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();

            var moveX = new AxisMove(10, 200, 10d);
            var moveY = new AxisMove(15, 200, 10d);
            var moveZTop = new AxisMove(3, 200, 10d);
            var moveZBottom = new AxisMove(2.01, 200, 10d);
            axesSupervisor.GotoPointCustomSpeedAccel(moveX, moveY, moveZTop, moveZBottom);

            Thread.Sleep(_waitingEndMove);

            var pos = (XYZTopZBottomPosition)axesSupervisor.GetCurrentPosition()?.Result;
            Assert.IsTrue(pos.X.Near(10, _positionningAccuracy));
            Assert.IsTrue(pos.Y.Near(15, _positionningAccuracy));
            Assert.IsTrue(pos.ZTop.Near(3, _positionningAccuracy));
            Assert.IsTrue(pos.ZBottom.Near(2.01, _positionningAccuracy));
        }

        [TestMethod]
        public void TestGotoPositionCustomSpeedAccelWithoutCoord()
        {
            var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
            try
            {
                var moveX = new AxisMove(double.NaN, 200, 10d);
                var moveY = new AxisMove(double.NaN, 200, 10d);
                var moveZTop = new AxisMove(double.NaN, 200, 10d);
                var moveZBottom = new AxisMove(double.NaN, 200, 10d);
                axesSupervisor.GotoPointCustomSpeedAccel(moveX, moveY, moveZTop, moveZBottom);
                var ret = axesSupervisor.GotoPointCustomSpeedAccel(moveX, moveY, moveZTop, moveZBottom);
            }
            catch (Exception Ex)
            {
                Assert.IsTrue(Ex.Message.Equals("[HardwareException] Service : Function GotoPosition called without coordinates"));
            }
        }

        [TestMethod]
        public void TestGotoPositionWithoutCoord()
        {
            var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
            try
            {
                var destination = new XYZTopZBottomPosition(new StageReferential(), double.NaN, double.NaN, double.NaN, double.NaN);
                var ret = axesSupervisor.GotoPosition(destination, AxisSpeed.Fast);
            }
            catch (Exception Ex)
            {
                Assert.IsTrue(Ex.Message.Equals("[HardwareException] Service : Function GotoPosition called without coordinates"));
            }
        }

        [TestMethod]
        public void TestGotoPark()
        {
            var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
            var waferDiameter = 300.Millimeters();
            axesSupervisor.GoToPark(waferDiameter, AxisSpeed.Measure);

            Thread.Sleep(_waitingEndMove);

            var pos = (XYZTopZBottomPosition)axesSupervisor.GetCurrentPosition()?.Result;
            Assert.IsTrue(pos.X.Near(-215, _positionningAccuracy));
            Assert.IsTrue(pos.Y.Near(-175, _positionningAccuracy));
            Assert.IsTrue(pos.ZTop.Near(15, _positionningAccuracy));
            Assert.IsTrue(pos.ZBottom.Near(0, _positionningAccuracy));
        }

        [TestMethod]
        public void TestGotoHome()
        {
            var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();

            axesSupervisor.GoToHome(AxisSpeed.Measure);

            Thread.Sleep(_waitingEndMove);

            var pos = (XYZTopZBottomPosition)axesSupervisor.GetCurrentPosition()?.Result;
            Assert.IsTrue(pos.X.Near(0, _positionningAccuracy));
            Assert.IsTrue(pos.Y.Near(0, _positionningAccuracy));
            Assert.IsTrue(pos.ZTop.Near(0, _positionningAccuracy));
            Assert.IsTrue(pos.ZBottom.Near(0, _positionningAccuracy));
        }

        [TestMethod]
        public void TestGotoManualLoad()
        {
            var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
            var waferDiameter = 300.Millimeters();
            axesSupervisor.GoToManualLoad(waferDiameter, AxisSpeed.Measure);

            Thread.Sleep(_waitingEndMove);

            var pos = (XYZTopZBottomPosition)axesSupervisor.GetCurrentPosition()?.Result;
            Assert.IsTrue(pos.X.Near(0, _positionningAccuracy));
            Assert.IsTrue(pos.Y.Near(155, _positionningAccuracy));
            Assert.IsTrue(pos.ZTop.Near(19, _positionningAccuracy));
            Assert.IsTrue(pos.ZBottom.Near(0, _positionningAccuracy));
        }

        [TestMethod]
        public void TestWaitMotionEnd()
        {
            var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
            var waferDiameter = 300.Millimeters();
            axesSupervisor.GoToPark(waferDiameter, AxisSpeed.Measure);
            var axesState = axesSupervisor.GetCurrentState()?.Result;
            Assert.IsTrue(axesState.OneAxisIsMoving);

            axesSupervisor.WaitMotionEnd(60000);
            Thread.Sleep(500);
            axesState = axesSupervisor.GetCurrentState()?.Result;
            Assert.IsFalse(axesState.OneAxisIsMoving);
        }

        [TestMethod]
        public void TestWaitMotionEndXWithCustomSpeedAccel()
        {
            // Arrange
            var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
            double xCoord = 150;

            // Act
            var moveX = new AxisMove(xCoord, 200, 10d);
            var moveY = new AxisMove(double.NaN, 200, 10d);
            var moveZTop = new AxisMove(double.NaN, 200, 10d);
            var moveZBottom = new AxisMove(double.NaN, 200, 10d);
            var ret = axesSupervisor.GotoPointCustomSpeedAccel(moveX, moveY, moveZTop, moveZBottom);
            axesSupervisor.WaitMotionEnd(30000);
            Thread.Sleep(500);
            var axesState = axesSupervisor.GetCurrentState()?.Result;

            // Assert
            Assert.IsFalse(axesState.OneAxisIsMoving);
        }

        [TestMethod]
        public void TestWaitMotionEndX()
        {
            // Arrange
            var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
            double xCoord = 150;

            // Act
            var destination = new XYZTopZBottomPosition(new StageReferential(), xCoord, double.NaN, double.NaN, double.NaN);
            var ret = axesSupervisor.GotoPosition(destination, AxisSpeed.Fast);
            axesSupervisor.WaitMotionEnd(30000);
            Thread.Sleep(500);
            var axesState = axesSupervisor.GetCurrentState()?.Result;

            // Assert
            Assert.IsFalse(axesState.OneAxisIsMoving);
        }

        [TestMethod]
        public void TestWaitMotionEndCallback()
        {
            var axesSupervisorMock = new Mock<AxesSupervisor>(ClassLocator.Default.GetInstance<ILogger<AxesSupervisor>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClassLocator.Default.GetInstance<IDialogOwnerService>());
            var axesSupervisor = axesSupervisorMock.Object;
            axesSupervisor.GoToHome(AxisSpeed.Fast);
            Thread.Sleep(4000);
            var waferDiameter = 300.Millimeters();
            axesSupervisor.GoToManualLoad(waferDiameter, AxisSpeed.Fast);
            Thread.Sleep(4000);
            axesSupervisor.GoToPark(waferDiameter, AxisSpeed.Fast);
            Thread.Sleep(4000);
            axesSupervisorMock.Verify(x => x.EndMoveCallback(It.Is<bool>(i => i == true)), Times.AtLeast(3));
        }

        [TestMethod]
        public void TestStopMove()
        {
            // Arrange
            var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
            double[] posCommand = new double[4] { 100.0, -65.0, 5.0, -0.7 };
            var moveX = new AxisMove(posCommand[0], 20, 10d);
            var moveY = new AxisMove(posCommand[1], 20, 10d);
            var moveZTop = new AxisMove(posCommand[2], 20, 10d);
            var moveZBottom = new AxisMove(posCommand[3], 20, 10d);
            var pos = (XYZTopZBottomPosition)axesSupervisor.GetCurrentPosition()?.Result;
            var ret = axesSupervisor.GotoPointCustomSpeedAccel(moveX, moveY, moveZTop, moveZBottom);
            bool Success = SpinWait.SpinUntil(() =>
            {
                var axesState = axesSupervisor.GetCurrentState()?.Result;
                return axesState.OneAxisIsMoving == true;
            }, 2000);

            axesSupervisor.StopAllMoves();

            Success = SpinWait.SpinUntil(() =>
            {
                var axesState = axesSupervisor.GetCurrentState()?.Result;
                return axesState.OneAxisIsMoving == false;
            }, 2000);
            pos = (XYZTopZBottomPosition)axesSupervisor.GetCurrentPosition()?.Result;

            Assert.IsFalse(pos.X.Near(posCommand[0], _positionningAccuracy));
            Assert.IsFalse(pos.Y.Near(posCommand[1], _positionningAccuracy));
            Assert.IsFalse(pos.ZTop.Near(posCommand[2], _positionningAccuracy));
            Assert.IsFalse(pos.ZBottom.Near(posCommand[3], _positionningAccuracy));
        }

        [TestMethod]
        public void TestLandStopLanding()
        {
            // Arrange
            var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
            var axesState = axesSupervisor.GetCurrentState()?.Result;

            // Act
            if (axesState.Landed == false)
                axesSupervisor.Land();
            Thread.Sleep(500);

            axesState = axesSupervisor.GetCurrentState()?.Result;

            // Assert
            Assert.IsTrue(axesState.Landed);

            // Act
            axesSupervisor.StopLanding();
            axesState = axesSupervisor.GetCurrentState()?.Result;

            // Assert
            Assert.IsFalse(axesState.Landed);
        }

        [TestMethod]
        public void TestHALExceptionsCatching()
        {
            // Arrange
            var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
            // Act
            var destination = new XYZTopZBottomPosition(new StageReferential(), 0, 0, 0, 0);
            var response = axesSupervisor.GotoPosition(destination, AxisSpeed.Normal);
            // Assert
            Assert.AreEqual(0, response.Messages.Count);

            // Arrange
            double outOfRangeX = _axesConfig.AxisConfigs.Find(a => a.MovingDirection == MovingDirection.X).PositionMax.Millimeters + 10;
            var expectedError = "CheckAxisLimits " + outOfRangeX.ToString("0.000") + "mm out of axis maximum limit " + _axesConfig.AxisConfigs.Find(a => a.MovingDirection == MovingDirection.X).PositionMax.ToString("0.000") + "mm";
            // Act
            destination = new XYZTopZBottomPosition(new StageReferential(), outOfRangeX, 0, 0, 0);
            response = axesSupervisor.GotoPosition(destination, AxisSpeed.Normal);
            string errorMessage = "";
            foreach (var message in response.Messages)
                if (message.AdvancedContent.Contains(expectedError))
                    errorMessage = message.AdvancedContent;
            // Assert
            Assert.IsTrue(response.Messages.Count >= 0);
            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));
        }

        [TestMethod]
        public void TestWaferClamp()
        {
            // Arrange
            var chuckSupervisor = ClassLocator.Default.GetInstance<ChuckSupervisor>();
            var chuckState = chuckSupervisor.GetCurrentState()?.Result;

            WaferDimensionalCharacteristic wafer = new WaferDimensionalCharacteristic();
            wafer.Diameter = 0.Millimeters();

            // Act
            chuckSupervisor.ReleaseWafer(wafer);

            Thread.Sleep(1000);
            chuckState = chuckSupervisor.GetCurrentState()?.Result;
            // Assert
            Assert.IsFalse(chuckState.WaferClampStates[chuckSupervisor.ChuckVM.SelectedWaferCategory.DimentionalCharacteristic.Diameter]);
        }
    }
}
