#include "CImageOperators.hpp"
#include "CImageTypeConvertor.hpp"
#include "ErrorLogging.hpp"
#include <iostream>
#include <fstream>

#pragma unmanaged

using namespace cv;
using namespace std;

class ParallelRootSumOfSquares32BitImage : public ParallelLoopBody
{
    private:
        const Mat& firstImg;
        const Mat& secondImg;
        Mat& combinedImg;

    public:
        ParallelRootSumOfSquares32BitImage(const Mat& firstImg, const Mat& secondImg, Mat& combinedImg) : firstImg(firstImg), secondImg(secondImg), combinedImg(combinedImg)
        {
        }

        virtual void operator()(const Range& range) const CV_OVERRIDE
        {
            for (int r = range.start; r < range.end; r++)
            {
                int i = r / combinedImg.cols;
                int j = (r % combinedImg.cols);

                float x = firstImg.ptr<float>(i)[j];
                float y = secondImg.ptr<float>(i)[j];

                combinedImg.ptr<float>(i)[j] = sqrt(x * x + y * y);
            }
        }
};

namespace img_operators {

    double StandardDeviation(cv::Mat img)
    {
        cv::Scalar stdDev;
        cv::meanStdDev(img, cv::noArray(), stdDev);
        return stdDev[0];
    }

    int GrayscaleMedian(cv::Mat channel) {
        int histSize = (channel.depth() == CV_8U) ? 256 : 65536;
        float range[] = { 0, (float)histSize };
        const float* histRange = { range };
        bool uniform = true; bool accumulate = false;
        cv::Mat hist;

        double middleIndex = (channel.rows * channel.cols) / 2;
        int bin = 0;
        int med = 0;

        cv::calcHist(&channel, 1, 0, cv::Mat(), hist, 1, &histSize, &histRange, uniform, accumulate);

        for (int i = 0; i < histSize; ++i)
        {
            bin += cvRound(hist.at< float >(i));
            if (bin > middleIndex)
            {
                med = i;
                return med;  
            }
        }

        return -1;
    }


    double standartDeviationZoneByZone(const cv::Mat& img, int zoneSize) {
        int height = img.rows;
        int width = img.cols;

        std::vector<double> zone_std_devs;

        //Divide the image into zones
        std::vector<cv::Mat> zones;
        for (int i = 0; i < height - zoneSize; i += zoneSize) {
            for (int j = 0; j < width - zoneSize; j += zoneSize) {
                cv::Rect roi(j, i, zoneSize, zoneSize);
                cv::Mat zoneMat = img(roi);
                cv::Scalar  stddev;
                cv::meanStdDev(zoneMat, cv::noArray(), stddev);
                zone_std_devs.push_back(stddev[0]);

            }
        }

        // Calculate the global standard deviation of the new image using the vector
        cv::Scalar global_std_dev;
        cv::meanStdDev(zone_std_devs, cv::noArray(), global_std_dev);
        return global_std_dev[0];
    }

    double TenenbaumGradient(const Mat& img, int kernelSize) {
        Mat gradientX, gradientY;
        Sobel(img, gradientX, CV_64F, 1, 0, kernelSize);
        Sobel(img, gradientY, CV_64F, 0, 1, kernelSize);

        Mat tenengrad = gradientX.mul(gradientX) + gradientY.mul(gradientY);
        double focusMeasure = mean(tenengrad).val[0];
        return focusMeasure;
    }

    double SumOfModifiedLaplacian(const cv::Mat& img)
    {
        Mat M = (Mat_<double>(3, 1) << -1, 2, -1);
        Mat G = getGaussianKernel(3, -1, CV_64F);

        Mat laplacianX;
        sepFilter2D(img, laplacianX, CV_64F, M, G);

        Mat laplacianY;
        sepFilter2D(img, laplacianY, CV_64F, G, M);

        Mat modifiedLaplacian = abs(laplacianX) + abs(laplacianY);
        double focusMeasure = mean(modifiedLaplacian).val[0];
        return focusMeasure;
    }

    double VarianceOfLaplacian(const Mat& img) {
        Mat laplacian;
        Laplacian(img, laplacian, CV_64F);

        Scalar mean, stddev;
        meanStdDev(img, mean, stddev);

        double focusMeasure = stddev.val[0] * stddev.val[0];
        return focusMeasure;
    }

    double VollathF4(const cv::Mat& img)
    {
        cv::Mat firstAutocorrelation;
        cv::matchTemplate(img.rowRange(0, img.rows - 2).colRange(0, img.cols), img.rowRange(1, img.rows - 1).colRange(0, img.cols), firstAutocorrelation, cv::TM_CCOEFF);
        cv::Mat secondAutocorrelation;
        cv::matchTemplate(img.rowRange(0, img.rows - 2).colRange(0, img.cols), img.rowRange(2, img.rows).colRange(0, img.cols), secondAutocorrelation, cv::TM_CCOEFF);
        cv::Mat correlationSubstract;
        cv::subtract(firstAutocorrelation, secondAutocorrelation, correlationSubstract);

        double focusMeasure = mean(correlationSubstract).val[0];

        return focusMeasure;
    }

    double NormalizedVariance(const Mat& img) {
        Scalar mean, stddev;
        meanStdDev(img, mean, stddev);

        double focusMeasure = (stddev.val[0] * stddev.val[0]) / mean.val[0];
        return focusMeasure;
    }

    double Saturation(const Mat& img) {
        Mat img_bgr = Convertor::ConvertTo8UC3(img);
        Mat img_hsv;
        cvtColor(img_bgr, img_hsv, COLOR_BGR2HSV);

        vector<Mat> channels;
        split(img_hsv, channels);

        double saturationMeasure = mean(channels[2]).val[0] / 250;
        return saturationMeasure;
    }

    vector<Point2d> ExtractIntensityProfile(const Mat& img, Point2i startPixel, Point2i endPixel) {
        vector<Point2d> profile;
        if (startPixel == endPixel) {
            return profile;
        }

        int imgDepth = img.depth();

        LineIterator line_iterator(img, startPixel, endPixel);

        for (size_t i = 0; i < line_iterator.count; i++, ++line_iterator)
        {
            Point position = line_iterator.pos();

            switch (imgDepth)
            {
                case CV_8UC1:
                {
                    double pixelValue = img.at<uchar>(position);
                    profile.push_back(Point2d((double)i, pixelValue));
                    break;
                }   
                case CV_16UC1:
                {
                    double pixelValue = img.at<ushort>(position);
                    profile.push_back(Point2d((double)i, pixelValue));
                    break;
                }
                default:
                {
                    ErrorLogging::LogErrorAndThrow("[ImageOperators] Image type not supported (supported image types are CV_8UC1 & CV_16UC1).");
                    break;
                }
            }
        }
        return profile;
    }

    cv::Mat Resize(const cv::Mat& img, cv::Rect roi, double scale) {
        cv::Rect inRangeRoi = roi & cv::Rect(0, 0, img.cols, img.rows);
        if (inRangeRoi.height == 0 || inRangeRoi.width == 0) {
            inRangeRoi = cv::Rect(0, 0, img.cols, img.rows);
        }
        cv::Mat croppedImage = img(inRangeRoi);

        cv::Mat resizedImage;
        cv::resize(croppedImage, resizedImage, Size(), scale, scale);
        return resizedImage;
    }

    Mat SaturatedNormalization(const Mat& img, int lowerIntensity, int upperIntensity)
    {
        int maxValuePerPixel = 0;

        switch (img.depth())
        {
            case CV_8UC1:
                maxValuePerPixel = 255;
                break;
            case CV_16UC1:
                maxValuePerPixel = 65535;
                break;
            default:
                ErrorLogging::LogErrorAndThrow("[ImageOperators] Image type not supported (supported image types are CV_8UC1 & CV_16UC1).");
                break;
        }

        cv::Mat normalizedImg;

        double min;
        double max;
        minMaxLoc(img, &min, &max);

        if (lowerIntensity < 0 || upperIntensity > maxValuePerPixel)
        {
            ErrorLogging::LogErrorAndThrow("[ImageOperators] Lower intensity & upper intensity must be between 0 & the max of the bit depth.");
        }

        if (lowerIntensity > upperIntensity)
        {
            ErrorLogging::LogErrorAndThrow("[ImageOperators] Lower intensity must be less than upper intensity");
        }

        if (lowerIntensity < min && upperIntensity > max)
        {
            return img;
        }

        //convertTo : converts using alpha * x + beta
        double alpha = (double)maxValuePerPixel / (upperIntensity - lowerIntensity);
        double beta = -(double)(maxValuePerPixel * lowerIntensity) / (upperIntensity - lowerIntensity);
        img.convertTo(normalizedImg, -1, alpha, beta);

        return normalizedImg;
    }

    std::vector<float> CalculateHistogram(const cv::Mat& img, const cv::Mat& mask, int binsNb)
    {
        cv::Mat histogram;
        bool uniform = true;
        bool accumulate = false;

        if (!mask.empty())
        {
            if (mask.type() != CV_8UC1)
            {
                ErrorLogging::LogErrorAndThrow("[ImageOperators] Mask can only be an empty image or an unsigned 8-bit image");
            }

            if (mask.size != img.size)
            {
                ErrorLogging::LogErrorAndThrow("[ImageOperators] Mask must have the same size as the given image");
            }
        }
        

        if (img.type() == CV_8UC1)
        {
            int minPixelValue = 0;
            int maxPixelValue = 255;
            float range[] = { (float)minPixelValue,(float)(maxPixelValue + 1) };
            const float* histRange[] = { range };

            cv::calcHist(&img, 1, 0, mask, histogram, 1, &binsNb, histRange, uniform, accumulate);
        }
        else if (img.type() == CV_32FC1)
        {
            double minPixelValue = 0;
            double maxPixelValue = 1;
            minMaxLoc(img, &minPixelValue, &maxPixelValue, 0, 0);
            float range[] = { (float)minPixelValue, (float)(maxPixelValue + FLT_EPSILON) };
            const float* histRange[] = { range };
            cv::calcHist(&img, 1, 0, mask, histogram, 1, &binsNb, histRange, uniform, accumulate);
        }
        else {
            ErrorLogging::LogErrorAndThrow("[ImageOperators] Only unsigned 8-bit or flaot 32-bit images are supported for histogram calculation.");
        }

        std::vector<float>vecHist(histogram.begin<float>(), histogram.end<float>());
        return vecHist;
    }
    
    double ComputeGreyLevelSaturation(const cv::Mat& img, const cv::Mat& mask, float acceptablePercentageOfSaturatedPixels)
    {
        if (img.type() != CV_8UC1 && img.type() != CV_32FC1)
        {
            ErrorLogging::LogErrorAndThrow("[ImageOperators] Only unsigned 8-bit or float 32-bit images are supported for grey level saturation computation.");
        }

        int binsNumber = 256;
        
        if (img.type() == CV_32FC1)
        {
            double maxPixelValue;
            minMaxLoc(img, NULL, &maxPixelValue, 0, 0);
            binsNumber = static_cast<int>(maxPixelValue);
        }
        
        std::vector<float> vecHist = CalculateHistogram(img, mask, binsNumber);
        int lastPopulatedBin = (int)(vecHist.size() - 1);
        while (vecHist[lastPopulatedBin] == 0.0f)
        {
            lastPopulatedBin--;
        }

        int numberOfPixels;
        if (mask.empty())
        {
            numberOfPixels = img.cols * img.rows;
        }
        else
        {
            numberOfPixels = cv::countNonZero(mask);
        }
        
        int binNumber = lastPopulatedBin + 1;
        float saturatedPixelCount = 0;
        float maxPixelNumber = acceptablePercentageOfSaturatedPixels * (float)numberOfPixels;
        while(saturatedPixelCount < maxPixelNumber && binNumber > 0)
        {
            saturatedPixelCount += vecHist[binNumber - 1];
            binNumber--;
        }

        double imageSaturation = binNumber;
        if (binNumber != lastPopulatedBin)
        {
            double delta = saturatedPixelCount - maxPixelNumber;
            imageSaturation += delta / vecHist[binNumber];
        }
        
        return imageSaturation;
    }

    std::vector<Point> FindPixelCoordinates(const cv::Mat& img, ThresholdType type, float threshold1, float threshold2)
    {
        std::vector<Point> coordinates;
        cv::Mat mask;

        switch (type)
        {
        case AboveOrEqualThreshold:
            cv::threshold(img, mask, (threshold1 - 1.0E-7), 255, cv::ThresholdTypes::THRESH_BINARY);
            break;
        case BelowOrEqualThreshold:
            cv::threshold(img, mask, threshold1, 255, cv::ThresholdTypes::THRESH_BINARY_INV);
            break;
        case StrictlyAboveThreshold:
            cv::threshold(img, mask, threshold1, 255, cv::ThresholdTypes::THRESH_BINARY);
            break;
        case StrictlyBelowThreshold:
            cv::threshold(img, mask, (threshold1 - 1.0E-7), 255, cv::ThresholdTypes::THRESH_BINARY_INV);
            break;
        case InsideRange:
            cv::inRange(img, Scalar(threshold1), Scalar(threshold2), mask);
            break;
        }

        cv::findNonZero(mask, coordinates);

        return coordinates;
    }

    cv::Mat RootSumOfSquares32BitImage(const cv::Mat& firstImg, const cv::Mat& secondImg)
    {
        if (firstImg.type() != CV_32FC1 || secondImg.type() != CV_32FC1)
        {
            ErrorLogging::LogErrorAndThrow("[CPhaseShiftingDeflectometry] One of the provided unwrapped phase maps is not of the right type.");
        }

        if (firstImg.size() != secondImg.size())
        {
            ErrorLogging::LogErrorAndThrow("[CPhaseShiftingDeflectometry] The provided unwrapped phase maps should have the same size.");
        }

        Mat combined = Mat::zeros(firstImg.size(), CV_32FC1);

        ParallelRootSumOfSquares32BitImage obj(firstImg, secondImg, combined);
        parallel_for_(Range(0, combined.rows * combined.cols), obj);

        return combined;
    }
};  // namespace img_operators