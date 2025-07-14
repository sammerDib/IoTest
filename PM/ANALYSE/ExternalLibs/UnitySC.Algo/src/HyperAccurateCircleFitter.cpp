#include <HyperAccurateCircleFitter.hpp>
#include <math.h>
#include <opencv2/imgproc.hpp>

#include <Wafer.hpp>
#include <iomanip>
#include <iostream>

namespace {

  template <typename T> inline T pow2(T t) { return std::pow(t, 2); };

  /*
   *
   * A data has 5 fields:
   *       n (of type int), the number of data points
   *       X and Y (arrays of type reals), arrays of x- and y-coordinates
   *       meanX and meanY (of type reals), coordinates of the centroid (x and y
   * sample means)
   */
  class Data {
  public:
    int n;
    double *X; // space is allocated in the constructors
    double *Y; // space is allocated in the constructors
    double meanX, meanY;

    Data(int N, double *dataX, double *dataY) : n(N), X(dataX), Y(dataY) {}

    /*
     * computes the x- and y- sample means (the coordinates of the centeroid)
     */
    void means(void) {
      meanX = 0.;
      meanY = 0.;

      for (int i = 0; i < n; i++) {
        meanX += X[i];
        meanY += Y[i];
      }
      meanX /= n;
      meanY /= n;
    }
  };

  double rmse(Data &data, HyperAccurateCircleFitter::Result &circle) {
    double sum = 0., dx, dy;

    for (int i = 0; i < data.n; i++) {
      dx = data.X[i] - circle.center.x;
      dy = data.Y[i] - circle.center.y;
      sum += pow2(sqrt(dx * dx + dy * dy) - circle.radius);
    }
    return sqrt(sum / data.n);
  }

} // namespace

using namespace Algorithms;

Status HyperAccurateCircleFitter::Fit(std::vector<cv::Point> const &points, ICircleFitter::Result &fit, Wafer *wafer) const {

  int i, iter;
  Status status;

  double Xi, Yi, Zi;
  double Mz, Mxy, Mxx, Myy, Mxz, Myz, Mzz, Cov_xy, Var_z;
  double A0, A1, A2, A22;
  double Dy, xnew, x, ynew, y;
  double DET, Xcenter, Ycenter;

  const int dataSize = static_cast<int>(points.size());

  if (dataSize < 3) {
    fit.success = false;
    fit.message = "I can only fit with three or more reference points";
    fit.radius = std::numeric_limits<double>::quiet_NaN();
    fit.center = {std::numeric_limits<double>::quiet_NaN(), std::numeric_limits<double>::quiet_NaN()};
    status.code = StatusCode::BWA_FIT_FAILED;
    status.confidence = 0;
    status.message = fit.message;
  } else {

    std::vector<double> xs;
    std::vector<double> ys;

    for (auto &pt : points) {
      xs.push_back(pt.x);
      ys.push_back(pt.y);
    }

    Data data(dataSize, &xs[0], &ys[0]);

    data.means(); // Compute x- and y- sample means (via a function in the class
                  // "data")

    //     computing moments

    Mxx = Myy = Mxy = Mxz = Myz = Mzz = 0.;

    for (i = 0; i < data.n; i++) {
      Xi = data.X[i] - data.meanX; //  centered x-coordinates
      Yi = data.Y[i] - data.meanY; //  centered y-coordinates
      Zi = Xi * Xi + Yi * Yi;

      Mxy += Xi * Yi;
      Mxx += Xi * Xi;
      Myy += Yi * Yi;
      Mxz += Xi * Zi;
      Myz += Yi * Zi;
      Mzz += Zi * Zi;
    }
    Mxx /= data.n;
    Myy /= data.n;
    Mxy /= data.n;
    Mxz /= data.n;
    Myz /= data.n;
    Mzz /= data.n;

    //    computing the coefficients of the characteristic polynomial

    Mz = Mxx + Myy;
    Cov_xy = Mxx * Myy - Mxy * Mxy;
    Var_z = Mzz - Mz * Mz;

    A2 = 4.0 * Cov_xy - 3.0 * Mz * Mz - Mzz;
    A1 = Var_z * Mz + 4.0 * Cov_xy * Mz - Mxz * Mxz - Myz * Myz;
    A0 = Mxz * (Mxz * Myy - Myz * Mxy) + Myz * (Myz * Mxx - Mxz * Mxy) - Var_z * Cov_xy;
    A22 = A2 + A2;

    //    finding the root of the characteristic polynomial
    //    using Newton's method starting at x=0
    //     (it is guaranteed to converge to the right root)

    for (x = 0., y = A0, iter = 0; iter < MAX_ITERATIONS; iter++) // usually, 4-6 iterations are enough
    {
      Dy = A1 + x * (A22 + 16. * x * x);
      xnew = x - y / Dy;
      if ((xnew == x) || (!std::isfinite(xnew)))
        break;
      ynew = A0 + xnew * (A1 + xnew * (A2 + 4.0 * xnew * xnew));
      if (abs(ynew) >= abs(y))
        break;
      x = xnew;
      y = ynew;
    }

    //    computing paramters of the fitting circle

    DET = x * x - x * Mz + Cov_xy;
    Xcenter = (Mxz * (Myy - x) - Myz * Mxy) / DET / 2.0;
    Ycenter = (Myz * (Mxx - x) - Mxz * Mxy) / DET / 2.0;

    //       assembling the output

    fit.center.x = Xcenter + data.meanX;
    fit.center.y = Ycenter + data.meanY;
    fit.radius = sqrt(Xcenter * Xcenter + Ycenter * Ycenter + Mz - x - x);
    fit.rmse = rmse(data, fit);
    fit.iterations = iter;

    status = BuildStatus(fit, wafer);
  }
  return status;
}

Status HyperAccurateCircleFitter::BuildStatus(const ICircleFitter::Result &fit, Wafer *wafer) const {

  Status status;
  status.code = StatusCode::OK;
  if (0 == fit.rmse) {
    status.confidence = 1;
  } else {
    // compute the confidence as:
    //
    auto waferProperties = wafer->GetProperties();
    double diameter = waferProperties.radiusInMicrons * 2;

    // confidence goes down lineary until zero, for a RMSE
    // of 1% of wafer diameter.
    const double maxError = diameter * (1 / 100.0);
    status.confidence = 1 - (fit.rmse / maxError);
  }

  if (fit.iterations == MAX_ITERATIONS) {
    status.code = StatusCode::BWA_FIT_FAILED;
    std::ostringstream stringStream;
    stringStream << "Fit failed with a RMSE of " << fit.rmse;
  } else if (fit.rmse > 100) {
    status.code = StatusCode::BWA_FIT_FAILED;
    std::ostringstream stringStream;
    stringStream << "Fit failed (too high circle point dispersion) with a RMSE of " << fit.rmse;
    status.message = stringStream.str();
    status.message = stringStream.str();
  } else {
    std::ostringstream stringStream;
    stringStream << "Fit succeeded with " << fit.iterations << " iterations, with a RMSE of " << fit.rmse;
    status.message = stringStream.str();
  }
  return status;
}
