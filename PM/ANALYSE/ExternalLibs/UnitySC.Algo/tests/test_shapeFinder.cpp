#include <exception>
#include <gtest/gtest.h>
#include <iostream>
#include <opencv2/opencv.hpp>
#include <stdlib.h>

#include <BaseAlgos/shapeFinder.hpp>

using namespace shape_finder;

namespace {
  class ShapeFinderTest : public ::testing::Test {};

  void drawEllipsesOnImage(cv::Mat &img, std::vector<Ellipse> ellipses) {
    for (size_t i = 0; i < ellipses.size(); ++i) {
      cv::RotatedRect rect = cv::RotatedRect(ellipses[i].CenterPos, cv::Size(ellipses[i].WidthAxis, ellipses[i].HeightAxis), ellipses[i].Angle);
      cv::circle(img, ellipses[i].CenterPos, 1, cv::Scalar(0, 255, 0), 1, cv::LINE_AA); // center
      cv::ellipse(img, rect, cv::Scalar(0, 0, 255), 2);
    }
  }

  void drawCirclesOnImage(cv::Mat &img, std::vector<Circle> circles) {
    for (size_t i = 0; i < circles.size(); i++) {
      cv::Point2f center = circles[i].CenterPos;
      cv::circle(img, center, 1, cv::Scalar(0, 255, 0), 1, cv::LINE_AA); // center
      float radius = circles[i].Diameter / 2;
      cv::circle(img, center, radius, cv::Scalar(0, 0, 255), 1, cv::LINE_AA);
    }
  }

  void drawRawContoursOnImage(cv::Mat &img, std::vector<std::vector<cv::Point>> contours) {
    for (size_t i = 0; i < contours.size(); ++i) {
      cv::drawContours(img, contours[i], -1, cv::Scalar(0, 0, 255), 1, cv::LINE_AA);
    }
  }
} // namespace

TEST_F(ShapeFinderTest, test_hough_circle_algorithm_detects_correct_number_of_circles) {
  // Given
  std::string image_path = TEST_DATA_PATH + std::string("shape/63_circles_of_70_pixels_in_diameter.png");
  cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
  int expectedCircleNb = 63;
  int expectedCircleDiameter = 70;
  float minDistBetweenTwoCircles = 10;

  // When
  float approximateDiameter = expectedCircleDiameter;
  int cannyThreshold = 100;
  float detectionTolerance = 10;
  std::vector<Circle> circles = CircleFinder(input, CircleFinderParams(minDistBetweenTwoCircles, approximateDiameter, detectionTolerance, cannyThreshold));

  // Then
  int detectedCircleNb = circles.size();
  ASSERT_EQ(expectedCircleNb, detectedCircleNb);
}

TEST_F(ShapeFinderTest, test_ellipse_contour_fitting_algorithm_detects_correct_number_of_circle) {
  // Given
  std::string image_path = TEST_DATA_PATH + std::string("shape/63_circles_of_70_pixels_in_diameter.png");
  cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
  int expectedCircleNb = 63;
  std::pair<float, float> expectedAxes = std::pair<float, float>(70, 70);
  int cannyThreshold = 100;
  float detectionTolerance = 10;

  // When
  std::vector<Ellipse> ellipses = EllipseFinder(input, EllipseFinderParams(expectedAxes, detectionTolerance, cannyThreshold));

  // Then
  int detectedCircleNb = ellipses.size();
  ASSERT_EQ(expectedCircleNb, detectedCircleNb);
}

TEST_F(ShapeFinderTest, test_ellipse_contour_fitting_algorithm_detects_correct_number_of_ellipse) {
  // Given
  std::string image_path = TEST_DATA_PATH + std::string("shape/1_centered_ellipse.png");
  cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
  int expectedEllipseNb = 1;
  std::pair<float, float> expectedAxes = std::pair<float, float>(295, 385);
  int cannyThreshold = 100;
  int detectionTolerance = 10;

  // When
  std::vector<Ellipse> ellipses = EllipseFinder(input, EllipseFinderParams(expectedAxes, detectionTolerance, cannyThreshold));

  // Then
  int detectedEllipseNb = ellipses.size();
  ASSERT_EQ(expectedEllipseNb, detectedEllipseNb);
}

TEST_F(ShapeFinderTest, test_hough_circle_algorithm_detects_circles_with_correct_diameter) {
  // Given
  std::string image_path = TEST_DATA_PATH + std::string("shape/63_circles_of_70_pixels_in_diameter.png");
  cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
  int expectedCircleDiameter = 70;
  float minDistBetweenTwoCircles = 10;
  int cannyThreshold = 100;
  float detectionTolerance = 10;

  // When
  std::vector<Circle> circles = CircleFinder(input, CircleFinderParams(minDistBetweenTwoCircles, expectedCircleDiameter, detectionTolerance, cannyThreshold));

  // Then
  for (int i = 0; i < circles.size(); i++) {
    ASSERT_NEAR(expectedCircleDiameter, circles[i].Diameter, 3);
  }
}

TEST_F(ShapeFinderTest, test_ellipse_contour_fitting_algorithm_detects_circle_with_correct_diameter) {
  // Given
  std::string image_path = TEST_DATA_PATH + std::string("shape/16_circles_of_60_pixels_in_diameter.tif");
  cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
  std::pair<float, float> expectedAxes = std::pair<float, float>(60, 60);
  int cannyThreshold = 100;
  int detectionTolerance = 10;

  // When
  std::vector<Ellipse> ellipses = EllipseFinder(input, EllipseFinderParams(expectedAxes, detectionTolerance, cannyThreshold));

  // Then
  float expectedMajorAxis = expectedAxes.first > expectedAxes.second ? expectedAxes.first : expectedAxes.second;
  float expectedMinorAxis = expectedAxes.first <= expectedAxes.second ? expectedAxes.first : expectedAxes.second;

  for (int i = 0; i < ellipses.size(); i++) {
    float majorAxis = ellipses[i].WidthAxis > ellipses[i].HeightAxis ? ellipses[i].WidthAxis : ellipses[i].HeightAxis;
    float minorAxis = ellipses[i].WidthAxis >= ellipses[i].HeightAxis ? ellipses[i].WidthAxis : ellipses[i].HeightAxis;
    ASSERT_NEAR(expectedMajorAxis, majorAxis, 3);
    ASSERT_NEAR(expectedMinorAxis, minorAxis, 3);
  }
}

TEST_F(ShapeFinderTest, test_ellipse_contour_fitting_algorithm_detects_ellipse_with_correct_diameter) {
  // Given
  std::string image_path = TEST_DATA_PATH + std::string("shape/1_centered_ellipse.png");
  cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
  int expectedEllipseNb = 1;
  std::pair<float, float> expectedAxes = std::pair<float, float>(295, 385);
  int cannyThreshold = 100;
  int detectionTolerance = 10;

  // When
  std::vector<Ellipse> ellipses = EllipseFinder(input, EllipseFinderParams(expectedAxes, detectionTolerance, cannyThreshold));

  // Then
  float expectedMajorAxis = expectedAxes.first > expectedAxes.second ? expectedAxes.first : expectedAxes.second;
  float expectedMinorAxis = expectedAxes.first <= expectedAxes.second ? expectedAxes.first : expectedAxes.second;

  for (int i = 0; i < ellipses.size(); i++) {
    float majorAxis = ellipses[i].WidthAxis > ellipses[i].HeightAxis ? ellipses[i].WidthAxis : ellipses[i].HeightAxis;
    float minorAxis = ellipses[i].WidthAxis <= ellipses[i].HeightAxis ? ellipses[i].WidthAxis : ellipses[i].HeightAxis;
    ASSERT_NEAR(expectedMajorAxis, majorAxis, 3);
    ASSERT_NEAR(expectedMinorAxis, minorAxis, 3);
  }
}

TEST_F(ShapeFinderTest, test_hough_circle_algorithm_detects_correct_center_of_circle) {
  // Given
  std::string image_path = TEST_DATA_PATH + std::string("shape/1_centered_circle.jpg");
  cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
  cv::Point expectedCenter = cv::Point(input.size().width / 2, input.size().height / 2);
  int expectedCircleNb = 1;
  int expectedCircleDiameter = 170;
  float minDistBetweenTwoCircles = 10;
  int cannyThreshold = 100;
  float detectionTolerance = 10;

  // When
  std::vector<Circle> circles = CircleFinder(input, CircleFinderParams(minDistBetweenTwoCircles, expectedCircleDiameter, detectionTolerance, cannyThreshold));

  // Then
  ASSERT_EQ(expectedCircleNb, circles.size());
  cv::Point center = circles[0].CenterPos;
  ASSERT_NEAR(expectedCenter.y, center.y, 10);
  ASSERT_NEAR(expectedCenter.x, center.x, 10);
}

TEST_F(ShapeFinderTest, test_ellipse_contour_fitting_algorithm_detects_correct_center_of_ellipse) {
  // Given
  std::string image_path = TEST_DATA_PATH + std::string("shape/1_centered_ellipse.png");
  cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
  cv::Point expectedCenter = cv::Point(input.size().width / 2, input.size().height / 2);
  int expectedEllipseNb = 1;
  std::pair<float, float> expectedAxes = std::pair<float, float>(295, 385);
  int cannyThreshold = 100;
  float detectionTolerance = 10;

  // When
  std::vector<Ellipse> ellipses = EllipseFinder(input, EllipseFinderParams(expectedAxes, detectionTolerance, cannyThreshold));

  // Then
  ASSERT_EQ(expectedEllipseNb, ellipses.size());
  cv::Point center = ellipses[0].CenterPos;
  ASSERT_NEAR(expectedCenter.y, center.y, 10);
  ASSERT_NEAR(expectedCenter.x, center.x, 10);
}