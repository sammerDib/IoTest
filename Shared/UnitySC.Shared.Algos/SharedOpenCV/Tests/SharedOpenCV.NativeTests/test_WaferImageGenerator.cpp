#include "CppUnitTest.h"

#include <filesystem>

#include "WaferImageGenerator.hpp"
#include "Point.hpp"

#pragma unmanaged

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedOpenCVNativeTests
{
    namespace {

        WaferImageGenerator::Parameters GetParametersForWholeWaferInImage() {
            WaferImageGenerator::Parameters params;
            params.pixelSize = cv::Point2d(1, 1);
            params.waferShift = cv::Point2d(0, 0);
            params.imageSize = cv::Point2i(1024, 1024);
            params.waferRadius = 300;
            params.waferRotationAngle = 0;
            params.notchRadius = 80;
            params.notchCenterDistanceFromWaferBorder = 10;

            params.imagesToTake.emplace_back("wholeWafer", WaferImageGenerator::ImageType::EDGE, cv::Point2i(0, 0));

            return params;
        }

        WaferImageGenerator::Parameters GetParametersFor300mm2x() {
            WaferImageGenerator::Parameters params;
            params.pixelSize = cv::Point2d(5.3, 5.3);
            params.waferShift = cv::Point2d(0, 0);
            params.imageSize = cv::Point2i(1164, 872);
            params.waferRadius = 150000;
            params.waferRotationAngle = 0;
            params.notchRadius = 530;

            // positions taken from FPMS
            params.imagesToTake.emplace_back("bottom-left", WaferImageGenerator::ImageType::EDGE, cv::Point2i(-26047, -147721));
            params.imagesToTake.emplace_back("top-left", WaferImageGenerator::ImageType::EDGE, cv::Point2i(23465, 148153));
            params.imagesToTake.emplace_back("up-right", WaferImageGenerator::ImageType::EDGE, cv::Point2i(139077, 56190));
            params.imagesToTake.emplace_back("notch", WaferImageGenerator::ImageType::NOTCH, cv::Point2i(0, -150000));

            return params;
        }

        WaferImageGenerator::Parameters GetParametersFor300mm5x() {
            WaferImageGenerator::Parameters params;
            params.pixelSize = cv::Point2d(2.111, 2.111);
            params.waferShift = cv::Point2d(0, 0);
            params.imageSize = cv::Point2i(1280, 1024);
            params.waferRadius = 150000;
            params.waferRotationAngle = 0;
            params.notchRadius = 340;

            // positions taken from FPMS
            params.imagesToTake.emplace_back("up-right", WaferImageGenerator::ImageType::EDGE, cv::Point2i(139077, 56190));
            params.imagesToTake.emplace_back("top-left", WaferImageGenerator::ImageType::EDGE, cv::Point2i(23465, 148153));
            params.imagesToTake.emplace_back("bottom-left", WaferImageGenerator::ImageType::EDGE, cv::Point2i(-26047, -147721));
            params.imagesToTake.emplace_back("notch", WaferImageGenerator::ImageType::NOTCH, cv::Point2i(0, -150000));

            return params;
        }

        WaferImageGenerator::Parameters GetParametersForTestTiles() {
            WaferImageGenerator::Parameters params;
            params.pixelSize = cv::Point2d(2, 2);
            params.waferShift = cv::Point2d(0, 0);
            params.imageSize = cv::Point2i(512, 512);
            params.waferRadius = 400;
            params.waferRotationAngle = 0;
            params.notchRadius = 20;
            params.imagesToTake.emplace_back("upleft", WaferImageGenerator::ImageType::EDGE, cv::Point2i((int)(-256.0 * params.pixelSize.x), (int)(256.0 * params.pixelSize.y)));
            params.imagesToTake.emplace_back("upright", WaferImageGenerator::ImageType::EDGE, cv::Point2i((int)(256.0 * params.pixelSize.x), (int)(256.0 * params.pixelSize.y)));
            params.imagesToTake.emplace_back("bottomright", WaferImageGenerator::ImageType::EDGE, cv::Point2i((int)(256.0 * params.pixelSize.x), (int)(-256.0 * params.pixelSize.y)));
            params.imagesToTake.emplace_back("bottomleft", WaferImageGenerator::ImageType::EDGE, cv::Point2i((int)(-256.0 * params.pixelSize.x), (int)(-256.0 * params.pixelSize.y)));

            return params;
        }
        void WriteToDisk(WaferImageGenerator::Result const& result) {

            srand((uint)time(0));
            auto tmpDir = std::filesystem::temp_directory_path();
            auto dataDir = tmpDir.append("test-" + std::to_string(rand() % 65535));
            std::filesystem::create_directory(dataDir);
            result.WriteToDisk(dataDir.string());
        }
    } // namespace

	TEST_CLASS(WaferImageGeneratorTests)
	{
	public:
		
		TEST_METHOD(_01_Expect_pixelSize_to_be_considered)
		{
            Logger::WriteMessage("Start\n");
            const cv::Point2d pixelSize(5.3, 5.3);
            WaferImageGenerator::Parameters params = GetParametersFor300mm2x();
            WaferImageGenerator generator;
            WaferImageGenerator::Result result = generator.Generate(params);
            Assert::IsTrue(result.pixelSize == pixelSize, L"Result pixelSize must be the one provided in Generate() call");
            Logger::WriteMessage("Done\n");
		}

        TEST_METHOD(_02_Expect_radius_to_be_considered)
        {
            Logger::WriteMessage("Start\n");
            WaferImageGenerator::Parameters params = GetParametersFor300mm2x();
            const int radiusFor300mm = 150000;
            WaferImageGenerator generator;
            WaferImageGenerator::Result result = generator.Generate(params);
            Assert::IsTrue(result.radius == radiusFor300mm, L"Result wafer radius must be the one provided in Generate() call");
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_03_Expect_notch_rotation_to_be_right)
        {
            Logger::WriteMessage("Start\n");
            wchar_t wbuf[255];
            std::vector<double> angles;

            angles.push_back(0);

            angles.push_back(200);
            angles.push_back(20);
            angles.push_back(2);
            angles.push_back(0.30);
            angles.push_back(0.02);

            angles.push_back(-200);
            angles.push_back(-20);
            angles.push_back(-2);
            angles.push_back(-0.2);
            angles.push_back(-0.02);

            angles.push_back(-0.78);
            angles.push_back(-0.79);
            angles.push_back(-0.80);

            WaferImageGenerator generator;

            // we create a wafer image where notch is a single pixel, at mid distance
            // between wafer center and its border. These information allow to ensure
            // there is one black (notch) pixel at the expected image coordinates.
            WaferImageGenerator::Parameters params = GetParametersForWholeWaferInImage();
            params.notchRadius = 1;
            params.postProcess = nullptr;
            const double BOTTOM_POLAR = (3.0 / 2.0 * CV_PI); // 270°, bottom
            params.notchCenterDistanceFromWaferBorder = -params.waferRadius / 2;
            for (auto rotationAngle : angles) {

                double polarAngle = BOTTOM_POLAR + ((rotationAngle / 180.0) * CV_PI);
                cv::Point2d expectedNotchPolarPoint = cv::Point2d(std::abs(params.notchCenterDistanceFromWaferBorder), polarAngle);
                cv::Point2d expectedNotchCartesianPoint = Point::PolarToCartesian(expectedNotchPolarPoint);

                // shift point to global image coordinates
                expectedNotchCartesianPoint.x += params.imageSize.width / 2.0;
                expectedNotchCartesianPoint.y = -expectedNotchCartesianPoint.y + params.imageSize.height / 2.0;

                params.waferRotationAngle = rotationAngle;
                auto generationResult = generator.Generate(params);
                auto& generatedImage = generationResult.takenImages.at(0).content;

                generatedImage.convertTo(generatedImage, CV_8UC1);
                auto notch = generatedImage.at<unsigned char>(expectedNotchCartesianPoint);

                _snwprintf_s(wbuf, 255, L"Failed for %lf at (%lf,%lf)", rotationAngle, expectedNotchCartesianPoint.x, expectedNotchCartesianPoint.y);
                Assert::IsTrue(notch < 127, wbuf);
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_04_Expect_pixel_shift_to_be_considered)
        {
            Logger::WriteMessage("Start\n");
            WaferImageGenerator::Parameters params = GetParametersFor300mm2x();
            params.waferShift = cv::Point2d(10, -5);
            WaferImageGenerator generator;
            WaferImageGenerator::Result result = generator.Generate(params);
            Assert::IsTrue(result.pixelShift == params.waferShift, L"Result wafer shift must be the one provided in Generate() call");
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_05_Expect_test_tiles)
        {
            Logger::WriteMessage("Start\n");
            // generate a wafer image split on 4 quandrants

            WaferImageGenerator::Parameters params = GetParametersForTestTiles();

            WaferImageGenerator generator;
            Logger::WriteMessage("Perform WaferImageGenerator.Generate\n");
            WaferImageGenerator::Result result = generator.Generate(params);

            Assert::IsTrue(result.takenImages.size() == 4,L"We should have 4 quadrant images");

            cv::Mat merged(cv::Size(1024, 1024), CV_8U);

            // manually merge the 4 quadrants
            auto upLeft = merged(cv::Rect(0, 0, 512, 512));
            result.takenImages.at(0).content.copyTo(upLeft);
            auto upRight = merged(cv::Rect(512, 0, 512, 512));
            result.takenImages.at(1).content.copyTo(upRight);
            auto downRight = merged(cv::Rect(512, 512, 512, 512));
            result.takenImages.at(2).content.copyTo(downRight);
            auto downLeft = merged(cv::Rect(0, 512, 512, 512));
            result.takenImages.at(3).content.copyTo(downLeft);

            // use the generator to generate one tile with the
            // same content as above
            params.imageSize = cv::Size(1024, 1024);
            params.imagesToTake.clear();
            params.imagesToTake.emplace_back("onlyOne", WaferImageGenerator::ImageType::EDGE, cv::Point2i(0, 0));
            result = generator.Generate(params);
            cv::Mat expected = result.takenImages.at(0).content;

            cv::Mat diff;
            cv::absdiff(merged, expected, diff);

            // erode a bit the difference image so that we ignore slight error due to
            // pixel approximation when using discrete geometry
            auto kernel = cv::getStructuringElement(cv::MORPH_CROSS, cv::Size(3, 3));
            cv::morphologyEx(diff, diff, cv::MORPH_OPEN, kernel);
            int differentPixeCount = cv::countNonZero(diff);

            Assert::AreEqual(differentPixeCount, 0, L"There must be no differences between generated and expected images");
            Logger::WriteMessage("Done\n");
        }
	};
}
