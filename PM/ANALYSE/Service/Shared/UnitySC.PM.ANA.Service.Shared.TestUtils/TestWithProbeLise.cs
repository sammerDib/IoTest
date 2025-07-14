using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Moq;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.MeasureCalibration;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Shared.TestUtils
{
    // The probe lise needs objective calibration, hence the parent interface
    public interface ITestWithProbeLise : ITestWithObjective
    {
        List<string> SimpleProbesLise { get; set; }

        string LiseUpId { get; set; }
        string LiseBottomId { get; set; }
        string DualLiseId { get; set; }
        double DefaultGain { get; set; } // /!\ Must be between 0 and 5.5

        Mock<ProbeLise> FakeLiseUp { get; set; }
        Mock<ProbeLise> FakeLiseBottom { get; set; }
        Mock<IProbeDualLise> FakeDualLise { get; set; }
    }

    public static class TestWithProbeLiseHelper
    {
        public static void Setup(ITestWithProbeLise test)
        {
            test.LiseUpId = "ProbeLiseUp";
            test.LiseBottomId = "ProbeLiseBottom";
            test.SimpleProbesLise = new List<string> { test.LiseUpId, test.LiseBottomId };
            test.DualLiseId = "ProbeLiseDouble";
            test.DefaultGain = 1.8;

            // Mock ProbeLiseUp
            test.FakeLiseUp = new Mock<ProbeLise>();
            var configUp = new ProbeLiseConfig
            {
                DeviceID = test.LiseUpId,
                ModulePosition = ModulePositions.Up,
                Lag = 1000,
                DetectionCoef = 9,
                PeakInfluence = 0,
                ThicknessThresholdInTheAir = 30.Micrometers(),
                SaturationValue = 6.7F,
                DiscriminationDistanceInTheAir = 40.Micrometers(),
            };
            test.FakeLiseUp.SetupGet(_ => _.Configuration).Returns(configUp);
            test.FakeLiseUp.SetupGet(_ => _.DeviceID).Returns(test.LiseUpId);
            test.HardwareManager.Probes[test.LiseUpId] = test.FakeLiseUp.Object;

            // Mock ProbeLiseBottom
            test.FakeLiseBottom = new Mock<ProbeLise>();
            var configDown = new ProbeLiseConfig
            {
                DeviceID = test.LiseBottomId,
                ModulePosition = ModulePositions.Down,
                Lag = 1000,
                DetectionCoef = 9,
                PeakInfluence = 0,
                ThicknessThresholdInTheAir = 30.Micrometers(),
                SaturationValue = 6.7F,
                DiscriminationDistanceInTheAir = 40.Micrometers(),
            };
            test.FakeLiseBottom.SetupGet(_ => _.Configuration).Returns(configDown);
            test.FakeLiseBottom.SetupGet(_ => _.DeviceID).Returns(test.LiseBottomId);
            test.HardwareManager.Probes[test.LiseBottomId] = test.FakeLiseBottom.Object;

            // Mock DualProbeLise
            test.FakeDualLise = new Mock<IProbeDualLise>();
            var calibrationManager = new ProbeCalibrationManagerLise("ProbeDualLise", new CancellationToken(), 10);
            var configDouble = new ProbeDualLiseConfig
            {
                DeviceID = test.DualLiseId,
                ProbeUpID = test.LiseUpId,
                ProbeDownID = test.LiseBottomId,
                ModulePosition = ModulePositions.Up,
                ThicknessThresholdInTheAir = 30.Micrometers(),
            };
            test.FakeDualLise.SetupGet(_ => _.CalibrationManager).Returns(calibrationManager);
            test.FakeDualLise.SetupGet(_ => _.Configuration).Returns(configDouble);
            test.FakeDualLise.SetupGet(_ => _.ProbeLiseUp).Returns(test.FakeLiseUp.Object);
            test.FakeDualLise.SetupGet(_ => _.ProbeLiseDown).Returns(test.FakeLiseBottom.Object);
            test.HardwareManager.Probes[test.DualLiseId] = test.FakeDualLise.Object;
        }

        public static Mock<ProbeLise> GetFakeProbeLise(string probeLiseId, ITestWithProbeLise test)
        {
            return probeLiseId == test.LiseUpId ? test.FakeLiseUp :test.FakeLiseBottom;
        }

        public static Mock<IProbeDualLise> GetFakeDualLise(ITestWithProbeLise test)
        {
            return test.FakeDualLise;
        }

        public static void AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(string probeId, IProbeSignal signal, ITestWithProbeLise test)
        {
            var fakeLise = GetFakeProbeLise(probeId, test);
            fakeLise.Invocations.Clear();
            fakeLise.Setup(_ => _.DoSingleAcquisition(It.IsAny<SingleLiseInputParams>())).Returns(signal);
            fakeLise.Setup(_ => _.DoMultipleAcquisitions(It.IsAny<SingleLiseInputParams>())).Returns((SingleLiseInputParams param) => Enumerable.Repeat(signal, param.NbMeasuresAverage));
        }

        public static void AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(string probeId, List<IProbeSignal> signals, ITestWithProbeLise test)
        {
            var fakeLise = GetFakeProbeLise(probeId, test);
            fakeLise.Invocations.Clear();

            var queueSignals = new Queue<IProbeSignal>();
            foreach (var signal in signals)
            {
                queueSignals.Enqueue((signal));
            }

            fakeLise.Setup(m => m.DoSingleAcquisition(It.IsAny<SingleLiseInputParams>())).Returns(queueSignals.Dequeue);
            fakeLise.Setup(m => m.DoMultipleAcquisitions(It.IsAny<SingleLiseInputParams>())).Returns((SingleLiseInputParams param) => Enumerable.Repeat(queueSignals.Dequeue(), param.NbMeasuresAverage));
        }
    }
}
