using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;

using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes.Enum;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.DMT.Service.Test
{
    internal class AxesTestCallbackProxy : IAxesServiceCallbackProxy
    {
        public List<PositionBase> PositionHistory = new List<PositionBase>();
        public bool AxesMovingDetected = false;
        public AxesState LastAxesState;

        public void EndMove(bool targetReached)
        {
            // nop
        }

        public void PositionChanged(PositionBase position)
        {
            PositionHistory.Add((PositionBase)position.Clone());
        }

        public void StateChanged(AxesState state)
        {
            LastAxesState = state;
            if (!AxesMovingDetected)
                AxesMovingDetected = state.OneAxisIsMoving;
        }
    }

    [Ignore("No more maintained")]
    [TestClass]
    public class DummyAxesTest
    {
        [TestMethod]
        public void Expected_position_polling_thread_to_stop()
        {
            var container = new Container();
            var dummyAxes = CreateAndInitDummyAxes(container);

            var movementTracker = (AxesTestCallbackProxy)ClassLocator.Default.GetInstance<IAxesServiceCallbackProxy>();

            // Axes movement
            dummyAxes.GotoPosition(
                new XYZTopZBottomPosition(new MotorReferential(), 50, 50, 9, 9),
                AxisSpeed.Fast);
            dummyAxes.StopAllMoves();
            dummyAxes.WaitMotionEnd(5000);
            Assert.IsTrue(movementTracker.PositionHistory.Count < 10); // Complete movement => PositionHistory = 100
        }

        [TestMethod]
        public void Expected_position_to_change_when_moving()
        {
            var container = new Container();
            var dummyAxes = CreateAndInitDummyAxes(container);

            var movementTracker = (AxesTestCallbackProxy)ClassLocator.Default.GetInstance<IAxesServiceCallbackProxy>();
            movementTracker.AxesMovingDetected = false;

            // Axes movement
            dummyAxes.GotoPosition(
                new XYZTopZBottomPosition(new MotorReferential(), 50, 50, 9, 9),
                AxisSpeed.Fast);
            dummyAxes.WaitMotionEnd(5000);

            Assert.IsTrue(AxesHasMovedDuringTest());
            Assert.IsTrue(movementTracker.AxesMovingDetected);
        }

        [TestMethod]
        public void Expected_positions_Detected()
        {
            var container = new Container();
            var dummyAxes = CreateAndInitDummyAxes(container);

            // Manual loding position
            var manualLogingPosition = new XYZTopZBottomPosition(new MotorReferential(), 100, -100, 50, -50);
            dummyAxes.GotoPosition(manualLogingPosition, AxisSpeed.Fast);
            dummyAxes.WaitMotionEnd(5000);
            Assert.IsTrue(dummyAxes.IsAtPosition(manualLogingPosition));

            //Automatic park position
            var parkPosition = new XYZTopZBottomPosition(new MotorReferential(), 100, 10, 20, 30);
            dummyAxes.GotoPosition(parkPosition, AxisSpeed.Fast);           
            dummyAxes.WaitMotionEnd(5000);
            Assert.IsTrue(dummyAxes.IsAtPosition(parkPosition));
        }

        private IAxes CreateAndInitDummyAxes(Container container)
        {
            ClassLocator.ExternalInit(container, true);

            ClassLocator.Default.Register(typeof(IAxesServiceCallbackProxy),
                                          typeof(AxesTestCallbackProxy), true);


            const string controllerName = "testController";
            IHardwareLogger logger = Mock.Of<IHardwareLogger>();

            // ACSAxisIDLink List
            //List<ACSAxisIDLink> acsAxisIDLinks = new List<ACSAxisIDLink>();
            //ACSAxisIDLink newACSID = new ACSAxisIDLink();
            //newACSID.ACSID = "ACSC_AXIS_2";
            //newACSID.AxisID = "X";
            //acsAxisIDLinks.Add(newACSID);
            //newACSID = new ACSAxisIDLink();
            //newACSID.ACSID = "ACSC_AXIS_0";
            //newACSID.AxisID = "Y";
            //acsAxisIDLinks.Add(newACSID);
            //newACSID = new ACSAxisIDLink();
            //newACSID.ACSID = "ACSC_AXIS_5";
            //newACSID.AxisID = "ZTop";
            //acsAxisIDLinks.Add(newACSID);
            //newACSID = new ACSAxisIDLink();
            //newACSID.ACSID = "ACSC_AXIS_4";
            //newACSID.AxisID = "ZBottom";
            //acsAxisIDLinks.Add(newACSID);

            // Connection parameters
            //ConnectionInfoParams connectionInfo = new ConnectionInfoParams();
            //connectionInfo.EthernetIP = "20.20.249.41";
            //connectionInfo.EthernetPort = "701";

            //var config = new ACSControllerConfig
            //{
            //    Side = controllerName,
            //    DeviceID = controllerName,
            //    ACSAxisIDLinks = acsAxisIDLinks,
            //    ConnectionInfo = connectionInfo
            //}

            // axesConfig : Axes configuration
            AxesConfig axesConfig = new AxesConfig();
            axesConfig.Name = "DummyAxes";
            axesConfig.DeviceID = "DummyAxes";
            axesConfig.IsEnabled = true;
            axesConfig.IsSimulated = true;
            axesConfig.LogLevel = DeviceLogLevel.Debug;
            axesConfig.AxisConfigs = new List<AxisConfig>();

            // Axis X
            AxisConfig newAxisConfig = new ACSAxisConfig();
            newAxisConfig.AxisID = "X";
            newAxisConfig.ControllerID = controllerName;
            newAxisConfig.Name = "Axis X";
            newAxisConfig.PositionMax = 200.Millimeters();
            newAxisConfig.PositionMin = -200.Millimeters();
            newAxisConfig.PositionManualLoad = 100.Millimeters();
            newAxisConfig.PositionPark = 100.Millimeters();
            newAxisConfig.PositionZero = 0.Millimeters();
            newAxisConfig.PositionHome = 10.Millimeters();
            newAxisConfig.MovingDirection = MovingDirection.X;
            axesConfig.AxisConfigs.Add(newAxisConfig);
            //Axis Y
            newAxisConfig = new ACSAxisConfig();
            newAxisConfig.AxisID = "Y";
            newAxisConfig.ControllerID = controllerName;
            newAxisConfig.Name = "Axis Y";
            newAxisConfig.PositionMax = 200.Millimeters();
            newAxisConfig.PositionMin = -200.Millimeters();
            newAxisConfig.PositionManualLoad = -100.Millimeters();
            newAxisConfig.PositionPark = 10.Millimeters();
            newAxisConfig.PositionZero = 0.Millimeters();
            newAxisConfig.PositionHome = 20.Millimeters();
            newAxisConfig.MovingDirection = MovingDirection.Y;
            axesConfig.AxisConfigs.Add(newAxisConfig);
            //Axis Z Top
            newAxisConfig = new ACSAxisConfig();
            newAxisConfig.AxisID = "ZTop";
            newAxisConfig.ControllerID = controllerName;
            newAxisConfig.Name = "Axis ZTop";
            newAxisConfig.PositionMax = 50.Millimeters();
            newAxisConfig.PositionMin = -50.Millimeters();
            newAxisConfig.PositionManualLoad = 50.Millimeters();
            newAxisConfig.PositionPark = 20.Millimeters();
            newAxisConfig.PositionZero = 0.Millimeters();
            newAxisConfig.PositionHome = 30.Millimeters();
            newAxisConfig.MovingDirection = MovingDirection.ZTop;
            axesConfig.AxisConfigs.Add(newAxisConfig);
            //Axis Z Bottom
            newAxisConfig = new ACSAxisConfig();
            newAxisConfig.AxisID = "ZBottom";
            newAxisConfig.ControllerID = controllerName;
            newAxisConfig.Name = "Axis ZBottom";
            newAxisConfig.PositionMax = 50.Millimeters();
            newAxisConfig.PositionMin = -50.Millimeters();
            newAxisConfig.PositionManualLoad = -50.Millimeters();
            newAxisConfig.PositionPark = 30.Millimeters();
            newAxisConfig.PositionZero = 0.Millimeters();
            newAxisConfig.PositionHome = 40.Millimeters();
            newAxisConfig.MovingDirection = MovingDirection.ZBottom;
            axesConfig.AxisConfigs.Add(newAxisConfig);

            // ctrlDico : Controller
            Dictionary<string, ControllerBase> ctrlDico = new Dictionary<string, ControllerBase>();
            ControllerConfig ctrlConfig = new ControllerConfig();
            ctrlConfig.DeviceID = "DummyCtrl";
            ctrlConfig.Name = ctrlConfig.DeviceID;
            var dummyController = new DummyController(ctrlConfig, axesConfig, logger);
            ctrlDico.Add(dummyController.DeviceID, dummyController);

            var dummyAxes = AxesFactory.CreateAxes(axesConfig, ctrlDico, null, logger, null);

            var initErrors = new List<Message>();
            dummyAxes.Init(initErrors);

            return (IAxes)dummyAxes;
        }

        private bool AxesHasMovedDuringTest()
        {
            var movementTracker = (AxesTestCallbackProxy)ClassLocator.Default.GetInstance<IAxesServiceCallbackProxy>();
            XYZTopZBottomPosition previousPosition = null;
            bool hasMoved = false;
            foreach (XYZTopZBottomPosition aPosition in movementTracker.PositionHistory)
            {
                if (previousPosition == null)
                {
                    previousPosition = aPosition;
                }
                else
                {
                    if (hasMoved == false)
                    {
                        if (!previousPosition.X.Equals(aPosition.X))
                            hasMoved = true;
                        if (!previousPosition.Y.Equals(aPosition.Y))
                            hasMoved = true;
                    }
                    previousPosition = aPosition;
                }
            }
            return hasMoved;
        }
    }
}
