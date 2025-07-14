#pragma once
#include <opencv2/opencv.hpp>
#include <math.h>
#include <filesystem>
#include "CalibrationParameters.hpp"
#include "CheckerBoardsSettings.hpp"

#pragma unmanaged

namespace psd {
    enum CheckerBoardPosition {
        Top,
        Bottom,
        Left,
        Right,
        Unknown
    };

    struct CheckerBoard {
        CheckerBoard(CheckerBoardPosition position, cv::Point topLeftCoordinate, cv::Mat checkerBoardImg) {
            Position = position;
            TopLeftCoordinate = topLeftCoordinate;
            CheckerBoardImg = checkerBoardImg;
        }
        CheckerBoardPosition Position;
        cv::Point TopLeftCoordinate;
        cv::Mat CheckerBoardImg;
    };

    struct CornersPoints {
        CornersPoints(std::vector<std::vector<cv::Point3f>> cornersObjPoints, std::vector<std::vector<cv::Point2f>> cornersImgPoints) {
            CornersObjPoints = cornersObjPoints;
            CornersImgPoints = cornersImgPoints;
        }
        //vectors of 3D points for all chessboard images (world coordinate frame)
        std::vector<std::vector<cv::Point3f>> CornersObjPoints;
        //vectors of 2D points for all chessboard images (camera coordinate frame)
        std::vector<std::vector<cv::Point2f>> CornersImgPoints;
    };

    /**
    * Detect checker boards on calibration wafer image
    *
    * @param img                                - calibration wafer image
    * @param sampleRatio                        - ratio to rescale image to gain some time by detecting at lower resolution
    * @param checkerBoardsSettings              - checker boards settings
    *
    * @return All checker board detected
    */
    std::vector<CheckerBoard> DetectCheckerBoards(cv::Mat img, int sampleRatio, CheckerBoardsSettings checkerBoardsSettings);

    /**
    * Detect all checker board corners on calibration wafer image
    *
    * @param imgs                               - calibration wafer images
    * @param checkerBoardsSettings              - checker boards settings
    * @param useRealScreenCoordinate            - if true, use real coordinates on wafer for image corner coordinate
    * @param directoryPathToStoreReport         - directory path to store report if not empty
    *
    * @return Real corners coordinates and image corners coordinates
    */
    CornersPoints ComputeCornersPoints(std::vector<cv::Mat> imgs, CheckerBoardsSettings checkerBoardsSettings, bool useRealScreenCoordinate, std::filesystem::path directoryPathToStoreReport);
}