#pragma once
#include <opencv2/opencv.hpp>
#include <filesystem>

#pragma unmanaged

namespace psd {
    /**
    * Create a mask to select the area inside the wafer, excluding the edge area
    *
    * @param img                                - image of wafer on stage
    * @param pixelSize                          - Pixel size to convert pixels to millimeters
    * @param waferRadius                        - radius of wafer to be detected
    * @param edgeExclusion                      - edge size to exclude
    * @param directoryPathToStoreReport         - directory path to store report if not empty
    *
    * @return Mask of region of interest
    */
    cv::Mat CreateWaferMask(cv::Mat img, float pixelSize, float waferRadius, float edgeExclusion = 0.0, std::filesystem::path directoryPathToStoreReport = "");
}