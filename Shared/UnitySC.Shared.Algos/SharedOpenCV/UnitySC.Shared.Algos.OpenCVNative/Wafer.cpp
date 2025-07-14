#include "Wafer.hpp"
#include "LoggerOCV.hpp"
#include <opencv2/highgui.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/imgproc.hpp>


#pragma unmanaged

const int Wafer::MIN_EDGE_IMAGE_FOR_FIT = 3;
const double Wafer::MAX_RMSE_FOR_SUCCESS_IN_MICRONS = 10;
const int Wafer::MAX_ALLOWED_X_SHIFT_IN_MICRONS = 5000;
const int Wafer::MAX_ALLOWED_Y_SHIFT_IN_MICRONS = 5000;
const std::string Wafer::ERR_THREE_IMAGES_NEEDED = "Three images are needed for good results";

using namespace Algorithms;

void Wafer::AddEdgeImage(EdgeImage::Pointer edgeImage, std::string const &reportPath) {
  std::string message = "[Wafer] add new EdgeImage '" + edgeImage->GetName() + "'";
  LoggerOCV::Debug(message);
  _edgeImages.push_back(edgeImage);

#if !defined(NDEBUG) && defined(SHOW_IMAGES)
  int waferDiameterInMm = (_waferData.radiusInMicrons * 2) / 1000;
  cv::Mat foundEdgeImage = edgeImage->WithWaferEdge(waferDiameterInMm);
  cv::resize(foundEdgeImage, foundEdgeImage, cv::Size(), 0.80, 0.80);
  cv::imshow("edges", foundEdgeImage);
  cv::waitKey();
#endif
}

void Wafer::AddNotchImage(NotchImage::Pointer notchImage, std::string const &reportPath) {
  std::string message = "[Wafer] add new NotchImage '" + notchImage->GetName() + "'";
  LoggerOCV::Debug(message);
  _notchImage = notchImage;
#if !defined(NDEBUG) && defined(SHOW_IMAGES)
  cv::imshow("notch", notchImage->Mat());
  cv::waitKey();
#endif
}

Status Wafer::GetGeometricalParameters(Wafer::WaferGeometricalParameters* result, int edgeDetectionVersion, int cannyThreshold, std::string const& reportPath, bwa::ReportOption reportOption) {
    Status status;
    float nbOfGoodImages = 0.0f;
    float goodImageWeight = 0.8f;
    float rmseWeight = 0.2f;
    float goodImageRatio = 0.0f;
    float missingImagePenalty = 6.0f;
    if (_edgeImages.size() >= Wafer::MIN_EDGE_IMAGE_FOR_FIT) {
        std::vector<std::vector<cv::Point2d>> contours;
        for (auto const& image : _edgeImages) {
            std::vector <cv::Point2d> contour = collectContoursFromOneEdgeImage(image, edgeDetectionVersion, cannyThreshold, reportPath, reportOption);
            contours.push_back(contour);
        }

        std::vector<cv::Point2d> stitchedContours;

        for (size_t i = 0; i < contours.size(); i++)
        {
            ICircleFitter::Result fitResult;
            status = _circleFitter->Fit(contours[i], fitResult);
            if (status.code == StatusCode::OK)
            {
                stitchedContours.insert(stitchedContours.end(), contours[i].begin(), contours[i].end());
                nbOfGoodImages++;
            }
        }

        goodImageRatio = nbOfGoodImages >= 3.0f ? 1.0f : nbOfGoodImages / missingImagePenalty;

        ICircleFitter::Result fitResult;
        status = _circleFitter->Fit(stitchedContours, fitResult);

        if (status.code == StatusCode::OK) {
            if (std::abs(fitResult.center.x) > MAX_ALLOWED_X_SHIFT_IN_MICRONS) {
                status.code = StatusCode::BWA_FIT_FAILED;
                std::ostringstream stringStream;
                stringStream << "Fit failed (x shift [" << fitResult.center.x << "] too high)";
                status.message = stringStream.str();
                LoggerOCV::Error("[Wafer] " + status.message);
            }
            else if (std::abs(fitResult.center.y) > MAX_ALLOWED_Y_SHIFT_IN_MICRONS) {
                status.code = StatusCode::BWA_FIT_FAILED;
                std::ostringstream stringStream;
                stringStream << "Fit failed (y shift [" << fitResult.center.y << "] too high)";
                status.message = stringStream.str();
                LoggerOCV::Error("[Wafer] " + status.message);
            }
            else {
                LoggerOCV::Info("[Wafer] fit succeeded");
                status.confidence = goodImageRatio * goodImageWeight + status.confidence * rmseWeight;
                result->centerShift = fitResult.center;
                result->radius = fitResult.radius;
            }
        }
        if (status.code != StatusCode::OK) {
            status.confidence = 0;
        }
    }
    else {
        status.code = StatusCode::MISSING_EDGE_IMAGE;
        status.message = ERR_THREE_IMAGES_NEEDED;
        LoggerOCV::Error("[Wafer] " + ERR_THREE_IMAGES_NEEDED);
    }
    return status;
}

std::vector<cv::Point2d> Wafer::collectContoursFromOneEdgeImage(EdgeImage::Pointer image, int edgeDetectionVersion, int cannyThreshold, std::string const& reportPath, bwa::ReportOption reportOption) {
    std::vector<cv::Point2d> result;
    int waferDiameterInMm = (_waferData.radiusInMicrons * 2) / 1000;
    auto contour = image->GetShiftedContourPoints(waferDiameterInMm, edgeDetectionVersion, cannyThreshold, reportPath, reportOption);
    result.insert(result.end(), contour.begin(), contour.end());
    return result;
}

Status Wafer::GetMisalignmentAngle(double &angle, cv::Point2d waferCenter, int notchDetectionVersion, std::string const &reportPath, bwa::ReportOption reportOption) {
  Status status;
  if (_notchImage) {
    int waferDiameterInMm = (_waferData.radiusInMicrons * 2) / 1000;
    status = _notchImage->GetRotation(angle, waferCenter, waferDiameterInMm, _waferData.notchWidthInMicrons, notchDetectionVersion, reportPath, reportOption);
  } else {
    status.code = StatusCode::MISSING_NOTCH_IMAGE;
    status.message = "No notch image, cannot compute rotation misalignment";
  }
  return status;
}