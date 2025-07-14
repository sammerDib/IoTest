#pragma once
#include <opencv2/core/mat.hpp>
#include <filesystem>

#include "WrappedPhaseMap.hpp"

#pragma unmanaged
namespace residual_fringe_removing {

    /**
     * @brief Calculate a wrapped phase map without residual fringe from interferometry images.
     * @details At least three steps and one image per step are necessary to run this algorithm.
     *
     * @param imgs                          - interferometry images (must contain enough visible fringe)
     * @param stepNb                        - number of phase shifts used to capture interferometry images
     * @param directoryPathToStoreReport    - directory path to store report if not empty
     *
     * @return The wrapped phase map
     */
    WrappedPhaseMap WrappedPhaseMapWithoutResidualFringe(std::vector<cv::Mat>& imgs, int stepNb, std::filesystem::path directoryPathToStoreReport = "");
}