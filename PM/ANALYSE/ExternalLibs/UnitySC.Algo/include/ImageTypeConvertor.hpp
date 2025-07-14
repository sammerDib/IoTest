#pragma once

#include <opencv2/opencv.hpp>

class Convertor {
public:
  static cv::Mat ConvertTo32FC1(const cv::Mat &img);
  static cv::Mat ConvertTo8UC1(const cv::Mat &img);
  static cv::Mat ConvertTo32FC3(const cv::Mat& img);
  static cv::Mat ConvertTo8UC3(const cv::Mat& img);
  static cv::Mat ConvertBgrToGray(const cv::Mat &img);
  static cv::Mat ConvertGrayToBgr(const cv::Mat& img);

};