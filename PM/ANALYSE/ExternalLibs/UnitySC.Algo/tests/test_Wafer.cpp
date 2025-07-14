#include <EdgeImage.hpp>
#include <Wafer.hpp>
#include <WaferImageGenerator.hpp>
#include <chrono>
#include <filesystem>
#include <gtest/gtest.h>
#include <opencv2/core.hpp>
#include <random>

namespace {

  class WaferGenerator {
  public:
    struct GenerationParam {

      // micrometers
      cv::Point2d shift;

      // micrometers
      double radius;

      double angle;

      // micrometers per pixel
      cv::Point2d pixelSize;

      std::string reportPath;

      GenerationParam() { reportPath = std::filesystem::current_path().string(); }
    };

    /*!
     *
     * Generate a wafer which fits in a 2000*2000 image, split in four quadrants
     *
     */
    static Wafer GetSmallGeneratedWafer(GenerationParam input) {
      WaferImageGenerator::Parameters params;

      AddPixelIntensityGenerators(params);

      params.pixelSize = input.pixelSize;

      // params.waferShift = cv::Point2d(0, 0); // shift in pixels
      params.waferShift = input.shift; // shift in pixels
      // convert shift to µm
      params.waferShift.x *= params.pixelSize.x;
      params.waferShift.y *= params.pixelSize.y;

      params.pixelSize = cv::Point2d(1, 1);
      params.imageSize = cv::Point2i(1000, 1000);

      params.waferRadius = input.radius;
      params.waferRotationAngle = input.angle;
      params.notchRadius = 80;

      params.imagesToTake.emplace_back("top", WaferImageGenerator::ImageType::EDGE, cv::Point2i(0, 500));
      params.imagesToTake.emplace_back("left", WaferImageGenerator::ImageType::EDGE, cv::Point2i(-500, 0));
      params.imagesToTake.emplace_back("right", WaferImageGenerator::ImageType::EDGE, cv::Point2i(500, -0));
      params.imagesToTake.emplace_back("notch", WaferImageGenerator::ImageType::NOTCH, cv::Point2i(0, -500));

      WaferImageGenerator generator;
      auto result = generator.Generate(params);
      auto pixelSize = EdgeImage::PixelSize(params.pixelSize);

      auto data = Wafer::WaferData();
      data.radiusInMicrons = input.radius;
      data.type = Wafer::WaferData::Type::NOTCH;

      Wafer wafer(data);

      for (auto image : result.takenImages) {
        auto imageCentroid = EdgeImage::ImageCentroid(image.centroid);

        EdgeImage::Pointer edgeImage;
        NotchImage::Pointer notchImage;
        switch (image.type) {

        case WaferImageGenerator::ImageType::EDGE:
          edgeImage = EdgeImage::New(image.label, image.content, pixelSize, imageCentroid);
          wafer.AddEdgeImage(edgeImage, input.reportPath);
          continue;

        case (WaferImageGenerator::ImageType::NOTCH):
          notchImage = NotchImage::New(image.label, image.content, pixelSize, imageCentroid);
          wafer.AddNotchImage(notchImage, input.reportPath);
          continue;
        }
      }
      return wafer;
    }

    /*!
     *
     * Generate a 300mm wafer using 2x objective.
     *
     * A pixel measure 5.3µm here
     *
     */
    static Wafer Get300mmAt2x(GenerationParam input) {

      WaferImageGenerator::Parameters params;
      params.pixelSize = input.pixelSize;
      params.waferShift.x = input.shift.x * input.pixelSize.x;
      params.waferShift.y = input.shift.y * input.pixelSize.y;

      params.imageSize = cv::Point2i(1164, 872);
      params.waferRadius = input.radius;
      params.waferRotationAngle = input.angle;
      params.notchRadius = 2000;

      // positions taken from FPMS's BWA procedure
      params.imagesToTake.emplace_back("-26047,-147721 bottom-left", WaferImageGenerator::ImageType::EDGE, cv::Point2i(-26047, -147721));
      params.imagesToTake.emplace_back("23465,148153 top-right", WaferImageGenerator::ImageType::EDGE, cv::Point2i(23465, 148153));
      params.imagesToTake.emplace_back("139077,56190 top-right-right", WaferImageGenerator::ImageType::EDGE, cv::Point2i(139077, 56190));
      params.imagesToTake.emplace_back("0,-150000 notch", WaferImageGenerator::ImageType::NOTCH, cv::Point2i(0, -150000));

      AddPixelIntensityGenerators(params);

      WaferImageGenerator generator;
      auto result = generator.Generate(params);
      auto pixelSize = EdgeImage::PixelSize(params.pixelSize);

      auto data = Wafer::WaferData();
      Wafer wafer(data);

      for (auto image : result.takenImages) {
        auto imageCentroid = EdgeImage::ImageCentroid(image.centroid);

        EdgeImage::Pointer edgeImage;
        NotchImage::Pointer notchImage;
        switch (image.type) {

        case WaferImageGenerator::ImageType::EDGE:
          edgeImage = EdgeImage::New(image.label, image.content, pixelSize, imageCentroid);
          wafer.AddEdgeImage(edgeImage, input.reportPath);
          continue;

        case (WaferImageGenerator::ImageType::NOTCH):
          notchImage = NotchImage::New(image.label, image.content, pixelSize, imageCentroid);
          wafer.AddNotchImage(notchImage, input.reportPath);
          continue;
        }
      }
      return wafer;
    }

    Wafer GetParametersForTest(GenerationParam input) {
      WaferImageGenerator::Parameters params;
      params.pixelSize = input.pixelSize;
      params.waferShift = input.shift; // shift in pixels
      params.imageSize = cv::Point2i(20, 20);
      params.waferRadius = input.radius;
      params.waferRotationAngle = input.angle;
      params.notchRadius = 2;

      // positions taken from FPMS's BWA procedure
      params.imagesToTake.emplace_back("top-right", WaferImageGenerator::ImageType::EDGE, cv::Point2i(35, 35));
      params.imagesToTake.emplace_back("bottom-left", WaferImageGenerator::ImageType::EDGE, cv::Point2i(-15, -45));

      WaferImageGenerator generator;

      auto result = generator.Generate(params);
      auto pixelSize = EdgeImage::PixelSize(params.pixelSize);

      auto data = Wafer::WaferData();
      Wafer wafer(data);
      for (auto takenImage : result.takenImages) {
        EdgeImage::Pointer edgeImage = takenImage.ToEdgeImage();
        wafer.AddEdgeImage(edgeImage, input.reportPath);
      }
      return wafer;
    }

  private:
    static void AddPixelIntensityGenerators(WaferImageGenerator::Parameters &params) {

      auto waferIntensityGenerator = [](cv::Point const &location) {
        uchar intensity = cv::theRNG().uniform(180, 255);
        return intensity;
      };
      params.waferPixelIntensityGenerator = std::make_shared<WaferImageGenerator::Parameters::PixelGenerator>(waferIntensityGenerator);

      auto chuckIntensityGenerator = [](cv::Point const &location) {
        uchar intensity = cv::theRNG().uniform(10, 30);
        return intensity;
      };
      params.chuckPixelIntensityGenerator = std::make_shared<WaferImageGenerator::Parameters::PixelGenerator>(chuckIntensityGenerator);
    }
  };
} // namespace

TEST(WaferTest, Expect_metrological_wafer_fit) {
  std::vector<EdgeImage::Pointer> images;
  auto data = Wafer::WaferData();
  Wafer wafer(data);
  auto currentPath = std::filesystem::current_path().string();
  std::string BASE = TEST_DATA_PATH "/metrological_wafer/";

  EdgeImage::PixelSize pixelSize(cv::Point2d(5.3, 5.3));
  EdgeImage::ImageCentroid imageCenter(cv::Point2i(139077, 56190));
  auto image = EdgeImage::New(BASE + std::string("EdgeDetection_3_2X_VIS_X_139077_Y_56190.png"), pixelSize, imageCenter);
  assert(!image->Mat().empty());
  wafer.AddEdgeImage(image, currentPath);

  imageCenter = EdgeImage::ImageCentroid(cv::Point2i(23465, 148153));
  image = EdgeImage::New(BASE + std::string("EdgeDetection_2_2X_VIS_X_23465_Y_148153.png"), pixelSize, imageCenter);
  assert(!image->Mat().empty());
  wafer.AddEdgeImage(image, currentPath);

  imageCenter = EdgeImage::ImageCentroid(cv::Point2i(-26047, -147721));
  image = EdgeImage::New(BASE + std::string("EdgeDetection_1_2X_VIS_X_-26047_Y_-147721.png"), pixelSize, imageCenter);
  assert(!image->Mat().empty());
  wafer.AddEdgeImage(image, currentPath);

  imageCenter = EdgeImage::ImageCentroid(cv::Point2i(0, -150000));
  NotchImage::Pointer notchImage;
  notchImage = NotchImage::New(BASE + std::string("EdgeDetection_0_2X_VIS_X_0_Y_-150000.png"), pixelSize, imageCenter);
  assert(!image->Mat().empty());
  wafer.AddNotchImage(notchImage, currentPath);

  Wafer::WaferGeometricalParameters result;
  Algorithms::Status status = wafer.GetGeometricalParameters(&result, currentPath);

  // 111 and 246 are pixel values measured manually
  const cv::Point2d expectedWaferShift = cv::Point2d(111, 246);

  // FIXME: with the black border, it should be closer to 150mm
  const double expectedRadius = 150000;

  cv::Point2d measuredCenterShift = result.centerShift;
  double measuredRadius = result.radius;

  int shiftToleranceInMicrons = 10 * pixelSize.get().x;
  double expectedShiftXInMicrons = expectedWaferShift.x * pixelSize.get().x;
  double expectedShiftYInMicrons = expectedWaferShift.y * pixelSize.get().y;

  EXPECT_NEAR(expectedShiftXInMicrons, measuredCenterShift.x, shiftToleranceInMicrons);
  EXPECT_NEAR(expectedShiftYInMicrons, measuredCenterShift.y, shiftToleranceInMicrons);

  // 260 microns over 150'000 microns of radius
  int diameterToleranceInMicrons = 260;
  EXPECT_NEAR(expectedRadius, measuredRadius, diameterToleranceInMicrons);

  // angle is -0.78, it means:
  // - notch is on the left of the image
  // - angle between wafer center and noch center is less than 270°
  const double expectedAngleInDegrees = -0.78; // -0.01378651059
  double measuredAngleInRadians;
  auto StatusB = wafer.GetMisalignmentAngle(measuredAngleInRadians, expectedWaferShift, currentPath);
  double measuredAngleInDegrees = measuredAngleInRadians / CV_PI * 180;

  // FIXME this tolerance is FAR TO HIGH
  double rotationTolerance = 0.1582;
  EXPECT_NEAR(expectedAngleInDegrees, measuredAngleInDegrees, rotationTolerance);

#ifndef NDEBUG
//  cv::waitKey();
#endif
}

TEST(WaferTest, Expect_right_result_on_small_generated_wafer) {

  WaferGenerator::GenerationParam A;
  A.shift = cv::Point2d(0, 0);
  A.radius = 500;
  A.angle = 0;
  A.pixelSize = cv::Point2d(1, 1);
  auto currentPath = std::filesystem::current_path();
  A.shift = cv::Point2d(111 / A.pixelSize.x, 246 / A.pixelSize.y);

  Wafer wafer = WaferGenerator::GetSmallGeneratedWafer(A);
  Wafer::WaferGeometricalParameters resultA;
  auto StatusA = wafer.GetGeometricalParameters(&resultA, currentPath.string());
  cv::Point2d expectedWaferShift = A.shift;
  cv::Point2d measuredCenterShift = resultA.centerShift;
  double expectedRadius = A.radius;
  double measuredRadius = resultA.radius;

  EXPECT_NEAR(expectedWaferShift.x, measuredCenterShift.x, 10e-2);
  EXPECT_NEAR(expectedWaferShift.y, measuredCenterShift.y, 10e-2);
  EXPECT_NEAR(expectedRadius, measuredRadius, 10e-1);
}

TEST(WaferTest, Expect_right_result_on_realistic_300mm_generated_wafer) {
  auto currentPath = std::filesystem::current_path();

  WaferGenerator::GenerationParam A;
  A.radius = 150000;
  A.angle = 0.78;
  A.pixelSize = cv::Point2d(5.3, 5.3);
  A.shift = cv::Point2d(111 / A.pixelSize.x, 246 / A.pixelSize.y);

  Wafer wafer = WaferGenerator::Get300mmAt2x(A);
  Wafer::WaferGeometricalParameters resultA;
  auto StatusA = wafer.GetGeometricalParameters(&resultA, currentPath.string());
  cv::Point2d expectedWaferShiftInMicrometers = A.shift;
  cv::Point2d measuredWaferShiftInPixels = resultA.centerShift;
  double expectedRadius = A.radius;
  double measuredRadius = resultA.radius;

  // 1/10e of micrometer of allowed error
  EXPECT_NEAR(expectedWaferShiftInMicrometers.x, measuredWaferShiftInPixels.x / A.pixelSize.x, 10e-1);
  EXPECT_NEAR(expectedWaferShiftInMicrometers.y, measuredWaferShiftInPixels.y / A.pixelSize.y, 10e-1);

  // 5 micrometers
  EXPECT_NEAR(expectedRadius, measuredRadius, 5);

  double measuredAngleInRadians;
  wafer.GetMisalignmentAngle(measuredAngleInRadians, A.shift, currentPath.string());
  double expectedAngleInDegrees = A.angle;
  double measuredAngleInDegrees = measuredAngleInRadians * 180 / CV_PI;

  // FIXME that tolerance is FAR TO HIGH
  double angleToleranceInDegrees = 1.611;
  EXPECT_NEAR(expectedAngleInDegrees, measuredAngleInDegrees, angleToleranceInDegrees);

#ifndef NDEBUG
  cv::waitKey();
#endif
}