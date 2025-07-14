#pragma once

#include "Profile2d.h"
#include "ExclusionZone.h"
#include "ProfileAnalyser.h"

struct ProfileTrenchAnalyserParameters;
struct ProfileTrenchAnalyserResult;
class ProfileTrenchAnalyser;

#pragma managed

namespace UnitySCSharedAlgosCppWrapper {
    public ref class ProfileTrenchAnalyserParameters : public ProfileAnalyserParameters
    {
    public:
        ProfileTrenchAnalyserParameters(double targetDepth, double toleranceDepth, double targetWidth, double toleranceWidth);
        virtual ~ProfileTrenchAnalyserParameters() override;
        !ProfileTrenchAnalyserParameters();

        void AddTrenchDownExclusionZone(ExclusionZone^ exclusionZone);
        void AddTrenchUpExclusionZone(ExclusionZone^ exclusionZone);

        const ::ProfileTrenchAnalyserParameters& GetNative();
    };

    public ref class ProfileTrenchAnalyserResult : public ProfileAnalyserResult
    {
    public:
        ProfileTrenchAnalyserResult(::ProfileTrenchAnalyserResult* nativeResult);
        ProfileTrenchAnalyserResult(Status status, double depth, double width);
        ~ProfileTrenchAnalyserResult();
        !ProfileTrenchAnalyserResult();

        double GetDepth();
        double GetWidth();
    };

    public ref class ProfileTrenchAnalyser : public ProfileAnalyser
    {
    public:
        ProfileTrenchAnalyser(ProfileTrenchAnalyserParameters^ parameters);
        ~ProfileTrenchAnalyser();
        !ProfileTrenchAnalyser();

        ProfileAnalyserResult^ Process(Profile2d^ scan) override;

    private:
        ::ProfileTrenchAnalyser* _native;
    };
}

