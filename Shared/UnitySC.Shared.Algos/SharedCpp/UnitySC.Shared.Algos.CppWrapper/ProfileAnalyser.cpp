#include "pch.h"
#include "ProfileAnalyser.h"

#include "../Include/ProfileAnalyser.h"

#pragma managed

namespace UnitySCSharedAlgosCppWrapper
{
    ProfileAnalyserParameters::~ProfileAnalyserParameters()
    {
        this->!ProfileAnalyserParameters();
    }

    ProfileAnalyserParameters::!ProfileAnalyserParameters()
    {
        delete _native;
        _native = nullptr;
    }

    void ProfileAnalyserParameters::AddStartExclusionZone(ExclusionZone^ exclusionZone)
    {
        _native->startZone = exclusionZone->GetNative();
    }

    void ProfileAnalyserParameters::AddEndExclusionZone(ExclusionZone^ exclusionZone)
    {
        _native->endZone = exclusionZone->GetNative();
    }

    void ProfileAnalyserParameters::SetNbStdDevFiltering(const double nbStdDev)
    {
        _native->nbStdDevFiltering = nbStdDev;
    }

    void ProfileAnalyserParameters::SetMaxConvolutionKernelHalfSize(const int size)
    {
        _native->maxConvolutionKernelHalfSize = size;
    }



    ProfileAnalyserResult::~ProfileAnalyserResult()
    {
        this->!ProfileAnalyserResult();
    }

    ProfileAnalyserResult::!ProfileAnalyserResult()
    {
        delete _native;
        _native = nullptr;
    }

    ProfileAnalyserResult::Status ProfileAnalyserResult::GetStatus()
    {
        const auto nativeStatus = _native->status;
        switch (nativeStatus)
        {
        case ::ProfileAnalyserResult::Status::OK:
            return Status::Ok;
        case ::ProfileAnalyserResult::Status::EMPTY_PROFILE:
            return Status::EmptyProfile;
        case ::ProfileAnalyserResult::Status::EMPTY_PROFILE_NAN:
            return Status::EmptyProfileNan;
        case ::ProfileAnalyserResult::Status::PROFILE_TOO_SMALL_AFTER_STD_DEV_FILTERING:
            return Status::ProfileTooSmallAfterStdDevFiltering;
        }
    }
}
