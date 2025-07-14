// label.cpp.
//
//////////////////////////////////////////////////////////////////////
#include "stdafx.h"
#include "label.h"
#include "H3BaseDef.h"


//recherche dans une image les pixels dont la valeur de gris est !=0
//groupe ces pixels avec leurs voisins s'ils existent (connexion 4 voisins)
//renvoi une cartographie sur laquelle les groupes sont coloriés avec des couleurs de 1 à n
static void bwLabel_color_fn_2colors(unsigned __int32& Color,const unsigned __int32* const pColor_1,const unsigned __int32* const pColor_2,CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	Color= (*pColor_2);

	unsigned long lColor_1;

	if((lColor_1=(*pConnectedColor)[*pColor_1])==0L)
		lColor_1=(*pColor_1);

	unsigned long lColor_2;
	if((lColor_2 =(*pConnectedColor)[*pColor_2 ])==0L)
		lColor_2 =(*pColor_2 );

	if(lColor_1!=lColor_2)
	{

		unsigned long lColorMin;
		unsigned long lColorMax;
		if(lColor_1>lColor_2){
			lColorMax=lColor_1;
			lColorMin=lColor_2;
		}
		else{
			lColorMax=lColor_2;
			lColorMin=lColor_1;
		}			

		Color=lColorMin;

		for(unsigned long k=lColorMax;k<=LastColor;k++)
		{
			if((*pConnectedColor)[k]==lColorMax)
				(*pConnectedColor)[k]=lColorMin;
		}
		(*pConnectedColor)[lColorMax]=lColorMin;
	}
}
//bwLabel_color_fn0: pas de connection
static void bwLabel_color_fn0(unsigned __int32& Color,const unsigned __int32* const pColor_left,const unsigned __int32* const pColor_top,CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	LastColor++;

	Color=LastColor;
}
//bwLabel_color_fn1: connection left
static void bwLabel_color_fn1(unsigned __int32& Color,const unsigned __int32* const pColor_left,const unsigned __int32* const pColor_top,CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	Color= (*pColor_left);
}
//bwLabel_color_fn2: connection top
static void bwLabel_color_fn2(unsigned __int32& Color,const unsigned __int32* const pColor_left,const unsigned __int32* const pColor_top,CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	Color= (*pColor_top);
}
//bwLabel_color_fn3: connection top et left
static void bwLabel_color_fn3(unsigned __int32& Color,const unsigned __int32* const pColor_left,const unsigned __int32* const pColor_top,CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	bwLabel_color_fn_2colors(Color,pColor_left,pColor_top,pConnectedColor,LastColor);
}
//bwLabel_color_fn00: pas de connection
static void bwLabel_color_fn00(unsigned __int32& Color,
							  const unsigned __int32* const pColor_left,
							  const unsigned __int32* const pColor_top,
							  const unsigned __int32* const pColor_topleft,
							  const unsigned __int32* const pColor_topright,
							  CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	LastColor++;

	Color=LastColor;
}
//bwLabel_color_fn01: connection left
static void bwLabel_color_fn01(unsigned __int32& Color,
							  const unsigned __int32* const pColor_left,
							  const unsigned __int32* const pColor_top,
							  const unsigned __int32* const pColor_topleft,
							  const unsigned __int32* const pColor_topright,
							  CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	Color= (*pColor_left);
}
//bwLabel_color_fn02: connection top
static void bwLabel_color_fn02(unsigned __int32& Color,
							  const unsigned __int32* const pColor_left,
							  const unsigned __int32* const pColor_top,
							  const unsigned __int32* const pColor_topleft,
							  const unsigned __int32* const pColor_topright,
							  CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	Color= (*pColor_top);
}
//bwLabel_color_fn03: connection top et left
static void bwLabel_color_fn03(unsigned __int32& Color,
							  const unsigned __int32* const pColor_left,
							  const unsigned __int32* const pColor_top,
							  const unsigned __int32* const pColor_topleft,
							  const unsigned __int32* const pColor_topright,
							  CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	bwLabel_color_fn_2colors(Color,pColor_left,pColor_top,pConnectedColor,LastColor);
}
//bwLabel_color_fn4: connection top left
static void bwLabel_color_fn04(unsigned __int32& Color,
							  const unsigned __int32* const pColor_left,
							  const unsigned __int32* const pColor_top,
							  const unsigned __int32* const pColor_topleft,
							  const unsigned __int32* const pColor_topright,
							  CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	Color= (*pColor_topleft);
}
static void bwLabel_color_fn05(unsigned __int32& Color,
							  const unsigned __int32* const pColor_left,
							  const unsigned __int32* const pColor_top,
							  const unsigned __int32* const pColor_topleft,
							  const unsigned __int32* const pColor_topright,
							  CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	//bwLabel_color_fn_2colors(Color,pColor_left,pColor_topright,pConnectedColor,LastColor);
	Color = (*pColor_left) ;
}
static void bwLabel_color_fn06(unsigned __int32& Color,
							  const unsigned __int32* const pColor_left,
							  const unsigned __int32* const pColor_top,
							  const unsigned __int32* const pColor_topleft,
							  const unsigned __int32* const pColor_topright,
							  CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	//bwLabel_color_fn2(Color,pColor_left,pColor_top,pConnectedColor,LastColor);
	Color = (*pColor_top) ;
}
static void bwLabel_color_fn07(unsigned __int32& Color,
							  const unsigned __int32* const pColor_left,
							  const unsigned __int32* const pColor_top,
							  const unsigned __int32* const pColor_topleft,
							  const unsigned __int32* const pColor_topright,
							  CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	//bwLabel_color_fn3(Color,pColor_left,pColor_top,pConnectedColor,LastColor);
	Color = (*pColor_left) ;
}
static void bwLabel_color_fn08(unsigned __int32& Color,
							  const unsigned __int32* const pColor_left,
							  const unsigned __int32* const pColor_top,
							  const unsigned __int32* const pColor_topleft,
							  const unsigned __int32* const pColor_topright,
							  CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	Color= (*pColor_topright);
}
static void bwLabel_color_fn09(unsigned __int32& Color,
							  const unsigned __int32* const pColor_left,
							  const unsigned __int32* const pColor_top,
							  const unsigned __int32* const pColor_topleft,
							  const unsigned __int32* const pColor_topright,
							  CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	//bwLabel_color_fn_2colors(Color,pColor_left,pColor_topleft,pConnectedColor,LastColor);
	bwLabel_color_fn_2colors(Color,pColor_left,pColor_topright,pConnectedColor,LastColor);
}
static void bwLabel_color_fn10(unsigned __int32& Color,
							  const unsigned __int32* const pColor_left,
							  const unsigned __int32* const pColor_top,
							  const unsigned __int32* const pColor_topleft,
							  const unsigned __int32* const pColor_topright,
							  CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	//bwLabel_color_fn2(Color,pColor_left,pColor_top,pConnectedColor,LastColor);
	Color = (*pColor_top) ;
}
static void bwLabel_color_fn11(unsigned __int32& Color,
							  const unsigned __int32* const pColor_left,
							  const unsigned __int32* const pColor_top,
							  const unsigned __int32* const pColor_topleft,
							  const unsigned __int32* const pColor_topright,
							  CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	//bwLabel_color_fn3(Color,pColor_left,pColor_top,pConnectedColor,LastColor);
	Color = (*pColor_left) ;
}
static void bwLabel_color_fn12(unsigned __int32& Color,
							  const unsigned __int32* const pColor_left,
							  const unsigned __int32* const pColor_top,
							  const unsigned __int32* const pColor_topleft,
							  const unsigned __int32* const pColor_topright,
							  CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	bwLabel_color_fn_2colors(Color,pColor_topleft,pColor_topright,pConnectedColor,LastColor);
}
static void bwLabel_color_fn13(unsigned __int32& Color,
							  const unsigned __int32* const pColor_left,
							  const unsigned __int32* const pColor_top,
							  const unsigned __int32* const pColor_topleft,
							  const unsigned __int32* const pColor_topright,
							  CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	//bwLabel_color_fn05(Color,pColor_left,pColor_top,pColor_topleft,pColor_topright,pConnectedColor,LastColor);
	bwLabel_color_fn_2colors(Color,pColor_topleft,pColor_topright,pConnectedColor,LastColor);
}
static void bwLabel_color_fn14(unsigned __int32& Color,
							  const unsigned __int32* const pColor_left,
							  const unsigned __int32* const pColor_top,
							  const unsigned __int32* const pColor_topleft,
							  const unsigned __int32* const pColor_topright,
							  CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	//bwLabel_color_fn08(Color,pColor_left,pColor_top,pColor_topleft, pColor_topright,pConnectedColor,LastColor);
	Color = (*pColor_topleft) ;
}
static void bwLabel_color_fn15(unsigned __int32& Color,
							  const unsigned __int32* const pColor_left,
							  const unsigned __int32* const pColor_top,
							  const unsigned __int32* const pColor_topleft,
							  const unsigned __int32* const pColor_topright,
							  CH3Array<unsigned __int32>* const pConnectedColor,unsigned long& LastColor)
{
	//bwLabel_color_fn1(Color,pColor_left,pColor_top,pConnectedColor,LastColor);
	Color = (*pColor_topleft) ;
}

typedef void (*Label_color_fn )(unsigned __int32& Color,
								const unsigned __int32* const ,
								const unsigned __int32* const ,
								CH3Array<unsigned __int32>* const ,
								unsigned long&) ;
typedef void (*Label_color_fn0)(unsigned __int32& Color,
								const unsigned __int32* const ,
								const unsigned __int32* const ,
								const unsigned __int32* const ,
								const unsigned __int32* const ,
								CH3Array<unsigned __int32>* const ,
								unsigned long&) ;


#define TEST 1//test =1 est sensé ameliorer la vitesse de calcul en ne prenant en compte que les points valides des images

bool bwLabel4(H3_ARRAY2D_UINT32* pa2dLabel,const H3_ARRAY2D_UINT8& Im);
bool bwLabel8(H3_ARRAY2D_UINT32* pa2dLabel,const H3_ARRAY2D_UINT8& Im);

bool bwLabel(H3_ARRAY2D_UINT32* pa2dLabel,const H3_ARRAY2D_UINT8& Im ,const int nbLink)
{
	return bwLabel4(pa2dLabel,Im );
}

bool bwLabel4(H3_ARRAY2D_UINT32* pa2dLabel,const H3_ARRAY2D_UINT8& Im)
{
	const size_t nx=Im.GetCo(),ny=Im.GetLi(),sz=ny*nx;
	size_t i,j,k;
	unsigned __int8 *pI=Im.GetData();

	//recherche du nombre de points dont la valeur n'est pas 0 dans l'image
	unsigned long nbValid = 0L;

	H3_ARRAY2D_UINT8 Mask(ny,nx);
	CH3Array2D< unsigned __int8 > Neigburg(ny,nx);
	Neigburg.Fill(0L);

	for(i=0;i<sz;i++)
		nbValid += (Mask[i]=(Im[i]>0L));

	if(nbValid<2)
		return false;

#if TEST
	H3_ARRAY_UINT32 ValidIndex(nbValid);
	for(i=0L,j=0L;i<sz;i++)
		if(Mask[i]) ValidIndex[j++]=i;
#endif

	//Neigburg ne considere que les voisins top et left
	//si left: Neigburg=1
	//si top : Neigburg=2 
	//si top et left : Neigburg=3
	unsigned __int8 *pN=Neigburg.GetData();
	unsigned __int8 *pM=Mask.GetData(),*pMt=pM-nx,*pMl=pM-1L;
	//le point (0,0) n'a donc pas de voisin valide
	pN++;
	pM++;pMt++;pMl++;
	//suite de la ligne n°0	(pas de top)
	for(k=1;k<nx;k++)
	{
		(*pN) += (*pMl);

		pN++;
		pMl++;
	}

	pMt=(pMl+1L-nx);
	//la suite
	for(j=1;j<ny;j++)
	{

		//debut de ligne (pas de left)
		(*pN) += (*pMt);
		(*pN) *= 2L;

		pN++;
		pMt++;
		pMl++;
		//suite de la ligne	
		for(i=1;i<nx;i++)
		{
			(*pN)  = (*pMt);
			(*pN) *= 2L;
			(*pN) += (*pMl);

			pN++;
			pMt++;
			pMl++;
		}
	}
	Neigburg*=Mask;

	//couleur
	CH3Array<unsigned __int32> ConnectedColor00(nbValid+1);
	CH3Array2D<unsigned __int32> Color00(ny,nx);//couleur affectée au kieme pixel valide, fonction de son voisinage
	Color00.Fill(0L);
	ConnectedColor00.Fill(0L);

	Label_color_fn pbwLabel_color_fn[4];
	pbwLabel_color_fn[0]=bwLabel_color_fn0;
	pbwLabel_color_fn[1]=bwLabel_color_fn1;
	pbwLabel_color_fn[2]=bwLabel_color_fn2;
	pbwLabel_color_fn[3]=bwLabel_color_fn3;

	pM=Mask.GetData();
	pMt=pM-nx;
	pMl=pM-1L;

	unsigned __int32 *pC=Color00.GetData(),*pCt=pC-nx,*pCl=pC-1L;
	pM=Mask.GetData();

	pN=Neigburg.GetData();
	unsigned long LastColor=0L;

#if TEST
	unsigned __int32* pCtmp;
	unsigned __int8 * pNtmp;
	
	pN=Neigburg.GetData();
	pC=Color00.GetData();

	for(j=0L;j<nbValid;j++)
	{
		i=ValidIndex[j];
		pCtmp=pC;
		pCtmp += i;
		pCl=pCtmp-1L;
		pCt=pCtmp-nx;

		pNtmp=pN;
		pNtmp += i;

		pbwLabel_color_fn[*pNtmp](*pCtmp,pCl,pCt,&ConnectedColor00,LastColor);

	}
#else
	for(j=0L;j<sz;j++)
	{
		if(*pM)
			pbwLabel_color_fn[*pN](*pC,pCl,pCt,&ConnectedColor00,LastColor);

		pM++;
		pN++;
		pC ++;
		pCt++;
		pCl++;
	}
#endif

	CH3Array<unsigned __int32> ConnectedColor1=ConnectedColor00.GetAt(0,LastColor+1);
	unsigned __int32 *pConnectedColor1=ConnectedColor1.GetData();

	CH3Array2D<unsigned __int32> Color10=Color00;
	unsigned __int32 *pColor00=Color00.GetData();
	unsigned __int32 *pColor10=Color10.GetData();

	long lColor;

#if TEST
	for(j=0;j<nbValid;j++)
	{
		i=ValidIndex[j];
		lColor=ConnectedColor1[Color00[i]];
		if(lColor!=0L)
			Color10[i]=lColor;
	}
#else
	for(i=0;i<sz;i++)
	{
		#if 1
			if(Mask[i]){
				lColor=ConnectedColor1[Color00[i]];
				if(lColor!=0L)
					Color10[i]=lColor;
			}
		#else
			lColor=pConnectedColor1[*pColor00];
			if(lColor!=0L)
				(*pColor10)=lColor;

			pColor00++;
			pColor10++;
		#endif
	}
#endif

	//Histogramme des couleurs utilisée
	CH3Array<long> HistoColor(LastColor+1)/*,NumColor(LastColor+1)*/;//il y a LastColor couleur + le 0
	unsigned long nbColors=0;
	HistoColor.Fill(0);
	for(i=0;i<sz;i++)
		HistoColor[Color10[i]] ++;

	for(i=1,k=0;i<=LastColor;i++){
		if(HistoColor[i] >0) (++k);
	}
	nbColors=k;

	//Re-indexation des couleurs de 1 a k
	CH3Array<unsigned __int32> ColorTrue(LastColor+1);
	ColorTrue.Fill(0);
	for(i=1,k=0;i<LastColor+1;i++)
		if(HistoColor[i]>0) ColorTrue[i]=(++k);

	nbColors=k+1;

	H3_ARRAY2D_UINT32 a2dLabel;//au cas ou pa2dLabel==NULL
	if((pa2dLabel)==nullptr){
		a2dLabel.Alloc(ny,nx);
		pa2dLabel=&a2dLabel;
	}
	else
		if(!(pa2dLabel->GetLi()==ny && pa2dLabel->GetCo()==nx))
			pa2dLabel->ReAlloc(ny,nx);

	pa2dLabel->Fill(0L);

#if TEST
	for(j=0;j<nbValid;j++)
	{
		i=ValidIndex[j];
		lColor=ColorTrue[Color10[i]];
		if(lColor!=0L)
			(*pa2dLabel)[i]=lColor;
	}
#else
	for(i=0L;i<sz;i++)
	{
		lColor=ColorTrue[Color10[i]];
		if(lColor!=0L)
			(*pa2dLabel)[i]=lColor;
	}
#endif

	return true;
}

bool bwLabel8(H3_ARRAY2D_UINT32* pa2dLabel,const H3_ARRAY2D_UINT8& Im)
{
	const unsigned long nx=Im.GetCo(),ny=Im.GetLi(),sz=ny*nx;
	unsigned long i,j,k;
	unsigned __int8 *pI=Im.GetData();

	//recherche du nombre de points dont la valeur n'est pas 0 dans l'image
	unsigned long nbValid = 0L;

	H3_ARRAY2D_UINT8 Mask(ny,nx);
	CH3Array2D< unsigned __int8 > Neigburg(ny,nx);
	Neigburg.Fill(0L);

	for(i=0;i<sz;i++)
		nbValid += (Mask[i]=(Im[i]>0L));

#if TEST
	H3_ARRAY_UINT32 ValidIndex(nbValid);
	for(i=0L,j=0L;i<sz;i++)
		if(Mask[i]) ValidIndex[j++]=i;
#endif

	//Neigburg ne considere que les voisins top et left
	//si left: Neigburg=1
	//si top : Neigburg=2 
	//si top et left : Neigburg=3
	//pour 8 voisins voir top_left et top_right
	//si topleft: Neigburg=4
	//si topright: Neigburg=8
	unsigned __int8 *pN=Neigburg.GetData();
	unsigned __int8 *pM=Mask.GetData(),*pMt=pM-nx,*pMl=pM-1L,*pMtl=pMt-1L,*pMtr=pMt+1L;
	//le point (0,0) n'a donc pas de voisin valide
	pN++;
	pM++;pMt++;pMl++;pMtl++;pMtr++;
	//suite de la ligne n°0	(pas de top)
	for(k=1;k<nx;k++)
	{
		(*pN) = (*pMl);

		pN++;
		pMl++;
	}

	pMt=(pMl+1L-nx);pMtl=pMt-1;pMtr=pMt+1;
	//la suite
	for(j=1;j<ny;j++)
	{

		//debut de ligne (pas de left)
		{
			(*pN)  = (*pMtr);
			(*pN)<<=2L;
			(*pN) += (*pMt);
			(*pN)<<=1L;

			pN++;
			pMt++;pMtr++;pMtl++;
			pMl++;
		}
		//suite de la ligne	
		for(i=1;i<nx-1;i++)
		{
			(*pN)   = (*pMtr);
			(*pN) <<= 1L;
			(*pN)  += (*pMtl);
			(*pN) <<= 1L;
			(*pN)  += (*pMt);
			(*pN) <<= 1L;
			(*pN)  += (*pMl);

			pN++;
			pMt++;pMtl++;pMtr++;
			pMl++;
		}
		//fin de la ligne //pas de r
		{
			(*pN)   = (*pMtl);
			(*pN) <<= 1L;
			(*pN)  += (*pMt);
			(*pN) <<= 1L;
			(*pN)  += (*pMl);

			pN++;
			pMt++;pMtl++;pMtr++;
			pMl++;
		}
	}
	Neigburg*=Mask;

	//couleur
	CH3Array<unsigned __int32> ConnectedColor00(nbValid+1);
	CH3Array2D<unsigned __int32> Color00(ny,nx);//couleur affectée au kieme pixel valide, fonction de son voisinage
	Color00.Fill(0L);
	ConnectedColor00.Fill(0L);

	Label_color_fn0 pbwLabel_color_fn[16];
	pbwLabel_color_fn[0]=bwLabel_color_fn00;
	pbwLabel_color_fn[1]=bwLabel_color_fn01;
	pbwLabel_color_fn[2]=bwLabel_color_fn02;
	pbwLabel_color_fn[3]=bwLabel_color_fn03;
		pbwLabel_color_fn[4]=bwLabel_color_fn04;
	pbwLabel_color_fn[5]=bwLabel_color_fn05;
	pbwLabel_color_fn[6]=bwLabel_color_fn06;
	pbwLabel_color_fn[7]=bwLabel_color_fn07;
		pbwLabel_color_fn[8]=bwLabel_color_fn08;
	pbwLabel_color_fn[9]=bwLabel_color_fn09;
	pbwLabel_color_fn[10]=bwLabel_color_fn10;
	pbwLabel_color_fn[11]=bwLabel_color_fn11;
		pbwLabel_color_fn[12]=bwLabel_color_fn12;
	pbwLabel_color_fn[13]=bwLabel_color_fn13;
	pbwLabel_color_fn[14]=bwLabel_color_fn14;
	pbwLabel_color_fn[15]=bwLabel_color_fn15;

	pM=Mask.GetData();
	pMt=pM-nx;
	pMl=pM-1L;

	unsigned __int32 *pC=Color00.GetData(),*pCt=pC-nx,*pCl=pC-1L,*pCtl=pCt-1L,*pCtr=pCt+1L;
	pM=Mask.GetData();

	pN=Neigburg.GetData();
	unsigned long LastColor=0L;

#if TEST
	unsigned __int32* pCtmp;
	unsigned __int8 * pNtmp;
	
	pN=Neigburg.GetData();
	pC=Color00.GetData();

	for(j=0L;j<nbValid;j++)
	{
		i=ValidIndex[j];
		pCtmp=pC;
		pCtmp += i;
		pCl=pCtmp-1L;
		pCt=pCtmp-nx;
		pCtl=pCt-1L;
		pCtr=pCt+1L;

		pNtmp=pN;
		pNtmp += i;

		pbwLabel_color_fn[*pNtmp](*pCtmp,pCl,pCt,pCtl,pCtr,&ConnectedColor00,LastColor);

	}
#else
	for(j=0L;j<sz;j++)
	{
		if(*pM)
			pbwLabel_color_fn[*pN](*pC,pCl,pCt,pCtl,pCtr,&ConnectedColor00,LastColor);

		pM++;
		pN++;
		pC ++;
		pCt++;
		pCl++;
	}
#endif

	CH3Array<unsigned __int32> ConnectedColor1=ConnectedColor00.GetAt(0,LastColor+1);
	unsigned __int32 *pConnectedColor1=ConnectedColor1.GetData();

	CH3Array2D<unsigned __int32> Color10=Color00;
	unsigned __int32 *pColor00=Color00.GetData();
	unsigned __int32 *pColor10=Color10.GetData();

	long lColor;

#if TEST
	for(j=0;j<nbValid;j++)
	{
		i=ValidIndex[j];
		lColor=ConnectedColor1[Color00[i]];
		if(lColor!=0L)
			Color10[i]=lColor;
	}
#else
	for(i=0;i<sz;i++)
	{
		#if 1
			if(Mask[i]){
				lColor=ConnectedColor1[Color00[i]];
				if(lColor!=0L)
					Color10[i]=lColor;
			}
		#else
			lColor=pConnectedColor1[*pColor00];
			if(lColor!=0L)
				(*pColor10)=lColor;

			pColor00++;
			pColor10++;
		#endif
	}
#endif

	//Histogramme des couleurs utilisée
	CH3Array<long> HistoColor(LastColor+1);//il y a LastColor couleur + le 0
	unsigned long nbColors=0;
	HistoColor.Fill(0);
	for(i=0;i<sz;i++)
		HistoColor[Color10[i]] ++;

	for(i=1,k=0;i<=LastColor;i++){
		if(HistoColor[i] >0) (++k);
	}
	nbColors=k;

	//Re-indexation des couleurs de 1 a k
	CH3Array<unsigned __int32> ColorTrue(LastColor+1);
	ColorTrue.Fill(0);
	for(i=1,k=0;i<LastColor+1;i++)
		if(HistoColor[i]>0) ColorTrue[i]=(++k);

	nbColors=k+1;

	H3_ARRAY2D_UINT32 a2dLabel;//au cas ou pa2dLabel==NULL
	if((pa2dLabel)==nullptr){
		a2dLabel.Alloc(ny,nx);
		pa2dLabel=&a2dLabel;
	}
	else
		if(!(pa2dLabel->GetLi()==ny && pa2dLabel->GetCo()==nx))
			pa2dLabel->ReAlloc(ny,nx);

	pa2dLabel->Fill(0L);

#if TEST
	for(j=0;j<nbValid;j++)
	{
		i=ValidIndex[j];
		lColor=ColorTrue[Color10[i]];
		if(lColor!=0L)
			(*pa2dLabel)[i]=lColor;
	}
#else
	for(i=0L;i<sz;i++)
	{
		lColor=ColorTrue[Color10[i]];
		if(lColor!=0L)
			(*pa2dLabel)[i]=lColor;
	}
#endif

	return true;
}

bool GetBlobs(H3_ARRAY_PT2DFLT32& CentresTaches,const H3_ARRAY2D_UINT32& Label,const InquiryOnBlobs * const pIOB)
{
	const size_t nx=Label.GetCo(),ny=Label.GetLi(),sz=ny*nx;
	unsigned long nbColors=0,i,j,k,l,lColor;

	for(i=0;i<sz;i++)
	{
		lColor=Label[i];
		if(lColor>nbColors)
			nbColors=lColor;
	}
	nbColors++;//de 0 à nbColors il y a nbColors+1

	//Recherche des centres des taches
	CentresTaches.ReAlloc(nbColors);
	for(i=0;i<nbColors;i++){
		CentresTaches[i].x=CentresTaches[i].y=0;
	}

	H3_ARRAY_UINT32 Area(nbColors);
	Area.Fill(0L);
	for(i=0L,k=0L;i<ny;i++){
		for(j=0L;j<nx;j++){
			l=Label[k];
			if(l>0){
				CentresTaches[l].x += j;
				CentresTaches[l].y += i;
				Area[l] ++;
			}
			k++;
		}
	}

	for(i=1;i<nbColors;i++){
		CentresTaches[i].x /=(float)Area[i];
		CentresTaches[i].y /=(float)Area[i];
	}

	if(pIOB==nullptr)
		return true;
	else
	{
		H3_ARRAY_UINT8 IsValid(nbColors);
		IsValid.Fill(1L);
		IsValid[0]=0L;

		long lValue;
		float fValue;

		fValue=pIOB->Get("MinArea");
		if((lValue=(long)fValue)>=0)
		{
			for(i=1;i<nbColors;i++)
				if(IsValid[i])
					if(Area[i]<(unsigned int)lValue)IsValid[i]=0L;
		}

		fValue=pIOB->Get("MaxArea");
		if((lValue=(long)fValue)>=0)
		{
			for(i=1;i<nbColors;i++)
				if(IsValid[i])
					if(Area[i]> (unsigned int)lValue)IsValid[i]=0L;
		}

		if((fValue=pIOB->Get("Compactness"))>=0)
		{
			H3_ARRAY2D_UINT8 Mask(ny,nx);
			for(i=0;i<sz;i++)
				Mask[i]=(Label[i]>0);

			Mask *=255L;

			H3_ARRAY2D_UINT8 DilatedMask;
			if(!Dilate(&DilatedMask,Mask)) return false;

			DilatedMask = DilatedMask-Mask;

			H3_ARRAY_UINT32 sum1(nbColors),sum2(nbColors);
			sum1.Fill(0L);
			sum2.Fill(0L);

			H3_ARRAY2D_UINT8 Mask01= Mask/255L;
			unsigned __int8 *pM=Mask01.GetData(),*pMt=pM-nx,*pMb=pM+nx,*pMl=pM-1,*pMr=pM+1;

			H3_ARRAY2D_UINT32 Contour(ny,nx);
			Contour.Fill(0L);
			long HB=0L,GD=0L;//nombre de contact d'un pixel sur les cotés haut/bas ou gauche/droite
			unsigned __int32 *pL=Label.GetData(),*pLt=pL-nx,*pLb=pL+nx,*pLr=pL+1,*pLl=pL-1;

			/////////////////////////////////////////////////////////////////
			//DilatedMask contient les frontieres exterieures de Mask
			//Mask est à 1 si le pixel correspondant de Label contient une couleur
			//on parcours DilatedMask 
			//si 0 et si bord: la couleur qui touche le bord de l'image voit sa frontiere croitre
			//si 1 : on cherche parmi les 4 voisins de DilatedMask(i,j) le(s)quel(s) est(sont) colorés
			//	on augmente la frontiere _sum1_ de cette (ces) couleur(s)
			//	si un pixel de DilatedMask touche une meme couleur sur 2(seulement) cotés voisins,
			//	la frontiere n'est pas 2 mais (sqrt(2)) 
			//	dans sum2 on note qu'il faudra faire une correction
			//i==0///////////////////////////////////////////////////////////
			//i==0; j==0
			k=0L;
			if(DilatedMask[k]!=0L){
				HB =*pMb; sum1[*pLb]++;
				GD =*pMr; sum1[*pLr]++;

				if(HB==1L && GD==1L){
					unsigned long color1=(*pLb);//nb l'un des 2 est à 0 >> color1, c'est l'autre
					unsigned long color2=(*pLr);
					sum2[color1]+=(color1==color2);
				}
			}
			else
				sum1[*pL]+=2L;//le pixel touche le bord >> on ajoute 1 au masque pour haut et 1 pour gauche

			k++;
			pM++;pMt++;pMb++;pMl++;pMr++;
			pL++;pLt++;pLb++;pLl++;pLr++;
			for(j=1L;j<nx-1;j++)
			{
				if(DilatedMask[k]!=0L){
					HB =*pMb; sum1[*pLb]++;
					GD =*pMl; sum1[*pLl]++;
					GD+=*pMr; sum1[*pLr]++;

					if(HB==1L && GD==1L){
						unsigned long color1=(*pLb);
						unsigned long color2=(*pLr)+(*pLl);//nb l'un des 2 est à 0 >> color2, c'est l'autre
						sum2[color1]+=(color1==color2);
					}
				}
				else
					sum1[*pL]++;//le pixel touche le bord >> on ajoute 1 au masque
				k++;
				pM++;pMt++;pMb++;pMl++;pMr++;
				pL++;pLt++;pLb++;pLl++;pLr++;
			}
			if(DilatedMask[k]!=0L){
				HB =*pMb; sum1[*pLb]++;
				GD =*pMl; sum1[*pLl]++;

				if(HB==1L && GD==1L){
					unsigned long color1=(*pLb);
					unsigned long color2=(*pLl);
					sum2[color1]+=(color1==color2);
				}
			}
			else
				sum1[*pL]+=2L;//le pixel touche le bord >> on ajoute 1 au masque pour t et 1 pour l

			k++;
			pM++;pMt++;pMb++;pMl++;pMr++;
			pL++;pLt++;pLb++;pLl++;pLr++;

			//i==1 à ny-1; j==nx-1	///////////////////////////////////////////////////////////		
			for(i=1L,k=nx;i<ny-1;i++)
			{
				if(DilatedMask[k]!=0L){
					HB =*pMt; sum1[*pLt]++;
					HB+=*pMb; sum1[*pLb]++;
					GD =*pMr; sum1[*pLr]++;

					if(HB==1L && GD==1L){
						unsigned long color1=(*pLt)+(*pLb);//nb l'un des 2 est à 0 >> color1, c'est l'autre
						unsigned long color2=(*pLr);
						sum2[color1]=(color1==color2);
					}
				}
				else
					sum1[*pL]+=1L;//le pixel touche le bord >> on ajoute 1 au masque
				k++;
				pM++;pMt++;pMb++;pMl++;pMr++;
				pL++;pLt++;pLb++;pLl++;pLr++;
				for(j=1L;j<nx-1;j++)
				{
					if(DilatedMask[k]!=0L){
						HB =*pMt; sum1[*pLt]++;
						HB+=*pMb; sum1[*pLb]++;
						GD =*pMl; sum1[*pLl]++;
						GD+=*pMr; sum1[*pLr]++;

						if(HB==1L && GD==1L){
							unsigned long color1=(*pLt)+(*pLb);//nb l'un des 2 est à 0 >> color1, c'est l'autre
							unsigned long color2=(*pLr)+(*pLl);//nb l'un des 2 est à 0 >> color2, c'est l'autre
							sum2[color1]+=(color1==color2);
						}
					}	
					k++;
					pM++;pMt++;pMb++;pMl++;pMr++;
					pL++;pLt++;pLb++;pLl++;pLr++;
				}
				if(DilatedMask[k]!=0L){
					HB =*pMt; sum1[*pLt]++;
					HB+=*pMb; sum1[*pLb]++;
					GD =*pMl; sum1[*pLl]++;
					
					if(HB==1L && GD==1L){
						unsigned long color1=(*pLt)+(*pLb);//nb l'un des 2 est à 0 >> color1, c'est l'autre
						unsigned long color2=(*pLl);
						sum2[color1]+=(color1==color2);
					}
				}
				else
					sum1[*pL]+=1L;//le pixel touche le bord >> on ajoute 1 au masque
				k++;
				pM++;pMt++;pMb++;pMl++;pMr++;
				pL++;pLt++;pLb++;pLl++;pLr++;
			}
			//i==ny-1 j=0à nx-1////////////////////////////////////////////////////////////////

			if(DilatedMask[k]!=0L){
				HB =*pMt; sum1[*pLt]++;
				GD =*pMr; sum1[*pLr]++;

				if(HB==1L && GD==1L){
					unsigned long color1=(*pLt);
					unsigned long color2=(*pLr);
					sum2[color1]+=(color1==color2);
				}
			}
			else
				sum1[*pL]+=2L;//le pixel touche le bord >> on ajoute 1 au masque pour bas et 1 pour gauche

			k++;
			pM++;pMt++;pMb++;pMl++;pMr++;
			pL++;pLt++;pLb++;pLl++;pLr++;
			for(j=1L;j<nx-1;j++)
			{
				if(DilatedMask[k]!=0L){
					HB = *pMt; sum1[*pLt]++;
					GD =*pMl; sum1[*pLl]++;
					GD+=*pMr; sum1[*pLr]++;

					if(HB==1L && GD==1L){
						unsigned long color1=(*pLt);
						unsigned long color2=(*pLr)+(*pLl);//nb l'un des 2 est à 0 >> color2, c'est l'autre
						sum2[color1]+=(color1==color2);
					}
				}
				else
					sum1[*pL]++;//le pixel touche le bord >> on ajoute 1 au masque
				k++;
				pM++;pMt++;pMb++;pMl++;pMr++;
				pL++;pLt++;pLb++;pLl++;pLr++;
			}
			if(DilatedMask[k]!=0L){
				HB =*pMt; sum1[*pLt]++;
				GD =*pMl; sum1[*pLl]++;

				if(HB==1L && GD==1L){
					unsigned long color1=(*pLt);
					unsigned long color2=(*pLl);
					sum2[color1]+=(color1==color2);
				}
			}
			else
				sum1[*pL]+=2L;//le pixel touche le bord >> on ajoute 1 au masque pour t et 1 pour l

			///////////////////////////////////////////////////////////////////////////////////

			H3_ARRAY_FLT32 Perimeter(nbColors);
			Perimeter.Fill(0.0f);
			float cst= ::sqrt(2.0f) -2.0f;
			for(i=1;i<nbColors;i++)
				Perimeter[i] = sum1[i]+cst*sum2[i];

			H3_ARRAY_FLT32 Compactness(nbColors);

			Compactness= Perimeter;
			Compactness *= Perimeter;
			Compactness /= Area;
			Compactness /= (2*fTWO_PI);

			for(i=1;i<nbColors;i++)
				if(Compactness[i]>fValue) IsValid[i]=0L;

		}
		for(i=0L,j=0L;i<nbColors;i++)
			if(IsValid[i])
				CentresTaches[j++]=CentresTaches[i];


		CentresTaches=CentresTaches.GetAt(0,j);
		return true;
	}

	return false;//on ne passe pas par la
}

//GaussImage
//renvoie une matrice de taille ny*nx
//les données dans la matrices varie comme l'intensité d'une gaussienne
//nx, ny: taille de la fenetre de recherche
//wx2,wy2: zone à 0 autour du centre de la fenetre de recherche
//si wx2 et wy2 sont <0 , pas de mise à 0 
H3_ARRAY2D_FLT32 GaussImage(unsigned long ny,unsigned long nx,long wy2,long wx2)
{
	//ny et nx doivent etre impaire pour avoir un centre
	if((nx+1)/2 == (nx/2))//nx est paire
		nx++;
	if((ny+1)/2 == (ny/2))//ny est paire
		ny++;
	H3_ARRAY2D_FLT32 Gauss(ny,nx);

	unsigned long wintx=nx/2,winty=ny/2;
	unsigned long i,j,I1,I2;
	float expIi;

	float tmp,*expI,*expJ;
	expI=new float[2*winty+1];
	expJ=new float[2*wintx+1];

	expI[winty]=expJ[wintx]=1.0;
	for (i=0;i<winty;i++){
		tmp=float(winty-i)/winty;
		expI[i]=expI[2*winty-i]=exp(-tmp*tmp);
	}
	for (j=0;j<wintx;j++){
		tmp=float(wintx-j)/wintx;
		expJ[j]=expJ[2*wintx-j]=exp(-tmp*tmp);
	}
	for (i=0;i<=winty;i++){
		expIi=expI[i];
		for (j=0;j<=wintx;j++){
			Gauss(i,j)=
			Gauss(2*winty-i,j)=
			Gauss(i,2*wintx-j)=
			Gauss(2*winty-i,2*wintx-j)=	expIi*expJ[j];
		}
	}

	if(wx2>0 && wy2>0){
		if((wintx-wx2>2)&&(winty-wy2>2)){
			Gauss(winty,wintx)=0;
			for (i=0;i<wy2;i++){
				I1=winty-1-i;
				I2=winty+1+i;
				Gauss(I1,wintx)=0;
				Gauss(I2,wintx)=0;
				for (j=0;j<wx2;j++){
					Gauss(I1,wintx+1+j)=0;
					Gauss(I1,wintx-1-j)=0;
					Gauss(I2,wintx+1+j)=0;
					Gauss(I2,wintx-1-j)=0;
				}
			}
		}
	}

	delete [] expI;
	delete [] expJ;
	return Gauss;
}


//////////////////////////////////////////class InquiryOnBlobs begin
//fixe les parametres que l'on pourra utiliser dans la recherche des blobs
//minArea : en pixel
//maxArea : en pixel
//compactness : en perimetre*perimetre/area/4pi (1 pour disque, 1.3 pour carré)
//reste à faire
//Feret, BoxSize...
InquiryOnBlobs::InquiryOnBlobs()
{
	lMinArea=lMaxArea=-1L;
	fCompactness=-1.0f;
}

InquiryOnBlobs::~InquiryOnBlobs()
{
}

bool InquiryOnBlobs::Set(const CString Data, float value)
{
	if(Data==_T("MinArea"))
		lMinArea=(long)value;
	else if(Data==_T("MaxArea"))
		lMaxArea=(long)value;
	else if(Data==_T("Compactness"))
		fCompactness=value;
	else
		return false;

	return true;
}

float InquiryOnBlobs::Get(const CString Data)const
{
	if(Data==_T("MinArea"))
		return (float)lMinArea;
	else if(Data==_T("MaxArea"))
		return (float)lMaxArea;
	else if(Data==_T("Compactness"))
		return (float)fCompactness;

	return (-1.0f);
}

//////////////////////////////////////////class InquiryOnBlobs fin

//////////////////////////////////////////fonctions basiques begin
//erosion binaire
//Im contient comme niveau de gris 255 et autre chose
//les zones à 255 sont erodés
bool Erode(H3_ARRAY2D_UINT8* pa2dErodedIm,const H3_ARRAY2D_UINT8& Im)
{
	const size_t  nx=Im.GetCo(),ny=Im.GetLi();

	H3_ARRAY2D_FLT32 filtreX(ny,nx);
	filtre1(Im, 3L, 'x',&filtreX);

	H3_ARRAY2D_FLT32 filtreXY(ny,nx);
	filtre1(filtreX, 3L, 'y',&filtreXY);

	H3_ARRAY2D_UINT8 a2dErodedIm;//au cas ou pa2dErodedIm==NULL//PB: detruit en fin de fct, non?
	if(pa2dErodedIm==nullptr){
		a2dErodedIm.Alloc(ny,nx);
		pa2dErodedIm = &a2dErodedIm;
	}
	else
		if(!(nx==pa2dErodedIm->GetCo() && ny==pa2dErodedIm->GetLi() ))
			pa2dErodedIm->ReAlloc(ny,nx);

	long i;
	unsigned __int8 ThresValue=254L;
	for(i=0L;i<ny*nx;i++){
		(*pa2dErodedIm)[i]=(filtreXY[i]>ThresValue);//==255L ne fonctionne pas bien en mode debug. en mode release:?
	}
	(*pa2dErodedIm) *= 255L;

	return true;
}

//dilatation binaire
//Im contient comme niveau de gris 0 et autre chose
//les zones à "autre chose" sont dilatés
bool Dilate(H3_ARRAY2D_UINT8* pa2dDilatedIm,const H3_ARRAY2D_UINT8& Im)
{
	const size_t  nx=Im.GetCo(),ny=Im.GetLi();

	H3_ARRAY2D_FLT32 filtreX=filtre(Im, 3L, 'x');
	H3_ARRAY2D_FLT32 filtreXY=filtre(filtreX, 3L, 'y');

	if(pa2dDilatedIm==nullptr)
		pa2dDilatedIm->Alloc(ny,nx);
	else
		if(!(nx==pa2dDilatedIm->GetCo() && ny==pa2dDilatedIm->GetLi() ))
			pa2dDilatedIm->ReAlloc(ny,nx);

	long i;
	for(i=0L;i<ny*nx;i++){
		(*pa2dDilatedIm)[i]=(filtreXY[i]>0.1f);
	}

	(*pa2dDilatedIm) *= 255L;
	return true;
}
//////////////////////////////////////////fonctions basiques end
