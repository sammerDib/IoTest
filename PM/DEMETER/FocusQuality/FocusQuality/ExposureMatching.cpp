#include "pch.h"
#include "framework.h"
#include "ExposureMatching.h"
#include <cmath>
#include <ppl.h>
#include <ios>
#include <iostream>
#include <fstream>


/*CExposureMatching::CExposureMatching()
{
    return;
}*/


extern "C" int __declspec(dllexport) CalibrateExposureMatching(unsigned char* pIm, unsigned int nSizeH, unsigned int nSizeV, unsigned int listOfGoldenLuminanceValues[], float& ExposureMatchingCoeff) {
    
    // Object instance for processing
    CExposureMatchingComputation ExposureCalib(nSizeH, nSizeV);

    std::vector<unsigned int> vListOfGoldenLuminanceValues(5);
    for (size_t i = 0; i < 5; i++)
    {
        vListOfGoldenLuminanceValues[i] = listOfGoldenLuminanceValues[i];
    }
    
    // Processing
    ExposureMatchingCoeff = ExposureCalib.ComputeExposureMatchingCoefficient(pIm, vListOfGoldenLuminanceValues);

    if (ExposureMatchingCoeff > 0.0f)
        return 0;
    else
        return -1;
}

extern "C" int __declspec(dllexport) GetGoldenValuesFromImage(unsigned char* pIm, unsigned int nSizeH, unsigned int nSizeV, unsigned int *goldenLuminanceValues)
{
    
    CExposureMatchingComputation exposureCalib(nSizeH, nSizeV);
    
    std::vector<unsigned int> vListOfGoldenLuminanceValues(5, 0);
    
    if (exposureCalib.ComputeGoldenValuesOut(pIm, vListOfGoldenLuminanceValues) == EXIT_SUCCESS)
    {
        for (size_t i = 0; i < 5; i++)
        {
            goldenLuminanceValues[i] = vListOfGoldenLuminanceValues[i];
        }
        return EXIT_SUCCESS;
    }
    return EXIT_FAILURE;
}


 
// Function used in case the golden file is not valid
extern "C" int __declspec(dllexport) GenerateArbitraryGoldenValues(std::vector<unsigned int>& vListOfGoldenLuminanceValues)
{
    // These values come from a measurement in the POC PSD HR 300 mm
    vListOfGoldenLuminanceValues[0] = 197;  // Top
    vListOfGoldenLuminanceValues[1] = 208;  // Left
    vListOfGoldenLuminanceValues[2] = 218;  // Bottom
    vListOfGoldenLuminanceValues[3] = 210;  // Right
    vListOfGoldenLuminanceValues[4] = 221;  // Center

    return 0;
}


    CExposureMatchingComputation::CExposureMatchingComputation()
    {
        
    }

    CExposureMatchingComputation::CExposureMatchingComputation(unsigned int nImSizeH, unsigned int nImSizeV)
    {
        m_nImSizeH = nImSizeH;
        m_nImSizeV = nImSizeV;
        m_nSubSizeH = 0;
        m_nSubSizeV = 0;
        m_nSubImagesNb = 5;
        m_opticalMountShape = OpticalMountShape::Cross;
    }

    CExposureMatchingComputation::~CExposureMatchingComputation()
    {
        m_vGoldenValues.clear();
        m_vMeasuredValues.clear();
    }


    float CExposureMatchingComputation::AverageOfSubImage(const unsigned char* pIm, const unsigned int nID)
    {
        unsigned char* pSubIm = new unsigned char[m_nSubSizeV * m_nSubSizeH];
        bool bSubIm;

        // Data of sub-image
        bSubIm = GetSubImage(pIm, nID, pSubIm);

        if (bSubIm)
        {
            // Sub-Image average
            float fAv = (float)Average(pSubIm, m_nSubSizeV, m_nSubSizeH, 0, 0);

            // Threshold
            float fThreshold = 2.0f * fAv / 3.0f;
            float fSum = 0.0f;
            int iNb = 0;
            for (unsigned int nIndV = 0; nIndV < m_nSubSizeV; nIndV++)
            {
                for (unsigned int nIndH = 0; nIndH < m_nSubSizeH; nIndH++)
                {
                    if (pSubIm[nIndV * m_nSubSizeH + nIndH] > fThreshold)
                    {
                        fSum += (float)pSubIm[nIndV * m_nSubSizeH + nIndH];
                        iNb++;
                    }
                }
            }

            // Reference value for current area
            m_vMeasuredValues[nID] = fSum / iNb;

            return m_vMeasuredValues[nID];
        }
        else
            return -1.0f;   // Error
    }


    void CExposureMatchingComputation::AllocateCoefficients(const unsigned int nNb)
    {
        m_vMeasuredValues.resize(nNb);
        m_vGoldenValues.resize(nNb);
    }


    void CExposureMatchingComputation::SetGoldenValue(const unsigned int nRank, const unsigned int nValue)
    {
        m_vGoldenValues[nRank] = (float)nValue;
    }


    float CExposureMatchingComputation::FitSubImagesValues()const
    {
        int iLength = min(size(m_vGoldenValues), size(m_vMeasuredValues));

        if (iLength > 0)
        {
            int iInd;

            float fInvSquare = 0.0f;
            float fVectorsProduct = 0.0f;
            for (iInd = 0; iInd < iLength; iInd++)
            {
                fInvSquare += (float)(m_vMeasuredValues[iInd] * m_vMeasuredValues[iInd]); // The matrix product is only a float here
                fVectorsProduct += (float)(m_vMeasuredValues[iInd] * m_vGoldenValues[iInd]);
            }

            fInvSquare = 1.0f / fInvSquare;

            return fInvSquare * fVectorsProduct;
        }
        else
            return -1.0f;   // Error of number of values
    }


    // Public method called to get the coefficient to apply to exposure time for matching
    float CExposureMatchingComputation::ComputeExposureMatchingCoefficient(unsigned char* pIm, std::vector<unsigned int> vListOfGoldenLuminances)
    {
        if(MeasureLuminanceValues(pIm) == EXIT_SUCCESS)
        {
            int nAreasNb = static_cast<int>(m_vMeasuredValues.size());
            for (unsigned int nArea = 0; nArea < nAreasNb; nArea++)
            {
                SetGoldenValue(nArea, vListOfGoldenLuminances[nArea]);
            }

            return FitSubImagesValues();
        }
        return -1.0f;
    }

    int CExposureMatchingComputation::MeasureLuminanceValues(unsigned char* pIm)
    {
        // Wafer position in image
        bool bWaferFound = FindWaferPosition(pIm);

        // Sub-images processing
        if (bWaferFound)
        {
            // Computing position of sub-images
            unsigned int nAreasNb = ComputeSubImages();
            AllocateCoefficients(nAreasNb);

            for (unsigned int nArea = 0; nArea < nAreasNb; nArea++)
            {
                AverageOfSubImage(pIm, nArea);
            }
            return EXIT_SUCCESS;
        }
        return EXIT_FAILURE;
    }
  


    int CExposureMatchingComputation::ComputeGoldenValuesOut(unsigned char* pIm, std::vector<unsigned int> &vListOfGoldenLuminances)
    {
        if (MeasureLuminanceValues(pIm) == EXIT_SUCCESS)
        {
            int nbAreas = static_cast<int>(m_vMeasuredValues.size());
            for (int nArea = 0; nArea < nbAreas; nArea++)
            {
                vListOfGoldenLuminances[nArea] = static_cast<unsigned int>(m_vMeasuredValues[nArea]);
            }
            return EXIT_SUCCESS;
        }
        return EXIT_FAILURE;
    }