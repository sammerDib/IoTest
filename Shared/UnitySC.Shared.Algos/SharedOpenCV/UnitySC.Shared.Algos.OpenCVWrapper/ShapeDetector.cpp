#include "ShapeDetector.h"

#pragma managed
using namespace System;

namespace UnitySCSharedAlgosOpenCVWrapper {
    namespace {
        array<Circle^>^ ConvertNativeCircleVectorToManagedCircleArray(std::vector<shape_finder::Circle>& dataVec) {
            const int size = (int)dataVec.size();
            array<Circle^>^ dataArray = gcnew array<Circle^>(size);
            for (int i = 0; i < size; i++) {
                dataArray[i] = gcnew Circle(gcnew Point2d(dataVec[i].CenterPos.x, dataVec[i].CenterPos.y), dataVec[i].Diameter);
            }
            return dataArray;
        }

        array<Ellipse^>^ ConvertNativeEllipseVectorToManagedEllipseArray(std::vector<shape_finder::Ellipse>& dataVec) {
            const int size = (int)dataVec.size();
            array<Ellipse^>^ dataArray = gcnew array<Ellipse^>(size);
            for (int i = 0; i < size; i++) {
                dataArray[i] = gcnew Ellipse(gcnew Point2d(dataVec[i].CenterPos.x, dataVec[i].CenterPos.y), dataVec[i].HeightAxis, dataVec[i].WidthAxis, dataVec[i].Angle);
            }
            return dataArray;
        }
    }

    CircleResult^ ShapeDetector::CircleDetect(ImageData^ img, CircleFinderParams^ params, RegionOfInterest^ regionOfInterest) {
        // Process input parameters
        cv::Mat image = CreateMatFromImageData(img);
        cv::Rect roi;
        if (regionOfInterest != nullptr)
        {
            roi = cv::Rect(regionOfInterest->X, regionOfInterest->Y, (int)regionOfInterest->Width, (int)regionOfInterest->Height);
        }

        // to do coralie/pierre : uniformisation Float/double dan sles params (managé en double mais native en float !!!! oO'
        shape_finder::CircleFinderParams parameters = shape_finder::CircleFinderParams(
            (float) params->DistBetweenCircles,
            (float) params->ApproximateDiameter,
            (float) params->DetectionTolerance,
            params->CannyThreshold, params->UseScharrAlgorithm, params->UseMorphologicialOperations);

        // Call native method
        cv::Mat preprocessedImg = shape_finder::PreprocessImageForCircleFinder(image, parameters);
        std::vector<shape_finder::Circle> results = shape_finder::CircleFinder(image, parameters, roi);

        // Process output result
        ImageData^ resultImage = gcnew ImageData();
        resultImage->ByteArray = CreateByteArrayFromMat(preprocessedImg);
        resultImage->Type = CreateImageType(preprocessedImg);
        resultImage->Height = img->Height;
        resultImage->Width = img->Width;

        CircleResult^ result = gcnew CircleResult();
        result->PreprocessedImage = resultImage;
        result->Circles = ConvertNativeCircleVectorToManagedCircleArray(results);

        return result;
    }

    EllipseResult^ ShapeDetector::EllipseDetect(ImageData^ img, EllipseFinderParams^ params, RegionOfInterest^ regionOfInterest) {
        // Process input parameters
        cv::Mat image = CreateMatFromImageData(img);
        cv::Rect roi;
        if (regionOfInterest != nullptr)
        {
            roi = cv::Rect(regionOfInterest->X, regionOfInterest->Y, (int) regionOfInterest->Width, (int) regionOfInterest->Height);
        }

        shape_finder::EllipseFinderParams parameters = shape_finder::EllipseFinderParams(
            std::pair<float, float>((float)params->ApproximateAxes->Item1, 
                                    (float)params->ApproximateAxes->Item2),
            (float) params->MajorAxisTolerance, 
            (float) params->MinorAxisTolerance,
            params->CannyThreshold);

        // Call native method
        cv::Mat preprocessedImg = shape_finder::PreprocessImageForEllipseFinder(image, parameters);
        std::vector<shape_finder::Ellipse> results = shape_finder::EllipseFinder(image, parameters, roi);

        // Process output result
        ImageData^ resultImage = gcnew ImageData();
        resultImage->ByteArray = CreateByteArrayFromMat(preprocessedImg);
        resultImage->Type = CreateImageType(preprocessedImg);
        resultImage->Height = img->Height;
        resultImage->Width = img->Width;

        EllipseResult^ result = gcnew EllipseResult();
        result->PreprocessedImage = resultImage;
        result->Ellipses = ConvertNativeEllipseVectorToManagedEllipseArray(results);

        return result;
    }
}