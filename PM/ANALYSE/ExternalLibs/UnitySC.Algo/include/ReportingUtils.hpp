#pragma once

#include <opencv2/opencv.hpp>

class Reporting {
public:
  static void writePngImage(const cv::Mat& image, const std::string& filePath);
  static void writeRawImage(const cv::Mat& image, const std::string& filePath);
  static void writeYamlImage(const cv::Mat& image, const std::string& filePath, const std::string& imageName);


};