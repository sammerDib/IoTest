#pragma once

#include <vector>
#include <list>

namespace signal_1D {

    /**
     * Represents the spike type
     */
    enum class SpikeType {
        Drop,
        Peak,
    };

    /**
     * Represents a spike with its associated characteristics
     */
    struct Spike {
        Spike(double value, size_t index, SpikeType type) {
            Value = value;
            Index = index;
            Type = type;
        }

        double Value;
        size_t Index;
        SpikeType Type;
    };

    /**
     * Represents a signal with its associated characteristics
     */
    struct SignalStats {
        std::vector<Spike> Spikes;
        std::vector<double> MovingMeans;
        std::vector<double> MovingStddev;
    };

    /**
     * The Smoothed Z-Score Algorithm analyse an input signal and return peaks and statistics measured on the signal.
     *
     * @param input         - the input signal to analyse
     * @param sampleSize    - the sample size of the moving window
     * @param threshold     - the z-score at which the algorithm signals a peak
     * @param influence     - the influence (between 0 and 1) of new detected peak on the mean and standard deviation
     *
     * @return Statistics on the analyzed signal
     *
     */
    SignalStats SignalAnalysisByZScoreThresholding(const std::vector<double>& input, int sampleSize, double threshold, double influence);

    std::vector<Spike> FindPeaks(const std::vector<double>& input, double threshold);
    std::vector<Spike> FindDrops(const std::vector<double>& input, double threshold);

} // namespace signal_1D