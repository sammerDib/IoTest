
#if defined(USE_MATHPLOTLIB)
#include <matplotlibcpp.h>
namespace plt = matplotlibcpp;
#endif

#include <NotchImage.hpp>
#include <Point.hpp>
#include <PolarTransform.hpp>
#include <SymmetryDetector.hpp>

#include <iostream>
#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>

namespace {
  static constexpr double ThreePiOnTwo = 3 * CV_PI / 2;
} // namespace

Algorithms::Status NotchImage::GetRotation(double &angle, cv::Point2d waferCenter, std::string const &reportPath) {

  SymmetryDetector sd;
  Algorithms::Status status;
  double rawAngle = sd.Detect(this, waferCenter, reportPath);
  if (!std::isnan(rawAngle)) {
    // The detected angle will be around 3/2 * PI (270°), where notch lies.
    // To extract the rotation, we have to shift angle
    angle = rawAngle - ThreePiOnTwo;
    status.confidence = 1;
    status.code = Algorithms::StatusCode::OK;
  } else {
    status.code = Algorithms::StatusCode::UNKNOWN_ERROR;
    status.message = "Unable to compute the wafer rotation";
    status.confidence = 0;
  }
  return status;
}
