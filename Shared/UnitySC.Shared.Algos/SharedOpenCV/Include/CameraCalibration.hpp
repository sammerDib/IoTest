#pragma once
#include <opencv2/opencv.hpp>
#include <math.h>
#include <filesystem>
#include "CalibrationParameters.hpp"
#include "CheckerBoardsSettings.hpp"

#pragma unmanaged

namespace psd {

    /**
    * Calibrate camera to find intrinsic parameters of the camera
    *
    * @param imgs                       - chessboard images
    * @param checkerBoardsSettings      - settings of checker boards on calibration wafer
    * @param fixAspectRatio             - if true, functions consider only fy as a free parameter (fix the aspect ratio for the focal length). The ratio fx/fy stays the same as in the input cameraMatrix
    * @param directoryPathToStoreReport - directory path to store report if not empty
    *
    * @return Intrinsic parameters of the camera (cx, cy, fx, fy, distortion coefficients)
    */
    CalibrationParameters CalibrateCamera(std::vector<cv::Mat> imgs, CheckerBoardsSettings checkerBoardsSettings, bool fixAspectRatio = false, std::filesystem::path directoryPathToStoreReport = "");
}