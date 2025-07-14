#include "CameraCalibration.hpp"
#include "CImageTypeConvertor.hpp"
#include <opencv2/calib3d.hpp>
#include "ReportingUtils.hpp"
#include "NormalEstimation.hpp"
#include <WaferDetector.hpp>

using namespace std;
using namespace cv;

class ParallelCMNormalizedAndMComputation : public ParallelLoopBody
{
private:
    std::vector<cv::Point2f> uv;
    Mat& CMNormalized;
    Mat& M;
    Mat& Mask;
    Mat m;

    float r00;
    float r01;
    float r02;
    float r10;
    float r11;
    float r12;
    float r20;
    float r21;
    float r22;
    float tx;
    float ty;
    float tz;

public:
    ParallelCMNormalizedAndMComputation(std::vector<cv::Point2f> uv, Mat& CMNormalized, Mat& M, Mat& mask, Mat& RCameraToWafer, Mat& TWaferToCamera)
        : CMNormalized(CMNormalized), M(M), uv(uv), Mask(mask)
    {
        r00 = static_cast<float>(RCameraToWafer.at<double>(0, 0));
        r01 = static_cast<float>(RCameraToWafer.at<double>(0, 1));
        r02 = static_cast<float>(RCameraToWafer.at<double>(0, 2));
        r10 = static_cast<float>(RCameraToWafer.at<double>(1, 0));
        r11 = static_cast<float>(RCameraToWafer.at<double>(1, 1));
        r12 = static_cast<float>(RCameraToWafer.at<double>(1, 2));
        r20 = static_cast<float>(RCameraToWafer.at<double>(2, 0));
        r21 = static_cast<float>(RCameraToWafer.at<double>(2, 1));
        r22 = static_cast<float>(RCameraToWafer.at<double>(2, 2));

        tx = static_cast<float>(TWaferToCamera.at<double>(0, 0));
        ty = static_cast<float>(TWaferToCamera.at<double>(1, 0));
        tz = static_cast<float>(TWaferToCamera.at<double>(2, 0));
    }

    virtual void operator()(const Range& range) const CV_OVERRIDE
    {

        for (int r = range.start; r < range.end; r++)
        {
            int i = r / CMNormalized.cols;
            int j = (r % CMNormalized.cols);

            if (Mask.ptr<uchar>(i)[j] != 0)
            {
                float vx = uv.at(r).x;
                float vy = uv.at(r).y;
                float vz = 1.0;

                float norm = sqrt(vx * vx + vy * vy + vz * vz);
                vx /= norm;
                vy /= norm;
                vz /= norm;

                float CMx = r00 * vx + r01 * vy + r02 * vz;
                float CMy = r10 * vx + r11 * vy + r12 * vz;
                float CMz = r20 * vx + r21 * vy + r22 * vz;
                CMNormalized.ptr<Vec3f>(i)[j][0] = CMx;
                CMNormalized.ptr<Vec3f>(i)[j][1] = CMy;
                CMNormalized.ptr<Vec3f>(i)[j][2] = CMz;

                float factor = -tz / CMNormalized.ptr<Vec3f>(i)[j][2];
                float Mx = tx + CMNormalized.ptr<Vec3f>(i)[j][0] * factor;
                float My = ty + CMNormalized.ptr<Vec3f>(i)[j][1] * factor;
                float Mz = 0.0;
                M.ptr<Vec3f>(i)[j][0] = Mx;
                M.ptr<Vec3f>(i)[j][1] = My;
                M.ptr<Vec3f>(i)[j][2] = Mz;
            }
        }
    }
};

class ParallelMPNormalizedAndPComputation : public ParallelLoopBody
{
private:
    const Mat& UnwrapX;
    const Mat& UnwrapY;
    const Mat& M;
    Mat& MPNormalized;
    Mat& P;
    Mat& Mask;

    float PixelSizeInMillimeters;
    float FringePeriodInPixels;

    float r00;
    float r01;
    float r02;
    float r10;
    float r11;
    float r12;
    float r20;
    float r21;
    float r22;
    float tx;
    float ty;
    float tz;

public:
    ParallelMPNormalizedAndPComputation(const Mat& M, const Mat& unwrapX, const Mat& unwrapY, Mat& MPNormalized, Mat& P, Mat& mask, Mat& RScreenToWafer, Mat& TScreenToWafer, float pixelSizeInMillimeters, int fringePeriodInPixels)
        : MPNormalized(MPNormalized), P(P), M(M), UnwrapX(unwrapX), UnwrapY(unwrapY), Mask(mask), PixelSizeInMillimeters(pixelSizeInMillimeters), FringePeriodInPixels((float)fringePeriodInPixels)
    {
        r00 = static_cast<float>(RScreenToWafer.at<double>(0, 0));
        r01 = static_cast<float>(RScreenToWafer.at<double>(0, 1));
        r02 = static_cast<float>(RScreenToWafer.at<double>(0, 2));
        r10 = static_cast<float>(RScreenToWafer.at<double>(1, 0));
        r11 = static_cast<float>(RScreenToWafer.at<double>(1, 1));
        r12 = static_cast<float>(RScreenToWafer.at<double>(1, 2));
        r20 = static_cast<float>(RScreenToWafer.at<double>(2, 0));
        r21 = static_cast<float>(RScreenToWafer.at<double>(2, 1));
        r22 = static_cast<float>(RScreenToWafer.at<double>(2, 2));

        tx = static_cast<float>(TScreenToWafer.at<double>(0, 0));
        ty = static_cast<float>(TScreenToWafer.at<double>(1, 0));
        tz = static_cast<float>(TScreenToWafer.at<double>(2, 0));

    }

    virtual void operator()(const Range& range) const CV_OVERRIDE
    {
        float coeff = static_cast<float>((double)(FringePeriodInPixels * PixelSizeInMillimeters) / (2.0 * CV_PI));

        for (int r = range.start; r < range.end; r++)
        {
            int i = r / MPNormalized.cols;
            int j = (r % MPNormalized.cols);

            if (Mask.ptr<uchar>(i)[j] != 0)
            {
                float x = UnwrapX.ptr<float>(i)[j];
                float y = UnwrapY.ptr<float>(i)[j];

                x *= coeff;
                y *= coeff;
                float mesX = r00 * x + r01 * y + tx;
                float mesY = r10 * x + r11 * y + ty;
                float mesZ = r20 * x + r21 * y + tz;
                P.ptr<Vec3f>(i)[j][0] = mesX;
                P.ptr<Vec3f>(i)[j][1] = mesY;
                P.ptr<Vec3f>(i)[j][2] = mesZ;

                float mpX = mesX - M.ptr<Vec3f>(i)[j][0];
                float mpY = mesY - M.ptr<Vec3f>(i)[j][1];
                float mpZ = mesZ;

                float norm = sqrt(mpX * mpX + mpY * mpY + mpZ * mpZ);
                float mpXNormalized = mpX / norm; //use only once - optimize skip variable TODO
                float mpYNormalized = mpY / norm; //use only once - optimize skip variable TODO
                float mpZNormalized = mpZ / norm; //use only once - optimize skip variable TODO

                MPNormalized.ptr<Vec3f>(i)[j][0] = mpXNormalized;
                MPNormalized.ptr<Vec3f>(i)[j][1] = mpYNormalized;
                MPNormalized.ptr<Vec3f>(i)[j][2] = mpZNormalized;

            }
        }
    }
};

class ParallelNormalizedNormalComputation : public ParallelLoopBody
{
private:
    const Mat& CMNormalized;
    const Mat& MPNormalized;
    const Mat& Mask;
    Mat& NormalNormalized;

public:
    ParallelNormalizedNormalComputation(const Mat& CMNormalized, const Mat& MPNormalized, const Mat& mask, Mat& normalNormalized)
        : CMNormalized(CMNormalized), MPNormalized(MPNormalized), Mask(mask), NormalNormalized(normalNormalized)
    {
    }

    virtual void operator()(const Range& range) const CV_OVERRIDE
    {
        for (int r = range.start; r < range.end; r++)
        {
            int i = r / NormalNormalized.cols;
            int j = (r % NormalNormalized.cols);

            if (Mask.ptr<uchar>(i)[j] != 0)
            {
                NormalNormalized.ptr<Vec3f>(i)[j][0] = MPNormalized.ptr<Vec3f>(i)[j][0] - CMNormalized.ptr<Vec3f>(i)[j][0];
                NormalNormalized.ptr<Vec3f>(i)[j][1] = MPNormalized.ptr<Vec3f>(i)[j][1] - CMNormalized.ptr<Vec3f>(i)[j][1];
                NormalNormalized.ptr<Vec3f>(i)[j][2] = MPNormalized.ptr<Vec3f>(i)[j][2] - CMNormalized.ptr<Vec3f>(i)[j][2];

                float norm = sqrt(NormalNormalized.ptr<Vec3f>(i)[j][0] * NormalNormalized.ptr<Vec3f>(i)[j][0] + NormalNormalized.ptr<Vec3f>(i)[j][1] * NormalNormalized.ptr<Vec3f>(i)[j][1] + NormalNormalized.ptr<Vec3f>(i)[j][2] * NormalNormalized.ptr<Vec3f>(i)[j][2]);
                NormalNormalized.ptr<Vec3f>(i)[j][0] /= norm;
                NormalNormalized.ptr<Vec3f>(i)[j][1] /= norm;
                NormalNormalized.ptr<Vec3f>(i)[j][2] /= norm;
            }
        }
    }
};

namespace psd {
    namespace {
        /**
        * Apply camera distortion
        *
        * @param point                      - input point
        * @param intrinsicCameraMatrix      - Input camera matrix K = [fx 0 cx; 0 fy cy; 0 0 1]
        * @param distortionMatrix           - Input vector of distortion coefficients [k1,k2,p1,p2,k3,k4,k5,k6,s1,s2,s3,s4,taux,tauy] of 4, 5, 8, 12 or 14 elements (radial and tangeantial distorsion).
        *
        * @return   distorted point
        */
        cv::Point2d Distort(cv::Point2d point, cv::Mat intrinsicCameraMatrix, cv::Mat distortionMatrix);

        /**
        * Converts the input coordinates from observed to ideal coordinates and uses an iterative process to remove distortions from the ideal or normalized points.
        * The reason the process is iterative is because the OpenCV distortion model is not easy to invert analytically.
        *
        * @param xy                         - Observed coordinates (also called 'image' coordinates, not normalized) or 'u' and 'v' in OpenCV docs
        * @param intrinsicCameraMatrix      - Input camera matrix K = [fx 0 cx; 0 fy cy; 0 0 1]
        * @param distortionMatrix           - Input vector of distortion coefficients [k1,k2,p1,p2,k3,k4,k5,k6,s1,s2,s3,s4,taux,tauy] of 4, 5, 8, 12 or 14 elements (radial and tangeantial distorsion).
        *
        * @return   Ideal coordinates (also called 'normalized' or 'sensor' coordinates) which are the input variables to the distortion model or 'x' and 'y' in the OpenCV docs.
        *           Ideal coordinates have been normalized by the intrinsic parameters so that they have been scaled by the focal length and are relative to the image centroid at (cx,cy).
        */
        std::vector<cv::Point2f> UndistortPoints(const std::vector<cv::Point2f>& xy, const cv::Mat& intrinsicCameraMatrix, const cv::Mat& distortionMatrix);
    }

    CMResult ComputeCMNormalizedAndM(cv::Size imgSize, CalibrationParameters intrinsicCameraParam, ExtrinsicCameraParameters extrinsicCameraParam) {
        // From the coordinates of the pixels perceived by the camera xy we calculate the coordinates normalized of the pixels without distortion uv

        std::vector<cv::Point2f> xy;
        for (int r = 0; r < imgSize.height; r++) {
            for (int c = 0; c < imgSize.width; c++) {
                xy.push_back(cv::Point2f((float)c, (float)r));
            }
        }
        std::vector<cv::Point2f> uv = UndistortPoints(xy, intrinsicCameraParam.CameraIntrinsic, intrinsicCameraParam.Distortion);

        // Calculate the vector W = CMNormalized from the normalized pixel coordinates and the extrinsic parameters of the camera
        // Calculate the point M in the wafer frame (is in the plane z = 0)

        Mat RCameraToWafer = extrinsicCameraParam.RWaferToCamera.t();
        Mat TCameraToWafer = -RCameraToWafer * extrinsicCameraParam.TWaferToCamera;

        Mat M = Mat::zeros(imgSize, CV_32FC3);
        Mat CMNormalized = Mat::zeros(imgSize, CV_32FC3);
        Mat mask = Mat::ones(imgSize, CV_8UC1);
        ParallelCMNormalizedAndMComputation obj(uv, CMNormalized, M, mask, RCameraToWafer, TCameraToWafer);
        parallel_for_(Range(0, CMNormalized.size().height * CMNormalized.size().width), obj);

        CMResult result;
        result.CMNormalized = CMNormalized;
        result.M = M;
        return result;
    }

    MPResult ComputeMPNormalizedAndP(const cv::Mat& M, const cv::Mat& unwrapX, const cv::Mat& unwrapY, ExtrinsicScreenParameters extrinsicScreenParam, float screenPixelSizeInMillimeters, int fringePeriodInPixels) {
        // For each metric position on the screen, we must calculate the position in the wafer referential using the extrinsic parameters of the screen
        // And calculate the vector V = MPNormalized

        Mat P = Mat::zeros(unwrapX.size(), CV_32FC3);
        Mat MPNormalized = Mat::zeros(unwrapX.size(), CV_32FC3);
        Mat mask = Mat::ones(unwrapX.size(), CV_8UC1);
        ParallelMPNormalizedAndPComputation obj(M, unwrapX, unwrapY, MPNormalized, P, mask, extrinsicScreenParam.RScreenToWafer, extrinsicScreenParam.TScreenToWafer, screenPixelSizeInMillimeters, fringePeriodInPixels);
        parallel_for_(Range(0, unwrapX.size().height * unwrapX.size().width), obj);

        MPResult result;
        result.MPNormalized = MPNormalized;
        result.P = P;
        return result;
    }

    cv::Mat ComputeNormalizedNormal(const cv::Mat& M, const cv::Mat& CMNormalized, const cv::Mat& MPNormalized, const cv::Mat& mask, std::string directoryPathToStoreReport) {
        cv::Mat normalNormalized = Mat::zeros(CMNormalized.size(), CV_32FC3);

        ParallelNormalizedNormalComputation obj(CMNormalized, MPNormalized, mask, normalNormalized);
        parallel_for_(Range(0, normalNormalized.size().height * normalNormalized.size().width), obj);

        if (directoryPathToStoreReport != "")
        {
            cv::Mat normalX, normalY, normalZ;
            std::vector<cv::Mat> channels(3);
            split(normalNormalized, channels);
            normalX = channels[0];
            normalY = channels[1];
            normalZ = channels[2];

            cv::imwrite(directoryPathToStoreReport + "\\normalX.tif", normalX);
            cv::imwrite(directoryPathToStoreReport + "\\normalY.tif", normalY);
            cv::imwrite(directoryPathToStoreReport + "\\normalZ.tif", normalZ);
        }
        return normalNormalized;
    }

    namespace {
        cv::Point2d Distort(cv::Point2d point, cv::Mat intrinsicCameraMatrix, cv::Mat distortionMatrix)
        {
            // ATTENTION retour en double mais tous les calculs était effectuer en float WTF oO' !! pourquoi ? (RTI: 27/06/2024)

            // To relative coordinates
            auto fx = intrinsicCameraMatrix.at<double>(0, 0);
            auto fy = intrinsicCameraMatrix.at<double>(1, 1);
            auto cx = intrinsicCameraMatrix.at<double>(0, 2);
            auto cy = intrinsicCameraMatrix.at<double>(1, 2);
            auto x = (point.x - cx) / fx;
            auto y = (point.y - cy) / fy;

            auto r2 = x * x + y * y;

            // Radial distorsion
            auto k1 = distortionMatrix.at<double>(0, 0);
            auto k2 = distortionMatrix.at<double>(1, 0);
            auto k3 = distortionMatrix.at<double>(4, 0);
            auto xDistort = x * (1 + k1 * r2 + k2 * r2 * r2 + k3 * r2 * r2 * r2);
            auto yDistort = y * (1 + k1 * r2 + k2 * r2 * r2 + k3 * r2 * r2 * r2);

            // Tangential distorsion
            auto p1 = distortionMatrix.at<double>(2, 0);
            auto p2 = distortionMatrix.at<double>(3, 0);
            xDistort = xDistort + (2 * p1 * x * y + p2 * (r2 + 2 * x * x));
            yDistort = yDistort + (p1 * (r2 + 2 * y * y) + 2 * p2 * x * y);

            // Back to absolute coordinates.
            xDistort = xDistort * fx + cx;
            yDistort = yDistort * fy + cy;

            return cv::Point2d(xDistort, yDistort);
        }

        std::vector<cv::Point2f> UndistortPoints(const std::vector<cv::Point2f>& xy, const cv::Mat& intrinsicCameraMatrix, const cv::Mat& distortionMatrix)
        {
            //compute uv : ideal point coordinates after undistortion and reverse perspective transformation. If matrix P is identity or omitted, uv will contain normalized point coordinates. Same size and type as the input points src.
            //option : Criteria Termination criteria for the iterative distortion compensation. By default does 5 iterations to compute undistorted points. default struct('type','Count', 'maxCount',5, 'epsilon',0.01)
            // 5 iterations is not enough if your lens has significant distortion.
            std::vector<cv::Point2f> uv;
            cv::undistortPoints(xy, uv, intrinsicCameraMatrix, distortionMatrix);

            return uv;
        }
    }
}