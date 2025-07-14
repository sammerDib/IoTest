// H3IOHoloMAP.h : fichier d'en-tête principal pour la DLL H3IOHoloMAP
//

#pragma once

#ifndef __AFXWIN_H__
	#error "incluez 'stdafx.h' avant d'inclure ce fichier pour PCH"
#endif

#include "resource.h"		// symboles principaux
#include "H3Array2D.h"
#include "freeImagePlus.h"

// CH3IOHoloMAPApp
// Consultez H3IOHoloMAP.cpp pour l'implémentation de cette classe
//

struct CImageByte
{
	size_t nLi;
	size_t nCo;

	BYTE* pData;
    bool _OwnsBuffer = true;

    /// <summary>
    /// Memcpy data from a freeImage format.
    /// Changes origin from bottom left (freeImage) to top left (CImageByte), and removes lines paddings.
    /// </summary>
    void CopyFrom(const fipImage* pFreeImage)
    {
        free();

        const fipImage* pSource;
        bool deleteRequired;
        switch (pFreeImage->getBitsPerPixel())
        {
            case 8:
            {
                pSource = pFreeImage;
                deleteRequired = false;
                break;
            }
            case 1:
            {
                fipImage* pCopie = new fipImage(*pFreeImage);
                pCopie->convertTo8Bits();
                pSource = pCopie;
                deleteRequired = true;
                break;
            }
            default:
            {
                throw std::runtime_error("Only 1 and 8 bits images are supported!");
            }
        }

        nLi = pSource->getHeight();
        nCo = pSource->getWidth();
        pData = new BYTE[nLi * nCo];
        _OwnsBuffer = true;

        BYTE* pTarget = pData;
        for (int l = nLi; (--l) >= 0;)// Reverse line order to change origin from bottom left (freeImage) to top left (CImageByte).
        {
            memcpy(pTarget, pSource->getScanLine(l), nCo); // Line copy, padding excluded.
            pTarget += nCo;
        }

        if (deleteRequired)
        {
            delete pSource;
        }
    }

    /// <summary>
    /// Shares the buffer of the image given (which retains ownership).
    /// </summary>
    void reference(CH3GenericArray2D* pImage)
    {
        if ((pImage->IsFloatingPoint()) || (pImage->GetTypeSize() != 8))
        {
            throw std::runtime_error("CImageByte needs 8 bit data");
        }
        free();

        nCo = pImage->GetCo();
        nLi = pImage->GetLi();
        pData = (BYTE*)(pImage->GetDataAsVoid());
        _OwnsBuffer = false;
    }

	CImageByte::CImageByte()
	{
		nCo=nLi=0;
		pData=nullptr;
	}

    void free()
    {
        nCo = nLi = 0;
        if (pData != nullptr)
        {
            if (_OwnsBuffer)
            {
                delete[] pData;
            }
            pData = nullptr;
        }
    }

	CImageByte::~CImageByte()
	{
        free();
	}

	CImageByte & CImageByte::operator =(const CImageByte & Src)
	{
		if (this==&Src) return *this;

		if(pData != nullptr)
			delete [] pData;
		pData=new BYTE [Src.nLi*Src.nCo];
		memcpy(pData, Src.pData, sizeof(Src.pData));

		if(pData!=nullptr){
			nLi=Src.nLi;
			nCo=Src.nCo;
		}
		else
			nLi=nCo=0;

		return *this;
	}

	bool Load(const CString& FileName){
		bool bRetValue=false;
		//FILE *Stream=fopen(FileName,"rb");

		FILE *Stream=NULL;
		_tfopen_s(&Stream, FileName, _T("rb")) ;

		if (Stream)
		{
			//unsigned long nCo;
			if (fread(&nCo,sizeof(__int32),1,(FILE *)Stream)!=1){
				fclose(Stream);
				return false;
			}
			//unsigned long nLi;
			if (fread(&nLi,sizeof(__int32),1,(FILE *)Stream)!=1){
				fclose(Stream);
				return false;
			}
			pData=new BYTE[nLi*nCo];
			if (fread(pData,sizeof(BYTE),nLi*nCo,(FILE *)Stream)!=(nLi*nCo)){
				fclose(Stream);
				return false;
			}
		}
		else{
			return false;
		}

		fclose(Stream);
		return true;
	}
	bool ReAlloc(unsigned long _ny,unsigned long _nx, byte * pf=NULL)
   {
//	if(m_bExternInitialisation)
//	{
//		//on ne modifie pas les données existantes
//		if(_nx*_ny==0L)
//		{
//			pData=NULL;
//			nCo=nLi=0L;
//		}
//		else
//		{
//			pData=new byte[_nx*_ny];
//			if(pData==NULL)
//			{
//				nCo=nLi=0L;
//				//SetLastTestError(H3_IM_BYT_REALLOC_ERROR);
//				return false;
//			}
//			else
//			{
//				nCo=_nx;
//				nLi=_ny;
//				return true;
//			}
//		}
//	}
	//else
	{
		if(_ny*_nx!=nLi*nCo || pf!=NULL)
		{
			delete[] pData;
			pData=NULL;
		}
		if(pf==NULL)
		{
			if(_nx*_ny==0L)
			{
				delete[]pData;
				pData=NULL;
				nCo=nLi=0L;
				return true;
			}
			else
			{
				if(pData==NULL)
				{
					pData=new byte[_nx*_ny];
					if(pData==NULL)
					{
						nCo=nLi=0L;
						//SetLastTestError(H3_IM_BYT_REALLOC_ERROR);
						return false;
					}
					else
					{
						nCo=_nx;
						nLi=_ny;
						return true;
					}
				}
				//sinon:on ne modifie pas le tableau car il est de la bonne taille		
			}
		}
		else
		{
			pData=pf;
			nCo=_nx;
			nLi=_ny;
			return true;
		}
	}
	return false;
}
	void CImageByte::Fill(const byte b)
{
	if(pData!=NULL)
	{
		size_t i=nCo*nLi;
		byte *pb=pData;
		while(i--)
			(*(pb++))=b;
	}
}
};

struct CImageFloat
{
	size_t nLi;
	size_t nCo;

	float* pData;
    bool _OwnsBuffer = true;

    /// <summary>
    /// Memcpy data from a freeImage format.
    /// Changes origin from bottom left (freeImage) to top left (CImageByte), and removes lines paddings.
    /// </summary>
    void CopyFrom(const fipImage* pFreeImage)
    {
        free();

        switch (pFreeImage->getBitsPerPixel())
        {
        case 32:
            break;
        default:
            throw std::runtime_error("Only 32 bits images are supported!");
        }

        nLi = pFreeImage->getHeight();
        nCo = pFreeImage->getWidth();
        pData = new float[nLi * nCo];
        _OwnsBuffer = true;

        float* pTarget = pData;
        size_t lineSize_bytes = nCo * sizeof(float);
        for (int l = nLi; (--l) >= 0;)// Reverse line order to change origin from bottom left (freeImage) to top left (CImageByte).
        {
            memcpy(pTarget, pFreeImage->getScanLine(l), lineSize_bytes); // Line copy, padding excluded.
            pTarget += nCo;
        }
    }

    CImageFloat::CImageFloat()
    {
        nCo = nLi = 0;
        pData = nullptr;
    }

    CImageFloat::CImageFloat(float* pRawData, size_t sizeX, size_t sizeY)
    {
        nCo = sizeX;
        nLi = sizeY;
        pData = pRawData;
    }

    /// <summary>
    /// Shares the buffer of the image given (which retains ownership).
    /// </summary>
    void reference(CH3GenericArray2D* pImage)
    {
        if ((!pImage->IsFloatingPoint())||(pImage->GetTypeSize()!=32))
        {
            throw std::runtime_error("CImageFloat needs float32 data");
        }
        free();

        nCo = pImage->GetCo();
        nLi = pImage->GetLi();
        pData = (float*)(pImage->GetDataAsVoid());
        _OwnsBuffer = false;
    }

    void free()
    {
        nCo = nLi = 0;
        if (pData != nullptr)
        {
            if (_OwnsBuffer)
            {
                delete[] pData;
            }
            pData = nullptr;
        }
    }

	CImageFloat::~CImageFloat()
	{
        free();
	}

	CImageFloat & CImageFloat::operator =(const CImageFloat & Src)
	{
		if (this==&Src) return *this;

		if(pData != nullptr)
			delete [] pData;
		pData=new float [Src.nLi*Src.nCo];
		void*ptest=memcpy(pData, Src.pData, Src.nLi*Src.nCo*sizeof(float));

		if(ptest!=nullptr){
			nLi=Src.nLi;
			nCo=Src.nCo;
		}

		return *this;
	}

	bool Load(const CString& FileName){
		bool bRetValue=false;
		//FILE *Stream=fopen(FileName,"rb");

		FILE *Stream=NULL;
		_tfopen_s(&Stream, FileName, _T("rb")) ;

		if (Stream)
		{
			//unsigned long nCo;
			if (fread(&nCo,sizeof(__int32),1,(FILE *)Stream)!=1){
				fclose(Stream);
				return false;
			}

			//unsigned long nLi;
			if (fread(&nLi,sizeof(__int32),1,(FILE *)Stream)!=1){
				fclose(Stream);
				return false;
			}

			pData=new float[nLi*nCo];
			if (fread(pData,sizeof(float),nLi*nCo,(FILE *)Stream)!=(nLi*nCo)){
				fclose(Stream);
				return false;
			}
		}
		else{
			return false;
		}

		fclose(Stream);
		return true;
	}
};

class CH3IOHoloMAPApp : public CWinApp
{
public:
	CH3IOHoloMAPApp();

// Substitutions
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};