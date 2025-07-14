#include "CUtils.hpp"

#pragma unmanaged

using namespace cv;
using namespace std;

namespace utils {

    Mat CreateMaskForImage(int maskHeight, int maskWidth, cv::Rect roi)
    {
        Mat mask = Mat::zeros(maskHeight, maskWidth, CV_8UC1);
        Rect roiRect = roi & cv::Rect(0, 0, maskHeight, maskWidth);
        rectangle(mask, roiRect, 255, FILLED);
        return mask;
    }

    Mat CreateMaskForImage(int maskHeight, int maskWidth, cv::Point2i center, int radius)
    {
        Mat mask = Mat::zeros(maskHeight, maskWidth, CV_8UC1);
        circle(mask, center, radius, 255, FILLED, LINE_AA);
        return mask;
    }
}
