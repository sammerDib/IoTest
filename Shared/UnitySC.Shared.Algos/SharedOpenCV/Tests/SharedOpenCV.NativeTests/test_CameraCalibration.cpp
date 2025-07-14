#include <opencv2/opencv.hpp>
#include <chrono>
#include <filesystem>
#include <iostream>
#include <fstream>

#include "CppUnitTest.h"
#include "CameraCalibration.hpp"
#include "ReportingUtils.hpp"
#include "CImageTypeConvertor.hpp"
#include "CShapeFinder.hpp"
#include "CDistortionCalibration.hpp"
#include "CShapeFinder.hpp"
#include "fouriertransform.hpp"
#include "RadonTransform.hpp"
#include "CRegistration.hpp"

using namespace std;
using namespace cv;
using namespace Microsoft::VisualStudio::CppUnitTestFramework;

#pragma unmanaged
namespace SharedOpenCVNativeTests
{
    TEST_CLASS(CameraCalibrationTest)
    {
    protected:

        std::vector<cv::Point2f> FindCircles(const cv::Mat& image, float gridCircleDiameterInMicrons, float gridPeriodicityInMicrons, float pixelSizeInMicrons)
        {
            float expectedCircleDiameter = (gridCircleDiameterInMicrons / pixelSizeInMicrons) * 5.0f;
            float minDistBetweenTwoCircles = (gridPeriodicityInMicrons / pixelSizeInMicrons) / 2.0f;
            int cannyThreshold = 100;
            float detectionTolerance = 80.0f;
            auto parameters = shape_finder::CircleFinderParams(minDistBetweenTwoCircles, expectedCircleDiameter,
                detectionTolerance, cannyThreshold);
            std::vector<shape_finder::Circle> circles = shape_finder::CircleFinder(image, parameters);

            std::vector<cv::Point2f> centroids;
            for (shape_finder::Circle circle : circles) {
                centroids.push_back(circle.CenterPos);
            }

            return centroids;
        }

        cv::Mat applyDistortion(const cv::Mat& image, const std::vector<double>& distortionParams) {
            cv::Mat distortedImage;

            int height = image.rows;
            int width = image.cols;
            double center_x = width / 2.0;
            double center_y = height / 2.0;

            cv::Mat map_x(height, width, CV_32F);
            cv::Mat map_y(height, width, CV_32F);

            cv::Mat mask = cv::Mat::zeros(image.size(), CV_8U);

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    double dx = x - center_x;
                    double dy = y - center_y;
                    double r = std::sqrt(dx * dx + dy * dy);

                    double r_distorted = r * (1 + distortionParams[0] * r + distortionParams[1] * r * r);

                    double theta = std::atan2(dy, dx);

                    double x_distorted = center_x + r_distorted * std::cos(theta);
                    double y_distorted = center_y + r_distorted * std::sin(theta);

                    if (x_distorted >= 0 && x_distorted < width && y_distorted >= 0 && y_distorted < height) {
                        map_x.at<float>(y, x) = static_cast<float>(x_distorted);
                        map_y.at<float>(y, x) = static_cast<float>(y_distorted);
                    }
                    else {
                        mask.at<uchar>(y, x) = 255;
                    }
                }
            }

            cv::remap(image, distortedImage, map_x, map_y, cv::INTER_LINEAR, cv::BORDER_CONSTANT, cv::Scalar(128));

            distortedImage.setTo(cv::Scalar(128, 128, 128), mask);

            return distortedImage;
        }

        cv::Mat generateImage(int size, int circleSize, int period, std::vector<cv::Point2f>& circlePositions) {
            cv::Mat image(size, size, CV_8UC3, cv::Scalar(255, 255, 255));

            circlePositions.clear();

            for (int i = period; i < size; i += period) {
                for (int j = period; j < size; j += period) {
                    cv::circle(image, cv::Point(i, j), circleSize, cv::Scalar(0, 0, 0), -1);
                    circlePositions.push_back(cv::Point2f((float)i, (float)j));
                }
            }

            return image;
        }
        
    public:
        TEST_METHOD(undistorted_image_should_be_very_similar_to_image_before_distortion) {

            // Given
            std::vector<cv::Point2f> circlePositions;

            int imageSize = 1000;
            int circleSize = 20;
            int period = 100;

            float circleSizeInMicrons = (float)circleSize;
            float periodInMicrons = (float)period;
            float pixelSizeInMicrons = 1.0f;

            cv::Mat generatedImg = generateImage(imageSize, circleSize, period, circlePositions);
            std::vector<double> radialDistortionParams = { 0.0000001, 0.0000001 };
            cv::Mat distortedImg = applyDistortion(generatedImg, radialDistortionParams);

            auto coeffs = distortionCalibration::ComputeDistoMatrix(distortedImg, circleSizeInMicrons, periodInMicrons, pixelSizeInMicrons);

            cv::Mat undistortedImg = distortionCalibration::UndistortImage(distortedImg, coeffs);

            double expectedUndistortedSimilarity = 1.0;
            double similarityTolerance = 0.02;

            double distortedSimilarity = registration::ComputeSimilarity(distortedImg, undistortedImg);
            double undistortedSimilarity = registration::ComputeSimilarity(generatedImg, undistortedImg);

            Assert::IsTrue(undistortedSimilarity > distortedSimilarity);
            Assert::AreEqual(expectedUndistortedSimilarity, undistortedSimilarity, similarityTolerance);
        }
    };
}