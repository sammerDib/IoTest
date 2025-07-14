#pragma once

#include "Profile2d.h"
#include "ExclusionZone.h"
#include "ProfileAnalyser.h"
struct ProfileStepAnalyserParameters;
struct ProfileStepAnalyserResult;
class ProfileStepAnalyser;

#pragma managed

namespace UnitySCSharedAlgosCppWrapper {
    public ref class ProfileStepAnalyserParameters : public ProfileAnalyserParameters
    {
    public:
        enum class KindStep
        {
            Up,
            Down,
        };

        ProfileStepAnalyserParameters(KindStep kindStep, double targetHeight, double tolerance);
        virtual ~ProfileStepAnalyserParameters() override;
        !ProfileStepAnalyserParameters();

        void AddStepExclusionZone(ExclusionZone^ exclusionZone);

        const ::ProfileStepAnalyserParameters& GetNative();
    };

    public ref class ProfileStepAnalyserResult : public ProfileAnalyserResult
    {
    public:
        ProfileStepAnalyserResult(::ProfileStepAnalyserResult* nativeResult);
        ProfileStepAnalyserResult(Status status, double stepX, double stepHeight);
        virtual ~ProfileStepAnalyserResult() override;
        !ProfileStepAnalyserResult();

        double GetStepX();
        double GetStepHeight();
    };

    public ref class ProfileStepAnalyser : public ProfileAnalyser
    {
    public:
        ProfileStepAnalyser(ProfileStepAnalyserParameters^ parameters);
        virtual ~ProfileStepAnalyser() override;
        !ProfileStepAnalyser();

        ProfileAnalyserResult^ Process(Profile2d^ scan) override;

    private:
        ::ProfileStepAnalyser* _native;
    };
}
