#include "CameraCalibration.hpp"
#include "CImageTypeConvertor.hpp"
#include <opencv2/calib3d.hpp>
#include "ReportingUtils.hpp"
#include "CheckerBoardsDetector.hpp"
#include "CShapeFinder.hpp"

using namespace std;
using namespace cv;

namespace psd {
    namespace {
    }

    CalibrationParameters CalibrateCamera(std::vector<cv::Mat> imgs, CheckerBoardsSettings checkerBoardsSettings, bool fixAspectRatio, std::filesystem::path directoryPathToStoreReport) {
        CornersPoints cornersPoints = ComputeCornersPoints(imgs, checkerBoardsSettings, false, directoryPathToStoreReport);
        vector<vector<Point3f>> cornersObjPoints = cornersPoints.CornersObjPoints;
        vector<vector<Point2f>> cornersImgPoints = cornersPoints.CornersImgPoints;

        //perform camera calibration from detecting corners
        Size imageSize = imgs[0].size();

        Mat intrinsicCameraMatrix = Mat::eye(3, 3, CV_64F);
        Mat distortionMatrix = Mat::zeros(8, 1, CV_64F);
        vector<Mat> rotationMatrix;
        vector<Mat> translationMatrix;

        double rms;

        if (fixAspectRatio)
        {
            rms = calibrateCamera(cornersObjPoints, cornersImgPoints, imageSize, intrinsicCameraMatrix, distortionMatrix, rotationMatrix, translationMatrix, CALIB_FIX_ASPECT_RATIO);
        }
        else
        {
            rms = calibrateCamera(cornersObjPoints, cornersImgPoints, imageSize, intrinsicCameraMatrix, distortionMatrix, rotationMatrix, translationMatrix);
        }

        double fx = intrinsicCameraMatrix.at<double>(0, 0);
        double fy = intrinsicCameraMatrix.at<double>(1, 1);
        double cx = intrinsicCameraMatrix.at<double>(0, 2);
        double cy = intrinsicCameraMatrix.at<double>(1, 2);
        double k1 = distortionMatrix.at<double>(0, 0);
        double k2 = distortionMatrix.at<double>(1, 0);
        double p1 = distortionMatrix.at<double>(2, 0);
        double p2 = distortionMatrix.at<double>(3, 0);
        double k3 = distortionMatrix.at<double>(4, 0);

        std::map<int, float> reprojectionErrorByChessboard;
        //float meanError = 0; // unused at final only calculate for internal unknown reason
        int i = 0;
        for (vector<Point3f> cornerObj : cornersObjPoints)
        {
            vector<Point2f> cornerImg;
            cv::projectPoints(cornerObj, rotationMatrix[i], translationMatrix[i], intrinsicCameraMatrix, distortionMatrix, cornerImg);
            double error = cv::norm(cornersImgPoints[i], cornerImg, cv::NORM_L2);
            //meanError += (error / cornerImg.size()); //// unused at final only calculate for internal unknown reason
            reprojectionErrorByChessboard[i] = static_cast<float>(error);
            i++;
        }

        return CalibrationParameters(intrinsicCameraMatrix, distortionMatrix, rotationMatrix, translationMatrix, rms);
    }

    namespace {
    }
}