#pragma once

#include "../Include/Point2d.h"

#pragma managed

namespace UnitySCSharedAlgosCppWrapper {
    public ref struct Point2d {
        Point2d(geometry::Point2d& nativePoint);
        Point2d(double x, double y);
        ~Point2d();
        !Point2d();

        property double X
        {
            double get() { return _native->X; }
            void set(double x) { _native->X = x; }
        }

        property double Y
        {
            double get() { return _native->Y; }
            void set(double y) { _native->Y = y; }
        }

    private:
        geometry::Point2d* _native{ nullptr };
        bool _handlesMemory{ false };
    };
}