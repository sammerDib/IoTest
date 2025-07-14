#pragma once
#include <opencv2/core.hpp>

#pragma unmanaged
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
  static cv::Mat Crop(cv::Mat &image, int waferDiameterInMm, cv::Rect *boundingRect = nullptr);

  /*
     * After the polar transform, the image will be more or less warped depending on the wafer diameter
     * The more warped the image, the more black / empty pixels will appear (300mm = little to no warp, 200mm = way more warp)
     * Therefore : for each different wafer diameter, a different percentage of non empty pixels for a line / row to be considered good and not be cropped must be used
     */
  static const double GOOD_NON_EMPTY_PIXEL_PERCENTAGE_300MM;
  static const double GOOD_NON_EMPTY_PIXEL_PERCENTAGE_200MM;
  static const double GOOD_NON_EMPTY_PIXEL_PERCENTAGE_150MM;
  static const double GOOD_NON_EMPTY_PIXEL_PERCENTAGE_100MM;
};