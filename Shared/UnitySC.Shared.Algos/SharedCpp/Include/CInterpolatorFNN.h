#pragma once
#include "CInterpolator2DBase.h"

#pragma unmanaged
#include <array>
#include <vector>

using input2d = std::vector<double>;
using value2d = std::vector<double>;

class CInterpolatorFNN : public CInterpolator2DBase
{
public: 
    CInterpolatorFNN();
    CInterpolatorFNN(const CInterpolatorFNN& fnn);
    virtual ~CInterpolatorFNN();

     virtual bool InitSettings(int Nb, double* settingList);
     bool SetInputsPoints(size_t NbPts, double* coordX, double* coordY, double* value);
     bool SetInputsPoints(size_t NbPts, double* coordX, double* coordY, double* value1, double* value2);
     bool AddInputsPoint(double coordX, double coordY, double value);
     void ResetInputsPoints();
     bool ComputeData();

     double Interpolate(double x, double y);
     void Interpolate(double x, double y, double& out1, double& out2);

     bool InterpolateGrid(double* pGridArray, int gridWidth, int gridHeiht, double xStart, double xStep, double yStart, double yStep);
     bool InterpolateGrid(double* pGridArray1, double* pGridArray2, int gridWidth, int gridHeiht, double xStart, double xStep, double yStart, double yStep);

protected :
    void ClearNativeInstance();

protected:
    std::vector<input2d> _inputs;
    std::vector<double> _values1;
    std::vector<value2d> _values2;

    bool _isComputed;
    void* _pKdTreeInst_ptr;

    // internal perf
    bool _useV1 = false;
    size_t _reserved = 1024;
};

