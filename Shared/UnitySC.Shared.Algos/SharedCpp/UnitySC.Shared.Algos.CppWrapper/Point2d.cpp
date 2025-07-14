#include "pch.h"
#include "Point2d.h"

#pragma managed

namespace UnitySCSharedAlgosCppWrapper {
    Point2d::Point2d(geometry::Point2d& nativePoint)
    {
        _native = &nativePoint;
        _handlesMemory = false;
    }

    Point2d::Point2d(double x, double y)
    {
        _native = new geometry::Point2d();
        _native->X = x;
        _native->Y = y;
        _handlesMemory = true;
    }

    Point2d::~Point2d()
    {
        this->!Point2d();
    }

    Point2d::!Point2d()
    {
        if (_handlesMemory)
        {
            delete _native;
        }
        _native = nullptr;
    }
}
