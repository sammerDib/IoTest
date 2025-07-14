#include <math.h>
#include <iomanip>
#include <iostream>
#include <opencv2/imgproc.hpp>

#include "HyperAccurateEllipseFitter.hpp"
#include "Wafer.hpp"

#pragma unmanaged
namespace {
    double rmse(const std::vector<cv::Point2f>& points, HyperAccurateEllipseFitter::Result& ellipse) {
        float sumError = 0.0;
        float a = ellipse.semiMajorAxis;
        float b = ellipse.semiMinorAxis;
        float theta = ellipse.majorAxisAngleFromHorizontal;

        for (const auto& pt : points) {
            // transform the point to the ellipse's coordinate system
            float dx = pt.x - ellipse.center.x;
            float dy = pt.y - ellipse.center.y;
            float dxRot = dx * cos(theta) + dy * sin(theta);
            float dyRot = -dx * sin(theta) + dy * cos(theta);

            // Compute the implicit ellipse equation value
            float ellipseEquation = (dxRot * dxRot) / (a * a) + (dyRot * dyRot) / (b * b) - 1.0;

            // Squared error
            sumError += ellipseEquation * ellipseEquation;
        }

        return std::sqrt(sumError / points.size());
    }
} // namespace

using namespace Algorithms;

Status HyperAccurateEllipseFitter::Fit(std::vector<cv::Point2f> const& points, IEllipseFitter::Result& fit) const {
    Status status;

    const int dataSize = static_cast<int>(points.size());

    if (dataSize < 5) {
        fit.success = false;
        fit.message = "I can only fit an ellipse with five or more reference points";
        fit.semiMinorAxis = std::numeric_limits<double>::quiet_NaN();
        fit.semiMajorAxis = std::numeric_limits<double>::quiet_NaN();
        fit.majorAxisAngleFromHorizontal = std::numeric_limits<double>::quiet_NaN();
        fit.center = { std::numeric_limits<double>::quiet_NaN(), std::numeric_limits<double>::quiet_NaN() };
        status.code = StatusCode::BWA_FIT_FAILED;
        status.confidence = 0;
        status.message = fit.message;
    }
    else {
        cv::RotatedRect ellipse = cv::fitEllipseDirect(points);
        float ellipseAngle = (ellipse.size.width < ellipse.size.height) ? ellipse.angle - 90 : ellipse.angle;
        ellipseAngle = (ellipseAngle < 0) ? ellipseAngle + 180 : ellipseAngle;

        fit.center.x = ellipse.center.x;
        fit.center.y = ellipse.center.y;
        double a = ellipse.size.width / 2;
        double b = ellipse.size.height / 2;
        fit.semiMajorAxis = a > b ? a : b;
        fit.semiMinorAxis = a > b ? b : a;
        fit.majorAxisAngleFromHorizontal = ellipseAngle;
        fit.rmse = rmse(points, fit);

        status = BuildStatus(fit);
    }
    return status;
}

Status HyperAccurateEllipseFitter::BuildStatus(const IEllipseFitter::Result& fit) const {
    Status status;
    int rmseThreshold = 100;
    status.code = StatusCode::OK;
    if (0 == fit.rmse) {
        status.confidence = 1;
    }
    else {
        // compute the confidence as:
        status.confidence = 1 - (fit.rmse / rmseThreshold);
    }

    if (fit.rmse > rmseThreshold) {
        status.code = StatusCode::BWA_FIT_FAILED;
        std::ostringstream stringStream;
        stringStream << "Fit failed (too high ellipse point dispersion) with a RMSE of " << fit.rmse;
        status.message = stringStream.str();
        status.message = stringStream.str();
    }
    else {
        std::ostringstream stringStream;
        stringStream << "Fit succeeded with a RMSE of " << fit.rmse;
        status.message = stringStream.str();
    }
    return status;
}