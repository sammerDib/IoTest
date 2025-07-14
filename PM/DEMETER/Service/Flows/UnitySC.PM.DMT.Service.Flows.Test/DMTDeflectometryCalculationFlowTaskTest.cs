using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Flows.Deflectometry;
using UnitySC.PM.DMT.Service.Flows.FlowTask;
using UnitySC.PM.DMT.Service.Flows.SaveImage;
using UnitySC.PM.DMT.Service.Interface.AlgorithmManager;
using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.DMT.Service.Interface.Fringe;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Shared.TestUtils;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Data.Ada;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Proxy;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Flows.Test
{
    [TestClass]
    [Ignore("Test temporarily disabled due to instability. to be corrected.")]
    public class
        DMTDeflectometryCalculationFlowTaskTest : TestWithMockedCameraAndScreen<DMTDeflectometryCalculationFlowTaskTest>
    {
        private readonly Mock<IDMTAlgorithmManager> _algorithmManagerMock = new Mock<IDMTAlgorithmManager>();

        private readonly Mock<ICalibrationManager> _calibrationManagerMock = new Mock<ICalibrationManager>();

        private readonly Mock<IDMTInternalCameraMethods> _cameraMethodsMock = new Mock<IDMTInternalCameraMethods>();

        private readonly Mock<DbRegisterAcquisitionServiceProxy> _dbResultServiceMock =
            new Mock<DbRegisterAcquisitionServiceProxy>();

        private readonly Mock<IFringeManager> _fringeManagerMock = new Mock<IFringeManager>();

        private readonly Mock<PMConfiguration> _pmConfigurationMock = new Mock<PMConfiguration>();

        [TestInitialize]
        public void MockSetup()
        {
            _cameraMethodsMock.Reset();
            _fringeManagerMock.Reset();
            _algorithmManagerMock.Reset();
            _calibrationManagerMock.Reset();
            _pmConfigurationMock.Reset();
            _dbResultServiceMock.Reset();
            _fringeManagerMock.Setup(m => m.GetFringeImageDict(Side.Front,
                                                               It.Is<Fringe>(fringe =>
                                                                                 fringe.FringeType ==
                                                                                 FringeType.Standard &&
                                                                                 fringe.Period == 32 &&
                                                                                 fringe.NbImagesPerDirection == 8)))
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
        public void GivenAPreviousSinglePeriodAcquisitionFlowTaskRelevantFlowsAreExecuted()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            var xSequence = new MockSequence();
            var ySequence = new MockSequence();
            var fringe = CreateDefaultFringe();
            var mockedXPhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(xSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedYPhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(ySequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);
            var mockedXComputePhaseMapFlow =
                CreateComputePhaseMapFlow(xSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedYComputePhaseMapFlow =
                CreateComputePhaseMapFlow(ySequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);
            var mockedAdaWriter = new Mock<AdaWriter>(MockBehavior.Default, 0, 0, "adaFile.tmp");
            var mockedSaveMaskFlow = CreateSaveMaskFlow(mockedAdaWriter);
            mockedSaveMaskFlow.InSequence(ySequence).Setup(saveMaskFlow => saveMaskFlow.Execute()).Callback(() => mockedSaveMaskFlow.Object.Result = new SaveMaskResult
            {
                Status = new FlowStatus(FlowState.Success, ""),
                MaskSide = Side.Front,
                SavePath = @"C:\Some\Path",
                MaskFileName = "Mask.tif"
            });
            var phaseAcquisitionFlowList = new List<AcquirePhaseImagesForPeriodAndDirectionFlow>
                                           {
                                               mockedXPhaseAcquisitionFlow.Object,
                                               mockedYPhaseAcquisitionFlow.Object
                                           };
            var computePhaseMapFlowList = new List<ComputePhaseMapAndMaskForPeriodAndDirectionFlow>
                                          {
                                              mockedXComputePhaseMapFlow.Object,
                                              mockedYComputePhaseMapFlow.Object
                                          };
            var acquisitionFlowTask = new DMTDeflectometryAcquisitionFlowTask(phaseAcquisitionFlowList);
            acquisitionFlowTask.Start(cancellationTokenSource, null, null);
            var computationFlowTask =
                new DMTDeflectometryCalculationFlowTask(acquisitionFlowTask, computePhaseMapFlowList, mockedSaveMaskFlow.Object);

            // When
            computationFlowTask.CreateAndChainComputationContinuationTasks(args => {});
            Task.WaitAll((Task)acquisitionFlowTask.LastAcquisitionTask, computationFlowTask.LastComputationTask.ToTask());

            // Then
            mockedXComputePhaseMapFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedYComputePhaseMapFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedSaveMaskFlow.Verify(smFlow => smFlow.Execute(), Times.Once());
            mockedSaveMaskFlow.Object.Input.MaskToSave.Should().BeSameAs(mockedXComputePhaseMapFlow.Object.Result.PsdResult.Mask);
        }

        [TestMethod]
        public void GivenASinglePeriodAcquisitionFlowTaskAndCurvatureMapOutputsRelevantFlowsShouldBeExecuted()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            var xSequence = new MockSequence();
            var ySequence = new MockSequence();
            var fringe = CreateDefaultFringe();

            var mockedXPhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(xSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedYPhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(ySequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);

            var mockedXComputePhaseMapFlow =
                CreateComputePhaseMapFlow(xSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedYComputePhaseMapFlow =
                CreateComputePhaseMapFlow(ySequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);

            var mockedXComputeRawCurvatureMapFlow =
                CreateMockedRawComputeCurvatureMapFlow(xSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedYComputeRawCurvatureMapFlow =
                CreateMockedRawComputeCurvatureMapFlow(ySequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);

            var mockedXAdjustCurvatureMapFlow =
                CreateMockedAdjustCurvatureMapFlow(xSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedYAdjustCurvatureMapFlow =
                CreateMockedAdjustCurvatureMapFlow(ySequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);

            var mockedFrontAdaWriter = new Mock<AdaWriter>(MockBehavior.Default, 0,  1, "someAdaFile.tmp");
            
            var mockedXCurvatureMapSaveImageFlow = CreateMockedCurvatureMapSaveImageFlow(xSequence,
             mockedFrontAdaWriter, FringesDisplacement.X, FlowState.Success);
            var mockedYCurvatureMapSaveImageFlow = CreateMockedCurvatureMapSaveImageFlow(ySequence,
             mockedFrontAdaWriter, FringesDisplacement.Y, FlowState.Success);
            var mockedSaveMaskFlow = CreateSaveMaskFlow(mockedFrontAdaWriter);
            mockedSaveMaskFlow.InSequence(ySequence).Setup(flow => flow.Execute()).Callback(() =>
            {
                mockedSaveMaskFlow.Object.Result = new SaveMaskResult
                {
                    Status = new FlowStatus(FlowState.Success, ""),
                    MaskSide = Side.Front,
                    SavePath = @"C:\Some\Path",
                    MaskFileName = @"Mask.tif"
                };
            });

            var phaseAcquisitionFlowList = new List<AcquirePhaseImagesForPeriodAndDirectionFlow>
                                           {
                                               mockedXPhaseAcquisitionFlow.Object,
                                               mockedYPhaseAcquisitionFlow.Object
                                           };
            var computePhaseMapFlowList = new List<ComputePhaseMapAndMaskForPeriodAndDirectionFlow>
                                          {
                                              mockedXComputePhaseMapFlow.Object,
                                              mockedYComputePhaseMapFlow.Object
                                          };
            var adjustCurvatureMapFlowByComputeRawCurvatureMapFlows =
                new Dictionary<ComputeRawCurvatureMapForPeriodAndDirectionFlow,
                    AdjustCurvatureDynamicsForRawCurvatureMapFlow>
                {
                    { mockedXComputeRawCurvatureMapFlow.Object, mockedXAdjustCurvatureMapFlow.Object },
                    { mockedYComputeRawCurvatureMapFlow.Object, mockedYAdjustCurvatureMapFlow.Object }
                };

            var saveImageFlowsByAdjustCurvatureMapFlows =
                new Dictionary<AdjustCurvatureDynamicsForRawCurvatureMapFlow, SaveImageFlow>
                {
                    { mockedXAdjustCurvatureMapFlow.Object, mockedXCurvatureMapSaveImageFlow.Object },
                    { mockedYAdjustCurvatureMapFlow.Object, mockedYCurvatureMapSaveImageFlow.Object }
                };

            var acquisitionFlowTask = new DMTDeflectometryAcquisitionFlowTask(phaseAcquisitionFlowList);
            acquisitionFlowTask.Start(cancellationTokenSource, null, null);
            var computationFlowTask = new DMTDeflectometryCalculationFlowTask(acquisitionFlowTask,
                    computePhaseMapFlowList)
                .AddSaveMaskFlow(mockedSaveMaskFlow.Object)
                .AddCurvatureMapFlows(adjustCurvatureMapFlowByComputeRawCurvatureMapFlows,
                    saveImageFlowsByAdjustCurvatureMapFlows);

            // When
            computationFlowTask.CreateAndChainComputationContinuationTasks(args => {});
            var tasksToWaitList = new List<IFlowTask>(computationFlowTask.SaveImageTasks.Count + 2)
                                  {
                                      computationFlowTask.LastComputationTask,
                                      computationFlowTask.SaveMaskFlowTask,
                                  };
            tasksToWaitList.AddRange(computationFlowTask.SaveImageTasks);
            Task.WaitAll(tasksToWaitList.Select(flowTask => flowTask.ToTask()).ToArray());

            // Then
            mockedXComputePhaseMapFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedYComputePhaseMapFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedXComputeRawCurvatureMapFlow.Verify(ccmFlow => ccmFlow.Execute(), Times.Once());
            mockedYComputeRawCurvatureMapFlow.Verify(ccmFlow => ccmFlow.Execute(), Times.Once());
            mockedXCurvatureMapSaveImageFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedYCurvatureMapSaveImageFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedSaveMaskFlow.Verify(smFlow => smFlow.Execute(), Times.Once());
            mockedXComputeRawCurvatureMapFlow.Object.Input.PhaseMapAndMask.Should()
                                             .BeSameAs(mockedXComputePhaseMapFlow.Object.Result.PsdResult);
            mockedYComputeRawCurvatureMapFlow.Object.Input.PhaseMapAndMask.Should()
                                             .BeSameAs(mockedYComputePhaseMapFlow.Object.Result.PsdResult);
            mockedXAdjustCurvatureMapFlow.Object.Input.RawCurvatureMap.Should()
                                         .BeSameAs(mockedXComputeRawCurvatureMapFlow.Object.Result.RawCurvatureMap);
            mockedXAdjustCurvatureMapFlow.Object.Input.Mask.Should()
                                         .BeSameAs(mockedXComputeRawCurvatureMapFlow.Object.Result.Mask)
                                         .And.BeSameAs(mockedXComputePhaseMapFlow.Object.Result.PsdResult.Mask);
            mockedYAdjustCurvatureMapFlow.Object.Input.RawCurvatureMap.Should()
                                         .BeSameAs(mockedYComputeRawCurvatureMapFlow.Object.Result.RawCurvatureMap);
            mockedYAdjustCurvatureMapFlow.Object.Input.Mask.Should()
                                         .BeSameAs(mockedYComputeRawCurvatureMapFlow.Object.Result.Mask)
                                         .And.BeSameAs(mockedYComputePhaseMapFlow.Object.Result.PsdResult.Mask);
            mockedXCurvatureMapSaveImageFlow.Object.Input.ImageDataToSave.Should()
                                            .BeSameAs(mockedXAdjustCurvatureMapFlow.Object.Result.CurvatureMap);
            mockedYCurvatureMapSaveImageFlow.Object.Input.ImageDataToSave.Should()
                                            .BeSameAs(mockedYAdjustCurvatureMapFlow.Object.Result.CurvatureMap);
            mockedSaveMaskFlow.Object.Input.MaskToSave.Should().BeSameAs(mockedXComputePhaseMapFlow.Object.Result.PsdResult.Mask);
        }

        [TestMethod]
        public void GivenASinglePeriodAcquisitionAndDarkOutputRelevantFlowsShouldBeExecuted()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            var xSequence = new MockSequence();
            var ySequence = new MockSequence();
            var fringe = CreateDefaultFringe();

            var mockedXPhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(xSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedYPhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(ySequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);

            var mockedXComputePhaseMapFlow =
                CreateComputePhaseMapFlow(xSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedYComputePhaseMapFlow =
                CreateComputePhaseMapFlow(ySequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);
            var mockedComputeDarkImageFlow = CreateMockedComputeDarkImageFlow(ySequence, fringe, 32, FlowState.Success);

            var mockedFrontAdaWriter = new Mock<AdaWriter>(MockBehavior.Default, 0, 1, "someAdaFile.tmp");
            var mockedDarkSaveImageFlow = CreateMockedDarkSaveImageFlow(ySequence,
                                                                        mockedFrontAdaWriter, FlowState.Success);
            var mockedSaveMaskFlow = CreateSaveMaskFlow(mockedFrontAdaWriter);
            mockedSaveMaskFlow.InSequence(ySequence).Setup(flow => flow.Execute()).Callback(() =>
            {
                mockedSaveMaskFlow.Object.Result = new SaveMaskResult
                {
                    Status = new FlowStatus(FlowState.Success, ""),
                    MaskSide = Side.Front,
                    SavePath = @"C:\Some\Path",
                    MaskFileName = @"Mask.tif"
                };
            });
            var phaseAcquisitionFlowList = new List<AcquirePhaseImagesForPeriodAndDirectionFlow>
                                           {
                                               mockedXPhaseAcquisitionFlow.Object,
                                               mockedYPhaseAcquisitionFlow.Object
                                           };
            var computePhaseMapFlowList = new List<ComputePhaseMapAndMaskForPeriodAndDirectionFlow>
                                          {
                                              mockedXComputePhaseMapFlow.Object,
                                              mockedYComputePhaseMapFlow.Object
                                          };
            var acquisitionFlowTask = new DMTDeflectometryAcquisitionFlowTask(phaseAcquisitionFlowList);
            acquisitionFlowTask.Start(cancellationTokenSource, null, null);
            var computationFlowTask = new DMTDeflectometryCalculationFlowTask(acquisitionFlowTask,
                    computePhaseMapFlowList)
                .AddSaveMaskFlow(mockedSaveMaskFlow.Object)
                .AddLowAngleDarkFieldFlows(mockedComputeDarkImageFlow.Object, mockedDarkSaveImageFlow.Object);

            // When
            computationFlowTask.CreateAndChainComputationContinuationTasks(args => {});
            var tasksToWaitList = new List<IFlowTask>(computationFlowTask.SaveImageTasks.Count + 1)
                                  {
                                      computationFlowTask.LastComputationTask
                                  };
            tasksToWaitList.AddRange(computationFlowTask.SaveImageTasks);
            Task.WaitAll(tasksToWaitList.Select(flowTask => flowTask.ToTask()).ToArray());

            // Then
            mockedComputeDarkImageFlow.Verify(cdiFlow => cdiFlow.Execute(), Times.Once());
            mockedDarkSaveImageFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedSaveMaskFlow.Verify(smFlow => smFlow.Execute(), Times.Once());
            mockedComputeDarkImageFlow.Object.Input.XResult.Should()
                                      .BeSameAs(mockedXComputePhaseMapFlow.Object.Result.PsdResult);
            mockedComputeDarkImageFlow.Object.Input.YResult.Should()
                                      .BeSameAs(mockedYComputePhaseMapFlow.Object.Result.PsdResult);
            mockedDarkSaveImageFlow.Object.Input.ImageDataToSave.Should()
                                   .BeSameAs(mockedComputeDarkImageFlow.Object.Result.DarkImage);
            mockedSaveMaskFlow.Object.Input.MaskToSave.Should().BeSameAs(mockedXComputePhaseMapFlow.Object.Result.PsdResult.Mask);
        }

        [TestMethod]
        public void GivenASinglePeriodAcquisitionAndCurvatureMapsAndDarkOutputsRelevantFlowsShouldBeExecuted()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            var xSequence = new MockSequence();
            var ySequence = new MockSequence();
            var fringe = CreateDefaultFringe();

            var mockedXPhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(xSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedYPhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(ySequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);

            var mockedXComputePhaseMapFlow =
                CreateComputePhaseMapFlow(xSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedYComputePhaseMapFlow =
                CreateComputePhaseMapFlow(ySequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);

            var mockedXComputeRawCurvatureMapFlow =
                CreateMockedRawComputeCurvatureMapFlow(xSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedYComputeRawCurvatureMapFlow =
                CreateMockedRawComputeCurvatureMapFlow(ySequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);

            var mockedXAdjustCurvatureMapFlow =
                CreateMockedAdjustCurvatureMapFlow(xSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedYAdjustCurvatureMapFlow =
                CreateMockedAdjustCurvatureMapFlow(ySequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);

            var mockedFrontAdaWriter = new Mock<AdaWriter>(MockBehavior.Default, 0, 1, "someAdaFile.tmp");
            var mockedXCurvatureMapSaveImageFlow = CreateMockedCurvatureMapSaveImageFlow(xSequence,
             mockedFrontAdaWriter, FringesDisplacement.X, FlowState.Success);
            var mockedYCurvatureMapSaveImageFlow = CreateMockedCurvatureMapSaveImageFlow(ySequence,
             mockedFrontAdaWriter, FringesDisplacement.Y, FlowState.Success);
            var mockedComputeDarkImageFlow = CreateMockedComputeDarkImageFlow(ySequence, fringe, 32, FlowState.Success);
            var mockedDarkSaveImageFlow = CreateMockedDarkSaveImageFlow(ySequence, mockedFrontAdaWriter, FlowState.Success);

            var mockedSaveMaskFlow = CreateSaveMaskFlow(mockedFrontAdaWriter);
            mockedSaveMaskFlow.InSequence(ySequence).Setup(flow => flow.Execute()).Callback(() =>
            {
                mockedSaveMaskFlow.Object.Result = new SaveMaskResult
                {
                    Status = new FlowStatus(FlowState.Success, ""),
                    MaskSide = Side.Front,
                    SavePath = @"C:\Some\Path",
                    MaskFileName = @"Mask.tif"
                };
            });
            
            var phaseAcquisitionFlowList = new List<AcquirePhaseImagesForPeriodAndDirectionFlow>
                                           {
                                               mockedXPhaseAcquisitionFlow.Object,
                                               mockedYPhaseAcquisitionFlow.Object
                                           };
            var computePhaseMapFlowList = new List<ComputePhaseMapAndMaskForPeriodAndDirectionFlow>
                                          {
                                              mockedXComputePhaseMapFlow.Object,
                                              mockedYComputePhaseMapFlow.Object
                                          };
            var adjustCurvatureMapFlowByComputeRawCurvatureMapFlows =
                new Dictionary<ComputeRawCurvatureMapForPeriodAndDirectionFlow,
                    AdjustCurvatureDynamicsForRawCurvatureMapFlow>
                {
                    { mockedXComputeRawCurvatureMapFlow.Object, mockedXAdjustCurvatureMapFlow.Object },
                    { mockedYComputeRawCurvatureMapFlow.Object, mockedYAdjustCurvatureMapFlow.Object }
                };

            var saveImageFlowsByAdjustCurvatureMapFlows =
                new Dictionary<AdjustCurvatureDynamicsForRawCurvatureMapFlow, SaveImageFlow>
                {
                    { mockedXAdjustCurvatureMapFlow.Object, mockedXCurvatureMapSaveImageFlow.Object },
                    { mockedYAdjustCurvatureMapFlow.Object, mockedYCurvatureMapSaveImageFlow.Object }
                };
            var acquisitionFlowTask = new DMTDeflectometryAcquisitionFlowTask(phaseAcquisitionFlowList);
            acquisitionFlowTask.Start(cancellationTokenSource, null, null);
            var computationFlowTask = new DMTDeflectometryCalculationFlowTask(acquisitionFlowTask,
                    computePhaseMapFlowList)
                .AddSaveMaskFlow(mockedSaveMaskFlow.Object)
                .AddCurvatureMapFlows(adjustCurvatureMapFlowByComputeRawCurvatureMapFlows,
                    saveImageFlowsByAdjustCurvatureMapFlows)
                .AddLowAngleDarkFieldFlows(mockedComputeDarkImageFlow.Object, mockedDarkSaveImageFlow.Object);

            // When
            computationFlowTask.CreateAndChainComputationContinuationTasks(args => {}, acquisitionFlowTask.LastAcquisitionTask);

            var tasksToWait = new IFlowTask[computationFlowTask.SaveImageTasks.Count + 3];
            tasksToWait[0] = acquisitionFlowTask.LastAcquisitionTask;
            tasksToWait[1] = computationFlowTask.LastComputationTask;
            tasksToWait[2] = computationFlowTask.SaveMaskFlowTask;
            Array.Copy(computationFlowTask.SaveImageTasks.ToArray(), 0, tasksToWait, 3,
                       computationFlowTask.SaveImageTasks.Count);
            Task.WaitAll(tasksToWait.Select(flowTask => flowTask.ToTask()).ToArray());

            // Then
            mockedXComputeRawCurvatureMapFlow.Verify(ccmFlow => ccmFlow.Execute(), Times.Once());
            mockedYComputeRawCurvatureMapFlow.Verify(ccmFlow => ccmFlow.Execute(), Times.Once());
            mockedXCurvatureMapSaveImageFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedYCurvatureMapSaveImageFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedComputeDarkImageFlow.Verify(cdiFlow => cdiFlow.Execute(), Times.Once());
            mockedDarkSaveImageFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedSaveMaskFlow.Verify(smFlow => smFlow.Execute(), Times.Once());
            mockedXComputeRawCurvatureMapFlow.Object.Input.PhaseMapAndMask.Should()
                                             .BeSameAs(mockedXComputePhaseMapFlow.Object.Result.PsdResult);
            mockedYComputeRawCurvatureMapFlow.Object.Input.PhaseMapAndMask.Should()
                                             .BeSameAs(mockedYComputePhaseMapFlow.Object.Result.PsdResult);
            mockedXCurvatureMapSaveImageFlow.Object.Input.ImageDataToSave.Should()
                                            .BeSameAs(mockedXAdjustCurvatureMapFlow.Object.Result.CurvatureMap);
            mockedYCurvatureMapSaveImageFlow.Object.Input.ImageDataToSave.Should()
                                            .BeSameAs(mockedYAdjustCurvatureMapFlow.Object.Result.CurvatureMap);
            mockedComputeDarkImageFlow.Object.Input.XResult.Should()
                                      .BeSameAs(mockedXComputePhaseMapFlow.Object.Result.PsdResult);
            mockedComputeDarkImageFlow.Object.Input.YResult.Should()
                                      .BeSameAs(mockedYComputePhaseMapFlow.Object.Result.PsdResult);
            mockedDarkSaveImageFlow.Object.Input.ImageDataToSave.Should()
                                   .BeSameAs(mockedComputeDarkImageFlow.Object.Result.DarkImage);
            mockedSaveMaskFlow.Object.Input.MaskToSave.Should().BeSameAs(mockedXComputePhaseMapFlow.Object.Result.PsdResult.Mask);
        }

        [TestMethod]
        public void GivenAMultiPeriodAcquisitionAndUnwrappedPhaseOutputRelevantFlowsShouldBeExecuted()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            var acquisitionSequence = new MockSequence();
            var computationSequence = new MockSequence();
            var saveImageSequence = new MockSequence();
            var fringe = CreateMultiPeriodFringe(new List<int>
                                                 {
                                                     32,
                                                     320,
                                                     3200
                                                 });

            var mockedX32PhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedX320PhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 320, FringesDisplacement.X, FlowState.Success);
            var mockedX3200PhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 3200, FringesDisplacement.X, FlowState.Success);
            var mockedY32PhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);
            var mockedY320PhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 320, FringesDisplacement.Y, FlowState.Success);
            var mockedY3200PhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 3200, FringesDisplacement.Y, FlowState.Success);

            var acquirePhaseImagesForPeriodAndDirectionFlows = new List<AcquirePhaseImagesForPeriodAndDirectionFlow>
                                                               {
                                                                   mockedX32PhaseAcquisitionFlow.Object,
                                                                   mockedX320PhaseAcquisitionFlow.Object,
                                                                   mockedX3200PhaseAcquisitionFlow.Object,
                                                                   mockedY32PhaseAcquisitionFlow.Object,
                                                                   mockedY320PhaseAcquisitionFlow.Object,
                                                                   mockedY3200PhaseAcquisitionFlow.Object
                                                               };

            var acquisitionFlowTask = new DMTDeflectometryAcquisitionFlowTask(acquirePhaseImagesForPeriodAndDirectionFlows);
            acquisitionFlowTask.Start(cancellationTokenSource, null, null);

            var mockedX32CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedX320CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 320, FringesDisplacement.X, FlowState.Success);
            var mockedX3200CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 3200, FringesDisplacement.X, FlowState.Success);

            var mockedXCUPMFlow =
                CreateComputeUnwrappedPhaseMapFlow(computationSequence, fringe, FringesDisplacement.X,
                                                   FlowState.Success);

            var mockedY32CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);
            var mockedY320CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 320, FringesDisplacement.Y, FlowState.Success);
            var mockedY3200CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 3200, FringesDisplacement.Y, FlowState.Success);

            var mockedYCUPMFlow =
                CreateComputeUnwrappedPhaseMapFlow(computationSequence, fringe, FringesDisplacement.Y,
                                                   FlowState.Success);

            var mockedFrontAdaWriter = new Mock<AdaWriter>(MockBehavior.Default, 0, 1, "someAdaFile.tmp");
            var mockedXUPMSIFlow =
                CreateMockedUnwrappedPhaseMapSaveImageFlow(saveImageSequence,
                                                           FringesDisplacement.X, mockedFrontAdaWriter,
                                                           FlowState.Success);
            var mockedYUPMSIFlow =
                CreateMockedUnwrappedPhaseMapSaveImageFlow(saveImageSequence,
                                                           FringesDisplacement.Y, mockedFrontAdaWriter,
                                                           FlowState.Success);
            
            var mockedSaveMaskFlow = CreateSaveMaskFlow(mockedFrontAdaWriter);
            mockedSaveMaskFlow.InSequence(computationSequence).Setup(flow => flow.Execute()).Callback(() =>
            {
                mockedSaveMaskFlow.Object.Result = new SaveMaskResult
                {
                    Status = new FlowStatus(FlowState.Success, ""),
                    MaskSide = Side.Front,
                    SavePath = @"C:\Some\Path",
                    MaskFileName = @"Mask.tif"
                };
            });
            var cpmFlows =
                new List<ComputePhaseMapAndMaskForPeriodAndDirectionFlow>
                {
                    mockedX32CPMFlow.Object,
                    mockedX320CPMFlow.Object,
                    mockedX3200CPMFlow.Object,
                    mockedY32CPMFlow.Object,
                    mockedY320CPMFlow.Object,
                    mockedY3200CPMFlow.Object
                };

            var siFlowByCUPMFlows = new Dictionary<ComputeUnwrappedPhaseMapForDirectionFlow, SaveImageFlow>
                                    {
                                        { mockedXCUPMFlow.Object, mockedXUPMSIFlow.Object },
                                        { mockedYCUPMFlow.Object, mockedYUPMSIFlow.Object }
                                    };

            var computationFlowTask =
                new DMTDeflectometryCalculationFlowTask(acquisitionFlowTask, cpmFlows)
                    .AddSaveMaskFlow(mockedSaveMaskFlow.Object)
                    .AddUnwrappedPhaseMapsFlows(siFlowByCUPMFlows);
            
            // When
            computationFlowTask.CreateAndChainComputationContinuationTasks(args => {});
            var tasksToWait = new List<IFlowTask>(4)
                              {
                                  acquisitionFlowTask.LastAcquisitionTask,
                                  computationFlowTask.LastComputationTask
                              };
            tasksToWait.AddRange(computationFlowTask.SaveImageTasks);
            Task.WaitAll(tasksToWait.Select(flowTask => flowTask.ToTask()).ToArray());

            // Then
            mockedX32CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once);
            mockedX320CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once);
            mockedX3200CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once);
            mockedY32CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once);
            mockedY320CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once);
            mockedY3200CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once);
            mockedXCUPMFlow.Verify(cupmFlow => cupmFlow.Execute(), Times.Once);
            mockedYCUPMFlow.Verify(cupmFlow => cupmFlow.Execute(), Times.Once);
            mockedXCUPMFlow.Object.Input.PhaseMaps.Should()
                           .SatisfyRespectively(
                                                kvPair =>
                                                {
                                                    kvPair.Key.Should().Be(32);
                                                    kvPair.Value.Should()
                                                          .BeSameAs(mockedX32CPMFlow
                                                                    .Object.Result
                                                                    .PsdResult);
                                                },
                                                kvPair =>
                                                {
                                                    kvPair.Key.Should().Be(320);
                                                    kvPair.Value.Should()
                                                          .BeSameAs(mockedX320CPMFlow
                                                                    .Object.Result
                                                                    .PsdResult);
                                                },
                                                kvPair =>
                                                {
                                                    kvPair.Key.Should().Be(3200);
                                                    kvPair.Value.Should()
                                                          .BeSameAs(mockedX3200CPMFlow
                                                                    .Object.Result
                                                                    .PsdResult);
                                                });
            mockedYCUPMFlow.Object.Input.PhaseMaps.Should()
                           .SatisfyRespectively(
                                                kvPair =>
                                                {
                                                    kvPair.Key.Should().Be(32);
                                                    kvPair.Value.Should()
                                                          .BeSameAs(mockedY32CPMFlow
                                                                    .Object.Result
                                                                    .PsdResult);
                                                },
                                                kvPair =>
                                                {
                                                    kvPair.Key.Should().Be(320);
                                                    kvPair.Value.Should()
                                                          .BeSameAs(mockedY320CPMFlow
                                                                    .Object.Result
                                                                    .PsdResult);
                                                },
                                                kvPair =>
                                                {
                                                    kvPair.Key.Should().Be(3200);
                                                    kvPair.Value.Should()
                                                          .BeSameAs(mockedY3200CPMFlow
                                                                    .Object.Result
                                                                    .PsdResult);
                                                });

            mockedXUPMSIFlow.Verify(siFlow => siFlow.Execute(), Times.Once);
            mockedYUPMSIFlow.Verify(siFlow => siFlow.Execute(), Times.Once);
            mockedSaveMaskFlow.Verify(smFlow => smFlow.Execute(), Times.Once);
            mockedXUPMSIFlow.Object.Input.ImageDataToSave.Should()
                            .BeSameAs(mockedXCUPMFlow.Object.Result.UnwrappedPhaseMap);
            mockedYUPMSIFlow.Object.Input.ImageDataToSave.Should()
                            .BeSameAs(mockedYCUPMFlow.Object.Result.UnwrappedPhaseMap);
            mockedSaveMaskFlow.Object.Input.MaskToSave.Should().BeSameAs(mockedX32CPMFlow.Object.Result.PsdResult.Mask);
        }

        [TestMethod]
        public void
            GivenMultiPeriodAcquisitionAndUnwrappedPhaseAndCurvatureMapOutputsAddComputationTaskShouldExecuteTheRelevantFlows()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            var acquisitionSequence = new MockSequence();
            var computationSequence = new MockSequence();
            var saveImageSequence = new MockSequence();
            var fringe = CreateMultiPeriodFringe(new List<int>
                                                 {
                                                     32,
                                                     320,
                                                     3200
                                                 });

            var mockedX32APIFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedX320APIFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 320, FringesDisplacement.X, FlowState.Success);
            var mockedX3200APIFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 3200, FringesDisplacement.X, FlowState.Success);
            var mockedY32APIFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);
            var mockedY320APIFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 320, FringesDisplacement.Y, FlowState.Success);
            var mockedY3200APIFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 3200, FringesDisplacement.Y, FlowState.Success);

            var apiFlows = new List<AcquirePhaseImagesForPeriodAndDirectionFlow>
                           {
                               mockedX32APIFlow.Object,
                               mockedX320APIFlow.Object,
                               mockedX3200APIFlow.Object,
                               mockedY32APIFlow.Object,
                               mockedY320APIFlow.Object,
                               mockedY3200APIFlow.Object
                           };
            var acquisitionFlowTask = new DMTDeflectometryAcquisitionFlowTask(apiFlows);
            acquisitionFlowTask.Start(cancellationTokenSource, null, null);

            var mockedAdaWriter = new Mock<AdaWriter>(MockBehavior.Default, 0, 1, "SomeAdaFile.tmp");

            var mockedX32CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedX32CRCMFlow =
                CreateMockedRawComputeCurvatureMapFlow(computationSequence, fringe, 32, FringesDisplacement.X,
                                                       FlowState.Success);
            var mockedX32ACMFlow =
                CreateMockedAdjustCurvatureMapFlow(computationSequence, fringe, 32, FringesDisplacement.X,
                                                   FlowState.Success);
            var mockedX32CMSIFlow = CreateMockedCurvatureMapSaveImageFlow(saveImageSequence,
                                                                          mockedAdaWriter, FringesDisplacement.X,
                                                                          FlowState.Success);
            var mockedX320CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 320, FringesDisplacement.X, FlowState.Success);
            var mockedX3200CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 3200, FringesDisplacement.X, FlowState.Success);

            var mockedXCUPMFlow =
                CreateComputeUnwrappedPhaseMapFlow(computationSequence, fringe, FringesDisplacement.X,
                                                   FlowState.Success);
            var mockedXUPMSIFlow = CreateMockedUnwrappedPhaseMapSaveImageFlow(
                                                                              saveImageSequence, FringesDisplacement.X,
                                                                              mockedAdaWriter, FlowState.Success);

            var mockedY32CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);
            var mockedY32CRCMFlow =
                CreateMockedRawComputeCurvatureMapFlow(computationSequence, fringe, 32, FringesDisplacement.Y,
                                                       FlowState.Success);
            var mockedY32ACMFlow =
                CreateMockedAdjustCurvatureMapFlow(computationSequence, fringe, 32, FringesDisplacement.Y,
                                                   FlowState.Success);
            var mockedY32CMSIFlow = CreateMockedCurvatureMapSaveImageFlow(saveImageSequence,
                                                                          mockedAdaWriter, FringesDisplacement.Y,
                                                                          FlowState.Success);
            var mockedY320CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 320, FringesDisplacement.Y, FlowState.Success);
            var mockedY3200CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 3200, FringesDisplacement.Y, FlowState.Success);

            var mockedYCUPMFlow =
                CreateComputeUnwrappedPhaseMapFlow(computationSequence, fringe, FringesDisplacement.Y,
                                                   FlowState.Success);
            var mockedYUPMSIFlow = CreateMockedUnwrappedPhaseMapSaveImageFlow(
                                                                              saveImageSequence, FringesDisplacement.X,
                                                                              mockedAdaWriter, FlowState.Success);

            var mockedSaveMaskFlow = CreateSaveMaskFlow(mockedAdaWriter);
            mockedSaveMaskFlow.InSequence(computationSequence).Setup(flow => flow.Execute()).Callback(() =>
            {
                mockedSaveMaskFlow.Object.Result = new SaveMaskResult
                {
                    Status = new FlowStatus(FlowState.Success, ""),
                    MaskSide = Side.Front,
                    SavePath = @"C:\Some\Path",
                    MaskFileName = @"Mask.tif"
                };
            });
            
            var cpmFlows = new List<ComputePhaseMapAndMaskForPeriodAndDirectionFlow>
                           {
                               mockedX32CPMFlow.Object,
                               mockedX320CPMFlow.Object,
                               mockedX3200CPMFlow.Object,
                               mockedY32CPMFlow.Object,
                               mockedY320CPMFlow.Object,
                               mockedY3200CPMFlow.Object
                           };

            var apmFlowByCrcmFlow =
                new Dictionary<ComputeRawCurvatureMapForPeriodAndDirectionFlow,
                    AdjustCurvatureDynamicsForRawCurvatureMapFlow>
                {
                    { mockedX32CRCMFlow.Object, mockedX32ACMFlow.Object },
                    { mockedY32CRCMFlow.Object, mockedY32ACMFlow.Object }
                };

            var siFlowByApmFlow = new Dictionary<AdjustCurvatureDynamicsForRawCurvatureMapFlow, SaveImageFlow>
                                  {
                                      { mockedX32ACMFlow.Object, mockedX32CMSIFlow.Object },
                                      { mockedY32ACMFlow.Object, mockedY32CMSIFlow.Object }
                                  };

            var siFlowByCupmFlow = new Dictionary<ComputeUnwrappedPhaseMapForDirectionFlow, SaveImageFlow>
                                   {
                                       { mockedXCUPMFlow.Object, mockedXUPMSIFlow.Object },
                                       { mockedYCUPMFlow.Object, mockedYUPMSIFlow.Object }
                                   };

            var computationFlowTask =
                new DMTDeflectometryCalculationFlowTask(acquisitionFlowTask, cpmFlows)
                    .AddSaveMaskFlow(mockedSaveMaskFlow.Object)
                    .AddCurvatureMapFlows(apmFlowByCrcmFlow, siFlowByApmFlow)
                    .AddUnwrappedPhaseMapsFlows(siFlowByCupmFlow);

            // When
            computationFlowTask.CreateAndChainComputationContinuationTasks(args => {});
            var tasksToWait = new IFlowTask[6];
            tasksToWait[0] = acquisitionFlowTask.LastAcquisitionTask;
            tasksToWait[1] = computationFlowTask.LastComputationTask;
            Array.ConstrainedCopy(computationFlowTask.SaveImageTasks.ToArray(), 0, tasksToWait, 2,
                                  computationFlowTask.SaveImageTasks.Count);
            Task.WaitAll(tasksToWait.Select(flowTask => flowTask.ToTask()).ToArray());

            // Then
            mockedX32CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedX320CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedX3200CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedY32CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedY320CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedY3200CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedX32CRCMFlow.Verify(crcmFlow => crcmFlow.Execute(), Times.Once());
            mockedY32CRCMFlow.Verify(crcmFlow => crcmFlow.Execute(), Times.Once());
            mockedX32ACMFlow.Verify(acmFlow => acmFlow.Execute(), Times.Once());
            mockedY32ACMFlow.Verify(acmFlow => acmFlow.Execute(), Times.Once());
            mockedX32CMSIFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedY32CMSIFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedXUPMSIFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedYUPMSIFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedSaveMaskFlow.Verify(smFlow => smFlow.Execute(), Times.Once());
        }

        [TestMethod]
        public void
            GivenMultiPeriodAcquisitionAndUnwrappedPhaseAndDarkOutputsAddComputationTaskShouldExecuteTheRelevantFlows()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            var acquisitionSequence = new MockSequence();
            var computationSequence = new MockSequence();
            var saveImageSequence = new MockSequence();
            var fringe = CreateMultiPeriodFringe(new List<int>
                                                 {
                                                     32,
                                                     320,
                                                     3200
                                                 });

            var mockedX32APIFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedX320APIFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 320, FringesDisplacement.X, FlowState.Success);
            var mockedX3200APIFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 3200, FringesDisplacement.X, FlowState.Success);
            var mockedY32APIFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);
            var mockedY320APIFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 320, FringesDisplacement.Y, FlowState.Success);
            var mockedY3200APIFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 3200, FringesDisplacement.Y, FlowState.Success);

            var apiFlows = new List<AcquirePhaseImagesForPeriodAndDirectionFlow>
                           {
                               mockedX32APIFlow.Object,
                               mockedX320APIFlow.Object,
                               mockedX3200APIFlow.Object,
                               mockedY32APIFlow.Object,
                               mockedY320APIFlow.Object,
                               mockedY3200APIFlow.Object
                           };
            var acquisitionFlowTask = new DMTDeflectometryAcquisitionFlowTask(apiFlows);
            acquisitionFlowTask.Start(cancellationTokenSource, null, null);

            var mockedAdaWriter = new Mock<AdaWriter>(MockBehavior.Default, 0, 1, "SomeAdaFile.tmp");

            var mockedX32CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedX320CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 320, FringesDisplacement.X, FlowState.Success);
            var mockedX3200CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 3200, FringesDisplacement.X, FlowState.Success);

            var mockedXCUPMFlow =
                CreateComputeUnwrappedPhaseMapFlow(computationSequence, fringe, FringesDisplacement.X,
                                                   FlowState.Success);
            var mockedXUPMSIFlow = CreateMockedUnwrappedPhaseMapSaveImageFlow(
                                                                              saveImageSequence, FringesDisplacement.X,
                                                                              mockedAdaWriter, FlowState.Success);

            var mockedY32CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);

            var mockedCDIFlow = CreateMockedComputeDarkImageFlow(computationSequence, fringe, 32, FlowState.Success);
            var mockedDarkSIFlow =
                CreateMockedDarkSaveImageFlow(saveImageSequence, mockedAdaWriter,
                                              FlowState.Success);
            var mockedY320CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 320, FringesDisplacement.Y, FlowState.Success);
            var mockedY3200CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 3200, FringesDisplacement.Y, FlowState.Success);

            var mockedYCUPMFlow =
                CreateComputeUnwrappedPhaseMapFlow(computationSequence, fringe, FringesDisplacement.Y,
                                                   FlowState.Success);
            var mockedYUPMSIFlow = CreateMockedUnwrappedPhaseMapSaveImageFlow(
                                                                              saveImageSequence, FringesDisplacement.X,
                                                                              mockedAdaWriter, FlowState.Success);

            var mockedSaveMaskFlow = CreateSaveMaskFlow(mockedAdaWriter);
            mockedSaveMaskFlow.InSequence(computationSequence).Setup(flow => flow.Execute()).Callback(() =>
            {
                mockedSaveMaskFlow.Object.Result = new SaveMaskResult
                {
                    Status = new FlowStatus(FlowState.Success, ""),
                    MaskSide = Side.Front,
                    SavePath = @"C:\Some\Path",
                    MaskFileName = @"Mask.tif"
                };
            });
            
            var cpmFlows = new List<ComputePhaseMapAndMaskForPeriodAndDirectionFlow>
                           {
                               mockedX32CPMFlow.Object,
                               mockedX320CPMFlow.Object,
                               mockedX3200CPMFlow.Object,
                               mockedY32CPMFlow.Object,
                               mockedY320CPMFlow.Object,
                               mockedY3200CPMFlow.Object
                           };

            var siFlowByCupmFlow = new Dictionary<ComputeUnwrappedPhaseMapForDirectionFlow, SaveImageFlow>
                                   {
                                       { mockedXCUPMFlow.Object, mockedXUPMSIFlow.Object },
                                       { mockedYCUPMFlow.Object, mockedYUPMSIFlow.Object }
                                   };

            var computationFlowTask = new DMTDeflectometryCalculationFlowTask(acquisitionFlowTask, cpmFlows)
                .AddSaveMaskFlow(mockedSaveMaskFlow.Object)
                .AddLowAngleDarkFieldFlows(mockedCDIFlow.Object, mockedDarkSIFlow.Object)
                .AddUnwrappedPhaseMapsFlows(siFlowByCupmFlow);

            // When
            computationFlowTask.CreateAndChainComputationContinuationTasks(args => {});
            var tasksToWait = new IFlowTask[5];
            tasksToWait[0] = acquisitionFlowTask.LastAcquisitionTask;
            tasksToWait[1] = computationFlowTask.LastComputationTask;
            Array.ConstrainedCopy(computationFlowTask.SaveImageTasks.ToArray(), 0, tasksToWait, 2,
                                  computationFlowTask.SaveImageTasks.Count);
            Task.WaitAll(tasksToWait.Select(flowTask => flowTask.ToTask()).ToArray());

            // Then
            mockedX32CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedX320CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedX3200CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedY32CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedY320CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedY3200CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedCDIFlow.Verify(cdiFlow => cdiFlow.Execute(), Times.Once());
            mockedDarkSIFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedXUPMSIFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedYUPMSIFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedSaveMaskFlow.Verify(smFlow => smFlow.Execute(), Times.Once());
        }

        [TestMethod]
        public void
            GivenMultiPeriodAcquisitionAndUnwrappedPhaseCurvatureMapAndDarkOutputsAddComputationTaskShouldExecuteTheRelevantFlows()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            var acquisitionSequence = new MockSequence();
            var computationSequence = new MockSequence();
            var saveImageSequence = new MockSequence();
            var fringe = CreateMultiPeriodFringe(new List<int>
                                                 {
                                                     32,
                                                     320,
                                                     3200
                                                 });

            var mockedX32APIFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedX320APIFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 320, FringesDisplacement.X, FlowState.Success);
            var mockedX3200APIFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 3200, FringesDisplacement.X, FlowState.Success);
            var mockedY32APIFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);
            var mockedY320APIFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 320, FringesDisplacement.Y, FlowState.Success);
            var mockedY3200APIFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 3200, FringesDisplacement.Y, FlowState.Success);

            var apiFlows = new List<AcquirePhaseImagesForPeriodAndDirectionFlow>
                           {
                               mockedX32APIFlow.Object,
                               mockedX320APIFlow.Object,
                               mockedX3200APIFlow.Object,
                               mockedY32APIFlow.Object,
                               mockedY320APIFlow.Object,
                               mockedY3200APIFlow.Object
                           };
            var acquisitionFlowTask = new DMTDeflectometryAcquisitionFlowTask(apiFlows);
            acquisitionFlowTask.Start(cancellationTokenSource, null, null);

            var mockedAdaWriter = new Mock<AdaWriter>(MockBehavior.Default, 0, 1, "SomeAdaFile.tmp");

            var mockedX32CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedX32CRCMFlow =
                CreateMockedRawComputeCurvatureMapFlow(computationSequence, fringe, 32, FringesDisplacement.X,
                                                       FlowState.Success);
            var mockedX32ACMFlow =
                CreateMockedAdjustCurvatureMapFlow(computationSequence, fringe, 32, FringesDisplacement.X,
                                                   FlowState.Success);
            var mockedX32CMSIFlow = CreateMockedCurvatureMapSaveImageFlow(saveImageSequence,
                                                                          mockedAdaWriter, FringesDisplacement.X,
                                                                          FlowState.Success);
            var mockedX320CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 320, FringesDisplacement.X, FlowState.Success);
            var mockedX3200CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 3200, FringesDisplacement.X, FlowState.Success);

            var mockedXCUPMFlow =
                CreateComputeUnwrappedPhaseMapFlow(computationSequence, fringe, FringesDisplacement.X,
                                                   FlowState.Success);
            var mockedXUPMSIFlow = CreateMockedUnwrappedPhaseMapSaveImageFlow(
                                                                              saveImageSequence, FringesDisplacement.X,
                                                                              mockedAdaWriter, FlowState.Success);

            var mockedY32CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);
            var mockedY32CRCMFlow =
                CreateMockedRawComputeCurvatureMapFlow(computationSequence, fringe, 32, FringesDisplacement.Y,
                                                       FlowState.Success);
            var mockedY32ACMFlow =
                CreateMockedAdjustCurvatureMapFlow(computationSequence, fringe, 32, FringesDisplacement.Y,
                                                   FlowState.Success);
            var mockedY32CMSIFlow = CreateMockedCurvatureMapSaveImageFlow(saveImageSequence,
                                                                          mockedAdaWriter, FringesDisplacement.Y,
                                                                          FlowState.Success);
            var mockedCDIFlow = CreateMockedComputeDarkImageFlow(computationSequence, fringe, 32, FlowState.Success);
            var mockedDarkSIFlow =
                CreateMockedDarkSaveImageFlow(saveImageSequence, mockedAdaWriter,
                                              FlowState.Success);
            var mockedY320CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 320, FringesDisplacement.Y, FlowState.Success);
            var mockedY3200CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 3200, FringesDisplacement.Y, FlowState.Success);

            var mockedYCUPMFlow =
                CreateComputeUnwrappedPhaseMapFlow(computationSequence, fringe, FringesDisplacement.Y,
                                                   FlowState.Success);
            var mockedYUPMSIFlow = CreateMockedUnwrappedPhaseMapSaveImageFlow(
                                                                              saveImageSequence, FringesDisplacement.X,
                                                                              mockedAdaWriter, FlowState.Success);

            var mockedSaveMaskFlow = CreateSaveMaskFlow(mockedAdaWriter);
            mockedSaveMaskFlow.InSequence(computationSequence).Setup(flow => flow.Execute()).Callback(() =>
            {
                mockedSaveMaskFlow.Object.Result = new SaveMaskResult
                {
                    Status = new FlowStatus(FlowState.Success, ""),
                    MaskSide = Side.Front,
                    SavePath = @"C:\Some\Path",
                    MaskFileName = @"Mask.tif"
                };
            });
            
            var cpmFlows = new List<ComputePhaseMapAndMaskForPeriodAndDirectionFlow>
                           {
                               mockedX32CPMFlow.Object,
                               mockedX320CPMFlow.Object,
                               mockedX3200CPMFlow.Object,
                               mockedY32CPMFlow.Object,
                               mockedY320CPMFlow.Object,
                               mockedY3200CPMFlow.Object
                           };

            var apmFlowByCrcmFlow =
                new Dictionary<ComputeRawCurvatureMapForPeriodAndDirectionFlow,
                    AdjustCurvatureDynamicsForRawCurvatureMapFlow>
                {
                    { mockedX32CRCMFlow.Object, mockedX32ACMFlow.Object },
                    { mockedY32CRCMFlow.Object, mockedY32ACMFlow.Object }
                };

            var siFlowByApmFlow = new Dictionary<AdjustCurvatureDynamicsForRawCurvatureMapFlow, SaveImageFlow>
                                  {
                                      { mockedX32ACMFlow.Object, mockedX32CMSIFlow.Object },
                                      { mockedY32ACMFlow.Object, mockedY32CMSIFlow.Object }
                                  };

            var siFlowByCupmFlow = new Dictionary<ComputeUnwrappedPhaseMapForDirectionFlow, SaveImageFlow>
                                   {
                                       { mockedXCUPMFlow.Object, mockedXUPMSIFlow.Object },
                                       { mockedYCUPMFlow.Object, mockedYUPMSIFlow.Object }
                                   };

            var computationFlowTask = new DMTDeflectometryCalculationFlowTask(acquisitionFlowTask, cpmFlows)
                .AddSaveMaskFlow(mockedSaveMaskFlow.Object)
                .AddCurvatureMapFlows(apmFlowByCrcmFlow, siFlowByApmFlow)
                .AddLowAngleDarkFieldFlows(mockedCDIFlow.Object, mockedDarkSIFlow.Object)
                .AddUnwrappedPhaseMapsFlows(siFlowByCupmFlow);

            // When
            computationFlowTask.CreateAndChainComputationContinuationTasks(args => {});
            var tasksToWait = new IFlowTask[7];
            tasksToWait[0] = acquisitionFlowTask.LastAcquisitionTask;
            tasksToWait[1] = computationFlowTask.LastComputationTask;
            Array.ConstrainedCopy(computationFlowTask.SaveImageTasks.ToArray(), 0, tasksToWait, 2,
                                  computationFlowTask.SaveImageTasks.Count);
            Task.WaitAll(tasksToWait.Select(flowTask => flowTask.ToTask()).ToArray());

            // Then
            mockedX32CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedX320CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedX3200CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedY32CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedY320CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedY3200CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedX32CRCMFlow.Verify(crcmFlow => crcmFlow.Execute(), Times.Once());
            mockedY32CRCMFlow.Verify(crcmFlow => crcmFlow.Execute(), Times.Once());
            mockedX32ACMFlow.Verify(acmFlow => acmFlow.Execute(), Times.Once());
            mockedY32ACMFlow.Verify(acmFlow => acmFlow.Execute(), Times.Once());
            mockedX32CMSIFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedY32CMSIFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedCDIFlow.Verify(cdiFlow => cdiFlow.Execute(), Times.Once());
            mockedDarkSIFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedXUPMSIFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedYUPMSIFlow.Verify(siFlow => siFlow.Execute(), Times.Once());
            mockedSaveMaskFlow.Verify(smFlow => smFlow.Execute(), Times.Once());
        }

        [TestMethod]
        public void GivenAMultiPeriodAcquisitionAndNanoTopoOutputRelevantFlowsShouldBeExecuted()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            var acquisitionSequence = new MockSequence();
            var computationSequence = new MockSequence();
            var fringe = CreateMultiPeriodFringe(new List<int>
            {
                32,
                320,
                3200
            });

            var mockedX32PhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedX320PhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 320, FringesDisplacement.X, FlowState.Success);
            var mockedX3200PhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 3200, FringesDisplacement.X, FlowState.Success);
            var mockedY32PhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);
            var mockedY320PhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 320, FringesDisplacement.Y, FlowState.Success);
            var mockedY3200PhaseAcquisitionFlow =
                CreatePhaseAcquisitionFlow(acquisitionSequence, fringe, 3200, FringesDisplacement.Y, FlowState.Success);

            var acquirePhaseImagesForPeriodAndDirectionFlows = new List<AcquirePhaseImagesForPeriodAndDirectionFlow>
            {
                mockedX32PhaseAcquisitionFlow.Object,
                mockedX320PhaseAcquisitionFlow.Object,
                mockedX3200PhaseAcquisitionFlow.Object,
                mockedY32PhaseAcquisitionFlow.Object,
                mockedY320PhaseAcquisitionFlow.Object,
                mockedY3200PhaseAcquisitionFlow.Object
            };

            var acquisitionFlowTask = new DMTDeflectometryAcquisitionFlowTask(acquirePhaseImagesForPeriodAndDirectionFlows);
            acquisitionFlowTask.Start(cancellationTokenSource, null, null);

            var mockedX32CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedX320CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 320, FringesDisplacement.X, FlowState.Success);
            var mockedX3200CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 3200, FringesDisplacement.X, FlowState.Success);

            var mockedY32CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);
            var mockedY320CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 320, FringesDisplacement.Y, FlowState.Success);
            var mockedY3200CPMFlow =
                CreateComputePhaseMapFlow(computationSequence, fringe, 3200, FringesDisplacement.Y, FlowState.Success);

            var mockedXCUPMFlow =
                CreateComputeUnwrappedPhaseMapFlowForNanoTopo(computationSequence, fringe, FringesDisplacement.X, FlowState.Success);
            var mockedYCUPMFlow =
                CreateComputeUnwrappedPhaseMapFlowForNanoTopo(computationSequence, fringe, FringesDisplacement.Y, FlowState.Success);

            var input = new ComputeNanoTopoInput()
            {
                Periods = fringe.Periods,
                Side = Side.Front,
            };

            var mockedComputeNanoTopoFlow = new Mock<ComputeNanoTopoFlow>(input, _calibrationManagerMock.Object);
            mockedComputeNanoTopoFlow.InSequence(computationSequence).Setup(cntFlow => cntFlow.Execute()).Callback(() =>
            {
                mockedComputeNanoTopoFlow.Object.Result = new ComputeNanoTopoResult
                {
                    Status = new FlowStatus(FlowState.Success, ""), NanoTopoImage = new ImageData()
                };
            });

            var mockedAdaWriter = new Mock<AdaWriter>(MockBehavior.Default, 0, 1, "SomeAdaFile.tmp");
            var mockedSaveMaskFlow = CreateSaveMaskFlow(mockedAdaWriter);
            mockedSaveMaskFlow.InSequence(computationSequence).Setup(flow => flow.Execute()).Callback(() =>
            {
                mockedSaveMaskFlow.Object.Result = new SaveMaskResult
                {
                    Status = new FlowStatus(FlowState.Success, ""),
                    MaskSide = Side.Front,
                    SavePath = @"C:\Some\Path",
                    MaskFileName = @"Mask.tif"
                };
            });
            
            var cpmFlows =
                new List<ComputePhaseMapAndMaskForPeriodAndDirectionFlow>
                {
                    mockedX32CPMFlow.Object,
                    mockedX320CPMFlow.Object,
                    mockedX3200CPMFlow.Object,
                    mockedY32CPMFlow.Object,
                    mockedY320CPMFlow.Object,
                    mockedY3200CPMFlow.Object
                };

            var cupmFlowsForNanoTopo = new List<ComputeUnwrappedPhaseMapForDirectionFlow>
            {
                { mockedXCUPMFlow.Object },
                { mockedYCUPMFlow.Object }
            };

            var computationFlowTask =
                new DMTDeflectometryCalculationFlowTask(acquisitionFlowTask, cpmFlows)
                    .AddSaveMaskFlow(mockedSaveMaskFlow.Object)
                    .AddNanoTopoFlows(cupmFlowsForNanoTopo, mockedComputeNanoTopoFlow.Object, null);

            // When
            computationFlowTask.CreateAndChainComputationContinuationTasks(args => {});
            var tasksToWait = new List<IFlowTask>(5)
            {
                acquisitionFlowTask.LastAcquisitionTask,
                computationFlowTask.LastComputationTask
            };
            tasksToWait.AddRange(computationFlowTask.SaveImageTasks);
            Task.WaitAll(tasksToWait.Select(flowTask => flowTask.ToTask()).ToArray());

            // Then
            mockedX32CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once);
            mockedX320CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once);
            mockedX3200CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once);
            mockedY32CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once);
            mockedY320CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once);
            mockedY3200CPMFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once);

            mockedXCUPMFlow.Verify(cupmFlow => cupmFlow.Execute(), Times.Once);
            mockedYCUPMFlow.Verify(cupmFlow => cupmFlow.Execute(), Times.Once);
            mockedXCUPMFlow.Object.Input.PhaseMaps.Should()
                .SatisfyRespectively(
                    kvPair =>
                    {
                        kvPair.Key.Should().Be(32);
                        kvPair.Value.Should()
                            .BeSameAs(mockedX32CPMFlow
                                .Object.Result
                                .PsdResult);
                    },
                    kvPair =>
                    {
                        kvPair.Key.Should().Be(320);
                        kvPair.Value.Should()
                            .BeSameAs(mockedX320CPMFlow
                                .Object.Result
                                .PsdResult);
                    },
                    kvPair =>
                    {
                        kvPair.Key.Should().Be(3200);
                        kvPair.Value.Should()
                            .BeSameAs(mockedX3200CPMFlow
                                .Object.Result
                                .PsdResult);
                    });
            mockedYCUPMFlow.Object.Input.PhaseMaps.Should()
                .SatisfyRespectively(
                    kvPair =>
                    {
                        kvPair.Key.Should().Be(32);
                        kvPair.Value.Should()
                            .BeSameAs(mockedY32CPMFlow
                                .Object.Result
                                .PsdResult);
                    },
                    kvPair =>
                    {
                        kvPair.Key.Should().Be(320);
                        kvPair.Value.Should()
                            .BeSameAs(mockedY320CPMFlow
                                .Object.Result
                                .PsdResult);
                    },
                    kvPair =>
                    {
                        kvPair.Key.Should().Be(3200);
                        kvPair.Value.Should()
                            .BeSameAs(mockedY3200CPMFlow
                                .Object.Result
                                .PsdResult);
                    });
            mockedComputeNanoTopoFlow.Verify(cntFlow => cntFlow.Execute(), Times.Once);
            mockedSaveMaskFlow.Verify(smFlow => smFlow.Execute(), Times.Once);
            mockedComputeNanoTopoFlow.Object.Input.UnwrappedX.Should().BeSameAs(mockedXCUPMFlow.Object.Result.UnwrappedPhaseMap);
            mockedComputeNanoTopoFlow.Object.Input.UnwrappedY.Should().BeSameAs(mockedYCUPMFlow.Object.Result.UnwrappedPhaseMap);
            
        }
        
        private Mock<SaveMaskFlow> CreateSaveMaskFlow(Mock<AdaWriter> adaWriterMock)
        {
            var saveMaskInput = new SaveMaskInput
            {
                MaskSide = Side.Front,
                SaveFullPath = @"C:\Some\Path\Mask.tif",
                AdaWriterLock = new object(),
                AdaWriterForSide = adaWriterMock.Object
            };
            return new Mock<SaveMaskFlow>(saveMaskInput, _calibrationManagerMock.Object);
        }

        private static Fringe CreateMultiPeriodFringe(List<int> periods)
        {
            return new Fringe
            {
                FringeType = FringeType.Multi,
                Period = periods.Min(),
                Periods = periods,
                NbImagesPerDirection = 8
            };
        }

        private static Fringe CreateDefaultFringe()
        {
            return new Fringe
            {
                FringeType = FringeType.Standard,
                Period = 32,
                Periods = new List<int> { 32 },
                NbImagesPerDirection = 8
            };
        }

        private Mock<AcquirePhaseImagesForPeriodAndDirectionFlow> CreatePhaseAcquisitionFlow(
            MockSequence sequence,
            Fringe fringe, int period, FringesDisplacement direction, FlowState resultState)
        {
            var phaseAcquisitionInput =
                new AcquirePhaseImagesForPeriodAndDirectionInput(Side.Front, fringe, period, direction, 0.150);
            var mockedPhaseAcquisitionFlow = new Mock<AcquirePhaseImagesForPeriodAndDirectionFlow>(
             phaseAcquisitionInput, HardwareManager, _cameraMethodsMock.Object, _fringeManagerMock.Object);
            mockedPhaseAcquisitionFlow.InSequence(sequence)
                                      .Setup(paFlow => paFlow.Execute())
                                      .Callback(() =>
                                      {
                                          mockedPhaseAcquisitionFlow.Object.Result =
                                              new AcquirePhaseImagesForPeriodAndDirectionResult
                                              {
                                                  ExposureTimeMs = 150,
                                                  Fringe = fringe,
                                                  FringesDisplacementDirection = direction,
                                                  Period = period,
                                                  Status = new FlowStatus { State = resultState },
                                                  TemporaryResults = Enumerable.Repeat(new ServiceImage(),
                                                                                fringe.NbImagesPerDirection)
                                                                               .ToList()
                                              };
                                      });
            return mockedPhaseAcquisitionFlow;
        }

        private static Mock<ComputePhaseMapAndMaskForPeriodAndDirectionFlow> CreateComputePhaseMapFlow(
            MockSequence sequence, Fringe fringe, int period, FringesDisplacement direction, FlowState resultState)
        {
            var computePhaseMapInput = new ComputePhaseMapAndMaskForPeriodAndDirectionInput(fringe, period, direction, Side.Front);
            var mockedComputePhaseMapFlow =
                new Mock<ComputePhaseMapAndMaskForPeriodAndDirectionFlow>(computePhaseMapInput);
            mockedComputePhaseMapFlow.InSequence(sequence)
                                     .Setup(cpmFlow => cpmFlow.Execute())
                                     .Callback(() =>
                                     {
                                         mockedComputePhaseMapFlow.Object.Result =
                                             new ComputePhaseMapAndMaskForPeriodAndDirectionResult
                                             {
                                                 Fringe = fringe,
                                                 FringesDisplacementDirection = direction,
                                                 Period = period,
                                                 PsdResult = new PSDResult(),
                                                 Status = new FlowStatus { State = resultState }
                                             };
                                     });
            return mockedComputePhaseMapFlow;
        }

        private static Mock<ComputeUnwrappedPhaseMapForDirectionFlow> CreateComputeUnwrappedPhaseMapFlowForNanoTopo(
            MockSequence sequence, Fringe fringe, FringesDisplacement direction, FlowState resultState)
        {
            var cupmFlow = CreateComputeUnwrappedPhaseMapFlow(sequence, fringe, direction, resultState);
            cupmFlow.Object.Input.IsNeededForTopography = true;
            cupmFlow.Object.Input.IsNeededForSlopeMaps = false;
            return cupmFlow;
        }

        private static Mock<ComputeUnwrappedPhaseMapForDirectionFlow> CreateComputeUnwrappedPhaseMapFlow(
            MockSequence sequence, Fringe fringe, FringesDisplacement direction, FlowState resultState)
        {
            var computeUnwrappedPhaseMapInput = new ComputeUnwrappedPhaseMapForDirectionInput
            {
                Fringe = fringe,
                FringesDisplacementDirection = direction,
                PhaseMaps = new Dictionary<int, PSDResult>(3),
                IsNeededForSlopeMaps = true,
                IsNeededForTopography = false
            };
            var mockedComputeUnwrappedPhaseMapFlow =
                new Mock<ComputeUnwrappedPhaseMapForDirectionFlow>(computeUnwrappedPhaseMapInput);
            mockedComputeUnwrappedPhaseMapFlow.InSequence(sequence)
                                              .Setup(cupmFlow => cupmFlow.Execute())
                                              .Callback(() =>
                                              {
                                                  mockedComputeUnwrappedPhaseMapFlow.Object.Result =
                                                      new ComputeUnwrappedPhaseMapForDirectionResult
                                                      {
                                                          Fringe = fringe,
                                                          FringesDisplacementDirection = direction,
                                                          UnwrappedPhaseMap = new ImageData(),
                                                          Status = new FlowStatus { State = resultState }
                                                      };
                                              });
            return mockedComputeUnwrappedPhaseMapFlow;
        }

        private static Mock<ComputeRawCurvatureMapForPeriodAndDirectionFlow> CreateMockedRawComputeCurvatureMapFlow(
            MockSequence sequence, Fringe fringe, int period, FringesDisplacement direction, FlowState resultState)
        {
            var computeCurvatureMapInput =
                new ComputeRawCurvatureMapForPeriodAndDirectionInput(fringe, 32, direction, Side.Front);
            var mockedComputeCurvatureMapFlow =
                new Mock<ComputeRawCurvatureMapForPeriodAndDirectionFlow>(computeCurvatureMapInput);
            mockedComputeCurvatureMapFlow.InSequence(sequence)
                                         .Setup(ccmFlow => ccmFlow.Execute())
                                         .Callback(() =>
                                         {
                                             mockedComputeCurvatureMapFlow.Object.Result =
                                                 new ComputeRawCurvatureMapForPeriodAndDirectionResult
                                                 {
                                                     RawCurvatureMap = new ImageData(),
                                                     Fringe = fringe,
                                                     FringesDisplacementDirection = direction,
                                                     Period = period,
                                                     Status = new FlowStatus { State = resultState }
                                                 };
                                         });
            return mockedComputeCurvatureMapFlow;
        }

        private Mock<SaveImageFlow> CreateMockedCurvatureMapSaveImageFlow(
            MockSequence sequence, Mock<AdaWriter> mockedFrontAdaWriter,
            FringesDisplacement direction, FlowState resultState)
        {
            var dmtresultType = direction == FringesDisplacement.X
                ? DMTResult.CurvatureX_Front
                : DMTResult.CurvatureY_Front;
            string imageName = direction == FringesDisplacement.X ? "CX" : "CY";
            var rcpinfo = new RecipeInfo() { ActorType = ActorType.DEMETER, Name = "test", Key = new Guid(), Version = 0 };
            var curvatureMapSaveImageInput = new SaveImageInput(rcpinfo, new RemoteProductionInfo(), mockedFrontAdaWriter.Object,
                                                                new object(), dmtresultType, imageName,
                                                                $"C:\\Some\\Save\\Path\\{imageName}.tif");
            var mockedCurvatureMapSaveImageFlow = new Mock<SaveImageFlow>(curvatureMapSaveImageInput,
                                                                          _algorithmManagerMock.Object,
                                                                          _calibrationManagerMock.Object,
                                                                          _dbResultServiceMock.Object);
            mockedCurvatureMapSaveImageFlow.InSequence(sequence)
                                           .Setup(siFlow => siFlow.Execute())
                                           .Callback(() =>
                                           {
                                               mockedCurvatureMapSaveImageFlow.Object.Result = new SaveImageResult
                                               {
                                                   ImageName = imageName,
                                                   ImageSide = Side.Front,
                                                   Status = new FlowStatus { State = resultState }
                                               };
                                           });
            return mockedCurvatureMapSaveImageFlow;
        }

        private static Mock<ComputeLowAngleDarkFieldImageFlow> CreateMockedComputeDarkImageFlow(
            MockSequence sequence, Fringe fringe,
            int period, FlowState resultState)
        {
            var darkFieldImageInput = new ComputeLowAngleDarkFieldImageInput(null, null, fringe, period, Side.Front);
            var mockedComputeDarkImageFlow = new Mock<ComputeLowAngleDarkFieldImageFlow>(darkFieldImageInput);
            mockedComputeDarkImageFlow.InSequence(sequence)
                                      .Setup(cdiFlow => cdiFlow.Execute())
                                      .Callback(() =>
                                      {
                                          mockedComputeDarkImageFlow.Object.Result = new ComputeLowAngleDarkFieldImageResult
                                          {
                                              DarkImage = new ImageData(),
                                              Fringe = fringe,
                                              Period = period,
                                              Status = new FlowStatus { State = resultState }
                                          };
                                      });
            return mockedComputeDarkImageFlow;
        }

        private Mock<SaveImageFlow> CreateMockedDarkSaveImageFlow(
            MockSequence sequence, Mock<AdaWriter> mockedFrontAdaWriter, FlowState resultState)
        {
            var rcpinfo = new RecipeInfo() { ActorType = ActorType.DEMETER, Name = "test", Key = new Guid(), Version = 0 };
            var darkSaveImageInput = new SaveImageInput(rcpinfo, new RemoteProductionInfo(), mockedFrontAdaWriter.Object,
                                                        new object(), DMTResult.LowAngleDarkField_Front, "dark.tiff",
                                                         $"C:\\Some\\Save\\Path\\dark.tiff");
            var mockedDarkSaveImageFlow = new Mock<SaveImageFlow>(darkSaveImageInput,
                                                                  _algorithmManagerMock.Object,
                                                                  _calibrationManagerMock.Object,
                                                                  _dbResultServiceMock.Object);
            mockedDarkSaveImageFlow.InSequence(sequence)
                                   .Setup(siFlow => siFlow.Execute())
                                   .Callback(() =>
                                   {
                                       mockedDarkSaveImageFlow.Object.Result = new SaveImageResult
                                       {
                                           ImageName = "dark.tiff",
                                           ImageSide = Side.Front,
                                           Status = new FlowStatus
                                           {
                                               State = resultState
                                           }
                                       };
                                   });
            return mockedDarkSaveImageFlow;
        }

        private Mock<SaveImageFlow> CreateMockedUnwrappedPhaseMapSaveImageFlow(
            MockSequence sequence, FringesDisplacement direction, Mock<AdaWriter> mockedFrontAdaWriter,
            FlowState resultState)
        {
            var rcpinfo = new RecipeInfo() { ActorType = ActorType.DEMETER, Name = "test", Key = new Guid(), Version = 0 };
            var unwrappedPhaseMapSaveImageInput = new SaveImageInput(rcpinfo, new RemoteProductionInfo(), mockedFrontAdaWriter.Object,
                                                                     new object(), DMTResult.LowAngleDarkField_Front,
                                                                     $"Slope{Enum.GetName(typeof(FringesDisplacement), direction)}.tiff",
                                                                     "C:\\Some\\Save\\Path\\" + $"Slope{Enum.GetName(typeof(FringesDisplacement), direction)}.tiff");
            var mockedUnwrappedPhaseMapSaveImageFlow = new Mock<SaveImageFlow>(unwrappedPhaseMapSaveImageInput,
                                                                               _algorithmManagerMock.Object,
                                                                               _calibrationManagerMock.Object,
                                                                               _dbResultServiceMock.Object);
            mockedUnwrappedPhaseMapSaveImageFlow.InSequence(sequence)
                                                .Setup(siFlow => siFlow.Execute())
                                                .Callback(() =>
                                                {
                                                    mockedUnwrappedPhaseMapSaveImageFlow.Object.Result =
                                                        new SaveImageResult
                                                        {
                                                            ImageName = $"Slope{Enum.GetName(typeof(FringesDisplacement), direction)}.tiff",
                                                            ImageSide = Side.Front,
                                                            Status = new FlowStatus { State = resultState }
                                                        };
                                                });
            return mockedUnwrappedPhaseMapSaveImageFlow;
        }

        private Mock<AdjustCurvatureDynamicsForRawCurvatureMapFlow> CreateMockedAdjustCurvatureMapFlow(
            MockSequence sequence, Fringe fringe, int period, FringesDisplacement direction, FlowState resultState)
        {
            var adjustCurvatureMapInput = new AdjustCurvatureDynamicsForRawCurvatureMapInput
            {
                Fringe = fringe,
                Period = period,
                FringesDisplacementDirection = direction
            };
            var mockAdjustCurvatureDynamicsFlow =
                new Mock<AdjustCurvatureDynamicsForRawCurvatureMapFlow>(adjustCurvatureMapInput);
            mockAdjustCurvatureDynamicsFlow.InSequence(sequence)
                                           .Setup(acmFlow => acmFlow.Execute())
                                           .Callback(() =>
                                           {
                                               mockAdjustCurvatureDynamicsFlow.Object.Result =
                                                   new AdjustCurvatureDynamicsForRawCurvatureMapResult
                                                   {
                                                       CurvatureMap = new ImageData(),
                                                       Fringe = fringe,
                                                       FringesDisplacementDirection = direction,
                                                       Period = period,
                                                       Status = new FlowStatus { State = resultState }
                                                   };
                                           });
            return mockAdjustCurvatureDynamicsFlow;
        }
    }
}
