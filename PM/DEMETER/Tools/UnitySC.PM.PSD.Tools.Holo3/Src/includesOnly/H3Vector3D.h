#ifndef CH3VECTOR3D__INCLUDED_
#define CH3VECTOR3D__INCLUDED_

#include "H3Triplet.h"
#include "H3Point3D.h"

#define H3_VECTOR3D			CH3Vector3D< TYPE >
#define H3_VECTOR3D_INT32	CH3Vector3D< long >
#define H3_VECTOR3D_FLT32	CH3Vector3D< float >
#define H3_VECTOR3D_FLT64	CH3Vector3D< double >

#define H3_VECTOR3D_FILE_VERSION		100

template <class TYPE> 
class CH3Vector3D:
	virtual public H3_TRIPLET
{
public:
	// Constructeurs et destructeur
	H3_VECTOR3D(TYPE val=0){x=y=z=val;};
    H3_VECTOR3D(TYPE Coordx,TYPE Coordy,TYPE Coordz){x=Coordx;y=Coordy;z=Coordz;};
	~H3_VECTOR3D(){};
	H3_VECTOR3D(const H3_POINT3D &P1,const H3_POINT3D &P2){H3_POINT3D P(P2-P1);x=P.x;y=P.y;z=P.z; };
	H3_VECTOR3D(const H3_TRIPLET &T){x=T.x;y=T.y;z=T.z;};
public:

	// Methodes d'E/S
	bool fSave(FILE *Stream)const;
	bool fSaveRAW(FILE *Stream)const;
	bool fLoad(FILE *Stream);
	bool fLoadRAW(FILE *Stream);

	// Operateurs de copie
	H3_VECTOR3D(H3_VECTOR3D & V):H3_TRIPLET(V){};
	
	// Attributs
	bool IsEmpty()const;

	// Methodes diverses
	void Normalize();
	TYPE Norm()const;
	TYPE Prod_Scal(const H3_VECTOR3D & V)const { return (x*V.x+y*V.y+z*V.z); };

	// Operateurs
	H3_VECTOR3D operator ^(const H3_VECTOR3D & V)const;

//////////////////////////////////////////////////////////////
//Modif du type retour des fonctions de la classe de base
		// Methodes et operateurs arithmetiques
	H3_VECTOR3D operator +(const H3_VECTOR3D & V)	const{return H3_VECTOR3D( (*this).H3_TRIPLET::operator+(V)); };
	H3_VECTOR3D operator -(const H3_VECTOR3D & V)	const{return H3_VECTOR3D( (*this).H3_TRIPLET::operator-(V)); };
	H3_VECTOR3D operator *(const H3_VECTOR3D & V)	const{return H3_VECTOR3D( (*this).H3_TRIPLET::operator*(V)); };
	H3_VECTOR3D operator /(const H3_VECTOR3D & V)	const{return H3_VECTOR3D( (*this).H3_TRIPLET::operator/(V)); };
	H3_VECTOR3D & operator =(const H3_VECTOR3D & V);

	// Methodes et operateur arithmetiques avec un scalaire
	H3_VECTOR3D operator +(const double Value)const{return H3_VECTOR3D( (*this).H3_TRIPLET::operator+(Value)); };
	H3_VECTOR3D operator -(const double Value)const{return H3_VECTOR3D( (*this).H3_TRIPLET::operator-(Value)); };
	H3_VECTOR3D operator *(const double Value)const{return H3_VECTOR3D( (*this).H3_TRIPLET::operator*(Value)); };
	H3_VECTOR3D operator /(const double Value)const{return H3_VECTOR3D( (*this).H3_TRIPLET::operator/(Value)); };

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
H3_VECTOR3D & H3_VECTOR3D::operator =(const H3_VECTOR3D &T)
{
	if (this==&T)
		return *this;

	x=T.x;
	y=T.y;
	z=T.z;
	return *this;
}

// Operateur de produit vectoriel
template <class TYPE> 
H3_VECTOR3D H3_VECTOR3D::operator^(const H3_VECTOR3D & V)const
{
	H3_VECTOR3D Tmp;
	Tmp.x= y*V.z - z*V.y;
	Tmp.y= z*V.x - x*V.z;
	Tmp.z= x*V.y - y*V.x;
	return Tmp;
}

// Normalise le vecteur
template <class TYPE> 
void H3_VECTOR3D::Normalize()
{
	TYPE n=Norm();
	if (!_isnan(n))
	{
		if (n!=0)
		{
			x/=n;
			y/=n;
			z/=n;
		}
		else
		{
			x=0;
			y=0;
			z=0;
		}
	}
	else
	{
		x=n;
		y=n;
		z=n;
	}
}

//////////////////////////////////////////////////////////////////////////////////////
// Methodes d'E/S
//////////////////////////////////////////////////////////////////////////////////////
template <class TYPE> 
bool H3_VECTOR3D::fSave(FILE *Stream)const
{
	// Ecrire type
	// EC 03/08/07 pour compatibilite VC6 avec VC8
	{
		const type_info& t = typeid(this);   
		string strName=t.name();
		H3ChangeTypeIdName(strName);
		::fwrite(strName.c_str(),sizeof(char),strName.length()+1,Stream);
	}

	unsigned long nVersion=H3_VECTOR3D_FILE_VERSION;
	::fwrite(&nVersion,sizeof(unsigned long),1,Stream);

	return fSaveRAW(Stream);
}

template <class TYPE> 
bool H3_VECTOR3D::fSaveRAW(FILE *Stream)const
{
	if (Stream)
	{
		if (::fwrite(&x,sizeof(TYPE),1,Stream)!=1)
			return false;
		if (::fwrite(&y,sizeof(TYPE),1,Stream)!=1)
			return false;
		if (::fwrite(&z,sizeof(TYPE),1,Stream)!=1)
			return false;
	}
	return true;
}

template <class TYPE> 
bool H3_VECTOR3D::fLoad(FILE *Stream)
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
	if (nVersion!=H3_VECTOR3D_FILE_VERSION)
	{
		return false;
	}

	// Lire les donnees
	return fLoadRAW(Stream);
}

template <class TYPE> 
bool H3_VECTOR3D::fLoadRAW(FILE *Stream)
{
	if (Stream)
	{
		if (::fread(&x,sizeof(TYPE),1,Stream)!=1)
			return false;
		if (::fread(&y,sizeof(TYPE),1,Stream)!=1)
			return false;	
		if (::fread(&z,sizeof(TYPE),1,Stream)!=1)
			return false;	
	}
	return true;
}


//////////////////////////////////////////////////////////////////////////////////////
// Attributs
//////////////////////////////////////////////////////////////////////////////////////
// Retourne la norme du vecteur
template <class TYPE> 
TYPE H3_VECTOR3D::Norm()const
{
	return sqrt(x*x + y*y + z*z);
}

// Retourne vrai si le vecteur est vide (idem IsNull())
template <class TYPE> 
bool H3_VECTOR3D::IsEmpty()const
{
	return IsNull();
}

#endif