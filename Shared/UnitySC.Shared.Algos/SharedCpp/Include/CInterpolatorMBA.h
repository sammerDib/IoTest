#pragma once
#include "CInterpolator2DBase.h"

#pragma unmanaged
#include <array>
#include <vector>
#include <memory>
namespace mba
{
    template <unsigned NDim>
    class MBA;
}
using point2d = std::array<double, 2>;
using index2d = std::array<size_t, 2>;

class CInterpolatorMBA : public CInterpolator2DBase
{
public: 
    CInterpolatorMBA();
    CInterpolatorMBA(const CInterpolatorMBA& mba);
    ~CInterpolatorMBA();

     bool InitSettings(int Nb, double* settingList);
     bool SetInputsPoints(size_t NbPts, double* coordX, double* coordY, double* value);
     bool SetInputsPoints(size_t NbPts, double* coordX, double* coordY, double* value1, double* value2) { throw std::exception("Bad call MBA does not support 2-values."); } ;
     bool AddInputsPoint(double coordX, double coordY, double value);
     void ResetInputsPoints();
     bool ComputeData();

     double Interpolate(double x, double y);
     void Interpolate(double x, double y, double& out1, double& out2) { throw std::exception("Bad call MBA does not support 2-values."); };

     bool InterpolateGrid(double* pGridArray, int gridWidth, int gridHeiht, double xStart, double xStep, double yStart, double yStep);
     bool InterpolateGrid(double* pGridArray1, double* pGridArray2, int gridWidth, int gridHeiht, double xStart, double xStep, double yStart, double yStep) { throw std::exception("Bad grid call MBA does not support 2-values."); };



private :
    void ClearNativeInstance();

private:
    point2d _loRange;
    point2d _hiRange;
    index2d _gridSize;
    double _initialValue;
    std::vector<point2d> _inputs;
    std::vector<double> _values;

    bool _isComputed;
    std::unique_ptr<mba::MBA<2>> _pMBAInst_ptr;

    // internal perf
    size_t _reserved = 1024;
};

