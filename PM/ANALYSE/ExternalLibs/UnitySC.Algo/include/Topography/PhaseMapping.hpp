#pragma once

#include <opencv2/opencv.hpp>

namespace phase_mapping {
    /*
     * Average the interferograms corresponding to the same step, to have only one average interferogram for each steps by pi / 2 at each step
     *
     * @param interferometricImgs     - The interferometric images
     * @param stepsNb                 - Number of steps (phase-shift)
     *
     * @return the list of interferograms, containing the average interferogram for each step.
     */
    std::vector<cv::Mat> AverageImgs(std::vector<cv::Mat> interferometricImgs, int stepsNb);
    /*
     * Phase mapping algorithm based on phase-shifted interferograms by pi / 2 at each step
     *
     * @param interferometricImgs     - The interferometric images shifted by pi/2 at each step
     * @param phaseMap                - The 2D wrapped phase map computed by this algorithm
     * @param intensityMap            - The 2D intensity map associated at the phase map, computed by this algorithm
     * @param backgroundMap           - The 2D background associated at the phase map, computed by this algorithm
     *
     * @return the phase map, wrapped between [-pi, pi] and stored in CV_32FC1 Mat.
     */
    cv::Mat PhaseMapping(std::vector<cv::Mat> interferometricImgs, cv::Mat& phaseMap, cv::Mat& intensityMap, cv::Mat& backgroundMap);
} // namespace phase_mapping