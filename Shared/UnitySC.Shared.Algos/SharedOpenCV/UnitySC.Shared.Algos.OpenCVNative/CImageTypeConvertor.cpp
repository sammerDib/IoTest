#include <opencv2/opencv.hpp>

#include "CImageTypeConvertor.hpp"

using namespace std;
using namespace cv;

#pragma unmanaged

vector<Mat> Convertor::ConvertAllToCV32FC1(const vector<Mat>& imgs)
{
    vector<Mat> convertedImgs;
    for (Mat img : imgs)
    {
        if (img.channels() == 3) {
            Mat convertedImg = ConvertBgrToGray(img);
            convertedImgs.push_back(convertedImg);
        }
        else {
            convertedImgs.push_back(img);
        }
    }

    std::for_each(convertedImgs.begin(), convertedImgs.end(), [&](Mat& img)
        {
            if (img.type() != CV_32FC1) {
                double min, max;
                minMaxLoc(img, &min, &max);
                if (min != max) {
                    img.convertTo(img, CV_32FC1, 1.0 / (max - min), -1.0 * min / (max - min));
                }
                else {
                    img.convertTo(img, CV_32FC1, 1.0 / max);
                }
            }
        });
    int t3 = convertedImgs[0].type();

    return convertedImgs;
}

vector<Mat> Convertor::ConvertAllToCV8UC1(const vector<Mat>& imgs)
{
    vector<Mat> convertedImgs;
    for (Mat img : imgs)
    {
        if (img.channels() == 3) {
            Mat convertedImg = ConvertBgrToGray(img);
            convertedImgs.push_back(convertedImg);
        }
        else {
            convertedImgs.push_back(img);
        }
    }

    std::for_each(convertedImgs.begin(), convertedImgs.end(), [&](Mat img)
        {
            if (img.type() != CV_8UC1) {
                double min, max;
                minMaxLoc(img, &min, &max);
                if (min != max) {
                    img.convertTo(img, CV_8UC1, 255.0 / (max - min), -255.0 * min / (max - min));
                }
                else {
                    img.convertTo(img, CV_8UC1, 255.0 / max);
                }
            }
        });

    return convertedImgs;
}

Mat Convertor::ConvertTo32FC1(const Mat& img) {
    if (img.type() == CV_32FC1) {
        return img.clone();
    }

    Mat convertedImg = ConvertBgrToGray(img);

    if (convertedImg.type() != CV_32FC1) {
        double min, max;
        minMaxLoc(convertedImg, &min, &max);
        if (min != max) {
            convertedImg.convertTo(convertedImg, CV_32FC1, 1.0 / (max - min), -1.0 * min / (max - min));
        }
        else {
            convertedImg.convertTo(convertedImg, CV_32FC1, 1.0 / max);
        }
    }

    return convertedImg;
}

Mat Convertor::ConvertTo8UC1(const Mat& img) {
    if (img.type() == CV_8UC1) {
        return img.clone();
    }

    Mat convertedImg = ConvertBgrToGray(img);

    if (convertedImg.type() != CV_8UC1) {
        double min, max;
        minMaxLoc(convertedImg, &min, &max);
        if (min != max) {
            convertedImg.convertTo(convertedImg, CV_8UC1, 255.0 / (max - min), -255.0 * min / (max - min));
        }
        else {
            convertedImg.convertTo(convertedImg, CV_8UC1, 255.0 / max);
        }
    }

    return convertedImg;
}

Mat Convertor::ConvertTo32FC3(const Mat& img) {
    if (img.type() == CV_32FC3) {
        return img.clone();
    }

    Mat convertedImg = ConvertGrayToBgr(img);

    if (convertedImg.type() != CV_32FC3) {
        double min, max;
        minMaxLoc(convertedImg, &min, &max);
        if (min != max) {
            convertedImg.convertTo(convertedImg, CV_32FC3, 1.0 / (max - min), -1.0 * min / (max - min));
        }
        else {
            convertedImg.convertTo(convertedImg, CV_32FC3, 1.0 / max);
        }
    }

    return convertedImg;
}

Mat Convertor::ConvertTo8UC3(const Mat& img) {
    if (img.type() == CV_8UC3) {
        return img.clone();
    }

    Mat convertedImg = ConvertGrayToBgr(img);

    if (convertedImg.type() != CV_8UC3) {
        double min, max;
        minMaxLoc(convertedImg, &min, &max);
        if (min != max) {
            convertedImg.convertTo(convertedImg, CV_8UC3, 255.0 / (max - min), -255.0 * min / (max - min));
        }
        else {
            convertedImg.convertTo(convertedImg, CV_8UC3, 255.0 / max);
        }
    }

    return convertedImg;
}

Mat Convertor::ConvertBgrToGray(const Mat& img) {
    Mat convertedImg;
    switch (img.channels()) {
    case 3:
        cvtColor(img, convertedImg, COLOR_BGR2GRAY);
        break;
    case 1:
        convertedImg = img.clone();
        break;
    default:
        throw runtime_error("[Convert BGR to GRAY] Channel number is unsupported.");
    }
    return convertedImg;
}

Mat Convertor::ConvertGrayToBgr(const Mat& img) {
    Mat convertedImg;
    switch (img.channels()) {
    case 3:
        convertedImg = img.clone();
        break;
    case 1:
        cvtColor(img, convertedImg, COLOR_GRAY2BGR);
        break;
    default:
        throw runtime_error("[Convert GRAY to BGR] Channel number is unsupported.");
    }
    return convertedImg;
}