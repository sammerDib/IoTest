using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Shared.TestUtils
{
    public static class SimulatedData
    {
        public static WaferDimensionalCharacteristic ValidWaferCharacteristic()
        {
            var wafer = new WaferDimensionalCharacteristic();
            wafer.Diameter = 300.Millimeters();
            return wafer;
        }

        public static ScanRange ValidScanRange()
        {
            return new ScanRange(
                min: -9,
                max: 12);
        }

        public static ScanRange InvalidScanRange()
        {
            return new ScanRange(
                min: 10,
                max: 9);
        }

        public static ScanRangeWithStep ValidScanRangeWithStep()
        {
            return new ScanRangeWithStep(
                min: 1,
                max: 15,
                step: 0.5);
        }

        public static ScanRangeWithStep InvalidScanRangeWithStep()
        {
            return new ScanRangeWithStep(
                min: 10,
                max: 9,
                step: -0.5);
        }

        public static PatternRecognitionData ValidPatternRecognitionData()
        {
            return new PatternRecognitionData(
                patternReference: new DummyUSPImage(10, 10, 255).ToExternalImage(),
                roi: new RegionOfInterest(),
                gamma: 0.25);
        }

        public static PatternRecognitionData InvalidPatternRecognitionData()
        {
            return new PatternRecognitionData(
                patternReference: null,
                roi: null,
                gamma: double.NaN);
        }

        public static PositionWithPatternRec ValidPositionWithPatternRecData()
        {
            return new PositionWithPatternRec(
                position: new XYZPosition(new WaferReferential(), 0, 0, 0),
                patternRec: ValidPatternRecognitionData());
        }

        public static PositionWithPatternRec InvalidPositionWithPatternRecData()
        {
            return new PositionWithPatternRec(
                position: null,
                patternRec: InvalidPatternRecognitionData());
        }

        public static AutoFocusCameraInput ValidAutoFocusCameraInput()
        {
            return new AutoFocusCameraInput(
                    rangeType: ScanRangeType.Configured,
                    scanRange: ValidScanRangeWithStep());
        }

        public static AutoFocusCameraInput InvalidAutoFocusCameraInput()
        {
            return new AutoFocusCameraInput(
                    rangeType: ScanRangeType.Configured,
                    scanRange: null);
        }

        public static PatternRecInput ValidPatternRecInput()
        {
            return new PatternRecInput(
              data: ValidPatternRecognitionData(),
              runAutofocus: false,
              getZFocusInput: ValidGetZFocusInput());
        }

        public static GetZFocusInput ValidGetZFocusInput()
        {
            return new GetZFocusInput() { TargetDistanceSensor = 7500.0 };
        }

        public static AxisOrthogonalityInput ValidAxisOrthogonalityInput()
        {
            return new AxisOrthogonalityInput();
        }
        public static MultiSizeChuckInput ValidMultiSizeChuckInput()
        {
            return new MultiSizeChuckInput(150.Millimeters());
        }

        public static PixelSizeComputationInput ValidPixelSizeComputationInput()
        {
            return new PixelSizeComputationInput();
        }
        public static AutoExposureInput ValidAutoExposureInput()
        {
            return new AutoExposureInput();
        }        
    }
}
