#include "stdafx.h"
#include "H3Matrix.h"
#include "H3MatrixOperations.h"
#include <math.h>
#include <stdio.h>
#include <vector>

/*static float H3GetFPNaN()
{
	//1 bit de signe à 0 ou 1,
	//8 bits d'exposant à 1,
	//23 bits de mantisse, mantisse non nulle(mantisse nulle pour infini)
	//si la mantisse commence par 1: Quiet NaN
	//si la mantisse commence par 0: Signal NaN (erreur)
	float Value;
	long *pf=(long *)&Value;
	*pf=0x7FC00000;

	return Value;
}

static double H3GetFPdNaN()
{
	//1 bit de signe à 0 ou 1,
	//11 bits d'exposant à 1,(7FF)
	//52 bits de mantisse, mantisse non nulle(mantisse nulle pour infini)
	//si la mantisse commence par 1: Quiet NaN
	//si la mantisse commence par 0: Signal NaN (erreur)
	//ATTENTION: NaN peut etre > ou < à un double avec Visual C++ au 19/04/07
	double Value;
	//solution 1
	//hyper *pf=(hyper*)&Value;
	//*pf=0x7FFFFFFFFFFFFFFF;
	//*pf=0x7FF8000000000000;
	//*pf=0xffffffff7fffffff;

	//solution 2
	//Value=numeric_limits<double>::quiet_NaN();//(Value>PI) est true

	//solution 3
	unsigned long nan[2]={0xffffffff, 0x7fffffff};
	Value = *( double* )nan;
	return Value;
}

const float  NaN = H3GetFPNaN();*/

//-------------------------------------------------------------------------------------
//fonction OK 28/03/2011
H3_MATRIX_FLT32 CH3MatrixOperations::Alta_CreateCoefMap3(int radius)
{
	int r2 = (int)floor((float)radius/2.0f);
	H3_MATRIX_FLT32 resultMatrix(radius,radius);
	H3_MATRIX_FLT32 coeff1_Mat(radius,radius);
	H3_MATRIX_FLT32 coeff2_Mat(radius,radius);

	// Gaussian 2D
/*	double A = 0.75;
	double dSigX = 40.0;
	double dSigY = 40.0;
	double dX0 = r2;
	double dY0 = r2;

	for(int j=0;j<radius;j++)
	{
		for(int i=0;i<radius;i++)
		{
			resultMatrix(i,j) = A * exp( - ( (i-dX0)*(i-dX0)/(2.0*dSigX*dSigX) + (j-dY0)*(j-dY0)/(2.0*dSigY*dSigY) ) );
		}
	}
	return(resultMatrix);  */

	// Pyramide
	for(int j=0;j<radius;j++)
	{
		for(int i=0;i<r2;i++)
		{
			coeff1_Mat(i,j) = (float)(((float)1/(r2-1))*i);
		}

		for(int i=r2;i<radius;i++)
		{
			coeff1_Mat(i,j) = (float)(((float)1/(r2-1))*(radius-(i+1)));
		}
	}

	for(int j=0;j<radius;j++)
	{
		for(int i=0;i<r2;i++)
		{
			coeff2_Mat(j,i) = (float)(((float)1/(r2-1))*i);
		}

		for(int i=r2;i<radius;i++)
		{
			coeff2_Mat(j,i) = (float)(((float)1/(r2-1))*(radius-(i+1)));
		}
	}
	for(int i=0;i<radius;i++)
	{
		for(int j=0;j<radius;j++)
		{
			resultMatrix(i,j)=0.5f*coeff1_Mat(i,j)*coeff2_Mat(i,j);
		}
	}
	return(resultMatrix);
}

//-------------------------------------------------------------------------------------
H3_MATRIX_UINT8 CH3MatrixOperations::Alta_strel(Strel_Type type, int radius)
{
	// Dans un premier temps, determiner le type
	switch(type)
	{
	case disk:
		// Cree la matrice de forme "disque"
		return Create_DiskStrelMatrix(radius);
		break;
	}
	H3_MATRIX_UINT8 nullmatrix;
	return nullmatrix;
}
//-------------------------------------------------------------------------------------
//Fonction validée 12/04/2011
H3_MATRIX_UINT8 CH3MatrixOperations::Create_DiskStrelMatrix(int radius)
{
	H3_MATRIX_UINT8 resultMatrix(radius*2,radius*2);

	for(int i=0;i<2*radius;i++)
	{
		for(int j=0;j<2*radius;j++)
		{
			float radius_value = sqrt(pow(float(i-radius),2)+pow(float(j-radius),2));
			if(radius_value<radius)
			{
				// le point est dans le rayon, tout va bien
				resultMatrix(i,j)=1;
			}
			else
			{
				// on est en dehors du cercle !
				resultMatrix(i,j)=0;
			}
		}
	}
	// matrice terminee
	return resultMatrix;

}
//-------------------------------------------------------------------------------------
//Fonction validée 12/04/2011
H3_MATRIX_UINT8 CH3MatrixOperations::Alta_imDilate(H3_MATRIX_UINT8 SourceMatrix,H3_MATRIX_UINT8 &StrelMatrix)
{
	
	int x_max = SourceMatrix.GetCo();
	int y_max = SourceMatrix.GetLi();
	// Matrice de resultats
	H3_MATRIX_UINT8 FinalMatrix(SourceMatrix);
	FinalMatrix.Fill(0);

	//les lignes H3Matrix sont les colonnes matlab... logique !

	for(int iCurrentLi = 0;iCurrentLi<y_max;iCurrentLi++)
	{
		for(int iCurrentCo = 0;iCurrentCo<x_max;iCurrentCo++)
		{
			if(SourceMatrix(iCurrentLi,iCurrentCo)!=0)
			{
				if(iCurrentLi>247 && iCurrentCo<900)
				{
					int fnldnvd = 0;
				}
				ReplaceDilateMatrix(FinalMatrix,StrelMatrix,iCurrentCo,iCurrentLi);
				iCurrentCo+=StrelMatrix.GetCo()/2;
			}
			else
			{
				FinalMatrix(iCurrentLi,iCurrentCo)=max(0,FinalMatrix(iCurrentLi,iCurrentCo));
			}

		}
	}

	return(FinalMatrix);
}
//-------------------------------------------------------------------------------------
//Fonction validée 12/04/2011
void CH3MatrixOperations::ReplaceDilateMatrix(H3_MATRIX_UINT8 &sourceMatrix, H3_MATRIX_UINT8 &strelMatrix, long xPos,long yPos)
{
	long yPosOffset = strelMatrix.GetLi()/2;
	long xPosOffset = strelMatrix.GetCo()/2;

	for(long yIndex= -yPosOffset;yIndex<yPosOffset;yIndex++)	// ligne !
	{
		for(long xIndex= -xPosOffset;xIndex<xPosOffset;xIndex++) // colonne
		{
			if((xPos+xIndex)>=0 &&(yPos+yIndex)>=0 &&(yPos+yIndex)<(long)sourceMatrix.GetLi() && (xPos+xIndex)<(long)sourceMatrix.GetCo())
			{
				// Nous sommes dans des coordonnees valides
				// La valeur de point est la plus grande du masque ou du voisin concerné
				int temp2 = strelMatrix(yIndex + yPosOffset,xIndex + xPosOffset);
				H3_UINT8 temp = max(sourceMatrix(yPos+yIndex,xPos+xIndex),temp2);
 				sourceMatrix(yPos+yIndex,xPos+xIndex) = temp;
			}
		}
	}	
}
//-------------------------------------------------------------------------------------

void CH3MatrixOperations::meshgrid(int x_start,int x_end,int y_start,int y_end, H3_MATRIX_FLT32 &xMatrix,H3_MATRIX_FLT32 &yMatrix)
{
	// Ajout de 1 pour avoir le nb d'elts effectif
	int nbLi = y_end - y_start+1;
	int nbCo = x_end - x_start+1;
	xMatrix.Alloc(nbLi,nbCo);
	yMatrix.Alloc(nbLi,nbCo);
	//création des deux matrices du meshgrid : x et y
	// leurs valeurs sont données par les indices des boucles for ET les valeurs de départ passées en paramètre
	for(int xIndex = 0;xIndex<nbLi;xIndex++)
	{
		for(int yIndex = 0;yIndex<nbCo;yIndex++)
		{
			xMatrix(yIndex,xIndex)=(float)(y_start+yIndex);
			yMatrix(yIndex,xIndex)=(float)(x_start+xIndex);
		}
	}
	FILE* pFile;
	errno_t err;

	ASSERT(00); // strin path en dur à gerer

	err = fopen_s(&pFile, "C:\\test_mesh_x.bin","wb");
	xMatrix.fSaveRAW(pFile);
	fclose(pFile);

	err = fopen_s(&pFile, "C:\\test_mesh_y.bin","wb");
	yMatrix.fSaveRAW(pFile);
	fclose(pFile);

}
H3_MATRIX_FLT32 CH3MatrixOperations::fspecial(fspecial_Type type,double p2,double p3)
{
	switch (type)
	{
	case gaussian:
		return fspecial_gaussian(p2,p3);
		break;
	}

	H3_MATRIX_FLT32 nullmatrix;
	return nullmatrix;
}
H3_MATRIX_FLT32 CH3MatrixOperations::fspecial_gaussian(double p2, double p3)
{
	//p2 détermine la dimension de la matrice gaussienne
	H3_MATRIX_FLT32 gaussianMatrix((long)p2,(long)p2);
	H3_MATRIX_FLT32 xMatrix;
	H3_MATRIX_FLT32 yMatrix;
	double siz = (p2-1)/2;
	// création des matrices X et Y (meshgrid matlab)
	meshgrid((int)-siz,(int)siz,(int)-siz,(int)siz,xMatrix,yMatrix);
	float sumh=0;
	// calcul de chaque point selon la formule dans le fichier Matlab fspecial.m
	for(int xIndex=0;xIndex<p2;xIndex++)
	{
		for(int yIndex=0;yIndex<p2;yIndex++)
		{
			float tempValue= exp(-(xMatrix(yIndex,xIndex)*xMatrix(yIndex,xIndex)+yMatrix(yIndex,xIndex)*yMatrix(yIndex,xIndex))/(float)(2*p3*p3));
			sumh+=tempValue;
			gaussianMatrix(yIndex,xIndex)=tempValue;
		}
	}
	if(sumh!=0)
	{		 
		gaussianMatrix = gaussianMatrix/sumh; // Attention, division par un scalaire uniquement (13/04/2011), sert à normaliser la matrice
	}
	return gaussianMatrix;
}
//14/04/2011 viabilité à confirmer, calcul juste sur le papier mais différent de matlab !
H3_MATRIX_FLT32 CH3MatrixOperations::conv(H3_MATRIX_FLT32 &AMatrix,H3_MATRIX_FLT32 &BMatrix,Conv_Type type)
{
	switch (type)
	{
	case full:
		return conv_full(AMatrix,BMatrix);
		break;
	//case same:
	//	return
	}
	H3_MATRIX_FLT32 nullmatrix;
	return nullmatrix;
}
//14/04/2011 viabilité à confirmer, calcul juste sur le papier mais différent de matlab !
H3_MATRIX_FLT32 CH3MatrixOperations::conv_full(H3_MATRIX_FLT32 &AMatrix,H3_MATRIX_FLT32 &BMatrix)
{
//calcul des dimensions de la matrice finale
	int finalNbLi = AMatrix.GetLi()+BMatrix.GetLi()-1;
	int finalNbCo= AMatrix.GetCo()+BMatrix.GetCo()-1;

	H3_MATRIX_FLT32 resultMatrix(finalNbLi,finalNbCo);
	// la matrice B est fixe, la matrice A se déplace 
	TRACE("matrice : \r\n");
	for(int yIndex=0;yIndex<finalNbLi;yIndex++)
	{
		for(int xIndex=0;xIndex<finalNbCo;xIndex++)
		{
			// application de la convolution locale
			resultMatrix(yIndex,xIndex)=LocalConv(AMatrix,BMatrix,yIndex,xIndex);
			//TRACE("%f ",resultMatrix(yIndex,xIndex));
		}
		//TRACE("\r\n");
	}
	return resultMatrix;
}

//14/04/2011 viabilité à confirmer, calcul juste sur le papier mais différent de matlab !
H3_FLT32 CH3MatrixOperations::LocalConv(H3_MATRIX_FLT32 &AMatrix,H3_MATRIX_FLT32 &BMatrix,int LiPos,int CoPos)
{
	H3_FLT32 sum = 0;

	//Comme pour la précédente fonction, la matrice B est fixe, la matrice A est mobile
	for(unsigned int LiIndex= 0; LiIndex<AMatrix.GetLi();LiIndex++)
	{
		for(unsigned int  CoIndex= 0; CoIndex<AMatrix.GetCo();CoIndex++)
		{
			int CurrentLiIndex=LiPos-LiIndex;
			int CurrentCoIndex=CoPos-CoIndex;
			if(CurrentLiIndex>=0 && CurrentCoIndex>=0 && CurrentLiIndex<(int)BMatrix.GetLi() && CurrentCoIndex<(int)BMatrix.GetCo())
			{ // On est dans un point de coordonnées valides !
				sum+=AMatrix(AMatrix.GetLi()-1-LiIndex,AMatrix.GetCo()-1-CoIndex)*BMatrix(CurrentLiIndex,CurrentCoIndex);	
			}
		}
	}
	return sum;
}
H3_MATRIX_FLT32 CH3MatrixOperations::padarray(H3_MATRIX_FLT32 &AMatrix,H3_FLT32 value,int before,int after ,Pad_Type type)
{
	switch (type)
	{
	case both:
		return padarray_Both(AMatrix,value,before,after);
		break;
	}
	H3_MATRIX_FLT32 nullmatrix;
	return nullmatrix;
}

H3_MATRIX_FLT32 CH3MatrixOperations::padarray_Both(H3_MATRIX_FLT32 &AMatrix,H3_FLT32 value,int before,int after)
{
	// calcul des nouvelles dimensions :
	int finalNbLi = AMatrix.GetLi()+before+after;
	int finalNbCo = AMatrix.GetCo()+before+after;

	H3_MATRIX_FLT32 finalMatrix(finalNbLi,finalNbCo);

	TRACE("Matrice padarray : \r\n");
	for(int yIndex = 0;yIndex<finalNbLi;yIndex++)
	{
		for(int xIndex = 0;xIndex<finalNbCo;xIndex++)
		{
			int originLi= yIndex -before;
			int originCo = xIndex - before;

			if(originLi>=0 && originCo>=0 && originLi<(int)AMatrix.GetLi() && originCo<(int)AMatrix.GetCo())
			{
				finalMatrix(yIndex,xIndex)=AMatrix(originLi,originCo);
			}
			else
			{
				finalMatrix(yIndex,xIndex)= value;
			}
			//TRACE("%f ",finalMatrix(yIndex,xIndex));
		}
		//TRACE("\r\n");
	}
	return(finalMatrix);
}

// Validée le 14/04/2011
H3_MATRIX_FLT32 CH3MatrixOperations::sparse(H3_MATRIX_FLT32 iVector,H3_MATRIX_FLT32 jVector,H3_MATRIX_FLT32 valueVector,int iMax,int jMax,int sMax)
{
	int i_max,j_max,s_max;
	H3_MATRIX_FLT32 dummy;
	// une valeur a été spécifiée
	if(iMax >=0)
	{
		//mais elle n'est pas cohérente avec le vecteur
		if(maxValue(iVector)>iMax)
		{
			return(dummy);
		}
		else
		{
		i_max = iMax;
		}
	}
	else
	{
		i_max=(int)maxValue(iVector);
	}
	// une valeur a été spécifiée
	if(jMax >=0)
	{
		//mais elle n'est pas cohérente avec le vecteur
		if(maxValue(jVector)>jMax)
		{
			return(dummy);
		}
		else
		{
			j_max=jMax;
		}
	}
	else
	{
		j_max = (int) maxValue(jVector);
		
	}
	// une valeur a été spécifiée
	if(sMax >=0)
	{
		//mais elle n'est pas cohérente avec le vecteur
		if(maxValue(valueVector)>sMax)
		{
			return(dummy);
		}
		else
		{
		s_max = sMax;
		}
	}
	else
	{
		s_max = (int) maxValue(valueVector);
	}	
	
	H3_MATRIX_FLT32 finalMatrix(i_max,j_max);
		finalMatrix.Fill(0);
	// il faut satisfaire : matriceSortie(iVector(k),jVector(k))=valueVector(k), pour cela les dimensions de i,j et s doivent être identiques...
	for(int yIndex=0;yIndex<iVector.GetSize();yIndex++)
	{
		finalMatrix((long)iVector(yIndex)-1,(long)jVector(yIndex)-1)=valueVector(yIndex); // NOTE DE RT à VALIDER ?????
	}
	//sparse créée !

	return(finalMatrix);
}

// Validée le 14/04/2011
H3_FLT32 CH3MatrixOperations::maxValue(H3_MATRIX_FLT32 &Matrix)
{
	//si la matrice n'est pas vide
	if(Matrix.GetSize()>0)
	{
		H3_FLT32 finalValue = Matrix(0);
		//parcours de tous les éléments pour trouver le plus grand
		for(int i=1;i<Matrix.GetSize();i++)
		{
			finalValue=max(Matrix(i),finalValue);
		}
		return(finalValue);
	}
	else
		return(NaN);
}

H3_FLT32 CH3MatrixOperations::mean(H3_MATRIX_FLT32 & Matrix)
{
	if(Matrix.GetSize()!=0)
	{
		H3_FLT32 meanValue=0;

		for(int i=0;i<Matrix.GetSize();i++)
		{
			meanValue+=Matrix(i);
		}
		return(meanValue/Matrix.GetSize());
	}
	return NaN;
}

H3_MATRIX_FLT32 CH3MatrixOperations::mat_sqrt(H3_MATRIX_FLT32 &Matrix)
{
	H3_MATRIX_FLT32 finalMatrix(Matrix.GetLi(),Matrix.GetCo());

	for(int i=0;i<finalMatrix.GetSize();i++)
	{
		finalMatrix(i)=sqrt(Matrix(i));
	}
	return(finalMatrix);
}

H3_MATRIX_FLT32 CH3MatrixOperations::scalar_mul(H3_MATRIX_FLT32 &AMatrix,H3_MATRIX_FLT32 &BMatrix)
{
	if(AMatrix.GetLi()==BMatrix.GetLi()&&AMatrix.GetCo()&&BMatrix.GetCo())
	{
		H3_MATRIX_FLT32 finalMatrix(AMatrix.GetLi(),BMatrix.GetLi());
		
		for(int i=0;i<finalMatrix.GetSize();i++)
		{
			finalMatrix(i)=AMatrix(i)*BMatrix(i);
		}
		return(finalMatrix);
	}
	else
	{
		throw new exception("Matrix dimensions must agree");
	}

}


H3_MATRIX_FLT32 CH3MatrixOperations::affineTransformation(H3_MATRIX_FLT32 gxMatrix,H3_MATRIX_FLT32 gyMatrix,H3_MATRIX_FLT32 &D11Matrix, H3_MATRIX_FLT32 &D12Matrix,H3_MATRIX_FLT32 &D22Matrix)
{
	float sigma = 0.5;
	float ss = floor(6*sigma);
	if(ss<3)
		ss = 3;
	H3_MATRIX_FLT32 ww = CH3MatrixOperations::fspecial(gaussian,ss,sigma);	// fspecial('gaussian',ss,sigma);
	
	H3_MATRIX_FLT32 T11 = CH3MatrixOperations::conv(ww,CH3MatrixOperations::scalar_mul(gxMatrix,gxMatrix),full);
	H3_MATRIX_FLT32 T22 = CH3MatrixOperations::conv(ww,CH3MatrixOperations::scalar_mul(gyMatrix,gyMatrix),full);
	H3_MATRIX_FLT32 T12 = CH3MatrixOperations::conv(ww,CH3MatrixOperations::scalar_mul(gxMatrix,gyMatrix),full);
	
	H3_MATRIX_FLT32 ImagPart = CH3MatrixOperations::mat_sqrt( CH3MatrixOperations::scalar_mul((T11-T22),(T11-T22))+ CH3MatrixOperations::scalar_mul(T12,T12)*4);
	
	H3_MATRIX_FLT32 EigD_1 = (T22 + T11 + ImagPart)/2.0;
	H3_MATRIX_FLT32 EigD_2 = (T22 + T11 - ImagPart)/2.0;
	
	float fThreshold_Small = 1* CH3MatrixOperations:: maxValue(EigD_1)/100;
	float alpha = 0.1f;
	H3_MATRIX_FLT32 L1(gxMatrix.GetLi(),gxMatrix.GetCo());
	H3_MATRIX_FLT32 L2(gxMatrix.GetLi(),gxMatrix.GetCo());
	L1.Fill(1);
	
	for(unsigned long iLi = 0;iLi<L1.GetLi();iLi++)
	{
		for(unsigned long iCo =0;iCo<L1.GetCo();iCo++)
		{
			if(EigD_1(iLi,iCo)>fThreshold_Small)
			{
				L1(iLi,iCo)= alpha + 1.0f - exp(-3.315f/pow(EigD_1(iLi,iCo),4)); // -3.315 == PI WTF ?????
			}
		}
	}

	//getting tensor weight (© Matlab)
	H3_MATRIX_FLT32 D11(gxMatrix.GetLi(),gxMatrix.GetCo());
	D11.Fill(0);
	H3_MATRIX_FLT32 D12(gxMatrix.GetLi(),gxMatrix.GetCo());
	D12.Fill(0);
	H3_MATRIX_FLT32 D22(gxMatrix.GetLi(),gxMatrix.GetCo());
	D22.Fill(0);

	for(unsigned int ii = 0;ii<gxMatrix.GetLi();ii++)
	{
		for(unsigned int jj=0;jj<gxMatrix.GetCo();jj++)
		{
			H3_MATRIX_FLT32 Wmat(2,2);
			Wmat(0,0)=T11(ii,jj);
			Wmat(0,1)=T12(ii,jj);
			Wmat(1,0)=T12(ii,jj);
			Wmat(1,1)=T22(ii,jj);
			H3_MATRIX_FLT32 v(2,2);
			H3_MATRIX_FLT32 d(2,2);

			eig_2x2(Wmat,d,v);
			if(d(0,0)>d(1,1))
			{
				//swapping

				//permutation des diagonales pour les valeurs
				float tampon = d(0,0);
				d(0,0) = d(1,1);
				d(1,1)=tampon;
				tampon = v(0,0);
				float tampon2 =v(1,0);

				//permutation des colonnes pour les vecteurs (logique, ça suit les valeurs
				v(0,0)=v(0,1);
				v(1,0)=v(1,1);
				v(0,1)=tampon;
				v(1,1)=tampon2;
			}
			d(0,0)=L2(ii,jj);
			d(1,1)=L1(ii,jj);
			Wmat = v*d*v.transpose();
			D11(ii,jj)=Wmat(0,0);
			D22(ii,jj)=Wmat(1,1);
			D12(ii,jj)=Wmat(0,1);
		}
	}
	// fin calcul tensors

	H3_MATRIX_FLT32 A = CH3MatrixOperations::laplacian_matrix_tensor(gxMatrix.GetLi(),gxMatrix.GetCo(),D11,D12,D12,D22);
	H3_MATRIX_FLT32 f = CH3MatrixOperations::calculate_f_tensor(gxMatrix,gyMatrix,D11,D12,D12,D22);
	
	A=A.SubMat(0,1);

	// il faut mettre les colonnes les unes à la suite des autres... normalement ça : devrait marcher

	H3_MATRIX_FLT32 temp_f= f.transpose();
	temp_f.ReSetDims(temp_f.GetSize(),1);
	
	//Cette opération retourne une matrice Z de dimension rxr
	H3_MATRIX_FLT32 Z = A.Inv()*f;
	
	H3_MATRIX_FLT32 Z_temp(Z.GetLi()+1,1);
	Z_temp.Fill(0);

	for(int i=0;i<Z.GetSize();i++)
	{
		Z_temp(i+1)=-Z(i);
	}
	//reshape du Z en question : 

	H3_MATRIX_FLT32 Z_final(gxMatrix.GetCo(),gxMatrix.GetLi());

	Z_temp=Z_temp.transpose();
	for(unsigned int i=0;i<Z_final.GetLi();i++)
	{
		for(unsigned int j=0;j<Z_final.GetCo();j++)
		{
			Z_final(i,j)=Z_temp(i*Z_final.GetCo()+j);
		}
	}
	
	return Z_final.transpose();
}

int CH3MatrixOperations::eig_2x2(H3_MATRIX_FLT32 &sourceMatrix,H3_MATRIX_FLT32 &eigenValues, H3_MATRIX_FLT32 &eigenVectors)
{
	// Calcul des valeurs propres d'une matrice 2x2.

	// Dans un premier temps, calcul du déterminant de A-lambda*I, qui pour une matrice
	//	| a   b |
	//  | c   d |
	// vaut (a+d)²-4(ad-bc)
	if(sourceMatrix.GetCo()==2&&sourceMatrix.GetLi()==2)
	{
		float delta = pow(sourceMatrix(0,0)+sourceMatrix(1,1),2) -4*(sourceMatrix(0,0)*sourceMatrix(1,1)-sourceMatrix(0,1)*sourceMatrix(1,0));

		// Trois cas de figure : delta positif : 2 solutions réelles
		//						 delta nul : 1 solution double
		//						 delta négatif : 2 solutions complexes
		if(delta>0)
		{
			// 2 solutions réelles 
			float lambda1 = (-(-sourceMatrix(0,0)-sourceMatrix(1,1))- sqrt(delta))/(2);
			float lambda2 = (-(-sourceMatrix(0,0)-sourceMatrix(1,1))+ sqrt(delta))/(2);
			
			eigenValues.ReAlloc(2,2);
			eigenValues.Fill(0);
			
			eigenValues(0,0)=lambda1;
			eigenValues(1,1)=lambda2;

			eigenVectors.ReAlloc(2,2);
			
			// vecteur propre 1 : affecté comme dans le PDF !
			eigenVectors(0,0) = -sourceMatrix(0,1)/(sourceMatrix(0,0)-lambda1);
			eigenVectors(1,0) = 1;
			// vecteur propre 2 : tout pareil
			eigenVectors(0,1) = -sourceMatrix(0,1)/(sourceMatrix(0,0)-lambda2);
			eigenVectors(1,1) = 1;

			return(2);
		}
		else if(delta==0)
		{
			// 2 solutions réelles 
			float lambda = (-(-sourceMatrix(0,0)-sourceMatrix(1,1))/(2));
			
			eigenValues.ReAlloc(2,2);
			eigenValues.Fill(0);
			
			eigenValues(0,0)=lambda;
			eigenValues(1,1)=lambda;

			eigenVectors.ReAlloc(2,2);
			
			// vecteur propre 1 : affecté comme dans le PDF !
			eigenVectors(0,0) = -sourceMatrix(0,1)/(sourceMatrix(0,0)-lambda);
			eigenVectors(1,0) = 1;
			// vecteur propre 2 : tout pareil
			eigenVectors(0,1) = -sourceMatrix(0,1)/(sourceMatrix(0,0)-lambda);
			eigenVectors(1,1) = 1;

			return 1;
		}
		else
		{
//			AfxMessageBox("Et là, c'est le drame : déterminant négatif !");
			ASSERT(FALSE);
			return(0);
		}
	}
	else
	return(-1);


}

H3_MATRIX_FLT32 CH3MatrixOperations::laplacian_matrix_tensor(int H,int W,H3_MATRIX_FLT32 D11,H3_MATRIX_FLT32 D12,H3_MATRIX_FLT32 D21,H3_MATRIX_FLT32 D22)
{
	//test des matrices passées en paramètre
	if(D11.GetSize()==0||D12.GetSize()==0||D21.GetSize()==0||D22.GetSize())
	{
		D11.Fill(1);
		D12.Fill(1);
		D21.Fill(1);
		D22.Fill(1);
	}
	D21=D12;
	D11=padarray(D11,0,1,1,both);
	D12=padarray(D12,0,1,1,both);
	D21=padarray(D21,0,1,1,both);
	D22=padarray(D22,0,1,1,both);
	
	int N = (H+2)*(W+2);
	H3_MATRIX_FLT32 mask(H,W);
	mask.Fill(1);
	mask = padarray(mask,0,1,1,both);
	
	vector<int> temp_idx;
	for(int i=0;i<mask.GetSize();i++)
	{
		if(mask(i)==1.0)
		{
			temp_idx.push_back(i);
		}
	}
	
	H3_MATRIX_FLT32 idx((long)temp_idx.size(),1);
	for(int i=0;i<(int)temp_idx.size();i++)
	{
		idx(i)=(float)temp_idx[i];
	}
	
	H3_MATRIX_FLT32 A(H+2,W+2);
	
	A = sparse(idx,idx+1.0f,D22.VectorMatrixFromIndiceMatrix(idx)*(-1.0f),N,N);
	A = A + sparse(idx,idx+(((float)H)+2.0f),D11.VectorMatrixFromIndiceMatrix(idx)*(-1.0f),N,N);
	A = A + sparse(idx,idx-1.0f,D22.VectorMatrixFromIndiceMatrix(idx-1.0f)*(-1.0f),N,N);
	A = A + sparse(idx,idx-(H+2.0f),D11.VectorMatrixFromIndiceMatrix(idx-(((float)H)+2.0f))*(-1.0f),N,N);

	A = A + sparse(idx,idx+1,D12.VectorMatrixFromIndiceMatrix(idx)*(-1),N,N);
	A = A + sparse(idx,idx -(((float)H)+2.0f),D12.VectorMatrixFromIndiceMatrix(idx-(((float)H)+2))*(-1.0f),N,N);
	A = A + sparse(idx,idx - (((float)H)+2.0f-1.0f),D12.VectorMatrixFromIndiceMatrix(idx-(((float)H)+2.0f)),N,N);
	A = A + sparse(idx,idx + (((float)H) + 2.0f),D21.VectorMatrixFromIndiceMatrix(idx),N,N);
	A = A + sparse(idx,idx - 1.0f,D21.VectorMatrixFromIndiceMatrix(idx-1.0f)*(-1.0f),N,N);
	A = A + sparse(idx,idx-(1.0f-((float)H)-2.0f),D21.VectorMatrixFromIndiceMatrix(idx-1.0f),N,N);

	A = A.SubMat(1,1,H,W);
	
	A = A.VectorMatrixFromLineColumsMatrixes(idx,idx);
	
	N = A.GetLi();

	// Code matlab : dd = sum(A,2)
	H3_MATRIX_FLT32 dd(A.GetLi(),1);
	dd.Fill(0);
	for(unsigned int i=0;i<A.GetLi();i++)
	{
		for(unsigned int j=0;j<A.GetCo();j++)
		{
			dd(i)+=A(i,j);
		}
	}
	//code Matlab : idx = [1:N]'
	
	for(int i=0;i<N;i++)
	{
		idx(i)=(float)i;
	}
	
	A= A+ sparse(idx,idx,dd*(-1),N,N);
// FINIIII !
	return(A);
}

H3_MATRIX_FLT32 CH3MatrixOperations::calculate_f_tensor(H3_MATRIX_FLT32 p,H3_MATRIX_FLT32 q,H3_MATRIX_FLT32 D11,H3_MATRIX_FLT32 D12,H3_MATRIX_FLT32 D21,H3_MATRIX_FLT32 D22)
{
	//test de la taille des matrices d'entrée, si elles sont égales à zéro, alors on les remplit de 1
	if(D11.GetSize()==0||D12.GetSize()==0||D21.GetSize()==0||D22.GetSize())
	{
		D11.Fill(1);
		D12.Fill(1);
		D21.Fill(1);
		D22.Fill(1);
	}
	
	//normalement ce n'est pas nécessaire vu l'appel de la fonction qui est fait dans AffineTransformation
	D21.Copy(D12);
	
	H3_MATRIX_FLT32 gx1 = CH3MatrixOperations::scalar_mul(p,D11);
	H3_MATRIX_FLT32 gy1 = CH3MatrixOperations::scalar_mul(p,D22);
	gx1= CH3MatrixOperations::padarray(gx1,0,1,1,both);
	gy1= CH3MatrixOperations::padarray(gy1,0,1,1,both);

	H3_MATRIX_FLT32 gxx(gx1.GetLi(),gx1.GetCo());
	gxx.Fill(0);
	H3_MATRIX_FLT32 gyy(gx1.GetLi(),gx1.GetCo());
	gyy.Fill(0);
	
// 	H3_MATRIX_FLT32 j(1,p.GetLi()+1);
// 	H3_MATRIX_FLT32 k(1,p.GetLi()+1);
	
	// Laplacien (beurk !)
	unsigned int i = 0;
	unsigned int j = 0;
	unsigned int k = 0;
	for(j=0;j<p.GetLi()+1;j++)
	{
		for( k=0;k<p.GetCo()+1;k++)
		{
			gyy(j,k+1)=gy1(j+1,k)-gy1(j,k);
			gxx(j,k+1)=gx1(j,k+1)-gx1(j,k);
		}
	}
	
	H3_MATRIX_FLT32 f = gxx+gyy;
	f= f.SubMat(1,1,f.GetLi()-1,f.GetCo()-1);

	//On efface les matrices pour le calcul suivant
	gx1.Fill(0);
	gy1.Fill(0);
	gxx.Fill(0);
	gyy.Fill(0);

	gx1 = CH3MatrixOperations::scalar_mul(p,D12);
	gy1 = CH3MatrixOperations::scalar_mul(p,D21);

	H3_MATRIX_FLT32 gx2 = CH3MatrixOperations::scalar_mul(q,D12);
	H3_MATRIX_FLT32 gy2 = CH3MatrixOperations::scalar_mul(p,D21);


	for(j=0;j<gx2.GetCo();j++)
	{
		gx2(gx2.GetLi()-1,j)=gx1(gx1.GetLi()-1,j);

	}

	for(j=0;j<gx2.GetLi();j++)
	{
		gy2(j,gy2.GetCo()-1)=gy1(j,gy2.GetCo()-1);
	}

	for(j=0;j<gx2.GetLi();j++)
	{
		gx2(j,gx2.GetCo()-1)=0;
	}

	for(j=0;j<gy2.GetCo();j++)
	{
		gy2(gx2.GetLi()-1,j)=0;
	}

	gx2 = CH3MatrixOperations::padarray(gx2,0,1,1,both);
	gy2 = CH3MatrixOperations::padarray(gy2,0,1,1,both);

	gxx.ReAlloc(gx2.GetLi(),gx2.GetCo());
	gxx.Fill(0);

	gyy.ReAlloc(gx2.GetLi(),gx2.GetCo());
	gyy.Fill(0);

	// Laplacien (beurk !)
	for(j=0;j<p.GetLi()+1;j++)
	{
		for(k=0;k<p.GetCo()+1;k++)
		{
			gyy(j,k+1)=gy2(j+1,k)-gy2(j,k);
			gxx(j,k+1)=gx2(j,k+1)-gx2(j,k);
		}
	}

	H3_MATRIX_FLT32 f2 = gxx + gyy;
	f2 = f2.SubMat(1,1,f2.GetLi()-2,f2.GetCo()-2);

	f = f + f2;

	return(f);
}

