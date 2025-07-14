#pragma once

#pragma unmanaged

#include <opencv2/opencv.hpp>
#include <opencv2/core/types.hpp>

namespace utils {

    /**
     * Create a rectangular mask using the given dimensions
     * 
     * @param maskHeight            - source image height
     * @param maskWidth             - source image width
     * @param roi                   - rectangular region of interest to create the mask
     * 
     * @return a mask image of given dimensions
     */
    cv::Mat CreateMaskForImage(int maskHeight, int maskWidth, cv::Rect roi);

    /**
     * Create a circular mask using the given dimensions
     *
     * @param maskHeight            - source image height
     * @param maskWidth             - source image width
     * @param center                - point used as the circle's center
     * @param radius                - circle radius for the mask
     *
     * @return a mask image of given dimensions
     */
    cv::Mat CreateMaskForImage(int maskHeight, int maskWidth, cv::Point2i center, int radius);
}