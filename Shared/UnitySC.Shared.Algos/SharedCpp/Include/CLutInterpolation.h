#pragma once

#pragma unmanaged
class CLutInterpolation
{
public:
    CLutInterpolation();
    CLutInterpolation(const int size, const double* pX, const double* pY);
    CLutInterpolation(const CLutInterpolation& lut);
    ~CLutInterpolation();

    // Values bigger than the last point simply have to be cropped to the max value.
    const double Y(const double x);
    const double X(const double y);

    // Initialization using the fixed points to interpolate from.
    void CopyArraysFrom(const int size, const double* pX, const double* pY);

protected:
    void CleanArrays();

private:
    int _size;
    double* _pX = 0;
    double* _pY = 0;
};

