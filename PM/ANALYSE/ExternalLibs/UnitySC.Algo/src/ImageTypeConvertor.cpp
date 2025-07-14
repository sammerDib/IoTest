#include <opencv2/opencv.hpp>

#include <ImageTypeConvertor.hpp>

cv::Mat Convertor::ConvertTo32FC1(const cv::Mat& img) {
    if (img.type() == CV_32FC1) {
        return img;
    }

    cv::Mat convertedImg = ConvertBgrToGray(img);

    if (convertedImg.type() != CV_32FC1) {
        double min, max;
        cv::minMaxLoc(convertedImg, &min, &max);
        if (min != max) {
            convertedImg.convertTo(convertedImg, CV_32FC1, 1.0 / (max - min), -1.0 * min / (max - min));
        } else {
            convertedImg.convertTo(convertedImg, CV_32FC1, 1.0 / max);
        }
    }

    return convertedImg;
}

cv::Mat Convertor::ConvertTo8UC1(const cv::Mat& img) {
    if (img.type() == CV_8UC1) {
        return img;
    }

    cv::Mat convertedImg = ConvertBgrToGray(img);

    if (convertedImg.type() != CV_8UC1) {
        double min, max;
        cv::minMaxLoc(convertedImg, &min, &max);
        if (min != max) {
            convertedImg.convertTo(convertedImg, CV_8UC1, 255.0 / (max - min), -255.0 * min / (max - min));
        } else {
            convertedImg.convertTo(convertedImg, CV_8UC1, 255.0 / max);
        }
    }

    return convertedImg;
}

cv::Mat Convertor::ConvertTo32FC3(const cv::Mat& img) {
    if (img.type() == CV_32FC3) {
        return img;
    }

    cv::Mat convertedImg = ConvertGrayToBgr(img);

    if (convertedImg.type() != CV_32FC3) {
        double min, max;
        cv::minMaxLoc(convertedImg, &min, &max);
        if (min != max) {
            convertedImg.convertTo(convertedImg, CV_32FC3, 1.0 / (max - min), -1.0 * min / (max - min));
        } else {
            convertedImg.convertTo(convertedImg, CV_32FC3, 1.0 / max);
        }
    }

    return convertedImg;
}

cv::Mat Convertor::ConvertTo8UC3(const cv::Mat& img) {
    if (img.type() == CV_8UC3) {
        return img;
    }

    cv::Mat convertedImg = ConvertGrayToBgr(img);

    if (convertedImg.type() != CV_8UC3) {
        double min, max;
        cv::minMaxLoc(convertedImg, &min, &max);
        if (min != max) {
            convertedImg.convertTo(convertedImg, CV_8UC3, 255.0 / (max - min), -255.0 * min / (max - min));
        } else {
            convertedImg.convertTo(convertedImg, CV_8UC3, 255.0 / max);
        }
    }

    return convertedImg;
}

cv::Mat Convertor::ConvertBgrToGray(const cv::Mat& img) {
    cv::Mat convertedImg;
    switch (img.channels()) {
    case 3:
        cvtColor(img, convertedImg, cv::COLOR_BGR2GRAY);
        break;
    case 1:
        convertedImg = img.clone();
        break;
    default:
        throw std::runtime_error("[Convert BGR to GRAY] Channel number is unsupported.");
    }
    return convertedImg;
}

cv::Mat Convertor::ConvertGrayToBgr(const cv::Mat& img) {
    cv::Mat convertedImg;
    switch (img.channels()) {
    case 3:
        convertedImg = img.clone();
        break;
    case 1:
        cvtColor(img, convertedImg, cv::COLOR_GRAY2BGR);
        break;
    default:
        throw std::runtime_error("[Convert GRAY to BGR] Channel number is unsupported.");
    }
    return convertedImg;
}