#include <gtest/gtest.h>
#include <opencv2/opencv.hpp>

#include <Topography/GoldsteinUnwrap.hpp>
#include <Topography/PhaseMapping.hpp>
#include <Topography/QualGuidedUnwrap.hpp>
#include <Topography/ReliabilityHistUnwrap.hpp>

using namespace std;
using namespace cv;
using namespace phase_mapping;
using namespace phase_unwrapping;

namespace {
  class PhaseMappingTest : public ::testing::Test {
  protected:
    std::vector<cv::Mat> _interferoImgs90;

    int _cols = 300;
    int _rows = 10;
    int _max = 50;

    PhaseMappingTest() {}
    virtual ~PhaseMappingTest() {}

    virtual void SetUp() { _interferoImgs90 = create7PhaseShifted2DSignals(); }

    virtual void TearDown() {}

    std::vector<cv::Mat> create7PhaseShifted1DSignals(double phaseShift = CV_PI / 2) {
      cv::Mat wrappedRowValues(1, _cols, CV_32FC1);
      cv::Mat wrappedRowValues1(1, _cols, CV_32FC1);
      cv::Mat wrappedRowValues2(1, _cols, CV_32FC1);
      cv::Mat wrappedRowValues3(1, _cols, CV_32FC1);
      cv::Mat wrappedRowValues4(1, _cols, CV_32FC1);
      cv::Mat wrappedRowValues5(1, _cols, CV_32FC1);
      cv::Mat wrappedRowValues6(1, _cols, CV_32FC1);
      cv::Mat wrappedRowValues7(1, _cols, CV_32FC1);

      for (int i = 0; i < _cols; ++i) {
        float v = (float)i * (float)_max / (float)_cols;
        wrappedRowValues1.at<float>(0, i) = atan2(sin(v), cos(v));
        wrappedRowValues2.at<float>(0, i) = atan2(sin(v + CV_PI / 2), cos(v + CV_PI / 2));
        wrappedRowValues3.at<float>(0, i) = atan2(sin(v + 2 * CV_PI / 2), cos(v + 2 * CV_PI / 2));
        wrappedRowValues4.at<float>(0, i) = atan2(sin(v + 3 * CV_PI / 2), cos(v + 3 * CV_PI / 2));
        wrappedRowValues5.at<float>(0, i) = atan2(sin(v + 4 * CV_PI / 2), cos(v + 4 * CV_PI / 2));
        wrappedRowValues6.at<float>(0, i) = atan2(sin(v + 5 * CV_PI / 2), cos(v + 5 * CV_PI / 2));
        wrappedRowValues7.at<float>(0, i) = atan2(sin(v + 6 * CV_PI / 2), cos(v + 6 * CV_PI / 2));
      }

      std::vector<cv::Mat> phaseShifted1DSignal{wrappedRowValues1, wrappedRowValues2, wrappedRowValues3, wrappedRowValues4, wrappedRowValues5, wrappedRowValues6, wrappedRowValues7};
      return phaseShifted1DSignal;
    }

    std::vector<cv::Mat> create7PhaseShifted2DSignals(double phaseShift = CV_PI / 2) {
      std::vector<cv::Mat> phaseShifted1DSignal = create7PhaseShifted1DSignals();

      cv::Mat interferogram1(_rows, _cols, CV_32FC1);
      cv::Mat interferogram2(_rows, _cols, CV_32FC1);
      cv::Mat interferogram3(_rows, _cols, CV_32FC1);
      cv::Mat interferogram4(_rows, _cols, CV_32FC1);
      cv::Mat interferogram5(_rows, _cols, CV_32FC1);
      cv::Mat interferogram6(_rows, _cols, CV_32FC1);
      cv::Mat interferogram7(_rows, _cols, CV_32FC1);

      for (int i = 0; i < _rows; ++i) {
        phaseShifted1DSignal[0].row(0).copyTo(interferogram1.row(i));
        phaseShifted1DSignal[1].row(0).copyTo(interferogram2.row(i));
        phaseShifted1DSignal[2].row(0).copyTo(interferogram3.row(i));
        phaseShifted1DSignal[3].row(0).copyTo(interferogram4.row(i));
        phaseShifted1DSignal[4].row(0).copyTo(interferogram5.row(i));
        phaseShifted1DSignal[5].row(0).copyTo(interferogram6.row(i));
        phaseShifted1DSignal[6].row(0).copyTo(interferogram7.row(i));
      }

      std::vector<cv::Mat> phaseShifted2DSignal{interferogram1, interferogram2, interferogram3, interferogram4, interferogram5, interferogram6, interferogram7};
      return phaseShifted2DSignal;
    }
  };
}; // namespace

TEST_F(PhaseMappingTest, hariharan_7steps_phase_mapping) {
  // Given : 7 phase-shifted interferograms of a constant and increasing pi/2 phase

  // When : Compute phase map
  cv::Mat phaseMap;
  cv::Mat intensityMap;
  cv::Mat backgroundMap;
  PhaseMapping(_interferoImgs90, phaseMap, intensityMap, backgroundMap);
  cv::Mat wrappedPhaseMap = phaseMap + backgroundMap;
  cv::normalize(wrappedPhaseMap, wrappedPhaseMap, -CV_PI, CV_PI, cv::NORM_MINMAX);

  // Then : wrapped phase values ​​correspond to expected wrapped phase values
  bool correspondanceFounded = false;
  for (int index = 0; index < _interferoImgs90.size(); ++index) {
    float dist = _interferoImgs90.at(index).at<float>(0, 0) - wrappedPhaseMap.at<float>(0, 0);
    if (abs(dist) <= 0.1) {
      correspondanceFounded = true;
      for (int i = 0; i < _rows; ++i) {
        for (int j = 0; j < _cols; ++j) {
          EXPECT_NEAR(_interferoImgs90.at(index).at<float>(i, j), wrappedPhaseMap.at<float>(i, j), 0.1);
        }
      }
    }
  }
  EXPECT_TRUE(correspondanceFounded);

  // Display ----------------------------------------------------------------------

  // cv::imshow("Interferogram 1", _interferoImgs90[0]);
  // cv::imshow("Interferogram 2", _interferoImgs90[1]);
  // cv::imshow("Interferogram 3", _interferoImgs90[2]);
  // cv::imshow("Interferogram 4", _interferoImgs90[3]);
  // cv::imshow("Interferogram 5", _interferoImgs90[4]);
  // cv::imshow("Interferogram 6", _interferoImgs90[5]);
  // cv::imshow("Interferogram 7", _interferoImgs90[6]);

  // cv::Mat ewphase;
  // cv::normalize(_expectedWrappedPhase, ewphase, 0, 1, NORM_MINMAX, CV_32FC1);
  // ewphase.convertTo(ewphase, CV_8U, 255);
  // cv::imshow("expectedWrappedPhase", ewphase);

  // cv::Mat wphase;
  // cv::normalize(wrappedPhaseMap, wphase, 0, 1, NORM_MINMAX, CV_32FC1);
  // wphase.convertTo(wphase, CV_8U, 255);
  // cv::imshow("wrappedPhase", wphase);

  // cv::waitKey();
}

TEST_F(PhaseMappingTest, hariharan_6steps_phase_mapping) {
  // Given : 6 phase-shifted interferograms of a constant and increasing pi/2 phase
  std::vector<cv::Mat> subInterferosImgs90 = {_interferoImgs90.begin(), _interferoImgs90.begin() + 6};

  // When : Compute phase map
  cv::Mat phaseMap;
  cv::Mat intensityMap;
  cv::Mat backgroundMap;
  PhaseMapping(subInterferosImgs90, phaseMap, intensityMap, backgroundMap);
  cv::Mat wrappedPhaseMap = phaseMap + backgroundMap;
  cv::normalize(wrappedPhaseMap, wrappedPhaseMap, -CV_PI, CV_PI, cv::NORM_MINMAX);

  // Then : wrapped phase values ​​correspond to expected wrapped phase values
  bool correspondanceFounded = false;
  for (int index = 0; index < _interferoImgs90.size(); ++index) {
    float dist = _interferoImgs90.at(index).at<float>(0, 0) - wrappedPhaseMap.at<float>(0, 0);
    if (abs(dist) <= 0.1) {
      correspondanceFounded = true;
      for (int i = 0; i < _rows; ++i) {
        for (int j = 0; j < _cols; ++j) {
          EXPECT_NEAR(_interferoImgs90.at(index).at<float>(i, j), wrappedPhaseMap.at<float>(i, j), 0.1);
        }
      }
    }
  }
  EXPECT_TRUE(correspondanceFounded);
}
TEST_F(PhaseMappingTest, hariharan_5steps_phase_mapping) {
  // Given : 5 phase-shifted interferograms of a constant and increasing pi/2 phase
  std::vector<cv::Mat> subInterferosImgs90 = {_interferoImgs90.begin(), _interferoImgs90.begin() + 5};

  // When : Compute phase map
  cv::Mat phaseMap;
  cv::Mat intensityMap;
  cv::Mat backgroundMap;
  PhaseMapping(subInterferosImgs90, phaseMap, intensityMap, backgroundMap);
  cv::Mat wrappedPhaseMap = phaseMap + backgroundMap;
  cv::normalize(wrappedPhaseMap, wrappedPhaseMap, -CV_PI, CV_PI, cv::NORM_MINMAX);

  // Then : wrapped phase values ​​correspond to expected wrapped phase values
  bool correspondanceFounded = false;
  for (int index = 0; index < _interferoImgs90.size(); ++index) {
    float dist = _interferoImgs90.at(index).at<float>(0, 0) - wrappedPhaseMap.at<float>(0, 0);
    if (abs(dist) <= 0.1) {
      correspondanceFounded = true;
      for (int i = 0; i < _rows; ++i) {
        for (int j = 0; j < _cols; ++j) {
          EXPECT_NEAR(_interferoImgs90.at(index).at<float>(i, j), wrappedPhaseMap.at<float>(i, j), 0.1);
        }
      }
    }
  }
  EXPECT_TRUE(correspondanceFounded);
}

TEST_F(PhaseMappingTest, hariharan_4steps_phase_mapping) {
  // Given : 4 phase-shifted interferograms of a constant and increasing pi/2 phase
  std::vector<cv::Mat> subInterferosImgs90 = {_interferoImgs90.begin(), _interferoImgs90.begin() + 4};

  // When : Compute phase map
  cv::Mat phaseMap;
  cv::Mat intensityMap;
  cv::Mat backgroundMap;
  PhaseMapping(subInterferosImgs90, phaseMap, intensityMap, backgroundMap);
  cv::Mat wrappedPhaseMap = phaseMap + backgroundMap;
  cv::normalize(wrappedPhaseMap, wrappedPhaseMap, -CV_PI, CV_PI, cv::NORM_MINMAX);

  // Then : wrapped phase values ​​correspond to expected wrapped phase values
  bool correspondanceFounded = false;
  for (int index = 0; index < _interferoImgs90.size(); ++index) {
    float dist = _interferoImgs90.at(index).at<float>(0, 0) - wrappedPhaseMap.at<float>(0, 0);
    if (abs(dist) <= 0.1) {
      correspondanceFounded = true;
      for (int i = 0; i < _rows; ++i) {
        for (int j = 0; j < _cols; ++j) {
          EXPECT_NEAR(_interferoImgs90.at(index).at<float>(i, j), wrappedPhaseMap.at<float>(i, j), 0.1);
        }
      }
    }
  }
  EXPECT_TRUE(correspondanceFounded);
}

TEST_F(PhaseMappingTest, hariharan_3steps_phase_mapping) {
  // Given : 4 phase-shifted interferograms of a constant and increasing pi/2 phase
  std::vector<cv::Mat> subInterferosImgs90 = {_interferoImgs90.begin(), _interferoImgs90.begin() + 3};

  // When : Compute phase map
  cv::Mat phaseMap;
  cv::Mat intensityMap;
  cv::Mat backgroundMap;
  PhaseMapping(subInterferosImgs90, phaseMap, intensityMap, backgroundMap);
  cv::Mat wrappedPhaseMap = phaseMap + backgroundMap;
  cv::normalize(wrappedPhaseMap, wrappedPhaseMap, -CV_PI, CV_PI, cv::NORM_MINMAX);

  // Then : wrapped phase values ​​correspond to expected wrapped phase values
  bool correspondanceFounded = false;
  for (int index = 0; index < _interferoImgs90.size(); ++index) {
    float dist = _interferoImgs90.at(index).at<float>(0, 0) - wrappedPhaseMap.at<float>(0, 0);
    if (abs(dist) <= 0.1) {
      correspondanceFounded = true;
      for (int i = 0; i < _rows; ++i) {
        for (int j = 0; j < _cols; ++j) {
          EXPECT_NEAR(_interferoImgs90.at(index).at<float>(i, j), wrappedPhaseMap.at<float>(i, j), 0.1);
        }
      }
    }
  }
  EXPECT_TRUE(correspondanceFounded);
}

TEST_F(PhaseMappingTest, average_interferograms) {
  // Given : A constant number of interferograms at each step
  std::vector<cv::Mat> interferogramsToAverage;
  int imageNbPerStep = 10;
  int stepNb = 7;
  for (int step = 0; step < stepNb; step++) {
    for (int index = 0; index < imageNbPerStep; index++) {
      interferogramsToAverage.push_back(_interferoImgs90.at(step));
    }
  }

  // When : We average the interferograms corresponding to the same step, to have only one average interferogram for each step
  std::vector<cv::Mat> averagedInterferosImgs = AverageImgs(interferogramsToAverage, stepNb);

  // Then :Final interferograms ​​correspond to mean interferogram for each step
  for (int step = 0; step < stepNb; step++) {
    for (int i = 0; i < _rows; ++i) {
      for (int j = 0; j < _cols; ++j) {
        EXPECT_NEAR(_interferoImgs90.at(step).at<float>(i, j), averagedInterferosImgs.at(step).at<float>(i, j), 0.1);
      }
    }
  }
}