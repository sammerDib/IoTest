#include <algorithm>
#include <cmath>
#include <opencv2/core.hpp>
#include <opencv2/opencv.hpp>
#include <fstream>

#include "ReportingUtils.hpp"

#pragma unmanaged
std::vector<float> createVectorFromMat32(const cv::Mat& image)
{
    if (image.type() != CV_32F)
    {
        throw new std::exception("Image type must be CV_32F in ");
    }
    std::vector<float> array;
    if (image.isContinuous())
    {
        array.assign((float*)image.data, (float*)image.data + image.total() * image.channels());
    }
    else
    {
        for (int i = 0; i < image.rows; ++i)
        {
            array.insert(array.end(), image.ptr<float>(i), image.ptr<float>(i) + image.cols * image.channels());
        }
    }
    return array;
}

void Reporting::writePngImage(const cv::Mat& image, const std::string& filePath)
{
    cv::Mat normalizeImage;
    cv::normalize(image, normalizeImage, 0, 255, cv::NORM_MINMAX);
    cv::imwrite(filePath, normalizeImage);
}

void Reporting::writeRawImage(const cv::Mat& image, const std::string& filePath)
{
    try {
        std::vector<float> array = createVectorFromMat32(image);
        uint32_t height = image.rows;
        uint32_t width = image.cols;

        std::ofstream strm;
        strm.open(filePath, std::ios::out | std::ios::binary);
        strm.write(reinterpret_cast<const char*>(&width), sizeof(uint32_t));
        strm.write(reinterpret_cast<const char*>(&height), sizeof(uint32_t));
        strm.write(reinterpret_cast<const char*>(&array[0]), array.size() * sizeof(float));
        strm.close();
    }
    catch (std::exception e) {
        std::cout << "Error! Could not write Binary file" << e.what() << std::endl;
    }
}

void Reporting::writeYamlImage(const cv::Mat& image, const std::string& filePath, const std::string& imageName)
{
    cv::FileStorage fileStorage(filePath, cv::FileStorage::WRITE);
    fileStorage << imageName << image;
}

cv::Mat Reporting::readYamlImage(const std::string& filePath, const std::string& imageName)
{
    std::cout << "Opening storage for YAML" << '\n';
    std::cout.flush();
    cv::FileStorage fs(filePath, cv::FileStorage::READ);
    std::cout << "Loading YAML" << '\n';
    std::cout.flush();

    if (!fs.isOpened()) {
        throw std::runtime_error("can't open requested file");
    }

    cv::Mat image;
    fs[imageName] >> image;
    std::cout << "Loaded YAML" << '\n';
    return image;
}

cv::Mat Reporting::readF32TxtImage(const std::string& filePath)
{
    std::cout << "TXT" << '\n';
    std::ifstream f(filePath);

    if (!f.is_open()) {
        throw std::runtime_error("can't open requested file");
    }

    cv::Mat image;
    bool first_iter = true;

    std::string line;
    int x{ 0 };
    std::vector<float> line_vec{};

    while (std::getline(f, line))
    {
        std::stringstream lineStream(line);
        std::string s;
        line_vec.clear();

        while (std::getline(lineStream, s, ','))
        {
            float val = std::stof(s);
            line_vec.push_back(val);
        }
        
        if (first_iter) {
            image = cv::Mat(0, line_vec.size(), CV_32F);
            first_iter = false;
        }
        image.push_back(cv::Mat(1, line_vec.size(), CV_32F, line_vec.data()));

        if (++x % 100 == 0) {
            std::cout << "line " << x << std::endl;
        }
    }
    std::cout << "end TXT" << '\n';
    return image;
}

cv::Mat Reporting::asHeatMap(cv::Mat& input, double min, double max, double mean, cv::Mat& mask)
{
    // We want values that range from -255 to 255, negatives becoming blue and positive becoming red
    // and values at the mean become 0
    double half_amplitude = std::max(max - mean, mean - min);
    cv::Mat centered = input - mean;

    std::cout << "mean: " << mean << std::endl;
    std::cout << "half_amplitude: " << half_amplitude << std::endl;

    std::vector<cv::Mat> to_merge{ cv::Mat(), cv::Mat(), cv::Mat() };
    std::cout << "blue..." << std::endl;
    centered.convertTo(to_merge[0], CV_8U, 255.0 / half_amplitude);
    to_merge[0] = mask - to_merge[0];
    std::cout << "red..." << std::endl;
    centered.convertTo(to_merge[2], CV_8U, -255.0 / half_amplitude);
    to_merge[2] = mask - to_merge[2];
    std::cout << "green..." << std::endl;
    cv::Mat(cv::min(to_merge[0], to_merge[2])).convertTo(to_merge[1], CV_8U);
    
    cv::Mat img;
    std::cout << "merging..." << std::endl;
    cv::merge(to_merge, img);
    return img;
}
