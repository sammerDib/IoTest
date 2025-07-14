#include <Wafer.hpp>

const int Wafer::MIN_EDGE_IMAGE_FOR_FIT = 3;
const double Wafer::MAX_RMSE_FOR_SUCCESS_IN_MICRONS = 10;
const int Wafer::MAX_ALLOWED_X_SHIFT_IN_MICRONS = 5000;
const int Wafer::MAX_ALLOWED_Y_SHIFT_IN_MICRONS = 5000;
const std::string Wafer::ERR_THREE_IMAGES_NEEDED = "Three images are needed for good results";

using namespace Algorithms;

void Wafer::AddEdgeImage(EdgeImage::Pointer edgeImage, std::string const &reportPath) {
  std::string message = "[Wafer] add new EdgeImage '" + edgeImage->GetName() + "'";
  Logger::Debug(message);
  _edgeImages.push_back(edgeImage);

  if (!reportPath.empty()) {
    auto mat = edgeImage->WithWaferEdge();
    cv::imwrite(reportPath + "\\" + edgeImage->GetName() + ".png", mat);
  }

#ifndef NDEBUG
  cv::imshow("edges", edgeImage->WithWaferEdge());
  cv::waitKey();
#endif
}

void Wafer::AddNotchImage(NotchImage::Pointer notchImage, std::string const &reportPath) {
  std::string message = "[Wafer] add new NotchImage '" + notchImage->GetName() + "'";
  Logger::Debug(message);
  _notchImage = notchImage;
#ifndef NDEBUG
  cv::imshow("notch", notchImage->Mat());
  cv::waitKey();
#endif
}

Status Wafer::GetGeometricalParameters(Wafer::WaferGeometricalParameters *result, std::string const &reportPath) {
  Status status;
  if (_edgeImages.size() >= Wafer::MIN_EDGE_IMAGE_FOR_FIT) {
    std::vector<cv::Point2i> contours;
    contours = collectContoursFromEdgeImages(reportPath);
    ICircleFitter::Result fitResult;
    status = _circleFitter->Fit(contours, fitResult, this);
    if (status.code == StatusCode::OK) {
      if (std::abs(fitResult.center.x) > MAX_ALLOWED_X_SHIFT_IN_MICRONS) {
        status.code = StatusCode::BWA_FIT_FAILED;
        std::ostringstream stringStream;
        stringStream << "Fit failed (x shift [" << fitResult.center.x << "] too high)";
        status.message = stringStream.str();
        Logger::Error("[Wafer] " + status.message);
      } else if (std::abs(fitResult.center.y) > MAX_ALLOWED_Y_SHIFT_IN_MICRONS) {
        status.code = StatusCode::BWA_FIT_FAILED;
        std::ostringstream stringStream;
        stringStream << "Fit failed (y shift [" << fitResult.center.y << "] too high)";
        status.message = stringStream.str();
        Logger::Error("[Wafer] " + status.message);
      } else {
        Logger::Info("[Wafer] fit succeeded");
        result->centerShift = fitResult.center;
        result->radius = fitResult.radius;
      }
    }
  } else {
    status.code = StatusCode::MISSING_EDGE_IMAGE;
    status.message = ERR_THREE_IMAGES_NEEDED;
    Logger::Error("[Wafer] " + ERR_THREE_IMAGES_NEEDED);
  }
  return status;
}

std::vector<cv::Point> Wafer::collectContoursFromEdgeImages(std::string const &reportPath) {
  std::vector<cv::Point> result;
  for (auto const &image : _edgeImages) {
    auto contour = image->GetShiftedContourPoints(reportPath);
    result.insert(result.end(), contour.begin(), contour.end());
  }
  return result;
}

Status Wafer::GetMisalignmentAngle(double &angle, cv::Point2d waferCenter, std::string const &reportPath) {
  Status status;
  if (_notchImage) {
    status = _notchImage->GetRotation(angle, waferCenter, reportPath);
  } else {
    status.code = StatusCode::MISSING_NOTCH_IMAGE;
    status.message = "No notch image, cannot compute rotation misalignment";
  }
  return status;
}