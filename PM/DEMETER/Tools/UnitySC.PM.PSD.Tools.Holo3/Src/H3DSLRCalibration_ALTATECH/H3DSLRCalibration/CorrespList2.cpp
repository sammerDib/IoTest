// CorrespList2.cpp: implementation of the CCorrespList2 class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "CorrespList2.h"
#include "H3AppToolsDecl.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

static CString strModule("CorrespList2");

//test de validité des points (MetricData(3*n) et Pixel(2*n))
//renvoie dans validMetricData et validPixel les colonnes qui n'ont pas de NaN
static bool GetValidData(const CL_ARRAY2D& MetricData,const CL_ARRAY2D& Pixel,CL_ARRAY2D& validMetricData,CL_ARRAY2D& validPixel)
{
	CString strFunction("GetValidData");

	long co=MetricData.GetCo();
	if(Pixel.GetCo() != co )
	{
		H3DebugError(strModule,strFunction,"les données sont de tailles distinctes");
		return false;
	}
	if(Pixel.GetLi() != 2 )
	{
		H3DebugError(strModule,strFunction,"les données Pixel n'ont pas 2 lignes");
		return false;
	}
	if(MetricData.GetLi() != 3 )
	{
		H3DebugError(strModule,strFunction,"les données  Metric n'ont pas 3 lignes");
		return false;
	}

	CL_TYPE* px_p=Pixel.GetLine(0);
	CL_TYPE* py_p=Pixel.GetLine(1);
	CL_TYPE* px_m=MetricData.GetLine(0);
	CL_TYPE* py_m=MetricData.GetLine(1);
	CL_TYPE* pz_m=MetricData.GetLine(2);
		
	bool *isValid;
	isValid=new bool[co];

	long i,j,nbValid=0;

	for(i=0,j=0;i<co;i++)
	{
		isValid[i]=(_finite( (*(px_p++))+(*(py_p++))+(*(px_m++))+(*(py_m++))+(*(pz_m++)) )/*!=0*/ );
		j+=isValid[i];
	}

	validMetricData.ReAlloc(3,j);
	validPixel.ReAlloc(2,j);

	px_p=Pixel.GetLine(0);
	py_p=Pixel.GetLine(1);
	px_m=MetricData.GetLine(0);
	py_m=MetricData.GetLine(1);
	pz_m=MetricData.GetLine(2);

	CL_TYPE* px_vp=validPixel.GetLine(0);
	CL_TYPE* py_vp=validPixel.GetLine(1);
	CL_TYPE* px_vm=validMetricData.GetLine(0);
	CL_TYPE* py_vm=validMetricData.GetLine(1);
	CL_TYPE* pz_vm=validMetricData.GetLine(2);

	for(i=0;i<co;i++)
	{
		if(isValid[i])
		{
			(*(px_vp++))=(*px_p);
			(*(py_vp++))=(*py_p);
			(*(px_vm++))=(*px_m);
			(*(py_vm++))=(*py_m);
			(*(pz_vm++))=(*pz_m);
		}
		(px_p++);
		(py_p++);
		(px_m++);
		(py_m++);
		(pz_m++);
	}
	delete [] isValid;

	return true;
}
//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CCorrespList2::CCorrespList2()
{
	CString strFunction("CCorrespList2()");
	H3DebugInfo(strModule,strFunction,"1");

	m_nTarget=0;
	
	Err.ReAlloc(0,0);
	MetricData.ReAlloc(0,0);
	Pixel.ReAlloc(0,0);

	m_MetricMean.ReAlloc(1,3);

	m_bInitialised=false;
	m_bHinitialised=false;
	m_3D=false;
}


CCorrespList2::CCorrespList2(const CL_ARRAY2D& Pix,const CL_ARRAY2D& Metric)
{
	CString strFunction("CCorrespList2");
	H3DebugInfo(strModule,strFunction,"2");

	m_bInitialised=false;
	m_bHinitialised=false;
	long nbEl=Pix.GetCo();


	if( nbEl!=Metric.GetCo())
	{
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugWarning(strModule,strFunction,"le nombre de pixels ne correspond pas au nombre de points");
		#endif
		return;
	}

	if((Pix.GetLi()!=2)||(Metric.GetLi()!=3))
	{
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugWarning(strModule,strFunction,"les pixels ou les points sont mals dimensionnés");
		#endif
		return;
	}

	Pixel=Pix;
	MetricData=Metric;
	m_MetricMean.ReAlloc(1,3);

	if(!GetValidData(MetricData,Pixel,validMetricData,validPixel))
	{
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugWarning(strModule,strFunction,"validation impossible");
		#endif
		return;
	}

	m_nTarget=nbEl;
	ImageName.Format("No Image");

	CheckPlanarity();

	m_bInitialised=true;
}

CCorrespList2::~CCorrespList2()
{
	CString strFunction("~CCorrespList2()");

	m_MetricMean.ReAlloc(0,0);
	Err.ReAlloc(0,0);
	MetricData.ReAlloc(0,0);
	Pixel.ReAlloc(0,0);
	m_3D=false;
}

/*! 
* 	\fn      bool CCorrespList2::CheckPlanarity()
* 	\author  V Chalvidan
* 	\brief   
* 	\return  bool
* 	\remarks 
*/ 

bool CCorrespList2::CheckPlanarity()
{
	CString strFunction("CheckPlanarity"),msg;
	H3DebugInfo(strModule,strFunction,"0");

	long NbPoints=validMetricData.GetCo();

	//calcul de la valeur metrique moyenne
	CL_MATRIX Y(3,NbPoints);
	CL_MATRIX YYt(3,3);
	CL_TYPE xmean=0,ymean=0,zmean=0;
	CL_TYPE *pX,*pY,*pZ,*p_Y1,*p_Y2,*p_Y3;
	CL_MATRIX U(3,3),W(1,3),V(3,3);

	long i=NbPoints;
	pX=validMetricData.GetLine(0);
	pY=validMetricData.GetLine(1);
	pZ=validMetricData.GetLine(2);
	while(i--)
	{
		xmean += (*(pX++));
		ymean += (*(pY++));
		zmean += (*(pZ++));
	}
	xmean/=NbPoints;
	ymean/=NbPoints;
	zmean/=NbPoints;

	m_MetricMean[0]=xmean;
	m_MetricMean[1]=ymean;
	m_MetricMean[2]=zmean;

	i=NbPoints;
	pX=validMetricData.GetLine(0);
	pY=validMetricData.GetLine(1);
	pZ=validMetricData.GetLine(2);


	p_Y1=Y.GetLine(0);
	p_Y2=Y.GetLine(1);
	p_Y3=Y.GetLine(2);

	while(i--)
	{
		(*(p_Y1++)) = (*(pX++))-xmean;
		(*(p_Y2++)) = (*(pY++))-ymean;
		(*(p_Y3++)) = (*(pZ++))-zmean;
	}

	YYt=Mat_XXTrans(Y);
	YYt.SVDcmp(U,W,V);
	CL_TYPE r=W(2)/W(1);

	m_V=V;

	if(NbPoints<5)
	{
		m_3D=false;
		return true;
	}

	if(r<1e-3)
	{
		m_3D=false;
		return true;
	}
	else
	{
		m_3D=true;
		return false;
	}
}

bool CCorrespList2::Load(CString FileName)
{
	CString strFunction("Load");
	H3DebugInfo(strModule,strFunction,"");

	m_bInitialised=false;
	CL_ARRAY2D Points(0,0);
	try{	
		bool b=Points.LoadASCII(FileName);
		if(!b)
		{
			CString msg;
			msg.Format("Le fichier <%s> n'existe pas ou ne convient pas",FileName);
			
			#if defined(H3APPTOOLSDECL_H__INCLUDED_)
				AfxMessageBox(strModule+msg);
				H3DebugError(strModule,strFunction,msg);
			#else
				#if H3_CHECKALL_MODE
					AfxMessageBox(strModule + msg);
				#endif
			#endif
			AfxThrowUserException();
			return false;
		}
		else
		{
			CString msg;
			msg.Format("Le fichier <%s> contient %d lignes * %d colones",FileName,Points.GetLi(),Points.GetCo());
			H3DebugInfo(strModule,strFunction,msg);
		}
	
		if(Points.GetCo()<5)
		{
			CString msg;
			msg.Format("Le fichier <%s> contient un nombre de colonnes insuffisant (<5)",FileName);

			#if defined(H3APPTOOLSDECL_H__INCLUDED_)
				H3DebugError(strModule,strFunction,msg);
			#else
				#if H3_CHECKALL_MODE
					AfxMessageBox(strModule + msg);
				#endif
			#endif
			AfxThrowUserException();

			return false;
		}

		if(Points.GetLi()>10000)
		{
			CString msg;
			msg.Format("Le fichier <%s> contient un nombre de points trop important (>10000)",FileName);

			#if defined(H3APPTOOLSDECL_H__INCLUDED_)
				H3DebugError(strModule,strFunction,msg);
			#else
				#if H3_CHECKALL_MODE
					AfxMessageBox(strModule + msg);
				#endif
			#endif
			AfxThrowUserException();

			return false;
		}

	}
	catch(...)
	{
		return false;
	}

	//le fichier peut contenir plus de 5 colonnes quand 5 seulement sont necessaire
	size_t offset=Points.GetCo()-5;	
	m_nTarget=Points.GetLi();

	MetricData= CL_ARRAY2D(3,m_nTarget);
	Pixel=		CL_ARRAY2D(2,m_nTarget);

	CL_TYPE *pP,*pMx,*pMy,*pMz,*pPx,*pPy;
	pP=Points.GetData();
	pMx=MetricData.GetLine(0);
	pMy=MetricData.GetLine(1);
	pMz=MetricData.GetLine(2);
	pPx=Pixel.GetLine(0);
	pPy=Pixel.GetLine(1);

	size_t size=m_nTarget;

	for (size_t i=0; i<size; i++)
	{
		(*(pMx++))=(*(pP++));
		(*(pMy++))=(*(pP++));
		(*(pMz++))=(*(pP++));
		(*(pPx++))=(*(pP++));
		(*(pPy++))=(*(pP++));
		
		pP += offset;
	}
	ImageName=FileName;

	if(!GetValidData(MetricData,Pixel,validMetricData,validPixel))
		return false;

	if(CheckPlanarity())
		m_3D=0;
	else m_3D=1;

	m_bInitialised=true;
	return true;
}

bool CCorrespList2::SetPix(const H3_ARRAY2D_FLT64& Pix)
{
	size_t nbEl=Pix.GetCo();

	if( nbEl!=MetricData.GetCo())
	{
		CString strFunction("SetPix");
		CString msg;
		msg.Format("nb Points=%d au lieu de %d",nbEl,MetricData.GetCo());

		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError(strModule,strFunction,msg);
		#else
			#if H3_CHECKALL_MODE
				AfxMessageBox(strModule + msg);
			#endif
		#endif
		AfxThrowUserException();

		return false;
	}
	if((Pix.GetLi()!=2))
	{
		CString strFunction("SetPix");
		CString msg;
		msg.Format("nb Lignes=%d au lieu de 2",Pix.GetLi());

		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError(strModule,strFunction,msg);
		#else
			#if H3_CHECKALL_MODE
				AfxMessageBox(strModule + msg);
			#endif
		#endif
		AfxThrowUserException();

		return false;
	}

	Pixel=Pix;
	ImageName.Format("No Image");

	return true;
}

bool CCorrespList2::SetMetric(const H3_ARRAY2D_FLT64& Metric)
{
	size_t nbEl=Metric.GetCo();

	if( nbEl!=Pixel.GetCo())
	{
		CString strFunction("SetMetric");
		CString msg;
		msg.Format("nb Points=%d au lieu de %d",nbEl,Pixel.GetCo());

		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError(strModule,strFunction,msg);
		#else
			#if H3_CHECKALL_MODE
				AfxMessageBox(strModule + msg);
			#endif
		#endif
		AfxThrowUserException();

		return false;
	}

	if((Metric.GetLi()!=3))
	{
		CString strFunction("SetMetric");
		CString msg;
		msg.Format("nb Lignes=%d au lieu de 3",Metric.GetLi());

		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError(strModule,strFunction,msg);
		#else
			#if H3_CHECKALL_MODE
				AfxMessageBox(strModule + msg);
			#endif
		#endif
		AfxThrowUserException();

		return false;
	}

	MetricData=Metric;

	ImageName.Format("No Image");

	if(CheckPlanarity())
		m_3D=0;
	else m_3D=1;
	
	return true;
}

void CCorrespList2::operator=(const CCorrespList2& CL)
{
	//variables de base
	m_nTarget=CL.m_nTarget;
	Pixel=CL.Pixel;
	MetricData=CL.MetricData;
	validPixel =CL.validPixel;
	validMetricData =CL.validMetricData;
	ImageName=CL.ImageName;

	m_bInitialised=CL.m_bInitialised;

	//initialisées par CheckPlanarity
	m_3D=CL.m_3D;
	m_V=CL.m_V;
	m_MetricMean=CL.m_MetricMean;

	//initialisée par une fonction externe (compute_extrinsic)
	Err=CL.Err;
	H=CL.H;
	m_bHinitialised=CL.m_bHinitialised;
}