#include "CFilters.hpp"
#include "ErrorLogging.hpp"

#include <opencv2/imgproc.hpp>

#pragma unmanaged

using namespace cv;
using namespace std;

namespace filter {
    namespace {
        /**
         * Garcia Lorca operators are one-dimensional operators of order one, used in cascade :
         *   - causal filter : y(n) = (1-γ)*x(n) + γ*y(n-1)
         *   - anti-causal filter : y(n) = (1-γ)*x(n) + γ*y(n+1)
         *   - with γ = exp(-α)
         *  By applying this cascade filter once, we obtain a good approximation of Shen's one-dimensional filter.
         *  By applying it twice, we obtain a good approximation of the Deriche's one-dimensional filter.
         *  Two-dimensional filters are obtained by horizontal and vertical application of one-dimensional filters.
         *
         * @param img   - input two-dimensional signal
         * @param gamma   - γ = exp(-α)
         *
         * @result Filtered image
         */
        template <typename T> Mat GLSmoothingDeriche2D(const Mat& img, float gamma);
        template <typename T> Mat GLSmoothingShen2D(const Mat& img, float gamma);

        /** Causal smoothing filter (Garcia Lorca operator) : y(n) = (1-γ)*x(n) + γ*y(n-1)
         * @param x      - input 1D signal
         * @param y      - filtered 1D signal
         * @param n      - size of the input signal
         * @param gamma  - expt(-α)
         * @param stride - the step
         */
        template <typename T> static void CausalFilter(const T* x, T* y, uint16_t n, float gamma, uint16_t stride);

        /** Anti-causal smoothing filter (Garcia Lorca operator) : y(n) = (1-γ)*x(n) + γ*y(n+1)
         * @param x      - input 1D signal
         * @param y      - filtered 1D signal
         * @param n      - size of the input signal
         * @param gamma  - expt(-α)
         * @param stride - the step
         */
        template <typename T> static void AntiCausalFilter(const T* x, T* y, uint16_t n, float gamma, uint16_t stride);

        /**
         * Apply causal smoothing filter * 2.
         */
        template <typename T> void DualCausalFilter(const T* x, T* y, uint16_t n, float gamma, uint16_t stride);

        /**
         * Apply anti-causal smoothing filter * 2.
         */
        template <typename T> void DualAntiCausalFilter(const T* x, T* y, uint16_t n, float gamma, uint16_t stride);

        /**
         * Approximation of the Deriche's one-dimensional filter by Garcia-Lorca operators :
         * Apply causal smoothing filter * 2 and anti-causal smoothing filter * 2.
         */
        template <typename T> void GLSmoothingDeriche1D(const T* x, T* y, uint16_t n, float gamma, uint16_t stride);

        /**
         * Approximation of the Shen's one-dimensional filter by Garcia-Lorca operators :
         * Apply causal smoothing filter and anti-causal smoothing filter.
         */
        template <typename T> void GLSmoothingShen1D(const T* x, T* y, uint16_t n, float gamma, uint16_t stride);
    } // namespace

    Mat DericheBlur(const Mat& img, float gamma) {
        switch (img.depth()) {
        case CV_32F:
            return GLSmoothingDeriche2D<float>(img, gamma);
        case CV_8U:
            return GLSmoothingDeriche2D<uint8_t>(img, gamma);
        case CV_16S:
            return GLSmoothingDeriche2D<int16_t>(img, gamma);
        default:
            ErrorLogging::LogErrorAndThrow("[Deriche smoothing filter] Data type unsupported : ", img.type());
        }
        return Mat();
    }

    Mat ShenBlur(const Mat& img, float gamma) {
        switch (img.depth()) {
        case CV_32F:
            return GLSmoothingShen2D<float>(img, gamma);
        case CV_8U:
            return GLSmoothingShen2D<uint8_t>(img, gamma);
        case CV_16S:
            return GLSmoothingShen2D<int16_t>(img, gamma);
        default:
            ErrorLogging::LogErrorAndThrow("[Shen smoothing filter] Data type unsupported : ", img.type());
        }
        return Mat();
    }

    Mat CannySobel(const Mat& img, int cannyThreshold)
    {
        Mat dx, dy, edges;
        int kernelSize = 3;

        switch (img.depth()) {
        case CV_32F:
            Sobel(img, dx, CV_32F, 1, 0, kernelSize, 1, 0, BORDER_REPLICATE);
            Sobel(img, dy, CV_32F, 0, 1, kernelSize, 1, 0, BORDER_REPLICATE);
            Canny(dx, dy, edges, std::max(1, cannyThreshold / 2), cannyThreshold, false);
        case CV_8U:
            Sobel(img, dx, CV_8U, 1, 0, kernelSize, 1, 0, BORDER_REPLICATE);
            Sobel(img, dy, CV_8U, 0, 1, kernelSize, 1, 0, BORDER_REPLICATE);
            Canny(dx, dy, edges, std::max(1, cannyThreshold / 2), cannyThreshold, false);
        case CV_16S:
            Sobel(img, dx, CV_16S, 1, 0, kernelSize, 1, 0, BORDER_REPLICATE);
            Sobel(img, dy, CV_16S, 0, 1, kernelSize, 1, 0, BORDER_REPLICATE);
            Canny(dx, dy, edges, std::max(1, cannyThreshold / 2), cannyThreshold, false);
        default:
            ErrorLogging::LogErrorAndThrow("[Canny Dobel filter] Data type unsupported : ", img.type());
        }

        return edges;
    }

    Mat CannyScharr(const Mat& img, int cannyThreshold)
    {
        Mat dx, dy, edges;

        switch (img.depth()) {
        case CV_32F:
            Scharr(img, dx, CV_32F, 1, 0);
            Scharr(img, dy, CV_32F, 0, 1);
            Canny(dx, dy, edges, std::max(1, cannyThreshold / 2), cannyThreshold, true);
        case CV_8U:
            Scharr(img, dx, CV_8U, 1, 0);
            Scharr(img, dy, CV_8U, 0, 1);
            Canny(dx, dy, edges, std::max(1, cannyThreshold / 2), cannyThreshold, true);
        case CV_16S:
            Scharr(img, dx, CV_16S, 1, 0);
            Scharr(img, dy, CV_16S, 0, 1);
            Canny(dx, dy, edges, std::max(1, cannyThreshold / 2), cannyThreshold, true);
        default:
            ErrorLogging::LogErrorAndThrow("[Canny Dobel filter] Data type unsupported : ", img.type());
        }

        return edges;
    }

    Mat Highpass(Size size) {
        Mat a = Mat(size.height, 1, CV_32FC1);
        Mat b = Mat(1, size.width, CV_32FC1);

        double stepY = CV_PI / size.height;
        double valueY = -CV_PI * 0.5;
        for (int i = 0; i < size.height; ++i) {
            a.at<float>(i) = static_cast<float>(cos(valueY)); //possible loss of data
            valueY += stepY;
        }

        double stepX = CV_PI / size.width;
        double valueX = -CV_PI * 0.5;
        for (int i = 0; i < size.width; ++i) {
            b.at<float>(i) = static_cast<float>(cos(valueX)); //possible loss of data
            valueX += stepX;
        }

        Mat tmp = a * b;
        Mat highpassFilter = (1.0 - tmp).mul(2.0 - tmp);
        return highpassFilter;
    }

    Mat HanningApodization(const Mat& src) {
        if (src.type() != CV_32FC1 && src.type() != CV_64FC1) {
            ErrorLogging::LogErrorAndThrow("[Hanning apodization] Input image type must be single channel: 32 - bits float or 64 - bits float");
        }
        Mat hann_window;
        createHanningWindow(hann_window, src.size(), src.type());
        return src.mul(hann_window);
    }

    void NoiseRemovedBelowThreshold(Mat& src)
    {
        cv::Mat blurredImg;
        cv::Mat binaryMask;
        int gaussianBlurSize = 3;
        cv::GaussianBlur(src, blurredImg, cv::Size(gaussianBlurSize, gaussianBlurSize), 0);
        cv::threshold(blurredImg, binaryMask, 0, 1, cv::THRESH_OTSU);
        src = src.mul(binaryMask);
        return;
    }

    namespace {
        template <typename T> static void CausalFilter(const T* x, T* y, uint16_t n, float gamma, uint16_t stride) {
            double accumulator = x[0];
            for (uint16_t i = 0; i < n; i++) {
                accumulator = (1.0 - gamma) * x[i * stride] + gamma * accumulator;
                y[i * stride] = (T)accumulator;
            }
        }

        template <typename T> static void AntiCausalFilter(const T* x, T* y, uint16_t n, float gamma, uint16_t stride) {
            double accumulator = x[(n - 1) * stride];
            for (signed short i = n - 1; i >= 0; i--) {
                accumulator = (1.0 - gamma) * x[i * stride] + gamma * accumulator;
                y[i * stride] = (T)accumulator;
            }
        }

        template <typename T> void DualCausalFilter(const T* x, T* y, uint16_t n, float gamma, uint16_t stride) {
            CausalFilter<T>(x, y, n, gamma, stride);
            CausalFilter<T>(x, y, n, gamma, stride);
        }

        template <typename T> void DualAntiCausalFilter(const T* x, T* y, uint16_t n, float gamma, uint16_t stride) {
            AntiCausalFilter<T>(x, y, n, gamma, stride);
            AntiCausalFilter<T>(x, y, n, gamma, stride);
        }

        template <typename T> void GLSmoothingDeriche1D(const T* x, T* y, uint16_t n, float gamma, uint16_t stride) {
            DualCausalFilter<T>(x, y, n, gamma, stride);
            DualAntiCausalFilter<T>(x, y, n, gamma, stride);
        }

        template <typename T> void GLSmoothingShen1D(const T* x, T* y, uint16_t n, float gamma, uint16_t stride) {
            CausalFilter<T>(x, y, n, gamma, stride);
            AntiCausalFilter<T>(x, y, n, gamma, stride);
        }

        template <typename T> Mat GLSmoothingDeriche2D(const cv::Mat& input, float gamma) {
            uint16_t nbCols = input.cols;
            uint16_t nbRows = input.rows;
            uint16_t nbChannels = input.channels();

            // Transposition of the input signal
            cv::Mat filteredSignal = input.t();

            // Vertical filtering with causal smoothing filter * 2 and anti-causal smoothing filter * 2.
            for (auto c = 0; c < nbChannels; c++) {
                for (uint16_t x = 0u; x < nbCols; x++)
                    GLSmoothingDeriche1D<T>(filteredSignal.ptr<T>(x) + c, filteredSignal.ptr<T>(x) + c, nbRows, gamma, nbChannels); // ligne x
            }

            // Transposition of the vertically filtered matrix
            Mat imgFiltered = filteredSignal.t();

            // Horizontal filtering with causal smoothing filter * 2 and anti-causal smoothing filter * 2.
            for (auto c = 0; c < nbChannels; c++) {
                for (uint16_t y = 0u; y < nbRows; y++)
                    GLSmoothingDeriche1D<T>(imgFiltered.ptr<T>(y) + c, imgFiltered.ptr<T>(y) + c, nbCols, gamma, nbChannels);
            }

            return imgFiltered;
        }

        template <typename T> Mat GLSmoothingShen2D(const cv::Mat& input, float gamma) {
            uint16_t nbCols = input.cols;
            uint16_t nbRows = input.rows;
            uint16_t nbChannels = input.channels();

            // Transposition of the input signal
            cv::Mat filteredSignal = input.t();

            // Vertical filtering with causal smoothing filter * 2 and anti-causal smoothing filter * 2.
            for (auto c = 0; c < nbChannels; c++) {
                for (uint16_t x = 0u; x < nbCols; x++)
                    GLSmoothingShen1D<T>(filteredSignal.ptr<T>(x) + c, filteredSignal.ptr<T>(x) + c, nbRows, gamma, nbChannels); // ligne x
            }

            // Transposition of the vertically filtered matrix
            Mat imgFiltered = filteredSignal.t();

            // Horizontal filtering with causal smoothing filter * 2 and anti-causal smoothing filter * 2.
            for (auto c = 0; c < nbChannels; c++) {
                for (uint16_t y = 0u; y < nbRows; y++)
                    GLSmoothingShen1D<T>(imgFiltered.ptr<T>(y) + c, imgFiltered.ptr<T>(y) + c, nbCols, gamma, nbChannels);
            }

            return imgFiltered;
        }
    } // namespace
} // namespace filter