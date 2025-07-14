#pragma once

#include <opencv2/opencv.hpp>

#pragma unmanaged

/**
 * Represents a wrapped phase map with it's wrapped phase and it's background
 */
struct WrappedPhaseMap {
    WrappedPhaseMap() {}

    WrappedPhaseMap(cv::Mat wrappedPhase, cv::Mat amplitude, cv::Mat background) {
        Phase = wrappedPhase;
        Amplitude = amplitude;
        Background = background;
    }

    WrappedPhaseMap(cv::Mat wrappedPhase, cv::Mat wrappedPhase2, cv::Mat amplitude, cv::Mat background, cv::Mat dark) {
        Phase = wrappedPhase;
        Phase2 = wrappedPhase2;
        Amplitude = amplitude;
        Background = background;
        Dark = dark;
    }

    cv::Mat Phase; // phase map of the wavefront, wrapped between [-pi, pi]
    cv::Mat Phase2; // phase map of the wavefront, wrapped between [0, 2pi]
    cv::Mat Amplitude; // amplitude of the sine curve
    cv::Mat Background;
    cv::Mat Dark;
};