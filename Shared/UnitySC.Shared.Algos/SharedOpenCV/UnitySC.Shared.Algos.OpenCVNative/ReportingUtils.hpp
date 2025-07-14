#pragma once

#include <opencv2/opencv.hpp>

#pragma unmanaged
class Reporting {
public:
  static void writePngImage(const cv::Mat& image, const std::string& filePath);
  static void writeRawImage(const cv::Mat& image, const std::string& filePath);
  static void writeYamlImage(const cv::Mat& image, const std::string& filePath, const std::string& imageName);
  static cv::Mat readYamlImage(const std::string& filePath, const std::string& imageName);

  /// Read an image in TXT format, recieved from the innovation team. It is
  /// assumed to be in a CSV-like format, that is where each cell is seperated
  /// by a comma and each line by a new line ('\n')
  static cv::Mat readF32TxtImage(const std::string& filePath);
  static cv::Mat asHeatMap(cv::Mat& input, double min, double max, double mean, cv::Mat& mask);
};