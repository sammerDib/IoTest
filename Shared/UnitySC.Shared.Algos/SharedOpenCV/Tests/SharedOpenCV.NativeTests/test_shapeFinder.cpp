#include "CppUnitTest.h"

#include <exception>
#include <iostream>
#include <opencv2/opencv.hpp>
#include <stdlib.h>

#include "CShapeFinder.hpp"

using namespace shape_finder;
#pragma unmanaged

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

#define TEST_DATA_PATH std::string(".\\..\\..\\Tests\\Data\\")

namespace SharedOpenCVNativeTests
{
    TEST_CLASS(ShapeFinderTest)
    {
    public:
        void drawEllipsesOnImage(cv::Mat& img, std::vector<Ellipse> ellipses) {
            for (size_t i = 0; i < ellipses.size(); ++i) {
                cv::RotatedRect rect = cv::RotatedRect(ellipses[i].CenterPos, cv::Size((int)ellipses[i].WidthAxis, (int)ellipses[i].HeightAxis), ellipses[i].Angle);
                cv::circle(img, ellipses[i].CenterPos, 1, cv::Scalar(0, 255, 0), 1, cv::LINE_AA); // center
                cv::ellipse(img, rect, cv::Scalar(0, 0, 255), 2);
            }
        }

        void drawCirclesOnImage(cv::Mat& img, std::vector<Circle> circles) {
            for (size_t i = 0; i < circles.size(); i++) {
                cv::Point2f center = circles[i].CenterPos;
                cv::circle(img, center, 1, cv::Scalar(0, 255, 0), 1, cv::LINE_AA); // center
                float radius = circles[i].Diameter * 0.5f;
                cv::circle(img, center, (int)radius, cv::Scalar(0, 0, 255), 1, cv::LINE_AA);
            }
        }

        void drawRawContoursOnImage(cv::Mat& img, std::vector<std::vector<cv::Point>> contours) {
            for (size_t i = 0; i < contours.size(); ++i) {
                cv::drawContours(img, contours[i], -1, cv::Scalar(0, 0, 255), 1, cv::LINE_AA);
            }
        }

        TEST_METHOD(_01_test_hough_circle_algorithm_detects_correct_number_of_circles)
        {
            Logger::WriteMessage("test_hough_circle_algorithm_detects_correct_number_of_circles \n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("63_circles_of_70_pixels_in_diameter.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            Assert::IsFalse(input.empty());
            int expectedCircleNb = 63;
            float expectedCircleDiameter = 70.0f;
            float minDistBetweenTwoCircles = 70.0f;

            // When
            float approximateDiameter = expectedCircleDiameter;
            int cannyThreshold = 100;
            float detectionTolerance = 10.0f;
            Logger::WriteMessage("perform CircleFinder\n");
            std::vector<Circle> circles = CircleFinder(input, CircleFinderParams(minDistBetweenTwoCircles, approximateDiameter, detectionTolerance, cannyThreshold, false, true));

            // Then
            int detectedCircleNb = (int) circles.size();
            Assert::AreEqual(expectedCircleNb, detectedCircleNb, L"Nb Circles not reached");
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_01_test_hough_circle_alternative_algorithm_detects_correct_number_of_circles)
        {
            Logger::WriteMessage("test_hough_circle_algorithm_detects_correct_number_of_circles \n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("63_circles_of_70_pixels_in_diameter.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            Assert::IsFalse(input.empty());
            int expectedCircleNb = 63;
            float expectedCircleDiameter = 70.0f;
            float minDistBetweenTwoCircles = 70.0f;

            // When
            float approximateDiameter = expectedCircleDiameter;
            int cannyThreshold = 300;
            float detectionTolerance = 10.0f;
            Logger::WriteMessage("perform CircleFinder\n");
            std::vector<Circle> circles = CircleFinder(input, CircleFinderParams(minDistBetweenTwoCircles, approximateDiameter, detectionTolerance, cannyThreshold, true, true));

            // Then
            int detectedCircleNb = (int)circles.size();
            Assert::AreEqual(expectedCircleNb, detectedCircleNb, L"Nb Circles not reached");
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_02_test_hough_circle_algorithm_detects_correct_number_of_circles)
        {
            Logger::WriteMessage("test_hough_circle_alternative_algorithm_detects_correct_number_of_circles \n");

            // Given
            std::string image_path = TEST_DATA_PATH + std::string("16_circles_of_60_pixels_in_diameter.tif");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            Assert::IsFalse(input.empty());
            int expectedCircleNb = 16;
            float expectedCircleDiameter = 60.0f;
            float minDistBetweenTwoCircles = 60.0f;

            // When
            float approximateDiameter = expectedCircleDiameter;
            int cannyThreshold = 100;
            float detectionTolerance = 10.0f;
            Logger::WriteMessage("perform CircleFinder\n");
            std::vector<Circle> circles = CircleFinder(input, CircleFinderParams(minDistBetweenTwoCircles, approximateDiameter, detectionTolerance, cannyThreshold, false, true));

            // Then
            int detectedCircleNb = (int) circles.size();
            Assert::AreEqual(expectedCircleNb, detectedCircleNb, L"Nb Circles not reached");
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_02_test_hough_circle_alternative_algorithm_detects_correct_number_of_circles)
        {
            Logger::WriteMessage("test_hough_circle_alternative_algorithm_detects_correct_number_of_circles \n");

            // Given
            std::string image_path = TEST_DATA_PATH + std::string("16_circles_of_60_pixels_in_diameter.tif");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            Assert::IsFalse(input.empty());
            int expectedCircleNb = 16;
            float expectedCircleDiameter = 60.0f;
            float minDistBetweenTwoCircles = 60.0f;

            // When
            float approximateDiameter = expectedCircleDiameter;
            int cannyThreshold = 300;
            float detectionTolerance = 10.0f;
            Logger::WriteMessage("perform CircleFinder\n");
            std::vector<Circle> circles = CircleFinder(input, CircleFinderParams(minDistBetweenTwoCircles, approximateDiameter, detectionTolerance, cannyThreshold, true, true));

            // Then
            int detectedCircleNb = (int) circles.size();
            Assert::AreEqual(expectedCircleNb, detectedCircleNb, L"Nb Circles not reached");
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_03_test_hough_circle_algorithm_detects_correct_number_of_circles)
        {
            Logger::WriteMessage("test_hough_circle_algorithm_detects_correct_number_of_circles \n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("4_circles_of_30_pixels_in_diameter.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            Assert::IsFalse(input.empty());
            int expectedCircleNb = 5;
            float expectedCircleDiameter = 30.0f;
            float minDistBetweenTwoCircles = 30.f;

            // When
            float approximateDiameter = expectedCircleDiameter;
            int cannyThreshold = 100;
            float detectionTolerance = 10.f;
            Logger::WriteMessage("perform CircleFinder\n");
            std::vector<Circle> circles = CircleFinder(input, CircleFinderParams(minDistBetweenTwoCircles, approximateDiameter, detectionTolerance, cannyThreshold, false, true));

            // Then
            int detectedCircleNb = (int)circles.size();
            Assert::AreEqual(expectedCircleNb, detectedCircleNb, L"Nb Circles not reached");
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_03_test_hough_circle_alternative_algorithm_detects_correct_number_of_circles)
        {
            Logger::WriteMessage("test_hough_circle_algorithm_detects_correct_number_of_circles \n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("4_circles_of_30_pixels_in_diameter.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            Assert::IsFalse(input.empty());
            int expectedCircleNb = 5;
            float expectedCircleDiameter = 30.f;
            float minDistBetweenTwoCircles = 30.f;

            // When
            float approximateDiameter = expectedCircleDiameter;
            int cannyThreshold = 300;
            float detectionTolerance = 10.f;
            Logger::WriteMessage("perform CircleFinder\n");
            std::vector<Circle> circles = CircleFinder(input, CircleFinderParams(minDistBetweenTwoCircles, approximateDiameter, detectionTolerance, cannyThreshold, true, true));

            // Then
            int detectedCircleNb = (int) circles.size();
            Assert::AreEqual(expectedCircleNb, detectedCircleNb, L"Nb Circles not reached");
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_04_test_hough_circle_alternative_algorithm_detects_correct_number_of_circles_on_noisy_image)
        {
            Logger::WriteMessage("test_hough_circle_alternative_algorithm_detects_correct_number_of_circles \n");

            // Given
            std::string image_path = TEST_DATA_PATH + std::string("CD_noisy_wafer_case.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            Assert::IsFalse(input.empty());
            int expectedCircleNb = 54;
            float expectedCircleDiameter = 20.f;
            float minDistBetweenTwoCircles = 20.f;

            // When
            float approximateDiameter = expectedCircleDiameter;
            int cannyThreshold = 100;
            float detectionTolerance = 10.f;
            Logger::WriteMessage("perform CircleFinder\n");
            std::vector<Circle> circles = CircleFinder(input, CircleFinderParams(minDistBetweenTwoCircles, approximateDiameter, detectionTolerance, cannyThreshold, true, true));

            // Then
            int detectedCircleNb = (int) circles.size();
            Assert::AreEqual(expectedCircleNb, detectedCircleNb, L"Nb Circles not reached");
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_05_test_ellipse_contour_fitting_algorithm_detects_correct_number_of_circle)
        {
            Logger::WriteMessage("test_ellipse_contour_fitting_algorithm_detects_correct_number_of_circle \n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("63_circles_of_70_pixels_in_diameter.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            Assert::IsFalse(input.empty());

            int expectedCircleNb = 63;
            std::pair<float, float> expectedAxes = std::pair<float, float>(70.f, 70.f);
            int cannyThreshold = 100;
            float majorAxisTolerance = 10.f;
            float minorAxisTolerance = 10.f;

            // When
            Logger::WriteMessage("perform EllipseFinder\n");
            std::vector<Ellipse> ellipses = EllipseFinder(input, EllipseFinderParams(expectedAxes, majorAxisTolerance, minorAxisTolerance, cannyThreshold));

            // Then
            int detectedCircleNb = (int) ellipses.size();
            Assert::AreEqual(expectedCircleNb, detectedCircleNb, L"Nb Circle not reached");
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_06_test_ellipse_contour_fitting_algorithm_detects_correct_number_of_ellipse)
        {
            Logger::WriteMessage("test_ellipse_contour_fitting_algorithm_detects_correct_number_of_ellipse \n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("1_centered_ellipse.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            Assert::IsFalse(input.empty());

            int expectedEllipseNb = 1;
            std::pair<float, float> expectedAxes = std::pair<float, float>(295.f, 385.f);
            int cannyThreshold = 100;
            float majorAxisTolerance = 10.f;
            float minorAxisTolerance = 10.f;

            // When
            Logger::WriteMessage("perform EllipseFinder\n");
            std::vector<Ellipse> ellipses = EllipseFinder(input, EllipseFinderParams(expectedAxes, majorAxisTolerance, minorAxisTolerance, cannyThreshold));

            // Then
            int detectedEllipseNb = (int) ellipses.size();
            Assert::AreEqual(expectedEllipseNb, detectedEllipseNb, L"Nb Ellispe not reached");
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_07_test_hough_circle_algorithm_detects_circles_with_correct_diameter)
        {
            Logger::WriteMessage("test_hough_circle_algorithm_detects_circles_with_correct_diameter \n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("63_circles_of_70_pixels_in_diameter.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            Assert::IsFalse(input.empty());

            float expectedCircleDiameter = 70.f;
            float minDistBetweenTwoCircles = 10.f;
            int cannyThreshold = 100;
            float detectionTolerance = 10.f;

            // When
            Logger::WriteMessage("perform CircleFinder\n");
            std::vector<Circle> circles = CircleFinder(input, CircleFinderParams(minDistBetweenTwoCircles, expectedCircleDiameter, detectionTolerance, cannyThreshold));

            // Then
            char buf[255];
            for (int i = 0; i < circles.size(); i++) {
                _snprintf_s(buf, 255, "[%d] Circle Diameter =  %lf  - (expected = %lf)\n", i, (double)circles[i].Diameter, expectedCircleDiameter);
                Logger::WriteMessage(buf);
                Assert::AreEqual((float)expectedCircleDiameter, circles[i].Diameter, 3.0f);
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_08_test_ellipse_contour_fitting_algorithm_detects_circle_with_correct_diameter)
        {
            Logger::WriteMessage("test_ellipse_contour_fitting_algorithm_detects_circle_with_correct_diameter \n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("16_circles_of_60_pixels_in_diameter.tif");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            Assert::IsFalse(input.empty());

            std::pair<float, float> expectedAxes = std::pair<float, float>(60.f, 60.f);
            int cannyThreshold = 100;
            float majorAxisTolerance = 10.f;
            float minorAxisTolerance = 10.f;

            // When
            Logger::WriteMessage("perform EllipseFinder\n");
            std::vector<Ellipse> ellipses = EllipseFinder(input, EllipseFinderParams(expectedAxes, majorAxisTolerance, minorAxisTolerance, cannyThreshold));

            // Then
            float expectedMajorAxis = expectedAxes.first > expectedAxes.second ? expectedAxes.first : expectedAxes.second;
            float expectedMinorAxis = expectedAxes.first <= expectedAxes.second ? expectedAxes.first : expectedAxes.second;
            char buf[255];
            for (int i = 0; i < ellipses.size(); i++) {
                float majorAxis = ellipses[i].WidthAxis > ellipses[i].HeightAxis ? ellipses[i].WidthAxis : ellipses[i].HeightAxis;
                float minorAxis = ellipses[i].WidthAxis >= ellipses[i].HeightAxis ? ellipses[i].WidthAxis : ellipses[i].HeightAxis;

                _snprintf_s(buf, 255, "[%d] Major Axis =  %lf  - (expected = %lf)\n", i, majorAxis, expectedMajorAxis);
                Logger::WriteMessage(buf);
                Assert::AreEqual(expectedMajorAxis, majorAxis, 3.0f);

                _snprintf_s(buf, 255, "[%d] Minor Axis =  %lf  - (expected = %lf)\n", i, minorAxis, expectedMinorAxis);
                Logger::WriteMessage(buf);
                Assert::AreEqual(expectedMinorAxis, minorAxis, 3.0f);
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_09_test_ellipse_contour_fitting_algorithm_detects_ellipse_with_correct_diameter)
        {
            Logger::WriteMessage("test_ellipse_contour_fitting_algorithm_detects_ellipse_with_correct_diameter \n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("1_centered_ellipse.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            Assert::IsFalse(input.empty());

            int expectedEllipseNb = 1;
            std::pair<float, float> expectedAxes = std::pair<float, float>(295.f, 385.f);
            int cannyThreshold = 100;
            float majorAxisTolerance = 10.f;
            float minorAxisTolerance = 10.f;

            // When
            Logger::WriteMessage("perform EllipseFinder\n");
            std::vector<Ellipse> ellipses = EllipseFinder(input, EllipseFinderParams(expectedAxes, majorAxisTolerance, minorAxisTolerance, cannyThreshold));

            // Then
            float expectedMajorAxis = expectedAxes.first > expectedAxes.second ? expectedAxes.first : expectedAxes.second;
            float expectedMinorAxis = expectedAxes.first <= expectedAxes.second ? expectedAxes.first : expectedAxes.second;
            char buf[255];
            for (int i = 0; i < ellipses.size(); i++) {
                float majorAxis = ellipses[i].WidthAxis > ellipses[i].HeightAxis ? ellipses[i].WidthAxis : ellipses[i].HeightAxis;
                float minorAxis = ellipses[i].WidthAxis <= ellipses[i].HeightAxis ? ellipses[i].WidthAxis : ellipses[i].HeightAxis;
                _snprintf_s(buf, 255, "[%d] Major Axis =  %lf  - (expected = %lf)\n", i, majorAxis, expectedMajorAxis);
                Logger::WriteMessage(buf);
                Assert::AreEqual(expectedMajorAxis, majorAxis, 3.0f);

                _snprintf_s(buf, 255, "[%d] Minor Axis =  %lf  - (expected = %lf)\n", i, minorAxis, expectedMinorAxis);
                Logger::WriteMessage(buf);
                Assert::AreEqual(expectedMinorAxis, minorAxis, 3.0f);
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_10_test_hough_circle_algorithm_detects_correct_center_of_circle)
        {
            Logger::WriteMessage("test_hough_circle_algorithm_detects_correct_center_of_circle\n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("1_centered_circle.jpg");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            Assert::IsFalse(input.empty());

            cv::Point expectedCenter = cv::Point(input.size().width / 2, input.size().height / 2);
            int expectedCircleNb = 1;
            float expectedCircleDiameter = 170.f;
            float minDistBetweenTwoCircles = 10.f;
            int cannyThreshold = 100;
            float detectionTolerance = 10;

            // When
            Logger::WriteMessage("perform CircleFinder\n");
            std::vector<Circle> circles = CircleFinder(input, CircleFinderParams(minDistBetweenTwoCircles, expectedCircleDiameter, detectionTolerance, cannyThreshold));

            // Then
            Assert::AreEqual(expectedCircleNb, (int)circles.size(), L"Bad Nb Circle");
            cv::Point center = circles[0].CenterPos;
            Assert::AreEqual((float)expectedCenter.y, (float)center.y, 10.0f, L"Bad Y center");
            Assert::AreEqual((float)expectedCenter.x, (float)center.x, 10.0f, L"Bad X center");
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_11_test_ellipse_contour_fitting_algorithm_detects_correct_center_of_ellipse)
        {
            Logger::WriteMessage("test_ellipse_contour_fitting_algorithm_detects_correct_center_of_ellipse \n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("1_centered_ellipse.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            Assert::IsFalse(input.empty());

            cv::Point expectedCenter = cv::Point(input.size().width / 2, input.size().height / 2);
            int expectedEllipseNb = 1;
            std::pair<float, float> expectedAxes = std::pair<float, float>(295.f, 385.f);
            int cannyThreshold = 100;
            float majorAxisTolerance = 10.f;
            float minorAxisTolerance = 10.f;

            // When
            Logger::WriteMessage("perform EllipseFinder\n");
            std::vector<Ellipse> ellipses = EllipseFinder(input, EllipseFinderParams(expectedAxes, majorAxisTolerance, minorAxisTolerance, cannyThreshold));

            // Then
            Assert::AreEqual(expectedEllipseNb, (int)ellipses.size(), L"Bad Nb Ellispe");
            cv::Point center = ellipses[0].CenterPos;
            Assert::AreEqual((float)expectedCenter.y, (float)center.y, 10.0f, L"Bad Y center");
            Assert::AreEqual((float)expectedCenter.x, (float)center.x, 10.0f, L"Bad X center");
            Logger::WriteMessage("Done\n");
        }
    };
}