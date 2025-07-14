#include <ShapeDetector.h>

using namespace AlgosLibrary;
using namespace System;

namespace AlgosLibrary {

    namespace {
        array<Circle^>^ ConvertNativeCircleVectorToManagedCircleArray(std::vector<shape_finder::Circle>& dataVec) {
            const int size = dataVec.size();
            array<Circle^>^ dataArray = gcnew array<Circle^>(size);
            for (int i = 0; i < size; i++) {
                dataArray[i] = gcnew Circle(gcnew Point2d(dataVec[i].CenterPos.x, dataVec[i].CenterPos.y), dataVec[i].Diameter);
            }
            return dataArray;
        }

        array<Ellipse^>^ ConvertNativeEllipseVectorToManagedEllipseArray(std::vector<shape_finder::Ellipse>& dataVec) {
            const int size = dataVec.size();
            array<Ellipse^>^ dataArray = gcnew array<Ellipse^>(size);
            for (int i = 0; i < size; i++) {
                dataArray[i] = gcnew Ellipse(gcnew Point2d(dataVec[i].CenterPos.x, dataVec[i].CenterPos.y), dataVec[i].HeightAxis, dataVec[i].WidthAxis, dataVec[i].Angle);
            }
            return dataArray;
        }
    }

    array<Circle^>^ ShapeDetector::CircleDetect(ImageData^ img, CircleFinderParams^ params) {
        // Process input parameters
        cv::Mat image = CreateMatFromImageData(img);
        shape_finder::CircleFinderParams parameters = shape_finder::CircleFinderParams(params->DistBetweenCircles, params->ApproximateDiameter, params->DetectionTolerance, params->CannyThreshold);

        // Call native method
        std::vector<shape_finder::Circle> results = shape_finder::CircleFinder(image, parameters);

        // Process output result
        return ConvertNativeCircleVectorToManagedCircleArray(results);
    }

    array<Ellipse^>^ ShapeDetector::EllipseDetect(ImageData^ img, EllipseFinderParams^ params) {
        // Process input parameters
        cv::Mat image = CreateMatFromImageData(img);
        shape_finder::EllipseFinderParams parameters = shape_finder::EllipseFinderParams(std::pair<float, float>(params->ApproximateAxes->Item1, params->ApproximateAxes->Item2), params->DetectionTolerance, params->CannyThreshold);

        // Call native method
        std::vector<shape_finder::Ellipse> results = shape_finder::EllipseFinder(image, parameters);

        // Process output result
        return ConvertNativeEllipseVectorToManagedEllipseArray(results);
    }
}