#pragma once
#include <filesystem>
#include <opencv2/opencv.hpp>

#pragma unmanaged
namespace psd {
    struct MaskOutlierNormalOptions {
        // scalar defining the maximum allowed gradient magnitude above which the normal vectors will be considered outliers.
        float maxAllowedGrad = NAN;
        // scalar factor for automatic outlier threshold detection. Larger values allow fewer outliers to be identified.
        float outlierIQRScale = 1.5f;
    };

    struct NanoTopoData {
        cv::Mat PX, PY, NX, NY;
    };

    struct NanoTopoInput {
        NanoTopoData Data;
        int ColumnElementNumberByGroup;
        int RowElementNumberByGroup;
    };

    struct SubsampledData {
        cv::Mat AveragedPX, AveragedPY, AveragedNX, AveragedNY;
        std::vector<int> ValidRowIndices, ValidColumnIndices;
        std::vector<int> SubsampledRowIndices, SubsampledColumnIndices;
        cv::Mat Ic, Jc;
        std::vector<float> I, J;
    };

    /*
    This function is designed to mask out "outlier" normal vectors, based on the gradients of these vectors.
    The normal vectors are represented by the fields NX (x-component) and NY (y-component),
    and their horizontal gradients' magnitudes are checked to see if they exceed a threshold.

    The function accepts a user-defined threshold, or estimates an appropriate outlier threshold
    based on the distribution of the gradients if threshold provided is NaN.

    @param NX, NY, PX, PY   - These fields represent the normal vectors or data arrays
    @param options          - A structure that can contain the options

    @return The input fieds are modified in-place.
    If any normal vectors exceed the outlier threshold, the corresponding fields (e.g., NX, NY, PX, PY)
    are updated by setting the outlier values to NaN.
    */
    NanoTopoData MaskOutlierNormal(cv::Mat& NX, cv::Mat& NY, cv::Mat& PX, cv::Mat& PY, const MaskOutlierNormalOptions& options);

    /*
    * Downsampling input data by averaging
    */
    SubsampledData Subsampling(NanoTopoInput& data);

    /*
    * Downsampling input data using pixel area relation
    */
    SubsampledData SubsamplingOpenCV(NanoTopoInput& data);

    cv::Mat GetNonValidMask(const cv::Mat& PX, const std::vector<int>& validColumnIndices, const std::vector<int>& validRowIndices);
}