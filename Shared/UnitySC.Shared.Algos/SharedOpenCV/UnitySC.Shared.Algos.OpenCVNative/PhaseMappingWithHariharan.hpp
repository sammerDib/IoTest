#pragma once

#include <opencv2/opencv.hpp>

#include "WrappedPhaseMap.hpp"

#pragma unmanaged
namespace hariharan {
    /*
     * Phase mapping algorithm based on phase-shifted interferograms by pi / 2 at each step.
     * Be careful, there must be the same number of interferograms per step and the number of steps must be between 3 and 7 !
     *
     * @param interferometricImgs     - The interferometric images shifted by pi/2 at each step
     * @param stepNb                  - Number of step
     *
     * @return the wrapped phase map
     */
    WrappedPhaseMap HariharanPhaseMapping(std::vector<cv::Mat> interferometricImgs, int stepNb);
} // namespace hariharan