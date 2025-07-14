#include "CppUnitTest.h"

#include "CImageOperators.hpp"
#include "CUtils.hpp"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace img_operators;

#include <filesystem>
using std::string;
using std::filesystem::current_path;

#define TEST_DATA_PATH std::string(".\\..\\..\\Tests\\Data\\")

namespace SharedOpenCVNativeTests
{
    TEST_CLASS(CImageOperators)
    {
    private: 
        cv::Mat applyVignette(const cv::Mat& img, double vignetteStrength)
        {
            cv::Mat grayImg;
            cv::cvtColor(img, grayImg, cv::COLOR_BGR2GRAY);
            if (vignetteStrength == 0) {
                return grayImg;
            }
            int strengthCols = (int) ((1.0 - vignetteStrength) * grayImg.cols);
            int strengthRows = (int) ((1.0 - vignetteStrength) * grayImg.rows);

            cv::Mat kernelX = cv::getGaussianKernel(grayImg.cols, strengthCols);
            cv::Mat kernelY = cv::getGaussianKernel(grayImg.rows, strengthRows);
            cv::Mat kernelXtransposed;
            cv::transpose(kernelX, kernelXtransposed);
            cv::Mat kernel = kernelY * kernelXtransposed;

            cv::Mat mask, vignetteImg;
            cv::normalize(kernel, mask, 0, 1, cv::NORM_MINMAX);
            grayImg.convertTo(vignetteImg, CV_64F);
            cv::multiply(mask, vignetteImg, vignetteImg);
            cv::convertScaleAbs(vignetteImg, vignetteImg);

            return vignetteImg;
        }

        cv::Mat generateImage(int size, int circleSize, int period) {
            cv::Mat image(size, size, CV_8UC3, cv::Scalar(255, 0, 0));

            int offset = circleSize + (period - 2 * circleSize) / 2;

            for (int i = offset; i < size; i += period) {
                for (int j = offset; j < size; j += period) {
                    cv::circle(image, cv::Point(i, j), circleSize, cv::Scalar(0, 255, 255), -1);
                }
            }

            return image;

        }
    public:

        TEST_METHOD(stddev_of_constant_image)
        {
            cv::Size imageSize = cv::Size(3, 3);
            //Constant image with all pixels set to 255
            cv::Mat constantImage = cv::Mat(imageSize, CV_8UC1, cv::Scalar(255));
            double stdDev = img_operators::StandardDeviation(constantImage);
            Assert::AreEqual(0.0, stdDev);
        }

        TEST_METHOD(stddev_of_image_with_variation_gives_expected_result)
        {
            cv::Size imageSize = cv::Size(2, 1);
            //1 pixel set to 255 and 1 pixel set to 0
            cv::Mat constantImage = cv::Mat(imageSize, CV_8UC1, cv::Scalar(255));
            constantImage.at<uchar>(0, 0) = 0;
            double stdDev = img_operators::StandardDeviation(constantImage);
            Assert::AreEqual(127.5, stdDev);
        }

        TEST_METHOD(median_of_white_image)
        {
            int imageSize = 400;
            cv::Mat whiteImg = cv::Mat::ones(imageSize, imageSize, CV_8U) * 255;

            Logger::WriteMessage("Perform test");
            double median_value = img_operators::GrayscaleMedian(whiteImg);
            Assert::AreEqual(255.0, median_value);
            Logger::WriteMessage("Done \n");
        }

        TEST_METHOD(median_of_black_image_16bit)
        {
            int imageSize = 400;
            cv::Mat blackImg = cv::Mat::zeros(imageSize, imageSize, CV_16U);

            Logger::WriteMessage("Perform test");
            double median_value = img_operators::GrayscaleMedian(blackImg);
            Assert::AreEqual(0.0, median_value);
            Logger::WriteMessage("Done \n");
        }

        TEST_METHOD(median_of_gray_image_with_noise)
        {
            int imageSize = 400;
            cv::Mat grayImg = cv::Mat::ones(imageSize, imageSize, CV_8U) * 200;

            // Add noise: for each pixel there's a 10% chance that it's 0
            int numNoisePixels = static_cast<int>(0.1 * imageSize * imageSize);
            for (int i = 0; i < numNoisePixels; ++i) {
                int x = rand() % imageSize;
                int y = rand() % imageSize;
                grayImg.at<uchar>(y, x) = 0;
            }

            Logger::WriteMessage("Perform test");
            double median_value = img_operators::GrayscaleMedian(grayImg);
            Assert::AreEqual(200.0, median_value);
            Logger::WriteMessage("Done \n");
        }
        

        TEST_METHOD(detection_of_vignetting_on_an_image_without_vignetting)
        {
            int imageSize = 410;
            int circleSize = 15;
            int period = 50;

            cv::Mat generatedImg = generateImage(imageSize, circleSize, period);

            cv::Point vignetteCenter(100, 100);
            double vignetteStrength = 0;

            cv::Mat vignettedImage = applyVignette(generatedImg, vignetteStrength);

            Logger::WriteMessage("Perform test");
            double vignetting_value = img_operators::standartDeviationZoneByZone(vignettedImage, 100);
            Assert::IsTrue(vignetting_value == 0.0);
            Logger::WriteMessage("Done \n");

        }


        TEST_METHOD(detection_of_vignetting_on_an_image_with_vignetting)
        {
            int imageSize = 410;
            int circleSize = 15;
            int period = 50;

            cv::Mat generatedImg = generateImage(imageSize, circleSize, period);

            cv::Point vignetteCenter(100, 100);
            double vignetteStrength = 0.5;

            cv::Mat vignettedImage = applyVignette(generatedImg, vignetteStrength);

            Logger::WriteMessage("Perform test");
            double vignetting_value = img_operators::standartDeviationZoneByZone(vignettedImage, 100);
            Assert::IsTrue(vignetting_value > 0.0);
            Logger::WriteMessage("Done \n");

        }
       
        TEST_METHOD(normalized_gray_level_variance_decreases_when_the_image_is_overexposed_or_underexposed)
        {
            // Given: images good exposed, overexposed and underexposed
            auto path1 = current_path();
            auto stpth = path1.generic_string();
            Logger::WriteMessage("CURRENT DIR = ");
            Logger::WriteMessage(stpth.c_str());
            Logger::WriteMessage("\n");

            Logger::WriteMessage("Load Image Matrices from Tests\\Data \n");
            cv::Mat img = cv::imread(TEST_DATA_PATH + std::string("notch.png"));
            Assert::IsTrue(img.cols != 0, L"Image dimension X should not be == 0, check Tests data files path");
            Assert::IsTrue(img.rows != 0, L"Image dimension Y should not be == 0, check Tests data files path");
            cv::Mat img_too_light = cv::imread(TEST_DATA_PATH + std::string("notch_too_light.png"));
            cv::Mat img_too_dark = cv::imread(TEST_DATA_PATH + std::string("notch_too_dark.png"));
            // When: compute contrast with normalized gray level variance
            Logger::WriteMessage("perform good exposed \n");
            double good_exposed = img_operators::NormalizedVariance(img);
            Logger::WriteMessage("perform over exposed \n");
            double over_exposed = img_operators::NormalizedVariance(img_too_light);
            Logger::WriteMessage("perform under_exposed \n");
            double under_exposed = img_operators::NormalizedVariance(img_too_dark);
            // Then: contrast score measured decrease when the image is overexposed or underexposed
            Assert::IsTrue(good_exposed > over_exposed && good_exposed > under_exposed);

            Logger::WriteMessage("Done \n");
        }

        TEST_METHOD(focus_measurement_decreases_as_image_blur_increases)
        {
            Logger::WriteMessage("Load Image from Tests\\Data \n");
            cv::Mat ref_img = cv::imread(TEST_DATA_PATH + std::string("notch.png"));
            // Given: an image more and more blured
            std::vector<cv::Mat> blured_imgs;
            int blur_coef = 0;
            Logger::WriteMessage("perform GaussianBlur\n");
            std::vector<int> odd_kernel_size = { 1, 3, 5, 7, 9 };
            for (int kernel_size : odd_kernel_size) {
                cv::Mat blured_img;
                cv::GaussianBlur(ref_img, blured_img, cv::Size(kernel_size, kernel_size), 0);
                blured_imgs.push_back(blured_img);
            }

            // When : calculate the focus on the images, from the least blurred to the most blurred, with all algorithms
            std::vector<double> focus_scores_with_LAPV;
            std::vector<double> focus_scores_with_LAPM;
            std::vector<double> focus_scores_with_TENG;
            std::vector<double> focus_scores_with_VF4;
            std::vector<double> focus_scores_with_NVAR;
            Logger::WriteMessage("perform img_operators \n");
            for (int i = 0; i < blured_imgs.size(); i++) {
                focus_scores_with_LAPV.push_back(img_operators::VarianceOfLaplacian(blured_imgs.at(i)));
                focus_scores_with_LAPM.push_back(img_operators::SumOfModifiedLaplacian(blured_imgs.at(i)));
                focus_scores_with_TENG.push_back(img_operators::TenenbaumGradient(blured_imgs.at(i)));
                focus_scores_with_VF4.push_back(img_operators::VollathF4(blured_imgs.at(i)));
                focus_scores_with_NVAR.push_back(img_operators::NormalizedVariance(blured_imgs.at(i)));
            }

            // Then : focus measurement decreases as image blur increases

            // LAPV algorithm
            Logger::WriteMessage("perform Variance of Laplacian algorithm \n");
            double previous_score = std::numeric_limits<double>::infinity();
            for (int i = 0; i < focus_scores_with_LAPV.size(); i++) {
                std::cout << focus_scores_with_LAPV.at(i) << std::endl;
                Assert::IsTrue(previous_score > focus_scores_with_LAPV.at(i));
                previous_score = focus_scores_with_LAPV.at(i);
            }

            // LAPM algorithm
            Logger::WriteMessage("perform Modified Laplacian algorithm \n");
            previous_score = std::numeric_limits<double>::infinity();
            for (int i = 0; i < focus_scores_with_LAPM.size(); i++) {
                std::cout << focus_scores_with_LAPM.at(i) << std::endl;
                Assert::IsTrue(previous_score > focus_scores_with_LAPM.at(i));
                previous_score = focus_scores_with_LAPM.at(i);
            }

            // TENG algorithm
            Logger::WriteMessage("perform Tenengrad algorithm \n");
            previous_score = std::numeric_limits<double>::infinity();
            for (int i = 0; i < focus_scores_with_TENG.size(); i++) {
                std::cout << focus_scores_with_TENG.at(i) << std::endl;
                Assert::IsTrue(previous_score > focus_scores_with_TENG.at(i));
                previous_score = focus_scores_with_TENG.at(i);
            }

            // VF4 algorithm
            Logger::WriteMessage("perform Vollath F4 algorithm \n");
            previous_score = std::numeric_limits<double>::infinity();
            for (int i = 0; i < focus_scores_with_VF4.size(); i++) {
                std::cout << focus_scores_with_VF4.at(i) << std::endl;
                Assert::IsTrue(previous_score > focus_scores_with_VF4.at(i));
                previous_score = focus_scores_with_VF4.at(i);
            }

            // NVAR algorithm
            Logger::WriteMessage("perform Normalized Variance algorithm \n");
            previous_score = std::numeric_limits<double>::infinity();
            for (int i = 0; i < focus_scores_with_NVAR.size(); i++) {
                std::cout << focus_scores_with_NVAR.at(i) << std::endl;
                Assert::IsTrue(previous_score > focus_scores_with_NVAR.at(i));
                previous_score = focus_scores_with_NVAR.at(i);
            }

            Logger::WriteMessage("Done \n");
        }

        TEST_METHOD(saturation_increases_from_0_to_1_when_lightness_increases)
        {
            // Given: completely black, intermediate and completely white images
            std::vector<cv::Mat> imgs;
            cv::Mat black_img(10, 10, CV_8UC3, cv::Scalar(0, 0, 0));
            Logger::WriteMessage("Load Image from Tests\\Data \n");
            cv::Mat intermediate_img = cv::imread(TEST_DATA_PATH + std::string("notch.png"));
            cv::Mat white_img(10, 10, CV_8UC3, cv::Scalar(250, 250, 250));

            // When : calculate the saturation on the images, from the least lighted to the most lighted

            Logger::WriteMessage("perform score_on_black_image\n");
            double score_on_black_image = img_operators::Saturation(black_img);
            Logger::WriteMessage("perform score_on_intermediate_image\n");
            double score_on_intermediate_image = img_operators::Saturation(intermediate_img);
            Logger::WriteMessage("perform score_on_white_image\n");
            double score_on_white_image = img_operators::Saturation(white_img);

            Logger::WriteMessage("assertion\n");

            // Then : saturation measurement increases from 0 (completely black image) to 1 (completely white image)
            Assert::IsTrue(score_on_black_image == 0);
            Assert::IsTrue(score_on_black_image < score_on_intermediate_image);
            Assert::IsTrue(score_on_intermediate_image < score_on_white_image);
            Assert::IsTrue(score_on_white_image == 1);

            Logger::WriteMessage("Done \n");
        }

        TEST_METHOD(extract_intensity_pixel_along_a_diagonal_line_from_3x3_matrix)
        {
            // Given: completely black image
            cv::Mat img(3, 3, CV_8UC1, cv::Scalar(42));

            // When
            std::vector<cv::Point2d> profile = img_operators::ExtractIntensityProfile(img, cv::Point2i(0, 0), cv::Point2i(2, 2));

            // Then
            Assert::AreEqual(3, (int)profile.size());
            Assert::AreEqual(0., profile.at(0).x);
            Assert::AreEqual(42., profile.at(0).y);
            Assert::AreEqual(1., profile.at(1).x);
            Assert::AreEqual(42., profile.at(1).y);
            Assert::AreEqual(2., profile.at(2).x);
            Assert::AreEqual(42., profile.at(2).y);
        }

        TEST_METHOD(extract_intensity_pixel_along_a_non_diagonal_line_from_5x5_matrix)
        {
            // Given: completely black image
            cv::Mat img(12, 12, CV_8UC1, cv::Scalar(42));

            // When
            std::vector<cv::Point2d> profile = img_operators::ExtractIntensityProfile(img, cv::Point2i(1, 1), cv::Point2i(11, 5));

            // Then
            Assert::AreEqual(11, (int)profile.size());
            for (size_t i = 0; i < profile.size(); i++)
            {
                Assert::AreEqual(42., profile.at(i).y);
            }
        }

        TEST_METHOD(extract_intensity_pixel_on_empty_line_returns_no_points)
        {
            // Given
            cv::Mat img(12, 12, CV_8UC1, cv::Scalar(42));

            // When
            std::vector<cv::Point2d> profile = img_operators::ExtractIntensityProfile(img, cv::Point2i(1, 1), cv::Point2i(1, 1));

            // Then
            Assert::AreEqual(0, (int)profile.size());
        }

        TEST_METHOD(extract_intensity_pixel_with_end_pixel_outside_of_matrix_does_extract_outside_pixel)
        {
            // Given
            cv::Mat img(3, 3, CV_8UC1, cv::Scalar(42));

            // When
            std::vector<cv::Point2d> profile = img_operators::ExtractIntensityProfile(img, cv::Point2i(0, 0), cv::Point2i(6, 1));

            // Then
            Assert::AreEqual(3, (int)profile.size());
            Assert::AreEqual(0., profile.at(0).x);
            Assert::AreEqual(42., profile.at(0).y);
            Assert::AreEqual(1., profile.at(1).x);
            Assert::AreEqual(42., profile.at(1).y);
            Assert::AreEqual(2., profile.at(2).x);
            Assert::AreEqual(42., profile.at(2).y);
        }

        TEST_METHOD(extract_intensity_pixel_with_both_pixel_outside_of_matrix_returns_empty_profile)
        {
            // Given
            cv::Mat img(3, 3, CV_8UC1, cv::Scalar(42));

            // When
            std::vector<cv::Point2d> profile = img_operators::ExtractIntensityProfile(img, cv::Point2i(4, 4), cv::Point2i(8, 5));

            // Then
            Assert::AreEqual(0, (int)profile.size());
        }

        TEST_METHOD(resize_image_with_full_roi_and_no_rescale_should_return_same_image)
        {
            // Given
            cv::Mat img(10, 10, CV_8UC3, cv::Scalar(42, 42, 42));
            cv::Rect roi(0, 0, 10, 10);

            // When
            cv::Mat resizedImage = img_operators::Resize(img, roi, 1.0);

            // Then
            Assert::AreEqual(img.rows, resizedImage.rows);
            Assert::AreEqual(img.cols, resizedImage.cols);
        }

        TEST_METHOD(resize_image_with_smaller_roi_should_give_image_with_same_size_as_roi)
        {
            // Given
            cv::Mat img(10, 10, CV_8UC3, cv::Scalar(42, 42, 42));
            cv::Rect roi(3, 2, 5, 3);

            // When
            cv::Mat resizedImage = img_operators::Resize(img, roi, 1.0);

            // Then
            Assert::AreEqual(roi.height, resizedImage.rows);
            Assert::AreEqual(roi.width, resizedImage.cols);
        }

        TEST_METHOD(resize_image_with_half_scale_should_give_image_with_half_size)
        {
            // Given
            uchar pixels[4][4] = { {1, 1, 2, 2}, {1, 1, 2, 2}, {3, 3, 4, 4}, {3, 3, 4, 4} };
            cv::Mat img(4, 4, CV_8UC1, pixels);
            cv::Rect roi(0, 0, 4, 4);
            double scale = 0.5;

            // When
            cv::Mat resizedImage = img_operators::Resize(img, roi, scale);

            // Then
            Assert::AreEqual(2, resizedImage.rows);
            Assert::AreEqual(2, resizedImage.cols);
            Assert::AreEqual((uchar)1, resizedImage.at<uchar>(0, 0));
            Assert::AreEqual((uchar)2, resizedImage.at<uchar>(0, 1));
            Assert::AreEqual((uchar)3, resizedImage.at<uchar>(1, 0));
            Assert::AreEqual((uchar)4, resizedImage.at<uchar>(1, 1));
        }

        TEST_METHOD(resized_image_with_roi_outside_of_range)
        {
            // Given
            cv::Mat img(10, 10, CV_8UC3, cv::Scalar(42, 42, 42));
            cv::Rect roi(-2, -1, 5, 3);

            // When
            cv::Mat resizedImage = img_operators::Resize(img, roi, 1.0);

            // Then
            Assert::AreEqual(2, resizedImage.rows);
            Assert::AreEqual(3, resizedImage.cols);
        }

        TEST_METHOD(resized_image_with_empty_roi)
        {
            // Given
            cv::Mat img(10, 10, CV_8UC3, cv::Scalar(42, 42, 42));
            cv::Rect roi(0, 0, 0, 0);

            // When
            cv::Mat resizedImage = img_operators::Resize(img, roi, 1.0);

            // Then
            Assert::AreEqual(10, resizedImage.rows);
            Assert::AreEqual(10, resizedImage.cols);
        }

        TEST_METHOD(normalized_pixels_outside_of_min_and_max_are_saturated)
        {
            uchar pixels[1][3] = { {10, 100, 200} };
            cv::Mat img(1, 3, CV_8UC1, pixels);
            int min = 50;
            int max = 150;
            cv::Mat normalizedImage = img_operators::SaturatedNormalization(img, min, max);
            Assert::AreEqual((uchar)0, normalizedImage.at<uchar>(0, 0));
            Assert::AreEqual((uchar)255, normalizedImage.at<uchar>(0, 2));
        }

        TEST_METHOD(normalized_pixels_dont_change_if_the_original_values_are_already_between_min_and_max)
        {
            uchar pixels[1][3] = { {10, 100, 200} };
            cv::Mat img(1, 3, CV_8UC1, pixels);
            int min = 0;
            int max = 250;
            cv::Mat normalizedImage = img_operators::SaturatedNormalization(img, min, max);
            Assert::AreEqual(img.at<uchar>(0, 0), normalizedImage.at<uchar>(0, 0));
            Assert::AreEqual(img.at<uchar>(0, 1), normalizedImage.at<uchar>(0, 1));
            Assert::AreEqual(img.at<uchar>(0, 2), normalizedImage.at<uchar>(0, 2));
        }

        TEST_METHOD(find_pixel_coordinates_of_given_range_value_on_grayscale_8bits_image)
        {
            // Given: images
            cv::Mat img = cv::Mat::zeros(cv::Size(10, 10), CV_8UC1);
            img.at<uchar>(0, 0) = 1;
            img.at<uchar>(0, 2) = 251;
            img.at<uchar>(0, 1) = 252;
            img.at<uchar>(0, 5) = 253;
            img.at<uchar>(6, 5) = 254;
            img.at<uchar>(2, 4) = 255;

            // When:
            std::vector<cv::Point> coordinates = img_operators::FindPixelCoordinates(img, ThresholdType::InsideRange, 252, 254);

            // Then:
            Assert::AreEqual(3, (int)coordinates.size());
            cv::Point firstCoordinate = coordinates.at(0);
            cv::Point secondCoordinate = coordinates.at(1);
            cv::Point thirdCoordinate = coordinates.at(2);
            Assert::AreEqual(1, firstCoordinate.x);
            Assert::AreEqual(0, firstCoordinate.y);
            Assert::AreEqual(5, secondCoordinate.x);
            Assert::AreEqual(0, secondCoordinate.y);
            Assert::AreEqual(5, thirdCoordinate.x);
            Assert::AreEqual(6, thirdCoordinate.y);
        }

        TEST_METHOD(find_pixel_coordinates_of_given_range_value_on_grayscale_32bits_image)
        {
            // Given: images
            cv::Mat img = cv::Mat::zeros(cv::Size(10, 10), CV_32FC1);
            img.at<float>(0, 0) = 1.f;
            img.at<float>(0, 2) = 0.251f;
            img.at<float>(0, 1) = 0.252f;
            img.at<float>(0, 5) = 0.253f;
            img.at<float>(6, 5) = 0.254f;
            img.at<float>(2, 4) = 0.255f;

            // When:
            std::vector<cv::Point> coordinates = img_operators::FindPixelCoordinates(img, ThresholdType::InsideRange, 0.252f, 0.254f);

            // Then:
            Assert::AreEqual(3, (int)coordinates.size());
            cv::Point firstCoordinate = coordinates.at(0);
            cv::Point secondCoordinate = coordinates.at(1);
            cv::Point thirdCoordinate = coordinates.at(2);
            Assert::AreEqual(1, firstCoordinate.x);
            Assert::AreEqual(0, firstCoordinate.y);
            Assert::AreEqual(5, secondCoordinate.x);
            Assert::AreEqual(0, secondCoordinate.y);
            Assert::AreEqual(5, thirdCoordinate.x);
            Assert::AreEqual(6, thirdCoordinate.y);
        }

        TEST_METHOD(find_pixel_coordinates_of_given_value_on_grayscale_8bits_image)
        {
            // Given: images
            cv::Mat img = cv::Mat::zeros(cv::Size(10, 10), CV_8UC1);
            img.at<uchar>(0, 0) = 1;
            img.at<uchar>(0, 2) = 251;
            img.at<uchar>(0, 1) = 252;
            img.at<uchar>(0, 5) = 253;
            img.at<uchar>(6, 5) = 255;
            img.at<uchar>(2, 4) = 255;

            // When:
            std::vector<cv::Point> coordinates = img_operators::FindPixelCoordinates(img, ThresholdType::InsideRange, 255, 255);

            // Then:;
            Assert::AreEqual(2, (int) coordinates.size());
            cv::Point firstCoordinate = coordinates.at(0);
            cv::Point secondCoordinate = coordinates.at(1);
            Assert::AreEqual(4, firstCoordinate.x);
            Assert::AreEqual(2, firstCoordinate.y);
            Assert::AreEqual(5, secondCoordinate.x);
            Assert::AreEqual(6, secondCoordinate.y);
        }

        TEST_METHOD(find_pixel_coordinates_of_given_value_on_grayscale_32bits_image)
        {
            // Given: images
            cv::Mat img = cv::Mat::zeros(cv::Size(10, 10), CV_32FC1);
            img.at<float>(0, 0) = 1.f;
            img.at<float>(0, 2) = 0.251f;
            img.at<float>(0, 1) = 0.252f;
            img.at<float>(0, 5) = 0.253f;
            img.at<float>(6, 5) = 0.255f;
            img.at<float>(2, 4) = 0.255f;

            // When:
            std::vector<cv::Point> coordinates = img_operators::FindPixelCoordinates(img, ThresholdType::InsideRange, 0.255f, 0.255f);

            // Then:
            Assert::AreEqual(2, (int)coordinates.size());
            cv::Point firstCoordinate = coordinates.at(0);
            cv::Point secondCoordinate = coordinates.at(1);
            Assert::AreEqual(4, firstCoordinate.x);
            Assert::AreEqual(2, firstCoordinate.y);
            Assert::AreEqual(5, secondCoordinate.x);
            Assert::AreEqual(6, secondCoordinate.y);
        }

        TEST_METHOD(find_pixel_coordinates_strictly_above_value_on_grayscale_8bits_image)
        {
            // Given: images
            cv::Mat img = cv::Mat::zeros(cv::Size(10, 10), CV_8UC1);
            img.at<uchar>(0, 0) = 1;
            img.at<uchar>(0, 2) = 251;
            img.at<uchar>(0, 1) = 252;
            img.at<uchar>(0, 5) = 253;
            img.at<uchar>(6, 5) = 255;
            img.at<uchar>(2, 4) = 255;

            // When:
            std::vector<cv::Point> coordinates = img_operators::FindPixelCoordinates(img, ThresholdType::StrictlyAboveThreshold, 253);

            // Then:
            Assert::AreEqual(2, (int) coordinates.size());
            cv::Point firstCoordinate = coordinates.at(0);
            cv::Point secondCoordinate = coordinates.at(1);
            Assert::AreEqual(4, firstCoordinate.x);
            Assert::AreEqual(2, firstCoordinate.y);
            Assert::AreEqual(5, secondCoordinate.x);
            Assert::AreEqual(6, secondCoordinate.y);
        }

        TEST_METHOD(find_pixel_coordinates_strictly_above_value_on_grayscale_32bits_image)
        {
            // Given: images
            cv::Mat img = cv::Mat::zeros(cv::Size(10, 10), CV_32FC1);
            img.at<float>(0, 0) = 0.1f;
            img.at<float>(0, 2) = 0.251f;
            img.at<float>(0, 1) = 0.252f;
            img.at<float>(0, 5) = 0.253f;
            img.at<float>(6, 5) = 0.255f;
            img.at<float>(2, 4) = 0.255f;

            // When:
            std::vector<cv::Point> coordinates = img_operators::FindPixelCoordinates(img, ThresholdType::StrictlyAboveThreshold, 0.253f);

            // Then:
            Assert::AreEqual(2, (int) coordinates.size());
            cv::Point firstCoordinate = coordinates.at(0);
            cv::Point secondCoordinate = coordinates.at(1);
            Assert::AreEqual(4, firstCoordinate.x);
            Assert::AreEqual(2, firstCoordinate.y);
            Assert::AreEqual(5, secondCoordinate.x);
            Assert::AreEqual(6, secondCoordinate.y);
        }

        TEST_METHOD(find_pixel_coordinates_strictly_below_value_on_grayscale_8bits_image)
        {
            // Given: images
            cv::Mat img = cv::Mat(cv::Size(10, 10), CV_8UC1, 255);
            img.at<uchar>(0, 0) = 1;
            img.at<uchar>(0, 2) = 251;
            img.at<uchar>(0, 1) = 252;
            img.at<uchar>(0, 5) = 253;

            // When:
            std::vector<cv::Point> coordinates = img_operators::FindPixelCoordinates(img, ThresholdType::StrictlyBelowThreshold, 253);

            // Then:
            Assert::AreEqual(3, (int) coordinates.size());
            cv::Point firstCoordinate = coordinates.at(0);
            cv::Point secondCoordinate = coordinates.at(1);
            cv::Point thirdCoordinate = coordinates.at(2);
            Assert::AreEqual(0, firstCoordinate.x);
            Assert::AreEqual(0, firstCoordinate.y);
            Assert::AreEqual(1, secondCoordinate.x);
            Assert::AreEqual(0, secondCoordinate.y);
            Assert::AreEqual(2, thirdCoordinate.x);
            Assert::AreEqual(0, thirdCoordinate.y);
        }

        TEST_METHOD(find_pixel_coordinates_strictly_below_value_on_grayscale_32bits_image)
        {
            // Given: images
            cv::Mat img = cv::Mat(cv::Size(10, 10), CV_32FC1, 0.255);
            img.at<float>(0, 0) = 0.1f;
            img.at<float>(0, 2) = 0.251f;
            img.at<float>(0, 1) = 0.252f;
            img.at<float>(0, 5) = 0.253f;
            img.at<float>(6, 5) = 0.255f;
            img.at<float>(2, 4) = 0.255f;

            // When:
            std::vector<cv::Point> coordinates = img_operators::FindPixelCoordinates(img, ThresholdType::StrictlyBelowThreshold, 0.253f);

            // Then:
            Assert::AreEqual(3, (int) coordinates.size());
            cv::Point firstCoordinate = coordinates.at(0);
            cv::Point secondCoordinate = coordinates.at(1);
            cv::Point thirdCoordinate = coordinates.at(2);
            Assert::AreEqual(0, firstCoordinate.x);
            Assert::AreEqual(0, firstCoordinate.y);
            Assert::AreEqual(1, secondCoordinate.x);
            Assert::AreEqual(0, secondCoordinate.y);
            Assert::AreEqual(2, thirdCoordinate.x);
            Assert::AreEqual(0, thirdCoordinate.y);
        }

        TEST_METHOD(find_pixel_coordinates_above_or_equal_value_on_grayscale_8bits_image)
        {
            // Given: images
            cv::Mat img = cv::Mat::zeros(cv::Size(10, 10), CV_8UC1);
            img.at<uchar>(0, 0) = 1;
            img.at<uchar>(0, 2) = 251;
            img.at<uchar>(0, 1) = 252;
            img.at<uchar>(0, 5) = 253;
            img.at<uchar>(6, 5) = 255;
            img.at<uchar>(2, 4) = 255;

            // When:
            std::vector<cv::Point> coordinates = img_operators::FindPixelCoordinates(img, ThresholdType::AboveOrEqualThreshold, 253);

            // Then:
            Assert::AreEqual(3, (int) coordinates.size());
            cv::Point firstCoordinate = coordinates.at(0);
            cv::Point secondCoordinate = coordinates.at(1);
            cv::Point thirdCoordinate = coordinates.at(2);
            Assert::AreEqual(5, firstCoordinate.x);
            Assert::AreEqual(0, firstCoordinate.y);
            Assert::AreEqual(4, secondCoordinate.x);
            Assert::AreEqual(2, secondCoordinate.y);
            Assert::AreEqual(5, thirdCoordinate.x);
            Assert::AreEqual(6, thirdCoordinate.y);
        }

        TEST_METHOD(find_pixel_coordinates_above_or_equal_value_on_grayscale_32bits_image)
        {
            // Given: images
            cv::Mat img = cv::Mat::zeros(cv::Size(10, 10), CV_32FC1);
            img.at<float>(0, 0) = 0.1f;
            img.at<float>(0, 2) = 0.251f;
            img.at<float>(0, 1) = 0.252f;
            img.at<float>(0, 5) = 0.253f;
            img.at<float>(6, 5) = 0.255f;
            img.at<float>(2, 4) = 0.255f;

            // When:
            std::vector<cv::Point> coordinates = img_operators::FindPixelCoordinates(img, ThresholdType::AboveOrEqualThreshold, 0.253f);

            // Then:
            Assert::AreEqual(3, (int) coordinates.size());
            cv::Point firstCoordinate = coordinates.at(0);
            cv::Point secondCoordinate = coordinates.at(1);
            cv::Point thirdCoordinate = coordinates.at(2);
            Assert::AreEqual(5, firstCoordinate.x);
            Assert::AreEqual(0, firstCoordinate.y);
            Assert::AreEqual(4, secondCoordinate.x);
            Assert::AreEqual(2, secondCoordinate.y);
            Assert::AreEqual(5, thirdCoordinate.x);
            Assert::AreEqual(6, thirdCoordinate.y);
        }

        TEST_METHOD(find_pixel_coordinates_below_or_equal_value_on_grayscale_32bits_image)
        {
            // Given: images
            cv::Mat img = cv::Mat(cv::Size(10, 10), CV_32FC1, 0.255);
            img.at<float>(0, 0) = 0.1f;
            img.at<float>(0, 2) = 0.251f;
            img.at<float>(0, 1) = 0.252f;
            img.at<float>(0, 5) = 0.253f;
            img.at<float>(6, 5) = 0.255f;
            img.at<float>(2, 4) = 0.255f;

            // When:
            std::vector<cv::Point> coordinates = img_operators::FindPixelCoordinates(img, ThresholdType::BelowOrEqualThreshold, 0.253f);

            // Then:
            Assert::AreEqual(4, (int) coordinates.size());
            cv::Point firstCoordinate = coordinates.at(0);
            cv::Point secondCoordinate = coordinates.at(1);
            cv::Point thirdCoordinate = coordinates.at(2);
            cv::Point fourthCoordinate = coordinates.at(3);
            Assert::AreEqual(0, firstCoordinate.x);
            Assert::AreEqual(0, firstCoordinate.y);
            Assert::AreEqual(1, secondCoordinate.x);
            Assert::AreEqual(0, secondCoordinate.y);
            Assert::AreEqual(2, thirdCoordinate.x);
            Assert::AreEqual(0, thirdCoordinate.y);
            Assert::AreEqual(5, fourthCoordinate.x);
            Assert::AreEqual(0, fourthCoordinate.y);
        }

        TEST_METHOD(find_pixel_coordinates_below_or_equal_value_on_grayscale_8bits_image)
        {
            // Given: images
            cv::Mat img = cv::Mat(cv::Size(10, 10), CV_8UC1, 255);
            img.at<uchar>(0, 0) = 1;
            img.at<uchar>(0, 2) = 251;
            img.at<uchar>(0, 1) = 252;
            img.at<uchar>(0, 5) = 253;

            // When:
            std::vector<cv::Point> coordinates = img_operators::FindPixelCoordinates(img, ThresholdType::BelowOrEqualThreshold, 253);

            // Then:
            Assert::AreEqual(4, (int) coordinates.size());
            cv::Point firstCoordinate = coordinates.at(0);
            cv::Point secondCoordinate = coordinates.at(1);
            cv::Point thirdCoordinate = coordinates.at(2);
            cv::Point fourthCoordinate = coordinates.at(3);
            Assert::AreEqual(0, firstCoordinate.x);
            Assert::AreEqual(0, firstCoordinate.y);
            Assert::AreEqual(1, secondCoordinate.x);
            Assert::AreEqual(0, secondCoordinate.y);
            Assert::AreEqual(2, thirdCoordinate.x);
            Assert::AreEqual(0, thirdCoordinate.y);
            Assert::AreEqual(5, fourthCoordinate.x);
            Assert::AreEqual(0, fourthCoordinate.y);
        }

        TEST_METHOD(calculate_histogram_grayscale_8bits_image)
        {
            // Given: images
            cv::Mat img = cv::Mat::zeros(cv::Size(10, 10), CV_8UC1);
            img.at<uchar>(0, 0) = 1;
            img.at<uchar>(0, 1) = 255;
            img.at<uchar>(6, 5) = 255;

            // When:
            std::vector<float> hist = img_operators::CalculateHistogram(img, cv::Mat(), 256);

            // Then:
            auto pixelAtZeroNb = hist.at(0);
            Assert::AreEqual(97.f, pixelAtZeroNb);

            auto pixelAtOneNb = hist.at(1);
            Assert::AreEqual(1.f, pixelAtOneNb);

            auto pixelAtMaxValueNb = hist.at(255);
            Assert::AreEqual(2.f, pixelAtMaxValueNb);

            for (int bin = 2; bin < 255; bin++)
            {
                auto nonZerosPixelValueNb = hist.at(bin);
                Assert::AreEqual(0.f, nonZerosPixelValueNb);
            }
        }

        TEST_METHOD(calculate_histogram_grayscale_8bits_image_with_mask)
        {
            // Given: images
            cv::Mat img = cv::Mat::zeros(cv::Size(10, 10), CV_8UC1);
            img.at<uchar>(0, 0) = 1;
            img.at<uchar>(0, 1) = 255;
            img.at<uchar>(6, 5) = 255;

            cv::Mat mask = cv::Mat::zeros(cv::Size(10, 10), CV_8UC1);
            for (int col = 0; col < 10; col++)
            {
                mask.at<uchar>(0, col) = 255;
            };

            // When:
            std::vector<float> hist = img_operators::CalculateHistogram(img, mask, 256);

            // Then:
            auto pixelAtZeroNb = hist.at(0);
            Assert::AreEqual(8.f, pixelAtZeroNb);

            auto pixelAtOneNb = hist.at(1);
            Assert::AreEqual(1.f, pixelAtOneNb);

            auto pixelAtMaxValueNb = hist.at(255);
            Assert::AreEqual(1.f, pixelAtMaxValueNb);

            for (int bin = 2; bin < 255; bin++)
            {
                auto nonZerosPixelValueNb = hist.at(bin);
                Assert::AreEqual(0.f, nonZerosPixelValueNb);
            }
        }

        TEST_METHOD(calculate_histogram_grayscale_32bits_image)
        {
            // Given images
            cv::Mat img = cv::Mat::zeros(cv::Size(100, 100), CV_32FC1);
            for (int col = 0; col < img.cols; col++)
            {
                for (int row = 0; row < 50; row++)
                {
                    img.at<float>(row, col) = 1;
                }
            }

            // When
            std::vector<float> hist = img_operators::CalculateHistogram(img, cv::Mat(), 10);

            // Then:
            auto pixelAtZeroNb = hist.at(0);
            Assert::AreEqual(5000.f, pixelAtZeroNb);

            auto pixelAtMaxValueNb = hist.at(9);
            Assert::AreEqual(5000.f, pixelAtMaxValueNb);

            for (int bin = 1; bin < 9; bin++)
            {
                auto nonZerosPixelValueNb = hist.at(bin);
                Assert::AreEqual(0.f, nonZerosPixelValueNb);
            }
        }

        TEST_METHOD(calculate_histogram_grayscale_32bits_image_with_mask)
        {
            // Given images
            cv::Mat img = cv::Mat::zeros(cv::Size(100, 100), CV_32FC1);
            for (int col = 0; col < img.cols; col++)
            {
                for (int row = 0; row < 50; row++)
                {
                    img.at<float>(row, col) = 1;
                }
            }

            cv::Mat mask = cv::Mat::zeros(cv::Size(100, 100), CV_8UC1);
            for (int col = 25; col < 75; col++)
            {
                for (int row = 25; row < 75; row++)
                {
                    mask.at<uchar>(row, col) = 255;
                }
            }

            // When
            std::vector<float> hist = img_operators::CalculateHistogram(img, mask, 10);

            // Then:
            auto pixelAtZeroNb = hist.at(0);
            Assert::AreEqual(1250.f, pixelAtZeroNb);

            auto pixelAtMaxValueNb = hist.at(9);
            Assert::AreEqual(1250.f, pixelAtMaxValueNb);

            for (int bin = 1; bin < 9; bin++)
            {
                auto nonZerosPixelValueNb = hist.at(bin);
                Assert::AreEqual(0.f, nonZerosPixelValueNb);
            }
        }

        TEST_METHOD(compute_saturation_8bit_image)
        {
            // Given image
            cv::Mat img = cv::Mat(cv::Size(10, 10), CV_8UC1, cv::Scalar(128));

            double sat = img_operators::ComputeGreyLevelSaturation(img, cv::Mat(), 0.03f);
            Assert::AreEqual(128.0, sat);
        }
    };
}