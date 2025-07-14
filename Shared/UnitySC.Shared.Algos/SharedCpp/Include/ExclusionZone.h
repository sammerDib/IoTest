#pragma once
#include "Point2d.h"

#pragma unmanaged

/// <summary>
/// This class represents an exclusion zone, i.e. a range of values: [x-left, x+right].
/// It is use to exclude some part of a profile, such as in Metrology Profile 1D scans (cf Profile.h)
/// </summary>
class ExclusionZone
{
public:
    constexpr ExclusionZone(const double x, const double left, const double right) :
        _x(x), _left(left), _right(right)
    {}

    constexpr ExclusionZone(const double left, const double right) :
        _left(left), _right(right)
    {}

    void SetX(const double x);

    double GetX() const;
    double GetLeft() const;
    double GetRight() const;

private:
    double _x{ 0.0 };
    double _left{ 0.0 };
    double _right{ 0.0 };
};
