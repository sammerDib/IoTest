#include "CppUnitTest.h"

#pragma unmanaged
#include <opencv2/opencv.hpp>

#include "Linspace.hpp"

#pragma unmanaged
using namespace Microsoft::VisualStudio::CppUnitTestFramework;
#define TEST_DATA_PATH std::string(".\\..\\..\\Tests\\Data\\")
namespace SharedOpenCVNativeTests
{

	TEST_CLASS(SharedOpenCVNativeTests)
	{
	public:
		
		TEST_METHOD(TestImageFormatRead)
		{
            Logger::WriteMessage("Load BMP from Tests\\Data \n");
            cv::Mat img = cv::imread(TEST_DATA_PATH + std::string("Dies.bmp"));
            Assert::IsTrue(img.cols != 0, L"BMP could not be read check Tests data files path or OpenCV feature installed");
            Assert::IsTrue(img.rows != 0, L"BMP could not be read check Tests data files path or OpenCV feature installed");
            Logger::WriteMessage("Done\n");

            Logger::WriteMessage("Load PNG from Tests\\Data \n");
            img = cv::imread(TEST_DATA_PATH + std::string("notch.png"));
            Assert::IsTrue(img.cols != 0, L"PNG could not be read check Tests data files path or OpenCV feature installed");
            Assert::IsTrue(img.rows != 0, L"PNG could not be read check Tests data files path or OpenCV feature installed");
            Logger::WriteMessage("Done\n");

            Logger::WriteMessage("Load TIF from Tests\\Data \n");
            img = cv::imread(TEST_DATA_PATH + std::string("cat_gray8.tif"));
            Assert::IsTrue(img.cols != 0, L"TIF could not be read check Tests data files path or OpenCV feature installed");
            Assert::IsTrue(img.rows != 0, L"TIF could not be read check Tests data files path or OpenCV feature installed");
            Logger::WriteMessage("Done\n");

            Logger::WriteMessage("Load JPG from Tests\\Data \n");
            img = cv::imread(TEST_DATA_PATH + std::string("1_centered_circle.jpg"));
            Assert::IsTrue(img.cols != 0, L"JPG could not be read check Tests data files path or OpenCV feature installed");
            Assert::IsTrue(img.rows != 0, L"JPG could not be read check Tests data files path or OpenCV feature installed");
            Logger::WriteMessage("Done\n");
            
		}

        TEST_METHOD(TestLinspace_Int)
        {
            Logger::WriteMessage("Expect_integer_values_to_be_linearized\n");
            std::vector<int> actual = linspace(0, 10, 6);
            std::vector<int> expected{ 0, 2, 4, 6, 8, 10 };
            Assert::AreEqual(expected.size(), actual.size());
            char buf[255];
            for (size_t index = 0; index < expected.size(); ++index) {
                _snprintf_s(buf, 255, "Expect= %d \t=>\t Actual= %d\n", expected.at(index), actual.at(index));
                Logger::WriteMessage(buf);
                Assert::AreEqual<int>(expected.at(index), actual.at(index));
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(TestLinspace_Double)
        {
            Logger::WriteMessage("Expect_double_values_to_be_linearized\n");
            std::vector<double> actual = linspace(0.0, 1.0, 5);
            std::vector<double> expected{ 0.0, 0.25, 0.5, 0.75, 1.0 };
            Assert::AreEqual(expected.size(), actual.size());
            char buf[255];
            for (size_t index = 0; index < expected.size(); ++index) {
                _snprintf_s(buf, 255, "Expect= %lf \t=>\t Actual= %lf\n", expected.at(index), actual.at(index));
                Logger::WriteMessage(buf);
                Assert::AreEqual<double>(expected.at(index), actual.at(index));
            }
            Logger::WriteMessage("Done\n");
        }
	};
}
