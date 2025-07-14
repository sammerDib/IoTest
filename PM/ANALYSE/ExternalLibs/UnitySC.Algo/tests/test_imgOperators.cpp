#include <exception>
#include <gtest/gtest.h>
#include <iostream>
#include <opencv2/opencv.hpp>
#include <stdlib.h>
#include <vector>

#include <BaseAlgos/ImageOperators.hpp>

namespace {
  class ImgOperatorTest : public ::testing::Test {
  protected:
    ImgOperatorTest() {}

    virtual ~ImgOperatorTest() {}

    virtual void SetUp() {
      std::string image_path = TEST_DATA_PATH + std::string("notch.png");
      ref_img = cv::imread(image_path);
      if (ref_img.empty()) {
        std::cout << "Could not read the image: " << image_path << std::endl;
      }

      EXPECT_EQ(false, ref_img.empty());
    }

    virtual void TearDown() {}

    cv::Mat ref_img;
  };
} // namespace

TEST_F(ImgOperatorTest, normalized_gray_level_variance_decreases_when_the_image_is_overexposed_or_underexposed) {
  // Given: images good exposed, overexposed and underexposed
  cv::Mat img = cv::imread(TEST_DATA_PATH + std::string("notch.png"));
  cv::Mat img_too_light = cv::imread(TEST_DATA_PATH + std::string("notch_too_light.png"));
  cv::Mat img_too_dark = cv::imread(TEST_DATA_PATH + std::string("notch_too_dark.png"));
  // When: compute contrast with normalized gray level variance
  double good_exposed = img_operators::NormalizedGraylevelVariance(img);
  double over_exposed = img_operators::NormalizedGraylevelVariance(img_too_light);
  double under_exposed = img_operators::NormalizedGraylevelVariance(img_too_dark);
  // Then: contrast score measured decrease when the image is overexposed or underexposed
  EXPECT_TRUE(good_exposed > over_exposed && good_exposed > under_exposed);
}

TEST_F(ImgOperatorTest, focus_measurement_decreases_as_image_blur_increases) {
  // Given: an image more and more blured
  std::vector<cv::Mat> blured_imgs;
  int blur_coef = 0;
  std::vector<int> odd_kernel_size = {1, 3, 5, 7, 9};
  for (int kernel_size : odd_kernel_size) {
    cv::Mat blured_img;
    cv::GaussianBlur(ref_img, blured_img, cv::Size(kernel_size, kernel_size), 0);
    blured_imgs.push_back(blured_img);
  }

  // When : calculate the focus on the images, from the least blurred to the most blurred, with all algorithms
  std::vector<double> focus_scores_with_LAPV;
  std::vector<double> focus_scores_with_LAPM;
  std::vector<double> focus_scores_with_TENG;
  for (int i = 0; i < blured_imgs.size(); i++) {
    focus_scores_with_LAPV.push_back(img_operators::VarianceOfLaplacian(blured_imgs.at(i)));
    focus_scores_with_LAPM.push_back(img_operators::ModifiedLaplacian(blured_imgs.at(i)));
    focus_scores_with_TENG.push_back(img_operators::TenenbaumGradient(blured_imgs.at(i)));
  }

  // Then : focus measurement decreases as image blur increases

  // LAPV algorithm
  double previous_score = std::numeric_limits<double>::infinity();
  for (int i = 0; i < focus_scores_with_LAPV.size(); i++) {
    std::cout << focus_scores_with_LAPV.at(i) << std::endl;
    EXPECT_TRUE(previous_score > focus_scores_with_LAPV.at(i));
    previous_score = focus_scores_with_LAPV.at(i);
  }

  // LAPM algorithm
  previous_score = std::numeric_limits<double>::infinity();
  for (int i = 0; i < focus_scores_with_LAPM.size(); i++) {
    std::cout << focus_scores_with_LAPM.at(i) << std::endl;
    EXPECT_TRUE(previous_score > focus_scores_with_LAPM.at(i));
    previous_score = focus_scores_with_LAPM.at(i);
  }

  // TENG algorithm
  previous_score = std::numeric_limits<double>::infinity();
  for (int i = 0; i < focus_scores_with_TENG.size(); i++) {
    std::cout << focus_scores_with_TENG.at(i) << std::endl;
    EXPECT_TRUE(previous_score > focus_scores_with_TENG.at(i));
    previous_score = focus_scores_with_TENG.at(i);
  }
}

TEST_F(ImgOperatorTest, saturation_increases_from_0_to_1_when_lightness_increases) {
  // Given: completely black, intermediate and completely white images
  std::vector<cv::Mat> imgs;
  cv::Mat black_img(10, 10, CV_8UC3, cv::Scalar(0, 0, 0));
  cv::Mat intermediate_img = ref_img;
  cv::Mat white_img(10, 10, CV_8UC3, cv::Scalar(250, 250, 250));

  // When : calculate the saturation on the images, from the least lighted to the most lighted
  double score_on_black_image = img_operators::Saturation(black_img);
  double score_on_intermediate_image = img_operators::Saturation(intermediate_img);
  double score_on_white_image = img_operators::Saturation(white_img);

  // Then : saturation measurement increases from 0 (completely black image) to 1 (completely white image)
  EXPECT_TRUE(score_on_black_image == 0);
  EXPECT_TRUE(score_on_black_image < score_on_intermediate_image);
  EXPECT_TRUE(score_on_intermediate_image < score_on_white_image);
  EXPECT_TRUE(score_on_white_image == 1);
}