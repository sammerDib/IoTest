#include "CPhaseShiftingDeflectometry.hpp"
#include "CPhaseShiftingInterferometry.hpp"
#include "GoldsteinUnwrap.hpp"
#include "GeneralizedPhaseMapping.hpp"
#include "C2DSignalAnalysis.hpp"
#include "ReportingUtils.hpp"
#include "CImageTypeConvertor.hpp"
#include "C2DSignalAnalysis.hpp"

using namespace std;
using namespace cv;
namespace fs = std::filesystem;

class ParallelDeriveXComputationForFloat : public ParallelLoopBody
{
private:
    Mat& phaseMap;
    Mat& phaseMap2;
    Mat& gradient;
    float max;
    float min;

public:
    ParallelDeriveXComputationForFloat(Mat& phaseMap, Mat& phaseMap2, Mat& gradient) : phaseMap(phaseMap), phaseMap2(phaseMap2), gradient(gradient)
    {
        max = static_cast<float>(CV_PI * 0.5);
        min = static_cast<float>(-CV_PI * 0.5);
    }

    virtual void operator()(const Range& range) const CV_OVERRIDE
    {
        for (int r = range.start; r < range.end; r++)
        {
            int i = r / phaseMap.cols;
            int j = (r % phaseMap.cols);

            if (i == 0 || i >= phaseMap.rows - 1) {
                gradient.ptr<float>(i)[j] = 0;
            }
            else {
                int phaseInterval = (min < phaseMap.ptr<float>(i)[j] && phaseMap.ptr<float>(i)[j] < max);
                if (phaseInterval == 1) {
                    gradient.ptr<float>(i)[j] = phaseMap.ptr<float>(i - 1)[j] - phaseMap.ptr<float>(i + 1)[j];
                }
                else {
                    gradient.ptr<float>(i)[j] = phaseMap2.ptr<float>(i - 1)[j] - phaseMap2.ptr<float>(i + 1)[j];
                }
            }
        }
    }
};

class ParallelDeriveYComputationForFloat : public ParallelLoopBody
{
private:
    Mat& phaseMap;
    Mat& phaseMap2;
    Mat& gradient;
    float max;
    float min;

public:
    ParallelDeriveYComputationForFloat(Mat& phaseMap, Mat& phaseMap2, Mat& gradient) : phaseMap(phaseMap), phaseMap2(phaseMap2), gradient(gradient)
    {
        max = static_cast<float>(CV_PI * 0.5);
        min = static_cast<float>(-CV_PI * 0.5);
    }

    virtual void operator()(const Range& range) const CV_OVERRIDE
    {
        for (int r = range.start; r < range.end; r++)
        {
            int i = r / phaseMap.cols;
            int j = (r % phaseMap.cols);

            if (j == 0 || j >= phaseMap.cols - 1) {
                gradient.ptr<float>(i)[j] = 0;
            }
            else {
                int phaseInterval = (min < phaseMap.ptr<float>(i)[j] && phaseMap.ptr<float>(i)[j] < max);
                if (phaseInterval == 1) {
                    gradient.ptr<float>(i)[j] = phaseMap.ptr<float>(i)[j - 1] - phaseMap.ptr<float>(i)[j + 1];
                }
                else {
                    gradient.ptr<float>(i)[j] = phaseMap2.ptr<float>(i)[j - 1] - phaseMap2.ptr<float>(i)[j + 1];
                }
            }
        }
    }
};

class ParallelCurvatureDynamicComputation : public ParallelLoopBody
{
private:
    Mat& initialImg;
    Mat& calibratedImg;
    Mat& mask;
    float calibrationDynamicCoef;
    float noisyGrayLevel;
    float userDynamicCoef;

public:
    ParallelCurvatureDynamicComputation(Mat& initialImg, Mat& calibratedImg, Mat& mask, float calibrationDynamicCoef, float noisyGrayLevel, float userDynamicCoef) : initialImg(initialImg), calibratedImg(calibratedImg), mask(mask), calibrationDynamicCoef(calibrationDynamicCoef), noisyGrayLevel(noisyGrayLevel), userDynamicCoef(userDynamicCoef)
    {
    }

    virtual void operator()(const Range& range) const CV_OVERRIDE
    {
        for (int r = range.start; r < range.end; r++)
        {
            int i = r / initialImg.cols;
            int j = (r % initialImg.cols);

            if (mask.ptr<uchar>(i)[j] != 0) {
                // Using dynamics calibration to adjust background noise: scaling based on the calibrated noise level for current machine, that we want to reach the value of noiseGrayLevel in the final image.
                float calibratedValue = initialImg.ptr<float>(i)[j] / calibrationDynamicCoef * noisyGrayLevel / userDynamicCoef + 128.0f;
                calibratedValue = std::fmax(calibratedValue, 0.0f);
                calibratedValue = std::fmin(calibratedValue, 255.0f);
                calibratedImg.ptr<uchar>(i)[j] = (uchar)std::max(3, (int)calibratedValue);
            }
            else {
                calibratedImg.ptr<uchar>(i)[j] = 0;
            }
        }
    }
};

class ParallelDarkDynamicComputation : public ParallelLoopBody
{
private:
    Mat& dark;
    Mat& finalDark;
    Mat& mask;
    float dynamicCoef;
    float shift;

public:
    ParallelDarkDynamicComputation(Mat& dark, Mat& mask, Mat& finalDark, float dynamicCoef, float shift) : dark(dark), mask(mask), finalDark(finalDark), dynamicCoef(dynamicCoef), shift(shift) {
    }

    virtual void operator()(const Range& range) const CV_OVERRIDE
    {
        for (int r = range.start; r < range.end; r++)
        {
            int i = r / dark.cols;
            int j = (r % dark.cols);

            if (mask.ptr<uchar>(i)[j] != 0) {
                // Using dynamics calibration to adjust background noise: scaling based on the calibrated noise level for current machine, that we want to reach the value of noiseGrayLevel in the final image.
                finalDark.ptr<uchar>(i)[j] = (unsigned char)(std::min((int)std::max(floor((dark.ptr<float>(i)[j] + shift + 0.5f) * dynamicCoef), 0.0f), 255));
            }
            else {
                finalDark.ptr<uchar>(i)[j] = 0;
            }
        }
    }
};

#pragma unmanaged
namespace psd {
    namespace {
        Mat ComputeCurvature(Mat& wrappedPhaseMap, Mat& wrappedPhaseMap2, FringesDisplacement fringesDisplacement);
        void ApplyMaskAndLevel(Mat& curvature, Mat& mask);
        void WriteReport(WrappedPhaseMap psdResult, fs::path directoryPathToStoreReport, FringesDisplacement fringesDirection);
    }

    WrappedPhaseMap ComputePhaseMap(std::vector<cv::Mat> imgs, int stepNb, FringesDisplacement fringesDirection, std::filesystem::path directoryPathToStoreReport)
    {
        generalized_phase_mapping::Precision precision = generalized_phase_mapping::Low;
        if (imgs[0].depth() == CV_32F)
        {
            precision = generalized_phase_mapping::High;
        }

        auto wrappedPhaseMap = generalized_phase_mapping::GeneralizedPhaseMapping(imgs, stepNb, precision);

        if (directoryPathToStoreReport.string() != "")
        {
            WriteReport(wrappedPhaseMap, directoryPathToStoreReport, fringesDirection);
        }

        return wrappedPhaseMap;
    }

    cv::Mat ComputeMask(WrappedPhaseMap phaseMap, double pixelSizeInMicrons, double waferDiameterInMicrons, double waferShiftXInMicrons, double waferShiftYInMicrons, bool useWaferFill, double fillEdgeExclusionInMicrons, float amplitudeMinCurvature, float intensityMinCurvature, std::filesystem::path directoryPathToStoreReport)
    {
        Size size = phaseMap.Phase.size();
        Mat mask = Mat::zeros(size, CV_8UC1);

        cv::bitwise_and(phaseMap.Amplitude > amplitudeMinCurvature, phaseMap.Background > intensityMinCurvature, mask);

        Mat dilatedMask, erodedMask;
        int morphSize = 3;
        Mat morphElement = getStructuringElement(MORPH_ELLIPSE,
            Size(2 * morphSize + 1, 2 * morphSize + 1),
            Point(morphSize, morphSize));
        dilate(mask, dilatedMask, morphElement);
        erode(dilatedMask, erodedMask, morphElement);
       
        bool imageIsSquare = erodedMask.cols == erodedMask.rows;

        if (imageIsSquare && useWaferFill)
        {
            int waferRadiusInPixels = (int)(waferDiameterInMicrons / pixelSizeInMicrons) / 2;
            int edgeExclusionInPixels = (int)(fillEdgeExclusionInMicrons / pixelSizeInMicrons);

            int shiftXInPixels = (int)(waferShiftXInMicrons / pixelSizeInMicrons);
            int shiftYInPixels = (int)(waferShiftYInMicrons / pixelSizeInMicrons);

            int waferXInImageReferential = shiftXInPixels + erodedMask.cols / 2;
            int waferYInImageReferential = shiftYInPixels + erodedMask.rows / 2;

            cv::circle(erodedMask, cv::Point(waferXInImageReferential, waferYInImageReferential), waferRadiusInPixels - edgeExclusionInPixels, cv::Scalar(255), -1);
        }

        if (directoryPathToStoreReport.string() != "")
        {
            Reporting::writePngImage(erodedMask, directoryPathToStoreReport.string() + "\\Mask.png");
        }

        return erodedMask;
    }

    cv::Mat ComputeCurvature(WrappedPhaseMap phaseMap, cv::Mat& mask, int stepNb, FringesDisplacement fringesDisplacement, std::filesystem::path directoryPathToStoreReport)
    {
        cv::Mat curvatureMap = ComputeCurvature(phaseMap.Phase, phaseMap.Phase2, fringesDisplacement);
        ApplyMaskAndLevel(curvatureMap, mask);
        cv::GaussianBlur(curvatureMap, curvatureMap, Size(3, 3), 3);

        if (directoryPathToStoreReport.string() != "")
        {
            string subfolder = fringesDisplacement == X ? "\\X" : "\\Y";
            filesystem::create_directories(directoryPathToStoreReport.string() + subfolder);
            Reporting::writePngImage(curvatureMap, directoryPathToStoreReport.string() + subfolder + "\\CurvatureMap.png");
        }

        return curvatureMap;
    }

    cv::Mat ComputeDark(cv::Mat& darkX, cv::Mat& darkY, cv::Mat& mask, FitSurface removeBackgroundSurfaceMethod, std::filesystem::path directoryPathToStoreReport)
    {
        cv::Mat dark = (darkX + darkY) / 2;

        if (directoryPathToStoreReport.string() != "")
        {
            Reporting::writePngImage(dark, directoryPathToStoreReport.string() + "\\Dark.png");
        }

        if (removeBackgroundSurfaceMethod == FitSurface::None)
        {
            return dark;
        }

        // Downsampling
        Size size = dark.size();
        int sampleRatio = 5;
        int sampleSizeWidth = std::max(size.width / sampleRatio, 10);
        int sampleSizeHeight = std::max(size.height / sampleRatio, 10);

        Size downsampleSize = cv::Size(sampleSizeWidth, sampleSizeHeight);
        Mat downsampledDark = Mat::zeros(downsampleSize, CV_32FC1);
        Mat downsampledMask = Mat::zeros(downsampleSize, CV_8UC1);
        cv::resize(dark, downsampledDark, downsampleSize, 0, 0, INTER_AREA);
        cv::resize(mask, downsampledMask, downsampleSize, 0, 0, INTER_AREA);
        cv::threshold(downsampledMask, downsampledMask, 0.5, 1, THRESH_BINARY);

        // Fit polynom
        cv::Mat polynomialSurface;
        switch (removeBackgroundSurfaceMethod) {
        case FitSurface::PolynomeOrder2:
            polynomialSurface = signal_2D::SolveQuadraticEquation(downsampledDark, downsampledMask);
            break;
        case FitSurface::PolynomeOrder3:
            polynomialSurface = signal_2D::SolveCubicEquation(downsampledDark, downsampledMask);
            break;
        case FitSurface::PolynomeOrder4:
            polynomialSurface = signal_2D::SolveQuarticEquation(downsampledDark, downsampledMask);
            break;
        }

        // Resize
        cv::Mat fullSizePolynomialSurface;
        cv::resize(polynomialSurface, fullSizePolynomialSurface, size, 0, 0, INTER_LINEAR);
        Mat darkWithoutPolynomialBackground;
        cv::subtract(dark, fullSizePolynomialSurface, darkWithoutPolynomialBackground, mask);

        if (directoryPathToStoreReport.string() != "")
        {
            Reporting::writePngImage(polynomialSurface, directoryPathToStoreReport.string() + "\\PolynomialSurface.png");
            Reporting::writePngImage(darkWithoutPolynomialBackground, directoryPathToStoreReport.string() + "\\DarkWithoutPolynomialBackground.png");
        }

        return darkWithoutPolynomialBackground;
    }

    float CalibrateCurvatureDynamics(const cv::Mat& curvatureX, const cv::Mat& curvatureY, const cv::Mat& mask)
    {
        Mat curvatureXMasked;
        Mat curvatureYMasked;
        curvatureX.copyTo(curvatureXMasked, mask);
        curvatureY.copyTo(curvatureYMasked, mask);

        // Filtering to remove the abnormal pixels (patterns and dust) and keep only the background.
        int countValueInsideMask = cv::countNonZero(mask);
        float meanInMaskX = cv::sum(cv::abs(curvatureX))[0] / countValueInsideMask;
        float meanInMaskY = cv::sum(cv::abs(curvatureY))[0] / countValueInsideMask;

        // Standard deviation of the filtered background

        cv::Mat maskX = curvatureX > -3.0f * meanInMaskX & curvatureX < 3.0f * meanInMaskX;
        cv::Mat maskY = curvatureY > -3.0f * meanInMaskY & curvatureY < 3.0f * meanInMaskY;
        Mat curvatureXFiltered;
        Mat curvatureYFiltered;
        curvatureX.copyTo(curvatureXFiltered, maskX);
        curvatureY.copyTo(curvatureYFiltered, maskY);

        int countX = cv::countNonZero(curvatureXFiltered);
        int countY = cv::countNonZero(curvatureYFiltered);
        float filteredMeanX = cv::sum(curvatureXFiltered)[0] / countX;
        float filteredMeanY = cv::sum(curvatureYFiltered)[0] / countY;
        cv::Mat curvatureXPow = cv::Mat::zeros(curvatureXFiltered.size(), curvatureXFiltered.type());
        cv::Mat curvatureYPow = cv::Mat::zeros(curvatureYFiltered.size(), curvatureYFiltered.type());
        cv::pow(curvatureXFiltered, 2, curvatureXPow);
        cv::pow(curvatureYFiltered, 2, curvatureYPow);
        float filteredSquaredMeanX = cv::sum(curvatureXPow)[0] / countY;
        float filteredSquaredMeanY = cv::sum(curvatureYPow)[0] / countX;

        // Average standard deviation
        float stdX = sqrt(filteredSquaredMeanX - filteredMeanX * filteredMeanX);
        float stdY = sqrt(filteredSquaredMeanY - filteredMeanY * filteredMeanY);
        float stdev = 0.5f * (stdX + stdY);

        return stdev;
    }

    cv::Mat ApplyDynamicCalibration(cv::Mat& img, cv::Mat& mask, float calibrationDynamicCoef, float noisyGrayLevel, float userDynamicCoef, std::filesystem::path directoryPathToStoreReport)
    {
        Size size = img.size();
        Mat calibratedCurvature = Mat::zeros(size, CV_8UC1);

        ParallelCurvatureDynamicComputation obj(img, calibratedCurvature, mask, calibrationDynamicCoef, noisyGrayLevel, userDynamicCoef);
        parallel_for_(Range(0, img.rows * img.cols), obj);

        cv::GaussianBlur(img, img, Size(3, 3), 3, 3);

        if (directoryPathToStoreReport.string() != "")
        {
            Reporting::writePngImage(calibratedCurvature, directoryPathToStoreReport.string() + "\\CalibratedCurvature.png");
        }

        return calibratedCurvature;
    }

    cv::Mat ApplyDynamicCoefficient(cv::Mat& dark, cv::Mat& mask, float dynamicCoef, float percentageOfLowSaturation, std::filesystem::path directoryPathToStoreReport) {
        Size size = dark.size();

        double min = std::numeric_limits<double>::max();
        double max = std::numeric_limits<double>::min();
        cv::Point min_loc, max_loc;
        cv::minMaxLoc(dark, &min, &max, &min_loc, &max_loc, mask);

        Mat calibratedDark = Mat::zeros(size, CV_8UC1);
        bool darkIsEmpty = (max - min < 0);
        if (darkIsEmpty)
        {
            return calibratedDark;
        }

        int histSize = (int)floor(max - min) + 2;
        vector<int> histogram(histSize, 0);

        //Size size = dark.size();

        int maskPixelCount = 0;
        for (int r = 0; r < size.height; r++) {
            for (int c = 0; c < size.width; c++) {
                if (mask.at<uchar>(r, c) != 0)
                {
                    histogram[(int)std::floor(dark.at<float>(r, c) - min + 0.5f)]++;
                    maskPixelCount++;
                }
            }
        }

        // Looking for the limit of the percentageOfLowSaturation of lowest values
        unsigned int sum = 0;
        unsigned int count = 0;
        while (sum < (unsigned int)floor(percentageOfLowSaturation * maskPixelCount))
        {
            sum += histogram[count];
            count++;
        }
        float shift = static_cast<float>(-1.0 * ((double)count + min));

        // Final shift and conversion to uchar

        ParallelDarkDynamicComputation obj(dark, mask, calibratedDark, 1 / dynamicCoef, shift);
        parallel_for_(Range(0, dark.rows * dark.cols), obj);

        if (directoryPathToStoreReport.string() != "")
        {
            Reporting::writePngImage(calibratedDark, directoryPathToStoreReport.string() + "\\CalibratedDark.png");
        }

        return calibratedDark;
    }

    namespace {
        Mat ComputeCurvature(Mat& wrappedPhaseMap, Mat& wrappedPhaseMap2, FringesDisplacement fringesDisplacement)
        {
            //float max = CV_PI / 2; //not used
            //float min = -CV_PI / 2;  //not used

            Size size = wrappedPhaseMap.size();
            Mat gradient = Mat::zeros(size, CV_32FC1);

            if (fringesDisplacement == X)
            {
                ParallelDeriveYComputationForFloat obj(wrappedPhaseMap, wrappedPhaseMap2, gradient);
                parallel_for_(Range(0, wrappedPhaseMap.rows * wrappedPhaseMap.cols), obj);
            }
            else {
                ParallelDeriveXComputationForFloat obj(wrappedPhaseMap, wrappedPhaseMap2, gradient);
                parallel_for_(Range(0, wrappedPhaseMap.rows * wrappedPhaseMap.cols), obj);
            }

            return gradient;
        }

        void ApplyMaskAndLevel(Mat& curvature, Mat& mask)
        {
            // Replacing the Degauchi by a simple application of mask, then bringing all data to a mean of 0.
            // Computing the average in the mask
            int count = 0;
            double sum = 0.0;

            Scalar mean = cv::mean(curvature, mask);
            float graylevelMean = (float) mean[0];

            Size size = curvature.size();

            for (int r = 0; r < size.height; r++) {
                for (int c = 0; c < size.width; c++) {
                    if (mask.at<uchar>(r, c) == 0)
                    {
                        curvature.at<float>(r, c) = 0;
                    }
                    else {
                        curvature.at<float>(r, c) -= graylevelMean;
                    }
                }
            }
        }

        void WriteReport(WrappedPhaseMap wrappedPhaseMap, fs::path directoryPathToStoreReport, FringesDisplacement fringesDisplacement)
        {
            fs::create_directory(directoryPathToStoreReport);
            string subfolder = fringesDisplacement == X ? "\\X" : "\\Y";
            filesystem::create_directories(directoryPathToStoreReport.string() + subfolder);

            Reporting::writePngImage(wrappedPhaseMap.Phase, directoryPathToStoreReport.string() + subfolder + "\\WrappedPhaseMap.png");
            Reporting::writePngImage(wrappedPhaseMap.Phase2, directoryPathToStoreReport.string() + subfolder + "\\WrappedPhaseMap2.png");
            Reporting::writePngImage(wrappedPhaseMap.Background, directoryPathToStoreReport.string() + subfolder + "\\Background.png");
            Reporting::writePngImage(wrappedPhaseMap.Amplitude, directoryPathToStoreReport.string() + subfolder + "\\Amplitude.png");
        }
    }
}