#pragma once

#include <BaseAlgos/ShapeFinder.hpp>
#include <ImageData.h>
#include <Tools.h>

namespace AlgosLibrary {

    public ref struct CircleFinderParams {
        CircleFinderParams(double distBetweenCircles, double diameter, double detectionTolerance, int cannyThreshold) : DistBetweenCircles(distBetweenCircles), ApproximateDiameter(diameter), DetectionTolerance(detectionTolerance), CannyThreshold(cannyThreshold) {}
        double DistBetweenCircles;
        double ApproximateDiameter;
        double DetectionTolerance;
        int CannyThreshold;
    };

    public ref struct EllipseFinderParams {
        EllipseFinderParams(Tuple<double, double>^ approximateAxes, double detectionTolerance, int cannyThreshold) : ApproximateAxes(approximateAxes), DetectionTolerance(detectionTolerance), CannyThreshold(cannyThreshold) {}
        Tuple<double, double>^ ApproximateAxes;
        double DetectionTolerance;
        int CannyThreshold;
    };

    public ref struct Circle {
        Circle(Point2d^ center, double diameter) : Center(center), Diameter(diameter) {}
        Point2d^ Center;
        double Diameter;
    };

    public ref struct Ellipse {
        Ellipse(Point2d^ center, double heightAxis, double widthAxis, double angle) : Center(center), HeightAxis(heightAxis), WidthAxis(widthAxis), Angle(angle) {}
        Point2d^ Center;
        double HeightAxis;
        double WidthAxis;
        double Angle;
    };

    public ref class ShapeDetector {

    public:
        /*
         * Detect circles corresponding to the parameters provided, in a given image
         *
         * @param img      - image to look for cicles on
         * @param params   - cicles detection parameters
         *
         * @return Circles found
         */
        static array<Circle^>^ CircleDetect(ImageData^ img, CircleFinderParams^ params);

        /*
         * Detect ellipses corresponding to the parameters provided, in a given image
         *
         * @param img      - image to look for ellipses on
         * @param params   - ellipses detection parameters
         *
         * @return Ellipses found
         */
        static array<Ellipse^>^ EllipseDetect(ImageData^ img, EllipseFinderParams^ params);
    };
}
