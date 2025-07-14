#include "CImageOperators.hpp"

#include "ImageOperators.h"
#include "Tools.h"

#pragma managed
using namespace System;

namespace UnitySCSharedAlgosOpenCVWrapper {

    double ImageOperators::StandardDeviation(ImageData^ img)
    {
        cv::Mat imgMat = CreateMatFromImageData(img);
        double stdDev = img_operators::StandardDeviation(imgMat);
        return stdDev;
    }

    int ImageOperators::GrayscaleMedianComputation(ImageData^ img) {
        cv::Mat imgMat = CreateMatFromImageData(img);
        auto grayscaleMedian = img_operators::GrayscaleMedian(imgMat);
        return grayscaleMedian;
    }

    double ImageOperators::VignettingOperator(ImageData^ img, int period) {
        cv::Mat imgMat = CreateMatFromImageData(img);
        auto vignettingValue = img_operators::standartDeviationZoneByZone(imgMat, period);
        return vignettingValue;
    }

    ImageData^ ImageOperators::SaturatedNormalization(ImageData^ img, int lowerIntensity, int upperIntensity)
    {
        // Process input parameters
        cv::Mat imgMat = CreateMatFromImageData(img);

        // Call native method
        imgMat = img_operators::SaturatedNormalization(imgMat, lowerIntensity, upperIntensity);
        // Process output result
        ImageData^ normalizedImg = gcnew ImageData();
        normalizedImg->ByteArray = CreateByteArrayFromMat(imgMat);
        normalizedImg->Type = CreateImageType(imgMat);
        normalizedImg->Height = img->Height;
        normalizedImg->Width = img->Width;

        return normalizedImg;
    }

    double ImageOperators::FocusMeasurement(ImageData^ img, FocusMeasureMethod method) {
        cv::Mat imgMat = CreateMatFromImageData(img);
        switch (method) {
        case FocusMeasureMethod::TenenbaumGradient:
            return img_operators::TenenbaumGradient(imgMat);
        case FocusMeasureMethod::VarianceOfLaplacian:
            return img_operators::VarianceOfLaplacian(imgMat);
        case FocusMeasureMethod::SumOfModifiedLaplacian:
            return img_operators::SumOfModifiedLaplacian(imgMat);
        case FocusMeasureMethod::VollathF4:
            return img_operators::VollathF4(imgMat);
        case FocusMeasureMethod::NormalizedVariance:
            return img_operators::NormalizedVariance(imgMat);
        }
        return -1.0;
    }

    double ImageOperators::ContrastMeasurement(ImageData^ img) {
        cv::Mat imgMat = CreateMatFromImageData(img);
        double contrastValue = img_operators::NormalizedVariance(imgMat);
        return contrastValue;
    }

    double ImageOperators::SaturationMeasurement(ImageData^ img) {
        cv::Mat imgMat = CreateMatFromImageData(img);
        double saturationValue = img_operators::Saturation(imgMat);
        return saturationValue;
    }

    array<Point2d^>^ ImageOperators::ExtractIntensityProfile(ImageData^ img, Point2i^ startPixel, Point2i^ endPixel) {
        cv::Mat imgMat = CreateMatFromImageData(img);
        cv::Point2i startPixelOpenCV = cv::Point2i(startPixel->X, startPixel->Y);
        cv::Point2i endPixelOpenCV = cv::Point2i(endPixel->X, endPixel->Y);
        std::vector<cv::Point2d> profile = img_operators::ExtractIntensityProfile(imgMat, startPixelOpenCV, endPixelOpenCV);

        array<Point2d^>^ dataArray = gcnew array<Point2d^>((int)profile.size());
        for (size_t i = 0; i < profile.size(); i++)
        {
            dataArray[(int)i] = gcnew Point2d(profile.at(i).x, profile.at(i).y);
        }
        return dataArray;
    }

    ImageData^ ImageOperators::Resize(ImageData^ img, RegionOfInterest^ roi, double scale) {
        cv::Mat imgMat = CreateMatFromImageData(img);
        cv::Rect roiOpenCV(roi->X, roi->Y, (int)roi->Width, (int)roi->Height);
        cv::Mat resizedMat = img_operators::Resize(imgMat, roiOpenCV, scale);

        ImageData^ resizedImg = gcnew ImageData();
        resizedImg->ByteArray = CreateByteArrayFromMat(resizedMat);
        resizedImg->Type = CreateImageType(resizedMat);
        resizedImg->Height = resizedMat.rows;
        resizedImg->Width = resizedMat.cols;

        return resizedImg;
    }

    array<float>^ ImageOperators::CalculateHistogram(ImageData^ img, int binsNb)
    {
        return ImageOperators::CalculateHistogram(img, (ImageData^) nullptr, binsNb);
    }

    array<float>^ ImageOperators::CalculateHistogram(ImageData^ img, ImageData^ mask, int binsNb)
    {
        cv::Mat imgMat = CreateMatFromImageData(img);
        cv::Mat maskMat;
        if (mask == (ImageData^) nullptr)
        {
            maskMat = cv::Mat();
        }
        else
        {
            maskMat = CreateMatFromImageData(mask);
        }

        std::vector<float> hist = img_operators::CalculateHistogram(imgMat, maskMat, binsNb);

        array<float>^ histArray = gcnew array<float>((int)hist.size());
        for (size_t i = 0; i < hist.size(); i++)
        {
            histArray[(int)i] = hist.at(i);
        }
        return histArray;
    }
    
    double ImageOperators::ComputeGreyLevelSaturation(ImageData^ img, float acceptablePercentage)
    {
        return ImageOperators::ComputeGreyLevelSaturation(img, (ImageData^) nullptr, acceptablePercentage);
    }
    
    double ImageOperators::ComputeGreyLevelSaturation(ImageData^ img, ImageData^ mask, float acceptablePercentage)
    {
        cv::Mat imgMat = CreateMatFromImageData(img);
        cv::Mat maskMat;
        if (mask == (ImageData^) nullptr)
        {
            maskMat = cv::Mat();
        }
        else
        {
            maskMat = CreateMatFromImageData(mask);
        }
        try
        {
            return img_operators::ComputeGreyLevelSaturation(imgMat, maskMat, acceptablePercentage);
        }
        catch (const std::exception& e)
        {
            HandleAndRethrowCppException(e);
        }

        return -1.0; // fix : warning C4715:  not all control paths return a value
    }

    array<Point2i^>^ ImageOperators::FindPixelCoordinatesByThresholding(ImageData^ img, float threshold, ThresholdType type)
    {
        cv::Mat imgMat = CreateMatFromImageData(img);

        std::vector<cv::Point> coordinates;
        switch (type) {
        case ThresholdType::StrictlyAboveThreshold:
            coordinates = img_operators::FindPixelCoordinates(imgMat, img_operators::StrictlyAboveThreshold, threshold);
            break;
        case ThresholdType::StrictlyBelowThreshold:
            coordinates = img_operators::FindPixelCoordinates(imgMat, img_operators::StrictlyBelowThreshold, threshold);
            break;
        case ThresholdType::AboveOrEqualThreshold:
            coordinates = img_operators::FindPixelCoordinates(imgMat, img_operators::AboveOrEqualThreshold, threshold);
            break;
        case ThresholdType::BelowOrEqualThreshold:
            coordinates = img_operators::FindPixelCoordinates(imgMat, img_operators::BelowOrEqualThreshold, threshold);
            break;
        }

        array<Point2i^>^ coordinateArray = gcnew array<Point2i^>((int)coordinates.size());
        for (size_t i = 0; i < coordinates.size(); i++)
        {
            coordinateArray[(int)i] = gcnew Point2i(coordinates.at(i).x, coordinates.at(i).y);
        }
        return coordinateArray;
    }

    array<Point2i^>^ ImageOperators::FindPixelCoordinatesByRange(ImageData^ img, float minThreshold, float maxThreshold)
    {
        cv::Mat imgMat = CreateMatFromImageData(img);

        std::vector<cv::Point> coordinates = img_operators::FindPixelCoordinates(imgMat, img_operators::InsideRange, minThreshold, maxThreshold);

        array<Point2i^>^ coordinateArray = gcnew array<Point2i^>((int)coordinates.size());
        for (size_t i = 0; i < coordinates.size(); i++)
        {
            coordinateArray[(int)i] = gcnew Point2i(coordinates.at(i).x, coordinates.at(i).y);
        }
        return coordinateArray;
    }

    ImageData^ ImageOperators::RootSumOfSquares32BitImage(ImageData^ firstImage, ImageData^ secondImage)
    {
        cv::Mat firstImageMat = CreateMatFromImageData(firstImage);
        cv::Mat secondImageMat = CreateMatFromImageData(secondImage);

        cv::Mat combinedMat = img_operators::RootSumOfSquares32BitImage(firstImageMat, secondImageMat);

        ImageData^ combined = gcnew ImageData();
        combined->ByteArray = CreateByteArrayFromMat(combinedMat);
        combined->Type = CreateImageType(combinedMat);
        combined->Height = combinedMat.size().height;
        combined->Width = combinedMat.size().width;

        return combined;
    }
}