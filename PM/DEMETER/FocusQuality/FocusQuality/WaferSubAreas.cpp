#include "pch.h"
#include "framework.h"
#include "WaferSubAreas.h"
#include <cmath>
#include <ppl.h>
#include <vector>
#include <ios>
#include <iostream>
#include <fstream>
using namespace Concurrency;


CWaferSubAreas::CWaferSubAreas(OpticalMountShape opticalMount)
{
    // Full image size:
    m_nImSizeH = 0;
    m_nImSizeV = 0;

    m_nVStart = 0;
    m_nVEnd = 0;
    m_nHStart = 0;
    m_nHEnd = 0;

    m_opticalMountShape = opticalMount;
    if (OpticalMountShape::Cross == opticalMount || OpticalMountShape::SquarePlusCenter == opticalMount)
        m_nSubImagesNb = 5;
    else if (OpticalMountShape::LBottomLeft == opticalMount || OpticalMountShape::LBottomRight == opticalMount || OpticalMountShape::LTopLeft == opticalMount || OpticalMountShape::LTopRight == opticalMount)
        m_nSubImagesNb = 3;
    else
        m_nSubImagesNb = 0;

}


CWaferSubAreas::CWaferSubAreas()
{
    // Full image size:
    m_nImSizeH = 0;
    m_nImSizeV = 0;

    m_nVStart = 0;
    m_nVEnd = 0;
    m_nHStart = 0;
    m_nHEnd = 0;

    m_nSubImagesNb = 0;
}


CWaferSubAreas::~CWaferSubAreas()
{

}



// Finds the limit coordinates of the wafer area. Also finds the typical pattern size.
bool CWaferSubAreas::FindWaferPosition(const unsigned char* pIm)
{
    // Average in a center area
    unsigned int nVAverageStart = (unsigned int)(m_nImSizeV / 2 - 0.2 * m_nImSizeV);
    unsigned int nHAverageStart = (unsigned int)(m_nImSizeH / 2 - 0.2 * m_nImSizeH);
    double dAverage = Average(pIm, m_nImSizeV, m_nImSizeH, nVAverageStart, nHAverageStart);

    // Data in mask. The mask size is divided by 2x2 to save computation time.
    double dThreshold = 0.15 * dAverage;
    unsigned int nHalfSizeH = m_nImSizeH / 2;
    unsigned int nHalfSizeV = m_nImSizeV / 2;
    unsigned char* pWaferMask = new unsigned char[nHalfSizeH * nHalfSizeV];

    for (unsigned int nV = 0; nV < nHalfSizeV; nV++)
    {
        for (unsigned int nH = 0; nH < nHalfSizeH; nH++)
        {
            if (pIm[(nV * 2) * m_nImSizeH + nH * 2] > dThreshold)
                pWaferMask[nV * nHalfSizeH + nH] = true;
            else
                pWaferMask[nV * nHalfSizeH + nH] = false;
        }
    }

    // Histograms of rows and columns
    unsigned int* pHistoRow = new unsigned int[nHalfSizeV];
    unsigned int* pHistoCol = new unsigned int[nHalfSizeH];

    // On rows
    unsigned int nStripeWidth, nStripeHeight;
    MakeHistoOnRows(pWaferMask, nHalfSizeV, nHalfSizeH, pHistoRow, nStripeWidth);

    // On columns
    MakeHistoOnCol(pWaferMask, nHalfSizeV, nHalfSizeH, pHistoCol, nStripeHeight);

    delete[] pWaferMask;

    // Detecting indices of wafer limits, starting from the center
    unsigned int nInd = (unsigned int)(nHalfSizeV * 0.5);
    unsigned int nLimit = (unsigned int)(0.8 * nStripeWidth);

    // Vertical
    while (nInd > 0 && pHistoRow[nInd] > nLimit)
    {
        nInd--;
    }
    m_nVStart = nInd * 2;

    nInd = (unsigned int)(nHalfSizeV * 0.5);
    while (nInd < nHalfSizeV && pHistoRow[nInd - 1] > nLimit)
    {
        nInd++;
    }
    m_nVEnd = nInd * 2;

    // Horizontal
    nLimit = (unsigned int)(0.8 * nStripeHeight);
    nInd = (unsigned int)(nHalfSizeH * 0.5);

    while (nInd > 0 && pHistoCol[nInd] > nLimit)
    {
        nInd--;
    }
    m_nHStart = nInd * 2;

    nInd = (unsigned int)(nHalfSizeH * 0.5);
    while (nInd < nHalfSizeH && pHistoCol[nInd - 1] > nLimit)
    {
        nInd++;
    }
    m_nHEnd = nInd * 2;

    delete[] pHistoRow;
    delete[] pHistoCol;

    // Checking the wafer was found
    if (m_nVEnd - m_nVStart < 10 || m_nHEnd - m_nHStart < 10)
        return false;
    else
        return true;
}


// Average value on an area starting at position nVStartComputingPos,nHStartComputingPos in an image pIm of total size nVImSize,nHImSize.
double CWaferSubAreas::Average(const unsigned char* pIm, const unsigned int nVImSize, const unsigned int nHImSize, const unsigned int nVStartComputingPos, const unsigned int nHStartComputingPos) const
{
    double dAverage = 0.0;
    int iCount = 0;

    for (unsigned int nV = nVStartComputingPos; nV < nVImSize - nVStartComputingPos; nV += 2)
    {
        for (unsigned int nH = nHStartComputingPos; nH < nHImSize - nHStartComputingPos; nH += 2)
        {
            dAverage += pIm[nV * nHImSize + nH];
            iCount++;
        }
    }

    dAverage /= iCount;

    return dAverage;
}


void CWaferSubAreas::MakeHistoOnCol(const unsigned char* pMask, const unsigned int nVImSize, const unsigned int nHImSize, unsigned int* pHisto, unsigned int& nStripeHeight)
{
    unsigned int nStripeStart = (unsigned int)(nVImSize * 0.5 - nVImSize / 20);
    unsigned int nStripeEnd = (unsigned int)(nVImSize * 0.5 + nVImSize / 20);
    nStripeHeight = nStripeEnd - nStripeStart;

    // On columns
    for (unsigned int nH = 0; nH < nHImSize; nH++)
    {
        pHisto[nH] = 0;

        for (unsigned int nV = nStripeStart; nV < nStripeEnd; nV++)
        {
            pHisto[nH] += pMask[nV * nHImSize + nH];
        }
    }
}


void CWaferSubAreas::MakeHistoOnRows(const unsigned char* pMask, const unsigned int nVImSize, const unsigned int nHImSize, unsigned int* pHisto, unsigned int& nStripeWidth)
{
    unsigned int nStripeStart = (unsigned int)(nHImSize * 0.5 - nHImSize / 20);
    unsigned int nStripeEnd = (unsigned int)(nHImSize * 0.5 + nHImSize / 20);
    nStripeWidth = nStripeEnd - nStripeStart;

    // On rows
    for (unsigned int nV = 0; nV < nVImSize; nV++)
    {
        pHisto[nV] = 0;

        for (unsigned int nH = nStripeStart; nH < nStripeEnd; nH++)
        {
            pHisto[nV] += pMask[nV * nHImSize + nH];
        }
    }
}


unsigned int CWaferSubAreas::ComputeSubImages()
{
    // Wafer size
    unsigned int nWaferSizeV = m_nVEnd - m_nVStart;
    unsigned int nWaferSizeH = m_nHEnd - m_nHStart;

    // Size of sub-images (floor value):
    m_nSubSizeV = nWaferSizeV / 8;
    m_nSubSizeH = nWaferSizeH / 8;

    m_vSubIm.resize(m_nSubImagesNb);

    if (OpticalMountShape::Cross == m_opticalMountShape)
    {
        // Center sub-image
        m_vSubIm[0].nPixStartH = m_nHStart + (unsigned int)floor(7.0 / 16 * nWaferSizeH);
        m_vSubIm[0].nPixStartV = m_nVStart + (unsigned int)floor(7.0 / 16 * nWaferSizeV);
        m_vSubIm[0].nNum = 0;

        // Top sub-image
        m_vSubIm[1].nPixStartH = m_nHStart + (unsigned int)floor(7.0 / 16 * nWaferSizeH);
        m_vSubIm[1].nPixStartV = m_nVStart + (unsigned int)floor(1.0 / 16 * nWaferSizeV);
        m_vSubIm[1].nNum = 1;

        // Right sub-image
        m_vSubIm[2].nPixStartH = m_nHStart + (unsigned int)floor(13.0 / 16 * nWaferSizeH);
        m_vSubIm[2].nPixStartV = m_nVStart + (unsigned int)floor(7.0 / 16 * nWaferSizeV);
        m_vSubIm[2].nNum = 2;

        // Bottom sub-image
        m_vSubIm[3].nPixStartH = m_nHStart + (unsigned int)floor(7.0 / 16 * nWaferSizeH);
        m_vSubIm[3].nPixStartV = m_nVStart + (unsigned int)floor(13.0 / 16 * nWaferSizeV);
        m_vSubIm[3].nNum = 3;

        // Left sub-image
        m_vSubIm[4].nPixStartH = m_nHStart + (unsigned int)floor(1.0 / 16 * nWaferSizeH);
        m_vSubIm[4].nPixStartV = m_nVStart + (unsigned int)floor(7.0 / 16 * nWaferSizeV);
        m_vSubIm[4].nNum = 4;
    }
    else if (OpticalMountShape::LBottomLeft == m_opticalMountShape)
    {
        // Top left sub-image (top of the "L")
        m_vSubIm[0].nPixStartH = m_nHStart + (unsigned int)floor(2.5 / 16 * nWaferSizeH);
        m_vSubIm[0].nPixStartV = m_nVStart + (unsigned int)floor(2.5 / 16 * nWaferSizeV);
        m_vSubIm[0].nNum = 0;

        // Bottom left sub-image (corner of the "L")
        m_vSubIm[1].nPixStartH = m_nHStart + (unsigned int)floor(2.5 / 16 * nWaferSizeH);
        m_vSubIm[1].nPixStartV = m_nVStart + (unsigned int)floor(11.5 / 16 * nWaferSizeV);
        m_vSubIm[1].nNum = 1;

        // Bottom right sub-image (right end of the "L")
        m_vSubIm[2].nPixStartH = m_nHStart + (unsigned int)floor(11.5 / 16 * nWaferSizeH);
        m_vSubIm[2].nPixStartV = m_nVStart + (unsigned int)floor(11.5 / 16 * nWaferSizeV);
        m_vSubIm[2].nNum = 2;
    }
    else if (OpticalMountShape::LBottomRight == m_opticalMountShape)
    {
        // Bottom left sub-image
        m_vSubIm[0].nPixStartH = m_nHStart + (unsigned int)floor(2.5 / 16 * nWaferSizeH);
        m_vSubIm[0].nPixStartV = m_nVStart + (unsigned int)floor(11.5 / 16 * nWaferSizeV);
        m_vSubIm[0].nNum = 0;

        // Bottom right sub-image (corner)
        m_vSubIm[1].nPixStartH = m_nHStart + (unsigned int)floor(11.5 / 16 * nWaferSizeH);
        m_vSubIm[1].nPixStartV = m_nVStart + (unsigned int)floor(11.5 / 16 * nWaferSizeV);
        m_vSubIm[1].nNum = 1;

        // Top right sub-image
        m_vSubIm[2].nPixStartH = m_nHStart + (unsigned int)floor(11.5 / 16 * nWaferSizeH);
        m_vSubIm[2].nPixStartV = m_nVStart + (unsigned int)floor(2.5 / 16 * nWaferSizeV);
        m_vSubIm[2].nNum = 2;

    }
    else if (OpticalMountShape::LTopRight == m_opticalMountShape)
    {
        // Bottom right sub-image
        m_vSubIm[0].nPixStartH = m_nHStart + (unsigned int)floor(11.5 / 16 * nWaferSizeH);
        m_vSubIm[0].nPixStartV = m_nVStart + (unsigned int)floor(11.5 / 16 * nWaferSizeV);
        m_vSubIm[0].nNum = 0;

        // Top right sub-image (corner)
        m_vSubIm[1].nPixStartH = m_nHStart + (unsigned int)floor(11.5 / 16 * nWaferSizeH);
        m_vSubIm[1].nPixStartV = m_nVStart + (unsigned int)floor(2.5 / 16 * nWaferSizeV);
        m_vSubIm[1].nNum = 1;

        // Top left sub-image
        m_vSubIm[2].nPixStartH = m_nHStart + (unsigned int)floor(2.5 / 16 * nWaferSizeH);
        m_vSubIm[2].nPixStartV = m_nVStart + (unsigned int)floor(2.5 / 16 * nWaferSizeV);
        m_vSubIm[2].nNum = 2;
    }
    else if (OpticalMountShape::LTopLeft == m_opticalMountShape)
    {
        // Top right sub-image
        m_vSubIm[0].nPixStartH = m_nHStart + (unsigned int)floor(11.5 / 16 * nWaferSizeH);
        m_vSubIm[0].nPixStartV = m_nVStart + (unsigned int)floor(2.5 / 16 * nWaferSizeV);
        m_vSubIm[0].nNum = 0;

        // Top left sub-image (corner)
        m_vSubIm[1].nPixStartH = m_nHStart + (unsigned int)floor(2.5 / 16 * nWaferSizeH);
        m_vSubIm[1].nPixStartV = m_nVStart + (unsigned int)floor(11.5 / 16 * nWaferSizeV);
        m_vSubIm[1].nNum = 1;

        // Bottom left sub-image
        m_vSubIm[2].nPixStartH = m_nHStart + (unsigned int)floor(2.5 / 16 * nWaferSizeH);
        m_vSubIm[2].nPixStartV = m_nVStart + (unsigned int)floor(11.5 / 16 * nWaferSizeV);
        m_vSubIm[2].nNum = 2;
    }
    else if (OpticalMountShape::SquarePlusCenter == m_opticalMountShape)
    {
        // Center sub-image
        m_vSubIm[0].nPixStartH = m_nHStart + (unsigned int)floor(7.0 / 16 * nWaferSizeH);
        m_vSubIm[0].nPixStartV = m_nVStart + (unsigned int)floor(7.0 / 16 * nWaferSizeV);
        m_vSubIm[0].nNum = 0;

        // Top left sub-image
        m_vSubIm[1].nPixStartH = m_nHStart + (unsigned int)floor(2.5 / 16 * nWaferSizeH);
        m_vSubIm[1].nPixStartV = m_nVStart + (unsigned int)floor(2.5 / 16 * nWaferSizeV);
        m_vSubIm[1].nNum = 1;

        // Bottom left sub-image
        m_vSubIm[2].nPixStartH = m_nHStart + (unsigned int)floor(2.5 / 16 * nWaferSizeH);
        m_vSubIm[2].nPixStartV = m_nVStart + (unsigned int)floor(11.5 / 16 * nWaferSizeV);
        m_vSubIm[2].nNum = 2;

        // Bottom right sub-image
        m_vSubIm[3].nPixStartH = m_nHStart + (unsigned int)floor(11.5 / 16 * nWaferSizeH);
        m_vSubIm[3].nPixStartV = m_nVStart + (unsigned int)floor(11.5 / 16 * nWaferSizeV);
        m_vSubIm[3].nNum = 3;

        // Top right sub-image
        m_vSubIm[4].nPixStartH = m_nHStart + (unsigned int)floor(11.5 / 16 * nWaferSizeH);
        m_vSubIm[4].nPixStartV = m_nVStart + (unsigned int)floor(2.5 / 16 * nWaferSizeV);
        m_vSubIm[4].nNum = 4;


    }

    return m_nSubImagesNb;
}


// Getting the image data corresponding to sub-areas
bool CWaferSubAreas::GetSubImage(const unsigned char* pIm, const unsigned int nID, unsigned char* pSubIm) const
{
    if (pIm != nullptr && pSubIm != nullptr && m_vSubIm[nID].nPixStartV + m_nSubSizeV < m_nImSizeV && m_vSubIm[nID].nPixStartH + m_nSubSizeH < m_nImSizeH)	// Avoid crashes
    {
        for (unsigned int nV = 0; nV < m_nSubSizeV; nV++)
        {
            for (unsigned int nH = 0; nH < m_nSubSizeH; nH++)
            {
                pSubIm[nV * m_nSubSizeH + nH] = pIm[(m_vSubIm[nID].nPixStartV + nV) * m_nImSizeH + (m_vSubIm[nID].nPixStartH + nH)];
            }
        }

        return true;
    }

    // If problem in input data:
    __debugbreak();
    return false;
}