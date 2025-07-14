#include <opencv2/opencv.hpp>
#include <fstream>

#include <ReportingUtils.hpp>

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