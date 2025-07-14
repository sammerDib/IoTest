#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

namespace matrix_2D {
    struct TransformationParameters {
        TransformationParameters(double rotationRad, cv::Point2d translation, double scale) :
            RotationRad(rotationRad), Translation(translation), Scale(scale) {}

        double RotationRad;
        cv::Point2d Translation;
        double Scale;
    };

    /**
      * Computes an optimal transformation parameters with 4 degrees of freedom between two 2D point sets:
      *  - the rotation angle
      *  - the scaling factor
      *  - the translations in x,y axes
      *
      * @param from - Origin point set, should have at least 3 points
      * @param to   - Destination point set, should have the same size as origin point set "from"
      *
      * @return TransformationParameters to apply to transform the origin points to points closest to the destination ones.
      *
      * @throws exception if the transformation could not be estimated
      */
    TransformationParameters OptimalTransformationParameters(std::vector<cv::Point2d> from, std::vector<cv::Point2d> to);
}