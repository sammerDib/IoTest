#include <gtest/gtest.h>
#include <opencv2/opencv.hpp>

#include <Topography/GoldsteinUnwrap.hpp>
#include <Topography/QualGuidedUnwrap.hpp>
#include <Topography/ReliabilityHistUnwrap.hpp>

using namespace std;
using namespace cv;
using namespace phase_unwrapping;

namespace {
  class PhaseUnwrappingTest : public ::testing::Test {
  protected:
    cv::Mat _expectedUnwrappedPhase;
    cv::Mat _wrappedPhase;
    int _cols = 300;
    int _rows = 10;
    int _max = 50;

    PhaseUnwrappingTest() {}
    virtual ~PhaseUnwrappingTest() {}

    virtual void SetUp() {
      _expectedUnwrappedPhase.create(_rows, _cols, CV_32FC1);
      _wrappedPhase.create(_rows, _cols, CV_32FC1);
      create2DPhaseMaps(_wrappedPhase, _expectedUnwrappedPhase, _cols, _rows, _max);
    }

    virtual void TearDown() {}

    void create2DPhaseMaps(cv::Mat &wrappedPhase, cv::Mat &expectedUnwrappedPhase, int colsNb, int rowsNb, int maxValue) {
      cv::Mat rowValues(1, colsNb, CV_32FC1);
      cv::Mat wrappedRowValues(1, colsNb, CV_32FC1);
      // create 1D sine wave
      for (int i = 0; i < colsNb; ++i) {
        float v = (float)i * (float)maxValue / (float)colsNb;
        rowValues.at<float>(0, i) = v;
        wrappedRowValues.at<float>(0, i) = atan2(sin(v), cos(v));
        // create 2D phase map by duplicating this 1D signal into each row of the matrix
        for (int i = 0; i < rowsNb; ++i) {
          rowValues.row(0).copyTo(expectedUnwrappedPhase.row(i));
          wrappedRowValues.row(0).copyTo(wrappedPhase.row(i));
        }
      }
    }
  };
} // namespace

TEST_F(PhaseUnwrappingTest, reliability_histogram_phase_unwrapping) {
  // Given : wrapped phase map and its expected unwrapped phase

  // When : unwrap phase map with an algorithm based on histogram processing of reliability
  phase_unwrapping::ReliabilityHistUnwrap phase_unwraper;
  cv::Mat unwrappedPhase = phase_unwraper.UnwrapPhaseMap(_wrappedPhase, cv::Mat());

  // Then : unwrapped values ​​correspond to expected unwrapped values
  for (int i = 0; i < _rows; ++i) {
    for (int j = 0; j < _cols; ++j) {
      EXPECT_NEAR(_expectedUnwrappedPhase.at<float>(i, j), unwrappedPhase.at<float>(i, j), 0.001);
    }
  }

  // Display ----------------------------------------------------------------------
  // cv::Mat wphase;
  // cv::normalize(_wrappedPhase, wphase, 0, 1, NORM_MINMAX, CV_32FC1);
  // wphase.convertTo(wphase, CV_8U, 255);
  // cv::imshow("wrappedPhase", wphase);

  // cv::Mat reliabilityMap = phase_unwraper.getInverseReliabilityMap();
  // cv::Mat reliability;
  // if (reliabilityMap.type() == CV_32FC1) {
  //  cv::normalize(reliabilityMap, reliability, 0, 1, NORM_MINMAX, CV_32FC1);
  //  reliability.convertTo(reliability, CV_8U, 255);
  //  cv::imshow("Reliability Map", reliability);
  //}

  // cv::Mat uphase;
  // cv::normalize(_expectedUnwrappedPhase, uphase, 0, 1, NORM_MINMAX, CV_32FC1);
  // uphase.convertTo(uphase, CV_8U, 255);
  // cv::imshow("expectedUnWrappedPhase", uphase);

  // cv::Mat phase;
  // cv::normalize(unwrappedPhase, phase, 0, 1, NORM_MINMAX, CV_32FC1);
  // phase.convertTo(phase, CV_8U, 255);
  // cv::imshow("UnWrappedPhase", phase);
  // cv::waitKey();
}

TEST_F(PhaseUnwrappingTest, goldstein_phase_unwrapping) {
  // Given : wrapped phase map and its expected unwrapped phase

  // When : unwrap phase map with golstein algorithm
  cv::Mat unwrappedPhase = GoldsteinUnwrap(_wrappedPhase).UnwrappedPhase;

  // Then : unwrapped values ​​correspond to expected unwrapped values (except for borders which are no computed)
  for (int i = 1; i < _rows - 1; ++i) {
    for (int j = 1; j < _cols - 1; ++j) {
      EXPECT_NEAR(_expectedUnwrappedPhase.at<float>(i, j), unwrappedPhase.at<float>(i, j), 0.001);
    }
  }
}


/*
TEST_F(PhaseUnwrappingTest, gradient_quality_guided_phase_unwrapping) {
  // Given : wrapped phase map and its expected unwrapped phase

  // When : unwrap phase map with an quality guided algorithm
  cv::Mat unwrappedPhase = QualityGuidedUnwrap(_wrappedPhase, QualityMode::Gradient);

  // Then : unwrapped values ​​correspond to expected unwrapped values
  for (int i = 0; i < _rows; ++i) {
    for (int j = 0; j < _cols; ++j) {
      EXPECT_NEAR(_expectedUnwrappedPhase.at<float>(i, j), unwrappedPhase.at<float>(i, j), 0.001);
    }
  }
}

TEST_F(PhaseUnwrappingTest, variance_quality_guided_phase_unwrapping) {
  // Given : wrapped phase map and its expected unwrapped phase

  // When : unwrap phase map with an quality guided algorithm
  cv::Mat unwrappedPhase = QualityGuidedUnwrap(_wrappedPhase, QualityMode::Variance);

  // Then : unwrapped values ​​correspond to expected unwrapped values
  for (int i = 0; i < _rows; ++i) {
    for (int j = 0; j < _cols; ++j) {
      EXPECT_NEAR(_expectedUnwrappedPhase.at<float>(i, j), unwrappedPhase.at<float>(i, j), 0.001);
    }
  }
}

TEST_F(PhaseUnwrappingTest, pseudo_correlation_quality_guided_phase_unwrapping) {
  // Given : wrapped phase map and its expected unwrapped phase

  // When : unwrap phase map with an quality guided algorithm
  cv::Mat unwrappedPhase = QualityGuidedUnwrap(_wrappedPhase, QualityMode::PseudoCorrelation);

  // Then : unwrapped values ​​correspond to expected unwrapped values
  for (int i = 0; i < _rows; ++i) {
    for (int j = 0; j < _cols; ++j) {
      EXPECT_NEAR(_expectedUnwrappedPhase.at<float>(i, j), unwrappedPhase.at<float>(i, j), 0.001);
    }
  }
}
*/