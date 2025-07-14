#include <opencv2/opencv.hpp>

#include "CShapeFinder.hpp"
#include "CImageTypeConvertor.hpp"

#pragma unmanaged
using namespace cv;
using namespace std;

namespace shape_finder {
    namespace {
        typedef vector<vector<Point>> VecOfPointsVec;
        typedef vector<Vec3f> VecOfCircles;

        Mat PreprocessToImproveEllipseContourSearch(const Mat& img, int cannyThreshold);

        Mat PreprocessToImproveHoughTransform(const Mat& img, bool useMorphologicialOperations);

        vector<Ellipse> FitEllipseToEachContour(const VecOfPointsVec& contours, const EllipseFinderParams& params);

        bool ContourIsOpen(vector<Point> contour);

        vector<Circle> ExtractsCircleRelevantValuesFromRawValues(const VecOfCircles& circles);

        bool CheckRegionOfInterestValidity(cv::Mat image, cv::Rect roi);
    }; // namespace


    vector<Ellipse> EllipseFinder(const Mat& img, const EllipseFinderParams& params, Rect roi) {
        Mat preprocessedImg = PreprocessToImproveEllipseContourSearch(img, params.CannyThreshold);

        if (!roi.empty() && CheckRegionOfInterestValidity(preprocessedImg, roi))
        {
            cv::Mat mask = cv::Mat::zeros(preprocessedImg.rows, preprocessedImg.cols, CV_8U); // all 0
            mask(roi) = 1;
            preprocessedImg = preprocessedImg.mul(mask);
        }

        VecOfPointsVec contours;
        findContours(preprocessedImg, contours, RETR_EXTERNAL, CHAIN_APPROX_NONE);

        vector<Ellipse> ellipses = FitEllipseToEachContour(contours, params);
        return ellipses;
    }

    vector<Circle> CircleFinder(const Mat& img, const CircleFinderParams& params, Rect roi) {

        // PreprocessImageForCircleFinder is not called here because the other parts of image préprocessing is done within cv::HoughCircles method
        // this why we just call PreprocessToImproveHoughTransform
        Mat preprocessedImg = PreprocessToImproveHoughTransform(img, params.UseMorphologicalOperations);

        if (!roi.empty() && CheckRegionOfInterestValidity(preprocessedImg, roi))
        {
            cv::Mat mask = cv::Mat::zeros(preprocessedImg.rows, preprocessedImg.cols, CV_8U); // all 0
            mask(roi) = 1;
            preprocessedImg = preprocessedImg.mul(mask);
        }

        VecOfCircles rawCircles;
        int minRadius = (int) ((params.ApproximateDiameter * 0.5f) - params.DetectionTolerance);
        int maxRadius = (int) ((params.ApproximateDiameter * 0.5f) + params.DetectionTolerance);
        if (params.UseScharrAlgorithm)
        {
            HoughCircles(preprocessedImg, rawCircles, HOUGH_GRADIENT_ALT, 1.5, params.DistBetweenCircles, params.CannyThreshold, 0.9, minRadius, maxRadius);
        }
        else
        {
            HoughCircles(preprocessedImg, rawCircles, HOUGH_GRADIENT, 1, params.DistBetweenCircles, params.CannyThreshold, 30, minRadius, maxRadius);
        }

        vector<Circle> intelligibleCircles = ExtractsCircleRelevantValuesFromRawValues(rawCircles);
        return intelligibleCircles;
    }

    Mat PreprocessImageForEllipseFinder(const Mat& img, const EllipseFinderParams& params)
    {
        Mat preprocessedImg = PreprocessToImproveEllipseContourSearch(img, params.CannyThreshold);
        return preprocessedImg;
    }

    Mat PreprocessImageForCircleFinder(const Mat& img, const CircleFinderParams& params)
    {
        Mat preprocessedImg = PreprocessToImproveHoughTransform(img, params.UseMorphologicalOperations);

        Mat dx, dy, edges;
        int kernelSize = 3;
        if (params.UseScharrAlgorithm)
        {
            Scharr(preprocessedImg, dx, CV_16S, 1, 0);
            Scharr(preprocessedImg, dy, CV_16S, 0, 1);
            Canny(dx, dy, edges, max(1, params.CannyThreshold / 2), params.CannyThreshold, true);
        }
        else
        {
            Sobel(preprocessedImg, dx, CV_16S, 1, 0, kernelSize, 1, 0, BORDER_REPLICATE);
            Sobel(preprocessedImg, dy, CV_16S, 0, 1, kernelSize, 1, 0, BORDER_REPLICATE);
            Canny(dx, dy, edges, std::max(1, params.CannyThreshold / 2), params.CannyThreshold, false);
        }

        return edges;
    }

    namespace {
        Mat PreprocessToImproveEllipseContourSearch(const Mat& img, int cannyThreshold) {
            Mat wellTypedImg = Convertor::ConvertTo8UC1(img);
            Mat bluredImg;
            medianBlur(wellTypedImg, bluredImg, 3);

            Mat dx, dy, edges;
            Sobel(bluredImg, dx, CV_16S, 1, 0, 3, 1, 0, BORDER_REPLICATE);
            Sobel(bluredImg, dy, CV_16S, 0, 1, 3, 1, 0, BORDER_REPLICATE);
            Canny(dx, dy, edges, max(1, cannyThreshold / 2), cannyThreshold, false);

            Mat preprocessedImg;
            int morphologicalOperationSize = 2;
            Size morphologicalKernelSize = Size(2 * morphologicalOperationSize + 1, 2 * morphologicalOperationSize + 1);
            Point anchorPosition = Point(morphologicalOperationSize, morphologicalOperationSize);
            Mat kernelStructuringElement = getStructuringElement(MORPH_ELLIPSE, morphologicalKernelSize, anchorPosition);
            dilate(edges, preprocessedImg, kernelStructuringElement);
            erode(preprocessedImg, preprocessedImg, kernelStructuringElement);

            return preprocessedImg;
        }

        Mat PreprocessToImproveHoughTransform(const Mat& img, bool useMorphologicalOperations) {
            Mat wellTypedImg = Convertor::ConvertTo8UC1(img);

            Mat bluredImg;
            Mat erodedImg;
            Mat dilatedImg;
            medianBlur(wellTypedImg, bluredImg, 5); // This is done so as to prevent a lot of false circles from being detected

            if (!useMorphologicalOperations)
            {
                return bluredImg;
            }

            int morphSize = 3;
            Mat morphElement = getStructuringElement(MORPH_ELLIPSE,
                Size(2 * morphSize + 1, 2 * morphSize + 1),
                Point(morphSize, morphSize));
            erode(bluredImg, erodedImg, morphElement);
            dilate(erodedImg, dilatedImg, morphElement);

            return dilatedImg;
        }

        vector<Ellipse> FitEllipseToEachContour(const VecOfPointsVec& contours, const EllipseFinderParams& params) {
            vector<Ellipse> ellipses;
            for (const auto& currentContour : contours) {
                if (currentContour.size() < 5) {
                    continue; //  we need at least 5 points to fit the ellipse
                }
                if (ContourIsOpen(currentContour)) {
                    continue;
                }
                RotatedRect rect = fitEllipse(currentContour);
                Point2f center = rect.center;
                float angle = rect.angle;
                float heightAxis = rect.size.height;
                float widthAxis = rect.size.width;
                //float perimeter = arcLength(currentContour, false); // not used

                float approximateMajorAxis = params.ApproximateAxes.first > params.ApproximateAxes.second ? params.ApproximateAxes.first : params.ApproximateAxes.second;
                float approximateMinorAxis = params.ApproximateAxes.first <= params.ApproximateAxes.second ? params.ApproximateAxes.first : params.ApproximateAxes.second;
                float majorAxis = heightAxis > widthAxis ? heightAxis : widthAxis;
                float minorAxis = heightAxis <= widthAxis ? heightAxis : widthAxis;
                bool validMajorAxis = majorAxis <= approximateMajorAxis + params.MajorAxisTolerance && majorAxis >= approximateMajorAxis - params.MajorAxisTolerance;
                bool validMinorAxis = minorAxis <= approximateMinorAxis + params.MinorAxisTolerance && minorAxis >= approximateMinorAxis - params.MinorAxisTolerance;
                if (validMajorAxis && validMinorAxis) {
                    ellipses.push_back(Ellipse(center, heightAxis, widthAxis, angle));
                }
            }

            return ellipses;
        }

        bool ContourIsOpen(vector<Point> contour) {
            auto contourArea = cv::contourArea(contour);
            auto contourPerimeter = arcLength(contour, false);
            return contourPerimeter > contourArea;
        }

        vector<Circle> ExtractsCircleRelevantValuesFromRawValues(const VecOfCircles& rawCircles) {
            vector<Circle> intelligibleCircles;
            for (size_t i = 0; i < rawCircles.size(); i++) {
                Point2f center = Point2f(rawCircles[i][0], rawCircles[i][1]);
                float diameter = rawCircles[i][2] * 2;
                intelligibleCircles.push_back(Circle(center, diameter));
            }

            return intelligibleCircles;
        }

        bool CheckRegionOfInterestValidity(cv::Mat image, cv::Rect roi)
        {
            bool topLeftCornerIsOutside = roi.x < 0 || roi.y < 0;
            bool bottomRightCornerIsOutside = roi.x + roi.width > image.cols || roi.y + roi.height > image.rows;
            if (topLeftCornerIsOutside || bottomRightCornerIsOutside)
            {
                return false;
            }

            return true;
        }
    }; // namespace
};   // namespace shape_finder