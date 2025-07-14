#pragma once

#include <opencv2/opencv.hpp>

namespace signal_2D {
  /**
   * Calculate the tilt of the plane from a sample 2D depth map
   * DepthMap = ax + by + c
   *
   * @param depthMap     - 2D depth map tilted
   *
   * @return Plane representing the tilt of the depth map
   */
  cv::Mat SolvePlaneEquation(cv::Mat depthMap);

  /**
   * Apodization, being defined as the multiplication of a sinc function by some window,
   * corresponds in the Fourier domain to a convolution-based construction.
   * The Hanning window is one out of several attempts to design a window that has favorable properties in the Fourier domain.
   *
   * @param src - Source image
   *
   * @return Filtered image
   */
  cv::Mat HanningApodization(const cv::Mat &src);

  /**
   * Calcul the Discret Fourier Transform on input image and compute the associated fourier spectrum.
   *
   * @param img - Source image
   *
   * @return Fourier spectrum
   */
  cv::Mat DiscretFourierTransform(const cv::Mat &img);
} // namespace signal_2D