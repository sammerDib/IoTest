#pragma once
#include <EdgeImage.hpp>
#include <HyperAccurateCircleFitter.hpp>
#include <ICircleFitter.hpp>
#include <NotchImage.hpp>
#include <Status.hpp>
#include <opencv2/core.hpp>

class Wafer {

public:
  // Match of CSharp wrapper WaferData
  struct WaferData {
    enum class Type : int { UNKNOWN, NOTCH };
    int radiusInMicrons;
    Type type;

    WaferData() : radiusInMicrons(-1), type(Type::UNKNOWN) {}
  };

  struct WaferGeometricalParameters {

    /*
     *
     * Given as a shift in micrometers relative to chuck center
     *
     */
    cv::Point2d centerShift;

    // Given in micrometers
    double radius;
  };

  Wafer(Wafer::WaferData const &waferData) : _waferData(waferData) { SetCircleFitter(std::shared_ptr<ICircleFitter>(new HyperAccurateCircleFitter())); }

  void AddNotchImage(NotchImage::Pointer notchImage, std::string const &reportPath);

  void AddEdgeImage(EdgeImage::Pointer edge, std::string const &reportPath);

  void SetCircleFitter(ICircleFitter::Pointer fitter) { _circleFitter = fitter; }

  const Wafer::WaferData GetProperties() const { return _waferData; }

  /*!
   * Use (at least 3) EdgeImages to fit a circle representing the actual
   * position of the wafer against the chuck ideal position.
   * WaferGeometricalParameters contains the measured radius as well as the
   * wafer center shift from chuck center.
   *
   * NOTE: The confidence of the status is computed as 1/RMSE of the circle fit
   */
  Algorithms::Status GetGeometricalParameters(WaferGeometricalParameters *result, std::string const &reportPath);

  /*!
   * Using the notch image, computes the angular misalignment
   * of the wafer. The algorithm uses the (notch) image center.
   *
   * \param angle contains the rotation angle in radians
   */
  Algorithms::Status GetMisalignmentAngle(double &angle, cv::Point2d waferCenter, std::string const &reportPath);

private:
  class WaferData;

  const WaferData _waferData;
  std::vector<EdgeImage::Pointer> _edgeImages;
  NotchImage::Pointer _notchImage;
  ICircleFitter::Pointer _circleFitter;

  static const int MAX_ALLOWED_X_SHIFT_IN_MICRONS;
  static const int MAX_ALLOWED_Y_SHIFT_IN_MICRONS;
  static const int MIN_EDGE_IMAGE_FOR_FIT;
  static const std::string ERR_THREE_IMAGES_NEEDED;
  static const std::string MSG_FIT_OK;

  /*!
   * if the RMSE of a fit is larger than this value, then
   * the fit is considered as failed.
   */
  static const double MAX_RMSE_FOR_SUCCESS_IN_MICRONS;

  std::vector<cv::Point2i> collectContoursFromEdgeImages(std::string const &reportPath);
};
