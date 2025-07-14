using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Flows.AcquireOneImage;
using UnitySC.PM.DMT.Service.Flows.AutoExposure;
using UnitySC.PM.DMT.Service.Flows.Deflectometry;
using UnitySC.PM.DMT.Service.Flows.FlowTask;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.DMT.Service.Interface.Fringe;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Shared.TestUtils;
using UnitySC.PM.DMT.Shared;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Flows.Test
{
    [TestClass]
    public class
        DMTDeflectometryAcquisitionFlowTaskTest : TestWithMockedCameraAndScreen<DMTDeflectometryAcquisitionFlowTaskTest>
    {
        private Mock<IFringeManager> _fringeManagerMock = new Mock<IFringeManager>();
        
        protected override void PostGenericSetup()
        {
            var fringeManagerMock = new Mock<IFringeManager>();
            fringeManagerMock.Setup(fm => fm.GetFringeImageDict(Side.Front, It.Is<Fringe>(fringe => fringe.Period == 32
                                                                    && fringe.FringeType == FringeType.Standard
                                                                    && fringe.NbImagesPerDirection == 8)))
                             .Returns(new Dictionary<FringesDisplacement, Dictionary<int, List<USPImageMil>>>
                                      {
                                          {
                                              FringesDisplacement.X,
                                              new Dictionary<int, List<USPImageMil>>
                                              {
                                                  { 32, Enumerable.Repeat(new USPImageMil(), 8).ToList() }
                                              }
                                          },
                                          {
                                              FringesDisplacement.Y,
                                              new Dictionary<int, List<USPImageMil>>
                                              {
                                                  { 32, Enumerable.Repeat(new USPImageMil(), 8).ToList() }
                                              }
                                          }
                                      });
        }

        [TestMethod]
        public void GivenASinglePeriodMeasureStartShouldCreateAndExecuteTheRelevantFlows()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            var sequence = new MockSequence();
            var fringe = CreateSinglePeriodFringe();
            var mockedXPhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlow(sequence, fringe, Side.Front, FringesDisplacement.X);
            var mockedYPhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlow(sequence, fringe, Side.Front, FringesDisplacement.Y);

            var phaseAcquisitionFlows = new List<AcquirePhaseImagesForPeriodAndDirectionFlow>
                                        {
                                            mockedXPhaseAcquisitionFlow.Object,
                                            mockedYPhaseAcquisitionFlow.Object
                                        };

            var flowTask = new DMTDeflectometryAcquisitionFlowTask(phaseAcquisitionFlows);

            // When
            flowTask.Start(cancellationTokenSource, null, null);
            flowTask.LastAcquisitionTask.Wait();

            // Then
            mockedXPhaseAcquisitionFlow.Verify(paFlow => paFlow.Execute(), Times.Once());
            mockedYPhaseAcquisitionFlow.Verify(paFlow => paFlow.Execute(), Times.Once());
            flowTask.TemporaryResultsByPeriodAndDirection.Should()
                    .AllSatisfy(intDictKeyValuePair =>
                    {
                        intDictKeyValuePair.Key.Should().Be(32);
                        intDictKeyValuePair.Value.Should()
                                           .HaveCount(2)
                                           .And.AllSatisfy(directionListKeyValuePair =>
                                           {
                                               directionListKeyValuePair.Value.Should()
                                                                        .HaveCount(8)
                                                                        .And
                                                                        .BeSameAs(directionListKeyValuePair.Key ==
                                                                         FringesDisplacement.X
                                                                             ? mockedXPhaseAcquisitionFlow.Object
                                                                                 .Result
                                                                                 .TemporaryResults
                                                                             : mockedYPhaseAcquisitionFlow.Object
                                                                                 .Result
                                                                                 .TemporaryResults);
                                           });
                    });
        }

        [TestMethod]
        public void GivenASinglePeriodMeasureWithAutoExposureStartShouldCreateAndExecuteRelevantFlows()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            var sequence = new MockSequence();
            var fringe = CreateSinglePeriodFringe();
            var mockedAEFlow = CreateMockAutoExposureFlow(sequence, fringe, Side.Front);
            var mockedXPhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlowWithResultExposureTimeReferenceToInput(sequence, fringe, 32, Side.Front,
                 FringesDisplacement.X);
            var mockedYPhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlowWithResultExposureTimeReferenceToInput(sequence, fringe, 32, Side.Front,
                 FringesDisplacement.Y);

            var phaseAcquisitionFlows = new List<AcquirePhaseImagesForPeriodAndDirectionFlow>
                                        {
                                            mockedXPhaseAcquisitionFlow.Object,
                                            mockedYPhaseAcquisitionFlow.Object
                                        };
            var flowTask = new DMTDeflectometryAcquisitionFlowTask(mockedAEFlow.Object, phaseAcquisitionFlows);

            // When
            flowTask.Start(cancellationTokenSource, null, null);
            flowTask.LastAcquisitionTask.Wait();

            // Then
            mockedAEFlow.Verify(aeFlow => aeFlow.Execute(), Times.Once());
            mockedXPhaseAcquisitionFlow.Verify(paFlow => paFlow.Execute(), Times.Once());
            mockedYPhaseAcquisitionFlow.Verify(paFlow => paFlow.Execute(), Times.Once());
            mockedXPhaseAcquisitionFlow.Object.Result.ExposureTimeMs.Should().Be(145);
            mockedYPhaseAcquisitionFlow.Object.Result.ExposureTimeMs.Should().Be(145);
        }

        [TestMethod]
        public void GivenTwoSinglePeriodAcquisitionContinueWithShouldExecuteFlowsInCorrectOrder()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            var sequence = new MockSequence();
            var fringe = CreateSinglePeriodFringe();
            var mockedFrontXPhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlow(sequence, fringe, Side.Front, FringesDisplacement.X);
            var mockedFrontYPhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlow(sequence, fringe, Side.Front, FringesDisplacement.Y);
            var mockedBackXPhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlow(sequence, fringe, Side.Back, FringesDisplacement.X);
            var mockedBackYPhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlow(sequence, fringe, Side.Back, FringesDisplacement.Y);

            var firstTaskFlowList = new List<AcquirePhaseImagesForPeriodAndDirectionFlow>
                                    {
                                        mockedFrontXPhaseAcquisitionFlow.Object,
                                        mockedFrontYPhaseAcquisitionFlow.Object
                                    };
            var secondTaskFlowList = new List<AcquirePhaseImagesForPeriodAndDirectionFlow>
                                     {
                                         mockedBackXPhaseAcquisitionFlow.Object,
                                         mockedBackYPhaseAcquisitionFlow.Object
                                     };
            var firstFlowTask = new DMTDeflectometryAcquisitionFlowTask(firstTaskFlowList);
            var secondFlowTask = new DMTDeflectometryAcquisitionFlowTask(secondTaskFlowList);

            // Then
            firstFlowTask.Start(cancellationTokenSource, null, null);
            firstFlowTask.ContinueWith(secondFlowTask, null, null);
            Task.WaitAll((Task)firstFlowTask.LastAcquisitionTask, (Task)secondFlowTask.LastAcquisitionTask);

            // Then
            mockedFrontXPhaseAcquisitionFlow.Verify(paFlow => paFlow.Execute(), Times.Once());
            mockedFrontYPhaseAcquisitionFlow.Verify(paFlow => paFlow.Execute(), Times.Once());
            mockedBackXPhaseAcquisitionFlow.Verify(paFlow => paFlow.Execute(), Times.Once());
            mockedBackYPhaseAcquisitionFlow.Verify(paFlow => paFlow.Execute(), Times.Once());
            firstFlowTask.TemporaryResultsByPeriodAndDirection.Should()
                         .AllSatisfy(intDictKeyValuePair =>
                         {
                             intDictKeyValuePair.Key.Should().Be(32);
                             intDictKeyValuePair.Value.Should()
                                                .HaveCount(2)
                                                .And.AllSatisfy(directionListKeyValuePair =>
                                                {
                                                    directionListKeyValuePair.Value.Should()
                                                                             .HaveCount(8)
                                                                             .And
                                                                             .BeSameAs(directionListKeyValuePair.Key ==
                                                                              FringesDisplacement.X
                                                                                  ? mockedFrontXPhaseAcquisitionFlow
                                                                                      .Object.Result
                                                                                      .TemporaryResults
                                                                                  : mockedFrontYPhaseAcquisitionFlow
                                                                                      .Object.Result
                                                                                      .TemporaryResults);
                                                });
                         });
            secondFlowTask.TemporaryResultsByPeriodAndDirection.Should()
                          .AllSatisfy(intDictKeyValuePair =>
                          {
                              intDictKeyValuePair.Key.Should().Be(32);
                              intDictKeyValuePair.Value.Should()
                                                 .HaveCount(2)
                                                 .And.AllSatisfy(directionListKeyValuePair =>
                                                 {
                                                     directionListKeyValuePair.Value.Should()
                                                                              .HaveCount(8)
                                                                              .And
                                                                              .BeSameAs(directionListKeyValuePair.Key ==
                                                                               FringesDisplacement.X
                                                                                   ? mockedBackXPhaseAcquisitionFlow
                                                                                       .Object.Result
                                                                                       .TemporaryResults
                                                                                   : mockedBackYPhaseAcquisitionFlow
                                                                                       .Object.Result
                                                                                       .TemporaryResults);
                                                 });
                          });
        }

        [TestMethod]
        public void GivenTwoSinglePeriodAcquisitionsWithAutoExposureContinueWithShouldExecuteAllFlows()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            var sequence = new MockSequence();
            var fringe = CreateSinglePeriodFringe();
            var mockedFrontAutoExposureFlow =
                CreateMockAutoExposureFlow(sequence, fringe, Side.Front, 145);
            var mockedFrontXPhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlowWithResultExposureTimeReferenceToInput(sequence, fringe, 32, Side.Front,
                 FringesDisplacement.X);
            var mockedFrontYPhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlowWithResultExposureTimeReferenceToInput(sequence, fringe, 32, Side.Front,
                 FringesDisplacement.Y);

            var mockedBackAutoExposureFlow =
                CreateMockAutoExposureFlow(sequence, fringe, Side.Back, 165);
            var mockedBackXPhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlowWithResultExposureTimeReferenceToInput(sequence, fringe, 32, Side.Back,
                 FringesDisplacement.X);
            var mockedBackYPhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlowWithResultExposureTimeReferenceToInput(sequence, fringe, 32, Side.Back,
                 FringesDisplacement.Y);

            var firstTaskFlowList = new List<AcquirePhaseImagesForPeriodAndDirectionFlow>
                                    {
                                        mockedFrontXPhaseAcquisitionFlow.Object,
                                        mockedFrontYPhaseAcquisitionFlow.Object
                                    };
            var secondTaskFlowList = new List<AcquirePhaseImagesForPeriodAndDirectionFlow>
                                     {
                                         mockedBackXPhaseAcquisitionFlow.Object,
                                         mockedBackYPhaseAcquisitionFlow.Object
                                     };
            var firstFlowTask =
                new DMTDeflectometryAcquisitionFlowTask(mockedFrontAutoExposureFlow.Object, firstTaskFlowList);
            var secondFlowTask =
                new DMTDeflectometryAcquisitionFlowTask(mockedBackAutoExposureFlow.Object, secondTaskFlowList);

            // When
            firstFlowTask.Start(cancellationTokenSource, null, null);
            firstFlowTask.ContinueWith(secondFlowTask, null, null);
            Task.WaitAll((Task)firstFlowTask.LastAcquisitionTask, (Task)secondFlowTask.LastAcquisitionTask);

            // Then
            mockedFrontAutoExposureFlow.Verify(aeFlow => aeFlow.Execute(), Times.Once());
            mockedFrontXPhaseAcquisitionFlow.Verify(paFlow => paFlow.Execute(), Times.Once());
            mockedFrontYPhaseAcquisitionFlow.Verify(paFlow => paFlow.Execute(), Times.Once());
            mockedBackAutoExposureFlow.Verify(aeFlow => aeFlow.Execute(), Times.Once());
            mockedBackXPhaseAcquisitionFlow.Verify(paFlow => paFlow.Execute(), Times.Once());
            mockedBackYPhaseAcquisitionFlow.Verify(paFlow => paFlow.Execute(), Times.Once());
            mockedFrontXPhaseAcquisitionFlow.Object.Result.ExposureTimeMs.Should().Be(145);
            mockedFrontYPhaseAcquisitionFlow.Object.Result.ExposureTimeMs.Should().Be(145);
            mockedBackXPhaseAcquisitionFlow.Object.Result.ExposureTimeMs.Should().Be(165);
            mockedBackYPhaseAcquisitionFlow.Object.Result.ExposureTimeMs.Should().Be(165);
        }

        [TestMethod]
        public void GivenAPreviousSingleAcquisitionFlow()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            var sequence = new MockSequence();
            var acquireOneImageInput = new AcquireOneImageInput(Side.Front, Side.Front, 100,
                                                                MeasureType.BrightFieldMeasure,
                                                                AcquisitionScreenDisplayImage.Color, Colors.White);
            var mockedAcquireOneImageFlow = new Mock<AcquireOneImageFlow>(acquireOneImageInput,
                                                                          ClassLocator.Default
                                                                              .GetInstance<IDMTInternalCameraMethods>(),
                                                                          HardwareManager);
            mockedAcquireOneImageFlow.InSequence(sequence)
                                     .Setup(aiFlow => aiFlow.Execute())
                                     .Callback(() =>
                                     {
                                         mockedAcquireOneImageFlow.Object.Result = new AcquireOneImageResult
                                         {
                                             Status = new FlowStatus { State = FlowState.Success }
                                         };
                                     });
            var previousAcquisitionFlowTask = new DMTSingleAcquisitionFlowTask(mockedAcquireOneImageFlow.Object);

            var fringe = CreateSinglePeriodFringe();
            var mockedAEFlow = CreateMockAutoExposureFlow(sequence, fringe, Side.Front);
            var mockedXPhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlowWithResultExposureTimeReferenceToInput(sequence, fringe, 32, Side.Front,
                 FringesDisplacement.X);
            var mockedYPhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlowWithResultExposureTimeReferenceToInput(sequence, fringe, 32, Side.Front,
                 FringesDisplacement.Y);

            var phaseAcquisitionFlows = new List<AcquirePhaseImagesForPeriodAndDirectionFlow>
                                        {
                                            mockedXPhaseAcquisitionFlow.Object,
                                            mockedYPhaseAcquisitionFlow.Object
                                        };
            var flowTask = new DMTDeflectometryAcquisitionFlowTask(mockedAEFlow.Object, phaseAcquisitionFlows);

            // When
            previousAcquisitionFlowTask.Start(cancellationTokenSource, null, null, null, null);
            previousAcquisitionFlowTask.ContinueWith(flowTask, null, null);
            Task.WaitAll((Task)previousAcquisitionFlowTask.LastAcquisitionTask, (Task)flowTask.LastAcquisitionTask);

            // Then
            mockedAcquireOneImageFlow.Verify(aiFlow => aiFlow.Execute(), Times.Once());
            mockedXPhaseAcquisitionFlow.Verify(apiFlow => apiFlow.Execute(), Times.Once());
            mockedYPhaseAcquisitionFlow.Verify(apiFlow => apiFlow.Execute(), Times.Once());
        }

        [TestMethod]
        public void GivenMultiPeriodAcquisitionAllFlowsShouldExecute()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            var sequence = new MockSequence();
            var fringe = CreateMultiPeriodFringe(new List<int>
                                                 {
                                                     32,
                                                     320,
                                                     3200
                                                 });

            var mockedAEFlow = CreateMockAutoExposureFlow(sequence, fringe, Side.Front);
            var mockedX32PhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlowWithResultExposureTimeReferenceToInput(sequence, fringe, 32, Side.Front,
                 FringesDisplacement.X);
            var mockedX320PhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlowWithResultExposureTimeReferenceToInput(sequence, fringe, 320, Side.Front,
                 FringesDisplacement.X);
            var mockedX3200PhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlowWithResultExposureTimeReferenceToInput(sequence, fringe, 3200, Side.Front,
                 FringesDisplacement.X);

            var mockedY32PhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlowWithResultExposureTimeReferenceToInput(sequence, fringe, 32, Side.Front,
                 FringesDisplacement.Y);
            var mockedY320PhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlowWithResultExposureTimeReferenceToInput(sequence, fringe, 320, Side.Front,
                 FringesDisplacement.Y);
            var mockedY3200PhaseAcquisitionFlow =
                CreateMockPhaseAcquisitionFlowWithResultExposureTimeReferenceToInput(sequence, fringe, 3200, Side.Front,
                 FringesDisplacement.Y);

            var acquirePhaseImagesForPeriodAndDirectionFlows = new List<AcquirePhaseImagesForPeriodAndDirectionFlow>
                                                               {
                                                                   mockedX32PhaseAcquisitionFlow.Object,
                                                                   mockedX320PhaseAcquisitionFlow.Object,
                                                                   mockedX3200PhaseAcquisitionFlow.Object,
                                                                   mockedY32PhaseAcquisitionFlow.Object,
                                                                   mockedY320PhaseAcquisitionFlow.Object,
                                                                   mockedY3200PhaseAcquisitionFlow.Object
                                                               };

            var flowTask =
                new DMTDeflectometryAcquisitionFlowTask(mockedAEFlow.Object, acquirePhaseImagesForPeriodAndDirectionFlows);

            // When
            flowTask.Start(cancellationTokenSource, null, null);
            flowTask.LastAcquisitionTask.Wait();

            // Then
            mockedAEFlow.Verify(aeflow => aeflow.Execute(), Times.Once);
            mockedX32PhaseAcquisitionFlow.Verify(apiFlow => apiFlow.Execute(), Times.Once);
            mockedX320PhaseAcquisitionFlow.Verify(apiFlow => apiFlow.Execute(), Times.Once);
            mockedX3200PhaseAcquisitionFlow.Verify(apiFlow => apiFlow.Execute(), Times.Once);
            mockedY32PhaseAcquisitionFlow.Verify(apiFlow => apiFlow.Execute(), Times.Once);
            mockedY320PhaseAcquisitionFlow.Verify(apiFlow => apiFlow.Execute(), Times.Once);
            mockedY3200PhaseAcquisitionFlow.Verify(apiFlow => apiFlow.Execute(), Times.Once);
        }

        private static Fringe CreateSinglePeriodFringe(int period = 32)
        {
            return new Fringe
            {
                NbImagesPerDirection = 8,
                Period = period,
                FringeType = FringeType.Standard,
                Periods = new List<int> { period }
            };
        }

        private static Fringe CreateMultiPeriodFringe(List<int> periods)
        {
            return new Fringe
            {
                NbImagesPerDirection = 8,
                Period = periods.Min(),
                FringeType = FringeType.Multi,
                Periods = periods
            };
        }

        private static AcquirePhaseImagesForPeriodAndDirectionResult CreatePhaseAcquisitionResult(
            Fringe fringe, FringesDisplacement direction = FringesDisplacement.X, FlowState state = FlowState.Success)
        {
            return new AcquirePhaseImagesForPeriodAndDirectionResult
            {
                ExposureTimeMs = 150,
                Fringe = fringe,
                FringesDisplacementDirection = direction,
                Period = fringe.Period,
                Status = new FlowStatus { State = state },
                TemporaryResults =
                           Enumerable.Repeat(new ServiceImage(),
                                             fringe.NbImagesPerDirection)
                                     .ToList()
            };
        }

        private static ROI CreateWholeWaferROI()
        {
            return new ROI
            {
                RoiType = RoiType.WholeWafer,
                WaferRadius = 300.Millimeters().Micrometers,
                EdgeExclusion = 300.Millimeters().Micrometers
            };
        }

        private Mock<AcquirePhaseImagesForPeriodAndDirectionFlow> CreateMockPhaseAcquisitionFlow(
            MockSequence sequence, Fringe fringe, Side side, FringesDisplacement direction)
        {
            var xPhaseAcquisitionInput =
                new AcquirePhaseImagesForPeriodAndDirectionInput(side, fringe, 32, direction, 0.150);
            var mockedXPhaseAcquisitionFlow = new Mock<AcquirePhaseImagesForPeriodAndDirectionFlow>(
             xPhaseAcquisitionInput, HardwareManager, ClassLocator.Default.GetInstance<IDMTInternalCameraMethods>(),
             _fringeManagerMock.Object);

            mockedXPhaseAcquisitionFlow.InSequence(sequence)
                                       .Setup(paFlow => paFlow.Execute())
                                       .Callback(() => mockedXPhaseAcquisitionFlow.Object.Result =
                                                     CreatePhaseAcquisitionResult(fringe, direction));

            return mockedXPhaseAcquisitionFlow;
        }

        private Mock<AcquirePhaseImagesForPeriodAndDirectionFlow>
            CreateMockPhaseAcquisitionFlowWithResultExposureTimeReferenceToInput(
                MockSequence sequence, Fringe fringe, int period, Side side, FringesDisplacement direction)
        {
            var xPhaseAcquisitionInput =
                new AcquirePhaseImagesForPeriodAndDirectionInput(side, fringe, period, direction, 0.150);
            var mockedXPhaseAcquisitionFlow = new Mock<AcquirePhaseImagesForPeriodAndDirectionFlow>(
             xPhaseAcquisitionInput, HardwareManager, ClassLocator.Default.GetInstance<IDMTInternalCameraMethods>(),
             _fringeManagerMock.Object);

            mockedXPhaseAcquisitionFlow.InSequence(sequence)
                                       .Setup(paFlow => paFlow.Execute())
                                       .Callback(() =>
                                       {
                                           mockedXPhaseAcquisitionFlow.Object.Result =
                                               CreatePhaseAcquisitionResult(fringe, direction);
                                           mockedXPhaseAcquisitionFlow.Object.Result.ExposureTimeMs =
                                               xPhaseAcquisitionInput.ExposureTimeMs;
                                       });

            return mockedXPhaseAcquisitionFlow;
        }

        private Mock<AutoExposureFlow> CreateMockAutoExposureFlow(
            MockSequence sequence, Fringe fringe, Side side, double expectedExposureTimeMs = 145)
        {
            var roi = CreateWholeWaferROI();
            var aeInput = new AutoExposureInput(side, MeasureType.DeflectometryMeasure, roi,
                                                AcquisitionScreenDisplayImage.FringeImage, Colors.White, fringe,
                                                new USPImageMil(), 220);
            var mockedAEFlow = new Mock<AutoExposureFlow>(aeInput, HardwareManager,
                                                          ClassLocator.Default
                                                                      .GetInstance<IDMTInternalCameraMethods>());
            mockedAEFlow.InSequence(sequence)
                        .Setup(aeFlow => aeFlow.Execute())
                        .Callback(() => mockedAEFlow.Object.Result = new AutoExposureResult
                        {
                            ExposureTimeMs = expectedExposureTimeMs,
                            WaferSide = Side.Front,
                            Status = new FlowStatus
                            {
                                State = FlowState.Success
                            }
                        });
            return mockedAEFlow;
        }
    }
}
