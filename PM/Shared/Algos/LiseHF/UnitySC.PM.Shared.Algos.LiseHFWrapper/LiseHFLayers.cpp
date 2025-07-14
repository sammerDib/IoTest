#include "LiseHFLayers.h"
#include "LiseHFMacros.h"

#pragma managed
using namespace System;

namespace UnitySCPMSharedAlgosLiseHFWrapper 
{
    LiseHFLayers::LiseHFLayers()
    {
        Init();
    }

    LiseHFLayers::LiseHFLayers(double depth_um, double toleranceSearch_um)
    {
        Init();
        double defaultrefractiveindex = 3.8; //  Sillicium refractive index ?
        AddNewDepthLayer(depth_um, toleranceSearch_um, defaultrefractiveindex);
        ComputeNative();
    }

    LiseHFLayers::LiseHFLayers(double depth_um, double toleranceSearch_um, double refractiveIndex)
    {
        Init();
        AddNewDepthLayer(depth_um, toleranceSearch_um, refractiveIndex);
        ComputeNative();
    }

    // destructeur
    LiseHFLayers::~LiseHFLayers()
    {
        this->!LiseHFLayers();
    }

    //Finalizer
    LiseHFLayers::!LiseHFLayers()
    {
        //unmanaged
        ClearNatives();

        // managed
        SAFE_DELETE_GC(DepthsRefractiveIndex);
        SAFE_DELETE_GC(DepthsToleranceSearch_um);
        SAFE_DELETE_GC(Depths_um);
    }

    void LiseHFLayers::AddNewDepthLayer(double depth_um, double toleranceSearch_um, double refractiveIndex)
    {
        Depths_um->Add(depth_um);
        DepthsToleranceSearch_um->Add(toleranceSearch_um);
        DepthsRefractiveIndex->Add(refractiveIndex);
    }

    void LiseHFLayers::Init()
    {
        int layercapacity = 16;
        Depths_um = gcnew List<double>(layercapacity);
        DepthsToleranceSearch_um = gcnew List<double>(layercapacity);
        DepthsRefractiveIndex = gcnew List<double>(layercapacity);
        _LayerCount = 0;
    }

   
    void LiseHFLayers::ComputeNative()
    {
        ClearNatives();

        _LayerCount = Depths_um->Count;
        _pNativeOpticalDepths = new double[_LayerCount];
        _pNativeOpticalToleranceSearch = new double[_LayerCount];
        _pNativeRefIndex = new double[_LayerCount];

        _pNativeMeasuredDepths = new double[_LayerCount];
        _pNativePeaksAmplitude = new double[_LayerCount];
        _pNativePeaks = new double[_LayerCount];
        // ok for car peu de layer
        double dTotalThickness = 0.0;
        for (unsigned int i = 0; i < _LayerCount; i++)
        {
            _pNativeOpticalDepths[i] = Depths_um[i] * DepthsRefractiveIndex[i];
            _pNativeOpticalToleranceSearch[i] = DepthsToleranceSearch_um[i] * DepthsRefractiveIndex[i];
            _pNativeRefIndex[i] = DepthsRefractiveIndex[i];

            dTotalThickness += _pNativeOpticalDepths[i] + _pNativeOpticalToleranceSearch[i];
            
            _pNativeMeasuredDepths[i] = 0.0;
            _pNativePeaksAmplitude[i] = 0.0;
            _pNativePeaks[i] = 0.0;
        }

        if (dTotalThickness < 25.0)
            _FTdim = 2048;
        else if (dTotalThickness < 50.0)
            _FTdim = 4096;
        else
            _FTdim = 8192;

        _pFTMod = new double[_FTdim];
        _pFTz = new double[_FTdim];
    }

    bool LiseHFLayers::HasNativeBeenComputed()
    {
        if (_LayerCount == 0) 
            return false;
        else if (Depths_um->Count != _LayerCount)
            return false; //need updates _pNativeOpticalDepths should differ from Depth_um;
           
        return (_pNativeOpticalDepths != nullptr);
    }


    void LiseHFLayers::ClearNatives()
    {
        SAFE_DELETE_ARRAY(_pFTz);
        SAFE_DELETE_ARRAY(_pFTMod);
        _FTdim = 0;

        SAFE_DELETE_ARRAY(_pNativePeaks);
        SAFE_DELETE_ARRAY(_pNativePeaksAmplitude);
        SAFE_DELETE_ARRAY(_pNativeMeasuredDepths);

        SAFE_DELETE_ARRAY(_pNativeRefIndex);
        SAFE_DELETE_ARRAY(_pNativeOpticalToleranceSearch);
        SAFE_DELETE_ARRAY(_pNativeOpticalDepths);
        _LayerCount = 0;
    }

}
