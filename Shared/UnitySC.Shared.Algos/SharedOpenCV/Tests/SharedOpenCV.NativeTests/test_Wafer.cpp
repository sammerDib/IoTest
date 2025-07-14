#include "CppUnitTest.h"
#include <random>
#include <chrono>
#include <filesystem>

#include "Wafer.hpp"
#include "WaferImageGenerator.hpp"

#pragma unmanaged

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

#define TEST_DATA_PATH std::string(".\\..\\..\\Tests\\Data\\")

namespace SharedOpenCVNativeTests
{
    namespace {

        class WaferGenerator {
        public:
            struct GenerationParam {

                // micrometers
                cv::Point2d shift;

                // micrometers
                double radius;

                double angle;

                // micrometers per pixel
                cv::Point2d pixelSize;

                std::string reportPath;

                GenerationParam() { reportPath = std::filesystem::current_path().string(); }
            };

            /*!
             *
             * Generate a wafer which fits in a 2000*2000 image, split in four quadrants
             *
             */
            static Wafer GetSmallGeneratedWafer(GenerationParam input) {
                WaferImageGenerator::Parameters params;

                AddPixelIntensityGenerators(params);

                params.pixelSize = input.pixelSize;

                // params.waferShift = cv::Point2d(0, 0); // shift in pixels
                params.waferShift = input.shift; // shift in pixels
                // convert shift to µm
                params.waferShift.x *= params.pixelSize.x;
                params.waferShift.y *= params.pixelSize.y;

                params.pixelSize = cv::Point2d(1, 1);
                params.imageSize = cv::Point2i(1000, 1000);

                params.waferRadius = (int) input.radius;
                params.waferRotationAngle = input.angle;
                params.notchRadius = 80;

                params.imagesToTake.emplace_back("top", WaferImageGenerator::ImageType::EDGE, cv::Point2i(0, 500));
                params.imagesToTake.emplace_back("left", WaferImageGenerator::ImageType::EDGE, cv::Point2i(-500, 0));
                params.imagesToTake.emplace_back("right", WaferImageGenerator::ImageType::EDGE, cv::Point2i(500, -0));
                params.imagesToTake.emplace_back("notch", WaferImageGenerator::ImageType::NOTCH, cv::Point2i(0, -500));

                WaferImageGenerator generator;
                auto result = generator.Generate(params);
                auto pixelSize = EdgeImage::PixelSize(params.pixelSize);

                auto data = Wafer::WaferData();
                data.radiusInMicrons = (int) input.radius;
                data.notchWidthInMicrons = params.notchRadius;
                data.type = Wafer::WaferData::Type::NOTCH;

                Wafer wafer(data);

                for (auto image : result.takenImages) {
                    auto imageCentroid = EdgeImage::ImageCentroid(image.centroid);

                    EdgeImage::Pointer edgeImage;
                    NotchImage::Pointer notchImage;
                    switch (image.type) {

                    case WaferImageGenerator::ImageType::EDGE:
                        edgeImage = EdgeImage::New(image.label, image.content, pixelSize, imageCentroid);
                        wafer.AddEdgeImage(edgeImage, input.reportPath);
                        continue;

                    case (WaferImageGenerator::ImageType::NOTCH):
                        notchImage = NotchImage::New(image.label, image.content, pixelSize, imageCentroid);
                        wafer.AddNotchImage(notchImage, input.reportPath);
                        continue;
                    }
                }
                return wafer;
            }

            /*!
             *
             * Generate a 300mm wafer using 5x objective.
             *
             * A pixel measure 2.125µm here
             *
             */
            static Wafer Get300mmAt5x(GenerationParam input) {

                WaferImageGenerator::Parameters params;
                params.pixelSize = input.pixelSize;
                params.waferShift.x = input.shift.x * input.pixelSize.x;
                params.waferShift.y = input.shift.y * input.pixelSize.y;

                params.imageSize = cv::Point2i(2328, 1744);
                params.waferRadius = (int) input.radius;
                params.waferRotationAngle = input.angle;
                params.notchRadius = 2000;

                // positions taken from FPMS's BWA procedure
                params.imagesToTake.emplace_back("-26047,-147721 bottom-left", WaferImageGenerator::ImageType::EDGE, cv::Point2i(-26047, -147721));
                params.imagesToTake.emplace_back("23465,148153 top-right", WaferImageGenerator::ImageType::EDGE, cv::Point2i(23465, 148153));
                params.imagesToTake.emplace_back("139077,56190 top-right-right", WaferImageGenerator::ImageType::EDGE, cv::Point2i(139077, 56190));
                params.imagesToTake.emplace_back("0,-150000 notch", WaferImageGenerator::ImageType::NOTCH, cv::Point2i(0, -150000));

                AddPixelIntensityGenerators(params);

                WaferImageGenerator generator;
                auto result = generator.Generate(params);
                auto pixelSize = EdgeImage::PixelSize(params.pixelSize);

                auto data = Wafer::WaferData();
                data.notchWidthInMicrons = params.notchRadius;
                data.radiusInMicrons = params.waferRadius;
                Wafer wafer(data);

                for (auto image : result.takenImages) {
                    auto imageCentroid = EdgeImage::ImageCentroid(image.centroid);

                    EdgeImage::Pointer edgeImage;
                    NotchImage::Pointer notchImage;
                    switch (image.type) {

                    case WaferImageGenerator::ImageType::EDGE:
                        edgeImage = EdgeImage::New(image.label, image.content, pixelSize, imageCentroid);
                        wafer.AddEdgeImage(edgeImage, input.reportPath);
                        continue;

                    case (WaferImageGenerator::ImageType::NOTCH):
                        notchImage = NotchImage::New(image.label, image.content, pixelSize, imageCentroid);
                        wafer.AddNotchImage(notchImage, input.reportPath);
                        continue;
                    }
                }
                return wafer;
            }

            /*!
             *
             * Generate a 100mm wafer using 2x objective.
             *
             * A pixel measure 5.25µm here
             *
             */
            static Wafer Get100mmAt2x(GenerationParam input) {

                WaferImageGenerator::Parameters params;
                params.pixelSize = input.pixelSize;
                params.waferShift.x = input.shift.x * input.pixelSize.x;
                params.waferShift.y = input.shift.y * input.pixelSize.y;

                params.imageSize = cv::Point2i(1164, 872);
                params.waferRadius = (int)input.radius;
                params.waferRotationAngle = input.angle;
                params.notchRadius = 2000;

                // positions taken from FPMS's BWA procedure
                
                params.imagesToTake.emplace_back("-6882,-49240 bottom-left", WaferImageGenerator::ImageType::EDGE, cv::Point2i(-6882, -49240));
                params.imagesToTake.emplace_back("7821,49384 top-right", WaferImageGenerator::ImageType::EDGE, cv::Point2i(7821, 49384));
                params.imagesToTake.emplace_back("46359,18730 top-right-right", WaferImageGenerator::ImageType::EDGE, cv::Point2i(46359, 18730));
                params.imagesToTake.emplace_back("0,-50000 notch", WaferImageGenerator::ImageType::NOTCH, cv::Point2i(0, -50000));
                


                AddPixelIntensityGenerators(params);

                WaferImageGenerator generator;
                auto result = generator.Generate(params);
                auto pixelSize = EdgeImage::PixelSize(params.pixelSize);

                auto data = Wafer::WaferData();
                data.notchWidthInMicrons = params.notchRadius;
                data.radiusInMicrons = params.waferRadius;
                Wafer wafer(data);

                for (auto image : result.takenImages) {
                    auto imageCentroid = EdgeImage::ImageCentroid(image.centroid);

                    EdgeImage::Pointer edgeImage;
                    NotchImage::Pointer notchImage;
                    switch (image.type) {

                    case WaferImageGenerator::ImageType::EDGE:
                        edgeImage = EdgeImage::New(image.label, image.content, pixelSize, imageCentroid);
                        wafer.AddEdgeImage(edgeImage, input.reportPath);
                        continue;

                    case (WaferImageGenerator::ImageType::NOTCH):
                        notchImage = NotchImage::New(image.label, image.content, pixelSize, imageCentroid);
                        wafer.AddNotchImage(notchImage, input.reportPath);
                        continue;
                    }
                }
                return wafer;
            }

            Wafer GetParametersForTest(GenerationParam input) {
                WaferImageGenerator::Parameters params;
                params.pixelSize = input.pixelSize;
                params.waferShift = input.shift; // shift in pixels
                params.imageSize = cv::Point2i(20, 20);
                params.waferRadius = (int) input.radius;
                params.waferRotationAngle = input.angle;
                params.notchRadius = 2;

                // positions taken from FPMS's BWA procedure
                params.imagesToTake.emplace_back("top-right", WaferImageGenerator::ImageType::EDGE, cv::Point2i(35, 35));
                params.imagesToTake.emplace_back("bottom-left", WaferImageGenerator::ImageType::EDGE, cv::Point2i(-15, -45));

                WaferImageGenerator generator;

                auto result = generator.Generate(params);
                auto pixelSize = EdgeImage::PixelSize(params.pixelSize);

                auto data = Wafer::WaferData();
                data.notchWidthInMicrons = params.notchRadius;
                data.radiusInMicrons = params.waferRadius;
                Wafer wafer(data);
                for (auto takenImage : result.takenImages) {
                    EdgeImage::Pointer edgeImage = takenImage.ToEdgeImage();
                    wafer.AddEdgeImage(edgeImage, input.reportPath);
                }
                return wafer;
            }

        private:
            static void AddPixelIntensityGenerators(WaferImageGenerator::Parameters& params) {

                auto waferIntensityGenerator = [](cv::Point const& location) {
                    uchar intensity = cv::theRNG().uniform(180, 255);
                    return intensity;
                };
                params.waferPixelIntensityGenerator = std::make_shared<WaferImageGenerator::Parameters::PixelGenerator>(waferIntensityGenerator);

                auto chuckIntensityGenerator = [](cv::Point const& location) {
                    uchar intensity = cv::theRNG().uniform(10, 30);
                    return intensity;
                };
                params.chuckPixelIntensityGenerator = std::make_shared<WaferImageGenerator::Parameters::PixelGenerator>(chuckIntensityGenerator);
            }
        };
    } // namespace

	TEST_CLASS(WaferTests)
	{
	public:
		
		TEST_METHOD(Expect_metrological_wafer_fit)
		{
            Logger::WriteMessage("Start\n");
            std::vector<EdgeImage::Pointer> images;
            auto data = Wafer::WaferData();
            data.notchWidthInMicrons = 3000;
            data.radiusInMicrons = 150000;
            Wafer wafer(data);
            auto currentPath = std::filesystem::current_path().string();
          
            EdgeImage::PixelSize pixelSize(cv::Point2d(5.3, 5.3));
            EdgeImage::ImageCentroid imageCenter(cv::Point2i(139077, 56190));

            auto image = EdgeImage::New(TEST_DATA_PATH + std::string("EdgeDetection_3_2X_VIS_X_139077_Y_56190.png"), pixelSize, imageCenter);
            assert(!image->Mat().empty());
            wafer.AddEdgeImage(image, currentPath);

            imageCenter = EdgeImage::ImageCentroid(cv::Point2i(23465, 148153));
            image = EdgeImage::New(TEST_DATA_PATH + std::string("EdgeDetection_2_2X_VIS_X_23465_Y_148153.png"), pixelSize, imageCenter);
            assert(!image->Mat().empty());
            wafer.AddEdgeImage(image, currentPath);

            imageCenter = EdgeImage::ImageCentroid(cv::Point2i(-26047, -147721));
            image = EdgeImage::New(TEST_DATA_PATH + std::string("EdgeDetection_1_2X_VIS_X_-26047_Y_-147721.png"), pixelSize, imageCenter);
            assert(!image->Mat().empty());
            wafer.AddEdgeImage(image, currentPath);

            imageCenter = EdgeImage::ImageCentroid(cv::Point2i(0, -150000));
            NotchImage::Pointer notchImage;
            notchImage = NotchImage::New(TEST_DATA_PATH + std::string("EdgeDetection_0_2X_VIS_X_0_Y_-150000.png"), pixelSize, imageCenter);
            assert(!image->Mat().empty());
            wafer.AddNotchImage(notchImage, currentPath);

            Wafer::WaferGeometricalParameters result;
            int edgeDetectionVersion = 2;
            int cannyThreshold = 200;
            Algorithms::Status status = wafer.GetGeometricalParameters(&result, edgeDetectionVersion, cannyThreshold, currentPath);

            // 111 and 246 are pixel values measured manually
            const cv::Point2d expectedWaferShift = cv::Point2d(111, 246);

            // FIXME: with the black border, it should be closer to 150mm
            const double expectedRadius = 150000;

            cv::Point2d measuredCenterShift = result.centerShift;
            double measuredRadius = result.radius;

            auto shiftToleranceInMicrons = 10.0 * pixelSize.get().x;
            double expectedShiftXInMicrons = expectedWaferShift.x * pixelSize.get().x;
            double expectedShiftYInMicrons = expectedWaferShift.y * pixelSize.get().y;

            Assert::AreEqual(expectedShiftXInMicrons, measuredCenterShift.x, shiftToleranceInMicrons);
            Assert::AreEqual(expectedShiftYInMicrons, measuredCenterShift.y, shiftToleranceInMicrons);

            // 260 microns over 150'000 microns of radius
            int diameterToleranceInMicrons = 260;
            Assert::AreEqual(expectedRadius, measuredRadius, diameterToleranceInMicrons);

            // angle is -0.78, it means:
            // - notch is on the left of the image
            // - angle between wafer center and noch center is less than 270°
            const double expectedAngleInDegrees = -0.78; // -0.01378651059
            double measuredAngleInRadians;
            int notchDetectionVersion = 3;
            auto StatusB = wafer.GetMisalignmentAngle(measuredAngleInRadians, expectedWaferShift, notchDetectionVersion, currentPath);
            double measuredAngleInDegrees = measuredAngleInRadians / CV_PI * 180;

            // FIXME this tolerance is FAR TO HIGH
            double rotationTolerance = 0.1582;
            Assert::AreEqual(expectedAngleInDegrees, measuredAngleInDegrees, rotationTolerance);

#ifndef NDEBUG
            //  cv::waitKey();
#endif
            Logger::WriteMessage("Done\n");
		}
        
        TEST_METHOD(Expect_right_result_on_realistic_100mm_generated_wafer_2X)
        {
            Logger::WriteMessage("Start\n");
            auto currentPath = std::filesystem::current_path();

            WaferGenerator::GenerationParam A;
            A.radius = 50000;
            A.angle = 0.78;
            A.pixelSize = cv::Point2d(5.25, 5.25);
            A.shift = cv::Point2d(111 / A.pixelSize.x, 246 / A.pixelSize.y);

            Wafer wafer = WaferGenerator::Get100mmAt2x(A);
            Wafer::WaferGeometricalParameters resultA;
            int edgeDetectionVersion = 1; // using v1 here, while using v2 some errors occurs in find_contour due to image border, which affect correct edge rmse
            int cannyThreshold = 200;
            auto StatusA = wafer.GetGeometricalParameters(&resultA, edgeDetectionVersion, cannyThreshold, currentPath.string());
            cv::Point2d expectedWaferShiftInMicrometers = A.shift;
            cv::Point2d measuredWaferShiftInPixels = resultA.centerShift;
            double expectedRadius = A.radius;
            double measuredRadius = resultA.radius;

            // 1/15e of micrometer of allowed error
            Assert::AreEqual(expectedWaferShiftInMicrometers.x, measuredWaferShiftInPixels.x / A.pixelSize.x, 15e-1);
            Assert::AreEqual(expectedWaferShiftInMicrometers.y, measuredWaferShiftInPixels.y / A.pixelSize.y, 15e-1);

            // 10 micrometers
            Assert::AreEqual(expectedRadius, measuredRadius, 10);

            double measuredAngleInRadians;
            int notchDetectionVersion = 3;
            wafer.GetMisalignmentAngle(measuredAngleInRadians, A.shift, notchDetectionVersion, currentPath.string());
            double expectedAngleInDegrees = A.angle;
            double measuredAngleInDegrees = measuredAngleInRadians * 180 / CV_PI;

            double angleToleranceInDegrees = 0.3;
            Assert::AreEqual(expectedAngleInDegrees, measuredAngleInDegrees, angleToleranceInDegrees);

#ifndef NDEBUG
            cv::waitKey();
#endif
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(Expect_right_result_on_realistic_300mm_generated_wafer_5X)
        {
            Logger::WriteMessage("Start\n");

            auto currentPath = std::filesystem::current_path();

            WaferGenerator::GenerationParam A;
            A.radius = 150000;
            A.angle = 0.30;
            A.pixelSize = cv::Point2d(2.125, 2.125);
            A.shift = cv::Point2d(111 / A.pixelSize.x, 246 / A.pixelSize.y);

            Wafer wafer = WaferGenerator::Get300mmAt5x(A);
            Wafer::WaferGeometricalParameters resultA;
            int edgeDetectionVersion = 2;
            int cannyThreshold = 200;
            auto StatusA = wafer.GetGeometricalParameters(&resultA, edgeDetectionVersion, cannyThreshold, currentPath.string());
            cv::Point2d expectedWaferShiftInMicrometers = A.shift;
            cv::Point2d measuredWaferShiftInPixels = resultA.centerShift;
            double expectedRadius = A.radius;
            double measuredRadius = resultA.radius;

            // 1/10e of micrometer of allowed error
            Assert::AreEqual(expectedWaferShiftInMicrometers.x, measuredWaferShiftInPixels.x / A.pixelSize.x, 20e-1);
            Assert::AreEqual(expectedWaferShiftInMicrometers.y, measuredWaferShiftInPixels.y / A.pixelSize.y, 20e-1);

            // 5 micrometers
            Assert::AreEqual(expectedRadius, measuredRadius, 5);

            double measuredAngleInRadians;
            int notchDetectionVersion = 3;
            wafer.GetMisalignmentAngle(measuredAngleInRadians, A.shift, notchDetectionVersion, currentPath.string());
            double expectedAngleInDegrees = A.angle;
            double measuredAngleInDegrees = measuredAngleInRadians * 180 / CV_PI;

            double angleToleranceInDegrees = 0.3;
            Assert::AreEqual(expectedAngleInDegrees, measuredAngleInDegrees, angleToleranceInDegrees);

#ifndef NDEBUG
            cv::waitKey();
#endif
            Logger::WriteMessage("Done\n");
        }
	};
}
