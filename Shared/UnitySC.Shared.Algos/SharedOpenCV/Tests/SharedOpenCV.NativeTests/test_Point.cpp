#include "CppUnitTest.h"

#include "Point.hpp"

#pragma unmanaged

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedOpenCVNativeTests
{
	TEST_CLASS(PointTests)
	{
	public:
		
		TEST_METHOD(Expect_radians_from_0_to_two_pi_anticlockwise)
		{
            Logger::WriteMessage("Start\n");
            // cartesian point <=> expected polar point
            using expectation = std::pair<cv::Point2i, cv::Point2d>;

            using result = std::pair<bool, cv::Point2d>;

            std::vector<expectation> given;
            std::vector<result> expectationResults;

            given.push_back(expectation(cv::Point2i(0, 0), cv::Point2d(0, 0)));
            given.push_back(expectation(cv::Point2i(10, 0), cv::Point2d(10, 0)));
            given.push_back(expectation(cv::Point2i(0, 10), cv::Point2d(10, CV_PI / 2)));
            given.push_back(expectation(cv::Point2i(-10, 0), cv::Point2d(10, CV_PI)));
            given.push_back(expectation(cv::Point2i(0, -10), cv::Point2d(10, 3.0 / 2 * CV_PI)));

            const double radiusForTenByTenSide = std::sqrt(std::pow(10, 2) + std::pow(10, 2));
            given.push_back(expectation(cv::Point2i(-10, -10), cv::Point2d(radiusForTenByTenSide, 5.0 / 4 * CV_PI)));
            given.push_back(expectation(cv::Point2i(10, -10), cv::Point2d(radiusForTenByTenSide, 7.0 / 4 * CV_PI)));

            // when
            for (auto const& test : given) {
                auto cartesianPoint = test.first;
                auto expectedPolarPoint = test.second;
                auto actual = Point::CartesianToPolar(cartesianPoint);
                expectationResults.push_back(result(actual == expectedPolarPoint, actual));
            }

            // then
            int index = 0;
            for (auto const& expectationResult : expectationResults) {
                auto const& expectation = given.at(index);
                auto const& result = expectationResult;
                if (!expectationResult.first)
                {
                    char buf[255];
                    _snprintf_s(buf, 255, "[%d] Fail point : {%d,%d} Expect : {%lf,%lf} Actual : {%lf,%lf}\n",
                        index, expectation.first.x, expectation.first.y, expectation.second.x, expectation.second.y, expectationResult.second.x, expectationResult.second.y);
                    Logger::WriteMessage(buf);
                }
                Assert::IsTrue(expectationResult.first, L"Conversion failed");
                index++;
            }
            Logger::WriteMessage("Done\n");
		}

        TEST_METHOD(Expect_conversion_to_be_reversible)
        {
            Logger::WriteMessage("Start\n");

            // cartesian -> polar -> cartesian
            {
                cv::Point2d cartesian(-3084.6, -147689.2);
                cv::Point2d polar = Point::CartesianToPolar(cartesian);
                cv::Point2d reCartesian = Point::PolarToCartesian(polar);
                Assert::AreEqual(cartesian.x, reCartesian.x, 10e-1);
                Assert::AreEqual(cartesian.y, reCartesian.y, 10e-1);
            }

            // polar -> cartesian -> polar
            {
                cv::Point2d polar = cv::Point2d(147689.2, 4.691506);
                cv::Point2d cartesian = Point::PolarToCartesian(polar);
                cv::Point2d rePolar = Point::CartesianToPolar(cartesian);
                Assert::AreEqual(polar.x, rePolar.x, 10e-3);
                Assert::AreEqual(polar.y, rePolar.y, 10e-3);
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(Expect_radians_to_degree_to_be_right)
        {
            Logger::WriteMessage("Start\n");
            // given
            using expectation = std::pair<cv::Point2d, cv::Point2d>;
            using result = std::pair<bool, cv::Point2d>;

            std::vector<expectation> given;
            std::vector<result> expectationResults;

            given.push_back(expectation(cv::Point2d(0, 0), cv::Point2d(0, 0)));
            given.push_back(expectation(cv::Point2d(0, CV_PI / 2), cv::Point2d(0, 90)));
            given.push_back(expectation(cv::Point2d(0, CV_PI), cv::Point2d(0, 180)));
            given.push_back(expectation(cv::Point2d(0, 3.0 / 2 * CV_PI), cv::Point2d(0, 270)));
            given.push_back(expectation(cv::Point2d(0, 2.0 * CV_PI), cv::Point2d(0, 360)));

            // when
            for (auto const& test : given) {
                auto cartesianPoint = test.first;
                auto expectedPolarPoint = test.second;
                auto actual = Point::PolarRadianToDegree(cartesianPoint);
                expectationResults.push_back(result(actual == expectedPolarPoint, actual));
            }

            // then
            int index = 0;
            for (auto const& expectationResult : expectationResults) {
                auto const& expectation = given.at(index);
                auto const& result = expectationResult;
                if (!expectationResult.first)
                {
                    char buf[255];
                    _snprintf_s(buf, 255, "[%d] Fail point : {%lf,%lf} Expect : {%lf,%lf} Actual : {%lf,%lf}\n",
                        index, expectation.first.x, expectation.first.y, expectation.second.x, expectation.second.y, expectationResult.second.x, expectationResult.second.y);
                    Logger::WriteMessage(buf);
                }
                Assert::IsTrue(expectationResult.first, L"Conversion failed");
                index++;
            }
            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(Expect_polar_point_to_be_convertible_to_cartesian)
        {
            Logger::WriteMessage("Start\n");
            //  expected polar point <=> expected cartesian point
            using expectation = std::pair<cv::Point2d, cv::Point2d>;

            // note: Point::PolarToCartesian gives double,
            // but we box to int here to avoid rounding issue.
            // BTW test values are choosen to be round.
            using result = std::pair<bool, cv::Point2i>;

            std::vector<expectation> given;
            std::vector<result> expectationResults;

            given.push_back(expectation(cv::Point2d(0, 0), cv::Point2i(0, 0)));
            given.push_back(expectation(cv::Point2d(10, 0), cv::Point2i(10, 0)));
            given.push_back(expectation(cv::Point2d(10, CV_PI / 2), cv::Point2i(0, 10)));
            given.push_back(expectation(cv::Point2d(10, CV_PI), cv::Point2i(-10, 0)));
            given.push_back(expectation(cv::Point2d(10, 3.0 / 2 * CV_PI), cv::Point2i(0, -10)));

            const double radiusForTenByTenSide = std::sqrt(std::pow(10, 2) + std::pow(10, 2));
            given.push_back(expectation(cv::Point2d(radiusForTenByTenSide, 5.0 / 4 * CV_PI), cv::Point2i(-10, -10)));
            given.push_back(expectation(cv::Point2d(radiusForTenByTenSide, 7.0 / 4 * CV_PI), cv::Point2i(10, -10)));

            // when
            for (auto const& test : given) {
                auto polarPoint = test.first;
                cv::Point2i expectedCartesianPoint = test.second;
                cv::Point2i actual = Point::PolarToCartesian(polarPoint);
                expectationResults.push_back(result(actual == expectedCartesianPoint, actual));
            }

            // then
            int index = 0;
            for (auto const& expectationResult : expectationResults) {
                auto const& expectation = given.at(index);
                auto const& result = expectationResult;
                if (!expectationResult.first)
                {
                    char buf[255];
                    _snprintf_s(buf, 255, "[%d] Fail point : {%lf,%lf} Expect : {%lf,%lf} Actual : {%d,%d}\n",
                        index, expectation.first.x, expectation.first.y, expectation.second.x, expectation.second.y, expectationResult.second.x, expectationResult.second.y);
                    Logger::WriteMessage(buf);
                }
                Assert::IsTrue(expectationResult.first, L"Conversion failed");
                index++;
            }
            Logger::WriteMessage("Done\n");
        }
	};
}
