#include "pch.h"
#include "ExclusionZone.h"

void ExclusionZone::SetX(const double x)
{
    _x = x;
}

double ExclusionZone::GetX() const
{
    return _x;
}
double ExclusionZone::GetLeft() const
{
    return _left;
}
double ExclusionZone::GetRight() const
{
    return _right;
}
