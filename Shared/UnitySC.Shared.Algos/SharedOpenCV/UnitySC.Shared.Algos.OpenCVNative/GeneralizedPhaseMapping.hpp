#pragma once

#include <opencv2/opencv.hpp>

#include "WrappedPhaseMap.hpp"

#pragma unmanaged
namespace generalized_phase_mapping {
    enum Precision {
        High,
        Low,
    };
    /*
     * Although not fundamentally new, this generalized algorithm bring together many existing methods and
     * removes many of the restrictions that had previously applied to the selection of the data collection scheme.
     * This approch requires a series of sample interferograms I(x,y) recorded at different phase shifts chosen
     * in a particular manner, namely : phase shift (i) = i * 2PI/N with N = number of step and i = 1,...,N
     *
     * Based on "Generalized data reduction for heterodyne interferometry", Optical Engineering 23(4), 350-352 (July/August 1984)
     *
     * @param interferometricImgs     - The interferometric images recorded with different phase shifts
     * @param stepNb                  - Number of step
     * @param precision               - Bits in computer memory used to compute and store result (Low precision used uchar and High precision used float)
     *
     * @return the wrapped phase map
     */
    WrappedPhaseMap GeneralizedPhaseMapping(std::vector<cv::Mat> interferometricImgs, int stepNb, Precision precision);
} // namespace generalized_phase_mapping