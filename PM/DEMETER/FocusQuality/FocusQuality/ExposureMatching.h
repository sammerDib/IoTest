#pragma once

#ifdef EXPOSUREMATCHING_EXPORTS
#define EXPOSUREMATCHING_API __declspec(dllexport)
#else
#define EXPOSUREMATCHING_API __declspec(dllimport)
#endif

#include <string>

#include "WaferSubAreas.h"


// This class is exported from the dll
/*class EXPOSUREMATCHING_API CExposureMatching {
public:
    CExposureMatching(void);
    // TODO: add your methods here.
};*/


extern "C" __declspec(dllexport) int CalibrateExposureMatching(unsigned char* pIm, unsigned int nSizeH, unsigned int nSizeV, unsigned int listOfGoldenLuminanceValues[], float& ExposureMatchingCoeff);    // To call for exposure calibration (top left bottom right center)

extern "C" __declspec(dllexport) int GetGoldenValuesFromImage(unsigned char* pIm, unsigned int nSiveH, unsigned int nSizeV, unsigned int *goldenLuminanceValues);

extern "C" __declspec(dllexport) int GenerateArbitraryGoldenValues(std::vector<unsigned int>& vListOfGoldenLuminanceValues);  // In case no file for golden values has been found, or if it is not valid, generate golden values.


class CExposureMatchingComputation : public CWaferSubAreas
{
    int MeasureLuminanceValues(unsigned char* pIm);
    
protected:
    std::vector<float> m_vGoldenValues;
    std::vector<float> m_vMeasuredValues;

public:
    CExposureMatchingComputation();
    CExposureMatchingComputation(unsigned int nImSizeH, unsigned int nImSizeV);
    ~CExposureMatchingComputation();

    float ComputeExposureMatchingCoefficient(unsigned char* pIm, std::vector<unsigned int> vListOfGoldenLuminances); // Main processing function, for calling by the exported function. Can return -1 if the calculation fails.
    int ComputeGoldenValuesOut(unsigned char* pIm, std::vector<unsigned int> &vListOfGoldenLuminances);

protected:

    void SetGoldenValue(const unsigned int nRank, const unsigned int nValue);

    void AllocateCoefficients(const unsigned int);
    float AverageOfSubImage(const unsigned char* pIm, const unsigned int nID);  // The reference gray value for each sub-image is its average level, except dusts and patterns
    float FitSubImagesValues()const; // Find the best multiplicative coefficient to fit the golden values, and return it as the expected coefficient for exposure time

};