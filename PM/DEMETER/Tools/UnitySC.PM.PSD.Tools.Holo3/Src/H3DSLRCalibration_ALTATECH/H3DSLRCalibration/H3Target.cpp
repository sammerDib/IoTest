// Target.cpp: implementation of the CH3Target class.
//
//////////////////////////////////////////////////////////////////////
#include "stdafx.h"
#include "H3DSLRCalibration.h"
#include "H3Target.h"
#include "H3WinApp.h"
#include "FindSquare.h"
#include "H3Mire.h" //pour avoir les parametres de la mire (lignes colonnes). Rend la classe dependante >> pb . cv290908 

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

extern void Tic();
extern DWORD Toc();
extern DWORD Toc(CString);

#define DEFAULT_CALC_NAME "Target"

//#define H3WINTX (6L)
//#define H3WINTY (6L)

//#define H3DEFAULT_COEFF_FILTER (0.25f)

//#define H3DEFAULT_SEUIL2	(3.0f)
//#define H3DEFAULT_SEUIL3	(1)
//#define H3DEFAULT_N			(5.5f)

//#define H3CORNERFINDER_FIRSTGUESS_STEPSE (5)	//facteur de sous echantillonage
//#define H3CORNERFINDER_FIRSTGUESS_N (3)			//le pattern recherché sera de taille (2n+1)*(2n+1)
//#define H3CORNERFINDER_RESOLUTION (0.005f)		//arret de la recheche fine quand ce critere est atteind
//#define H3CORNERFINDER_MAXITER (3)				//nb d'iteration max pour trouver le pattern

//seuil pour la selection des points valides dans la cartographie de convolution
	//valeur determinée experimentalement pour image avec damier uniforme
	//avec 0.5, on detecte aussi les coins exterieurs du damier
//#define CORNERFINDER_SEUIL float(1.0/1.7)
	//valeur pour image avec damier dont le contour est un dégradé de gris
//#define H3CORNERFINDER_SEUIL float(0.5) 
#define DETA_GL_WHITE_BLACK_MIN 5L
#define DEFAULT_TARGET_NAME			"CH3Target"

static CString strModule("CH3Target");

template < class TYPE > static
H3_ARRAY_PT2DFLT32 FindOrigine(const H3_ARRAY2D& Image,H3_ARRAY2D_PT2DFLT32& TargetPos)
{
	CString strFunction("FindOrigine()");

	if (TargetPos.GetSize()==0)
		return H3_ARRAY_PT2DFLT32(0);

	size_t Ili=Image.GetLi(),Ico=Image.GetCo();

	H3_ARRAY_PT2DFLT32 PPP(1);

	//Recherche des 4 zones
	H3_ARRAY2D_INT32 Carre = FindCarre(TargetPos,Ili,Ico);

	//Extraire les 4 zones
	H3_ARRAY2D_FLT32 ImCarre1 = Image.GetAt(Carre(0,2),Carre(0,0),
		Carre(0,3)-Carre(0,2)+1 ,Carre(0,1)-Carre(0,0)+1  );

	H3_ARRAY2D_FLT32 ImCarre2 = Image.GetAt(Carre(1,2),Carre(1,0),
		Carre(1,3)-Carre(1,2)+1 ,Carre(1,1)-Carre(1,0)+1  );

	H3_ARRAY2D_FLT32 ImCarre3 = Image.GetAt(Carre(2,2),Carre(2,0),
		Carre(2,3)-Carre(2,2)+1 ,Carre(2,1)-Carre(2,0)+1  );

	H3_ARRAY2D_FLT32 ImCarre4 = Image.GetAt(Carre(3,2),Carre(3,0),
		Carre(3,3)-Carre(3,2)+1 ,Carre(3,1)-Carre(3,0)+1  );

	// Recherche le cercle dans les 4 zones

	long nBlod1=FindBlob3((H3_ARRAY2D_UINT8)ImCarre1);
	long nBlod2=FindBlob3((H3_ARRAY2D_UINT8)ImCarre2);
	long nBlod3=FindBlob3((H3_ARRAY2D_UINT8)ImCarre3);
	long nBlod4=FindBlob3((H3_ARRAY2D_UINT8)ImCarre4);

	// Definir la zone Origine
	if((nBlod1+nBlod2+nBlod3+nBlod4)!=1L)//erreur
	{
		H3DebugError(strModule,strFunction,"l'origine de la mire n'a pas été déterminé");
	}

	if  (nBlod1==1L)
	{
		PPP[0].x=(float)Carre(0,0);
		PPP[0].y=(float)Carre(0,2);

	}
	else if(nBlod2==1L)
	{
		PPP[0].x=(float)Carre(1,0);
		PPP[0].y=(float)Carre(1,2);
	}
	else if(nBlod3==1L)
	{
		PPP[0].x=(float)Carre(2,0);
		PPP[0].y=(float)Carre(2,2);
	}
	else 
	{
		PPP[0].x=(float)Carre(3,0);
		PPP[0].y=(float)Carre(3,2);
	}

	return PPP;
}
////////////////////////////////////////////////////Fin Static
template < class TYPE > static 
long FindBlob(H3_ARRAY2D& ImCarre)
{

	size_t co=ImCarre.GetCo(),li=ImCarre.GetLi(),co2=co/2;

	H3_FLT32 AncienPoint=0.0f;
	H3_FLT32 AncienAncienPoint=0.0f;
	long nNbFront=0;

	size_t i,j;
	
	if (co<5)
		return false;

	//recherche des min et max
	H3_ARRAY_FLT32 Stat=ImCarre.GetStatistics();//Stat sur toute l'imagette
	TYPE Min=Stat[1],Max=Stat[2],Seuil=(Max+Min)/2*5;//5, a cause du nb de ligne utilisé

	H3_ARRAY2D_FLT32 ImLine(li-2,1),u(li,1),du(li-2,1);
	u.Fill(0);

	for(i=0;i<li;i++)
	{
		for(j=co2-2;j<co2+3;j++)
		{
			u[i] += ImCarre(i,j);
		}
	}

	for(i=0;i<li;i++)
		u[i]=(u[i]>Seuil);

	// je cherche une alternance blanc noir blanc avec des paliers larges >> le plus large noir possible encadré par deux blancs 
	long BkSz=0,BkSz0=0,WSz=0;
	i=0;
	while(u[i]==0 && i<li) i++;//on se place sur le 1er blanc >> le disque noir ne doit pas toucher le bord
	while(u[i]==1 && i<li) i++;//on se place sur le 1er noir ne touchant pas le bord
	while(i<li)
	{
		BkSz=WSz=0;
		while(u[i]==0 && i<li){
			BkSz++;
			i++;
		}	
		while(u[i]==1 && i<li)
		{
			WSz++;
			i++;
		}
		if(WSz>0)//il faut finir par un blanc
			if(BkSz>BkSz0) BkSz0=BkSz;
	}

	if (BkSz0>=2)
		return BkSz0;
	else 
		return 0;
}
/*
* FindBlob3
*  in H3_ARRAY2D& ImCarre: imagette dans laquelle il se peut que se trouve 1 blob
*	out parametre proportionnel aux chances d'avoir un blob dans l'image entrée
*
*/

static 
size_t FindBlob3(H3_ARRAY2D_UINT8& ImCarre)
{
	const size_t  co=ImCarre.GetCo(),li=ImCarre.GetLi(),sz=ImCarre.GetSize(),co2=co/2L,li2=li/2L;

	size_t  i;
	long CenterColor;//supposée noire dans les appelations Bk et W
	//long BorderColor;//supposée blanche (correspond à W)

	//recherche des min et max
	H3_ARRAY_FLT32 Stat=ImCarre.GetStatistics();//Stat sur toute l'imagette
	long Min=Stat[1],Max=Stat[2];
	//
	long Seuil=Min+(Max-Min)/3L;//pour test: les variation de ng sont + importantes dans le blanc que dans le noir

	H3_ARRAY2D_UINT8 ImBin(li,co);

	CenterColor=(ImCarre(li2,co2)>Seuil);

	if(Max-Min<DETA_GL_WHITE_BLACK_MIN) return 0L;

	if(CenterColor==1L)
		for(i=0;i<sz;i++)
			ImBin[i] = (ImCarre[i]>Seuil);
	else
		for(i=0;i<sz;i++)
			ImBin[i] = (ImCarre[i]<Seuil);

	H3_ARRAY2D_UINT32 Label;
	if(!bwLabel(&Label,ImBin,4))
			return 0;

	long MinArea=(li*co)/16L;
	long MaxArea=(li*co*2L)/5L;//pour un disque de rayon=arete_carré/4 li*co/5 suffit
	float Compactness=1.75f;

	InquiryOnBlobs IOB;
	IOB.Set("MinArea",MinArea);
	IOB.Set("MaxArea",MaxArea);
	IOB.Set("Compactness",Compactness);

	H3_ARRAY_PT2DFLT32 CentresTaches;
	if(!GetBlobs(CentresTaches,Label,&IOB))
			return 0;

	size_t  nbTaches=CentresTaches.GetSize();

	return nbTaches;
}


//on cherche 4 carrés au voisinage des sommets et exterieurs à TargetPos
//les coordonnées Xmin,Xmax,Ymin,Ymax de chacun des carrés trouvés sont renvoyés sur une ligne du resultat
static H3_ARRAY2D_INT32 FindCarre( H3_ARRAY2D_PT2DFLT32& TargetPos,size_t  li, size_t co)
{
	size_t nLi=TargetPos.GetLi(),nCo=TargetPos.GetCo();
	long xx[2]={0,nCo-1},yy[2]={0,nLi-1},sens[2]={1,-1};
	H3_ARRAY2D_INT32 out(4,4);

	float x00,x10,x01,x11;
	float y00,y10,y01,y11;
	float X00,X10,X01,X11;
	float Y00,Y10,Y01,Y11;
	
	size_t i,j,k;
	for(i=0,k=0;i<2;i++){
		for(j=0;j<2;j++){
			x00=TargetPos(yy[i],xx[j]).x;			
			y00=TargetPos(yy[i],xx[j]).y;

			x10=TargetPos(yy[i]+sens[i],xx[j]).x;	
			y10=TargetPos(yy[i]+sens[i],xx[j]).y;

			x01=TargetPos(yy[i],xx[j]+sens[j]).x;
			y01=TargetPos(yy[i],xx[j]+sens[j]).y;
			
			x11=TargetPos(yy[i]+sens[i],xx[j]+sens[j]).x;
			y11=TargetPos(yy[i]+sens[i],xx[j]+sens[j]).y;

			X00=x00;		Y00=y00;
			X01=2*x00-x01;	Y01=2*y00-y01;
			X10=2*x00-x10;	Y10=2*y00-y10;
			X11=2*x00-x11;	Y11=2*y00-y11;

			out(k,0)=__max(__min(__min(X00,X01),__min(X10,X11))-1,0);//Xmin
			out(k,1)=__min(__max(__max(X00,X01),__max(X10,X11))+1,co-1);//Xmax
			out(k,2)=__max(__min(__min(Y00,Y01),__min(Y10,Y11))-1,0);//Ymin
			out(k,3)=__min(__max(__max(Y00,Y01),__max(Y10,Y11))+1,li-1);//Ymax

			k++;
		}
	}

	return out;
}
//////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////
CH3Target::CH3Target(const CString & Filename, int CamNum)
{
 	LoadCalib(Filename,CamNum); 
}

CH3Target::~CH3Target()
{
}

bool CH3Target::CheckPlanarity(H3_ARRAY2D_FLT32& X)
{
	CString strFunction("CheckPlanarity");

	if(X.GetLi()!=3){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugError(strModule,strFunction,"mauvais dimensionnement des données");
		#endif
		AfxThrowUserException();
	}
	size_t  NbPoints=X.GetCo();
	if(NbPoints<5) return true;

	//calcul de la valeur metrique moyenne
	H3_MATRIX_FLT32 Y(3,NbPoints);
	H3_MATRIX_FLT32 YY(3,3);
	float xmean=0,ymean=0,zmean=0;
	float *pX,*pY,*pZ,*p_Y1,*p_Y2,*p_Y3;
	H3_MATRIX_FLT32 U(3,3),W(1,3),V(3,3);

	long i=NbPoints;
	pX=X.GetLine(0);
	pY=X.GetLine(1);
	pZ=X.GetLine(2);
	while(i--){
		xmean += (*(pX++));
		ymean += (*(pY++));
		zmean += (*(pZ++));
	}
	xmean/=NbPoints;
	ymean/=NbPoints;
	zmean/=NbPoints;

	i=NbPoints;
	pX=X.GetLine(0);
	pY=X.GetLine(1);
	pZ=X.GetLine(2);

	p_Y1=Y.GetLine(0);
	p_Y2=Y.GetLine(1);
	p_Y3=Y.GetLine(2);

	while(i--){
		(*(p_Y1++)) = (*(pX++))-xmean;
		(*(p_Y2++)) = (*(pY++))-ymean;
		(*(p_Y3++)) = (*(pZ++))-zmean;
	}

	YY=Y*Y.Trans();
	YY.SVDcmp(U,W,V);

	double r=W(2)/W(1); 

	if(r<1e-3) return true;
	else return false;
}

H3_ARRAY2D_PT2DFLT32 CH3Target::Find(const H3_ARRAY2D_UINT8& Image,short TargetType, bool OriginOnTarget)
{
	CString strFunction("Find(uint8)"),msg;

	H3DebugInfo(strModule,strFunction,"");

	H3_ARRAY2D_PT2DFLT32 TargetPos;
	H3_ARRAY2D_PT2DFLT32 xt2;
	strGuessResult GR;
	switch(TargetType)
	{
	case TARGET_SQUARE:
		{
		bool b=cornerfinder_firstGuess(GR,Image,m_param.nCornerFinderFirstGuessN,m_param.nCornerFinderFirstGuessStepSE,m_param.fCornerFinderSeuil);
		if(!b){
			H3DebugError(strModule, strFunction,_T("pb CornerFinder"));
			return H3_ARRAY2D_PT2DFLT32(0,0);
		}
		if(GR.Points.GetSize()<9){
			msg.Format("%d point(s) seulement ont(a) été trouvé(s)",GR.Points.GetSize());
			H3DebugError(strModule, strFunction,msg);
			return H3_ARRAY2D_PT2DFLT32(0,0);
		 }

		if(!MySortFunc(GR.Points,xt2,CH3Mire::m_iLiIntersection,CH3Mire::m_iCoIntersection,0,0))
		{
			msg.Format("Echec de SortFunc pour %d lignes %d colonnes.Le nombre de point trouvé était de %d.",CH3Mire::m_iLiIntersection,CH3Mire::m_iCoIntersection,GR.Points.GetSize());
			H3DebugError(strModule,strFunction,msg);
			return H3_ARRAY2D_PT2DFLT32(0,0);
		}

		if(xt2.GetLi()<3 || xt2.GetCo()<3){
			msg.Format("la mire identifiée n'a que %d ligne(s) et %d colonne(s)",xt2.GetLi(),xt2.GetCo());
			H3DebugError(strModule, strFunction,msg);
			return H3_ARRAY2D_PT2DFLT32(0,0);
		}

		//appel cornerfinder
		long wintx=m_param.nWintX,winty=m_param.nWintY;//taille de la fenetre (2*winty+1,2*wintx+1) 
		//			autour des premiers points trouvés dans laquelle 
		//			on va chercher une position fine
		long wx2=-1,wy2=-1;//zeros au centre de la fenetre

		TargetPos=cornerfinder(xt2,Image,wintx,winty,wx2,wy2,m_param.fCornerFinderResolution,m_param.nCornerFinderMaxIter);

		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			CString msg;
			msg.Format("nb Points trouvés: %d\nrépartis sur %d lignes et %d colonnes",GR.Points.GetSize(),TargetPos.GetLi(),TargetPos.GetCo());
			H3DebugInfo(strModule,strFunction,msg);
		#endif
		}
	
		break;
	case(TARGET_CIRCLE):{
		//pas geré =erreur
		}
	default:{
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			CString msg;
			msg.Format("Type de cible non géré");
			H3DebugError(strModule,strFunction,msg);
		#endif
		AfxThrowUserException();
		//error
			}
		break;
	}

	//Trouver les zones des 4 coins du damier où peut se trouver le cercle
	//les points sont trié dans xt2; ils sont données à quelques pixels pret, mais ca suffit
	//les points de TargetPos sont dans le meme ordre
	size_t  i,xt2co=xt2.GetCo(),xt2li=xt2.GetLi();

	// Trouver l'origine
	H3_ARRAY_PT2DFLT32 PPP;
	if(OriginOnTarget){
		PPP=FindOrigine(Image,TargetPos);
	}else{
		//origine en bas à gauche
		size_t imLi=Image.GetLi(),imCo=Image.GetCo();
		long delta=imLi+imCo,i,I;

		for(i=0;i<TargetPos.GetSize();i++){
			if((TargetPos[i].x-TargetPos[i].y)<delta){//version delta entier(peut etre negatif)
				delta=TargetPos[i].x-TargetPos[i].y;
				I=i;
			}
		}
		if(I>=0){
			PPP=H3_ARRAY_PT2DFLT32(1);
			PPP[0]=TargetPos[I];
			//pour la suite, on sait deja quel coin de TargetPos est celui que l'on cherche >> en tenir compte eventuellement
		}
	}

	//trier avec le repere Cercle
	if(PPP.GetSize()>0)
	{
		//calcul des distances (au carré) de l'origine aux quatres coins de TargetPos
		float x0=PPP[0].x,y0=PPP[0].y,dx,dy,d[4];
		dx=TargetPos(0,0).x-x0				;	dy=TargetPos(0,0).y-y0;				d[0]=dx*dx+dy*dy;
		dx=TargetPos(xt2li-1,0).x-x0		;	dy=TargetPos(xt2li-1,0).y-y0;		d[1]=dx*dx+dy*dy;
		dx=TargetPos(0,xt2co-1).x-x0		;	dy=TargetPos(0,xt2co-1).y-y0;		d[2]=dx*dx+dy*dy;
		dx=TargetPos(xt2li-1,xt2co-1).x-x0	;	dy=TargetPos(xt2li-1,xt2co-1).y-y0;	d[3]=dx*dx+dy*dy;


		//recherche de la distance mini
		float dmin=d[0];
		long index=0;
		
		for(i=1;i<4;i++){
			if(d[i]<dmin){
				dmin=d[i];
				index=i;
			}
		}

		//l'origine est le i_ieme coin
		//on veut que ce coin soit en position (0,0) de TargetPos
		if(index==1){
			TargetPos.FlipUD();
			TargetPos=TargetPos.Trans();
		}
		else if(index==2){
			TargetPos.FlipLR();
			TargetPos=TargetPos.Trans();
		}
		else if(index==3){
			TargetPos.FlipLR();
			TargetPos.FlipUD();
		}

		// Tri selon les Axes Y X
		float X00=TargetPos(0,0).x;
		float X10=TargetPos(1,0).x;
		float X01=TargetPos(0,1).x;
		float Y00=TargetPos(0,0).y;
		float Y10=TargetPos(1,0).y;
		float Y01=TargetPos(0,1).y;

		float dx10=(X10-X00);
		float dx01=(X01-X00);
		float dy10=(Y10-Y00);
		float dy01=(Y01-Y00);

#if 0	//je ne sais plus à quoi cela correspond
		float dx01b=-dy01;
		float dy01b=dx01;

		float d1=(dx10-dx01b)*(dx10-dx01b)+(dy10-dy01b)*(dy10-dy01b);
		float d2=(dx10+dx01b)*(dx10+dx01b)+(dy10+dy01b)*(dy10+dy01b);

		if (d2<d1)
#else
		//l'axe Ypixel est descendant alors que Yw est montant
		dy10 *= -1;
		dy01 *= -1;
		//produit vectoriel i^j=v01^v10 > repere direct >> >0
		float res=dx01*dy10-dx10*dy01;

		if (res<0)
#endif
		
		{
			TargetPos=TargetPos.Trans();
		}
	}

	return TargetPos;
}


H3_ARRAY2D_PT2DFLT32 CH3Target::Find(const H3_ARRAY2D_FLT32& Image,short TargetType)
{
	CString strFunction("Find(flt)"),msg;
	msg.Format("param= %d %d %f",m_param.nCornerFinderFirstGuessN,m_param.nCornerFinderFirstGuessStepSE,m_param.fCornerFinderSeuil);
	H3DebugInfo(strModule,strFunction,msg);
	H3_ARRAY2D_PT2DFLT32 TargetPos;
	H3_ARRAY2D_PT2DFLT32 xt2;
	strGuessResult GR;

	/// pour le cas ou l'image ne seraait pas 8 bits, normalisation de l'image
	H3_ARRAY_FLT32 ImaStats=Image.GetStatistics();
	float mmin=ImaStats[1],mmin2=ImaStats[5]-3*ImaStats[6];
	float mmax=ImaStats[2],mmax2=ImaStats[5]+3*ImaStats[6];
	if(mmin<mmin2) mmin=mmin2;
	if(mmax>mmax2) mmax=mmax2;

	H3_ARRAY2D_FLT32 mImage=(Image-mmin)*(255.0f/(mmax-mmin+1));
	//

	switch(TargetType){

	case(TARGET_SQUARE):{ 
		 bool b= cornerfinder_firstGuess(GR,mImage,m_param.nCornerFinderFirstGuessN,m_param.nCornerFinderFirstGuessStepSE,m_param.fCornerFinderSeuil);
		 if(!b){
			H3DebugError(strModule, strFunction,_T("pb CornerFinder"));
			return H3_ARRAY2D_PT2DFLT32(0,0);
		}
		 if(GR.Points.GetSize()<9){
			 msg.Format("%d point(s) seulement ont(a) été trouvé(s)",GR.Points.GetSize());
			H3DebugError(strModule, strFunction,msg);
			return H3_ARRAY2D_PT2DFLT32(0,0);
		 }

		 if(!MySortFunc(GR.Points,xt2,CH3Mire::m_iLiIntersection,CH3Mire::m_iCoIntersection,0,0))
			 return H3_ARRAY2D_PT2DFLT32(0,0);

		 if(xt2.GetLi()<3 || xt2.GetCo()<3){
			msg.Format("la mire identifiée n'a que %d ligne(s) et %d colonne(s)",xt2.GetLi(),xt2.GetCo());
			H3DebugError(strModule, strFunction,msg);
			return H3_ARRAY2D_PT2DFLT32(0,0);
		 }

		 //appel cornerfinder
		long wintx=m_param.nWintX,winty=m_param.nWintY;//taille de la fenetre (2*winty+1,2*wintx+1) 
		//			autour des premiers points trouvés dans laquelle 
		//			on va chercher une position fine
		long wx2=-1,wy2=-1;//zeros au centre de la fenetre
		TargetPos=cornerfinder(xt2,mImage,wintx,winty,wx2,wy2,m_param.fCornerFinderResolution,m_param.nCornerFinderMaxIter);
		}
		break;
	case(TARGET_CIRCLE):{
		//pas geré =erreur
		}
	default:{
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		CString msg;
		msg.Format("Type de cible non géré");
		H3DebugError(strModule,strFunction,msg);
		#endif
		AfxThrowUserException();
		//error
			}
		break;
	}

	//Trouver les zones des 4 coins du damier où peut se trouver le cercle
	//les points sont trié dans xt2; ils sont données à quelques pixels pret, mais ca suffit
	//les points de TargetPos sont dans le meme ordre
	size_t  i,xt2co=xt2.GetCo(),xt2li=xt2.GetLi();

	// Trouver l'origine
	H3_ARRAY_PT2DFLT32 PPP=FindOrigine(mImage,TargetPos);
	//trier avec le repere Cercle
	if(PPP.GetSize()>0)
	{
		//calcul des distances (au carré) de l'origine aux quatres coins de TargetPos
		float x0=PPP[0].x,y0=PPP[0].y,dx,dy,d[4];
		dx=TargetPos(0,0).x-x0				;	dy=TargetPos(0,0).y-y0;				d[0]=dx*dx+dy*dy;
		dx=TargetPos(xt2li-1,0).x-x0		;	dy=TargetPos(xt2li-1,0).y-y0;		d[1]=dx*dx+dy*dy;
		dx=TargetPos(0,xt2co-1).x-x0		;	dy=TargetPos(0,xt2co-1).y-y0;		d[2]=dx*dx+dy*dy;
		dx=TargetPos(xt2li-1,xt2co-1).x-x0	;	dy=TargetPos(xt2li-1,xt2co-1).y-y0;	d[3]=dx*dx+dy*dy;

		//recherche de la distance mini
		float dmin=d[0];
		size_t  index=0;
		
		for(i=1;i<4;i++){
			if(d[i]<dmin){
				dmin=d[i];
				index=i;
			}
		}

		//l'origine est le i_ieme coin
		//on veut que ce coin soit en position (0,0) de TargetPos
		if(index==1){
			TargetPos.FlipUD();
			TargetPos.Trans();
		}
		else if(index==2){
			TargetPos.FlipLR();
			TargetPos=TargetPos.Trans();}
		else if(index==3){
			TargetPos.FlipLR();
			TargetPos.FlipUD();
		}

		// Tri selon les Axes Y X
		float X00=TargetPos(0,0).x;
		float X10=TargetPos(1,0).x;
		float X01=TargetPos(0,1).x;
		float Y00=TargetPos(0,0).y;
		float Y10=TargetPos(1,0).y;
		float Y01=TargetPos(0,1).y;

		float dx10=(X10-X00);
		float dx01=(X01-X00);
		float dy10=(Y10-Y00);
		float dy01=(Y01-Y00);

#if 0	//je ne sais plus à quoi cela correspond
		float dx01b=-dy01;
		float dy01b=dx01;

		float d1=(dx10-dx01b)*(dx10-dx01b)+(dy10-dy01b)*(dy10-dy01b);
		float d2=(dx10+dx01b)*(dx10+dx01b)+(dy10+dy01b)*(dy10+dy01b);

		if (d2<d1)
#else
		//l'axe Ypixel est descendant alors que Yw est montant
		dy10 *= -1;
		dy01 *= -1;
		//produit vectoriel i^j=v01^v10 > repere direct >> >0
		float res=dx01*dy10-dx10*dy01;

		if (res<0)
#endif
		
		{
			TargetPos=TargetPos.Trans();
		}
	}
	return TargetPos;
}


//////////////////////////////////////////////////////////////////////////////////////////////
#if XML_FILE==0
BOOL CH3Target::LoadCalib(CString strFileName,int Indice)
{
	CString strFunction("LoadCalib"),strSection0("Target");
	CString msg;msg.Format("in LoadCalib. Filename=%s",strFileName);
	#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugInfo(strModule,strFunction,msg);
	#else
		AfxMessageBox(msg);
	#endif

	CString strInd;strInd.Format("%d",Indice);
	CString strSection=strSection0+strInd;

	m_param.nWintX=H3GetPrivProfileLong(strSection,_T("WINTX"),strFileName);
	m_param.nWintY=H3GetPrivProfileLong(strSection,_T("WINTY"),strFileName);

	m_param.fdefaultCoeffFilter=H3GetPrivProfileFloat(strSection,_T("DEFAULT_COEFF_FILTER"),strFileName);
	m_param.fDefaultSeuil2=H3GetPrivProfileFloat(strSection,_T("DEFAULT_SEUIL2"),strFileName);
	m_param.fDefaultSeuil3=H3GetPrivProfileFloat(strSection,_T("DEFAULT_SEUIL3"),strFileName);
	m_param.fDefaultN=H3GetPrivProfileFloat(strSection,_T("DEFAULT_N"),strFileName);

	m_param.nCornerFinderFirstGuessStepSE=H3GetPrivProfileLong(strSection,_T("CORNERFINDER_FIRSTGUESS_STEPSE"),strFileName);
	m_param.nCornerFinderFirstGuessN=H3GetPrivProfileLong(strSection,_T("CORNERFINDER_FIRSTGUESS_N"),strFileName);
	m_param.fCornerFinderResolution=H3GetPrivProfileFloat(strSection,_T("CORNERFINDER_RESOLUTION"),strFileName);
	m_param.nCornerFinderMaxIter=H3GetPrivProfileLong(strSection,_T("CORNERFINDER_MAXITER"),strFileName);
	m_param.fCornerFinderSeuil=H3GetPrivProfileFloat(strSection,_T("CORNERFINDER_SEUIL"),strFileName);

	m_param.nba=H3GetPrivProfileLong(strSection,_T("nda"),strFileName);
	return TRUE;
}

BOOL CH3Target::SaveCalib(CString strFileName,int Indice)
{
	CString strFunction("SaveCalib"),strSection("Target");
	CString msg("in SaveCalib");

	return TRUE;
}

#endif

////////////////////////////////////////////////////////////////////////////////////////////////////////
#if XML_FILE
BOOL CH3Target::LoadCalib(H3XMLFile* file,int Indice)
{
	CString strFunction("LoadCalib");
	CString msg("in LoadCalib");
	#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugInfo(strModule,strFunction,msg);
	#else
		AfxMessageBox(msg);
	#endif

	CString strEntry;
	strEntry.Format("%s%d",DEFAULT_CALC_NAME,Indice);
	m_param.nWintX=file->GetProfileInt(strEntry,_T("WINTX"));
	m_param.nWintY=file->GetProfileInt(strEntry,_T("WINTY"));

	m_param.fdefaultCoeffFilter=file->GetProfileFloat(strEntry,_T("DEFAULT_COEFF_FILTER"));
	m_param.fDefaultSeuil2=file->GetProfileFloat(strEntry,_T("DEFAULT_SEUIL2"));
	m_param.fDefaultSeuil3=file->GetProfileFloat(strEntry,_T("DEFAULT_SEUIL3"));
	m_param.fDefaultN=file->GetProfileFloat(strEntry,_T("DEFAULT_N"));

	m_param.nCornerFinderFirstGuessStepSE=file->GetProfileInt(strEntry,_T("CORNERFINDER_FIRSTGUESS_STEPSE"));
	m_param.nCornerFinderFirstGuessN=file->GetProfileInt(strEntry,_T("CORNERFINDER_FIRSTGUESS_N"));
	m_param.fCornerFinderResolution=file->GetProfileFloat(strEntry,_T("CORNERFINDER_RESOLUTION"));
	m_param.nCornerFinderMaxIter=file->GetProfileInt(strEntry,_T("CORNERFINDER_MAXITER"));
	m_param.fCornerFinderSeuil=file->GetProfileFloat(strEntry,_T("CORNERFINDER_SEUIL"));

	m_param.nba=file->GetProfileInt(strEntry,_T("nda"));
	return TRUE;
}

BOOL CH3Target::SaveCalib(H3XMLFile* file,int Indice)
{
	CString strFunction("SaveCalib");
	CString msg("in SaveCalib");

	#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugError(strModule,strFunction,msg);
	#else
		AfxMessageBox(msg);
	#endif

	CString strEntry;
	strEntry.Format("%s%d",DEFAULT_CALC_NAME,Indice);

	file->SetProfileInt(strEntry,_T("WINTX"),m_param.nWintX);
	file->SetProfileInt(strEntry,_T("WINTY"),m_param.nWintY);

	file->SetProfileFloat(strEntry,_T("DEFAULT_COEFF_FILTER"),m_param.fdefaultCoeffFilter);
	file->SetProfileFloat(strEntry,_T("DEFAULT_SEUIL2"),m_param.fDefaultSeuil2);
	file->SetProfileFloat(strEntry,_T("DEFAULT_SEUIL3"),m_param.fDefaultSeuil3);
	file->SetProfileFloat(strEntry,_T("DEFAULT_N"),m_param.fDefaultN);

	file->SetProfileInt(strEntry,_T("CORNERFINDER_FIRSTGUESS_STEPSE"),m_param.nCornerFinderFirstGuessStepSE);
	file->SetProfileInt(strEntry,_T("CORNERFINDER_FIRSTGUESS_N"),m_param.nCornerFinderFirstGuessN);
	file->SetProfileFloat(strEntry,_T("CORNERFINDER_RESOLUTION"),m_param.fCornerFinderResolution);
	file->SetProfileInt(strEntry,_T("CORNERFINDER_MAXITER"),m_param.nCornerFinderMaxIter);
	file->SetProfileFloat(strEntry,_T("CORNERFINDER_SEUIL"),m_param.fCornerFinderSeuil);

	file->SetProfileInt(strEntry,_T("nda"),m_param.nba);

	return TRUE;
}
#endif


