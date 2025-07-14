#include "pch.h"
#include "ProfileEdgeTrimAnalyser.h"

#include "../Include/ProfileEdgeTrimAnalyser.h"

#pragma managed

namespace UnitySCSharedAlgosCppWrapper {
    ProfileEdgeTrimAnalyserParameters::ProfileEdgeTrimAnalyserParameters(KindStep kindStep, double targetHeight, double toleranceHeight, double targetWidth, double toleranceWidth)
        : ProfileStepAnalyserParameters(kindStep, targetHeight, toleranceHeight)
    {
        _native = new ::ProfileEdgeTrimAnalyserParameters(
            kindStep == KindStep::Up ? ::KindStep::UP : ::KindStep::DOWN,
            targetHeight,
            toleranceHeight,
            targetWidth,
            toleranceWidth
        );
    }

    ProfileEdgeTrimAnalyserParameters::~ProfileEdgeTrimAnalyserParameters()
    {
        this->!ProfileEdgeTrimAnalyserParameters();
    }

    ProfileEdgeTrimAnalyserParameters::!ProfileEdgeTrimAnalyserParameters()
    {
        delete _native;
        _native = nullptr;
    }

    const ::ProfileEdgeTrimAnalyserParameters& ProfileEdgeTrimAnalyserParameters::GetNative()
    {
        return *static_cast<::ProfileEdgeTrimAnalyserParameters*>(_native);
    }



    ProfileEdgeTrimAnalyserResult::ProfileEdgeTrimAnalyserResult(::ProfileEdgeTrimAnalyserResult* nativeResult)
        : ProfileStepAnalyserResult(nativeResult)
    {
        delete _native;
        _native = new ::ProfileEdgeTrimAnalyserResult(*nativeResult);
    }

    ProfileEdgeTrimAnalyserResult::ProfileEdgeTrimAnalyserResult(Status status, double stepX, double height, double width)
        : ProfileStepAnalyserResult(status, stepX, height)
    {
        const auto previousNative = *static_cast<::ProfileEdgeTrimAnalyserResult*>(_native);
        delete _native;
        _native = new ::ProfileEdgeTrimAnalyserResult(previousNative);
        static_cast<::ProfileEdgeTrimAnalyserResult*>(_native)->width = width;
    }

    ProfileEdgeTrimAnalyserResult::~ProfileEdgeTrimAnalyserResult()
    {
        this->!ProfileEdgeTrimAnalyserResult();
    }

    ProfileEdgeTrimAnalyserResult::!ProfileEdgeTrimAnalyserResult()
    {
        delete _native;
        _native = nullptr;
    }

    double ProfileEdgeTrimAnalyserResult::GetWidth()
    {
        return dynamic_cast<::ProfileEdgeTrimAnalyserResult*>(_native)->width;
    }



    ProfileEdgeTrimAnalyser::ProfileEdgeTrimAnalyser(ProfileEdgeTrimAnalyserParameters^ parameters)
    {
        _native = new ::ProfileEdgeTrimAnalyser(parameters->GetNative());
    }

    ProfileEdgeTrimAnalyser::~ProfileEdgeTrimAnalyser()
    {
        this->!ProfileEdgeTrimAnalyser();
    }

    ProfileEdgeTrimAnalyser::!ProfileEdgeTrimAnalyser()
    {
        delete _native;
        _native = nullptr;
    }

    ProfileAnalyserResult^ ProfileEdgeTrimAnalyser::Process(Profile2d^ scan)
    {
        auto result = (*_native)(scan->GetNative());
        auto edgeTrimResult = static_cast<::ProfileEdgeTrimAnalyserResult*>(result.get());
        return gcnew ProfileEdgeTrimAnalyserResult(edgeTrimResult);
    }
}
