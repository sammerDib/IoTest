#include "CppUnitTest.h"

#include "PolarImageCropper.hpp"
#pragma unmanaged

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

/*namespace cv {
    bool operator==(const Rect& left, const Rect& right) {
        return left.width == right.width && left.height == right.height && left.x == right.x && left.y == right.y;
    }
    bool operator==(const Size& left, const Size& right) { return left.width == right.width && left.height == right.height; }

} // namespace cv*/

namespace {

    struct GivenType {
        std::wstring label;
        cv::Size originalSize;
        cv::Size expectedSizeAfterCrop;
        cv::Rect expectedCropDimensions;
    };

    // construct image according to GivenType data
    cv::Mat CreateImageUsingGivenInformation(GivenType given) {
        cv::Mat image;
        if (given.originalSize == given.expectedSizeAfterCrop) {
            image = cv::Mat::ones(given.originalSize, CV_8U);
        }
        else if (given.expectedSizeAfterCrop == cv::Size(0, 0)) {
            image = cv::Mat::zeros(given.originalSize, CV_8U);
        }
        else {
            image = cv::Mat::zeros(given.originalSize, CV_8U);
            cv::Mat cropRoi = image(given.expectedCropDimensions);
            cropRoi = cv::Scalar(255);
        }
        return image;
    }

} // namespace

namespace SharedOpenCVNativeTests
{
	TEST_CLASS(PolarImageCropperTests)
	{
    private :
       
        double ProjectCorrelationOrdinateOnPolarImage(double point, cv::Rect polarCroppedPadded, cv::Rect cropRegion) {
            double result = -1;

            double minimalProjectablePointOrdinate = cropRegion.height / 2;
            if (point < minimalProjectablePointOrdinate) {
                throw std::exception("Correlation value is under acceptable value");
            }

            double maximalProjectablePointOrdinate = cropRegion.height * 3 / 2;
            if (point > maximalProjectablePointOrdinate) {
                throw std::exception("Correlation value is over acceptable value");
            }
            double minimalPolarImageOrdinate = static_cast<double>(cropRegion.y);
            double maximalPolarImageOrdinate = static_cast<double>(cropRegion.y) + static_cast<double>(cropRegion.height);

            auto Map = [](double value, double fromLow, double fromHigh, double toLow, double toHigh) {
                return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
            };

            result = Map(point, minimalProjectablePointOrdinate, maximalProjectablePointOrdinate, minimalPolarImageOrdinate, maximalPolarImageOrdinate);

            return result;
        }

	public:
        double _givenpoint;
        cv::Rect _polarCroppedPadded;
        cv::Rect _cropRegion;

        double CallProjectCorrelationOrdinateOnPolarImage() { return ProjectCorrelationOrdinateOnPolarImage(_givenpoint, _polarCroppedPadded, _cropRegion); }

		TEST_METHOD(Expect_non_complete_rows_and_columns_to_be_cropped)
		{
            Logger::WriteMessage("Start\n");
            std::vector<GivenType> givens;
            int waferDiameterInMm = 300;

            givens.push_back(GivenType({ L"All black image returns totally cropped", cv::Size(10, 10), cv::Size(0, 0), cv::Rect(0, 0, 10, 10) }));
            givens.push_back(GivenType({ L"All white image returns unchanged", cv::Size(10, 10), cv::Size(10, 10), cv::Rect(0, 0, 0, 0) }));
            givens.push_back(GivenType({ L"First row should be cropped", cv::Size(10, 10), cv::Size(10, 9), cv::Rect(0, 1, 10, 9) }));
            givens.push_back(GivenType({ L"Two first rows should be cropped", cv::Size(100, 100), cv::Size(100, 98), cv::Rect(0, 2, 100, 98) }));
            givens.push_back(
                GivenType({ L"Two first rows and two first columns should be cropped", cv::Size(100, 100), cv::Size(98, 98), cv::Rect(2, 2, 98, 98) }));
            givens.push_back(
                GivenType({ L"A border of 1 elements should be cropped on each side", cv::Size(100, 100), cv::Size(98, 98), cv::Rect(1, 1, 98, 98) }));
            givens.push_back(GivenType(
                { L"A border of 10 elements should be cropped on each side (beyond 8U)", cv::Size(1000, 1000), cv::Size(980, 980), cv::Rect(10, 10, 980, 980) }));
            givens.push_back(GivenType({ L"A border of 2 elements should be cropped on each side (not square image)", cv::Size(1000, 800), cv::Size(998, 798),
                                        cv::Rect(2, 2, 998, 798) }));

            wchar_t wbuf[255];
            for (auto const& given : givens) {
                cv::Rect cropDimensions;
                cv::Mat image = CreateImageUsingGivenInformation(given);

                cv::Mat cropped = PolarImageCropper::Crop(image, waferDiameterInMm, &cropDimensions);
                _snwprintf_s(wbuf, 255, L"Fail <%s> for case = <%s>\n", L"Width", given.label.c_str());
                Assert::AreEqual(given.expectedSizeAfterCrop.width, cropped.size().width, wbuf);

                _snwprintf_s(wbuf, 255, L"Fail <%s> for case = <%s>\n", L"Heigth", given.label.c_str());
                Assert::AreEqual(given.expectedSizeAfterCrop.height, cropped.size().height, wbuf);

                _snwprintf_s(wbuf, 255, L"Fail <%s> for case = <%s>\n", L"crop dimensions W ", given.label.c_str());
                 Assert::AreEqual(given.expectedCropDimensions.width, cropDimensions.width, wbuf);
                 _snwprintf_s(wbuf, 255, L"Fail <%s> for case = <%s>\n", L"crop dimensions H ", given.label.c_str());
                 Assert::AreEqual(given.expectedCropDimensions.height, cropDimensions.height, wbuf);
                 _snwprintf_s(wbuf, 255, L"Fail <%s> for case = <%s>\n", L"crop dimensions x ", given.label.c_str());
                 Assert::AreEqual(given.expectedCropDimensions.x, cropDimensions.x, wbuf);
                 _snwprintf_s(wbuf, 255, L"Fail <%s> for case = <%s>\n", L"crop dimensions y ", given.label.c_str());
                 Assert::AreEqual(given.expectedCropDimensions.y, cropDimensions.y, wbuf);
            }
            Logger::WriteMessage("Done\n");
		}

        TEST_METHOD(Expect_correlation_value_on_cropped_padded_polar_image_to_be_transferrable_on_source_polar)
        {
            Logger::WriteMessage("Start\n");
            // The case:
            // a 1000x1000 polar image, with 50 top and 100 bottom lines cropped
            // a 1000x850 cropped polar image
            // the padded image adds half the cropped image on top and at bottom of polar image

            // The phaseCorrelation on the croppedPaddedImage should return a value
            // between 425 and 1275.
            cv::Rect rawPolar(0, 0, 1000, 1000);
            cv::Rect cropRegion(0, 50, 1000, 850);
            cv::Rect croppedPolar(0, 0, 1000, 850);
            cv::Rect polarCroppedPadded(0, 0, 1000, croppedPolar.height * 2);

            struct GivenType {
                std::wstring label;
                double givenPoint;
                cv::Rect givenPolarCroppedImageDimensions;
                cv::Rect givenCropRegionDimensions;
                double expectedPoint;
                bool expectException;
            };

            std::vector<GivenType> givens;

            givens.push_back(GivenType({ L"value under minimal accepted should throw", 400, polarCroppedPadded, cropRegion, -1, true }));
            givens.push_back(GivenType({ L"value over maximal accepted should throw", 1300, polarCroppedPadded, cropRegion, -1, true }));
            givens.push_back(GivenType({ L"value at minimal accepted should returns", 425, polarCroppedPadded, cropRegion, 50, false }));
            givens.push_back(GivenType({ L"value at maximal accepted should returns", 1275, polarCroppedPadded, cropRegion, 900, false }));

            wchar_t wbuf[255];
            for (auto const& given : givens) {

                if (given.expectException) {

                    _snwprintf_s(wbuf, 255, L"Fail in case = <%s>\n", given.label.c_str());
                    _givenpoint = given.givenPoint;
                    _polarCroppedPadded = polarCroppedPadded;
                    _cropRegion = cropRegion;
                    auto funcAnalysis = [this] { CallProjectCorrelationOrdinateOnPolarImage();};
                    Assert::ExpectException<std::exception>(funcAnalysis, wbuf);
                }
                else {
                    double actualPoint = ProjectCorrelationOrdinateOnPolarImage(given.givenPoint, polarCroppedPadded, cropRegion);
                    _snwprintf_s(wbuf, 255, L"Fail in case = <%s>\n", given.label.c_str());
                    Assert::AreEqual(given.expectedPoint, actualPoint, wbuf );
                }
            }
            Logger::WriteMessage("Done\n");
        }
	};
}
