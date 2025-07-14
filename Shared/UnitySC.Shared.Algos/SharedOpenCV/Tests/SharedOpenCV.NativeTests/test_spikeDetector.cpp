#include "CppUnitTest.h"
#include <exception>
#include <fstream>
#include <iostream>
#include <random>
#include <stdlib.h>

#include <chrono>
using namespace std::chrono;

#include "C1DSignalAnalysis.hpp"

#pragma unmanaged
using namespace Microsoft::VisualStudio::CppUnitTestFramework;

#define TEST_DATA_PATH std::string(".\\..\\..\\Tests\\Data\\")


namespace SharedOpenCVNativeTests
{
    TEST_CLASS(SpikeDetectorTests)
    {
    private:

        double GetRandomNumber(double minimum, double maximum)
        {
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
            std::string csv_path = TEST_DATA_PATH + filename;
            std::ifstream datacsv(csv_path);
            std::string line;
            std::vector<double> parsedCsv;
            while (std::getline(datacsv, line)) {
                std::stringstream lineStream(line);
                std::string cell;
                std::vector<double> parsedRow;
                std::istream& tmp = std::getline(lineStream, cell, ',');
                std::istream& tmp2 = std::getline(lineStream, cell, ',');
                parsedCsv.push_back(std::stod(cell));
            }

            Assert::IsTrue(parsedCsv.size() > 3, L"Signal file Empty or too short");

            // Delete the last two values ??(artefac peak at the end of the signal)
            parsedCsv.pop_back();
            parsedCsv.pop_back();

            return parsedCsv;
        }

    public:

        TEST_CLASS_INITIALIZE(SpikeDetectorInitialize)
        {
            Logger::WriteMessage("SpikeDetectorTests Initialize");
            srand(565); // init random feed
        }

        TEST_METHOD(_01_Peak_detector_on_real_lise_signal)
        {
            Logger::WriteMessage("Peak_detector_on_real_lise_signal\n");

            // Given : real raw Lise signal with one reference peak and two nearly peaks on each signal parts + 1 drop between two signal parts
            std::vector<double> signal = createSignalFromCSV(std::string("LiseSignal_peaks(1445_9035_9152_23545_23660_31250)_drops(from_16084_to_16649).csv"));

            int peakNbOnGoingPartOfSignal = 3;
            int peakNbOnAllSignal = peakNbOnGoingPartOfSignal * 2;
            int dropNbOnAllSignal = 1; // drop between going and coming part of lise signal
            int totalSpikesNb = peakNbOnAllSignal + dropNbOnAllSignal;

            size_t refPeakPos = 1445;
            size_t firstPeakPos = 9035;
            size_t secondPeakPos = 9152;
            size_t startPositionOfDropBetweenTwoSignalParts = 16084;
            size_t endPositionOfDropBetweenTwoSignalParts = 16649;
            size_t secondPeakPosBis = 23545;
            size_t firstPeakPosBis = 23660;
            size_t refPeakPosBis = 31250;

            // When : Analyze this signal
            int sampleSize = 1000;
            double threshold = 10.0;
            double influence = 0.0;

            Logger::WriteMessage("perform signal analysis\n");
            double occ = 1.0;
            auto start = high_resolution_clock::now();
            signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(signal, sampleSize, threshold, influence);
            if (occ > 1.0)
            {
                for (int i = 1; i < 100; i++)
                {
                    auto signalAnalyzed2 = signal_1D::SignalAnalysisByZScoreThresholding(signal, sampleSize, threshold, influence);
                }
            }
            auto stop = high_resolution_clock::now();
            auto duration = duration_cast<std::chrono::milliseconds>(stop - start);
            double duration_ms = duration.count() / occ;

            std::ostringstream ossexc;
            ossexc << "==> Exec time is <" << duration_ms << "> ms\n";
            Logger::WriteMessage(ossexc.str().c_str());

#ifdef _DEBUG
            std::ostringstream oss;
            oss << "i;value;index;type (1=peak);Mov Mean; Mov Std;\n";
            size_t simax = std::max(signalAnalyzed.Spikes.size(), signalAnalyzed.MovingMeans.size());
            simax = std::max(simax, signalAnalyzed.MovingStddev.size());
            for (int i = 0; i < simax; i++)
            {
                oss << i << ";";
                if (i < signalAnalyzed.Spikes.size())
                    oss << signalAnalyzed.Spikes[i].Value << ";" << signalAnalyzed.Spikes[i].Index << ";" << (int)signalAnalyzed.Spikes[i].Type << ";";
                else
                    oss << ";;;";
                if (i < signalAnalyzed.MovingMeans.size())
                    oss << signalAnalyzed.MovingMeans[i] << ";";
                else
                    oss << ";";
                if (i < signalAnalyzed.MovingStddev.size())
                    oss << signalAnalyzed.MovingStddev[i] << ";";
                else
                    oss << ";";
                oss << "\n";
            }
            std::ofstream outFile("C:\\Work\\Data\\Temp\\signallisTest01analyzed.csv");  // to change in debug for analysis purpose
            outFile << oss.str();
            outFile.close();
#endif // _DEBUG

            // Then
            Assert::AreEqual(totalSpikesNb, (int)signalAnalyzed.Spikes.size());
            Assert::AreEqual(refPeakPos, signalAnalyzed.Spikes.at(0).Index);
            Assert::AreEqual(firstPeakPos, signalAnalyzed.Spikes.at(1).Index);
            Assert::AreEqual(secondPeakPos, signalAnalyzed.Spikes.at(2).Index);
            Assert::IsTrue(signalAnalyzed.Spikes.at(3).Index >= startPositionOfDropBetweenTwoSignalParts && signalAnalyzed.Spikes.at(3).Index <= endPositionOfDropBetweenTwoSignalParts);
            Assert::AreEqual(refPeakPosBis, signalAnalyzed.Spikes.at(6).Index);
            Assert::AreEqual(firstPeakPosBis, signalAnalyzed.Spikes.at(5).Index);
            Assert::AreEqual(secondPeakPosBis, signalAnalyzed.Spikes.at(4).Index);

            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_02_Peak_detector_detect_correct_peaks_position)
        {
            Logger::WriteMessage("Peak_detector_detect_correct_peaks_position\n");

            // Given : a signal with three peaks placed at precise positions
            size_t peakPos1 = 45;
            size_t peakPos2 = 55;
            size_t peakPos3 = 405;
            double peakAmplitude = 5;
            std::vector<std::pair<int, double>> peaks = { std::pair<int, double>(peakPos1, peakAmplitude), std::pair<int, double>(peakPos2, peakAmplitude), std::pair<int, double>(peakPos3, peakAmplitude) };
            int signalSize = 500;
            double signalMean = -5;
            double signalDev = 0.5;
            Logger::WriteMessage("Create random signa with known peaks\n");
            std::vector<double> input = createSignal(signalSize, signalMean, signalDev, peaks);

            // When : Analyze this signal
            int sampleSize = 30;
            double threshold = 5.0;
            double influence = 0.0;

            Logger::WriteMessage("perform signal analysis\n");
            signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, sampleSize, threshold, influence);

#ifdef _DEBUG
            std::ostringstream oss;
            oss << "i;value;index;type (1=peak);Mov Mean; Mov Std;Input;\n";
            size_t simax = std::max(signalAnalyzed.Spikes.size(), signalAnalyzed.MovingMeans.size());
            simax = std::max(simax, signalAnalyzed.MovingStddev.size());
            for (int i = 0; i < simax; i++)
            {
                oss << i << ";";
                if (i < signalAnalyzed.Spikes.size())
                    oss << signalAnalyzed.Spikes[i].Value << ";" << signalAnalyzed.Spikes[i].Index << ";" << (int)signalAnalyzed.Spikes[i].Type << ";";
                else
                    oss << ";;;";
                if (i < signalAnalyzed.MovingMeans.size())
                    oss << signalAnalyzed.MovingMeans[i] << ";";
                else
                    oss << ";";
                if (i < signalAnalyzed.MovingStddev.size())
                    oss << signalAnalyzed.MovingStddev[i] << ";";
                else
                    oss << ";";
                if (i < input.size())
                    oss << input[i] << ";";
                else
                    oss << ";";
                oss << "\n";
            }
            std::ofstream outFile("C:\\Work\\Data\\Temp\\signallisTest02analyzed.csv"); // to change in debug for analysis purpose
            outFile << oss.str();
            outFile.close();
#endif //_DEBUG

            // Then : All peaks are detected at correct position
            Assert::AreEqual(peakPos1, signalAnalyzed.Spikes.at(0).Index);
            Assert::AreEqual(peakPos2, signalAnalyzed.Spikes.at(1).Index);
            Assert::AreEqual(peakPos3, signalAnalyzed.Spikes.at(2).Index);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_03_Peak_detector_detect_correct_peaks_amplitude)
        {
            Logger::WriteMessage("\n");
            // Given : a signal with three peaks placed at precise positions
            double peakAmplitude1 = 5;
            double peakAmplitude2 = 1;
            double peakAmplitude3 = 4;
            int peakPos1 = 45;
            int peakPos2 = 55;
            int peakPos3 = 405;
            std::vector<std::pair<int, double>> peaks = { std::pair<int, double>(peakPos1, peakAmplitude1), std::pair<int, double>(peakPos2, peakAmplitude2), std::pair<int, double>(peakPos3, peakAmplitude3) };
            int signalSize = 500;
            double signalMean = -5;
            double signalDev = 0.5;
            Logger::WriteMessage("Create random signal with known peaks\n");
            std::vector<double> input = createSignal(signalSize, signalMean, signalDev, peaks);

            // When : Analyze this signal
            int sampleSize = 30;
            double threshold = 5.0;
            double influence = 0.0;
            Logger::WriteMessage("perform signal analysis\n");
            signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, sampleSize, threshold, influence);

            // Then : All peaks are detected at correct position
            Assert::AreEqual(peakAmplitude1, signalAnalyzed.Spikes.at(0).Value);
            Assert::AreEqual(peakAmplitude2, signalAnalyzed.Spikes.at(1).Value);
            Assert::AreEqual(peakAmplitude3, signalAnalyzed.Spikes.at(2).Value);

            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_04_Peak_detector_detect_positive_and_negative_peaks)
        {
            Logger::WriteMessage("Peak_detector_detect_positive_and_negative_peaks \n");
            // Given : a signal with two positives peaks and one negative peak
            double peakAmplitude1 = 5;
            double peakAmplitude2 = -10;
            double peakAmplitude3 = 1;
            int peakPos1 = 45;
            int peakPos2 = 55;
            int peakPos3 = 405;
            std::vector<std::pair<int, double>> peaks = { std::pair<int, double>(peakPos1, peakAmplitude1), std::pair<int, double>(peakPos2, peakAmplitude2), std::pair<int, double>(peakPos3, peakAmplitude3) };
            int signalSize = 500;
            double signalMean = -5;
            double signalDev = 0.5;
            Logger::WriteMessage("Create random signal with known peaks\n");
            std::vector<double> input = createSignal(signalSize, signalMean, signalDev, peaks);

            // When : Analyze this signal
            int sampleSize = 30;
            double threshold = 5.0;
            double influence = 0.0;
            Logger::WriteMessage("perform signal analysis\n");
            signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, sampleSize, threshold, influence);

            // Then : All peaks are detected
             Assert::AreEqual(3, (int) signalAnalyzed.Spikes.size());
             Assert::AreEqual(peakAmplitude1, signalAnalyzed.Spikes.at(0).Value);
             Assert::AreEqual(peakAmplitude2, signalAnalyzed.Spikes.at(1).Value);
             Assert::AreEqual(peakPos1, (int)signalAnalyzed.Spikes.at(0).Index);
             Assert::AreEqual(peakPos2, (int)signalAnalyzed.Spikes.at(1).Index);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_05_Peak_detector_detect_peak_at_end_of_signal)
        {
            Logger::WriteMessage("Peak_detector_detect_peak_at_end_of_signal\n");
            // Given : a signal with one peak at the end of signal
            int signalSize = 500;
            double signalMean = -5;
            double signalDev = 0.5;
            double peakAmplitude = 1;
            int peakPos = signalSize - 1;
            std::vector<std::pair<int, double>> peaks = { std::pair<int, double>(peakPos, peakAmplitude) };

            Logger::WriteMessage("Create random signal with known peaks\n");
            std::vector<double> input = createSignal(signalSize, signalMean, signalDev, peaks);

            // When : Analyze this signal
            int sampleSize = 30;
            double threshold = 5.0;
            double influence = 0.0;
            Logger::WriteMessage("perform signal analysis\n");
            signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, sampleSize, threshold, influence);

            // Then : The final peak is detected
            Assert::AreEqual(1, (int)signalAnalyzed.Spikes.size());
            Assert::AreEqual(peakPos, (int)signalAnalyzed.Spikes.at(0).Index);
            Assert::AreEqual(peakAmplitude, signalAnalyzed.Spikes.at(0).Value);

            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_06_Peak_detector_detect_two_nearby_peaks)
        {
            Logger::WriteMessage("Peak_detector_detect_two_nearby_peaks\n");
            // Given : a signal with two nearby peaks
            int signalSize = 500;
            double signalMean = -5;
            double signalDev = 0.5;
            double peakAmplitude = -1;
            double nearbyPeakAmplitude = -2;
            int peakPos = 100;
            int peakPosNearby = peakPos + 6;
            std::vector<std::pair<int, double>> peaks = { std::pair<int, double>(peakPos - 4, -3),
                                                         std::pair<int, double>(peakPos - 3, -2),
                                                         std::pair<int, double>(peakPos - 2, -2.5),
                                                         std::pair<int, double>(peakPos - 1, -1.5),
                                                         std::pair<int, double>(peakPos, peakAmplitude),
                                                         std::pair<int, double>(peakPos + 1, -1.5),
                                                         std::pair<int, double>(peakPos + 2, -2.1),
                                                         std::pair<int, double>(peakPos + 3, -2),
                                                         std::pair<int, double>(peakPos + 4, -4),
                                                         std::pair<int, double>(peakPos + 5, nearbyPeakAmplitude),
                                                         std::pair<int, double>(peakPosNearby, peakAmplitude) };

            Logger::WriteMessage("Create random signal with known peaks\n");
            std::vector<double> input = createSignal(signalSize, signalMean, signalDev, peaks);

            // When : Analyze this signal
            int sampleSize = 30;
            double threshold = 5.0;
            double influence = 0.0;
            Logger::WriteMessage("perform signal analysis\n");
            signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, sampleSize, threshold, influence);

            // Then : The two nearby peaks are detected
            Assert::AreEqual(2, (int)signalAnalyzed.Spikes.size());
            Assert::AreEqual(peakPos, (int) signalAnalyzed.Spikes.at(0).Index);
            Assert::AreEqual(peakPosNearby, (int) signalAnalyzed.Spikes.at(1).Index);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_07_Peak_detector_does_not_detect_peak_on_noisy_signal_whithout_significant_peak)
        {
            Logger::WriteMessage("_07_Peak_detector_does_not_detect_peak_on_noisy_signal_whithout_significant_peak\n");
            // Given : a strongly noise signal without peak
            std::vector<std::pair<int, double>> noPeaks = {};
            int signalSize = 500;
            double signalMean = -5;
            double signalDev = 10; 
            Logger::WriteMessage("Create random signal with known peaks\n");
            std::vector<double> input = createSignal(signalSize, signalMean, signalDev, noPeaks);

            // When : Analyze this signal
            int sampleSize = 30;
            double threshold = 5.0;
            double influence = 0.0;
            Logger::WriteMessage("perform signal analysis\n");
            signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, sampleSize, threshold, influence);

            // Then : No peak is detected
            Assert::AreEqual(0, (int) signalAnalyzed.Spikes.size());
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_08_Peak_detector_detect_peak_on_noisy_signal_whith_significant_peak)
        {
            Logger::WriteMessage("Peak_detector_detect_peak_on_noisy_signal_whith_significant_peak\n");
            // Given : a strongly noise signal with significant peak
            int signalSize = 500;
            double signalMean = -5;
            double signalDev = 10;
            double peakAmplitude = 25;
            int peakPos = 100;
            
            std::vector<std::pair<int, double>> peaks = { std::pair<int, double>(peakPos, peakAmplitude) };

            Logger::WriteMessage("Create random signal with known peaks\n");
            std::vector<double> input = createSignal(signalSize, signalMean, signalDev, peaks);
         
            /*std::ofstream myfile;
            myfile.open(".\\..\\..\\Tests\\Data\\__08example.csv");
            int v = 0;
            for (const auto& value : input) {
                myfile << v << "," << value << "\n";
                v++;
            }
            myfile.close*/
            //std::vector<double> input = createSignalFromCSV(std::string("__08exampleFAIL.csv"));

            // When : Analyze this signal
            int sampleSize = 30;
            double threshold = 5.0;
            double influence = 0.0;
            Logger::WriteMessage("perform signal analysis\n");
            signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, sampleSize, threshold, influence);

            // Then : Significant peak is detected
            Assert::AreEqual(1, (int) signalAnalyzed.Spikes.size());
            Assert::AreEqual(peakPos, (int) signalAnalyzed.Spikes.at(0).Index);
            Assert::AreEqual(peakAmplitude, signalAnalyzed.Spikes.at(0).Value);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_09_Peak_detector_failed_if_signal_size_is_smaller_than_the_statistical_sample_needed_)
        {
            Logger::WriteMessage("Peak_detector_failed_if_signal_size_is_smaller_than_the_statistical_sample_needed_\n");
           
            // Given : a signal smaller than the statistical sample needed
            int signalSize = 29;
            double signalMean = -5;
            double signalDev = 10;
            std::vector<std::pair<int, double>> peaks = {};

            Logger::WriteMessage("Create random signal with known peaks\n");
            std::vector<double> input = createSignal(signalSize, signalMean, signalDev, peaks);
            int sampleSize = 30;

            // When : Analyze this signal
            double threshold = 5.0;
            double influence = 0.0;

            // Then : Signal analysis failed
            Logger::WriteMessage("perform signal analysis\n");
            auto funcAnalysisSignal = [input, sampleSize, threshold, influence] { signal_1D::SignalAnalysisByZScoreThresholding(input, sampleSize, threshold, influence); };
            Assert::ExpectException<std::exception>(funcAnalysisSignal);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_10_Peak_detector_return_correct_means_when_signal_have_no_significant_spikes)
        {
            Logger::WriteMessage("Peak_detector_return_correct_means_when_signal_have_no_significant_spikes \n");

            // Given : Input signal with no significante spikes
            double baseSignalMean = 1;
            double baseSignalStddev = 0.5;
            int lag = 4;
            double threshold = 10;
            double influence = 0;

            Logger::WriteMessage("Create signal\n");
            std::vector<double> input;
            for (int i = 0; i < 10; i++) {
                if (i % 2 == 0) {
                    input.push_back(baseSignalMean + baseSignalStddev);
                }
                else {
                    input.push_back(baseSignalMean - baseSignalStddev);
                }
            }

            // When : Analyze this signal
            Logger::WriteMessage("perform signal analysis\n");
            signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, lag, threshold, influence);

            // Then : We obtains correct means
            Assert::AreEqual(input.size(), signalAnalyzed.MovingMeans.size());
            for (int i = 0; i < lag - 1; i++) {
                Assert::AreEqual(0.0, signalAnalyzed.MovingMeans.at(i));
            }
            for (int i = lag - 1; i < signalAnalyzed.MovingMeans.size(); i++) {
                Assert::AreEqual(baseSignalMean, signalAnalyzed.MovingMeans.at(i), 0.1);
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_11_Peak_detector_return_correct_means_when_signal_have_significant_spikes_and_not_influence_means)
        {
            Logger::WriteMessage("Peak_detector_return_correct_means_when_signal_have_significant_spikes_and_not_influence_means \n");

            // Given : Input signal with significante spikes and an influence of these spikes of 0
            double baseSignalMean = 0.2;
            int lag = 4;
            double threshold = 10;
            double influence = 0;

            Logger::WriteMessage("Create signal\n");
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
            Logger::WriteMessage("perform signal analysis\n");
            signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, lag, threshold, influence);

            // Then : We obtains correct means
            Assert::AreEqual(input.size(), signalAnalyzed.MovingMeans.size());
            for (int i = 0; i < lag - 1; i++) {
                Assert::AreEqual(0.0, signalAnalyzed.MovingMeans.at(i));
            }
            for (int i = lag - 1; i < signalAnalyzed.MovingMeans.size()-1; i++) {
                Assert::AreEqual(baseSignalMean, signalAnalyzed.MovingMeans.at(i), 0.1);
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_12_Peak_detector_return_correct_stddev_when_signal_have_no_significant_spikes)
        {
            Logger::WriteMessage("Peak_detector_return_correct_stddev_when_signal_have_no_significant_spikes\n");


            // Given : Input signal with no significante spikes
            double baseSignalMean = 0.5;
            double baseSignalStddev = 0.5;
            int lag = 4;
            double threshold = 10;
            double influence = 0;

            Logger::WriteMessage("Create signal\n");
            std::vector<double> input;
            for (int i = 0; i < 12; i++) {
                if (i % 2 == 0) {
                    input.push_back(baseSignalMean + baseSignalStddev);
                }
                else {
                    input.push_back(baseSignalMean - baseSignalStddev);
                }
            }

            // When : Analyze this signal
            Logger::WriteMessage("perform signal analysis\n");
            signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, lag, threshold, influence);

            // Then : We obtains correct stddev
            Assert::AreEqual(input.size(), signalAnalyzed.MovingStddev.size());
            for (int i = 0; i < lag - 1; i++) {
                Assert::AreEqual(0.0, signalAnalyzed.MovingStddev.at(i));
            }
            for (int i = lag - 1; i < signalAnalyzed.MovingStddev.size(); i++) {
                Assert::AreEqual(baseSignalStddev, signalAnalyzed.MovingStddev.at(i), 0.1);
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_13_Peak_detector_return_correct_stddev_when_signal_have_significant_spikes_and_not_influence_means)
        {
            Logger::WriteMessage("\n");
            // Given : Input signal with significante spikes and an influence of these spikes of 0
            double baseSignalMean = 0.2;
            double baseSignalStddev = 0;
            int lag = 4;
            double threshold = 10;
            double influence = 0;

            Logger::WriteMessage("Create signal\n");
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
            Logger::WriteMessage("perform signal analysis\n");
            signal_1D::SignalStats signalAnalyzed = signal_1D::SignalAnalysisByZScoreThresholding(input, lag, threshold, influence);

            // Then : We obtains correct stddev
            Assert::AreEqual(input.size(), signalAnalyzed.MovingStddev.size());
            for (int i = 0; i < lag - 1; i++) {
                Assert::AreEqual(0.0, signalAnalyzed.MovingStddev.at(i));
            }
            for (int i = lag - 1; i < signalAnalyzed.MovingStddev.size(); i++) {
                Assert::AreEqual(baseSignalStddev, signalAnalyzed.MovingStddev.at(i), 0.1);
            }
            Logger::WriteMessage("Done\n");
        }
    };
}
