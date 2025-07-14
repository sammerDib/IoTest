#pragma once

#include "ImageData.h"
#include "Tools.h"

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {

    using namespace System::Runtime::InteropServices;

    public enum class Polarisation {
        Vertical,
        Horizontal
    };

    /**
     * Represents camera intrinsic parameters
     */
    public ref struct CalibrationParameters {
        CalibrationParameters(array<double, 2>^ intrinsic, array<double>^ distortion, double rms) {
            CameraIntrinsic = intrinsic;
            Distortion = distortion;
            RMS = rms;
        }

        /**
         * 3x3 floating-point camera intrinsic matrix =
         *      fx 0  cx
         *      0  fy cy
         *      0  0  1
         */
        array<double, 2>^ CameraIntrinsic;

        /**
         * vector of distortion coefficients of :
         *  5 elements: k1, k2, p1, p2, k3
         */
        array<double>^ Distortion;

        /**
        * average re-projection error
        */
        double RMS;
    };

    /**
     * Origins of the four checker boards on calibration wafer
     */
    public ref struct CheckerBoardsOrigins {
        CheckerBoardsOrigins(Point2f^ leftCheckerBoardOrigin, Point2f^ bottomCheckerBoardOrigin, Point2f^ rightCheckerBoardOrigin, Point2f^ topCheckerBoardOrigin)
        {
            LeftCheckerBoardOrigin = leftCheckerBoardOrigin;
            BottomCheckerBoardOrigin = bottomCheckerBoardOrigin;
            RightCheckerBoardOrigin = rightCheckerBoardOrigin;
            TopCheckerBoardOrigin = topCheckerBoardOrigin;
        }

        Point2f^ TopCheckerBoardOrigin;
        Point2f^ BottomCheckerBoardOrigin;
        Point2f^ RightCheckerBoardOrigin;
        Point2f^ LeftCheckerBoardOrigin;
    };

    /**
     * Settings of checker boards on calibration wafer
     */
    public ref struct CheckerBoardsSettings {
        CheckerBoardsSettings(CheckerBoardsOrigins^ checkerBoardOrigins, int squareXNumber, int squareYNumber, float squareSizeMm, float pixelSizeMm, bool useAllCheckerBoards)
        {
            UseAllCheckerBoards = useAllCheckerBoards;

            SquareXNb = squareXNumber;
            SquareYNb = squareYNumber;
            SquareSizeMm = squareSizeMm;
            PixelSizeMm = pixelSizeMm;

            CheckerBoardsTopLeftOrigins = checkerBoardOrigins;
        }

        bool UseAllCheckerBoards;
        int SquareXNb;
        int SquareYNb;
        float SquareSizeMm;
        float PixelSizeMm;
        CheckerBoardsOrigins^ CheckerBoardsTopLeftOrigins;
    };

    /**
     * Input parameters to compute system calibration
     */
    public ref struct InputSystemParameters {
        InputSystemParameters(CheckerBoardsSettings^ checkerBoardsSettings, float edgeExclusion, float waferRadius, float nbPtsScreen, float frangePixels, float screenPixelSizeMm) {
            EdgeExclusion = edgeExclusion;
            WaferRadius = waferRadius;
            NbPtsScreen = nbPtsScreen;
            FrangePixels = frangePixels;
            ScreenPixelSizeMm = screenPixelSizeMm;
            CheckerBoards = checkerBoardsSettings;
        }

        float EdgeExclusion;
        float WaferRadius;
        float NbPtsScreen;
        float FrangePixels;
        float ScreenPixelSizeMm;

        CheckerBoardsSettings^ CheckerBoards;
    };

    /**
    * Rotation and Translation vectors that transform a point expressed in the wafer coordinate referential to the camera coordinate referential
    * waferCoordinate * RotationWaferToCamera + TranslationWaferToCamera = cameraCoordinate
    */
    public ref struct ExtrinsicCameraParameters {
        ExtrinsicCameraParameters() {}
        ExtrinsicCameraParameters(array<double, 2>^ rotationMatrix, array<double>^ translationMatrix)
        {
            RWaferToCamera = rotationMatrix;
            TWaferToCamera = translationMatrix;
        }
        array<double, 2>^ RWaferToCamera; // Rotation matrix to convert from wafer to camera referential
        array<double>^ TWaferToCamera; // Translation vector to convert from wafer to camera referential
    };

    /**
    * Rotation and Translation vectors that transform a point expressed in the screen coordinate referential to the wafer coordinate referential
    * screenCoordinate * RotationScreenToWafer + TranslationScreenToWafer = waferCoordinate
    */
    public ref struct ExtrinsicScreenParameters {
        ExtrinsicScreenParameters() {}
        ExtrinsicScreenParameters(array<double, 2>^ rotationMatrix, array<double>^ translationMatrix)
        {
            RScreenToWafer = rotationMatrix;
            TScreenToWafer = translationMatrix;
        }
        array<double, 2>^ RScreenToWafer; // Rotation matrix to convert from screen to wafer referential
        array<double>^ TScreenToWafer; // Translation vector to convert from screen to wafer referential
    };

    /**
    * Rotation and Translation vectors that transform a point expressed in the screen coordinate referential to the camera coordinate referential
    * screenCoordinate * RotationScreenToCamera + TranslationScreenToCamera = cameraCoordinate
    */
    public ref struct ExtrinsicSystemParameters {
        ExtrinsicSystemParameters() {}
        ExtrinsicSystemParameters(array<double, 2>^ rotationMatrix, array<double>^ translationMatrix)
        {
            RScreenToCamera = rotationMatrix;
            TScreenToCamera = translationMatrix;
        }
        array<double, 2>^ RScreenToCamera; // Rotation matrix to convert from screen to camera referential
        array<double>^ TScreenToCamera; // Translation vector to convert from screen to camera referential
    };

    /**
    * All system calibration parameters
    */
    public ref struct SystemParameters {
        SystemParameters(ExtrinsicCameraParameters^ extrinsicCamera, ExtrinsicScreenParameters^ extrinsicScreen, ExtrinsicSystemParameters^ extrinsicSystem)
        {
            ExtrinsicScreen = extrinsicScreen;
            ExtrinsicSystem = extrinsicSystem;
            ExtrinsicCamera = extrinsicCamera;
        }
        ExtrinsicCameraParameters^ ExtrinsicCamera;
        ExtrinsicScreenParameters^ ExtrinsicScreen;
        ExtrinsicSystemParameters^ ExtrinsicSystem;
    };

    public ref class PSDCalibration {
    public:

        /**
         * Calibrate camera process for the PSD tool
         *
         * @param imgs                       - chessboard images
         * @param checkerBoardsSettings      - settings of checker boards on calibration wafer
         * @param fixAspectRatio             - if true, functions consider only fy as a free parameter (fix the aspect ratio for the focal length). The ratio fx/fy stays the same as in the input cameraMatrix
         *
         * @return Intrinsic parameters of the camera (cx, cy, fx, fy, distortion coefficients)
         */
        static CalibrationParameters^ CalibrateCamera(array<ImageData^>^ chessboardImages, CheckerBoardsSettings^ checkerBoardsSettings, bool fixAspectRatio);

        /**
        * System calibration process for the PSD tool
        *
        * @param phaseMapX                  - unwrapped phase map X
        * @param phaseMapY                  - unwrapped phase map Y
        * @param calibrationWaferImg        - grayscale image of the wafer with calibration patterns
        * @param cameraIntrinsic            - 3x3 floating-point camera intrinsic matrix = [fx 0  cx, 0  fy cy, 0  0  1]
        * @param cameraDistortion           - 5x1 floating-point matrix of distortion coefficients = [k1, k2, p1, p2, k3]
        * @param params                     - input parameters to configure the calibration computation
        *
        * @return System parameters
        */
        static SystemParameters^ CalibrateSystem(ImageData^ phaseMapX, ImageData^ phaseMapY, ImageData^ calibrationWaferImg, array<double, 2>^ cameraIntrinsic, array<double>^ cameraDistortion, InputSystemParameters^ params);

        /**
        * Uniformity correction process for the PSD tool
        *
        * @param brightfield                      - brightfield image
        * @param waferRadiusInMm                  - radius of the wafer present in the image in millimeters
        * @param pixelSizeInMm                    - pixel size of the image in millimeters
        * @param calibrationParams                - intrinsic camera parameters and distortion matrix from the camera calibration output
        * @param extrinsicCameraParams            - extrinsic camera params from the system calibration output (rotation / translation wafer to camera matrices)
        * @param extrinsicScreenParams            - extrinsic screen params from the system calibration output (rotation / translation screen to wafer matrices)
        * @param refractiveIndexByWavelength      - dictionary containing the refractive indices by wavelength of the material
        * @param wavelengths                      - array of waveslengths in nm that the PSD screen emits
        * @param patternThresholdForPolynomialFit - threshold to eliminate the patterns for the polynomial fit
        * @param screenPolarizationIsVertical     - whether the polarization of the screen is vertical or not
        * @param reportPath                       - optional path to report debug images to
        *
        * @return Uniformity correction image
        */
        static ImageData^ CalibrateUniformityCorrection(ImageData^ brightfield, float waferRadiusInMm, float pixelSizeInMm, CalibrationParameters^ calibrationParams, ExtrinsicCameraParameters^ extrinsicCameraParams, ExtrinsicScreenParameters^ extrinsicScreenParams, Dictionary<int, Tuple<double, double>^>^ refractiveIndexByWavelength, array<int>^ wavelengths, double patternThresholdForPolynomialFit, bool screenPolarizationIsVertical, [OptionalAttribute] String^ reportPath);

        /**
        * Apply the uniformity correction image to an acquisition
        *
        * @param brightfield                           - brightfield acquisition image
        * @param correctionImage                       - uniformity correction image
        * @param pixelSizeInMm                         - pixel size in millimeters
        * @param waferRaidusInMm                       - wafer radius in millimeters
        * @param targetSaturationLevel                 - target level of saturation for the resulting image
        * @param acceptablePercentageOfSaturatedPixels - percentage of sturated pixels to discard
        *
        * @return Corrected image
        */
        static ImageData^ ApplyUniformityCorrection(ImageData^ brightfield, ImageData^ correctionImage, float pixelSizeInMm, float waferRadiusInMm, int targetSaturationLevel, float acceptablePercentageOfSaturatedPixels);
    };
}
