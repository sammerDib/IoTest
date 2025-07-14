#include "CppUnitTest.h"

#include "CImageTypeConvertor.hpp"

#pragma unmanaged

#define TEST_DATA_PATH std::string(".\\..\\..\\Tests\\Data\\")

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedOpenCVNativeTests
{
	TEST_CLASS(ImageTypeConvertorTests)
	{
	public:
		
		TEST_METHOD(test_convert_from_RGB_unsigned_8_bits_to_graylevel_unsigned_8_bits)
		{
            Logger::WriteMessage("Start\n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("cat.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            Assert::AreEqual(CV_8UC3, input.type());

            // When
            cv::Mat img8UC1 = Convertor::ConvertTo8UC1(input);

            // Then
            Assert::AreEqual(CV_8UC1, img8UC1.type());
            Logger::WriteMessage("Done\n");
		}

        TEST_METHOD(test_convert_from_RGB_unsigned_8_bits_to_graylevel_float_32_bits)
        {
            Logger::WriteMessage("Start\n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("cat.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            Assert::AreEqual(CV_8UC3, input.type());

            // When
            cv::Mat img32FC1 = Convertor::ConvertTo32FC1(input);

            // Then
            Assert::AreEqual(CV_32FC1, img32FC1.type());
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(test_convert_from_RGB_unsigned_16_bits_to_graylevel_unsigned_8_bits)
        {
            Logger::WriteMessage("Start\n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("cat.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            input.convertTo(input, CV_16UC3, 255.0 / 65535.0);
            Assert::AreEqual(CV_16UC3, input.type());

            // When
            cv::Mat img8UC1 = Convertor::ConvertTo8UC1(input);

            // Then
            Assert::AreEqual(CV_8UC1, img8UC1.type());
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(test_convert_from_RGB_unsigned_16_bits_to_graylevel_float_32_bits)
        {
            Logger::WriteMessage("Start\n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("cat.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            input.convertTo(input, CV_16UC3, 255.0 / 65535.0);
            Assert::AreEqual(CV_16UC3, input.type());

            // When
            cv::Mat img32FC1 = Convertor::ConvertTo32FC1(input);

            // Then
            Assert::AreEqual(CV_32FC1, img32FC1.type());
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(test_convert_from_RGB_float_32_bits_to_graylevel_unsigned_8_bits)
        {
            Logger::WriteMessage("Start\n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("cat.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            input.convertTo(input, CV_32FC3, 1.0 / 255.0);
            Assert::AreEqual(CV_32FC3, input.type());

            // When
            cv::Mat img8UC1 = Convertor::ConvertTo8UC1(input);

            // Then
            Assert::AreEqual(CV_8UC1, img8UC1.type());
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(test_convert_from_RGB_float_32_bits_to_graylevel_float_32_bits)
        {
            Logger::WriteMessage("Start\n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("cat.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_COLOR);
            input.convertTo(input, CV_32FC3, 255.0 / 65535.0);
            Assert::AreEqual(CV_32FC3, input.type());

            // When
            cv::Mat img32FC1 = Convertor::ConvertTo32FC1(input);

            // Then
            Assert::AreEqual(CV_32FC1, img32FC1.type());
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(test_convert_from_graylevel_unsigned_8_bits_to_graylevel_unsigned_8_bits)
        {
            Logger::WriteMessage("Start\n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("cat.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_GRAYSCALE);
            Assert::AreEqual(CV_8UC1, input.type());

            // When
            cv::Mat img8UC1 = Convertor::ConvertTo8UC1(input);

            // Then
            Assert::AreEqual(CV_8UC1, img8UC1.type());
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(test_convert_from_graylevel_unsigned_8_bits_to_graylevel_float_32_bits)
        {
            Logger::WriteMessage("Start\n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("cat.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_GRAYSCALE);
            Assert::AreEqual(CV_8UC1, input.type());

            // When
            cv::Mat img32FC1 = Convertor::ConvertTo32FC1(input);

            // Then
            Assert::AreEqual(CV_32FC1, img32FC1.type());
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(test_convert_from_graylevel_unsigned_16_bits_to_graylevel_unsigned_8_bits)
        {
            Logger::WriteMessage("Start\n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("cat.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_GRAYSCALE);
            input.convertTo(input, CV_16UC1, 255.0 / 65535.0);
            Assert::AreEqual(CV_16UC1, input.type());

            // When
            cv::Mat img8UC1 = Convertor::ConvertTo8UC1(input);

            // Then
            Assert::AreEqual(CV_8UC1, img8UC1.type());
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(test_convert_from_graylevel_unsigned_16_bits_to_graylevel_float_32_bits)
        {
            Logger::WriteMessage("Start\n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("cat.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_GRAYSCALE);
            input.convertTo(input, CV_16UC1, 255.0 / 65535.0);
            Assert::AreEqual(CV_16UC1, input.type());

            // When
            cv::Mat img32FC1 = Convertor::ConvertTo32FC1(input);

            // Then
            Assert::AreEqual(CV_32FC1, img32FC1.type());
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(test_convert_from_graylevel_float_32_bits_to_graylevel_unsigned_8_bits)
        {
            Logger::WriteMessage("Start\n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("cat.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_GRAYSCALE);
            input.convertTo(input, CV_32FC1, 1.0 / 255.0);
            Assert::AreEqual(CV_32FC1, input.type());

            // When
            cv::Mat img8UC1 = Convertor::ConvertTo8UC1(input);

            // Then
            Assert::AreEqual(CV_8UC1, img8UC1.type());
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(test_convert_from_graylevel_float_32_bits_to_graylevel_float_32_bits)
        {
            Logger::WriteMessage("Start\n");
            // Given
            std::string image_path = TEST_DATA_PATH + std::string("cat.png");
            cv::Mat input = cv::imread(image_path, cv::IMREAD_GRAYSCALE);
            input.convertTo(input, CV_32FC1, 255.0 / 65535.0);
            Assert::AreEqual(CV_32FC1, input.type());

            // When
            cv::Mat img32FC1 = Convertor::ConvertTo32FC1(input);

            // Then
            Assert::AreEqual(CV_32FC1, img32FC1.type());
            Logger::WriteMessage("Done\n");
        }
	};
}
