#include "pch.h"
#include "ProfileTrenchAnalyser.h"

#include "../Include/ProfileTrenchAnalyser.h"

#pragma managed

namespace UnitySCSharedAlgosCppWrapper
{
    ProfileTrenchAnalyserParameters::ProfileTrenchAnalyserParameters(
        double targetDepth, double toleranceDepth, double targetWidth, double toleranceWidth)
    {
        _native = new ::ProfileTrenchAnalyserParameters(
            targetDepth, toleranceDepth, targetWidth, toleranceWidth);
    }

    ProfileTrenchAnalyserParameters::~ProfileTrenchAnalyserParameters()
    {
        this->!ProfileTrenchAnalyserParameters();
    }

    ProfileTrenchAnalyserParameters::!ProfileTrenchAnalyserParameters()
    {
        delete _native;
        _native = nullptr;
    }

    void ProfileTrenchAnalyserParameters::AddTrenchDownExclusionZone(ExclusionZone^ exclusionZone)
    {
        static_cast<::ProfileTrenchAnalyserParameters*>(_native)->trenchDownZone = exclusionZone->GetNative();
    }

    void ProfileTrenchAnalyserParameters::AddTrenchUpExclusionZone(ExclusionZone^ exclusionZone)
    {
        static_cast<::ProfileTrenchAnalyserParameters*>(_native)->trenchUpZone = exclusionZone->GetNative();
    }

    const ::ProfileTrenchAnalyserParameters& ProfileTrenchAnalyserParameters::GetNative()
    {
        return *static_cast<::ProfileTrenchAnalyserParameters*>(_native);
    }



    ProfileTrenchAnalyserResult::ProfileTrenchAnalyserResult(::ProfileTrenchAnalyserResult* nativeResult)
    {
        _native = new ::ProfileTrenchAnalyserResult(*nativeResult);
    }

    ProfileTrenchAnalyserResult::ProfileTrenchAnalyserResult(Status status, double depth, double width)
    {
        _native = new ::ProfileTrenchAnalyserResult();
        _native->status = static_cast<::ProfileTrenchAnalyserResult::Status>(status);
        static_cast<::ProfileTrenchAnalyserResult*>(_native)->depth = depth;
        static_cast<::ProfileTrenchAnalyserResult*>(_native)->width = width;
    }

    ProfileTrenchAnalyserResult::~ProfileTrenchAnalyserResult()
    {
        this->!ProfileTrenchAnalyserResult();
    }

    ProfileTrenchAnalyserResult::!ProfileTrenchAnalyserResult()
    {
        delete _native;
        _native = nullptr;
    }

    double ProfileTrenchAnalyserResult::GetDepth()
    {
        return static_cast<::ProfileTrenchAnalyserResult*>(_native)->depth;
    }

    double ProfileTrenchAnalyserResult::GetWidth()
    {
        return static_cast<::ProfileTrenchAnalyserResult*>(_native)->width;
    }



    ProfileTrenchAnalyser::ProfileTrenchAnalyser(ProfileTrenchAnalyserParameters^ parameters)
    {
        _native = new ::ProfileTrenchAnalyser(parameters->GetNative());
    }

    ProfileTrenchAnalyser::~ProfileTrenchAnalyser()
    {
        this->!ProfileTrenchAnalyser();
    }

    ProfileTrenchAnalyser::!ProfileTrenchAnalyser()
    {
        delete _native;
        _native = nullptr;
    }

    ProfileAnalyserResult^ ProfileTrenchAnalyser::Process(Profile2d^ scan)
    {
        auto result = (*_native)(scan->GetNative());
        auto trenchResult = static_cast<::ProfileTrenchAnalyserResult*>(result.get());
        return gcnew ProfileTrenchAnalyserResult(trenchResult);
    }
}
