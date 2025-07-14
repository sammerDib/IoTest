#pragma once

#include <opencv2/core/mat.hpp>

#pragma unmanaged
namespace radon_transform {
    /**
     * @brief Compute the radon transform of an image
     *
     * @param src the source matrix
     * @param dst the destination matrix
     * @theta angle resolution of the transform in degrees
     * @start_angle start angle of the transform in degrees
     * @end_angle end angle of the transform in degrees
     */
    void RadonTransform(cv::InputArray src, cv::OutputArray dst, double theta, double start_angle, double end_angle);
}