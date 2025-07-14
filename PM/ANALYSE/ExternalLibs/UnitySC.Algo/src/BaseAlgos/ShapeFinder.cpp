#include <opencv2/opencv.hpp>

#include <BaseAlgos/ShapeFinder.hpp>
#include <ImageTypeConvertor.hpp>
#include <Logger.hpp>

using namespace cv;
using namespace std;

namespace shape_finder {

    namespace {
        typedef vector<vector<Point>> VecOfPointsVec;
        typedef vector<Vec3f> VecOfCircles;

        Mat PreprocessToImproveContourSearch(const Mat& img, int cannyThreshold);

        Mat PreprocessToImproveHoughTransform(const Mat& img);

        vector<Ellipse> FitEllipseToEachContour(const VecOfPointsVec& contours, const EllipseFinderParams& params);

        bool ContourIsOpen(vector<Point> contour);

        vector<Circle> ExtractsCircleRelevantValuesFromRawValues(const VecOfCircles& circles);
    }; // namespace


    vector<Ellipse> EllipseFinder(const Mat& img, const EllipseFinderParams& params) {
        Mat preprocessedImg = PreprocessToImproveContourSearch(img, params.CannyThreshold);

        VecOfPointsVec contours;
        findContours(preprocessedImg, contours, RETR_EXTERNAL, CHAIN_APPROX_NONE);

        vector<Ellipse> ellipses = FitEllipseToEachContour(contours, params);
        return ellipses;
    }

    vector<Circle> CircleFinder(const Mat& img, const CircleFinderParams& params) {
        Mat preprocessedImg = PreprocessToImproveHoughTransform(img);

        VecOfCircles rawCircles;
        float minRadius = (params.ApproximateDiameter / 2) - params.DetectionTolerance;
        float maxRadius = (params.ApproximateDiameter / 2) + params.DetectionTolerance;
        HoughCircles(preprocessedImg, rawCircles, HOUGH_GRADIENT, 1, params.DistBetweenCircles, params.CannyThreshold, 30, minRadius, maxRadius);

        vector<Circle> intelligibleCircles = ExtractsCircleRelevantValuesFromRawValues(rawCircles);
        return intelligibleCircles;
    }

    namespace {

        Mat PreprocessToImproveContourSearch(const Mat& img, int cannyThreshold) {
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

        Mat PreprocessToImproveHoughTransform(const Mat& img) {
            Mat wellTypedImg = Convertor::ConvertTo8UC1(img);

            Mat bluredImg;
            medianBlur(wellTypedImg, bluredImg, 5); // This is done so as to prevent a lot of false circles from being detected

            return bluredImg;
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
                float perimeter = arcLength(currentContour, false);

                float approximateMajorAxis = params.ApproximateAxes.first > params.ApproximateAxes.second ? params.ApproximateAxes.first : params.ApproximateAxes.second;
                float approximateMinorAxis = params.ApproximateAxes.first <= params.ApproximateAxes.second ? params.ApproximateAxes.first : params.ApproximateAxes.second;
                float majorAxis = heightAxis > widthAxis ? heightAxis : widthAxis;
                float minorAxis = heightAxis <= widthAxis ? heightAxis : widthAxis;
                bool validMajorAxis = majorAxis <= approximateMajorAxis + params.DetectionTolerance && majorAxis >= approximateMajorAxis - params.DetectionTolerance;
                bool validMinorAxis = minorAxis <= approximateMinorAxis + params.DetectionTolerance && minorAxis >= approximateMinorAxis - params.DetectionTolerance;
                if (validMajorAxis && validMinorAxis) {
                    ellipses.push_back(Ellipse(center, heightAxis, widthAxis, angle));
                }
            }

            return ellipses;
        }

        bool ContourIsOpen(vector<Point> contour) {
            float contourArea = cv::contourArea(contour);
            float contourPerimeter = arcLength(contour, false);
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
    }; // namespace
};   // namespace shape_finder