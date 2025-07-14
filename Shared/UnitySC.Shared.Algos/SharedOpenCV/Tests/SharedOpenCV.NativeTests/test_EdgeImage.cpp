#include <opencv2/opencv.hpp>
#include "CppUnitTest.h"

#include <filesystem>
// for wstring converions
#include <codecvt>
#include <locale>

#include "EdgeImage.hpp"

#pragma unmanaged

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

#define TEST_DATA_PATH std::string(".\\..\\..\\Tests\\Data\\")

#pragma warning(suppress : 4996)
using convert_t = std::codecvt_utf8<wchar_t>;
#pragma warning(suppress : 4996)
std::wstring_convert<convert_t, wchar_t> strconverter;

namespace SharedOpenCVNativeTests
{
    namespace {

        struct GivenType {
            std::string label;
            cv::Size imageSize;
            cv::Point2d pixelSize;
            cv::Point2d imageCentroid;
        };

        struct GivenTypeForOrigin : GivenType {
            cv::Point2d expectedOrigin;
        };

        EdgeImage::Pointer CreateImageFromGiven(GivenType given) {
            cv::Mat matrix(given.imageSize, CV_8U);
            EdgeImage::PixelSize pixelSize(given.pixelSize);
            EdgeImage::ImageCentroid centroid(given.imageCentroid);
            EdgeImage::Pointer image = EdgeImage::New(given.label, matrix, pixelSize, centroid);
            return image;
        }
    } // namespace

	TEST_CLASS(EdgeImageTests)
	{
	public:
		
		TEST_METHOD(_01_Expect_image_without_filename_to_have_random_32_char_name)
		{
            Logger::WriteMessage("Start\n");
            EdgeImage::PixelSize pixelSize(cv::Point2d(1, 1));
            EdgeImage::ImageCentroid centroid(cv::Point(0, 0));
            std::string emptyFilepath = "";

            EdgeImage::Pointer image;
            image = EdgeImage::New(emptyFilepath, pixelSize, centroid);
            Assert::AreEqual(32, (int) image->GetName().length());

            cv::Mat dummy = cv::Mat::ones(cv::Size(0, 0), CV_8U);
            image = EdgeImage::New(dummy, pixelSize, centroid);
            Assert::AreEqual(32, (int) image->GetName().length());
            Logger::WriteMessage("Done\n");
		}

        TEST_METHOD(_02_Expect_image_generated_names_to_change_for_two_images)
        {
            Logger::WriteMessage("Start\n");
            EdgeImage::PixelSize pixelSize(cv::Point2d(1, 1));
            EdgeImage::ImageCentroid centroid(cv::Point(0, 0));
            std::string emptyFilepath = "";

            EdgeImage::Pointer imageOne;
            imageOne = EdgeImage::New(emptyFilepath, pixelSize, centroid);

            EdgeImage::Pointer imageTwo;
            imageTwo = EdgeImage::New(emptyFilepath, pixelSize, centroid);

            auto imageOneName = imageOne->GetName();
            Assert::AreEqual(32,(int) imageOneName.length());

            auto imageTwoName = imageTwo->GetName();
            Assert::AreEqual(32,(int)imageTwoName.length());

            Assert::AreNotEqual(imageOneName, imageTwoName, L"Two images must have different generated names");
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_03_Expect_image_generated_names_to_remain_same_during_image_lifetime)
        {
            Logger::WriteMessage("Start\n");
            EdgeImage::PixelSize pixelSize(cv::Point2d(1, 1));
            EdgeImage::ImageCentroid centroid(cv::Point(0, 0));
            std::string emptyFilepath = "";

            EdgeImage::Pointer image;
            image = EdgeImage::New(emptyFilepath, pixelSize, centroid);
            auto firstCallResult = image->GetName();
            Assert::AreEqual(32, (int) firstCallResult.length());

            auto secondCallResult = image->GetName();
            Assert::AreEqual(32, (int) secondCallResult.length());
            Assert::AreEqual(firstCallResult, secondCallResult,L"Generated name must remain the same against multiple calls");
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_04_Expect_image_origin_to_match_expected)
        {
            Logger::WriteMessage("Start\n");
            std::vector<GivenTypeForOrigin> givens;
            givens.emplace_back(GivenTypeForOrigin{ "simple image", cv::Size(4, 4), cv::Point(1, 1), cv::Point(0, -5), cv::Point2d(-1.5, -3.5) });
            givens.emplace_back(
                GivenTypeForOrigin{ "barycenter inside the image", cv::Size(1000, 1000), cv::Point(2, 2), cv::Point(0, 0), cv::Point2d(-999, 999) });
            givens.emplace_back(
                GivenTypeForOrigin{ "Image far from the 0,0", cv::Size(1000, 1000), cv::Point(1, 1), cv::Point(500, 500), cv::Point2d(.5, 999.5) });
            givens.emplace_back(
                GivenTypeForOrigin{ "Image far from the 0,0 with scaling", cv::Size(500, 500), cv::Point(2, 2), cv::Point(500, 500), cv::Point2d(1, 999) });

            for (auto const& given : givens) {

                EdgeImage::Pointer image = CreateImageFromGiven(given);

                // then
                cv::Point2d actualOrigin = image->GetOrigin();
                Assert::AreEqual(given.expectedOrigin.x, actualOrigin.x, L"Images origin not match in X");
                Assert::AreEqual(given.expectedOrigin.y, actualOrigin.y, L"Images origin not match in Y");
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_05_Expect_good_coordinates_when_point_shift_to_chuck_referential)
        {
            Logger::WriteMessage("Start\n");
            cv::Mat dummy(cv::Size(6, 6), CV_8U);
            EdgeImage::PixelSize pixelSize(cv::Point2d(1, 1));
            EdgeImage::ImageCentroid centroid(cv::Point(0, 0));
            std::string fileNotExist = "notExistFile.png";

            EdgeImage::Pointer image = EdgeImage::New(fileNotExist, dummy, pixelSize, centroid);

            std::vector<cv::Point2i> pointsToShift{ {0, 0}, {1, 1}, {3, 2} };
            std::vector<cv::Point2d> EXPECTED_POINTS{ {-2.5, 2.5}, {-1.5, 1.5}, {0.5, 0.5} };

            // when
            auto shiftedPoints = image->ShiftPointsFromImageToChuckReferential(pointsToShift);

            // then
            size_t pointIndex = 0;
            wchar_t wbuf[255];
            for (auto const& point : shiftedPoints) {
                auto expectedPoint = EXPECTED_POINTS.at(pointIndex);
                bool isOk = expectedPoint.x == point.x && expectedPoint.y == point.y;
                _snwprintf_s(wbuf, 255, L"Point [%d,%d] shifted at [%lf,%lf] does not match expected [%lf,%lf]\n", 
                    pointsToShift.at(pointIndex).x, pointsToShift.at(pointIndex).y,
                    point.x, point.y, expectedPoint.x, expectedPoint.y);
                Assert::IsTrue(isOk, wbuf);
                pointIndex++;
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_06_Expect_empty_to_return_true_if_there_is_no_data)
        {
            Logger::WriteMessage("Start\n");

            std::string fileNotExist = "notExistFile.png";
            auto image = EdgeImage::New(fileNotExist, EdgeImage::PixelSize(cv::Point2d(1, 1)), EdgeImage::ImageCentroid(cv::Point(1, 1)));
            Assert::IsTrue(image->Empty());
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_07_Expect_empty_to_return_false_if_there_is_data)
        {
            Logger::WriteMessage("Start\n");
            std::string filepath = TEST_DATA_PATH + std::string("dummyFullWafer.png");

            auto image = EdgeImage::New(filepath, EdgeImage::PixelSize(cv::Point2d(1, 1)),
                EdgeImage::ImageCentroid(cv::Point(1, 1)));

            Assert::IsFalse(image->Empty());
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_08_Expect_image_name_to_be_extracted_from_path)
        {
            Logger::WriteMessage("Start\n");

            std::string fileNotExist = "notExistFile.png";
            std::string filepath = TEST_DATA_PATH + fileNotExist;
            auto image = EdgeImage::New(filepath, EdgeImage::PixelSize(cv::Point2d(1, 1)),
                EdgeImage::ImageCentroid(cv::Point2d(139.077, 56.190)));
            std::string actual = image->GetName();
            Assert::AreEqual(fileNotExist, actual);

            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_09_Expect_image_center_to_be_readable)
        {
            Logger::WriteMessage("Start\n");
            auto pixelSize = EdgeImage::PixelSize(cv::Point2d(1, 1));
            auto expectedCenter = EdgeImage::ImageCentroid(cv::Point2d(-139.077, -56.190));
            std::string fileNotExist = "notExistFile.png";

            auto image = EdgeImage::New(fileNotExist, pixelSize, expectedCenter);

            double expectedX = expectedCenter.get().x;
            double expectedY = expectedCenter.get().y;

            auto actualCenter = image->GetCentroid();
            double actualX = actualCenter.get().x;
            double actualY = actualCenter.get().y;

            Assert::AreEqual(expectedX, actualX, L"Center coordinates must be the one used in constructor");
            Assert::AreEqual(expectedY, actualY, L"Center coordinates must be the one used in constructor");
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_10_Expect_image_dimensions_in_pixels_to_be_available)
        {
            Logger::WriteMessage("Start\n");
            auto pixelSize = EdgeImage::PixelSize(cv::Point2d(1, 1));
            auto center = EdgeImage::ImageCentroid(cv::Point2d(634, 383));
            const int expectedImageWidth = 200;
            const int expectedImageHeight = 200;
            std::string filepath = TEST_DATA_PATH + std::string("bigWaferShot_X_634_Y_383.png");

            auto image = EdgeImage::New(filepath, pixelSize, center);
            Assert::IsFalse(image->Empty());

            auto imageDimensionsInPixel = image->GetDimensionsInMicrometers();
            Assert::AreEqual((double)expectedImageWidth, imageDimensionsInPixel.x);
            Assert::AreEqual((double)expectedImageHeight, imageDimensionsInPixel.y);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_11_Expect_pixel_size_to_be_used_for_dimension_computation)
        {
            Logger::WriteMessage("Start\n");
            auto pixelSize = EdgeImage::PixelSize(cv::Point2d(2.11, 3.45));
            auto center = EdgeImage::ImageCentroid(cv::Point2d(634, 383));
            const cv::Point imageDimensionsInPixel = cv::Point2d(200, 200);
            std::string filepath = TEST_DATA_PATH + std::string("bigWaferShot_X_634_Y_383.png");

            auto image = EdgeImage::New(filepath, pixelSize, center);
            Assert::IsFalse(image->Empty());

            auto dimensionsInMicrons = image->GetDimensionsInMicrometers();

            Assert::AreEqual(200.0 * pixelSize.get().x, dimensionsInMicrons.x);
            Assert::AreEqual(200.0 * pixelSize.get().y, dimensionsInMicrons.y);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_12_Expect_image_center_relative_to_wafer_center_to_be_available)
        {
            Logger::WriteMessage("Start\n");
            auto pixelSize = EdgeImage::PixelSize(cv::Point2d(1, 1));
            auto center = EdgeImage::ImageCentroid(cv::Point2d(634, 383));
            std::string filepath = TEST_DATA_PATH + std::string("bigWaferShot_X_634_Y_383.png");

            auto image = EdgeImage::New(filepath, pixelSize, center);
            Assert::IsFalse(image->Empty());

            const int expectedX = 634;
            const int expectedY = 383;

            auto actualCenter = image->GetCentroid();
            Assert::AreEqual((double)expectedX, actualCenter.get().x);
            Assert::AreEqual((double)expectedY, actualCenter.get().y);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_13_Expect_opencv_points_to_be_projectible_on_stage_referential)
        {
            Logger::WriteMessage("Start\n");
            std::string currentPath = std::filesystem::current_path().string();
            std::string filepath = TEST_DATA_PATH + std::string("bigWaferShot_X_634_Y_383_Shifted.png");

            auto pixelSize = EdgeImage::PixelSize(cv::Point2d(1, 1));
            auto center = EdgeImage::ImageCentroid(cv::Point2d(634, 383));
            auto image = EdgeImage::New(filepath, pixelSize, center);
            Assert::IsFalse(image->Empty());

            auto actualCenter = image->GetCentroid();

            int waferDiameterInMm = 300;

            int edgeDetectionVersion = 2;
            int cannyThreshold = 200;

            auto edgePoints = image->GetShiftedContourPoints(waferDiameterInMm, edgeDetectionVersion, cannyThreshold, currentPath);

            // expect each point to be at [radius] pixel of center
            // 3 pixels of tolerance represents around 0,15% of error because image is
            // 2000x2000.
            const double expectedDistanceToCenter = 750;
            const double ERROR_PERCENTAGE = 0.7;
            const double TOLERANCE = (ERROR_PERCENTAGE / 100.0) * expectedDistanceToCenter;
            cv::Point2d waferCenter(10, -5);
            int lowerBound = (int) (expectedDistanceToCenter - TOLERANCE);
            int upperBound = (int) (expectedDistanceToCenter + TOLERANCE);
            int countOfPointsInsideSpec = 0;
            auto imageDimensions = image->GetDimensionsInMicrometers();
            for (cv::Point2d& pt : edgePoints) {
                // point must be in the image in the chuck referential
                Assert::IsTrue(pt.x > (actualCenter.get().x - imageDimensions.x / 2));
                Assert::IsTrue(pt.x < (actualCenter.get().x + imageDimensions.x / 2));
                Assert::IsTrue(pt.y > (actualCenter.get().y - imageDimensions.y / 2));
                Assert::IsTrue(pt.y < (actualCenter.get().y + imageDimensions.y / 2));

                auto actualDistanceToCenter = cv::norm(waferCenter - pt);

                if (actualDistanceToCenter > lowerBound && actualDistanceToCenter < upperBound) {
                    countOfPointsInsideSpec++;
                }

                // distance between center and point must be _very_ close to 750px
                Assert::AreEqual(expectedDistanceToCenter, actualDistanceToCenter, TOLERANCE);
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_14_Expect_opencv_points_with_negative_coordinates_to_be_projectible_on_stage_referential)
        {
            Logger::WriteMessage("Start\n");
            std::string currentPath = std::filesystem::current_path().string();

            auto pixelSize = EdgeImage::PixelSize(cv::Point2d(1, 1));
            auto center = EdgeImage::ImageCentroid(cv::Point2d(-609, -450));
            std::string filepath = TEST_DATA_PATH + std::string("bigWaferShot_X_-609_Y_-450.png");

            auto image = EdgeImage::New(filepath, pixelSize, center);
            Assert::IsFalse(image->Empty());

            auto actualCenter = image->GetCentroid();

            auto imageDimensions = image->GetDimensionsInMicrometers();

            int waferDiameterInMm = 300;

            int edgeDetectionVersion = 2;
            int cannyThreshold = 200;

            auto edgePoints = image->GetShiftedContourPoints(waferDiameterInMm, edgeDetectionVersion, cannyThreshold, currentPath);

            // expect each point to be at [radius] pixel of center
            for (cv::Point2d& pt : edgePoints) {

                // point must be in the image in the chuck referrential
                Assert::IsTrue(pt.x > (actualCenter.get().x - imageDimensions.x / 2));
                Assert::IsTrue(pt.x < (actualCenter.get().x + imageDimensions.x / 2));
                Assert::IsTrue(pt.y > (actualCenter.get().y - imageDimensions.y / 2));
                Assert::IsTrue(pt.y < (actualCenter.get().y + imageDimensions.y / 2));

                // distance from center (0,0) to point must be _very_ close to 750px
                auto actualDistanceToCenter = std::sqrt(pt.x * pt.x + pt.y * pt.y);

                const auto expectedDistanceToCenter = 750;
                const double ERROR_PERCENTAGE = 0.7;
                const double TOLERANCE = (ERROR_PERCENTAGE / 100.0) * expectedDistanceToCenter;
                Assert::AreEqual(expectedDistanceToCenter, actualDistanceToCenter, TOLERANCE);
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_15_Expect_bottom_right_image_to_be_classified)
        {
            Logger::WriteMessage("Start\n");
            std::vector<EdgeImage::Pointer> images;
            EdgeImage::PixelSize pixelSize(cv::Point2d(0, 0)); // not used
            EdgeImage::ImageCentroid imageCenter(cv::Point2d(68815, -29790));
            std::string fileNotExist = "notExistFile.png";

            auto image = EdgeImage::New(fileNotExist, pixelSize, imageCenter);

            Assert::AreEqual((int)image->GetPositionClass(), (int)EdgeImage::PositionClass::BOTTOM_RIGHT);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_16_Expect_top_right_image_to_be_classified)
        {
            Logger::WriteMessage("Start\n");
            std::vector<EdgeImage::Pointer> images;
            EdgeImage::PixelSize pixelSize(cv::Point2d(0, 0)); // not used
            EdgeImage::ImageCentroid imageCenter(cv::Point2d(63488, 30768));
            std::string fileNotExist = "notExistFile.png";

            auto image = EdgeImage::New(fileNotExist, pixelSize, imageCenter);

            Assert::AreEqual((int)image->GetPositionClass(), (int)EdgeImage::PositionClass::TOP_RIGHT);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_17_Expect_bottom_left_image_to_be_classified)
        {
            Logger::WriteMessage("Start\n");
            std::vector<EdgeImage::Pointer> images;
            EdgeImage::PixelSize pixelSize(cv::Point2d(0, 0)); // not used
            EdgeImage::ImageCentroid imageCenter(cv::Point2d(-47054, -60502));
            std::string fileNotExist = "notExistFile.png";

            auto image = EdgeImage::New(fileNotExist, pixelSize, imageCenter);

            Assert::AreEqual((int)image->GetPositionClass(), (int)EdgeImage::PositionClass::BOTTOM_LEFT);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_18_Expect_probable_outside_region_intensity_to_have_default_size)
        {
            Logger::WriteMessage("Start\n");
            std::vector<EdgeImage::Pointer> images;
            EdgeImage::PixelSize pixelSize(cv::Point2d(0, 0));       // not used
            EdgeImage::ImageCentroid imageCenter(cv::Point2d(0, 0)); // not used
            std::string filepath = TEST_DATA_PATH + std::string("bigWaferShot_X_-609_Y_-450.png");

            auto image = EdgeImage::New(filepath, pixelSize, imageCenter);

            Assert::IsFalse(image->Empty());

            cv::Rect actualOutsideRegionInImageReferential = image->GetProbableOutsideRegion();

            double expectedRegionOfInterestHeight = 0.10 * image->Mat().rows;
            double expectedRegionOfInterestWidth = 0.10 * image->Mat().cols;

            Assert::AreEqual(expectedRegionOfInterestHeight, actualOutsideRegionInImageReferential.height, 10e-1);
            Assert::AreEqual(expectedRegionOfInterestWidth, actualOutsideRegionInImageReferential.width, 10e-1);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_19_Expect_right_min_and_max_radius)
        {
            // NOTE: when computing distances, we measure from a pixel center to another pixel center. So we have
            //             to deal with a pixelSize/2 at some point.
            // NOTE 2: the image centroid is not in a pixel center, since image width is odd

            Logger::WriteMessage("Start\n");
            struct GivenTypeForRadius : GivenType {
                cv::Point2d waferCenter;
                EdgeImage::MinAndMax expectedMinAndMax;
            };

            // given
            std::vector<GivenTypeForRadius> givens;
            givens.emplace_back(GivenTypeForRadius{
                "Image aligned with 0,0 on X axis", cv::Size(4, 4), cv::Point(1, 1), cv::Point(0, -5), cv::Point2d(0, 0), {3.5, 6.6708320320631671} });
            givens.emplace_back(GivenTypeForRadius{ "Image shifted right of X axis",
                                                   cv::Size(6, 4),
                                                   cv::Point(1, 1),
                                                   cv::Point(4, -12),
                                                   cv::Point2d(0, 0),
                                                   {10.606601717798213, 14.7732867026941585} });
            givens.emplace_back(GivenTypeForRadius{ "Metrological wafer case",
                                                   cv::Size(1164, 872),
                                                   cv::Point2d(5.3, 5.3),
                                                   cv::Point(0, -150000),
                                                   cv::Point2d(588.3, 1303.8),
                                                   {148995.65, 153655.79038183036} });
            wchar_t wbuf[255];
            for (auto const& given : givens) {

                // when
                EdgeImage::Pointer image = CreateImageFromGiven(given);
                EdgeImage::MinAndMax actualMinAndMax = image->FindOptimalMinAndMaxRadius(given.waferCenter);

                // then
#pragma warning(suppress : 4996)
                std::wstring wlabel = strconverter.from_bytes(given.label);
                _snwprintf_s(wbuf, 255, L"Failed for <%s> radius computation of case: <%s>", L"min", wlabel.c_str());
                Assert::AreEqual(given.expectedMinAndMax.min, actualMinAndMax.min, wbuf);
                _snwprintf_s(wbuf, 255, L"Failed for <%s> radius computation of case: <%s>", L"max", wlabel.c_str());
                Assert::AreEqual(given.expectedMinAndMax.max, actualMinAndMax.max, wbuf);
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_20_Expect_right_min_and_max_angles)
        {
            Logger::WriteMessage("Start\n");
            struct GivenTypeForAngles : GivenType {
                cv::Point2d waferCenter;
                EdgeImage::MinAndMax expectedMinAndMax;
            };

            std::vector<GivenTypeForAngles> givens;

            givens.emplace_back(
                GivenTypeForAngles{ "Simple case", cv::Size(6, 6), cv::Point2d(1.0, 1.0), cv::Point(0, (int)-5.5), cv::Point2d(0.0, 0.0), {5.0 * CV_PI / 4.0, 7.0 * CV_PI / 4.0} });

            givens.emplace_back(GivenTypeForAngles{ "Metrological wafer case",
                                                   cv::Size(1164, 872),
                                                   cv::Point2d(5.3, 5.3),
                                                   cv::Point(0, -150000),
                                                   cv::Point2d(588.3, 1303.8),
                                                   {4.687760692, 4.7291238126404886} });
            wchar_t wbuf[255];
            for (auto const& given : givens) {

                // when
                EdgeImage::Pointer image = CreateImageFromGiven(given);
                EdgeImage::MinAndMax actualMinAndMax = image->FindOptimalMinAndMaxAngles(given.waferCenter);

                // then
#pragma warning(suppress : 4996)
                std::wstring wlabel = strconverter.from_bytes(given.label);
                _snwprintf_s(wbuf, 255, L"Failed for <%s> angle computation of case: <%s>", L"min", wlabel.c_str());
                Assert::AreEqual(given.expectedMinAndMax.min, actualMinAndMax.min, 10e-4, wbuf);
                _snwprintf_s(wbuf, 255, L"Failed for <%s> angle computation of case: <%s>", L"max", wlabel.c_str());
                Assert::AreEqual(given.expectedMinAndMax.max, actualMinAndMax.max, 10e-4, wbuf);
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_21_Expect_image_with_dark_wafer_to_be_seen_as_dark)
        {
            cv::Mat artificialDarkWaferImage = cv::Mat::zeros(100, 100, CV_8UC1);
            int width = artificialDarkWaferImage.cols;
            int height = artificialDarkWaferImage.cols;
            cv::rectangle(artificialDarkWaferImage, cv::Rect(width/2, 0, width/2, height), cv::Scalar(255, 255, 255), -1); //drawing the bright chuck section on the right (the wafer is dark on the left)

            EdgeImage::PixelSize pixelSize(cv::Point2d(0.0, 0.0)); //not used (only for initialization)
            EdgeImage::ImageCentroid centroid(cv::Point2d(10.0, 0.0)); // position : on the right so that the wafer border is on the left
            EdgeImage::Pointer image = EdgeImage::New(artificialDarkWaferImage, pixelSize, centroid);

            Assert::IsTrue(image->IsWaferDark());
        }

        TEST_METHOD(_22_Expect_image_with_bright_wafer_to_be_seen_as_bright)
        {
            cv::Mat artificialBrightWaferImage = cv::Mat::zeros(100, 100, CV_8UC1);
            int width = artificialBrightWaferImage.cols;
            int height = artificialBrightWaferImage.cols;
            cv::rectangle(artificialBrightWaferImage, cv::Rect(0, 0, width / 2, height), cv::Scalar(255, 255, 255), -1); //drawing the bright wafer section on the left (the wafer is bright on the left)

            EdgeImage::PixelSize pixelSize(cv::Point2d(0.0, 0.0)); //not used (only for initialization)
            EdgeImage::ImageCentroid centroid(cv::Point2d(10.0, 0.0)); // position : on the right so that the wafer border is on the left
            EdgeImage::Pointer image = EdgeImage::New(artificialBrightWaferImage, pixelSize, centroid);

            Assert::IsFalse(image->IsWaferDark());
        }

	};
}
