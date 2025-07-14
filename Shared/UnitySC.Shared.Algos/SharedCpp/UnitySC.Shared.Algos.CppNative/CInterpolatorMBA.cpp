#include "CInterpolatorMBA.h"


#pragma unmanaged
#include "mba.hpp"
#include <thread>

CInterpolatorMBA::CInterpolatorMBA()
{
    _loRange = { 0,0 };
    _hiRange = { 0,0 };
    _gridSize = { 0,0 };
    _initialValue = std::numeric_limits<double>::quiet_NaN();
    _inputs.reserve(_reserved);
    _values.reserve(_reserved);
    _isComputed = false;
    _pMBAInst_ptr = nullptr;
}

CInterpolatorMBA::CInterpolatorMBA(const CInterpolatorMBA& interp)
{
    _loRange = interp._loRange;
    _hiRange = interp._hiRange;
    _gridSize = interp._gridSize;
    _initialValue = interp._initialValue;
    _reserved = interp._reserved;
    _inputs = interp._inputs;
    _values = interp._values;
    _isComputed = interp._isComputed;

    _pMBAInst_ptr = nullptr;
    if (_isComputed)
    {
        _isComputed = ComputeData();
    }
}

CInterpolatorMBA::~CInterpolatorMBA()
{
    ClearNativeInstance();
    ResetInputsPoints();
}


void CInterpolatorMBA::ClearNativeInstance()
{
    if (_pMBAInst_ptr != nullptr)
    {
        _pMBAInst_ptr = nullptr;
        _isComputed = false;
    }
}

bool CInterpolatorMBA::InitSettings(int Nb, double* settingList)
{
    ClearNativeInstance();
    ResetInputsPoints();

    if ((Nb != 6 && Nb != 7) || settingList == nullptr)
        return false;

    _loRange = { settingList[0], settingList[1] };
    _hiRange = { settingList[2], settingList[3] };
    _gridSize = { (size_t)settingList[4], (size_t)settingList[5] };

    if (Nb == 7)
        _initialValue = settingList[6];
    else
        _initialValue = std::numeric_limits<double>::quiet_NaN();

    return true;
}

bool CInterpolatorMBA::SetInputsPoints(size_t NbPts, double* coordX, double* coordY, double* value)
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

bool CInterpolatorMBA::AddInputsPoint(double coordX, double coordY, double value)
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

void CInterpolatorMBA::ResetInputsPoints()
{
    _inputs.clear();
    _values.clear();
}

bool CInterpolatorMBA::ComputeData()
{
    ClearNativeInstance();
    if (_inputs.size() == 0 || _values.size() == 0)
        return false;

    try
    {
               
        std::function<double(point2d)> initial;
        if(! std::isnan(_initialValue))
            initial = [*this](point2d) { return _initialValue; };

        _pMBAInst_ptr = std::make_unique<mba::MBA<2>>(_loRange, _hiRange, _gridSize, _inputs, _values, 32, 1e-8, 0.5, initial);
        _isComputed = true;
    }
    catch (...)
    {
        _isComputed = false;
    }
    return _isComputed;
}

double CInterpolatorMBA::Interpolate(double x, double y)
{
    if(_isComputed && _pMBAInst_ptr != nullptr)
        return _pMBAInst_ptr->operator()(mba::point<2>{x, y});
    return std::nan("1");

}

bool CInterpolatorMBA::InterpolateGrid(double* pGridArray, int gridWidth, int gridHeiht, double xStart, double xStep, double yStart, double yStep)
{
    if (!_isComputed || _pMBAInst_ptr == nullptr)
        return false;

    bool success = true;
    try
    {
         mba::MBA<2> interp = *(_pMBAInst_ptr);

         int processor_count = (int)std::thread::hardware_concurrency();
         if (processor_count > 1)
             processor_count--;
         int chunk = ((gridHeiht / processor_count) <= 1) ? 1 : std::min( (int)(gridHeiht / processor_count) , 16);

#pragma omp parallel for schedule(static, chunk), firstprivate(interp, pGridArray, gridWidth, gridHeiht, xStart, yStart, xStep, yStep), num_threads(processor_count)
        for (int y = 0; y < gridHeiht; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                pGridArray[x + y * gridWidth] = interp(mba::point<2>{(double)x* xStep + xStart, (double)y* yStep + yStart});
            }
        }
    }
    catch (...)
    {
        success = false;
    }
    return success;
}

