#include "CppUnitTest.h"

#include <exception>
#include <iostream>
#include <opencv2/opencv.hpp>
#include <stdlib.h>

#include "CEdgeDetector.hpp"
#include "CRegistration.hpp"


#pragma unmanaged

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

#define TEST_DATA_PATH std::string(".\\..\\..\\Tests\\Data\\")

namespace SharedOpenCVNativeTests
{
    namespace transform_mat {
        /**
         * Extract an affine transformation from an homography matrix
         * /!\ leads to a loss of information (the perspective deformation is lost, only the affine deformation is preserved.
         *
         * @param  homography - homography matrix
         *
         * @return the affine matrix
         */
        cv::Mat ExtractAffineFromHomography(const cv::Mat& homography) {
            CV_Assert(homography.rows == 3 && homography.cols == 3);
            return homography(cv::Rect(0, 0, 3, 2));
        }

        /**
         * Extract the translation component from an affine transformation matrix
         *
         * @param  affine - affine transformation matrix
         *
         * @return the translation associated
         */
        cv::Point2f ExtractTranslationFromAffine(const cv::Mat& affine) {
            CV_Assert(affine.rows == 2 && affine.cols == 3);

            // The general rule for Matrix typenames in OpenCV is: CV_<bit_depth>(S|U|F)C<number_of_channels>
            // S = Signed integer
            // U = Unsigned integer
            // F = Float
            switch (affine.depth()) {
            case CV_8U:
                return cv::Point2f((float)affine.at<uint8_t>(0, 2), (float)affine.at<uint8_t>(1, 2));
            case CV_8S:
                return cv::Point2f((float)affine.at<int8_t>(0, 2), (float)affine.at<int8_t>(1, 2));
            case CV_16U:
                return cv::Point2f((float)affine.at<uint16_t>(0, 2), (float)affine.at<uint16_t>(1, 2));
            case CV_16S:
                return cv::Point2f((float)affine.at<int16_t>(0, 2), (float)affine.at<int16_t>(1, 2));
            case CV_32S:
                return cv::Point2f((float)affine.at<int32_t>(0, 2), (float)affine.at<int32_t>(1, 2));
            case CV_32F:
                return cv::Point2f(affine.at<float>(0, 2), affine.at<float>(1, 2));
            case CV_64F:
                return cv::Point2f((float)affine.at<double>(0, 2), (float)affine.at<double>(1, 2));
            default:
                return cv::Point2f(NAN, NAN);
            }
        }

        /**
         * Combine two sequential homography transformations
         * Sometimes if two transformations are performed after each other, significant image areas would get lost.
         * Instead if they are combined, it is like the full operation done in a single step without losing image parts in the intemediate step.
         *
         * @param  first  - the first transformation
         * @param  second - the second transformation (/!\ order is important !)
         *
         * @return the combined homography transformation
         */
        cv::Mat CombineHomographies(const cv::Mat& first, const cv::Mat& second) {
            CV_Assert(first.rows == 3 && first.cols == 3);
            CV_Assert(second.rows == 3 && second.cols == 3);

            // combined by homograhy multiplication
            return first * second;
        }

        /**
         * Combine two sequential affine transformations
         * Sometimes if two transformations are performed after each other, significant image areas would get lost.
         * Instead if they are combined, it is like the full operation done in a single step without losing image parts in the intemediate step.
         *
         * @param  first  - the first transformation
         * @param  second - the second transformation (/!\ order is important !)
         *
         * @return the combined affine transformation
         */
        cv::Mat CombineAffines(const cv::Mat& first, const cv::Mat& second) {
            CV_Assert(first.rows == 2 && first.cols == 3);
            CV_Assert(second.rows == 2 && second.cols == 3);

            // create homographies to combine by homograhy multiplication

            cv::Mat h1 = cv::Mat::eye(3, 3, CV_64FC1);
            cv::Mat h2 = cv::Mat::eye(3, 3, CV_64FC1);

            cv::Mat tmp1, tmp2, resultMat;
            first.convertTo(tmp1, CV_64FC1);
            second.convertTo(tmp2, CV_64FC1);

            tmp1.row(0).copyTo(h1.row(0));
            tmp1.row(1).copyTo(h1.row(1));

            tmp2.row(1).copyTo(h2.row(1));
            tmp2.row(0).copyTo(h2.row(0));

            resultMat = CombineHomographies(h1, h2);

            h1.release();
            h2.release();
            tmp1.release();
            tmp2.release();

            return ExtractAffineFromHomography(resultMat);
        }
    }; // namespace transform_mat

	TEST_CLASS(RegistrationTest)
	{
    protected:
        double const DEFAULT_SCALE = 1.0; // must be strictly greater than 0. A scale of 1 does not modify the scaling, above it, it enlarges and below it, it shrinks

        static void SetUp1(cv::Mat& ref_img) {
            char buf[255];
            std::string image_path = TEST_DATA_PATH + std::string("cat.png");
            ref_img = cv::imread(image_path);
            if (ref_img.empty()) {
                _snprintf_s(buf, 255, "Could not read the image: %s\n", image_path.c_str());
                Logger::WriteMessage(buf);
            }
            Assert::IsFalse(ref_img.empty());
        }

        static void SetUp2(cv::Mat& ref_img2) {
            char buf[255];
            std::string image_path2 = TEST_DATA_PATH + std::string("dandelion.jpg");
            ref_img2 = cv::imread(image_path2);
            if (ref_img2.empty()) {
                _snprintf_s(buf, 255, "Could not read the image: %s\n", image_path2.c_str());
                Logger::WriteMessage(buf);
            }
            Assert::IsFalse(ref_img2.empty());
        }

        void transform(cv::Mat& img, cv::Mat& img_transformed, double x, double y, int angle, float scale) {
            // Grab the shift matrix
            cv::Mat tr_mat = (cv::Mat_<double>(2, 3) << 1, 0, x, 0, 1, y);

            // Grab the rotation matrix  (applying the negative of the angle to rotate clockwise)
            cv::Point2f center = cv::Point2f(0.5f * float(img.cols) , 0.5f * float(img.rows));
            cv::Mat rot_mat = getRotationMatrix2D(center, -angle, scale);

            // Combine the shift matrix and rotation matrix
            cv::Mat M = transform_mat::CombineAffines(tr_mat, rot_mat);

            // Warp image
            cv::warpAffine(img, img_transformed, M, img.size(), cv::INTER_CUBIC, cv::BORDER_CONSTANT, cv::Scalar::all(0));
        }

	public:
		
        TEST_CLASS_INITIALIZE(ClassInitialize)
        {
            Logger::WriteMessage("RegistrationTest Initialize");
       
        }

        TEST_CLASS_CLEANUP(ClassCleanup)
        {
            Logger::WriteMessage("RegistrationTest Cleanup");
        }

		TEST_METHOD(image_shift_is_insignificant_between_two_same_images)
		{
            Logger::WriteMessage("image_shift_is_insignificant_between_two_same_images\n");
            cv::Mat ref_img;
            SetUp1(ref_img);

            // Given : Sensed image not shifted from the reference image
            cv::Mat sensed_img = ref_img;

            // When : Compute image registration between sensed and reference images
            std::tuple<double, cv::Point2f, double> result = registration::ComputeAngleAndShift(ref_img, sensed_img);
            cv::Point2f shift_vec = std::get<1>(result);

            // Then : Shift computed is near of zero (1 pixel precision)
            Assert::AreEqual(0, shift_vec.x, 1);
            Assert::AreEqual(0, shift_vec.y, 1);
            Logger::WriteMessage("Done\n");
		}

        TEST_METHOD(image_shift_is_correct_between_two_shifted_images)
        {
            Logger::WriteMessage("image_shift_is_correct_between_two_shifted_images\n");
            cv::Mat ref_img;
            SetUp1(ref_img);
            // Given : Sensed image shifted by a known value from the reference image
            double expected_shift_x = 10.0;
            double expected_shift_y = 15.0;
            int expected_angle = 0;
            double scale = DEFAULT_SCALE;
            cv::Mat sensed_img;
            transform(ref_img, sensed_img, expected_shift_x, expected_shift_y, expected_angle, (float) scale);

            // When : Compute image registration between sensed and reference images
            std::tuple<double, cv::Point2f, double> angleAndShift = registration::ComputeAngleAndShift(ref_img, sensed_img);
            cv::Point2f shift_vec = std::get<1>(angleAndShift);

            // Then : Shift computed is near of initial known shift (1 pixel precision)
             Assert::AreEqual(expected_shift_x, (double)shift_vec.x, 1.0);
             Assert::AreEqual(expected_shift_y, (double)shift_vec.y, 1.0);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(image_shift_is_correct_between_two_images_rotated_less_than_positive_45_degrees)
        {
            Logger::WriteMessage("image_shift_is_correct_between_two_images_rotated_less_than_positive_45_degrees\n");
            cv::Mat ref_img;
            SetUp1(ref_img);

            // Given : Sensed image shifted, scaled and rotated by a known value from the reference image
            double expected_shift_y = 0.0;
            double expected_shift_x = 0.0;
            double scale = DEFAULT_SCALE;
            std::list<int> expected_angles = { 5, 10, 25, 45 };
            for each (int expected_angle in expected_angles) {
                cv::Mat sensed_img;
                transform(ref_img, sensed_img, expected_shift_x, expected_shift_y, expected_angle, (float) scale);

                // When : Compute image registration between sensed and reference images
                std::tuple<double, cv::Point2f, double> angleAndShift = registration::ComputeAngleAndShift(ref_img, sensed_img, cv::Rect(), 50.0);
                double angle = std::get<0>(angleAndShift);
                cv::Point2f shift_vec = std::get<1>(angleAndShift);

                // Then : Angle computed is near of initial known angle (5 degrees precision)
                Assert::IsTrue(fabs(expected_angle - angle) < 5 || fabs((expected_angle - 360) - angle) < 5);
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(image_shift_is_correct_between_two_images_rotated_less_than_negative_45_degrees)
        {
            Logger::WriteMessage("image_shift_is_correct_between_two_images_rotated_less_than_negative_45_degrees\n");
            cv::Mat ref_img;
            SetUp1(ref_img);
            // Given : Sensed image shifted, scaled and rotated by a known value from the reference image
            double expected_shift_x = 0.0;
            double expected_shift_y = 0.0;
            double scale = DEFAULT_SCALE;
            std::list<double> expected_angles = { -15.0, -20.0, -45.0 };
            for each (double expected_angle in expected_angles) {
                cv::Mat sensed_img;
                transform(ref_img, sensed_img, expected_shift_x, expected_shift_y, (int) expected_angle, (float)scale);

                // When : Compute image registration between sensed and reference images
                std::tuple<double, cv::Point2f, double> angleAndShift = registration::ComputeAngleAndShift(ref_img, sensed_img, cv::Rect(), 50.0);
                double angle = std::get<0>(angleAndShift);
                cv::Point2f shift_vec = std::get<1>(angleAndShift);

                // Then : Angle computed is near of initial known angle (5 degrees precision)
                Assert::IsTrue(fabs(expected_angle - angle) < 5 || fabs((expected_angle + 360) - angle) < 5);
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(image_shift_is_correct_between_two_shifted_images_when_a_good_roi_is_provided)
        {
            Logger::WriteMessage("image_shift_is_correct_between_two_shifted_images_when_an_roi_is_provided \n");
            cv::Mat ref_img;
            SetUp1(ref_img);
            double gamma = 0.0;

            // Given : Sensed image shifted by a known value from the reference image and a correct roi is provided
            double expected_shift_x = 10.0;
            double expected_shift_y = 5.0;
            int expected_angle = 0;
            double scale = DEFAULT_SCALE;
            cv::Mat sensed_img;
            transform(ref_img, sensed_img, expected_shift_x, expected_shift_y, expected_angle, (float) scale);
            cv::Rect roi = cv::Rect(65, 115, 160, 120);

            // When : Compute image registration between sensed and reference images
            std::tuple<double, cv::Point2f, double> angleAndShift = registration::ComputeAngleAndShift(ref_img, sensed_img, roi);
            double angle = std::get<0>(angleAndShift);
            cv::Point2f shift_vec = std::get<1>(angleAndShift);

            // Then : Shift computed is near of initial known shift (1 pixel precision)
             Assert::AreEqual(expected_shift_x, (double)shift_vec.x, 1.0);
             Assert::AreEqual(expected_shift_y, (double)shift_vec.y, 1.0);
             Assert::AreEqual(expected_angle, angle, 1.0);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(image_shift_is_incorrect_between_two_shifted_images_when_a_bad_roi_is_provided)
        {
            Logger::WriteMessage("image_shift_is_correct_between_two_shifted_images_when_an_roi_is_provided \n");
            cv::Mat ref_img;
            SetUp1(ref_img);
            double gamma = 0.0;

            // Given : Sensed image shifted by a known value from the reference image and an too little roi is provided
            double expected_shift_x = 10.0;
            double expected_shift_y = 5.0;
            int expected_angle = 0;
            double scale = DEFAULT_SCALE;
            cv::Mat sensed_img;
            transform(ref_img, sensed_img, expected_shift_x, expected_shift_y, expected_angle, (float)scale);
            cv::Rect roi = cv::Rect(0, 0, 10, 10);

            // When : Compute image registration between sensed and reference images
            std::tuple<double, cv::Point2f, double> angleAndShift = registration::ComputeAngleAndShift(ref_img, sensed_img, roi, 360.0);
            double angle = std::get<0>(angleAndShift);
            cv::Point2f shift_vec = std::get<1>(angleAndShift);

            // Then : Shift computed is near of initial known shift (1 pixel precision)
            Assert::AreNotEqual(expected_shift_x, (double)shift_vec.x, 10);
            Assert::AreNotEqual(expected_shift_y, (double)shift_vec.y, 10);
            Assert::AreNotEqual(expected_angle, angle, 10);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(image_shift_is_correct_between_two_shifted_images_with_empty_roi_because_roi_is_ignored_in_this_case)
        {
            Logger::WriteMessage("image_shift_is_correct_between_two_shifted_images_when_an_roi_is_provided \n");
            cv::Mat ref_img;
            SetUp1(ref_img);
            double gamma = 0.0;

            // Given : Sensed image shifted by a known value from the reference image and an empty roi is provided
            double expected_shift_x = 10.0;
            double expected_shift_y = 5.0;
            int expected_angle = 0;
            double scale = DEFAULT_SCALE;
            cv::Mat sensed_img;
            transform(ref_img, sensed_img, expected_shift_x, expected_shift_y, expected_angle, (float)scale);
            cv::Rect roi = cv::Rect(0, 0, 0, 0);

            // When : Compute image registration between sensed and reference images
            std::tuple<double, cv::Point2f, double> angleAndShift = registration::ComputeAngleAndShift(ref_img, sensed_img, roi);
            double angle = std::get<0>(angleAndShift);
            cv::Point2f shift_vec = std::get<1>(angleAndShift);

            // Then : Shift computed is near of initial known shift (1 pixel precision)
            Assert::AreEqual(expected_shift_x, (double)shift_vec.x, 10);
            Assert::AreEqual(expected_shift_y, (double)shift_vec.y, 10);
            Assert::AreEqual(expected_angle, angle, 10);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(image_shift_is_correct_between_two_shifted_and_rotated_images)
        {
            Logger::WriteMessage("image_shift_is_correct_between_two_shifted_and_rotated_images \n");
            cv::Mat ref_img;
            SetUp1(ref_img);

            // Given : Sensed image shifted, scaled and rotated by a known value from the reference image
            double expected_shift_x = 10.0;
            double expected_shift_y = 5.0;
            int expected_angle = 15;
            double scale = DEFAULT_SCALE;
            cv::Mat sensed_img;
            transform(ref_img, sensed_img, expected_shift_x, expected_shift_y, expected_angle, (float)scale);

            // When : Compute image registration between sensed and reference images
            double angleSigmaTolerance = (double)expected_angle;
            std::tuple<double, cv::Point2f, double> angleAndShift = registration::ComputeAngleAndShift(ref_img, sensed_img, cv::Rect(), angleSigmaTolerance);
            double angle = std::get<0>(angleAndShift);
            cv::Point2f shift_vec = std::get<1>(angleAndShift);

            // Then : Shift computed is near of initial known shift (10 pixel precision)
            Assert::AreEqual(expected_shift_x, (double)shift_vec.x, 5.0);
            Assert::AreEqual(expected_shift_y, (double)shift_vec.y, 5.0);
            Assert::IsTrue(fabs(expected_angle - angle) < 5.0 || fabs((expected_angle - 360.0) - angle) < 5.0);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(image_similarity_is_maximal_when_images_are_identical)
        {
            Logger::WriteMessage("image_similarity_is_maximal_when_images_are_identical \n");
            cv::Mat ref_img;
            SetUp1(ref_img);
            // Given : Sensed image is strictly identical at the reference image
            cv::Mat sensed_img = ref_img;

            // When : Compute image similarity between sensed and reference images
            double similarity_score = registration::ComputeSimilarity(ref_img, sensed_img);

            // Then : Similarity score is the maximal score
            double maximal_score = 1.0;
            Assert::AreEqual(maximal_score, similarity_score, 0.1);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(image_similarity_is_minimal_when_images_are_inversed)
        {
            Logger::WriteMessage("image_similarity_is_minimal_when_images_are_inversed \n");
            cv::Mat ref_img;     
            SetUp1(ref_img);
            cv::Mat ref_img2;
            SetUp2(ref_img2);

            // Given :Two images strictly differents
            // When : Compute image similarity between sensed and reference images
            double similarity_score = registration::ComputeSimilarity(ref_img, ref_img2);

            // Then : Similarity score is nearly the minimal score
            double minimal_score = 0;
            Assert::AreEqual(minimal_score, similarity_score, 0.1);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(image_similarity_decreases_as_the_offset_between_images_increases)
        {
            char buf[255];
        
            Logger::WriteMessage("image_similarity_decreases_as_the_offset_between_images_increases \n");
            cv::Mat ref_img;
            SetUp1(ref_img);

            // Given : Sensed images increasingly shifted from the reference image
            std::vector<cv::Mat> sensed_imgs;
            double shift_x = 0.0;
            double shift_y = 0.0;
            int angle = 0;
            double scale = DEFAULT_SCALE;
            for (int i = 0; i < 20; i++) {
                _snprintf_s(buf, 255, "perform transform sensed[%d]\n", i);
                Logger::WriteMessage(buf);
                cv::Mat sensed_img;
                transform(ref_img, sensed_img, shift_x + i, shift_y + i, angle, (float) scale);
                sensed_imgs.push_back(sensed_img);
            }

            // When : Compute image similarity between sensed and reference images
            std::vector<double> similarity_scores;
            for (int i = 0; i < sensed_imgs.size(); i++) {
                _snprintf_s(buf, 255, "compute similarity ref vs sensed[%d]\n", i);
                Logger::WriteMessage(buf);
                similarity_scores.push_back(registration::ComputeSimilarity(ref_img, sensed_imgs.at(i)));
            }

            // Then : Similarity score decreases as the offset between images increases
            double maximal_score = 1.0;
            double previous_score = maximal_score + 0.1;
            for (int i = 0; i < similarity_scores.size(); i++) {
                _snprintf_s(buf, 255, "prevsim vs sim[%d] : %lf > %lf \n", i, previous_score, similarity_scores.at(i));
                Logger::WriteMessage(buf);      
                Assert::IsTrue(previous_score > similarity_scores.at(i));
                previous_score = similarity_scores.at(i);
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(image_similarity_compute_score_only_on_ROI)
        {
            Logger::WriteMessage("image_similarity_compute_score_only_on_ROI \n");
            cv::Mat ref_img;
            SetUp1(ref_img);
            cv::Mat ref_img2;
            SetUp2(ref_img2);

            // Given : Two images with a strictly similar region and a different region
            int colsNb = (ref_img.cols > ref_img2.cols) ? ref_img2.cols : ref_img.cols;
            cv::Rect similar_region = cv::Rect(0, 0, colsNb, ref_img.rows / 2);
            cv::Rect diff_region_1 = cv::Rect(0, 0, colsNb, ref_img2.rows / 2);
            cv::Rect diff_region_2 = cv::Rect(0, ref_img2.rows / 2, colsNb, ref_img2.rows / 2);
            cv::Mat sub_img_same = ref_img(similar_region).clone();
            cv::Mat sub_img_diff1 = ref_img2(diff_region_1).clone();
            cv::Mat sub_img_diff2 = ref_img2(diff_region_2).clone();
            cv::Mat im1;
            cv::Mat im2;
            cv::vconcat(sub_img_same, sub_img_diff1, im1);
            cv::vconcat(sub_img_same, sub_img_diff2, im2);

            // When : Compute the similarity on all the images, then only on the region of interest corresponding to a similar area between the two images.
            double similarity_score_all = registration::ComputeSimilarity(im1, im2);
            double similarity_score_roi = registration::ComputeSimilarity(im1, im2, similar_region);

            // Then : Similarity score is the maximal score if we give the similar region and less otherwise
            double maximal_score = 1;
            Assert::AreEqual(maximal_score, similarity_score_roi, 0.001);
            Assert::IsTrue(similarity_score_roi > similarity_score_all);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(image_similarity_increases_when_registration_is_applied)
        {
            Logger::WriteMessage("image_similarity_increases_when_registration_is_applied \n");
            cv::Mat ref_img;
            SetUp1(ref_img);

            // Given : Sensed image shifted from the reference image
            double shift_x = 10;
            double shift_y = 5;
            int angle = 0;
            double scale = DEFAULT_SCALE;
            cv::Mat sensed_img;
            transform(ref_img, sensed_img, shift_x, shift_y, angle, (float)scale);
            // Given : The image realigned according to result of registration computation
            std::tuple<double, cv::Point2f, double> angleAndShift = registration::ComputeAngleAndShift(ref_img, sensed_img);
            cv::Point2f shift_vec = std::get<1>(angleAndShift);
            cv::Mat shift_mat = (cv::Mat_<double>(2, 3) << 1, 0, -shift_vec.x, 0, 1, -shift_vec.y);
            cv::Mat realigned_img = registration::ImgRegistration(ref_img, sensed_img, shift_mat);

            // When : Compute image similarity between sensed/reference images and realigned/reference image
            double initial_similarity_score = registration::ComputeSimilarity(ref_img, sensed_img);
            double final_similarity_score = registration::ComputeSimilarity(ref_img, realigned_img);

            // Then : similarity score increase after registration and is near to 1
            double maximal_score = 1;
            Assert::IsTrue(initial_similarity_score < final_similarity_score);
            Assert::AreEqual(maximal_score, final_similarity_score, 0.1);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(image_similarity_increases_when_registration_is_applied_other_case)
        {
            Logger::WriteMessage("image_similarity_increases_when_registration_is_applied_other_case \n");
            cv::Mat ref_img;
            SetUp1(ref_img);

            // Given : Sensed image shifted from the reference image
            double shift_x = -20;
            double shift_y = -17;
            int angle = 0;
            double scale = DEFAULT_SCALE;
            cv::Mat sensed_img;
            transform(ref_img, sensed_img, shift_x, shift_y, angle, (float)scale);
            // Given : The image realigned according to result of registration computation
            std::tuple<double, cv::Point2f, double> angleAndShift = registration::ComputeAngleAndShift(ref_img, sensed_img);
            cv::Point2f shift_vec = std::get<1>(angleAndShift);
            cv::Mat shift_mat = (cv::Mat_<double>(2, 3) << 1, 0, -shift_vec.x, 0, 1, -shift_vec.y);
            cv::Mat realigned_img = registration::ImgRegistration(ref_img, sensed_img, shift_mat);

            // When : Compute image similarity between sensed/reference images and realigned/reference image
            double initial_similarity_score = registration::ComputeSimilarity(ref_img, sensed_img);
            double final_similarity_score = registration::ComputeSimilarity(ref_img, realigned_img);

            // Then : similarity score increase after registration and is near to 1
            double maximal_score = 1;
            Assert::IsTrue(initial_similarity_score < final_similarity_score);
            Assert::AreEqual(maximal_score, final_similarity_score, 0.1);
            Logger::WriteMessage("Done\n");
        }
        
        TEST_METHOD(image_similarity_remains_stable_when_registration_is_applied_on_not_shifted_image)
        {
            Logger::WriteMessage("image_similarity_remains_stable_when_registration_is_applied_on_not_shifted_image \n");
            cv::Mat ref_img;
            SetUp1(ref_img);

            // Given : Sensed image not shifted from the reference image
            cv::Mat sensed_img = ref_img;
            // Given : The image realigned according to result of registration computation

            std::tuple<double, cv::Point2f, double> angleAndShift = registration::ComputeAngleAndShift(ref_img, sensed_img);
            cv::Point2f shift_vec = std::get<1>(angleAndShift);
            cv::Mat shift_mat = (cv::Mat_<double>(2, 3) << 1, 0, -shift_vec.x, 0, 1, -shift_vec.y);
            cv::Mat realigned_img = registration::ImgRegistration(ref_img, sensed_img, shift_mat);

            // When : Compute image similarity between sensed/reference images and realigned/reference image
            double initial_similarity_score = registration::ComputeSimilarity(ref_img, sensed_img);
            double final_similarity_score = registration::ComputeSimilarity(ref_img, realigned_img);

            // Then : similarity score remains stable after registration and is near to 1
            double maximal_score = 1;
            Assert::AreEqual(initial_similarity_score, final_similarity_score, 0.01);
            Assert::AreEqual(maximal_score, final_similarity_score, 0.1);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(normal_sensed_image_and_sensed_image_upscaled_by_10_percent_give_similar_shifts_and_confidences)
        {
            Logger::WriteMessage("normal_sensed_image_and_sensed_image_upscaled_by_10_percent_give_similar_registrations\n");
            cv::Mat ref_img;
            SetUp1(ref_img);
            // Given : Sensed image shifted by a known value from the reference image
            double shift_x = 10.0;
            double shift_y = 15.0;
            int angle = 0;
            double scale = DEFAULT_SCALE;
            cv::Mat sensed_img;
            transform(ref_img, sensed_img, shift_x, shift_y, angle, (float)scale);

            // When : Compute image registration between sensed and reference images
            std::tuple<double, cv::Point2f, double> angleAndShift = registration::ComputeAngleAndShift(ref_img, sensed_img);
            cv::Point2f shift_vec = std::get<1>(angleAndShift);
            double confidence = std::get<2>(angleAndShift);

            // Upscaled by 10%
            scale = 1.1;
            cv::Mat sensed_img_scaled;
            transform(ref_img, sensed_img_scaled, shift_x, shift_y, angle, (float)scale);

            double angleSigmaTolerance = 6.0;
            // Set the scale tolerance so the sensed scale is still inside it
            double scaleSigmaTolerance = scale;

            // When : Compute image registration between sensed upscaled by 10% and reference images
            std::tuple<double, cv::Point2f, double> angleAndShiftScaled = registration::ComputeAngleAndShift(ref_img, sensed_img_scaled, cv::Rect(), angleSigmaTolerance, scaleSigmaTolerance);
            cv::Point2f shift_vec_scaled = std::get<1>(angleAndShiftScaled);
            double confidence_scaled = std::get<2>(angleAndShiftScaled);

            // Then : Shift computed is near the the scaled computed shift (1 pixel precision)
            // And the confidence is also near
            Assert::AreEqual((double)shift_vec.x, (double)shift_vec_scaled.x, 1.0);
            Assert::AreEqual((double)shift_vec.y, (double)shift_vec_scaled.y, 1.0);
            Assert::AreEqual(confidence, confidence_scaled, 0.05);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(normal_sensed_image_and_sensed_image_downscaled_by_10_percent_give_similar_shifts_and_confidences)
        {
            Logger::WriteMessage("normal_sensed_image_and_sensed_image_downscaled_by_10_percent_give_similar_registrations\n");
            cv::Mat ref_img;
            SetUp1(ref_img);
            // Given : Sensed image shifted by a known value from the reference image
            double shift_x = 10.0;
            double shift_y = 15.0;
            int angle = 0;
            double scale = DEFAULT_SCALE;
            cv::Mat sensed_img;
            transform(ref_img, sensed_img, shift_x, shift_y, angle, (float)scale);

            // When : Compute image registration between sensed and reference images
            std::tuple<double, cv::Point2f, double> angleAndShift = registration::ComputeAngleAndShift(ref_img, sensed_img);
            cv::Point2f shift_vec = std::get<1>(angleAndShift);
            double confidence = std::get<2>(angleAndShift);

            // Downscaled by 10%
            scale = 0.9;
            cv::Mat sensed_img_scaled;
            transform(ref_img, sensed_img_scaled, shift_x, shift_y, angle, (float)scale);

            double angleSigmaTolerance = 6.0;
            // Set the scale tolerance so the sensed scale is still inside it
            double scaleSigmaTolerance = scale;

            // When : Compute image registration between sensed downscaled by 10% and reference images
            std::tuple<double, cv::Point2f, double> angleAndShiftScaled = registration::ComputeAngleAndShift(ref_img, sensed_img_scaled, cv::Rect(), angleSigmaTolerance, scaleSigmaTolerance);
            cv::Point2f shift_vec_scaled = std::get<1>(angleAndShiftScaled);
            double confidence_scaled = std::get<2>(angleAndShiftScaled);

            // Then : Shift computed is near the the scaled computed shift (1 pixel precision)
            // And the confidence is also near
            Assert::AreEqual((double)shift_vec.x, (double)shift_vec_scaled.x, 1.0);
            Assert::AreEqual((double)shift_vec.y, (double)shift_vec_scaled.y, 1.0);
            Assert::AreEqual(confidence, confidence_scaled, 0.05);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(normal_sensed_image_and_sensed_image_upscaled_by_10_percent_give_different_shifts_and_confidences_when_the_scale_tolerance_is_too_low)
        {
            Logger::WriteMessage("normal_sensed_image_and_sensed_image_upscaled_by_10_percent_give_different_shifts_and_confidences_when_the_scale_tolerance_is_too_low\n");
            cv::Mat ref_img;
            SetUp1(ref_img);
            // Given : Sensed image shifted by a known value from the reference image
            double shift_x = 10.0;
            double shift_y = 15.0;
            int angle = 0;
            double scale = DEFAULT_SCALE;
            cv::Mat sensed_img;
            transform(ref_img, sensed_img, shift_x, shift_y, angle, (float)scale);

            // When : Compute image registration between sensed and reference images
            std::tuple<double, cv::Point2f, double> angleAndShift = registration::ComputeAngleAndShift(ref_img, sensed_img);
            cv::Point2f shift_vec = std::get<1>(angleAndShift);
            double confidence = std::get<2>(angleAndShift);

            // Upscaled by 10%
            scale = 1.1;
            cv::Mat sensed_img_scaled;
            transform(ref_img, sensed_img_scaled, shift_x, shift_y, angle, (float)scale);

            double angleSigmaTolerance = 6.0;
            // Set the scale tolerance very low so the sensed scale outside of it
            double scaleSigmaTolerance = 0.001;

            // When : Compute image registration between sensed upscaled by 10% and reference images
            std::tuple<double, cv::Point2f, double> angleAndShiftScaled = registration::ComputeAngleAndShift(ref_img, sensed_img_scaled, cv::Rect(), angleSigmaTolerance, scaleSigmaTolerance);
            cv::Point2f shift_vec_scaled = std::get<1>(angleAndShiftScaled);
            double confidence_scaled = std::get<2>(angleAndShiftScaled);

            // Then : Shift computed is different from the scaled computed shift (1 pixel precision)
            // And the confidence is also different
            Assert::AreNotEqual((double)shift_vec.x, (double)shift_vec_scaled.x, 1.0);
            Assert::AreNotEqual((double)shift_vec.y, (double)shift_vec_scaled.y, 1.0);
            Assert::AreNotEqual(confidence, confidence_scaled, 0.05);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(normal_sensed_image_and_sensed_image_downscaled_by_10_percent_give_different_shifts_and_confidences_when_the_scale_tolerance_is_too_low)
        {
            Logger::WriteMessage("normal_sensed_image_and_sensed_image_downscaled_by_10_percent_give_different_shifts_and_confidences_when_the_scale_tolerance_is_too_low\n");
            cv::Mat ref_img;
            SetUp1(ref_img);
            // Given : Sensed image shifted by a known value from the reference image
            double shift_x = 10.0;
            double shift_y = 15.0;
            int angle = 0;
            double scale = DEFAULT_SCALE;
            cv::Mat sensed_img;
            transform(ref_img, sensed_img, shift_x, shift_y, angle, (float)scale);

            // When : Compute image registration between sensed and reference images
            std::tuple<double, cv::Point2f, double> angleAndShift = registration::ComputeAngleAndShift(ref_img, sensed_img);
            cv::Point2f shift_vec = std::get<1>(angleAndShift);
            double confidence = std::get<2>(angleAndShift);

            // Downscaled by 10%
            scale = 0.9;
            cv::Mat sensed_img_scaled;
            transform(ref_img, sensed_img_scaled, shift_x, shift_y, angle, (float)scale);

            double angleSigmaTolerance = 6.0;
            // Set the scale tolerance very low so the sensed scale outside of it
            double scaleSigmaTolerance = 0.001;

            // When : Compute image registration between sensed downscaled by 10% and reference images
            std::tuple<double, cv::Point2f, double> angleAndShiftScaled = registration::ComputeAngleAndShift(ref_img, sensed_img_scaled, cv::Rect(), angleSigmaTolerance, scaleSigmaTolerance);
            cv::Point2f shift_vec_scaled = std::get<1>(angleAndShiftScaled);
            double confidence_scaled = std::get<2>(angleAndShiftScaled);

            // Then : Shift computed is different from the scaled computed shift (1 pixel precision)
            // And the confidence is also different
            Assert::AreNotEqual((double)shift_vec.x, (double)shift_vec_scaled.x, 1.0);
            Assert::AreNotEqual((double)shift_vec.y, (double)shift_vec_scaled.y, 1.0);
            Assert::AreNotEqual(confidence, confidence_scaled, 0.05);
            Logger::WriteMessage("Done\n");
        }
    };
}
