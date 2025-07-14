//SortFct.cpp

#include "stdafx.h"
#include "coveli.h"
#include "SortFct.h"

#define DEFAULT_COEFF_FILTER 0.25f
#define DEFAULT_SEUIL2 1.5f
#define DEFAULT_SEUIL3 2.9f
#define DEFAULT_N 5.5f

#define DEFAULT_NB_STD 2.0f

static CString strModule("SortFct");
//______________________________________________________________________________________
//calcul la moyenne et l'ecart type des coordonnées x et y d'un nuage de point
static bool GetStat(const H3_ARRAY_PT2DFLT32 &Pts, float &StdDevX, float &StdDevY, float &MeanX, float &MeanY) 
{
	long sz=Pts.GetSize();
	if(sz<2L) return false;

	H3_POINT2D_FLT32* pPt=Pts.GetData();

	MeanX=MeanY=0;// valeur moyenne de x et y
	StdDevX=StdDevY=0;// ecart type
	long i=0L;

	for(i=0L;i<sz;i++)
	{
		MeanX+=pPt->x;
		MeanY+=pPt->y;

		StdDevX+=(pPt->x * pPt->x);
		StdDevY+=(pPt->y * pPt->y);

		pPt++;
	}
	MeanX/=sz;
	MeanY/=sz;
	StdDevX=sqrt(StdDevX/sz-MeanX*MeanX);
	StdDevY=sqrt(StdDevY/sz-MeanY*MeanY);

	return true;
}

//recherche dans une liste de points ceux qui sont situés à moins ne n écart type du barycentre
//calcul le barycentre des points restants
//MeanX et MeanY: l'ancien puis le nouveau barycentre
static H3_ARRAY_UINT8 GetValid(const H3_ARRAY_PT2DFLT32 &Pts, float StdDevX, float StdDevY, float &MeanX, float &MeanY, float n)
{
	long sz=Pts.GetSize();
	H3_POINT2D_FLT32* pPt=Pts.GetData();

	float dx=0.0f,dy=0.0f;
	float stdx=n*StdDevX,stdy=n*StdDevY;
	long nbValidPoints=0L,i;
	H3_ARRAY_UINT8 IsIn(sz);//0: est trop loin du barycentre; 1: est bon

	for(i=0L;i<sz;i++){
		if((fabs(pPt->x - MeanX)<stdx) && (fabs(pPt->y - MeanY)<stdy)){ 
			//le point est valide
			nbValidPoints++;
			IsIn[i]=1;

			dx += pPt->x;
			dy += pPt->y;
		}
		else{
			IsIn[i]=0;
		}
		pPt++;
	}
	if (nbValidPoints<2L) return H3_ARRAY_UINT8(0L);// ce cas doit se produire si Pts est constitué d'un seul point

	dx/=nbValidPoints;//le barycentre du nuage interressant est B(dx2,dy2);
	dy/=nbValidPoints;

	MeanX=dx;
	MeanY=dy;

	return IsIn;
}

static H3_ARRAY_PT2DFLT32 GetCorner(H3_ARRAY_PT2DFLT32 &Pts, H3_ARRAY_UINT8 &IsIn, H3_POINT2D_FLT32 &B )
{
	//on cherche maintenant parmis les points valides (IsIn) d'un ensemble de points Pts , P1, le plus eloigné du barycentre B
	//puis P2 et P3, les plus eloignés de la droite BP1 de part et d'autre de celle ci
	//puis P4 et P5, les plus eloignés de la droite P2P3 (en principe P1 est l'un d'eux, sauf si il manque un point dans un coin)
	H3_POINT2D_FLT32 P1,P2,P3,P4,P5;
	H3_POINT2D_FLT32* pPt=Pts.GetData();
	long sz=Pts.GetSize();
	double dd,d1=0;

	long i;
	for(i=0L;i<sz;i++){
		if(IsIn[i]){
			dd=B.DistanceTo(pPt[i]);
			if(dd>d1){
				d1=dd;
				P1=pPt[i];
			}
		}
	}
	double a=(P1.y-B.y),b=(P1.x-B.x);
	double d2=0,d3=0;
	for(i=0L;i<sz;i++){
		if(IsIn[i]){
			dd=a*(pPt[i].x-B.x)-b*(pPt[i].y-B.y);//distance non normalisée, mais ce n'est pas un pb ici
			if(dd>d2){
				d2=dd;
				P2=pPt[i];
			}
			else{
				if(dd<d3){
					d3=dd;
					P3=pPt[i];
				}
			}
		}
	}

	// on a P2 et P3, on fait la meme chose pour P4 et P5
	a=(P2.y-P3.y);
	b=(P2.x-P3.x);
	double d4=0,d5=0;
	for(i=0L;i<sz;i++){
		if(IsIn[i]){
			dd=a*(pPt[i].x-P2.x)-b*(pPt[i].y-P2.y);//distance non normalisée, mais ce n'est pas un pb ici
			if(dd>d4){
				d4=dd;
				P4=pPt[i];
			}
			else{
				if(dd<d5){
					d5=dd;
					P5=pPt[i];
				}
			}
		}
	}
	//on a P4 et P5
	//tri des points pour que P2,P3,P4,P5 soit un trapeze
	//P2 le premier puis tri pour respecter le sens trigo inverse   (pourquoi/pourquoi pas...)
	H3_POINT2D_FLT32 V[4L]={(P2-B),(P3-B),(P4-B),(P5-B)}; //en fait: vecteur 2D
	H3_POINT2D_FLT32 Ptemp[4L]={(P2),(P3),(P4),(P5)},Ptmp;
	H3_ARRAY_PT2DFLT32 P(4L);

	V[0L]/= B.DistanceTo(Ptemp[0L]);
	V[1L]/= B.DistanceTo(Ptemp[1L]);
	V[2L]/= B.DistanceTo(Ptemp[2L]);
	V[3L]/= B.DistanceTo(Ptemp[3L]);

	float angle[3],angletmp;
	angle[0L]=atan2(V[1].x*(-V[0].y)+V[1].y*V[0].x,V[1].x*V[0].x+V[1].y*V[0].y);
	angle[1L]=atan2(V[2].x*(-V[0].y)+V[2].y*V[0].x,V[2].x*V[0].x+V[2].y*V[0].y);
	angle[2L]=atan2(V[3].x*(-V[0].y)+V[3].y*V[0].x,V[3].x*V[0].x+V[3].y*V[0].y);

	for(i=0L;i<3L;i++){
		if(angle[i]<0L) angle[i]+= float(2.0*PI);
	}

	long j,k;
	for(i=0L;i<2L;i++){	
		for(j=i+1L;j<3L;j++){
			if(angle[j]>angle[i]){//sens trigo: angle[j]<angle[i]
				angletmp =angle[i];
				angle[i]=angle[j];
				angle[j]=angletmp;

				Ptmp=Ptemp[i+1L];
				Ptemp[i+1L]=Ptemp[j+1L];
				Ptemp[j+1L]=Ptmp;
			}
		}
	}

	//on veux de plus que le point P0 soit le plus proche de l'origine pixel (aide dans certains cas)
	H3_POINT2D_FLT32 Origin(0.0f,0.0f);
	float dist,disti;
	dist=disti=Origin.DistanceTo(Ptemp[0L]);

	for(i=1L,j=0L;i<4L;i++){
		disti=Origin.DistanceTo(Ptemp[i]);
		if(disti<dist) {
			dist=disti;
			j=i;
		}
	}
	for(i=0L;i<4L;i++){
		k=(i+j)%4;
		P[i]=Ptemp[k];
	}

	return P;
}

//test si tous les points valides (IsIn) de Pts sont dans le trapeze P(P0,P1,P2,P3)
// certaines aberations peuvent les mettre en dehors cependant (barrillet)
//retourne le nombre de points restant (in2) et leur position IsIn2
static	H3_ARRAY_UINT8 GetValid2(H3_ARRAY_PT2DFLT32 &Pts,H3_ARRAY_PT2DFLT32& P, H3_ARRAY_UINT8 IsIn,long &in2)
{
	long i,j,k, sz=Pts.GetSize();
	H3_POINT2D_FLT32 B=(P[0L]+P[1L]+P[2L]+P[3L])/4.0;
	in2=0;

	H3_POINT2D_FLT32 V[4L],Ptmp;
	V[0L]=P[1L]-P[0L];	V[0L] /= P[1L].DistanceTo(P[0L]);
	V[1L]=P[2L]-P[1L];	V[1L] /= P[2L].DistanceTo(P[1L]);
	V[2L]=P[3L]-P[2L];	V[2L] /= P[3L].DistanceTo(P[2L]);
	V[3L]=P[0L]-P[3L];	V[3L] /= P[0L].DistanceTo(P[3L]);
	float s[4L],dd;
	for(i=0L;i<4L;i++){
		//determine de quel coté (+/-) de chaque droite se trouve B
		// tous les points devront se trouver du meme cote
		s[i]=V[i].x*(B.y-P[i].y)-V[i].y*(B.x-P[i].x);
	}

	H3_ARRAY_UINT8 IsIn2(sz);

	H3_ARRAY2D_FLT32 d_d(4L,sz),//distance à la droite
					d_b(4L,sz),d_e(4L,sz);//distance au debut de ligne et à la fin(begin,end)
	H3_POINT2D_FLT32* pPt=Pts.GetData();
	bool cond[4L];
	double theta1,theta2;

	for(i=0L;i<sz;i++){
		for(j=0L;j<4L;j++){
			k=((j+1L)%4);
			d_d(j,i)=(V[j].x*((pPt->y)-P[j].y)-V[j].y*((pPt->x)-P[j].x));
			d_b(j,i)=P[j].DistanceTo(*pPt);
			d_e(j,i)=P[k].DistanceTo(*pPt);
			cond[j]=(	((s[j]*d_d(j,i))>=0)	||	(d_b(j,i)<FLT_EPSILON)	||	(d_e(j,i)<FLT_EPSILON)	);
		}
		if(cond[0L]&&cond[1L]&&cond[2L]&&cond[3L]){
			IsIn2[i]=1L;
			in2++;
		}
		//si il y a des points proches des sommets et "loin" hors du trapeze >> pb un des coins n'etait pas dans la liste Pts
		//sinon: aberation barillet
		else{
			for(j=0L;j<4L;j++){
				if(!cond[j]){
					//on represente les triangle rectangles: extremité de ligne , Point, projection du point sur la ligne
					//je veux que les angles aux extremites soient inferieurs à angleMax (arbitraire)
					double anglemax=PI/12;
					theta1=fabs(asin(d_d(j,i)/d_b(j,i)));
					theta2=fabs(asin(d_d(j,i)/d_e(j,i)));
					if((theta1<anglemax) && (theta2<anglemax)){
						IsIn2[i]=1;
						in2++;
					}
					else{
						if(!IsIn[i]){
							IsIn2[i]=0;//le point ne fait vraiment pas parti de la mire
						}
						else{
							//la: pb sommet mal defini
							//le traitement demande peut etre des controles....? et en particulier de refaire le traitement concernant Pj
							//de toute facon, ce n'est pas la panacée
							if(theta1>(anglemax)){//le point P[j] est mal defini
								//on le redefini à l'aide de P[j],P,P[j+1]
								k=((j+1L)%4);
								Ptmp=((*pPt) - P[k]);
								dd=P[k].DistanceTo(P[j])/P[k].DistanceTo(*pPt);
								P[j]=H3_POINT2D_FLT32(Ptmp.x*dd,Ptmp.y*dd)+P[k];
							}
							if(theta2>(anglemax)){//le point P[j+1] est mal defini
								//on le redefini à l'aide de P[j],P,P[j+1]
								k=((j+1L)%4);
								Ptmp=((*pPt) - P[j]);
								dd=P[j].DistanceTo(P[((j+1L)%4)])/P[j].DistanceTo(*pPt);
								P[k]=H3_POINT2D_FLT32(Ptmp.x*dd,Ptmp.y*dd)+P[j];
							}
							IsIn2[i]=1;
							in2++;
						}
					}
				}
			}
		}
		pPt++;
	}

	return IsIn2;
}

//calcul de l'intersection entre deux droites definies par deux points
//droite A: A1A2
//droite B: B1B2
static H3_POINT2D_FLT32 GetIntersection(H3_POINT2D_FLT32& A1,H3_POINT2D_FLT32& A2,H3_POINT2D_FLT32& B1,H3_POINT2D_FLT32& B2)
{
	float xa1= A1.x , ya1= A1.y ,
		xa2= A2.x , ya2= A2.y ,
		xb1= B1.x , yb1= B1.y ,
		xb2= B2.x , yb2= B2.y ;
	float	dYA=( ya2 - ya1 ) ,
			dXB=( xb2 - xb1 ) ,
			dYB=( yb2 - yb1 ) ,
			dXA=( xa2 - xa1 );

	//equation de A: dYA x - dXA y =dYA xa1 - dXA ya1
	//equation de B: dYB x - dXB y =dYB xb1 - dXB yb1
	float delta = -dYA*dXB+dYB*dXA;
	float sol1=dYA*xa1-dXA*ya1 ,sol2=dYB*xb1-dXB*yb1 ;
	float deltaX= -sol1 * dXB + sol2 * dXA;
	float deltaY=  dYA * sol2 - dYB * sol1;

	if(delta!=0)
		return H3_POINT2D_FLT32(deltaX/delta,deltaY/delta);
	else
		return H3_POINT2D_FLT32(NaN,NaN);
}
//______________________________________________________________________________________

//cv 190704
//regarde dans une liste de points (Pts (colonne 0 1: position en pixel - x et y -)) ceux qui sont susceptible d'appartenir à une mire
//cherche les extremités
//redefini la position de chacun des points sur une grille reguliere
//ComputedPos :colonne 0 1: position des points retenus de Pts (pixel);colonnes 2 3 : position dans la grille (par pas de 1, en commencant à 0)
//Nx, Ny : nombre max de ligne et colonne que l'on est sensé trouver pour la grille
bool MySortFunc(H3_ARRAY_PT2DFLT32 &Pts,H3_ARRAY2D_FLT32 &ComputedPos,long Nx,long Ny,long maxx, long maxy)
{
	CString strFunction=("MySortFunc"),msg;

	float dx, dy;//valeur moyenne
	float sx, sy;//ecart type
	long i,j,k,l, sz=Pts.GetSize();
	H3_POINT2D_FLT32 Ptmp;

	if(!GetStat(Pts,sx,sy,dx,dy)) return false;

	//tri
	//les points qui se situent à une distance > n*std du barycentre sont éliminés
	//les autres permettent de calculer un nouveau barycentre
	//MeanX et MeanY sont modifiés (nouveau barycentre)
	H3_ARRAY_UINT8 IsIn =GetValid(Pts,sx,sy,dx,dy, 3);
	H3_POINT2D_FLT32 B(dx,dy);//le barycentre

	//recherche de 4 points du nuage (les coins si le nuage en a)
	H3_ARRAY_PT2DFLT32 Corners=GetCorner(Pts, IsIn, B );

	//nouveau tri
	long in2;
	H3_ARRAY_UINT8 IsIn2 =GetValid2(Pts,Corners,IsIn, in2);

	//on cherche maintenant une transformation simple pour redresser la forme observée
	H3_MATRIX_FLT32 Mx(4,4),X(4,1),Y(4,1),resX,resY,My;
	for(i=0;i<4;i++){
		Mx(i,0)=Corners[i].x;
		Mx(i,1)=Corners[i].y;
		Mx(i,2)=Corners[i].y*Corners[i].x;
		Mx(i,3)=1;
	}
	X(0,0)=0;X(1,0)=1;X(2,0)=1;X(3,0)=0;//les 4 coins d'une grille parfaite entre 0 et 1
	Y(0,0)=0;Y(1,0)=0;Y(2,0)=1;Y(3,0)=1;

	My=Mx;//Mat_MeanSquare modifie M (old version)

	resX=Mat_MeanSquare(Mx,X);
	if(resX.GetSize()!=4) {
		#if defined(H3APPMESSAGETOOLSDECL_H__INCLUDED_)
		H3DebugWarning("SortFct",strFunction,_T("Err resX "));
		#endif
		return false;
	}
	resY=Mat_MeanSquare(My,Y);
	if(resY.GetSize()!=4) {
		#if defined(H3APPMESSAGETOOLSDECL_H__INCLUDED_)
		H3DebugWarning("SortFct",strFunction,_T("Err resY "));
		#endif
		return false;
	}

	//transformation des points de la mire en quelque chose s'approchant d'un carré
	double ax=resX[0],bx=resX[1],cx=resX[2],ex=resX[3];
	double ay=resY[0],by=resY[1],cy=resY[2],ey=resY[3];
	double x,y;
	H3_ARRAY2D_FLT32 Ptm(in2,2);
	for(i=0,j=0;i<sz;i++){
		if(IsIn2[i]){
			x=Pts[i].x;
			y=Pts[i].y;

			Ptm(j,0)=ax*x+bx*y+cx*x*y+ex;
			Ptm(j,1)=ay*x+by*y+cy*x*y+ey;

			j++;
		}
	}

	//maitenant que les données sont presque organisées, je peux chercher le nb de ligne, le nb de colonne, les points manquant...
	long N=512;//N doit etre grand davant le nombre de ligne ou colone , mais pas trop pour eviter les trous dans l'histogramme
	H3_ARRAY_FLT32 Xval(N),Yval(N),Xedge(N),Yedge(N);
	Xval.Fill(0);
	Xedge.Fill(0);
	Yval.Fill(0);
	Yedge.Fill(0);

	float step=(1.0)/(N-1);
	// les valeurs se trouvent entre 0 et 1, je les mets dans [0,N-1]
	for(i=0;i<in2;i++){
		j=(long)floor((Ptm(i,0))/step+0.5);
		if(j<0) j=0;
		else if(j>(N-1)) j=N-1;
		Xval[j]++;

		j=(long)floor((Ptm(i,1))/step+0.5);
		if(j<0) j=0;
		else if(j>(N-1)) j=N-1;
		Yval[j]++;
	}

	//on cherche la position des fronts montants
	if(Xval[0]>0){
		Xedge[0]=1;
	}
	if(Yval[0]>0){
		Yedge[0]=1;
	}
	for(i=1;i<N;i++){
		if(Xval[i]>0){
			if(Xval[i-1]==0) Xedge[i]=1;
		}
		if(Yval[i]>0){
			if(Yval[i-1]==0) Yedge[i]=1;
		}
	}

	//distance moyenne d'un front au front suivant N/Nx et N/Ny au moins (Nx=26periode et Ny=18 d'apres les parametres de la feuille de marches)
	//si les points sont convenablement triés, on s'attend à des ecart entre fronts voisins de ces distances >> on elimine tout ce qui est inferieur au plus petit/1.25
	long NN=(long)(N/float(1.25*__max(Nx,Ny))),lastX=N,lastY=N;

	if(Xedge[0]==1){
		lastX=0;
	}
	if(Yedge[0]==1){
		lastY=0;
	}
	for(i=1;i<N;i++){
		if(Xedge[i]){
			if(lastX<i){
				if((i-lastX)<NN){
					Xedge[i]=0;//ecart insuffisant entre 2 valeurs valides
					lastX=i;
				}
				else{
					lastX=i;
				}
			}
			else{
				lastX=i;//premier element non nul
			}
		}
		if(Yedge[i]){
			if(lastY<i){
				if((i-lastY)<NN){
					Yedge[i]=0;
					lastY=i;
				}
				else{
					lastY=i;
				}
			}
			else{
				lastY=i;//premier element non nul
			}
		}
	}

	long sumX=-1,sumY=-1;
	for(i=0;i<N;i++){
		sumX += (long)Xedge[i];
		sumY += (long)Yedge[i];

		Xedge[i] = (float)sumX;
		Yedge[i] = (float)sumY;
	}

	long pos_edgex=2,pos_edgey=3;

#if 1//un nouveau tri est proposé plus bas utilisant la taille des deux points les plus gros,mais celui ci sert malgré tout	
	//tentative pour eviter des inversions de Xw et Yw pas forcement genantes par ailleurs
	//pb si on ne voit qu'un bout de la mire, que celle ci a plus de point en y qu'en x, et que c'est l'inverse pour la camera. par ex
	if(((sumY>sumX)&&(Nx>Ny))||((sumY<sumX)&&(Nx<Ny))){
		 pos_edgex=3;
		 pos_edgey=2;
	}
#endif

	ComputedPos=H3_ARRAY2D_FLT32(in2,4);
	H3_ARRAY2D_FLT32 ComputedPosTmp(in2,4);
	for(l=0,i=0;l<sz;l++){
		if(IsIn2[l]){
			j=(long)floor((Ptm(i,0))/step+0.5);
			k=(long)floor((Ptm(i,1))/step+0.5);

			if(j<0) j=0;
			else if(j>(N-1)) j=(N-1);
			if(k<0) k=0;
			else if(k>(N-1)) k=(N-1);
			
			ComputedPosTmp(i,0)=Pts[i].x;
			ComputedPosTmp(i,1)=Pts[i].y;
			ComputedPosTmp(i,pos_edgex)=Xedge[j];
			ComputedPosTmp(i,pos_edgey)=Yedge[k];

			i++;
		}
	}

	//tri des points

	H3_ARRAY_UINT32 Order(in2);
	H3_ARRAY_FLT32 Val(in2);
	float SSum=float(sumX+sumY+1.0),Otmp;
	float Vtmp;


	for(i=0;i<in2;i++){
		Val[i]=ComputedPosTmp(i,3)+ComputedPosTmp(i,2)/(SSum);//tri par ordre Y croissant puis X croissant
		Order[i]=i;
	}

	bool Continue=true;
	j=in2-1;
	while(Continue){
		Continue=false;
		for(i=0;i<(in2-1);i++){
			if(Val[i]>Val[i+1]){
				Vtmp=Val[i];
				Val[i]=Val[i+1];
				Val[i+1]=Vtmp;

				Otmp=(float)Order[i];
				Order[i]=Order[i+1];
				Order[i+1]=(H3_UINT32)Otmp;

				Continue=true;
			}
			k=j-i;
			if(Val[k-1]>Val[k]){
				Vtmp=Val[k-1];
				Val[k-1]=Val[k];
				Val[k]=Vtmp;

				Otmp=(float)Order[k-1];
				Order[k-1]=Order[k];
				Order[k]=(H3_UINT32)Otmp;

				Continue=true;
			}
		}
	}
	for(i=0;i<(in2);i++){
		ComputedPos.SetAt(i,0,ComputedPosTmp.GetAt(Order[i],0,1,4));
	}

	return true;
}

//cv 210905 d'apres fonction homonyme du meme auteur et du 241104
//regarde dans une liste de points (Pts (colonne 0 1: position en pixel - x et y -)) ceux qui sont susceptible d'appartenir à une mire
//redefini la position de chacun des points sur une grille reguliere
//ComputedPos :colonne 0 1: position des points retenus de Pts (pixel);colonnes 2 3 : position dans la grille (par pas de 1, en commencant à 0)
//Attention: les points arrivent triés 
//	le premier point est l'origine
//	le second est à la coordonnée (2;0)
//  le troisieme à la coordonnée (0;2)
//  si un de ces 3 points manque, le résultat est quaduc
bool MySortFunc3(H3_ARRAY_PT2DFLT32 &Pts,H3_ARRAY2D_FLT32 &ComputedPos,long maxx, long maxy)
{
	CString strFunction=("MySortFunc3"),msg;

	long sz=Pts.GetSize();
	H3_POINT2D_FLT32* pPt=Pts.GetData();
	//les points arrivent triés:
	//point0: Origine
	//point1: direction X
	//point2: direction Y
	H3_ARRAY_PT2DFLT32 U(sz),V(sz);//en fait: U et V sont des matrices de vecteurs. U(i) est le vecteur U au niveau du point i. idem V
	U[0]=(pPt[1]-pPt[0]);//on suppose que le voisinage de ces points est homogene
	U[0].x=U[0].x/2;
	U[0].y=U[0].y/2;
	U[1]=U[2]=U[0];
	V[0]=(pPt[2]-pPt[0]);
	V[0].x=V[0].x/2;
	V[0].y=V[0].y/2;
	V[1]=V[2]=V[0];

	H3_ARRAY_UINT8 unsortedPoints(sz);
	//0: j'ai deja cherché les voisin
	//1: non traité
	//2: je connais sa position et je dois chercher les voisins
	unsortedPoints.Fill(1);
	unsortedPoints[0]=unsortedPoints[1]=unsortedPoints[2]=2;
	H3_ARRAY2D_FLT32 PointsPlace(sz,2);
	PointsPlace.Fill(NaN);
	PointsPlace(0,0)=0;PointsPlace(0,1)=0;
	PointsPlace(1,0)=2;PointsPlace(1,1)=0;
	PointsPlace(2,0)=0;PointsPlace(2,1)=2;
	
	bool continu=true,cont2;
	float  temp1,temp2,xu,yu,xv,yv,xi,yi;
	long nbsortedpoints=0,i,j;
	float d0=(float)0.1,d1=(float)0.1,d2=(float)0.5,d3=1-d0,d4=1+d0;
	H3_MATRIX_FLT32 X(2,1),Y(2,1),M(2,2),Mtemp;

	while (continu){
		continu=false;
		for(long ii=0;ii<sz;ii++){//on cherche le premier point de valeur 2
			if(unsortedPoints[ii]==2){
				i=ii;
				continu=true;
				unsortedPoints[i]=0;
				nbsortedpoints++;
				ii=sz;
			}
		}

		xi=pPt[i].x;
		yi=pPt[i].y;
		xu=U[i].x;
		yu=U[i].y;
		xv=V[i].x;
		yv=V[i].y;
		M(0,0)=xu;M(1,0)=yu;
		M(0,1)=xv;M(1,1)=yv;

		for(j=0;j<sz;j++){//on trie les autres points par rapport à la reference
			if(unsortedPoints[j]==1){
				Y(0,0)=(pPt[j].x-xi);
				Y(1,0)=(pPt[j].y-yi);
				Mtemp=M;
				X=Mat_MeanSquare(Mtemp,Y);
				if(X.GetSize()!=2) {
					msg.Format("M00=%f M01=%f M10=%f M11=%f Y00=%f Y10=%f",
						M(0,0),M(0,1),M(1,0),M(1,1),Y(0,0),Y(1,0));

					temp1=(float)0.001;
					temp2=(float)0.001;

					cont2=false;
				}
				else{
					temp1=X(0,0);
					temp2=X(1,0);

					cont2=true;
				}

				if(cont2){
					if((temp1>d3)&&(temp1<d4)&&(fabs(temp2)<d1)){
						unsortedPoints[j]=2;
						U[j]=(pPt[j]-pPt[i]);
						V[j]=V[i];
						PointsPlace(j,0)=PointsPlace(i,0)+1;
						PointsPlace(j,1)=PointsPlace(i,1);
					}
					if((temp1<-d3)&&(temp1>-d4)&&(fabs(temp2)<d1)){
						unsortedPoints[j]=2;
						U[j]=(pPt[i]-pPt[j]);
						V[j]=V[i];
						PointsPlace(j,0)=PointsPlace(i,0)-1;
						PointsPlace(j,1)=PointsPlace(i,1);

					}
					if((temp2>d3)&&(temp2<d4)&&(fabs(temp1)<d1)){
						unsortedPoints[j]=2;
						U[j]=U[i];
						V[j]=(pPt[j]-pPt[i]);
						PointsPlace(j,0)=PointsPlace(i,0);
						PointsPlace(j,1)=PointsPlace(i,1)+1;

					}
					if((temp2<-d3)&&(temp2>-d4)&&(fabs(temp1)<d1)){
						unsortedPoints[j]=2;
						U[j]=U[i];
						V[j]=(pPt[i]-pPt[j]);
						PointsPlace(j,0)=PointsPlace(i,0);
						PointsPlace(j,1)=PointsPlace(i,1)-1;
						
					}
				}

			}
		}
	}
	ComputedPos=H3_ARRAY2D_FLT32(nbsortedpoints,4);
	for(i=0,j=0;i<sz;i++){
			if(unsortedPoints[i]==0){
				ComputedPos(j,0)=pPt[i].x;
				ComputedPos(j,1)=pPt[i].y;
				ComputedPos(j,2)=PointsPlace(i,0);
				ComputedPos(j,3)=PointsPlace(i,1);
				j++;
			}
	}

	return true;
}

//estimation de points manquant dans un tableau regulier(m*n elts) à partir de leurs voisins
//si un point a 1voisin au dessus+1 en dessous +1 a droite+1à gauche: intersection haut-bas, gauche-droite
//sinon on fait l'intersection des droites definis par 2points à droite (gauche) et 2 en haut (bas)
//EstimatedPoints : tableau decrivant si le point (i,j) est mesuré ou non (1:le point doit etre estimé)
//RetArray: tableau contenant les mesures initiales et le resultat à l'issu de la fonction
static void EstimMissing(H3_ARRAY2D_UINT8 &EstimatedPoints,H3_ARRAY2D_PT2DFLT32 &RetArray)
{
	unsigned long i,j;
	unsigned long Nco=EstimatedPoints.GetCo(),Nli=EstimatedPoints.GetLi();
	//si i et j ne sont pas extremes et si le point manquant a
	//4 voisins (haut/bas / gauche/droite), on prend l'intersection des bissectrices
	for(i=1;i<Nli-1;i++){
		for(j=1;j<Nco-1;j++){
			if(EstimatedPoints(i,j)==1){
				if( EstimatedPoints(i-1,j)==0 && EstimatedPoints(i,j-1)==0 &&
					EstimatedPoints(i+1,j)==0 && EstimatedPoints(i,j+1)==0 )

					RetArray(i,j)=GetIntersection(RetArray(i+1,j),RetArray(i-1,j),
												  RetArray(i,j+1),RetArray(i,j-1));
			}			
		}
	}
	for(j=1;j<Nco-1;j++){
		if(EstimatedPoints(0,j)==1){
			if( EstimatedPoints(2,j)==0 && EstimatedPoints(0,j-1)==0 &&
				EstimatedPoints(1,j)==0 && EstimatedPoints(0,j+1)==0 )

				RetArray(0,j)=GetIntersection(RetArray(2,j),RetArray(1,j),
											  RetArray(0,j+1),RetArray(0,j-1));
		}			
	}
	for(j=1;j<Nco-1;j++){
		if(EstimatedPoints(Nli-1,j)==1){
			if( EstimatedPoints(Nli-1-2,j)==0 && EstimatedPoints(Nli-1,j-1)==0 &&
				EstimatedPoints(Nli-1-1,j)==0 && EstimatedPoints(Nli-1,j+1)==0 )

				RetArray(0,j)=GetIntersection(RetArray(Nli-1-2,j),RetArray(Nli-1-1,j),
											  RetArray(Nli-1,j+1),RetArray(Nli-1,j-1));
		}			
	}
	for(i=1;i<Nli-1;i++){
		if(EstimatedPoints(i,0)==1){
			if( EstimatedPoints(i-1,0)==0 && EstimatedPoints(i,2)==0 &&
				EstimatedPoints(i+1,0)==0 && EstimatedPoints(i,1)==0 )

				RetArray(i,0)=GetIntersection(RetArray(i+1,0),RetArray(i-1,0),
											  RetArray(i,1),RetArray(i,2));
		}			
	}
	for(i=1;i<Nli-1;i++){
		if(EstimatedPoints(i,Nco-1)==1){
			if( EstimatedPoints(i-1,Nco-1)==0 && EstimatedPoints(i,Nco-1-2)==0 &&
				EstimatedPoints(i+1,Nco-1)==0 && EstimatedPoints(i,Nco-1-1)==0 )

				RetArray(i,Nco-1)=GetIntersection(RetArray(i+1,Nco-1),RetArray(i-1,Nco-1),
											  RetArray(i,Nco-1-1),RetArray(i,Nco-1-2));
		}
	}

			
	//reste les coins
	if(EstimatedPoints(0,0)){
		if( EstimatedPoints(1,0)==0 && EstimatedPoints(0,1)==0 &&
			EstimatedPoints(1,1)==0 )

			RetArray(0,0)=RetArray(1,0)+RetArray(0,1)-RetArray(1,1);
	}

	if(EstimatedPoints(0,Nco-1)){
		if( EstimatedPoints(1,Nco-1)==0 && EstimatedPoints(0,Nco-1-1)==0 &&
			EstimatedPoints(1,Nco-1-1)==0 )

			RetArray(0,Nco-1)=RetArray(1,Nco-1)+RetArray(0,Nco-1-1)-RetArray(1,Nco-1-1);
	}
	
	if(EstimatedPoints(Nli-1,0)){
		if( EstimatedPoints(Nli-1-1,0)==0 && EstimatedPoints(Nli-1,1)==0 &&
			EstimatedPoints(Nli-1-1,1)==0 )

			RetArray(Nli-1,0)=RetArray(Nli-1-1,0)+RetArray(Nli-1,1)-RetArray(Nli-1-1,1);
	}
	
	if(EstimatedPoints(Nli-1,Nco-1)){
		if( EstimatedPoints(Nli-1-1,Nco-1)==0 && EstimatedPoints(Nli-1,Nco-1-1)==0 &&
			EstimatedPoints(Nli-1-1,Nco-1-1)==0 )

			RetArray(Nli-1,Nco-1)=RetArray(Nli-1-1,Nco-1)+RetArray(Nli-1,Nco-1-1)-RetArray(Nli-1-1,Nco-1-1);
	}
}
