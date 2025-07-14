#pragma once

#include "IEllipseFitter.hpp"

#include <opencv2/core.hpp>
#include <vector>

/*
 * Ellipse fit to a given set of data points (in 2D)
 *
 * Based on HyperAccurateCircleFitter, but instead of fitting a circle,
 * we fit an ellipse of the form : Ax^2 + Bxy + Cy^2 + Dx + Ey + F = 0.
 * We now have six unknown parameters (A,B,C,D,E,F).
 * The solution involves solving a generalized eigenvalue problem or using least squares fitting.
 * This implementation follows the Fitzgibbon’s Direct Least Squares Ellipse Fitting algorithm.
 */
#pragma unmanaged
class HyperAccurateEllipseFitter : public IEllipseFitter {
public:
    Algorithms::Status Fit(std::vector<cv::Point2f> const& points, IEllipseFitter::Result& fit) const;

    Algorithms::Status BuildStatus(const IEllipseFitter::Result& fit) const;
};