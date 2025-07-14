#include "StdAfx.h"
#include "H3Calculation.h"
#include <ppl.h>

#include "H3Matrix.h"
#include "H3AppToolsDecl.h"
#include "ErrorDefineFile.h"
#include "MessageDefineFile.h"
#include "FrGrTreatementWrapper.h"
#include "fde.h"

//parametres à regler à l'exterieur du prgm
#define STEP (20L)				//pas d'echantillonage pour degauchissage
#define FIT_ORDER (2L)			//degré du polynome de degauchissage
//#define CONTRASTE_MIN 15	    //aide à la réalisation d'un masque
//#define INTENSITE_MIN 10	    //aide à la réalisation d'un masque
#define FILT_CST (3L)			//largeur du filtre
#define MASK_OPENING_HALF_SIZE 7;   // Odd number. For erosion + dilation of the mask


// This erosion function is specific for binary masks, not working for grayscale images.
void Erosion(const unsigned char* pIm, unsigned char* pOutIm, const unsigned int nImSizeV, const unsigned int nImSizeH, const unsigned int nKernelHalfSize)
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
    bool bZeros;

    for (unsigned int nV = nKernelHalfSize; nV < nImSizeV - nKernelHalfSize; nV++)
    {
        for (unsigned int nH = nKernelHalfSize; nH < nImSizeH - nKernelHalfSize; nH++)
        {
            bZeros = false;

            // Get the neighborhood of this pixel (nH,nV)
            for (nKV = 0; nKV < nKernelFullSize && !bZeros; nKV++)
            {
                for (nKH = 0; nKH < nKernelFullSize && !bZeros; nKH++)
                {
                    if (pKernel[nKV * nKernelFullSize + nKH] == 1)
                        if (pIm[(nV - nKernelHalfSize + nKV) * nImSizeH + nH - nKernelHalfSize + nKH] == 0)
                            bZeros = true;
                }
            }

            pOutIm[nV * nImSizeH + nH] = bZeros ? 0 : 1;
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


// Dilate + erode
void Opening(unsigned char* pIm, size_t& iHeight, size_t& iWidth)
{
    unsigned char* pTempIm = new unsigned char[(size_t)iHeight * (size_t)iWidth];
    unsigned int EltHalfSize = MASK_OPENING_HALF_SIZE;

    Erosion(pIm, pTempIm, iHeight, iWidth, EltHalfSize);

    Dilation(pTempIm, pIm, iHeight, iWidth, EltHalfSize);

    delete[] pTempIm;

}


//moyenne une image avec un filtre (N,1) ou (1,N)
//pImares doit etre allouer Et avoir la taille de Image
static bool filtre(H3_ARRAY2D_FLT32& imaRes,const H3_ARRAY2D_FLT32& Image, unsigned long N, char sens)
{
	unsigned long nx=Image.GetCo(),ny=Image.GetLi(),i,j,nbElts=0,nbElts_2;

	if(imaRes.GetCo()!=nx || imaRes.GetLi()!=ny){
		SetLastTestError(H3_CALCULATION_FILTRE2_ERROR);	
		return false;
	}

	float *p_filt0= imaRes.GetData(),*p_filt=p_filt0,*p_filt2;
	float *pI0=Image.GetData(),*pI=pI0,*pI2;
	unsigned long Ns2p1=N/2+1;

	switch(sens){
	case 'x':
	case 'X':
        if (nx >= N) {
            //on commence par filtrer ligne à ligne avec N elements
            //le filtre fait la moyenne de N elements consécutifs quand cela est possible
            //pour les case de 0 à N-1 et nx-N+1 à nx-1, la moyenne est faite sur moins d'éléments
            for (i = 0; i < ny; i++) {
                pI = pI0 + i * nx;
                p_filt = p_filt0 + i * nx;
                //le remplissage des N premiers éléments est speciale
                //la premiere case est la somme des (N/2)+1 premiers éléments
                (*p_filt) = *pI++;
                for (j = 1; j < Ns2p1; j++)
                    (*p_filt) += *pI++;

                nbElts = Ns2p1;
                *p_filt /= nbElts;
                p_filt++;
                //jusqu'à la Nieme case on rajoute des elements
                nbElts_2 = nbElts + 1;
                p_filt2 = p_filt - 1;
                for (j = 1; j < Ns2p1; j++) {
                    *p_filt = (*p_filt2)*nbElts + *pI++;
                    *p_filt /= nbElts_2;
                    nbElts++;
                    nbElts_2++;
                    p_filt++;
                    p_filt2++;
                }
                //puis on peut ajouter et enlever jusqu'à N cases de la fin
                pI2 = pI - N;
                for (j = Ns2p1; j < nx - Ns2p1 + 1; j++) {
                    *p_filt = *p_filt2 + (*pI - *pI2) / N;
                    p_filt++;
                    p_filt2++;
                    pI++;
                    pI2++;
                }
                //fin de ligne
                nbElts = N;
                nbElts_2 = nbElts - 1;
                for (j = nx - Ns2p1 + 1; j < nx; j++) {
                    *p_filt = (*p_filt2)*nbElts - *pI2;
                    *p_filt /= nbElts_2;
                    nbElts--;
                    nbElts_2--;
                    p_filt++;
                    p_filt2++;
                    pI2++;
                }
            }
        }
		break;
	case 'y':
	case 'Y':
		if(ny>=N){
		//on commence par filtrer colonne à colonne avec N elements
		//le filtre fait la moyenne de N elements consécutifs quand cela est possible
		//pour les case de 0 à N-1 et nx-N+1 à nx-1, la moyenne est faite sur moins d'éléments
			//le remplissage des N premiers éléments est speciale
			//la premiere case est la somme des (N/2)+1 premiers éléments
			for(j=0;j<nx;j++)
                *p_filt++ = *pI++;
			for(i=1;i<Ns2p1;i++){
				p_filt -= nx;
				for(j=0;j<nx;j++)
                    *p_filt++ += *pI++;
			}
			//il faut normer par (N+1)/2
			p_filt -= nx;
			nbElts=Ns2p1;
			for(j=0;j<nx;j++)
                *p_filt++ /= nbElts;
			//là, la premiere ligne est initialisée.

			//jusqu'à la Ns2p1 ieme ligne on rajoute des elements
			nbElts_2=nbElts+1;
			p_filt2=p_filt - nx;//1 ligne av;
			for(i=1;i<Ns2p1;i++){
				for(j=0;j<nx;j++){
                    *p_filt = (*p_filt2)*nbElts + *pI;
					*p_filt /= nbElts_2;
					p_filt++;
					p_filt2++;
					pI++;
				}
				nbElts++;
				nbElts_2++;
			}
			//puis on peut ajouter et enlever jusqu'à N cases de la fin
			pI2=pI - N*nx;//N lignes av
			for(i=Ns2p1;i<ny-Ns2p1+1;i++){
				for(j=0;j<nx;j++){
					*p_filt = *p_filt2 + (*pI-*pI2)/N;
					p_filt++;
					p_filt2++;
					pI++;
					pI2++;
				}
			}
			//on se rapproche de la fin
			nbElts=N;
			nbElts_2=nbElts-1;
			for(i=ny-Ns2p1+1;i<ny;i++){
				for(j=0;j<nx;j++){
                    *p_filt = (*p_filt2)*nbElts - *pI2;
                    *p_filt /= nbElts_2;
					p_filt ++;
					p_filt2++;
					pI2 ++;
				}
				nbElts--;
				nbElts_2--;
			}
		}
		break;
	default:
		SetLastTestError(H3_CALCULATION_FILTRE3_ERROR);
		return false;
	}
	return true;
}

/*! 
* 	\fn      :long H3InitMatCoef(H3_ARRAY2D_UINT8 & MatCoef,long FitOrder
* 	\brief   : H3Etalonnage
* 	\param   :
* 	\return  : 
* 	\remarks : 
*/
static long H3InitMatCoef(H3_MATRIX_UINT8 & MatCoef,long FitOrder)
{
	CString str;
	long c=0;
	for(long i=0L; i<FitOrder+1L; i++) {
		for(long j=0L; j<FitOrder+1L; j++) {
			if ((i+j)>FitOrder)
				MatCoef(i,j)=0L;
			else
			{
				MatCoef(i,j)=1L;
				c++;
			}
		}
	}
	return c;
}

/*! 
* 	\fn      bool H3BestFitSurf(const H3_ARRAY2D_FLT32& Src,
				   const H3_ARRAY2D_UINT8& SrcMask,
				   H3_ARRAY2D_FLT64 & MatResSurf,
				   long FitOrder,long MatVal,
				   const H3_ARRAY2D_UINT8& MatCoef)
* 	\brief : fit des points de Src (si SrcMask>0) par une fonction polynomiale dont les coefs sont decrit dans MatCoef  
* 	\param   const H3_ARRAY2D_FLT32& Src : 
* 	\param   const H3_ARRAY2D_UINT8& SrcMask : 
* 	\param out H3_ARRAY2D_FLT64 & MatResSurf : coefficient du polynome. La matrice doit avoir autant d'elements que MatCoef
* 	\param   long FitOrder : 
* 	\param   long MatVal : nombre d'elements à 1 dans MatCoef (à priori)
* 	\param   const H3_ARRAY2D_FLT32& MatCoef : matrice des coefficients à chercher (matrice carré)
*   \                        
							| 0 | 1 | 2 |... puissace de x
						______________________
						0	| 1	| 0	| 1	|
						1	| 0	| 1	| 0	|
	   puissance de y  ...						on cherche ici un polynome du type a00.x0y0+a20.x2y0+a11.x1y1
* 	\return  bool		
* 	\remarks 
*/ 
static bool H3BestFitSurf(const H3_MATRIX_FLT32& Src,
				   const H3_MATRIX_UINT8& SrcMask,
				   H3_MATRIX_FLT64 & MatResSurf,
				   long FitOrder,long MatVal,
				   const H3_MATRIX_UINT8& MatCoef)
{
	CString str;

	long nSizeX=Src.GetCo();
	long nSizeY=Src.GetLi();

	long ValidElement=0;
	long NumRow=0;
	long i,j,k=0;

	if((nSizeX!=SrcMask.GetCo())||(nSizeY!=SrcMask.GetLi()))
	{
		SetLastTestError(H3_CALCULATION_FIT1_ERROR);
		return false;
	}

	//Initialiser les pointeurs sur les données
	H3_FLT32 *pSource=Src.GetData();
	H3_UINT8 *pMask=SrcMask.GetData();

	// Determination du nombre d'éléments valide à partir du masque
	for(long li=0L; li<nSizeY; li++) {
		for(long co=0; co<nSizeX; co++) {
			if ( (*pMask>0) /*&& (!_isnan(*pSource))*/ )
				ValidElement++;
			pMask++;pSource++;
		}
	}

	if(ValidElement<MatVal)
	{
		SetLastTestError(H3_CALCULATION_FIT2_ERROR);
		return false;
	}

	pMask=SrcMask.GetData();
	pSource=Src.GetData();

	// Creation et initialisation des matrices Y,XjYi,M,TM
	H3_MATRIX_FLT64 MATRESSURF(MatVal,1),SURF(ValidElement,1);
	H3_MATRIX_FLT64 MM(MatVal,MatVal),MS(MatVal,1L),MatTmp(MatVal,1L);
	MM.Fill(0);
	MS.Fill(0);
	MatTmp.Fill(0);

	H3_FLT64 Yi,XjYi;
	// Calcul des coefficients de la matrice MatResSurf
	k=0L;
	for(long Ypix=0L; Ypix<nSizeY; Ypix++) {
		for(long Xpix=0L; Xpix<nSizeX; Xpix++) {
			if ((*pMask>0L)/* && (!_isnan(*pSource))*/ )
			{
				NumRow=0L;
				Yi=1.0;
				for(i=0L; i<=FitOrder; i++) {
					XjYi=Yi;
					for(j=0L; j<=FitOrder; j++)
					{									
						if (MatCoef(i,j)==1L) {
							MatTmp[NumRow]=XjYi;
							NumRow++; 
						}
						XjYi*=Xpix;
					}
					Yi*=Ypix;
				}
				SURF[k]=*pSource;

				for(i=0L;i<MatVal;i++)
				{
					MM(i,i)+=MatTmp[i]*MatTmp[i];
					for(j=i+1L;j<MatVal;j++)
						MM(j,i)+=MatTmp[i]*MatTmp[j];
					MS[i]+=MatTmp[i]*(*pSource);
				}

				k++;
			}
			pMask++;pSource++;
		}
	}

    for (i = 0L; i < MatVal; i++)
        for (j = i + 1L; j < MatVal; j++)
            MM(i, j) = MM(j, i);

	// Resoudre avec le critere des moindres carres

	//1 normalisation
	H3_MATRIX_FLT64 MatMaxCo(1L,MatVal),MatMaxLi(MatVal,1L);
	MatMaxCo.Fill(0.0);
	MatMaxLi.Fill(0.0);

	for(i=0L;i<MatVal;i++){
		for(j=0L;j<MatVal;j++){
			MatMaxCo[j]=__max(MatMaxCo[j],fabs(MM(i,j)));
			MatMaxLi[i]=__max(MatMaxLi[i],fabs(MM(i,j)));
		}
	}
	for(i=0L;i<MatVal;i++){
		MatMaxCo[i]=sqrt(MatMaxCo[i]);
		MatMaxLi[i]=sqrt(MatMaxLi[i]);
	}
	
	H3_MATRIX_FLT64 MM_tmp(MatVal,MatVal);
	for(i=0L;i<MatVal;i++)
		for(j=0L;j<MatVal;j++)
			MM_tmp(i,j)=MM(i,j)/(MatMaxCo[j]*MatMaxLi[i]);	
	
	//2 inversion
	H3_MATRIX_FLT64 iMM_tmp=MM_tmp.Inv();

	if(iMM_tmp.GetSize()==MatVal*MatVal){
		for(i=0L;i<MatVal;i++)
		for(j=0L;j<MatVal;j++)
			iMM_tmp(i,j)/=MatMaxCo[i]*MatMaxLi[j];

		MATRESSURF=iMM_tmp*MS;
	}
	else{
		SetLastTestError(H3_CALCULATION_FIT3_ERROR);
		return false;
	}

	if(MATRESSURF.GetSize()!=MatVal)
	{
		SetLastTestError(H3_CALCULATION_FIT3_ERROR);
		return false;
	}

	k=0L;
	for(i=0L; i<=FitOrder; i++) {
		for(j=0L; j<=FitOrder; j++) {
			if (MatCoef(i,j)==1L) {
				MatResSurf(i,j)=MATRESSURF[k];
				k++;
			}
			else
				MatResSurf(i,j)=0.0;
		}
	}
	return true;
}

/*! 
* 	\fn      bool H3BestFitSurf(const H3_ARRAY2D_FLT32& Src,const H3_ARRAY2D_UINT8& SrcMask,H3_ARRAY2D_FLT64 & MatResSurf,long FitOrder)
* 	\brief : appel simplifier de H3BestFitSurf
*/
static bool H3BestFitSurf(const H3_MATRIX_FLT32& Src, const H3_MATRIX_UINT8& SrcMask, H3_MATRIX_FLT64 & MatResSurf, long FitOrder)
{
	long i,j;
	long Imax=FitOrder+1;
	long Jmax=FitOrder+1;
	
	H3_MATRIX_UINT8 MatCoef(Imax,Jmax);
	long MatVal=0;
	MatCoef.Fill(0L);

	for(i=0L;i<Imax;i++)
	for(j=0L;j<Jmax-i;j++)
	{
		MatCoef(i,j)=1L;
		MatVal++;
	}
	MatResSurf.ReAlloc(MatCoef.GetLi(),MatCoef.GetCo());
	MatResSurf.Fill(NaN);
	return(H3BestFitSurf(Src,SrcMask,MatResSurf,FitOrder,MatVal,MatCoef));
}

static bool Degauchi(H3_ARRAY2D_FLT32& pSrc0,const H3_ARRAY2D_UINT8& Mask)
{
	const unsigned long nx=Mask.GetCo();
	const unsigned long ny=Mask.GetLi();

	//échantillonnage
	unsigned long k;
	unsigned long iStep,jStep,kStep,kStep0;	//index image entiere (colonne, ligne, total)
	unsigned long Step=STEP;
	unsigned long FitOrder=FIT_ORDER;
	const unsigned long nny=(ny-1L)/Step+1L/Step+1L/Step+1L,nnx=(nx-1L)/Step+1L;
	H3_MATRIX_FLT32 Src(nny,nnx);
	H3_MATRIX_UINT8 MatMask(nny,nnx);
	H3_MATRIX_FLT64 MatResSurf;

	byte *pMMask=MatMask.GetData();
	float *pSrc=Src.GetData();
	float *ppSrc0=pSrc0.GetData();

	//attention:il est probablement couteux en temps de fitter l'ensemble des données
	//faire un échantillonnage pour obtenir la fonction de fit
	kStep0=0L;k=0L;
	for(jStep=0L; jStep<ny; jStep+=Step){
		kStep=kStep0;
		for(iStep=0L; iStep<nx; iStep+=Step){
			pMMask[k]=Mask[kStep];
			pSrc[k]=  ppSrc0[kStep];
			k++;
			kStep+=Step;
		}
		kStep0+=nx*Step;
	}

	//calcul du polynome
	//MatResSurf contient les coef du polynome
	if(!H3BestFitSurf(Src,MatMask,MatResSurf,FitOrder)){//H3BestFitSurf gère ses propres messages d'erreur
		return false;
	}

	// Degauchissage
	//for (unsigned long y = 0L, ind_k0 = 0L; y < ny; y++)
	concurrency::parallel_for(size_t(0), size_t(ny), [&](size_t y) 
	{
		long index = y * nx;
		double dy = (double)y / double(Step);
		for (unsigned long x = 0L; x < nx; x++, index++)
		{
			if (!Mask[index])
			{
				ppSrc0[index] = 0.0f;
				continue;
			}
			double ze = 0.0f;
			long ind_k0 = 0L; //index dans MatResSurf
			double dx = (double)x / (double)Step;
			double Yj = 1.0f;
			for (unsigned long pow_j = 0L; pow_j <= FitOrder; pow_j++)
			{
				long ind_k = ind_k0;
				double YjXi = Yj;
				for (unsigned long pow_i = 0L; pow_i <= FitOrder - pow_j; pow_i++)
				{
					ze += YjXi * MatResSurf[ind_k];
					YjXi *= dx;
					ind_k++;
				}
				Yj *= dy;
				ind_k0 += (FitOrder + 1L);
			}
			ppSrc0[index] -= (float)ze;
		}
	}
	);

	return true;
}

//Contructeur par défaut initialise quelques membre à 0
CH3Calculation::CH3Calculation(void)
{
}


//Constructeur implémentant l'initialisation des variables.
bool CH3Calculation::Allocate(long ny, long nx, long nbFrames, long nbPeriods)
{
    m_nx = nx;
    m_ny = ny;
    m_nbImagesPerPeriod = nbFrames;
    m_nbPeriods = nbPeriods;

    // Allocation des images de Phases
    //................................
    bool ok = true;

    m_WrappedPhases.resize(m_nbPeriods);
    for (int p = 0; p < m_nbPeriods; p++)
        ok &= m_WrappedPhases[p].Alloc(m_ny, m_nx);

    ok &= m_Intensity.ReAlloc(m_ny, m_nx);
    ok &= m_PhaseInterval.ReAlloc(m_ny, m_nx);
    ok &= m_Contraste.ReAlloc(m_ny, m_nx);
    ok &= m_Phase_Map_2.ReAlloc(m_ny, m_nx);
    ok &= m_Mask.ReAlloc(m_ny, m_nx);
    ok &= m_Dark.ReAlloc(m_ny, m_nx);
    m_Phase_Map_1.LinkData(m_WrappedPhases[0]);

    m_IsAllocated = ok;
    return ok;
}

CH3Calculation::~CH3Calculation(void)
{
}

/*
//bool CH3Calculation::Init_Phase_Contraste(WORD** pArrayOfFrangingImage)
//resultat erroné si Init_Intensity n'a pas ete appelé prealablement avec les memes données
//la phase est le resultat d'un atan2 et est dans [-pi,pi[
//on garde phi1 dans [-pi,pi[
//et pour eviter des pb aux transitions on calcule phi2 dans [0,2pi[
//ainsi qu'un mask pour savoir quand utiliser phi1 (loin des bornes >> dans [-pi/2,pi/2[)
//et phi2 (loin des bornes >> dans [pi/2,3pi/2[)
//on calcule aussi le contraste
*/
bool CH3Calculation::ComputePhases(std::vector<BYTE*>& images)
{
    if (m_nbImagesPerPeriod < 3L || !m_IsAllocated) {
        SetLastTestError(H3_CALCULATION_PHASE1_ERROR);
        return false;
    }

    float tmp_const = 2.0f / m_nbImagesPerPeriod;

    // Precalcul des cos/sin
    //......................
    std::vector< std::vector<float>> cosTable;
    std::vector< std::vector<float>> sinTable;
    cosTable.resize(m_nbPeriods);
    sinTable.resize(m_nbPeriods);

    for (int p = 0; p < m_nbPeriods; p++)
    {
        float delta_phase0 = fTWO_PI / m_nbImagesPerPeriod;
        unsigned long ns2 = (m_nbImagesPerPeriod + 1L) / 2L;
        cosTable[p].resize(ns2);
        sinTable[p].resize(ns2);

        float phase = 0;
        for (unsigned long i = 0; i < ns2; i++) {
            cosTable[p][i] = cos(phase);
            sinTable[p][i] = sin(phase);
            phase += delta_phase0;
        }
    }

    unsigned long imageSize = m_ny * m_nx;
    bool nImage_even = (m_nbImagesPerPeriod & 1) == 0;

    //for (unsigned long k = 0; k < imageSize; k++)
    concurrency::parallel_for(0UL, imageSize, [&](size_t k)
    {
        for (int period = 0; period < m_nbPeriods; period++)
        {
            BYTE** pArrayOfFrangingImage = &images[m_nbImagesPerPeriod * period];

            // calcul de la phase (phi) et du contraste (m)
            // on calcule dans un premier temps Io.m.cos(phi) et Io.m.sin(phi)
            // on appelle (abusivement) phi_cos: Io.m.cos(phi)  et phi_sin: Io.m.sin(phi)
            float phi_cos;
            float phi_sin = 0;
            float sum;

            // i=0 phase=0 cos(phase)=1 sin(phase)=0;
            //.......................................
            const unsigned long ns2 = (m_nbImagesPerPeriod + 1L) / 2L;
            BYTE *FrangingImage_1, *FrangingImage_nn;

            // 2 cas pour le calcul de phase: nImage pair ou impair
            // influence phi_cos uniquement
            FrangingImage_1 = pArrayOfFrangingImage[0];
            if (nImage_even)
            {
                FrangingImage_nn = pArrayOfFrangingImage[ns2];
                phi_cos = (float)FrangingImage_1[k] - (float)FrangingImage_nn[k];
                sum = (float)FrangingImage_1[k] + (float)FrangingImage_nn[k];
            }
            else
            {
                phi_cos = (float)FrangingImage_1[k];
                sum = (float)FrangingImage_1[k];
            }

            // i>1
            //.......................
            for (unsigned long i = 1; i < ns2; i++)
            {
                float c = cosTable[period][i], s = sinTable[period][i];

                FrangingImage_1 = pArrayOfFrangingImage[i];
                FrangingImage_nn = pArrayOfFrangingImage[m_nbImagesPerPeriod - i];

                phi_cos += ((float)FrangingImage_1[k] + (float)FrangingImage_nn[k])*c;
                phi_sin += ((float)FrangingImage_1[k] - (float)FrangingImage_nn[k])*s;

                sum += (float)FrangingImage_1[k] + (float)FrangingImage_nn[k];
            }


            // Phase / Interval / Intensité / Contraste
            //..........................................
            float phase = atan2(phi_sin, phi_cos);
            H3_ARRAY2D_FLT32& W = m_WrappedPhases[period];
            W[k] = phase;

            if (period == 0)
            {
                //m_Phase_Map_1 est un alias de W
                if (phase < 0.0f)
                    m_Phase_Map_2[k] = phase + fTWO_PI;
                else
                    m_Phase_Map_2[k] = phase;

                m_PhaseInterval[k] = (fmPIs2 < phase && phase < fPIs2);

                m_Intensity[k] = sum / m_nbImagesPerPeriod;
                m_Contraste[k] = tmp_const * (float)sqrt(pow(phi_sin, 2) + pow(phi_cos, 2));

                // Computing "Dark" image with the new method
                m_Dark[k] = m_Intensity[k] - m_Contraste[k];

                // Operation needed for mask computation:
                m_Contraste[k] /= m_Intensity[k];
            }
        }
    }
    );

    m_IsContrasteInitialised = true;
    m_IsIntensityInitialised = true;
    return true;
}

/*
//bool CH3Calculation::Init_Phase_Contraste(WORD** pArrayOfFrangingImage)
//resultat erroné si Init_Intensity n'a pas ete appelé prealablement avec les memes données
//la phase est le resultat d'un atan2 et est dans [-pi,pi[
//on garde phi1 dans [-pi,pi[
//et pour eviter des pb aux transitions on calcule phi2 dans [0,2pi[
//ainsi qu'un mask pour savoir quand utiliser phi1 (loin des bornes >> dans [-pi/2,pi/2[)
//et phi2 (loin des bornes >> dans [pi/2,3pi/2[)
//on calcule aussi le contraste
*/
bool CH3Calculation::ComputePhases(std::vector<BYTE*>& images, int period)
{
    if (m_nbImagesPerPeriod < 3L) {
        SetLastTestError(H3_CALCULATION_PHASE1_ERROR);
        return false;
    }

    float tmp_const = 2.0f / m_nbImagesPerPeriod;

    // Precalcul des cos/sin
    //......................
    std::vector<float> cosTable;
    std::vector<float> sinTable;

    float delta_phase0 = fTWO_PI / m_nbImagesPerPeriod;
    unsigned long ns2 = (m_nbImagesPerPeriod + 1L) / 2L;
    cosTable.resize(ns2);
    sinTable.resize(ns2);

    float phase = 0;
    for (unsigned long i = 0; i < ns2; i++) {
        cosTable[i] = cos(phase);
        sinTable[i] = sin(phase);
        phase += delta_phase0;
    }

    float contrasteMin = ((float)m_wrapper->m_filterFactory.Contraste_min_curvature) / 100;
    float intensityMin = (float)m_wrapper->m_filterFactory.Intensite_min_curvature;

    unsigned long imageSize = m_ny * m_nx;
    bool nImage_even = (m_nbImagesPerPeriod & 1) == 0;

    //for (unsigned long k = 0; k < imageSize; k++)
    concurrency::parallel_for(0UL, imageSize, [&](size_t k)
    {
        BYTE** pArrayOfFrangingImage = &images[m_nbImagesPerPeriod * period];

        // calcul de la phase (phi) et du contraste (m)
        // on calcule dans un premier temps Io.m.cos(phi) et Io.m.sin(phi)
        // on appelle (abusivement) phi_cos: Io.m.cos(phi)  et phi_sin: Io.m.sin(phi)
        float phi_cos;
        float phi_sin = 0;
        float sum;

        // i=0 phase=0 cos(phase)=1 sin(phase)=0;
        //.......................................
        const unsigned long ns2 = (m_nbImagesPerPeriod + 1L) / 2L;
        BYTE* FrangingImage_1, * FrangingImage_nn;

        // 2 cas pour le calcul de phase: nImage paire ou impaire
        // influence phi_cos uniquement
        FrangingImage_1 = pArrayOfFrangingImage[0];
        if (nImage_even)
        {
            FrangingImage_nn = pArrayOfFrangingImage[ns2];
            phi_cos = (float)FrangingImage_1[k] - (float)FrangingImage_nn[k];
            sum = (float)FrangingImage_1[k] + (float)FrangingImage_nn[k];
        }
        else
        {
            phi_cos = (float)FrangingImage_1[k];
            sum = (float)FrangingImage_1[k];
        }

        // i>1
        //.......................
        for (unsigned long i = 1; i < ns2; i++)
        {
            float c = cosTable[i], s = sinTable[i];

            FrangingImage_1 = pArrayOfFrangingImage[i];
            FrangingImage_nn = pArrayOfFrangingImage[m_nbImagesPerPeriod - i];

            phi_cos += ((float)FrangingImage_1[k] + (float)FrangingImage_nn[k]) * c;
            phi_sin += ((float)FrangingImage_1[k] - (float)FrangingImage_nn[k]) * s;

            sum += (float)FrangingImage_1[k] + (float)FrangingImage_nn[k];
        }


        // Phase / Interval / Intensité / Contraste²
        //..........................................
        float phase = atan2(phi_sin, phi_cos);
        H3_ARRAY2D_FLT32& W = m_WrappedPhases[period];
        W[k] = phase;

        if (period == 0)
        {
            //m_Phase_Map_1 est un alias de W[0]
            if (phase < 0.0f)
                m_Phase_Map_2[k] = phase + fTWO_PI;
            else
                m_Phase_Map_2[k] = phase;

            m_PhaseInterval[k] = (fmPIs2 < phase&& phase < fPIs2);

            m_Intensity[k] = sum / m_nbImagesPerPeriod;
            m_Contraste[k] = tmp_const * (float)sqrt(pow(phi_sin, 2) + pow(phi_cos, 2));

            // Computing "Dark" image with the new method
            m_Dark[k] = m_Intensity[k] - m_Contraste[k];

            // Operation needed for mask computation:
            m_Contraste[k] /= m_Intensity[k];

            m_Mask[k] = (m_Contraste[k] > contrasteMin) && (m_Intensity[k] > intensityMin);
        }
    }

    );

    if (period == 0)
    {
        // Avoiding the risk of black dots inside the wafer due to scattering areas where fringes have not been detected 
    // (these areas are especially problematic in Dark images, where they look black on a dark background instead of light)

        // Opening operation after strong downsampling (to be fast)
        const unsigned int nSizeCo = m_Mask.GetCo();
        const unsigned int nSizeLi = m_Mask.GetLi();
        const unsigned int nSampRatio = 10;
        size_t nSampSizeCo = max(nSizeCo / nSampRatio, 10);   // Not smaller than 10
        size_t nSampSizeLi = max(nSizeLi / nSampRatio, 10);

        CH3Array2D<unsigned char> DownSampMask(nSampSizeLi, nSampSizeCo);

        // Down sampling
        for (int nRow = 0; nRow < nSampSizeLi; nRow++)
        {
            for (int nCol = 0; nCol < nSampSizeCo; nCol++)
            {
                DownSampMask(nRow, nCol) = m_Mask(min(nRow * nSampRatio + (int)(0.5* nSampRatio), nSizeLi-1), min(nCol * nSampRatio + (int)(0.5* nSampRatio), nSizeCo-1));
            }
        }

        // Opening operation
        Opening(DownSampMask.GetData(), nSampSizeLi, nSampSizeCo);

        // Using the small mask to improve the original one 
        concurrency::parallel_for(unsigned long(0), (unsigned long)nSizeLi, [&](unsigned long nRow)
            //for (unsigned int nRow = 0; nRow < nSizeLi; nRow++)
            {
                for (unsigned int nCol = 0; nCol < nSizeCo; nCol++)
                {
                    bool bIsOne;
                    // For each pixel of the original mask, the value is 1 if all the neighbours of the small mask are at 1, otherwise the original value is not modified.
                    bIsOne = DownSampMask((size_t)min(floor(nRow / nSampRatio), nSampSizeLi - 1), (size_t)min(floor(nCol / nSampRatio), nSampSizeCo - 1)) &&
                        DownSampMask((size_t)min(floor(nRow / nSampRatio) + 1, nSampSizeLi - 1), (size_t)min(floor(nCol / nSampRatio), nSampSizeCo - 1)) &&
                        DownSampMask((size_t)min(floor(nRow / nSampRatio), nSampSizeLi - 1), (size_t)min(floor(nCol / nSampRatio) + 1, nSampSizeCo - 1)) &&
                        DownSampMask((size_t)min(floor(nRow / nSampRatio) - 1, nSampSizeLi - 1), (size_t)min(floor(nCol / nSampRatio), nSampSizeCo - 1)) &&
                        DownSampMask((size_t)min(floor(nRow / nSampRatio), nSampSizeLi - 1), (size_t)min(floor(nCol / nSampRatio) - 1, nSampSizeCo - 1));

                    m_Mask(nRow, nCol) = bIsOne ? 1 : m_Mask(nRow, nCol);
                }
            });

        m_IsContrasteInitialised = true;
        m_IsIntensityInitialised = true;
        m_IsMaskInitialised = true;

    }
    return true;
}

//
bool CH3Calculation::LinkMasks()
{
    if (!m_IsContrasteInitialised)
    {
        SetLastTestError(H3_CALCULATION_MASK_ERROR);
        return false;
    }

    //.....
    int	iContrasteMin = m_wrapper->m_filterFactory.Contraste_min_curvature;
    int iIntensiteMin = m_wrapper->m_filterFactory.Intensite_min_curvature;

    if (!m_IsMaskInitialised)
        Compute_Mask(m_Mask, iContrasteMin, iIntensiteMin);

    // Mask de sauvegarde de l'amplitude
    //..................................

    m_Mask_Save_Amplitude.LinkData(m_Mask);

    // Mask de sauvegarde de la courbure
    //..................................

    m_Mask_Save_Curvature.LinkData(m_Mask);

    m_IsMaskInitialised = true;
    return true;
}

void CH3Calculation::Compute_Mask(H3_ARRAY2D_UINT8&  mask, int iContrasteMin, int iIntensiteMin)
{    
    mask.ReAlloc(m_ny, m_nx);

    float ContrasteMin = ((float)iContrasteMin) / 100;
    float IntensityMin = (float)iIntensiteMin;

    size_t imgSize = mask.GetSize();

    //for (size_t i = 0; i < imgSize; i++)
    concurrency::parallel_for(size_t(0), imgSize, [&](size_t i)
    {
        mask[i] = (m_Contraste[i] > ContrasteMin) && (m_Intensity[i] > IntensityMin);
    });
}

/*
brief: calcul de la derivé en X. En chaque point P(ligne, colon) dX=P(ligne, colon-1)-P(ligne, colon+1)
*/
bool CH3Calculation::ComputeDerivX()
{
    if (!m_IsMaskInitialised) {
        SetLastTestError(H3_CALCULATION_DERIVX1_ERROR);
        return false;
    }

    m_Deriv.Alloc(m_ny, m_nx);

    // Calcul de la derivée
    //.....................
    concurrency::parallel_for(unsigned long(0), m_ny, [&](unsigned long i)
    //for (unsigned long i = 0L; i < m_ny; i++)
    {
        float *ppDerivX = m_Deriv.GetLine(i);

        float *pPhi1 = m_Phase_Map_1.GetLine(i);
        float *pPhi2 = m_Phase_Map_2.GetLine(i);

        byte *pPI = m_PhaseInterval.GetLine(i);

        //j=0L (pas de calcul possible)
        ppDerivX[0] = 0.0f;

        //j=1..nx-2
        unsigned long j;
        for (j = 1L; j < m_nx - 1L; j++)
        {
            if (pPI[j] == 1L)
                ppDerivX[j] = pPhi1[j - 1] - pPhi1[j + 1];
            else
                ppDerivX[j] = pPhi2[j - 1] - pPhi2[j + 1];
        }

        //j=nx-1 (pas de calcul possible)
        ppDerivX[j] = 0.0f;
    }
    );

    ApplyMaskAndLevel();


    // Filtrage
    //.........
    const unsigned long N = FILT_CST;
    H3_ARRAY2D_FLT32 Fx(m_ny, m_nx);
    if (!filtre(Fx, m_Deriv, N, 'x'))
        return false;
    if (!filtre(m_Deriv, Fx, N, 'y'))
        return false;

    return true;
}

void CH3Calculation::ApplyMaskAndLevel()
{
    // Replacing the Degauchi by a simple application of mask, then bringing all data to a mean of 0.
    // Computing the average in the mask
    int iCount = 0;
    double dSum = 0.0;
    unsigned long iInd;
    for (iInd = 0; iInd < m_nx * m_ny; iInd++)
    {
        if (!m_Mask[iInd])
            m_Deriv[iInd] = 0.0f;
        else
        {
            dSum = dSum + m_Deriv[iInd];
            iCount++;
        }
    }

    const double dAverage = dSum / (double)iCount;
    for (iInd = 0; iInd < m_nx * m_ny; iInd++)
    {
        if (m_Mask[iInd])
            m_Deriv[iInd] = (float)(m_Deriv[iInd] - dAverage);
    }
}

/*
brief: calcul de la derivé en Y. En chaque point P(ligne, colon) dX=P(ligne-1, colon)-P(ligne+1, colon)
*/
bool CH3Calculation::ComputeDerivY()
{
    if (!m_IsMaskInitialised) {
        SetLastTestError(H3_CALCULATION_DERIVY1_ERROR);
        return false;
    }

    m_Deriv.Alloc(m_ny, m_nx);

    // Calcul de la derivée
    //.....................

    //i=0L  (pas de calcul possible)
    for (unsigned long j = 0L; j < m_nx; j++)
        m_Deriv(0, j) = 0.0f;

    //i=1..ny-2
    // for(unsigned long i=1L;i<m_ny-1L;i++)
    concurrency::parallel_for(unsigned long(1), m_ny - 1L, [&](unsigned long i)
        {
            for (unsigned long j = 0L; j < m_nx; j++) {
                if (m_PhaseInterval(i, j) == 1L)
                    m_Deriv(i, j) = m_Phase_Map_1(i - 1, j) - m_Phase_Map_1(i + 1, j);
                else
                    m_Deriv(i, j) = m_Phase_Map_2(i - 1, j) - m_Phase_Map_2(i + 1, j);
            }
        });

    //i=ny-1L  (pas de calcul possible)
    for (unsigned long j = 0L; j < m_nx; j++)
        m_Deriv(m_ny - 1, j) = 0.0f;

    ApplyMaskAndLevel();

    // Filtrage
    //.........
    const unsigned long N = FILT_CST;
    H3_ARRAY2D_FLT32 Fx(m_ny, m_nx);
    if (!filtre(Fx, m_Deriv, N, 'x'))
        return false;
    if (!filtre(m_Deriv, Fx, N, 'y'))
        return false;

    return true;
}

