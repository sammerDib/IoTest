#include <algorithm>
#include <cmath>
#include <exception>
#include <iostream>
#include <iterator>
#include <numeric>
#include <opencv2/opencv.hpp>
#include <unordered_map>

#include "C1DSignalAnalysis.hpp"
#include "ErrorLogging.hpp"

#pragma unmanaged
using namespace std;
using namespace cv;

typedef double ld;
typedef unsigned int uint;

//v2 // optimized to test on machine
namespace signal_1D {

    namespace {

        vector<Spike> FindSpikes(const vector<double>& input, size_t startingIndex, SpikeType spikeType, double threeshold);
        pair<double, double> computeStats(vector<double>::iterator start, vector<double>::iterator end, double& m2final);

    } // namespace


    SignalStats SignalAnalysisByZScoreThresholding(const vector<double>& input, int sampleSizearg, double threshold, double influence) {
        size_t sampleSize = (size_t) sampleSizearg;
        if (sampleSize > input.size())
            ErrorLogging::LogErrorAndThrow("[Signal analysis by ZScore thresholding] Statistical sample must be smaller than signal size: ", sampleSize, " > ", input.size());

        // Initialize variables
        size_t signalSize = input.size();
        vector<double> filteredInputsSignal(input);
        vector<double> movingMean(signalSize);
        vector<double> movingStddev(signalSize);

        vector<Spike> spikesDetected;
        spikesDetected.reserve(signalSize / 2); // Pré-allocation pour optimiser `push_back`

        // Initialize first values of mean and standard deviation in the initial moving window
        double mean = 0.0, m2 = 0.0;
        for (size_t i = 0; i < sampleSize; i++) {
            const double delta = input[i] - mean;
            mean += delta / (i + 1);
            m2 += delta * (input[i] - mean);
        }
        double variance = (sampleSize > 1) ? m2 / (sampleSize - 1) : 0.0;
        double stddev = sqrt(variance);

        movingMean[sampleSize - 1] = mean;
        movingStddev[sampleSize - 1] = stddev;

        // Begins signal analysis
        vector<double> currentSpike;
        bool potentialSpikeDetected = false;
        for (size_t i = sampleSize; i < signalSize; i++) {
            double currentValue = input[i];
            bool currentValueIsPotentialSpike = abs(currentValue - mean) > 2.0 * stddev; // 2 Sigma detection of potential spike to improve statistics avoid using threshold here
            bool isLastData = (i == signalSize - 1);

            // Value potentially belonging to a peak : save input data corresponding to a potential peak in a temp vector to process them later
            if (currentValueIsPotentialSpike) {
                potentialSpikeDetected = true;
                currentSpike.push_back(currentValue);
            }

            // A potential spike has just ended : Process it
            if ((!currentValueIsPotentialSpike || isLastData) && potentialSpikeDetected) {
                double maxValue = *max_element(currentSpike.begin(), currentSpike.end());
                double minValue = *min_element(currentSpike.begin(), currentSpike.end());
                bool isDrop = minValue < mean - threshold * stddev;
                bool isPeak = maxValue > mean + threshold * stddev;

                size_t spikeSize = currentSpike.size();

                if (isPeak || isDrop) {
                    // Process and store this peak
                    SpikeType type = isPeak ? SpikeType::Peak : SpikeType::Drop;
                    size_t startIndex = i - spikeSize;
                    if (isLastData)
                        startIndex += 1;  // If it is the last value of the signal we increment to simulate that we are after the peak
                    vector<Spike> spikes = FindSpikes(currentSpike, startIndex, type, threshold * stddev);
                    spikesDetected.insert(spikesDetected.end(), spikes.begin(), spikes.end());
                    
                    // The input signal is filtered to reduce the influence of the peaks on the statistics and therefore on the following detections
                      for (size_t j = 0; j < spikeSize; j++) {
                          size_t index = i - spikeSize + j;
                          filteredInputsSignal[index] = influence * input[index] + (1.0 - influence) * filteredInputsSignal[index - 1];
                      }
                }

                // Adjust the mean and standard deviation of the moving window
                for (size_t j = 0; j < spikeSize; j++) {
                    size_t index = i - spikeSize + j;
                    double oldValue = filteredInputsSignal[index - sampleSize];
                    double newValue = filteredInputsSignal[index];

                    double deltaOld = oldValue - mean;
                    double deltaNew = newValue - mean;
                    mean += deltaNew / sampleSize - deltaOld / sampleSize;
                    m2 += deltaNew * (newValue - mean) - deltaOld * (oldValue - mean);

                    variance = (sampleSize > 1) ? m2 / (sampleSize - 1) : 0.0;
                    stddev = sqrt(variance);

                    movingMean[index] = mean;
                    movingStddev[index] = stddev;
                }

                potentialSpikeDetected = false;
                currentSpike.clear();

            }

            // Value not belonging to a peak : adjust directly the mean and standard deviation of the moving window
            if (!currentValueIsPotentialSpike)
            {
                double oldValue = filteredInputsSignal[i - sampleSize];
                double deltaOld = oldValue - mean;
                double deltaNew = currentValue - mean;
                mean += deltaNew / sampleSize - deltaOld / sampleSize;
                m2 += deltaNew * (currentValue - mean) - deltaOld * (oldValue - mean);

                variance = (sampleSize > 1) ? m2 / (sampleSize - 1) : 0.0;
                stddev = sqrt(variance);

                movingMean[i] = mean;
                movingStddev[i] = stddev;
            }
        }

        SignalStats stats;
        stats.Spikes = spikesDetected;
        stats.MovingMeans = movingMean;
        stats.MovingStddev = movingStddev;

        return stats;
    }

    namespace {

        vector<Spike> FindSpikes(const vector<double>& input, size_t startingIndex, SpikeType spikeType, double threshold) {
            vector<Spike> spikes;

            if (input.size() < 1) {
                return spikes;
            }

            spikes.reserve(input.size() / 2);  // Réduction des reallocations

            bool currentlySearchingLocalMax = (spikeType == SpikeType::Peak);
            Spike localExtremum(input[0], startingIndex, spikeType);

            for (size_t index = 0; index < input.size(); index++) {
                double currentValue = input[index];

                if (currentlySearchingLocalMax) {
                    // if currentValueIsBetterLocalMax
                    if (currentValue > localExtremum.Value) { 
                        localExtremum = Spike(currentValue, startingIndex + index, spikeType);
                    }
                    // if localMaximumIsOver
                    if (currentValue <= localExtremum.Value - threshold || index == input.size() - 1) {
                        if (spikeType == SpikeType::Peak) {
                            spikes.push_back(localExtremum);
                        }
                        currentlySearchingLocalMax = false;
                        localExtremum = Spike(currentValue, startingIndex + index, SpikeType::Drop);
                    }
                }
                else {
                    // if currentValueIsBetterLocalMin
                    if (currentValue < localExtremum.Value) {
                        localExtremum = Spike(currentValue, startingIndex + index, spikeType);
                    }
                    // if localMinimumIsOver
                    if (currentValue >= localExtremum.Value + threshold || index == input.size() - 1) {
                        if (spikeType == SpikeType::Drop) {
                            spikes.push_back(localExtremum);
                        }
                        currentlySearchingLocalMax = true;
                        localExtremum = Spike(currentValue, startingIndex + index, SpikeType::Peak);
                    }
                }
            }
            return spikes;
        }

        // pre compute stats Utilisation de la méthode de Welford pour un calcul à passage unique
        pair<double, double> computeStats(vector<double>::iterator start, vector<double>::iterator end, double& m2final) {
            size_t count = 0;
            double mean = 0.0, m2 = 0.0;

            for (auto it = start; it != end; ++it) {
                count++;
                double delta = *it - mean;
                mean += delta / count;
                m2 += delta * (*it - mean);
            }

            double variance = (count > 1) ? (m2 / (count - 1)) : 0.0;
            double stdDev = sqrt(variance);

            m2final = m2;
            return { mean, stdDev };
        }
    }// namespace

    std::vector<Spike> FindPeaks(const std::vector<double>& input, double threshold)
    {
        return FindSpikes(input, 0, SpikeType::Peak, threshold);
    }

    std::vector<Spike> FindDrops(const std::vector<double>& input, double threshold)
    {
        return FindSpikes(input, 0, SpikeType::Drop, threshold);
    }

} // namespace signal_1D v2 // optimized to test on machine
