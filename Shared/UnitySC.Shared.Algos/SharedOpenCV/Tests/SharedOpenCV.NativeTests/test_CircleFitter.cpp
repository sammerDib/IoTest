#include "CppUnitTest.h"

// for wstring converions
#include <codecvt>
#include <locale>

#include "HyperAccurateCircleFitter.hpp"

#pragma unmanaged

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace Algorithms;

namespace SharedOpenCVNativeTests
{
	TEST_CLASS(CircleFitterTests)
	{
	public:
		
		TEST_METHOD(_01_Expect_fit_to_fail_with_zero_points)
		{
            Logger::WriteMessage("Start\n");
            std::vector<cv::Point2d> points;
            ICircleFitter::Result result;
            HyperAccurateCircleFitter fitter;
            auto status = fitter.Fit(points, result);
            Assert::AreEqual((int)StatusCode::BWA_FIT_FAILED, (int)status.code);
            Logger::WriteMessage("Done\n");
		}

        TEST_METHOD(_02_Expect_fit_to_return_nan_with_zero_points)
        {
            Logger::WriteMessage("Start\n");
            std::vector<cv::Point2d> points;
            ICircleFitter::Result result;
            HyperAccurateCircleFitter fitter;
            auto status = fitter.Fit(points, result);
            Assert::IsTrue(std::isnan(result.radius));
            Assert::IsTrue(std::isnan(result.center.x));
            Assert::IsTrue(std::isnan(result.center.y));
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_03_Expect_confidence_value_to_be_realistic)
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
                ICircleFitter::Result result;
                HyperAccurateCircleFitter fitter;
                ICircleFitter::Result fit;
                fit.rmse = given.rmse;
                auto status = fitter.BuildStatus(fit);
                _snwprintf_s(wbuf, 255, L"Failed for RMSE %lf, expected %lf,  got %lf", given.rmse, given.expectedConfidence, status.confidence);
                Assert::AreEqual(given.expectedConfidence, status.confidence, 10e-3, wbuf);
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_04_Expect_hyper_convergence_with_three_points)
        {
            Logger::WriteMessage("Start\n");
            HyperAccurateCircleFitter::Result expected;
            expected.center = cv::Point2d(75, 75);
            expected.radius = 50;
            const double HALF_PIXEL_ERROR = 0.5;
            cv::Point2d pixelSize(1, 1);

            std::vector<cv::Point2d> points;
            points.push_back(cv::Point2d(107, 37));
            points.push_back(cv::Point2d(105, 114));
            points.push_back(cv::Point2d(36, 106));

            HyperAccurateCircleFitter fitter;
            ICircleFitter::Result result;
            auto actual = fitter.Fit(points, result);

            char buf[255];
            _snprintf_s(buf, 255, "hyperFit center/radius/rmse: {%lf,%lf} / %lf / %lf\n", result.center.x, result.center.y, result.radius, result.rmse);
            Logger::WriteMessage(buf);
      
            Assert::AreEqual(expected.center.x, result.center.x, HALF_PIXEL_ERROR);
            Assert::AreEqual(expected.center.y, result.center.y, HALF_PIXEL_ERROR);
            Assert::AreEqual(expected.radius, result.radius, HALF_PIXEL_ERROR);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_05_Expect_hyper_convergence_with_three_wafer_points)
        {
            // a circle of radius 15, shifted 1,1 to upper right corner
            Logger::WriteMessage("Start\n");
            HyperAccurateCircleFitter::Result expected;
            expected.center = cv::Point2d(1, 1);
            expected.radius = 15;
            const double HALF_PIXEL_ERROR = 0.5;
            cv::Point2d pixelSize(1, 1);

            std::vector<cv::Point2d> points;
            points.push_back(cv::Point2d(1, 16));
            points.push_back(cv::Point2d(16, 0));
            points.push_back(cv::Point2d(1, -14));

            HyperAccurateCircleFitter fitter;
            ICircleFitter::Result result;
            auto actual = fitter.Fit(points, result);

            char buf[255];
            _snprintf_s(buf, 255, "hyperFit center/radius/rmse: {%lf,%lf} / %lf / %lf\n", result.center.x, result.center.y, result.radius, result.rmse);
            Logger::WriteMessage(buf);

            Assert::AreEqual(expected.center.x, result.center.x, HALF_PIXEL_ERROR);
            Assert::AreEqual(expected.center.y, result.center.y, HALF_PIXEL_ERROR);
            Assert::AreEqual(expected.radius, result.radius, HALF_PIXEL_ERROR);
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(_06_Expect_hyper_convergence_with_three_points_on_same_half)
        {
            Logger::WriteMessage("Start\n");
            HyperAccurateCircleFitter::Result expected;
            expected.center = cv::Point2d(1, 1);
            expected.radius = 15;
            const double HALF_PIXEL_ERROR = 0.5;
            cv::Point2d pixelSize(1, 1);

            std::vector<cv::Point2d> points;
            points.push_back(cv::Point2d(1, 16)); // top
            points.push_back(cv::Point2d(16, 1)); // right

            points.push_back(cv::Point2d(1, -14));
            points.push_back(cv::Point2d(-8, -11));

            HyperAccurateCircleFitter fitter;
            ICircleFitter::Result result;
            auto actual = fitter.Fit(points, result);
#pragma warning(suppress : 4996)
            std::wstring resWCharMsg = std::wstring_convert<std::codecvt_utf8<wchar_t>>().from_bytes(result.message);
            Assert::IsTrue(result.success, std::wstring(L"Fit must succeed (failed: " + resWCharMsg + L")").c_str());

            char buf[255];
            _snprintf_s(buf, 255, "hyperFit center/radius/rmse: {%lf,%lf} / %lf / %lf\n", result.center.x, result.center.y, result.radius, result.rmse);
            Logger::WriteMessage(buf);

            Assert::AreEqual(expected.center.x, result.center.x, HALF_PIXEL_ERROR);
            Assert::AreEqual(expected.center.y, result.center.y, HALF_PIXEL_ERROR);
            Assert::AreEqual(expected.radius, result.radius, HALF_PIXEL_ERROR);
            Logger::WriteMessage("Done\n");
        }

	};
}
