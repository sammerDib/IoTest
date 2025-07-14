#pragma once

#include "CShapeFinder.hpp"
#include "ImageData.h"
#include "Tools.h"

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {
    public ref struct CircleFinderParams {
        CircleFinderParams(double distBetweenCircles, double diameter, double detectionTolerance, int cannyThreshold, bool useScharrAlgorithm, bool useMorphologicalOperations) : DistBetweenCircles(distBetweenCircles), ApproximateDiameter(diameter), DetectionTolerance(detectionTolerance), CannyThreshold(cannyThreshold), UseScharrAlgorithm(useScharrAlgorithm), UseMorphologicialOperations(useMorphologicalOperations) {}
        CircleFinderParams(double distBetweenCircles, double diameter, double detectionTolerance, int cannyThreshold) : DistBetweenCircles(distBetweenCircles), ApproximateDiameter(diameter), DetectionTolerance(detectionTolerance), CannyThreshold(cannyThreshold), UseScharrAlgorithm(false), UseMorphologicialOperations(true) {}
        double DistBetweenCircles;  // dans le native on utilise des float !
        double ApproximateDiameter; // dans le native on utilise des float !
        double DetectionTolerance;  // dans le native on utilise des float !
        int CannyThreshold; // /!\ on utilise ici un int alors q'un double est utilisé dans opencv et un float dans l'appel oO' !!!
        bool UseScharrAlgorithm;
        bool UseMorphologicialOperations;
    };

    public ref struct EllipseFinderParams {
        EllipseFinderParams(Tuple<double, double>^ approximateAxes, double majorAxisTolerance, double minorAxisTolerance, int cannyThreshold) : ApproximateAxes(approximateAxes), MajorAxisTolerance(majorAxisTolerance), MinorAxisTolerance(minorAxisTolerance), CannyThreshold(cannyThreshold) {}
        Tuple<double, double>^ ApproximateAxes; // <float,float> dans le native
        double MajorAxisTolerance; // float dans le native
        double MinorAxisTolerance; // float dans le native
        int CannyThreshold; // /!\ on utilise ici un int alors q'un double est utilisé dans opencv et un float dans l'appel oO' !!!
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

    public ref struct CircleResult {
        array<Circle^>^ Circles;
        ImageData^ PreprocessedImage;
    };

    public ref struct EllipseResult {
        array<Ellipse^>^ Ellipses;
        ImageData^ PreprocessedImage;
    };

    public ref class ShapeDetector {
    public:
        /*
         * Detect circles corresponding to the parameters provided, in a given image
         *
         * @param img      - image to look for cicles on
         * @param params   - cicles detection parameters
         * @param regionOfInterest - roi where the ellipses will be detected
         *
         * @return Circles found
         */
        static CircleResult^ CircleDetect(ImageData^ img, CircleFinderParams^ params, RegionOfInterest^ regionOfInterest);

        /*
         * Detect ellipses corresponding to the parameters provided, in a given image
         *
         * @param img      - image to look for ellipses on
         * @param params   - ellipses detection parameters
         * @param RegionOfInterest - roi where the ellipses will be detected
         *
         * @return Ellipses found
         */
        static EllipseResult^ EllipseDetect(ImageData^ img, EllipseFinderParams^ params, RegionOfInterest^ regionOfInterest);
    };
}
