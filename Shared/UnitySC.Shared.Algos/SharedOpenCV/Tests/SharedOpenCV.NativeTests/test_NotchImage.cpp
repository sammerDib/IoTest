#include "CppUnitTest.h"

#include <filesystem>
#include <NotchImage.hpp>

#pragma unmanaged

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

#define TEST_DATA_PATH std::string(".\\..\\..\\Tests\\Data\\")

namespace SharedOpenCVNativeTests
{
	TEST_CLASS(NotchImageTests)
	{
	public:
		
		TEST_METHOD(Expect_notch_image_to_return_rotation_not_symmetry_axis_angle)
		{
            Logger::WriteMessage("Start\n");
            std::string currentPath = std::filesystem::current_path().string();
            // given
            cv::Point2d centroid(0, -150000);
            EdgeImage::PixelSize pixelSize({ 5.3, 5.3 });
            cv::Point2d detectedShift(111 * pixelSize.get().x, 246 * pixelSize.get().y);

            NotchImage::Pointer image =
                NotchImage::New(TEST_DATA_PATH + std::string("notch_0_2X_VIS_X_0_Y_-150000.png"), pixelSize, EdgeImage::ImageCentroid(centroid));
             Assert::IsFalse(image->Mat().empty(), L"image should not be empty - check image path or opencv install");

            // when
            double waferRotation;
            int notchWidthInMicrons = 3000;
            int waferDiameterInMm = 300;
            int notchDetectionVersion = 3;
            image->GetRotation(waferRotation, detectedShift, waferDiameterInMm, notchWidthInMicrons, notchDetectionVersion, currentPath);

            // then
            const double threePiOnTwo = 3 * CV_PI / 2;                                         // 270° / 4.7123889803846 rad
            double expectedRotationAxisAngle = 4.7137503372;                                   // 270.078°
            double EXPECTED_WAFER_ROTATION_RADIANS = expectedRotationAxisAngle - threePiOnTwo; // .078°
            double detectedRotationDeg = -waferRotation;
            Assert::AreEqual(EXPECTED_WAFER_ROTATION_RADIANS, -waferRotation, 10e-2, L"This should not be something like 270.078");
            Logger::WriteMessage("Done\n");
		}
	};
}
