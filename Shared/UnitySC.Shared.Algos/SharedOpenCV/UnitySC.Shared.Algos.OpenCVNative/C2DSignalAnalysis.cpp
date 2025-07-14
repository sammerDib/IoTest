#include "C2DSignalAnalysis.hpp"
#include "ErrorLogging.hpp"
#include "FourierTransform.hpp"

using namespace std;
using namespace cv;

#pragma unmanaged
namespace signal_2D {
    Mat SolvePlaneEquation(Mat img, Mat mask) {
        Mat matA = Mat::zeros(img.rows * img.cols, 3, CV_32FC1);
        Mat vecB = Mat::zeros(img.rows * img.cols, 1, CV_32FC1);

        int NbStatsPts = 0;
        for (int i = 0; i < img.cols; i++) {
            for (int j = 0; j < img.rows; j++) {
                if ((!isnan(img.at<float>(j, i)))) {
                    if (mask.at<uchar>(j, i) != 0) {
                        matA.at<float>(NbStatsPts, 0) = static_cast<float>(i);  //conversion possible loss of data
                        matA.at<float>(NbStatsPts, 1) = static_cast<float>(j);  //conversion possible loss of data
                        matA.at<float>(NbStatsPts, 2) = 1;
                        vecB.at<float>(NbStatsPts, 0) = img.at<float>(j, i);
                        NbStatsPts++;
                    }
                }
            }
        }

        Mat planeCoef;
        solve(matA, vecB, planeCoef, DECOMP_SVD);

        Mat plane = Mat::zeros(img.rows, img.cols, CV_32FC1);
        float a = planeCoef.at<float>(0, 0);
        float b = planeCoef.at<float>(1, 0);
        float c = planeCoef.at<float>(2, 0);
        for (int i = 0; i < img.cols; i++) {
            for (int j = 0; j < img.rows; j++) {
                plane.at<float>(j, i) = a * i + b * j + c;
            }
        }

        return plane;
    }

    Mat SolveQuadraticEquation(Mat img, Mat mask) {
        //A second order (k=2) polynomial forms a quadratic expression (parabolic curve) : f(x,y)= a*x^2 + b*y^2 + c*x*y + d*x +  e*y + f
        int nbOfWhitePixelsInMask = cv::countNonZero(mask);
        Mat matA = Mat::zeros(Size(6, nbOfWhitePixelsInMask), CV_32FC1);
        Mat vecB = Mat::zeros(Size(1, nbOfWhitePixelsInMask), CV_32FC1);

        Mat surface = Mat::zeros(img.rows, img.cols, CV_32FC1);
        //Can't compute if the nb of pixels in the mask is less than the nb of parameters in the equation
        if (nbOfWhitePixelsInMask < 6) return surface;

        int maskIndex = 0;

        for (int row = 0; row < img.rows; row++) {
            for (int col = 0; col < img.cols; col++) {
                if ((!isnan(img.at<float>(row, col)))) {
                    if (mask.at<uchar>(row, col) != 0) {
                        float x = (col - img.cols / 2) / float(img.cols);
                        float y = (row - img.rows / 2) / float(img.rows);
                        matA.at<float>(maskIndex, 0) = x * x;
                        matA.at<float>(maskIndex, 1) = y * y;
                        matA.at<float>(maskIndex, 2) = x * y;
                        matA.at<float>(maskIndex, 3) = x;
                        matA.at<float>(maskIndex, 4) = y;
                        matA.at<float>(maskIndex, 5) = 1;
                        vecB.at<float>(maskIndex, 0) = img.at<float>(row, col);
                        maskIndex++;
                    }
                }
            }
        }

        Mat q;
        solve(matA, vecB, q, DECOMP_SVD);

        float a = q.at<float>(0, 0);
        float b = q.at<float>(1, 0);
        float c = q.at<float>(2, 0);
        float d = q.at<float>(3, 0);
        float e = q.at<float>(4, 0);
        float f = q.at<float>(5, 0);

        for (int row = 0; row < img.rows; row++)
        {
            for (int col = 0; col < img.cols; col++)
            {
                float x = (col - img.cols / 2) / float(img.cols);
                float y = (row - img.rows / 2) / float(img.rows);
                float quad = (a * x * x) + (b * y * y) + (c * x * y) + (d * x) + (e * y) + f;
                surface.at<float>(row, col) = quad;
            }
        }
        return surface;
    }

    Mat SolveCubicEquation(Mat img, Mat mask) {
        //A third order (k=3) polynomial forms a cubic expression : f(x,y)= a*x^3 + b*y^3 + c*x*y^2 + d*y*x^2 + e*x^2 + f*y^2 + g*x*y + h*x +  i*y + j
        int nbOfWhitePixelsInMask = cv::countNonZero(mask);
        Mat matA = Mat::zeros(Size(10, nbOfWhitePixelsInMask), CV_32FC1);
        Mat vecB = Mat::zeros(Size(1, nbOfWhitePixelsInMask), CV_32FC1);

        int maskIndex = 0;

        for (int row = 0; row < img.rows; row++) {
            for (int col = 0; col < img.cols; col++) {
                if ((!isnan(img.at<float>(row, col)))) {
                    if (mask.at<uchar>(row, col) != 0) {
                        float x = (col - img.cols / 2) / float(img.cols);
                        float y = (row - img.rows / 2) / float(img.rows);
                        matA.at<float>(maskIndex, 0) = x * x * x;
                        matA.at<float>(maskIndex, 1) = y * y * y;
                        matA.at<float>(maskIndex, 2) = x * y * y;
                        matA.at<float>(maskIndex, 3) = x * x * y;
                        matA.at<float>(maskIndex, 4) = x * x;
                        matA.at<float>(maskIndex, 5) = y * y;
                        matA.at<float>(maskIndex, 6) = x * y;
                        matA.at<float>(maskIndex, 7) = x;
                        matA.at<float>(maskIndex, 8) = y;
                        matA.at<float>(maskIndex, 9) = 1;
                        vecB.at<float>(maskIndex, 0) = img.at<float>(row, col);
                        maskIndex++;
                    }
                }
            }
        }

        Mat q;
        solve(matA, vecB, q, DECOMP_SVD);

        Mat surface = Mat::zeros(img.rows, img.cols, CV_32FC1);
        float a = q.at<float>(0, 0);
        float b = q.at<float>(1, 0);
        float c = q.at<float>(2, 0);
        float d = q.at<float>(3, 0);
        float e = q.at<float>(4, 0);
        float f = q.at<float>(5, 0);
        float g = q.at<float>(6, 0);
        float h = q.at<float>(7, 0);
        float i = q.at<float>(8, 0);
        float j = q.at<float>(9, 0);
        for (int row = 0; row < img.rows; row++)
        {
            for (int col = 0; col < img.cols; col++)
            {
                float x = (col - img.cols / 2) / float(img.cols);
                float y = (row - img.rows / 2) / float(img.rows);
                float cub = (a * x * x * x) + (b * y * y * y) + (c * x * y * y) + (d * x * x * y) + (e * x * x) + (f * y * y) + (g * x * y) + (h * x) + (i * y) + j;
                surface.at<float>(row, col) = cub;
            }
        }
        return surface;
    }

    Mat SolveQuarticEquation(Mat img, Mat mask) {
        //A fourth order (k=4) polynomial forms a quartic expression : f(x,y)= ax^4 + by^4 + cx^3y + dx^2y^2 + exy^3 + fx^3 + gy^3 + hxy^2 + iyx^2 + jx^2 + ky^2 + lxy + mx +  ny + o
        int nbOfWhitePixelsInMask = cv::countNonZero(mask);
        Mat matA = Mat::zeros(Size(15, nbOfWhitePixelsInMask), CV_32FC1);
        Mat vecB = Mat::zeros(Size(1, nbOfWhitePixelsInMask), CV_32FC1);
        Mat surface = Mat::zeros(img.rows, img.cols, CV_32FC1);
        //Can't compute if the nb of pixels in the mask is less than the nb of parameters in the equation
        if (nbOfWhitePixelsInMask < 15) return surface;

        int maskIndex = 0;

        for (int row = 0; row < img.rows; row++) {
            for (int col = 0; col < img.cols; col++) {
                if ((!isnan(img.at<float>(row, col)))) {
                    if (mask.at<uchar>(row, col) != 0) {
                        float x = (col - img.cols / 2) / float(img.cols);
                        float y = (row - img.rows / 2) / float(img.rows);
                        matA.at<float>(maskIndex, 0) = x * x * x * x;
                        matA.at<float>(maskIndex, 1) = y * y * y * y;
                        matA.at<float>(maskIndex, 2) = x * x * x * y;
                        matA.at<float>(maskIndex, 3) = x * x * y * y;
                        matA.at<float>(maskIndex, 4) = x * y * y * y;
                        matA.at<float>(maskIndex, 5) = x * x * x;
                        matA.at<float>(maskIndex, 6) = y * y * y;
                        matA.at<float>(maskIndex, 7) = x * x * y;
                        matA.at<float>(maskIndex, 8) = x * y * y;
                        matA.at<float>(maskIndex, 9) = x * x;
                        matA.at<float>(maskIndex, 10) = y * y;
                        matA.at<float>(maskIndex, 11) = x * y;
                        matA.at<float>(maskIndex, 12) = x;
                        matA.at<float>(maskIndex, 13) = y;
                        matA.at<float>(maskIndex, 14) = 1;
                        vecB.at<float>(maskIndex, 0) = img.at<float>(row, col);
                        maskIndex++;
                    }
                }
            }
        }

        Mat q;
        solve(matA, vecB, q, DECOMP_SVD);

        float a = q.at<float>(0, 0);
        float b = q.at<float>(1, 0);
        float c = q.at<float>(2, 0);
        float d = q.at<float>(3, 0);
        float e = q.at<float>(4, 0);
        float f = q.at<float>(5, 0);
        float g = q.at<float>(6, 0);
        float h = q.at<float>(7, 0);
        float i = q.at<float>(8, 0);
        float j = q.at<float>(9, 0);
        float k = q.at<float>(10, 0);
        float l = q.at<float>(11, 0);
        float m = q.at<float>(12, 0);
        float n = q.at<float>(13, 0);
        float o = q.at<float>(14, 0);
        for (int row = 0; row < img.rows; row++)
        {
            for (int col = 0; col < img.cols; col++)
            {
                float x = (col - img.cols / 2) / float(img.cols);
                float y = (row - img.rows / 2) / float(img.rows);
                float cub = (a * x * x * x * x) + (b * y * y * y * y) + (c * x * x * x * y) + (d * x * x * y * y) + (e * x * y * y * y) + (f * x * x * x) + (g * y * y * y) + (h * x * x * y) + (i * x * y * y) + (j * x * x) + (k * y * y) + (l * x * y) + (m * x) + (n * y) + o;
                surface.at<float>(row, col) = cub;
            }
        }
        return surface;
    }

    Mat HanningApodization(const Mat& src) {
        if (src.type() != CV_32FC1 && src.type() != CV_64FC1)
            ErrorLogging::LogErrorAndThrow("[Hanning apodization] Input image should be stored into a single channel 32-bits float or 64-bits float: is ", src.type(), " instead (see CV_MAKETYPE)");

        Mat hann_window;
        createHanningWindow(hann_window, src.size(), src.type());
        return src.mul(hann_window);
    }

    Mat DiscretFourierTransform(const Mat& src) {
        Mat padded; // expand input image to optimal size
        int m = getOptimalDFTSize(src.rows);
        int n = getOptimalDFTSize(src.cols); // on the border add zero values
        copyMakeBorder(src, padded, 0, m - src.rows, 0, n - src.cols, BORDER_CONSTANT, Scalar::all(0));

        Mat planes[] = { Mat_<float>(padded), Mat::zeros(padded.size(), CV_32F) };
        Mat complexI;
        merge(planes, 2, complexI); // Add to the expanded another plane with zeros

        dft(complexI, complexI); // this way the result may fit in the source matrix

        // compute the magnitude and switch to logarithmic scale
        // => log(1 + sqrt(Re(DFT(I))^2 + Im(DFT(I))^2))
        split(complexI, planes); // planes[0] = Re(DFT(I), planes[1] = Im(DFT(I))

        magnitude(planes[0], planes[1], planes[0]); // planes[0] = magnitude
        Mat result = planes[0];

        result += Scalar::all(1); // switch to logarithmic scale
        log(result, result);

        // crop the spectrum, if it has an odd number of rows or columns
        result = result(Rect(0, 0, result.cols & -2, result.rows & -2));

        // rearrange the quadrants of Fourier image so that the origin is at the image center
        int cx = result.cols / 2;
        int cy = result.rows / 2;

        Mat q0(result, Rect(0, 0, cx, cy));   // Top-Left - Create a ROI per quadrant
        Mat q1(result, Rect(cx, 0, cx, cy));  // Top-Right
        Mat q2(result, Rect(0, cy, cx, cy));  // Bottom-Left
        Mat q3(result, Rect(cx, cy, cx, cy)); // Bottom-Right

        Mat tmp; // swap quadrants (Top-Left with Bottom-Right)
        q0.copyTo(tmp);
        q3.copyTo(q0);
        tmp.copyTo(q3);

        q1.copyTo(tmp); // swap quadrant (Top-Right with Bottom-Left)
        q2.copyTo(q1);
        tmp.copyTo(q2);

        // Transform the matrix with float values into a viewable image form (float between values 0 and 1).
        normalize(result, result, 0, 1, NORM_MINMAX);

        return result;
    }
} // namespace signal_2D