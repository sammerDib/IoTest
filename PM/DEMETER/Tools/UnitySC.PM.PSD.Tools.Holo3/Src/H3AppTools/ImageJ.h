#include "H3Array2D.h"

#ifdef _DLL
#ifdef IMAGE_J
#define IMAGE_JDECL __declspec(dllexport)
#else
#define IMAGE_JDECL __declspec(dllimport)
#endif
#else
#define IMAGE_JDECL
#endif


// Fonction pour le debug

IMAGE_JDECL long long ImageJMilSystemID;
IMAGE_JDECL int ImageJSizeX, ImageJSizeY;
IMAGE_JDECL int ImageJ(const void* data, int SizeX, int SizeY, int type, bool isfloat);
IMAGE_JDECL int ImageJ(const BYTE* data, int SizeX, int SizeY);
IMAGE_JDECL int ImageJ(const WORD* data, int SizeX, int SizeY);
IMAGE_JDECL int ImageJ(const float* data, int SizeX, int SizeY);
IMAGE_JDECL int ImageJ(const BYTE* data);
IMAGE_JDECL int ImageJ(const WORD* data);
IMAGE_JDECL int ImageJ(const float* data);

template <class TYPE>
IMAGE_JDECL int ImageJ(const CH3Array2D<TYPE>& img);

IMAGE_JDECL int ImageJ(const CH3Array2D<H3_FLT32>& img);
IMAGE_JDECL int ImageJ(const CH3Array2D<H3_UINT16>& img);
IMAGE_JDECL int ImageJ(const CH3Array2D<H3_UINT8>& img);
IMAGE_JDECL int ImageJ(long milid);