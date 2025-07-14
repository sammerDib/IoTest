#pragma once
#include "LiseHFRawSignal.h"

using namespace System;
using namespace System::Collections::Generic;

namespace UnitySCPMSharedAlgosLiseHFWrapper {

    public ref class LiseHFLayers
    {

    public:
        LiseHFLayers(); // Empty Constructor
        LiseHFLayers(double depth_um, double toleranceSearch_um); // one "TSV depth Real layer in SI"
        LiseHFLayers(double depth_um, double toleranceSearch_um, double refractiveIndex); // Construtor Single Layer

        ~LiseHFLayers();
        !LiseHFLayers();

        void AddNewDepthLayer(double depth_um, double toleranceSearch_um, double refractiveIndex); // add real physical layer
        void ComputeNative();
        bool HasNativeBeenComputed();

        unsigned int GetLayerCount() { return _LayerCount; };
        double* GetNativeOpticalDepths() { return _pNativeOpticalDepths; };
        double* GetNativeOpticalTolerancesSearch() { return _pNativeOpticalToleranceSearch; };
        double* GetNativeRefIndex() { return _pNativeRefIndex; };

    public:
        List<double>^ Depths_um;        // real physical layer thickness
        List<double>^ DepthsToleranceSearch_um;
        List<double>^ DepthsRefractiveIndex;

    private :
        // Inputs
        unsigned int _LayerCount = 0 ;
        double* _pNativeOpticalDepths = nullptr;
        double* _pNativeOpticalToleranceSearch = nullptr;
        double* _pNativeRefIndex = nullptr;

    internal:
        // Native Intermediates for Computation Lib
        double* _pNativeMeasuredDepths = nullptr;
        double* _pNativePeaksAmplitude = nullptr;
        double* _pNativePeaks = nullptr;
        int     _FTdim = 0;
        double * _pFTMod = nullptr;
        double * _pFTz = nullptr;

    private :
        void Init();
        void ClearNatives();
    };
}
