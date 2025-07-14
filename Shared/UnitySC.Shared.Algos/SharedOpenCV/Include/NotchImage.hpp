
#pragma once

#include <memory>
#include <string>

#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>

#include "NamedType.hpp"
#include "Status.hpp"
#include "EdgeImage.hpp"

#pragma unmanaged
class NotchImage : public EdgeImage {

public:
  // strong types
  using ImageCenter = NamedType<cv::Point2d, EdgeImage::ImageCenterTag>;
  using PixelSize = NamedType<cv::Point2d, EdgeImage::PixelSizeTag>;
  using Pointer = std::shared_ptr<NotchImage>;

  static Pointer New(std::string imageFileName, PixelSize &pixelSize, ImageCenter const &centroid) {
    return Pointer(new NotchImage(imageFileName, pixelSize, centroid));
  }

  static Pointer New(cv::Mat image, PixelSize &pixelSize, ImageCenter const &centroid) {
    return Pointer(new NotchImage("", image, pixelSize, centroid));
  }

  static Pointer New(std::string imageName, cv::Mat image, PixelSize &pixelSize, ImageCenter const &centroid) {
    return Pointer(new NotchImage(imageName, image, pixelSize, centroid));
  }

  /*!
   * @returns rotation of wafer in radians
   */
  Algorithms::Status GetRotation(double &angle, cv::Point2d waferCenter, int waferDiameterInMm, int notchWidthInMicrons, int notchDetectionVersion, std::string const &reportPath, bwa::ReportOption reportOption = bwa::ReportOption::OverlayReport);

private:
  explicit NotchImage(std::string const &imageFileName, PixelSize const &pixelSize, ImageCenter const &centroid)
      : EdgeImage(imageFileName, pixelSize, centroid)
  {}

  explicit NotchImage(std::string const &imageName, cv::Mat image, PixelSize const &pixelSize, ImageCentroid const &centroid)
      : EdgeImage(imageName, image, pixelSize, centroid)
  {}
};
