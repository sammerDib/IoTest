// The following ifdef block is the standard way of creating macros which make exporting
// from a DLL simpler. All files within this DLL are compiled with the FOCUSQUALITY_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see
// FOCUSQUALITY_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef FOCUSQUALITY_EXPORTS
#define FOCUSQUALITY_API __declspec(dllexport)
#else
#define FOCUSQUALITY_API __declspec(dllimport)
#endif

#include "WaferSubAreas.h"

struct sSubImProp {
    unsigned int nNum;	// Sub-image ID. For 5 areas: 0: center, 1: top, 2: right, 3: bottom, 4: left.
    unsigned int nPixStartV, nPixStartH;	// First pixel of the sub image (coordinates taken in the full image)
    unsigned int nSizeV, nSizeH;	// Size of sub-image in pixels
    double dFocusQuality;			// Result of focus quality for this sub-image
};

// This class is exported from the dll
class FOCUSQUALITY_API CFocusQuality {
public:
    CFocusQuality(void);
    // TODO: add your methods here.
};

extern FOCUSQUALITY_API int nFocusQuality;

FOCUSQUALITY_API int fnFocusQuality(void);

extern "C" __declspec(dllexport) int GetFocusQuality(sSubImProp * pSubImages[]); // For test!

extern "C" __declspec(dllexport) int FocusQuality(unsigned char* pIm, unsigned int nSizeH, unsigned int nSizeV, double dWaferSize, double dPatternSize, unsigned int nSubImPositions, sSubImProp pSubImages[]);
// pSubImages is a table of 5 elements allocated outside the function.
// Returned error: 0 = OK, 1 = table of sSubImProp not allocated, 2 = wafer not detected, 3: problem of position or size of a sub-image
// strSubImPositions describes the possible repartition of the focus areas: 5 areas on the whole wafer, or 3 areas in "L" shape. Allowed values: "TopBottomLeftRightCenter", "LBottomLeft" for L shape with the corner on bottom left |_, "LBottomRight" _|, "LTopRight" ��|, "LTopLeft" |��, "Unknown".

class CFocusQualityComputation : public CWaferSubAreas
{
protected:
    double m_dExpectedPatternSize; // In pixels

public:
    CFocusQualityComputation(OpticalMountShape strSubImPositions);
    CFocusQualityComputation();
    CFocusQualityComputation(unsigned int nImSizeH, unsigned int nImSizeV, OpticalMountShape strSubImPositions);
    ~CFocusQualityComputation();

    double ComputeFocusQuality(unsigned char* pIm, const unsigned int nID) const;   // nID : identifier of the sub-area (0 to 4)
    unsigned int GetSubImagesNb() const { return m_nSubImagesNb; }
    void SetExpectedPatternSizePix(const double dExpectedPatternSizePix) {
        m_dExpectedPatternSize = dExpectedPatternSizePix;
    }

protected:
    void MakeImageHisto(const unsigned char* pMask, unsigned int* pHisto) const;	// On 255 gray levels
    void Erosion(const unsigned char* pIm, unsigned char* pOutIm, const unsigned int nKernelHalfSize) const;
    double ClustersContrast(const unsigned char* pSubIm, unsigned char* pErodedMask) const;
};

void Dilation(const unsigned char* pIm, unsigned char* pOutIm, const unsigned int nImSizeV, const unsigned int nImSizeH, const unsigned int nKernelHalfSize);
//double StandardDeviationOfMaskedImage(const unsigned char* pIm, unsigned char* pMask, const unsigned int nImSizeV, const unsigned int nImSizeH, const double dSubImAverage);

//int FocusQuality(const unsigned char* pIm, const unsigned int nSizeV, const unsigned int nSizeH, const double dExpectedPatternSizePix, sSubImProp* pSubImages);