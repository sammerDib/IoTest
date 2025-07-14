#include <HyperAccurateCircleFitter.hpp>
#include <Wafer.hpp>

#include <gtest/gtest.h>

using namespace Algorithms;
TEST(CircleFitter, Expect_fit_to_fail_with_zero_points) {
  std::vector<cv::Point> points;
  ICircleFitter::Result result;
  HyperAccurateCircleFitter fitter;
  auto status = fitter.Fit(points, result, nullptr);
  ASSERT_EQ(StatusCode::BWA_FIT_FAILED, status.code);
}

TEST(CircleFitter, Expect_fit_to_return_nan_with_zero_points) {
  std::vector<cv::Point> points;
  ICircleFitter::Result result;
  HyperAccurateCircleFitter fitter;
  auto status = fitter.Fit(points, result, nullptr);
  ASSERT_TRUE(std::isnan(result.radius));
  ASSERT_TRUE(std::isnan(result.center.x));
  ASSERT_TRUE(std::isnan(result.center.y));
}

TEST(CircleFitter, Expect_confidence_value_to_be_realistic) {

  struct GivenType {
    double rmse;
    double expectedConfidence;
  };

  std::vector<GivenType> givens;

  // lower boundary value
  givens.push_back(GivenType({0, 1}));

  // 1px is close to perfection. Less cannot be garanteed anyway
  givens.push_back(GivenType({1, 0.9999999999}));

  // median value
  givens.push_back(GivenType({1500, 0.5}));

  // upper boundary value
  givens.push_back(GivenType({3000, 0}));

  auto data = Wafer::WaferData();
  data.radiusInMicrons = 150000;
  data.type = Wafer::WaferData::Type::NOTCH;
  Wafer wafer(data);

  for (auto const given : givens) {
    ICircleFitter::Result result;
    HyperAccurateCircleFitter fitter;
    ICircleFitter::Result fit;
    fit.rmse = given.rmse;
    auto status = fitter.BuildStatus(fit, &wafer);
    EXPECT_NEAR(given.expectedConfidence, status.confidence, 10e-3)
        << "Failed for RMSE " << given.rmse << ", expected " << given.expectedConfidence << " got " << status.confidence;
  }
}

TEST(CircleFitter, Expect_hyper_convergence_with_three_points) {

  HyperAccurateCircleFitter::Result expected;
  expected.center = cv::Point2d(75, 75);
  expected.radius = 50;
  const double HALF_PIXEL_ERROR = 0.5;
  cv::Point2d pixelSize(1, 1);

  std::vector<cv::Point> points;
  points.push_back(cv::Point2d(107, 37));
  points.push_back(cv::Point2d(105, 114));
  points.push_back(cv::Point2d(36, 106));

  auto data = Wafer::WaferData();
  data.radiusInMicrons = 150000;
  data.type = Wafer::WaferData::Type::NOTCH;
  Wafer wafer(data);

  HyperAccurateCircleFitter fitter;
  ICircleFitter::Result result;
  auto actual = fitter.Fit(points, result, &wafer);

  std::cout << "hyperFit center/radius:" << result.center << "/" << result.radius << "/" << result.rmse << std::endl;

  EXPECT_NEAR(expected.center.x, result.center.x, HALF_PIXEL_ERROR);
  EXPECT_NEAR(expected.center.y, result.center.y, HALF_PIXEL_ERROR);
  EXPECT_NEAR(expected.radius, result.radius, HALF_PIXEL_ERROR);
}

// a circle of radius 15, shifted 1,1 to upper right corner
TEST(CircleFitter, Expect_hyper_convergence_with_three_wafer_points) {

  HyperAccurateCircleFitter::Result expected;
  expected.center = cv::Point2d(1, 1);
  expected.radius = 15;
  const double HALF_PIXEL_ERROR = 0.5;
  cv::Point2d pixelSize(1, 1);

  std::vector<cv::Point> points;
  points.push_back(cv::Point2d(1, 16));
  points.push_back(cv::Point2d(16, 0));
  points.push_back(cv::Point2d(1, -14));

  auto data = Wafer::WaferData();
  data.radiusInMicrons = 150000;
  data.type = Wafer::WaferData::Type::NOTCH;
  Wafer wafer(data);

  HyperAccurateCircleFitter fitter;
  ICircleFitter::Result result;
  auto actual = fitter.Fit(points, result, &wafer);

  std::cout << "hyperFit center/radius:" << result.center << "/" << result.radius << "/" << result.rmse << std::endl;

  EXPECT_NEAR(expected.center.x, result.center.x, HALF_PIXEL_ERROR);
  EXPECT_NEAR(expected.center.y, result.center.y, HALF_PIXEL_ERROR);
  EXPECT_NEAR(expected.radius, result.radius, HALF_PIXEL_ERROR);
}

TEST(CircleFitter, Expect_hyper_convergence_with_three_points_on_same_half) {

  HyperAccurateCircleFitter::Result expected;
  expected.center = cv::Point2d(1, 1);
  expected.radius = 15;
  const double HALF_PIXEL_ERROR = 0.5;
  cv::Point2d pixelSize(1, 1);

  std::vector<cv::Point> points;
  points.push_back(cv::Point2d(1, 16)); // top
  points.push_back(cv::Point2d(16, 1)); // right

  points.push_back(cv::Point2d(1, -14));
  points.push_back(cv::Point2d(-8, -11));

  auto data = Wafer::WaferData();
  data.radiusInMicrons = 150000;
  data.type = Wafer::WaferData::Type::NOTCH;
  Wafer wafer(data);

  HyperAccurateCircleFitter fitter;
  ICircleFitter::Result result;
  auto actual = fitter.Fit(points, result, &wafer);

  EXPECT_TRUE(result.success) << "Fit must succeed (failed: " << result.message << ")";

  std::cout << "hyperFit center/radius:" << result.center << "/" << result.radius << "/" << result.rmse << std::endl;

  EXPECT_NEAR(expected.center.x, result.center.x, HALF_PIXEL_ERROR);
  EXPECT_NEAR(expected.center.y, result.center.y, HALF_PIXEL_ERROR);
  EXPECT_NEAR(expected.radius, result.radius, HALF_PIXEL_ERROR);
}
