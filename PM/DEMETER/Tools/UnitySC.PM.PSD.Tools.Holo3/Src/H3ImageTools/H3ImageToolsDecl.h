// H3ImageToolsDecl.h: declarations
//
//////////////////////////////////////////////////////////////////////

#if !defined(H3IMAGE_TOOLS_DECL_H__INCLUDED_)
#define H3IMAGE_TOOLS_DECL_H__INCLUDED_

#ifdef _DLL
#  ifdef AFX_H3IMAGETOOLS_H__4E542AE6_FA41_406F_9889_096978E36A80__INCLUDED_
#    define H3IMAGETOOLS_EXP __declspec(dllexport)
#  else
#    define H3IMAGETOOLS_EXP __declspec(dllimport)
#  endif
#else
#  define H3IMAGETOOLS_EXP
#endif

#include "H3Array2D.h"

//////////////////////////////////////////////////////////////////////
// Definitions
#define H3_IN_RANGE				0x0000
#define H3_OUT_RANGE			0x0001
#define H3_GREATER				0x0002
#define H3_GREATER_OR_EQUAL		0x0003
#define H3_LESS					0x0004
#define H3_LESS_OR_EQUAL		0x0005
#define H3_EQUAL				0x0006
#define H3_NOT_EQUAL			0x0007

#define H3_BINARY				0xF080

#define H3_FLIP_HORIZONTAL 		0xF010
#define H3_FLIP_VERTICAL 		0xF011

#ifdef __cplusplus
extern "C++" {
#endif

// Interpolations
H3IMAGETOOLS_EXP bool H3PolySurf(H3_ARRAY2D_FLT32& DestBuf,H3_ARRAY2D_FLT32& A,unsigned long SizeX,unsigned long SizeY);
H3IMAGETOOLS_EXP bool H3BestFitSurf2(const H3_ARRAY2D_FLT32& Src,const H3_ARRAY2D_UINT8& SrcMask,H3_ARRAY2D_FLT64 & MatResSurf,const unsigned long FitOrder,const unsigned long MatVal,const H3_ARRAY2D_UINT8& MatCoef);
H3IMAGETOOLS_EXP bool H3BestFitSurf2(const H3_ARRAY2D_FLT32& Src,const H3_ARRAY2D_UINT8& SrcMask,H3_ARRAY2D_FLT64 & MatResSurf,const unsigned long FitOrder);

// E/S Fichiers
typedef struct {
	long nFileFormat;	// Format du fichier 1="tif",2="bmp",3="jpg", "4=avi"
	long nSizeX;		// dimension X en pixels
	long nSizeY;		// dimension Y en pixels
	long nSizeBit;		// profondeur d'un element en bits
	long nSizeBand;		// nombre de plans 1=monochrome, 3=RGB
	long nType;			// Type
	long nNumber;		// Nombre d'images
}H3_STD_IMAGE_FILE_INFO;

#ifdef __cplusplus
}
#endif


#endif