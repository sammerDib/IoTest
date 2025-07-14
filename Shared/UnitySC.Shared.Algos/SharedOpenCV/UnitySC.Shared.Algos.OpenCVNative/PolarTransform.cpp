#include "EdgeImage.hpp"
#include "Point.hpp"
#include "PolarTransform.hpp"
#include "Linspace.hpp"
#include "LoggerOCV.hpp"

#include "CImageTypeConvertor.hpp"

#pragma unmanaged
namespace {

  void CopyIntensityFromCartesianToPolar(cv::Point2d cartesianPoint, cv::Point2d polarPoint, cv::Mat &src, cv::Mat &targetPolarImage) {
    cv::Point2i discreteCartesianPoint;
    discreteCartesianPoint.x = (int) std::round(cartesianPoint.x);
    discreteCartesianPoint.y = (int) std::round(cartesianPoint.y);
    uchar intensityValue = src.at<uchar>(discreteCartesianPoint);
    //changing values of 0 to 1 so they don't get cropped later
    if (intensityValue == 0) intensityValue++;
    targetPolarImage.at<uchar>(polarPoint) = intensityValue;
  }

  void LogRangesInformation(PolarTransform::Histograms &histograms) {
    std::string message;

    message = "[PolarTransform::Transform] radius/angle discrete count: " + std::to_string(PolarTransform::DISCRETIZED_RADIUS_COUNT) + "/" +
              std::to_string(PolarTransform::DISCRETIZED_ANGLES_COUNT);
    LoggerOCV::Debug(message);

    message = "[PolarTransform::Transform] Min/max radius: " + std::to_string(histograms.radius.at(0)) + "/" +
              std::to_string(histograms.radius.at(PolarTransform::DISCRETIZED_RADIUS_COUNT - 1));
    LoggerOCV::Debug(message);

    message = "[PolarTransform::Transform] Min/max angles: " + std::to_string(histograms.angles.at(0)) + "/" +
              std::to_string(histograms.angles.at(PolarTransform::DISCRETIZED_ANGLES_COUNT - 1));
    LoggerOCV::Debug(message);

    message = "[PolarTransform::Transform] median angle: " + std::to_string(histograms.angles.at(PolarTransform::DISCRETIZED_RADIUS_COUNT / 2));
    LoggerOCV::Debug(message);
  }
} // namespace

bool PolarTransform::ChuckToImageReferential(EdgeImage *image, cv::Point2d const &pointInChuckReferential, cv::Point2d &pointInImageReferential) {

  cv::Point2d imageOrigin = image->GetOrigin();
  cv::Point2d pixelSize = image->GetPixelSize().get();

  cv::Point2d bottomRight = imageOrigin;
  bottomRight.x += image->Mat().cols * image->GetPixelSize().get().x;
  bottomRight.y -= image->Mat().rows * image->GetPixelSize().get().y;

  bool xIsOut = pointInChuckReferential.x <= imageOrigin.x || pointInChuckReferential.x >= bottomRight.x;
  bool yIsOut = pointInChuckReferential.y >= imageOrigin.y || pointInChuckReferential.y <= bottomRight.y;

  if (xIsOut || yIsOut) {
    return false;
  }
  // compute x and y distance to image origin
  pointInImageReferential.x = cv::norm(pointInChuckReferential.x - imageOrigin.x);
  pointInImageReferential.y = cv::norm(pointInChuckReferential.y - imageOrigin.y);

  //// We are now in the image, but at chuck scale. rescale.
  pointInImageReferential.x /= pixelSize.x;
  pointInImageReferential.y /= pixelSize.y;

  return (std::round(pointInImageReferential.x) < image->Mat().cols && std::round(pointInImageReferential.y) < image->Mat().rows);
}

void PolarTransform::BackTransform(cv::Mat &polarImage, cv::Mat &cartesianImage, Histograms histograms, cv::Point2d pixelSize) {

  double minRadius = histograms.radius.at(0);
  double maxRadius = histograms.radius.at(histograms.radius.size() - 1);
  double minAngle = histograms.angles.at(0);
  double maxAngle = histograms.angles.at(histograms.angles.size() - 1);

  cv::Point2d minRadiusMinAngle = Point::PolarToCartesian({minRadius, minAngle});
  cv::Point2d maxRadiusMinAngle = Point::PolarToCartesian({maxRadius, minAngle});
  cv::Point2d maxRadiusMaxAngle = Point::PolarToCartesian({maxRadius, maxAngle});
  cv::Point2d minRadiusMaxAngle = Point::PolarToCartesian({minRadius, maxAngle});

  cv::Point2d topLeft;
  topLeft.x = std::min(std::min(minRadiusMinAngle.x, maxRadiusMinAngle.x), std::min(maxRadiusMaxAngle.x, minRadiusMaxAngle.x));
  topLeft.y = std::max(std::max(minRadiusMinAngle.y, maxRadiusMinAngle.y), std::max(maxRadiusMaxAngle.y, minRadiusMaxAngle.y));

  cv::Point2d bottomRight;
  bottomRight.x = std::max(std::max(minRadiusMinAngle.x, maxRadiusMinAngle.x), std::max(maxRadiusMaxAngle.x, minRadiusMaxAngle.x));
  bottomRight.y = std::min(std::min(minRadiusMinAngle.y, maxRadiusMinAngle.y), std::min(maxRadiusMaxAngle.y, minRadiusMaxAngle.y));

  auto width = (int) ((bottomRight.x - topLeft.x) / pixelSize.x);
  auto height = (int)((topLeft.y - bottomRight.y) / pixelSize.y);

  cartesianImage = cv::Mat(cv::Size(width, height), CV_8U);

  for (int radiusIndex = 0; radiusIndex < polarImage.cols; ++radiusIndex) {
    for (int angleIndex = 0; angleIndex < polarImage.rows; ++angleIndex) {

      uchar intensity = polarImage.at<uchar>(cv::Point(radiusIndex, angleIndex));
      cv::Point2d cartesianPoint = Point::PolarToCartesian({histograms.radius.at(radiusIndex), histograms.angles.at(angleIndex)});

      cartesianPoint.x = cv::norm(cartesianPoint.x - topLeft.x);
      cartesianPoint.y = cv::norm(cartesianPoint.y - topLeft.y);
      cartesianPoint.x /= pixelSize.x;
      cartesianPoint.y /= pixelSize.y;

      if (cartesianPoint.x > 0 && cartesianPoint.x < cartesianImage.cols && cartesianPoint.y > 0 && cartesianPoint.y < cartesianImage.rows) {
        cv::Point2i discreteCartesianPoint((int)cartesianPoint.x, (int)cartesianPoint.y);
        cartesianImage.at<uchar>(discreteCartesianPoint) = intensity;
      }
    }
  }
}

PolarTransform::Histograms PolarTransform::Transform(EdgeImage *const image, cv::Mat &targetPolarImage, cv::Point2d waferCenter) {

  cv::Mat src = image->Mat();
  cv::Mat gray = Convertor::ConvertTo8UC1(src);
  EdgeImage::MinAndMax minAndMaxRadius = image->FindOptimalMinAndMaxRadius(waferCenter);
  EdgeImage::MinAndMax minAndMaxAngles = image->FindOptimalMinAndMaxAngles(waferCenter);

  Histograms histograms;
  histograms.angles = linspace(minAndMaxAngles.min, minAndMaxAngles.max, DISCRETIZED_ANGLES_COUNT);
  histograms.radius = linspace(minAndMaxRadius.min, minAndMaxRadius.max, DISCRETIZED_RADIUS_COUNT);

  LogRangesInformation(histograms);

  double angleStep = (minAndMaxAngles.max - minAndMaxAngles.min) / (DISCRETIZED_ANGLES_COUNT);
  double radiusStep = (minAndMaxRadius.max - minAndMaxRadius.min) / (DISCRETIZED_RADIUS_COUNT);

  double minRadius = minAndMaxRadius.max;
  cv::Point2d minRadiusPoint;

  targetPolarImage = cv::Mat::zeros(cv::Size(DISCRETIZED_RADIUS_COUNT, DISCRETIZED_ANGLES_COUNT), CV_8UC1);
  auto pixelSize = image->GetPixelSize().get();
  double currentAngle = minAndMaxAngles.min - angleStep;
  double currentRadius = minAndMaxRadius.min - radiusStep;
  for (int angleIndex = 0; angleIndex < DISCRETIZED_ANGLES_COUNT; ++angleIndex) {

    currentAngle += angleStep;
    currentRadius = minAndMaxRadius.min - radiusStep;

    for (int radiusIndex = 0; radiusIndex < DISCRETIZED_RADIUS_COUNT; ++radiusIndex) {

      currentRadius += radiusStep;

      cv::Point2d polarPoint(currentRadius, currentAngle);
      cv::Point2d cartesianPoint = Point::PolarToCartesian(polarPoint);
      cartesianPoint += waferCenter;

      if (currentRadius < minRadius) {
        minRadius = currentRadius;
        minRadiusPoint = cartesianPoint;
      }

      cv::Point2d pointInImageReferential;
      if (ChuckToImageReferential(image, cartesianPoint, pointInImageReferential)) {

        cv::Point coordinatesOfPointInPolarImage(radiusIndex, angleIndex);
        CopyIntensityFromCartesianToPolar(pointInImageReferential, coordinatesOfPointInPolarImage, gray, targetPolarImage);
      }
    }
  }
  return histograms;
}
