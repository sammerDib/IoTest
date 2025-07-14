#include "pch.h"
#include "CLutInterpolation.h"
#include "InterpolationLutTemplates.h"

CLutInterpolation::CLutInterpolation()
{
    _size = 0;
    _pX = nullptr;
    _pY = nullptr;
}

CLutInterpolation::CLutInterpolation(const int size, const double* pX, const double* pY)
{
    CopyArraysFrom(size, pX, pY);
}

CLutInterpolation::CLutInterpolation(const CLutInterpolation& lut)
    :CLutInterpolation(lut._size, lut._pX, lut._pY)
{
   
}

CLutInterpolation::~CLutInterpolation()
{
    CleanArrays();
}

void CLutInterpolation::CleanArrays()
{
    if (_pX != nullptr)
    {
        delete[] _pX;
        _pX = nullptr;
    }

    if (_pY != nullptr)
    {
        delete[] _pY;
        _pY = nullptr;
    }
    _size = 0;
}

const double CLutInterpolation::Y(const double x)
{
    return InterpolArray(_size, _pX, _pY, x);
}

const double CLutInterpolation::X(const double y)
{
    return InterpolArray(_size, _pY, _pX, y);
}

// Initialization using the fixed points to interpolate from.
void CLutInterpolation::CopyArraysFrom(const int size, const double* pX, const double* pY)
{
    CleanArrays();

    _size = size;
    _pX = new double[size];
    _pY = new double[size];

    int size_bytes = size * sizeof(double);
    memcpy_s(_pX, size_bytes, pX, size_bytes);
    memcpy_s(_pY, size_bytes, pY, size_bytes);
}

