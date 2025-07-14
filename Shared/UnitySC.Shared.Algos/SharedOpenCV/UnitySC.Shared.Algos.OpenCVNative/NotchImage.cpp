
#include <iostream>
#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>

#include "NotchImage.hpp"
#include "Point.hpp"
#include "PolarTransform.hpp"
#include "SymmetryDetector.hpp"

#pragma unmanaged
namespace {
  static constexpr double ThreePiOnTwo = 3 * CV_PI / 2;
} // namespace

Algorithms::Status NotchImage::GetRotation(double &angle, cv::Point2d waferCenter, int waferDiameterInMm, int notchWidthInMicrons, int notchDetectionVersion, std::string const &reportPath, bwa::ReportOption reportOption) {

  SymmetryDetector sd;
  Algorithms::Status status;
  double angleConfidence = 0.0;
  double rawAngle = std::numeric_limits<double>::quiet_NaN();

  switch (notchDetectionVersion)
  {
  case 2:
      rawAngle = sd.DetectV2(this, waferCenter, waferDiameterInMm, notchWidthInMicrons, angleConfidence, reportPath);
      break;
  case 3:
      rawAngle = sd.DetectV3(this, waferCenter, waferDiameterInMm, notchWidthInMicrons, angleConfidence, reportPath, reportOption);
      break;
  case 1:
  default:
      rawAngle = sd.Detect(this, waferCenter, waferDiameterInMm, angleConfidence, reportPath);
      break;
  }
  
  status.confidence = angleConfidence;
  if (!std::isnan(rawAngle)) {
    // The detected angle will be around 3/2 * PI (270°), where notch lies.
    // To extract the rotation, we have to shift angle
    angle = rawAngle - ThreePiOnTwo;
    status.code = Algorithms::StatusCode::OK;
  } else {
    status.code = Algorithms::StatusCode::BWA_ANGLE_FAILED;
    status.message = "Unable to compute the wafer rotation";
  }
  return status;
}
