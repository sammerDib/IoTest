#include "2DMatrixAnalysis.hpp"

#include "ErrorLogging.hpp"

#include <string>
#include <sstream>
#include <exception>
#include <cmath>

#include <opencv2/calib3d.hpp>
#include <opencv2/core/mat.hpp>

namespace {
    /**
      * Computes an optimal limited affine transformation with 4 degrees of freedom between two 2D point sets.
      *
      * Estimated transformation matrix is:
      *
      * |cos(R).s  -sin(R).s tx|
      * |sin(R).s   cos(R).s ty|
      *
      * Where R is the rotation angle, s the scaling factor and tx,ty are translations in x,y axes respectively.
      *
      * @param from - Origin point set, should have at least 3 points
      * @param to   - Destination point set, should have the same size as origin point set "from"
      *
      * @return Output 2D affine transformation (4 degrees of freedom) matrix 2×3 or empty matrix if transformation could not be estimated.
      *
      * @throws exception if the input is invalid (< 3 points or different size)
      */
    static cv::Mat OptimalRotationScaleTranslation(std::vector<cv::Point2d> from, std::vector<cv::Point2d> to);
}

namespace matrix_2D {
    TransformationParameters OptimalTransformationParameters(std::vector<cv::Point2d> from, std::vector<cv::Point2d> to) {
        cv::Mat optimalTransformation = OptimalRotationScaleTranslation(from, to);

        if (optimalTransformation.empty())
            ErrorLogging::LogErrorAndThrow("Transformation could not be estimated: algorithm failed (empty matrix)");

        // I've seen the case of NaN values happen when giving only 2 degenerated points to OptimalRotationScaleTranslation.
        // Maybe it doesn't happen with the current check of having at least 3 points, but better safe than sorry.
        if (std::isnan(optimalTransformation.at<double>(0, 0)))
            ErrorLogging::LogErrorAndThrow("Transformation could not be estimated: algorithm failed (NaN values)");

        double scale = sqrt(optimalTransformation.at<double>(0, 0) * optimalTransformation.at<double>(1, 1)
            - optimalTransformation.at<double>(0, 1) * optimalTransformation.at<double>(1, 0));
        double rotation = std::atan2(optimalTransformation.at<double>(1, 0), optimalTransformation.at<double>(0, 0));
        cv::Point2d translation(optimalTransformation.at<double>(0, 2), optimalTransformation.at<double>(1, 2));
        return TransformationParameters(rotation, translation, scale);
    }
}

namespace {
    cv::Mat OptimalRotationScaleTranslation(std::vector<cv::Point2d> from, std::vector<cv::Point2d> to) {
        if (from.size() != to.size())
            ErrorLogging::LogErrorAndThrow("\"From\" points expected to have the same size than \"to\" points: ", from.size(), " != ", to.size());

        // In most cases only 2 points are enough but in some degenerated cases they can lead to NaN values in the resulting matrix. 
        // With 3 or more, when the algorithm fails it generally just returns an empty matrix.
        if (from.size() < 3)
            ErrorLogging::LogErrorAndThrow("At least 3 points are needed to find the transformation. Only ", from.size(), " found.");

        // Use least-median of squares algorithm, not supposed to have outliers
        return cv::estimateAffinePartial2D(from, to, cv::noArray(), cv::LMEDS);
    }
}