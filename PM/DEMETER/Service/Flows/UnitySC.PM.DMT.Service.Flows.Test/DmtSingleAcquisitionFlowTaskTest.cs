using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Flows.AcquireOneImage;
using UnitySC.PM.DMT.Service.Flows.AutoExposure;
using UnitySC.PM.DMT.Service.Flows.FlowTask;
using UnitySC.PM.DMT.Service.Flows.SaveImage;
using UnitySC.PM.DMT.Service.Interface.AlgorithmManager;
using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.RecipeService;
using UnitySC.PM.DMT.Service.Shared.TestUtils;
using UnitySC.PM.DMT.Shared;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.DMT.Service.Flows.Test
{
    [TestClass]
    public class DMTSingleAcquisitionFlowTaskTest : TestWithMockedCameraAndScreen<DMTSingleAcquisitionFlowTaskTest>
    {
        private readonly Mock<DbRegisterAcquisitionServiceProxy> _mockDbService = new Mock<DbRegisterAcquisitionServiceProxy>();

        [TestInitialize]
        public void DbRegisterResultServiceProxySetUp()
        {
            _mockDbService.Reset();
        }

        [TestMethod]
        public void GivenAcquireOneImageStartShouldExecuteFlow()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            double givenExposureTimeMs = 150;
            var aiInput = CreateAcquireImageInputWithExposureTimeMs(givenExposureTimeMs);
            var mockedAIFlow = new Mock<AcquireOneImageFlow>(aiInput,
                                                             ClassLocator.Default
                                                                         .GetInstance<IDMTInternalCameraMethods>(),
                                                             HardwareManager);
            var acquiredImage = new USPImageMil();
            mockedAIFlow.Setup(aiFlow => aiFlow.Execute())
                        .Callback(() =>
                                      mockedAIFlow.Object.Result =
                                          CreateAcquireImageFlowResult(givenExposureTimeMs, acquiredImage));
            var flowTask = new DMTSingleAcquisitionFlowTask(mockedAIFlow.Object);

            // When
            flowTask.Start(cancellationTokenSource, null, null, null, null);
            ((Task)flowTask.LastAcquisitionTask).Wait();

            // Then
            mockedAIFlow.Verify(aiFlow => aiFlow.Execute(), Times.Once());
        }

        [TestMethod]
        public void GivenAcquireOneImageFlowAndSaveImageFlowStartShouldExecuteAllFlows()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            double givenExposureTimeMs = 150;
            var sequence = new MockSequence();
            var aiInput = CreateAcquireImageInputWithExposureTimeMs(givenExposureTimeMs);
            var siInput = CreateSaveImageInput();
            var mockedAIFlow = new Mock<AcquireOneImageFlow>(aiInput,
                                                             ClassLocator.Default
                                                                         .GetInstance<IDMTInternalCameraMethods>(),
                                                             HardwareManager);
            var acquiredImage = new USPImageMil();
            mockedAIFlow.InSequence(sequence)
                        .Setup(aiFlow => aiFlow.Execute())
                        .Callback(() =>
                                      mockedAIFlow.Object.Result =
                                          CreateAcquireImageFlowResult(givenExposureTimeMs, acquiredImage));
            var mockedSIFlow = new Mock<SaveImageFlow>(siInput,
                                                       ClassLocator.Default.GetInstance<IDMTAlgorithmManager>(),
                                                       ClassLocator.Default.GetInstance<ICalibrationManager>(),
                                                       _mockDbService.Object);
            mockedSIFlow.InSequence(sequence)
                        .Setup(siFlow => siFlow.Execute())
                        .Callback(() => mockedSIFlow.Object.Result = CreateSaveImageResult());
            var mockedAIStatusChangedHandler =
                new Mock<FlowComponent<AcquireOneImageInput, AcquireOneImageResult, DefaultConfiguration>.
                    StatusChangedEventHandler>();
            var mockedOnResultGeneratedEventHandler = new Mock<Action<DMTResultGeneratedEventArgs>>();
            var flowTask = new DMTSingleAcquisitionFlowTask(mockedAIFlow.Object, mockedSIFlow.Object);

            // When
            flowTask.Start(cancellationTokenSource, null, mockedAIStatusChangedHandler.Object, null,
                           mockedOnResultGeneratedEventHandler.Object);
            Task.WaitAll((Task)flowTask.LastAcquisitionTask, (Task)flowTask.SaveImageTask);

            // Then
            mockedAIFlow.Verify(aiFlow => aiFlow.Execute(), Times.Once());
            mockedSIFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedOnResultGeneratedEventHandler.Verify(
                                                       resultHandler =>
                                                           resultHandler
                                                               .Invoke(It.IsAny<DMTResultGeneratedEventArgs>()),
                                                       Times.Once);
        }

        [TestMethod]
        public void GivenAutoExposureFlowAcquireOneImageFlowAndSaveImageFlowStartShouldExecuteAllFlows()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            double givenExposureTimeMs = 150;
            var sequence = new MockSequence();
            var aeInput = CreateAutoExposureInput();
            var aiInput = CreateAcquireImageInput();
            var siInput = CreateSaveImageInput();
            var mockedAEFlow = new Mock<AutoExposureFlow>(aeInput, HardwareManager,
                                                          ClassLocator.Default
                                                                      .GetInstance<IDMTInternalCameraMethods>());
            mockedAEFlow.InSequence(sequence)
                        .Setup(aeFlow => aeFlow.Execute())
                        .Callback(() =>
                                      mockedAEFlow.Object.Result =
                                          CreateAutoExposureResultWithExposureTimeMs(givenExposureTimeMs));
            var mockedAIFlow = new Mock<AcquireOneImageFlow>(aiInput,
                                                             ClassLocator.Default
                                                                         .GetInstance<IDMTInternalCameraMethods>(),
                                                             HardwareManager);
            var acquiredImage = new USPImageMil();
            mockedAIFlow.InSequence(sequence)
                        .Setup(aiFlow => aiFlow.Execute())
                        .Callback(() =>
                                      mockedAIFlow.Object.Result =
                                          CreateAcquireImageFlowResult(givenExposureTimeMs, acquiredImage));
            var mockedSIFlow = new Mock<SaveImageFlow>(siInput,
                                                       ClassLocator.Default.GetInstance<IDMTAlgorithmManager>(),
                                                       ClassLocator.Default.GetInstance<ICalibrationManager>(),
                                                       _mockDbService.Object);
            mockedSIFlow.InSequence(sequence)
                        .Setup(siFlow => siFlow.Execute())
                        .Callback(() => mockedSIFlow.Object.Result = CreateSaveImageResult());
            var mockedAEStatusChangedHandler =
                new Mock<FlowComponent<AutoExposureInput, AutoExposureResult, AutoExposureConfiguration>.
                    StatusChangedEventHandler>();
            var mockedAIStatusChangedHandler =
                new Mock<FlowComponent<AcquireOneImageInput, AcquireOneImageResult, DefaultConfiguration>.
                    StatusChangedEventHandler>();
            var mockedOnResultGeneratedEventHandler = new Mock<Action<DMTResultGeneratedEventArgs>>();
            var flowTask = new DMTSingleAcquisitionFlowTask(mockedAEFlow.Object, mockedAIFlow.Object, mockedSIFlow.Object);

            // When
            flowTask.Start(cancellationTokenSource, mockedAEStatusChangedHandler.Object,
                           mockedAIStatusChangedHandler.Object, null, mockedOnResultGeneratedEventHandler.Object);
            Task.WaitAll((Task)flowTask.LastAcquisitionTask, (Task)flowTask.SaveImageTask);

            // Then
            mockedAEFlow.Verify(aeFlow => aeFlow.Execute(), Times.Once());
            mockedAIFlow.Verify(aiFlow => aiFlow.Execute(), Times.Once());
            mockedSIFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedOnResultGeneratedEventHandler.Verify(
                                                       resultHandler =>
                                                           resultHandler
                                                               .Invoke(It.IsAny<DMTResultGeneratedEventArgs>()),
                                                       Times.Once);
        }

        [TestMethod]
        public void GivenAPreviousAcquisitionFlowTaskContinueWithShouldExecuteCorrespondingFlows()
        {
            // Given
            var firstCancellationTokenSource = new CancellationTokenSource();
            var secondCancellationTokenSource = new CancellationTokenSource();
            double firstGivenExposureTimeMs = 100;
            double secondGivenExposureTimeMs = 150;
            var firstAcquiredImage = new USPImageMil();
            var secondAcquiredImage = new USPImageMil();
            var sequence = new MockSequence();
            var firstAEInput = CreateAutoExposureInput();
            var firstAIInput = CreateAcquireImageInput();
            var firstSIInput = CreateSaveImageInput();
            var firstMockedAEFlow = new Mock<AutoExposureFlow>(firstAEInput,
                                                               HardwareManager,
                                                               ClassLocator.Default
                                                                           .GetInstance<IDMTInternalCameraMethods>());
            firstMockedAEFlow.InSequence(sequence)
                             .Setup(aeFlow => aeFlow.Execute())
                             .Callback(() =>
                                           firstMockedAEFlow.Object.Result =
                                               CreateAutoExposureResultWithExposureTimeMs(firstGivenExposureTimeMs));
            var firstMockedAIFlow = new Mock<AcquireOneImageFlow>(firstAIInput,
                                                                  ClassLocator.Default
                                                                              .GetInstance<IDMTInternalCameraMethods>(),
                                                                  HardwareManager);
            firstMockedAIFlow.InSequence(sequence)
                             .Setup(aiFlow => aiFlow.Execute())
                             .Callback(() =>
                                           firstMockedAIFlow.Object.Result =
                                               CreateAcquireImageFlowResult(firstGivenExposureTimeMs,
                                                                            firstAcquiredImage));
            var firstMockedSIFlow = new Mock<SaveImageFlow>(firstSIInput,
                                                            ClassLocator.Default.GetInstance<IDMTAlgorithmManager>(),
                                                            ClassLocator.Default.GetInstance<ICalibrationManager>(),
                                                            _mockDbService.Object);
            firstMockedSIFlow.Setup(siFlow => siFlow.Execute())
                             .Callback(() => firstMockedSIFlow.Object.Result = CreateSaveImageResult());
            var firstFlowTask = new DMTSingleAcquisitionFlowTask(firstMockedAEFlow.Object, firstMockedAIFlow.Object,
                                                           firstMockedSIFlow.Object);
            var secondAEInput = CreateAutoExposureInput();
            var secondAIInput = CreateAcquireImageInput();
            var secondSIInput = CreateSaveImageInput();
            var secondMockedAEFlow = new Mock<AutoExposureFlow>(secondAEInput,
                                                                HardwareManager,
                                                                ClassLocator.Default
                                                                            .GetInstance<IDMTInternalCameraMethods>());
            secondMockedAEFlow.InSequence(sequence)
                              .Setup(aeFlow => aeFlow.Execute())
                              .Callback(() =>
                                            secondMockedAEFlow.Object.Result =
                                                CreateAutoExposureResultWithExposureTimeMs(secondGivenExposureTimeMs));
            var secondMockedAIFlow = new Mock<AcquireOneImageFlow>(secondAIInput,
                                                                   ClassLocator.Default
                                                                               .GetInstance<
                                                                                   IDMTInternalCameraMethods>(),
                                                                   HardwareManager);
            secondMockedAIFlow.InSequence(sequence)
                              .Setup(aiFlow => aiFlow.Execute())
                              .Callback(() =>
                                            secondMockedAIFlow.Object.Result =
                                                CreateAcquireImageFlowResult(secondGivenExposureTimeMs,
                                                                             secondAcquiredImage));
            var secondMockedSIFlow = new Mock<SaveImageFlow>(secondSIInput,
                                                             ClassLocator.Default.GetInstance<IDMTAlgorithmManager>(),
                                                             ClassLocator.Default.GetInstance<ICalibrationManager>(),
                                                             _mockDbService.Object);
            secondMockedSIFlow.Setup(siFlow => siFlow.Execute())
                              .Callback(() => secondMockedSIFlow.Object.Result = CreateSaveImageResult());
            var secondFlowTask = new DMTSingleAcquisitionFlowTask(secondMockedAEFlow.Object, secondMockedAIFlow.Object,
                                                            secondMockedSIFlow.Object);

            // When
            firstFlowTask.Start(firstCancellationTokenSource, null, null, null, null);
            firstFlowTask.ContinueWith(secondFlowTask, null, null, null, null);
            Task.WaitAll((Task)secondFlowTask.LastAcquisitionTask, (Task)secondFlowTask.SaveImageTask);

            // Then
            firstMockedAEFlow.Verify(aeFlow => aeFlow.Execute(), Times.Once());
            firstMockedAIFlow.Verify(aiFlow => aiFlow.Execute(), Times.Once());
            firstMockedSIFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            secondMockedAEFlow.Verify(aeFlow => aeFlow.Execute(), Times.Once());
            secondMockedAIFlow.Verify(aiFlow => aiFlow.Execute(), Times.Once());
            secondMockedSIFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
        }

        [TestMethod]
        public void
            GivenAutoExposureFailureAndIgnoreAutoExposureFailureTestStartShouldExecuteAcquireImageFlowWithDefaultValue()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            double defaultExposureTimeMs = 125;
            var sequence = new MockSequence();
            var aeInput = CreateAutoExposureInput();
            var aiInput = CreateAcquireImageInput();
            var siInput = CreateSaveImageInput();
            var mockedAEFlow = new Mock<AutoExposureFlow>(aeInput, HardwareManager,
                                                          ClassLocator.Default
                                                                      .GetInstance<IDMTInternalCameraMethods>());
            mockedAEFlow.Object.Configuration.IgnoreAutoExposureFailure = true;
            mockedAEFlow.Object.Configuration.DefaultAutoExposureSetting = new List<AutoExposureConfiguration.Config>
                                                                           {
                                                                               new AutoExposureConfiguration.Config
                                                                               {
                                                                                   WaferSide = Side.Front,
                                                                                   Measure = MeasureType
                                                                                       .BrightFieldMeasure,
                                                                                   DefaultExposureTimeMsIfFailure =
                                                                                       defaultExposureTimeMs
                                                                               }
                                                                           };
            mockedAEFlow.InSequence(sequence)
                        .Setup(aeFlow => aeFlow.Execute())
                        .Callback(() =>
                                      mockedAEFlow.Object.Result = CreateFailedAutoExposureFlowResult());
            var mockedAIFlow = new Mock<AcquireOneImageFlow>(aiInput,
                                                             ClassLocator.Default
                                                                         .GetInstance<IDMTInternalCameraMethods>(),
                                                             HardwareManager);
            var acquiredImage = new USPImageMil();
            mockedAIFlow.InSequence(sequence)
                        .Setup(aiFlow => aiFlow.Execute())
                        .Callback(() =>
                                      mockedAIFlow.Object.Result =
                                          CreateAcquireImageFlowResult(aiInput.ExposureTimeMs, acquiredImage));
            var mockedSIFlow = new Mock<SaveImageFlow>(siInput,
                                                       ClassLocator.Default.GetInstance<IDMTAlgorithmManager>(),
                                                       ClassLocator.Default.GetInstance<ICalibrationManager>(),
                                                       _mockDbService.Object);
            mockedSIFlow.InSequence(sequence)
                        .Setup(siFlow => siFlow.Execute())
                        .Callback(() => mockedSIFlow.Object.Result = CreateSaveImageResult());
            var flowTask = new DMTSingleAcquisitionFlowTask(mockedAEFlow.Object, mockedAIFlow.Object, mockedSIFlow.Object);

            // When
            flowTask.Start(cancellationTokenSource, null, null, null, null);
            Task.WaitAll((Task)flowTask.LastAcquisitionTask, (Task)flowTask.SaveImageTask);

            // Then
            mockedAEFlow.Verify(aeFlow => aeFlow.Execute(), Times.Once());
            mockedAIFlow.Verify(aiFlow => aiFlow.Execute(), Times.Once());
            mockedSIFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            flowTask.LastAcquisitionTask.Result.ExposureTimeMs.Should().Be(defaultExposureTimeMs);
        }

        [TestMethod]
        public void GivenAutoExposureFlowFailureTaskExecutionShouldReturnFollowingFlowsInCancelledState()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var sequence = new MockSequence();
            var aeInput = CreateAutoExposureInput();
            var aiInput = CreateAcquireImageInput();
            var siInput = CreateSaveImageInput();
            var mockedAEFlow = new Mock<AutoExposureFlow>(aeInput, HardwareManager,
                                                          ClassLocator.Default
                                                                      .GetInstance<IDMTInternalCameraMethods>());
            mockedAEFlow.Object.Result = new AutoExposureResult();
            mockedAEFlow.InSequence(sequence).Setup(aeFlow => aeFlow.Execute()).Callback(() =>
            {
                mockedAEFlow.Object.Result = new AutoExposureResult
                {
                    Status = new FlowStatus
                    {
                        State = FlowState.Error,
                        Message = "Error"
                    }
                };
            });
            var mockedAIFlow = new Mock<AcquireOneImageFlow>(aiInput,
                                                             ClassLocator.Default
                                                                         .GetInstance<IDMTInternalCameraMethods>(),
                                                             HardwareManager);
            var acquiredImage = new USPImageMil();
            var mockedSIFlow = new Mock<SaveImageFlow>(siInput,
                                                       ClassLocator.Default.GetInstance<IDMTAlgorithmManager>(),
                                                       ClassLocator.Default.GetInstance<ICalibrationManager>(),
                                                       _mockDbService.Object);
            var flowTask = new DMTSingleAcquisitionFlowTask(mockedAEFlow.Object, mockedAIFlow.Object, mockedSIFlow.Object);

            // When + Then
            flowTask.Start(cancellationTokenSource, null, null, null, null);
            var tasksToWait = new Task[] { (Task)flowTask.LastAcquisitionTask, (Task)flowTask.SaveImageTask };
            Task.WaitAll(tasksToWait);

            flowTask.LastAcquisitionTask.Status.Should().Be(TaskStatus.RanToCompletion);
            flowTask.LastAcquisitionTask.Result.Should().NotBeNull().And.BeAssignableTo<IFlowResult>().Which.Status
                .State.Should().Be(FlowState.Canceled);
            flowTask.SaveImageTask.Status.Should().Be(TaskStatus.RanToCompletion);
            flowTask.SaveImageTask.Result.Should().NotBeNull().And.BeOfType<SaveImageResult>().Which.Status.State
                .Should().Be(FlowState.Canceled);
        }

        [TestMethod]
        public void GivenAcquireImageFlowFailureTaskExecutionShouldReturnASaveImageFlowInTheCancelledState()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            double givenExposureTimeMs = 150;
            var sequence = new MockSequence();

            var aeInput = CreateAutoExposureInput();
            var aiInput = CreateAcquireImageInput();
            var siInput = CreateSaveImageInput();
            var mockedAEFlow = new Mock<AutoExposureFlow>(aeInput, HardwareManager,
                                                          ClassLocator.Default
                                                                      .GetInstance<IDMTInternalCameraMethods>());
            mockedAEFlow.InSequence(sequence)
                        .Setup(aeFlow => aeFlow.Execute())
                        .Callback(() =>
                                      mockedAEFlow.Object.Result =
                                          CreateAutoExposureResultWithExposureTimeMs(givenExposureTimeMs));
            var mockedAIFlow = new Mock<AcquireOneImageFlow>(aiInput,
                                                             ClassLocator.Default
                                                                         .GetInstance<IDMTInternalCameraMethods>(),
                                                             HardwareManager);
            mockedAIFlow.Object.Result = new AcquireOneImageResult();
            mockedAIFlow.InSequence(sequence).Setup(aiFlow => aiFlow.Execute()).Callback(() =>
            {
                mockedAIFlow.Object.Result = new AcquireOneImageResult()
                {
                    Status = new FlowStatus(FlowState.Error, "Error during AcquireOneImageFlow"),
                    AcquiredImage = null,
                    ExposureTimeMs = givenExposureTimeMs,
                };
            });
            var acquiredImage = new USPImageMil();
            var mockedSIFlow = new Mock<SaveImageFlow>(siInput,
                                                       ClassLocator.Default.GetInstance<IDMTAlgorithmManager>(),
                                                       ClassLocator.Default.GetInstance<ICalibrationManager>(),
                                                       _mockDbService.Object);
            var flowTask = new DMTSingleAcquisitionFlowTask(mockedAEFlow.Object, mockedAIFlow.Object, mockedSIFlow.Object);

            // When + Then

            flowTask.Start(cancellationTokenSource, null, null, null, null);
            Task.WaitAll((Task)flowTask.LastAcquisitionTask, (Task)flowTask.SaveImageTask);

            mockedAEFlow.Verify(aeFlow => aeFlow.Execute(), Times.Once());
            mockedAIFlow.Verify(aiFlow => aiFlow.Execute(), Times.Once());
            mockedSIFlow.Verify(siFlow => siFlow.Execute(), Times.Never());
            flowTask.FirstTask.Status.Should().Be(TaskStatus.RanToCompletion);
            flowTask.LastAcquisitionTask.Status.Should().Be(TaskStatus.RanToCompletion);
            flowTask.SaveImageTask.Status.Should().Be(TaskStatus.RanToCompletion);
            flowTask.SaveImageTask.Result.Should().NotBeNull().And.BeOfType<SaveImageResult>().Which.Status.State.Should().Be(FlowState.Canceled);
        }

        [TestMethod]
        public void GivenAPreviousAcquisitionFlowFailureTaskContinueWithShouldNotExecuteCorrespondingFlows()
        {
            // Given
            var firstCancellationTokenSource = new CancellationTokenSource();
            var secondCancellationTokenSource = new CancellationTokenSource();
            double firstGivenExposureTimeMs = 100;
            double secondGivenExposureTimeMs = 150;
            var firstAcquiredImage = new USPImageMil();
            var secondAcquiredImage = new USPImageMil();
            var sequence = new MockSequence();
            var firstAEInput = CreateAutoExposureInput();
            var firstAIInput = CreateAcquireImageInput();
            var firstSIInput = CreateSaveImageInput();
            var firstMockedAEFlow = new Mock<AutoExposureFlow>(firstAEInput,
                                                               HardwareManager,
                                                               ClassLocator.Default
                                                                           .GetInstance<IDMTInternalCameraMethods>());
            firstMockedAEFlow.InSequence(sequence)
                             .Setup(aeFlow => aeFlow.Execute())
                             .Callback(() =>
                                           firstMockedAEFlow.Object.Result =
                                               CreateAutoExposureResultWithExposureTimeMs(firstGivenExposureTimeMs));
            var firstMockedAIFlow = new Mock<AcquireOneImageFlow>(firstAIInput,
                                                                  ClassLocator.Default
                                                                              .GetInstance<IDMTInternalCameraMethods>(),
                                                                  HardwareManager);
            firstMockedAIFlow.Object.Result = new AcquireOneImageResult();
            firstMockedAIFlow.InSequence(sequence).Setup(aiFlow => aiFlow.Execute()).Callback(() =>
            {
                firstMockedAIFlow.Object.Result = new AcquireOneImageResult()
                {
                    Status = new FlowStatus(FlowState.Error, "Error during AcquireOneImageFlow"),
                    AcquiredImage = null,
                    ExposureTimeMs = firstGivenExposureTimeMs
                };
            });
            var firstMockedSIFlow = new Mock<SaveImageFlow>(firstSIInput,
                                                            ClassLocator.Default.GetInstance<IDMTAlgorithmManager>(),
                                                            ClassLocator.Default.GetInstance<ICalibrationManager>(),
                                                            _mockDbService.Object);
            firstMockedSIFlow.InSequence(sequence)
                .Setup(siFlow => siFlow.Execute())
                .Callback(() => firstMockedSIFlow.Object.Result = CreateSaveImageResult());
            var firstFlowTask = new DMTSingleAcquisitionFlowTask(firstMockedAEFlow.Object, firstMockedAIFlow.Object,
                                                           firstMockedSIFlow.Object);
            var secondAEInput = CreateAutoExposureInput();
            var secondAIInput = CreateAcquireImageInput();
            var secondSIInput = CreateSaveImageInput();
            var secondMockedAEFlow = new Mock<AutoExposureFlow>(secondAEInput,
                                                                HardwareManager,
                                                                ClassLocator.Default
                                                                            .GetInstance<IDMTInternalCameraMethods>());
            secondMockedAEFlow.InSequence(sequence)
                .Setup(aeFlow => aeFlow.Execute())
                .Callback(() =>
                    secondMockedAEFlow.Object.Result =
                        CreateAutoExposureResultWithExposureTimeMs(secondGivenExposureTimeMs));
            var secondMockedAIFlow = new Mock<AcquireOneImageFlow>(secondAIInput,
                                                                   ClassLocator.Default
                                                                               .GetInstance<
                                                                                   IDMTInternalCameraMethods>(),
                                                                   HardwareManager);
            secondMockedAIFlow.InSequence(sequence)
                .Setup(aiFlow => aiFlow.Execute())
                .Callback(() => secondMockedAIFlow.Object.Result = CreateAcquireImageFlowResult(
                    secondGivenExposureTimeMs,
                    secondAcquiredImage));
            var secondMockedSIFlow = new Mock<SaveImageFlow>(secondSIInput,
                                                             ClassLocator.Default.GetInstance<IDMTAlgorithmManager>(),
                                                             ClassLocator.Default.GetInstance<ICalibrationManager>(),
                                                             _mockDbService.Object);
            secondMockedSIFlow.InSequence(sequence)
                .Setup(siFlow => siFlow.Execute())
                .Callback(() => secondMockedSIFlow.Object.Result = CreateSaveImageResult());
            var secondFlowTask = new DMTSingleAcquisitionFlowTask(secondMockedAEFlow.Object, secondMockedAIFlow.Object,
                                                            secondMockedSIFlow.Object);

            // When
            firstFlowTask.Start(firstCancellationTokenSource, null, null, null, null);
            firstFlowTask.ContinueWith(secondFlowTask, null, null, null, null);

            // Then
            Task.WaitAll((Task)firstFlowTask.LastAcquisitionTask, (Task)firstFlowTask.SaveImageTask,
                (Task)secondFlowTask.LastAcquisitionTask, (Task)secondFlowTask.SaveImageTask);
            firstMockedAEFlow.Verify(aeFlow => aeFlow.Execute(), Times.Once());
            firstMockedAIFlow.Verify(aiFlow => aiFlow.Execute(), Times.Once());
            firstMockedSIFlow.Verify(siFlow => siFlow.Execute(), Times.Never());
            secondMockedAEFlow.Verify(aeFlow => aeFlow.Execute(), Times.Never());
            secondMockedAIFlow.Verify(aiFlow => aiFlow.Execute(), Times.Never());
            secondMockedSIFlow.Verify(siFlow => siFlow.Execute(), Times.Never());
            firstFlowTask.SaveImageTask.Status.Should().Be(TaskStatus.RanToCompletion);
            firstFlowTask.SaveImageTask.Result.Should().NotBeNull().And.BeOfType<SaveImageResult>().Which.Status.State.Should().Be(FlowState.Canceled);
            secondFlowTask.FirstTask.Status.Should().Be(TaskStatus.RanToCompletion);
            secondFlowTask.FirstTask.As<AutoExposureFlowTask>().Result.Should().NotBeNull().And
                .BeOfType<AutoExposureResult>().Which.Status.State.Should().Be(FlowState.Canceled);
            secondFlowTask.LastAcquisitionTask.Status.Should().Be(TaskStatus.RanToCompletion);
            secondFlowTask.LastAcquisitionTask.Result.Should().NotBeNull().And.BeOfType<AcquireOneImageResult>().Which
                .Status.State.Should().Be(FlowState.Canceled);
            secondFlowTask.SaveImageTask.Status.Should().Be(TaskStatus.RanToCompletion);
            secondFlowTask.SaveImageTask.Result.Should().NotBeNull().And.BeOfType<SaveImageResult>().Which.Status.State
                .Should().Be(FlowState.Canceled);
        }

        private static AcquireOneImageInput CreateAcquireImageInput()
        {
            return new AcquireOneImageInput
            {
                CameraSide = Side.Front,
                ScreenColor = Colors.White,
                ScreenSide = Side.Front,
                DisplayImageType = AcquisitionScreenDisplayImage.Color,
                MeasureType = MeasureType.BrightFieldMeasure
            };
        }

        private static AcquireOneImageInput CreateAcquireImageInputWithExposureTimeMs(double givenExposureTimeMs)
        {
            var aiInput = CreateAcquireImageInput();
            aiInput.ExposureTimeMs = givenExposureTimeMs;
            return aiInput;
        }

        private static AcquireOneImageResult CreateAcquireImageFlowResult(
            double givenExposureTimeMs,
            USPImageMil acquiredImage, FlowState resultState = FlowState.Success)
        {
            return new AcquireOneImageResult
            {
                ExposureTimeMs = givenExposureTimeMs,
                AcquiredImage = acquiredImage,
                Status = new FlowStatus { State = resultState }
            };
        }

        private static SaveImageInput CreateSaveImageInput()
        {
            return new SaveImageInput
            {
                RecipeInfo = new RecipeInfo(),
                ImageName = "TestImage.tiff",
                SaveFullPath = "C:\\Some\\Save\\Path\\TestImage.tiff",
                DMTResultType = DMTResult.BrightField_Front,
                RemoteProductionInfo = new RemoteProductionInfo()
            };
        }

        private static SaveImageResult CreateSaveImageResult(FlowState resultState = FlowState.Success)
        {
            return new SaveImageResult
            {
                ImageName = "TestImage.tiff",
                ImageSide = Side.Front,
                SavePath = "C:\\Some\\Save\\Path",
                Status = new FlowStatus { State = resultState }
            };
        }

        private static AutoExposureInput CreateAutoExposureInput()
        {
            return new AutoExposureInput
            {
                InitialAutoExposureTimeMs = 20,
                Color = Colors.White,
                DisplayImageType = AcquisitionScreenDisplayImage.Color,
                MeasureType = MeasureType.BrightFieldMeasure,
                Side = Side.Front,
                RoiForMask = new ROI { RoiType = RoiType.WholeWafer }
            };
        }

        private static AutoExposureResult CreateAutoExposureResultWithExposureTimeMs(double givenExposureTimeMs)
        {
            var aeResult = CreateBaseAutoExposureResult();
            aeResult.ExposureTimeMs = givenExposureTimeMs;
            return aeResult;
        }

        private static AutoExposureResult CreateFailedAutoExposureFlowResult()
        {
            return CreateBaseAutoExposureResult(FlowState.Error, "[AutoExposure] Auto-exposure failed");
        }

        private static AutoExposureResult CreateBaseAutoExposureResult(
            FlowState resultState = FlowState.Success,
            string message = null)
        {
            var aeRsult = new AutoExposureResult
            {
                WaferSide = Side.Front,
                Status = new FlowStatus { State = resultState }
            };
            if (!message.IsNullOrEmpty())
            {
                aeRsult.Status.Message = message;
            }

            return aeRsult;
        }
    }
}
