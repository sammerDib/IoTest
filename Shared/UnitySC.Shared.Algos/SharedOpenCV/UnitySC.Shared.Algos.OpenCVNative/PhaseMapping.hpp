#pragma once

#include <opencv2/opencv.hpp>

#include "WrappedPhaseMap.hpp"

#pragma unmanaged
namespace phase_mapping {
    /*
     * Phase mapping algorithm
     * Be careful, there must be the same number of interferograms per step and the number of steps must be between 3 and 7 !
     *
     * @param interferometricImgs       - The interferometric images shifted by pi/2 at each step
     * @param stepNb                    - Number of step
     * @param removeResidualFringes     - Use method to remove residual fringes to compute the wrapped phase map. Be careful, this method takes more time !
     *
     * @return the wrapped phase map
     */
    WrappedPhaseMap PhaseMapping(std::vector<cv::Mat> interferometricImgs, int stepNb, bool removeResidualFringes = false);
} // namespace phase_mapping