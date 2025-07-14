#ifndef NDEBUG
#include <opencv2/highgui.hpp>
#endif
#include <opencv2/imgproc.hpp>

#include "CEventQueue.hpp"
#include "PolarImageCropper.hpp"
#include "PolarTransform.hpp"
#include "SymmetryDetector.hpp"
#include "LoggerOCV.hpp"
#include <opencv2/video/tracking.hpp>

#pragma unmanaged
namespace {
    /*!
     * Centers images intensity values in 0, keeping standard deviation of initial image
     */
    cv::Mat Standardize(cv::Mat& imageToStandardize) {
        cv::Scalar mean, std;
        cv::meanStdDev(imageToStandardize, mean, std);
        double alpha = std[0]; // rescale data

        cv::Mat result = imageToStandardize.clone();
        result -= mean[0]; // center data
        result.convertTo(result, CV_32F, 1 / alpha, 0);
        return result;
    }
    cv::Mat Normalize0_1(cv::Mat& imageToStandardize) {
        cv::Mat result;
        imageToStandardize.convertTo(result, CV_32F);
        cv::normalize(result, result, 0, 1, cv::NORM_MINMAX);
        return result;
    }
#ifndef NDEBUG
    void Disp(std::string const& name, cv::Mat const& data) {
        cv::Mat toDisplay;
        data.copyTo(toDisplay);
        cv::normalize(toDisplay, toDisplay, 0, 255, cv::NORM_MINMAX);
        toDisplay.convertTo(toDisplay, CV_8U);
        cv::imshow(name, toDisplay);
    }
#else
    void Disp(std::string const& name, cv::Mat const& data) {}
#endif

    double Map(double val, double in_min, double in_max, double out_min, double out_max) { return (val - in_min) * (out_max - out_min) / (in_max - in_min) + out_min; };

} // namespace


double SymmetryDetector::ComputeAngleFromCorrelation(double minLocInCroppedPolarImage, std::vector<double> angles, cv::Size const& croppedPaddedPolar, cv::Size const& polar, cv::Rect const& croppedRegion) {

    assert(angles.size() == polar.height);

    LoggerOCV::Debug("[SymmetryDetector::ComputeAngleFromCorrelation] Received notch location on the cropped polar image: " + std::to_string(minLocInCroppedPolarImage));

    double rawAngleInRadians = std::numeric_limits<double>::quiet_NaN();

    if (minLocInCroppedPolarImage < croppedRegion.height && minLocInCroppedPolarImage > 0) {

        //Shifting the axis coordinates to the polar image referential
        double minLocInPolarImage = minLocInCroppedPolarImage + croppedRegion.y;
        double minYValueInCroppedImage = 0;
        double maxYValueInCroppedImage = croppedRegion.height;

        auto maxAngleIndex = angles.size() - 1;
        double minAngleValue = angles[std::min(maxAngleIndex, (size_t)std::ceil(minYValueInCroppedImage))];
        double maxAngleValue = angles[std::min(maxAngleIndex, (size_t)std::floor(maxYValueInCroppedImage))];

        // NOTE: using Map() will provide better accuracy than just getting a discrete angle using
        // an index in the angles vector `angles`
        rawAngleInRadians = Map(minLocInPolarImage, minYValueInCroppedImage, maxYValueInCroppedImage, minAngleValue, maxAngleValue);

        LoggerOCV::Debug("[SymmetryDetector::ComputeAngleFromCorrelation] Found angle in radians: " + std::to_string(rawAngleInRadians));

        LoggerOCV::Debug("[SymmetryDetector::ComputeAngleFromCorrelation] Found angle in degrees: " + std::to_string(rawAngleInRadians * 180 / CV_PI));
    }
    return rawAngleInRadians;
}

double SymmetryDetector::Detect(EdgeImage* const image, cv::Point2d waferCenter, int waferDiameterInMm, double& angleConfidence, std::string const& reportPath, double minErrorThreshold, double slopeThreshold) {

    cv::Mat topSide;
    cv::Mat _polarImage;

    auto histograms = PolarTransform::Transform(image, _polarImage, waferCenter);

    //Cropping out the black pixels from the polar transform
    cv::Rect croppedRegion;
    cv::Mat croppedPolar = PolarImageCropper::Crop(_polarImage, waferDiameterInMm, &croppedRegion);

    //Padding the image to help with the symmetry detection
    auto paddedShifted = SymmetryDetector::ImagePadder::PadImage(croppedPolar);
    paddedShifted.convertTo(paddedShifted, CV_32F);
    topSide = paddedShifted;

    //Flip image to obtain a right side
    static const int FLIP_ON_X_AXIS = 0;
    cv::Mat bottomSide;
    cv::flip(topSide, bottomSide, FLIP_ON_X_AXIS);



    std::vector<float> meanSquareErrors;

    int paddingSize = topSide.rows / 4;
    int windowRatio = 4;
    int windowSize = croppedPolar.rows / windowRatio;

    //Sliding through the top side using a window and computing the mean square error between the window and the flipped window
    for (int i = paddingSize + windowSize / 2; i < (paddingSize * 3) - windowSize / 2; i++)
    {
        cv::Mat topWindow = topSide(cv::Rect(0, i - windowSize / 2, croppedPolar.cols, windowSize));
        cv::Mat botWindow;
        cv::flip(topWindow, botWindow, FLIP_ON_X_AXIS);

        cv::Mat diffMat;
        cv::subtract(topWindow, botWindow, diffMat);
        float error = 0.0;
        cv::Mat powMat = diffMat.mul(diffMat);
        error = (float) cv::sum(powMat)[0];
        error /= diffMat.rows * diffMat.cols;

        meanSquareErrors.push_back(error);
    }

    double maxError;
    double minError;
    cv::Point minLoc;
    //Finding the Y location with the smallest error (highest similarity, highest chance of being a symmetry axis)
    cv::minMaxLoc(meanSquareErrors, &minError, &maxError, &minLoc);
    double minErrorNorm = minError / maxError;

    //Image rejected if the error on the detected axis is too big (meaning the similarity between the best window and flipped window was too low)
    if (minErrorNorm > minErrorThreshold)
    {
        angleConfidence = 0.0;
        return std::numeric_limits<double>::quiet_NaN();
    }



    //Shifting coordinates of the detected axis to the cropped polar image referential
    double minLocOnCroppedPolarImage = minLoc.x + windowSize / 2.0;

    int leftRightWindowRatio = 4;

    //Finding 2 locations to the left and right of the detected axis
    int leftOfMin = minLoc.x - windowSize / leftRightWindowRatio > 0 ? minLoc.x - windowSize / leftRightWindowRatio : 0;
    int rightOfMin = minLoc.x + windowSize / leftRightWindowRatio < (int)meanSquareErrors.size() ? minLoc.x + windowSize / leftRightWindowRatio : (int)meanSquareErrors.size() - 1;
    //Computing the slopes between the axis and the 2 points
    double slope = minLoc.x - leftOfMin != 0 ? (meanSquareErrors[minLoc.x] - meanSquareErrors[leftOfMin]) / ((minLoc.x) - (leftOfMin)) : 0.0;
    double slope2 = minLoc.x - rightOfMin != 0 ? (meanSquareErrors[minLoc.x] - meanSquareErrors[rightOfMin]) / ((minLoc.x) - (rightOfMin)) : 0.0;

    //The notch should have slopes on both sides, so we reject the image unless both sides have significant slopes
    if (slope > -slopeThreshold || slope2 < slopeThreshold)
    {
        angleConfidence = 0.0;
        return std::numeric_limits<double>::quiet_NaN();
    }

    double rawAngleInRadians = ComputeAngleFromCorrelation(minLocOnCroppedPolarImage, histograms.angles, paddedShifted.size(), _polarImage.size(), croppedRegion);

    angleConfidence = 1.0 - (minErrorNorm / minErrorThreshold);

    return rawAngleInRadians;
}

double SymmetryDetector::DetectV2(EdgeImage* const image, cv::Point2d waferCenter, int waferDiameterInMm, int notchWidthInMicrons, double& angleConfidence, std::string const& reportPath, double minErrorThreshold, double slopeThreshold) {

    cv::Mat topSide;
    cv::Mat _polarImage;

    auto histograms = PolarTransform::Transform(image, _polarImage, waferCenter);

    //Cropping out the black pixels from the polar transform
    cv::Rect croppedRegion;
    cv::Mat croppedPolar = PolarImageCropper::Crop(_polarImage, waferDiameterInMm, &croppedRegion);

    //Padding the image to help with the symmetry detection
    auto paddedShifted = SymmetryDetector::ImagePadder::PadImage(croppedPolar);
    paddedShifted.convertTo(paddedShifted, CV_32F);
    topSide = paddedShifted;

    //Flip image to obtain a right side
    static const int FLIP_ON_X_AXIS = 0;
    cv::Mat bottomSide;
    cv::flip(topSide, bottomSide, FLIP_ON_X_AXIS);

    //int notchWidthInMicrons = 3000;

    auto notchWidthInPixels = ((double)notchWidthInMicrons / image->GetPixelSize().get().x);
    auto polarToOriginalRatio = (double)_polarImage.rows / (double)image->Mat().cols;
    int polarNotchWidthInPixels = (int) (notchWidthInPixels * polarToOriginalRatio);

    std::vector<float> meanSquareErrors;

    int paddingSize = topSide.rows / 4;
    int windowSize = polarNotchWidthInPixels / 2;

    int polarHeight = croppedPolar.rows;

    //Sliding through the top side using a window and computing the mean square error between the window and the flipped window
    for (int i = paddingSize + windowSize / 2; i < (paddingSize * 3) - windowSize / 2; i++)
    {
        cv::Mat topWindow = topSide(cv::Rect(0, i - windowSize / 2, croppedPolar.cols, windowSize));
        cv::Mat botWindow;
        cv::flip(topWindow, botWindow, FLIP_ON_X_AXIS);

        cv::Mat diffMat;
        cv::subtract(topWindow, botWindow, diffMat);
        float error = 0.0f;
        cv::Mat powMat = diffMat.mul(diffMat);
        error = (float) cv::sum(powMat)[0];
        error /= (float) (diffMat.rows * diffMat.cols);

        meanSquareErrors.push_back(error);
    }

    double maxError;
    double maxError2;
    double minError;
    cv::Point minLoc;
    //Finding the Y location with the smallest error (highest similarity, highest chance of being a symmetry axis)
    cv::minMaxLoc(meanSquareErrors, &minError, &maxError, &minLoc);
    double minErrorNorm = minError / maxError;

    int leftRightWindowRatio = 4;

    //Finding 2 locations to the left and right of the detected axis
    int leftOfMin = minLoc.x - windowSize / leftRightWindowRatio > 0 ? minLoc.x - windowSize / leftRightWindowRatio : 0;
    int rightOfMin = minLoc.x + windowSize / leftRightWindowRatio < (int)meanSquareErrors.size() ? minLoc.x + windowSize / leftRightWindowRatio : (int)meanSquareErrors.size() - 1;
    //Computing the slopes between the axis and the 2 points
    double slope = minLoc.x - leftOfMin != 0 ? (meanSquareErrors[minLoc.x] - meanSquareErrors[leftOfMin]) / ((minLoc.x) - (leftOfMin)) : -slopeThreshold;
    double slope2 = minLoc.x - rightOfMin != 0 ? (meanSquareErrors[minLoc.x] - meanSquareErrors[rightOfMin]) / ((minLoc.x) - (rightOfMin)) : slopeThreshold;

    std::vector<float> meanSquareErrors2 = meanSquareErrors;

    while (minErrorNorm > minErrorThreshold || (slope >= -slopeThreshold || slope2 <= slopeThreshold))
    {
        meanSquareErrors2[minLoc.x] = (float) maxError;

        //Image rejected if the error on the detected axis is too big (meaning the similarity between the best window and flipped window was too low)
        //and
        //The notch should have slopes on both sides, so we reject the image unless both sides have significant slopes
        if (minError == maxError)
        {
            angleConfidence = 0.0;
            return std::numeric_limits<double>::quiet_NaN();
        }
        cv::minMaxLoc(meanSquareErrors2, &minError, &maxError2, &minLoc);
        minErrorNorm = minError / maxError;

        //Finding 2 locations to the left and right of the detected axis
        leftOfMin = minLoc.x - windowSize / leftRightWindowRatio > 0 ? minLoc.x - windowSize / leftRightWindowRatio : 0;
        rightOfMin = minLoc.x + windowSize / leftRightWindowRatio < (int)meanSquareErrors.size() ? minLoc.x + windowSize / leftRightWindowRatio : (int)meanSquareErrors.size() - 1;
        //Computing the slopes between the axis and the 2 points
        slope = minLoc.x - leftOfMin != 0 ? (meanSquareErrors[minLoc.x] - meanSquareErrors[leftOfMin]) / ((minLoc.x) - (leftOfMin)) : -slopeThreshold;
        slope2 = minLoc.x - rightOfMin != 0 ? (meanSquareErrors[minLoc.x] - meanSquareErrors[rightOfMin]) / ((minLoc.x) - (rightOfMin)) : slopeThreshold;
    }

    //Shifting coordinates of the detected axis to the cropped polar image referential
    double minLocOnCroppedPolarImage = minLoc.x + windowSize / 2.0;

    double rawAngleInRadians = ComputeAngleFromCorrelation(minLocOnCroppedPolarImage, histograms.angles, paddedShifted.size(), _polarImage.size(), croppedRegion);

    angleConfidence = 1.0 - (minErrorNorm / minErrorThreshold);

    return rawAngleInRadians;
}

double SymmetryDetector::DetectV3(EdgeImage* const image, cv::Point2d waferCenter, int waferDiameterInMm, int notchWidthInMicrons, double& angleConfidence, std::string const& reportPath, bwa::ReportOption reportOption, double minErrorThreshold, double slopeThreshold) {

    cv::Mat topSide;
    cv::Mat _polarImage;

    auto histograms = PolarTransform::Transform(image, _polarImage, waferCenter);

    //Cropping out the black pixels from the polar transform
    cv::Rect croppedRegion;
    cv::Mat croppedPolar = PolarImageCropper::Crop(_polarImage, waferDiameterInMm, &croppedRegion);

    //Padding the image to help with the symmetry detection
    auto paddedShifted = SymmetryDetector::ImagePadder::PadImage(croppedPolar);
    paddedShifted.convertTo(paddedShifted, CV_32F);
    topSide = paddedShifted;

    //Flip image to obtain a right side
    static const int FLIP_ON_X_AXIS = 0;
    cv::Mat bottomSide;
    cv::flip(topSide, bottomSide, FLIP_ON_X_AXIS);

    auto notchWidthInPixels = (double)notchWidthInMicrons / image->GetPixelSize().get().x;
    auto polarToOriginalRatio = (double)_polarImage.rows / (double)image->Mat().cols;
    int polarNotchWidthInPixels = (int) (notchWidthInPixels * polarToOriginalRatio);

    std::vector<float> meanSquareErrors;
    std::vector<float> meanSquareErrorsNormalized;

    int paddingSize = topSide.rows / 4;
    int windowSize = polarNotchWidthInPixels / 2;

    //Sliding through the top side using a window and computing the mean square error between the window and the flipped window
    for (int i = paddingSize + windowSize / 2; i < (paddingSize * 3) - windowSize / 2; i++)
    {
        cv::Mat topWindow = topSide(cv::Rect(0, i - windowSize / 2, croppedPolar.cols, windowSize));
        cv::Mat botWindow;
        cv::flip(topWindow, botWindow, FLIP_ON_X_AXIS);

        cv::Mat diffMat;
        cv::subtract(topWindow, botWindow, diffMat);
        float error = 0.0f;
        cv::Mat powMat = diffMat.mul(diffMat);
        error = (float)cv::sum(powMat)[0];
        error /= (float)(diffMat.rows * diffMat.cols);

        meanSquareErrors.push_back(error);
    }

    double maxError;
    double maxError2;
    double minError;
    cv::Point minLoc;
    //Finding the Y location with the smallest error (highest similarity, highest chance of being a symmetry axis)
    cv::minMaxLoc(meanSquareErrors, &minError, &maxError, &minLoc);
    double minErrorNorm = minError / maxError;

    cv::Mat similarityCheckImage;
    //Creating the overlay image for later report
    if (!reportPath.empty() && reportOption > 0)
    {
        cv::cvtColor(croppedPolar, similarityCheckImage, cv::COLOR_GRAY2BGR);
        cv::rotate(similarityCheckImage, similarityCheckImage, cv::ROTATE_90_CLOCKWISE);
        cv::flip(similarityCheckImage, similarityCheckImage, 1);
        int simCheckPadAmount = 300;
        cv::copyMakeBorder(similarityCheckImage, similarityCheckImage, 0, simCheckPadAmount, 0, 0, cv::BORDER_CONSTANT, cv::Scalar(0, 0, 0));

        for (int i = windowSize / 2; i < similarityCheckImage.cols - (windowSize / 2) - 1; i++)
        {
            double currentError = meanSquareErrors[i - (windowSize / 2)];
            int pixelHeight = (int)round((currentError / maxError) * (simCheckPadAmount - 1));
            pixelHeight = simCheckPadAmount - pixelHeight;
            cv::circle(similarityCheckImage, cv::Point(i, pixelHeight + (similarityCheckImage.rows - simCheckPadAmount)), 3, cv::Scalar(0, 0, 255), -1);
        }
    }

    for (int i = 0; i < meanSquareErrors.size(); i++)
    {
        meanSquareErrorsNormalized.push_back((float)(meanSquareErrors[i] / maxError));
    }

    int leftRightWindowRatio = 3;

    //Finding 2 locations to the left and right of the detected axis
    int leftOfMin = minLoc.x - windowSize / leftRightWindowRatio > 0 ? minLoc.x - windowSize / leftRightWindowRatio : 0;
    int rightOfMin = minLoc.x + windowSize / leftRightWindowRatio < (int)meanSquareErrorsNormalized.size() ? minLoc.x + windowSize / leftRightWindowRatio : (int)meanSquareErrorsNormalized.size() - 1;

    double minLocXNormalized = (double)minLoc.x / (double)meanSquareErrorsNormalized.size();
    double leftOfMinNormalized = (double)leftOfMin / (double)meanSquareErrorsNormalized.size();
    double rightOfMinNormalized = (double)rightOfMin / (double)meanSquareErrorsNormalized.size();

    //Computing the slopes between the axis and the 2 points
    double slope = minLoc.x - leftOfMin != 0 ? (meanSquareErrorsNormalized[minLoc.x] - meanSquareErrorsNormalized[leftOfMin]) / (minLocXNormalized - leftOfMinNormalized) : -slopeThreshold;
    double slope2 = minLoc.x - rightOfMin != 0 ? (meanSquareErrorsNormalized[minLoc.x] - meanSquareErrorsNormalized[rightOfMin]) / (minLocXNormalized - rightOfMinNormalized) : slopeThreshold;

    //Computing the absolute difference between the slope intensities
    double slopeDiff = abs(abs(slope) - abs(slope2));

    //second meanSquareError vector where the minimum value is removed every step
    std::vector<float> meanSquareErrors2 = meanSquareErrors;

    //When 1 of these conditions isn't met, the notch location is rejected :
    //
    //The similarity must be good enough
    bool isAboveSimThreshold = minErrorNorm > minErrorThreshold;
    //The slopes on each side must be high enough
    bool hasNoSlopes = (slope >= -slopeThreshold || slope2 <= slopeThreshold);
    //The slopes on each side need to go opposite ways
    bool slopesAreNotOpposite = (std::signbit(slope) == std::signbit(slope2));
    //The slopes on each side need to be of similar intensity
    bool slopesAreNotSimilar = (slopeDiff > abs(slope/2) || slopeDiff > abs(slope2/2));

    while (isAboveSimThreshold || hasNoSlopes || slopesAreNotOpposite || slopesAreNotSimilar)
    {
        meanSquareErrors2[minLoc.x] = (float) maxError;

        //Image rejected is we've run out of minimum similarities / potential good notch locations
        if (minError == maxError)
        {
            if (!reportPath.empty() && reportOption > 0)
            {
                cv::imwrite(reportPath + "/BOTTOM_notch_similarity.png", similarityCheckImage);
            }
            angleConfidence = 0.0;
            return std::numeric_limits<double>::quiet_NaN();
        }
        cv::minMaxLoc(meanSquareErrors2, &minError, &maxError2, &minLoc);
        minErrorNorm = minError / maxError;

        //Finding 2 locations to the left and right of the detected axis
        leftOfMin = minLoc.x - windowSize / leftRightWindowRatio > 0 ? minLoc.x - windowSize / leftRightWindowRatio : 0;
        rightOfMin = minLoc.x + windowSize / leftRightWindowRatio < (int)meanSquareErrorsNormalized.size() ? minLoc.x + windowSize / leftRightWindowRatio : (int)meanSquareErrorsNormalized.size() - 1;

        minLocXNormalized = (double)minLoc.x / (double)meanSquareErrorsNormalized.size();
        leftOfMinNormalized = (double)leftOfMin / (double)meanSquareErrorsNormalized.size();
        rightOfMinNormalized = (double)rightOfMin / (double)meanSquareErrorsNormalized.size();
        //Computing the slopes between the axis and the 2 points
        slope = minLoc.x - leftOfMin != 0 ? (meanSquareErrorsNormalized[minLoc.x] - meanSquareErrorsNormalized[leftOfMin]) / (minLocXNormalized - leftOfMinNormalized) : -slopeThreshold;
        slope2 = minLoc.x - rightOfMin != 0 ? (meanSquareErrorsNormalized[minLoc.x] - meanSquareErrorsNormalized[rightOfMin]) / (minLocXNormalized - rightOfMinNormalized) : slopeThreshold;

        slopeDiff = abs(abs(slope) - abs(slope2));

        isAboveSimThreshold = minErrorNorm > minErrorThreshold;
        hasNoSlopes = (slope >= -slopeThreshold || slope2 <= slopeThreshold);
        slopesAreNotOpposite = (std::signbit(slope) == std::signbit(slope2));
        slopesAreNotSimilar = (slopeDiff > abs(slope/2) || slopeDiff > abs(slope2/2));

    }

    //Shifting coordinates of the detected axis to the cropped polar image referential
    double minLocOnCroppedPolarImage = (double)minLoc.x + ((double)windowSize * 0.5);

    if (!reportPath.empty() && reportOption > 0)
    {
        cv::line(similarityCheckImage, cv::Point((int)minLocOnCroppedPolarImage, 0), cv::Point((int)minLocOnCroppedPolarImage, similarityCheckImage.rows), cv::Scalar(0, 255, 0));
        cv::imwrite(reportPath + "/BOTTOM_notch_similarity.png", similarityCheckImage);
    }

    double rawAngleInRadians = ComputeAngleFromCorrelation(minLocOnCroppedPolarImage, histograms.angles, paddedShifted.size(), _polarImage.size(), croppedRegion);

    angleConfidence = 1.0 - (minErrorNorm / minErrorThreshold);

    return rawAngleInRadians;
}
