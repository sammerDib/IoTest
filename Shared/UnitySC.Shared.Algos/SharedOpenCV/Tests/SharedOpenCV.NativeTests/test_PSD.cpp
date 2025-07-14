#include <opencv2/opencv.hpp>

#include "CppUnitTest.h"

#include "NotchFinder.hpp"
#include "WaferFinder.hpp"
#include "CShapeFinder.hpp"
#include "CRegistration.hpp"
#include "CEdgeDetector.hpp"
#include <CShapeFinder.cpp>
#include <chrono>

#pragma unmanaged

using namespace cv;
using namespace std;
using namespace shape_finder;
using namespace Microsoft::VisualStudio::CppUnitTestFramework;

#define TEST_DATA_PATH std::string(".\\..\\..\\Tests\\Data\\")

namespace SharedOpenCVNativeTests
{
    TEST_CLASS(PSDTests)
    {
    private:

        enum class NotchLocation {
            Left,
            Right,
            Top,
            Bottom
        };

        cv::Mat drawWaferWithNotch(int waferDiameter, NotchLocation notchLocation) {
            int imgWidth = waferDiameter + 100;
            int imgHeight = waferDiameter + 100;
            cv::Mat img = cv::Mat::zeros(cv::Size(imgWidth, imgHeight), CV_8UC1);

            Point waferCenter = Point(imgWidth / 2, imgHeight / 2);
            int waferRadius = waferDiameter / 2;
            cv::circle(img, waferCenter, waferRadius, Scalar(255, 255, 255), -1, LINE_AA);

            Point notchCenter;
            switch (notchLocation)
            {
            case NotchLocation::Top:
                notchCenter = Point(waferCenter.x, waferCenter.y - waferDiameter / 2);
                break;
            case NotchLocation::Bottom:
                notchCenter = Point(waferCenter.x, waferCenter.y + waferDiameter / 2);
                break;
            case NotchLocation::Left:
                notchCenter = Point(waferCenter.x - waferDiameter / 2, waferCenter.y);
                break;
            case NotchLocation::Right:
                notchCenter = Point(waferCenter.x + waferDiameter / 2, waferCenter.y);
                break;
            }

            int notchRadius = waferDiameter / 100;
            cv::circle(img, notchCenter, notchRadius, Scalar(0, 0, 0), -1, LINE_AA);

            return img;
        }

    public:
        TEST_METHOD(test_wafer_circle_detection_with_bottom_notch)
        {
            // Given
            float waferDiameter = 300.f;
            float detectionTolerance = 1.0f;
            Mat waferImg = drawWaferWithNotch((int)waferDiameter, NotchLocation::Bottom);

            // When
            Circle waferCircle = psd::FindWaferCircle(waferImg, waferDiameter, detectionTolerance);

            // Then
            Assert::AreEqual(waferDiameter, waferCircle.Diameter, detectionTolerance);
            Assert::AreEqual(0.5f * (float) waferImg.size().width, waferCircle.CenterPos.x, detectionTolerance);
            Assert::AreEqual(0.5f * (float) waferImg.size().height, waferCircle.CenterPos.y, detectionTolerance);
        }

        TEST_METHOD(test_wafer_circle_detection_with_top_notch)
        {
            // Given
            float waferDiameter = 300.f;
            float detectionTolerance = 1.f;
            Mat waferImg = drawWaferWithNotch((int)waferDiameter, NotchLocation::Top);

            // When
            Circle waferCircle = psd::FindWaferCircle(waferImg, waferDiameter, detectionTolerance);

            // Then
            Assert::AreEqual(waferDiameter, waferCircle.Diameter, detectionTolerance);
            Assert::AreEqual(0.5f * (float) waferImg.size().width, waferCircle.CenterPos.x, detectionTolerance);
            Assert::AreEqual(0.5f * (float) waferImg.size().height, waferCircle.CenterPos.y, detectionTolerance);
        }

        TEST_METHOD(test_wafer_circle_detection_with_left_notch)
        {
            // Given
            float waferDiameter = 300.f;
            float detectionTolerance = 1.f;
            Mat waferImg = drawWaferWithNotch((int)waferDiameter, NotchLocation::Left);

            // When
            Circle waferCircle = psd::FindWaferCircle(waferImg, waferDiameter, detectionTolerance);

            // Then
            Assert::AreEqual(waferDiameter, waferCircle.Diameter, detectionTolerance);
            Assert::AreEqual(0.5f * (float) waferImg.size().width, waferCircle.CenterPos.x, detectionTolerance);
            Assert::AreEqual(0.5f * (float) waferImg.size().height, waferCircle.CenterPos.y, detectionTolerance);
        }

        TEST_METHOD(test_wafer_circle_detection_with_right_notch)
        {
            // Given
            float waferDiameter = 300.f;
            float detectionTolerance = 1.f;
            Mat waferImg = drawWaferWithNotch((int)waferDiameter, NotchLocation::Right);

            // When
            Circle waferCircle = psd::FindWaferCircle(waferImg, waferDiameter, detectionTolerance);

            // Then
            Assert::AreEqual(waferDiameter, waferCircle.Diameter, detectionTolerance);
            Assert::AreEqual(0.5f * (float) waferImg.size().width, waferCircle.CenterPos.x, detectionTolerance);
            Assert::AreEqual(0.5f * (float) waferImg.size().height, waferCircle.CenterPos.y, detectionTolerance);
        }

        TEST_METHOD(test_notch_location_detection_with_bottom_notch)
        {
            // Given
            float waferDiameter = 300.f;
            float detectionTolerance = 1.f;
            Mat waferImg = drawWaferWithNotch((int)waferDiameter, NotchLocation::Bottom);
            Circle waferCircle = Circle(Point(waferImg.size().width / 2, waferImg.size().height / 2), waferDiameter);

            int notchWidthInPixel = 7;

            int widthFactor = 18;
            double deviationFactor = 3.7;
            double similarityThreshold = 0.6;

            // When
            double notchAngle = psd::FindNotchCenterByPolarStats(waferImg, waferCircle, psd::NotchLocation::Bottom, notchWidthInPixel, widthFactor, deviationFactor, similarityThreshold, false);
            Assert::AreEqual(0.0, notchAngle, 0.5);
        }

        TEST_METHOD(test_notch_location_detection_with_top_notch)
        {
            // Given
            float waferDiameter = 300.f;
            float detectionTolerance = 1.f;
            Mat waferImg = drawWaferWithNotch((int)waferDiameter, NotchLocation::Top);
            Circle waferCircle = Circle(Point(waferImg.size().width / 2, waferImg.size().height / 2), waferDiameter);

            int notchWidthInPixel = 7;

            int widthFactor = 18;
            double deviationFactor = 3.7;
            double similarityThreshold = 0.6;

            // When
            double notchAngle = psd::FindNotchCenterByPolarStats(waferImg, waferCircle, psd::NotchLocation::Top, notchWidthInPixel, widthFactor, deviationFactor, similarityThreshold, false);
            Assert::AreEqual(-180.0, notchAngle, 0.5);
        }

        TEST_METHOD(test_notch_location_detection_with_left_notch)
        {
            // Given
            float waferDiameter = 300.f;
            float detectionTolerance = 1.f;
            Mat waferImg = drawWaferWithNotch((int)waferDiameter, NotchLocation::Left);
            Circle waferCircle = Circle(Point(waferImg.size().width / 2, waferImg.size().height / 2), waferDiameter);

            int notchWidthInPixel = 7;

            int widthFactor = 18;
            double deviationFactor = 3.7;
            double similarityThreshold = 0.6;

            // When
            double notchAngle = psd::FindNotchCenterByPolarStats(waferImg, waferCircle, psd::NotchLocation::Left, notchWidthInPixel, widthFactor, deviationFactor, similarityThreshold, false);
            Assert::AreEqual(90.0, notchAngle, 0.5);
        }

        TEST_METHOD(test_notch_location_detection_with_right_notch)
        {
            // Given
            float waferDiameter = 300.f;
            float detectionTolerance = 1.f;
            Mat waferImg = drawWaferWithNotch((int)waferDiameter, NotchLocation::Right);
            Circle waferCircle = Circle(Point(waferImg.size().width / 2, waferImg.size().height / 2), waferDiameter);

            int notchWidthInPixel = 7;

            int widthFactor = 18;
            double deviationFactor = 3.7;
            double similarityThreshold = 0.6;

            // When
            double notchAngle = psd::FindNotchCenterByPolarStats(waferImg, waferCircle, psd::NotchLocation::Right, notchWidthInPixel, widthFactor, deviationFactor, similarityThreshold, false);
            Assert::AreEqual(-90.0, notchAngle, 0.5);
        }

        TEST_METHOD(new_circle_detection_method_is_faster_than_old_method)
        {
            string image_path = TEST_DATA_PATH + string("top.tiff");
            Mat input = imread(image_path, IMREAD_COLOR);

            Assert::IsTrue(!input.empty());

            // Finds wafer circle

            float waferDiameterInMicrometer = 300 * 1000; //300 millimeters
            float notchWidthInMicrometer = 3 * 1000; // 3 millimeters
            float waferToleranceInMicrometer = 10 * 1000; // 10 millimeter
            float pixelSizeInMicrometer = 100;
            float waferDiameterInPixel = waferDiameterInMicrometer / pixelSizeInMicrometer;
            float notchWidthInPixel = notchWidthInMicrometer / pixelSizeInMicrometer;
            float waferDetectionToleranceInPixel = waferToleranceInMicrometer / pixelSizeInMicrometer;

            auto clock1 = std::chrono::high_resolution_clock::now();
            Circle waferCircle = psd::FindWaferCircle(input, waferDiameterInPixel, waferDetectionToleranceInPixel, 100);
            auto clock2 = std::chrono::high_resolution_clock::now();
            Circle waferCircle2 = psd::FasterFindWaferCircle(input, waferDiameterInMicrometer, waferToleranceInMicrometer, pixelSizeInMicrometer);
            auto clock3 = std::chrono::high_resolution_clock::now();

            auto oldTimeInMs = std::chrono::duration_cast<std::chrono::milliseconds>(clock2 - clock1);
            auto newTimeInMs = std::chrono::duration_cast<std::chrono::milliseconds>(clock3 - clock2);

            Assert::IsTrue(newTimeInMs < oldTimeInMs);

            
        }

        TEST_METHOD(nominal_case)
        {
            bool printForDebug = false;

            string image_path = TEST_DATA_PATH + string("top.tiff");
            Mat input = imread(image_path, IMREAD_COLOR);

            Assert::IsTrue(!input.empty());

            // Finds wafer circle

            float waferDiameterInMicrometer = 300 * 1000; //300 millimeters
            float notchWidthInMicrometer = 3 * 1000; // 3 millimeters
            float waferToleranceInMicrometer = 10 * 1000; // 10 millimeter
            float pixelSizeInMicrometer = 100;
            float waferDiameterInPixel = waferDiameterInMicrometer / pixelSizeInMicrometer;
            float notchWidthInPixel = notchWidthInMicrometer / pixelSizeInMicrometer;
            float waferDetectionToleranceInPixel = waferToleranceInMicrometer / pixelSizeInMicrometer;

            Circle waferCircle = psd::FindWaferCircle(input, waferDiameterInPixel, waferDetectionToleranceInPixel, 100, printForDebug);

            Assert::AreEqual(3070, waferCircle.Diameter, 1);
            Assert::AreEqual(1584, waferCircle.CenterPos.x, 1);
            Assert::AreEqual(1582, waferCircle.CenterPos.y, 1);

            // Find notch center

            psd::NotchLocation notchLocation = psd::NotchLocation::Top;

            int widthFactor = 18;
            double deviationFactor = 3.7;
            double similarityThreshold = 0.6;

            double notchAngle = psd::FindNotchCenterByPolarStats(input, waferCircle, notchLocation, (int)notchWidthInPixel, widthFactor, deviationFactor, similarityThreshold, printForDebug);

            Assert::AreEqual(-174.7, notchAngle, 0.1);
        }
    };
}