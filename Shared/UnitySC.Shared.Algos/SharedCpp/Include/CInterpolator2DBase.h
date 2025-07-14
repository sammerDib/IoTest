#pragma once

#pragma unmanaged
class CInterpolator2DBase
{
public: 
     virtual bool InitSettings(int Nb, double* settingList) = 0;
     virtual bool SetInputsPoints(size_t NbPts, double* coordX, double* coordY, double* value)= 0;
     virtual bool SetInputsPoints(size_t NbPts, double* coordX, double* coordY, double* value1, double* value2) = 0;
     virtual bool AddInputsPoint(double coordX, double coordY, double value) = 0;
     virtual void ResetInputsPoints() = 0;
     virtual bool ComputeData()= 0;

     virtual double Interpolate(double x, double y)= 0;
     virtual void Interpolate(double x, double y, double& out1, double& out2) = 0;

     virtual bool InterpolateGrid(double* pGridArray, int gridWidth, int gridHeiht, double xStart, double xStep, double yStart, double yStep)= 0;
     virtual bool InterpolateGrid(double* pGridArray1, double* pGridArray2, int gridWidth, int gridHeiht, double xStart, double xStep, double yStart, double yStep) = 0;

};

