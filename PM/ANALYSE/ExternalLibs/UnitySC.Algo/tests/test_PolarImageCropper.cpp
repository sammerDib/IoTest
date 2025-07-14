
#include <PolarImageCropper.hpp>
#include <gtest/gtest.h>
#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>

namespace cv {
  bool operator==(const Rect &left, const Rect &right) {
    return left.width == right.width && left.height == right.height && left.x == right.x && left.y == right.y;
  }
  bool operator==(const Size &left, const Size &right) { return left.width == right.width && left.height == right.height; }

} // namespace cv

namespace {

  struct GivenType {
    std::string label;
    cv::Size originalSize;
    cv::Size expectedSizeAfterCrop;
    cv::Rect expectedCropDimensions;
  };

  // construct image according to GivenType data
  cv::Mat CreateImageUsingGivenInformation(GivenType given) {
    cv::Mat image;
    if (given.originalSize == given.expectedSizeAfterCrop) {
      image = cv::Mat::ones(given.originalSize, CV_8U);
    } else if (given.expectedSizeAfterCrop == cv::Size(0, 0)) {
      image = cv::Mat::zeros(given.originalSize, CV_8U);
    } else {
      image = cv::Mat::zeros(given.originalSize, CV_8U);
      cv::Mat cropRoi = image(given.expectedCropDimensions);
      cropRoi = cv::Scalar(255);
    }
    return image;
  }

} // namespace

TEST(PolarImageCropper, Expect_non_complete_rows_and_columns_to_be_cropped) {

  std::vector<GivenType> givens;

  givens.push_back(GivenType({"All black image returns totally cropped", cv::Size(10, 10), cv::Size(0, 0), cv::Rect(0, 0, 10, 10)}));
  givens.push_back(GivenType({"All white image returns unchanged", cv::Size(10, 10), cv::Size(10, 10), cv::Rect(0, 0, 0, 0)}));
  givens.push_back(GivenType({"First row should be cropped", cv::Size(10, 10), cv::Size(10, 9), cv::Rect(0, 1, 10, 9)}));
  givens.push_back(GivenType({"Two first rows should be cropped", cv::Size(100, 100), cv::Size(100, 98), cv::Rect(0, 2, 100, 98)}));
  givens.push_back(
      GivenType({"Two first rows and two first columns should be cropped", cv::Size(100, 100), cv::Size(98, 98), cv::Rect(2, 2, 98, 98)}));
  givens.push_back(
      GivenType({"A border of 1 elements should be cropped on each side", cv::Size(100, 100), cv::Size(98, 98), cv::Rect(1, 1, 98, 98)}));
  givens.push_back(GivenType(
      {"A border of 10 elements should be cropped on each side (beyond 8U)", cv::Size(1000, 1000), cv::Size(980, 980), cv::Rect(10, 10, 980, 980)}));
  givens.push_back(GivenType({"A border of 2 elements should be cropped on each side (not square image)", cv::Size(1000, 800), cv::Size(998, 798),
                              cv::Rect(2, 2, 998, 798)}));

  for (auto const &given : givens) {
    cv::Rect cropDimensions;
    cv::Mat image = CreateImageUsingGivenInformation(given);

    cv::Mat cropped = PolarImageCropper::Crop(image, &cropDimensions);

    EXPECT_EQ(given.expectedSizeAfterCrop, cropped.size()) << "Failed size for case \"" << given.label << "\"";
    EXPECT_EQ(given.expectedCropDimensions, cropDimensions) << "Failed crop dimensions for case \"" << given.label << "\"";
  }
}

double ProjectCorrelationOrdinateOnPolarImage(double point, cv::Rect polarCroppedPadded, cv::Rect cropRegion) {
  double result = -1;

  double minimalProjectablePointOrdinate = cropRegion.height / 2;
  if (point < minimalProjectablePointOrdinate) {
    throw std::exception("Correlation value is under acceptable value");
  }

  double maximalProjectablePointOrdinate = cropRegion.height * 3 / 2;
  if (point > maximalProjectablePointOrdinate) {
    throw std::exception("Correlation value is over acceptable value");
  }
  double minimalPolarImageOrdinate = static_cast<double>(cropRegion.y);
  double maximalPolarImageOrdinate = static_cast<double>(cropRegion.y) + static_cast<double>(cropRegion.height);

  auto Map = [](double value, double fromLow, double fromHigh, double toLow, double toHigh) {
    return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
  };

  result = Map(point, minimalProjectablePointOrdinate, maximalProjectablePointOrdinate, minimalPolarImageOrdinate, maximalPolarImageOrdinate);

  return result;
}
TEST(PolarImageCropper, Expect_correlation_value_on_cropped_padded_polar_image_to_be_transferrable_on_source_polar) {

  // The case:
  // a 1000x1000 polar image, with 50 top and 100 bottom lines cropped
  // a 1000x850 cropped polar image
  // the padded image adds half the cropped image on top and at bottom of polar image

  // The phaseCorrelation on the croppedPaddedImage should return a value
  // between 425 and 1275.
  cv::Rect rawPolar(0, 0, 1000, 1000);
  cv::Rect cropRegion(0, 50, 1000, 850);
  cv::Rect croppedPolar(0, 0, 1000, 850);
  cv::Rect polarCroppedPadded(0, 0, 1000, croppedPolar.height * 2);

  struct GivenType {
    std::string label;
    double givenPoint;
    cv::Rect givenPolarCroppedImageDimensions;
    cv::Rect givenCropRegionDimensions;
    double expectedPoint;
    bool expectException;
  };

  std::vector<GivenType> givens;

  givens.push_back(GivenType({"value under minimal accepted should throw", 400, polarCroppedPadded, cropRegion, -1, true}));
  givens.push_back(GivenType({"value over maximal accepted should throw", 1300, polarCroppedPadded, cropRegion, -1, true}));
  givens.push_back(GivenType({"value at minimal accepted should returns", 425, polarCroppedPadded, cropRegion, 50, false}));
  givens.push_back(GivenType({"value at maximal accepted should returns", 1275, polarCroppedPadded, cropRegion, 900, false}));

  for (auto const &given : givens) {

    if (given.expectException) {
      EXPECT_THROW(ProjectCorrelationOrdinateOnPolarImage(given.givenPoint, polarCroppedPadded, cropRegion), std::exception)
          << "Failed in case\"" + given.label << "\"";
      ;
    } else {
      double actualPoint = ProjectCorrelationOrdinateOnPolarImage(given.givenPoint, polarCroppedPadded, cropRegion);
      EXPECT_EQ(given.expectedPoint, actualPoint) << "Failed in case\"" + given.label << "\"";
    }
  }
}
