
#include <gtest/gtest.h>

#include <EdgeImage.hpp>
#include <Linspace.hpp>
#include <Logger.hpp>
#include <PolarTransform.hpp>
#include <SymmetryDetector.hpp>
#include <WaferImageGenerator.hpp>

#include <filesystem>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>

// logging
#include <EventQueue.hpp>

#include <random>

namespace {
  EdgeImage::Pointer GenerateNotchImage(double angle, cv::Point2d waferShift) {
    WaferImageGenerator::Parameters params;
    params.pixelSize = cv::Point2d(5.3, 5.3);

    params.waferShift = waferShift;
    params.imageSize = cv::Point2i(1164, 872);
    params.waferRadius = 150000;
    params.waferRotationAngle = angle;
    params.notchRadius = 2500;
    params.imagesToTake.emplace_back("notch", WaferImageGenerator::ImageType::NOTCH, cv::Point2i(0, -150000));

    auto waferIntensityGenerator = [](cv::Point const &location) {
      uchar intensity = cv::theRNG().uniform(190, 255);
      return intensity;
    };
    params.waferPixelIntensityGenerator = std::make_shared<WaferImageGenerator::Parameters::PixelGenerator>(waferIntensityGenerator);

    auto chuckIntensityGenerator = [](cv::Point const &location) {
      uchar intensity = cv::theRNG().uniform(150, 180);
      return intensity;
    };
    params.chuckPixelIntensityGenerator = std::make_shared<WaferImageGenerator::Parameters::PixelGenerator>(chuckIntensityGenerator);

    WaferImageGenerator generator;
    auto result = generator.Generate(params, false);

    return result.takenImages.at(0).ToEdgeImage();
  }

  struct GivenType {
    std::string label;
    cv::Size imageSize;
    cv::Point2d pixelSize;
    cv::Point2d imageCentroid;
  };

  struct GivenTypeForRotation : GivenType {
    cv::Point2d waferCenter;
    double expectedRotation;
  };

  EdgeImage::Pointer CreateImageFromGiven(GivenType given) {
    cv::Mat matrix(given.imageSize, CV_8U);
    EdgeImage::PixelSize pixelSize(given.pixelSize);
    EdgeImage::ImageCentroid centroid(given.imageCentroid);
    EdgeImage::Pointer image = EdgeImage::New(given.label, matrix, pixelSize, centroid);
    return image;
  }
} // namespace

TEST(SymmetryDetector, Expect_padImage_to_create_expected_image) {

  SymmetryDetector::ImagePadder padder;
  const int EXPECTED_TOP_PADDING_VALUE = 200;
  const int EXPECTED_BOTTOM_PADDING_VALUE = 100;
  const int EXPECTED_PADDED_IMAGE_ROWS = 20;

  cv::Mat source = cv::Mat::ones(cv::Size(1, 10), CV_8U);
  source.at<uchar>(0, 0) = EXPECTED_BOTTOM_PADDING_VALUE; // all bottom padding should have this value
  source.at<uchar>(9, 0) = EXPECTED_TOP_PADDING_VALUE;    // all top padding should have this value

  cv::Mat actual = padder.PadImage(source);

  EXPECT_EQ(EXPECTED_PADDED_IMAGE_ROWS, actual.rows);

  for (int rowIndex = 0; rowIndex < 5; ++rowIndex) {
    EXPECT_EQ(EXPECTED_TOP_PADDING_VALUE, actual.at<uchar>(rowIndex, 0)) << "Bad value in top padding at row " << rowIndex;
  }

  for (int rowIndex = 15; rowIndex < 20; ++rowIndex) {
    EXPECT_EQ(EXPECTED_BOTTOM_PADDING_VALUE, actual.at<uchar>(rowIndex, 0)) << "Bad value in bottom padding at row " << rowIndex;
  }
}

TEST(SymmetryDetector, Expect_ComputeAngleFromCorrelation_to_return_right_values) {
  struct GivenType {
    std::string label;
    double givenCorrelationOrdinate;
    std::vector<double> givenAngles;
    cv::Size croppedPaddedImage;
    cv::Size polarImageBeforeCrop;
    cv::Rect givenCroppedRegion;
    double expectedAngle;
  };

  std::vector<GivenType> givens;
  std::vector<double> angles = linspace<double>(4.687761, 4.729124, 1024);
  cv::Rect croppedRegion(9, 18, 1006, 995);
  cv::Size polarImageBeforeCrop(1024, 1024);
  cv::Size croppedPadded(1006, 1989);

  const int EXPECTED_ANGLE_INDEX = 246;

  givens.push_back(GivenType({"Values from metrological wafer", 502.478002, angles, croppedPadded, polarImageBeforeCrop, croppedRegion, angles[EXPECTED_ANGLE_INDEX]}));

  // we use here an croppedPadded image which have 16 rows and one column.
  // 16 rows means 8 rows of cropped image at the center, and 4 rows of padding before and after the cropped image.
  // The polar image is cropped: one line on top and one on bottom.
  angles = linspace<double>(0, 9, 10);
  polarImageBeforeCrop = cv::Size(1, 10);
  croppedRegion = cv::Rect(0, 1, 1, 8);
  croppedPadded = cv::Size(1, 16);

  //  boundaries checks
  givens.push_back(GivenType({"[Bounds] Peak before end of first padding is out of bound", -9, angles, croppedPadded, polarImageBeforeCrop, croppedRegion, std::numeric_limits<double>::quiet_NaN()}));
  givens.push_back(GivenType({"[Bounds] Peak at end of first padding is out of bound", -8, angles, croppedPadded, polarImageBeforeCrop, croppedRegion, std::numeric_limits<double>::quiet_NaN()}));
  givens.push_back(GivenType({"[Bounds] Peak after end of cropped image is out of bound", 14, angles, croppedPadded, polarImageBeforeCrop, croppedRegion, std::numeric_limits<double>::quiet_NaN()}));
  givens.push_back(GivenType({"[Bounds] Peak at end cropped image is out of bound", 8, angles, croppedPadded, polarImageBeforeCrop, croppedRegion, std::numeric_limits<double>::quiet_NaN()}));
  givens.push_back(GivenType({"[Value] Theorical good angle for test data", -4, angles, croppedPadded, polarImageBeforeCrop, croppedRegion, 3}));

  for (auto const &given : givens) {

    SymmetryDetector detector;
    double actualAngle = detector.ComputeAngleFromCorrelation(given.givenCorrelationOrdinate, given.givenAngles, given.croppedPaddedImage, given.polarImageBeforeCrop, given.givenCroppedRegion);

    if (std::isnan(given.expectedAngle)) {
      EXPECT_TRUE(std::isnan(actualAngle)) << "Failed for case : " << given.label;
    } else {
      EXPECT_NEAR(given.expectedAngle, actualAngle, 10e-3) << "Failed for case : " << given.label;
    }
  }
}
TEST(SymmetryDetector, Works_on_metrological_wafer_notch) {
  // given
  std::string currentPath = std::filesystem::current_path().string();
  cv::Point2d centroid(0, -150000);
  EdgeImage::PixelSize pixelSize({5.3, 5.3});
  cv::Point2d detectedShift(111 * pixelSize.get().x, 246 * pixelSize.get().y);

  EdgeImage::Pointer image = EdgeImage::New(TEST_DATA_PATH + std::string("metrological_wafer/EdgeDetection_0_2X_VIS_X_0_Y_-150000.png"), pixelSize, EdgeImage::ImageCentroid(centroid));
  assert(!image->Mat().empty());

  // when
  SymmetryDetector sd;
  double measuredSymetryAxisAngleRad = sd.Detect(image.get(), detectedShift, currentPath);

  // then
  double EXPECTED_SYMMETRY_AXIS_ANGLE_RADIANS = 4.69860247;
  EXPECT_NEAR(EXPECTED_SYMMETRY_AXIS_ANGLE_RADIANS, measuredSymetryAxisAngleRad, 10e-4);
}

TEST(SymmetryDetector, Expect_opencv_phaseCorrelate_to_detect_shift) {
  auto cat = cv::imread(TEST_DATA_PATH + std::string("cat.png"), cv::IMREAD_GRAYSCALE);
  assert(!cat.empty());

  cv::Point2d expectedShift(10, 10);
  auto cat2 = cv::imread(TEST_DATA_PATH + std::string("cat2.png"), cv::IMREAD_GRAYSCALE);
  assert(!cat2.empty());

  cat.convertTo(cat, CV_32FC1);
  cat2.convertTo(cat2, CV_32FC1);

  cv::Point2d opencvShift = cv::phaseCorrelate(cat, cat2);
  EXPECT_NEAR(expectedShift.x, opencvShift.x, 10e-2);
  EXPECT_NEAR(expectedShift.y, opencvShift.y, 10e-2);
}

#ifndef NDEBUG
TEST(SymmetryDetector, Works_on_generated_wafer_image) {

  std::string currentPath = std::filesystem::current_path().string();

  std::vector<double> angles;
  
  // test all values from 2° to -2° using steps of 0.01°
  for ( double i = 1; i >= -1; i -= 0.01) {
    angles.push_back(i);

  }

  for (auto const &angle : angles) {

#ifndef NDEBUG
    std::string message = "[TEST][SymmetryDetector] Testing angle " + std::to_string(angle);
    Logger::Debug(message);
#endif

    // Given
    auto waferShift = cv::Point2d(0, 0);
    auto generatedImage = GenerateNotchImage(angle, waferShift);

    // When
    SymmetryDetector sd;
    double measuredAngleInRadians = sd.Detect(generatedImage.get(), waferShift, currentPath);

    // Then
    // angles are returned as raw polar angles, where generator accepts a
    // shift around 270°. We have to rescale angle to cancel that 270°
    //
    // NOTE: sign of angle must be reverted regarding generation order
    //
    double EXPECTED_ANGLE_IN_RADIANS = (270 - angle) / 180.0 * CV_PI;
    EXPECT_NEAR(EXPECTED_ANGLE_IN_RADIANS, measuredAngleInRadians, 10e-3) << "Validation failed for angle (radian validation) " << angle << "°";
  }
}
#endif