#pragma once

#include <opencv2/core/mat.hpp>

#pragma unmanaged
namespace fourier_transform {
    /**
     * @brief Compute discret frourier transform to obtain complex matrix
     *
     * @param source the source matrix
     * @return the result complex matrix
     */
    cv::Mat DiscretFourierTransform(const cv::Mat& source);

    /**
     * @brief Compute the magnitude spectrum from the complex values ​​obtained by the fourier transform
     * @details You have to calculate the fourier transform before applying this function on the result.
     * the magnitude spectrum is represented with swap quadrant (see fftShift function)
     *
     * @param p_complex the source matrix contained complex values
     * @param p_rowDft if the complex matrix was computed only using the dft of individual rows
     * @return the result matrix contained the magnitude spectrum
     */
    cv::Mat MagnitudeSpectrum(const cv::Mat& complex, bool rowDft = false);

    /**
     * @brief Compute the magnitude spectrum from the complex values ​​obtained by the fourier transform
     * @details You have to calculate the fourier transform before applying this function on the result.
     * the magnitude spectrum is represented with logarithmic scale and swap quadrant (see fftShift function)
     *
     * @param p_complex the source matrix contained complex values
     * @param p_rowDft if the complex matrix was computed only using the dft of individual rows
     * @return the result matrix contained the magnitude spectrum in logarithmic scale
     */
    cv::Mat LogMagnitudeSpectrum(const cv::Mat& complex, bool rowDft = false);

    /**
     * @brief Compute the phase spectrum from the complex values ​​obtained by the fourier transform
     * @details You have to calculate the fourier transform before applying this function on the result.
     * the phase spectrum is represented with swap quadrant (see fftShift function)
     *
     * @param p_complex the source matrix contained complex values
     * @return the result matrix contained the phase spectrum
     */
    cv::Mat PhaseSpectrum(const cv::Mat& complex);
}