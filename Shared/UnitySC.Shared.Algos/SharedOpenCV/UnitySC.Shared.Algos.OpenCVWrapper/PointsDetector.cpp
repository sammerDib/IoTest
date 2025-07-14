#include "PointsDetector.h"

#include "2DMatrixAnalysis.hpp"

#include <iostream>

#pragma managed
namespace
{
    std::vector<cv::Point2d> ConvertPoints(array<UnitySCSharedAlgosOpenCVWrapper::Point2d^>^ points)
    {
        std::vector<cv::Point2d> convertedPoints;
        convertedPoints.reserve(points->Length);
        for each (UnitySCSharedAlgosOpenCVWrapper::Point2d^ point in points) 
        {
            convertedPoints.push_back(cv::Point2d(point->X, point->Y));
        }
        return convertedPoints;
    }
}

namespace UnitySCSharedAlgosOpenCVWrapper
{
    TransformationParameters^ PointsDetector::OptimalTransformationParameters(array<Point2d^>^ from, array<Point2d^>^ to)
    {
        std::vector<cv::Point2d> inputFrom = ConvertPoints(from);
        std::vector<cv::Point2d> inputTo = ConvertPoints(to);

        try 
        {
            matrix_2D::TransformationParameters internalResult = matrix_2D::OptimalTransformationParameters(inputFrom, inputTo);

            TransformationParameters^ result = gcnew TransformationParameters();
            result->RotationRad = internalResult.RotationRad;
            result->Scale = internalResult.Scale;
            result->Translation = gcnew Point2d(internalResult.Translation.x, internalResult.Translation.y);
            return result;
        } 
        catch (std::exception ex)
        {
            std::cout << "Error during OptimalTransformationParameters: " << ex.what() << std::endl;
        }
        return nullptr;
    }
}