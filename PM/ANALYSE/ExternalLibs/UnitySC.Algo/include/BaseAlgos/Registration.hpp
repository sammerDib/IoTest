#pragma once

#include <opencv2/features2d.hpp>
#include <opencv2/opencv.hpp>

namespace registration {
    /**
     * Compute angle and shift between two images
     *
     * @param refImg            - reference image
     * @param sensedImg         - image to align
     * @param roi               - rectangle to select the ROI on reference img
     *
     * @return Angle and shift vector
     */
    std::pair<double, cv::Point2f> ComputeAngleAndShift(const cv::Mat& refImg, const cv::Mat& sensedImg, const cv::Rect& roi = cv::Rect());

    /**
     * Compute similarity score between two images
     *
     * @param refImg              - reference image
     * @param sensedImg           - image to be compare with the reference image
     * @param roi                 - selection area (defined as a rectangle) concerned by the similarity score computation
     *
     * @return Score value between 0 (no similarity) and 1 (images are same)
     */
    double ComputeSimilarity(const cv::Mat& refImg, const cv::Mat& sensedImg, const cv::Rect& roi = cv::Rect());

    /**
     * Align an image according to a transformation matrix
     *
     * @param refImg              - reference image
     * @param sensedImg           - image to align
     * @param transformation      - transformation matrix
     *
     * @return Sensed image registered according to the transform matrix used
     */
    cv::Mat ImgRegistration(const cv::Mat& refImg, const cv::Mat& sensedImg, const cv::Mat& transformation);
} // namespace registration