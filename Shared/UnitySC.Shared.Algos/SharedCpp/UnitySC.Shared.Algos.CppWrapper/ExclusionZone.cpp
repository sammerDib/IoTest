#include "pch.h"
#include "ExclusionZone.h"

#include "../Include/ExclusionZone.h"

#pragma managed

namespace UnitySCSharedAlgosCppWrapper {
    ExclusionZone::ExclusionZone(const double x, const double left, const double right)
    {
        _native = new ::ExclusionZone(x, left, right);
    }

    ExclusionZone::ExclusionZone(const double left, const double right)
    {
        _native = new ::ExclusionZone(left, right);
    }

    ExclusionZone::~ExclusionZone()
    {
        this->!ExclusionZone();
    }

    ExclusionZone::!ExclusionZone()
    {
        delete _native;
        _native = nullptr;
    }

    const ::ExclusionZone& ExclusionZone::GetNative()
    {
        return *_native;
    }
}