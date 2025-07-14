#include "CameraCalibration.hpp"
#include "CImageTypeConvertor.hpp"
#include <opencv2/calib3d.hpp>
#include "CShapeFinder.hpp"
#include <CDistortionCalibration.hpp>
#include "RadonTransform.hpp"
#include "FourierTransform.hpp"

using namespace cv;

namespace distortionCalibration {
    namespace {
        struct DistortionCoefficients
        {
            // (0, 0) : x^3
            // (0, 1) : y^3
            // (0, 2) : x * y^2
            // (0, 3) : x^2 * y
            // (0, 4) : x^2
            // (0, 5) : y^2
            // (0, 6) : x*y
            // (0, 7) : x
            // (0, 8) : y
            // (0, 9) : 1
            Mat coeffX;
            Mat coeffY;
        };

        DistortionCoefficients SolveCubicDistortionCoefficients(std::vector<cv::Point2f> observedPoints, std::vector<cv::Point2f> theoreticalPoints) {
            //A third order (k=3) polynomial forms a cubic expression : f(x,y)= a*x^3 + b*y^3 + c*x*y^2 + d*y*x^2 + e*x^2 + f*y^2 + g*x*y + h*x +  i*y + j
            if (observedPoints.size() != theoreticalPoints.size())
            {
                throw std::exception("SolveCubicDistortionCoefficients : observedPoints and theoreticalPoints need to be of the same size");
            }
            auto nbOfPoints = (int)observedPoints.size();
            Mat matA = Mat::zeros(Size(10, nbOfPoints), CV_32FC1);
            Mat matBX = Mat::zeros(Size(1, nbOfPoints), CV_32FC1);
            Mat matBY = Mat::zeros(Size(1, nbOfPoints), CV_32FC1);

            for (int i = 0; i < nbOfPoints; i++)
            {
                float x = observedPoints[i].x;
                float y = observedPoints[i].y;
                matA.at<float>(i, 0) = x * x * x;
                matA.at<float>(i, 1) = y * y * y;
                matA.at<float>(i, 2) = x * y * y;
                matA.at<float>(i, 3) = x * x * y;
                matA.at<float>(i, 4) = x * x;
                matA.at<float>(i, 5) = y * y;
                matA.at<float>(i, 6) = x * y;
                matA.at<float>(i, 7) = x;
                matA.at<float>(i, 8) = y;
                matA.at<float>(i, 9) = 1;

                float xTheoretical = theoreticalPoints[i].x;
                float yTheoretical = theoreticalPoints[i].y;
                matBX.at<float>(i, 0) = xTheoretical;
                matBY.at<float>(i, 0) = yTheoretical;
            }

            Mat coeffX;
            Mat coeffY;
            solve(matA, matBX, coeffX, DECOMP_SVD);
            solve(matA, matBY, coeffY, DECOMP_SVD);

            DistortionCoefficients coeffs = { coeffX, coeffY };

            return coeffs;
        }

        void ComputeGridAngleAndPeriod(cv::Mat img, double &angle, double &period, std::string reportPath = "")
        {
            //Resizing for lower computation time
            cv::Mat resized = img.clone();
            double resizeScale = 1.0 / ((double)img.cols / 1000.0); //Resizing to get 1000px width
            resizeScale = std::min(1.0, resizeScale); //We don't want to make the image larger

            cv::resize(img, resized, cv::Size(), resizeScale, resizeScale);

            //Radon transform to make the pattern more obvious on a single column
            cv::Mat radon;
            double theta = 0.1;
            double baseAngle = 45;
            double startAngle = -baseAngle;
            double endAngle = baseAngle - theta;
            resized.convertTo(resized, CV_32FC1);
            radon_transform::RadonTransform(resized, radon, theta, startAngle, endAngle);

            //transposing because there is no DFT_COLS flag
            cv::transpose(radon, radon);

            //DFT row by row (col by col transposed) of the image the find the pattern
            cv::Mat fft;
            cv::dft(radon, fft, cv::DFT_COMPLEX_OUTPUT + cv::DFT_ROWS);
            cv::Mat mag = fourier_transform::LogMagnitudeSpectrum(fft, true);

            //Filtering out the very intense low frequencies that we don't care about
            double largestPeriod = 50.0;
            double freqLimit = 1.0 / largestPeriod;
            int pixelFreqLimit = (int)(freqLimit * (mag.cols / 2) + (mag.cols / 2));
            cv::rectangle(mag, cv::Rect(0, 0, pixelFreqLimit, mag.rows), cv::Scalar(0.0), -1);

            //Max y location = angle (opposite since we transposed earlier)
            //Max x location = period (opposite since we transposed earlier)
            cv::Point maxLoc;
            cv::minMaxLoc(mag, NULL, NULL, NULL, &maxLoc);

            angle = maxLoc.y * theta + startAngle;
            period = ((double)mag.cols / (maxLoc.x - mag.cols / 2)) / resizeScale;

            if (!reportPath.empty())
            {
                cv::Mat radonNorm;
                cv::Mat magNorm;
                cv::normalize(radon, radonNorm, 0, 255, cv::NORM_MINMAX, CV_8UC1);
                cv::normalize(mag, magNorm, 0, 255, cv::NORM_MINMAX, CV_8UC1);
                cv::imwrite( reportPath + "/radon.png", radonNorm);
                cv::imwrite( reportPath + "/mag.png", magNorm);
            }
        }

        std::vector<cv::Point2f> FindCircles(const cv::Mat& image, float gridCircleDiameterInMicrons, float gridPeriodicityInMicrons, float pixelSizeInMicrons)
        {
            float expectedCircleDiameter = (gridCircleDiameterInMicrons / pixelSizeInMicrons)  * 5.0f;
            float minDistBetweenTwoCircles = (gridPeriodicityInMicrons / pixelSizeInMicrons) / 2.0f;
            int cannyThreshold = 100;
            float detectionTolerance = 80.0f;
            auto parameters = shape_finder::CircleFinderParams(minDistBetweenTwoCircles, expectedCircleDiameter,
                detectionTolerance, cannyThreshold);
            std::vector<shape_finder::Circle> circles = shape_finder::CircleFinder(image, parameters);

            std::vector<cv::Point2f> centroids;
            for (shape_finder::Circle circle : circles) {
                centroids.push_back(circle.CenterPos);
            }

            return centroids;
        }

        std::vector<cv::Point2f> ReconstructTheoreticalGrid(const cv::Mat& image, 
            const std::vector<cv::Point2f>& observed_centroids,
            float angle, float observed_periodicity, float theoritical_periodicity)
        {
            auto center = cv::Point2f(0.5f * float(image.cols), 0.5f * float(image.rows));
            cv::Mat rot_mat = cv::getRotationMatrix2D(center, -angle, 1.0);

            std::vector<cv::Point2f> rotated_centroids;
            cv::transform(observed_centroids, rotated_centroids, rot_mat);

            float lowestX = (*std::min_element(rotated_centroids.begin(), rotated_centroids.end(),
                [](const cv::Point2f& p1, const cv::Point2f& p2) {return p1.x < p2.x; })).x;
            float lowestY = (*std::min_element(rotated_centroids.begin(), rotated_centroids.end(),
                [](const cv::Point2f& p1, const cv::Point2f& p2) {return p1.y < p2.y; })).y;

            std::vector<cv::Point2f> theoretical_centroids;
            for (cv::Point2f centroid : rotated_centroids) {
                auto i = cvRound((centroid.x - lowestX) / observed_periodicity);
                auto j = cvRound((centroid.y - lowestY) / observed_periodicity);
                auto x = (float)i * theoritical_periodicity;
                auto y = (float)j * theoritical_periodicity;
                theoretical_centroids.push_back(cv::Point2f(x, y));
            }

            return theoretical_centroids;
        }

        std::array<float, 10> matToDoubleArray1x10(const cv::Mat& mat) {

            std::array<float, 10> arr{};

            if (mat.rows == 10 && mat.cols == 1) {
                for (int i = 0; i < mat.rows; ++i) {
                    for (int j = 0; j < mat.cols; ++j) {
                        arr[i] = mat.at<float>(i, j);
                    }
                }
            }
            else {
                std::cerr << "Error: Input matrix must be a 1x10 single-channel float matrix (CV_32FC1)" << std::endl;
            }
            return arr;
        }
    }

    std::array<std::array<float, 10>, 2> ComputeDistoMatrix(cv::Mat& img, float gridCircleDiameterInMicrons, float gridPeriodicityInMicrons, float pixelSizeInMicrons) {

        //Convert to gray if necessary
        cv::Mat gray;
        if (img.channels() == 3) {
            cv::cvtColor(img, gray, cv::COLOR_BGR2GRAY);
        }
        else {
            gray = img.clone();
        }

        double gridAngle = 0.0;
        double gridPeriod = 0.0;

        ComputeGridAngleAndPeriod(gray, gridAngle, gridPeriod);

        std::vector<cv::Point2f> observedPoints = FindCircles(gray, gridCircleDiameterInMicrons, gridPeriodicityInMicrons, pixelSizeInMicrons);

        std::vector<cv::Point2f> theoreticalPoints = ReconstructTheoreticalGrid(gray,
            observedPoints, (float)gridAngle, (float)gridPeriod, gridPeriodicityInMicrons);

        auto center = cv::Point2f((float)(img.cols / 2), (float)(img.rows / 2));
        for (size_t i = 0; i < observedPoints.size(); i++) {
            observedPoints[i] = observedPoints[i] - center;
            theoreticalPoints[i] = theoreticalPoints[i] - center;
        }

        DistortionCoefficients distortionCoeffs = SolveCubicDistortionCoefficients(observedPoints, theoreticalPoints);
        std::array<float, 10> xCoeffs = matToDoubleArray1x10(distortionCoeffs.coeffX);
        std::array<float, 10> yCoeffs = matToDoubleArray1x10(distortionCoeffs.coeffY);

        return { xCoeffs, yCoeffs };
    }

    cv::Mat UndistortImage(const cv::Mat& img, std::array<std::array<float, 10>, 2> distortionCoeffs) {
        cv::Mat undistortedImage = cv::Mat::zeros(img.size(), img.type());

        Mat mapX(img.size(), CV_32FC1);
        Mat mapY(img.size(), CV_32FC1);

        float a1 = -distortionCoeffs[0][0]; // x^3
        float b1 = -distortionCoeffs[0][1]; // y^3
        float c1 = -distortionCoeffs[0][2]; // x * y^2
        float d1 = -distortionCoeffs[0][3]; // x^2 * y
        float e1 = -distortionCoeffs[0][4]; // x^2
        float f1 = -distortionCoeffs[0][5]; // y^2
                   
        float a2 = -distortionCoeffs[1][0]; // x^3
        float b2 = -distortionCoeffs[1][1]; // y^3
        float c2 = -distortionCoeffs[1][2]; // x * y^2
        float d2 = -distortionCoeffs[1][3]; // x^2 * y
        float e2 = -distortionCoeffs[1][4]; // x^2
        float f2 = -distortionCoeffs[1][5]; // y^2
        //Not taking the rest of the coeffs (0th order and 1st order) because we don't want rotation, scale and x/y shift

        for (int i = 0; i < img.rows; i++)
        {
            for (int j = 0; j < img.cols; j++)
            {
                int x = j - img.cols / 2;
                int y = i - img.rows / 2;

                float newX = a1 * (float)(x*x*x) + 
                             b1 * (float)(y*y*y) +
                             c1 * (float)(x*y*y) +
                             d1 * (float)(x*x*y) + 
                             e1 * (float)(x*x) + 
                             f1 * (float)(y*y) + 
                             (float)x; // shifting x coordinate from (0,0)

                float newY = a2 * (float)(x*x*x) +
                             b2 * (float)(y*y*y) +
                             c2 * (float)(x*y*y) +
                             d2 * (float)(x*x*y) +
                             e2 * (float)(x*x) +
                             f2 * (float)(y*y) +
                             (float)y; // shifting y coordinate from (0,0)

                mapX.at<float>(i, j) = newX + (float)(img.cols / 2);
                mapY.at<float>(i, j) = newY + (float)(img.rows / 2);
            }
        }

        cv::remap(img, undistortedImage, mapX, mapY, cv::INTER_LINEAR);

        return undistortedImage;
    }
}