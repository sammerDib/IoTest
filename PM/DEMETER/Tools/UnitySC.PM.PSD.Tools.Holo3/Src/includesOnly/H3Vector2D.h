#ifndef CH3VECTOR2D__INCLUDED_
#define CH3VECTOR2D__INCLUDED_

#include "H3Doublet.h"
#include "H3Point2D.h"

#define H3_VECTOR2D			CH3Vector2D< TYPE >
#define H3_VECTOR2D_INT32	CH3Vector2D< long >
#define H3_VECTOR2D_FLT32	CH3Vector2D< float >
#define H3_VECTOR2D_FLT64	CH3Vector2D< double >

#define H3_VECTOR2D_FILE_VERSION		100

template <class TYPE> 
class CH3Vector2D:
	virtual public H3_DOUBLET
{
public:
	// Constructeurs et destructeur
	H3_VECTOR2D(TYPE val=0){x=y=val;};
    H3_VECTOR2D(TYPE Coordx,TYPE Coordy){x=Coordx;y=Coordy;};
	~H3_VECTOR2D(){};
	H3_VECTOR2D(H3_POINT2D P1,H3_POINT2D P2){H3_POINT2D P(P2-P1);x=P.x;y=P.y;};
private: 
	H3_VECTOR2D( H3_DOUBLET T){x=T.x;y=T.y;};
public:

	// Methodes d'E/S
	bool fSave(FILE *Stream)const;
	bool fSaveRAW(FILE *Stream)const;
	bool fLoad(FILE *Stream);
	bool fLoadRAW(FILE *Stream);

	// Operateurs de copie
	H3_VECTOR2D(const H3_VECTOR2D & V):H3_DOUBLET(V){};
	
	// Attributs
	bool IsEmpty()const;

	// Methodes diverses
	void Normalize();
	TYPE Norm()const;
	TYPE Prod_Scal(H3_VECTOR2D & V) const{ return (x*V.x+y*V.y); };

	// Operateurs
//	H3_VECTOR2D operator ^(H3_VECTOR2D & V);

//////////////////////////////////////////////////////////////
//Modif du type retour des fonctions de la classe de base
	// Methodes et operateurs arithmetiques
	H3_VECTOR2D operator +(H3_VECTOR2D & V)	const{return H3_VECTOR2D( (*this).H3_DOUBLET::operator+(V)); };
	H3_VECTOR2D operator -(H3_VECTOR2D & V)	const{return H3_VECTOR2D( (*this).H3_DOUBLET::operator-(V)); };
	H3_VECTOR2D operator *(H3_VECTOR2D & V)	const{return H3_VECTOR2D( (*this).H3_DOUBLET::operator*(V)); };
	H3_VECTOR2D operator /(H3_VECTOR2D & V)	const{return H3_VECTOR2D( (*this).H3_DOUBLET::operator/(V)); };
	H3_VECTOR2D & operator =(const H3_VECTOR2D & V);

	// Methodes et operateur arithmetiques avec un scalaire
	H3_VECTOR2D operator +(double Value)const{return H3_VECTOR2D( (*this).H3_DOUBLET::operator+(Value)); };
	H3_VECTOR2D operator -(double Value)const{return H3_VECTOR2D( (*this).H3_DOUBLET::operator-(Value)); };
	H3_VECTOR2D operator *(double Value)const{return H3_VECTOR2D( (*this).H3_DOUBLET::operator*(Value)); };
	/*H3_VECTOR2D operator *(double Value)const{
		H3_DOUBLET t(H3_DOUBLET(*this)*Value);
		return H3_VECTOR2D(t);  };*///cv250308__Pourquoi cette fct est elle ecrite differement des autres ???
	H3_VECTOR2D operator /(double Value)const{return H3_VECTOR2D( (*this).H3_DOUBLET::operator/(Value)); };

///////////////////////////////////////////////////////////////////////

protected:

};

//////////////////////////////////////////////////////////////////////////////////////
// Groupe des constructeurs/Destructeurs
//////////////////////////////////////////////////////////////////////////////////////
// Constructeur par defaut
// Autres Constructeurs

// Destructeur

// operateur
template <class TYPE> 
H3_VECTOR2D & H3_VECTOR2D::operator =(const H3_VECTOR2D &T)
{
	if (this==&T)
		return *this;

	x=T.x;
	y=T.y;
	return *this;
}

/*
// Operateur de produit vectoriel
template <class TYPE> 
H3_VECTOR2D H3_VECTOR2D::operator^(H3_VECTOR2D & V)
{
	H3_VECTOR3D Tmp;
	Tmp.x= y*V.z - z*V.y;
	Tmp.y= z*V.x - x*V.z;
	Tmp.z= x*V.y - y*V.x;
	return Tmp;
}
*/

// Normalise le vecteur
template <class TYPE> 
void H3_VECTOR2D::Normalize()
{
	TYPE n=Norm();
	if (!_isnan(n))
	{
		if (n!=0)
		{
			x/=n;
			y/=n;
		}
		else
		{
			x=0;
			y=0;
		}
	}
	else
	{
		x=n;
		y=n;
	}
}

//////////////////////////////////////////////////////////////////////////////////////
// Methodes d'E/S
//////////////////////////////////////////////////////////////////////////////////////
template <class TYPE> 
bool H3_VECTOR2D::fSave(FILE *Stream)const
{
/*	const type_info& t = typeid(this);
	unsigned long nTypeNameLen=strlen(t.name());
	::fwrite(t.name(),sizeof(char),nTypeNameLen+1,Stream);*/

	// Ecrire type
	// EC 03/08/07 pour compatibilite VC6 avec VC8
	{
		const type_info& t = typeid(this);   
		string strName=t.name();
		H3ChangeTypeIdName(strName);
		::fwrite(strName.c_str(),sizeof(char),strName.length()+1,Stream);
	}

	unsigned long nVersion=H3_VECTOR2D_FILE_VERSION;
	::fwrite(&nVersion,sizeof(unsigned long),1,Stream);

	return fSaveRAW(Stream);
}

template <class TYPE> 
bool H3_VECTOR2D::fSaveRAW(FILE *Stream)const
{
	if (Stream)
	{
		if (::fwrite(&x,sizeof(TYPE),1,Stream)!=1)
			return false;
		if (::fwrite(&y,sizeof(TYPE),1,Stream)!=1)
			return false;
	}
	return true;
}

template <class TYPE>
bool H3_VECTOR2D::fLoad(FILE *Stream)
{
	if (!Stream)
		return false;
	
	// Lire le type et le verifier
	// EC 03/08/07 pour compatibilite VC6 avec VC8
	{
		const type_info& t = typeid(this);

		string strName=t.name();
		H3ChangeTypeIdName(strName);
		unsigned long nTypeNameLen=strName.length()+1;
		char *pszTypeName=new char [nTypeNameLen];
		if (fread(pszTypeName,sizeof(char),nTypeNameLen,Stream)!=nTypeNameLen)
		{
			delete [] pszTypeName;
			return false;
		}
		pszTypeName[nTypeNameLen-1]='\0';

		if (strName!=string(pszTypeName))
		{
			delete [] pszTypeName;
			return false;
		}
		delete [] pszTypeName;
	}

	// Lire la version
	unsigned long nVersion;
	if (fread(&nVersion,sizeof(unsigned long),1,Stream)!=1)
	{
		return false;
	}
	if (nVersion!=H3_VECTOR2D_FILE_VERSION)
	{
		return false;
	}

	// Lire les donnees
	return fLoadRAW(Stream);
}

template <class TYPE> 
bool H3_VECTOR2D::fLoadRAW(FILE *Stream)
{
	if (Stream)
	{
		if (::fread(&x,sizeof(TYPE),1,Stream)!=1)
			return false;
		if (::fread(&y,sizeof(TYPE),1,Stream)!=1)
			return false;	
	}
	return true;
}


//////////////////////////////////////////////////////////////////////////////////////
// Attributs
//////////////////////////////////////////////////////////////////////////////////////
// Retourne la norme du vecteur
template <class TYPE> 
TYPE H3_VECTOR2D::Norm()const
{
	return sqrt(x*x + y*y);
}

// Retourne vrai si le vecteur est vide (idem IsNull())
template <class TYPE> 
bool H3_VECTOR2D::IsEmpty()const
{
	return IsNull();
}

#endif