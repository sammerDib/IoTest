#include <exception>
#include <fstream>
#include <gtest/gtest.h>
#include <iostream>
#include <random>
#include <stdlib.h>

#include <BaseAlgos/1DSignalAnalysis.hpp>

namespace {
  class SpikeDetectorTest : public ::testing::Test {
  protected:
    SpikeDetectorTest() { srand(5); }

    virtual ~SpikeDetectorTest() {}

    virtual void SetUp() {}

    virtual void TearDown() {}

    double GetRandomNumber(double minimum, double maximum) {
      double f = (double)rand() / RAND_MAX;
      return f * (maximum - minimum) + minimum;
    }

    std::vector<double> createSignal(int signalSize, double signalMean, double signalDev, std::vector<std::pair<int, double>> peaks) {
      std::vector<double> signal;
      for (int i = 0; i < signalSize; i++) {
        // create the whole noise signal
        signal.push_back(GetRandomNumber(signalMean - signalDev, signalMean + signalDev));
      }
      for (int i = 0; i < peaks.size(); i++) {
        // create peaks
        if (peaks.at(i).first < signal.size()) {
          signal[peaks.at(i).first] = peaks.at(i).second;
        }
      }

      return signal;
    }

    std::vector<double> createSignalFromCSV(std::string filename) {
      std::string csv_path = TEST_DATA_PATH + std::string("1DSignal\\") + filename;
      std::ifstream datacsv(csv_path);
      std::string line;
      std::vector<double> parsedCsv;
      while (std::getline(datacsv, line)) {
        std::stringstream lineStream(line);
        std::string cell;
        std::vector<double> parsedRow;
        std::istream &tmp = std::getline(lineStream, cell, ',');
        std::istream &tmp2 = std::getline(lineStream, cell, ',');
        parsedCsv.push_back(std::stod(cell));
      }

      // Delete the last two values ​​(artefac peak at the end of the signal)
      parsedCsv.pop_back();
      parsedCsv.pop_back();

      return parsedCsv;
    }
  };
} // namespace

TEST_F(SpikeDetectorTest, Peak_detector_on_real_lise_signal) {
  // Given : real raw Lise signal with one reference peak and two nearly peaks on each signal parts + 1 drop between two signal parts
  std::vector<double> signal = createSignalFromCSV(std::string("LiseSignal_peaks(1445_9035_9152_23545_23660_31250)_drops(from_16084_to_16649).csv"));

  int peakNbOnGoingPartOfSignal = 3;
  int peakNbOnAllSignal = peakNbOnGoingPartOfSignal * 2;
  int dropNbOnAllSignal = 1; // drop between going and coming part of lise signal
  int totalSpikesNb = peakNbOnAllSignal + dropNbOnAllSignal;
  int refPeakPos = 1445;
  int firstPeakPos = 9035;
  int secondPeakPos = 9152;
  int startPositionOfDropBetweenTwoSignalParts = 16084;
  int endPositionOfDropBetweenTwoSignalParts = 16649;
  int secondPeakPosBis = 23545;
  int firstPeakPosBis = 23660;
  int refPeakPosBis = 31250;

  // When : Analyze this signal
  int sampleSize = 1000;
  double threshold = 10;
  double influence = 0.0;
  signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(signal, sampleSize, threshold, influence);

  // Then
  EXPECT_EQ(totalSpikesNb, signalAnalyzed.Spikes.size());
  EXPECT_EQ(refPeakPos, signalAnalyzed.Spikes.at(0).Index);
  EXPECT_EQ(firstPeakPos, signalAnalyzed.Spikes.at(1).Index);
  EXPECT_EQ(secondPeakPos, signalAnalyzed.Spikes.at(2).Index);
  EXPECT_TRUE(signalAnalyzed.Spikes.at(3).Index >= startPositionOfDropBetweenTwoSignalParts && signalAnalyzed.Spikes.at(3).Index <= endPositionOfDropBetweenTwoSignalParts);
  EXPECT_EQ(refPeakPosBis, signalAnalyzed.Spikes.at(6).Index);
  EXPECT_EQ(firstPeakPosBis, signalAnalyzed.Spikes.at(5).Index);
  EXPECT_EQ(secondPeakPosBis, signalAnalyzed.Spikes.at(4).Index);
}

TEST_F(SpikeDetectorTest, Peak_detector_detect_correct_peaks_position) {
  // Given : a signal with three peaks placed at precise positions
  int peakPos1 = 45;
  int peakPos2 = 55;
  int peakPos3 = 405;
  double peakAmplitude = 5;
  std::vector<std::pair<int, double>> peaks = {std::pair<int, double>(peakPos1, peakAmplitude), std::pair<int, double>(peakPos2, peakAmplitude), std::pair<int, double>(peakPos3, peakAmplitude)};
  int signalSize = 500;
  double signalMean = -5;
  double signalDev = 0.5;
  std::vector<double> input = createSignal(signalSize, signalMean, signalDev, peaks);

  // When : Analyze this signal
  int sampleSize = 30;
  double threshold = 5.0;
  double influence = 0.0;
  signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, sampleSize, threshold, influence);

  // Then : All peaks are detected at correct position
  EXPECT_EQ(peakPos1, signalAnalyzed.Spikes.at(0).Index);
  EXPECT_EQ(peakPos2, signalAnalyzed.Spikes.at(1).Index);
  EXPECT_EQ(peakPos3, signalAnalyzed.Spikes.at(2).Index);
}

TEST_F(SpikeDetectorTest, Peak_detector_detect_correct_peaks_amplitude) {
  // Given : a signal with three peaks placed at precise positions
  double peakAmplitude1 = 5;
  double peakAmplitude2 = 1;
  double peakAmplitude3 = 4;
  int peakPos1 = 45;
  int peakPos2 = 55;
  int peakPos3 = 405;
  std::vector<std::pair<int, double>> peaks = {std::pair<int, double>(peakPos1, peakAmplitude1), std::pair<int, double>(peakPos2, peakAmplitude2), std::pair<int, double>(peakPos3, peakAmplitude3)};
  int signalSize = 500;
  double signalMean = -5;
  double signalDev = 0.5;
  std::vector<double> input = createSignal(signalSize, signalMean, signalDev, peaks);

  // When : Analyze this signal
  int sampleSize = 30;
  double threshold = 5.0;
  double influence = 0.0;
  signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, sampleSize, threshold, influence);

  // Then : All peaks are detected at correct position
  EXPECT_EQ(peakAmplitude1, signalAnalyzed.Spikes.at(0).Value);
  EXPECT_EQ(peakAmplitude2, signalAnalyzed.Spikes.at(1).Value);
  EXPECT_EQ(peakAmplitude3, signalAnalyzed.Spikes.at(2).Value);
}

TEST_F(SpikeDetectorTest, Peak_detector_detect_positive_and_negative_peaks) {
  // Given : a signal with two positives peaks and one negative peak
  double peakAmplitude1 = 5;
  double peakAmplitude2 = -10;
  double peakAmplitude3 = 1;
  int peakPos1 = 45;
  int peakPos2 = 55;
  int peakPos3 = 405;
  std::vector<std::pair<int, double>> peaks = {std::pair<int, double>(peakPos1, peakAmplitude1), std::pair<int, double>(peakPos2, peakAmplitude2), std::pair<int, double>(peakPos3, peakAmplitude3)};
  int signalSize = 500;
  double signalMean = -5;
  double signalDev = 0.5;
  std::vector<double> input = createSignal(signalSize, signalMean, signalDev, peaks);

  // When : Analyze this signal
  int sampleSize = 30;
  double threshold = 5.0;
  double influence = 0.0;
  signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, sampleSize, threshold, influence);

  // Then : All peaks are detected
  EXPECT_EQ(3, signalAnalyzed.Spikes.size());
  EXPECT_EQ(peakAmplitude1, signalAnalyzed.Spikes.at(0).Value);
  EXPECT_EQ(peakAmplitude2, signalAnalyzed.Spikes.at(1).Value);
  EXPECT_EQ(peakPos1, signalAnalyzed.Spikes.at(0).Index);
  EXPECT_EQ(peakPos2, signalAnalyzed.Spikes.at(1).Index);
}

TEST_F(SpikeDetectorTest, Peak_detector_detect_peak_at_end_of_signal) {
  // Given : a signal with one peak at the end of signal
  int signalSize = 500;
  double signalMean = -5;
  double signalDev = 0.5;
  double peakAmplitude = 1;
  int peakPos = signalSize - 1;
  ;
  std::vector<std::pair<int, double>> peaks = {std::pair<int, double>(peakPos, peakAmplitude)};
  std::vector<double> input = createSignal(signalSize, signalMean, signalDev, peaks);

  // When : Analyze this signal
  int sampleSize = 30;
  double threshold = 5.0;
  double influence = 0.0;
  signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, sampleSize, threshold, influence);

  // Then : The final peak is detected
  EXPECT_EQ(1, signalAnalyzed.Spikes.size());
  EXPECT_EQ(peakPos, signalAnalyzed.Spikes.at(0).Index);
  EXPECT_EQ(peakAmplitude, signalAnalyzed.Spikes.at(0).Value);
}

TEST_F(SpikeDetectorTest, Peak_detector_detect_two_nearby_peaks) {
  // Given : a signal with two nearby peaks
  int signalSize = 500;
  double signalMean = -5;
  double signalDev = 0.5;
  double peakAmplitude = -1;
  double nearbyPeakAmplitude = -2;
  int peakPos = 100;
  int peakPosNearby = peakPos + 6;
  std::vector<std::pair<int, double>> peaks = {std::pair<int, double>(peakPos - 4, -3),
                                               std::pair<int, double>(peakPos - 3, -2),
                                               std::pair<int, double>(peakPos - 2, -2.5),
                                               std::pair<int, double>(peakPos - 1, -1.5),
                                               std::pair<int, double>(peakPos, peakAmplitude),
                                               std::pair<int, double>(peakPos + 1, -1.5),
                                               std::pair<int, double>(peakPos + 2, -2.1),
                                               std::pair<int, double>(peakPos + 3, -2),
                                               std::pair<int, double>(peakPos + 4, -4),
                                               std::pair<int, double>(peakPos + 5, nearbyPeakAmplitude),
                                               std::pair<int, double>(peakPosNearby, peakAmplitude)};
  std::vector<double> input = createSignal(signalSize, signalMean, signalDev, peaks);

  // When : Analyze this signal
  int sampleSize = 30;
  double threshold = 5.0;
  double influence = 0.0;
  signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, sampleSize, threshold, influence);

  // Then : The two nearby peaks are detected
  EXPECT_EQ(2, signalAnalyzed.Spikes.size());
  EXPECT_EQ(peakPos, signalAnalyzed.Spikes.at(0).Index);
  EXPECT_EQ(peakPosNearby, signalAnalyzed.Spikes.at(1).Index);
}

TEST_F(SpikeDetectorTest, Peak_detector_does_not_detect_peak_on_noisy_signal_whithout_significant_peak) {
  // Given : a strongly noise signal without peak
  std::vector<std::pair<int, double>> noPeaks = {};
  int signalSize = 500;
  double signalMean = -5;
  double signalDev = 10;
  std::vector<double> input = createSignal(signalSize, signalMean, signalDev, noPeaks);

  // When : Analyze this signal
  int sampleSize = 30;
  double threshold = 5.0;
  double influence = 0.0;
  signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, sampleSize, threshold, influence);

  // Then : No peak is detected
  EXPECT_EQ(0, signalAnalyzed.Spikes.size());
}

TEST_F(SpikeDetectorTest, Peak_detector_detect_peak_on_noisy_signal_whith_significant_peak) {
  // Given : a strongly noise signal with significant peak
  int signalSize = 500;
  double signalMean = -5;
  double signalDev = 10;
  double peakAmplitude = 25;
  int peakPos = 100;
  ;
  std::vector<std::pair<int, double>> peaks = {std::pair<int, double>(peakPos, peakAmplitude)};
  std::vector<double> input = createSignal(signalSize, signalMean, signalDev, peaks);

  // When : Analyze this signal
  int sampleSize = 30;
  double threshold = 5.0;
  double influence = 0.0;
  signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, sampleSize, threshold, influence);

  // Then : Significant peak is detected
  EXPECT_EQ(1, signalAnalyzed.Spikes.size());
  EXPECT_EQ(peakPos, signalAnalyzed.Spikes.at(0).Index);
  EXPECT_EQ(peakAmplitude, signalAnalyzed.Spikes.at(0).Value);
}

TEST_F(SpikeDetectorTest, Peak_detector_failed_if_signal_size_is_smaller_than_the_statistical_sample_needed_) {
  // Given : a signal smaller than the statistical sample needed
  int signalSize = 29;
  double signalMean = -5;
  double signalDev = 10;
  std::vector<std::pair<int, double>> peaks = {};
  std::vector<double> input = createSignal(signalSize, signalMean, signalDev, peaks);
  int sampleSize = 30;

  // When : Analyze this signal
  double threshold = 5.0;
  double influence = 0.0;

  // Then : Signal analysis failed
  EXPECT_THROW(signal_1D::SignalAnalysisByZScoreThresholding(input, sampleSize, threshold, influence), std::exception);
}

TEST_F(SpikeDetectorTest, Peak_detector_return_correct_means_when_signal_have_no_significant_spikes) {
  // Given : Input signal with no significante spikes
  double baseSignalMean = 1;
  double baseSignalStddev = 0.5;
  int lag = 4;
  double threshold = 10;
  double influence = 0;

  std::vector<double> input;
  for (int i = 0; i < 10; i++) {
    if (i % 2 == 0) {
      input.push_back(baseSignalMean + baseSignalStddev);
    } else {
      input.push_back(baseSignalMean - baseSignalStddev);
    }
  }

  // When : Analyze this signal
  signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, lag, threshold, influence);

  // Then : We obtains correct means
  EXPECT_EQ(input.size(), signalAnalyzed.MovingMeans.size());
  for (int i = 0; i < lag - 1; i++) {
    EXPECT_DOUBLE_EQ(0.0, signalAnalyzed.MovingMeans.at(i));
  }
  for (int i = lag - 1; i < signalAnalyzed.MovingMeans.size(); i++) {
    EXPECT_NEAR(baseSignalMean, signalAnalyzed.MovingMeans.at(i), 0.1);
  }
}

TEST_F(SpikeDetectorTest, Peak_detector_return_correct_means_when_signal_have_significant_spikes_and_not_influence_means) {
  // Given : Input signal with significante spikes and an influence of these spikes of 0
  double baseSignalMean = 0.2;
  int lag = 4;
  double threshold = 10;
  double influence = 0;

  std::vector<double> input;
  for (int i = 0; i < lag; i++) {
    input.push_back(baseSignalMean);
  }
  input.push_back(baseSignalMean * threshold * 2);
  input.push_back(baseSignalMean * threshold * 3);
  input.push_back(baseSignalMean * threshold * 4);
  input.push_back(baseSignalMean * threshold * 2);
  input.push_back(baseSignalMean);
  input.push_back(baseSignalMean);
  input.push_back(baseSignalMean * threshold * 2);
  input.push_back(baseSignalMean * threshold * 5);
  input.push_back(baseSignalMean * threshold * 2);

  // When : Analyze this signal
  signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, lag, threshold, influence);

  // Then : We obtains correct means
  EXPECT_EQ(input.size(), signalAnalyzed.MovingMeans.size());
  for (int i = 0; i < lag - 1; i++) {
    EXPECT_DOUBLE_EQ(0.0, signalAnalyzed.MovingMeans.at(i));
  }
  for (int i = lag - 1; i < signalAnalyzed.MovingMeans.size(); i++) {
    EXPECT_NEAR(baseSignalMean, signalAnalyzed.MovingMeans.at(i), 0.1);
  }
}

TEST_F(SpikeDetectorTest, Peak_detector_return_correct_stddev_when_signal_have_no_significant_spikes) {
  // Given : Input signal with no significante spikes
  double baseSignalMean = 0.5;
  double baseSignalStddev = 0.5;
  int lag = 4;
  double threshold = 10;
  double influence = 0;

  std::vector<double> input;
  for (int i = 0; i < 12; i++) {
    if (i % 2 == 0) {
      input.push_back(baseSignalMean + baseSignalStddev);
    } else {
      input.push_back(baseSignalMean - baseSignalStddev);
    }
  }

  // When : Analyze this signal
  signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, lag, threshold, influence);

  // Then : We obtains correct stddev
  EXPECT_EQ(input.size(), signalAnalyzed.MovingStddev.size());
  for (int i = 0; i < lag - 1; i++) {
    EXPECT_DOUBLE_EQ(0.0, signalAnalyzed.MovingStddev.at(i));
  }
  for (int i = lag - 1; i < signalAnalyzed.MovingStddev.size(); i++) {
    EXPECT_NEAR(baseSignalStddev, signalAnalyzed.MovingStddev.at(i), 0.1);
  }
}

TEST_F(SpikeDetectorTest, Peak_detector_return_correct_stddev_when_signal_have_significant_spikes_and_not_influence_means) {
  // Given : Input signal with significante spikes and an influence of these spikes of 0
  double baseSignalMean = 0.2;
  double baseSignalStddev = 0;
  int lag = 4;
  double threshold = 10;
  double influence = 0;

  std::vector<double> input;
  for (int i = 0; i < lag; i++) {
    input.push_back(baseSignalMean);
  }
  input.push_back(baseSignalMean * threshold * 2);
  input.push_back(baseSignalMean * threshold * 3);
  input.push_back(baseSignalMean * threshold * 4);
  input.push_back(baseSignalMean * threshold * 2);
  input.push_back(baseSignalMean);
  input.push_back(baseSignalMean);
  input.push_back(baseSignalMean * threshold * 2);
  input.push_back(baseSignalMean * threshold * 5);
  input.push_back(baseSignalMean * threshold * 2);

  // When : Analyze this signal
  signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, lag, threshold, influence);

  // Then : We obtains correct stddev
  EXPECT_EQ(input.size(), signalAnalyzed.MovingStddev.size());
  for (int i = 0; i < lag - 1; i++) {
    EXPECT_DOUBLE_EQ(0.0, signalAnalyzed.MovingStddev.at(i));
  }
  for (int i = lag - 1; i < signalAnalyzed.MovingStddev.size(); i++) {
    EXPECT_NEAR(baseSignalStddev, signalAnalyzed.MovingStddev.at(i), 0.1);
  }
}