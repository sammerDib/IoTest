#include <gtest/gtest.h>

#include <EdgeImage.hpp>
#include <filesystem>
#include <iostream>
#include <memory>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>
#include <vector>

namespace {

  struct GivenType {
    std::string label;
    cv::Size imageSize;
    cv::Point2d pixelSize;
    cv::Point2d imageCentroid;
  };

  struct GivenTypeForOrigin : GivenType {
    cv::Point2d expectedOrigin;
  };

  EdgeImage::Pointer CreateImageFromGiven(GivenType given) {
    cv::Mat matrix(given.imageSize, CV_8U);
    EdgeImage::PixelSize pixelSize(given.pixelSize);
    EdgeImage::ImageCentroid centroid(given.imageCentroid);
    EdgeImage::Pointer image = EdgeImage::New(given.label, matrix, pixelSize, centroid);
    return image;
  }
} // namespace

TEST(EdgeImage, Expect_image_without_filename_to_have_random_32_char_name) {

  EdgeImage::PixelSize pixelSize(cv::Point2d(1, 1));
  EdgeImage::ImageCentroid centroid(cv::Point(0, 0));

  EdgeImage::Pointer image;
  image = EdgeImage::New("", pixelSize, centroid);
  ASSERT_EQ(32, image->GetName().length());

  cv::Mat dummy = cv::Mat::ones(cv::Size(0, 0), CV_8U);
  image = EdgeImage::New(dummy, pixelSize, centroid);
  ASSERT_EQ(32, image->GetName().length());
}
TEST(EdgeImage, Expect_image_generated_names_to_change_for_two_images) {

  EdgeImage::PixelSize pixelSize(cv::Point2d(1, 1));
  EdgeImage::ImageCentroid centroid(cv::Point(0, 0));

  EdgeImage::Pointer imageOne;
  imageOne = EdgeImage::New("", pixelSize, centroid);

  EdgeImage::Pointer imageTwo;
  imageTwo = EdgeImage::New("", pixelSize, centroid);

  auto imageOneName = imageOne->GetName();
  ASSERT_EQ(32, imageOneName.length());

  auto imageTwoName = imageTwo->GetName();
  ASSERT_EQ(32, imageTwoName.length());

  ASSERT_NE(imageOneName, imageTwoName) << "Two images must have different generated names";
}
TEST(EdgeImage, Expect_image_generated_names_to_remain_same_during_image_lifetime) {

  EdgeImage::PixelSize pixelSize(cv::Point2d(1, 1));
  EdgeImage::ImageCentroid centroid(cv::Point(0, 0));

  EdgeImage::Pointer image;
  image = EdgeImage::New("", pixelSize, centroid);
  auto firstCallResult = image->GetName();
  ASSERT_EQ(32, firstCallResult.length());

  auto secondCallResult = image->GetName();
  ASSERT_EQ(32, secondCallResult.length());
  ASSERT_EQ(firstCallResult, secondCallResult) << "Generated name must remain the same against multiple calls";
}

TEST(EdgeImage, Expect_image_origin_to_match_expected) {

  std::vector<GivenTypeForOrigin> givens;
  givens.emplace_back(GivenTypeForOrigin{"simple image", cv::Size(4, 4), cv::Point(1, 1), cv::Point(0, -5), cv::Point2d(-1.5, -3.5)});
  givens.emplace_back(
      GivenTypeForOrigin{"barycenter inside the image", cv::Size(1000, 1000), cv::Point(2, 2), cv::Point(0, 0), cv::Point2d(-999, 999)});
  givens.emplace_back(
      GivenTypeForOrigin{"Image far from the 0,0", cv::Size(1000, 1000), cv::Point(1, 1), cv::Point(500, 500), cv::Point2d(.5, 999.5)});
  givens.emplace_back(
      GivenTypeForOrigin{"Image far from the 0,0 with scaling", cv::Size(500, 500), cv::Point(2, 2), cv::Point(500, 500), cv::Point2d(1, 999)});

  for (auto const &given : givens) {

    EdgeImage::Pointer image = CreateImageFromGiven(given);

    // then
    cv::Point2d actualOrigin = image->GetOrigin();
    ASSERT_DOUBLE_EQ(given.expectedOrigin.x, actualOrigin.x)
        << "Image origin " << actualOrigin << " does not match expected " << given.expectedOrigin;
    ASSERT_DOUBLE_EQ(given.expectedOrigin.y, actualOrigin.y)
        << "Image origin " << actualOrigin << " does not match expected " << given.expectedOrigin;
  }
}
TEST(EdgeImage, Expect_good_coordinates_when_point_shift_to_chuck_referential) {

  cv::Mat dummy(cv::Size(6, 6), CV_8U);
  EdgeImage::PixelSize pixelSize(cv::Point2d(1, 1));
  EdgeImage::ImageCentroid centroid(cv::Point(0, 0));
  EdgeImage::Pointer image = EdgeImage::New("i do not exist", dummy, pixelSize, centroid);

  std::vector<cv::Point2i> pointsToShift{{0, 0}, {1, 1}, {3, 2}};
  std::vector<cv::Point2d> EXPECTED_POINTS{{-2.5, 2.5}, {-1.5, 1.5}, {0.5, 0.5}};

  // when
  auto shiftedPoints = image->ShiftPointsFromImageToChuckReferential(pointsToShift);

  // then
  size_t pointIndex = 0;
  for (auto const &point : shiftedPoints) {
    auto expectedPoint = EXPECTED_POINTS.at(pointIndex);
    bool isOk = expectedPoint.x == point.x && expectedPoint.y == point.y;
    EXPECT_TRUE(isOk) << "Point " << pointsToShift.at(pointIndex) << " shifted at " << point << " does not match expected " << expectedPoint;

    pointIndex++;
  }
}

TEST(EdgeImage, Expect_empty_to_return_true_if_there_is_no_data) {
  auto image = EdgeImage::New("i do not exist", EdgeImage::PixelSize(cv::Point2d(1, 1)), EdgeImage::ImageCentroid(cv::Point(1, 1)));
  ASSERT_TRUE(image->Empty());
}
TEST(EdgeImage, Expect_empty_to_return_false_if_there_is_data) {
  auto image = EdgeImage::New(TEST_DATA_PATH + std::string("synth_aligned/dummyFullWafer.png"), EdgeImage::PixelSize(cv::Point2d(1, 1)),
                              EdgeImage::ImageCentroid(cv::Point(1, 1)));

  ASSERT_FALSE(image->Empty());
}

TEST(EdgeImage, Expect_image_name_to_be_extracted_from_path) {

  auto image = EdgeImage::New("Any_3_2X_VIS_X_139077_Y_56190.png", EdgeImage::PixelSize(cv::Point2d(1, 1)),
                              EdgeImage::ImageCentroid(cv::Point2d(139.077, 56.190)));
  std::string actual = image->GetName();
  ASSERT_EQ("Any_3_2X_VIS_X_139077_Y_56190.png", actual);

  image = EdgeImage::New("/home/me/Any_3_2X_VIS_X_139077_Y_56190.png", EdgeImage::PixelSize(cv::Point2d(1, 1)),
                         EdgeImage::ImageCentroid(cv::Point2d(139.077, 56.190)));

  actual = image->GetName();
  ASSERT_EQ("Any_3_2X_VIS_X_139077_Y_56190.png", actual);

  image = EdgeImage::New("c:/path/Any_3_2X_VIS_X_139077_Y_56190.png", EdgeImage::PixelSize(cv::Point2d(1, 1)),
                         EdgeImage::ImageCentroid(cv::Point2d(139.077, 56.190)));
  image->GetName();
  ASSERT_EQ("Any_3_2X_VIS_X_139077_Y_56190.png", actual);

  image = EdgeImage::New("c:\\path\\Any_3_2X_VIS_X_139077_Y_56190.png", EdgeImage::PixelSize(cv::Point2d(1, 1)),
                         EdgeImage::ImageCentroid(cv::Point2d(139.077, 56.190)));
  image->GetName();
  ASSERT_EQ("Any_3_2X_VIS_X_139077_Y_56190.png", actual);
}

TEST(EdgeImage, Expect_image_center_to_be_readable) {

  auto pixelSize = EdgeImage::PixelSize(cv::Point2d(1, 1));
  auto expectedCenter = EdgeImage::ImageCentroid(cv::Point2d(-139.077, -56.190));
  auto image = EdgeImage::New("Any_3_2X_VIS_X_-139077_Y_-56190.png", pixelSize, expectedCenter);

  double expectedX = expectedCenter.get().x;
  double expectedY = expectedCenter.get().y;

  auto actualCenter = image->GetCentroid();
  double actualX = actualCenter.get().x;
  double actualY = actualCenter.get().y;

  ASSERT_EQ(expectedX, actualX) << "Center coordinates must be the one used in constructor";
  ASSERT_EQ(expectedY, actualY) << "Center coordinates must be the one used in constructor";
}

TEST(EdgeImage, Expect_image_dimensions_in_pixels_to_be_available) {

  auto pixelSize = EdgeImage::PixelSize(cv::Point2d(1, 1));
  auto center = EdgeImage::ImageCentroid(cv::Point2d(634, 383));
  const int expectedImageWidth = 200;
  const int expectedImageHeight = 200;

  auto image = EdgeImage::New(TEST_DATA_PATH + std::string("/synth_aligned/bigWaferShot_X_634_Y_383.png"), pixelSize, center);
  ASSERT_FALSE(image->Empty());

  auto imageDimensionsInPixel = image->GetDimensionsInMicrometers();
  ASSERT_EQ(expectedImageWidth, imageDimensionsInPixel.x);
  ASSERT_EQ(expectedImageHeight, imageDimensionsInPixel.y);
}

TEST(EdgeImage, Expect_pixel_size_to_be_used_for_dimension_computation) {

  auto pixelSize = EdgeImage::PixelSize(cv::Point2d(2.11, 3.45));
  auto center = EdgeImage::ImageCentroid(cv::Point2d(634, 383));
  const cv::Point imageDimensionsInPixel = cv::Point2d(200, 200);

  auto image = EdgeImage::New(TEST_DATA_PATH + std::string("/synth_aligned/bigWaferShot_X_634_Y_383.png"), pixelSize, center);
  ASSERT_FALSE(image->Empty());

  auto dimensionsInMicrons = image->GetDimensionsInMicrometers();

  ASSERT_EQ(200 * pixelSize.get().x, dimensionsInMicrons.x);
  ASSERT_EQ(200 * pixelSize.get().y, dimensionsInMicrons.y);
}
TEST(EdgeImage, Expect_image_center_relative_to_wafer_center_to_be_available) {

  auto pixelSize = EdgeImage::PixelSize(cv::Point2d(1, 1));
  auto center = EdgeImage::ImageCentroid(cv::Point2d(634, 383));

  auto image = EdgeImage::New(TEST_DATA_PATH + std::string("/synth_aligned/bigWaferShot_X_634_Y_383.png"), pixelSize, center);
  ASSERT_FALSE(image->Empty());

  const int expectedX = 634;
  const int expectedY = 383;

  auto actualCenter = image->GetCentroid();
  ASSERT_EQ(expectedX, actualCenter.get().x);
  ASSERT_EQ(expectedY, actualCenter.get().y);
}

// In this test, as pixel size is set to 1, we only work in pixel referential
TEST(EdgeImage, Expect_opencv_points_to_be_projectible_on_stage_referential) {

  std::string currentPath = std::filesystem::current_path().string();

  auto pixelSize = EdgeImage::PixelSize(cv::Point2d(1, 1));
  auto center = EdgeImage::ImageCentroid(cv::Point2d(634, 383));
  auto image = EdgeImage::New(TEST_DATA_PATH + std::string("/synth_shifted/bigWaferShot_X_634_Y_383.png"), pixelSize, center);
  ASSERT_FALSE(image->Empty());

  auto actualCenter = image->GetCentroid();

  auto edgePoints = image->GetShiftedContourPoints(currentPath);

  // expect each point to be at [radius] pixel of center
  // 3 pixels of tolerance represents around 0,15% of error because image is
  // 2000x2000.
  const double expectedDistanceToCenter = 750;
  const double ERROR_PERCENTAGE = 0.7;
  const double TOLERANCE = (ERROR_PERCENTAGE / 100.0) * expectedDistanceToCenter;
  cv::Point2d waferCenter(10, -5);
  int lowerBound = expectedDistanceToCenter - TOLERANCE;
  int upperBound = expectedDistanceToCenter + TOLERANCE;
  int countOfPointsInsideSpec = 0;
  auto imageDimensions = image->GetDimensionsInMicrometers();
  for (cv::Point2d &pt : edgePoints) {
    // point must be in the image in the chuck referential
    EXPECT_GT(pt.x, (actualCenter.get().x - imageDimensions.x / 2));
    EXPECT_LT(pt.x, (actualCenter.get().x + imageDimensions.x / 2));
    EXPECT_GT(pt.y, (actualCenter.get().y - imageDimensions.y / 2));
    EXPECT_LT(pt.y, (actualCenter.get().y + imageDimensions.y / 2));

    auto actualDistanceToCenter = cv::norm(waferCenter - pt);

    if (actualDistanceToCenter > lowerBound && actualDistanceToCenter < upperBound) {
      countOfPointsInsideSpec++;
    }

    // distance between center and point must be _very_ close to 750px
    EXPECT_NEAR(expectedDistanceToCenter, actualDistanceToCenter, TOLERANCE);
  }
}

TEST(EdgeImage, Expect_opencv_points_with_negative_coordinates_to_be_projectible_on_stage_referential) {
  std::string currentPath = std::filesystem::current_path().string();

  auto pixelSize = EdgeImage::PixelSize(cv::Point2d(1, 1));
  auto center = EdgeImage::ImageCentroid(cv::Point2d(-609, -450));

  auto image = EdgeImage::New(TEST_DATA_PATH + std::string("/synth_aligned/bigWaferShot_X_-609_Y_-450.png"), pixelSize, center);
  ASSERT_FALSE(image->Empty());

  auto actualCenter = image->GetCentroid();

  auto imageDimensions = image->GetDimensionsInMicrometers();

  auto edgePoints = image->GetShiftedContourPoints(currentPath);

  // expect each point to be at [radius] pixel of center
  for (cv::Point2d &pt : edgePoints) {

    // point must be in the image in the chuck referrential
    EXPECT_GT(pt.x, (actualCenter.get().x - imageDimensions.x / 2));
    EXPECT_LT(pt.x, (actualCenter.get().x + imageDimensions.x / 2));
    EXPECT_GT(pt.y, (actualCenter.get().y - imageDimensions.y / 2));
    EXPECT_LT(pt.y, (actualCenter.get().y + imageDimensions.y / 2));

    // distance from center (0,0) to point must be _very_ close to 750px
    auto actualDistanceToCenter = std::sqrt(pt.x * pt.x + pt.y * pt.y);

    const auto expectedDistanceToCenter = 750;
    const double ERROR_PERCENTAGE = 0.7;
    const double TOLERANCE = (ERROR_PERCENTAGE / 100.0) * expectedDistanceToCenter;
    EXPECT_NEAR(expectedDistanceToCenter, actualDistanceToCenter, TOLERANCE);
  }
}

TEST(EdgeImage, Expect_bottom_right_image_to_be_classified) {
  std::vector<EdgeImage::Pointer> images;
  EdgeImage::PixelSize pixelSize(cv::Point2d(0, 0)); // not used
  EdgeImage::ImageCentroid imageCenter(cv::Point2d(68815, -29790));
  auto image = EdgeImage::New(TEST_DATA_PATH + std::string("testImage_X_68815_Y_-29790_2X VIS.png"), pixelSize, imageCenter);

  ASSERT_EQ(image->GetPositionClass(), EdgeImage::PositionClass::BOTTOM_RIGHT);
}

TEST(EdgeImage, Expect_top_right_image_to_be_classified) {

  std::vector<EdgeImage::Pointer> images;
  EdgeImage::PixelSize pixelSize(cv::Point2d(0, 0)); // not used
  EdgeImage::ImageCentroid imageCenter(cv::Point2d(63488, 30768));
  auto image = EdgeImage::New(TEST_DATA_PATH + std::string("testImage_X_63488_Y_30768_2X VIS.png"), pixelSize, imageCenter);

  ASSERT_EQ(image->GetPositionClass(), EdgeImage::PositionClass::TOP_RIGHT);
}

TEST(EdgeImage, Expect_bottom_left_image_to_be_classified) {

  std::vector<EdgeImage::Pointer> images;
  EdgeImage::PixelSize pixelSize(cv::Point2d(0, 0)); // not used
  EdgeImage::ImageCentroid imageCenter(cv::Point2d(-47054, -60502));
  auto image = EdgeImage::New(TEST_DATA_PATH + std::string("testImage_X_-47054_Y_-60502_2X VIS.png"), pixelSize, imageCenter);

  ASSERT_EQ(image->GetPositionClass(), EdgeImage::PositionClass::BOTTOM_LEFT);
}
TEST(EdgeImage, Expect_probable_outside_region_intensity_to_have_default_size) {

  std::vector<EdgeImage::Pointer> images;
  EdgeImage::PixelSize pixelSize(cv::Point2d(0, 0));       // not used
  EdgeImage::ImageCentroid imageCenter(cv::Point2d(0, 0)); // not used
  auto image = EdgeImage::New(TEST_DATA_PATH + std::string("/synth_aligned/bigWaferShot_X_-609_Y_-450.png"), pixelSize, imageCenter);

  EXPECT_FALSE(image->Empty());

  cv::Rect actualOutsideRegionInImageReferential = image->GetProbableOutsideRegion();

  double expectedRegionOfInterestHeight = 0.10 * image->Mat().rows;
  double expectedRegionOfInterestWidth = 0.10 * image->Mat().cols;

  EXPECT_NEAR(expectedRegionOfInterestHeight, actualOutsideRegionInImageReferential.height, 10e-1);
  EXPECT_NEAR(expectedRegionOfInterestWidth, actualOutsideRegionInImageReferential.width, 10e-1);
}

TEST(EdgeImage, Expect_right_min_and_max_radius) {

  // NOTE: when computing distances, we measure from a pixel center to another pixel center. So we have
  //             to deal with a pixelSize/2 at some point.
  // NOTE 2: the image centroid is not in a pixel center, since image width is odd

  struct GivenTypeForRadius : GivenType {
    cv::Point2d waferCenter;
    EdgeImage::MinAndMax expectedMinAndMax;
  };

  // given
  std::vector<GivenTypeForRadius> givens;
  givens.emplace_back(GivenTypeForRadius{
      "Image aligned with 0,0 on X axis", cv::Size(4, 4), cv::Point(1, 1), cv::Point(0, -5), cv::Point2d(0, 0), {3.5, 6.6708320320631671}});
  givens.emplace_back(GivenTypeForRadius{"Image shifted right of X axis",
                                         cv::Size(6, 4),
                                         cv::Point(1, 1),
                                         cv::Point(4, -12),
                                         cv::Point2d(0, 0),
                                         {10.606601717798213, 14.7732867026941585}});
  givens.emplace_back(GivenTypeForRadius{"Metrological wafer case",
                                         cv::Size(1164, 872),
                                         cv::Point2d(5.3, 5.3),
                                         cv::Point(0, -150000),
                                         cv::Point2d(588.3, 1303.8),
                                         {148995.65, 153655.79038183036}});

  for (auto const &given : givens) {

    // when
    EdgeImage::Pointer image = CreateImageFromGiven(given);
    EdgeImage::MinAndMax actualMinAndMax = image->FindOptimalMinAndMaxRadius(given.waferCenter);

    // then
    ASSERT_DOUBLE_EQ(given.expectedMinAndMax.min, actualMinAndMax.min) << "Failed for min radius computation of case: \"" << given.label << "\"";
    ASSERT_DOUBLE_EQ(given.expectedMinAndMax.max, actualMinAndMax.max) << "Failed for max radius computation of case: \"" << given.label << "\"";
  }
}

TEST(EdgeImage, Expect_right_min_and_max_angles) {

  struct GivenTypeForAngles : GivenType {
    cv::Point2d waferCenter;
    EdgeImage::MinAndMax expectedMinAndMax;
  };

  std::vector<GivenTypeForAngles> givens;

  givens.emplace_back(
      GivenTypeForAngles{"Simple case", cv::Size(6, 6), cv::Point2d(1, 1), cv::Point(0, -5.5), cv::Point2d(0, 0), {5 * CV_PI / 4, 7 * CV_PI / 4}});

  givens.emplace_back(GivenTypeForAngles{"Metrological wafer case",
                                         cv::Size(1164, 872),
                                         cv::Point2d(5.3, 5.3),
                                         cv::Point(0, -150000),
                                         cv::Point2d(588.3, 1303.8),
                                         {4.687760692, 4.7291238126404886}});

  for (auto const &given : givens) {

    // when
    EdgeImage::Pointer image = CreateImageFromGiven(given);
    EdgeImage::MinAndMax actualMinAndMax = image->FindOptimalMinAndMaxAngles(given.waferCenter);

    // then
    EXPECT_NEAR(given.expectedMinAndMax.min, actualMinAndMax.min, 10e-4) << "Failed for min angle computation of case: \"" << given.label << "\"";
    EXPECT_NEAR(given.expectedMinAndMax.max, actualMinAndMax.max, 10e-4) << "Failed for max angle computation of case: \"" << given.label << "\"";
  }
}
