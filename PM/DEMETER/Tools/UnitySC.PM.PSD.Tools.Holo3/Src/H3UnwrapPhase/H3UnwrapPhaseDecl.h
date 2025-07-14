#if !defined(UNWRAPPHASE_H__INCLUDED_)
#define UNWRAPPHASE_H__INCLUDED_

#ifdef _DLL
#ifdef AFX_H3UNWRAPPHASE_H__50502A69_1BC4_431D_86C4_7BCA4B7AB607__INCLUDED_
#    define H3UNWRAPPHASE_EXPORT_DECL __declspec(dllexport)
#  else
#    define H3UNWRAPPHASE_EXPORT_DECL __declspec(dllimport)
#  endif
#else
#  define H3UNWRAPPHASE_EXPORT_DECL
#endif

/////////////////////////////////////////////////////////////////////////////
// Declaration des fonctions utilisables en C ou C++
#ifdef __cplusplus
	extern "C" {
#endif


long H3UNWRAPPHASE_EXPORT_DECL
H3UnwrapPhaseOrder8(unsigned short *pDest,
				const unsigned char *pSrc,
				const unsigned char *pMask,
				long nLi,
				long nCo,
				long nStartLi,
				long nStartCo,
				long nQuality);

#ifdef __cplusplus
	}
#endif

/////////////////////////////////////////////////////////////////////////////
// Declaration des fonctions en C++ avec la classe HOLO3 CH3ARRAY2D
#ifdef __cplusplus
	#ifdef CH3ARRAY2D__INCLUDED_
		extern "C++" {
			bool H3UNWRAPPHASE_EXPORT_DECL H3UnwrapPhase(
				H3_ARRAY2D_FLT32 &DestBuf,
				const H3_ARRAY2D_FLT32 &SrcBuf,
				const H3_POINT2D_INT32 &pt,
				long nQuality);

			bool H3UNWRAPPHASE_EXPORT_DECL H3UnwrapPhase(
				H3_ARRAY2D_FLT32 &DestBuf,
				const H3_ARRAY2D_FLT32 &SrcBuf,
				const H3_ARRAY2D_UINT8 &MaskBuf,
				long nStartLi,
				long nStartCo,
				long nQuality);

			H3_ARRAY2D_FLT32 H3UNWRAPPHASE_EXPORT_DECL H3UnwrapPhase(
				const H3_ARRAY2D_FLT32 &SrcBuf,
				long nStartLi,
				long nStartCo,
				long nQuality);

			H3_ARRAY2D_FLT32 H3UNWRAPPHASE_EXPORT_DECL H3UnwrapPhase(
				const H3_ARRAY2D_FLT32 &SrcBuf,
				const H3_ARRAY2D_UINT8 &MaskBuf,
				long nStartLi,
				long nStartCo,
				long nQuality);

			H3_ARRAY2D_UINT16 H3UNWRAPPHASE_EXPORT_DECL H3UnwrapPhaseOrder(
				const H3_ARRAY2D_UINT8 &SrcBuf,
				const H3_ARRAY2D_UINT8 &MaskBuf,
				long nLi,
				long nCo,
				long nQuality);
		}
	#endif
#endif

#endif




