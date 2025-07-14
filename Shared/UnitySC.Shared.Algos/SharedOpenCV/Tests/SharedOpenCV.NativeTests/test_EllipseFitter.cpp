#include "CppUnitTest.h"

// for wstring converions
#include <codecvt>
#include <locale>

#include "HyperAccurateEllipseFitter.hpp"

#pragma unmanaged

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace Algorithms;

namespace SharedOpenCVNativeTests
{
    TEST_CLASS(EllipseFitterTests)
    {
    public:

        TEST_METHOD(expect_fit_to_fail_with_zero_points)
        {
            Logger::WriteMessage("Start\n");
            std::vector<cv::Point2f> points;
            IEllipseFitter::Result result;
            HyperAccurateEllipseFitter fitter;
            auto status = fitter.Fit(points, result);
            Assert::AreEqual((int)StatusCode::BWA_FIT_FAILED, (int)status.code);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(expect_fit_to_fail_with_less_than_five_points)
        {
            Logger::WriteMessage("Start\n");
            std::vector<cv::Point2f> points;
            points.push_back(cv::Point2f(-20 + 50 * cos(0), 30 + 50 * sin(0)));
            points.push_back(cv::Point2f(-20 + 50 * cos(45), 30 + 50 * sin(45)));
            points.push_back(cv::Point2f(-20 + 50 * cos(90), 30 + 50 * sin(90)));
            IEllipseFitter::Result result;
            HyperAccurateEllipseFitter fitter;
            auto status = fitter.Fit(points, result);
            Assert::AreEqual((int)StatusCode::BWA_FIT_FAILED, (int)status.code);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(expect_fit_to_return_nan_with_zero_points)
        {
            Logger::WriteMessage("Start\n");
            std::vector<cv::Point2f> points;
            IEllipseFitter::Result result;
            HyperAccurateEllipseFitter fitter;
            auto status = fitter.Fit(points, result);
            Assert::IsTrue(std::isnan(result.semiMajorAxis));
            Assert::IsTrue(std::isnan(result.semiMajorAxis));
            Assert::IsTrue(std::isnan(result.majorAxisAngleFromHorizontal));
            Assert::IsTrue(std::isnan(result.center.x));
            Assert::IsTrue(std::isnan(result.center.y));
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(expect_fit_to_return_nan_with_less_than_five_points)
        {
            Logger::WriteMessage("Start\n");
            std::vector<cv::Point2f> points;
            points.push_back(cv::Point2f(-20 + 50 * cos(0), 30 + 50 * sin(0)));
            points.push_back(cv::Point2f(-20 + 50 * cos(45), 30 + 50 * sin(45)));
            points.push_back(cv::Point2f(-20 + 50 * cos(90), 30 + 50 * sin(90)));
            IEllipseFitter::Result result;
            HyperAccurateEllipseFitter fitter;
            auto status = fitter.Fit(points, result);
            Assert::IsTrue(std::isnan(result.semiMajorAxis));
            Assert::IsTrue(std::isnan(result.semiMajorAxis));
            Assert::IsTrue(std::isnan(result.majorAxisAngleFromHorizontal));
            Assert::IsTrue(std::isnan(result.center.x));
            Assert::IsTrue(std::isnan(result.center.y));
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(expect_confidence_value_to_be_realistic)
        {
            Logger::WriteMessage("Start\n");
            struct GivenType {
                double rmse;
                double expectedConfidence;
            };

            std::vector<GivenType> givens;

            // lower boundary value
            givens.push_back(GivenType({ 0, 1 }));

            // 1px is close to perfection. Less cannot be garanteed anyway
            givens.push_back(GivenType({ 1, 0.9999999999 }));

            // median value
            givens.push_back(GivenType({ 50, 0.5 }));

            // upper boundary value
            givens.push_back(GivenType({ 100, 0 }));

            wchar_t wbuf[255];
            for (auto const given : givens) {
                IEllipseFitter::Result result;
                HyperAccurateEllipseFitter fitter;
                IEllipseFitter::Result fit;
                fit.rmse = given.rmse;
                auto status = fitter.BuildStatus(fit);
                _snwprintf_s(wbuf, 255, L"Failed for RMSE %lf, expected %lf,  got %lf", given.rmse, given.expectedConfidence, status.confidence);
                Assert::AreEqual(given.expectedConfidence, status.confidence, 10e-3, wbuf);
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(expect_convergence_with_five_points_on_circle)
        {
            Logger::WriteMessage("Start\n");
            HyperAccurateEllipseFitter::Result expected;
            expected.center = cv::Point2f(-20, 30);
            expected.semiMajorAxis = 50;
            expected.semiMinorAxis = 50;
            const double HALF_PIXEL_ERROR = 0.5;
            cv::Point2f pixelSize(1, 1);

            std::vector<cv::Point2f> points;
            points.push_back(cv::Point2f(expected.center.x + expected.semiMajorAxis * cos(0), expected.center.y + expected.semiMinorAxis * sin(0)));
            points.push_back(cv::Point2f(expected.center.x + expected.semiMajorAxis * cos(45), expected.center.y + expected.semiMinorAxis * sin(45)));
            points.push_back(cv::Point2f(expected.center.x + expected.semiMajorAxis * cos(90), expected.center.y + expected.semiMinorAxis * sin(90)));
            points.push_back(cv::Point2f(expected.center.x + expected.semiMajorAxis * cos(115), expected.center.y + expected.semiMinorAxis * sin(115)));
            points.push_back(cv::Point2f(expected.center.x + expected.semiMajorAxis * cos(180), expected.center.y + expected.semiMinorAxis * sin(180)));

            HyperAccurateEllipseFitter fitter;
            IEllipseFitter::Result result;
            auto actual = fitter.Fit(points, result);

            char buf[255];
            _snprintf_s(buf, 255, "fit center/major-axis/minor-axis/rmse: {%lf,%lf} / %lf / %lf\n", result.center.x, result.center.y, result.semiMajorAxis, result.semiMinorAxis, result.rmse);
            Logger::WriteMessage(buf);

            Assert::AreEqual((int)StatusCode::OK, (int)actual.code);
            Assert::AreEqual(expected.center.x, result.center.x, HALF_PIXEL_ERROR);
            Assert::AreEqual(expected.center.y, result.center.y, HALF_PIXEL_ERROR);
            Assert::AreEqual(expected.semiMajorAxis, result.semiMajorAxis, HALF_PIXEL_ERROR);
            Assert::AreEqual(expected.semiMinorAxis, result.semiMinorAxis, HALF_PIXEL_ERROR);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(expect_convergence_with_five_points_on_ellipse)
        {
            Logger::WriteMessage("Start\n");
            HyperAccurateEllipseFitter::Result expected;
            expected.center = cv::Point2f(-20, 30);
            expected.semiMajorAxis = 50;
            expected.semiMinorAxis = 40;
            expected.majorAxisAngleFromHorizontal = 0;
            const double HALF_PIXEL_ERROR = 0.5;
            cv::Point2f pixelSize(1, 1);

            std::vector<cv::Point2f> points;
            points.push_back(cv::Point2f(expected.center.x + expected.semiMajorAxis * cos(0), expected.center.y + expected.semiMinorAxis * sin(0)));
            points.push_back(cv::Point2f(expected.center.x + expected.semiMajorAxis * cos(45), expected.center.y + expected.semiMinorAxis * sin(45)));
            points.push_back(cv::Point2f(expected.center.x + expected.semiMajorAxis * cos(90), expected.center.y + expected.semiMinorAxis * sin(90)));
            points.push_back(cv::Point2f(expected.center.x + expected.semiMajorAxis * cos(115), expected.center.y + expected.semiMinorAxis * sin(115)));
            points.push_back(cv::Point2f(expected.center.x + expected.semiMajorAxis * cos(180), expected.center.y + expected.semiMinorAxis * sin(180)));

            HyperAccurateEllipseFitter fitter;
            IEllipseFitter::Result result;
            auto actual = fitter.Fit(points, result);

            char buf[255];
            _snprintf_s(buf, 255, "fit center/major-axis/minor-axis/rmse: {%lf,%lf} / %lf / %lf\n", result.center.x, result.center.y, result.semiMajorAxis, result.semiMinorAxis, result.rmse);
            Logger::WriteMessage(buf);

            Assert::AreEqual((int)StatusCode::OK, (int)actual.code);
            Assert::AreEqual(expected.center.x, result.center.x, HALF_PIXEL_ERROR);
            Assert::AreEqual(expected.center.y, result.center.y, HALF_PIXEL_ERROR);
            Assert::AreEqual(expected.semiMajorAxis, result.semiMajorAxis, HALF_PIXEL_ERROR);
            Assert::AreEqual(expected.semiMinorAxis, result.semiMinorAxis, HALF_PIXEL_ERROR);
            bool angleIsValid = result.majorAxisAngleFromHorizontal == expected.majorAxisAngleFromHorizontal || result.majorAxisAngleFromHorizontal == expected.majorAxisAngleFromHorizontal + 180;
            Assert::IsTrue(angleIsValid);

            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(expect_convergence_with_five_points_on_ellipse_rotated)
        {
            Logger::WriteMessage("Start\n");

            for (const int& rotation : std::vector<int>{ 5, 10, 35, 45, 90, 115, 135, 175 })
            {
                HyperAccurateEllipseFitter::Result expected;
                expected.center = cv::Point2f(20, 10);
                expected.semiMajorAxis = 50;
                expected.semiMinorAxis = 40;
                expected.majorAxisAngleFromHorizontal = rotation;
                const double HALF_PIXEL_ERROR = 0.5;
                cv::Point2f pixelSize(1, 1);

                float alpha = rotation * CV_PI / 180;
                std::vector<cv::Point2f> points;
                float x0 = expected.semiMajorAxis * cos(0);
                float y0 = expected.semiMinorAxis * sin(0);
                float x0_ = expected.center.x + x0 * cos(alpha) - y0 * sin(alpha);
                float y0_ = expected.center.y + x0 * sin(alpha) + y0 * cos(alpha);
                points.push_back(cv::Point2f(x0_, y0_));
                float x1 = expected.semiMajorAxis * cos(45);
                float y1 = expected.semiMinorAxis * sin(45);
                float x1_ = expected.center.x + x1 * cos(alpha) - y1 * sin(alpha);
                float y1_ = expected.center.y + x1 * sin(alpha) + y1 * cos(alpha);
                points.push_back(cv::Point2f(x1_, y1_));
                float x2 = expected.semiMajorAxis * cos(90);
                float y2 = expected.semiMinorAxis * sin(90);
                float x2_ = expected.center.x + x2 * cos(alpha) - y2 * sin(alpha);
                float y2_ = expected.center.y + x2 * sin(alpha) + y2 * cos(alpha);
                points.push_back(cv::Point2f(x2_, y2_));
                float x3 = expected.semiMajorAxis * cos(105);
                float y3 = expected.semiMinorAxis * sin(105);
                float x3_ = expected.center.x + x3 * cos(alpha) - y3 * sin(alpha);
                float y3_ = expected.center.y + x3 * sin(alpha) + y3 * cos(alpha);
                points.push_back(cv::Point2f(x3_, y3_));
                float x4 = expected.semiMajorAxis * cos(115);
                float y4 = expected.semiMinorAxis * sin(115);
                float x4_ = expected.center.x + x4 * cos(alpha) - y4 * sin(alpha);
                float y4_ = expected.center.y + x4 * sin(alpha) + y4 * cos(alpha);
                points.push_back(cv::Point2f(x4_, y4_));

                HyperAccurateEllipseFitter fitter;
                IEllipseFitter::Result result;
                auto actual = fitter.Fit(points, result);

                char buf[255];
                _snprintf_s(buf, 255, "fit center/major-axis/minor-axis/rmse: {%lf,%lf} / %lf / %lf\n", result.center.x, result.center.y, result.semiMajorAxis, result.semiMinorAxis, result.rmse);
                Logger::WriteMessage(buf);

                Assert::AreEqual((int)StatusCode::OK, (int)actual.code);
                Assert::AreEqual(expected.center.x, result.center.x, HALF_PIXEL_ERROR);
                Assert::AreEqual(expected.center.y, result.center.y, HALF_PIXEL_ERROR);
                Assert::AreEqual(expected.semiMajorAxis, result.semiMajorAxis, HALF_PIXEL_ERROR);
                Assert::AreEqual(expected.semiMinorAxis, result.semiMinorAxis, HALF_PIXEL_ERROR);
                Assert::AreEqual(expected.majorAxisAngleFromHorizontal, result.majorAxisAngleFromHorizontal, HALF_PIXEL_ERROR);
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(expect_hyper_convergence_with_five_points_on_same_half_of_ellipse)
        {
            Logger::WriteMessage("Start\n");
            HyperAccurateEllipseFitter::Result expected;
            expected.center = cv::Point2f(-10, 40);
            expected.semiMajorAxis = 60;
            expected.semiMinorAxis = 20;
            expected.majorAxisAngleFromHorizontal = 25;
            const double HALF_PIXEL_ERROR = 0.5;
            cv::Point2f pixelSize(1, 1);

            float alpha = expected.majorAxisAngleFromHorizontal * CV_PI / 180;
            std::vector<cv::Point2f> points;
            float x0 = expected.semiMajorAxis * cos(10);
            float y0 = expected.semiMinorAxis * sin(10);
            float x0_ = expected.center.x + x0 * cos(alpha) - y0 * sin(alpha);
            float y0_ = expected.center.y + x0 * sin(alpha) + y0 * cos(alpha);
            points.push_back(cv::Point2f(x0_, y0_));
            float x1 = expected.semiMajorAxis * cos(20);
            float y1 = expected.semiMinorAxis * sin(20);
            float x1_ = expected.center.x + x1 * cos(alpha) - y1 * sin(alpha);
            float y1_ = expected.center.y + x1 * sin(alpha) + y1 * cos(alpha);
            points.push_back(cv::Point2f(x1_, y1_));
            float x2 = expected.semiMajorAxis * cos(30);
            float y2 = expected.semiMinorAxis * sin(30);
            float x2_ = expected.center.x + x2 * cos(alpha) - y2 * sin(alpha);
            float y2_ = expected.center.y + x2 * sin(alpha) + y2 * cos(alpha);
            points.push_back(cv::Point2f(x2_, y2_));
            float x3 = expected.semiMajorAxis * cos(35);
            float y3 = expected.semiMinorAxis * sin(35);
            float x3_ = expected.center.x + x3 * cos(alpha) - y3 * sin(alpha);
            float y3_ = expected.center.y + x3 * sin(alpha) + y3 * cos(alpha);
            points.push_back(cv::Point2f(x3_, y3_));
            float x4 = expected.semiMajorAxis * cos(45);
            float y4 = expected.semiMinorAxis * sin(45);
            float x4_ = expected.center.x + x4 * cos(alpha) - y4 * sin(alpha);
            float y4_ = expected.center.y + x4 * sin(alpha) + y4 * cos(alpha);
            points.push_back(cv::Point2f(x4_, y4_));

            HyperAccurateEllipseFitter fitter;
            IEllipseFitter::Result result;
            auto actual = fitter.Fit(points, result);

            char buf[255];
            _snprintf_s(buf, 255, "fit center/major-axis/minor-axis/rmse: {%lf,%lf} / %lf / %lf\n", result.center.x, result.center.y, result.semiMajorAxis, result.semiMinorAxis, result.rmse);
            Logger::WriteMessage(buf);

            Assert::AreEqual((int)StatusCode::OK, (int)actual.code);
            Assert::AreEqual(expected.center.x, result.center.x, HALF_PIXEL_ERROR);
            Assert::AreEqual(expected.center.y, result.center.y, HALF_PIXEL_ERROR);
            Assert::AreEqual(expected.semiMajorAxis, result.semiMajorAxis, HALF_PIXEL_ERROR);
            Assert::AreEqual(expected.semiMinorAxis, result.semiMinorAxis, HALF_PIXEL_ERROR);
            Assert::AreEqual(expected.majorAxisAngleFromHorizontal, result.majorAxisAngleFromHorizontal, HALF_PIXEL_ERROR);
            Logger::WriteMessage("Done\n");
        }
    };
}