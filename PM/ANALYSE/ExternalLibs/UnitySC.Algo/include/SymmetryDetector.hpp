#pragma once

#include <EdgeImage.hpp>
#include <opencv2/core.hpp>

class SymmetryDetector {
public:
  /*!
   * Given an image, this function will try to find a symmetry axis crossing that rotation pole and the image
   *
   * @param waferCenter coordinates, micrometers (ie, the previously detected x/y shift)
   * @return a angle in radians
   */
  static double Detect(EdgeImage *const image, cv::Point2d waferCenter, std::string const &reportPath);

  /*!
   * To detect symetry, we use a padded polar projection of a source image. The symetry axis can be deduced
   * using an OpenCV's cross-correlation shift, cropped source region and padding parameters
   *
   * @param correlation ordinate peak coordinate from OpenCV
   * @param histograms the angles distribution. As many rows as the source image polar projection have, where
   *                                       values spread from minimal to maximal angles.
   * @param croppedRegion position and dimension of cropped polar image in the complete polar projection
   */
  static double ComputeAngleFromCorrelation(double correlation, std::vector<double> histograms, cv::Size const &croppedPaddedPolar, cv::Size const &polar, cv::Rect const &croppedRegion);

  /*!
   * To ensure OpenCV cross correlation has enough usable data
   * to provide good result, ImagePadder will repeat the first image line
   * at image bottom and the last image line at image top, until we obtain
   * an image with two times more rows.
   */
  class ImagePadder {
  public:
    static cv::Mat PadImage(cv::Mat sourceImage) {

      cv::Mat paddedShifted;
      double padHeight = sourceImage.rows / 2.0;

      // 1. create image with top and bottom padding, each 1/2 the height
      //     of original image. Data of first line is used for top padding, last
      //     line is used for bottom padding.
      cv::copyMakeBorder(sourceImage, paddedShifted, padHeight, padHeight, 0, 0, cv::BORDER_REPLICATE | cv::BORDER_ISOLATED);

      // 2. Invert top and bottom padding
      cv::Mat topbanner = paddedShifted(cv::Rect(0, 0, sourceImage.cols, padHeight));
      cv::Mat bottomBanner = paddedShifted(cv::Rect(0, padHeight + sourceImage.rows, sourceImage.cols, padHeight));
      cv::Mat tmpTopBanner;
      topbanner.copyTo(tmpTopBanner);
      bottomBanner.copyTo(topbanner);
      tmpTopBanner.copyTo(bottomBanner);

      return paddedShifted;
    }
  };
};
