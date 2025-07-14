//FindSquare.h
#if!defined _FINDSQUARE__H
#define _FINDSQUARE__H


#include "label.h"
#include "h3matrix.h"

#include "structGuessResult.h"

//Les Fonctions etant 'template', elle sont instanciées dans le fichier '.h'
//FindSquare.cpp

static H3_ARRAY_INT8 GetSgn(const H3_ARRAY2D_FLT32& ChessConvolI,const H3_ARRAY_PT2DFLT32& IntersecSE)
{
	size_t nb=IntersecSE.GetSize();
	H3_ARRAY_INT8 Out(nb);
	for(size_t i=0;i<nb;i++)
	{
		Out[i]=SGN(ChessConvolI(IntersecSE[i].y,IntersecSE[i].x));
	}
	return Out;
}

//convolution par 2*2elements d'un damier(ligne 1: blanc/noir; ligne 2: noir blanc)
//la taille d'un élément est N*N pixels
//à l'issu de la convolution, les pixels dont le niveau de correlation
//correspond à un critere (> seuil par ex) sont retenus
//resultat:ChessConvol=image dont les pixels à 1 sont ceux retenus (zero ailleurs)
//etapes du calcul:
//1:calcul pour chaque pixel de la somme des ng pour les n*n pixels environant (filtreXY)
//2:cacul de la correlation en additionnant les valeurs de filtreXY qui conviennent (A)
//3: filtrage pour n'en retenir que certaines (ChessConvol)
template < class TYPE > static
bool ChessConvol(CH3Array2D<TYPE>& Image,unsigned long N,H3_ARRAY2D_FLT32 &ChessConvol,	float fCornerFinderSeuil,bool doSave=false)
{
	CString strFunction("ChessConvol");

	if(&ChessConvol==(H3_ARRAY2D_FLT32*)(&Image)) return false;

	size_t nx=Image.GetCo(),ny=Image.GetLi();

	if(ChessConvol.GetLi()!=ny || ChessConvol.GetCo()!=nx){
		ChessConvol = H3_ARRAY2D_FLT32(ny,nx);
	}
	ChessConvol.Fill(0);

	size_t i,j;
	size_t N2=(N+1)/2,NN2=N/2;
	float a_seuil,seuil_factor= fCornerFinderSeuil;

	H3_ARRAY2D_FLT32 filtreX =filtre(Image  ,N,'x');
	H3_ARRAY2D_FLT32 filtreXY=filtre(filtreX,N,'y');

	H3_ARRAY2D_FLT32 A(ny,nx);
	A.Fill(0);
	float a_max=0.0f;
	
	//filtreXY est le resultat du filtrage de Image par un filtre moyenneur de taille N*N
	//A va etre rempli a partir de 4 elements de filtreXY, distant de N lignes/colonnes 
	//(en realité distance=N2+NN2)
	//le point de A etudié est le centre du carré
	//il est inutile de s'interresser aux N/2 lignes superieures comme inferieures et
	//aux N/2 colonnes gauches et droites 
	float *pA0=A.GetData();
	float *pA;
	float *p10=filtreXY.GetData(),*p20,*p30,*p40;
	float *p1,*p2,*p3,*p4;

	pA0 += NN2*nx+NN2;
	//p10					//ligne 1 colonne 1 d'un damier 2lignes*2colonnes
	p40  = p10+(NN2+N2)*nx;	//ligne 2 colonne 1 d'un damier 2lignes*2colonnes
	p30  = p10+NN2+N2;		//ligne 1 colonne 2 d'un damier 2lignes*2colonnes
	p20  = p40+NN2+N2;		//ligne 2 colonne 2 d'un damier 2lignes*2colonnes

	unsigned long Imax=ny-N2,Jmax=nx-N2;
	
#if 1//en principe 1, pour test 0; cv juin06
	for(i=NN2;i<Imax;i++){
		pA=pA0;
		p1=p10; p2=p20; p3=p30; p4=p40;
		for(j=NN2;j<Jmax;j++){
			(*pA)=fabs( ( (*(p1++)) + (*(p2++))  )-
					    ( (*(p3++)) + (*(p4++))  ) )/4.0f;
			if((*pA)>a_max) a_max=(*pA);

			pA++;
		}
		pA0+=nx;
		p10+=nx;
		p20+=nx;
		p30+=nx;
		p40+=nx;
	}
#else
	float tmp1,tmp2,tmp3,tmp4;
	H3_ARRAY2D_FLT32 AA(ny,nx);
	AA.Fill(0);
	float *pAA0=AA.GetData(),*pAA;
	pAA0 += NN2*nx+NN2;
	for(i=NN2;i<Imax;i++){
		pA=pA0;
		pAA=pAA0;
		p1=p10; p2=p20; p3=p30; p4=p40;
		for(j=NN2;j<Jmax;j++){
			tmp1=(*(p1))-(*(p3));//en principe sur un damier tmp1 et tmp2 de meme signe et meme valeur
			tmp2=(*(p2))-(*(p4));
			tmp3=(*(p1))-(*(p4));
			tmp4=(*(p2))-(*(p3));
			(*pA)=fabs( tmp1+tmp2 )/4.0f;
			(*pAA)=((fabs(tmp1/tmp2-1.0f)<0.5) && (fabs(tmp3/tmp4-1.0f)<0.5));//il est souhaitable de dilater un peu les zones reperees avant de faire un AND entre A et AA

			if((*pA)>a_max) a_max=(*pA);

			p1++;p4++;
			p2++;p3++;
			pA++;
			pAA++;
		}
		pA0+=nx;
		pAA0+=nx;
		p10+=nx;
		p20+=nx;
		p30+=nx;
		p40+=nx;
	}

#endif

if(doSave)
	A.Save("c:\\temp\\A.hbf");
	//seuillage
	a_seuil=a_max*seuil_factor;

	float *pCC0,*pCC;
	pA0=A.GetData()+NN2*nx+NN2;
	pCC0=(ChessConvol.GetData()) + NN2*nx+NN2;

	for(i=NN2;i<Imax;i++){
		pA=pA0;
		pCC=pCC0;
		for(j=NN2;j<Jmax;j++){
			if( (*(pA))>a_seuil) (*(pCC))=1.0;
			pA++;
			pCC++;
		}
		pA0+=nx;
		pCC0+=nx;
	}
if(doSave)
	ChessConvol.Save("c:\\temp\\inChessConvol.hbf");

	return true;
}

template < class TYPE > static
bool ChessConvol1(CH3Array2D<TYPE>& Image,unsigned long N,H3_ARRAY2D_FLT32 &ChessConvol,bool doSave=false)
{
	CString strFunction("ChessConvol");

	if(&ChessConvol==(H3_ARRAY2D_FLT32*)(&Image)) return false;

	size_t nx=Image.GetCo(),ny=Image.GetLi();

	if(ChessConvol.GetLi()!=ny || ChessConvol.GetCo()!=nx){
		ChessConvol = H3_ARRAY2D_FLT32(ny,nx);
	}
	ChessConvol.Fill(0);

	size_t i,j;
	size_t N2=(N+1)/2,NN2=N/2;

	float a_seuil,seuil_factor= CORNERFINDER_SEUIL;

	H3_ARRAY2D_FLT32 filtreX =filtre(Image  ,N,'x');
	H3_ARRAY2D_FLT32 filtreXY=filtre(filtreX,N,'y');

	H3_ARRAY2D_FLT32 A(ny,nx);
	A.Fill(0);

	float a_max=0.0f;
	
	//filtreXY est le resultat du filtrage de Image par un filtre moyenneur de taille N*N
	//A va etre rempli a partir de 4 elements de filtreXY, distant de N lignes/colonnes 
	//(en realité distance=N2+NN2)
	//le point de A etudié est le centre du carré
	//il est inutile de s'interresser aux N/2 lignes superieures comme inferieures et
	//aux N/2 colonnes gauches et droites 
	float *pA0=A.GetData();
	float *pA;

	float *p10=filtreXY.GetData(),*p20,*p30,*p40;
	float *p1,*p2,*p3,*p4;

	pA0 += NN2*nx+NN2;


	//p10					//ligne 1 colonne 1 d'un damier 2lignes*2colonnes
	p40  = p10+(NN2+N2)*nx;	//ligne 2 colonne 1 d'un damier 2lignes*2colonnes
	p30  = p10+NN2+N2;		//ligne 1 colonne 2 d'un damier 2lignes*2colonnes
	p20  = p40+NN2+N2;		//ligne 2 colonne 2 d'un damier 2lignes*2colonnes

	unsigned long Imax=ny-N2,Jmax=nx-N2;

	float sigmaIM;
	float sigmaI;
	float sigmaI2;
	float mean;
	for(i=NN2;i<Imax;i++){
		pA=pA0;

		p1=p10; p2=p20; p3=p30; p4=p40;

		for(j=NN2;j<Jmax;j++){
			sigmaIM=fabs( ( (*p1) + (*p2)  )-
						  ( (*p3) + (*p4)  ) );
			mean=( (*p1) + (*p2)+ (*p3) + (*p4) )/4.0f;

			(*pA)=sigmaIM;

			if((*pA)>a_max) a_max=(*pA);

			pA++;
			p1++; p2++; p3++; p4++;
		}
		pA0+=nx;

		p10+=nx; p20+=nx; p30+=nx; p40+=nx;
	}

	//seuillage
	a_seuil=a_max*seuil_factor;

	float *pCC0,*pCC;
	pA0=A.GetData()+NN2*nx+NN2;
	pCC0=(ChessConvol.GetData()) + NN2*nx+NN2;

	for(i=NN2;i<Imax;i++){
		pA=pA0;
		pCC=pCC0;
		for(j=NN2;j<Jmax;j++){
			if( (*(pA))>a_seuil) (*(pCC))=1.0;
			pA++;
			pCC++;
		}
		pA0+=nx;
		pCC0+=nx;
	}

	return true;
}

#define CHESSCONVOL2_GL_SEUIL_NOIR_BLANC (15L)
#define CHESSCONVOL2_GL_SEUIL2 (0.5f)

template < class TYPE > static
bool ChessConvol2(CH3Array2D<TYPE>& Image,unsigned long N,H3_ARRAY2D_FLT32 &ChessConvol,	long  nCornerFinderFirstGuessStepSE,bool doSave=false)
{
	CString strFunction("ChessConvol2a");

	if(&ChessConvol==(H3_ARRAY2D_FLT32*)(&Image)) return false;

	unsigned long nx=Image.GetCo(),ny=Image.GetLi();

	if(ChessConvol.GetLi()!=ny || ChessConvol.GetCo()!=nx){
		ChessConvol = H3_ARRAY2D_FLT32(ny,nx);
	}
	ChessConvol.Fill(0);

	unsigned long i,j;
	unsigned long N2=(N+1)/2,NN2=N/2;

	//float a_seuil;
	float seuil_factor= nCornerFinderFirstGuessStepSE;

	H3_ARRAY2D_FLT32 filtreX =filtre(Image  ,N,'x');
	H3_ARRAY2D_FLT32 filtreXY=filtre(filtreX,N,'y');

	H3_ARRAY2D_FLT32 A(ny,nx);
	A.Fill(0);

	float a_max=0.0f;
	
	//filtreXY est le resultat du filtrage de Image par un filtre moyenneur de taille N*N
	//A va etre rempli a partir de 4 elements de filtreXY, distant de N lignes/colonnes 
	//(en realité distance=N2+NN2)
	//le point de A etudié est le centre du carré
	//il est inutile de s'interresser aux N/2 lignes superieures comme inferieures et
	//aux N/2 colonnes gauches et droites 
	float *pA0=A.GetData();
	float *pA;

	float *p10=filtreXY.GetData(),*p20,*p30,*p40;
	float *p1,*p2,*p3,*p4;

	pA0 += NN2*nx+NN2;


	//p10					//ligne 1 colonne 1 d'un damier 2lignes*2colonnes
	p40  = p10+(NN2+N2)*nx;	//ligne 2 colonne 1 d'un damier 2lignes*2colonnes
	p30  = p10+NN2+N2;		//ligne 1 colonne 2 d'un damier 2lignes*2colonnes
	p20  = p40+NN2+N2;		//ligne 2 colonne 2 d'un damier 2lignes*2colonnes

	unsigned long Imax=ny-N2,Jmax=nx-N2;
	
	float tmp1,tmp2,tmp3,tmp4,tmp5,tmp6;
	H3_ARRAY2D_FLT32 AA(ny,nx);
	AA.Fill(0);
	float *pAA0=AA.GetData(),*pAA;
	pAA0 += NN2*nx+NN2;

	float fSeuil=N*N*CHESSCONVOL2_GL_SEUIL_NOIR_BLANC;
	float fSeuil2=CHESSCONVOL2_GL_SEUIL2;
	for(i=NN2;i<Imax;i++){
		pA=pA0;
		pAA=pAA0;
		p1=p10; p2=p20; p3=p30; p4=p40;
		for(j=NN2;j<Jmax;j++){
			tmp1=(*p1)-(*p3);//en principe sur un damier tmp1 et tmp2 de meme signe et meme valeur
			tmp2=(*p2)-(*p4);
			tmp3=(*p1)-(*p4);//en principe sur un damier tmp3 et tmp4 de meme signe et meme valeur
			tmp4=(*p2)-(*p3);
			(*pA)=(tmp1+tmp2 );

			if(tmp1*tmp2>fSeuil)
			{
				tmp5=tmp1/tmp2;//idealement, tmp5=1 sur une arrete horizontale au voisinage d'une intersection
				if(tmp5>1.0f)
				{
					tmp5=1.0f/tmp5;
				}
				if (tmp5<0) (*pAA)=0;
				else
				{
					if(tmp3*tmp4>fSeuil)
					{
						tmp6=tmp3/tmp4;//idealement, tmp6=1 sur une arrete verticale au voisinage d'une intersection
						if(tmp6>1.0f) tmp6=1.0f/tmp6;
						if (tmp6<0) (*pAA)=0;
						else
						{
							//	il est souhaitable de dilater un peu les zones reperees avant de faire un AND entre A et AA
							if((tmp5>fSeuil2) && (tmp6>fSeuil2)) (*pAA)=SGN(tmp1);
						}
					}
				}
			}
			else
				(*pAA)=0;

			float absA=fabs(*pA);
			if(absA>a_max) a_max=absA;

			p1++; p2++; p3++; p4++;
			pA++;
			pAA++;
		}
		pA0+=nx;
		pAA0+=nx;
		p10+=nx;
		p20+=nx;
		p30+=nx;
		p40+=nx;
	}

	ChessConvol=AA;

	if(doSave)
	{
		ChessConvol.Save("c:\\temp\\inChessConvol.hbf");
		A.Save("c:\\temp\\inChessConvolA.hbf");
	}
	return true;
}

//convolution speciale d'une image par un element 1*3 ou 3*1 appelé V
//le premier ou le dernier element de V est nul
template < class TYPE > static//image est un tableau d'entier ou de float
bool Convol(const H3_ARRAY2D& Image,const H3_ARRAY2D_FLT32& V,H3_ARRAY2D_FLT32& ConvolI)
{
	CString strFunction("Convol");

	if(&ConvolI==(H3_ARRAY2D_FLT32*)&Image){
		return false;
	}

	unsigned long nx=Image.GetCo(),ny=Image.GetLi();
	unsigned long nxV=V.GetCo(),nyV=V.GetLi();
	unsigned long  i,j;
	long Imax,Jmax;
	float V0=V[0],V1=V[1],V2=V[2];
	
	if(ConvolI.GetLi()!=ny || ConvolI.GetCo()!=nx )
		ConvolI.ReAlloc(ny,nx);
	TYPE *pI_0=Image.GetData(),*pI_1;

	char cas='a';
	if(nxV==3 && nyV==1){
		cas='x';
		Jmax=nx;
	}
	if(nxV==1 && nyV==3){
		cas='y';
		Imax=ny;		
	}

	float *pItmp=ConvolI.GetData();

	switch(cas){
	case'x':
		//convolution sur les colonnes par [V2, V1, V0]
		if(V2!=0){
			pI_1=pI_0;pI_1--;
			for(i=0;i<ny;i++){
				{//remplissage speciale du 1er element de la colonne
					(*(pItmp++))=float(*(pI_0++))*V1;
					pI_1++;
				}
				for(j=1;j<Jmax;j++){
					(*(pItmp++))=float(*(pI_0++))*V1+float(*(pI_1++))*V2;
				}
			}
		}
		else{
			pI_1=pI_0;pI_1++;
			Jmax -= 1;//la derniere case subit un traitement speciale si 's'
			for(i=0;i<ny;i++){
				for(j=0;j<Jmax;j++){
					(*(pItmp++))=float(*(pI_0++))*V1+float(*(pI_1++))*V0;
				}
				//remplissage speciale du dernier element de la colonne
				(*(pItmp++))=float(*(pI_0++))*V1;
				pI_1++;

			}	
		}
	break;
	case'y':
		//convolution sur les lignes
		if(V2!=0){
			pI_1=pI_0;
			for(j=0;j<nx;j++){
				(*(pItmp++))=float(*(pI_0++))*V1;
			}
			for(i=1;i<ny;i++){
				for(j=0;j<nx;j++){
					(*(pItmp++))=float(*(pI_0++))*V1+float(*(pI_1++))*V2;
				}
			}
		}
		else{
			pI_1=pI_0;pI_1 += nx;
			unsigned long Imax=ny-1;
			for(i=0;i<Imax;i++){
				for(j=0;j<nx;j++){
					(*(pItmp++))=float(*(pI_0++))*V1+float(*(pI_1++))*V0;
				}
			}
			for(j=0;j<nx;j++){
				(*(pItmp++))=float(*(pI_0++))*V1;
				pI_1++;
			}
		}
		break;
	default:
		#if defined(H3APPMESSAGETOOLSDECL_H__INCLUDED_)
		H3DebugError(strModule,"Convol","error");
		#endif
		return false;
		break;
	}
	return true;
}

template < class TYPE > static
void Gradient(const H3_ARRAY2D & Image,char sens, H3_ARRAY2D_FLT32& GradImage)
{
	size_t ny=Image.GetLi(),nx=Image.GetCo();
	unsigned long i,j,Imax,Jmax;

	if(GradImage.GetLi()!=ny || GradImage.GetCo()!=nx ){
		GradImage.ReAlloc(ny,nx);
	}

	H3_ARRAY2D_FLT32 Image_flt=(const H3_ARRAY2D_FLT32)(Image);
	float *pI_0,*pI_1;
	float *pItmp;

	switch(sens){
	case'x':
	case'X':
		//gradient x
		pI_0=Image_flt.GetData();
		pI_1=pI_0;pI_1=++;
		pItmp=GradImage.GetData();
		//pour la premiere colonne: x(1)-x(0)
		//pour la derniere colonne: x(n)-x(n-1)
		//pour les autres: (x(i+1)-x(i-1))/2
		Jmax=nx-1;
		for(i=0;i<ny;i++){
			(*(pItmp  )) =(*(pI_1++));
			(*(pItmp++))-=(*(pI_0));
			for(j=1;j<Jmax;j++){
				(*(pItmp  ))= (*(pI_1++));
				(*(pItmp  ))-=(*(pI_0++));
				(*(pItmp++)) /= 2.0;
			}
			pI_1--;
			(*(pItmp  ))= (*(pI_1++));
			(*(pItmp++))-=(*(pI_0++));
			pI_1++;
			pI_0++;
		}
		break;
	case'y':
	case'Y':
		//gradient y
		pI_0=Image_flt.GetData();
		pI_1=pI_0;pI_1 += nx;
		pItmp=GradImage.GetData();
		//ligne 0
		for(j=0;j<nx;j++){
			(*(pItmp  ))= (*(pI_1++));
			(*(pItmp++))-=(*(pI_0++));
		}
		//lignes 1 à n-1
		pI_0-=nx;
		Imax=ny-1;
		for(i=1;i<Imax;i++){
			for(j=0;j<nx;j++){
				(*(pItmp  ))= (*(pI_1++));
				(*(pItmp  ))-=(*(pI_0++));
				(*(pItmp++)) /= 2.0;
			}
		}
		//derniere ligne
		pI_1-=nx;
		for(j=0;j<nx;j++){
			(*(pItmp  ))= (*(pI_1++));
			(*(pItmp++))-=(*(pI_0++));
		}

		break;
	}
}

//sous echantillonage d'une image (prelevement 1pixel tous les stepSE*stepSE)
//in= Ima
//in= stepSE: pas d'echantillonnage
//out=I_SE: imag sous echantillonnée
template < class TYPE > static
bool ReSample(const H3_ARRAY2D &Ima,long stepSE,H3_ARRAY2D& I_SE)
{
	CString strFunction("ReSample");
	H3DebugInfo(strModule,strFunction,"FindSquareFct in");

	if(&Ima==&I_SE){ 
		H3DebugInfo(strModule,strFunction,"FindSquareFct pb1");
		return false;
	}

	size_t nx=Ima.GetCo(),ny=Ima.GetLi();
	size_t i,j;

	//______________si stepSE>nx ou ny :PB
	size_t nxSE=nx/stepSE,nySE=ny/stepSE;

	if(stepSE>nx || stepSE>ny){
		CString msg;
		msg.Format("step=%d superieur à nx=%d ou à ny=%d",stepSE,nx,ny);
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugError(strModule,"ReSample",msg);
		#else
		AfxMessageBox(msg);
		#endif
		return false;
	}
	if(I_SE.GetLi()!=nySE || I_SE.GetCo()!=nxSE)
		I_SE.ReAlloc(nySE,nxSE);

	TYPE *pI0=Ima.GetData(),*pI,*pISE=I_SE.GetData();

	for(i=0;i<nySE;i++){
		pI=pI0;
		for(j=0;j<nxSE;j++){
			*pISE = *pI;
			pISE++;
			pI += stepSE;
		}
		pI0 +=nx*stepSE;
	}
	H3DebugInfo(strModule,strFunction,"FindSquareFct out");
	return true;
}

////////////////////////////////////////////////////Fin Static

//cv 18/07/05
//d'apres cornerfinder
//xt: InitialGuess
//wintx, winty: taille de la fenetre de recherche
//wx2,wy2: zone à 0 autour du centre de la fenetre de recherche
template <class TYPE>
H3_ARRAY_PT2DFLT32 cornerfinder1d(const H3_ARRAY_PT2DFLT32& xt,
								const H3_ARRAY2D & Image,
								unsigned long  wintx,unsigned long  winty,
								long  wx2,long  wy2,float fCornerFinderResolution,	long  nCornerFinderMaxIter)
{
	CString strFunction("cornerfinder");

	long nbPoints=xt.GetSize();

	//parametres d'optimisation
	float resolution=fCornerFinderResolution,v_extra;
	unsigned long MaxIter=nCornerFinderMaxIter;//le temps d'execution est directement proportionnel à MaxIter
	//
	bool line_feat=1;
	long  wXsz=2*(long)wintx+1,wYsz=2*(long)winty+1;
	H3_ARRAY_PT2DFLT32 xc(xt);
	H3_ARRAY2D_FLT32 mask=GaussImage(wYsz,wXsz,wy2,wx2);
	H3_ARRAY2D_FLT32 offx,offy;
	H3_MATRIX_FLT32 tmpX(1,wXsz),tmpY(wYsz,1);

	long i;
	const size_t nx=Image.GetCo(),ny=Image.GetLi();
	H3_ARRAY_UINT8 type(nbPoints),bad(nbPoints);

	//offx et offy sont des cartes d'offset en x et y
	//creation de offx
	for(i=0;i<wXsz;i++)
		tmpX[i]=float(i)-wintx;
	tmpY.Fill(1.0f);
	offx=H3_ARRAY2D_FLT32(tmpY*tmpX);
	//creation de offy
	for(i=0;i<wYsz;i++)
		tmpY[i]=float(i)-winty;
	tmpX.Fill(1.0f);
	offy=H3_ARRAY2D_FLT32(tmpY*tmpX);

	//la boucle principale
	unsigned long compt=0;
	//Boucle1
	float cIx,cIy;//les coordonnées du i_ieme point
	long crIx,crIy,crIx_old=-nx,crIy_old=-ny;//les coordonnées arrondies du i_ieme point
	float itIx,itIy;
	H3_ARRAY2D_FLT32 vIx(1,3),vIy(3,1);
	long xmin,xmax,ymin,ymax;
	bool change=true;
	//Boucle2
	H3_ARRAY2D SI;
	//Boucle3
	H3_ARRAY2D_FLT32 SIy,SIxy,SI0;
	//Boucle4
	//Gradient sous image
	H3_ARRAY2D_FLT32 gy,gx;
	//Boucle5
	H3_ARRAY2D_FLT32 px,py,gxx,gyy,gxy,tmp_Array2D;
	//Boucle6
	float bb0,bb1,a,b,c,dt,xc2_0,xc2_1;
	//Boucle7
	H3_MATRIX_FLT32 G(2,2),U(2,2),S(1,2),V(2,2);
	float tmp0,tmp1;

	for (i=0;i<nbPoints;i++){
		compt=0;
		v_extra=1.0+resolution;
		while((v_extra>resolution) && (compt<MaxIter)){//indices x et y inversés par rapport à Matlab
		{//Boucle1
			cIx=xc[i].x;
			cIy=xc[i].y;
			if(!_finite(cIx+cIy)){
				v_extra=0;
				continue;
			}

			crIx=floor(cIx+0.5);
			crIy=floor(cIy+0.5);

			itIx=cIx-crIx;
			itIy=cIy-crIy;

			if(itIx>0){
				vIx[0]=itIx; vIx[1]=1-itIx;	vIx[2]=0.0f;
			}
			else{
				vIx[0]=0.0f; vIx[1]=1+itIx;	vIx[2]=-itIx;
			}

			if(itIy>0){
				vIy[0]=itIy; vIy[1]=1-itIy;	vIy[2]=0.0f;
			}
			else{
				vIy[0]=0.0f; vIy[1]=1+itIy;	vIy[2]=-itIy;
			}
		}
		{//boucle2
			//on va extraire une sous image
			change=( crIx!=crIx_old || crIy!=crIy_old);

			if(change){
				if(crIx-(long)wintx-2 < 0){
					xmin=0;xmax=wXsz+3;
				}
				else{
					if(crIx+(long)wintx+2 >= nx){
						xmax=nx-1;xmin=nx-wXsz-4;
					}
					else{
						xmin=crIx-(long)wintx-2;
						xmax=crIx+(long)wintx+2;
					}
				}
				if(crIy-(long)winty-2 < 0){
					ymin=0;ymax=wYsz+3;
				}
				else{
					if(crIy+(long)winty+2 >= ny){
						ymax=ny-1;ymin=ny-wYsz-4;
					}
					else{
						ymin=crIy-(long)winty-2;
						ymax=crIy+(long)winty+2;
					}
				}
				//sous Image SI
				SI=(Image.GetAt(ymin,xmin,wYsz+4,wXsz+4));
			}
		}

			//convolution
		{//Boucle3
			Convol(SI ,vIy,SIy );
			Convol(SIy,vIx,SIxy);

			SI0=SIxy.GetAt(1,1,wYsz+2,wXsz+2);
		}
			//gradient
		{//Boucle4
			Gradient(SI0,'x',gx);
			Gradient(SI0,'y',gy);

			gx=gx.GetAt(1,1,wYsz,wXsz);
			gy=gy.GetAt(1,1,wYsz,wXsz);
		}
		{//Boucle5
			px=offx+cIx;
			py=offy+cIy;

			gxx=gx*gx*mask;
			gyy=   gy*mask;
			gxy=gx*gyy;
			gyy=gy*gyy;
		}
		{//boucle6	
			tmp_Array2D=gxx*px+gxy*py;

			bb1=tmp_Array2D.GetSum();
			tmp_Array2D=gxy*px+gyy*py;

			bb0=tmp_Array2D.GetSum();
			a=gyy.GetSum();
			b=gxy.GetSum();
			c=gxx.GetSum();

			dt=a*c-b*b;

			xc2_1=(c*bb0-b*bb1)/dt;
			xc2_0=(a*bb1-b*bb0)/dt;
		}
		{//Boucle 7
			if(line_feat){
				
				G(0,0)=a;
				G(1,0)=G(0,1)=b;
				G(1,1)=c;

				G.SVDcmp(U,S,V);

				if(S(0)/S(1) >50){//inverser G, c'est risquer de commettre une grosse erreur >> on projette le point sur 'edge-orthogonal'
					float tmp=(cIx-xc2_0)*V(0,1)+(cIy-xc2_1)*V(1,1);
					xc2_1 += tmp*V(0,1);
					xc2_0 += tmp*V(1,1);

					type[i]=1;
				}
			}
		}
			tmp0=xc[i].x-xc2_0;
			tmp1=xc[i].y-xc2_1;
			v_extra=sqrt(tmp0*tmp0+tmp1*tmp1);//dans matlab, on obtient un vecteur dont on utilise que la norme
			xc[i].x=xc2_0;
			xc[i].y=xc2_1;
			compt++;
		}//fin while
	}//fin for (tous les points)

	//verification pour les points qui divergent
	{
		float dx,dy;
		for(i=0;i<xt.GetSize();i++){
			dx=xc[i].x - xt[i].x;
			dy=xc[i].y - xt[i].y;

			bad[i]=(fabs(dx)>wintx || fabs(dy)>winty);

			if(bad[i]){
				xc[i].x= xt[i].x;
				xc[i].y= xt[i].y;
			}
		}
	}

	return xc;
}

template <class TYPE>
H3_ARRAY2D_PT2DFLT32 cornerfinder2d(const H3_ARRAY2D_PT2DFLT32& xt,
								const H3_ARRAY2D & Image,
								unsigned long  wintx,unsigned long  winty,
								long  wx2,long  wy2,float fCornerFinderResolution,	long  nCornerFinderMaxIter)
{
	const size_t nLi=xt.GetLi(),nCo=xt.GetCo();
	H3_ARRAY_PT2DFLT32 pt=cornerfinder1d(xt,
								Image,wintx,winty,
								wx2,wy2,
								fCornerFinderResolution,
								nCornerFinderMaxIter);
	H3_ARRAY2D_PT2DFLT32 pt2d(pt);

	return pt2d;
}

//cornerfinder_firstGuess
//cherche dans une image des zones equivalentes à la presence de carrés noir et blanc en damier
//principe: correlation avec un damier à 4 case
template <class TYPE>
bool cornerfinder_firstGuess(strGuessResult& GR, const H3_ARRAY2D & Image,long nCornerFinderFirstGuessN,	long  nCornerFinderFirstGuessStepSE,float fCornerFinderSeuil)
{
	CString strFunction("cornerfinder_firstGuess(a)"),msg;
H3DebugInfo(strModule, strFunction,"1");
	unsigned size_t nx=Image.GetCo(),ny=Image.GetLi();
	unsigned long i,j;
	//la matrice representant le motif que l'on va chercher dans l'image aura pour taille 
	//(2*N+1)*(2*N+1) pixels ou 2N*2N suivant le cas
	unsigned long N=nCornerFinderFirstGuessN;
	unsigned long N2=(N+1)/2;
	//on va sous echantilloner l'image pour une recherche rapide, avec un pas stepSE
	//attention, si une case du damier est plus petite que N*stepSE, elle ne sera pas détectée
	unsigned long stepSE=nCornerFinderFirstGuessStepSE;
	unsigned long N3=(N2*stepSE)/2;

	if(N3<2){
		N3=2;
		N2=__max(2*N3/stepSE,1);
	}

	//creation d'une image sous échantillonnée
	//unsigned long nxSE=nx/stepSE,nySE=ny/stepSE;//______________si stepSE>nx ou ny :PB
	H3_ARRAY2D ImageSE;

	if(!ReSample(Image,stepSE,ImageSE)){
		strGuessResult GR;
		return GR;
	}
	H3DebugInfo(strModule, strFunction,"2");
	//convolution par un damier
	H3_ARRAY2D_FLT32 ChessConvolI;
	ChessConvol2(ImageSE,N,ChessConvolI,fCornerFinderSeuil,false);
	H3DebugInfo(strModule, strFunction,"3");
	//IntersecSE va contenir les intersections à +- stepSE
	H3_ARRAY_PT2DFLT32 IntersecSE;
	H3_ARRAY_INT8 Sgn0;
	if(!bwLabel(ChessConvolI,IntersecSE)){
		strGuessResult GR;
		return GR;
	};
	Sgn0=GetSgn(ChessConvolI,IntersecSE);
	//il faut maintenant affiner 
	//pour cela on prend une imagette centrée sur chaque cible trouvée
	//dans l'image complete et on cherche un damier

	//soit (xm,ym) le centre trouvé dans l'image sous échantillonnée
	//(Xm,Ym) le centre dans l'image : (Xm,Ym)=(xm,ym)*stepSE
	//on selectionne une imagette centrée sur (Xm,Ym)
	//le coin en haut a gauche est (x0,y0)=(Xm-d,Ym-d)
	//hauteur et largeur valent dx0 et dy0 (2d+1)
	unsigned long nbPoints=IntersecSE.GetSize();

	if (nbPoints==0){
		// Le motif recherché n'a pas ete trouvé
		return false;
	}
	H3DebugInfo(strModule, strFunction,"4");
	H3_ARRAY_PT2DFLT32 Intersec(nbPoints),PointI;
	H3_ARRAY_INT8 Sgn(nbPoints);
	H3_ARRAY2D Imagette;
	H3_ARRAY2D_FLT32 bwImagette;

	long x0,y0,d=N2*stepSE,dx0=2*d+1,dy0=2*d+1,dx,dy,x00,y00;
	for(i=0,j=0;i<nbPoints;i++){
		x00=x0=(IntersecSE[i].x+0.5)*stepSE-d ;
		y00=y0=(IntersecSE[i].y+0.5)*stepSE-d ;

		if(x0<0){
			x0=0;
			dx=dx0/2+1;
		}
		else if(x0>nx-dx0){
			if(x0>nx-dx0/2) x0=nx-x0/2;

			dx=((nx-x0+1)/2)*2+1;
			x0=nx-dx;
		}
		else{
			dx=dx0;
		}
		if(y0<0){
			y0=0;
			dy=dy0/2+1;
		}
		else if(y0>ny-dy0){
			if(y0>ny-dy0/2) y0=ny-dy0/2;

			dy=((ny-y0+1)/2)*2+1;
			y0=ny-dy;
		}
		else{
			dy=dy0;
		}
		
		Imagette=Image.GetAt(y0,x0,dy,dx);
		
		ChessConvol2(Imagette,N3,bwImagette,fCornerFinderSeuil,false);
		if(!bwLabel(bwImagette,PointI)){
			PointI.ReAlloc(0);
		}

		//PointI est du type H3_ARRAY_PT2DFLT32 car il peut y avoir
		//plusieurs reponses pertinentes meme si cela ne devrait pas arriver

		if(PointI.GetSize()>0){
			Sgn[j]=Sgn0[i];
			Intersec[j++]=PointI[0]+H3_POINT2D_FLT32(x0,y0);		
		}
		else{
		}
	}
	if(j!=nbPoints){
		Intersec=Intersec.GetAt(0,j);
		Sgn=Sgn.GetAt(0,j);
	}

	GR.Points=Intersec;
	GR.Sgn=Sgn;
	H3DebugInfo(strModule, strFunction,"end");
	return false;
}

#endif