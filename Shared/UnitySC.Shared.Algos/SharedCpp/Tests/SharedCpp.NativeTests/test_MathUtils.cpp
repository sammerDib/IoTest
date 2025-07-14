#include "pch.h"
#include "CppUnitTest.h"

#include "MathUtils.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedCpp_NativeTests
{
    TEST_CLASS(MathUtilsTests)
    {
    private:
        static constexpr double _tolerance = 1e-4;

    public:
        TEST_METHOD(MathUtils_Average)
        {
            std::vector<double> v{ 1.0, 2.0, 3.0, 4.0 };
            const auto average = Average(v);
            Assert::AreEqual(2.5, average, _tolerance);
        }

        TEST_METHOD(MathUtils_AverageIt)
        {
            std::vector<double> v{ 1.0, 2.0, 3.0, 4.0 };
            const auto average = Average(v.begin(), v.end());
            Assert::AreEqual(2.5, average, _tolerance);
        }

        TEST_METHOD(MathUtils_WeightedAverage)
        {
            std::vector<double> v{ 1.0, 2.0, 3.0, 4.0 };
            std::vector<double> w{ 0.5, 1.0, 2.0, 1.5 };
            const auto average = WeightedAverage(v, w);
            Assert::AreEqual(2.9, average, _tolerance);
        }

        TEST_METHOD(MathUtils_Variance)
        {
            std::vector<double> v{ 1.0, 2.0, 3.0, 4.0 };
            const auto average = Average(v);
            const auto variance = Variance(v, average);
            Assert::AreEqual(1.6666, variance, _tolerance);
        }

        TEST_METHOD(MathUtils_VarianceIt)
        {
            std::vector<double> v{ 1.0, 2.0, 3.0, 4.0 };
            const auto average = Average(v.begin(), v.end());
            const auto variance = Variance(v.begin(), v.end(), average);
            Assert::AreEqual(1.6666, variance, _tolerance);
        }

        TEST_METHOD(MathUtils_WeightedVariance)
        {
            std::vector<double> v{ 1.0, 2.0, 3.0, 4.0 };
            std::vector<double> w{ 0.5, 1.0, 2.0, 1.5 };
            const auto average = WeightedAverage(v, w);
            const auto variance = WeightedVariance(v, w, average);
            Assert::AreEqual(0.89, variance, _tolerance);
        }

        TEST_METHOD(MathUtils_StdDev)
        {
            std::vector<double> v{ 1.0, 2.0, 3.0, 4.0 };
            const auto average = Average(v);
            const auto stddev = StandardDeviation(v, average);
            Assert::AreEqual(1.2909, stddev, _tolerance);
        }

        TEST_METHOD(MathUtils_StdDevIt)
        {
            std::vector<double> v{ 1.0, 2.0, 3.0, 4.0 };
            const auto average = Average(v.begin(), v.end());
            const auto stddev = StandardDeviation(v.begin(), v.end(), average);
            Assert::AreEqual(1.2909, stddev, _tolerance);
        }

        TEST_METHOD(MathUtils_WeightedStdDev)
        {
            std::vector<double> v{ 1.0, 2.0, 3.0, 4.0 };
            std::vector<double> w{ 0.5, 1.0, 2.0, 1.5 };
            const auto average = WeightedAverage(v, w);
            const auto stddev = WeightedStandardDeviation(v, w, average);
            Assert::AreEqual(0.9433, stddev, _tolerance);
        }
    };
}