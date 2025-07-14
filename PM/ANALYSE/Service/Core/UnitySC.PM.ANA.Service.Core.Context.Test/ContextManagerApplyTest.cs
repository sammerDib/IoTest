using System;
using System.Collections.Generic;
using System.Threading;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using SimpleInjector;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.ObjectiveSelector;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.TestUtils.ObjectiveSelector.Configuration;
using UnitySC.PM.ANA.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Hardware.Light;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.FDC;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Context.Test
{
    [TestClass]
    public class ContextManagerApplyTest
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

        [TestMethod]
        public void ApplyLightContext()
        {
            // Given
            var lightMock = new Mock<LightBase>();
            _anaHardwareManager.Lights = new Dictionary<string, LightBase> { { "light#1", lightMock.Object } };
            var lightContext = new LightContext("light#1", 22);

            // When
            _contextManager.Apply(lightContext);

            // Then
            lightMock.Verify(light => light.SetIntensity(22));
        }

        [TestMethod]
        public void ApplyLightContext_WhenLightNotFound()
        {
            // Given
            _anaHardwareManager.Lights = new Dictionary<string, LightBase>();
            var lightContext = new LightContext("not_existing_light_id", 22);

            // When & Then
            var ex = Assert.ThrowsException<AggregateException>(() => _contextManager.Apply(lightContext));
            Assert.AreEqual(1, ex.InnerExceptions.Count);
            Assert.IsInstanceOfType<Exception>(ex.InnerException);
            StringAssert.Contains(ex.InnerException.Message, "not_existing_light_id");
        }

        [TestMethod]
        public void ApplyLightsContext()
        {
            // Given
            var light1Mock = new Mock<LightBase>();
            var light2Mock = new Mock<LightBase>();
            _anaHardwareManager.Lights = new Dictionary<string, LightBase> { { "light#1", light1Mock.Object }, { "light#2", light2Mock.Object } };
            var lightsContext = new LightsContext
            {
                Lights = new List<LightContext> {
                    new LightContext ("light#1", 22),
                    new LightContext ("light#2", 5),
                }
            };

            // When
            _contextManager.Apply(lightsContext);

            // Then
            light1Mock.Verify(light => light.SetIntensity(22));
            light2Mock.Verify(light => light.SetIntensity(5));
        }

        [TestMethod]
        public void ApplyLightsContext_WhenLightNotFound()
        {
            // Given
            var light1Mock = new Mock<LightBase>();
            _anaHardwareManager.Lights = new Dictionary<string, LightBase> { { "light#1", light1Mock.Object } };
            var lightsContext = new LightsContext
            {
                Lights = new List<LightContext> {
                    new LightContext ("light#1", 22),
                    new LightContext ("not_existing_light_id", 5),
                }
            };

            // When & Then
            var ex = Assert.ThrowsException<AggregateException>(() => _contextManager.Apply(lightsContext));
            // Then light one set properly
            light1Mock.Verify(light => light.SetIntensity(22));
            // Then unknown light not found
            Assert.AreEqual(1, ex.InnerExceptions.Count);
            Assert.IsInstanceOfType<Exception>(ex.InnerException);
            StringAssert.Contains(ex.InnerException.Message, "not_existing_light_id");
        }

        [TestMethod]
        public void ApplyXYPositionContext()
        {
            // Given
            var axiesMock = new Mock<IAxes>();
            _anaHardwareManager.Axes = axiesMock.Object;
            var position = new XYPosition(new StageReferential(), 3, 10);
            var context = new XYPositionContext(position);

            // When
            _contextManager.Apply(context);

            // Then
            axiesMock.Verify(axis => axis.GotoPosition(position, AxisSpeed.Normal)
            );
        }

        [TestMethod]
        public void ApplyAnaPositionContext()
        {
            // Given
            var axesMock = new Mock<IAxes>();
            _anaHardwareManager.Axes = axesMock.Object;
            var position = new AnaPosition(new StageReferential(),
                3,
                10,
                double.NaN,
                1,
                new List<ZPiezoPosition>()
            );
            var context = new AnaPositionContext(position);

            // When
            _contextManager.Apply(context);

            // Then
            axesMock.Verify(axis => axis.GotoPosition(position, AxisSpeed.Normal)
            );
        }

        [TestMethod]
        public void ApplyObjectiveContext()
        {
            // Given
            var objectiveSelectorMock = new Mock<IObjectiveSelector>();
            var objectiveConfig = ObjectiveConfigFactory.Build();
            objectiveConfig.DeviceID = "objective#1";
            objectiveSelectorMock.Setup(selector => selector.DeviceID).Returns("selector#1");
            objectiveSelectorMock.Setup(selector => selector.GetObjectiveInUse()).Returns(objectiveConfig);
            objectiveSelectorMock.Setup(selector => selector.Config)
                .Returns(new SingleObjectiveSelectorConfig
                {
                    Objectives = new List<ObjectiveConfig> { objectiveConfig }
                }
                );
            _anaHardwareManager.ObjectivesSelectors =
                new Dictionary<string, IObjectiveSelector> { { "selector#1", objectiveSelectorMock.Object } };
            var context = new ObjectiveContext("objective#1");

            // When
            _contextManager.Apply(context);

            // Then
            objectiveSelectorMock.Verify(selector => selector.SetObjective(objectiveConfig));
            objectiveSelectorMock.Verify(selector => selector.WaitMotionEnd());
        }

        [TestMethod]
        public void ApplyObjectiveContext_WaitMotionEnd_after_SetObjective()
        {
            // Given
            var objectiveSelectorMock = new Mock<IObjectiveSelector>();
            var objectiveConfig = ObjectiveConfigFactory.Build();
            objectiveConfig.DeviceID = "objective#1";
            objectiveSelectorMock.Setup(selector => selector.DeviceID).Returns("selector#1");
            objectiveSelectorMock.Setup(selector => selector.GetObjectiveInUse()).Returns(objectiveConfig);
            objectiveSelectorMock.Setup(selector => selector.Config)
                .Returns(new SingleObjectiveSelectorConfig
                {
                    Objectives = new List<ObjectiveConfig> { objectiveConfig }
                }
                );
            DateTime objectiveSelectorSetObjectiveTime = DateTime.Now;
            objectiveSelectorMock.Setup(selector => selector.SetObjective(objectiveConfig))
                .Callback(() =>
                {
                    objectiveSelectorSetObjectiveTime = DateTime.Now;
                    Thread.Sleep(500);
                });
            DateTime objectiveSelectorWaitMotionTime = DateTime.Now;
            objectiveSelectorMock.Setup(selector => selector.WaitMotionEnd())
                .Callback(() =>
                {
                    objectiveSelectorWaitMotionTime = DateTime.Now;
                });
            _anaHardwareManager.ObjectivesSelectors =
                new Dictionary<string, IObjectiveSelector> { { "selector#1", objectiveSelectorMock.Object } };
            var context = new ObjectiveContext("objective#1");

            // When
            _contextManager.Apply(context);

            // Then
            objectiveSelectorMock.Verify(selector => selector.SetObjective(objectiveConfig));
            objectiveSelectorMock.Verify(selector => selector.WaitMotionEnd());
            // Wait for objective motion end called after setting the objective, with at least the waiting time in between
            Assert.IsTrue((objectiveSelectorWaitMotionTime - objectiveSelectorSetObjectiveTime).TotalMilliseconds > 500);
        }

        [TestMethod]
        public void ApplyObjectiveContext_when_objective_selector_not_found()
        {
            // Given
            var objectiveSelectorMock = new Mock<IObjectiveSelector>();
            objectiveSelectorMock.Setup(selector => selector.Config)
                .Returns(new SingleObjectiveSelectorConfig { Objectives = new List<ObjectiveConfig>() }
                );
            _anaHardwareManager.ObjectivesSelectors =
                new Dictionary<string, IObjectiveSelector> { { "selector#1", objectiveSelectorMock.Object } };
            var context = new ObjectiveContext("objective#1");

            // When & then
            var ex = Assert.ThrowsException<AggregateException>(() => _contextManager.Apply(context));
            Assert.AreEqual(2, ex.InnerExceptions.Count); // 2 exceptions because application task AND wait task
            Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException);
            StringAssert.Contains("Objective selector of objective \"objective#1\" not found.", ex.InnerException.Message);
        }

        [TestMethod]
        public void ApplyObjectiveContext_when_no_objective_selector_throws()
        {
            // Given
            _anaHardwareManager.ObjectivesSelectors = new Dictionary<string, IObjectiveSelector>();
            var context = new ObjectiveContext("objective#1");

            // When & then
            var ex = Assert.ThrowsException<AggregateException>(() => _contextManager.Apply(context));
            Assert.AreEqual(2, ex.InnerExceptions.Count); // 2 exceptions because application task AND wait task
            Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException);
            StringAssert.Contains("Objective selector of objective \"objective#1\" not found.", ex.InnerException.Message);
        }

        [TestMethod]
        public void ApplyObjectivesContext()
        {
            // Given
            var objective1SelectorMock = new Mock<IObjectiveSelector>();
            var objective1Config = ObjectiveConfigFactory.Build();
            objective1Config.DeviceID = "objective#1";
            objective1SelectorMock.Setup(selector => selector.DeviceID).Returns("selector#1");
            objective1SelectorMock.Setup(selector => selector.GetObjectiveInUse()).Returns(objective1Config);
            objective1SelectorMock.Setup(selector => selector.Config)
                .Returns(new SingleObjectiveSelectorConfig
                {
                    Objectives = new List<ObjectiveConfig> { objective1Config }
                }
                );

            var objective2SelectorMock = new Mock<IObjectiveSelector>();
            var objective2Config = ObjectiveConfigFactory.Build();
            objective2Config.DeviceID = "objective#2";
            objective2SelectorMock.Setup(selector => selector.DeviceID).Returns("selector#2");
            objective2SelectorMock.Setup(selector => selector.GetObjectiveInUse()).Returns(objective2Config);
            objective2SelectorMock.Setup(selector => selector.Config)
                .Returns(new SingleObjectiveSelectorConfig
                {
                    Objectives = new List<ObjectiveConfig> { objective2Config }
                }
                );
            _anaHardwareManager.ObjectivesSelectors =
                new Dictionary<string, IObjectiveSelector> { { "selector#1", objective1SelectorMock.Object }, { "selector#2", objective2SelectorMock.Object } };
            var context = new ObjectivesContext
            {
                Objectives = new List<ObjectiveContext> {
                    new ObjectiveContext("objective#1" ),
                    new ObjectiveContext("objective#2" )
                }
            };

            // When
            _contextManager.Apply(context);

            // Then
            objective1SelectorMock.Verify(selector => selector.SetObjective(objective1Config));
            objective1SelectorMock.Verify(selector => selector.WaitMotionEnd());
            objective2SelectorMock.Verify(selector => selector.SetObjective(objective2Config));
            objective2SelectorMock.Verify(selector => selector.WaitMotionEnd());
        }

        [TestMethod]
        public void ApplyObjectivesContext_when_objective_selector_not_found()
        {
            // Given
            var objective1SelectorMock = new Mock<IObjectiveSelector>();
            var objective1Config = ObjectiveConfigFactory.Build();
            objective1Config.DeviceID = "objective#1";
            objective1SelectorMock.Setup(selector => selector.DeviceID).Returns("selector#1");
            objective1SelectorMock.Setup(selector => selector.GetObjectiveInUse()).Returns(objective1Config);
            objective1SelectorMock.Setup(selector => selector.Config)
                .Returns(new SingleObjectiveSelectorConfig
                {
                    Objectives = new List<ObjectiveConfig> { objective1Config }
                }
                );

            _anaHardwareManager.ObjectivesSelectors =
                new Dictionary<string, IObjectiveSelector> { { "selector#1", objective1SelectorMock.Object } };
            var context = new ObjectivesContext
            {
                Objectives = new List<ObjectiveContext> {
                    new ObjectiveContext("objective#1" ),
                    new ObjectiveContext("objective_with_no_selector" )
                }
            };

            // When/Then
            var ex = Assert.ThrowsException<AggregateException>(() => _contextManager.Apply(context));
            // Then objective 1 set
            objective1SelectorMock.Verify(selector => selector.SetObjective(objective1Config));
            objective1SelectorMock.Verify(selector => selector.WaitMotionEnd());
            // Then other objective selector not found
            Assert.AreEqual(2, ex.InnerExceptions.Count); // 2 exceptions because application task AND wait task
            Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException);
            StringAssert.Contains("Objective selector of objective \"objective_with_no_selector\" not found.", ex.InnerException.Message);
        }

        [TestMethod]
        public void ApplyTopImageAcquisitionContext()
        {
            // Given
            var objectiveSelectorMock = new Mock<IObjectiveSelector>();
            var objectiveConfig = ObjectiveConfigFactory.Build();
            objectiveConfig.DeviceID = "objective#1";
            objectiveSelectorMock.Setup(selector => selector.DeviceID).Returns("selector#1");
            objectiveSelectorMock.Setup(selector => selector.GetObjectiveInUse()).Returns(objectiveConfig);
            objectiveSelectorMock.Setup(selector => selector.Config)
                .Returns(new SingleObjectiveSelectorConfig
                {
                    Objectives = new List<ObjectiveConfig> { objectiveConfig }
                }
                );
            _anaHardwareManager.ObjectivesSelectors =
                new Dictionary<string, IObjectiveSelector> { { "selector#1", objectiveSelectorMock.Object } };

            var light1Mock = new Mock<LightBase>();
            var light2Mock = new Mock<LightBase>();
            _anaHardwareManager.Lights = new Dictionary<string, LightBase> { { "light#1", light1Mock.Object }, { "light#2", light2Mock.Object } };

            var context = new TopImageAcquisitionContext
            {
                TopObjectiveContext = new TopObjectiveContext("objective#1"),
                Lights = new LightsContext
                {
                    Lights = new List<LightContext>
                    {
                    new LightContext ("light#1", 22 ),
                    new LightContext ("light#2", 5 ),
                    }
                }
            };

            // When
            _contextManager.Apply(context);

            // Then
            objectiveSelectorMock.Verify(selector => selector.SetObjective(objectiveConfig));
            objectiveSelectorMock.Verify(selector => selector.WaitMotionEnd());
            light1Mock.Verify(light => light.SetIntensity(22));
            light2Mock.Verify(light => light.SetIntensity(5));
        }

        [TestMethod]
        public void ApplyBottomImageAcquisitionContext()
        {
            // Given
            var objectiveSelectorMock = new Mock<IObjectiveSelector>();
            var objectiveConfig = ObjectiveConfigFactory.Build();
            objectiveConfig.DeviceID = "objective#1";
            objectiveSelectorMock.Setup(selector => selector.DeviceID).Returns("selector#1");
            objectiveSelectorMock.Setup(selector => selector.GetObjectiveInUse()).Returns(objectiveConfig);
            objectiveSelectorMock.Setup(selector => selector.Config)
                .Returns(new SingleObjectiveSelectorConfig
                {
                    Objectives = new List<ObjectiveConfig> { objectiveConfig }
                }
                );
            _anaHardwareManager.ObjectivesSelectors =
                new Dictionary<string, IObjectiveSelector> { { "selector#1", objectiveSelectorMock.Object } };

            var light1Mock = new Mock<LightBase>();
            var light2Mock = new Mock<LightBase>();
            _anaHardwareManager.Lights = new Dictionary<string, LightBase> { { "light#1", light1Mock.Object }, { "light#2", light2Mock.Object } };

            var context = new BottomImageAcquisitionContext
            {
                BottomObjectiveContext = new BottomObjectiveContext("objective#1"),
                Lights = new LightsContext
                {
                    Lights = new List<LightContext>
                    {
                    new LightContext("light#1", 22 ),
                    new LightContext("light#2", 5 ),
                    }
                }
            };

            // When
            _contextManager.Apply(context);

            // Then
            objectiveSelectorMock.Verify(selector => selector.SetObjective(objectiveConfig));
            objectiveSelectorMock.Verify(selector => selector.WaitMotionEnd());
            light1Mock.Verify(light => light.SetIntensity(22));
            light2Mock.Verify(light => light.SetIntensity(5));
        }

        [TestMethod]
        public void ApplyDualImageAcquisitionContext()
        {
            // Given
            var objective1SelectorMock = new Mock<IObjectiveSelector>();
            var objective1Config = ObjectiveConfigFactory.Build();
            objective1Config.DeviceID = "objective#1";
            objective1SelectorMock.Setup(selector => selector.DeviceID).Returns("selector#1");
            objective1SelectorMock.Setup(selector => selector.GetObjectiveInUse()).Returns(objective1Config);
            objective1SelectorMock.Setup(selector => selector.Config)
                .Returns(new SingleObjectiveSelectorConfig
                {
                    Objectives = new List<ObjectiveConfig> { objective1Config }
                }
                );

            var objective2SelectorMock = new Mock<IObjectiveSelector>();
            var objective2Config = ObjectiveConfigFactory.Build();
            objective2Config.DeviceID = "objective#2";
            objective2SelectorMock.Setup(selector => selector.DeviceID).Returns("selector#2");
            objective2SelectorMock.Setup(selector => selector.GetObjectiveInUse()).Returns(objective2Config);
            objective2SelectorMock.Setup(selector => selector.Config)
                .Returns(new SingleObjectiveSelectorConfig
                {
                    Objectives = new List<ObjectiveConfig> { objective2Config }
                }
                );
            _anaHardwareManager.ObjectivesSelectors =
                new Dictionary<string, IObjectiveSelector> { { "selector#1", objective1SelectorMock.Object }, { "selector#2", objective2SelectorMock.Object } };

            var light1Mock = new Mock<LightBase>();
            var light2Mock = new Mock<LightBase>();
            _anaHardwareManager.Lights = new Dictionary<string, LightBase> { { "light#1", light1Mock.Object }, { "light#2", light2Mock.Object } };

            var context = new DualImageAcquisitionContext
            {
                TopObjectiveContext = new TopObjectiveContext("objective#1"),
                BottomObjectiveContext = new BottomObjectiveContext("objective#2"),
                Lights = new LightsContext
                {
                    Lights = new List<LightContext>
                    {
                    new LightContext("light#1", 22 ),
                    new LightContext("light#2", 5 ),
                    }
                }
            };

            // When
            _contextManager.Apply(context);

            // Then
            objective1SelectorMock.Verify(selector => selector.SetObjective(objective1Config));
            objective1SelectorMock.Verify(selector => selector.WaitMotionEnd());
            objective2SelectorMock.Verify(selector => selector.SetObjective(objective2Config));
            objective2SelectorMock.Verify(selector => selector.WaitMotionEnd());
            light1Mock.Verify(light => light.SetIntensity(22));
            light2Mock.Verify(light => light.SetIntensity(5));
        }

        [TestMethod]
        public void ApplyPMContext()
        {
            // Given
            var objectiveSelectorMock = new Mock<IObjectiveSelector>();
            var objectiveConfig = ObjectiveConfigFactory.Build();
            objectiveConfig.DeviceID = "objective#1";
            objectiveSelectorMock.Setup(selector => selector.DeviceID).Returns("selector#1");
            objectiveSelectorMock.Setup(selector => selector.GetObjectiveInUse()).Returns(objectiveConfig);
            objectiveSelectorMock.Setup(selector => selector.Config)
                .Returns(new SingleObjectiveSelectorConfig
                {
                    Objectives = new List<ObjectiveConfig> { objectiveConfig }
                }
                );

            _anaHardwareManager.ObjectivesSelectors =
                new Dictionary<string, IObjectiveSelector> { { "selector#1", objectiveSelectorMock.Object } };

            var lightMock = new Mock<LightBase>();
            _anaHardwareManager.Lights = new Dictionary<string, LightBase> { { "light#1", lightMock.Object } };

            var axiesMock = new Mock<IAxes>();
            _anaHardwareManager.Axes = axiesMock.Object;
            var position = new XYPosition(new StageReferential(), 3, 10);

            var context = new PMContext
            {
                Objectives = new ObjectivesContext()
                {
                    Objectives = new List<ObjectiveContext>()
                    {
                        new ObjectiveContext("objective#1" )
                    }
                },
                Lights = new LightsContext
                {
                    Lights = new List<LightContext>
                    {
                        new LightContext("light#1", 22 ),
                    }
                },
                XyPosition = new XYPositionContext(position),
                Context = new ChamberContext()
            };

            // When
            _contextManager.Apply(context);

            // Then
            objectiveSelectorMock.Verify(selector => selector.SetObjective(objectiveConfig));
            objectiveSelectorMock.Verify(selector => selector.WaitMotionEnd());
            lightMock.Verify(light => light.SetIntensity(22));
            axiesMock.Verify(axis => axis.GotoPosition(position, AxisSpeed.Normal));
        }

        [TestMethod]
        public void ApplyUnknownContextThrows()
        {
            // Given unknown context
            var unknownContext = new UnknownContext();

            // When applying Then it throws an exception
            var exception = Assert.ThrowsException<ArgumentException>(() => _contextManager.Apply(unknownContext));
            StringAssert.Contains($"Unknow context type to apply: {typeof(UnknownContext)}", exception.Message);
        }

        [TestMethod]
        public void ApplyNullContext()
        {
            // Given null context
            ANAContextBase nullContext = null;

            // When applying it
            _contextManager.Apply(nullContext);

            // Then no throw
        }

        [TestMethod]
        public void ApplyListOfNullContext()
        {
            // Given list of null context
            ContextsList listOfNullContexts = new ContextsList(null, null);

            // When applying it
            _contextManager.Apply(listOfNullContexts);

            // Then no throw
        }

        [TestMethod]
        public void ApplyListOfContexts()
        {
            // Given list of contexts
            ContextsList list = new ContextsList();

            // Given light context in list
            var lightMock = new Mock<LightBase>();
            _anaHardwareManager.Lights = new Dictionary<string, LightBase> { { "light#1", lightMock.Object } };
            list.Contexts.Add(new LightsContext
            {
                Lights = new List<LightContext> { new LightContext("light#1", 22) }
            });

            // Given position context in list
            var axiesMock = new Mock<IAxes>();
            _anaHardwareManager.Axes = axiesMock.Object;
            var position = new XYPosition(new StageReferential(), 3, 10);
            list.Contexts.Add(new XYPositionContext(position));

            // When applying contexts
            _contextManager.Apply(list);

            // Then
            lightMock.Verify(light => light.SetIntensity(22));
            axiesMock.Verify(axis => axis.GotoPosition(position, AxisSpeed.Normal));
        }

        [TestMethod]
        public void ApplyPositionAfterObjectivesContext()
        {
            // Given position context
            var axesMock = new Mock<IAxes>();
            _anaHardwareManager.Axes = axesMock.Object;
            var position = new XYPosition(new StageReferential(), 3, 10);
            var positionContext = new XYPositionContext(position);

            // Given axes set to save time when called
            DateTime axesGotoTime = DateTime.Now;
            axesMock.Setup(axis => axis.GotoPosition(position, AxisSpeed.Normal)).Callback(() => axesGotoTime = DateTime.Now);

            // Given objective context
            var objectiveSelectorMock = new Mock<IObjectiveSelector>();
            var objectiveConfig = ObjectiveConfigFactory.Build();
            objectiveConfig.DeviceID = "objective#1";
            objectiveSelectorMock.Setup(selector => selector.DeviceID).Returns("selector#1");
            objectiveSelectorMock.Setup(selector => selector.GetObjectiveInUse()).Returns(objectiveConfig);
            objectiveSelectorMock.Setup(selector => selector.Config)
                .Returns(new SingleObjectiveSelectorConfig
                {
                    Objectives = new List<ObjectiveConfig> { objectiveConfig }
                }
                );
            _anaHardwareManager.ObjectivesSelectors =
                new Dictionary<string, IObjectiveSelector> { { "selector#1", objectiveSelectorMock.Object } };
            var objectiveContext = new ObjectivesContext
            {
                Objectives = new List<ObjectiveContext> { new ObjectiveContext("objective#1") }
            };

            // Given objective selector set to save time when called, and to wait 500ms
            DateTime objectiveSelectorSetObjectiveTime = DateTime.Now;
            objectiveSelectorMock.Setup(selector => selector.SetObjective(objectiveConfig))
                .Callback(() =>
                {
                    objectiveSelectorSetObjectiveTime = DateTime.Now;
                    Thread.Sleep(500);
                });

            // Given combined context of objective and position
            var contexts = new ContextsList(positionContext, objectiveContext);

            // When
            _contextManager.Apply(contexts);

            // Then contexts properly applied
            axesMock.Verify(axis => axis.GotoPosition(position, AxisSpeed.Normal));
            objectiveSelectorMock.Verify(selector => selector.SetObjective(objectiveConfig));
            // Then objective context applied before position with at least the waiting time in between
            Assert.IsTrue((axesGotoTime - objectiveSelectorSetObjectiveTime).TotalMilliseconds > 500);
        }
    }
}
