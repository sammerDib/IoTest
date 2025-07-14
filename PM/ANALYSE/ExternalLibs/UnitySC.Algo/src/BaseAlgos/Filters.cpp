#include <BaseAlgos/Filters.hpp>
#include <Logger.hpp>

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
            stringstream strStrm;
            strStrm << "[Deriche smoothing filter] Data type unsupported : " << img.type();
            string message = strStrm.str();
            Logger::Error(message);
            throw exception(message.c_str());
        }
    }

    Mat Highpass(Size size) {
        Mat a = Mat(size.height, 1, CV_32FC1);
        Mat b = Mat(1, size.width, CV_32FC1);

        double stepY = CV_PI / size.height;
        double valueY = -CV_PI * 0.5;
        for (int i = 0; i < size.height; ++i) {
            a.at<float>(i) = cos(valueY);
            valueY += stepY;
        }

        double stepX = CV_PI / size.width;
        double valueX = -CV_PI * 0.5;
        for (int i = 0; i < size.width; ++i) {
            b.at<float>(i) = cos(valueX);
            valueX += stepX;
        }

        Mat tmp = a * b;
        Mat highpassFilter = (1.0 - tmp).mul(2.0 - tmp);
        return highpassFilter;
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
            CausalFilter<T>(x, y, n, gamma, stride);
            AntiCausalFilter<T>(x, y, n, gamma, stride);
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
    } // namespace
} // namespace filter