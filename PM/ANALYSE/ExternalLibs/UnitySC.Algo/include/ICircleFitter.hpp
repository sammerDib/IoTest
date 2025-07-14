#pragma once

#include <Status.hpp>
#include <memory>
#include <opencv2/core.hpp>
#include <string>
#include <vector>

class Wafer;

/*!
 * Interface for circle fitters
 *
 * Also Provides description of fit result.
 */
class ICircleFitter {

public:
  using Pointer = std::shared_ptr<ICircleFitter>;

  struct Result {
    cv::Point2d center;
    double radius;
    double rmse;
    int iterations;

    bool success;
    std::string message;

    Result() : center(0, 0), radius(-1), rmse(std::numeric_limits<double>::max()), iterations(1), success(true) {}
  };

  virtual Algorithms::Status Fit(std::vector<cv::Point> const &points, Result &fit, Wafer *wafer) const = 0;
};
