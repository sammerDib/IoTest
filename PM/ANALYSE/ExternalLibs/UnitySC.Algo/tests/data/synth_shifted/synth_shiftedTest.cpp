#include <CircleFitter.hpp>
#include <EdgeImage.hpp>
#include <Wafer.hpp>
#include <gtest/gtest.h>
#include <opencv2/core.hpp>

// _TEST_DATA_ is set trough CMake configuration
// \see CMakeLists.txt
const std::string TEST_DATA = _TEST_DATA_;

// In this test all calculus are made considering 1px == 1µm
// for the sake of simplicity
TEST(
    synth_shifted,
    Expect_wafer_dimension_to_be_extractible_from_synthetic_images_pixel_scaled) {

  const double PIXEL_SIZE = 2.111;
  auto pixelSize = EdgeImage::PixelSize(cv::Point2d(PIXEL_SIZE, PIXEL_SIZE));

  std::vector<EdgeImage::Pointer> images;
  auto imageCenter =
      EdgeImage::ImageCenter(cv::Point(263 * PIXEL_SIZE, -757 * PIXEL_SIZE));
  images.push_back(
      EdgeImage::New(TEST_DATA + "synth_shifted/bigWaferShot_X_263_Y_-757.png",
                     pixelSize, imageCenter));

  imageCenter =
      EdgeImage::ImageCenter(cv::Point(-609 * PIXEL_SIZE, -450 * PIXEL_SIZE));
  images.push_back(
      EdgeImage::New(TEST_DATA + "synth_shifted/bigWaferShot_X_-609_Y_-450.png",
                     pixelSize, imageCenter));

  imageCenter =
      EdgeImage::ImageCenter(cv::Point(634 * PIXEL_SIZE, 383 * PIXEL_SIZE));
  images.push_back(
      EdgeImage::New(TEST_DATA + "synth_shifted/bigWaferShot_X_634_Y_383.png",
                     pixelSize, imageCenter));

  Wafer wafer;
  wafer.AddEdgeImage(images.at(0));
  wafer.AddEdgeImage(images.at(1));
  wafer.AddEdgeImage(images.at(2));

  Wafer::WaferGeometricalParameters waferGeometricalParameters;
  auto status = wafer.GetGeometricalParameters(waferGeometricalParameters);

  ASSERT_EQ(StatusCode::OK, status.code);
  ASSERT_GT(status.confidence, 0.99);

  const cv::Point2d EXPECTED_SHIFT(10 * PIXEL_SIZE, -5 * PIXEL_SIZE);
  const double translationTolerance = 1;

  double translationInX = waferGeometricalParameters.centerShift.x;
  ASSERT_NEAR(EXPECTED_SHIFT.x, translationInX, translationTolerance);

  double translationInY = waferGeometricalParameters.centerShift.y;
  ASSERT_NEAR(EXPECTED_SHIFT.y, translationInY, translationTolerance);

  const double EXPECTED_RADIUS = 750 * PIXEL_SIZE;
  const double radiusTolerance = 4 * PIXEL_SIZE;
  double measuredRadius = waferGeometricalParameters.radius;
  ASSERT_NEAR(EXPECTED_RADIUS, measuredRadius, radiusTolerance);
}

TEST(synth_shifted,
     Expect_wafer_dimension_to_be_extractible_from_synthetic_images) {

  std::vector<EdgeImage::Pointer> images;

  auto pixelSize = EdgeImage::PixelSize(cv::Point2d(1, 1));
  auto imageCenter = EdgeImage::ImageCenter(cv::Point(263, -757));
  images.push_back(
      EdgeImage::New(TEST_DATA + "synth_shifted/bigWaferShot_X_263_Y_-757.png",
                     pixelSize, imageCenter));

  imageCenter = EdgeImage::ImageCenter(cv::Point(-609, -450));
  images.push_back(
      EdgeImage::New(TEST_DATA + "synth_shifted/bigWaferShot_X_-609_Y_-450.png",
                     pixelSize, imageCenter));

  imageCenter = EdgeImage::ImageCenter(cv::Point(634, 383));
  images.push_back(
      EdgeImage::New(TEST_DATA + "synth_shifted/bigWaferShot_X_634_Y_383.png",
                     pixelSize, imageCenter));

  Wafer wafer;
  wafer.AddEdgeImage(images.at(0));
  wafer.AddEdgeImage(images.at(1));
  wafer.AddEdgeImage(images.at(2));

  Wafer::WaferGeometricalParameters waferGeometricalParameters;
  auto status = wafer.GetGeometricalParameters(waferGeometricalParameters);
  ASSERT_EQ(StatusCode::OK, status.code);
  ASSERT_GT(status.confidence, 0.99);

  const cv::Point2d EXPECTED_SHIFT(10, -5);
  const cv::Point2d EXPECTED_SHIFT_TOL(1, 1);
  const double EXPECTED_RADIUS = 750;
  const double EXPECTED_RADIUS_TOL = 3;

  ASSERT_NEAR(EXPECTED_RADIUS, waferGeometricalParameters.radius,
              EXPECTED_RADIUS_TOL);

  ASSERT_NEAR(EXPECTED_SHIFT.x, waferGeometricalParameters.centerShift.x,
              EXPECTED_SHIFT_TOL.x);
  ASSERT_NEAR(EXPECTED_SHIFT.y, waferGeometricalParameters.centerShift.y,
              EXPECTED_SHIFT_TOL.y);
}
