#pragma once

#include <opencv2/core/core.hpp>

namespace filter {
    /**
     * Deriche smoothing by Garcia Lorca operators (low-pass filter).
     *
     * Deriche filter is very similar to Gaussian filter, but much simpler to implement (based on simple first order IIR filters).
     * Indeed, contrary to a gaussian filter that is often implemented using a FIR (finite response) filter,
     * and which complexity is directly dependant on the desired filtering level (standard deviation sigma),
     * for a first order IIR, which equation is: y[n] = α*x[n] + (1-α)*y[n-1], the complexity is constant and very limited (2 multiplications per pixel),
     * and the filtering level can be arbitrary modified through the "forget factor" α, also called alpha.
     *
     * The advantage of such a filter is that it can be adapted to the characteristics of the processed image using only one parameter : alpha.
     *  - If the value of α is small (usually between 0.25 and 0.5), it results in better detection.
     *  - On the other hand, better localization is achieved when the parameter has a higher value (around 2 or 3).
     *  - For most of the normal cases, parameter value of around 1 is recommended.
     * Using the IIR filter makes sense especially in cases where the processed image is noisy or a large amount of smoothing is required
     * (which leads to large convolution kernel for FIR filter). In these cases, the Deriche detector has considerable advantage over the Canny detector,
     * because it is able to process images in a short constant time independent of the desired amount of smoothing.
     *
     * @param img     - input two-dimensional signal
     * @param gamma   - γ = exp(-α)
     *
     * @return Filtered image
     */
    cv::Mat DericheBlur(const cv::Mat& img, float gamma);

    /**
     * Create an highpass filter
     *
     * @param size    - size of the filter matrix
     *
     * @return Computed filter matrix
     */
    cv::Mat Highpass(cv::Size size);
} // namespace filter