#pragma once
#include <opencv2/core.hpp>
class PolarImageCropper {

public:
  /*
   * Returns new image where all pixels have intensity value superior to zero
   * *
   * Rows and/or columns are cropped for the borders untils "full" line or "full" row is encountered.
   *
   * @returns a new image, when all non-complete border lines and rows are cropped
   * @param image the image to crop
   * @param boundingRect will contain the position and dimension of cropped image inside the original image
   */
  static cv::Mat Crop(cv::Mat &image, cv::Rect *boundingRect = nullptr);
};