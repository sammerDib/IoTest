#include "pch.h"
#include "Profile2d.h"

#pragma managed

namespace UnitySCSharedAlgosCppWrapper {
    Profile2d::Profile2d()
    {
        _native = new Profile();
    }

    Profile2d::~Profile2d()
    {
        this->!Profile2d();
    }

    Profile2d::!Profile2d()
    {
        delete _native;
        _native = nullptr;
    }

    void Profile2d::Add(Point2d^ point)
    {
        _native->PushBack({ point->X, point->Y });
    }

    const Profile& Profile2d::GetNative()
    {
        return *_native;
    }
}
