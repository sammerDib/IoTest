#pragma once

extern "C" __declspec(dllexport) int InitLoadCall();
extern "C" __declspec(dllexport) int PerformDensity(unsigned char* pIn, unsigned char* pOut, int iPitchIn, int iSizeX, int iSizeY, int MaskSize, int SignificantDensity);
extern "C" __declspec(dllexport) int PerformRognage(unsigned char* pIn, unsigned char* pOut, int iPitchIn, int iSizeX, int iSizeY, int SignificantPixel);
extern "C" __declspec(dllexport) int PerformSmoothing(unsigned char* pIn, unsigned char* pOut, int iPitchIn, int iSizeX, int iSizeY);

extern "C" __declspec(dllexport) int PerformMedianFilter(unsigned char* pIn, unsigned char* pOut, int iPitchIn, int iPitchOut, int iSizeX, int iSizeY, int iRadius, long l2CacheMemSize);
extern "C" __declspec(dllexport) int PerformMedianFloatFilter(void* pIn, void* pOut, int iSizeX, int iSizeY, int iRadiusX, int iRadiusY, int NbCores);
extern "C" __declspec(dllexport) int PerformMedianFilter_U16(void* pIn, void* pOut, int iSizeX, int iSizeY, int iRadiusX, int iRadiusY, int NbCores);
extern "C" __declspec(dllexport) int PerformMedianFilter_U8(void* pIn, void* pOut, int iSizeX, int iSizeY, int iRadiusX, int iRadiusY, int NbCores);

extern "C" __declspec(dllexport) int PerformLinearDynScaling(unsigned char* pIn, unsigned char* pOut, int iPitchIn, int iPitchOut, int iSizeX, int iSizeY, unsigned char uMin, unsigned char uMax);
