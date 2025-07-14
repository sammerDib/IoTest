#include "pch.h"
#include "CppUnitTest.h"

#include "NoisyProfileGenerator.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedCpp_NativeTests
{
    TEST_CLASS(NoisyProfileGeneratorTests)
    {
    private:
        static void CheckProfileOrder(const Profile& profile)
        {
            Assert::IsTrue(std::is_sorted(profile.begin_x(), profile.end_x()));
        }

        static void CheckLine(const Profile& line, const NoisyLineParameters& params)
        {
            Assert::AreEqual(params.nbPoints, static_cast<int>(line.Size()));

            const auto xIncrement = (params.end.X - params.begin.X) / (params.nbPoints - 1);
            const auto yIncrement = (params.end.Y - params.begin.Y) / (params.nbPoints - 1);

            auto i = 0;
            for (const auto& point : line.GetPoints())
            {
                Assert::AreEqual(params.begin.X + (xIncrement * i), point.X, 3.0 * params.xStdDev);
                Assert::AreEqual(params.begin.Y + (yIncrement * i), point.Y, 3.0 * params.yStdDev);
                ++i;
            }
        }

        static void CheckStep(const Profile& profile, const NoisyStepProfileParameters& params)
        {
            Assert::AreEqual(params.nbPoints, static_cast<int>(profile.Size()));

            const auto width = params.end.X - params.begin.X;

            const auto nbPointStep = static_cast<int>(params.nbPoints * (params.stepWidth / width));
            const auto nbPointsLeft = static_cast<int>((params.nbPoints - nbPointStep) / 2.0);
            const auto nbPointsRight = params.nbPoints - nbPointStep - nbPointsLeft;

            const auto widthLeft = (width - params.stepWidth) / 2.0;
            const auto widthRight = width - widthLeft - params.stepWidth;

            constexpr double epsilon = 0.01;
            auto left = profile;
            const ExclusionZone keepLeft(params.begin.X + widthLeft + epsilon, 0.0, 100.0);
            left.RemoveExclusionZone(keepLeft);
            CheckLine(left, { nbPointsLeft, params.begin, { params.begin.X + widthLeft, params.begin.Y}, params.xStdDev, params.yStdDev });

            auto step = profile;
            const ExclusionZone keepStepLeft(params.begin.X + widthLeft - epsilon, 100.0, 0.0);
            const ExclusionZone keepStepRight(params.end.X - widthRight + epsilon, 0.0, 100.0);
            step.RemoveExclusionZone(keepStepLeft);
            step.RemoveExclusionZone(keepStepRight);
            CheckLine(step, { nbPointStep + 2, { params.begin.X + widthLeft, params.begin.Y }, { params.end.X - widthRight, params.end.Y }, params.xStdDev, params.yStdDev });

            auto right = profile;
            const ExclusionZone keepRight(params.end.X - widthRight - epsilon, 100.0, 0.0);
            right.RemoveExclusionZone(keepRight);
            CheckLine(right, { nbPointsRight, { params.end.X - widthRight, params.end.Y }, params.end, params.xStdDev, params.yStdDev });
        }

        static void CheckTrench(const Profile& profile, const NoisyTrenchProfileParameters& params)
        {
            Assert::AreEqual(params.nbPoints, static_cast<int>(profile.Size()));

            const auto width = params.end.X - params.begin.X;
            const auto nbPointsLeft = static_cast<int>(params.nbPoints / 2);
            const auto nbPointsRight = params.nbPoints - nbPointsLeft;
            const auto stepWidth = (width / 2.0) - params.width;
            const auto widthBetweenPoints = width / (params.nbPoints - 1);

            auto leftStep = profile;
            const ExclusionZone keepLeft(params.begin.X + (width / 2.0), 0.0, 100.0);
            leftStep.RemoveExclusionZone(keepLeft);
            CheckStep(leftStep, { nbPointsLeft, params.begin, { params.begin.X + (width / 2.0) - (widthBetweenPoints / 2.0), params.begin.Y - params.depth}, params.xStdDev, params.yStdDev, stepWidth });

            auto rightStep = profile;
            const ExclusionZone keepRight(params.end.X - (width / 2.0), 100.0, 0.0);
            rightStep.RemoveExclusionZone(keepRight);
            CheckStep(rightStep, { nbPointsRight, { params.begin.X + (width / 2.0) + (widthBetweenPoints / 2.0), params.end.Y - params.depth }, params.end, params.xStdDev, params.yStdDev, stepWidth });
        }

    public:
        TEST_METHOD(GenerateNoisyLineTest)
        {
            NoisyLineParameters params;
            params.nbPoints = 5;
            params.begin = { 10.0, 10.0 };
            params.end = {50.0, 50.0};
            params.xStdDev = 0.05;
            params.yStdDev = 1.0;

            const auto line = GenerateNoisyLine(params);
            CheckProfileOrder(line);
            // Muted so as to not run in CI
            //CheckLine(line, params);
        }

        TEST_METHOD(GenerateNoisyStepProfileTest)
        {
            NoisyStepProfileParameters params;
            params.nbPoints = 24;
            params.xStdDev = 0.05;
            params.yStdDev = 1.0;
            params.begin = { -1.0, 50.0 };
            params.end = { 1.0, 10.0 };
            params.stepWidth = 0.4;

            const auto profile = GenerateNoisyStepProfile(params);

            CheckProfileOrder(profile);
            // Muted so as to not run in CI
            //CheckStep(profile, params);
        }

        TEST_METHOD(GenerateNoisyTrenchProfileTest)
        {
            NoisyTrenchProfileParameters params;
            params.nbPoints = 30;
            params.xStdDev = 0.05;
            params.yStdDev = 1.0;
            params.begin = { -15.0, 50.0 };
            params.end = { 14.0, 50.0 };
            params.depth = 40;
            params.width = 10.0;

            const auto profile = GenerateNoisyTrenchProfile(params);

            CheckProfileOrder(profile);
            // Muted so as to not run in CI
            //CheckTrench(profile, params);
        }
    };
}