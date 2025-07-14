#pragma once
#include <filesystem>

#include <opencv2/opencv.hpp>

#pragma unmanaged
namespace psi {
    /*
     * Unwrapping techniques used
     */
    enum class UnwrapMode {
        GradientQuality,
        VarianceQuality,
        PseudoCorrelationQuality,
        HistogramReliabilityPath,
        Goldstein
    };

    struct PSIResultData {
        cv::Mat WrappedPhaseMap;
        cv::Mat UnwrappedPhaseMap;
        cv::Mat RawTopographyMap;
        cv::Mat TopographyMap;
        cv::Mat Plane;
    };

    /**
     * Calculate a topography image from interferometry images
     *
     * @param imgs                          - interferometry images
     * @param wavelengthNm                  - wavelength in nanometers used to capture interferometry images
     * @param stepNb                        - number of phase shifts used to capture interferometry images
     * @param mode                          - technique used to compute the phase unwrapping
     * @param removeResidualFringes         - use method to remove residual fringes to compute the wrapped phase map. Be careful, this method takes more time !
     * àparam directoryPathToStoreReport    - directory path to store report if not empty
     *
     * @return The wrapped map, the unwrapped map and the topography image
     */
    PSIResultData TopoReconstructionFromPhase(std::vector<cv::Mat> imgs, double wavelengthNm, int stepNb, UnwrapMode mode, bool removeResidualFringes = false, std::filesystem::path directoryPathToStoreReport = "");
} // namespace psi