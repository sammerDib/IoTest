#include <EventQueue.hpp>
#include <PolarImageCropper.hpp>
#include <PolarTransform.hpp>
#include <SymmetryDetector.hpp>

#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>

namespace {
  /*!
   * Centers images intensity values in 0, keeping standard deviation of initial image
   */
  cv::Mat Standardize(cv::Mat &imageToStandardize) {
    cv::Scalar mean, std;
    cv::meanStdDev(imageToStandardize, mean, std);
    double alpha = std[0]; // rescale data

    cv::Mat result = imageToStandardize.clone();
    result -= mean[0]; // center data
    result.convertTo(result, CV_32F, 1 / alpha, 0);
    return result;
  }
  cv::Mat Normalize0_1(cv::Mat &imageToStandardize) {
    cv::Mat result;
    imageToStandardize.convertTo(result, CV_32F);
    cv::normalize(result, result, 0, 1, cv::NORM_MINMAX);
    return result;
  }
#ifndef NDEBUG
  void Disp(std::string const &name, cv::Mat const &data) {
    cv::Mat toDisplay;
    data.copyTo(toDisplay);
    cv::normalize(toDisplay, toDisplay, 0, 255, cv::NORM_MINMAX);
    toDisplay.convertTo(toDisplay, CV_8U);
    cv::imshow(name, toDisplay);
  }
#endif

  double Map(double val, double in_min, double in_max, double out_min, double out_max) { return (val - in_min) * (out_max - out_min) / (in_max - in_min) + out_min; };

} // namespace

double SymmetryDetector::ComputeAngleFromCorrelation(double correlationShiftY, std::vector<double> angles, cv::Size const &croppedPaddedPolar, cv::Size const &polar, cv::Rect const &croppedRegion) {

  assert(angles.size() == polar.height);

  Logger::Debug("[SymmetryDetector::ComputeAngleFromCorrelation] Received correlation shift y: " + std::to_string(correlationShiftY));

  double rawAngleInRadians = std::numeric_limits<double>::quiet_NaN();

  if (std::abs(correlationShiftY) < croppedRegion.height) {

    double croppedPaddedCenter = croppedPaddedPolar.height / 2.0;

    // the correlation is a shift, we have to retain only half of that shift to find a location
    // then we center the correlation on the cropped polar
    double correlationInCroppedPadded = croppedPaddedCenter - (std::abs(correlationShiftY) / 2.0);

    Logger::Debug("[SymmetryDetector::ComputeAngleFromCorrelation] Peak in cropped/padded image: " + std::to_string(correlationInCroppedPadded));

    const double padHeight = croppedPaddedPolar.height * 0.25;
    double correlationInCroppedPolarImage = correlationInCroppedPadded - padHeight;

    Logger::Debug("[SymmetryDetector::ComputeAngleFromCorrelation] Peak in cropped polar image: " + std::to_string(correlationInCroppedPolarImage));

    correlationInCroppedPolarImage += croppedRegion.y;
    double minYValueInCroppedImage = 0;
    double maxYValueInCroppedImage = croppedRegion.height;

    auto maxAngleIndex = angles.size() - 1;
    double minAngleValue = angles[std::min(maxAngleIndex, (size_t)std::ceil(minYValueInCroppedImage))];
    double maxAngleValue = angles[std::min(maxAngleIndex, (size_t)std::floor(maxYValueInCroppedImage))];

    // NOTE: using Map() will provide better accuracy than just getting a discrete angle using
    // an index in the angles vector `angles`
    rawAngleInRadians = Map(correlationInCroppedPolarImage, minYValueInCroppedImage, maxYValueInCroppedImage, minAngleValue, maxAngleValue);

    Logger::Debug("[SymmetryDetector::ComputeAngleFromCorrelation] Found angle in radians: " + std::to_string(rawAngleInRadians));

    Logger::Debug("[SymmetryDetector::ComputeAngleFromCorrelation] Found angle in degrees: " + std::to_string(rawAngleInRadians * 180 / CV_PI));
  }
  return rawAngleInRadians;
}

double SymmetryDetector::Detect(EdgeImage *const image, cv::Point2d waferCenter, std::string const &reportPath) {
  cv::Mat topSide;

  cv::Mat _polarImage;

  auto histograms = PolarTransform::Transform(image, _polarImage, waferCenter);

  cv::Rect croppedRegion;
  cv::Mat croppedPolar = PolarImageCropper::Crop(_polarImage, &croppedRegion);

  auto paddedShifted = SymmetryDetector::ImagePadder::PadImage(croppedPolar);

   cv::Mat a, b;
   cv::blur(paddedShifted, a, cv::Size(1, 1));
   cv::blur(paddedShifted, b, cv::Size(7, 7));
   paddedShifted = b - a;

  paddedShifted = Standardize(paddedShifted);

  // standardize data
  topSide = paddedShifted;

  // 3. Flip image to obtain a right side
  static const int FLIP_ON_X_AXIS = 0;
  cv::Mat bottomSide;
  cv::flip(topSide, bottomSide, FLIP_ON_X_AXIS);

  cv::Point2d correlation = cv::phaseCorrelate(topSide, bottomSide);

  double rawAngleInRadians = ComputeAngleFromCorrelation(correlation.y, histograms.angles, paddedShifted.size(), _polarImage.size(), croppedRegion);

  return rawAngleInRadians;
}
