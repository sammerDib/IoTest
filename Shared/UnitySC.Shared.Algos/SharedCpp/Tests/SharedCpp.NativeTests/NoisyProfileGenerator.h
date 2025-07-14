#pragma once
#include "CppUnitTest.h"

#include "ProfileStepAnalyser.h"

namespace Microsoft::VisualStudio::CppUnitTestFramework
{
    struct NoisyLineParameters
    {
        int nbPoints{ 0 };

        geometry::Point2d begin;
        geometry::Point2d end;

        double xStdDev{ 0.0 };
        double yStdDev{ 0.0 };
    };
    struct NoisyStepProfileParameters : public NoisyLineParameters
    {
        double stepWidth{ 0.0 };
    };
    struct NoisyTrenchProfileParameters : public NoisyLineParameters
    {
        double depth{ 0.0 };
        double width{ 0.0 };
    };

    Profile GenerateNoisyLine(const NoisyLineParameters& parameters);
    Profile GenerateNoisyStepProfile(const NoisyStepProfileParameters& parameters);
    Profile GenerateNoisyTrenchProfile(const NoisyTrenchProfileParameters& parameters);
}