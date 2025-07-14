#include <EventQueue.hpp>
#include <cmath>
#include <filesystem>
#include <iostream>
#include <opencv2/highgui.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/imgproc.hpp>
#include <random>
#include <regex>

#include <BaseAlgos/EdgeDetector.hpp>
#include <EdgeImage.hpp>

namespace {
  void drawContours(cv::Mat &image, std::vector<std::vector<cv::Point>> contours) {
    CV_Assert(image.depth() == CV_8U);

    for (int i = 0; i < contours.size(); i++) {
      cv::Scalar color = cv::Scalar(cv::theRNG().uniform(0, 255), cv::theRNG().uniform(0, 255), cv::theRNG().uniform(0, 255));
      drawContours(image, contours, i, color, 2, 8, cv::noArray(), 0, cv::Point());
    }
  }

  std::string RandomString(int stringLength) {
    std::string str(std::string("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"));

    std::random_device rd;
    std::mt19937 generator(rd());

    std::shuffle(str.begin(), str.end(), generator);

    return str.substr(0, stringLength);
  }
} // namespace

const double EdgeImage::PROBABLE_OUTSIDE_REGION_HEIGHT_FRACTION = (10. / 100);
const double EdgeImage::PROBABLE_OUTSIDE_REGION_WIDTH_FRACTION = (10. / 100);

/**
 * Given a contour here with origin relative to upper left corner of field of
 * view, at 0,0. Coordinates of each contour point must be translated so
 * that origin is no longer the upper left corner at 0,0, but the field of view
 * center. Coordinates of that center are provided, as well as total image
 * dimensions.
 */
std::vector<cv::Point2d> EdgeImage::ShiftPointsFromImageToChuckReferential(std::vector<cv::Point2i> &contour) const {
  auto pixelSizeInMicrons = GetPixelSize().get();

  std::stringstream s;
  s << "[EdgeImage::ShiftPointsFromImageToChuckReferential] PixelSize: " << pixelSizeInMicrons;
  Logger::Debug(s.str());

  cv::Point2d imageOrigin = GetOrigin();
  std::vector<cv::Point2d> shiftedContour;
  shiftedContour.reserve(contour.size());

  // translate the point to the new image origin coordinates
  std::transform(contour.begin(), contour.end(), std::back_inserter(shiftedContour), [&imageOrigin, pixelSizeInMicrons](cv::Point2i &pt) {
    cv::Point2d shiftedPoint;
    shiftedPoint.x = imageOrigin.x + (pt.x * pixelSizeInMicrons.x);
    shiftedPoint.y = imageOrigin.y - (pt.y * pixelSizeInMicrons.y);

    return shiftedPoint;
  });

  return shiftedContour;
}

std::string EdgeImage::GetName() const {
  std::string result = "";
  if (_filename.empty()) {
    static const int FILENAME_LENGTH = 32;
    _filename = RandomString(FILENAME_LENGTH);
  }
  std::filesystem::path path(_filename);
  return path.filename().string();
}

const EdgeImage::PixelSize EdgeImage::GetPixelSize() const { return _pixelSize; }

/*
 *
 * @return image centroid distance to wafer center, in micrometer
 *
 * `[o]` is the image, x and y are coordinates of its centroid relative to wafer center.
 *
 *
 */
const EdgeImage::ImageCentroid EdgeImage::GetCentroid() const { return _center; }

const cv::Point2d EdgeImage::GetDimensionsInMicrometers() const {
  cv::Point2d result;
  result.x = _image.cols * _pixelSize.get().x;
  result.y = _image.rows * _pixelSize.get().y;
  return result;
}

std::vector<cv::Point> EdgeImage::GetContourPoints(std::string const &reportPath) {
  std::vector<cv::Point> contour = FindWaferBorder(reportPath);
  return RemovePointsTouchingBorders(contour);
}

std::vector<cv::Point2d> EdgeImage::GetShiftedContourPoints(std::string const &reportPath) {
  auto contour = GetContourPoints(reportPath);
  auto shiftedContour = ShiftPointsFromImageToChuckReferential(contour);
  return shiftedContour;
}

const EdgeImage::PositionClass EdgeImage::GetPositionClass() const {
  bool isRight = (_center.get().x > 0);
  bool isTop = (_center.get().y > 0);
  if (isRight) {
    if (isTop) {
      return PositionClass::TOP_RIGHT;
    } else {
      return PositionClass::BOTTOM_RIGHT;
    }
  } else {
    if (isTop) {
      return PositionClass::TOP_LEFT;
    } else {
      return PositionClass::BOTTOM_LEFT;
    }
  }
  return _positionClass;
}

const cv::Rect EdgeImage::GetProbableOutsideRegion() const {
  static const int margin = 5;

  cv::Rect region;
  region.width = static_cast<int>(_image.cols * PROBABLE_OUTSIDE_REGION_WIDTH_FRACTION);
  region.height = static_cast<int>(_image.rows * PROBABLE_OUTSIDE_REGION_WIDTH_FRACTION);

  switch (GetPositionClass()) {
  case PositionClass::BOTTOM_LEFT:
    region.x = margin;
    region.y = _image.rows - (margin + region.height);
    break;

  case PositionClass::BOTTOM_RIGHT:
    region.x = _image.cols - (margin + region.width);
    region.y = _image.rows - (margin + region.height);
    break;

  case PositionClass::TOP_LEFT:
    region.x = margin;
    region.y = margin;
    break;

  case PositionClass::TOP_RIGHT:
    region.x = _image.cols - (margin + region.width);
    region.y = margin;
    break;
  }
  return region;
}

std::vector<cv::Point> EdgeImage::FindContourContainingTheProbableChuckRegion(std::vector<std::vector<cv::Point>> const &contours, cv::Rect const &probableBackgroundRegion) const {
  std::vector<cv::Point> result;
  cv::Mat tmp = cv::Mat::zeros(_image.size(), CV_8U);

  for (std::vector<cv::Point> const contour : contours) {
    // draw full contoured region (with its content) in `tmp`
    cv::fillConvexPoly(tmp, contour, cv::Scalar(255));

    std::vector<cv::Point> interestRegionPixels;
    cv::findNonZero(tmp, interestRegionPixels);

    // if at least one pixel of the region defined by current contour
    // is inside the probable outside region, we're done
    for (auto &interestRegionPixel : interestRegionPixels) {
      if (probableBackgroundRegion.contains(interestRegionPixel)) {
        result = contour;
        break;
      }
    }
    // "clear" temp matrix
    tmp = 0;
  }
  return result;
}

std::vector<cv::Point> EdgeImage::FindWaferBorder(std::string const &reportPath) {
#if defined NDEBUG && defined(SHOW_IMAGES)
  cv::imshow(GetName() + " input", _image);
#endif

  cv::Mat gray;
  cv::Mat binary;

  if (_image.depth() == CV_32F) {
    _image.convertTo(gray, CV_8UC1, 1 / 255);
  } else {
    gray = _image.clone();
  }

  cv::Rect probableChuckRegion = GetProbableOutsideRegion();
  auto probableChuckROI = gray(probableChuckRegion);
  cv::Scalar probableChuckROIMean;
  cv::Scalar probableChuckROIStdDev;
  cv::meanStdDev(probableChuckROI, probableChuckROIMean, probableChuckROIStdDev);
  if (probableChuckROIMean[0] < 127) {
    Logger::Debug("[EdgeImage] The chuck region is dark");
    // non-wafer region is dark, we
    // have to invert the image because "findcontours"
    // will "contour" bright regions
    gray = 255 - gray;
  } else {
    Logger::Debug("[EdgeImage] The chuck region is bright");
  }

  if (probableChuckROIStdDev[0] > 10) {
    Logger::Warning("[EdgeImage] The probable chuck ROI is not uniform");
  }

#if defined(USE_MORPH_OPERATOR)
  // illumination correction top-hat
  auto kernel = cv::getStructuringElement(cv::MORPH_ELLIPSE, cv::Size(30, 30));
  cv::morphologyEx(gray, gray, cv::MORPH_TOPHAT, kernel);

  cv::Mat dilated;
  int dilation_size = 3;
  cv::Mat element = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(2 * dilation_size + 1, 2 * dilation_size + 1), cv::Point(dilation_size, dilation_size));
  cv::dilate(gray, dilated, element);

  cv::Mat eroded;
  int erosion_size = 3;
  element = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(2 * erosion_size + 1, 2 * erosion_size + 1), cv::Point(erosion_size, erosion_size));
  cv::erode(dilated, eroded, element);
  gray = eroded;

  if (!reportPath.empty()) {
    cv::imwrite(GetName() + "_morphed.png", gray);
  }
#if defined NDEBUG && defined(SHOW_IMAGES)
  cv::imshow(GetName() + " morphed", gray);
#endif
#endif

  cv::Mat filtered;
  cv::bilateralFilter(gray, filtered, 5, 5, 5);
  cv::medianBlur(filtered, filtered, 15);

  if (!reportPath.empty()) {
    cv::imwrite(GetName() + "_filtered.png", filtered);
  }

  cv::threshold(filtered, binary, 0, 255, cv::THRESH_OTSU);

  if (!reportPath.empty()) {
    cv::imwrite(GetName() + "_thresholded.png", binary);
  }

#if defined NDEBUG && defined(SHOW_IMAGES)
  cv::imshow(GetName() + " thresholded", binary);
#endif

  std::vector<std::vector<cv::Point>> contours;
  cv::findContours(binary, contours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_NONE);

  if (!reportPath.empty()) {
    cv::Mat colour(gray.size(), CV_8UC3);
    drawContours(colour, contours);
    cv::imwrite(GetName() + "_contours.png", colour);
#if defined NDEBUG && defined(SHOW_IMAGES)
    cv::imshow(GetName() + " contours", colour);
#endif
  }

  return FindContourContainingTheProbableChuckRegion(contours, probableChuckRegion);
}

std::vector<cv::Point> EdgeImage::RemovePointsTouchingBorders(std::vector<cv::Point> const &initial) const {
  // Some buggy points appear when we are close to borders of the image.
  // The workaround is for now to remove points closer to 20px of each border.
  const int OPENCV_FINDCONTOUR_FIX = 20;
  std::vector<cv::Point> filtered;
  auto isPixelFarEnoughFromImageBorder = [this, OPENCV_FINDCONTOUR_FIX](cv::Point const &point) {
    bool tooCloseOfLeft = (point.x < OPENCV_FINDCONTOUR_FIX);
    bool tooCloseOfBottom = (point.y > (_image.rows - OPENCV_FINDCONTOUR_FIX));
    bool tooCloseOfTop = (point.y < OPENCV_FINDCONTOUR_FIX);
    bool tooCloseOfRight = (point.x > _image.cols - OPENCV_FINDCONTOUR_FIX);
    return (!tooCloseOfLeft && !tooCloseOfBottom && !tooCloseOfTop && !tooCloseOfRight);
  };
  std::copy_if(initial.begin(), initial.end(), std::back_inserter(filtered), isPixelFarEnoughFromImageBorder);
  return filtered;
}

bool EdgeImage::Empty() { return _image.empty(); }

cv::Mat EdgeImage::WithWaferEdge() {
  cv::Mat imageWithContours;
  cv::cvtColor(_image.clone(), imageWithContours, cv::COLOR_GRAY2BGR);
  auto green = cv::Scalar(0, 255, 0);
  auto blue = cv::Scalar(255, 0, 0);
  auto contour = FindWaferBorder("");
  contour = RemovePointsTouchingBorders(contour);
  cv::polylines(imageWithContours, contour, false, green);
  cv::rectangle(imageWithContours, GetProbableOutsideRegion(), blue);
  return imageWithContours;
}