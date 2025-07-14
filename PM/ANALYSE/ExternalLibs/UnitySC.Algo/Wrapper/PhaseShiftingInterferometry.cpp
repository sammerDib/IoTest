#include <PhaseShiftingInterferometry.h>
#include <Tools.h>

using namespace AlgosLibrary;
using namespace System;

namespace AlgosLibrary {

    namespace {
        psi::UnwrapMode ConvertToCppType(UnwrapMode mode)
        {
            switch(mode)
            {
            case UnwrapMode::Goldstein:
                return psi::UnwrapMode::Goldstein;
            case UnwrapMode::GradientQuality:
                return psi::UnwrapMode::GradientQuality;
            case UnwrapMode::HistogramReliabilityPath:
                return psi::UnwrapMode::HistogramReliabilityPath;
            case UnwrapMode::PseudoCorrelationQuality:
                return psi::UnwrapMode::PseudoCorrelationQuality;
            case UnwrapMode::VarianceQuality:
                return psi::UnwrapMode::VarianceQuality;
            }
        }
    }

    TopographyPSI^ PhaseShiftingInterferometry::ComputeTopography(array<ImageData^>^ input, PSIParams^ params) {
        // Process input parameters
        std::vector<cv::Mat> imgs;
        for each (ImageData ^ data in input) { imgs.push_back(CreateMatFromImageData(data)); }

        // Call native method
        psi::PSIResultData psiResult = psi::TopoReconstructionFromPhase(imgs, params->WavelengthInNanometers, params->StepNb, ConvertToCppType(params->UnwrappingMode));
        cv::Mat wrappedPhaseMap = psiResult.WrappedPhaseMap;
        cv::Mat unwrappedPhaseMap = psiResult.UnwrappedPhaseMap;
        cv::Mat rawTopographyMap = psiResult.RawTopographyMap;
        cv::Mat topographyMap = psiResult.TopographyMap;
        cv::Mat plane = psiResult.Plane;

        // Process output result
        ImageData^ wrappedPhaseMapImg = gcnew ImageData();
        wrappedPhaseMapImg->ByteArray = CreateByteArrayFromMat(wrappedPhaseMap);
        wrappedPhaseMapImg->Depth = wrappedPhaseMap.depth();
        wrappedPhaseMapImg->Height = wrappedPhaseMap.size().height;
        wrappedPhaseMapImg->Width = wrappedPhaseMap.size().width;

        ImageData^ unwrappedPhaseMapImg = gcnew ImageData();
        unwrappedPhaseMapImg->ByteArray = CreateByteArrayFromMat(unwrappedPhaseMap);
        unwrappedPhaseMapImg->Depth = unwrappedPhaseMap.depth();
        unwrappedPhaseMapImg->Height = unwrappedPhaseMap.size().height;
        unwrappedPhaseMapImg->Width = unwrappedPhaseMap.size().width;

        ImageData^ rawTopographyMapImg = gcnew ImageData();
        rawTopographyMapImg->ByteArray = CreateByteArrayFromMat(rawTopographyMap);
        rawTopographyMapImg->Depth = rawTopographyMap.depth();
        rawTopographyMapImg->Height = rawTopographyMap.size().height;
        rawTopographyMapImg->Width = rawTopographyMap.size().width;

        ImageData^ topographyMapImg = gcnew ImageData();
        topographyMapImg->ByteArray = CreateByteArrayFromMat(topographyMap);
        topographyMapImg->Depth = topographyMap.depth();
        topographyMapImg->Height = topographyMap.size().height;
        topographyMapImg->Width = topographyMap.size().width;

        ImageData^ planeImg = gcnew ImageData();
        planeImg->ByteArray = CreateByteArrayFromMat(plane);
        planeImg->Depth = plane.depth();
        planeImg->Height = plane.size().height;
        planeImg->Width = plane.size().width;

        TopographyPSI^ result = gcnew TopographyPSI();
        result->WrappedPhaseMap = wrappedPhaseMapImg;
        result->UnwrappedPhaseMap = unwrappedPhaseMapImg;
        result->RawTopographyMap = rawTopographyMapImg;
        result->TopographyMap = topographyMapImg;
        result->Plane = planeImg;

        return result;
    }
}