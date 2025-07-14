#include "pch.h"
#include "Interpolator2D.h"

#include "CInterpolatorMBA.h"
#include "CInterpolatorIDW.h"
#include "CInterpolatorFNN.h"
#include "CInterpolatorQuadNN.h"

#pragma managed
namespace UnitySCSharedAlgosCppWrapper {

    Interpolator2D::Interpolator2D(InterpolateAlgoType Algotype)
    {
        _algotype = Algotype;
        switch (_algotype)
        {
        case UnitySCSharedAlgosCppWrapper::InterpolateAlgoType::MBA:
            _native = new CInterpolatorMBA();
            break;
        case UnitySCSharedAlgosCppWrapper::InterpolateAlgoType::IDW:
            _native = new CInterpolatorIDW();
            break;
        case UnitySCSharedAlgosCppWrapper::InterpolateAlgoType::fNN:
            _native = new CInterpolatorFNN();
            break;
        case UnitySCSharedAlgosCppWrapper::InterpolateAlgoType::QuadNN:
            _native = new CInterpolatorQuadNN();
            break;
        case UnitySCSharedAlgosCppWrapper::InterpolateAlgoType::Unknown:
        default:
            throw gcnew System::Exception(gcnew System::String("Unkown Interpolator Type"));
            break;
        }
    }

    Interpolator2D::Interpolator2D(InterpolateAlgoType Algotype, CInterpolator2DBase* native)
    {
        _algotype = Algotype;
        _native = native;
    }

    Interpolator2D::Interpolator2D(const Interpolator2D^& copier)
    {
        if(copier->_native != nullptr)
           _algotype = (copier->_algotype);

        switch (_algotype)
        {
        case UnitySCSharedAlgosCppWrapper::InterpolateAlgoType::MBA:
           _native = new CInterpolatorMBA(*((CInterpolatorMBA*)copier->_native));
            break;
        case UnitySCSharedAlgosCppWrapper::InterpolateAlgoType::IDW:
            _native = new CInterpolatorIDW(*((CInterpolatorIDW*)copier->_native));
            break;
        case UnitySCSharedAlgosCppWrapper::InterpolateAlgoType::fNN:
            _native = new CInterpolatorFNN(*((CInterpolatorFNN*)copier->_native));
            break;
        case UnitySCSharedAlgosCppWrapper::InterpolateAlgoType::QuadNN:
            _native = new CInterpolatorQuadNN(*((CInterpolatorQuadNN*)copier->_native));
            break;
        case UnitySCSharedAlgosCppWrapper::InterpolateAlgoType::Unknown:
        default:
            throw gcnew System::Exception(gcnew System::String("Empty or Unkown Interpolator Type"));
            break;
        }
    }

    Interpolator2D::~Interpolator2D()
    {
        this->!Interpolator2D();
    }

    Interpolator2D::!Interpolator2D()
    {
        if (_native != nullptr)
        {
            delete (_native);
            _native = nullptr;
            _algotype = InterpolateAlgoType::Unknown;
        }
    }
    bool Interpolator2D::InitSettings(array<double>^ settingList)
    {
        if (settingList == nullptr)
            return false;

        pin_ptr<double> pSettings = &settingList[0];
        return _native->InitSettings(settingList->Length, pSettings);
    }

    bool Interpolator2D::SetInputsPoints(array<double>^ coordsX, array<double>^ coordsY, array<double>^ values)
    {
        if(coordsX == nullptr || coordsY == nullptr || values == nullptr)
            return false;

        int size = coordsX->Length;
        if(coordsY->Length < size || values->Length < size)
            return false;

        pin_ptr<double> pX = &coordsX[0];
        pin_ptr<double> pY = &coordsY[0];
        pin_ptr<double> pVal = &values[0];
         return _native->SetInputsPoints(size, pX, pY, pVal);
    }

    bool Interpolator2D::SetInputsPoints(array<double>^ coordsX, array<double>^ coordsY, array<double>^ values1, array<double>^ values2)
    {
        if (coordsX == nullptr || coordsY == nullptr || values1 == nullptr || values2 == nullptr)
            return false;

        int size = coordsX->Length;
        if (coordsY->Length < size || values1->Length < size || values2->Length < size)
            return false;

        pin_ptr<double> pX = &coordsX[0];
        pin_ptr<double> pY = &coordsY[0];
        pin_ptr<double> pVal1 = &values1[0];
        pin_ptr<double> pVal2 = &values2[0];
        return _native->SetInputsPoints(size, pX, pY, pVal1, pVal2);
    }

    bool Interpolator2D::AddInputsPoint(double coordX, double coordY, double value)
    {
         return _native->AddInputsPoint(coordX, coordY, value);
    }
    void Interpolator2D::ResetInputsPoints()
    {
        return _native->ResetInputsPoints();
    }
    
    bool Interpolator2D::ComputeData()
    {
         return _native->ComputeData();
    }

    double Interpolator2D::Interpolate(double x, double y)
    {
        return _native->Interpolate(x,y);
    }

    void Interpolator2D::Interpolate(double x, double y, double % out1, double % out2)
    {
        double nativeout1; double nativeout2;
        _native->Interpolate(x, y, nativeout1, nativeout2);
        out1 = nativeout1; out2 = nativeout2;
    }

    bool Interpolator2D::InterpolateGrid(unsigned long long pGridArray, int gridWidth, int gridHeiht, double xStart, double xStep, double yStart, double yStep)
    {
        return _native->InterpolateGrid((double*)pGridArray, gridWidth, gridHeiht, xStart, xStep, yStart, yStep);
    }

    bool Interpolator2D::InterpolateGrid(unsigned long long pGridArray1, unsigned long long pGridArray2, int gridWidth, int gridHeiht, double xStart, double xStep, double yStart, double yStep)
    {
        return _native->InterpolateGrid((double*)pGridArray1, (double*)pGridArray2, gridWidth, gridHeiht, xStart, xStep, yStart, yStep);
    }
}