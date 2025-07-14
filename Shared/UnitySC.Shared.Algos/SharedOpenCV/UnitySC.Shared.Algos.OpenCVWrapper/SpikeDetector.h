#pragma once
#include <vector>

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {
    public enum class SpikeType {
        Drop,
        Peak,
    };

    public ref struct Spike {
        double Value;
        double Index;
        SpikeType^ Type;
    };

    public ref struct SignalStats {
        array<Spike^>^ Spikes;
        array<double>^ Means;
        array<double>^ Stddev;
    };

    public ref class SpikeDetector {

    public:
        /**
         * Analyse an input signal and return spikes and statistics measured on the signal.
         *
         * @param signalValues      - the input signal to analyse
         * @param sampleSize        - the sample size of the moving window (introduce an offset at the start of the signal before starting the analysis)
         * @param threshold         - the z-score at which the algorithm signals a peak
         * @param spikeInfluence    - the influence (between 0 and 1) of new detected spike on the mean and standard deviation measured
         *
         * @return Statistics on the analyzed signal
         *
         */
        static SignalStats^ AnalyzeSignal(System::Collections::Generic::List<double>^ signalValues, int sampleSize, double threshold, double spikeInfluence);
    };
}
