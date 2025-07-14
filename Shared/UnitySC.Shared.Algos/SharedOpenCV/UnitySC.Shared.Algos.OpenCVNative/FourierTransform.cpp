#include <opencv2/core.hpp>

#include "fouriertransform.hpp"

using namespace cv;
using namespace std;

#pragma unmanaged
namespace fourier_transform
{
    namespace {
        /**
         * @brief Rearrange the halves of Fourier image (if it was computed only row by row indivudually) so that the origin is at the image center
         * @details You have to calculate the magnitude or phase spectrum before applying this function on the result.
         *
         * @param spectrum the matrix to swap
         */
        void ShiftSpectrumHalves(cv::Mat& spectrum);
        /**
         * @brief Rearrange the quadrants of Fourier image so that the origin is at the image center
         * @details You have to calculate the magnitude or phase spectrum before applying this function on the result.
         *
         * @param spectrum the matrix to swap
         */
        void ShiftSpectrumQuadrants(cv::Mat& spectrum);
    }

    Mat DiscretFourierTransform(const Mat& source) {
        Mat complex;

        // Expand input image to optimal size
        Mat padded;
        int m = getOptimalDFTSize(source.rows);
        int n = getOptimalDFTSize(source.cols);

        // On the border add zero values
        copyMakeBorder(source, padded, 0, m - source.rows, 0, n - source.cols, BORDER_CONSTANT, Scalar::all(0));

        // Perform DFT
        Mat planes[] = { Mat_<float>(padded), Mat::zeros(padded.size(), CV_32F) };
        merge(planes, 2, complex); // add to the expanded another plane with zeros
        dft(complex, complex, DFT_COMPLEX_OUTPUT); // this way the result may fit in the source matrix

        return complex;
    }

    Mat MagnitudeSpectrum(const Mat& complex, bool rowDft) {
        Mat magnitudeImg;

        // Split the complex (2 channel) image in two separate images containing the real and imaginary part
        Mat planes[2];
        split(complex, planes); // planes[0] = Re(DFT(I)), planes[1] = Im(DFT(I))

        // Get the magnitude : log(1 + sqrt(Re(DFT(I))^2 + Im(DFT(I))^2)) => magnitude = sqrt(Re(DFT(I))^2 + Im(DFT(I))^2))
        magnitude(planes[0], planes[1], magnitudeImg);

        // rearrage quadrants (or halves)
        if (rowDft)
        {
            ShiftSpectrumHalves(magnitudeImg);
        }
        else
        {
            ShiftSpectrumQuadrants(magnitudeImg);
        }

        return magnitudeImg;
    }

    Mat LogMagnitudeSpectrum(const Mat& complex, bool rowDft) {
        Mat magnitudeImg;

        // Split the complex (2 channel) image in two separate images containing the real and imaginary part
        Mat planes[2];
        split(complex, planes); // planes[0] = Re(DFT(I)), planes[1] = Im(DFT(I))

        // Get the magnitude : log(1 + sqrt(Re(DFT(I))^2 + Im(DFT(I))^2)) => magnitude = sqrt(Re(DFT(I))^2 + Im(DFT(I))^2))
        magnitude(planes[0], planes[1], magnitudeImg);

        // switch to logarithmic scale: log(1 + magnitude)
        magnitudeImg += Scalar::all(1);
        log(magnitudeImg, magnitudeImg);

        // rearrage quadrants (or halves)
        if (rowDft)
        {
            ShiftSpectrumHalves(magnitudeImg);
        }
        else
        {
            ShiftSpectrumQuadrants(magnitudeImg);
        }

        return magnitudeImg;
    }

    Mat PhaseSpectrum(const Mat& complex) {
        Mat phaseImg;

        // Split the complex (2 channel) image in two separate images containing the real and imaginary part
        Mat planes[2];
        split(complex, planes);

        // Get the phase (angle measured in radians, from 0 to 2*Pi): log(1 + sqrt(Re(DFT(I))^2 + Im(DFT(I))^2))
        phase(planes[0], planes[1], phaseImg, false);

        // rearrage quadrants
        ShiftSpectrumQuadrants(phaseImg);

        return phaseImg;
    }

    namespace {
        void ShiftSpectrumQuadrants(Mat& spectrum) {
            // crop if it has an odd number of rows or columns
            spectrum = spectrum(Rect(0, 0, spectrum.cols & -2, spectrum.rows & -2));

            // rearrange the quadrants of Fourier image  so that the origin is at the image center
            int cx = spectrum.cols / 2;
            int cy = spectrum.rows / 2;

            Mat q0(spectrum, Rect(0, 0, cx, cy));   // Top-Left - Create a ROI per quadrant
            Mat q1(spectrum, Rect(cx, 0, cx, cy));  // Top-Right
            Mat q2(spectrum, Rect(0, cy, cx, cy));  // Bottom-Left
            Mat q3(spectrum, Rect(cx, cy, cx, cy)); // Bottom-Right

            Mat tmp;
            // swap quadrants (Top-Left with Bottom-Right)
            q0.copyTo(tmp);
            q3.copyTo(q0);
            tmp.copyTo(q3);
            // swap quadrant (Top-Right with Bottom-Left)
            q1.copyTo(tmp);
            q2.copyTo(q1);
            tmp.copyTo(q2);
        }

        void ShiftSpectrumHalves(Mat& spectrum) 
        {
            // crop if it has an odd number of rows or columns
            spectrum = spectrum(Rect(0, 0, spectrum.cols & -2, spectrum.rows & -2));

            // rearrange the quadrants of Fourier image  so that the origin is at the image center
            int cx = spectrum.cols / 2;

            Mat h0(spectrum, Rect(0, 0, cx, spectrum.rows));
            Mat h1(spectrum, Rect(cx, 0, cx, spectrum.rows));

            Mat tmp;

            // swap halves (Left with Right)
            h0.copyTo(tmp);
            h1.copyTo(h0);
            tmp.copyTo(h1);
        }
    }
}