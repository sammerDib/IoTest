#include "CppUnitTest.h"

#include <opencv2/imgproc.hpp> // pour cv::phaseCorrelate

//#include <random>
#include <filesystem>

#include "SymmetryDetector.hpp"
#include "WaferImageGenerator.hpp"
#include "Linspace.hpp"

#pragma unmanaged

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

#define TEST_DATA_PATH std::string(".\\..\\..\\Tests\\Data\\")

namespace SharedOpenCVNativeTests
{
    namespace {
        EdgeImage::Pointer GenerateNotchImage(double angle, cv::Point2d waferShift) {
            WaferImageGenerator::Parameters params;
            params.pixelSize = cv::Point2d(5.3, 5.3);

            params.waferShift = waferShift;
            params.imageSize = cv::Point2i(1164, 872);
            params.waferRadius = 150000;
            params.waferRotationAngle = angle;
            params.notchRadius = 2500;
            params.imagesToTake.emplace_back("notch", WaferImageGenerator::ImageType::NOTCH, cv::Point2i(0, -150000));

            auto waferIntensityGenerator = [](cv::Point const& location) {
                uchar intensity = cv::theRNG().uniform(190, 255);
                return intensity;
            };
            params.waferPixelIntensityGenerator = std::make_shared<WaferImageGenerator::Parameters::PixelGenerator>(waferIntensityGenerator);

            auto chuckIntensityGenerator = [](cv::Point const& location) {
                uchar intensity = cv::theRNG().uniform(150, 180);
                return intensity;
            };
            params.chuckPixelIntensityGenerator = std::make_shared<WaferImageGenerator::Parameters::PixelGenerator>(chuckIntensityGenerator);

            WaferImageGenerator generator;
            auto result = generator.Generate(params, false);

            return result.takenImages.at(0).ToEdgeImage();
        }

        struct GivenType {
            std::string label;
            cv::Size imageSize;
            cv::Point2d pixelSize;
            cv::Point2d imageCentroid;
        };

        struct GivenTypeForRotation : GivenType {
            cv::Point2d waferCenter;
            double expectedRotation;
        };

        EdgeImage::Pointer CreateImageFromGiven(GivenType given) {
            cv::Mat matrix(given.imageSize, CV_8U);
            EdgeImage::PixelSize pixelSize(given.pixelSize);
            EdgeImage::ImageCentroid centroid(given.imageCentroid);
            EdgeImage::Pointer image = EdgeImage::New(given.label, matrix, pixelSize, centroid);
            return image;
        }
    } // namespace

	TEST_CLASS(SymmetryDetectorTests)
	{
	public:
		
		TEST_METHOD(_01_Expect_padImage_to_create_expected_image)
		{
            Logger::WriteMessage("Start\n");
            SymmetryDetector::ImagePadder padder;
            const int EXPECTED_TOP_PADDING_VALUE = 200;
            const int EXPECTED_BOTTOM_PADDING_VALUE = 100;
            const int EXPECTED_PADDED_IMAGE_ROWS = 20;

            cv::Mat source = cv::Mat::ones(cv::Size(1, 10), CV_8U);
            source.at<uchar>(0, 0) = EXPECTED_BOTTOM_PADDING_VALUE; // all bottom padding should have this value
            source.at<uchar>(9, 0) = EXPECTED_TOP_PADDING_VALUE;    // all top padding should have this value

            Logger::WriteMessage("Perform PadImage\n");
            cv::Mat actual = padder.PadImage(source);

            Assert::AreEqual(EXPECTED_PADDED_IMAGE_ROWS, actual.rows, L"Bad nb of rows");

            wchar_t wbuf[255];
            for (int rowIndex = 0; rowIndex < 5; ++rowIndex) {
                _snwprintf_s(wbuf, 255, L"Bad value in top padding at row index [%d]\n", rowIndex);
                Assert::AreEqual(EXPECTED_TOP_PADDING_VALUE, (int) actual.at<uchar>(rowIndex, 0), wbuf);
            }

            for (int rowIndex = 15; rowIndex < 20; ++rowIndex) {
                _snwprintf_s(wbuf, 255, L"Bad value in bottom padding at row index [%d]\n", rowIndex);
                Assert::AreEqual(EXPECTED_BOTTOM_PADDING_VALUE, (int)actual.at<uchar>(rowIndex, 0), wbuf);
            }
            Logger::WriteMessage("Done\n");
		}

        TEST_METHOD(_02_Expect_ComputeAngleFromCorrelation_to_return_right_values)
        {
            Logger::WriteMessage("Start\n");
            struct GivenType {
                std::wstring label;
                double givenCorrelationOrdinate;
                std::vector<double> givenAngles;
                cv::Size croppedPaddedImage;
                cv::Size polarImageBeforeCrop;
                cv::Rect givenCroppedRegion;
                double expectedAngle;
            };

            std::vector<GivenType> givens;
            std::vector<double> angles = linspace<double>(4.687761, 4.729124, 1024);
            cv::Rect croppedRegion(9, 18, 1006, 995);
            cv::Size polarImageBeforeCrop(1024, 1024);
            cv::Size croppedPadded(1006, 1989);

            const int EXPECTED_ANGLE_INDEX = 246;

            givens.push_back(GivenType({ L"Values from metrological wafer", 228, angles, croppedPadded, polarImageBeforeCrop, croppedRegion, angles[EXPECTED_ANGLE_INDEX] }));

            // we use here an croppedPadded image which have 16 rows and one column.
            // 16 rows means 8 rows of cropped image at the center, and 4 rows of padding before and after the cropped image.
            // The polar image is cropped: one line on top and one on bottom.
            angles = linspace<double>(0, 9, 10);
            polarImageBeforeCrop = cv::Size(1, 10);
            croppedRegion = cv::Rect(0, 1, 1, 8);
            croppedPadded = cv::Size(1, 16);

            //  boundaries checks
            givens.push_back(GivenType({ L"[Bounds] Peak before the cropped polar image", -9, angles, croppedPadded, polarImageBeforeCrop, croppedRegion, std::numeric_limits<double>::quiet_NaN() }));
            givens.push_back(GivenType({ L"[Bounds] Peak after the cropped polar image", 12, angles, croppedPadded, polarImageBeforeCrop, croppedRegion, std::numeric_limits<double>::quiet_NaN() }));
            givens.push_back(GivenType({ L"[Value] Theorical good angle for test data", 3, angles, croppedPadded, polarImageBeforeCrop, croppedRegion, 4 }));


            wchar_t wbuf[255];
            for (auto const& given : givens) {

                SymmetryDetector detector;
                double actualAngle = detector.ComputeAngleFromCorrelation(given.givenCorrelationOrdinate, given.givenAngles, given.croppedPaddedImage, given.polarImageBeforeCrop, given.givenCroppedRegion);

                if (std::isnan(given.expectedAngle)) {
                    _snwprintf_s(wbuf, 255, L"NaN angle : Fail for case = <%s>\n", given.label.c_str());
                    Assert::IsTrue(std::isnan(actualAngle), wbuf);
                }
                else {
                    _snwprintf_s(wbuf, 255, L"Angle : Fail for case = <%s>\n", given.label.c_str());
                    Assert::AreEqual(given.expectedAngle, actualAngle, 10e-3, wbuf);
                }
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_03_Works_on_metrological_wafer_notch)
        {
            Logger::WriteMessage("Start\n");
            std::string currentPath = std::filesystem::current_path().string();
            cv::Point2d centroid(0, -150000);
            EdgeImage::PixelSize pixelSize({ 5.3, 5.3 });
            cv::Point2d detectedShift(111 * pixelSize.get().x, 246 * pixelSize.get().y);

            EdgeImage::Pointer image = EdgeImage::New(TEST_DATA_PATH + std::string("EdgeDetection_0_2X_VIS_X_0_Y_-150000.png"), pixelSize, EdgeImage::ImageCentroid(centroid));
            Assert::IsFalse(image->Mat().empty(), L"image should not be empty - check image path or opencv install");

            // when
            SymmetryDetector sd;
            double angleConfidence = 0.0;
            int notchWidthInMicrons = 3000;
            int waferDiameterInMm = 300;
            Logger::WriteMessage("Perform SymmetryDetector\n");
            double measuredSymetryAxisAngleRad = sd.DetectV3(image.get(), detectedShift, waferDiameterInMm, notchWidthInMicrons, angleConfidence, currentPath);

            // then
            double EXPECTED_SYMMETRY_AXIS_ANGLE_RADIANS = 4.69860247;
            Assert::AreEqual(EXPECTED_SYMMETRY_AXIS_ANGLE_RADIANS, measuredSymetryAxisAngleRad, 10e-4);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_04_Expect_opencv_phaseCorrelate_to_detect_shift)
        {
            Logger::WriteMessage("Start\n");
            auto cat = cv::imread(TEST_DATA_PATH + std::string("cat.png"), cv::IMREAD_GRAYSCALE);
            Assert::IsFalse(cat.empty(), L"Image 1 is missing");

            cv::Point2d expectedShift(10, 10);
            auto cat2 = cv::imread(TEST_DATA_PATH + std::string("cat2.png"), cv::IMREAD_GRAYSCALE);
            Assert::IsFalse(cat2.empty(), L"Image 2 is missing");

            cat.convertTo(cat, CV_32FC1);
            cat2.convertTo(cat2, CV_32FC1);

            Logger::WriteMessage("Perform phaseCorrelate\n");
            cv::Point2d opencvShift = cv::phaseCorrelate(cat, cat2);
            Assert::AreEqual(expectedShift.x, opencvShift.x, 10e-2);
            Assert::AreEqual(expectedShift.y, opencvShift.y, 10e-2);
            Logger::WriteMessage("Done\n");
        }

#ifndef NDEBUG
        TEST_METHOD(_05_DEBUG_Works_on_generated_wafer_image)
        {
            Logger::WriteMessage("Start\n");
            std::string currentPath = std::filesystem::current_path().string();

            std::vector<double> angles;

            // test all values from 0.5 to -0.5° using steps of 0.1°
            for (double i = 0.5; i >= -0.5; i -= 0.1) {
                angles.push_back(i);
            }

            wchar_t wbuf[255];
            for (auto const& angle : angles) {

                _snwprintf_s(wbuf, 255, L"Testing angle = %lf °\n", angle);
                Logger::WriteMessage(wbuf);

                // Given
                auto waferShift = cv::Point2d(0, 0);
                auto generatedImage = GenerateNotchImage(angle, waferShift);

                // When
                SymmetryDetector sd;
                double angleConfidence;
                int notchWidthInMicrons = 3000;
                int waferDiameterInMm = 300;
                double measuredAngleInRadians = sd.DetectV3(generatedImage.get(), waferShift, waferDiameterInMm, notchWidthInMicrons, angleConfidence, currentPath);

                // Then
                // angles are returned as raw polar angles, where generator accepts a
                // shift around 270°. We have to rescale angle to cancel that 270°
                //
                // NOTE: sign of angle must be reverted regarding generation order
                //
                double EXPECTED_ANGLE_IN_RADIANS = (270 - angle) / 180.0 * CV_PI;
                _snwprintf_s(wbuf, 255, L"Exp<%lf>;\tVal<%lf>\t\tdiff<%lf>\ta=%lf\n", EXPECTED_ANGLE_IN_RADIANS, measuredAngleInRadians, EXPECTED_ANGLE_IN_RADIANS- measuredAngleInRadians, angle);
                Logger::WriteMessage(wbuf);
                _snwprintf_s(wbuf, 255, L"Validation failed for angle (radian validation) = %lf °\n", angle);
                Assert::AreEqual(EXPECTED_ANGLE_IN_RADIANS, measuredAngleInRadians, 0.03, wbuf);

            }
            Logger::WriteMessage("Done\n");
        }
#endif

        TEST_METHOD(_06_Expect_angle_detect_to_fail_when_notch_is_out_of_frame)
        {
            Logger::WriteMessage("Start\n");
            std::string currentPath = std::filesystem::current_path().string();
            
            auto waferShift = cv::Point2d(0, 0);
            double angle = -2.0;
            auto generatedImage = GenerateNotchImage(angle, waferShift);

            // When
            SymmetryDetector sd;
            double angleConfidence;
            int notchWidthInMicrons = 3000;
            int waferDiameterInMm = 300;
            double measuredAngleInRadians = sd.DetectV3(generatedImage.get(), waferShift, waferDiameterInMm, notchWidthInMicrons, angleConfidence, currentPath);

            Assert::IsTrue(std::isnan(measuredAngleInRadians));

            Logger::WriteMessage("Done\n");
        }


	};
}
