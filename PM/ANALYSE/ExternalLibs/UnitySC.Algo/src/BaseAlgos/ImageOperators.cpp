#include <BaseAlgos/ImageOperators.hpp>
#include <ImageTypeConvertor.hpp>
#include <Logger.hpp>

using namespace cv;
using namespace std;

namespace img_operators {
  double VarianceOfLaplacian(const Mat &img) {
    Mat laplacian;
    Laplacian(img, laplacian, CV_64F);

    Scalar mu, sigma;
    meanStdDev(img, mu, sigma);

    double focusMeasure = sigma.val[0] * sigma.val[0];
    return focusMeasure;
  }

  double ModifiedLaplacian(const Mat &img) {
    Mat M = (Mat_<double>(3, 1) << -1, 2, -1);
    Mat G = getGaussianKernel(3, -1, CV_64F);

    Mat laplacianX;
    sepFilter2D(img, laplacianX, CV_64F, M, G);

    Mat laplacianY;
    sepFilter2D(img, laplacianY, CV_64F, G, M);

    Mat modifiedLaplacian = abs(laplacianX) + abs(laplacianY);
    double focusMeasure = mean(modifiedLaplacian).val[0];
    return focusMeasure;
  }

  double TenenbaumGradient(const Mat &img, int kernelSize) {
    Mat gradientX, gradientY;
    Sobel(img, gradientX, CV_64F, 1, 0, kernelSize);
    Sobel(img, gradientY, CV_64F, 0, 1, kernelSize);

    Mat tenengrad = gradientX.mul(gradientX) + gradientY.mul(gradientY);
    double focusMeasure = mean(tenengrad).val[0];
    return focusMeasure;
  }

  double NormalizedGraylevelVariance(const Mat &img) {
    Scalar mu, sigma;
    meanStdDev(img, mu, sigma);

    double focusMeasure = (sigma.val[0] * sigma.val[0]) / mu.val[0];
    return focusMeasure;
  }

  double Saturation(const Mat &img) {
    Mat img_bgr = Convertor::ConvertTo8UC3(img);
    Mat img_hsv;
    cvtColor(img_bgr, img_hsv, COLOR_BGR2HSV);

    vector<Mat> channels;
    split(img_hsv, channels);

    double saturationMeasure = mean(channels[2]).val[0] / 250;
    return saturationMeasure;
  }
};  // namespace img_operators