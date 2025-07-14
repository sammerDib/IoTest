#pragma once

#include <opencv2/opencv.hpp>

#pragma unmanaged

namespace psd {
    /**
     * Represents camera intrinsic parameters
     */
    struct CalibrationParameters {
        CalibrationParameters(cv::Mat intrinsic, cv::Mat distortion, std::vector<cv::Mat> rotation, std::vector<cv::Mat> translation, double rms) {
            CameraIntrinsic = intrinsic;
            Distortion = distortion;
            Rotation = rotation;
            Translation = translation;
            RMS = rms;
        }

        /**
         * 3x3 floating-point camera intrinsic matrix =
         *      fx 0  cx
         *      0  fy cy
         *      0  0  1
         */
        cv::Mat CameraIntrinsic;

        /**
         * vector of distortion coefficients of :
         *  4 elements: k1, k2, p1, p2
         *  or 5 elements: k1, k2, p1, p2, k3
         *  or 8 elements: k1, k2, p1, p2, k3, k4, k5, k6
         *  or 12 elements: k1, k2, p1, p2, k3, k4, k5, k6, s1, s2, s3, s4
         *  or 14 elements:  k1, k2, p1, p2, k3, k4, k5, k6, s1, s2, s3, s4, tx, ty
         */
        cv::Mat Distortion;

        /**
        * vector of matrices containing in the i-th position the rotation vector for the i-th object point to the i-th image point.
        */
        std::vector<cv::Mat> Rotation;

        /**
        * vector of matrices containing in the i-th position the translation vector for the i-th object point to the i-th image point.
        */
        std::vector<cv::Mat> Translation;

        /**
        * average re-projection error
        */
        double RMS;
    };
}