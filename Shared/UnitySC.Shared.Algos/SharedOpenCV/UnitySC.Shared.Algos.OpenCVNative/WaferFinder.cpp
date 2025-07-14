#include "ErrorLogging.hpp"
#include "WaferFinder.hpp"
#include "CEdgeDetector.hpp"
#include <Wafer.hpp>

using namespace shape_finder;
using namespace std;
using namespace cv;
namespace fs = std::filesystem;

#pragma unmanaged
namespace psd {
    namespace {
        /**
         * @brief Draw all given circles on image.
         */
        void DrawCirclesOnImage(Mat& img, vector<Circle> circles);

        /**
         * @brief Draw a given circle on image
         */
        void DrawCircleOnImage(Mat& img, Circle circle);
    }

    Circle FasterFindWaferCircle(const Mat& waferImg, float waferDiameterInMicrometers, float waferToleranceInMicrometers, double pixelSizeInMicrometer, std::string const& reportPath)
    {
        bwa::ReportOption reportOption = bwa::ReportOption::OverlayReport;

        cv::Mat grayWaferImg;
        if (waferImg.channels() == 3) cv::cvtColor(waferImg.clone(), grayWaferImg, cv::COLOR_BGR2GRAY);
        else grayWaferImg = waferImg.clone();

        //Cropping 3 small edge images (left, top, right) from the original wafer image
        int cropSizeRatio = 20;
        int edgeSize = waferImg.rows / cropSizeRatio;
        int centerOffset = edgeSize;
        int edgeLeftStartX = 0;
        int edgeLeftStartY = (waferImg.rows / 2) - (edgeSize / 2) - centerOffset;
        int edgeRightStartX = waferImg.cols - edgeSize - 1;
        int edgeRightStartY = (waferImg.rows / 2) - (edgeSize / 2) - centerOffset;
        int edgeTopStartX = (waferImg.cols / 2) - (edgeSize / 2) - centerOffset;
        int edgeTopStartY = 0;
        cv::Mat edgeLeftMat = grayWaferImg(Rect(edgeLeftStartX, edgeLeftStartY, edgeSize, edgeSize));
        cv::Mat edgeRightMat = grayWaferImg(Rect(edgeRightStartX, edgeRightStartY, edgeSize, edgeSize));
        cv::Mat edgeTopMat = grayWaferImg(Rect(edgeTopStartX, edgeTopStartY, edgeSize, edgeSize));

        Wafer::WaferData waferData;
        waferData.radiusInMicrons = (int)(waferDiameterInMicrometers / 2.0f);
        Wafer wafer = Wafer(waferData);
        EdgeImage::PixelSize pixelSize(Point2d(pixelSizeInMicrometer, pixelSizeInMicrometer));

        //Image centroids in the chuck referential
        EdgeImage::ImageCentroid centroidLeft(cv::Point2d(((edgeLeftStartX + edgeLeftMat.cols / 2) - (waferImg.cols / 2)) * pixelSizeInMicrometer, -((edgeLeftStartY + edgeLeftMat.rows / 2) - (waferImg.rows / 2)) * pixelSizeInMicrometer));
        EdgeImage::ImageCentroid centroidRight(cv::Point2d(((edgeRightStartX + edgeRightMat.cols / 2) - (waferImg.cols / 2)) * pixelSizeInMicrometer, -((edgeRightStartY + edgeRightMat.rows / 2) - (waferImg.rows / 2)) * pixelSizeInMicrometer));
        EdgeImage::ImageCentroid centroidTop(cv::Point2d(((edgeTopStartX + edgeTopMat.cols / 2) - (waferImg.cols / 2)) * pixelSizeInMicrometer, -((edgeTopStartY + edgeTopMat.rows / 2) - (waferImg.rows / 2)) * pixelSizeInMicrometer));
        EdgeImage::Pointer edgeLeftImage = EdgeImage::New(edgeLeftMat, pixelSize, centroidLeft, EdgeImage::EdgePosition::LEFT);
        EdgeImage::Pointer edgeRightImage = EdgeImage::New(edgeRightMat, pixelSize, centroidRight, EdgeImage::EdgePosition::RIGHT);
        EdgeImage::Pointer edgeTopImage = EdgeImage::New(edgeTopMat, pixelSize, centroidTop, EdgeImage::EdgePosition::TOP);
        wafer.AddEdgeImage(edgeLeftImage, reportPath);
        wafer.AddEdgeImage(edgeRightImage, reportPath);
        wafer.AddEdgeImage(edgeTopImage, reportPath);

        Wafer::WaferGeometricalParameters geoParams;
        int edgeDetectionVersion = 0;
        int cannyThreshold = 0; //not used with this version of the edge detection
        Algorithms::Status status = wafer.GetGeometricalParameters(&geoParams, edgeDetectionVersion, cannyThreshold, reportPath, reportOption);

        float minRadius = (waferDiameterInMicrometers / 2) - waferToleranceInMicrometers;
        float maxRadius = (waferDiameterInMicrometers / 2) + waferToleranceInMicrometers;

        if (geoParams.radius < minRadius || geoParams.radius > maxRadius)
        {
            ErrorLogging::LogError("No circle that could correspond to the wafer of given diameter has been detected.\n");
            return Circle(Point2f(0, 0), 0);
        }

        //Switching back to image coordinates
        geoParams.centerShift.x = (geoParams.centerShift.x / pixelSizeInMicrometer) + (waferImg.cols / 2);
        geoParams.centerShift.y = (-(geoParams.centerShift.y / pixelSizeInMicrometer) + (waferImg.rows / 2));
        geoParams.radius = geoParams.radius / pixelSizeInMicrometer;

        if (!reportPath.empty() && reportOption > 0)
        {
            Mat waferCircleImg = waferImg.clone();
            DrawCircleOnImage(waferCircleImg, Circle(geoParams.centerShift, float(geoParams.radius * 2.0)));
            imwrite(reportPath + "/wafercircle.png", waferCircleImg);
        }

        return Circle(geoParams.centerShift,(float)( geoParams.radius * 2.0));
    }

    Circle FindWaferCircle(const Mat& waferImg, float waferDiameter, float detectionTolerance, float cannyThreshold, bool print)
    {
        //canny threshold de float en int ???? alors que opencv utilise du double ... oO'
        vector<Circle> circles = CircleFinder(waferImg, CircleFinderParams(waferDiameter, waferDiameter, detectionTolerance, (int) cannyThreshold, false, false));

        if (circles.empty())
        {
            ErrorLogging::LogError("No circle that could correspond to the wafer of given diameter has been detected.\n");
            return Circle(Point2f(0, 0), 0);
        }

        // Among all detected circles, the circle whose diameter is closest to the expected diameter is selected

        Circle waferCircle = circles[0];
        for (int i = 0; i < circles.size(); i++)
        {
            if (abs(waferDiameter - circles[i].Diameter) < abs(waferDiameter - waferCircle.Diameter))
            {
                waferCircle = circles[i];
            }
        }

        /*
        if (print)
        {
            Mat allCirclesImg = waferImg.clone();
            DrawCirclesOnImage(allCirclesImg, circles);
            resize(allCirclesImg, allCirclesImg, cv::Size(700, 700));
            imshow("all circles", allCirclesImg);

            Mat waferCircleImg = waferImg.clone();
            DrawCircleOnImage(waferCircleImg, waferCircle);
            resize(waferCircleImg, waferCircleImg, Size(700, 700));
            imshow("wafer circle", waferCircleImg);

            waitKey();
        }
        */

        return waferCircle;
    }

    namespace {
        void DrawCirclesOnImage(Mat& img, vector<Circle> circles) {
            for (size_t i = 0; i < circles.size(); i++) {
                DrawCircleOnImage(img, circles[i]);
            }
        }

        void DrawCircleOnImage(Mat& img, Circle circle) {
            Point2f center = circle.CenterPos;
            cv::circle(img, center, 1, Scalar(0, 255, 0), 10, LINE_AA); // center
            int radius = (int) (circle.Diameter * 0.5f);
            cv::circle(img, center, radius, Scalar(0, 0, 255), 2, LINE_AA);
        }
    }
}