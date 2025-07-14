#if!defined _LABEL__H
#define _LABEL__H

#include "h3matrix.h"

class InquiryOnBlobs
{
private:
	long lMinArea;
	long lMaxArea;
	float fCompactness;
public:
	InquiryOnBlobs();
	~InquiryOnBlobs();
	bool Set(const CString Data, float value);
	float Get(const CString Data)const;
};

//recherche dans une image les pixels dont la valeur de gris est !=0
//groupe ces pixels avec leurs voisins s'ils existent (connexion 4 voisins)
//renvoi une cartographie sur laquelle les groupes sont coloriés avec des couleurs de 1 à n
//renvoi le centre de chaque tache
bool bwLabel(H3_ARRAY2D_UINT32* pa2dLabel,const H3_ARRAY2D_UINT8& Im ,const int nbLink=4);

//recherche dans une image labellisée les caracteristiques des taches
//renvoi le centre de chaque tache
bool GetBlobs(H3_ARRAY_PT2DFLT32& CentresTaches,const H3_ARRAY2D_UINT32& Label,const InquiryOnBlobs* const pIOB=nullptr);

//GaussImage
//renvoie une matrice de taille ny*nx
//les données dans la matrices varie comme l'intensité d'une gaussienne
//nx, ny: taille de la fenetre de recherche
//wx2,wy2: zone à 0 autour du centre de la fenetre de recherche
//si wx2 et wy2 sont <0 , pas de mise à 0 
H3_ARRAY2D_FLT32 GaussImage(unsigned long ny,unsigned long nx,long wy2,long wx2);

bool Erode(H3_ARRAY2D_UINT8* pa2dErodedIm,const H3_ARRAY2D_UINT8& Im);
bool Dilate(H3_ARRAY2D_UINT8* pa2dErodedIm,const H3_ARRAY2D_UINT8& Im);

/***************Fct templates >> à mettre dans le .h ************************/
//recherche dans une image les pixels dont la valeur de gris est !=0
//groupe ces pixels avec leurs voisins s'ils existent (connexion 4 voisins)
//renvoi le centre de chaque tache
template<class TYPE>
bool bwLabel(const CH3Array2D<TYPE>& Im,H3_ARRAY_PT2DFLT32 & CentresTaches)
{
	CString msg;

	unsigned long nx=Im.GetCo(),ny=Im.GetLi();
	unsigned long i,j,k,l,nbPointsValid=0,inx,im1,jm1;
	TYPE *pI=Im.GetData();

	//recherche du nombre de points dont la valeur n'est pas 0 dans l'image
	for(i=0;i<ny*nx;i++)
		nbPointsValid += ( (*(pI++))!=0 );
	if(nbPointsValid==0) return false;

	//Coordonnées des points valides
	pI=Im.GetData();
	CH3Array<TYPE> X(nbPointsValid),Y(nbPointsValid);
	//KK sera une image dans laquelle les points invalide sont a 0 (sans importance)
	//et les points valides de 0 à nbPointsValid-1
	CH3Array2D<long> KK(ny,nx);KK.Fill(0);
	for(i=0,k=0;i<ny;i++){
		inx=i*nx;
		for(j=0;j<nx;j++){
			if( (*(pI++)) != 0){
				Y[k]=(TYPE)i;
				X[k]=(TYPE)j;
				KK(i,j)=k;
				k++;
			}
		}
	}

	//Connexion des points valides (connexions à 4 voisins. Vu le sens de parcours seuls deux sont testés)
	pI=Im.GetData();
	CH3Array<long> Color(nbPointsValid);//couleur affectée au kieme pixel valide, fonction de son voisinage
	Color.Fill(0);
	CH3Array<long> ColorTrue(nbPointsValid);
	H3_ARRAY2D_UINT8 ConnectedColor(nbPointsValid+1,nbPointsValid+1);
	ConnectedColor.Fill(0);
	unsigned long LastColor=0,Color1,Color2,Color0;
	bool left,top;
	for(k=0;k<nbPointsValid;k++){
		i=(unsigned long)Y[k];
		j=(unsigned long)X[k];
		im1=i-1;
		jm1=j-1;
		if(j>0)		
			left=(Im(i,jm1)!=0);
		else
			left=false;

		if(i>0)		
			top =(Im(im1,j)!=0);
		else
			top=false;

		if(left || top){
			if(left && top){
				Color[k]=Color[KK(im1,j)];//vu le sens de parcours,
						//la couleur 'top' est numeriquement +petite que 'left'
				ConnectedColor( Color[k],Color[k-1] ) = 1;
			}
			else{
				if(left)
					Color[k]=Color[k-1];
				else//top
					Color[k]=Color[KK(im1,j)];
			}

		}
		else{
			LastColor++;
			Color[k]=LastColor;
		}
	}

	//opération sur les couleurs connectées
	bool Change = true;
	k=0;
#if 1
	bool b1,b2;
	unsigned __int8 connect_1_2,connect_2_1;
#endif
	while(Change){
		Change = false;
		k++;
		
#if 0
		for(Color2=1;Color2<=LastColor;Color2++){
			for(Color1=Color2+1;Color1<=LastColor;Color1++){
				if(ConnectedColor(Color1,Color2) || ConnectedColor(Color2,Color1) ){
					ConnectedColor(Color1,Color2)=ConnectedColor(Color2,Color1)=0;
#else
		for(Color2=1;Color2<=LastColor;Color2++)
		{
			for(Color1=Color2+1;Color1<=LastColor;Color1++)
			{
				long index1_2 = Color1*(nbPointsValid+1)+ Color2;
				long index2_1 = Color2*(nbPointsValid+1)+ Color1;
				connect_1_2=ConnectedColor[index1_2];
				connect_2_1=ConnectedColor[index2_1];;
				b1=(connect_1_2==1);
				b2=(connect_2_1==1);
				if( b1 || b2){
					ConnectedColor[index1_2]=ConnectedColor[index2_1]=0;

#endif
					for(l=0;l<nbPointsValid;l++){
						if(Color[l]==Color1) Color[l]=Color2 ;
					}
					Change=true;
				
					for(Color0=Color1+1;Color0<=LastColor;Color0++){
						if(ConnectedColor(Color1,Color0) || ConnectedColor(Color0,Color1) ){
							ConnectedColor(Color1,Color0) = ConnectedColor(Color0,Color1)=0;
							ConnectedColor(Color2,Color0) = ConnectedColor(Color0,Color2)=1;
						}
					}
				}
			}
		}
	}

	//Histogramme des couleurs utilisée
	CH3Array<long> HistoColor(LastColor+1)/*,NumColor(LastColor+1)*/;//il y a LastColor couleur + le 0
	unsigned long nbColors=0;
	HistoColor.Fill(0);
	for(i=0;i<nbPointsValid;i++)
		HistoColor[Color[i]] ++;
	for(i=1,k=0;i<=LastColor;i++){
		if(HistoColor[i] >0) (++k);
	}
	nbColors=k;

	//Re-indexation des couleurs de 1 a k
	ColorTrue.Fill(-1);
	for(Color1=1,Color2=1;Color1<=LastColor;Color1++){
		Change=false;
		for(k=0;k<nbPointsValid;k++){
			if(Color[k]==Color1){
				Change=true;
				ColorTrue[k]=Color2;
			}
		}
		if(Change) Color2++;
	}

	//Recherche des centres des taches
	CentresTaches.ReAlloc(nbColors);
	for(i=0;i<nbColors;i++){
		CentresTaches[i].x=CentresTaches[i].y=0;
	}
	CH3Array<float> C(nbColors); C.Fill(0);
	for(i=0;i<nbPointsValid;i++){
		j=ColorTrue[i];
		if(j>0){
			k=j-1;
			CentresTaches[k].x += X[i];
			CentresTaches[k].y += Y[i];
			C[k] ++;
		}
	}
	for(i=0;i<nbColors;i++){
		CentresTaches[i].x /=C[i];
		CentresTaches[i].y /=C[i];
	}
	return true;	
}

//moyenne une image avec un filtre (N,1) ou (1,N)
template<class TYPE>
H3_ARRAY2D_FLT32 filtre(const H3_ARRAY2D & Image, unsigned long N, char sens)
{
	unsigned long nx=Image.GetCo(),ny=Image.GetLi(),i,j,nbElts=0,nbElts_2;
	H3_ARRAY2D_FLT32 filteredImage(ny,nx);
	float *p_filt0=filteredImage.GetData(),*p_filt=p_filt0,*p_filt2;
	TYPE *pI0=Image.GetData(),*pI=pI0,*pI2;
	unsigned long Ns2p1=N/2+1;

	switch(sens){
	case 'x':
	case 'X':
		if(nx>=N){
		//on commence par filtrer ligne à ligne avec N elements
		//le filtre fait la moyenne de N elements consécutifs quand cela est possible
		//pour les case de 0 à N-1 et nx-N+1 à nx-1, la moyenne est faite sur moins d'éléments
			for(i=0;i<ny;i++){
				pI=pI0+i*nx;
				p_filt=p_filt0+i*nx;
				//le remplissage des N premiers éléments est speciale
				//la premiere case est la somme des (N/2)+1 premiers éléments
				(*p_filt) = float(*(pI++));
				for(j=1;j<Ns2p1;j++) (*p_filt) += float(*(pI++));

				nbElts=Ns2p1;
				(*p_filt)/=nbElts;
				p_filt++;
				//jusqu'à la Nieme case on rajoute des elements
				nbElts_2=nbElts+1;
				p_filt2=p_filt-1;
				for(j=1;j<Ns2p1;j++){
					(*p_filt) = (*(p_filt2))*nbElts + (*(pI++));	
					(*p_filt)/=nbElts_2;
					nbElts++;
					nbElts_2++;
					p_filt++;
					p_filt2++;
				}
				//puis on peut ajouter et enlever jusqu'à N cases de la fin
				pI2=pI-N;
				for(j=Ns2p1;j<nx-Ns2p1+1;j++){
					(*p_filt) = (*(p_filt2))+ (float(*pI)-float(*(pI2)))/N;
					p_filt++;
					p_filt2++;
					pI++;
					pI2++;
				}
				//fin de ligne
				nbElts=N;
				nbElts_2=nbElts-1;
				for(j=nx-Ns2p1+1;j<nx;j++){
					(*p_filt) = (*(p_filt2))*nbElts -float(*(pI2));					
					(*p_filt) /= nbElts_2;
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
			for(j=0;j<nx;j++) (*(p_filt++)) = float(*(pI++));
			for(i=1;i<Ns2p1;i++){
				p_filt -= nx;
				for(j=0;j<nx;j++) (*(p_filt++)) += float(*(pI++));
			}
			//il faut normer par (N+1)/2
			p_filt -= nx;
			nbElts=Ns2p1;
			for(j=0;j<nx;j++) (*(p_filt++)) /= nbElts;
			//là, la premiere ligne est initialisée.

			//jusqu'à la Ns2p1 ieme ligne on rajoute des elements
			nbElts_2=nbElts+1;
			p_filt2=p_filt - nx;//1 ligne av;
			for(i=1;i<Ns2p1;i++){
				for(j=0;j<nx;j++){
					(*p_filt) = (*(p_filt2))*nbElts + (*(pI));
					(*p_filt)/=(nbElts_2);
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
					(*p_filt) = (*(p_filt2))+ (float(*pI)-float(*(pI2)))/N;
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
					(*p_filt) = (*(p_filt2))*nbElts - float(*(pI2));
					(*p_filt)/=(nbElts_2);
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
		#if defined(H3APPMESSAGETOOLSDECL_H__INCLUDED_)
//		H3DebugError(strModule,"Filtre","error");
		#endif
		AfxThrowUserException();
	}
	return filteredImage;
}


/*! 
* 	\fn      template<class TYPE> static H3_ARRAY2D_FLT32 filtre(const H3_ARRAY2D & Image, unsigned long N, char sens)* 	\brief   
* 	\param   const H3_ARRAY2D & Image : Image à filtrer
* 	\param   unsigned long N :taille de la matrice de filtrage 
* 	\param   char sens : 'x' ou 'y'
* 	\return  H3_ARRAY2D_FLT32 image filtrée
*   \brief   moyenne une image avec un filtre (N,1)(cas 'x') ou (1,N)(cas 'y')
* 	\remarks propagation des NaN dans l'image.
*/ 
template<class TYPE> //static
H3_ARRAY2D_FLT32 filtre1(const H3_ARRAY2D & Image, unsigned long N0, char sens,
						H3_ARRAY2D_FLT32* pFilteredImage=nullptr)
{
	CString strFunction("filtre");

	const unsigned long N=(N0/2L)*2L+1L;
	unsigned long nx=Image.GetCo(),ny=Image.GetLi(),i,j,nbElts=0,nbElts_2;
	H3_ARRAY2D_FLT32 FilteredImageTmp;//En cas de pFilteredImage==nullptr

	if(pFilteredImage==nullptr){
		FilteredImageTmp.ReAlloc(ny,nx);
		pFilteredImage=&FilteredImageTmp;
	}
	else if(!((*pFilteredImage).GetLi()==ny && (*pFilteredImage).GetCo()==nx))
		(*pFilteredImage).ReAlloc(ny,nx);

	float *p_filt0=(*pFilteredImage).GetData(),*p_filt=p_filt0,*p_filt2;
	TYPE *pI0=Image.GetData(),*pI=pI0,*pI2;
	unsigned long Ns2p1=N/2+1;

	if(N==1) return (H3_ARRAY2D_FLT32)Image;

	switch(sens){
	case 'x':
	case 'X':
		if(nx>=N){
		//on commence par filtrer ligne à ligne avec N elements
		//le filtre fait la moyenne de N elements consécutifs quand cela est possible
		//pour les case de 0 à N-1 et nx-N+1 à nx-1, la moyenne est faite sur moins d'éléments
			for(i=0L;i<ny;i++){
				pI=pI0;
				p_filt=p_filt0;

				//le remplissage des N premiers éléments est speciale
				//la premiere case est la somme des (N/2)+1 premiers éléments
				(*p_filt) = float(*(pI++));
				for(j=1L;j<Ns2p1;j++) (*p_filt) += float(*(pI++));

				nbElts=Ns2p1;
				(*p_filt)/=nbElts;
				p_filt++;
				//jusqu'à la Nieme case on rajoute des elements
				nbElts_2=nbElts+1L;
				p_filt2=p_filt-1;
				for(j=1L;j<Ns2p1;j++){
					(*p_filt) = (*(p_filt2))*nbElts + (*(pI++));	
					(*p_filt)/=nbElts_2;
					nbElts++;
					nbElts_2++;
					p_filt++;
					p_filt2++;
				}
				//puis on peut ajouter et enlever jusqu'à N cases de la fin
				pI2=pI-N;
				for(j=Ns2p1;j<nx-Ns2p1+1L;j++){
					(*p_filt) = (*(p_filt2))+ (float(*pI)-float(*(pI2)))/N;
					p_filt++;
					p_filt2++;
					pI++;
					pI2++;
				}
				//fin de ligne
				nbElts=N;
				nbElts_2=nbElts-1L;
				for(j=nx-Ns2p1+1;j<nx;j++){
					(*p_filt) = (*(p_filt2))*nbElts -float(*(pI2));					
					(*p_filt) /= nbElts_2;
					nbElts--;
					nbElts_2--;
					p_filt++;
					p_filt2++;
					pI2++;
				}
				pI0+=nx;
				p_filt0+=nx;
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
			for(j=0L;j<nx;j++) (*(p_filt++)) = float(*(pI++));
			for(i=1L;i<Ns2p1;i++){
				p_filt -= nx;
				for(j=0L;j<nx;j++) (*(p_filt++)) += float(*(pI++));
			}
			//il faut normer par (N+1)/2
			p_filt -= nx;
			nbElts=Ns2p1;
			for(j=0L;j<nx;j++) (*(p_filt++)) /= nbElts;
			//là, la premiere ligne est initialisée.

			//jusqu'à la Ns2p1 ieme ligne on rajoute des elements
			nbElts_2=nbElts+1L;
			p_filt2=p_filt - nx;//1 ligne av;
			for(i=1L;i<Ns2p1;i++){
				for(j=0L;j<nx;j++){
					(*p_filt) = (*(p_filt2))*nbElts + (*(pI));
					(*p_filt)/=(nbElts_2);
					p_filt++;
					p_filt2++;
					pI++;
				}
				nbElts++;
				nbElts_2++;
			}
			//puis on peut ajouter et enlever jusqu'à N cases de la fin
			pI2=pI - N*nx;//N lignes av
			for(i=Ns2p1;i<ny-Ns2p1+1L;i++){
				for(j=0L;j<nx;j++){
					(*p_filt) = (*(p_filt2))+ (float(*pI)-float(*(pI2)))/N;
					p_filt++;
					p_filt2++;
					pI++;
					pI2++;
				}
			}
			//on se rapproche de la fin
			nbElts=N;
			nbElts_2=nbElts-1L;
			for(i=ny-Ns2p1+1;i<ny;i++){
				for(j=0L;j<nx;j++){
					(*p_filt) = (*(p_filt2))*nbElts - float(*(pI2));
					(*p_filt)/=(nbElts_2);
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
		#if defined(H3APPMESSAGETOOLSDECL_H__INCLUDED_)
//		H3DebugError(strModule,"Filtre","error");
		#endif
		AfxThrowUserException();
	}

	return (*pFilteredImage);
}

#endif