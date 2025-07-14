#include <SpikeDetector.h>

using namespace AlgosLibrary;
using namespace System;

namespace AlgosLibrary {

    namespace {
        array<Spike^>^ ConvertNativeSpikeVectorToManagedSpikeArray(
            std::vector<signal_1D::Spike>& dataVec) {
            const int size = dataVec.size();
            array<Spike^>^ dataArray = gcnew array<Spike^>(size);
            for (int i = 0; i < size; i++) {
                dataArray[i] = gcnew Spike();
                dataArray[i]->Value = dataVec[i].Value;
                dataArray[i]->Index = dataVec[i].Index;
                dataArray[i]->Type = (dataVec[i].Type == signal_1D::SpikeType::Peak)? SpikeType::Peak: SpikeType::Drop;
            }
            return dataArray;
        }
    }

    SignalStats^ SpikeDetector::AnalyzeSignal(array<double>^ signalValues, int sampleSize, double threshold, double spikeInfluence) {
        // Process input parameters
        std::vector<double> inputSignal;
        for each (double value in signalValues) {
            inputSignal.push_back(value);
        }

        // Call native method
        signal_1D::SignalStats tempVec = signal_1D::SignalAnalysisByZScoreThresholding(inputSignal, sampleSize, threshold, spikeInfluence);

        // Process output result
        SignalStats^ signalAnalyzed = gcnew SignalStats();
        signalAnalyzed->Means = CreateArrayFromVector(tempVec.MovingMeans);
        signalAnalyzed->Stddev = CreateArrayFromVector(tempVec.MovingStddev);
        signalAnalyzed->Spikes = ConvertNativeSpikeVectorToManagedSpikeArray(tempVec.Spikes);
        return signalAnalyzed;
    }
}