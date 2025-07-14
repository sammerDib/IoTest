#include "pch.h"
#include "PlaneDetector.h"
#include "LeastSquareFitter.h"
#include <vector>
#include <iostream>

#pragma managed
namespace UnitySCSharedAlgosCppWrapper {
    Plane^ PlaneDetector::FindLeastSquarePlane(array<Point3d^>^ points) {
        
        std::vector<geometry::Point3d> convertedPoints;
        convertedPoints.reserve(points->Length);
        for each (Point3d^ point in points)
        {
            convertedPoints.push_back(geometry::Point3d(point->X, point->Y, point->Z));
        }
        
        try 
        {
            geometry::Plane result = least_square_fitter::FitLeastSquarePlane(convertedPoints);
            Point3d^ center = gcnew Point3d(result.Center.X, result.Center.Y, result.Center.Z);
            Point3d^ normal = gcnew Point3d(result.Normal.X, result.Normal.Y, result.Normal.Z);
            return gcnew Plane(center, normal);
        }
        catch (std::exception& ex) 
        {
#ifdef DEBUG
          std::cout << "Error during OptimalTransformationParameters: " << ex.what() << std::endl;
#endif
        }
        return nullptr;
    }
}