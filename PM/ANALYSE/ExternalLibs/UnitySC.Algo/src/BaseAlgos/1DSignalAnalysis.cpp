#include <algorithm>
#include <cmath>
#include <exception>
#include <iostream>
#include <iterator>
#include <numeric>
#include <opencv2/opencv.hpp>
#include <unordered_map>

#include <BaseAlgos/1DSignalAnalysis.hpp>
#include <Logger.hpp>

using namespace std;
using namespace cv;

typedef double ld;
typedef unsigned int uint;

namespace signal_1D {

    namespace {

        list<Spike> FindSpikes(const vector<double>& input, int startingIndex, SpikeType spikeType, double threeshold);

        class VectorStats {

        public:
            VectorStats(vector<double>::iterator startOfWindow, vector<double>::iterator endOfWindow);
            double Mean();
            double StandardDeviation();

        private:
            void Compute();
            vector<double>::iterator _start;
            vector<double>::iterator _end;
            double _mean;
            double _stddev;
        };
    } // namespace


    SignalStats SignalAnalysisByZScoreThresholding(vector<double> input, int sampleSize, double threshold, double influence) {
        if (sampleSize > input.size()) {
            stringstream strStrm;
            strStrm << "[Signal analysis by ZScore thresholding] Statistical sample must be smaller than signal size.";
            string message = strStrm.str();
            Logger::Error(message);
            throw exception(message.c_str());
        }

        // Initialize variables
        vector<ld> filteredInputsSignal(input.begin(), input.end());
        vector<ld> movingMean;
        vector<ld> movingStddev;
        vector<ld> currentSpike;
        vector<Spike> spikesDetected;

        // Initialize first values of mean and standard deviation in the initial moving window
        VectorStats subvectorStats(input.begin(), input.begin() + sampleSize);
        movingMean.insert(movingMean.end(), (size_t)sampleSize - 1, 0);
        movingStddev.insert(movingStddev.end(), (size_t)sampleSize - 1, 0);
        movingMean.push_back(subvectorStats.Mean());
        movingStddev.push_back(subvectorStats.StandardDeviation());

        // Begins signal analysis
        bool potentialSpikeDetected = false;
        for (size_t i = sampleSize; i < input.size(); i++) {
            bool currentValueIsPotentialSpike = abs(input[i] - movingMean.back()) > 2 * movingStddev.back();
            bool isLastData = (i == (uint)input.size() - 1);

            // Value potentially belonging to a peak : save input data corresponding to a potential peak in a temp vector to process them later
            if (currentValueIsPotentialSpike) {
                potentialSpikeDetected = true;
                currentSpike.push_back(input[i]);
            }

            // A potential spike has just ended : Process it
            bool newSpikeToProcess = (!currentValueIsPotentialSpike || isLastData) && potentialSpikeDetected;
            if (newSpikeToProcess) {
                double maxValue = *max_element(currentSpike.begin(), currentSpike.end());
                double minValue = *min_element(currentSpike.begin(), currentSpike.end());
                bool isDrop = minValue < movingMean.back() - threshold * movingStddev.back();
                bool isPeak = maxValue > movingMean.back() + threshold * movingStddev.back();

                int spikeSize = currentSpike.size();

                if (isPeak || isDrop) {
                    // The input signal is filtered to reduce the influence of the peaks on the statistics and therefore on the following detections
                    for (int indexInSpike = spikeSize; indexInSpike > 0; indexInSpike--) {
                        size_t indexInGlobalSignal = i - indexInSpike;
                        filteredInputsSignal[indexInGlobalSignal] = influence * input[indexInGlobalSignal] + (1 - influence) * filteredInputsSignal[indexInGlobalSignal - 1];
                    }

                    // Process and store this peak
                    SpikeType type = isPeak ? SpikeType::Peak : SpikeType::Drop;
                    int indexAfterSpike = (isLastData) ? i + 1 : i; // If it is the last value of the signal we increment to simulate that we are after the peak
                    list<Spike> spikes = FindSpikes(currentSpike, indexAfterSpike - spikeSize, type, threshold * movingStddev.back());
                    for each(Spike spike in spikes) { spikesDetected.push_back(spike); }
                }

                // Adjust the mean and standard deviation of the moving window
                for (int indexInSpike = spikeSize; indexInSpike > 0; indexInSpike--) {
                    size_t indexInGlobalSignal = i - indexInSpike;
                    VectorStats subvectorStats(filteredInputsSignal.begin() + (indexInGlobalSignal - sampleSize), filteredInputsSignal.begin() + indexInGlobalSignal);
                    movingMean.push_back(subvectorStats.Mean());
                    movingStddev.push_back(subvectorStats.StandardDeviation());
                }
                potentialSpikeDetected = false;
                currentSpike.clear();
            }

            // Value not belonging to a peak : adjust directly the mean and standard deviation of the moving window
            if (!currentValueIsPotentialSpike) {
                VectorStats subvectorStats(filteredInputsSignal.begin() + (i - sampleSize), filteredInputsSignal.begin() + i);
                movingMean.push_back(subvectorStats.Mean());
                movingStddev.push_back(subvectorStats.StandardDeviation());
            }
        }

        SignalStats stats;
        stats.Spikes = spikesDetected;
        stats.MovingMeans = movingMean;
        stats.MovingStddev = movingStddev;

        return stats;
    }

    namespace {

        list<Spike> FindSpikes(const vector<double>& input, int startingIndex, SpikeType spikeType, double threeshold) {
            list<Spike> spikes;

            if (input.empty()) {
                return spikes;
            }

            bool currentlySearchingLocalMax = spikeType == SpikeType::Peak ? true : false;
            Spike localExtremum = Spike(input[0], startingIndex, spikeType);

            for (int index = 0; index < input.size(); index++) {
                if (currentlySearchingLocalMax) {
                    bool currentValueIsBetterLocalMax = input[index] > localExtremum.Value;
                    bool localMaximumIsOver = input[index] <= localExtremum.Value - threeshold || index == input.size() - 1;
                    if (currentValueIsBetterLocalMax) {
                        localExtremum.Index = startingIndex + index;
                        localExtremum.Value = input[index];
                    }
                    if (localMaximumIsOver) {
                        if (spikeType == SpikeType::Peak) {
                            spikes.push_back(localExtremum);
                        }
                        currentlySearchingLocalMax = false;
                        localExtremum = Spike(input[index], startingIndex + index, SpikeType::Drop);
                    }
                }
                else {
                    bool currentValueIsBetterLocalMin = input[index] < localExtremum.Value;
                    bool localMinimumIsOver = input[index] >= localExtremum.Value + threeshold || index == input.size() - 1;
                    if (currentValueIsBetterLocalMin) {
                        localExtremum.Index = startingIndex + index;
                        localExtremum.Value = input[index];
                    }
                    if (localMinimumIsOver) {
                        if (spikeType == SpikeType::Drop) {
                            spikes.push_back(localExtremum);
                        }
                        currentlySearchingLocalMax = true;
                        localExtremum = Spike(input[index], startingIndex + index, SpikeType::Peak);
                    }
                }
            }
            return spikes;
        }

        VectorStats::VectorStats(vector<ld>::iterator start, vector<ld>::iterator end) {
            _start = start;
            _end = end;
            Compute();
        }

        ld VectorStats::Mean() { return _mean; }

        ld VectorStats::StandardDeviation() { return _stddev; }

        void VectorStats::Compute() {
            // Calculates the mean and standard deviation using STL function.
            // This is the two pass implementation of the Mean & Variance calculation.
            ld sum = accumulate(_start, _end, 0.0);
            int sliceSize = distance(_start, _end);
            ld mean = sum / sliceSize;
            vector<ld> diff(sliceSize);
            transform(_start, _end, diff.begin(), [mean](ld x) { return x - mean; });
            ld sqSum = inner_product(diff.begin(), diff.end(), diff.begin(), 0.0);
            ld stddev = sqrt(sqSum / ((size_t)sliceSize - 1));

            _mean = mean;
            _stddev = stddev;
        }
    } // namespace
} // namespace signal_1D