#pragma once

#include <vector>
#include <string>


enum OpticalMountShape : unsigned int {
    Cross = 0,
    LTopLeft = 1,
    LTopRight = 2,
    LBottomLeft = 3,
    LBottomRight = 4,
    SquarePlusCenter = 5
};

// Class that defines the wafer position in image and sub-areas useful for several calculations in adjustment/calibration section.
// Several specific classes inherit this one.

class CWaferSubAreas
{
protected:
    unsigned int m_nVStart, m_nVEnd, m_nHStart, m_nHEnd;    // Vertical and horizontal limits of the wafer in image, in pixels
    unsigned int m_nImSizeV, m_nImSizeH;    // Initial image dimensions in pixels
    unsigned int m_nSubSizeH, m_nSubSizeV;	// Size of sub-image in pixels
    OpticalMountShape m_opticalMountShape;    // Values can be: "TopBottomLeftRightCenter", "LBottomLeft" for L shape with the corner on bottom left, "LBottomRight", "LTopRight", "LTopLeft", "SquarePlusCenter", "Unknown"
    unsigned int m_nSubImagesNb; // Currently, allowed values are 3 and 5.

    struct sSubImCoord {
        unsigned int nNum;	// Sub-image ID. 0: center, 1: top, 2: right, 3: bottom, 4: left.
        unsigned int nPixStartV, nPixStartH;	// First pixel of the sub image (coordinates taken in the full image)
    };

    std::vector<sSubImCoord> m_vSubIm;

public:
    CWaferSubAreas(OpticalMountShape opticalMount);
    CWaferSubAreas();
    ~CWaferSubAreas();

    unsigned int GetWaferVStart() const { return m_nVStart; }
    unsigned int GetWaferVEnd() const { return m_nVEnd; }
    unsigned int GetWaferHStart() const { return m_nHStart; }
    unsigned int GetWaferHEnd() const { return m_nHEnd; }
    unsigned int GetSubImageVStart(const unsigned int nID) const { return m_vSubIm[nID].nPixStartV; }
    unsigned int GetSubImageHStart(const unsigned int nID) const { return m_vSubIm[nID].nPixStartH; }
    unsigned int GetSubImageVSize() const { return m_nSubSizeV; }
    unsigned int GetSubImageHSize() const { return m_nSubSizeH; }

public:
    bool FindWaferPosition(const unsigned char* pIm);
    unsigned int ComputeSubImages();
    bool GetSubImage(const unsigned char* pIm, const unsigned int nID, unsigned char* pSubIm) const;	// Makes a copy

protected:
    // Tools for above functions:
    double Average(const unsigned char* pIm, const unsigned int nVImSize, const unsigned int nHImSize, const unsigned int nVStartComputingPos, const unsigned int nHStartComputingPos) const;
    void MakeHistoOnRows(const unsigned char* pMask, const unsigned int nVImSize, const unsigned int nHImSize, unsigned int* pHisto, unsigned int& nStripeWidth);
    void MakeHistoOnCol(const unsigned char* pMask, const unsigned int nVImSize, const unsigned int nHImSize, unsigned int* pHisto, unsigned int& nStripeHeight);
};