#ifndef CH3POINT3D__INCLUDED_
#define CH3POINT3D__INCLUDED_

#define H3_POINT3D			CH3Point3D< TYPE >
#define H3_POINT3D_INT32	CH3Point3D< long >
#define H3_POINT3D_FLT32	CH3Point3D< float >
#define H3_POINT3D_FLT64	CH3Point3D< double >

#include "H3Triplet.h"

#define H3_POINT3D_FILE_VERSION		100

template <class TYPE> 
class CH3Array;

template <class TYPE> 
class CH3Point3D:
	public H3_TRIPLET
{
public:
	// Constructeurs et destructeur
	H3_POINT3D(TYPE val=0):H3_TRIPLET(val){};
    H3_POINT3D(TYPE Coordx,TYPE Coordy,TYPE Coordz): H3_TRIPLET(Coordx,Coordy,Coordz){};
	~H3_POINT3D(){};
	H3_POINT3D(const H3_TRIPLET &T){x=T.x;y=T.y;z=T.z;};

public:
	// Methodes d'E/S
	bool fSave(FILE *Stream)const;
	bool fSaveRAW(FILE *Stream)const;
	bool fLoad(FILE *Stream);
	bool fLoadRAW(FILE *Stream);

	// Methodes de conversion de type
	operator H3_POINT3D_INT32()const;
	operator H3_POINT3D_FLT32()const;
	operator H3_POINT3D_FLT64()const;

	// Operateurs de copie
	//H3_POINT3D(H3_POINT3D & P);
	H3_POINT3D(const H3_POINT3D & P):H3_TRIPLET(P){};
	
	// Methodes diverses
	TYPE DistanceTo(H3_POINT3D &P)const;
	CH3Array< TYPE > DistanceTo(CH3Array< H3_POINT3D > &pt)const;

	// Operateurs
//////////////////////////////////////////////////////////////
//Modif du type retour des fonctions de la classe de base
		// Methodes et operateurs arithmetiques
	H3_POINT3D operator +(const H3_POINT3D & P)const;
	H3_POINT3D operator -(const H3_POINT3D & P)const;
	H3_POINT3D operator *(const H3_POINT3D & P)const;
	H3_POINT3D operator /(const H3_POINT3D & P)const;
	H3_POINT3D&operator =(const H3_POINT3D & P);

	// Methodes et operateur arithmetiques avec un scalaire
	H3_POINT3D operator +(const double Value)const;
	H3_POINT3D operator -(const double Value)const;
	H3_POINT3D operator *(const double Value)const;
	H3_POINT3D operator /(const double Value)const;

///////////////////////////////////////////////////////////////////////
//public:TYPE x,y,z;
protected:

};

//////////////////////////////////////////////////////////////////////////////////////
// Groupe des constructeurs/Destructeurs
//////////////////////////////////////////////////////////////////////////////////////

template <class TYPE> 
H3_POINT3D & H3_POINT3D::operator =(const H3_POINT3D &T)
{
	if (this==&T) return *this;

	x=T.x;
	y=T.y;
	z=T.z;
	return *this;
}

//////////////////////////////////////////////////////////////////////////////////////
// Methodes d'E/S
//////////////////////////////////////////////////////////////////////////////////////
template <class TYPE> 
bool H3_POINT3D::fSave(FILE *Stream)const
{
	/*const type_info& t = typeid(this);
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

	unsigned long nVersion=H3_POINT3D_FILE_VERSION;
	::fwrite(&nVersion,sizeof(unsigned long),1,Stream);

	return fSaveRAW(Stream);
}

template <class TYPE> 
bool H3_POINT3D::fSaveRAW(FILE *Stream)const
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
bool H3_POINT3D::fLoad(FILE *Stream)
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
	if (nVersion!=H3_POINT3D_FILE_VERSION)
	{
		return false;
	}

	// Lire les donnees
	return fLoadRAW(Stream);
}

template <class TYPE> 
bool H3_POINT3D::fLoadRAW(FILE *Stream)
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
// Retourne la distance au point
template <class TYPE> 
TYPE H3_POINT3D::DistanceTo(H3_POINT3D &pt)const
{
	TYPE dx=x-pt.x;
	TYPE dy=y-pt.y;
	TYPE dz=z-pt.z;
	return sqrt(dx*dx+dy*dy+dz*dz);
}

template <class TYPE> 
CH3Array< TYPE > H3_POINT3D::DistanceTo(CH3Array< H3_POINT3D >  &pt)const
{
	long Count=pt.GetSize();
	CH3Array< TYPE > Res(Count);
	TYPE dx,dy,dz;

	for (long i=0;i<Count;i++){
		dx=x-pt[i].x;
		dy=y-pt[i].y;
		dz=z-pt[i].z;
		Res[i]=sqrt(dx*dx+dy*dy+dz*dz);
	}

	return Res;
}


//////////////////////////////////////////////////////////////////////////////////////
// Methodes et operateur arithmetiques
//////////////////////////////////////////////////////////////////////////////////////

template <class TYPE> 
H3_POINT3D H3_POINT3D::operator +(const H3_POINT3D & P)const
{
	//H3_POINT3D Tmp( H3_TRIPLET(*this)+P);
	H3_POINT3D Tmp( (*this).H3_TRIPLET::operator+(P));
	return Tmp;
}

template <class TYPE> 
H3_POINT3D H3_POINT3D::operator -(const H3_POINT3D & P)const
{
	//H3_POINT3D Tmp( H3_TRIPLET(*this)-P);
	H3_POINT3D Tmp( (*this).H3_TRIPLET::operator-(P));
	return Tmp;
}

template <class TYPE> 
H3_POINT3D H3_POINT3D::operator *(const H3_POINT3D & P)const
{
	//H3_POINT3D Tmp( H3_TRIPLET(*this)*P);
	H3_POINT3D Tmp( (*this).H3_TRIPLET::operator*(P));
	return Tmp;
}

template <class TYPE> 
H3_POINT3D H3_POINT3D::operator /(const H3_POINT3D & P)const
{
	//H3_POINT3D Tmp( H3_TRIPLET(*this)/P);
	H3_POINT3D Tmp( (*this).H3_TRIPLET::operator/(P));
	return Tmp;
}
//////////////////////////////////////////////////////////////////////////////////////
// Methodes et operateur arithmetiques avec un scalaire
//////////////////////////////////////////////////////////////////////////////////////
template <class TYPE> 
H3_POINT3D H3_POINT3D::operator +(const double value)const
{
	H3_POINT3D Tmp(*this);
	Tmp+=value;
	return Tmp;
}

template <class TYPE> 
H3_POINT3D H3_POINT3D::operator -(const double value)const
{
	H3_POINT3D Tmp(*this);
	Tmp-=value;
	return Tmp;
}

template <class TYPE> 
H3_POINT3D H3_POINT3D::operator *(const double value)const
{
	H3_POINT3D Tmp(*this);
	Tmp*=value;
	return Tmp;
}

template <class TYPE> 
H3_POINT3D H3_POINT3D::operator /(const double value)const
{
	H3_POINT3D Tmp(*this);
	Tmp/=value;
	return Tmp;
}

//////////////////////////////////////////////////////////////////////////////////////
// Methodes de conversion de type
//////////////////////////////////////////////////////////////////////////////////////
// Conversion vers H3_INT32
template <class TYPE> 
H3_POINT3D::operator H3_POINT3D_INT32()const
{
	return H3_POINT3D_INT32((H3_INT32)x,(H3_INT32)y,(H3_INT32)z);
}

// Conversion vers H3_FLT32
template <class TYPE> 
H3_POINT3D::operator H3_POINT3D_FLT32()const
{
	return H3_POINT3D_FLT32(x,y,z);
}

// Conversion vers H3_FLT64
template <class TYPE> 
H3_POINT3D::operator H3_POINT3D_FLT64()const
{
	return H3_POINT3D_FLT64(x,y,z);
}

//////////////////////////////////////////////////////////////////////////////////////
// Methodes diverses
//////////////////////////////////////////////////////////////////////////////////////

#endif