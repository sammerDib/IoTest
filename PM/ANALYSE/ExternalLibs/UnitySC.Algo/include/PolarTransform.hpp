#pragma once

#include <EdgeImage.hpp>
#include <opencv2/core.hpp>
#include <vector>

class PolarTransform {
public:
  static constexpr int DISCRETIZED_ANGLES_COUNT = 1024;
  static constexpr int DISCRETIZED_RADIUS_COUNT = 1024;

  struct Histograms {
    std::vector<double> angles;
    std::vector<double> radius;
  };

  /*!
   * Compute a polar projection of given src image into dst image
   *
   * Note: Polar pole used for tranformation is the 0,0 coordinate point. Distance to that point is obtained using
   * the provided image centroid.
   *
   *
   * In the polar projection, radius are on the x axis, angles on the y axis to match OpenCV way of work.
   *
   *  @param image                      - image to project
   *  @param targetPolarImage  - where to store projected image
   *  @param waferCenter           - Real wafer center position, relative to chuck referential 0,0
   *
   */
  static Histograms Transform(EdgeImage *const image, cv::Mat &targetPolarImage, cv::Point2d waferCenter);

  /*!
   *
   * Compute a cartesian projection of given polar image into a cartesian image image
   *
   * In the polar projection, radius are on the x axis, angles on the y axis to match OpenCV way of work.
   *
   *  @param polarImage          - image to project
   *  @param cartesianImage   - where to store projected image
   *  @param histograms           - angle and radius histograms for given polar image
   * @param pixelSize                - pixel size in micrometers
   */
  static void BackTransform(cv::Mat &polarImage, cv::Mat &cartesianImage, Histograms histograms, cv::Point2d pixelSize);

  /*
   * Compute sub-pixel conversion and return discrete nearest neighbour
   *
   * @param image the edgeImage where point lies
   * @param pointInChuckReferential the point to translate to image referential
   * @param image the image where the point may lie
   * @param pointInImageReferential the point in the image, if its in
   * @return true if the given point is in the image, false otherwise
   */
  static bool ChuckToImageReferential(EdgeImage *image, cv::Point2d const &pointInChuckReferential, cv::Point2d &pointInImageReferential);
};
