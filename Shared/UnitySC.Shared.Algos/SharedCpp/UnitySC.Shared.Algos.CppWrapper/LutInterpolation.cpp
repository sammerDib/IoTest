#include "pch.h"
#include "LutInterpolation.h"

#include "CLutInterpolation.h"

#pragma managed
namespace UnitySCSharedAlgosCppWrapper {
    LutInterpolation::LutInterpolation()
    {
        _native = new CLutInterpolation();
    }

    LutInterpolation::LutInterpolation(CLutInterpolation* native)
    {
        _native = native;
    }

    LutInterpolation::LutInterpolation(const LutInterpolation^& copier)
    {
        _native = new CLutInterpolation(*(copier->_native));
    }

    const double LutInterpolation::Y(const double x)
    {
        return _native->Y(x);
    }

    const double LutInterpolation::X(const double y)
    {
        return _native->X(y);
    }

    void LutInterpolation::CopyArraysFrom(const int size, const unsigned long long pX, const unsigned long long pY)
    {
        _native->CopyArraysFrom(size, (double*)pX, (double*)pY);
    }

    LutInterpolation::~LutInterpolation()
    {
        this->!LutInterpolation();
    }

    LutInterpolation::!LutInterpolation()
    {
        if (_native != nullptr)
        {
            delete ((CLutInterpolation*)_native);
            _native = nullptr;
        }
    }
}