#pragma once

#include "Profile2d.h"
#include "ExclusionZone.h"

struct ProfileAnalyserParameters;
struct ProfileAnalyserResult;

#pragma managed

namespace UnitySCSharedAlgosCppWrapper {
    public ref class ProfileAnalyserParameters abstract
    {
    public:
        virtual ~ProfileAnalyserParameters();
        !ProfileAnalyserParameters();

        void AddStartExclusionZone(ExclusionZone^ exclusionZone);
        void AddEndExclusionZone(ExclusionZone^ exclusionZone);

        void SetNbStdDevFiltering(const double nbStdDev);
        void SetMaxConvolutionKernelHalfSize(const int size);

    protected:
        ::ProfileAnalyserParameters* _native;
    };

    public ref class ProfileAnalyserResult abstract
    {
    public:
        virtual ~ProfileAnalyserResult();
        !ProfileAnalyserResult();

        enum class Status
        {
            Ok,
            EmptyProfile,
            EmptyProfileNan,
            ProfileTooSmallAfterStdDevFiltering,
        };

        Status GetStatus();

    protected:
        ::ProfileAnalyserResult* _native;
    };

    public interface class ProfileAnalyser
    {
    public:
        ProfileAnalyserResult^ Process(Profile2d^ scan);
    };
}
