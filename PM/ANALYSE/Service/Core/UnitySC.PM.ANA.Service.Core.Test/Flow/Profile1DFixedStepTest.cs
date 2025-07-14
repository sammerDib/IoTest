using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.Profile1D;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Profile1D;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosCppWrapper;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class Profile1DFixedStepTest : TestWithMockedHardware<Profile1DFixedStepTest>, ITestWithAxes, ITestWithProbeLise, ITestWithCamera
    {
        #region Interface properties
        public Mock<IAxes> SimulatedAxes { get; set; }

        public List<string> SimpleProbesLise { get; set; }
        public string LiseUpId { get; set; }
        public string LiseBottomId { get; set; }
        public string DualLiseId { get; set; }
        public double DefaultGain { get; set; }
        public Mock<ProbeLise> FakeLiseUp { get; set; }
        public Mock<ProbeLise> FakeLiseBottom { get; set; }
        public Mock<IProbeDualLise> FakeDualLise { get; set; }
        public string ObjectiveUpId { get; set; }
        public string ObjectiveBottomId { get; set; }
        public Length PixelSizeX { get; set; }
        public Length PixelSizeY { get; set; }

        public string CameraUpId { get; set; }
        public string CameraBottomId { get; set; }
        public Mock<CameraBase> SimulatedCameraUp { get; set; }
        public Mock<CameraBase> SimulatedCameraBottom { get; set; }
        #endregion Interface properties

        private Profile1DFixedStepInput GetMinimalInput()
        {
            return new Profile1DFixedStepInput()
            {
                LiseData = new LiseInput(LiseUpId),
            };
        }

        [TestMethod]
        public void NominalCase()
        {
            var flowInput = GetMinimalInput();
            flowInput.StepLength = new Length(1.0, LengthUnit.Millimeter);
            flowInput.StartPosition = new XYPosition()
            {
                Referential = new MotorReferential(),
                X = 0.0,
                Y = 0.0,
            };
            flowInput.EndPosition = new XYPosition()
            {
                Referential = new MotorReferential(),
                X = 0.0,
                Y = 4.0,
            };

            var signals = new List<IProbeSignal>()
            {
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(100.0.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(101.Micrometers()),

                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(200.Micrometers()),

                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(302.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(298.5.Micrometers()),
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);

            var flow = new Profile1DFixedStepFlow(flowInput);
            var result = flow.Execute();

            var expectedProfile = new Profile2d
            {
                new Point2d(0.0, 202.0),
                new Point2d(1.0, 201.0),
                new Point2d(2.0, 102.0),
                new Point2d(3.0, 0.0),
                new Point2d(4.0, 3.5),
            };
            double toleranceY = 3.0;

            Assert.IsTrue(
                expectedProfile.Count() == result.Profile.Count() &&
                expectedProfile.Zip(result.Profile, (p1, p2) => p1.X == p2.X && Math.Abs(p1.Y - p2.Y) < toleranceY).All(b => b)
            );
        }
    }
}
