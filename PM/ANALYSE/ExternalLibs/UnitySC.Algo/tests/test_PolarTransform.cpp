#include <exception>
#include <gtest/gtest.h>
#include <iostream>
#include <opencv2/opencv.hpp>
#include <random>

#include <EdgeImage.hpp>
#include <Point.hpp>
#include <PolarTransform.hpp>
#include <WaferImageGenerator.hpp>

namespace {

  struct GenerationParam {
    cv::Point2d shift;
    double radius;
    double angle;
    cv::Point2d pixelSize;
  };

  WaferImageGenerator::Parameters GetParametersForSmallWafer() {
    WaferImageGenerator::Parameters params;
    params.pixelSize = cv::Point2d(1, 1);
    params.waferShift = cv::Point2d(0, 0);
    params.imageSize = cv::Point2i(1024, 1024);
    params.waferRadius = 300;
    params.waferRotationAngle = 0;
    params.notchRadius = 40;

    auto waferIntensityGenerator = [](cv::Point const &location) { return 255; };
    params.waferPixelIntensityGenerator = std::make_shared<WaferImageGenerator::Parameters::PixelGenerator>(waferIntensityGenerator);

    auto chuckIntensityGenerator = [](cv::Point const &location) { return 20; };
    params.chuckPixelIntensityGenerator = std::make_shared<WaferImageGenerator::Parameters::PixelGenerator>(chuckIntensityGenerator);

    cv::Point2i centroidAtZero(0, 0);
    params.imagesToTake.emplace_back("center", WaferImageGenerator::ImageType::EDGE, centroidAtZero);

    return params;
  }

  WaferImageGenerator::Parameters GetParametersFor300mm2x() {
    WaferImageGenerator::Parameters params;
    params.pixelSize = cv::Point2d(5.3, 5.3);
    params.waferShift = cv::Point2d(0, 0);
    params.imageSize = cv::Point2i(1164, 872);
    params.waferRadius = 150000;
    params.waferRotationAngle = 0;
    params.notchRadius = 3000;
    params.imagesToTake.emplace_back("0,-150000 notch", WaferImageGenerator::ImageType::NOTCH, cv::Point2i(0, -150000));
    return params;
  }
} // namespace
TEST(PolarTransform, Validate_ChuckToImage_without_scale) {

  EdgeImage::Pointer image;
  EdgeImage::PixelSize pixelSize(cv::Point2d(1, 1));
  EdgeImage::ImageCentroid centroid(cv::Point2d(-15, -5));
  image = EdgeImage::New("dummy", cv::Mat(cv::Size(40, 40), CV_8U), pixelSize, centroid);

  cv::Point2d pointOnChuck(-25, 5);

  // when
  cv::Point2d pointInImage;
  bool ignored = PolarTransform::ChuckToImageReferential(image.get(), pointOnChuck, pointInImage);

  // then
  cv::Point2d expectedCoordInImage(9.5, 9.5);
  EXPECT_EQ(expectedCoordInImage.x, pointInImage.x);
  EXPECT_EQ(expectedCoordInImage.y, pointInImage.y);
}
TEST(PolarTransform, Validate_ChuckToImage_with_scale) {

  EdgeImage::Pointer image;
  EdgeImage::PixelSize pixelSize(cv::Point2d(2, 2));
  EdgeImage::ImageCentroid centroid(cv::Point2d(-30, -10));
  image = EdgeImage::New("dummy", cv::Mat(cv::Size(40, 40), CV_8U), pixelSize, centroid);

  cv::Point2d pointOnChuck(-50, 10);

  // when
  cv::Point2d pointInImage;
  bool ignored = PolarTransform::ChuckToImageReferential(image.get(), pointOnChuck, pointInImage);

  // then
  cv::Point2d expectedCoordInImage(9.5, 9.5);
  EXPECT_EQ(expectedCoordInImage.x, pointInImage.x);
  EXPECT_EQ(expectedCoordInImage.y, pointInImage.y);
}

TEST(PolarTransform, Expect_ChuckToImage_to_translate_points_in_the_image) {

  EdgeImage::Pointer image;
  EdgeImage::PixelSize pixelSize(cv::Point2d(1, 1));
  EdgeImage::ImageCentroid centroid(cv::Point2d(-30, -10));
  image = EdgeImage::New("dummy", cv::Mat(cv::Size(40, 30), CV_8U), pixelSize, centroid);

  std::vector<cv::Point> pointsInsideTheImage;
  pointsInsideTheImage.push_back({-30, -10}); //  centroid

  for (auto const &outsidePoint : pointsInsideTheImage) {
    // when
    cv::Point2d pointInImage;
    bool isInTheImage = PolarTransform::ChuckToImageReferential(image.get(), outsidePoint, pointInImage);

    // then
    EXPECT_EQ(true, isInTheImage) << "Point " << outsidePoint << " is INSIDE the image!";
  }
}

TEST(PolarTransform, Expect_ChuckToImage_to_detect_out_of_bound_point) {

  EdgeImage::Pointer image;
  EdgeImage::PixelSize pixelSize(cv::Point2d(1, 1));
  EdgeImage::ImageCentroid centroid(cv::Point2d(-30, -10));
  image = EdgeImage::New("dummy", cv::Mat(cv::Size(40, 30), CV_8U), pixelSize, centroid);

  std::vector<cv::Point> pointsOutsideImage;
  pointsOutsideImage.push_back({-60, 10});  // top left
  pointsOutsideImage.push_back({-5, 10});   // top right
  pointsOutsideImage.push_back({0, -25});   // bottom right
  pointsOutsideImage.push_back({-60, -25}); //  bottom left

  // image corners are considered outside
  pointsOutsideImage.push_back({-50, 5});   // top left corner
  pointsOutsideImage.push_back({-10, 5});   // top right corner
  pointsOutsideImage.push_back({-10, -25}); // bottom right corner
  pointsOutsideImage.push_back({-50, -25}); //  bottom left corner

  for (auto const &outsidePoint : pointsOutsideImage) {
    // when
    cv::Point2d pointInImage;
    bool isInTheImage = PolarTransform::ChuckToImageReferential(image.get(), outsidePoint, pointInImage);

    // then
    EXPECT_EQ(false, isInTheImage) << "Point " << outsidePoint << " is OUTSIDE the image!";
  }
}
