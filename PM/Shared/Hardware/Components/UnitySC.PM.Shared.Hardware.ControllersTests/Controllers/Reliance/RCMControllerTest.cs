using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Serilog.Events;

using SimpleInjector;

using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Reliance;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Reliance.Simulation;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Implementation;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.ControllersTests.Controllers.Reliance
{
    [TestClass]
    public class RCMControllerTest
    {
        private RCMControllerConfig _config;
        private RCMSimulator _simulator;
        private RelianceAxis _xAxis;
        private RelianceAxis _yAxis;

        private CancellationTokenSource _cancellationTokenSource;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            ClassLocator.ExternalInit(new Container(), true);
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));
            ClassLocator.Default.Register<IGlobalStatusServer>(() =>
                ClassLocator.Default.GetInstance<GlobalStatusService>()
            );

            var mockLogger = Mock.Of<IHardwareLogger>();
            var mockLoggerFactory = Mock.Of<IHardwareLoggerFactory>(x => x.CreateHardwareLogger(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()) == mockLogger);
            ClassLocator.Default.Register<IHardwareLoggerFactory>(() => mockLoggerFactory);
        }

        [TestInitialize]
        public void Initialize()
        {
            _xAxis = BuildRelianceAxis(MovingDirection.X);
            _yAxis = BuildRelianceAxis(MovingDirection.Y);
            _config = BuildRCMControllerConfig();
            _simulator = BuildRCMSimulator(new List<RelianceAxis> { _xAxis, _yAxis });
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private RCMController BuildController()
        {
            var logger = ClassLocator.Default.GetInstance<IHardwareLoggerFactory>()
                .CreateHardwareLogger(It.IsAny<LogEventLevel>().ToString(),It.IsAny<string>(), It.IsAny<string>());
            var controller = new RCMController(_config,null,logger, _simulator);
            controller.AxesList = new List<IAxis> { _xAxis, _yAxis };
            return controller;
        }

        [TestCleanup]
        public void Cleanup()
        {
            _cancellationTokenSource.Cancel();
        }

        [TestMethod]
        public void InitializationAllAxes_starts_state_notification_polling()
        {
            using (var controller = BuildController())
            {
                // Given
                _config.PollingDelay = TimeSpan.FromMilliseconds(10);
                controller.Init();

                // When
                var stateTask = GetNextAxesStateEvent(controller, TimeSpan.FromMilliseconds(100));
                controller.InitializationAllAxes();

                // Then
                Assert.IsNotNull(stateTask.Result);
            }
        }

        [TestMethod]
        public async Task Dispose_stops_state_notification_polling()
        {
            // Given
            _config.PollingDelay = TimeSpan.FromMilliseconds(10);
            var controller = BuildController();
            controller.Init();
            controller.InitializationAllAxes();

            // When
            controller.Dispose();

            // Then
            await AssertNoAxesStateEventSent(controller, TimeSpan.FromMilliseconds(200));
        }

        [TestMethod]
        public async Task AxesStateChangedEvent_is_sent_when_one_axis_is_moving()
        {
            using (var controller = BuildController())
            {
                // Given
                _config.PollingDelay = TimeSpan.FromMilliseconds(10);
                controller.Init();
                controller.InitializationAllAxes();

                // Wait until StateEvent is notified by InitializationAllAxes()
                await Task.Delay(_config.PollingDelay, _cancellationTokenSource.Token);

                _simulator.MotionDelay = TimeSpan.FromMilliseconds(100);

                // When
                var nextStateTask = GetNextAxesStateEvent(controller, TimeSpan.FromMilliseconds(200));
                _ = controller.MoveToAsync(new RCMAxisMove(_xAxis, 200.Millimeters()));

                // Then
                var nextState = nextStateTask.Result;
                Assert.IsNotNull(nextState);
                Assert.AreEqual(true, nextState.OneAxisIsMoving);
            }
        }

        // TODO: tadd test "AxesStateChangedEvent_is_sent_when_one_axis_is_disabled"

        [TestMethod]
        public void SetPosAxis()
        {
            using (var controller = BuildController())
            {
                // Given
                _simulator.MotionDelay = TimeSpan.Zero;
                controller.Init();
                controller.InitializationAllAxes();

                // When
                controller.SetPosAxis(
                    new List<double> { -1, 2 },
                    new List<IAxis> { _xAxis, _yAxis },
                    new List<AxisSpeed> { AxisSpeed.Fast, AxisSpeed.Slow }
                );
                controller.WaitMotionEnd(500);

                // Then
                var currentX = controller.GetCurrentPosition(_xAxis);
                Assert.AreEqual(-1, currentX.Millimeters);
                var currentY = controller.GetCurrentPosition(_yAxis);
                Assert.AreEqual(2, currentY.Millimeters);
            }
        }

        // TODO: add test SetPosAxis) with single axis

        [TestMethod]
        public async Task SetPosAxis_sends_events_when_motion_ends()
        {
            using (var controller = BuildController())
            {
                // Given
                _simulator.MotionDelay = TimeSpan.Zero;
                _simulator.MoveTo(0, _xAxis.Config.RelianceAxisID);
                _simulator.MoveTo(0, _yAxis.Config.RelianceAxisID);

                controller.Init();
                controller.InitializationAllAxes();

                var getMotionEndEventTask = GetNextMotionEndEvent(controller, TimeSpan.FromMilliseconds(500));

                var mock = new Mock<AxesPositionChangedDelegate>();
                controller.AxesPositionChangedEvent += mock.Object;

                // When
                controller.SetPosAxis(
                    new List<double> { -1, 0 },
                    new List<IAxis> { _xAxis, _yAxis },
                    new List<AxisSpeed> { AxisSpeed.Fast, AxisSpeed.Slow }
                );
                controller.WaitMotionEnd(500);
                await getMotionEndEventTask;

                // Then
                Assert.IsTrue(getMotionEndEventTask.Result);
                mock.AssertInvocationUntil(
                    m => m.Invoke(new XYPosition(new MotorReferential(), -1, 0)),
                    TimeSpan.FromMilliseconds(500),
                    _cancellationTokenSource.Token
                );
            }
        }

        [TestMethod]
        public void SetPosAxisWithSpeedAndAccel()
        {
            using (var controller = BuildController())
            {
                // Given
                _simulator.MotionDelay = TimeSpan.Zero;
                controller.Init();
                controller.InitializationAllAxes();

                // When
                controller.SetPosAxisWithSpeedAndAccel(
                    new List<double> { -1, 2 },
                    new List<IAxis> { _xAxis, _yAxis },
                    new List<double> { 300, 150 },
                    new List<double> { 40, 10 }
                );
                controller.WaitMotionEnd(500);

                // Then
                var currentX = controller.GetCurrentPosition(_xAxis);
                Assert.AreEqual(-1, currentX.Millimeters);
                var currentY = controller.GetCurrentPosition(_yAxis);
                Assert.AreEqual(2, currentY.Millimeters);
            }
        }

        [TestMethod]
        public async Task SetPosAxisWithSpeedAndAccel_sends_event_when_motion_ends()
        {
            using (var controller = BuildController())
            {
                // Given
                _simulator.MotionDelay = TimeSpan.Zero;
                _simulator.MoveTo(0, _xAxis.Config.RelianceAxisID);
                _simulator.MoveTo(0, _yAxis.Config.RelianceAxisID);

                controller.Init();
                controller.InitializationAllAxes();

                var getMotionEndEventTask = GetNextMotionEndEvent(controller, TimeSpan.FromMilliseconds(100));
                var positionChangedEvents = CollectPositionsChangedEvent(controller, TimeSpan.FromMilliseconds(500));

                // When
                controller.SetPosAxisWithSpeedAndAccel(
                    new List<double> { 0, 2 },
                    new List<IAxis> { _xAxis, _yAxis },
                    new List<double> { 300, 150 },
                    new List<double> { 40, 10 }
                );
                controller.WaitMotionEnd(500);
                await getMotionEndEventTask;

                // Then
                Assert.IsTrue(getMotionEndEventTask.Result);

                positionChangedEvents.TryPeek(out var lastPositionChangedEvent).Should()
                    .BeTrue("No position changed event sent");
                lastPositionChangedEvent.Should()
                    .BeAssignableTo<XYPosition>()
                    .And
                    .BeEquivalentTo(new XYPosition(new MotorReferential(), 0, 2));
            }
        }

        [TestMethod]
        public void StopAxesMotion()
        {
            using (var controller = BuildController())
            {
                // Given
                controller.Init();
                controller.InitializationAllAxes();

                _ = controller.MoveToAsync(new RCMAxisMove(_xAxis, 110.Millimeters()));

                // When
                var stopWatch = Stopwatch.StartNew();
                controller.StopAxesMotion();
                stopWatch.Stop();

                // Then
                Assert.IsTrue(stopWatch.Elapsed < TimeSpan.FromSeconds(5),
                    "StopAxesMotion() do not wait for requested motion ending (10s), and return earlier"
                );
                var motorStatus = controller.GetMotorStatus(_xAxis);
                Assert.AreEqual(MotorStatus.InPosition, motorStatus);
            }
        }

        [TestMethod]
        public void RefreshCurrentPos()
        {
            using (var controller = BuildController())
            {
                // Given
                _xAxis.Config.PositionZero = 130.Millimeters();
                controller.Init();
                controller.InitializationAllAxes();

                const int hardwarePosition_pulses = 3_000;
                _simulator.MoveTo(hardwarePosition_pulses, _xAxis.Config.RelianceAxisID);

                // When
                controller.RefreshCurrentPos(new List<IAxis> { _xAxis });

                // Then
                double hardwarePosition_mm = hardwarePosition_pulses /
                                             (double)_config.MotorResolution *
                                             RCMController.DistancePerMotorRotation.Millimeters;
                double motorPosition_mm =
                    (hardwarePosition_mm - _xAxis.Config.PositionZero.Millimeters) * _xAxis.Config.MotorDirection;
                Assert.AreEqual(motorPosition_mm, _xAxis.CurrentPos.Millimeters);
            }
        }

        [TestMethod]
        public async Task MoveIncremental()
        {
            using (var controller = BuildController())
            {
                // Given
                controller.Init();
                controller.InitializationAllAxes();

                await controller.MoveToAsync(new RCMAxisMove(_xAxis, 150.Millimeters(), AxisSpeed.Fast));

                // When
                controller.MoveIncremental(_xAxis, AxisSpeed.Measure, -1.2);
                controller.WaitMotionEnd(1_000);

                // Then
                var currentPosition = controller.GetCurrentPosition(_xAxis);
                Assert.AreEqual(150 - 1.2, currentPosition.Millimeters, 0.1);
            }
        }

        [TestMethod]
        public void RefreshAxisState_when_not_moving()
        {
            using (var controller = BuildController())
            {
                // Given
                _simulator.MotionDelay = TimeSpan.FromMilliseconds(50);
                controller.Init();
                controller.InitializationAllAxes();
                controller.WaitMotionEnd(1_000);

                // When
                controller.RefreshAxisState(_xAxis);

                // Then
                Assert.IsFalse(_xAxis.Moving);
            }
        }

        [TestMethod]
        public void RefreshAxisState_when_moving()
        {
            using (var controller = BuildController())
            {
                // Given
                _simulator.MotionDelay = TimeSpan.FromMilliseconds(1_000);
                controller.Init();
                controller.InitializationAllAxes();
                _ = controller.MoveToAsync(new RCMAxisMove(_xAxis, 2.Millimeters()));

                // When
                controller.RefreshAxisState(_xAxis);

                // Then
                Assert.IsTrue(_xAxis.Moving);
            }
        }

        private static async Task<AxesState> GetNextAxesStateEvent(RCMController controller, TimeSpan timeout)
        {
            var cancellationTokenSource = new CancellationTokenSource(timeout);
            var taskCompletionSource = new TaskCompletionSource<AxesState>();
            cancellationTokenSource.Token.Register(() => taskCompletionSource.TrySetCanceled());

            void Handler(AxesState newState)
            {
                taskCompletionSource.TrySetResult(newState);
                controller.AxesStateChangedEvent -= Handler;
            }

            controller.AxesStateChangedEvent += Handler;
            return await taskCompletionSource.Task;
        }

        private static async Task<bool> GetNextMotionEndEvent(RCMController controller, TimeSpan timeout)
        {
            var cancellationTokenSource = new CancellationTokenSource(timeout);
            var taskCompletionSource = new TaskCompletionSource<bool>();
            cancellationTokenSource.Token.Register(() => taskCompletionSource.TrySetCanceled());

            void Handler(bool targetReached)
            {
                taskCompletionSource.TrySetResult(targetReached);
                controller.AxesEndMoveEvent -= Handler;
            }

            controller.AxesEndMoveEvent += Handler;
            return await taskCompletionSource.Task;
        }

        private ConcurrentStack<PositionBase> CollectPositionsChangedEvent(RCMController controller, TimeSpan timeout)
        {
            var events = new ConcurrentStack<PositionBase>();
            void Handler(PositionBase position)
            {
                events.Push(position);
            }
            controller.AxesPositionChangedEvent += Handler;
            _cancellationTokenSource.Token.Register(() => controller.AxesPositionChangedEvent -= Handler);
            return events;
        }

        private async Task AssertNoAxesStateEventSent(RCMController controller, TimeSpan timeout)
        {
            try
            {
                await GetNextAxesStateEvent(controller, timeout);
            }
            catch (TaskCanceledException)
            {
                // No event received until timeout
                return;
            }

            Assert.Fail("At least one event received before timeout, which is not expected");
        }

        private RCMSimulator BuildRCMSimulator(IEnumerable<RelianceAxis> axes, TimeSpan? motionDelay = null)
        {
            return new RCMSimulator("COM3", axes, motionDelay ?? TimeSpan.FromMilliseconds(50));
        }

        // TODO Move these methods in dedicated factories (to make them reusable)
        private int _axisId = 1;

        private RelianceAxis BuildRelianceAxis(MovingDirection movingDirection)
        {
            return new RelianceAxis(new RelianceAxisConfig()
            {
                AxisID = movingDirection.ToString("g"),
                Name = $"{movingDirection}-Axis",
                MovingDirection = movingDirection,
                RelianceAxisID = _axisId++.ToString(),
                SpeedSlow = 100,
                SpeedNormal = 2000,
                SpeedFast = 4000,
                PositionZero = 146.Millimeters(),
                PositionMin = -150.Millimeters(),
                PositionMax = 150.Millimeters(),
                MotorDirection = -1,
            }
            );
        }

        private RCMControllerConfig BuildRCMControllerConfig()
        {
            var config = new RCMControllerConfig()
            {
                Name = "RCM Controller",
                DeviceID = "RCM-1",
                PortName = "COM3",
                BaudRate = 38400,
                MotorResolution = 50000,
                PollingDelay = TimeSpan.FromMilliseconds(10)
            };
            return config;
        }
    }
}
