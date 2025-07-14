#include <BaseAlgos/2DSignalAnalysis.hpp>
#include <Logger.hpp>

using namespace std;
using namespace cv;

namespace signal_2D {

  Mat SolvePlaneEquation(Mat img) {
    Mat matA = Mat::zeros(img.rows * img.cols, 3, CV_32FC1);
    Mat vecB = Mat::zeros(img.rows * img.cols, 1, CV_32FC1);

    int NbStatsPts = 0;
    for (int i = 0; i < img.cols; i++) {
      for (int j = 0; j < img.rows; j++) {
        if ((!isnan(img.at<float>(j, i)))) {
          matA.at<float>(NbStatsPts, 0) = i;
          matA.at<float>(NbStatsPts, 1) = j;
          matA.at<float>(NbStatsPts, 2) = 1;
          vecB.at<float>(NbStatsPts, 0) = img.at<float>(j, i);
          NbStatsPts++;
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

  Mat HanningApodization(const Mat &src) {
    if (src.type() != CV_32FC1 && src.type() != CV_64FC1) {
      stringstream strStrm;
      strStrm << "[Hanning apodization] Input image should be stored into a single channel 32-bits float or 64-bits float";
      string message = strStrm.str();
      Logger::Error(message);
      throw exception(message.c_str());
    }

    Mat hann_window;
    createHanningWindow(hann_window, src.size(), src.type());
    return src.mul(hann_window);
  }

  Mat DiscretFourierTransform(const Mat &src) {
    Mat padded; // expand input image to optimal size
    int m = getOptimalDFTSize(src.rows);
    int n = getOptimalDFTSize(src.cols); // on the border add zero values
    copyMakeBorder(src, padded, 0, m - src.rows, 0, n - src.cols, BORDER_CONSTANT, Scalar::all(0));

    Mat planes[] = {Mat_<float>(padded), Mat::zeros(padded.size(), CV_32F)};
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