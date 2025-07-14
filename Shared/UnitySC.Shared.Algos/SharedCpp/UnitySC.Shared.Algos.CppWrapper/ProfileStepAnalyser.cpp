#include "pch.h"
#include "ProfileStepAnalyser.h"

#include "../Include/ProfileStepAnalyser.h"

#pragma managed

namespace UnitySCSharedAlgosCppWrapper {
    ProfileStepAnalyserParameters::ProfileStepAnalyserParameters(KindStep kindStep, double targetHeight, double tolerance)
    {
        _native = new ::ProfileStepAnalyserParameters(
            kindStep == KindStep::Up ? ::KindStep::UP : ::KindStep::DOWN,
            targetHeight,
            tolerance
        );
    }

    ProfileStepAnalyserParameters::~ProfileStepAnalyserParameters()
    {
        this->!ProfileStepAnalyserParameters();
    }

    ProfileStepAnalyserParameters::!ProfileStepAnalyserParameters()
    {
        delete _native;
        _native = nullptr;
    }

    void ProfileStepAnalyserParameters::AddStepExclusionZone(ExclusionZone^ exclusionZone)
    {
        static_cast<::ProfileStepAnalyserParameters*>(_native)->stepZone = exclusionZone->GetNative();
    }

    const ::ProfileStepAnalyserParameters& ProfileStepAnalyserParameters::GetNative()
    {
        return *static_cast<::ProfileStepAnalyserParameters*>(_native);
    }



    ProfileStepAnalyserResult::ProfileStepAnalyserResult(::ProfileStepAnalyserResult* nativeResult)
    {
        _native = new ::ProfileStepAnalyserResult(*nativeResult);
    }

    ProfileStepAnalyserResult::ProfileStepAnalyserResult(Status status, double stepX, double stepHeight)
    {
        _native = new ::ProfileStepAnalyserResult();
        _native->status = static_cast<::ProfileStepAnalyserResult::Status>(status);
        static_cast<::ProfileStepAnalyserResult*>(_native)->stepX = stepX;
        static_cast<::ProfileStepAnalyserResult*>(_native)->stepHeight = stepHeight;
    }

    ProfileStepAnalyserResult::~ProfileStepAnalyserResult()
    {
        this->!ProfileStepAnalyserResult();
    }

    ProfileStepAnalyserResult::!ProfileStepAnalyserResult()
    {
        delete _native;
        _native = nullptr;
    }

    double ProfileStepAnalyserResult::GetStepX()
    {
        return static_cast<::ProfileStepAnalyserResult*>(_native)->stepX;
    }

    double ProfileStepAnalyserResult::GetStepHeight()
    {
        return static_cast<::ProfileStepAnalyserResult*>(_native)->stepHeight;
    }



    ProfileStepAnalyser::ProfileStepAnalyser(ProfileStepAnalyserParameters^ parameters)
    {
        _native = new ::ProfileStepAnalyser(parameters->GetNative());
    }

    ProfileStepAnalyser::~ProfileStepAnalyser()
    {
        this->!ProfileStepAnalyser();
    }

    ProfileStepAnalyser::!ProfileStepAnalyser()
    {
        delete _native;
        _native = nullptr;
    }

    ProfileAnalyserResult^ ProfileStepAnalyser::Process(Profile2d^ scan)
    {
        auto result = (*_native)(scan->GetNative());
        auto stepResult = static_cast<::ProfileStepAnalyserResult*>(result.get());
        return gcnew ProfileStepAnalyserResult(stepResult);
    }
}
