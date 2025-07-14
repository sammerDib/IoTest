#pragma once
#include <opencv2/opencv.hpp>
#include <math.h>
#include <filesystem>
#include "CalibrationParameters.hpp"
#include "InputSystemParameters.hpp"

#pragma unmanaged

namespace psd {
    /**
    * Rotation and Translation vectors that transform a point expressed in the wafer coordinate referential to the camera coordinate referential
    * waferCoordinate * RotationWaferToCamera + TranslationWaferToCamera = cameraCoordinate
    */
    struct ExtrinsicCameraParameters {
        ExtrinsicCameraParameters() {}
        ExtrinsicCameraParameters(cv::Mat rotationMatrix, cv::Mat translationMatrix)
        {
            RWaferToCamera = rotationMatrix;
            TWaferToCamera = translationMatrix;
        }
        cv::Mat RWaferToCamera; // Rotation matrix to convert from wafer to camera referential
        cv::Mat TWaferToCamera; // Translation vector to convert from wafer to camera referential
    };

    /**
    * Rotation and Translation vectors that transform a point expressed in the screen coordinate referential to the wafer coordinate referential
    * screenCoordinate * RotationScreenToWafer + TranslationScreenToWafer = waferCoordinate
    */
    struct ExtrinsicScreenParameters {
        ExtrinsicScreenParameters() {}
        ExtrinsicScreenParameters(cv::Mat rotationMatrix, cv::Mat translationMatrix)
        {
            RScreenToWafer = rotationMatrix;
            TScreenToWafer = translationMatrix;
        }
        cv::Mat RScreenToWafer; // Rotation matrix to convert from screen to wafer referential
        cv::Mat TScreenToWafer; // Translation vector to convert from screen to wafer referential
    };

    /**
    * Rotation and Translation vectors that transform a point expressed in the screen coordinate referential to the camera coordinate referential
    * screenCoordinate * RotationScreenToCamera + TranslationScreenToCamera = cameraCoordinate
    */
    struct ExtrinsicSystemParameters {
        ExtrinsicSystemParameters() {}
        ExtrinsicSystemParameters(cv::Mat rotationMatrix, cv::Mat translationMatrix)
        {
            RScreenToCamera = rotationMatrix;
            TScreenToCamera = translationMatrix;
        }
        cv::Mat RScreenToCamera; // Rotation matrix to convert from screen to camera referential
        cv::Mat TScreenToCamera; // Translation vector to convert from screen to camera referential
    };

    /**
    * All system calibration parameters
    */
    struct SystemParameters {
        SystemParameters(ExtrinsicCameraParameters extrinsicCamera, ExtrinsicScreenParameters extrinsicScreen, ExtrinsicSystemParameters extrinsicSystem)
        {
            ExtrinsicScreen = extrinsicScreen;
            ExtrinsicSystem = extrinsicSystem;
            ExtrinsicCamera = extrinsicCamera;
        }
        ExtrinsicCameraParameters ExtrinsicCamera;
        ExtrinsicScreenParameters ExtrinsicScreen;
        ExtrinsicSystemParameters ExtrinsicSystem;
    };

    /**
    * System calibration process for the PSD tool
    *
    * @param phaseMapX                  - unwrapped phase map X
    * @param phaseMapY                  - unwrapped phase map Y
    * @param calibrationWaferImg        - grayscale image of the wafer with calibration patterns
    * @param cameraIntrinsic            - 3x3 floating-point camera intrinsic matrix = [fx 0  cx, 0  fy cy, 0  0  1]
    * @param cameraDistortion           - 5x1 floating-point matrix of distortion coefficients = [k1, k2, p1, p2, k3]
    * @param params                     - input parameters to configure the calibration computation
    * @param mask                       - optional mask to use (if not provided it will be calculate during calibration process)
    * @param directoryPathToStoreReport - directory path to store report if not empty
    *
    */
    SystemParameters CalibrateSystem(cv::Mat phaseMapX, cv::Mat phaseMapY, cv::Mat calibrationWaferImg, cv::Mat cameraIntrinsic, cv::Mat cameraDistortion, InputSystemParameters params, cv::Mat mask = cv::Mat(), std::filesystem::path directoryPathToStoreReport = "");
}