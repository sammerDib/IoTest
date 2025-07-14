#pragma once

#include <opencv2/opencv.hpp>

namespace phase_unwrapping {
    /*
     * Describes technique used to compute the quality map and unwrap phase
     */
    enum class QualityMode {
        Gradient,
        Variance,
        PseudoCorrelation
    };

    /*
     * Describes the index and quality of a pixel
     */
    struct QualityPixel {
        QualityPixel(int row, int col, float quality) : row(row), col(col), quality(quality) {}
        int row;
        int col;
        float quality;

        bool operator==(const QualityPixel& rpixel) const { return quality == rpixel.quality; };
        bool operator>=(const QualityPixel& rpixel) const { return quality >= rpixel.quality; };
        bool operator<=(const QualityPixel& rpixel) const { return quality <= rpixel.quality; };
        bool operator>(const QualityPixel& rpixel) const { return quality > rpixel.quality; };
        bool operator<(const QualityPixel& rpixel) const { return quality < rpixel.quality; };
    };

    /**
     * Two-dimensional phase unwrapping based on 2D quality map unwrapping algorithms.
     *
     * @param wrappedPhaseMap       - The phase map of type CV_32FC1 wrapped between [-pi, pi]
     * @param mode                  - Technique used to compute the quality map
     *
     * @return the unwrapped phase map, stored in CV_32FC1 Mat.
     */
    cv::Mat QualityGuidedUnwrap(cv::Mat& wrappedPhaseMap, QualityMode mode);
} // namespace phase_unwrapping