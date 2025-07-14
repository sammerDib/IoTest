#pragma once

#include <Logger.hpp>
#include <NamedType.hpp>

#include <Point.hpp>
#include <iostream>
#include <memory>
#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>

/*
 * Contains information about wafer edge: This kind of image is used for "blank
 * wafer alignment" operation. N images (N >= 3) of wafer edge are used to fit a
 * circle and obtain its radius and real center position.
 * The radius should be very close of expected wafer type (ie 300mm, 200mm).
 * The real center should be close of chuck center (at 0,0), with a small translation.
 * This translation will be used further to correct measurements coordinates.
 *
 * NOTE: EdgeImage is provided at early stage and uses chuck coordinates, in micrometers.
 * It means provided image center is relative to chuck center, in micrometers.
 * To allow wafer circle fit, image dimension has to be converted to micrometers using
 * pixel size information provided by the used objective (ie 5x, 50x and so on).
 *
 * \NOTE: the image stored internally must use the CV_8U format, by convention.
 */
class EdgeImage {

protected:
  struct PixelSizeTag {};
  struct ImageCenterTag {};

public:
  // PixelSize must be expressed as micrometers
  using PixelSize = NamedType<cv::Point2d, PixelSizeTag>;
  using ImageCentroid = NamedType<cv::Point2d, ImageCenterTag>;
  using Pointer = std::shared_ptr<EdgeImage>;

  /*
   * When a sample region is extracted for "outside" detection,
   * its size is calculated using a percentage of image dimension.
   * That percentage is provided for image width and height
   */
  static const double PROBABLE_OUTSIDE_REGION_HEIGHT_FRACTION;

  /*
   * When a sample region is extracted for "outside" detection,
   * its size is calculated using a percentage of image dimension.
   * That percentage is provided for image width and height
   */
  static const double PROBABLE_OUTSIDE_REGION_WIDTH_FRACTION;

  /*!
   * An edge image must be in one of these four quadrants
   */
  enum class PositionClass { UNKNOWN, BOTTOM_RIGHT, BOTTOM_LEFT, TOP_LEFT, TOP_RIGHT };

  static Pointer New(std::string imageFileName, PixelSize &pixelSize, ImageCentroid const &center) { return Pointer(new EdgeImage(imageFileName, pixelSize, center)); }

  static Pointer New(cv::Mat data, PixelSize &pixelSize, ImageCentroid const &center) { return Pointer(new EdgeImage("", data, pixelSize, center)); }

  static Pointer New(std::string imageName, cv::Mat data, PixelSize &pixelSize, ImageCentroid const &center) { return Pointer(new EdgeImage(imageName, data, pixelSize, center)); }

  const PixelSize GetPixelSize() const;
  const cv::Point2d GetDimensionsInMicrometers() const;
  const PositionClass GetPositionClass() const;
  std::string GetName() const;
  bool Empty();

  /*!
   * @return image centroid coordinates, before any correction
   */
  const ImageCentroid GetCentroid() const;

  /*
   * @return image origin (upper left corner) in the chuck referential
   */
  const cv::Point2d GetOrigin() const {
    auto imageCentroid = GetCentroid().get();
    auto pixelSize = GetPixelSize().get();
    // when moving up and left, X diminishes, and Y increases
    double xShift = (std::ceil(_image.cols / 2) - 0.5) * pixelSize.x;
    double yShift = (std::ceil(_image.rows / 2) - 0.5) * pixelSize.y;

    cv::Point2d origin(imageCentroid.x - xShift, imageCentroid.y + yShift);
    return origin;
  }

  cv::Mat const &Mat() const { return _image; }

  /*!
   * Given image classification (top-left, bottom-left..) we can deduce that
   * some region in the corner of the image farthest from the center of the
   * wafer will contain chuck (an no wafer) pixels
   */
  const cv::Rect GetProbableOutsideRegion() const;

  /*!
   *
   * Return extracted wafer contour points in the wafer referential
   *
   * NOTE: only image size and center position are used to shift points position.
   */
  std::vector<cv::Point2d> GetShiftedContourPoints(std::string const &reportPath);

  /*!
   *
   * \return extracted wafer contour points in the image referential
   */
  std::vector<cv::Point> GetContourPoints(std::string const &reportPath);

  /**
   * Return source image with fitted wafer border and chuck region
   */
  cv::Mat WithWaferEdge();

  /*!
   * When the wafer contour is detected, its points coordinates are relative to image origin, at upper left corner.
   * This method shifts these coordinates to make them relative to theorical wafer center at (0,0).
   */
  std::vector<cv::Point2d> ShiftPointsFromImageToChuckReferential(std::vector<cv::Point2i> &points) const;

  struct MinAndMax {
    double min;
    double max;
  };

  MinAndMax FindOptimalMinAndMaxRadius(cv::Point2d actualWaferCenterPosition) {

    MinAndMax result{0, 0};
    const cv::Point2d centroidAtZero(0, 0);
    cv::Point2d imageCentroid = GetCentroid().get();
    cv::Point2d origin = GetOrigin();
    if (imageCentroid == centroidAtZero) {
      result.min = 0;
      result.max = cv::norm(GetOrigin());
    } else {
      result.min = std::numeric_limits<long>::max();
      result.max = 0;
      double scaledHalfHeightOfImage = ((Mat().rows * GetPixelSize().get().y) / 2) - 0.5 * GetPixelSize().get().x;
      double lastLineYPosition = imageCentroid.y - scaledHalfHeightOfImage;
      for (size_t colIndex = 0; colIndex < Mat().cols; colIndex++) {
        cv::Point2d currentPoint = origin;
        currentPoint.x += colIndex * GetPixelSize().get().x;
        if (colIndex > 0) {
          currentPoint.x -= 0.5 * GetPixelSize().get().x;
        }
        double distanceToCenter = cv::norm(currentPoint - actualWaferCenterPosition);
        if (distanceToCenter < result.min) {
          result.min = distanceToCenter;
        }
        currentPoint.y = lastLineYPosition;
        distanceToCenter = cv::norm(currentPoint - actualWaferCenterPosition);
        if (distanceToCenter > result.max) {
          result.max = distanceToCenter;
        }
      }
    }
    return result;
  }
  /*
   *  Compute an angle range so that all image pixel will be projected in the polar image
   *
   *
   */
  MinAndMax FindOptimalMinAndMaxAngles(cv::Point2d actualWaferCenterPosition) {

    MinAndMax result{0, 0};
    std::pair<double, double> minAndMaxAngles;
    cv::Point imageCentroid = GetCentroid().get();
    bool centerIsInsideTheImage = (imageCentroid.x > 0 && imageCentroid.x < Mat().cols && imageCentroid.y > 0 && imageCentroid.y < Mat().rows);
    if (centerIsInsideTheImage) {
      result.min = 0;
      result.max = CV_PI * 2;
    } else {

      /*
       *
       *            + wafer center
       *               / \
       *              /    \
       *            /        \
       *           /           \
       *          /              \
       * -----(A)---------(B)----- <-- named points used for angle computation
       *         |                |
       *         |                |
       * ....../-|-\..........| <-- wafer edge and notch
       *         |                |
       * -----(C) ---------(D)
       *
       * The function will search min and max angle between wafer center and these four points.
       */

      cv::Mat src = Mat();
      cv::Point2d imageOrigin = GetOrigin();
      cv::Point2d imageCentroid = GetCentroid().get();

      double halfPixelX = GetPixelSize().get().x / 2;
      double halfPixelY = GetPixelSize().get().y / 2;

      double scaledHalfWidth = (src.cols / 2 * GetPixelSize().get().x) - halfPixelX;
      double scaledHalfHeight = (src.rows / 2 * GetPixelSize().get().y) + halfPixelY;

      cv::Point2d cartesianPointA = imageOrigin;
      cv::Point2d cartesianPointB = cv::Point2d(imageCentroid.x + scaledHalfWidth, imageOrigin.y);
      cv::Point2d cartesianPointC = cv::Point2d(imageOrigin.x, imageCentroid.y - scaledHalfHeight);
      cv::Point2d cartesianPointD = cv::Point2d(imageCentroid.x + scaledHalfWidth, imageOrigin.y - scaledHalfHeight);

      cv::Point2d polarPointA(Point::CartesianToPolar(cartesianPointA - actualWaferCenterPosition));
      cv::Point2d polarPointB(Point::CartesianToPolar(cartesianPointB - actualWaferCenterPosition));
      cv::Point2d polarPointC(Point::CartesianToPolar(cartesianPointC - actualWaferCenterPosition));
      cv::Point2d polarPointD(Point::CartesianToPolar(cartesianPointD - actualWaferCenterPosition));

      double thetaA = polarPointA.y;
      double thetaB = polarPointB.y;
      double thetaC = polarPointC.y;
      double thetaD = polarPointD.y;

      result.min = std::min(std::min(thetaA, thetaB), std::min(thetaC, thetaD));
      result.max = std::max(std::max(thetaA, thetaB), std::max(thetaC, thetaD));
    }
    return result;
  }

protected:
  cv::Mat _image;

  // can be changed from the inside
  mutable std::string _filename;

  PixelSize _pixelSize;
  ImageCentroid _center;
  PositionClass _positionClass;

  explicit EdgeImage::EdgeImage(std::string const &imageFileName, PixelSize const &PixelSize, ImageCentroid const &imageCenter) : _filename(imageFileName), _pixelSize(PixelSize), _center(imageCenter), _positionClass(PositionClass::UNKNOWN) {

    _image = cv::imread(_filename, cv::ImreadModes::IMREAD_GRAYSCALE);

    Logger::Debug("[EdgeImage] Loaded new '" + imageFileName + "' instance");
  }

  explicit EdgeImage::EdgeImage(std::string const &imageName, cv::Mat &data, PixelSize const &PixelSize, ImageCentroid const &imageCenter) : _filename(imageName), _image(data), _pixelSize(PixelSize), _center(imageCenter), _positionClass(PositionClass::UNKNOWN) { Logger::Debug("[EdgeImage] Create new '" + imageName + "' instance"); }

private:
  /*!
   * Detect wafer border in the image
   */
  std::vector<cv::Point> FindWaferBorder(std::string const &reportPath);

  /*!
   * Filtering method which removes points touching border from a point list
   */
  std::vector<cv::Point> RemovePointsTouchingBorders(std::vector<cv::Point> const &initial) const;

  /*!
   * When searching wafer edge, the algorithm collects contour of objects in the image.
   * The only one we want to retain is the one containing the "probable chuck region",
   * a region containing only chuck and no wafer pixels.
   */
  std::vector<cv::Point> FindContourContainingTheProbableChuckRegion(std::vector<std::vector<cv::Point>> const &contours, cv::Rect const &outsideRegion) const;
};
