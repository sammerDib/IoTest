#pragma once

#include <opencv2/opencv.hpp>

#pragma unmanaged

/*
 * Average the interferograms corresponding to the same step, to have only one average interferogram for each steps by pi / 2 at each step
 *
 * @param interferometricImgs     - The interferometric images
 * @param stepsNb                 - Number of steps (phase-shift)
 *
 * @return the list of interferograms, containing the average interferogram for each step.
 */
std::vector<cv::Mat> AverageInterferogramsPerStep(std::vector<cv::Mat>& interferometricImgs, int stepsNb);
