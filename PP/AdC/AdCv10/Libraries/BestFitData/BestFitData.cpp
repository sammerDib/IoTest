// BestFitData.cpp : définit les routines d'initialisation pour la DLL.
//

#include "stdafx.h"
#include "BestFitData.h"

#include "G2mat.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

//
//TODO: si cette DLL est liée dynamiquement aux DLL MFC,
//		toute fonction exportée de cette DLL qui appelle
//		MFC doit avoir la macro AFX_MANAGE_STATE ajoutée au
//		tout début de la fonction.
//
//		Par exemple :
//
//		extern "C" BOOL PASCAL EXPORT ExportedFunction()
//		{
//			AFX_MANAGE_STATE(AfxGetStaticModuleState());
//			// corps de fonction normal ici
//		}
//
//		Il est très important que cette macro se trouve dans chaque
//		fonction, avant tout appel à MFC. Cela signifie qu'elle
//		doit être la première instruction dans la 
//		fonction, avant toute déclaration de variable objet
//		dans la mesure où leurs constructeurs peuvent générer des appels à la DLL
//		MFC.
//
//		Consultez les notes techniques MFC 33 et 58 pour plus de
//		détails.
//

// CBestFitDataApp

BEGIN_MESSAGE_MAP(CBestFitDataApp, CWinApp)
END_MESSAGE_MAP()


// construction CBestFitDataApp

CBestFitDataApp::CBestFitDataApp()
{
	// TODO: ajoutez ici du code de construction,
	// Placez toutes les initialisations significatives dans InitInstance
}


// Seul et unique objet CBestFitDataApp

CBestFitDataApp theApp;


// initialisation de CBestFitDataApp

BOOL CBestFitDataApp::InitInstance()
{
	CWinApp::InitInstance();

	return TRUE;
}

int  Simple_BWThreshold(float* pIn, byte* pInMask,  byte* pOut, int iSizeX, int iSizeY, float fAcceptMin, float fAcceptMax, int nPitchIN=0, int nPitchOUT = 0)
{
	// on pars du principe que les buffer sont initilisé avant et coté c# ...
	// on verra pour des amélio openmp plus tard...

	// HERE pitch are expresses in Pixel NOT IN BYTES !!!!
	if (nPitchIN == 0 || nPitchIN < iSizeX)
		nPitchIN = iSizeX;
	if (nPitchOUT == 0 || nPitchOUT < iSizeX)
		nPitchOUT = iSizeX;


	try
	{
		float* pPtrLineIN = pIn;
		byte* pPtrLineOUT = pOut; // le mask doit avoir le meme pitch que le put car même taille et meme type de data 

		if (pInMask == nullptr)
		{
			for (int j = 0; j < iSizeY; j++)
			{
				for (int i = 0; i < iSizeX; i++)
				{
					pPtrLineOUT[i] = (byte)((pPtrLineIN[i] >= fAcceptMin) && (pPtrLineIN[i] <= fAcceptMax));
				}
				pPtrLineIN += nPitchIN;
				pPtrLineOUT += nPitchOUT;
			}
		}
		else
		{
			byte* pPtrLineIN_MASK = pInMask; // le mask doit avoir le meme pitch que le put car même taille et meme type de data 
			for (int j = 0; j < iSizeY; j++)
			{
				for (int i = 0; i < iSizeX; i++)
				{
					if (pPtrLineIN_MASK[i])
						pPtrLineOUT[i] = (byte)((pPtrLineIN[i] >= fAcceptMin) && (pPtrLineIN[i] <= fAcceptMax));
					else
						pPtrLineOUT[i] = (byte)0;

				}
				pPtrLineIN += nPitchIN;
				pPtrLineOUT += nPitchOUT;
				pPtrLineIN_MASK += nPitchOUT;
			}

		}
		
		/*int nLenght = iSizeX*iSizeY;
		if(pInMask == nullptr)
		{
			for(int i=0; i<nLenght; i++)
			{
				pOut[i] = (byte)((pIn[i] >= fAcceptMin) && (pIn[i] <= fAcceptMax)) ;
			}
		}
		else
		{
			for(int i=0; i<nLenght; i++)
			{
				if(pInMask[i])
					pOut[i] = (byte)((pIn[i] >= fAcceptMin) && (pIn[i] <= fAcceptMax));
				else
					pOut[i] = (byte)0;
			}
		}*/
	}
	catch (CException* e)
	{
		TCHAR   szCause[255];
		e->GetErrorMessage(szCause, 255);
		return -1;
	}
	return 0;
}

int BestFitSurf(float* pSIn, byte* pInSMask, double** ppMatResSurf, long FitOrder, const unsigned long uSizeX, const unsigned long uSizeY, int nPitchIN, int nPitchMASK)
{
	long i,j;
	long Imax=FitOrder+1;
	long Jmax=FitOrder+1;

	MATRIX_UINT8 MatCoef(Imax,Jmax);
	long lMatVal=0L;
	MatCoef.Fill(0L);

	for(i=0L;i<Imax;i++)
	{
		for(j=0L;j<Jmax-i;j++)
		{
			MatCoef(i,j)=1L;
			lMatVal++;
		}
	}
	MATRIX_DBL	MatResSurf(MatCoef.GetLi(),MatCoef.GetCo());
    MatResSurf.Fill(dNaN);

	// Determination du nombre d'éléments valide à partir du masque
    long lValidElement=0;
	float *pSource = nullptr;
	byte *pMask = nullptr;
	MATRIX_UINT8 Mask;
	if(pInSMask != nullptr)
	{
		/*byte*  pdtSMsk = pInSMask;
		for(unsigned long li=0L; li<uSizeY; li++)
		{
			for(unsigned long co=0; co<uSizeX; co++)
			{
				if ( (*pdtSMsk>0))
					lValidElement++;
				pdtSMsk++;
			}
		}*/
		byte*  pdtSMsk_LINE = pInSMask;
		for (unsigned long li = 0L; li < uSizeY; li++)
		{
			for (unsigned long co = 0; co<uSizeX; co++)
			{
				if ((pdtSMsk_LINE[co]>0))
					lValidElement++;
			}
			pdtSMsk_LINE += nPitchMASK;
		}
		pMask = pInSMask;
	}
	else
	{

		lValidElement = uSizeX * uSizeY;
		Mask.ReAlloc(uSizeY,nPitchMASK);
		Mask.Fill(1);
		pMask=Mask.GetData();
	}
	pSource = pSIn;

	if(lValidElement<lMatVal)
	{
		// unable to perform fitting
		return -5;
	}

	// Creation et initialisation des matrices Y,XjYi,M,TM
	MATRIX_DBL MATRESSURF(lMatVal,1),SURF(lValidElement,1);
	MATRIX_DBL MM(lMatVal,lMatVal),MS(lMatVal,1L),MatTmp(lMatVal,1L);
	MM.Fill(0.0);
	MS.Fill(0.0);
	MatTmp.Fill(0.0);

	double Yi,XjYi;

	// Calcul des coefficients de la matrice MatResSurf
	long k=0L;
    long NumRow=0L;
	long nSizeX = (long) uSizeX;
	long nSizeY = (long) uSizeY;

	float* pSourceLine = pSource;
	byte* pMaskLine = pMask;

	for (long Ypix = 0L; Ypix < nSizeY; Ypix++)
	{
		for (long Xpix = 0L; Xpix<nSizeX; Xpix++)
		{
			if ((pMaskLine[Xpix]>0))
			{
				NumRow = 0L;
				Yi = 1.0;
				for (i = 0L; i <= FitOrder; i++)
				{
					XjYi = Yi;
					for (j = 0L; j <= FitOrder; j++)
					{
						if (MatCoef(i, j) == 1L)
						{
							MatTmp[NumRow] = XjYi;
							NumRow++;
						}
						XjYi *= Xpix;
					}
					Yi *= Ypix;
				}
				SURF[k] = pSourceLine[Xpix];

				for (i = 0L; i < lMatVal; i++)
				{
					MM(i, i) += MatTmp[i] * MatTmp[i];
					for (j = i + 1L; j < lMatVal; j++)
						MM(j, i) += MatTmp[i] * MatTmp[j];
					MS[i] += MatTmp[i] * (pSourceLine[Xpix]);
				}

				for (i = 0L; i < lMatVal; i++)
				for (j = i + 1L; j < lMatVal; j++)
					MM(i, j) = MM(j, i);

				k++;
			}
		}
		pMaskLine += nPitchMASK;
		pSourceLine += nPitchIN;
	}

// 	for(long Ypix=0L; Ypix<nSizeY; Ypix++)
// 	{
// 		for(long Xpix=0L; Xpix<nSizeX; Xpix++)
// 		{
// 			if ( (*pMask>0) )
// 			{
// 				NumRow=0L;
// 				Yi=1.0;
// 				for(i=0L; i<=FitOrder; i++)
// 				{
// 					XjYi=Yi;
// 					for(j=0L; j<=FitOrder; j++)
// 					{									
// 						if (MatCoef(i,j)==1L)
// 						{
// 							MatTmp[NumRow]=XjYi;
// 							NumRow++; 
// 						}
// 						XjYi*=Xpix;
// 					}
// 					Yi*=Ypix;
// 				}
// 				SURF[k]=*pSource;
// 
// 				for(i=0L;i<lMatVal;i++)
// 				{
// 					MM(i,i)+=MatTmp[i]*MatTmp[i];
// 					for(j=i+1L;j<lMatVal;j++)
// 						MM(j,i)+=MatTmp[i]*MatTmp[j];
// 					MS[i]+=MatTmp[i]*(*pSource);
// 				}
// 
// 				for(i=0L;i<lMatVal;i++)
// 					for(j=i+1L;j<lMatVal;j++)
// 						MM(i,j)=MM(j,i);
// 
// 				k++;
// 			}
// 			pMask++;
// 			pSource++;
// 		}
// 	}

	//1 normalisation
	MATRIX_DBL MatMaxCo(1L,lMatVal);
	MATRIX_DBL MatMaxLi(lMatVal,1L);
	MatMaxCo.Fill(0.0);
	MatMaxLi.Fill(0.0);

	for(i=0L;i<lMatVal;i++)
	{
		for(j=0L;j<lMatVal;j++)
		{
			MatMaxCo[j]=__max(MatMaxCo[j],fabs(MM(i,j)));
			MatMaxLi[i]=__max(MatMaxLi[i],fabs(MM(i,j)));
		}
	}
	for(i=0L;i<lMatVal;i++)
	{
		MatMaxCo[i]=sqrt(MatMaxCo[i]);
		MatMaxLi[i]=sqrt(MatMaxLi[i]);
	}

	MATRIX_DBL MM_tmp(lMatVal,lMatVal);
	for(i=0L;i<lMatVal;i++)
		for(j=0L;j<lMatVal;j++)
			MM_tmp(i,j)=MM(i,j)/(MatMaxCo[j]*MatMaxLi[i]);	

	//2 inversion
	MATRIX_DBL iMM_tmp=MM_tmp.Inv();

	if(iMM_tmp.GetSize()==lMatVal*lMatVal)
	{
		for(i=0L;i<lMatVal;i++)
			for(j=0L;j<lMatVal;j++)
				iMM_tmp(i,j)/=MatMaxCo[i]*MatMaxLi[j];

		MATRESSURF=iMM_tmp*MS;
	}
	else
	{
		return -6; 
	}

	if(MATRESSURF.GetSize() != lMatVal )
	{
		return -7;
	}

	k=0L;
	for(i=0L; i<= FitOrder; i++)
	{
		for(j=0L; j<= FitOrder; j++)
		{
			if (MatCoef(i,j)==1L)
			{
				MatResSurf(i,j)=MATRESSURF[k];
				k++;
			}
			else
				MatResSurf(i,j)=0.0;
		}
	}

	// Maj 
	(*ppMatResSurf) = new double[Imax * Jmax]; //où "T"(i,j) =  T[j*Imax + i];
	for(i=0L;i<(Imax * Jmax);i++)
	{
		(*ppMatResSurf)[i] = MatResSurf[i];
	}
	return 0;
}

int BestFit_Threshold(float* pIn, byte* pInMask, byte* pOut, int iSizeX, int iSizeY, int nSampleStep, int nOrder, float fAcceptMin, float fAcceptMax, int nPitchIN = 0, int nPitchOUT = 0)
{
	bool bUseSampleBuff = false;
	bool bUseMask = false;
	if(nOrder <= 0)
	{
		return Simple_BWThreshold(pIn, pInMask, pOut, iSizeX, iSizeY, fAcceptMin, fAcceptMax);
	}

	// HERE pitch are expresses in Pixel NOT IN BYTES !!!!
	if (nPitchIN == 0 || nPitchIN < iSizeX)
		nPitchIN = iSizeX;
	if (nPitchOUT == 0 || nPitchOUT < iSizeX)
		nPitchOUT = iSizeX;

	const unsigned long nx= iSizeX;
	const unsigned long ny= iSizeY;

	if(nx <= 0L || ny <= 0L)
		return -2;

	//échantillonnage / sampling
	unsigned long k;
	unsigned long iStep,jStep,/*kStep,*/kStep0;	//index image entiere (colonne, ligne, total)
	unsigned long Step= (unsigned long) nSampleStep;
	unsigned long FitOrder = (unsigned long) nOrder;
	const unsigned long nnx=(nx-1L)/Step+1L;
	const unsigned long nny=(ny-1L)/Step+1L;
	
	float* pSamplesIn = nullptr;
	byte* pSamplesInMsk = nullptr;
	double* pMatResSurf = nullptr;
	
	int nPichtIN_SAMPLE = nPitchIN;
	int nPichtMASK_SAMPLE = nPitchOUT;

	if(nSampleStep <= 1)
	{
		pSamplesIn = pIn;
		pSamplesInMsk = pInMask;
	}
	else
	{
		pSamplesIn = new float[nny * nnx];
		nPichtIN_SAMPLE = nnx;
		bUseSampleBuff = true;
		if(pInMask != nullptr)
		{
			pSamplesInMsk = new byte[nny * nnx];
			nPichtMASK_SAMPLE = nnx;
			bool bUseMask = true;

			//attention:il est probablement couteux en temps de fitter l'ensemble des données
			//faire un échantillonnage pour obtenir la fonction de fit
			kStep0=0L;k=0L;

			float* pPtrLineIN = pIn;
			byte* pPtrLineINMASK = pInMask; 

			for(jStep=0L; jStep<ny; jStep+=Step)
			{
				//kStep=kStep0;
				for(iStep=0L; iStep<nx; iStep+=Step)
				{
					//pSamplesInMsk[k]= pInMask[kStep];
					//pSamplesIn[k]=  pIn[kStep];
					pSamplesInMsk[k] = pPtrLineINMASK[iStep];
					pSamplesIn[k] = pPtrLineIN[iStep];
					k++;
					//kStep+=Step;
				}
				pPtrLineIN += Step * nPitchIN;
				pPtrLineINMASK += Step * nPitchOUT;
				//kStep0+=nx*Step;
			}
		}
		else
		{
			//attention:il est probablement couteux en temps de fitter l'ensemble des données
			//faire un échantillonnage pour obtenir la fonction de fit
			float* pPtrLineIN = pIn;

			kStep0=0L;k=0L;
			for(jStep=0L; jStep<ny; jStep+=Step)
			{
				//kStep=kStep0;
				for(iStep=0L; iStep<nx; iStep+=Step)
				{
					//pSamplesIn[k]=  pIn[kStep];
					pSamplesIn[k] = pPtrLineIN[iStep];
					k++;
					//kStep+=Step;
				}
				pPtrLineIN += Step * nPitchIN;
				//kStep0+=nx*Step;
			}
		}
	}

	//calcul du polynome
	//MatResSurf contient les coef du polynome
	if(0 != BestFitSurf(pSamplesIn, pSamplesInMsk, &pMatResSurf, nOrder, nnx, nny, nPichtIN_SAMPLE, nPichtMASK_SAMPLE))
	{
		if(bUseMask && (pSamplesInMsk != nullptr ))
			delete[] pSamplesInMsk;
		if(bUseSampleBuff && (pSamplesIn!= nullptr))
			delete[] pSamplesIn;

		return 1;
	}

	// Comparaison hauteur estimé vs realité
	unsigned long x,y;//coordonnées dans un tableau
	double dx,dy; //les memes en double
	long index;
	unsigned long pow_i,pow_j;//puissance
	double ze; // hauteur de la surface fitté
	double Yj;
	double YjXi;
	long ind_k,ind_k0;//index dans MatResSurf

	float* pLineIN = pIn;
	byte* pLineOUT = pOut;

	float fBorneMax = 0.0f;
	float fBorneMin = 0.0f;
	unsigned long uOrder = (unsigned long)nOrder;
	for (y=0L,index=0L,ind_k0=0L;y<ny;y++) 
	{
		dy=(double)y/double(Step);
		for (x=0L;x<nx;x++,index++) 
		{
			ze=0.0;
			ind_k0=0L;
			dx=(double)x/(double)Step;
			Yj=1.0f;
			for (pow_j=0L;pow_j<=uOrder;pow_j++)
			{
				ind_k=ind_k0;
				YjXi=Yj;
				for (pow_i=0L;pow_i<=uOrder-pow_j;pow_i++)
				{				
					ze+=YjXi*pMatResSurf[ind_k];
					YjXi *= dx;
					ind_k++;
				}
				Yj *= dy;
				ind_k0+=(nOrder+1L);
			}

			// Limit Max
			fBorneMax = fAcceptMax + (float)ze;
			fBorneMin = fAcceptMin + (float)ze;
			
			//pOut[index] = (byte)((pIn[index] >= fBorneMin) && (pIn[index] <= fBorneMax));
			pLineOUT[x] = (byte)((pLineIN[x] >= fBorneMin) && (pLineIN[x] <= fBorneMax));
		}

		pLineIN += nPitchIN;
		pLineOUT += nPitchOUT;
	}

	if(pMatResSurf != nullptr)
		delete[] pMatResSurf;
	if(bUseMask && (pSamplesInMsk != nullptr ))
		delete[] pSamplesInMsk;
	if(bUseSampleBuff && (pSamplesIn != nullptr))
		delete[] pSamplesIn;

	return 0;
}

int BestFitSurface(float* pIn, byte* pInMask, float* pOut, int iSizeX, int iSizeY, int nSampleStep, int nOrder, int nPitchIN = 0, int nPitchMASK = 0)
{
	// HERE pitch are expresses in Pixel NOT IN BYTES !!!!
	if (nPitchIN == 0 || nPitchIN < iSizeX)
		nPitchIN = iSizeX;
	if (nPitchMASK == 0 || nPitchMASK < iSizeX)
		nPitchMASK = iSizeX;

	bool bUseSampleBuff = false;
	bool bUseMask = false;
	if(nOrder <= 0)
	{
		if (pIn != pOut)
		{
		
			UINT gBufSize = nPitchIN * iSizeY;
			std::copy(pIn, pIn + gBufSize, pOut);

		/*	float* pPtrLineIN = pIn;
			float* pPtrLineOUT = pOut; 
			for (int j = 0; j < iSizeY; j++)
			{
				for (int i = 0; i < iSizeX; i++)
				{
					pPtrLineOUT[i] = pPtrLineIN[i];
				}
				pPtrLineIN += nPitchIN;
				pPtrLineOUT += nPitchIN;
			}*/
		}
		return 0;
	}

	const unsigned long nx= iSizeX;
	const unsigned long ny= iSizeY;
	if(nx <= 0L || ny <= 0L)
		return -2;

	//échantillonnage / sampling
	unsigned long k;
	unsigned long iStep,jStep,/*kStep,*/kStep0;	//index image entiere (colonne, ligne, total)
	unsigned long Step= (unsigned long) nSampleStep;
	unsigned long FitOrder = (unsigned long) nOrder;
	const unsigned long nnx=(nx-1L)/Step+1L;
	const unsigned long nny=(ny-1L)/Step+1L;

	float* pSamplesIn = nullptr;
	byte* pSamplesInMsk = nullptr;
	double* pMatResSurf = nullptr;


	int nPichtIN_SAMPLE = nPitchIN;
	int nPichtMASK_SAMPLE = nPitchMASK;

	if(nSampleStep <= 1)
	{
		pSamplesIn = pIn;
		pSamplesInMsk = pInMask;
	}
	else
	{
		pSamplesIn = new float[nny * nnx];
		nPichtIN_SAMPLE = nnx;
		bUseSampleBuff = true;
		if(pInMask != nullptr)
		{
			pSamplesInMsk = new byte[nny * nnx];
			nPichtMASK_SAMPLE = nnx;
			bool bUseMask = true;

			//attention:il est probablement couteux en temps de fitter l'ensemble des données
			//faire un échantillonnage pour obtenir la fonction de fit
			kStep0=0L;k=0L;

			float* pPtrLineIN = pIn;
			byte* pPtrLineINMASK = pInMask;

			for(jStep=0L; jStep<ny; jStep+=Step)
			{
				//kStep=kStep0;
				for(iStep=0L; iStep<nx; iStep+=Step)
				{
					//pSamplesInMsk[k]= pInMask[kStep];
					//pSamplesIn[k]=  pIn[kStep];
					pSamplesInMsk[k] = pPtrLineINMASK[iStep];
					pSamplesIn[k] = pPtrLineIN[iStep];
					k++;
					//kStep+=Step;
				}
				pPtrLineIN += Step * nPitchIN;
				pPtrLineINMASK += Step * nPitchMASK;
				//kStep0+=nx*Step;
			}
		}
		else
		{
			//attention:il est probablement couteux en temps de fitter l'ensemble des données
			//faire un échantillonnage pour obtenir la fonction de fit
			float* pPtrLineIN = pIn;

			kStep0=0L;k=0L;
			for(jStep=0L; jStep<ny; jStep+=Step)
			{
				//kStep=kStep0;
				for(iStep=0L; iStep<nx; iStep+=Step)
				{
					//pSamplesIn[k]=  pIn[kStep];
					pSamplesIn[k] = pPtrLineIN[iStep];
					k++;
					//kStep+=Step;
				}
				pPtrLineIN += Step * nPitchIN;
				//kStep0+=nx*Step;
			}
		}
	}

	//calcul du polynome
	//MatResSurf contient les coef du polynome
	if (0 != BestFitSurf(pSamplesIn, pSamplesInMsk, &pMatResSurf, nOrder, nnx, nny, nPichtIN_SAMPLE, nPichtMASK_SAMPLE))
	{
		if(bUseMask && (pSamplesInMsk != nullptr ))
			delete[] pSamplesInMsk;
		if(bUseSampleBuff && (pSamplesIn!= nullptr))
			delete[] pSamplesIn;
		return 1;
	}

	// Generation Surface estimé
	unsigned long x,y;//coordonnées dans un tableau
	double dx,dy; //les memes en double
	long index;
	unsigned long pow_i,pow_j;//puissance
	double ze; // hauteur de la surface fitté
	double Yj;
	double YjXi;
	long ind_k,ind_k0;//index dans MatResSurf

	float* pLineOUT = pOut;

	unsigned long uOrder = (unsigned long)nOrder;
	for (y=0L,index=0L,ind_k0=0L;y<ny;y++) 
	{
		dy=(double)y/double(Step);
		for (x=0L;x<nx;x++,index++) 
		{
			ze=0.0;
			ind_k0=0L;
			dx=(double)x/(double)Step;
			Yj=1.0f;
			for (pow_j=0L;pow_j<=uOrder;pow_j++)
			{
				ind_k=ind_k0;
				YjXi=Yj;
				for (pow_i=0L;pow_i<=uOrder-pow_j;pow_i++)
				{				
					ze+=YjXi*pMatResSurf[ind_k];
					YjXi *= dx;
					ind_k++;
				}
				Yj *= dy;
				ind_k0+=(nOrder+1L);
			}
			//pOut[index] = (float)ze;
			pLineOUT[x] = (float)ze;
		}
		pLineOUT += nPitchIN; // ici picth IN et OUT on le même pitch car même taille ? --- 
	}

	if(pMatResSurf != nullptr)
		delete[] pMatResSurf;
	if(bUseMask && (pSamplesInMsk != nullptr ))
		delete[] pSamplesInMsk;
	if(bUseSampleBuff && (pSamplesIn != nullptr))
		delete[] pSamplesIn;

	return 0;
}

int BestFitFlattenSurface(float* pIn, byte* pInMask, float* pOut, int iSizeX, int iSizeY, int nSampleStep, int nOrder, int nPitchIN = 0, int nPitchMASK = 0)
{

	
	// HERE pitch are expresses in Pixel NOT IN BYTES !!!!
	if (nPitchIN == 0 || nPitchIN < iSizeX)
		nPitchIN = iSizeX;
	if (nPitchMASK == 0 || nPitchMASK < iSizeX)
		nPitchMASK = iSizeX;
// 
// 	float* pPtr = pIn;
// 	int jStep2 = 2;
// 	int  iStep2 = 3;
// 	for (int j = 0; j < iSizeY; j += jStep2)
// 	{
// 		for (int i = 0; i < iSizeX; i += iStep2)
// 		{
// 			pPtr[i] = 2.1f;
// 		}
// 		pPtr += jStep2 * nPitchIN;
// 	}

	bool bUseSampleBuff = false;
	bool bUseMask = false;
	if (nOrder <= 0)
	{
		if (pIn != pOut)
		{
			UINT gBufSize = nPitchIN * iSizeY;
			std::copy(pIn, pIn + gBufSize, pOut);

/*			float* pPtrLineIN = pIn;
			float* pPtrLineOUT = pOut;
			for (int j = 0; j < iSizeY; j++)
			{
				for (int i = 0; i < iSizeX; i++)
				{
					pPtrLineOUT[i] = pPtrLineIN[i];
				}
				pPtrLineIN += nPitchIN;
				pPtrLineOUT += nPitchIN;
			}*/
		}
		return 0;
	}

	const unsigned long nx = iSizeX;
	const unsigned long ny = iSizeY;
	if (nx <= 0L || ny <= 0L)
		return -2;

	//échantillonnage / sampling
	unsigned long k;
	unsigned long iStep, jStep, /*kStep,*/ kStep0;	//index image entiere (colonne, ligne, total)
	unsigned long Step = (unsigned long)nSampleStep;
	unsigned long FitOrder = (unsigned long)nOrder;
	const unsigned long nnx = (nx - 1L) / Step + 1L;
	const unsigned long nny = (ny - 1L) / Step + 1L;

	float* pSamplesIn = nullptr;
	byte* pSamplesInMsk = nullptr;
	double* pMatResSurf = nullptr;


	int nPichtIN_SAMPLE = nPitchIN;
	int nPichtMASK_SAMPLE = nPitchMASK;

	if (nSampleStep <= 1)
	{
		pSamplesIn = pIn;
		pSamplesInMsk = pInMask;
	}
	else
	{
		pSamplesIn = new float[nny * nnx];
		nPichtIN_SAMPLE = nnx;
		bUseSampleBuff = true;
		if (pInMask != nullptr)
		{
			pSamplesInMsk = new byte[nny * nnx];
			nPichtMASK_SAMPLE = nnx;
		   bool bUseMask = true;

			//attention:il est probablement couteux en temps de fitter l'ensemble des données
			//faire un échantillonnage pour obtenir la fonction de fit
			kStep0 = 0L; k = 0L;

			float* pPtrLineIN = pIn;
			byte* pPtrLineINMASK = pInMask;

			for (jStep = 0L; jStep < ny; jStep += Step)
			{
				//kStep=kStep0;
				for (iStep = 0L; iStep < nx; iStep += Step)
				{
					//pSamplesInMsk[k]= pInMask[kStep];
					//pSamplesIn[k]=  pIn[kStep];
					pSamplesInMsk[k] = pPtrLineINMASK[iStep];
					pSamplesIn[k] = pPtrLineIN[iStep];
					k++;
					//kStep+=Step;
				}
				pPtrLineIN += Step * nPitchIN;
				pPtrLineINMASK += Step * nPitchMASK;
				//kStep0+=nx*Step;
			}
		}
		else
		{
			//attention:il est probablement couteux en temps de fitter l'ensemble des données
			//faire un échantillonnage pour obtenir la fonction de fit
			float* pPtrLineIN = pIn;

			kStep0 = 0L; k = 0L;
			for (jStep = 0L; jStep < ny; jStep += Step)
			{
				//kStep=kStep0;
				for (iStep = 0L; iStep < nx; iStep += Step)
				{
					//pSamplesIn[k]=  pIn[kStep];
					pSamplesIn[k] = pPtrLineIN[iStep];
					k++;
					//kStep+=Step;
				}
				pPtrLineIN += Step * nPitchIN;
				//kStep0+=nx*Step;
			}
		}
	}

	//calcul du polynome
	//MatResSurf contient les coef du polynome
	if (0 != BestFitSurf(pSamplesIn, pSamplesInMsk, &pMatResSurf, nOrder, nnx, nny, nPichtIN_SAMPLE, nPichtMASK_SAMPLE))
	{
		if (bUseMask && (pSamplesInMsk != nullptr))
			delete[] pSamplesInMsk;
		if (bUseSampleBuff && (pSamplesIn != nullptr))
			delete[] pSamplesIn;
		return 1;
	}

	// Generation Surface estimé
	unsigned long x, y;//coordonnées dans un tableau
	double dx, dy; //les memes en double
	long index;
	unsigned long pow_i, pow_j;//puissance
	double ze; // hauteur de la surface fitté
	double Yj;
	double YjXi;
	long ind_k, ind_k0;//index dans MatResSurf

	float* pLineIN = pIn;
	float* pLineOUT = pOut;

	unsigned long uOrder = (unsigned long)nOrder;
	for (y = 0L, index = 0L, ind_k0 = 0L; y < ny; y++)
	{
		dy = (double)y / double(Step);
		for (x = 0L; x < nx; x++, index++)
		{
			ze = 0.0;
			ind_k0 = 0L;
			dx = (double)x / (double)Step;
			Yj = 1.0f;
			for (pow_j = 0L; pow_j <= uOrder; pow_j++)
			{
				ind_k = ind_k0;
				YjXi = Yj;
				for (pow_i = 0L; pow_i <= uOrder - pow_j; pow_i++)
				{
					ze += YjXi*pMatResSurf[ind_k];
					YjXi *= dx;
					ind_k++;
				}
				Yj *= dy;
				ind_k0 += (nOrder + 1L);
			}
			//pOut[index] = (float)ze;
			// pIn et Pout peuvent pointé vers le même buffer, je préfere pour eviter les blagues passwer par une variable pour le calcul
			float FlattenHeight = pLineIN[x] - (float)ze;
			pLineOUT[x] = FlattenHeight;
		}
		pLineIN += nPitchIN;
		pLineOUT += nPitchIN; // ici picth IN et OUT on le même pitch car même taille ? --- 
	}

	if (pMatResSurf != nullptr)
		delete[] pMatResSurf;
	if (bUseMask && (pSamplesInMsk != nullptr))
		delete[] pSamplesInMsk;
	if (bUseSampleBuff && (pSamplesIn != nullptr))
		delete[] pSamplesIn;

	return 0;
}