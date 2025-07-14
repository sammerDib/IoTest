// FocusQuality.cpp : Defines the exported functions for the DLL.
//

#include "pch.h"
#include "framework.h"
#include "FocusQuality.h"
#include <cmath>
#include <ppl.h>
#include <ios>
#include <iostream>
#include <fstream>
#include <algorithm>
using namespace Concurrency;

// This is an example of an exported variable
FOCUSQUALITY_API int nFocusQuality = 0;

// This is an example of an exported function.
FOCUSQUALITY_API int fnFocusQuality(void)
{
    return 0;
}

// This is the constructor of a class that has been exported.
CFocusQuality::CFocusQuality()
{
    return;
}

extern "C" int __declspec(dllexport) GetFocusQuality(sSubImProp * pSubImages[])
{
    pSubImages[0]->nNum = 2;
    return 42;
}

/*void TestFocusQuality()
{
    // Input data and parameters
    const char* pImPath("D:\\PSD\\Matching\\FocusQuality\\Focus\\Img20_Reflectivity.tiff");

    CH3Array2D<unsigned char> aMyIm;

    // Loading image (it is stored line after line)
    LoadFromTIF(pImPath, aMyIm);

    // Using a simple pointer on data
    unsigned char* pMyIm;
    pMyIm = aMyIm.GetData();
    unsigned int nImSizeV = (unsigned int)aMyIm.GetLi();
    unsigned int nImSizeH = (unsigned int)aMyIm.GetCo();

    // Computing focus criterion
    sSubImProp* pSubImages = new sSubImProp[5];
    int Error = FocusQuality(pMyIm, nImSizeV, nImSizeH, 500.0 / 100.0, pSubImages);	/: pattern size �m / camera resolution pix

    delete[] pSubImages;
}*/

// wafersize real wafer size in mm
// patternSize real patternsize in µm
extern "C" int __declspec(dllexport) FocusQuality(unsigned char* pIm, unsigned int nSizeH, unsigned int nSizeV, double dWaferSize, double  dPatternSizeMicrons, unsigned int opticalMountShape, sSubImProp pSubImages[])
{

    CFocusQualityComputation WaferFocus(nSizeH, nSizeV, static_cast<OpticalMountShape>(opticalMountShape));


    // Wafer position in image
    bool bWaferFound = WaferFocus.FindWaferPosition(pIm);

    if (!pSubImages || WaferFocus.GetSubImagesNb() == 0)
        return 1;

    // Making sub-image list
    if (bWaferFound)
    {
        double patternSizePix = dPatternSizeMicrons / (dWaferSize * 1000 / (WaferFocus.GetWaferHEnd() - WaferFocus.GetWaferHStart()));
        if (patternSizePix < 1)
        {
            return 4;
        }

        WaferFocus.SetExpectedPatternSizePix(patternSizePix);

        // Computing position of sub-images: if the wafer moves from one image to another, the sub-images must follow.
        WaferFocus.ComputeSubImages();

        // Updating sub-areas properties:
        for (int iL = 0; iL < WaferFocus.GetSubImagesNb(); iL++)
        {
            pSubImages[iL].nNum = iL;
            pSubImages[iL].nPixStartH = WaferFocus.GetSubImageHStart(iL);
            pSubImages[iL].nPixStartV = WaferFocus.GetSubImageVStart(iL);
            pSubImages[iL].nSizeH = WaferFocus.GetSubImageHSize();
            pSubImages[iL].nSizeV = WaferFocus.GetSubImageVSize();
        }

        // Computing focus quality for sub-image

        bool bOK = true;
        const int NbIm = WaferFocus.GetSubImagesNb();
        //for (int iL = 0; iL < WaferFocus.GetSubImagesNb(); iL++)
        //{
        parallel_for(0, NbIm, [&](int iL)
            {
                // Compute sub-image focus quality

                pSubImages[iL].dFocusQuality = WaferFocus.ComputeFocusQuality(pIm, iL);

                if (pSubImages[iL].dFocusQuality < 0.0 || isnan(pSubImages[iL].dFocusQuality))
                    bOK = false;
            });
        //}

        if (!bOK)
            return 3;
    }
    else
    {
        return 2;
    }

    return 0;	// Everything OK
}

CFocusQualityComputation::CFocusQualityComputation()
{
    m_dExpectedPatternSize = 0.0;
    m_nSubSizeH = 0;
    m_nSubSizeV = 0;
}

CFocusQualityComputation::CFocusQualityComputation(OpticalMountShape opticalMountShape)
{
    m_dExpectedPatternSize = 0.0;
    m_nSubSizeH = 0;
    m_nSubSizeV = 0;
    m_opticalMountShape = opticalMountShape;

    if (OpticalMountShape::Cross == opticalMountShape || OpticalMountShape::SquarePlusCenter == opticalMountShape)
        m_nSubImagesNb = 5;
    else if (OpticalMountShape::LBottomLeft == opticalMountShape || OpticalMountShape::LBottomRight == opticalMountShape || OpticalMountShape::LTopLeft == opticalMountShape || OpticalMountShape::LTopRight == opticalMountShape)
        m_nSubImagesNb = 3;
    else
        m_nSubImagesNb = 0;
}

CFocusQualityComputation::CFocusQualityComputation(unsigned int nImSizeH, unsigned int nImSizeV, OpticalMountShape opticalMountShape)
{

    m_dExpectedPatternSize = 0.0;
    m_nImSizeH = nImSizeH;
    m_nImSizeV = nImSizeV;
    m_nSubSizeH = 0;
    m_nSubSizeV = 0;
    m_opticalMountShape = opticalMountShape;
    

    if (OpticalMountShape::Cross == opticalMountShape || OpticalMountShape::SquarePlusCenter == opticalMountShape)
    {
        m_nSubImagesNb = 5;
    }
    else if (OpticalMountShape::LBottomLeft == opticalMountShape || OpticalMountShape::LBottomRight == opticalMountShape || OpticalMountShape::LTopLeft == opticalMountShape || OpticalMountShape::LTopRight == opticalMountShape)
    {
        m_nSubImagesNb = 3;
    }
    else
    {
        m_nSubImagesNb = 0;
    }
        
}

CFocusQualityComputation::~CFocusQualityComputation()
{
}

double CFocusQualityComputation::ComputeFocusQuality(unsigned char* pIm, const unsigned int nID) const
{
    double dPatternSurfaceCoeff = 0.01 * 0.5;	// Patterns occupy less than 1% of the surface

    // Get the sub-image
    unsigned char* pSubIm;
    bool bSubIm;
    pSubIm = new unsigned char[m_nSubSizeV * m_nSubSizeH];
    bSubIm = GetSubImage(pIm, nID, pSubIm);

    if (bSubIm)
    {
        // Computing image average value
        double dAverage = Average(pSubIm, m_nSubSizeV, m_nSubSizeH, 0, 0);

        // Computing a threshold depending on the data content
        double dThreshold;
        unsigned int* pImHisto = new unsigned int[256];
        MakeImageHisto(pSubIm, pImHisto);	// Histogram on 1/4 pixels
        // Examine the histogram starting from low values until 1% of the number of pixels is found
        unsigned int nCount = 0, nHistoIntegration = 0;
        while (nCount < 256 && nHistoIntegration < dPatternSurfaceCoeff * m_nSubSizeV * m_nSubSizeH / 4)	// /4 because of downsampling factor in the histogram
        {
            nHistoIntegration += pImHisto[nCount];
            nCount++;
        }
        if (nCount < dAverage)
            dThreshold = (dAverage - (double)nCount) * 0.5 + (double)nCount;
        else
            dThreshold = 2.0 / 3.0 * dAverage;

        delete[] pImHisto;
        pImHisto = nullptr;

        // To emphasize the importance of wafer patterns and decrease the possible weight of screen pixels, patterns are detected and a mask is created.
        unsigned char* pPatternsMask = new unsigned char[m_nSubSizeV * m_nSubSizeH];

        // For every pixel, bring its neighborhood area average to the value of the global average
        int iLocalAverageSizeV = (unsigned int)floor(m_nSubSizeV / 10.0);
        int iLocalAverageSizeH = (unsigned int)floor(m_nSubSizeH / 10.0);
        // Ensuring even sizes
        if (iLocalAverageSizeV % 2 != 0)
            iLocalAverageSizeV++;
        if (iLocalAverageSizeH % 2 != 0)
            iLocalAverageSizeH++;

        double dPixNeighborhood;
        unsigned int nRealLocalSizeV, nRealLocalSizeH, nRealStartV, nRealStartH;
        double dCurrentValue;

        unsigned char* pNeighborIm = new unsigned char[iLocalAverageSizeV * iLocalAverageSizeH];	// May be smaller along edges

        // Preparing the neighborhood averaging: computing a matrix of integration
        unsigned int* pIntegrationMatrix = new unsigned int[m_nSubSizeV * m_nSubSizeH];
        pIntegrationMatrix[0] = pSubIm[0];

        // Starting with the first row and the first column
        for (int iV = 1; iV < (int)m_nSubSizeV; iV++)
            pIntegrationMatrix[iV * m_nSubSizeH] = pIntegrationMatrix[(iV - 1) * m_nSubSizeH] + pSubIm[iV * m_nSubSizeH];

        for (int iH = 1; iH < (int)m_nSubSizeH; iH++)
            pIntegrationMatrix[iH] = pIntegrationMatrix[iH - 1] + pSubIm[iH];

        for (int iV = 1; iV < (int)m_nSubSizeV; iV++)
        {
            for (int iH = 1; iH < (int)m_nSubSizeH; iH++)
            {
                pIntegrationMatrix[iV * m_nSubSizeH + iH] = pIntegrationMatrix[iV * m_nSubSizeH + iH - 1] + pIntegrationMatrix[(iV - 1) * m_nSubSizeH + iH] - pIntegrationMatrix[(iV - 1) * m_nSubSizeH + iH - 1] + pSubIm[iV * m_nSubSizeH + iH];
            }
        }

        // Local average and mask
        for (int iV = 0; iV < (int)m_nSubSizeV; iV++)
        {
            // Managing edge effects
            nRealStartV = max(iV - iLocalAverageSizeV / 2, 0);

            if (iV < (int)m_nSubSizeV - iLocalAverageSizeV / 2)
                nRealLocalSizeV = iLocalAverageSizeV / 2 + iV - nRealStartV;
            else
                nRealLocalSizeV = m_nSubSizeV - nRealStartV - 1;

            for (int iH = 0; iH < (int)m_nSubSizeH; iH++)
            {
                // Managing edge effects
                nRealStartH = max(iH - iLocalAverageSizeH / 2, 0);

                if (iH < (int)m_nSubSizeH - iLocalAverageSizeH / 2)
                    nRealLocalSizeH = iLocalAverageSizeH / 2 + iH - nRealStartH;
                else
                    nRealLocalSizeH = m_nSubSizeH - nRealStartH - 1;

                // Local averaging using the integration matrix
                if (nRealStartV > 0 && nRealStartH > 0)
                {
                    dPixNeighborhood = (double)(pIntegrationMatrix[(nRealStartV + nRealLocalSizeV - 1) * m_nSubSizeH + nRealStartH + nRealLocalSizeH - 1]
                        - pIntegrationMatrix[(nRealStartV - 1) * m_nSubSizeH + nRealStartH + nRealLocalSizeH - 1]
                        - pIntegrationMatrix[(nRealStartV + nRealLocalSizeV - 1) * m_nSubSizeH + nRealStartH - 1]
                        + pIntegrationMatrix[(nRealStartV - 1) * m_nSubSizeH + nRealStartH - 1]) / (double)(nRealLocalSizeV * nRealLocalSizeH);
                }
                else if (nRealStartV == 0)
                {
                    if (nRealStartH == 0)
                        dPixNeighborhood = (double)pIntegrationMatrix[(nRealLocalSizeV - 1) * m_nSubSizeH + nRealLocalSizeH - 1] / (double)(nRealLocalSizeV * nRealLocalSizeH);
                    else
                        dPixNeighborhood = (double)(pIntegrationMatrix[(nRealLocalSizeV - 1) * m_nSubSizeH + nRealStartH + nRealLocalSizeH - 1]
                            - pIntegrationMatrix[(nRealStartV + nRealLocalSizeV - 1) * m_nSubSizeH + nRealStartH - 1]) / (double)(nRealLocalSizeV * nRealLocalSizeH);
                }
                // nRealStartH == 0
                else
                {
                    dPixNeighborhood = (double)(pIntegrationMatrix[(nRealStartV + nRealLocalSizeV - 1) * m_nSubSizeH + nRealStartH + nRealLocalSizeH - 1]
                        - pIntegrationMatrix[(nRealStartV - 1) * m_nSubSizeH + nRealStartH + nRealLocalSizeH - 1]) / (double)(nRealLocalSizeV * nRealLocalSizeH);
                }

                // Correcting current pixel regarding its neighborhood average and deciding if this corrected pixel must go in the mask
                dCurrentValue = (double)pSubIm[iV * m_nSubSizeH + iH] + dAverage - dPixNeighborhood;

                if (dCurrentValue < dThreshold)
                    pPatternsMask[iV * m_nSubSizeH + iH] = 1;
                else
                    pPatternsMask[iV * m_nSubSizeH + iH] = 0;
            }
        }

        delete[] pNeighborIm;
        delete[] pIntegrationMatrix;
        pNeighborIm = nullptr;

        // Slight erosion of mask to remove isolated pixels
        unsigned char* pErodedSubIm = new unsigned char[m_nSubSizeV * m_nSubSizeH];
		unsigned char* pDilatedSubIm = new unsigned char[m_nSubSizeV * m_nSubSizeH];
        Erosion(pPatternsMask, pErodedSubIm, 2);

		// Dilation to retrieve the initial pattern size
        Dilation(pErodedSubIm, pDilatedSubIm, m_nSubSizeV, m_nSubSizeH, 2);
        
        // Clusters method
        double dQuality = ClustersContrast(pSubIm, pDilatedSubIm);

        // Dilation to get an extended area around each pattern
        //Dilation(pErodedSubIm, pPatternsMask, nVSize, nHSize, (unsigned int)dExpectedPatternSize + 1);
        delete[] pErodedSubIm;
        delete[] pDilatedSubIm;

        // Standard deviation for areas around patterns (dilated zones):
        //double dQuality = StandardDeviationOfMaskedImage(pSubIm, pPatternsMask, nVSize, nHSize, dAverage)*100;

        delete[] pPatternsMask;

        return dQuality;
    }
    else
        return -1.0;
}

// This erosion function is specific for binary masks, not working for grayscale images.
void CFocusQualityComputation::Erosion(const unsigned char* pIm, unsigned char* pOutIm, const unsigned int nKernelHalfSize) const
{
    unsigned int nKernelFullSize = nKernelHalfSize * 2 + 1;

    // Creating erosion kernel
    unsigned char* pKernel = new unsigned char[nKernelFullSize * nKernelFullSize];
    unsigned int nKH, nKV;
    unsigned int nNbOnes = 0;

    for (nKV = 0; nKV < nKernelFullSize; nKV++)
    {
        for (nKH = 0; nKH < nKernelFullSize; nKH++)
        {
            if (sqrt((double)((nKernelHalfSize - nKH) * (nKernelHalfSize - nKH) + (nKernelHalfSize - nKV) * (nKernelHalfSize - nKV))) <= (double)nKernelHalfSize)
            {
                pKernel[nKV * nKernelFullSize + nKH] = 1;
                nNbOnes++;
            }
            else
                pKernel[nKV * nKernelFullSize + nKH] = 0;
        }
    }

    // Put 0 in pixels at the image edges
    // Horizontal
    for (unsigned int nV = 0; nV < nKernelHalfSize; nV++)
    {
        for (unsigned int nH = 0; nH < m_nSubSizeH; nH++)
        {
            pOutIm[nV * m_nSubSizeH + nH] = 0;
            pOutIm[(m_nSubSizeV - nV - 1) * m_nSubSizeH + nH] = 0;
        }
    }

    // Vertical
    for (unsigned int nV = 0; nV < m_nSubSizeV; nV++)
    {
        for (unsigned int nH = 0; nH < nKernelHalfSize; nH++)
        {
            pOutIm[nV * m_nSubSizeH + nH] = 0;
            pOutIm[nV * m_nSubSizeH + m_nSubSizeH - nH - 1] = 0;
        }
    }

    // Everywhere except image edges
    bool bZeros;

    for (unsigned int nV = nKernelHalfSize; nV < m_nSubSizeV - nKernelHalfSize; nV++)
    {
        for (unsigned int nH = nKernelHalfSize; nH < m_nSubSizeH - nKernelHalfSize; nH++)
        {
            bZeros = false;

            // Get the neighborhood of this pixel (nH,nV)
            for (nKV = 0; nKV < nKernelFullSize && !bZeros; nKV++)
            {
                for (nKH = 0; nKH < nKernelFullSize && !bZeros; nKH++)
                {
                    if (pKernel[nKV * nKernelFullSize + nKH] == 1)
                        if (pIm[(nV - nKernelHalfSize + nKV) * m_nSubSizeH + nH - nKernelHalfSize + nKH] == 0)
                            bZeros = true;
                }
            }

            pOutIm[nV * m_nSubSizeH + nH] = bZeros ? 0 : 1;
        }
    }

    delete[] pKernel;
}


// Dilation with circular kernel.
// Todo: dilate until image edges.
void Dilation(const unsigned char* pIm, unsigned char* pOutIm, const unsigned int nImSizeV, const unsigned int nImSizeH, const unsigned int nKernelHalfSize)
{
    unsigned int nKernelFullSize = nKernelHalfSize * 2 + 1;

    // Creating dilation kernel
    unsigned char* pKernel = new unsigned char[nKernelFullSize * nKernelFullSize];
    unsigned int nKH, nKV;
    unsigned int nNbOnes = 0;

    for (nKV = 0; nKV < nKernelFullSize; nKV++)
    {
        for (nKH = 0; nKH < nKernelFullSize; nKH++)
        {
            if (sqrt((double)((nKernelHalfSize - nKH) * (nKernelHalfSize - nKH) + (nKernelHalfSize - nKV) * (nKernelHalfSize - nKV))) <= (double)nKernelHalfSize)
            {
                pKernel[nKV * nKernelFullSize + nKH] = 1;
                nNbOnes++;
            }
            else
                pKernel[nKV * nKernelFullSize + nKH] = 0;
        }
    }

    // Put 0 in pixels at the image edges
    // Horizontal
    for (unsigned int nV = 0; nV < nKernelHalfSize; nV++)
    {
        for (unsigned int nH = 0; nH < nImSizeH; nH++)
        {
            pOutIm[nV * nImSizeH + nH] = 0;
            pOutIm[(nImSizeV - nV - 1) * nImSizeH + nH] = 0;
        }
    }

    // Vertical
    for (unsigned int nV = 0; nV < nImSizeV; nV++)
    {
        for (unsigned int nH = 0; nH < nKernelHalfSize; nH++)
        {
            pOutIm[nV * nImSizeH + nH] = 0;
            pOutIm[nV * nImSizeH + nImSizeH - nH - 1] = 0;
        }
    }

    // Everywhere except image edges
    bool bOnes;

    for (unsigned int nV = nKernelHalfSize; nV < nImSizeV - nKernelHalfSize; nV++)
    {
        for (unsigned int nH = nKernelHalfSize; nH < nImSizeH - nKernelHalfSize; nH++)
        {
            bOnes = false;

            // Get the neighborhood of this pixel (nH,nV)
            for (nKV = 0; nKV < nKernelFullSize && !bOnes; nKV++)
            {
                for (nKH = 0; nKH < nKernelFullSize && !bOnes; nKH++)
                {
                    if (pKernel[nKV * nKernelFullSize + nKH] == 1)
                        if (pIm[(nV - nKernelHalfSize + nKV) * nImSizeH + nH - nKernelHalfSize + nKH] == 1)
                            bOnes = true;
                }
            }

            pOutIm[nV * nImSizeH + nH] = bOnes ? 1 : 0;
        }
    }

    delete[] pKernel;
}



// On 256 gray levels
void CFocusQualityComputation::MakeImageHisto(const unsigned char* pIm, unsigned int* pHisto) const
{
    for (unsigned int nInd = 0; nInd < 256; nInd++)
        pHisto[nInd] = 0;

    // On full image with gray levels
    for (unsigned int nV = 0; nV < m_nSubSizeV; nV += 2)
    {
        for (unsigned int nH = 0; nH < m_nSubSizeH; nH += 2)
        {
            pHisto[pIm[nV * m_nSubSizeH + nH]]++;
        }
    }
}


// Computing the standard deviation only accounting for the unmasked areas of the image
double StandardDeviationOfMaskedImage(const unsigned char* pIm, unsigned char* pMask, const unsigned int nImSizeV, const unsigned int nImSizeH, const double dSubImAverage)
{
    double dStdev = 0;
    double dAverage = 0;
    double dSqAverage = 0;
    unsigned int nNbInMask = 0;

    for (unsigned int nV = 0; nV < nImSizeV; nV++)
    {
        for (unsigned int nH = 0; nH < nImSizeH; nH++)
        {
            if (pMask[nV * nImSizeH + nH] == 1)
            {
                dAverage += pIm[nV * nImSizeH + nH];
                dSqAverage += pIm[nV * nImSizeH + nH] * pIm[nV * nImSizeH + nH];
                nNbInMask++;
            }
        }
    }

    dAverage /= nNbInMask;
    dSqAverage /= nNbInMask;

    return sqrt(dSqAverage - dAverage * dAverage) / dSubImAverage;
}

// Goal: listing the coordinates of pixels to 1 in the mask and their background coordinates, and computing the contrast between patterns and background
// The input mask will be modified to avoid counting twice the same pixels!
// Returned value between 0 and 100
double CFocusQualityComputation::ClustersContrast(const unsigned char* pSubIm, unsigned char* pErodedMask) const
{
    // Looking for pixels of value 1 in the mask and the clusters they belong to.
    // Use the pixels in the mask to compute the barycenter of each cluster.
    // Define an area of the expected pattern size around this barycenter and compute the contrast between this area and its own neighborhood.
    unsigned int nNeighborhoodSizeH, nNeighborhoodSizeV, nStartH, nStartV;
    unsigned int nPatternPixCount = 0, nBackgroundPixCount = 0;
    unsigned int nClusterCount = 0;
    float fBaryV, fBaryH;
    unsigned int nBaryV, nBaryH;
    unsigned int nNV, nNH;
    double dAveragePat, dAverageBack;
    unsigned int nPatternStartH, nPatternStartV, nPatternEndH, nPatternEndV;

    // Variables for patterns neighborhood count
    std::vector<double> ListOfContrasts;

    const unsigned int nNeighborhoodMaxSize = (unsigned int)(4 * m_dExpectedPatternSize) + 1;
    std::pair<unsigned int, unsigned int>* pClusterList = new std::pair<unsigned int, unsigned int>[nNeighborhoodMaxSize * nNeighborhoodMaxSize]; // Too large, but useful to allocate once only

    // Catch the first pixel of a pattern, and count how many they are around it
    const float fHalfExpectedPattern = (float)m_dExpectedPatternSize * 0.5f;
    const unsigned int nNeighborhoodHalfSizeTheoretical = (unsigned int)(2 * m_dExpectedPatternSize);

    unsigned int nWeight, nWeightSum;

    for (unsigned int nV = 0; nV < m_nSubSizeV; nV++)
    {
        for (unsigned int nH = 0; nH < m_nSubSizeH; nH++)
        {
            if (pErodedMask[nV * m_nSubSizeH + nH] == 1)
            {
                // Defining the neighborhood and managing edge effects
                // The neighborhood needs to be significantly large to make the impact of the pattern edges negligible
                nStartH = (unsigned int)(max((float)nH + (float)m_dExpectedPatternSize * 0.5f - (float)nNeighborhoodHalfSizeTheoretical, 0.0f));

                if (nH < m_nSubSizeH - nNeighborhoodHalfSizeTheoretical)
                    nNeighborhoodSizeH = nNeighborhoodHalfSizeTheoretical + nH - nStartH + 1;  // +1 because nH is in the center (odd number of pixels)
                else
                    nNeighborhoodSizeH = m_nSubSizeH - nStartH;

                nStartV = (unsigned int)(max((float)nV + (float)m_dExpectedPatternSize * 0.5f - (float)nNeighborhoodHalfSizeTheoretical, 0.0f));

                if (nV < m_nSubSizeV - nNeighborhoodHalfSizeTheoretical)
                    nNeighborhoodSizeV = nNeighborhoodHalfSizeTheoretical + nV - nStartV + 1;
                else
                    nNeighborhoodSizeV = m_nSubSizeV - nStartV;

                // Looking for pixels equal to 1: cluster
                nClusterCount = 0;
                for (nNV = nStartV; nNV < nStartV + nNeighborhoodSizeV; nNV++)
                {
                    for (nNH = nStartH; nNH < nStartH + nNeighborhoodSizeH; nNH++)
                    {
                        if (pErodedMask[min(nNV * m_nSubSizeH + nNH, m_nSubSizeV * m_nSubSizeH - 1)] == 1)
                        {
                            pClusterList[nClusterCount] = std::make_pair(nNV, nNH); // This array of pairs is never emptied, only re-written
                            nClusterCount++;

                            // Avoiding counting the same pixel twice: not equal to 1 anymore
                            pErodedMask[min(nNV * m_nSubSizeH + nNH, m_nSubSizeV * m_nSubSizeH - 1)] = 2;
                        }
                    }
                }

                // No pattern detected, or pattern of the wrong size. In case of defocus, the pattern can be larger, but not too much.
                if (!(nClusterCount == 0 || nClusterCount > 4 * m_dExpectedPatternSize * m_dExpectedPatternSize))
                {
                    // Computing the barycenter (weighted by the pixel values) of the cluster coming from threshold
                    nBaryV = 0, nBaryH = 0;
                    nWeight = 0, nWeightSum = 0;

                    for (unsigned int nC = 0; nC < nClusterCount; nC++)
                    {
                        // The weights considered for the barycenter are taken from the negative image (255-value) to give the deepest darks the maximum weight.
                        nWeight = 255 - pSubIm[std::get<1>(pClusterList[nC]) * m_nSubSizeV + std::get<0>(pClusterList[nC])];
                        nWeightSum += nWeight;
                        nBaryV += std::get<0>(pClusterList[nC]) * nWeight;
                        nBaryH += std::get<1>(pClusterList[nC]) * nWeight;
                    }

                    // Use a float barycenter, to compute the most accurate pattern location around it
                    fBaryV = (float)nBaryV / (float)nWeightSum;
                    fBaryH = (float)nBaryH / (float)nWeightSum;
                    nBaryV = (unsigned int)round(fBaryV);
                    nBaryH = (unsigned int)round(fBaryH);

                    // Avoid the patterns too close to the sub-image edge, because the impact of the blurred pattern edge becomes important compared to the impact of the well-focused pixels, it degrades the contrast.
                    if (!(fBaryV < fHalfExpectedPattern || fBaryV >(float)m_nSubSizeV - fHalfExpectedPattern || fBaryH < fHalfExpectedPattern || fBaryH >(float)m_nSubSizeH - fHalfExpectedPattern) && pErodedMask[nBaryV * m_nSubSizeH + nBaryH] > 0)
                    {
                        // Use the barycenter to gather the pattern pixels, excluding a margin of 1 pixel all around it
                        dAveragePat = 0.0;
                        nPatternPixCount = 0;
                        nPatternStartH = (unsigned int)round(max(fBaryH - fHalfExpectedPattern, 0.0f));
                        nPatternEndH = (unsigned int)round(min(fBaryH + fHalfExpectedPattern, (float)m_nSubSizeH));
                        nPatternStartV = (unsigned int)round(max(fBaryV - fHalfExpectedPattern, 0.0f));
                        nPatternEndV = (unsigned int)round(min(fBaryV + fHalfExpectedPattern, (float)m_nSubSizeV));

                        for (unsigned int nNV = nPatternStartV; nNV < nPatternEndV; nNV++)
                        {
                            for (unsigned int nNH = nPatternStartH; nNH < nPatternEndH; nNH++)
                            {
                                pErodedMask[nNV * m_nSubSizeH + nNH] = 3;

                                if (nNH > nPatternStartH && nNH< nPatternEndH - 1 && nNV>nPatternStartV && nNV < nPatternEndV - 1)
                                {
                                    dAveragePat += pSubIm[nNV * m_nSubSizeH + nNH];
                                    nPatternPixCount++;
                                }
                            }
                        }

                        // Use the barycenter to define a pattern area around it and compute its average.

                        // Re-compute the neighbor area starting this time from the barycenter
                        nStartH = (unsigned int)round(max(fBaryH + fHalfExpectedPattern - (float)nNeighborhoodHalfSizeTheoretical - 1.0f, 0.0f));

                        if (nBaryH < m_nSubSizeH - nNeighborhoodHalfSizeTheoretical)
                            nNeighborhoodSizeH = nNeighborhoodHalfSizeTheoretical + nBaryH - nStartH;
                        else
                            nNeighborhoodSizeH = m_nSubSizeH - nStartH;

                        nStartV = (unsigned int)round(max(fBaryV + fHalfExpectedPattern - (float)nNeighborhoodHalfSizeTheoretical - 1.0f, 0.0f));

                        if (nBaryV < m_nSubSizeV - nNeighborhoodHalfSizeTheoretical)
                            nNeighborhoodSizeV = nNeighborhoodHalfSizeTheoretical + nBaryV - nStartV;
                        else
                            nNeighborhoodSizeV = m_nSubSizeV - nStartV;

                        // Background pixels (those in the neighborhood that are not in the mask)
                        dAverageBack = 0.0;
                        nBackgroundPixCount = 0;
                        for (nNV = nStartV; nNV < nStartV + nNeighborhoodSizeV; nNV++)
                        {
                            for (nNH = nStartH; nNH < nStartH + nNeighborhoodSizeH; nNH++)
                            {
                                if (pErodedMask[nNV * m_nSubSizeH + nNH] == 0)
                                {
                                    dAverageBack += pSubIm[nNV * m_nSubSizeH + nNH];
                                    nBackgroundPixCount++;
                                }
                                pErodedMask[nNV * m_nSubSizeH + nNH] = 3;   // Never investigate this neighborhood again
                            }
                        }

                        // Checking regions sizes (the last condition is used to eliminate the center pattern, which is large and has many pixels inside the neighbor area)
                        if (nPatternPixCount >= (m_dExpectedPatternSize - 2) * (m_dExpectedPatternSize - 2) * 0.5 && nBackgroundPixCount >= nPatternPixCount * 2 && nBackgroundPixCount > 0.7 * nNeighborhoodSizeV * nNeighborhoodSizeH)
                        {
                            dAveragePat /= (float)nPatternPixCount;
                            dAverageBack /= (float)nBackgroundPixCount;

                            if (dAveragePat < 0.95 * dAverageBack)
                                ListOfContrasts.push_back((dAverageBack - dAveragePat) / (dAverageBack + dAveragePat));
                        }
                    }
                }
            }
        }
    }

    // Average contrast of patterns compared to background
    double dQuality = 0.0;

    if (ListOfContrasts.size() > 0)
    {
        // Removing outliers
        double dMedian = 0.0;
        std::sort(ListOfContrasts.begin(), ListOfContrasts.end());
        if (ListOfContrasts.size() % 2 == 0)
            dMedian = (ListOfContrasts[ListOfContrasts.size() / 2.0 - 1.0] + ListOfContrasts[ListOfContrasts.size() / 2.0]) / 2.0;
        else
            dMedian = ListOfContrasts[ListOfContrasts.size() / 2.0 - 1.0];

        // Compute contrast
        int nNbList = 0;
        for (unsigned int nContr = 0; nContr < ListOfContrasts.size(); nContr++)
        {
            if (ListOfContrasts[nContr] >= dMedian - 0.2 && ListOfContrasts[nContr] <= dMedian + 0.2)
            {
                dQuality += ListOfContrasts[nContr];
                nNbList++;
            }
        }
        dQuality /= nNbList;
    }

    delete[] pClusterList;

    return 100.0 * dQuality;	// Percentage
}

/*	std::ofstream TempStream("C:\\Users\\i.bergoend\\Documents\\Temp\\TempPatternMask.bin", std::ofstream::out | std::ofstream::binary);
    TempStream.write((char*)pPatternsMask, nHSize*nVSize * sizeof(char));
    TempStream.close();
    */