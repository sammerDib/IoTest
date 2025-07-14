using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Shared.TestUtils
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
                cameraId: "cameraId",
                patternReference: new DummyUSPImage(10, 10, 255).ToExternalImage(),
                roi: new RegionOfInterest(),
                gamma: 0.25);
        }

        public static PatternRecognitionData InvalidPatternRecognitionData()
        {
            return new PatternRecognitionData(
                cameraId: null,
                patternReference: null,
                roi: null,
                gamma: double.NaN);
        }

        public static PositionWithPatternRec ValidPositionWithPatternRecData()
        {
            return new PositionWithPatternRec(
                position: new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0),
                patternRec: ValidPatternRecognitionData(),
                context: new TopImageAcquisitionContext());
        }

        public static PositionWithPatternRec InvalidPositionWithPatternRecData()
        {
            return new PositionWithPatternRec(
                position: null,
                patternRec: InvalidPatternRecognitionData(),
                context: null);
        }

        public static PatternRecognitionDataWithContext ValidPatternRecognitionDataWithContext()
        {
            return new PatternRecognitionDataWithContext(
                 context: new TopImageAcquisitionContext(),
                 cameraId: "cameraId",
                 patternReference: new DummyUSPImage(10, 10, 255).ToExternalImage(),
                 roi: new RegionOfInterest(),
                 gamma: 0.25);
        }

        public static PatternRecognitionDataWithContext InvalidPatternRecognitionDataWithContext()
        {
            return new PatternRecognitionDataWithContext(
                context: null,
                cameraId: null,
                patternReference: null,
                roi: null,
                gamma: double.NaN);
        }

        public static AFLiseInput ValidAFLiseInput()
        {
            return new AFLiseInput(
                probeID: "probeID",
                gain: 1.8,
                zPosScanRange: ValidScanRange());
        }

        public static AFLiseInput InvalidAFLiseInput()
        {
            return new AFLiseInput(
                probeID: null,
                gain: double.NaN,
                zPosScanRange: null);
        }

        public static AutoFocusSettings ValidAFSettings()
        {
            return new AutoFocusSettings()
            {
                CameraId = "cameraId",
                CameraScanRange = ScanRangeType.Medium,
                LiseGain = 1.3,
                LiseOffsetX = 1.0.Micrometers(),
                LiseOffsetY = 2.0.Micrometers(),
                ProbeId = "probeID",
                Type = AutoFocusType.LiseAndCamera
            };
        }

        public static AutoFocusSettings InvalidAFSettings()
        {
            return new AutoFocusSettings()
            {
                CameraId = null,
                CameraScanRange = ScanRangeType.Medium,
                LiseGain = 1.3,
                ProbeId = null,
                Type = AutoFocusType.LiseAndCamera
            };
        }

        public static AutofocusInput ValidAFInput()
        {
            return new AutofocusInput(ValidAFSettings());
        }

        public static AutofocusInput InvalidAFInput()
        {
            return new AutofocusInput(InvalidAFSettings());
        }

        public static AFCameraInput ValidAFCameraInput()
        {
            return new AFCameraInput(
                    cameraId: "cameraId",
                    rangeType: ScanRangeType.Configured,
                    scanRange: ValidScanRangeWithStep());
        }

        public static AFCameraInput InvalidAFCameraInput()
        {
            return new AFCameraInput(
                    cameraId: null,
                    rangeType: ScanRangeType.Configured,
                    scanRange: null);
        }

        public static PatternRecInput ValidPatternRecInput()
        {
            return new PatternRecInput(
              data: ValidPatternRecognitionData(),
              runAutofocus: true,
              autofocusSettings: ValidAFSettings());
        }

        public static VSIInput ValidVSIInput()
        {
            return new VSIInput()
            {
                InitialContext = null,
                ObjectiveId = "ObjectiveId",
                CameraId = "CameraId",
                StartPosition = new XYZTopZBottomPosition(new WaferReferential(), double.NaN, double.NaN, 10, double.NaN),
                StepSize = 40.Nanometers(),
                StepCount = 500,
                ROI = null,
            };
        }

        public static PSIInput ValidPSIInput()
        {
            return new PSIInput(
              initialContext: null,
              objectiveId: "ObjectiveId",
              cameraId: "CameraId",
              step: 10.Micrometers(),
              stepCount: 7,
              imagesPerStep: 12,
              roi: null,
              phaseCalculation: PSIInput.PhaseCalculationAlgo.Hariharan,
              phaseUnwrapping: PSIInput.PhaseUnwrappingAlgo.Goldstein,
              wavelength: 618.Nanometers());
        }

        public static AlignmentMarksInput ValidAlignmentMarksInput()
        {
            return new AlignmentMarksInput(
                  site1Images: new List<PositionWithPatternRec>() { ValidPositionWithPatternRecData() },
                  site2Images: new List<PositionWithPatternRec>() { ValidPositionWithPatternRecData() },
                  autofocusSettings: ValidAFSettings());
        }

        public static WaferMapInput ValidWaferMapInput()
        {
            return new WaferMapInput(
                topLeftCorner: ValidPositionWithPatternRecData(),
                bottomRightCorner: ValidPositionWithPatternRecData(),
                waferCharacteristics: new WaferDimensionalCharacteristic() { Diameter = 300.Millimeters() },
                edgeExclusion: 1.Millimeters(),
                dieDimensions: new DieDimensionalCharacteristic(10.Millimeters(), 10.Millimeters(), 1.Millimeters(), 1.Millimeters(), 0.Degrees()));
        }

        public static DieAndStreetSizesInput ValidDieAndStreetSizesInput()
        {
            var topLeft = ValidPositionWithPatternRecData();
            topLeft.Position = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);
            var bottomRight = ValidPositionWithPatternRecData();
            bottomRight.Position = new XYZTopZBottomPosition(new WaferReferential(), 15, -10, 0, 0);
            var wafer = ValidWaferCharacteristic();
            wafer.Diameter = 20.Millimeters();
            return new DieAndStreetSizesInput(
                topLeftCorner: topLeft,
                bottomRightCorner: bottomRight,
                waferCharacteristics: wafer,
                edgeExclusion: 1.Millimeters(),
                autofocusSettings: ValidAFSettings());
        }

        public static CheckPatternRecInput ValidCheckPatternRecInput()
        {
            return
          new CheckPatternRecInput(
              ValidPositionWithPatternRecData(),
              new List<XYZTopZBottomPosition>() {
                    new XYZTopZBottomPosition(new WaferReferential(), 100, 100, 0, 0),
                    new XYZTopZBottomPosition(new WaferReferential(), 100, 25, 0, 0)},
              1.Millimeters());
        }

        public static ImagePreprocessingInput ValidImagePreprocessingInput()
        {
            return
          new ImagePreprocessingInput(
              cameraId: "CameraUpId",
              position: new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0),
                roi: new CenteredRegionOfInterest(),
                gamma: 0.25
                );
        }

        public static EllipseCriticalDimensionInput ValidEllipseCriticalDimensionInput()
        {
            return
          new EllipseCriticalDimensionInput(
              image: new DummyUSPImage(10, 10, 255).ToServiceImage(),
              objectiveId: "ObjectiveId",
              roi: new CenteredRegionOfInterest(),
              approximateLength: 5.Millimeters(),
              approximateWidth: 5.Millimeters(),
              lengthTolerance: 1.Millimeters(),
              widthTolerance: 1.Millimeters());
        }

        public static CircleCriticalDimensionInput ValidCircleCriticalDimensionInput()
        {
            return
          new CircleCriticalDimensionInput(
              image: new DummyUSPImage(10, 10, 255).ToServiceImage(),
              objectiveId: "ObjectiveId",
              roi: new CenteredRegionOfInterest(),
              approximateDiameter: 5.Millimeters(),
              diameterTolerance: 1.Millimeters());
        }
    }
}
