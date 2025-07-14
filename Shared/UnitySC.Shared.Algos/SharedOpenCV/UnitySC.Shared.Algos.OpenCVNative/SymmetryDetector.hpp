#pragma once

#include "EdgeImage.hpp"
#include <opencv2/core.hpp>

#pragma unmanaged
class SymmetryDetector {
public:
  /*!
   * Given an image, this function will try to find a symmetry axis crossing that rotation pole and the image
   *
   * @param waferCenter coordinates, micrometers (ie, the previously detected x/y shift)
   * @param minErrorThreshold the threshold of similarity used to reject a detected symmetry axis, ranges from 0 to 1 (high values = low symmetry axes are accepted, low values = only axes with very high symmetry are accepted)
   * @param slopeThreshold the threshold used to detect big changes in intensity in the image (detect the border)
   * @return a angle in radians
   */
  static double Detect(EdgeImage* const image, cv::Point2d waferCenter, int waferDiameterInMm, double& angleConfidence, std::string const& reportPath, double minErrorThreshold = 0.1, double slopeThreshold = 1.0);
  static double DetectV2(EdgeImage *const image, cv::Point2d waferCenter, int waferDiameterInMm, int notchWidthInMicrons, double &angleConfidence, std::string const &reportPath, double minErrorThreshold = 0.30, double slopeThreshold = 1.0);
  static double DetectV3(EdgeImage* const image, cv::Point2d waferCenter, int waferDiameterInMm, int notchWidthInMicrons, double& angleConfidence, std::string const& reportPath, bwa::ReportOption reportOption = bwa::ReportOption::OverlayReport, double minErrorThreshold = 0.40, double slopeThreshold = 0.8);

  /*!
   * To detect symetry, we use a padded polar projection of a source image. The symetry axis can be deduced
   * using the mean square error between a sliding window and its flipped counterpart
   * The detected symmetry axis is then used in this function to find the angle
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
      auto padHeight = (int)((double)sourceImage.rows * 0.5);

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
