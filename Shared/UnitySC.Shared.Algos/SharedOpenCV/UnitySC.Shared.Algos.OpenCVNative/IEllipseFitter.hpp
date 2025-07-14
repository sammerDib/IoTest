#pragma once

#include <memory>
#include <opencv2/core.hpp>
#include <string>
#include <vector>

#include "Status.hpp"

#pragma unmanaged

/*!
 * Interface for circle fitters
 *
 * Also Provides description of fit result.
 */
class IEllipseFitter {
public:
    using Pointer = std::shared_ptr<IEllipseFitter>;

    struct Result {
        cv::Point2d center;
        double semiMajorAxis;
        double semiMinorAxis;
        double majorAxisAngleFromHorizontal; //trigonometric direction, between 0 and 180 degrees
        double rmse;

        bool success;
        std::string message;

        Result() : center(0, 0), semiMajorAxis(-1), semiMinorAxis(-1), majorAxisAngleFromHorizontal(0), rmse(std::numeric_limits<double>::max()), success(true) {}
    };

    virtual Algorithms::Status Fit(std::vector<cv::Point2f> const& points, Result& fit) const = 0;
};