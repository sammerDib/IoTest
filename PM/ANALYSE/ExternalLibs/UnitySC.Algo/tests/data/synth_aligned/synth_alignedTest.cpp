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
TEST(synth_aligned,
     Expect_wafer_dimension_to_be_extractible_from_synthetic_images) {

  std::vector<EdgeImage::Pointer> images;
  const double PIXEL_SIZE = 1;

  auto pixelSize = EdgeImage::PixelSize(cv::Point2d(PIXEL_SIZE, PIXEL_SIZE));
  auto imageCenter =
      EdgeImage::ImageCenter(cv::Point(263 * PIXEL_SIZE, -757 * PIXEL_SIZE));
  images.push_back(
      EdgeImage::New(TEST_DATA + "synth_aligned/bigWaferShot_X_263_Y_-757.png",
                     pixelSize, imageCenter));

  imageCenter =
      EdgeImage::ImageCenter(cv::Point(-609 * PIXEL_SIZE, -450 * PIXEL_SIZE));
  images.push_back(
      EdgeImage::New(TEST_DATA + "synth_aligned/bigWaferShot_X_-609_Y_-450.png",
                     pixelSize, imageCenter));

  imageCenter =
      EdgeImage::ImageCenter(cv::Point(634 * PIXEL_SIZE, 383 * PIXEL_SIZE));
  images.push_back(
      EdgeImage::New(TEST_DATA + "synth_aligned/bigWaferShot_X_634_Y_383.png",
                     pixelSize, imageCenter));

  Wafer wafer;

  wafer.AddEdgeImage(images.at(0));
  wafer.AddEdgeImage(images.at(1));
  wafer.AddEdgeImage(images.at(2));

  Wafer::WaferGeometricalParameters waferGeometricalParameters;
  auto status = wafer.GetGeometricalParameters(waferGeometricalParameters);
  ASSERT_EQ(StatusCode::OK, status.code);
  ASSERT_GT(status.confidence, 0.99);

  const cv::Point2d EXPECTED_SHIFT(0, 0);
  const cv::Point2d EXPECTED_SHIFT_TOL(1, 1);
  const double EXPECTED_RADIUS = 750 * PIXEL_SIZE;
  const double EXPECTED_RADIUS_TOL = 2 * PIXEL_SIZE; // 4µ of tolerance on radius  

  ASSERT_NEAR(EXPECTED_RADIUS, waferGeometricalParameters.radius,
              EXPECTED_RADIUS_TOL);

  ASSERT_NEAR(EXPECTED_SHIFT.x, waferGeometricalParameters.centerShift.x,
              EXPECTED_SHIFT_TOL.x);
  ASSERT_NEAR(EXPECTED_SHIFT.y, waferGeometricalParameters.centerShift.y,
              EXPECTED_SHIFT_TOL.y);
}
