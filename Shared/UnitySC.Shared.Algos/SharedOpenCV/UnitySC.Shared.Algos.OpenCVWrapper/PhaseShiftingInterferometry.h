#pragma once

#include "ImageData.h"


#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {

    public enum class UnwrapMode {
        GradientQuality,
        VarianceQuality,
        PseudoCorrelationQuality,
        HistogramReliabilityPath,
        Goldstein
    };

    public ref struct PSIParams {
        PSIParams(double wavelengthNm, int stepNb, bool residualFringesRemoving, UnwrapMode unwrappingMode)
        {
            WavelengthInNanometers = wavelengthNm;
            StepNb = stepNb;
            ResidualFringesRemoving = residualFringesRemoving;
            UnwrappingMode = unwrappingMode;
        };
        double WavelengthInNanometers;      //wavelength in nanometers used to capture interferometry images
        int StepNb;                         //number of phase shifts used to capture interferometry images
        bool ResidualFringesRemoving;       //use technique to remove residual fringes on wrapped phase map
        UnwrapMode UnwrappingMode;          //technique used to compute the phase unwrapping
    };

    public ref struct TopographyPSI {
        ImageData^ WrappedPhaseMap;
        ImageData^ UnwrappedPhaseMap;
        ImageData^ RawTopographyMap;
        ImageData^ TopographyMap;
        ImageData^ Plane;
    };

    public ref class PhaseShiftingInterferometry {

    public:
        /**
         * Calculate a topography depth map from interferometry images
         *
         * @param imgs              - interferometry images
         * @param params            - unwrapping parameters (wavelength & number of phase shifts used to capture interferometry images, technique used to compute the phase unwrapping)
         *
         * @return The Topography depth map
         */
        static TopographyPSI^ ComputeTopography(array<ImageData^>^ input, PSIParams^ params);
    };
} // namespace UnitySCSharedAlgosOpenCVWrapper
