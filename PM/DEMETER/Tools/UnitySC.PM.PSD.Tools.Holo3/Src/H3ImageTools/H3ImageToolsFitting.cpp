// H3ImageToolsFitting.cpp - Fonctions de fitting
// Auteur : E.COLON
//

#include "stdafx.h"
#include "H3ImageTools.h"
#include "H3Matrix.h"
#include <math.h>
#include <ppl.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// Variables globales
static CString strModule("H3ImageToolsFitting");

/////////////////////////////////////////////////////////////////////////////
// Cette fonction trace la surface (polynome) z=f(x,y)
// A contient la liste des coefficients du polynome genere par H3MSPolySurf()
// A:	x0y0	x0y1	x0y2 ...
//		x1y0	x1y1	x1y2 ...
//		...	
// voir aussi : H3MSPolySurf()
// modif cv 161210
extern "C++" bool /*__declspec(dllexport)*/
H3PolySurf(
	H3_ARRAY2D_FLT32& DestBuf,		// Buffer destination
	H3_ARRAY2D_FLT32& A,			// Coefficient du polynome 
	unsigned long SizeX,						// Dimension X souhaitée pour DestBuf
	unsigned long SizeY)						// Dimension Y souhaitée pour DestBuf
{
	CString strFunction("H3PolySurf()");
	CString strMsg("Impossible de calculer le polynôme.");

	/////////////////////////////////////////////////////////////////////////
	// Verifications de routine
	if (A.IsEmpty())
	{
		H3DisplayError(strModule,strFunction,strMsg+_T("\nEchec A est vide."));
		return false;
	}

	/////////////////////////////////////////////////////////////////////////
	// Preparer le buffer resultat
	DestBuf.ReAlloc(SizeY,SizeX);
	if (DestBuf.IsEmpty())
	{
		H3DisplayError(strModule,strFunction,strMsg+_T("\nEchec à l'allocation de DestBuf."));
		return false;
	}

	/////////////////////////////////////////////////////////////////////////
	// Determiner l'ordre du polynome en fonction de A
	size_t FitOrderX=A.GetLi()-1;
	size_t FitOrderY=A.GetCo()-1;


	/////////////////////////////////////////////////////////////////////////
	// Calcul de la surface estimee a l'echelle 1
    concurrency::parallel_for((unsigned long)0, SizeY, [&](unsigned long y)
    //for (unsigned long y=0;y<SizeY;y++)
	{
		double Y=double(y);
        float* ptr = DestBuf.GetLine(y);
		for (unsigned long x=0;x<SizeX;x++) 
		{
			double X=double(x);
			double ze=0;
			
			double Xi=1.0;
			for (long i=0,k=0;i<=FitOrderX;i++)
			{
				double XiYj=Xi;
				for (long j=0;j<=FitOrderY;j++,k++)
				{
					ze+=XiYj*((A)[k]);
					XiYj*=Y;
				}
				Xi*=X;
			}
            *ptr++ = (float)ze;
		}
	}
    );
	return true;
}

extern "C++" bool /*__declspec(dllexport)*/
H3BestFitSurf2(const H3_ARRAY2D_FLT32& Src,
    const H3_ARRAY2D_UINT8& SrcMask,
    H3_ARRAY2D_FLT64 & MatResSurf,
    const unsigned long FitOrder, const unsigned long MatVal,
    const H3_ARRAY2D_UINT8& MatCoef)
{
    CString strFunction("H3BestFitSurf2()");
    CString str;

    size_t nSizeX = Src.GetCo();
    size_t nSizeY = Src.GetLi();

    unsigned long ValidElement = 0;
    long NumRow = 0;
    unsigned long i, j, k = 0;

    if ((nSizeX != SrcMask.GetCo()) || (nSizeY != SrcMask.GetLi()))
    {
        H3DisplayError(strModule, strFunction, "L'image et le masque n'ont pas la meme dimension");
        return false;
    }

    //Initialiser les pointeurs sur les données
    H3_FLT32 *pSource = Src.GetData();
    H3_UINT8 *pMask = SrcMask.GetData();

    // Determination du nombre d'éléments valide à partir du masque
    for (unsigned long li = 0L; li < nSizeY; li++) {
        for (unsigned long co = 0; co < nSizeX; co++) {
            if ((*pMask > 0) && (!_isnan(*pSource)))
                ValidElement++;
            pMask++; pSource++;
        }
    }

    if (ValidElement < MatVal)
    {
        str.Format("Manque d'elements valides (%d)", ValidElement);
        H3DebugError(strModule, strFunction, str);
        return false;
    }

    pMask = SrcMask.GetData();
    pSource = Src.GetData();

    // Creation et initialisation des matrices Y,XjYi,M,TM
    H3_MATRIX_FLT64 Mtmp(1, MatVal), MATRESSURF(MatVal, 1);
    H3_MATRIX_FLT64 PtxP(MatVal, MatVal), PtxZ(MatVal, 1);
    PtxP.Fill(0);
    PtxZ.Fill(0);

    H3_FLT64 Yi, XjYi;
    // Calcul des coefficients de la matrice MatResSurf
    for (size_t Ypix = 0L; Ypix < nSizeY; Ypix++) {
        for (size_t Xpix = 0L; Xpix < nSizeX; Xpix++) {
            if ((*pMask > 0L) && (!_isnan(*pSource)))
            {
                NumRow = 0L;
                Yi = 1.0;

                for (i = 0L; i <= FitOrder; i++) {

                    XjYi = Yi;
                    for (j = 0L; j <= FitOrder; j++)
                    {
                        if (MatCoef(i, j) == 1L) {
                            Mtmp[NumRow] = XjYi;
                            NumRow++;
                        }
                        XjYi *= Xpix;
                    }
                    Yi *= Ypix;
                }

                for (i = 0L; i < MatVal; i++)
                {
                    PtxP(i, i) += Mtmp[i] * Mtmp[i];
                    for (j = i + 1L; j < MatVal; j++)
                        PtxP(j, i) += Mtmp[j] * Mtmp[i];
                    PtxZ[i] += Mtmp[i] * (*pSource);
                }
            }

            pMask++; pSource++;
        }
    }

    for (i = 0L; i < MatVal; i++) {
        for (j = i + 1L; j < MatVal; j++)
            PtxP(i, j) = PtxP(j, i);
    }

    H3_MATRIX_FLT64 iPtxP_tmp = PtxP.Inv2();

    if (!iPtxP_tmp.IsEmpty())
    {
        H3_MATRIX_FLT64 resTmp(iPtxP_tmp*PtxZ);

        MATRESSURF = resTmp;
    }

    if (MATRESSURF.GetSize() != MatVal)
    {
        CString msg;
        msg.Format("echec de la recherche du polynome d'ordre %d à partir de %d elements valides", FitOrder, ValidElement);
        H3DebugError(strModule, strFunction, msg);
        return false;
    }

    k = 0L;
    if (MatResSurf.GetSize() != (FitOrder + 1)*(FitOrder + 1))
        MatResSurf.ReAlloc(FitOrder + 1, FitOrder + 1);

    for (i = 0L; i <= FitOrder; i++) {
        for (j = 0L; j <= FitOrder; j++) {
            if (MatCoef(i, j) == 1L) {
                MatResSurf(i, j) = MATRESSURF[k];
                k++;
            }
            else
                MatResSurf(i, j) = 0.0;
        }
    }

    // Fin
    return true;
}

extern "C++" bool /*__declspec(dllexport)*/
H3BestFitSurf2(const H3_ARRAY2D_FLT32& Src,
			const H3_ARRAY2D_UINT8& SrcMask,
			H3_ARRAY2D_FLT64 & MatResSurf,
			const unsigned long FitOrder)
{
	size_t i,j;
	const size_t Imax=FitOrder+1,Jmax=FitOrder+1;
	
	H3_ARRAY2D_UINT8 MatCoef(Imax,Jmax);
	long MatVal=0;
	MatCoef.Fill(0L);

	for(i=0L;i<Imax;i++)
	{
		for(j=0L;j<Jmax-i;j++)
		{
			MatCoef(i,j)=1L;
			MatVal++;
		}
	}
	MatResSurf.ReAlloc(MatCoef.GetLi(),MatCoef.GetCo());
	MatResSurf.Fill(NaN);
	return(H3BestFitSurf2(Src,SrcMask,MatResSurf,FitOrder,MatVal,MatCoef));
}