#include "stdafx.h"
#include "H3UnwrapPhase.h"
#include "H3UnwrapPhaseDecl.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

long Unwrap4(unsigned short *pDest, unsigned char *pSrc, unsigned char *pMask,
			 long nLi,long nCo,
             long yorg,long xorg,
			 long phasestep,long phasenoise,long matsize,long MinFiability);

static CString strModule(_T("UnwrapPhase"));

#define fPI		(3.14159265358979F)//#define PI	3.1415926F
#define f2PI	(6.28318530717959F)//cv:NB PI et TWO_PI sont defini ailleurs

extern "C++" H3UNWRAPPHASE_EXPORT_DECL H3_ARRAY2D_UINT16
H3UnwrapPhaseOrder(
	const H3_ARRAY2D_UINT8 &SrcBuf,
	const H3_ARRAY2D_UINT8 &MaskBuf,
	long nLi,
	long nCo,
	long nQuality)
{
	// Ligne de code obligatoire pour les MFC DLL
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction(_T("H3UnwrapPhaseOrder"));
	CString strMsg(_T("Impossible de démoduler la phase."));

	// Verifications
	if (nLi<0 || nLi>SrcBuf.GetLi() ||
		nCo<0 || nCo>SrcBuf.GetCo())
	{
		H3DisplayError(strModule,strFunction,strMsg+_T("\nPoint de départ invalide."));
		return H3_ARRAY2D_INT16(0,0);
	}

	if (SrcBuf.GetSize() != MaskBuf.GetSize())
	{
		H3DisplayError(strModule,strFunction,strMsg+_T("\nDimensions SrcBuf,MaskBuf invalides."));
		return H3_ARRAY2D_INT16(0,0);
	}

	long phasestep,phasenoise,MinFiability,matsize;
	switch (nQuality) 
	{
	// Images de speckle
	case 0:
		phasestep=140;phasenoise=60;matsize=9;MinFiability=50;
		break;
	case 1:
		phasestep=150;phasenoise=60;matsize=9;MinFiability=50;
		break;
	case 2:
		phasestep=200;phasenoise=20;matsize=9;MinFiability=50;
		break;
	// Images de phase FFT ou fortement filtrees avec grosses ouvertures de franges
	case 5: // phase tres filtree et 2PI de phase code sur 50 pixels au moins
		// ouverture de frange detectable 17 pixels
		phasestep=250;phasenoise=5;matsize=17;MinFiability=95;
		break;
	case 4: // phase tres filtree et 2PI de phase code sur 25 pixels au moins
		// ouverture de frange detectable 17 pixels
		phasestep=245;phasenoise=10;matsize=17;MinFiability=95;
		break;
	case 3:	// phase tres filtree et 2PI de phase code sur 10 pixels au moins
		// ouverture de frange detectable 17 pixels
//		phasestep=230;phasenoise=25;matsize=17;MinFiability=20;
		phasestep=230;phasenoise=25;matsize=17;MinFiability=95;
		break;
	case 6:	// phase tres filtree et 2PI de phase code sur 5 pixels au moins
		// ouverture de frange detectable 17 pixels
		phasestep=200;phasenoise=50;matsize=17;MinFiability=95;
		break;
	case 7:// phase tres filtree et 2PI de phase code sur 3 pixels au moins
		// ouverture de frange detectable 17 pixels
		phasestep=150;phasenoise=100;matsize=17;MinFiability=95;
		break;
	default:
		H3DisplayError(strModule,strFunction,strMsg+_T("\nParamètre nQuality invalide."));
		return H3_ARRAY2D_INT16(0,0);
		break;
	}

////////////////////////////////////////////////////////////////////////////////////
// Remarque EC 22/10/01 : Etant donne que la demodulation native ne se fait pas
// jusqu'au bord, on agrandi l'image. Le probleme est la consommation memoire
// ainsi que la perte de temps ....
////////////////////////////////////////////////////////////////////////////////////
	long ms=matsize+2;
	H3_ARRAY2D_UINT8 SrcBuf2(SrcBuf.GetLi()+ms*2,SrcBuf.GetCo()+ms*2);
	H3_ARRAY2D_UINT8 MaskBuf2(SrcBuf.GetLi()+ms*2,SrcBuf.GetCo()+ms*2);
	H3_ARRAY2D_UINT16 DestBuf2(SrcBuf.GetLi()+ms*2,SrcBuf.GetCo()+ms*2);
	SrcBuf2.SetAt(ms+1,ms+1,SrcBuf);
	MaskBuf2.SetAt(ms+1,ms+1,MaskBuf);

	if (!SrcBuf2.IsEmpty() && 
		!MaskBuf2.IsEmpty() &&
		!DestBuf2.IsEmpty())
	{
		// demodulation hierarchique
		Unwrap4((unsigned short *)DestBuf2.GetData(),
				(unsigned char *)SrcBuf2.GetData(),
				(unsigned char *)MaskBuf2.GetData(),
				SrcBuf2.GetLi(),
				SrcBuf2.GetCo(),
				nLi+ms+1,nCo+ms+1,
				phasestep,phasenoise,matsize,MinFiability);

		return DestBuf2.GetAt(ms+1,ms+1,SrcBuf.GetLi(),SrcBuf.GetCo());
	}
	else
	{
		H3DisplayError(strModule,strFunction,strMsg+_T("\nSrcBuf2 ou MaskBuf2 est NULL."));
	}

	return H3_ARRAY2D_UINT16(0,0);
}

// Demodulation de la phase codee modulo 256 SrcBuf
extern "C++" H3UNWRAPPHASE_EXPORT_DECL H3_ARRAY2D_FLT32
H3UnwrapPhase(
	const H3_ARRAY2D_FLT32 &SrcBuf,
	long nStartLi,
	long nStartCo,
	long nQuality)
{
	// Ligne de code obligatoire pour les MFC DLL
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	H3_ARRAY2D_UINT8 MaskBuf(SrcBuf.GetLi(),SrcBuf.GetCo());
	MaskBuf.Fill(1);

	return H3UnwrapPhase(SrcBuf,MaskBuf,nStartLi,nStartCo,nQuality);
}

extern "C++" H3UNWRAPPHASE_EXPORT_DECL H3_ARRAY2D_FLT32
H3UnwrapPhase(
	const H3_ARRAY2D_FLT32 &SrcBuf,
	const H3_ARRAY2D_UINT8 &MaskBuf,
	long nStartLi,
	long nStartCo,
	long nQuality)
{
	// Ligne de code obligatoire pour les MFC DLL
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	H3_ARRAY2D_FLT32 DestBuf(SrcBuf.GetLi(),SrcBuf.GetCo());
	if (!H3UnwrapPhase(DestBuf,SrcBuf,MaskBuf,nStartLi,nStartCo,nQuality))
	{
		return H3_ARRAY2D_FLT32(0,0);
	}

	return DestBuf;
}

extern "C++" H3UNWRAPPHASE_EXPORT_DECL bool
H3UnwrapPhase(
	H3_ARRAY2D_FLT32 &DestBuf,
	const H3_ARRAY2D_FLT32 &SrcBuf,
	const H3_POINT2D_INT32 &pt,
	long nQuality)
{
	// Ligne de code obligatoire pour les MFC DLL
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction(_T("H3UnwrapPhase"));
	CString strMsg(_T("Impossible de démoduler la phase."));

	DestBuf.ReAlloc(SrcBuf.GetLi(),SrcBuf.GetCo());

	// Verification des parametres
	if (SrcBuf.IsEmpty())
	{
		H3DisplayError(strModule,strFunction,strMsg+_T("\nSrcBuf est vide."));
		return false;
	}

	if (pt.y<0 || pt.y>=SrcBuf.GetLi() || 
		pt.x<0 || pt.x>=SrcBuf.GetCo())
	{
		H3DisplayError(strModule,strFunction,strMsg+_T("\nPoint de départ invalide."));
		return false;
	}

	if (nQuality<0 || nQuality>7)
	{
		H3DisplayError(strModule,strFunction,strMsg+_T("\nParamètre 'nQuality' invalide."));
		return false;
	}

	// Preparer un nouveau masque n'incluant pas les eventuelles valeurs NAN de la source
	H3_ARRAY2D_UINT8 MaskBuf2(SrcBuf.GetLi(),SrcBuf.GetCo());
	H3_UINT8 *pMask2=MaskBuf2.GetData();
	H3_FLT32 *pSrc=SrcBuf.GetData();
	size_t size=SrcBuf.GetSize();
	for (size_t n = 0;  n < size; n++)
	{
		if (_isnan(*pSrc))
			*pMask2=0;
		else 
			*pMask2=1;
		
		pSrc++;pMask2++;
	}

	// Demodulation proprement dite
	H3_ARRAY2D_UINT8 SrcInt(SrcBuf.GetLi(),SrcBuf.GetCo());
	pSrc=SrcBuf.GetData();
	pMask2=MaskBuf2.GetData();
	unsigned __int8 *pSrcInt=SrcInt.GetData();
	size=SrcBuf.GetSize();
	float flt;
	const float factor=128.0F/fPI;

	for (size_t n = 0;  n < size; n++)
	{
		if(*pMask2){
			flt=floor( ((*pSrc)+fPI)*factor );
			if(flt>255){//il arrive que l'on ait 256, mais pas plus
				flt-=256;
				(*pSrc)-=(fTWO_PI);
			}
			(*pSrcInt)=flt;//unsigned __int8(flt);
		}
		
		pSrc++;pSrcInt++;pMask2++;
	}

	H3_ARRAY2D_UINT16 TmpBuf=H3UnwrapPhaseOrder(
		SrcInt,
		MaskBuf2,
		pt.y,pt.x,
		nQuality);

	H3_FLT32 FPNaN=H3GetFPNaN();

	size=DestBuf.GetSize();
	H3_FLT32 *pDest=DestBuf.GetData();
	H3_UINT16 *pTmp=TmpBuf.GetData();

	H3_UINT16 orderOffset=TmpBuf(pt.y,pt.x);
	float forderOffset=(float)orderOffset;
	pSrc=SrcBuf.GetData();

	for (size_t n = 0;  n < size; n++)
	{
		if (*pTmp)
			*pDest = ((float)(*pTmp)-forderOffset) *  f2PI + (*pSrc);
		else
			*pDest = FPNaN;
		pDest++;pSrc++;pTmp++;
	}

	DestBuf-=DestBuf(pt.y,pt.x);

	return true;
}

extern "C++" H3UNWRAPPHASE_EXPORT_DECL bool
H3UnwrapPhase(
	H3_ARRAY2D_FLT32 &DestBuf,
	const H3_ARRAY2D_FLT32 &SrcBuf,
	const H3_ARRAY2D_UINT8 &MaskBuf,
	long nStartLi,
	long nStartCo,
	long nQuality)
{

	// Ligne de code obligatoire pour les MFC DLL
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction(_T("H3UnwrapPhase"));
	CString strMsg(_T("Impossible de démoduler la phase."));

	DestBuf.ReAlloc(SrcBuf.GetLi(),SrcBuf.GetCo());

	// Verification des parametres
	if (SrcBuf.IsEmpty() || MaskBuf.IsEmpty())
	{
		H3DisplayError(strModule,strFunction,strMsg+_T("\nSrcBuf ou MaskBuf est vide."));
		return false;
	}

	if (SrcBuf.GetLi()!=MaskBuf.GetLi() ||
		SrcBuf.GetCo()!=MaskBuf.GetCo() )
	{
		H3DisplayError(strModule,strFunction,strMsg+_T("\nDimensions SrcBuf,MaskBuf invalides."));
		return false;
	}

	if (nStartLi>=SrcBuf.GetLi() || nStartCo>=SrcBuf.GetCo())
	{
		H3DisplayError(strModule,strFunction,strMsg+_T("\nCoordonnées [nStartLi,nStartCo] invalides."));
		return false;
	}

	if (nQuality<0 || nQuality>7)
	{
		H3DisplayError(strModule,strFunction,strMsg+_T("\nParamètre nQuality invalide."));
		return false;
	}

	// Preparer un nouveau masque n'incluant pas les eventuelles valeurs NAN de la source
	H3_ARRAY2D_UINT8 MaskBuf2(MaskBuf);
	H3_UINT8 *pMask2=MaskBuf2.GetData();
	H3_FLT32 *pSrc=SrcBuf.GetData();
	size_t size=SrcBuf.GetSize();
	for (size_t n = 0;  n < size; n++)
	{
		if (_isnan(*pSrc))
		{
			*pMask2=0;
		}
		pSrc++;pMask2++;
	}

	// Demodulation proprement dite
	H3_ARRAY2D_UINT8 SrcInt(SrcBuf.GetLi(),SrcBuf.GetCo());
	pSrc=SrcBuf.GetData();
	pMask2=MaskBuf2.GetData();
	unsigned __int8 *pSrcInt=SrcInt.GetData();
	size=DestBuf.GetSize();
	float flt;
	const float factor=128.0F/fPI;

	for (size_t n = 0;  n < size; n++)
	{
		if(*pMask2){
			flt=floor( ((*pSrc)+fPI)*factor );
			if(flt>255){//il arrive que l'on ait 256, mais pas plus
				flt-=256;
				(*pSrc)-=(fTWO_PI);
			}
			(*pSrcInt)=flt;//unsigned __int8(flt);
		}
		
		pSrc++;pSrcInt++;pMask2++;
	}

	H3_ARRAY2D_UINT16 TmpBuf=H3UnwrapPhaseOrder(
		SrcInt,
		MaskBuf2,
		nStartLi,nStartCo,
		nQuality);

	H3_FLT32 FPNaN=H3GetFPNaN();

	size=DestBuf.GetSize();
	H3_FLT32 *pDest=DestBuf.GetData();
	H3_UINT16 *pTmp=TmpBuf.GetData();
	pSrc=SrcBuf.GetData();

	H3_UINT16 orderOffset=TmpBuf(nStartLi,nStartCo);
	float forderOffset=(float)orderOffset;

	for (size_t n = 0;  n < size; n++)
	{
		if (*pTmp)
			*pDest = ((float)(*pTmp)-forderOffset)  *  f2PI + (*pSrc);
		else
			*pDest = FPNaN;
		pDest++;pSrc++;pTmp++;
	}

	DestBuf-=DestBuf(nStartLi,nStartCo);

	return true;
}

////////////////////////////////////////////////////////////////////////////////////////

extern "C" H3UNWRAPPHASE_EXPORT_DECL long
H3UnwrapPhaseOrder8(
	unsigned short *pDest,
	const unsigned char *pSrc,
	const unsigned char *pMask,
	long nLi,
	long nCo,
	long nStartLi,
	long nStartCo,
	long nQuality)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction(_T("H3UnwrapPhaseOrder8"));
	CString strMsg(_T("Impossible de démoduler la phase."));

	if (pDest && pSrc && pMask)
	{
		if (nCo>0 && nLi>0)
		{
			if (nStartCo<nCo && nStartLi<nLi)
			{
				// Copier dans des buffers H3
				H3_ARRAY2D_UINT8 SrcBuf(nLi,nCo);
				H3_ARRAY2D_UINT8 MaskBuf(nLi,nCo);
				{
					size_t size=SrcBuf.GetSize();
					H3_UINT8 *pSrcBuf=SrcBuf.GetData();
					H3_UINT8 *pMaskBuf=MaskBuf.GetData();
					const unsigned char *pS=pSrc;
					const unsigned char *pM=pMask;
					for (size_t n = 0;  n < size; n++)
					{
						*pSrcBuf++=*pS++;
						*pMaskBuf++=*pM++;
					}
				}

				// Demodulation
				H3_ARRAY2D_UINT16 DestBuf=H3UnwrapPhaseOrder(
					SrcBuf,MaskBuf,nStartLi,nStartCo,nQuality);

				// Copier resultat
				if (!DestBuf.IsEmpty())
				{
					size_t size=DestBuf.GetSize();
					H3_UINT16 *pDestBuf=DestBuf.GetData();
					unsigned short *pD=pDest;
					for (size_t n = 0;  n < size; n++)
					{
						*pD++ = *pDestBuf++;
					}
					return 0;
				}
				else
				{
					H3DisplayError(strModule,strFunction,strMsg+_T("\nDestBuf est vide."));
				}
			}
			else
			{
				H3DisplayError(strModule,strFunction,strMsg+_T("\nCoordonnées [nStartLi,nStartCo] invalides."));
			}
		}
		else
		{
			H3DisplayError(strModule,strFunction,strMsg+_T("\nDimensions [nLi,nCo] invalides."));
		}
	}
	else
	{
		H3DisplayError(strModule,strFunction,strMsg+_T("\npDest ou pSrc ou pMask est NULL."));
	}

	return -1;
}