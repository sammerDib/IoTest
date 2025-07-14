#include "CInterpolatorFNN.h"

#pragma unmanaged
#include <thread>
#include <cmath>
#include "kdtree.hpp"

CInterpolatorFNN::CInterpolatorFNN()
{
    _inputs.reserve(_reserved);
    _values1.reserve(_reserved);
    _values2.reserve(_reserved);
    _useV1 = false;
    _isComputed = false;
    _pKdTreeInst_ptr = nullptr;
}

CInterpolatorFNN::CInterpolatorFNN(const CInterpolatorFNN& interp)
{
    _reserved = interp._reserved;
    _inputs = interp._inputs;
    _values1 = interp._values1;
    _values2 = interp._values2;
    _useV1 = false;
    _isComputed = interp._isComputed;

    _pKdTreeInst_ptr = nullptr;
    if (_isComputed)
    {
        _isComputed = ComputeData();
    }
}

CInterpolatorFNN::~CInterpolatorFNN()
{
    ClearNativeInstance();
    ResetInputsPoints();
}

void CInterpolatorFNN::ClearNativeInstance()
{
    if (_pKdTreeInst_ptr != nullptr)
    {
        delete ((Kdtree::KdTree*)_pKdTreeInst_ptr);
        _pKdTreeInst_ptr = nullptr;
    }
    _isComputed = false;
}


bool CInterpolatorFNN::InitSettings(int Nb, double* settingList)
{
    ClearNativeInstance();
    ResetInputsPoints();

    // No settings for fNN

    return true;
}

bool CInterpolatorFNN::SetInputsPoints(size_t NbPts, double* coordX, double* coordY, double* value)
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
            _inputs.push_back(input2d{ *ptrX, *ptrY });
            _values1.push_back(*ptrVal);
            ++ptrX; ++ptrY; ++ptrVal;
        }
        _inputs.push_back(input2d{ *ptrX, *ptrY });
        _values1.push_back(*ptrVal);
    }
    catch (...)
    {
        success = false;
    }
    return success;
}

bool CInterpolatorFNN::SetInputsPoints(size_t NbPts, double* coordX, double* coordY, double* value1, double* value2)
{
    if ((NbPts <= 0) || coordX == nullptr || coordY == nullptr || value1 == nullptr || value2 == nullptr)
        return false;

    bool success = true;
    try
    {
        double* ptrX = coordX;
        double* ptrY = coordY;
        double* ptrVal1 = value1;
        double* ptrVal2 = value2;
        for (size_t i = 0; i < NbPts - 1; i++)
        {
            _inputs.push_back(input2d{ *ptrX, *ptrY });
            _values2.push_back(value2d{ *ptrVal1, *ptrVal2 });
            ++ptrX; ++ptrY; ++ptrVal1;  ++ptrVal2;
        }
        _inputs.push_back(input2d{ *ptrX, *ptrY });
        _values2.push_back(value2d{ *ptrVal1, *ptrVal2 });
    }
    catch (...)
    {
        success = false;
    }
    return success;
}

bool CInterpolatorFNN::AddInputsPoint(double coordX, double coordY, double value)
{
    bool success = true;
    try
    {
        _inputs.push_back(input2d{ coordX,coordY });
        _values1.push_back(value);
    }
    catch (...)
    {
        success = false;
    }
    return success;
}

void CInterpolatorFNN::ResetInputsPoints()
{
    _inputs.clear();
    _values1.clear();
    _values2.clear();
    _useV1 = false;
}

bool CInterpolatorFNN::ComputeData()
{
    ClearNativeInstance();
    _isComputed = false;

    bool use1d = _values1.size() != 0;
    bool use2d = _values2.size() != 0;
    if (use2d)
    {
        use1d = false;
        if (_inputs.size() == 0 || _values2.size() == 0)
            return false;
    }
    else if (_inputs.size() == 0 || _values1.size() == 0)
        return false;

    _useV1 = !use2d;
    try
    {
        Kdtree::KdNodeVector nodes;
        for (int i = 0; i < _inputs.size() ; ++i) {
            nodes.push_back(Kdtree::KdNode(_inputs[i], (_useV1 ? (void*)&(_values1[i]) : (void*)&(_values2[i])), i));
        }
        _pKdTreeInst_ptr = (void*)new Kdtree::KdTree(&nodes);
        _isComputed = true;  
    }
    catch (...)
    {
        _isComputed = false;
    }
    return _isComputed;
}

double CInterpolatorFNN::Interpolate(double x, double y)
{
    if (_isComputed)
    {
        input2d Pti = { x,y };
        Kdtree::KdNodeVector result;
        static_cast<Kdtree::KdTree*>(_pKdTreeInst_ptr)->k_nearest_neighbors(Pti, 1, &result);
        if (result.size() > 0)
        {
            if (_useV1)
                return *(static_cast<double*>(result[0].data));
            else
                return (*(static_cast<value2d*>(result[0].data)))[0];
        }
    }
    return std::nan("1");
}

void  CInterpolatorFNN::Interpolate(double x, double y, double& out1, double& out2)
{
    out1 = std::nan("1");
    out2 = std::nan("1");

    if (_isComputed)
    {
        input2d Pti = { x,y };
        Kdtree::KdNodeVector result;
        static_cast<Kdtree::KdTree*>(_pKdTreeInst_ptr)->k_nearest_neighbors(Pti, 1, &result);
        if (result.size() > 0)
        {
            if (_useV1)
            {
                out1 = *(static_cast<double*>(result[0].data));
                out2 = std::nan("1");
            }
            else
            {
                value2d values2 = *(static_cast<value2d*>(result[0].data));
                out1 = values2[0];
                out2 = values2[1];
            }
        }
    }
}

bool CInterpolatorFNN::InterpolateGrid(double* pGridArray, int gridWidth, int gridHeiht, double xStart, double xStep, double yStart, double yStep)
{
    if (!_isComputed || _pKdTreeInst_ptr == nullptr)
        return false;

    bool success = true;
    try
    {  
        
         int processor_count = (int)std::thread::hardware_concurrency();
         if (processor_count > 1)
             processor_count--;
         int chunk = ((gridHeiht / processor_count) <= 1) ? 1 : std::min( (int)(gridHeiht / processor_count) , 16);

// note de rti desactivation pour l'instant de l'openMP car le KDtree ne gere pas correctement les predicates en openMP (cas QuadNN)
#pragma omp parallel for schedule(static, chunk), firstprivate(pGridArray, gridWidth, gridHeiht, xStart, yStart, xStep, yStep), num_threads(processor_count)
        for (int y = 0; y < gridHeiht; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                pGridArray[x + y * gridWidth] = Interpolate((double)x * xStep + xStart, (double)y * yStep + yStart);
            }
        }
    }
    catch (...)
    {
        success = false;
    }
    return success;
}

bool CInterpolatorFNN::InterpolateGrid(double* pGridArray1, double* pGridArray2, int gridWidth, int gridHeiht, double xStart, double xStep, double yStart, double yStep)
{
    if (!_isComputed || _pKdTreeInst_ptr == nullptr)
        return false;

    bool success = true;
    try
    {
        int processor_count = (int)std::thread::hardware_concurrency();
        if (processor_count > 1)
            processor_count--;
        int chunk = ((gridHeiht / processor_count) <= 1) ? 1 : std::min((int)(gridHeiht / processor_count), 16);

// note de rti desactivation pour l'instant de l'openMP car le KDtree ne gere pas correctement les predicates en openMP (cas QuadNN)
#pragma omp parallel for schedule(static, chunk), firstprivate(pGridArray1, pGridArray2, gridWidth, gridHeiht, xStart, yStart, xStep, yStep), num_threads(processor_count)
        for (int y = 0; y < gridHeiht; y++)
        {
            double out1 = 0.0; double out2 = 0.0;
            for (int x = 0; x < gridWidth; x++)
            {
                Interpolate((double)x * xStep + xStart, (double)y * yStep + yStart, out1, out2);
                pGridArray1[x + y * gridWidth] = out1;
                pGridArray2[x + y * gridWidth] = out2;
            }
        }
    }
    catch (...)
    {
        success = false;
    }
    return success;
}