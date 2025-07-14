#include <gtest/gtest.h>

#include <NotchImage.hpp>
#include <WaferImageGenerator.hpp>
#include <iostream>
#include <memory>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>

#include <Point.hpp>

TEST(Point, Expect_radians_from_0_to_two_pi_anticlockwise) {

  // given

  // cartesian point <=> expected polar point
  using expectation = std::pair<cv::Point2i, cv::Point2d>;

  using result = std::pair<bool, cv::Point2d>;

  std::vector<expectation> given;
  std::vector<result> expectationResults;

  given.push_back(expectation(cv::Point2i(0, 0), cv::Point2d(0, 0)));
  given.push_back(expectation(cv::Point2i(10, 0), cv::Point2d(10, 0)));
  given.push_back(expectation(cv::Point2i(0, 10), cv::Point2d(10, CV_PI / 2)));
  given.push_back(expectation(cv::Point2i(-10, 0), cv::Point2d(10, CV_PI)));
  given.push_back(expectation(cv::Point2i(0, -10), cv::Point2d(10, 3.0 / 2 * CV_PI)));

  const double radiusForTenByTenSide = std::sqrt(std::pow(10, 2) + std::pow(10, 2));
  given.push_back(expectation(cv::Point2i(-10, -10), cv::Point2d(radiusForTenByTenSide, 5.0 / 4 * CV_PI)));
  given.push_back(expectation(cv::Point2i(10, -10), cv::Point2d(radiusForTenByTenSide, 7.0 / 4 * CV_PI)));

  // when
  for (auto const &test : given) {
    auto cartesianPoint = test.first;
    auto expectedPolarPoint = test.second;
    auto actual = Point::CartesianToPolar(cartesianPoint);
    expectationResults.push_back(result(actual == expectedPolarPoint, actual));
  }

  // then
  int index = 0;
  for (auto const &expectationResult : expectationResults) {
    auto expectation = given.at(index);
    auto result = expectationResult;
    EXPECT_TRUE(expectationResult.first) << "Conversion failed for point " << expectation.first << ": expected " << expectation.second << " got "
                                         << result.second;
    index++;
  }
}
TEST(Point, Expect_conversion_to_be_reversible) {

  // cartesian -> polar -> cartesian
  {
    cv::Point2d cartesian(-3084.6, -147689.2);
    cv::Point2d polar = Point::CartesianToPolar(cartesian);
    cv::Point2d reCartesian = Point::PolarToCartesian(polar);
    EXPECT_NEAR(cartesian.x, reCartesian.x, 10e-1);
    EXPECT_NEAR(cartesian.y, reCartesian.y, 10e-1);
  }

  // polar -> cartesian -> polar
  {
    cv::Point2d polar = cv::Point2d(147689.2, 4.691506);
    cv::Point2d cartesian = Point::PolarToCartesian(polar);
    cv::Point2d rePolar = Point::CartesianToPolar(cartesian);
    EXPECT_NEAR(polar.x, rePolar.x, 10e-3);
    EXPECT_NEAR(polar.y, rePolar.y, 10e-3);
  }
}

TEST(Point, Expect_radians_to_degree_to_be_right) {
  // given
  using expectation = std::pair<cv::Point2d, cv::Point2d>;
  using result = std::pair<bool, cv::Point2d>;

  std::vector<expectation> given;
  std::vector<result> expectationResults;

  given.push_back(expectation(cv::Point2d(0, 0), cv::Point2d(0, 0)));
  given.push_back(expectation(cv::Point2d(0, CV_PI / 2), cv::Point2d(0, 90)));
  given.push_back(expectation(cv::Point2d(0, CV_PI), cv::Point2d(0, 180)));
  given.push_back(expectation(cv::Point2d(0, 3.0 / 2 * CV_PI), cv::Point2d(0, 270)));
  given.push_back(expectation(cv::Point2d(0, 2.0 * CV_PI), cv::Point2d(0, 360)));

  // when
  for (auto const &test : given) {
    auto cartesianPoint = test.first;
    auto expectedPolarPoint = test.second;
    auto actual = Point::PolarRadianToDegree(cartesianPoint);
    expectationResults.push_back(result(actual == expectedPolarPoint, actual));
  }

  // then
  int index = 0;
  for (auto const &expectationResult : expectationResults) {
    auto const &expectation = given.at(index);
    auto const &result = expectationResult;
    EXPECT_TRUE(expectationResult.first) << "Conversion failed for point " << expectation.first << ": expected " << expectation.second << " got "
                                         << result.second;
    index++;
  }
}

TEST(Point, Expect_polar_point_to_be_convertible_to_cartesian) {

  // given

  //  expected polar point <=> expected cartesian point
  using expectation = std::pair<cv::Point2d, cv::Point2d>;

  // note: Point::PolarToCartesian gives double,
  // but we box to int here to avoid rounding issue.
  // BTW test values are choosen to be round.
  using result = std::pair<bool, cv::Point2i>;

  std::vector<expectation> given;
  std::vector<result> expectationResults;

  given.push_back(expectation(cv::Point2d(0, 0), cv::Point2i(0, 0)));
  given.push_back(expectation(cv::Point2d(10, 0), cv::Point2i(10, 0)));
  given.push_back(expectation(cv::Point2d(10, CV_PI / 2), cv::Point2i(0, 10)));
  given.push_back(expectation(cv::Point2d(10, CV_PI), cv::Point2i(-10, 0)));
  given.push_back(expectation(cv::Point2d(10, 3.0 / 2 * CV_PI), cv::Point2i(0, -10)));

  const double radiusForTenByTenSide = std::sqrt(std::pow(10, 2) + std::pow(10, 2));
  given.push_back(expectation(cv::Point2d(radiusForTenByTenSide, 5.0 / 4 * CV_PI), cv::Point2i(-10, -10)));
  given.push_back(expectation(cv::Point2d(radiusForTenByTenSide, 7.0 / 4 * CV_PI), cv::Point2i(10, -10)));

  // when
  for (auto const &test : given) {
    auto polarPoint = test.first;
    cv::Point2i expectedCartesianPoint = test.second;
    cv::Point2i actual = Point::PolarToCartesian(polarPoint);
    expectationResults.push_back(result(actual == expectedCartesianPoint, actual));
  }

  // then
  int index = 0;
  for (auto const &expectationResult : expectationResults) {
    auto const &expectation = given.at(index);
    auto const &result = expectationResult;
    EXPECT_TRUE(expectationResult.first) << "Conversion failed for point " << expectation.first << ": expected " << expectation.second << " got "
                                         << result.second;
    index++;
  }
}
