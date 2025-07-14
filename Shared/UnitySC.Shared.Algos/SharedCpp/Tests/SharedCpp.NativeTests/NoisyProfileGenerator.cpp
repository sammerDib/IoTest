#include "pch.h"
#include "NoisyProfileGenerator.h"

namespace Microsoft::VisualStudio::CppUnitTestFramework
{
    static std::mt19937 GetRandomGenerator()
    {
        std::random_device rd;
        return std::mt19937(rd());
    }

    Profile GenerateNoisyLine(const NoisyLineParameters& parameters)
    {
        std::normal_distribution<double> xNoiseGen(0.0, parameters.xStdDev);
        std::normal_distribution<double> yNoiseGen(0.0, parameters.yStdDev);
        Profile profile;
        const auto xIncrement = (parameters.end.X - parameters.begin.X) / (parameters.nbPoints - 1);
        const auto yIncrement = (parameters.end.Y - parameters.begin.Y) / (parameters.nbPoints - 1);

        profile.PushBack(parameters.begin);
        for (auto i = 1; i < parameters.nbPoints-1; ++i)
        {
            // Clamp the noise because in a Profile all points must be in X ascending order
            auto xNoise = std::clamp(xNoiseGen(GetRandomGenerator()), -xIncrement / (2.0 + 0.01), xIncrement / (2.0 + 0.01));
            profile.PushBack({ parameters.begin.X + (xIncrement * i) + xNoise, parameters.begin.Y + (yIncrement * i) + yNoiseGen(GetRandomGenerator()) });
        }
        profile.PushBack(parameters.end);

        return profile;
    }

    Profile GenerateNoisyStepProfile(const NoisyStepProfileParameters& parameters)
    {
        const auto widthTotal = parameters.end.X - parameters.begin.X;
        const auto nbPointsStep = static_cast<int>(parameters.nbPoints * (parameters.stepWidth / widthTotal));
        const auto nbPointsLeft = static_cast<int>((parameters.nbPoints - nbPointsStep) / 2 );
        const auto nbPointsRight = parameters.nbPoints - nbPointsLeft - nbPointsStep;

        const auto widthLeft = (widthTotal - parameters.stepWidth) / 2.0;
        const auto widthRight = widthTotal - widthLeft - parameters.stepWidth;

        const geometry::Point2d leftToStep(parameters.begin.X + widthLeft, parameters.begin.Y);
        auto profile = GenerateNoisyLine(
            { nbPointsLeft, parameters.begin, leftToStep, parameters.xStdDev, parameters.yStdDev });
        profile.PopBack();

        const geometry::Point2d stepToRight(parameters.end.X - widthRight, parameters.end.Y);
        // We add 2 because we remove duplicated points
        profile.Concat(GenerateNoisyLine({ nbPointsStep + 2, leftToStep, stepToRight, parameters.xStdDev, parameters.yStdDev }));
        profile.PopBack();

        profile.Concat(GenerateNoisyLine({ nbPointsRight, stepToRight, parameters.end, parameters.xStdDev, parameters.yStdDev }));

        return profile;
    }

    Profile GenerateNoisyTrenchProfile(const NoisyTrenchProfileParameters& parameters)
    {
        const auto totalWidth = parameters.end.X - parameters.begin.X;
        const auto widthBetweenPoints = totalWidth / (parameters.nbPoints - 1);

        NoisyStepProfileParameters paramsLeft;
        paramsLeft.nbPoints = parameters.nbPoints / 2;
        paramsLeft.begin = parameters.begin;
        paramsLeft.end =
        {
            parameters.begin.X + (totalWidth / 2.0) - (widthBetweenPoints / 2.0),
            parameters.begin.Y - parameters.depth
        };
        paramsLeft.xStdDev = parameters.xStdDev;
        paramsLeft.yStdDev = parameters.yStdDev;
        paramsLeft.stepWidth = (totalWidth / 2.0) - parameters.width;

        NoisyStepProfileParameters paramsRight;
        paramsRight.nbPoints = parameters.nbPoints - paramsLeft.nbPoints;
        paramsRight.begin = 
        {
            parameters.begin.X + (totalWidth / 2.0) + (widthBetweenPoints / 2.0),
            parameters.begin.Y - parameters.depth
        };
        paramsRight.end = parameters.end;
        paramsRight.xStdDev = parameters.xStdDev;
        paramsRight.yStdDev = parameters.yStdDev;
        paramsRight.stepWidth = (totalWidth / 2.0) - parameters.width;

        auto leftProfile = GenerateNoisyStepProfile(paramsLeft);
        const auto rightProfile = GenerateNoisyStepProfile(paramsRight);

        leftProfile.Concat(rightProfile);

        return leftProfile;
    }
}
