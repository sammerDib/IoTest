#pragma once

#include <opencv2/features2d.hpp>
#include <opencv2/opencv.hpp>

namespace shape_finder {
    struct Circle {
        Circle(cv::Point2f center, float diameter) : CenterPos(center), Diameter(diameter) {}

        cv::Point2f CenterPos;
        float Diameter;
    };

    struct Ellipse {
        Ellipse(cv::Point2f center, float heightAxis, float widthAxis, float angle) : CenterPos(center), HeightAxis(heightAxis), WidthAxis(widthAxis), Angle(angle) {}

        cv::Point2f CenterPos;
        float HeightAxis;
        float WidthAxis;
        float Angle;
    };

    struct CircleFinderParams {
        CircleFinderParams(float distBetweenCircles, float diameter, float detectionTolerance, int cannyThreshold, bool useScharrAlgorithm, bool useMorphologicalOperations) : DistBetweenCircles(distBetweenCircles), ApproximateDiameter(diameter), DetectionTolerance(detectionTolerance), CannyThreshold(cannyThreshold), UseScharrAlgorithm(useScharrAlgorithm), UseMorphologicalOperations(useMorphologicalOperations) {}
        CircleFinderParams(float distBetweenCircles, float diameter, float detectionTolerance, int cannyThreshold) : DistBetweenCircles(distBetweenCircles), ApproximateDiameter(diameter), DetectionTolerance(detectionTolerance), CannyThreshold(cannyThreshold), UseScharrAlgorithm(false), UseMorphologicalOperations(true) {}

        float DistBetweenCircles;
        float ApproximateDiameter;
        float DetectionTolerance;
        int CannyThreshold; // /!\ on utilise ici un int alors q'un double est utilisé dans opencv et un float dans l'appel oO' !!!
        bool UseScharrAlgorithm;
        bool UseMorphologicalOperations;
    };

    struct EllipseFinderParams {
        EllipseFinderParams(std::pair<float, float> approximateAxes, float majorAxisTolerance, float minorAxisTolerance, int cannyThreshold) : ApproximateAxes(approximateAxes), MajorAxisTolerance(majorAxisTolerance), MinorAxisTolerance(minorAxisTolerance), CannyThreshold(cannyThreshold) {}

        std::pair<float, float> ApproximateAxes;
        float MajorAxisTolerance;
        float MinorAxisTolerance;
        int CannyThreshold;
    };

    /*
     * Detect ellipses corresponding to the parameters provided, in a given image
     *
     * @param img      - image to look for ellipses on
     * @param params   - ellipses detection parameters
     * @param roi      - region of interest
     *
     * @return Ellipses found
     */
    std::vector<Ellipse> EllipseFinder(const cv::Mat& img, const EllipseFinderParams& params, cv::Rect roi = cv::Rect());

    /*
     * Detect circles corresponding to the parameters provided, in a given image
     *
     * @param img      - image to look for circles on
     * @param params   - circle detection parameters
     * @param roi      - region of interest
     *
     * @return Circles found
     */
    std::vector<Circle> CircleFinder(const cv::Mat& img, const CircleFinderParams& params, cv::Rect roi = cv::Rect());

    cv::Mat PreprocessImageForEllipseFinder(const cv::Mat& img, const EllipseFinderParams& params);

    cv::Mat PreprocessImageForCircleFinder(const cv::Mat& img, const CircleFinderParams& params);
} // namespace shape_finder