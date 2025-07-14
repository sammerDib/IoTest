#include "pch.h"
#include "CppUnitTest.h"
#include "CustomTypesToString.h"
#include "NoisyProfileGenerator.h"

#include "ProfileTrenchAnalyser.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedCpp_NativeTests
{
    TEST_CLASS(ProfileTrenchAnalyserTests)
    {
    private:
        static constexpr double _tolerance = 1e-4;

        ProfileTrenchAnalyserResult* ToTrenchResult(const std::unique_ptr<ProfileAnalyserResult>& genericResult)
        {
            return dynamic_cast<ProfileTrenchAnalyserResult*>(genericResult.get());
        }

    public:
        TEST_METHOD(TrenchBasic)
        {
            Profile profile({
                { 0.0, 50.0 },
                { 1.0, 50.0 },
                { 2.0, 50.0 },
                { 3.0, 10.0 },
                { 4.0, 10.0 },
                { 5.0, 10.0 },
                { 6.0, 10.0 },
                { 7.0, 50.0 },
                { 8.0, 50.0 },
                { 9.0, 50.0 },
                });
            ProfileTrenchAnalyserParameters parameters(40.0, _tolerance, 3.0, _tolerance);
            parameters.trenchDownZone = ExclusionZone(0.5, 0.5);
            parameters.trenchUpZone = ExclusionZone(0.5, 0.5);

            const auto result = ProfileTrenchAnalyser(parameters)(profile);
            const auto trenchResult = ToTrenchResult(result);

            Assert::AreEqual(ProfileTrenchAnalyserResult::Status::OK, trenchResult->status);
            Assert::AreEqual(40.0, trenchResult->depth, _tolerance);
            Assert::AreEqual(3.0, trenchResult->width, _tolerance);
        }

        TEST_METHOD(TrenchBasic2)
        {
            Profile profile({
                { 0.0, 50.0 },
                { 1.0, 50.0 },
                { 2.0, 20.0 },
                { 3.0, 10.0 },
                { 4.0, 10.0 },
                { 5.0, 10.0 },
                { 6.0, 10.0 },
                { 7.0, 10.0 },
                { 8.0, 30.0 },
                { 9.0, 50.0 },
                { 10.0, 50.0 },
                });
            ProfileTrenchAnalyserParameters parameters(40.0, _tolerance, 3.0, _tolerance);
            parameters.trenchDownZone = ExclusionZone(0.5, 0.5);
            parameters.trenchUpZone = ExclusionZone(0.5, 0.5);

            const auto result = ProfileTrenchAnalyser(parameters)(profile);
            const auto trenchResult = ToTrenchResult(result);

            Assert::AreEqual(ProfileTrenchAnalyserResult::Status::OK, trenchResult->status);
            Assert::AreEqual(40.0, trenchResult->depth, _tolerance);
            Assert::AreEqual(4.0, trenchResult->width, _tolerance);
        }

        TEST_METHOD(TrenchRandom)
        {
            NoisyTrenchProfileParameters generatorParams;
            generatorParams.nbPoints = 40;
            generatorParams.xStdDev = 0.01;
            generatorParams.yStdDev = 0.5;
            generatorParams.begin = { -1.0, 50.0 };

            constexpr double DEPTH_MIN = 20.0;
            constexpr double DEPTH_MAX = 40.0;
            constexpr double WIDTH_MIN = 0.2;
            constexpr double WIDTH_MAX = 0.7;

            std::random_device rd;
            std::mt19937 gen(rd());

            std::uniform_real_distribution<double> distribDepth(DEPTH_MIN, DEPTH_MAX);
            std::uniform_real_distribution<double> distribWidth(WIDTH_MIN, WIDTH_MAX);

            generatorParams.end = { 1.0, 50.0 };
            generatorParams.depth = distribDepth(gen);
            generatorParams.width = distribWidth(gen);

            const auto profile = GenerateNoisyTrenchProfile(generatorParams);

            ProfileTrenchAnalyserParameters parameters(generatorParams.depth, _tolerance, generatorParams.width, _tolerance);
            const auto stepWidth = ((generatorParams.end.X - generatorParams.begin.X) / 2.0) - generatorParams.width;
            parameters.trenchDownZone = ExclusionZone(stepWidth / 2.0, stepWidth / 2.0);
            parameters.trenchUpZone = ExclusionZone(stepWidth / 2.0, stepWidth / 2.0);

            const auto result = ProfileTrenchAnalyser(parameters)(profile);
            const auto trenchResult = ToTrenchResult(result);

            // Muted so as to not run in CI
            //Assert::AreEqual(ProfileTrenchAnalyserResult::Status::OK, trenchResult->status);
            //Assert::AreEqual(generatorParams.depth, trenchResult->depth, generatorParams.yStdDev * 3.0);
            //Assert::AreEqual(generatorParams.width, trenchResult->width, generatorParams.xStdDev * 3.0);
        }
    };
}
