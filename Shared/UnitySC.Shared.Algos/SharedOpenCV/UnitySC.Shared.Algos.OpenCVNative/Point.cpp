#include <opencv2/opencv.hpp>

#include "Point.hpp"

#pragma unmanaged
namespace {
  /*
   * @return theta coordinate of the point, in radians
   *
   */
  static double ComputeTheta(cv::Point2d const &cartesianPoint) {
    double radians = std::atan2(cartesianPoint.y, cartesianPoint.x);
    if (radians < 0) {
      radians += 2 * CV_PI;
    }
    return radians;
  }
} // namespace

/*!
 * @return polar coordinate of the given cartesian point.
 *
 * NOTE: Theta coordinate is given in radians
 */
cv::Point2d Point::CartesianToPolar(cv::Point2d const &cartesianPoint) {

  cv::Point2d polarCoords;
  polarCoords.x = cv::norm(cartesianPoint);     // r
  polarCoords.y = ComputeTheta(cartesianPoint); // theta
  return polarCoords;
}
cv::Point2d Point::PolarRadianToDegree(cv::Point2d const &polarPointInRadians) {

  cv::Point2d result(polarPointInRadians);
  result.y *= 180 / CV_PI;
  return result;
}

cv::Point2d Point::PolarToCartesian(cv::Point2d const &polarPoint) {
  cv::Point2d result{0, 0};
  result.x = polarPoint.x * std::cos(polarPoint.y);
  result.y = polarPoint.x * std::sin(polarPoint.y);
  return result;
}
