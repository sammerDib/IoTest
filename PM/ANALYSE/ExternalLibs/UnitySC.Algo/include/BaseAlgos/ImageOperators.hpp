#pragma once

#include <opencv2/opencv.hpp>

namespace img_operators {
  /**
   * Focus measurement based on Laplacian variance.
   * Based on second order derivative operator to calculate  the  edge  value  gain.
   *
   * @param img     - image on which to calculate the focus measure
   *
   * @return Focus measure
   */
  double VarianceOfLaplacian(const cv::Mat &img);

  /**
   * Focus measurement based on an alternative Laplacian definition.
   * Based on second order derivative operator to calculate  the  edge  value  gain.
   * Reference : [1989, Nayar] Shape from Focus
   *
   * @param img     - image on which to calculate the focus measure
   *
   * @return Focus measure
   */
  double ModifiedLaplacian(const cv::Mat &img);

  /**
   * Focus measurement based on Tenenbaum gradient (Tenengrad).
   * Based on derivative operator to calculate  the  edge  value  gain.
   * Reference : [2000, Pech-Pacheco et al.’s] Diatom autofocusing in brightfield microscopy: a comparative study
   *
   * @param img         - image on which to calculate the focus measure
   * @param kernelSize  - size of the kernel
   *
   * @return Focus measure
   */
  double TenenbaumGradient(const cv::Mat &img, int kernelSize = 5);

  /**
   * Contrast measurement based on normalized gray level variance.
   * Based on the assumption that the perceptibility of objects in the scene increases
   * with the difference in brightness between objects and their background.
   *
   * @param img     - image on which to calculate the contrast measure
   *
   * @return Contrast measure
   */
  double NormalizedGraylevelVariance(const cv::Mat &img);

  /**
   * Saturation measurement based on Hue, Saturation, Lightness representation of image,
   * saturation being the average of the lightness channel of this image.
   *
   * @param img     - image on which to calculate the saturation measure
   *
   * @return Saturation measure
   */
  double Saturation(const cv::Mat &img);
} // namespace img_operators