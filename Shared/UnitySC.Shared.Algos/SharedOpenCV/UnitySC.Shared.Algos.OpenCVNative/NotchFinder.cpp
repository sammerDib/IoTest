#include "ErrorLogging.hpp"
#include "NotchFinder.hpp"
#include "CEdgeDetector.hpp"
#include "CRegistration.hpp"

using namespace shape_finder;
using namespace std;
using namespace cv;
namespace fs = std::filesystem;

#pragma unmanaged
namespace psd {
    namespace {
        /**
         * @brief Compute theorical notch center on wafer circle.
         */
        Point ComputeTheoricalNotchCenter(const Circle& wafer, NotchLocation notchLocation);

        /**
         * @brief Compute theorical notch center on the polar image.
         */
        Point ComputeTheoricalPolarNotchCenter(const Mat& img, NotchLocation notchLocation);

        /**
         * @brief Compute the region of interest around the notch.
         *        Should not be too small to ensure that the notch will be included in it despite its positional uncertainties,
         *        and not too large to prevent the area from including too much artifact in addition to the notch.
         */
        Rect ComputeNotchROI(const Mat& img, const Circle& wafer, NotchLocation notchLocation, int roiMaxSize = 400);

        /**
         * @brief Compute the region of interest around the notch on the polar image.
         *        Should not be too small to ensure that the notch will be included in it despite its positional uncertainties,
         *        and not too large to prevent the area from including too much artifact in addition to the notch..
         */
        Mat ComputePolarNotchROI(const Mat& img, NotchLocation notchLocation, Point theoricalNotchCenter, int roiMaxWidth = 250, int roiMaxHeight = 30);


        /**
         * @brief Draw all given small circles on image.
         */
        void DrawSmallerCirclesOnImage(Mat& img, vector<Circle> circles);

        /**
         * @brief Draw a given smaller circle on image
         */
        void DrawSmallerCircleOnImage(Mat& img, Circle circle);

        /**
         * @brief Compute the means and std deviations of each row of greyscale image.
                  The areas with high intensity changes are then highlighted in red in the resulting image.
         */
        Mat ComputeDeviationMeanImage(const Mat& img, double deviationFactor, Mat mask = Mat());


        /**
         * @brief Translates point coordinates on a polar wafer image to the corresponding coordinates on the original wafer image.
         *        This method isn't approximate / precise enough to give an accurate location of the entered coordinates in the cartesian image (usually 2-3 pixels off)
         */
        Point PolarToCartesianCoordinates(const Mat& img, Mat baseimg, const Circle& wafer, Point notchCenter);

        pair<double, int> GetMaxSimilarityAndLocation(Mat img, Mat mask, int notchWindowWidth, int roiMaxHeight);

        Mat FilterConnectedComponentsToMask(const Mat& img, int roiMaxHeight, int polarMargin);
    }

    Point FindNotchCenterByPatternRecognition(const Mat& img, const Circle& wafer, NotchLocation notchLocation, float edgeDetectorGamma, int roiMaxSize, bool print)
    {
        Point theoricalNotchCenter = ComputeTheoricalNotchCenter(wafer, notchLocation);

        Rect roi = ComputeNotchROI(img, wafer, notchLocation, roiMaxSize);

        Mat notchImg = img(roi).clone();
        notchImg = filter::edge_detector::ShenGradient(notchImg, edgeDetectorGamma, false);

        Mat notchImgFlipped;
        switch (notchLocation)
        {
        case NotchLocation::Left:
        case NotchLocation::Right:
            flip(notchImg, notchImgFlipped, 0);
            break;
        case NotchLocation::Top:
        case NotchLocation::Bottom:
            flip(notchImg, notchImgFlipped, 1);
            break;
        }



        tuple<double, Point2f, double> result = registration::ComputeAngleAndShift(notchImg, notchImgFlipped);
        Point2f shift = std::get<1>(result);
        Point notchCenter = Point(theoricalNotchCenter.x - (int)(shift.x * 0.5f), theoricalNotchCenter.y - (int)(shift.y * 0.5f));

        /*
        if (print)
        {
            Mat shiftMat = (Mat_<double>(2, 3) << 1, 0, -shift.x, 0, 1, -shift.y);
            Mat notchRealigned = registration::ImgRegistration(notchImg, notchImgFlipped, shiftMat);
            imshow("notch", notchImg);
            imshow("notch flipped", notchImgFlipped);
            imshow("notch realigned", notchRealigned);
            resizeWindow("notch", roiMaxSize, roiMaxSize);
            resizeWindow("notch flipped", roiMaxSize, roiMaxSize);
            resizeWindow("notch realigned", roiMaxSize, roiMaxSize);

            Mat notchCenterImg = img.clone();
            circle(notchCenterImg, notchCenter, 1, Scalar(0, 0, 255), 3, cv::LINE_AA);
            circle(notchCenterImg, theoricalNotchCenter, 1, Scalar(255, 0, 0), 1, cv::LINE_AA);
            notchCenterImg = notchCenterImg(roi).clone();
            imshow("notch center", notchCenterImg);
            resizeWindow("notch center", roiMaxSize, roiMaxSize);

            waitKey();
        }
        */

        return notchCenter;
    }

    Point FindNotchCenterByCorrelation(const Mat& img, const Circle& wafer, NotchLocation notchLocation, float edgeDetectorGamma, int roiMaxSize, bool print)
    {
        Point theoricalNotchCenter = ComputeTheoricalNotchCenter(wafer, notchLocation);

        Rect roi = ComputeNotchROI(img, wafer, notchLocation, roiMaxSize);

        Mat notchImg = img(roi).clone();
        notchImg = filter::edge_detector::ShenGradient(notchImg, edgeDetectorGamma, false);

        double similarity = 0;
        int shiftXMaxSimilarity = 0;
        int shiftYMaxSimilarity = 0;

        switch (notchLocation)
        {
        case NotchLocation::Left:
        case NotchLocation::Right:
            for (int i = 0; i < notchImg.size().height; i++)
            {
                Rect switchedROI = cv::Rect(roi.x, roi.y - i, roi.width, roi.height);
                Mat notchImgFlippedSwitched = img(switchedROI).clone();
                notchImgFlippedSwitched = filter::edge_detector::ShenGradient(notchImgFlippedSwitched, edgeDetectorGamma, false);
                flip(notchImgFlippedSwitched, notchImgFlippedSwitched, 0);

                double currentSimilarity = registration::ComputeSimilarity(notchImg, notchImgFlippedSwitched);
                if (currentSimilarity > similarity)
                {
                    similarity = currentSimilarity;
                    shiftYMaxSimilarity = i;
                }
            }
            break;
        case NotchLocation::Top:
        case NotchLocation::Bottom:
            for (int i = 0; i < notchImg.size().width; i++)
            {
                Rect switchedROI = cv::Rect(roi.x - i, roi.y, roi.width, roi.height);
                Mat notchImgFlippedSwitched = img(switchedROI).clone();
                notchImgFlippedSwitched = filter::edge_detector::ShenGradient(notchImgFlippedSwitched, edgeDetectorGamma, false);
                flip(notchImgFlippedSwitched, notchImgFlippedSwitched, 1);

                double currentSimilarity = registration::ComputeSimilarity(notchImg, notchImgFlippedSwitched);
                if (currentSimilarity > similarity)
                {
                    similarity = currentSimilarity;
                    shiftXMaxSimilarity = i;
                }
            }
            break;
        }

        Point notchCenter = Point(theoricalNotchCenter.x - shiftXMaxSimilarity / 2, theoricalNotchCenter.y - shiftYMaxSimilarity / 2);


        /*
        if (print)
        {
            Mat notchImgFlipped;
            switch (notchLocation)
            {
            case NotchLocation::Left:
            case NotchLocation::Right:
                flip(notchImg, notchImgFlipped, 0);
                break;
            case NotchLocation::Top:
            case NotchLocation::Bottom:
                flip(notchImg, notchImgFlipped, 1);
                break;
            }
            Mat shiftMat = (Mat_<double>(2, 3) << 1, 0, -shiftXMaxSimilarity, 0, 1, -shiftYMaxSimilarity);
            Mat notchRealigned = registration::ImgRegistration(notchImg, notchImgFlipped, shiftMat);
            imshow("notch", notchImg);
            imshow("notch flipped", notchImgFlipped);
            imshow("notch realigned", notchRealigned);
            resizeWindow("notch", roiMaxSize, roiMaxSize);
            resizeWindow("notch flipped", roiMaxSize, roiMaxSize);
            resizeWindow("notch realigned", roiMaxSize, roiMaxSize);

            Mat notchCenterImg = img.clone();
            circle(notchCenterImg, notchCenter, 1, Scalar(0, 0, 255), 3, cv::LINE_AA);
            circle(notchCenterImg, theoricalNotchCenter, 1, Scalar(255, 0, 0), 1, cv::LINE_AA);
            notchCenterImg = notchCenterImg(roi).clone();
            imshow("notch center", notchCenterImg);
            resizeWindow("notch center", roiMaxSize, roiMaxSize);

            waitKey();
        }
        */

        return notchCenter;
    }

    double FindNotchCenterByPolarStats(const Mat& img, const Circle& wafer, NotchLocation notchLocation, int notchWidthInPixel, int widthFactor, double deviationFactor, double similarityThreshold, bool print)
    {

        int polarMargin = 0;

        int roiMaxWidth = (fmod(notchWidthInPixel * widthFactor, 2)) ? notchWidthInPixel * widthFactor : notchWidthInPixel * widthFactor + 1;
        int roiMaxHeight = notchWidthInPixel;

        Mat polarImg = TransformImageToPolar(img, wafer, polarMargin);

        Point theoricalNotchCenter = ComputeTheoricalPolarNotchCenter(polarImg, notchLocation);

        Mat notchImg = ComputePolarNotchROI(polarImg, notchLocation, theoricalNotchCenter, roiMaxWidth, roiMaxHeight);

        if (notchImg.type() == CV_8UC3) cvtColor(notchImg, notchImg, COLOR_BGR2GRAY);

        Mat notchColor = ComputeDeviationMeanImage(notchImg, deviationFactor);

        if (print)
        {
            imshow("color check", notchColor);
            waitKey();
        }

        Mat notchImgBin;
        inRange(notchColor, Scalar(0, 0, 255), Scalar(0, 0, 255), notchImgBin);

        Mat mask = FilterConnectedComponentsToMask(notchImgBin, roiMaxHeight, polarMargin);

        int notchWindowWidth = (fmod(roiMaxHeight * 2, 2)) ? roiMaxHeight * 2 : roiMaxHeight * 2 + 1;

        pair<double, int> maxSimAndLocation = GetMaxSimilarityAndLocation(notchImgBin, mask, notchWindowWidth, roiMaxHeight);

        double maxSimilarity = maxSimAndLocation.first;
        int maxSimilarityLocation = maxSimAndLocation.second;

        Rect roiMaxSimilarity = Rect(maxSimilarityLocation, 0, notchWindowWidth, notchImgBin.rows);
        Mat maxSimilaritySection = notchImgBin(roiMaxSimilarity).clone();

        if (print)
        {
            imshow("ideal section", maxSimilaritySection);
            waitKey();
        }

        int nonZeroPixelsMaxSection = countNonZero(maxSimilaritySection);

        double nonZeroWholePercent = (double)countNonZero(notchImgBin) / (notchImgBin.cols * notchImgBin.rows);

        if ((nonZeroPixelsMaxSection < roiMaxHeight / 2 && nonZeroWholePercent > 0.01) || maxSimilarity < similarityThreshold)
        {
            Mat notchImgBinInv;
            bitwise_not(notchImgBin, notchImgBinInv);
            notchColor = ComputeDeviationMeanImage(notchImg, deviationFactor, notchImgBinInv);

            if (print)
            {
                imshow("color check", notchColor);
                waitKey();
            }

            inRange(notchColor, Scalar(0, 0, 255), Scalar(0, 0, 255), notchImgBin);

            mask = FilterConnectedComponentsToMask(notchImgBin, roiMaxHeight, polarMargin);

            maxSimAndLocation = GetMaxSimilarityAndLocation(notchImgBin, mask, notchWindowWidth, roiMaxHeight);

            maxSimilarity = maxSimAndLocation.first;
            maxSimilarityLocation = maxSimAndLocation.second;

            Rect roiMaxSimilarity = Rect(maxSimilarityLocation, 0, notchWindowWidth, notchImgBin.rows);
            Mat maxSimilaritySection = notchImgBin(roiMaxSimilarity).clone();

            if (print)
            {
                imshow("ideal section", maxSimilaritySection);
                waitKey();
            }
            
        }

        int xShift = notchImgBin.cols / 2 - (maxSimilarityLocation + maxSimilaritySection.cols / 2);

        Point notchCenter = Point(theoricalNotchCenter.x - xShift, theoricalNotchCenter.y - polarMargin);

        if (print)
        {
            Mat shiftCheck = polarImg.clone();
            if (shiftCheck.type() == CV_8UC1) cvtColor(shiftCheck, shiftCheck, COLOR_GRAY2BGR);
            circle(shiftCheck, notchCenter, 1, Scalar(0, 0, 255), -1);
            imwrite("shiftCheck.png", shiftCheck);
        }
        
        Point notchCenterCart = PolarToCartesianCoordinates(polarImg, img, wafer, notchCenter);

        double notchAngle = atan2(notchCenterCart.y - wafer.CenterPos.y, notchCenterCart.x - wafer.CenterPos.x) * (180.0 / CV_PI) - 90.0;

        if (maxSimilarity < similarityThreshold) notchAngle = std::numeric_limits<double>::quiet_NaN();

        return notchAngle;
    }

    Mat TransformImageToPolar(const Mat& img, const Circle& wafer, int polarMargin)
    {

        Mat baseImg = img.clone();
        normalize(baseImg, baseImg, 0, 255, NORM_MINMAX);
        Mat polarImg;
        double radius = (double)(wafer.Diameter * 0.5f);
        int circumference =(int) round(2.0 * CV_PI * radius);
        warpPolar(baseImg, polarImg, Size((int)radius, circumference), wafer.CenterPos, radius + (double)polarMargin, INTER_LINEAR + WARP_FILL_OUTLIERS);
        rotate(polarImg, polarImg, cv::ROTATE_90_CLOCKWISE);

        return polarImg;
    }

    namespace {
        Point ComputeTheoricalNotchCenter(const Circle& wafer, NotchLocation notchLocation)
        {
            float waferRadius = wafer.Diameter * 0.5f;

            float theoricalNotchPosX = wafer.CenterPos.x;
            float theoricalNotchPosY = wafer.CenterPos.y;

            switch (notchLocation)
            {
            case NotchLocation::Left:
                theoricalNotchPosX -= waferRadius;
                break;
            case NotchLocation::Right:
                theoricalNotchPosX += waferRadius;
                break;
            case NotchLocation::Top:
                theoricalNotchPosY -= waferRadius;
                break;
            case NotchLocation::Bottom:
                theoricalNotchPosY += waferRadius;
                break;
            }

            return Point((int) theoricalNotchPosX, (int)theoricalNotchPosY);
        }

        Point ComputeTheoricalPolarNotchCenter(const Mat& img, NotchLocation notchLocation)
        {
            int width = img.size().width;
            int height = img.size().height;
            float theoricalNotchPosX = static_cast<float>(width);
            float theoricalNotchPosY = static_cast<float>(height) - 1;

            switch (notchLocation)
            {
            case NotchLocation::Left:
                theoricalNotchPosX = static_cast<float>(width * 1 / 2);
                break;
            case NotchLocation::Right:
                theoricalNotchPosX = static_cast<float>(width - 1);
                break;
            case NotchLocation::Top:
                theoricalNotchPosX = static_cast<float>(width * 1 / 4);
                break;
            case NotchLocation::Bottom:
                theoricalNotchPosX = static_cast<float>(width * 3 / 4);
                break;
            }

            return Point((int)theoricalNotchPosX, (int)theoricalNotchPosY);
        }

        Rect ComputeNotchROI(const Mat& img, const Circle& wafer, NotchLocation notchLocation, int roiMaxSize)
        {
            Point theoricalNotchCenter = ComputeTheoricalNotchCenter(wafer, notchLocation);

            int width = img.size().width;
            int height = img.size().height;
            int roiHeight = std::min(std::min((height - theoricalNotchCenter.y) * 2, theoricalNotchCenter.y * 2), roiMaxSize);
            int roiWidth = std::min(std::min((width - theoricalNotchCenter.x) * 2, theoricalNotchCenter.x * 2), roiMaxSize);
            auto roiLeftBoundary = theoricalNotchCenter.x - roiWidth / 2;
            auto roiTopBoundary = theoricalNotchCenter.y - roiHeight / 2;
            Rect roi = Rect(roiLeftBoundary, roiTopBoundary, roiWidth, roiHeight);

            return roi;
        }

        Mat ComputePolarNotchROI(const Mat& img, NotchLocation notchLocation, Point theoricalNotchCenter, int roiMaxWidth, int roiMaxHeight)
        {

            Mat polarImg = img.clone();
            Mat notchImg = Mat(roiMaxHeight, roiMaxWidth, polarImg.type());
            int width = polarImg.size().width;
            int height = polarImg.size().height;

            int theoricalNotchPosX = theoricalNotchCenter.x;
            int theoricalNotchPosY = theoricalNotchCenter.y;

            if (notchLocation == NotchLocation::Right)
            {
                Mat notchFirstHalf;
                Mat notchSecondHalf;
                int roiHeight = roiMaxHeight;
                int roiHalfWidth = std::min(roiMaxWidth / 2, polarImg.cols);

                auto roiLeftBoundary = theoricalNotchPosX - roiHalfWidth;
                auto roiTopBoundary = height - roiHeight;
                Rect roi = Rect(roiLeftBoundary, roiTopBoundary, roiHalfWidth + 1, roiHeight);
                notchFirstHalf = polarImg(roi).clone();

                roiLeftBoundary = 0;
                roiTopBoundary = height - roiHeight;
                roi = Rect(roiLeftBoundary, roiTopBoundary, roiHalfWidth, roiHeight);
                notchSecondHalf = polarImg(roi).clone();

                notchFirstHalf.copyTo(notchImg(Rect(0, 0, roiHalfWidth + 1, roiHeight)));
                notchSecondHalf.copyTo(notchImg(Rect(roiHalfWidth + 1, 0, roiHalfWidth, roiHeight)));
            }
            else
            {
                int roiHeight = roiMaxHeight;
                int roiWidth = std::min(roiMaxWidth, (width - theoricalNotchPosX) * 2);
                auto roiLeftBoundary = std::max(0, theoricalNotchPosX - roiWidth / 2);
                auto roiTopBoundary = height - roiHeight;
                Rect roi = Rect(roiLeftBoundary, roiTopBoundary, roiWidth, roiHeight);
                notchImg = polarImg(roi).clone();
            }
            

            return notchImg;
        }

        void DrawSmallerCirclesOnImage(Mat& img, vector<Circle> circles) {
            for (size_t i = 0; i < circles.size(); i++) {
                DrawSmallerCircleOnImage(img, circles[i]);
            }
        }

        void DrawSmallerCircleOnImage(Mat& img, Circle circle) {
            Point2f center = circle.CenterPos;
            cv::circle(img, center, 1, Scalar(0, 255, 0), 2, LINE_AA); // center
            cv::circle(img, center, (int) (circle.Diameter / 2), Scalar(0, 0, 255), 1, LINE_AA);
        }

        Mat ComputeDeviationMeanImage(const Mat& img, double deviationFactor, Mat mask)
        {
            vector<double> means, stddevs;
            Mat notchImg = img.clone();
            Mat notchMask = mask.clone();

            if(notchMask.empty()) notchMask = Mat(notchImg.size(), CV_8UC1, Scalar(255));
            
            for (int i = 0; i < notchImg.rows; i++)
            {
                Mat row = notchImg.row(i).clone();
                Mat maskRow = notchMask.row(i).clone();
                Mat mean, stddev;
                meanStdDev(row, mean, stddev, maskRow);
                means.push_back(mean.at<double>(0, 0));
                stddevs.push_back(stddev.at<double>(0, 0));
            }

            Mat notchColor;
            if (img.type() == CV_8UC1) cvtColor(img, notchColor, COLOR_GRAY2BGR);

            for (int y = 0; y < notchImg.rows; y++)
            {
                for (int x = 0; x < notchImg.cols; x++)
                {
                    //if (notchMask.at<uchar>(y, x) == 0) continue;
                    int pix = notchImg.at<uchar>(y, x);
                    double stat1 = means[y] - (deviationFactor * stddevs[y]);
                    double stat2 = means[y] + (deviationFactor * stddevs[y]);
                    if (pix < stat1 || pix > stat2) circle(notchColor, Point(x, y), 1, Scalar(0, 0, 255));
                }
            }

            return notchColor;
        }

        Point PolarToCartesianCoordinates(const Mat& img, Mat baseimg, const Circle& wafer, Point notchCenter)
        {

            Mat magnitudes = Mat(3, 3, CV_64FC1);
            Mat angles = Mat(3, 3, CV_64FC1);
            Mat x = Mat(3, 3, CV_64FC1);
            Mat y = Mat(3, 3, CV_64FC1);
            double radialFactor = (2 * CV_PI) / (img.cols);
            magnitudes.at<double>(0, 0) = wafer.Diameter * 0.5;
            angles.at<double>(0, 0) = (img.cols - notchCenter.x - 1) * radialFactor;
            /*
            float radialFactor = (2 * CV_PI) / img.cols;
            int radius = wafer.Diameter / 2;
            float theta = (img.cols - notchCenter.x) * radialFactor;
            Point notchCenterCart = Point(0, 0);
            double x, y;
            */
            Point notchCenterCart = Point(0, 0);
            polarToCart(magnitudes, angles, x, y);

            notchCenterCart.x = (int) round(x.at<double>(0, 0) + (double) wafer.CenterPos.x);
            notchCenterCart.y = (int) round(y.at<double>(0, 0) + (double) wafer.CenterPos.y);

            return notchCenterCart;
        }

        pair<double, int> GetMaxSimilarityAndLocation(Mat img, Mat mask, int notchWindowWidth, int roiMaxHeight)
        {
            double maxSimilarity = 0.0;
            int maxSimilarityLocation = 0;

            Mat notchImgBin = img.clone();

            pair<double, int> maxSimAndLocation;

            for (int i = 0; i < notchImgBin.cols - notchWindowWidth; i++)
            {
                Rect roiSection = Rect(i, 0, notchWindowWidth, notchImgBin.rows);
                Mat notchSection = notchImgBin(roiSection).clone();
                Mat maskSection = mask(roiSection).clone();


                Mat notchSectionFiltered(notchImgBin.size(), CV_8UC1, Scalar(0));
                notchSection.copyTo(notchSectionFiltered, maskSection);

                //imshow("testoo", notchSectionFiltered);
                //imshow("bef", notchSection);
                //waitKey();
                

                Mat notchSectionFlipped;
                flip(notchSectionFiltered, notchSectionFlipped, 1);

                double currentSimilarity = registration::ComputeSimilarity(notchSectionFiltered, notchSectionFlipped);
                int nonZeroPixels = countNonZero(notchSectionFiltered);
                if (currentSimilarity > maxSimilarity && nonZeroPixels > roiMaxHeight / 2)
                {
                    maxSimilarity = currentSimilarity;
                    maxSimilarityLocation = i;
                }
            }

            maxSimAndLocation.first = maxSimilarity;
            maxSimAndLocation.second = maxSimilarityLocation;

            return maxSimAndLocation;

        }

        Mat FilterConnectedComponentsToMask(const Mat& img, int roiMaxHeight, int polarMargin)
        {
            Mat notchImgBin = img.clone();
            Mat labels, stats, centroids;
            int componentCount = connectedComponentsWithStats(notchImgBin, labels, stats, centroids);

            Mat mask(labels.size(), CV_8UC1, Scalar(0));

            double lowestAreaFactor = 1.4;
            int heightRatio = 6;
            int lowestWidthRatio = 3;
            double highestWidthFactor = 1.5;
            int positionRatio = 3;

            Mat lowestAreaMask = stats.col(4) > roiMaxHeight * lowestAreaFactor;
            Mat highestAreaMask = stats.col(4) < roiMaxHeight * roiMaxHeight;
            Mat heightMask = stats.col(3) > roiMaxHeight / heightRatio;
            Mat lowestWidthMask = stats.col(2) > roiMaxHeight / lowestWidthRatio;
            Mat highestWidthMask = stats.col(2) < roiMaxHeight * highestWidthFactor;
            Mat positionMask = (notchImgBin.rows - polarMargin) - (stats.col(1) + stats.col(3)) < roiMaxHeight / positionRatio;


            for (int j = 0; j < componentCount; j++)
            {

                if (lowestAreaMask.at<uchar>(j, 0) && highestAreaMask.at<uchar>(j, 0) && heightMask.at<uchar>(j, 0) && lowestWidthMask.at<uchar>(j, 0) && highestWidthMask.at<uchar>(j, 0) && positionMask.at<uchar>(j, 0))
                {
                    mask = mask | (labels == j);
                }
            }

            return mask;
        }
    }
}