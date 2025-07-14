#include "NanoTopography.hpp"
#include <numeric>

using namespace std;
using namespace cv;
namespace fs = std::filesystem;

#pragma unmanaged
namespace psd {
    namespace {
        struct ValidIndice {
            int First;
            int Last;
        };

        /*
        The function to subsample a range of indices from firstIndice to lastIndice by dividing them
        into groups of ncoarse elements

        @param firstIndice      - The starting indice of a range.
        @param lastIndice       - The ending indice of a range.
        @param ncoarse          - The number of elements in each group for the subsampling operation.

        @return The function returns a list of values, which is the result of performing the subsampling operation
        on the range defined by firstIndice and lastIndice, with the subsampling step size of ncoarse.
        */
        vector<int> GroupIndices(int firstIndice, int lastIndice, int ncoarse);

        /*
        Computes the average of values grouped by bin indices
        @param binIndices       - Bin indice associated with each value provided in the second parameter
        @param values           - Values to bin for averaging

        @return Average of the values ​​grouped in each bin
        */
        vector<float> AverageByBinWiseAccumulation(const vector<int>& binIndices, const vector<int>& values);

        /*
        Computes the average of values grouped by bin indices
        @param binRowIndices       - Bin row indice associated with each value provided in the matrix
        @param binColumnIndices    - Bin column indice associated with each value provided in the matrix
        @param data                - Matrix containing data to averaged
        @param mask                - Mask defining which data are valid in data matrix

        @return Matrix containing average of the values grouped in each bin
        */
        Mat AverageByBinWiseAccumulation(const vector<int>& binRowIndices, const vector<int>& binColumnIndices,
            const Mat& data, const Mat& mask);

        /*
        */
        vector<int> SubsamplingIndices(const vector<int>& groupIndices, const vector<int>& validIndices);

        /*
        Compute first and last valid column (not NaN or Inf value)

        @param data     - Matrix of float values

        @return First and last valid indice
        */
        ValidIndice ComputeFirstAndLastValidColumn(const cv::Mat& data)
        {
            ValidIndice indice;
            indice.First = -1;
            indice.Last = -1;
            for (int j = 0; j < data.cols; ++j) {
                for (int i = 0; i < data.rows; ++i) {
                    if (std::isfinite(data.at<float>(i, j))) {
                        indice.First = (indice.First == -1) ? j : indice.First;
                        indice.Last = j;
                    }
                }
            }
            return indice;
        }

        /*
        Compute first and last valid row (not NaN or Inf value)

        @param data         - Matrix of float values

        @return First and last valid indice
        */
        ValidIndice ComputeFirstAndLastValidRow(const cv::Mat& data)
        {
            ValidIndice indice;
            indice.First = -1;
            indice.Last = -1;
            for (int i = 0; i < data.rows; ++i) {
                for (int j = 0; j < data.cols; ++j) {
                    if (std::isfinite(data.at<float>(i, j))) {
                        indice.First = (indice.First == -1) ? i : indice.First;
                        indice.Last = i;
                    }
                }
            }
            return indice;
        }
    }

    NanoTopoData MaskOutlierNormal(cv::Mat& NX, cv::Mat& NY, cv::Mat& PX, cv::Mat& PY, const MaskOutlierNormalOptions& options)
    {
        NanoTopoData result;
        result.NX = NX.clone();
        result.NY = NY.clone();
        result.PX = PX.clone();
        result.PY = PY.clone();

        float maxAllowedGrad = options.maxAllowedGrad;
        float outlierIQRScale = options.outlierIQRScale;
        float threshold;

        cv::Mat NXY2 = NX.mul(NX) + NY.mul(NY);

        if (std::isnan(maxAllowedGrad)) {
            // Automatically calculates a threshold for outliers using 1.5 * IQR rule
            std::vector<float> values;
            for (int i = 0; i < NXY2.rows; ++i) {
                for (int j = 0; j < NXY2.cols; ++j) {
                    if (std::isfinite(NXY2.at<float>(i, j))) {
                        values.push_back(NXY2.at<float>(i, j));
                    }
                }
            }

            std::sort(values.begin(), values.end());

            int q1_idx = values.size() * 0.25;
            int q3_idx = values.size() * 0.75;
            float Q1 = values[q1_idx];
            float Q3 = values[q3_idx];
            float IQR = Q3 - Q1;

            threshold = Q3 + outlierIQRScale * IQR;
        }
        else {
            // Use provided maxAllowedGrad to calculates a threshold for outliers
            threshold = maxAllowedGrad * maxAllowedGrad;
        }

        if (std::isfinite(threshold)) {
            cv::Mat NaNmask = NXY2 > threshold;

            if (cv::countNonZero(NaNmask) > 0) {
                result.NX.setTo(NAN, NaNmask);
                result.NY.setTo(NAN, NaNmask);
                result.PX.setTo(NAN, NaNmask);
                result.PY.setTo(NAN, NaNmask);
            }
        }

        return result;
    }

    SubsampledData Subsampling(NanoTopoInput& data)
    {
        SubsampledData result;

        ValidIndice validCol = ComputeFirstAndLastValidColumn(data.Data.NX);
        int firstCol = validCol.First;
        int lastCol = validCol.Last;

        ValidIndice validRow = ComputeFirstAndLastValidRow(data.Data.NX);
        int firstRow = validRow.First;
        int lastRow = validRow.Last;

        std::vector<int> validColumnIndices(lastCol - firstCol + 1);
        std::generate(validColumnIndices.begin(), validColumnIndices.end(), [n = firstCol]()mutable {return n++; });
        std::vector<int> validRowIndices(lastRow - firstRow + 1);
        std::generate(validRowIndices.begin(), validRowIndices.end(), [n = firstRow]()mutable {return n++; });
        result.ValidColumnIndices = validColumnIndices;
        result.ValidRowIndices = validRowIndices;

        std::vector<int> groupColumnIndices;
        groupColumnIndices = GroupIndices(firstCol, lastCol, data.ColumnElementNumberByGroup);
        std::vector<int> groupRowIndices;
        groupRowIndices = GroupIndices(firstRow, lastRow, data.RowElementNumberByGroup);

        std::vector<int> subsampledColumnIndices = SubsamplingIndices(groupColumnIndices, validColumnIndices);
        std::vector<int> subsampledRowIndices = SubsamplingIndices(groupRowIndices, validRowIndices);
        result.SubsampledColumnIndices = subsampledColumnIndices;
        result.SubsampledRowIndices = subsampledRowIndices;

        // Computes the average of values (firstCol:lastCol) grouped by indices x for J
        std::vector<float> J = AverageByBinWiseAccumulation(groupColumnIndices, validColumnIndices);
        result.J = J;

        // Computes the average of values (firstRow:lastRow) grouped by indices y for I
        std::vector<float> I = AverageByBinWiseAccumulation(groupRowIndices, validRowIndices);
        result.I = I;

        // Subsampling input matrix by averaging

        cv::Mat submatrixNx(validRowIndices.size(), validColumnIndices.size(), CV_32F);
        cv::Mat submatrixNy(validRowIndices.size(), validColumnIndices.size(), CV_32F);
        cv::Mat submatrixPx(validRowIndices.size(), validColumnIndices.size(), CV_32F);
        cv::Mat submatrixPy(validRowIndices.size(), validColumnIndices.size(), CV_32F);
        cv::Mat submatrixMask = cv::Mat::zeros(submatrixNx.size(), CV_8U);
        for (size_t i = 0; i < validRowIndices.size(); ++i) {
            for (size_t j = 0; j < validColumnIndices.size(); ++j) {
                submatrixNx.at<float>(i, j) = data.Data.NX.at<float>(validRowIndices[i], validColumnIndices[j]);
                submatrixNy.at<float>(i, j) = data.Data.NY.at<float>(validRowIndices[i], validColumnIndices[j]);
                submatrixPx.at<float>(i, j) = data.Data.PX.at<float>(validRowIndices[i], validColumnIndices[j]);
                submatrixPy.at<float>(i, j) = data.Data.PY.at<float>(validRowIndices[i], validColumnIndices[j]);
                submatrixMask.at<uchar>(i, j) = !std::isnan(submatrixNx.at<float>(i, j)) ? 1 : 0;
            }
        }

        cv::Mat averagedNx = AverageByBinWiseAccumulation(groupRowIndices, groupColumnIndices, submatrixNx, submatrixMask);
        result.AveragedNX = averagedNx;
        cv::Mat averagedNy = AverageByBinWiseAccumulation(groupRowIndices, groupColumnIndices, submatrixNy, submatrixMask);
        result.AveragedNY = averagedNy;
        cv::Mat averagedPx = AverageByBinWiseAccumulation(groupRowIndices, groupColumnIndices, submatrixPx, submatrixMask);
        result.AveragedPX = averagedPx;
        cv::Mat averagedPy = AverageByBinWiseAccumulation(groupRowIndices, groupColumnIndices, submatrixPy, submatrixMask);
        result.AveragedPY = averagedPy;

        cv::Mat Ic(validRowIndices.size(), validColumnIndices.size(), CV_32F);  // Grid for row indices
        cv::Mat Jc(validRowIndices.size(), validColumnIndices.size(), CV_32F);  // Grid for column indices
        for (int i = 0; i < validRowIndices.size(); ++i) {
            for (int j = 0; j < validColumnIndices.size(); ++j) {
                Ic.at<float>(i, j) = validRowIndices[i];
                Jc.at<float>(i, j) = validColumnIndices[j];
            }
        }

        cv::Mat averagedJc = AverageByBinWiseAccumulation(groupRowIndices, groupColumnIndices, Jc, submatrixMask);
        result.Jc = averagedJc;

        cv::Mat averagedIc = AverageByBinWiseAccumulation(groupRowIndices, groupColumnIndices, Ic, submatrixMask);
        result.Ic = averagedIc;

        return result;
    }

    SubsampledData SubsamplingOpenCV(NanoTopoInput& data)
    {
        SubsampledData result;

        ValidIndice validCol = ComputeFirstAndLastValidColumn(data.Data.NX);
        int firstCol = validCol.First;
        int lastCol = validCol.Last;
        int sampleSizeWidth = std::floor((lastCol - firstCol) / data.ColumnElementNumberByGroup);

        ValidIndice validRow = ComputeFirstAndLastValidRow(data.Data.NX);
        int firstRow = validRow.First;
        int lastRow = validRow.Last;
        int sampleSizeHeight = std::floor((lastRow - firstRow) / data.RowElementNumberByGroup);

        Size downsampleSize = cv::Size(sampleSizeWidth, sampleSizeHeight);

        Mat downsampledNx = Mat::zeros(downsampleSize, CV_32F);
        cv::resize(data.Data.NX, downsampledNx, downsampleSize, 0, 0, INTER_AREA);
        Mat downsampledNy = Mat::zeros(downsampleSize, CV_32F);
        cv::resize(data.Data.NY, downsampledNy, downsampleSize, 0, 0, INTER_AREA);
        Mat downsampledPx = Mat::zeros(downsampleSize, CV_32F);
        cv::resize(data.Data.PX, downsampledPx, downsampleSize, 0, 0, INTER_AREA);
        Mat downsampledPy = Mat::zeros(downsampleSize, CV_32F);
        cv::resize(data.Data.PY, downsampledPy, downsampleSize, 0, 0, INTER_AREA);

        result.AveragedNX = downsampledNx;
        result.AveragedNY = downsampledNy;
        result.AveragedPX = downsampledPx;
        result.AveragedPY = downsampledPy;

        return result;
    }

    cv::Mat GetNonValidMask(const cv::Mat& PX, const std::vector<int>& validColumnIndices, const std::vector<int>& validRowIndices) {
        // Step 1: Create grids for xi and yi
        cv::Mat xi, yi;
        cv::repeat(cv::Mat(validColumnIndices).reshape(1, 1), validRowIndices.size(), 1, xi);
        cv::repeat(cv::Mat(validRowIndices).reshape(1, 1).t(), 1, validColumnIndices.size(), yi);

        // Step 2: Find invalid indices
        cv::Mat mask = cv::Mat::zeros(PX.size(), CV_8U);
        for (size_t row = 0; row < validRowIndices.size(); row++) {
            for (size_t col = 0; col < validColumnIndices.size(); col++) {
                float value = PX.at<float>(validRowIndices[row], validColumnIndices[col]);
                if (!std::isfinite(PX.at<float>(validRowIndices[row], validColumnIndices[col])))
                {
                    mask.at<uchar>(validRowIndices[row], validColumnIndices[col]) = 1;
                }
            }
        }

        return mask;
    }

    namespace {
        std::vector<int> GroupIndices(int firstIndice, int lastIndice, int ncoarse) {
            int size = lastIndice - firstIndice + 1;
            std::vector<int> indices(size), result(size);
            std::vector<float> i(size);
            std::generate(indices.begin(), indices.end(), [n = 0]()mutable {return n++; });
            std::transform(indices.begin(), indices.end(), i.begin(), [ncoarse](int val) { return float(val) / ncoarse; });

            float a = std::fmod(float(size) / ncoarse, 1.0f);
            //float a = std::fmod(float(i9 - i1) / ncoarse, 1.0f); // For Matlab results comparaison
            float s = (a == 0) ? 1.0f : (2.0f - a / 2.0f);
            std::transform(i.begin(), i.end(), result.begin(), [s](float val) { return std::floor(val + s); });

            return result;
        }

        std::vector<float> AverageByBinWiseAccumulation(const std::vector<int>& binIndices, const std::vector<int>& values) {
            std::vector<float> averageValues;

            if (binIndices.size() == values.size()) {
                int binsNb = binIndices.back();
                std::vector<float> ones(binIndices.size(), 1.0);

                std::map<int, float> binMap;
                std::map<int, int> binCount;

                for (size_t i = 0; i < binIndices.size(); ++i) {
                    binMap[binIndices[i]] += values[i];
                    binCount[binIndices[i]] += ones[i];
                }

                for (size_t i = 1; i <= binMap.size(); ++i) {
                    binMap[i] /= binCount[i];
                }
                binMap[0] = values[0];
                binMap[binMap.size() + 1] = values.back();

                for (std::map<int, float>::iterator it = binMap.begin(); it != binMap.end(); ++it) {
                    averageValues.push_back(it->second);
                }
            }
            return averageValues;
        }

        std::vector<int> SubsamplingIndices(const std::vector<int>& groupIndices, const std::vector<int>& validIndices)
        {
            std::vector<int> diffBetweenGroupIndices(groupIndices.size());
            std::adjacent_difference(groupIndices.begin(), groupIndices.end(), diffBetweenGroupIndices.begin());
            diffBetweenGroupIndices.erase(diffBetweenGroupIndices.begin()); // adjacent_difference adds a dummy first element

            std::vector<bool> mask(validIndices.size(), false);
            mask[0] = true; // Always include the first element
            for (size_t i = 0; i < diffBetweenGroupIndices.size(); ++i) {
                mask[i] = (diffBetweenGroupIndices[i] != 0);
            }

            std::vector<int> subsampleIndices;
            for (size_t i = 0; i < mask.size(); ++i) {
                if (mask[i]) {
                    subsampleIndices.push_back(validIndices[i]);
                }
            }
            subsampleIndices.push_back(validIndices.back());

            return subsampleIndices;
        }

        cv::Mat AverageByBinWiseAccumulation(const std::vector<int>& binRowIndices, const std::vector<int>& binColumnIndices,
            const cv::Mat& data, const cv::Mat& mask)
        {
            int dataRowNb = data.rows;
            int dataColNb = data.cols;
            int groupRowIndicesNb = binRowIndices.size();
            int groupColIndicesNb = binColumnIndices.size();

            int rowNb = binRowIndices.back();
            int colNb = binColumnIndices.back();
            cv::Mat accumulator = cv::Mat::zeros(rowNb, colNb, CV_32F);
            cv::Mat averagedData = cv::Mat::zeros(rowNb, colNb, CV_32F);
            cv::Mat countValues = cv::Mat::zeros(rowNb, colNb, CV_32S);

            for (size_t i = 0; i < binRowIndices.size(); ++i) {
                for (size_t j = 0; j < binColumnIndices.size(); ++j) {
                    int rowNb = binRowIndices[i] - 1;
                    int colNb = binColumnIndices[j] - 1;
                    bool isValidValue = mask.at<uchar>(i, j) == 1;
                    if (isValidValue) {
                        accumulator.at<float>(rowNb, colNb) += data.at<float>(i, j);
                        countValues.at<int>(rowNb, colNb) += 1;
                    }
                }
            }

            for (size_t i = 0; i < binRowIndices.back(); ++i) {
                for (size_t j = 0; j < binColumnIndices.back(); ++j) {
                    int temp = countValues.at<int>(i, j);
                    float tempVal = accumulator.at<float>(i, j);
                    if (countValues.at<int>(i, j) != 0)
                    {
                        float average = accumulator.at<float>(i, j) / countValues.at<int>(i, j);
                        averagedData.at<float>(i, j) = average;
                    }
                }
            }

            return averagedData;
        }
    }
}