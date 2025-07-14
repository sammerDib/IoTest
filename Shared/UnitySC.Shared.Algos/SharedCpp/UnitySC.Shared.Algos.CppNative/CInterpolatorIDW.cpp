#include "CInterpolatorIDW.h"

#pragma unmanaged
#include <thread>
#include <cmath>

CInterpolatorIDW::CInterpolatorIDW()
{
    _Pow = 3;
    _inputs.reserve(_reserved);
    _values.reserve(_reserved);
    _isComputed = false;
}

CInterpolatorIDW::CInterpolatorIDW(const CInterpolatorIDW& interp)
{
    _Pow = interp._Pow;
    _reserved = interp._reserved;
    _inputs = interp._inputs;
    _values = interp._values;
    _isComputed = interp._isComputed;
    if (_isComputed)
    {
        _isComputed = ComputeData();
    }
}

CInterpolatorIDW::~CInterpolatorIDW()
{
    ResetInputsPoints();
}

bool CInterpolatorIDW::InitSettings(int Nb, double* settingList)
{
    ResetInputsPoints();

    if ((Nb != 1) || settingList == nullptr)
        return false;

    if (settingList[0] < 1.0)
        return false;

    _Pow = settingList[0];

    return true;
}

bool CInterpolatorIDW::SetInputsPoints(size_t NbPts, double* coordX, double* coordY, double* value)
{
    if ((NbPts <= 0) || coordX == nullptr || coordY == nullptr || value == nullptr)
        return false;

    bool success = true;
    try
    {
        double* ptrX = coordX;
        double* ptrY = coordY;
        double* ptrVal = value;
        for (size_t i = 0; i < NbPts - 1; i++)
        {
            _inputs.push_back(point2d{ *ptrX, *ptrY });
            _values.push_back(*ptrVal);
            ++ptrX; ++ptrY; ++ptrVal;
        }
        _inputs.push_back(point2d{ *ptrX, *ptrY });
        _values.push_back(*ptrVal);
    }
    catch (...)
    {
        success = false;
    }
    return success;
}

bool CInterpolatorIDW::AddInputsPoint(double coordX, double coordY, double value)
{
    bool success = true;
    try
    {
        _inputs.push_back(point2d{ coordX,coordY });
        _values.push_back(value);
    }
    catch (...)
    {
        success = false;
    }
    return success;
}

void CInterpolatorIDW::ResetInputsPoints()
{
    _inputs.clear();
    _values.clear();
    _isComputed = false;
}

bool CInterpolatorIDW::ComputeData()
{
    _isComputed = false;
    if (_inputs.size() == 0 || _values.size() == 0)
        return false;

    try
    {
        _isComputed = true;  
    }
    catch (...)
    {
        _isComputed = false;
    }
    return _isComputed;
}

static double Weigthed(const point2d& a, const point2d& b, double power)
{
    double xDist = a[0] - b[0];
    double yDist = a[1] - b[1];
    if (xDist == 0.0 && yDist == 0.0)
    {
        return 1.0;
    }
    return 1.0 / (pow(sqrt(xDist * xDist + yDist * yDist), power));
}

static double InterpolatePt(const point2d& Pti, const std::vector<point2d>& inputs, const std::vector<double>& values, double& pow)
{
    int i = 0;
    auto vt = values.begin();
    double sumWi = 0.0;
    double sumWiVi = 0.0;
    for (auto xyt = inputs.begin(); xyt != inputs.end(); xyt++, vt++, i++)
    {
        double wi = Weigthed(Pti, *xyt, pow);
        if (wi == 1.0)
        {
            return *vt;
        }
        sumWi += wi;
        sumWiVi += wi * *vt;
    }
    if (sumWi == 0.0)
    {
        return std::nan("1");
    }
    return sumWiVi / sumWi;
}


double CInterpolatorIDW::Interpolate(double x, double y)
{
    if (_isComputed)
    {
        point2d Pti = { x,y };
        return InterpolatePt(Pti, _inputs, _values, _Pow);
    }
    return std::nan("1");

}

bool CInterpolatorIDW::InterpolateGrid(double* pGridArray, int gridWidth, int gridHeiht, double xStart, double xStep, double yStart, double yStep)
{
    if (!_isComputed)
        return false;

    bool success = true;
    try
    {  
         int processor_count = (int)std::thread::hardware_concurrency();
         if (processor_count > 1)
             processor_count--;
         int chunk = ((gridHeiht / processor_count) <= 1) ? 1 : std::min( (int)(gridHeiht / processor_count) , 16);

         double pow = _Pow;
         std::vector<point2d> inputs = _inputs;
         std::vector<double> values = _values;

#pragma omp parallel for schedule(static, chunk), firstprivate(pGridArray, gridWidth, gridHeiht, xStart, yStart, xStep, yStep, inputs, values, pow), num_threads(processor_count)
        for (int y = 0; y < gridHeiht; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                pGridArray[x + y * gridWidth] = InterpolatePt(point2d{ (double)x * xStep + xStart, (double)y * yStep + yStart } , inputs, values, pow);
            }
        }
    }
    catch (...)
    {
        success = false;
    }
    return success;
}