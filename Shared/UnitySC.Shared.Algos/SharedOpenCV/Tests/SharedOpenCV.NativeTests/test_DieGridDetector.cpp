#include "CEdgeDetector.hpp"
#include "CppUnitTest.h"
#include "CDetectDies.hpp"
#include <CppUnitTestAssert.h>
#include <exception>
#include <filesystem>
#include <iostream>
#include <fstream>
#include <opencv2/core.hpp>
#include <opencv2/core/base.hpp>
#include <opencv2/core/hal/interface.h>
#include <opencv2/core/mat.hpp>
#include <opencv2/core/types.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/opencv.hpp>
#include <optional>
#include <vector>

#pragma unmanaged

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedOpenCVNativeTests
{

    TEST_CLASS(DieGridDetectorTest)
    {

    public:

        static void load_sublayout(
            std::vector<std::vector<cv::Rect2d>> &dies_layout,
            std::vector<cv::Mat> &ref_dies,
            std::vector<std::optional<cv::Mat>> &die_mask,
            const std::string &in_grid_path,
            const std::string &ref_die_prefix,
            const std::string &ref_die_ext
        ) {
            std::ifstream gridIn(in_grid_path);

            std::string line;
            int die_index = 0;
            while (std::getline(gridIn, line))
            {
                 if (line == "#newDie") {
                    dies_layout.push_back(std::vector<cv::Rect2d>());
                    ref_dies.push_back(cv::imread(ref_die_prefix + std::to_string(die_index) + '.' + ref_die_ext, -1));
                    auto die_mask_path = ref_die_prefix + std::to_string(die_index) + "_mask." + ref_die_ext;
                    if (std::filesystem::exists(die_mask_path)) {
                        die_mask.push_back(cv::imread(die_mask_path, -1));
                    } else {
                        die_mask.push_back(std::nullopt);
                    }
                    ++die_index;
                    continue;
                }

                auto current_die_pos = &dies_layout[dies_layout.size() - 1];
                current_die_pos->push_back(parseRect(line));

            }
        }

        static cv::Point parsePos(std::string content) {
            std::istringstream line_stream(content);
            double x, y;
            char c[1];
            if (
                !(line_stream >> x >> c[0] >> y)
                || c[0] != ','
            ) {
                std::cerr << "Error in '" << content << "' (c[0]: "<< c[0] << ")\n";
                throw new std::runtime_error("parsePos error");
            }
            return cv::Point(x, y);
        }

        static double parseDouble(std::string content) {
            std::istringstream line_stream(content);
            double val;
            if (
                !(line_stream >> val)
            ) {
                throw new std::runtime_error("parsePos error");
            }
            return val;
        }

        static cv::Size parseSize(std::string content) {
            std::istringstream line_stream(content);
            double width, height;
            char c[1];
            if (
                !(line_stream >> width >> c[0] >> height)
                || c[0] != 'x'
            ) {
                throw new std::runtime_error("parsePos error");
            }
            return cv::Size(width, height);
        }

        static cv::Rect2d parseRect(std::string content) {
            std::istringstream line_stream(content);
            double x, y, width, height;
            char c[3];
            if (
                !(line_stream >> x >> c[0] >> y >> c[1] >> width >> c[2] >> height)
                || c[0] != ',' || c[1] != ';' || c[2] != 'x'
            ) {
                throw new std::runtime_error("parseRect error");
            }
            return cv::Rect2d(x, y, width, height);
        }

        static std::vector<psd::DieResult> load_manual_flow_test_detection_results(
            const std::string &results_path
        ) {
            std::ifstream gridIn(results_path);
            std::vector<psd::DieResult> results;
            std::vector<psd::MissingDie> missing;

            std::string line;
            while (std::getline(gridIn, line))
            {
                auto column = line.find('(');
                std::string token = line.substr(0, column);
                if (token == "MissingDie") {
                    throw new std::runtime_error("not yet implemented");
                }
                if (token != "DetectedDie") {
                    throw new std::runtime_error("Formatting error: token is not 'Theoretical'");
                }
                auto real_pos = parsePos(line.substr(line.find("pos:") + 4));
                auto theorical_pos = parsePos(line.substr(line.find("theo:") + 5));
                auto size = parseSize(line.substr(line.find("size:") + 5));
                auto angle = parseDouble(line.substr(line.find("angle:") + 6));
                auto pos_conf = parseDouble(line.substr(line.find("pos_conf:") + 9));
                auto angle_conf = parseDouble(line.substr(line.find("angle_conf:") + 11));
                int pass = parseDouble(line.substr(line.find("pass:") + 5));

                results.push_back(psd::DieResult{
                    cv::Rect(theorical_pos, size),
                    cv::Rect(real_pos, size),
                    angle,
                    pos_conf,
                    angle_conf,
                    0,
                    0,
                    pass
                });
            }

            return results;
        }

        static void drawDieRes(cv::Mat &checkResults, cv::Mat &checkOverlay, psd::DieResult die) {
                cv::Scalar rect_color =
                    die.passNb == 1 ? cv::Scalar(0, 0, 255) :
                    die.passNb == 2 ? cv::Scalar(0, 127, 200) :
                    cv::Scalar(255, 0, 0);
                cv::Scalar line_color =
                    die.passNb == 1 ? cv::Scalar(255, 0, 0) :
                    die.passNb == 2 ? cv::Scalar(255, 100, 0) :
                    cv::Scalar(255, 0, 0);

                auto center = die.DetectedROI.tl() + (cv::Point) die.DetectedROI.size()/2;
                std::vector<cv::Point2f> pts;
                cv::RotatedRect(center, die.DetectedROI.size(), die.Angle).points(pts);
                std::vector<cv::Point> pts_int;
                for (auto pt : pts) {
                    pts_int.push_back(pt);
                }
                cv::polylines(checkOverlay, std::vector<std::vector<cv::Point>> {
                    std::vector<std::vector<cv::Point>> {pts_int}
                }, true, rect_color, 1);

                cv::circle(
                    checkOverlay,
                    cv::Point(die.DetectedROI.x + die.DetectedROI.width / 2, die.DetectedROI.y + die.DetectedROI.height / 2),
                    20, rect_color, 2);

                cv::line(checkResults,
                         cv::Point(die.DetectedROI.x + die.DetectedROI.width / 2, die.DetectedROI.y + die.DetectedROI.height / 2),
                         cv::Point(die.TheoreticalROI.x + die.TheoreticalROI.width / 2, die.TheoreticalROI.y + die.TheoreticalROI.height / 2),
                         line_color, 1);
                cv::line(checkOverlay,
                         cv::Point(die.DetectedROI.x + die.DetectedROI.width / 2, die.DetectedROI.y + die.DetectedROI.height / 2),
                         cv::Point(die.TheoreticalROI.x + die.TheoreticalROI.width / 2, die.TheoreticalROI.y + die.TheoreticalROI.height / 2),
                         line_color, 3);

                double pos_line_len, angle_line_len;
                cv::Scalar pos_line_col, angle_line_col;
                angle_line_col = cv::Scalar(255, 255, 0);
                if (die.PosConfidence >= 0) {
                    pos_line_len = ((double) die.DetectedROI.width) * die.PosConfidence;
                    angle_line_len = ((double) die.DetectedROI.width) * die.AngleConfidence;
                    pos_line_col = cv::Scalar(0, 255, 0);
                } else {
                    pos_line_len = (double) die.DetectedROI.width;
                    angle_line_len = 0;
                    pos_line_col = cv::Scalar(0, 180, 255);
                }

                cv::line(checkResults,
                         die.DetectedROI.tl() + cv::Point(0, die.DetectedROI.height - 15),
                         die.DetectedROI.tl() + cv::Point(angle_line_len, die.DetectedROI.height - 15),
                         angle_line_col, 2);
                cv::line(checkOverlay,
                         die.DetectedROI.tl() + cv::Point(0, die.DetectedROI.height - 15),
                         die.DetectedROI.tl() + cv::Point(angle_line_len, die.DetectedROI.height - 15),
                         angle_line_col, 2);

                cv::line(checkResults,
                         die.DetectedROI.tl() + cv::Point(0, die.DetectedROI.height - 20),
                         die.DetectedROI.tl() + cv::Point(pos_line_len, die.DetectedROI.height - 20),
                         pos_line_col, 2);
                cv::line(checkOverlay,
                         die.DetectedROI.tl() + cv::Point(0, die.DetectedROI.height - 20),
                         die.DetectedROI.tl() + cv::Point(pos_line_len, die.DetectedROI.height - 20),
                         pos_line_col, 2);
            };

        const std::string IMG_PATH_NO_EXTENSION = "C:\\Users\\Gwennan\\Documents\\Unity\\USP\\PM\\DEMETER\\Service\\Flows\\UnitySC.PM.DMT.Service.Flows.Test\\Data\\testing_wafer_images\\voids\\voids";
        const std::string IMG_EXTENSION = "tif";
        const std::string RESULTS_DIR = "C:\\Users\\Gwennan\\Documents\\Unity\\USP\\PM\\DEMETER\\Service\\Flows\\UnitySC.PM.DMT.Service.Flows.Test\\Data\\testing_wafer_images\\results\\";

        BEGIN_TEST_METHOD_ATTRIBUTE(pass1_manual_test)
            TEST_IGNORE()
        END_TEST_METHOD_ATTRIBUTE()
        TEST_METHOD(pass1_manual_test)
        {
            auto img_path = IMG_PATH_NO_EXTENSION + "." + IMG_EXTENSION;
            auto out_img_path = IMG_PATH_NO_EXTENSION + ".pass1_res." + IMG_EXTENSION;
            auto inGridPath = IMG_PATH_NO_EXTENSION + ".dies_positions";
            auto refDiePrefix = IMG_PATH_NO_EXTENSION + ".refdie";

            cv::Mat waferImg = cv::imread(img_path, -1);
            std::vector<std::vector<cv::Rect2d>> dies_layout;
            std::vector<cv::Mat> refDies;
            std::vector<std::optional<cv::Mat>> die_masks;

            load_sublayout(dies_layout, refDies, die_masks, inGridPath, refDiePrefix, IMG_EXTENSION);

            cv::Mat waferMask = cv::Mat(waferImg.size(), CV_8UC1, cv::Scalar(255));
            double confidenceThreshold = 0.7;
            double angleConfidenceThreshold = 0.8;
            
            cv::Mat checkResults;
            cv::cvtColor(waferImg, checkResults, cv::COLOR_GRAY2BGR);
            // a copy of checkResults that will also have transparent elements
            cv::Mat checkOverlay(checkResults.clone());
            double alpha = 0.3;
            double gamma = 0.25;

            auto waferImgProcessed = filter::edge_detector::ShenGradient(waferImg, gamma, false, false);
            for (int i = 0; i < refDies.size(); ++i)
            {
                std::vector<cv::Point> elmts;
                auto refDieProcessed = filter::edge_detector::ShenGradient(refDies[i], gamma, true, false);
                for (auto e : dies_layout[i]) {
                    elmts.push_back(e.tl());
                }

                auto pass1Res = psd::DetectDiesPass1(
                    waferImgProcessed,
                    refDieProcessed,
                    waferMask, elmts, i, confidenceThreshold, angleConfidenceThreshold,
                    [
                        &checkResults, &checkOverlay, &waferImgProcessed, &refDieProcessed, &waferMask,
                        &die_mask = die_masks[i].has_value() ? die_masks[i].value() : cv::noArray()
                    ](psd::DieResult die){
                        psd::DetectAngle(waferImgProcessed, refDieProcessed, waferMask, die);
                        drawDieRes(checkResults,checkOverlay,die);
                    }
                );

                for (auto die : pass1Res.missingDies) {
                    drawDieRes(checkResults,checkOverlay,die);
                }
            }


            cv::Mat res(waferImg.size(), waferImg.type());
            cv::addWeighted(checkOverlay, alpha, checkResults, 1 - alpha, 0, res);
            cv::imwrite(RESULTS_DIR + "check.png", res);
        }

        BEGIN_TEST_METHOD_ATTRIBUTE(detect_dies_manual_test)
            TEST_IGNORE()
        END_TEST_METHOD_ATTRIBUTE()
        TEST_METHOD(detect_dies_manual_test)
        {
            auto img_path = IMG_PATH_NO_EXTENSION + "." + IMG_EXTENSION;
            auto out_img_path = IMG_PATH_NO_EXTENSION + ".pass1_res." + IMG_EXTENSION;
            auto inGridPath = IMG_PATH_NO_EXTENSION + ".dies_positions";
            auto refDiePrefix = IMG_PATH_NO_EXTENSION + ".refdie";

            cv::Mat waferImg = cv::imread(img_path, -1);
            std::vector<std::vector<cv::Rect2d>> dies_layout;
            std::vector<cv::Mat> refDies;
            std::vector<std::optional<cv::Mat>> die_masks;

            load_sublayout(dies_layout, refDies, die_masks, inGridPath, refDiePrefix, IMG_EXTENSION);

            cv::Mat waferMask = cv::Mat(waferImg.size(), CV_8UC1, cv::Scalar(255));
            double confidenceThreshold = 0.7;
            double angleConfidenceThreshold = 0.8;

            
            cv::Mat checkResults;
            cv::cvtColor(waferImg, checkResults, cv::COLOR_GRAY2BGR);
            // a copy of checkResults that will also have transparent elements
            cv::Mat checkOverlay(checkResults.clone());
            double alpha = 0.3;
            double gamma = 0.25;

            std::vector<std::vector<cv::Point>> elmts;
            for (auto die_layout : dies_layout) {
                std::vector<cv::Point> die_elems;
                for (auto e : die_layout) {
                    die_elems.push_back(e.tl());
                }
                elmts.push_back(die_elems);
            }

            psd::DetectDies(
                waferImg, waferMask, refDies, elmts, 0, gamma,
                false, confidenceThreshold, angleConfidenceThreshold,
                [&checkResults, &checkOverlay](psd::DieResult die){
                    drawDieRes(checkResults, checkOverlay, die);
                },
                [&checkResults, &checkOverlay](psd::MissingDie die){
                    drawDieRes(
                        checkResults,
                        checkOverlay,
                        psd::DieResult {
                            die.TheoreticalROI,
                            die.TheoreticalROI,
                            0.,
                            0.,
                            0.,
                            die.index,
                            die.dieKindIndex,
                            -1
                        }
                    );
                }
            );


            cv::Mat res(waferImg.size(), waferImg.type());
            cv::addWeighted(checkOverlay, alpha, checkResults, 1 - alpha, 0, res);
            cv::imwrite(RESULTS_DIR + "check.png", res);
        }

        BEGIN_TEST_METHOD_ATTRIBUTE(manual_draw_flow_results)
            TEST_IGNORE()
        END_TEST_METHOD_ATTRIBUTE()
        TEST_METHOD(manual_draw_flow_results)
        {
            auto results_file = IMG_PATH_NO_EXTENSION + ".detected_positions";
            auto img_path = IMG_PATH_NO_EXTENSION + "." + IMG_EXTENSION;

            auto results = load_manual_flow_test_detection_results(results_file);

            cv::Mat waferImg = cv::imread(img_path, cv::IMREAD_COLOR);
            cv::Mat checkOverlay(waferImg.clone());
            

            for (auto &die : results) {
                drawDieRes(waferImg, checkOverlay, die);
            }

            cv::Mat res;
            double alpha = 0.3;
            cv::addWeighted(checkOverlay, alpha, waferImg, 1 - alpha, 0, res);

            cv::imwrite(IMG_PATH_NO_EXTENSION + "_flow_results." + IMG_EXTENSION, res);
        }
    };
}
