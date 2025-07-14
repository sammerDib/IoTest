#pragma once

#include <opencv2/opencv.hpp>
#pragma unmanaged

namespace phase_unwrapping {
    /**
     * Two-dimensional phase unwrapping based on multiperiod phase unwrapping algorithm.
     *
     * @param wrappedPhaseMaps     - The phase map of type CV_32FC1 wrapped between [-pi, pi], at each period
     * @param mask                 - Binary mask of same dimension than phases images to select wafer area in witch calculate the unwrapping
     * @param periods              - Period associated at each phase map
     * @param nbPeriod             - Number of different periods
     *
     * @return the nwrapped phase map, stored in CV_32FC1 Mat
     */
    cv::Mat MultiperiodUnwrap(std::vector<cv::Mat>& wrappedPhaseMaps, cv::Mat mask, std::vector<int> periods, int nbPeriod);
    /**
     * Substract the global wafer plane from an unwrapped phase map
     *
     * @param unwrappedPhase       - The phase map to substract the plane from
     * @param mask                 - Binary mask of same dimension than the phase map to select wafer area in which to substract the plane from
     *
     * @return the wrapped phase map, with the plane substracted
     */
    void SubstractPlaneFromUnwrapped(cv::Mat& unwrappedPhase, cv::Mat mask);
} // namespace phase_unwrapping