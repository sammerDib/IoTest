#include "pch.h"
#include "CppUnitTest.h"
#include "CustomTypesToString.h"
#include "NoisyProfileGenerator.h"

#include "ProfileEdgeTrimAnalyser.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedCpp_NativeTests
{
    TEST_CLASS(ProfileEdgeTrimAnalyserTests)
    {
    private:
        static constexpr double _tolerance = 1e-4;

        ProfileEdgeTrimAnalyserResult* ToEdgeTrimResult(const std::unique_ptr<ProfileAnalyserResult>& genericResult)
        {
            return dynamic_cast<ProfileEdgeTrimAnalyserResult*>(genericResult.get());
        }

    public:
        TEST_METHOD(EdgeTrimDownBasic)
        {
            Profile profile({
                { 0.0, 50.0 },
                { 1.0, 50.0 },
                { 2.0, 50.0 },
                { 3.0, 10.0 },
                { 4.0, 10.0 },
                { 5.0, 10.0 },
                { 6.0, 10.0 },
            });
            ProfileEdgeTrimAnalyserParameters parameters(KindStep::UP, 
                40.0, _tolerance, 3.0, _tolerance);
            parameters.stepZone = ExclusionZone(0.5, 0.5);

            const auto result = ProfileEdgeTrimAnalyser(parameters)(profile);
            const auto edgeTrimResult = ToEdgeTrimResult(result);

            Assert::AreEqual(ProfileEdgeTrimAnalyserResult::Status::OK, edgeTrimResult->status);
            Assert::AreEqual(40.0, edgeTrimResult->stepHeight, _tolerance);
            Assert::AreEqual(3.0, edgeTrimResult->width, _tolerance);
        }

        TEST_METHOD(EdgeTrimRandom)
        {
            NoisyStepProfileParameters generatorParams;
            generatorParams.nbPoints = 40;
            generatorParams.xStdDev = 0.01;
            generatorParams.yStdDev = 0.5;
            generatorParams.begin = { -1.0, 50.0 };

            constexpr double WIDTH_MIN = 0.2;
            constexpr double WIDTH_MAX = 0.7;
            constexpr double HEIGHT_MIN = 20.0;
            constexpr double HEIGHT_MAX = 40.0;

            std::random_device rd;
            std::mt19937 gen(rd());

            std::uniform_real_distribution<double> distribWidth(WIDTH_MIN, WIDTH_MAX);
            std::uniform_real_distribution<double> distribHeight(HEIGHT_MIN, HEIGHT_MAX);

            generatorParams.stepWidth = distribWidth(gen);
            const auto leftWidth = (2.0 - generatorParams.stepWidth) / 2.0;
            const auto rightWidth = (2.0 - generatorParams.stepWidth) / 2.0;
            const auto height = distribHeight(gen);
            generatorParams.end = { 1.0, generatorParams.begin.Y - height };

            const auto profile = GenerateNoisyStepProfile(generatorParams);

            ProfileEdgeTrimAnalyserParameters parameters(KindStep::DOWN,
                height, 0.5, rightWidth, 0.1);
            parameters.stepZone = ExclusionZone(generatorParams.stepWidth / 2.0, generatorParams.stepWidth / 2.0);

            const auto result = ProfileEdgeTrimAnalyser(parameters)(profile);
            const auto edgeTrimResult = ToEdgeTrimResult(result);

            // Muted so as to not run in CI
            Assert::AreEqual(ProfileEdgeTrimAnalyserResult::Status::OK, edgeTrimResult->status);
            Assert::AreEqual(height, edgeTrimResult->stepHeight, 0.5);
            Assert::AreEqual(rightWidth, edgeTrimResult->width, 2.0 / generatorParams.nbPoints);
        }
    };
}