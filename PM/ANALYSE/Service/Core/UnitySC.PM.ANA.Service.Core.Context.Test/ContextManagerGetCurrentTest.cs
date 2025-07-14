using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleInjector;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.ObjectiveSelector;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.TestUtils.ObjectiveSelector.Configuration;
using UnitySC.PM.ANA.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Hardware.Light;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Referentials.TestUtils.Positions;
using UnitySC.Shared.FDC;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Context.Test
{
    [TestClass]
    public class ContextManagerGetCurrentTest
    {
        private ContextManager _contextManager;
        private AnaHardwareManager _anaHardwareManager;

        [TestInitialize]
        public void Init()
        {
            var container = new Container();
            ClassLocator.ExternalInit(container, true);
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));
            ClassLocator.Default.Register<IMessenger>(() => WeakReferenceMessenger.Default, true);
            ClassLocator.Default.Register(() => new FDCManager("test", "test"), true);
            ClassLocator.Default.Register<IPMServiceConfigurationManager>(() => new FakeConfigurationManager(), true);
            ClassLocator.Default.Register<CalibrationManager>(() => new CalibrationManager(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().CalibrationFolderPath));

            _anaHardwareManager = new AnaHardwareManager(Mock.Of<ILogger>(), Mock.Of<IHardwareLoggerFactory>(), new FakeConfigurationManager());
            _contextManager = new ContextManager(_anaHardwareManager);
        }

        private class FixedIntensityLight : LightBase
        {
            public override DeviceFamily Family { get; }

            private readonly int _intensity;

            public FixedIntensityLight(int intensity)
            {
                _intensity = intensity;
            }

            public override double GetIntensity()
            {
                return _intensity;
            }
        }

        [TestMethod]
        public void GetCurrentLightsContext()
        {
            // Given
            _anaHardwareManager.Lights = new Dictionary<string, LightBase> { { "light#1", new FixedIntensityLight(5) } };

            // When
            var lightsContext = _contextManager.GetCurrent<LightsContext>();

            // Then
            Assert.AreEqual(1, lightsContext.Lights.Count);
            var lightContext = lightsContext.Lights[0];
            Assert.AreEqual("light#1", lightContext.DeviceID);
            Assert.AreEqual(5, lightContext.Intensity);
        }

        [TestMethod]
        public void GetCurrentXYPositionContext()
        {
            // Given
            var axesMock = new Mock<IAxes>();
            _anaHardwareManager.Axes = axesMock.Object;
            var position = new AnaPosition(new MotorReferential(),
                3,
                4,
                1,
                1,
                new List<ZPiezoPosition>()
            );
            ;
            axesMock.Setup(axes => axes.GetPos()).Returns(position);

            // When
            var context = _contextManager.GetCurrent<XYPositionContext>();

            // Then
            Assert.AreEqual(new XYPosition(new MotorReferential(), 3, 4), context.Position);
        }

        [TestMethod]
        public void GetCurrentXYPositionContext_when_not_aAnaPosition()
        {
            // Given
            var axesMock = new Mock<IAxes>();
            _anaHardwareManager.Axes = axesMock.Object;
            var position = new XYPosition(new MotorReferential(), 3, 4);
            axesMock.Setup(axes => axes.GetPos()).Returns(position);

            // When & then
            var exception = Assert.ThrowsException<Exception>(() => _contextManager.GetCurrent<XYPositionContext>());

            // Then
            Assert.IsNotNull(exception);
            StringAssert.Contains(exception.Message, $"Position type {typeof(XYPosition)} not supported");
        }

        [TestMethod]
        public void GetCurrentAnaPositionContextContext()
        {
            // Given
            var axesMock = new Mock<IAxes>();
            _anaHardwareManager.Axes = axesMock.Object;

            var position = AnaPositionFactory.Build();
            axesMock.Setup(axes => axes.GetPos()).Returns(position);

            // When
            var context = _contextManager.GetCurrent<AnaPositionContext>();

            // Then
            Assert.AreEqual(position, context.Position);
        }

        [TestMethod]
        public void GetCurrentAnaPositionContextContext_when_not_aAnaPosition_returned()
        {
            // Given
            var axesMock = new Mock<IAxes>();
            _anaHardwareManager.Axes = axesMock.Object;
            var position = new XYPosition(new MotorReferential(), 3, 4);
            axesMock.Setup(axes => axes.GetPos()).Returns(position);

            // When
            var exception = Assert.ThrowsException<Exception>(() => _contextManager.GetCurrent<AnaPositionContext>());

            // Then
            Assert.IsNotNull(exception);
            StringAssert.Contains(exception.Message, $"Position type {typeof(XYPosition)} not supported");
        }

        [TestMethod]
        public void GetCurrentObjectivesContext()
        {
            // Given
            var objectiveSelectorMock = new Mock<IObjectiveSelector>();
            objectiveSelectorMock.SetupGet(_ => _.Position).Returns(PM.Shared.Hardware.Service.Interface.ModulePositions.Up);
            var objectiveConfig = ObjectiveConfigFactory.Build();
            objectiveConfig.DeviceID = "objective#1";
            objectiveSelectorMock.Setup(selector => selector.GetObjectiveInUse())
                .Returns(objectiveConfig);
            _anaHardwareManager.ObjectivesSelectors =
                new Dictionary<string, IObjectiveSelector> { { "selector#1", objectiveSelectorMock.Object } };

            // When
            var context = _contextManager.GetCurrent<ObjectivesContext>();

            // Then
            context.Objectives.Should()
                .HaveCount(1)
                .And
                .Contain(objective => objective.ObjectiveId == "objective#1"
                && objective is TopObjectiveContext);
        }

        [TestMethod]
        public void GetCurrentTopObjectiveContext()
        {
            // Given
            var objectiveSelectorMock = new Mock<IObjectiveSelector>();
            var objectiveConfig = ObjectiveConfigFactory.Build();
            objectiveConfig.DeviceID = "objective#1";
            objectiveSelectorMock.Setup(selector => selector.GetObjectiveInUse())
                .Returns(objectiveConfig);
            objectiveSelectorMock.Setup(selector => selector.Position).Returns(PM.Shared.Hardware.Service.Interface.ModulePositions.Up);
            _anaHardwareManager.ObjectivesSelectors =
                new Dictionary<string, IObjectiveSelector> { { "selector#1", objectiveSelectorMock.Object } };

            // When
            var context = _contextManager.GetCurrent<TopObjectiveContext>();

            // Then
            Assert.AreEqual("objective#1", context.ObjectiveId);
        }

        [TestMethod]
        public void GetCurrentBottomObjectiveContext()
        {
            // Given
            var objectiveSelectorMock = new Mock<IObjectiveSelector>();
            var objectiveConfig = ObjectiveConfigFactory.Build();
            objectiveConfig.DeviceID = "objective#1";
            objectiveSelectorMock.Setup(selector => selector.GetObjectiveInUse())
                .Returns(objectiveConfig);
            objectiveSelectorMock.Setup(selector => selector.Position).Returns(PM.Shared.Hardware.Service.Interface.ModulePositions.Down);
            _anaHardwareManager.ObjectivesSelectors =
                new Dictionary<string, IObjectiveSelector> { { "selector#1", objectiveSelectorMock.Object } };

            // When
            var context = _contextManager.GetCurrent<BottomObjectiveContext>();

            // Then
            Assert.AreEqual("objective#1", context.ObjectiveId);
        }

        [TestMethod]
        public void GetCurrentTopObjectiveContextWithNoTopObjectiveThrows()
        {
            // Given
            var objectiveSelectorMock = new Mock<IObjectiveSelector>();
            var objectiveConfig = ObjectiveConfigFactory.Build();
            objectiveConfig.DeviceID = "objective#1";
            objectiveSelectorMock.Setup(selector => selector.GetObjectiveInUse())
                .Returns(objectiveConfig);
            objectiveSelectorMock.Setup(selector => selector.Position).Returns(PM.Shared.Hardware.Service.Interface.ModulePositions.Down);
            _anaHardwareManager.ObjectivesSelectors =
                new Dictionary<string, IObjectiveSelector> { { "selector#1", objectiveSelectorMock.Object } };

            // When
            var exception = Assert.ThrowsException<Exception>(() => _contextManager.GetCurrent<TopObjectiveContext>());

            // Then
            Assert.IsNotNull(exception);
            StringAssert.Contains("Current top objective not found", exception.Message);
        }

        [TestMethod]
        public void GetCurrentBottomObjectiveContextWithNoBottomObjectiveThrows()
        {
            // Given
            var objectiveSelectorMock = new Mock<IObjectiveSelector>();
            var objectiveConfig = ObjectiveConfigFactory.Build();
            objectiveConfig.DeviceID = "objective#1";
            objectiveSelectorMock.Setup(selector => selector.GetObjectiveInUse())
                .Returns(objectiveConfig);
            objectiveSelectorMock.Setup(selector => selector.Position).Returns(PM.Shared.Hardware.Service.Interface.ModulePositions.Up);
            _anaHardwareManager.ObjectivesSelectors =
                new Dictionary<string, IObjectiveSelector> { { "selector#1", objectiveSelectorMock.Object } };

            // When
            var exception = Assert.ThrowsException<Exception>(() => _contextManager.GetCurrent<BottomObjectiveContext>());

            // Then
            Assert.IsNotNull(exception);
            StringAssert.Contains("Current bottom objective not found", exception.Message);
        }

        [TestMethod]
        public void GetUnknownContextThrowsUnknown()
        {
            // When applying Then it throws an exception
            var exception = Assert.ThrowsException<ArgumentException>(() => _contextManager.GetCurrent<UnknownContext>());

            // Then
            Assert.IsNotNull(exception);
            StringAssert.Contains($"Unknow context type to get: {typeof(UnknownContext)}", exception.Message);
        }

        [TestMethod]
        public void GetContextWithSubcontexts()
        {
            // Given
            var axesMock = new Mock<IAxes>();
            _anaHardwareManager.Axes = axesMock.Object;
            var position = new AnaPosition(new MotorReferential(),
                3,
                4,
                1,
                1,
                new List<ZPiezoPosition>()
            );
            ;
            axesMock.Setup(axes => axes.GetPos()).Returns(position);

            // When
            var context = _contextManager.GetCurrent<ContextWithSubcontexts>();

            // Then
            Assert.IsNotNull(context.PositionContext);
            Assert.AreEqual(new XYPosition(new MotorReferential(), 3, 4), context.PositionContext.Position);
        }

        [TestMethod]
        public void GetContextWithNotOnlySubcontextsThrowsUnknown()
        {
            // Given
            var axesMock = new Mock<IAxes>();
            _anaHardwareManager.Axes = axesMock.Object;
            var position = new AnaPosition(new MotorReferential(),
                3,
                4,
                1,
                1,
                new List<ZPiezoPosition>()
            );
            ;
            axesMock.Setup(axes => axes.GetPos()).Returns(position);

            // When/Then
            var exception = Assert.ThrowsException<ArgumentException>(() => _contextManager.GetCurrent<ContextWithNotOnlySubcontexts>());

            // Then
            Assert.IsNotNull(exception);
            StringAssert.Contains($"Unknow context type to get: {typeof(ContextWithNotOnlySubcontexts)}", exception.Message);
        }
    }
}
