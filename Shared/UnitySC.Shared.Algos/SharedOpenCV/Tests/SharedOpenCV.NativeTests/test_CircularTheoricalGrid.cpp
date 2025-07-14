#include "CircularTheoricalGrid.hpp"
#include <CppUnitTest.h>
#include <CppUnitTestAssert.h>
#include <cstdio>
#include <exception>
#include <iostream>
#include <opencv2/core/hal/interface.h>
#include <opencv2/core/persistence.hpp>
#include <opencv2/core/types.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/opencv.hpp>
#include <ostream>
#include <string>
#include <vector>
#include <fstream>

#pragma unmanaged
#define TEST_DATA_PATH std::string(".\\..\\..\\Tests\\Data\\")
namespace SharedOpenCVNativeTests
{
    using namespace Microsoft::VisualStudio::CppUnitTestFramework;

    const std::string YAML_GRID_EXAMPLE("%YAML:1.0\n---\ngrid:\n   wafer_center: [ 1024., 1024. ]\n   wafer_radius: 1024.\n   die_size: [ 384., 384. ]\n   origin: [ 1024., 1024. ]\n");
    const CircularTheoricalGrid GRID_EXAMPLE(
                std::vector<cv::Point2d>{cv::Point2d(1024.0, 2048.0), cv::Point2d(1024.0, 0.0), cv::Point2d(2048.0, 1024.0), cv::Point2d(0.0, 1024.0)},
                cv::Size2d(384.0, 384.0),
                cv::Point2d(1024.0, 1024.0)
            );
    TEST_CLASS(CircularTheoricalGridTests){
    public:
        BEGIN_TEST_METHOD_ATTRIBUTE(save_grid)
            TEST_IGNORE()
        END_TEST_METHOD_ATTRIBUTE()
        TEST_METHOD(save_grid){
            auto tmpFilePath = ".\\..\\..\\Tests\\Data\\grid_read.tmp.yaml";
            cv::FileStorage fs(tmpFilePath, cv::FileStorage::WRITE);
            
            fs << "grid" << GRID_EXAMPLE;

            std::string content;
            fs.release();

            FILE *fp;
            fopen_s(&fp, tmpFilePath, "r");

            if (fp == 0) {
                // Cleanup
                std::remove(tmpFilePath);
                // Exit test
                Assert::Fail(L"couldn't open file");
            }

            char buf[1024];
            while (size_t len = fread(buf, 1, sizeof(buf), fp))
                content.insert(content.end(), buf, buf + len);
            fclose(fp);

            // Cleanup
            fs.release();
            std::remove(tmpFilePath);

            // results
            std::cout << "content: " << content << '\n';
            Assert::AreEqual(YAML_GRID_EXAMPLE, content);
        }

        BEGIN_TEST_METHOD_ATTRIBUTE(read_grid)
            TEST_IGNORE()
        END_TEST_METHOD_ATTRIBUTE()
        TEST_METHOD(read_grid){
            auto tmpFilePath = ".\\..\\..\\Tests\\Data\\grid_write.tmp.yaml";

            std::ofstream myfile;
            myfile.open(tmpFilePath);
            myfile << YAML_GRID_EXAMPLE;
            myfile.close();

            cv::FileStorage fs(tmpFilePath, cv::FileStorage::READ);
            CircularTheoricalGrid grid;
            fs["grid"] >> grid;
            auto testSuccess = grid == GRID_EXAMPLE;
            
            // Cleanup
            fs.release();
            std::remove(tmpFilePath);

            // results
            if (!testSuccess) {
                Assert::Fail();
            }
        }


        /**********************************     Manual tests     **********************************/

        const std::vector<cv::Point2d> MANUAL_TEST_CIRCLE_PTS {
            {4358, 250}, {5148, 250}, {5934, 9080}, {3560, 9080}
        };

        const CircularTheoricalGrid MANUAL_TEST_CIRCLE_GRID = CircularTheoricalGrid(
            MANUAL_TEST_CIRCLE_PTS,
            (cv::Size2d(5937, 4267) - cv::Size2d(2780, 1076)) / 4.,
            cv::Point2d(2780, 1076)
        );

        const std::vector<std::vector<cv::Rect2d>> RETICLE_DIE_LAYOUT {
            computeDieLayout(
                cv::Size2d(3131, 1438) - cv::Size2d(2790, 1143),
                cv::Point2d(2780, 1076),
                {
                    cv::Point2d(2790, 1143),
                    cv::Point2d(3187, 1148),
                    cv::Point2d(2794, 1440),
                    cv::Point2d(3187, 1441)
                }
            ),
            computeDieLayout(
                cv::Size2d(3171, 1864) - cv::Size2d(2779, 1727),
                cv::Point2d(2780, 1076),
                {
                    cv::Point2d(2779, 1727),
                    cv::Point2d(3173, 1727)

                }
            )
        };

        std::vector<cv::Rect2d> computeDieLayout(cv::Size2d size, cv::Point2d refPoint, std::vector<cv::Point2d> allDiesPos) {
            std::vector<cv::Rect2d> res;
            for (auto diePos : allDiesPos) {
                res.push_back(cv::Rect2d(diePos - refPoint, size));
            }
            return res;
        }

        static void SaveSublayout(
            const cv::Mat &img,
            CircularTheoricalGrid &grid,
            const std::vector<std::vector<cv::Rect2d>> reticleDieLayout,
            const std::string &outGridPath,
            const std::string &refDiePrefix,
            const std::string &refDieExt
        ) {
            std::ofstream gridOut;
            gridOut.open(outGridPath);

            for (int i = 0; i < reticleDieLayout.size(); ++i) {
                cv::Mat refDieImg;
                cv::cvtColor(img(reticleDieLayout[i][0] + grid.origin), refDieImg, cv::COLOR_BGR2GRAY);
                cv::imwrite(
                    refDiePrefix + std::to_string(i) + "." + refDieExt,
                    refDieImg
                );

                auto roiList = grid.ApplySublayout(reticleDieLayout[i]);
                gridOut << "#newDie" << '\n';
                for (auto roi : roiList) {
                    gridOut << roi.x << ',' << roi.y << ';' << roi.width << 'x'
                            << roi.height << '\n';
                }
            }
            gridOut.close();
        }

        static void DrawSublayout(
            cv::Mat &img,
            CircularTheoricalGrid &grid,
            const std::vector<std::vector<cv::Rect2d>> reticleDieLayout,
            const std::vector<cv::Scalar> &diesColor
        ) {
            for (int i = 0; i < reticleDieLayout.size(); ++i) {
                auto roiList = grid.ApplySublayout(reticleDieLayout[i]);
                for (auto roi : roiList) {
                    cv::rectangle(img, roi, diesColor[i], 1);
                }
            }
        }

        static void LoadSublayout(
            std::vector<std::vector<cv::Rect2d>> &diesLayout,
            std::vector<cv::Mat> &refDies,
            const std::string &inGridPath,
            const std::string &refDiePrefix,
            const std::string &refDieExt
        ) {
            std::ifstream gridIn(inGridPath);

            std::string line;
            int dieIndex = 0;
            while (std::getline(gridIn, line))
            {
                 if (line == "#newDie") {
                    diesLayout.push_back(std::vector<cv::Rect2d>());
                    refDies.push_back(cv::imread(refDiePrefix + std::to_string(dieIndex) + '.' + refDieExt));
                    ++dieIndex;
                    continue;
                }

                auto currentDiePos = &diesLayout[diesLayout.size() - 1];
                double x, y, width, height;
                char c[3];
                std::istringstream lineStream(line);
                if (
                    !(lineStream >> x >> c[0] >> y >> c[1] >> width >> c[2] >> height)
                    || c[0] != ',' || c[1] != ';' || c[2] != 'x'
                ) {
                    throw std::runtime_error("wrong sublayout format");
                }

                auto roi = cv::Rect2d(x, y, width, height);
                currentDiePos->push_back(roi);

            }
        }


        const std::string IMG_PATH_NO_EXTENSION = "C:\\Users\\Gwennan\\Documents\\Unity\\USP\\PM\\DEMETER\\Service\\Flows\\UnitySC.PM.DMT.Service.Flows.Test\\Data\\testing_wafer_images\\voids\\voids";
        const std::string IMG_EXTENSION = "tif";

        BEGIN_TEST_METHOD_ATTRIBUTE(manual_save_grid)
            TEST_IGNORE()
        END_TEST_METHOD_ATTRIBUTE()
        TEST_METHOD(manual_save_grid){
            auto outFilePath = IMG_PATH_NO_EXTENSION + ".grid.yaml";
            cv::FileStorage fs(outFilePath, cv::FileStorage::WRITE);
            fs << "grid" << MANUAL_TEST_CIRCLE_GRID;
        }

        BEGIN_TEST_METHOD_ATTRIBUTE(manual_draw_parts)
            TEST_IGNORE()
        END_TEST_METHOD_ATTRIBUTE()
        TEST_METHOD(manual_draw_parts){
            auto inFilePath =  IMG_PATH_NO_EXTENSION + ".grid.yaml";
            CircularTheoricalGrid grid;
            cv::FileStorage fs(inFilePath, cv::FileStorage::READ);
            fs["grid"] >> grid;

            // auto grid = MANUAL_TEST_CIRCLE_GRID;

            auto imgPath =  IMG_PATH_NO_EXTENSION + "." + IMG_EXTENSION;
            auto outImgPath =  IMG_PATH_NO_EXTENSION + ".grid." + IMG_EXTENSION;

            cv::Mat gray = cv::imread(imgPath, -1);
            cv::Mat img;
            cv::cvtColor(gray, img, cv::COLOR_GRAY2BGR);
            grid.DrawParts(img, cv::Scalar(0, 0, 255));
            cv::imwrite(outImgPath, img);
        }

        BEGIN_TEST_METHOD_ATTRIBUTE(manual_draw_grid)
            TEST_IGNORE()
        END_TEST_METHOD_ATTRIBUTE()
        TEST_METHOD(manual_draw_grid){
            auto imgPath =  IMG_PATH_NO_EXTENSION + "." + IMG_EXTENSION;
            auto outImgPath =  IMG_PATH_NO_EXTENSION + ".grid." + IMG_EXTENSION;

            //auto grid = MANUAL_TEST_CIRCLE_GRID;
            auto inFilePath = IMG_PATH_NO_EXTENSION + ".grid.yaml";
            CircularTheoricalGrid grid;
            cv::FileStorage fs(inFilePath, cv::FileStorage::READ);
            fs["grid"] >> grid;

            auto roiList = grid.AsRects();
            cv::Mat img = cv::imread(imgPath, cv::IMREAD_COLOR);
            for (auto roi = roiList.begin(); roi != roiList.end(); roi++) {
                cv::rectangle(img, *roi, cv::Scalar(0, 150, 255), 2);
            }
            grid.DrawParts(img, cv::Scalar(255, 0, 0));

            for (auto pt : MANUAL_TEST_CIRCLE_PTS) {
                cv::circle(img, pt, 15, cv::Scalar(100, 0, 255), -1);
            }

            cv::imwrite(outImgPath, img);
        }

        BEGIN_TEST_METHOD_ATTRIBUTE(manual_draw_save_sublayout)
            TEST_IGNORE()
        END_TEST_METHOD_ATTRIBUTE()
        TEST_METHOD(manual_draw_save_sublayout) {
          auto imgPath = IMG_PATH_NO_EXTENSION + "." + IMG_EXTENSION;
          auto outImgPath = IMG_PATH_NO_EXTENSION + ".sublayout." + IMG_EXTENSION;
          auto outGridPath = IMG_PATH_NO_EXTENSION + ".dies_positions";
          auto refDiePrefix = IMG_PATH_NO_EXTENSION + ".refdie";

          auto inFilePath = IMG_PATH_NO_EXTENSION + ".grid.yaml";
          CircularTheoricalGrid grid;
          cv::FileStorage fs(inFilePath, cv::FileStorage::READ);
          fs["grid"] >> grid;

          cv::Mat img = cv::imread(imgPath, cv::IMREAD_COLOR);

          std::vector<cv::Scalar> diesColor{
                {255, 0, 0}, {0, 255, 0}, {0, 0, 255}, {127, 127, 0}, {0, 127, 127}, {127, 0, 127}
            };

          SaveSublayout(img, grid, RETICLE_DIE_LAYOUT, outGridPath, refDiePrefix, IMG_EXTENSION);
          DrawSublayout(img, grid, RETICLE_DIE_LAYOUT, diesColor);

          cv::imwrite(outImgPath, img);
        }

        BEGIN_TEST_METHOD_ATTRIBUTE(manual_load_sublayout)
            TEST_IGNORE()
        END_TEST_METHOD_ATTRIBUTE()
        TEST_METHOD(manual_load_sublayout) {
            auto imgPath = IMG_PATH_NO_EXTENSION + "." + IMG_EXTENSION;
            auto outImgPath = IMG_PATH_NO_EXTENSION + ".grid." + IMG_EXTENSION;
            auto inGridPath = IMG_PATH_NO_EXTENSION + ".dies_positions";
            auto refDiePrefix = IMG_PATH_NO_EXTENSION + ".refdie";

            auto inFilePath = IMG_PATH_NO_EXTENSION + ".grid.yaml";
            CircularTheoricalGrid grid;
            cv::FileStorage fs(inFilePath, cv::FileStorage::READ);
            fs["grid"] >> grid;

            cv::Mat img = cv::imread(imgPath, cv::IMREAD_COLOR);
            std::vector<std::vector<cv::Rect2d>> diesLayout;
            std::vector<cv::Mat> refDies;

            std::vector<cv::Scalar> diesColor{
                {255, 0, 0}, {0, 255, 0}, {0, 0, 255}, {127, 127, 0}, {0, 127, 127}, {127, 0, 127}
            };
            LoadSublayout(diesLayout, refDies, inGridPath, refDiePrefix, IMG_EXTENSION);
            for (int i = 0; i < diesLayout.size(); ++i)
            {
                for (int j = 0; j < diesLayout[i].size(); ++j)
                {
                    cv::rectangle(img, diesLayout[i][j], diesColor[i]);
                }
            }


            cv::imwrite(outImgPath, img);
        }

        BEGIN_TEST_METHOD_ATTRIBUTE(manual_draw_grid_and_border_dies)
            TEST_IGNORE()
        END_TEST_METHOD_ATTRIBUTE()
        TEST_METHOD(manual_draw_grid_and_border_dies){
            auto imgPath =  IMG_PATH_NO_EXTENSION + "." + IMG_EXTENSION;
            auto outImgPath =  IMG_PATH_NO_EXTENSION + ".grid." + IMG_EXTENSION;

            auto grid = MANUAL_TEST_CIRCLE_GRID;

            cv::Mat gray = cv::imread(imgPath, -1);
            cv::Mat img;

            auto roiList = grid.BorderDies();
            cv::cvtColor(gray, img, cv::COLOR_GRAY2BGR);
            for (auto roi = roiList.begin(); roi != roiList.end(); roi++) {
                cv::rectangle(img, *roi, cv::Scalar(0, 255, 255), 6);
            }

            roiList = grid.AsRects();
            for (auto roi = roiList.begin(); roi != roiList.end(); roi++) {
                cv::rectangle(img, *roi, cv::Scalar(255, 0, 0), 1);
            }
            cv::imwrite(outImgPath, img);
        }

        BEGIN_TEST_METHOD_ATTRIBUTE(manual_generate_grid_and_wafer)
            TEST_IGNORE()
        END_TEST_METHOD_ATTRIBUTE()
        TEST_METHOD(manual_generate_grid_and_wafer){
            cv::Mat waferImg;
            cv::Size imgSize(950, 420);
            int margin = 10;
            int dieSize = 80;
            cv::Point shift(20, 10);

            std::vector<cv::Rect> theoreticalGrid = generateWaferWithDiesAndTheoreticalGrid(
                waferImg,
                imgSize,
                margin * 2 + dieSize,
                std::vector{cv::Rect(margin + shift.x, margin + shift.y, dieSize, dieSize)},
                std::vector{cv::Scalar(0)}
            )[0];

            cv::Rect refDieRoi;
            refDieRoi.x = theoreticalGrid[0].x + shift.x;
            refDieRoi.y = theoreticalGrid[0].y + shift.y;
            refDieRoi.width = theoreticalGrid[0].width;
            refDieRoi.height = theoreticalGrid[0].height;

            cv::Mat refDie = waferImg(refDieRoi);

            auto outImgPath = TEST_DATA_PATH + "generated_grids\\auto_generated_wafer_80_10_shift20x10.png";
            auto outRefImgPath = TEST_DATA_PATH + "generated_grids\\auto_generated_wafer_80_10_shift20x10_ref.png";
            auto outGridPath = TEST_DATA_PATH + "generated_grids\\auto_generated_wafer_80_10_shift20x10.grid";

            std::ofstream gridOut;
            gridOut.open(outGridPath);
            for (auto roi : theoreticalGrid) {
                gridOut << roi.x << ',' << roi.y << ';' << roi.width << 'x' << roi.height << '\n';
            }
            gridOut.close();

            cv::imwrite(outImgPath, waferImg);
            cv::imwrite(outRefImgPath, refDie);
        }

        BEGIN_TEST_METHOD_ATTRIBUTE(manual_generate_grid_and_wafer_two_dies)
            TEST_IGNORE()
        END_TEST_METHOD_ATTRIBUTE()
        TEST_METHOD(manual_generate_grid_and_wafer_two_dies){
            cv::Mat waferImg;
            cv::Size imgSize(1000, 1000);
            int reticleSize = 120;

            std::vector<std::vector<cv::Rect>> theoreticalGrid = generateWaferWithDiesAndTheoreticalGrid(
                waferImg,
                imgSize,
                reticleSize,
                std::vector{
                    cv::Rect(5, 10, 20, 100),
                    cv::Rect(30, 10, 85, 45),
                    cv::Rect(30, 65, 85, 45),
                },
                std::vector{
                    cv::Scalar(0),
                    cv::Scalar(0),
                    cv::Scalar(0),
                }
            );
            
            std::vector<cv::Rect> refDieRoi;
            std::vector<cv::Mat> refDie;
            for (int i = 0; i < theoreticalGrid.size(); ++i)
            {
                auto dieGrid = theoreticalGrid[i];
                auto roi = cv::Rect(dieGrid[0].x, dieGrid[0].y, dieGrid[0].width, dieGrid[0].height);
                refDieRoi.push_back(roi);
                auto img = waferImg(roi);
                refDie.push_back(img);
            }

            auto outImgPath = TEST_DATA_PATH + "generated_grids\\auto_generated_wafer_2dies.png";
            auto outGridPath = TEST_DATA_PATH + "generated_grids\\auto_generated_wafer_2dies.grid";
            auto outRefImg0Path = TEST_DATA_PATH + "generated_grids\\auto_generated_wafer_2dies_ref0.png";
            auto outRefImg1Path = TEST_DATA_PATH + "generated_grids\\auto_generated_wafer_2dies_ref1.png";

            std::ofstream gridOut;
            gridOut.open(outGridPath);
            gridOut << "#newDie" << '\n';
            for (auto roi : theoreticalGrid[0]) {
                gridOut << roi.x << ',' << roi.y << ';' << roi.width << 'x' << roi.height << '\n';
            }
            gridOut << "#newDie" << '\n';
            for (int i = 0; i < theoreticalGrid[1].size(); ++i) {
                auto roi1 = theoreticalGrid[1][i];
                auto roi2 = theoreticalGrid[2][i];
                gridOut << roi1.x << ',' << roi1.y << ';' << roi1.width << 'x' << roi1.height << '\n';
                gridOut << roi2.x << ',' << roi2.y << ';' << roi2.width << 'x' << roi2.height << '\n';
            }
            gridOut.close();

            cv::imwrite(outImgPath, waferImg);
            cv::imwrite(outRefImg0Path, refDie[0]);
            cv::imwrite(outRefImg1Path, refDie[1]);
        }

    };
}
