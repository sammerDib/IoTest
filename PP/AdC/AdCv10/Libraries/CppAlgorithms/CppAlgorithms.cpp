#include "CppAlgorithms.h"
#include <windows.h>

#include "ctmf.h"
#include "IntrinsecFunctions.h"

#include "MedFloatFilterTmpl.h"

int InitLoadCall()
{
	int nCOunt = 0;
	nCOunt++;
	return 0;
}


//--------------------------------------------------------------------
//-------------------- Filtre de dentisté local ----------------------
//--------------------------------------------------------------------
//====================================================================
// Convolution par un masque de taille variable. Elimination des  
// points centraux du masque de convolution Si la densité de point 
// blanc totale est inférieure à un à un seuil paramétrable. 
//====================================================================
int PerformDensity(unsigned char* pIn, unsigned char* pOut, int iPitchIn, int iSizeX, int iSizeY, int MaskSize, int SignificantDensity)
{
	if (MaskSize <= 0)
		return -1;
	int m_iMaskSize = MaskSize;
	int m_iSignificantDensity = SignificantDensity;

	BYTE** pTmpArray_In = new BYTE*[iSizeX];
	BYTE** pTmpArray_Out = new BYTE*[iSizeX];
	for (int i = 0; i<iSizeX; i++)
	{
		pTmpArray_In[i] = new BYTE[iSizeY];
		pTmpArray_Out[i] = new BYTE[iSizeY];
	}

	for (int i = 0; i < iSizeX; i++)
	{
		for (int j = 0; j < iSizeY; j++)
		{
			pTmpArray_In[i][j] = pIn[j*iPitchIn + i];
			pTmpArray_Out[i][j] = pIn[j*iPitchIn + i];
		}
	}

	for (int ySize = 0; ySize < iSizeY - m_iMaskSize; ySize++)
	{
		int iDensityLocal = 0;
		// initialisation du mask callé à gauche

		for (int i = 0; i < m_iMaskSize; i++)
		{
			for (int j = 0; j < m_iMaskSize; j++)
			{
				if (pTmpArray_In[i][j + ySize] != 0)
				{
					iDensityLocal++;
				}
			}
		}

		for (int xSize = 0; xSize < iSizeX - m_iMaskSize; xSize++)
		{
			for (int j = ySize; j < ySize + m_iMaskSize; j++)
			{
				if (pTmpArray_In[xSize][j] != 0)
				{
					iDensityLocal--;
				}

				if (pTmpArray_In[xSize + m_iMaskSize - 1][j] != 0)
				{
					iDensityLocal++;
				}
			}

			// si zone locale non significative, on élimine les point blanc du centre
			if (iDensityLocal < m_iSignificantDensity)
			{
				for (int i = xSize + ((int)((double)m_iMaskSize / 4)); i < xSize + m_iMaskSize - ((int)((double)m_iMaskSize / 4)); i++)
				{
					for (int j = ySize + ((int)((double)m_iMaskSize / 4)); j < ySize + m_iMaskSize - ((int)((double)m_iMaskSize / 4)); j++)
					{
						pTmpArray_Out[i][j] = 0;
					}
				}
			}
		}
	}

	// copie l'image
	for (int i = 0; i<iSizeX; i++)
	{
		for (int j = 0; j<iSizeY; j++)
		{
			pOut[i + j * iSizeX] = pTmpArray_Out[i][j];
		}
	}

	for (int i = 0; i<iSizeX; i++)
	{
		delete pTmpArray_In[i];
		delete pTmpArray_Out[i];
	}
	delete[] pTmpArray_In;
	delete[] pTmpArray_Out;

	return 0;
}

//--------------------------------------------------------------------
//-------------------- Filtre de rognage ----------------------
//--------------------------------------------------------------------
//====================================================================
// Convolution par un masque de taille 3*3. Elimination des  
// points centraux du masque de convolution si le nombre de points 
// blanc voisin est inférieure à un à un seuil paramétrable. 
//====================================================================
int PerformRognage(unsigned char* pIn, unsigned char* pOut, int iPitchIn, int iSizeX, int iSizeY, int SignificantPixel)
{
	int iCount = 0;	

	int iValueForNeighbor;
	for (int i = 1; i < iSizeX - 1; i++)
	{
		for (int j = 1; j < iSizeY - 1; j++)
		{
			// Erosion global sur voisin proche
			iValueForNeighbor = pIn[j*iPitchIn + i + iPitchIn + 1];
			iValueForNeighbor += pIn[j*iPitchIn + i - iPitchIn - 1];
			iValueForNeighbor += pIn[j*iPitchIn + i - iPitchIn + 1];
			iValueForNeighbor += pIn[j*iPitchIn + i + iPitchIn - 1];
			iValueForNeighbor += pIn[j*iPitchIn + i + iPitchIn];
			iValueForNeighbor += pIn[j*iPitchIn + i - iPitchIn];
			iValueForNeighbor += pIn[j*iPitchIn + i + 1];
			iValueForNeighbor += pIn[j*iPitchIn + i - 1];

			if (iValueForNeighbor >= SignificantPixel * 255)
			{
				pOut[j*iSizeX + i] = pIn[j*iPitchIn + i];
			}
			else pOut[j*iSizeX + i] = 0;
		}


	}

	return 0;
}

//--------------------------------------------------------------------
//-------------------- Filtre de lissage ----------------------
//--------------------------------------------------------------------
//====================================================================
// Convolution par un masque de taille 3*3. Le point central est 
// remplacé par la moyenne de ses 8 voisins
//====================================================================
int PerformSmoothing(unsigned char* pIn, unsigned char* pOut, int iPitchIn, int iSizeX, int iSizeY)
{
	BYTE** pTmpArray_In = new BYTE*[iSizeX];
	BYTE** pTmpArray_Out = new BYTE*[iSizeX];
	for (int i = 0; i < iSizeX; i++)
	{
		pTmpArray_In[i] = new BYTE[iSizeY];
		pTmpArray_Out[i] = new BYTE[iSizeY];
	}

	for (int i = 0; i < iSizeX; i++)
	{
		for (int j = 0; j < iSizeY; j++)
		{
			pTmpArray_In[i][j] = pIn[j*iPitchIn + i];
			pTmpArray_Out[i][j] = pIn[j*iPitchIn + i];
		}
	}

	int iCount = 0;

	int iValueForNeighbor;
	for (int i = 1; i < iSizeX - 1; i++)
	{
		for (int j = 1; j < iSizeY - 1; j++)
		{
			// Erosion global sur voisin proche
			iValueForNeighbor = pTmpArray_In[i + 1][j + 1];
			iValueForNeighbor += pTmpArray_In[i - 1][j - 1];
			iValueForNeighbor += pTmpArray_In[i - 1][j + 1];
			iValueForNeighbor += pTmpArray_In[i + 1][j - 1];
			iValueForNeighbor += pTmpArray_In[i + 1][j];
			iValueForNeighbor += pTmpArray_In[i - 1][j];
			iValueForNeighbor += pTmpArray_In[i][j + 1];
			iValueForNeighbor += pTmpArray_In[i][j - 1];

			pTmpArray_Out[i][j] = iValueForNeighbor / 8;			
		}
	}

	for (int i = 0; i<iSizeX; i++)
	{
		for (int j = 0; j<iSizeY; j++)
		{
			pOut[i + j * iSizeX] = pTmpArray_Out[i][j];
		}
	}

	// 
	for (int i = 0; i<iSizeX; i++)
	{
		delete pTmpArray_In[i];
		delete pTmpArray_Out[i];
	}
	delete[] pTmpArray_In;
	delete[] pTmpArray_Out;

	return 0;
}


int PerformMedianFilter(unsigned char* pIn, unsigned char* pOut, int iPitchIn, int iPitchOut, int iSizeX, int iSizeY, int iRadius, long l2CacheMemSize)
{
	int iCount = 0;
	// we assume here that image buffer buffer are unsigned 8bit depth and one channel;
	// note that ctmf can handle RGB need to have right buffer structure (3-channel)
	ctmf(pIn, pOut, iSizeX, iSizeY, iPitchIn, iPitchOut, iRadius, 1, l2CacheMemSize);
	return 0;
}

int PerformMedianFloatFilter(void* pIn, void* pOut, int iSizeX, int iSizeY, int iRadiusX, int iRadiusY, int NbCores)
{
	float* pFltIn = (float*) pIn;
	float* pFltOut = (float*) pOut;

	median_filter_2d<float>(iSizeX, iSizeY, iRadiusX, iRadiusY, 0, NbCores, pFltIn, pFltOut);

	return 0;
}

int PerformMedianFilter_U16(void* pIn, void* pOut, int iSizeX, int iSizeY, int iRadiusX, int iRadiusY, int NbCores)
{
	UINT16* pU16In = (UINT16*)pIn;
	UINT16* pU16Out = (UINT16*)pOut;

	median_filter_2d<UINT16>(iSizeX, iSizeY, iRadiusX, iRadiusY, 0, NbCores, pU16In, pU16Out);

	return 0;
}

int PerformMedianFilter_U8(void* pIn, void* pOut, int iSizeX, int iSizeY, int iRadiusX, int iRadiusY, int NbCores)
{
	// if iRadiusX == iRadiusY use PerformMedianFilter instead, which is way more efficient on 8bits
	UINT8* pU8In = (UINT8*)pIn;
	UINT8* pU8Out = (UINT8*)pOut;

	median_filter_2d<UINT8>(iSizeX, iSizeY, iRadiusX, iRadiusY, 0, NbCores, pU8In, pU8Out);

	return 0;
}

int PerformLinearDynScaling(unsigned char* pIn, unsigned char* pOut, int iPitchIn, int iPitchOut, int iSizeX, int iSizeY, unsigned char uMin, unsigned char uMax)
{
	int iCount = 0;
	float lo = (float)uMin;
	float hi = (float)uMax;

	// attention on doit avoir uMin != uMax !!!

	float alpha = 255.0f / (hi - lo); // scale
	float beta = -lo * alpha; //shift
	if (alpha != 1.0f || beta != 0.0f)
	{
		Scaling8U(pIn, iPitchIn, pOut, iPitchOut, iSizeX, iSizeY, alpha, beta);
	}
	else
	{
		// simple copy 
		if (iPitchIn != iPitchOut)
		{
			// copy ligne à ligne
			int nHeight = iSizeY;
			unsigned char* src = pIn;
			unsigned char* dst = pOut;

			for (; nHeight--; src += iPitchIn, dst += iPitchOut)
			{
				memcpy_s(dst, iSizeX, src, iSizeX);
			}
		}
		else
		{
			//copy block
			memcpy_s(pOut, iSizeY * iPitchOut, pIn, iSizeY * iPitchIn);
		}
	}
	return 0;
 }
