using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Hardware.FilterWheel;
using UnitySC.PM.EME.Service.Core.Flows.AutoExposure;
using UnitySC.PM.EME.Service.Core.Flows.AutoFocus;
using UnitySC.PM.EME.Service.Core.Flows.FilterCalibration;
using UnitySC.PM.EME.Service.Core.Flows.PatternRec;
using UnitySC.PM.EME.Service.Core.Flows.PixelSize;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Shared.TestUtils;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Test.Flow
{
    [TestClass]
    public class FilterCalibrationTest : TestWithMockedHardware<FilterCalibrationTest>, ITestWithFilterWheel,
        ITestWithPhotoLumAxes, ITestWithCamera
    {
        private readonly List<Filter> _filters = new List<Filter>
        {
            new Filter("Empty", EMEFilter.NoFilter, 1, 7700.0),
            new Filter("Filter 1", EMEFilter.BandPass450nm50, 62, 7800.0),
            new Filter("Filter 2", EMEFilter.BandPass550nm50, 119, 7900.0),
            new Filter("Filter 3", EMEFilter.LowPass650nm, 179, 8000.0)
        };

        public Mock<FilterWheel> SimulatedFilterWheel { get; set; }
        public Mock<PhotoLumAxes> SimulatedMotionAxes { get; set; }
        public DummyIDSCamera SimulatedCamera { get; set; }
        public IEmeraCamera EmeraCamera { get; set; }
        private Mock<PatternRecFlow> _simulatedPatternRecFlow { get; set; }
        private Mock<PatternRecFlow> _simulatedPatternRecFlowInError { get; set; }
        private Mock<PixelSizeComputationFlow> _pixelSizeComputationFlow { get; set; }
        private Mock<AutoExposureFlow> _simulatedAutoExposureFlow { get; set; }
        private Mock<GetZFocusFlow> _simulatedGetZFocusFlow { get; set; }
        private FlowsConfiguration _flowConfiguration { get; set; }

        protected override void SpecializeRegister()
        {
            Bootstrapper.SimulatedEmeraCamera.Setup(x => x.GetCameraExposureTime()).Returns(100.0);
            _flowConfiguration = new FlowsConfiguration();

            Bootstrapper.SimulatedEmeraCamera.Setup(x => x.SingleScaledAcquisition(It.IsAny<Int32Rect>(), It.IsAny<double>())).Returns(
                new ServiceImage 
                { 
                    Data = GenerateImageData(0), DataHeight = 1, DataWidth = 1 
                });

            _simulatedPatternRecFlow =
                new Mock<PatternRecFlow>(SimulatedData.ValidPatternRecInput(), Bootstrapper.SimulatedEmeraCamera.Object, null, null, null) { CallBase = true };
            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(GetExpectedPatternRec(5.Millimeters(), 5.Millimeters()))
                .Returns(GetExpectedPatternRec(10.Millimeters(), 10.Millimeters()))
                .Returns(GetExpectedPatternRec(-5.Millimeters(), 10.Millimeters()))
                .Returns(GetExpectedPatternRec(0.Millimeters(), 0.Millimeters()));

            _simulatedPatternRecFlowInError =
                new Mock<PatternRecFlow>(SimulatedData.ValidPatternRecInput(), Bootstrapper.SimulatedEmeraCamera.Object, null, null, null) { CallBase = true };
            _simulatedPatternRecFlowInError.SetupSequence(_ => _.Execute())
                .Returns(GetExpectedPatternRec(5.Millimeters(), 5.Millimeters()))
                .Returns(GetExpectedPatternRec(10.Millimeters(), 10.Millimeters()))
                .Returns(new PatternRecResult(new FlowStatus(FlowState.Error), 0, null, null, null))
                .Returns(GetExpectedPatternRec(0.Millimeters(), 0.Millimeters()));

            _pixelSizeComputationFlow =
                new Mock<PixelSizeComputationFlow>(SimulatedData.ValidPixelSizeComputationInput(), Bootstrapper.SimulatedEmeraCamera.Object,
                    null, null) { CallBase = true };
            _pixelSizeComputationFlow.SetupSequence(_ => _.Execute())
                .Returns(GetExpectedPixelSize(0.Millimeters()))
                .Returns(GetExpectedPixelSize(1.Millimeters()))
                .Returns(GetExpectedPixelSize(2.Millimeters()))
                .Returns(GetExpectedPixelSize(3.Millimeters()));

            _simulatedAutoExposureFlow =
                new Mock<AutoExposureFlow>(SimulatedData.ValidAutoExposureInput(), Bootstrapper.SimulatedEmeraCamera.Object)
                {
                    CallBase = true
                };

            var autoExposureSuccess = new AutoExposureResult()
            {
                Brightness = 1.0, ExposureTime = 100, Status = new FlowStatus(FlowState.Success)
            };
            _simulatedAutoExposureFlow.Setup(_ => _.Execute()).Returns(autoExposureSuccess);

            _simulatedGetZFocusFlow =
                new Mock<GetZFocusFlow>(SimulatedData.ValidGetZFocusInput())
                {
                    CallBase = true
                };

            var getZFocusSuccess = new GetZFocusResult()
            {
                Z = 7800.0,
                Status = new FlowStatus(FlowState.Success)
            };

            _simulatedGetZFocusFlow.Setup(_ => _.Execute()).Returns(getZFocusSuccess);

        }

        [TestMethod]
        public void ShouldCalibrateFiltersShifts()
        {
            // Given
            var input = new FilterCalibrationInput { Filters = _filters };
            var flow = new FilterCalibrationFlow(input, _flowConfiguration, Bootstrapper.SimulatedEmeraCamera.Object,
                _simulatedPatternRecFlow.Object, _pixelSizeComputationFlow.Object, _simulatedAutoExposureFlow.Object, _simulatedGetZFocusFlow.Object);

            // When
            flow.Execute();

            // Then
            SimulatedFilterWheel.Verify(x => x.Move(It.IsAny<double>()), Times.Exactly(5));

            flow.Result.Filters.Should().HaveCount(4);
            var expectedShifts = new List<Tuple<Length, Length>>()
            {
                new Tuple<Length, Length>(0.Millimeters(), 0.Millimeters()),
                new Tuple<Length, Length>(5.Millimeters(), 5.Millimeters()),
                new Tuple<Length, Length>(-10.Millimeters(), 5.Millimeters()),
                new Tuple<Length, Length>(-5.Millimeters(), -5.Millimeters())
            };
            flow.Result.Filters.Select(x => new Tuple<Length, Length>(x.ShiftX, x.ShiftY)).Should()
                .BeEquivalentTo(expectedShifts);
            var originalFilterDistances = _filters.Select(x => x.DistanceOnFocus).ToList();
            flow.Result.Filters.Select(x => x.DistanceOnFocus).Should().BeEquivalentTo(originalFilterDistances);
        }

        [TestMethod]
        public void ShouldCalibrateFiltersPixelSize()
        {
            // Given
            var input = new FilterCalibrationInput { Filters = _filters };
            var flow = new FilterCalibrationFlow(input, _flowConfiguration, Bootstrapper.SimulatedEmeraCamera.Object,
                _simulatedPatternRecFlow.Object, _pixelSizeComputationFlow.Object, _simulatedAutoExposureFlow.Object, _simulatedGetZFocusFlow.Object);

            // When
            flow.Execute();

            // Then
            SimulatedFilterWheel.Verify(x => x.Move(It.IsAny<double>()), Times.Exactly(5));

            flow.Result.Filters.Should().HaveCount(4);
            var expectedPixelSizes = new List<Length>()
            {
                0.Millimeters(), 1.Millimeters(), 2.Millimeters(), 3.Millimeters()
            };
            flow.Result.Filters.Select(x => x.PixelSize).Should().BeEquivalentTo(expectedPixelSizes);
            var originalFilterDistances = _filters.Select(x => x.DistanceOnFocus).ToList();
            flow.Result.Filters.Select(x => x.DistanceOnFocus).Should().BeEquivalentTo(originalFilterDistances);
        }

        [TestMethod]
        public void ShouldGetResultWhenPatternRecFail()
        {
            // Given
            var input = new FilterCalibrationInput { Filters = _filters };
            var flow = new FilterCalibrationFlow(input, _flowConfiguration, Bootstrapper.SimulatedEmeraCamera.Object,
                _simulatedPatternRecFlowInError.Object, _pixelSizeComputationFlow.Object,
                _simulatedAutoExposureFlow.Object, _simulatedGetZFocusFlow.Object);

            // When
            flow.Execute();

            // Then
            flow.Result.Filters.Should().HaveCount(4);
            var expectedStatus = new List<FilterCalibrationStatus>()
            {
                new FilterCalibrationStatus { State = FilterCalibrationState.Calibrated },
                new FilterCalibrationStatus { State = FilterCalibrationState.Calibrated },
                new FilterCalibrationStatus
                {
                    State = FilterCalibrationState.CalibrationError,
                    Message = "Filter calibration failed for Filter 2: $Pattern Recognition Failed."
                },
                new FilterCalibrationStatus { State = FilterCalibrationState.Calibrated }
            };
            flow.Result.Filters.Select(x => x.CalibrationStatus).Should().BeEquivalentTo(expectedStatus);
            var originalFilterDistances = _filters.Select(x => x.DistanceOnFocus).ToList();
            flow.Result.Filters.Select(x => x.DistanceOnFocus).Should().BeEquivalentTo(originalFilterDistances);
        }

        private byte[] GenerateImageData(int brightness)
        {
            return new[] { (byte)brightness };
        }

        private static PatternRecResult GetExpectedPatternRec(Length shiftX, Length shiftY)
        {
            return new PatternRecResult(new FlowStatus(FlowState.Success), 0, shiftX, shiftY, null);
        }

        private static PixelSizeComputationResult GetExpectedPixelSize(Length size)
        {
            return new PixelSizeComputationResult { Status = new FlowStatus(FlowState.Success), PixelSize = size };
        }
    }
}
