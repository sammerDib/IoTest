#pragma once

#include "Profile2d.h"
#include "ProfileStepAnalyser.h"

struct ProfileEdgeTrimAnalyserParameters;
struct ProfileEdgeTrimAnalyserResult;
class ProfileEdgeTrimAnalyser;

#pragma managed

namespace UnitySCSharedAlgosCppWrapper {
    public ref class ProfileEdgeTrimAnalyserParameters : public ProfileStepAnalyserParameters
    {
    public:
        ProfileEdgeTrimAnalyserParameters(KindStep kindStep, double targetHeight, double toleranceHeight, double targetWidth, double toleranceWidth);
        virtual ~ProfileEdgeTrimAnalyserParameters() override;
        !ProfileEdgeTrimAnalyserParameters();

        const ::ProfileEdgeTrimAnalyserParameters& GetNative();
    };

    public ref class ProfileEdgeTrimAnalyserResult : public ProfileStepAnalyserResult
    {
    public:
        ProfileEdgeTrimAnalyserResult(::ProfileEdgeTrimAnalyserResult* nativeResult);
        ProfileEdgeTrimAnalyserResult(Status status, double stepX, double height, double width);
        virtual ~ProfileEdgeTrimAnalyserResult() override;
        !ProfileEdgeTrimAnalyserResult();

        double GetWidth();
    };

    public ref class ProfileEdgeTrimAnalyser : public ProfileAnalyser
    {
    public:
        ProfileEdgeTrimAnalyser(ProfileEdgeTrimAnalyserParameters^ parameters);
        virtual ~ProfileEdgeTrimAnalyser() override;
        !ProfileEdgeTrimAnalyser();

        ProfileAnalyserResult^ Process(Profile2d^ scan) override;

    private:
        ::ProfileEdgeTrimAnalyser* _native;
    };
}
