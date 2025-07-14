using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Media;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Flows.AutoExposure;
using UnitySC.PM.DMT.Service.Flows.Calibration;
using UnitySC.PM.DMT.Service.Flows.Deflectometry;
using UnitySC.PM.DMT.Service.Flows.FlowTask;
using UnitySC.PM.DMT.Service.Interface.AlgorithmManager;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.DMT.Service.Interface.Fringe;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Shared.TestUtils;
using UnitySC.PM.DMT.Shared;
using UnitySC.PM.Shared;
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
    public class DMTCurvatureCalibrationCalculationFlowTaskTest : TestWithMockedCameraAndScreen<DMTCurvatureCalibrationCalculationFlowTaskTest>
    {
        private readonly Mock<IDMTAlgorithmManager> _algorithmManagerMock = new Mock<IDMTAlgorithmManager>();

        private readonly Mock<IDMTInternalCameraMethods> _cameraMethodsMock = new Mock<IDMTInternalCameraMethods>();

        private readonly Mock<IFringeManager> _fringeManagerMock = new Mock<IFringeManager>();

        private readonly Mock<PMConfiguration> _pmConfigurationMock = new Mock<PMConfiguration>();

        [TestInitialize]
        public void MockSetup()
        {
            _cameraMethodsMock.Reset();
            _fringeManagerMock.Reset();
            _algorithmManagerMock.Reset();
            _pmConfigurationMock.Reset();
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
        public void GivenAnAppropriateAcquisitionAddComputationFlowShouldExecuteTheRelevantFlows()
        {
            // Given
            var cancellationTokenSource = new CancellationTokenSource();
            var xSequence = new MockSequence();
            var ySequence = new MockSequence();
            var fringe = CreateDefaultCalibrationFringe();


            var mockedAEFlow = CreateMockAutoExposureFlow(xSequence, fringe, Side.Front);

            var mockedAPMXFlow = CreateMockPhaseAcquisitionFlow(xSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedAPMYFlow = CreateMockPhaseAcquisitionFlow(ySequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);
            var apmFlowList = new List<AcquirePhaseImagesForPeriodAndDirectionFlow>
                              {
                                  mockedAPMXFlow.Object,
                                  mockedAPMYFlow.Object
                              };
            var acquisitionFlowTask = new DMTDeflectometryAcquisitionFlowTask(mockedAEFlow.Object, apmFlowList);
            acquisitionFlowTask.Start(cancellationTokenSource, null, null);

            var mockedCPMXFlow =
                CreateMockedComputePhaseMapFlow(xSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedCPMYFlow =
                CreateMockedComputePhaseMapFlow(ySequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);
            var mockedCRCMXFlow =
                CreateMockedRawComputeCurvatureMapFlow(xSequence, fringe, 32, FringesDisplacement.X, FlowState.Success);
            var mockedCRCMYFlow =
                CreateMockedRawComputeCurvatureMapFlow(ySequence, fringe, 32, FringesDisplacement.Y, FlowState.Success);
            var ccdcInput = new CurvatureDynamicsCalibrationInput();
            var mockedCDCCFlow = new Mock<CurvatureDynamicsCalibrationFlow>(ccdcInput);
            mockedCDCCFlow.Setup(cdccFlow => cdccFlow.Execute())
                          .Callback(() =>
                          {
                              mockedCDCCFlow.Object.Result = new CurvatureDynamicsCalibrationResult
                              {
                                  CurvatureDynamicsCoefficient = 0.003f,
                                  Status = new FlowStatus
                                  {
                                      State = FlowState.Success,
                                  }
                              };
                          });
            var cpmFlows = new List<ComputePhaseMapAndMaskForPeriodAndDirectionFlow>
                           {
                               mockedCPMXFlow.Object,
                               mockedCPMYFlow.Object,
                           };
            var crcmFlows = new List<ComputeRawCurvatureMapForPeriodAndDirectionFlow>
                           {
                               mockedCRCMXFlow.Object,
                               mockedCRCMYFlow.Object
                           };
            var calculationFlowTask =
                new DMTCurvatureCalibrationCalculationFlowTask(acquisitionFlowTask, cpmFlows, crcmFlows,
                                                               mockedCDCCFlow.Object);

            // When
            calculationFlowTask.CreateAndChainComputationContinuationTasks();
            calculationFlowTask.LastComputationTask.Wait();

            // Then
            mockedCPMXFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedCPMYFlow.Verify(cpmFlow => cpmFlow.Execute(), Times.Once());
            mockedCRCMXFlow.Verify(crcmFlow => crcmFlow.Execute(), Times.Once());
            mockedCRCMYFlow.Verify(crcmFlow => crcmFlow.Execute(), Times.Once());
            mockedCDCCFlow.Verify(cdccFlow => cdccFlow.Execute(), Times.Once());
            mockedCDCCFlow.Object.Input.XRawCurvatureMap.Should()
                          .BeSameAs(mockedCRCMXFlow.Object.Result.RawCurvatureMap);
            mockedCDCCFlow.Object.Input.YRawCurvatureMap.Should()
                          .BeSameAs(mockedCRCMYFlow.Object.Result.RawCurvatureMap);
            mockedCDCCFlow.Object.Input.CurvatureMapMask.Should()
                          .BeSameAs(mockedCRCMXFlow.Object.Result.Mask);
        }

        private Fringe CreateDefaultCalibrationFringe()
        {
            return new Fringe
            {
                Period = 32,
                FringeType = FringeType.Standard,
                Periods = new List<int> { 32 },
            };
        }

        private Mock<AutoExposureFlow> CreateMockAutoExposureFlow(MockSequence sequence, Fringe fringe, Side side, double expectedExposureTimeMs = 145)
        {
            var roi = CreateWholeWaferROI();
            var aeInput = new AutoExposureInput(side, MeasureType.DeflectometryMeasure, roi, AcquisitionScreenDisplayImage.FringeImage, Colors.White, fringe, new USPImageMil(), 220);
            var mockedAEFlow = new Mock<AutoExposureFlow>(aeInput, HardwareManager, ClassLocator.Default.GetInstance<IDMTInternalCameraMethods>());
            mockedAEFlow.InSequence(sequence).Setup(aeFlow => aeFlow.Execute()).Callback(() => mockedAEFlow.Object.Result = new AutoExposureResult
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

        private static ROI CreateWholeWaferROI()
        {
            return new ROI
            {
                RoiType = RoiType.WholeWafer,
                WaferRadius = 300.Millimeters().Micrometers,
                EdgeExclusion = 300.Millimeters().Micrometers,
            };
        }

        private Mock<AcquirePhaseImagesForPeriodAndDirectionFlow> CreateMockPhaseAcquisitionFlow(
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

        private static Mock<ComputePhaseMapAndMaskForPeriodAndDirectionFlow> CreateMockedComputePhaseMapFlow(
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
    }
}
