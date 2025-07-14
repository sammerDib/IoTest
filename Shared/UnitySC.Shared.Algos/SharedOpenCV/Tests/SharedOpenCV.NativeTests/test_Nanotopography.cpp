#include "CppUnitTest.h"
#include <opencv2/opencv.hpp>
#include <fstream>
#include <iostream>
#include <vector>
#include <numeric>
#include "NanoTopography.hpp"
#include <ReportingUtils.hpp>

using namespace psd;
using namespace std;
using namespace cv;
using namespace Microsoft::VisualStudio::CppUnitTestFramework;

#pragma unmanaged

namespace SharedOpenCVNativeTests
{
    TEST_CLASS(NanotopoTests)
    {
    protected:

        void WriteFile(filesystem::path directoryPathToStoreReport, string filename, const vector<float>& data)
        {
            string path = directoryPathToStoreReport.string() + filename;
            FileStorage fs(path, FileStorage::WRITE);
            fs << "data" << data;
            fs.release();
        }

        void WriteFile(filesystem::path directoryPathToStoreReport, string filename, const vector<int>& data)
        {
            string path = directoryPathToStoreReport.string() + filename;
            FileStorage fs(path, FileStorage::WRITE);
            fs << "data" << data;
            fs.release();
        }

        void WriteFile(filesystem::path directoryPathToStoreReport, string filename, const Mat& data)
        {
            string path = directoryPathToStoreReport.string() + filename;
            FileStorage fs(path, FileStorage::WRITE);
            fs << "data" << data;
            fs.release();
        }

    public:

        TEST_METHOD(test_mask_outlier_normal_with_no_outlier_values)
        {
            // Given
            Mat nx = cv::Mat::ones(100, 100, CV_32FC1);
            Mat ny = cv::Mat::ones(100, 100, CV_32FC1);
            Mat px = cv::Mat::ones(100, 100, CV_32FC1);
            Mat py = cv::Mat::ones(100, 100, CV_32FC1);

            // When
            MaskOutlierNormalOptions options;
            options.maxAllowedGrad = NAN;
            options.outlierIQRScale = 1.5f;
            auto result = MaskOutlierNormal(nx, ny, px, py, options);

            // Then
            for (int row = 0; row < nx.rows; row++) {
                for (int col = 0; col < nx.cols; col++) {
                    Assert::AreEqual(1.f, result.NX.at<float>(row, col), 0.f);
                    Assert::AreEqual(1.f, result.NY.at<float>(row, col), 0.f);
                    Assert::AreEqual(1.f, result.PX.at<float>(row, col), 0.f);
                    Assert::AreEqual(1.f, result.PY.at<float>(row, col), 0.f);
                }
            }
        }

        TEST_METHOD(test_mask_outlier_normal_with_outlier_values)
        {
            // Given
            Mat nx = cv::Mat::ones(100, 100, CV_32FC1);
            nx.at<float>(1, 9) = 1.05;
            nx.at<float>(17, 89) = 10.9;
            Mat ny = cv::Mat::ones(100, 100, CV_32FC1);
            ny.at<float>(1, 9) = 1.01;
            ny.at<float>(17, 89) = 2.9;
            Mat px = cv::Mat::ones(100, 100, CV_32FC1);
            Mat py = cv::Mat::ones(100, 100, CV_32FC1);

            // When
            MaskOutlierNormalOptions options;
            options.maxAllowedGrad = NAN;
            options.outlierIQRScale = 1.5f;
            auto result = MaskOutlierNormal(nx, ny, px, py, options);

            // Then
            Assert::IsTrue(std::isnan(result.NX.at<float>(1, 9)));
            Assert::IsTrue(std::isnan(result.NX.at<float>(17, 89)));
            Assert::IsTrue(std::isnan(result.NY.at<float>(1, 9)));
            Assert::IsTrue(std::isnan(result.NY.at<float>(17, 89)));
            Assert::IsTrue(std::isnan(result.PX.at<float>(1, 9)));
            Assert::IsTrue(std::isnan(result.PX.at<float>(17, 89)));
            Assert::IsTrue(std::isnan(result.PY.at<float>(1, 9)));
            Assert::IsTrue(std::isnan(result.PY.at<float>(17, 89)));
        }

        TEST_METHOD(test_mask_outlier_normal_with_provided_threshold)
        {
            // Given
            Mat nx = cv::Mat::ones(100, 100, CV_32FC1);
            Mat ny = cv::Mat::ones(100, 100, CV_32FC1);
            Mat px = cv::Mat::ones(100, 100, CV_32FC1);
            Mat py = cv::Mat::ones(100, 100, CV_32FC1);

            // When
            MaskOutlierNormalOptions options;
            options.maxAllowedGrad = 0.9f;
            options.outlierIQRScale = 1.5f;
            auto result = MaskOutlierNormal(nx, ny, px, py, options);

            // Then
            for (int row = 0; row < nx.rows; row++) {
                for (int col = 0; col < nx.cols; col++) {
                    Assert::IsTrue(std::isnan(result.NX.at<float>(row, col)));
                    Assert::IsTrue(std::isnan(result.NY.at<float>(row, col)));
                    Assert::IsTrue(std::isnan(result.PX.at<float>(row, col)));
                    Assert::IsTrue(std::isnan(result.PY.at<float>(row, col)));
                }
            }
        }

        TEST_METHOD(test_invalid_indices_are_masked)
        {
            // Given
            int rowNb = 10;
            int colNb = 20;
            Mat px = cv::Mat::ones(rowNb, colNb, CV_32FC1);
            px.at<float>(1, 1) = std::nan("");
            px.at<float>(5, 1) = std::nan("");
            std::vector<int> validRowIndices = { 1, 2, 3, 4, 5, 6, 7 };
            std::vector<int> validColumnIndices = { 0, 1, 2, 3, 4, 5 };

            // When
            cv::Mat mask = GetNonValidMask(px, validColumnIndices, validRowIndices);

            // Then
            Assert::AreEqual(px.rows, mask.rows);
            Assert::AreEqual(px.cols, mask.cols);

            for (int row = 0; row < px.rows; row++) {
                for (int col = 0; col < px.cols; col++) {
                    auto value = mask.at<uchar>(row, col);
                    if (row == 1 && col == 1)
                    {
                        Assert::AreEqual(uchar(1), mask.at<uchar>(row, col));
                    }
                    else if (row == 5 && col == 1)
                    {
                        Assert::AreEqual(uchar(1), mask.at<uchar>(row, col));
                    }
                    else {
                        Assert::AreEqual(uchar(0), mask.at<uchar>(row, col));
                    }
                }
            }
        }

        BEGIN_TEST_METHOD_ATTRIBUTE(only_for_manual_testing_and_debug)
            TEST_IGNORE()
            END_TEST_METHOD_ATTRIBUTE()
            TEST_METHOD(only_for_manual_testing_and_debug)
        {
            // Given

            // Read data
            Mat nx = cv::imread(string(".\\..\\..\\Tests\\Data\\nanotopo\\NX.tif"), cv::IMREAD_UNCHANGED);
            Mat ny = cv::imread(string(".\\..\\..\\Tests\\Data\\nanotopo\\NY.tif"), cv::IMREAD_UNCHANGED);
            Mat px = cv::imread(string(".\\..\\..\\Tests\\Data\\nanotopo\\PX.tif"), cv::IMREAD_UNCHANGED);
            Mat py = cv::imread(string(".\\..\\..\\Tests\\Data\\nanotopo\\PY.tif"), cv::IMREAD_UNCHANGED);

            filesystem::create_directories(".\\..\\..\\Tests\\Data\\nanotopo\\testsReporting");
            filesystem::path directoryPathToStoreReport = ".\\..\\..\\Tests\\Data\\nanotopo\\testsReporting";

            if (!nx.empty() && !ny.empty() && !px.empty() && !py.empty()) {
                // Mask normal with horizontal component above maxAllowedGrad value
                MaskOutlierNormalOptions options;
                options.maxAllowedGrad = NAN;
                options.outlierIQRScale = 1.5f;
                NanoTopoData data = MaskOutlierNormal(nx, ny, px, py, options);

                // Downsampling
                NanoTopoInput inputData;
                inputData.Data = data;
                inputData.ColumnElementNumberByGroup = 128;
                inputData.RowElementNumberByGroup = 128;
                SubsampledData subsampledData = Subsampling(inputData);

                WriteFile(directoryPathToStoreReport, "\\validColumnIndices(scorse_ix_subidx).yml", subsampledData.ValidColumnIndices);
                WriteFile(directoryPathToStoreReport, "\\validRowIndices(scorse_iy_subidx).yml", subsampledData.ValidRowIndices);
                WriteFile(directoryPathToStoreReport, "\\subsampledColumnIndices(scorse_start_ix).yml", subsampledData.SubsampledColumnIndices);
                WriteFile(directoryPathToStoreReport, "\\subsampledRowIndices(scorse_start_iy).yml", subsampledData.SubsampledRowIndices);
                WriteFile(directoryPathToStoreReport, "\\J(scorse_J).yml", subsampledData.J);
                WriteFile(directoryPathToStoreReport, "\\I(scorse_I).yml", subsampledData.I);
                WriteFile(directoryPathToStoreReport, "\\subsampledNX(scorse_NX).yml", subsampledData.AveragedNX);
                WriteFile(directoryPathToStoreReport, "\\subsampledNY(scorse_NY).yml", subsampledData.AveragedNY);
                WriteFile(directoryPathToStoreReport, "\\subsampledPX(scorse_PX).yml", subsampledData.AveragedPX);
                WriteFile(directoryPathToStoreReport, "\\subsampledPY(scorse_PY).yml", subsampledData.AveragedPY);
                WriteFile(directoryPathToStoreReport, "\\subsampledJc(scorse_Jc).yml", subsampledData.Jc);
                WriteFile(directoryPathToStoreReport, "\\subsampledIc(scorse_Ic).yml", subsampledData.Ic);

                cv::Mat mask = GetNonValidMask(px, subsampledData.ValidColumnIndices, subsampledData.ValidRowIndices);
                WriteFile(directoryPathToStoreReport, "\\mask(nonvalid).yml", mask);
            }

            //filesystem::remove_all(directoryPathToStoreReport.string());
        }

        BEGIN_TEST_METHOD_ATTRIBUTE(compare_result_with_expected)
            TEST_IGNORE()
            END_TEST_METHOD_ATTRIBUTE()
            TEST_METHOD(compare_result_with_expected) {
            filesystem::path directoryPathToStoreReport = ".\\..\\..\\Tests\\Data\\nanotopo\\testsReporting";

            auto expected_path = directoryPathToStoreReport.string() + "\\nonvalid.txt";
            auto result_path = directoryPathToStoreReport.string() + "\\mask(nonvalid).yml";
            auto mat_name = "data";
            auto diff_output = directoryPathToStoreReport.string() + "\\nonvalid_diff.yml";
            auto heat_output = directoryPathToStoreReport.string() + "\\nonvalid_heat.tiff";

            cv::Mat expected = Reporting::readF32TxtImage(expected_path);
            std::cout << "expected size: " << expected.size() << '\n';

            cv::Mat result = Reporting::readYamlImage(result_path, mat_name);
            cv::Mat result32f;
            result.convertTo(result32f, CV_32F);
            std::cout << "result size  : " << result32f.size() << '\n';

            std::cout << "Computing diff..." << std::endl;
            cv::Mat diff = expected - result32f;
            std::cout << "Saving it to yaml..." << std::endl;
            Reporting::writeYamlImage(diff, diff_output, "diff");
            std::cout << "Saved." << std::endl;

            double max, min;
            cv::minMaxIdx(diff, &min, &max, NULL, NULL);
            std::cout << "max: " << max << '\n';
            std::cout << "min: " << min << '\n';

            // Mask out any NaN. We're using the fact that NaN != NaN
            auto mask = cv::Mat(diff == diff);
            auto mean = cv::mean(diff, mask)[0];
            std::cout << "mean: " << mean << '\n';

            std::cout << "Creating heatmap..." << std::endl;

            cv::Mat img = Reporting::asHeatMap(diff, min, max, mean, mask);

            std::cout << "Saving it to tiff..." << std::endl;
            cv::imwrite(heat_output, img);
            std::cout << "Saved." << std::endl;
        }
    };
}