#pragma once

#include <opencv2/features2d.hpp>
#include <opencv2/opencv.hpp>

namespace registration {
    //Default values for the angle and scale limits of the pattern rec
    static const double  _defaultAngleSigmaTolerance = 6.0;
    static const double  _defaultScaleSigmaTolerance = 0.02;
    /**
     * Compute angle and shift between two images
     *
     * @param refImg            - reference image
     * @param sensedImg         - image to align
     * @param roi               - rectangle to select the ROI on reference img
     * @param reportPath        - path to report images
     * @param angleSigmaTolerance - Tolerance sigma in degrees in which the rotation angle is considered acceptable ( angles values too divergent from the sigma will be forced to 0.0)
     * @param scaleSigmaTolerance - Tolerance sigma in which the scale is considered acceptable ( scale values too divergent from the sigma will be forced to 1.0)
     * @param dilationSize        - dilation size to use for the mask of the pattern rec (Set to 0 to not apply any dilation mask)
     * 
     * @return Angle, shift vector and confidence
     */
    std::tuple<double, cv::Point2f, double> ComputeAngleAndShift(const cv::Mat& refImg, const cv::Mat& sensedImg, const cv::Rect& roi = cv::Rect(), double angleSigmaTolerance = _defaultAngleSigmaTolerance, double scaleSigmaTolerance = _defaultScaleSigmaTolerance, int dilationSize = 0, std::string reportPath = "");

    /**
     * Compute similarity score between two images
     *
     * @param refImg              - reference image
     * @param sensedImg           - image to be compare with the reference image
     * @param roi                 - selection area (defined as a rectangle) concerned by the similarity score computation
     * @param reportPath          - path to report images
     * @param dilationSize        - dilation size to use for the mask of the pattern rec (Set to 0 to not apply any dilation mask)
     * 
     * @return Score value between 0 (no similarity) and 1 (images are same)
     */
    double ComputeSimilarity(const cv::Mat& refImg, const cv::Mat& sensedImg, const cv::Rect& roi = cv::Rect(), int dilationSize = 0, std::string reportPath = "");


    /**
     * Shift images so they are aligned and compute similarity score between them
     *
     * @param refImg              - reference image
     * @param sensedImg           - image to be compare with the reference image
     * @param shift               - detected shift to align the images
     * @param roi                 - selection area (defined as a rectangle) concerned by the similarity score computation
     * @param reportPath          - path to report images
     * @param dilationSize        - dilation size to use for the mask of the pattern rec (Set to 0 to not apply any dilation mask)
     *
     * @return Score value between 0 (no similarity) and 1 (images are same)
     */
    double ShiftImagesAndComputeSimilarity(const cv::Mat& refImg, const cv::Mat& sensedImg, cv::Point2f shift, const cv::Rect& roi = cv::Rect(), int dilationSize = 0, std::string reportPath = "");

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