#pragma once

#include <opencv2/opencv.hpp>

#pragma unmanaged
class Point {
public:
  /*!
   * @return polar coordinate of the given cartesian point
   *
   * NOTE: Theta coordinate is given in radians, from 0 to 2pi, counterclockwise.
   */
  static cv::Point2d CartesianToPolar(cv::Point2d const &cartesianPoint);

  static cv::Point2d PolarToCartesian(cv::Point2d const &polarPoint);

  /*!
   * @return given polar point with theta value converted from radians to degrees
   *
   * NOTE: theta is given in degrees, from 0 to 360, counterclockwise.
   */
  static cv::Point2d PolarRadianToDegree(cv::Point2d const &polarPointInRadians);
};
