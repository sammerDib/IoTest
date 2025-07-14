#include "StdAfx.h"
#include "ImageFlt32.h"
#include "float.h"
#include "math.h"

#include <stdio.h>
#include <stdlib.h>

//#include "MessageDefineFile.h"
//#include "ErrorDefineFile.h"

CImageFlt32::CImageFlt32(unsigned long _ny,unsigned long _nx, float * pf):nx(_nx),ny(_ny)
{
	if(pf==NULL){
		m_bExternInitialisation=false;

		if(nx*ny==0L){
			m_pData=NULL;
			nx=ny=0L;
		}
		else
			m_pData=new float[nx*ny];
	}
	else{
		m_bExternInitialisation=true;
		m_pData=pf;
	}
}

CImageFlt32::~CImageFlt32(void)
{
	if(m_bExternInitialisation==false)
		if(m_pData!=NULL)
			delete[]m_pData;
}

bool CImageFlt32::ReAlloc(unsigned long _ny,unsigned long _nx, float * pf)
{
	if(m_bExternInitialisation){
		//on ne modifie pas les données existantes
		if(_nx*_ny==0L){
			m_pData=NULL;
			nx=ny=0L;
		}
		else{
			m_pData=new float[_nx*_ny];
			if(m_pData==NULL){
				nx=ny=0L;
				//SetLastTestError(H3_IM_FLT_REALLOC_ERROR);
				return false;
			}
			else{
				nx=_nx;
				ny=_ny;
				return true;
			}
		}
	}
	else{
		if(_ny*_nx!=ny*nx || pf!=NULL){
			delete[] m_pData;
			m_pData=NULL;
		}
		if(pf==NULL){
			m_bExternInitialisation=false;

			if(_nx*_ny==0L){
				delete[]m_pData;
				m_pData=NULL;
				nx=ny=0L;
				return true;
			}
			else{
				if(m_pData==NULL){
					m_pData=new float[_nx*_ny];
					if(m_pData==NULL){
						nx=ny=0L;
						//SetLastTestError(H3_IM_FLT_REALLOC_ERROR);
						return false;
					}
					else{
						nx=_nx;
						ny=_ny;
						return true;
					}
				}
				//sinon:on ne modifie pas le tableau car il est de la bonne taille		
			}
		}
		else{
			m_bExternInitialisation=true;
			m_pData=pf;
			nx=_nx;
			ny=_ny;

			return true;
		}
	}
	//on ne passe jamais par la, normalement
	//SetLastTestError(H3_IM_FLT_REALLOC_ERROR);
	return false;
}
bool CImageFlt32::GetStat(float& min,float& max,float& mean, float& std, const CImageByte* const pMask)const
{
	long i=nx*ny,j=0L;

	std=0.0f;
	mean=0.0f;

	float val;
	unsigned long nb=0L;

	if(pMask==NULL)
	{
		max=min=m_pData[0L];
		while(i--)
		{
			val=m_pData[j];
			std  += val*val;
			mean += val;
			nb++;
			min=__min(val,min);
			max=__max(val,max);
			j++;
		}
		mean/=nb;
		std/=nb;
		std=sqrt(std-mean*mean);
	}
	else
	{
		if(pMask->nCo !=nx || pMask->nLi !=ny){
			//SetLastTestError(H3_IM_FLT_GETSTAT1_ERROR);
			return false;
		}

		min=FLT_MAX;
		max=-min;
		while(i--)
		{
			if(pMask->pData[j])
			{
				val=m_pData[j];
				std  += val*val;
				mean += val;
				nb++;
				min=__min(val,min);
				max=__max(val,max);
			}
			j++;
		}
		if(min>max){
			//SetLastTestError(H3_IM_FLT_GETSTAT2_ERROR);
			return false;
		}

		mean/=nb;
		std/=nb;
		std=sqrt(std-mean*mean);
	}
	return true;
}

////////////////////////////////////////////////////////////////  CImageByte
//CImageByte::CImageByte(unsigned long _ny,unsigned long _nx, BYTE * pf):nx(_nx),ny(_ny)
//{
//	if(pf==NULL){
//		m_bExternInitialisation=false;
//
//		if(nx*ny==0L){
//			m_pData=NULL;
//			nx=ny=0L;
//		}
//		else
//			m_pData=new BYTE[nx*ny];
//	}
//	else{
//		m_bExternInitialisation=true;
//		m_pData=pf;
//	}
//}
//
//CImageByte::~CImageByte(void)
//{
//	if(m_bExternInitialisation==false)
//		if(m_pData!=NULL)
//			delete[]m_pData;
//}
//
//bool CImageByte::ReAlloc(unsigned long _ny, unsigned long _nx, BYTE * pf)
//{
//	if(m_bExternInitialisation)
//	{
//		//on ne modifie pas les données existantes
//		if(_nx*_ny==0L)
//		{
//			m_pData=NULL;
//			nx=ny=0L;
//		}
//		else
//		{
//			m_pData=new BYTE[_nx*_ny];
//			if(m_pData==NULL)
//			{
//				nx=ny=0L;
//				//SetLastTestError(H3_IM_BYT_REALLOC_ERROR);
//				return false;
//			}
//			else
//			{
//				nx=_nx;
//				ny=_ny;
//				return true;
//			}
//		}
//	}
//	else
//	{
//		if(_ny*_nx!=ny*nx || pf!=NULL)
//		{
//			delete[] m_pData;
//			m_pData=NULL;
//		}
//		if(pf==NULL)
//		{
//			m_bExternInitialisation=false;
//
//			if(_nx*_ny==0L)
//			{
//				delete[]m_pData;
//				m_pData=NULL;
//				nx=ny=0L;
//				return true;
//			}
//			else
//			{
//				if(m_pData==NULL)
//				{
//					m_pData=new BYTE[_nx*_ny];
//					if(m_pData==NULL)
//					{
//						nx=ny=0L;
//						//SetLastTestError(H3_IM_BYT_REALLOC_ERROR);
//						return false;
//					}
//					else
//					{
//						nx=_nx;
//						ny=_ny;
//						return true;
//					}
//				}
//				//sinon:on ne modifie pas le tableau car il est de la bonne taille		
//			}
//		}
//		else
//		{
//			m_bExternInitialisation=true;
//			m_pData=pf;
//			nx=_nx;
//			ny=_ny;
//
//			return true;
//		}
//	}
//	//on ne passe jamais par là, normalement
//	//SetLastTestError(H3_IM_BYT_REALLOC_ERROR);
//	return false;
//}
//
//void CImageByte::Fill(const BYTE b)
//{
//	if(m_pData!=NULL)
//	{
//		long i=nx*ny;
//		BYTE *pb=m_pData;
//		while(i--)
//			(*(pb++))=b;
//	}
//}


bool CImageFlt32::H3SaveData(LPCTSTR strFileName) const
{
	FILE *pStream=NULL;

//	if (pStream=fopen((char *)(LPCTSTR)strFileName,"wb"))//modif cv070907 pour eviter "warning C4706"
	errno_t err =0;
	if( (err=fopen_s(&pStream, (char *) strFileName,"wb")) == 0)
	{
		::fwrite(&nx,sizeof(nx),1,pStream);
		::fwrite(&ny,sizeof(ny),1,pStream);

		//Data.fSaveRAW(Stream);

		fwrite(m_pData,sizeof(float),nx*ny,pStream);
		fclose(pStream);
	}
	return (err == 0);
}