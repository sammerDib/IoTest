#ifndef CH3POINT2D__INCLUDED_
#define CH3POINT2D__INCLUDED_

#define H3_POINT2D			CH3Point2D< TYPE >
#define H3_POINT2D_INT32	CH3Point2D< long >
#define H3_POINT2D_UINT32	CH3Point2D< unsigned long >
#define H3_POINT2D_FLT32	CH3Point2D< float >
#define H3_POINT2D_FLT64	CH3Point2D< double >

#include "H3Doublet.h"
#include "H3BaseDef.h"

#define H3_POINT2D_FILE_VERSION		100

template <class TYPE> 
class CH3Array;

template <class TYPE> 
class CH3Point2D:
	public H3_DOUBLET
{
public:
	// Constructeurs et destructeur
	H3_POINT2D(TYPE val=0):H3_DOUBLET(val){};
    H3_POINT2D(TYPE Coordx,TYPE Coordy): H3_DOUBLET(Coordx,Coordy){};
	~H3_POINT2D(){};
	H3_POINT2D(H3_DOUBLET T){x=T.x;y=T.y;};

	// Methodes d'E/S
	bool fSave(FILE *Stream)const;
	bool fSaveRAW(FILE *Stream)const;
	bool fLoad(FILE *Stream)const;
	bool fLoadRAW(FILE *Stream)const;

	// Methodes de conversion de type
	operator H3_POINT2D_INT32()const;
	operator H3_POINT2D_FLT32()const;
	operator H3_POINT2D_FLT64()const;

	// Operateurs de copie
	H3_POINT2D(H3_POINT2D & P):H3_DOUBLET(P){};
	
	// Methodes diverses
	TYPE DistanceTo(H3_POINT2D &P)const;
	CH3Array< TYPE > DistanceTo(CH3Array< H3_POINT2D > &pt)const;

	// Operateurs
//////////////////////////////////////////////////////////////
//Modif du type retour des fonctions de la classe de base
		// Methodes et operateurs arithmetiques
	H3_POINT2D operator +(const H3_POINT2D & P)const;
	H3_POINT2D operator -(const H3_POINT2D & P)const;
	H3_POINT2D operator *(const H3_POINT2D & P)const;
	H3_POINT2D operator /(const H3_POINT2D & P)const;
	H3_POINT2D&operator =(const H3_POINT2D & P);

	// Methodes et operateur arithmetiques avec un scalaire
	H3_POINT2D operator +(const double Value)const;
	H3_POINT2D operator -(const double Value)const;
	H3_POINT2D operator *(const double Value)const;
	H3_POINT2D operator /(const double Value)const;
};

//////////////////////////////////////////////////////////////////////////////////////
// Groupe des constructeurs/Destructeurs
//////////////////////////////////////////////////////////////////////////////////////

template <class TYPE> 
H3_POINT2D & H3_POINT2D::operator =(const H3_POINT2D &T)
{
	if (this==&T)
		return *this;

	x=T.x;
	y=T.y;
	return *this;
}

//////////////////////////////////////////////////////////////////////////////////////
// Methodes d'E/S
//////////////////////////////////////////////////////////////////////////////////////
template <class TYPE> 
bool H3_POINT2D::fSave(FILE *Stream)const
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

	unsigned long nVersion=H3_POINT2D_FILE_VERSION;
	::fwrite(&nVersion,sizeof(unsigned long),1,Stream);

	return fSaveRAW(Stream);
}

template <class TYPE> 
bool H3_POINT2D::fSaveRAW(FILE *Stream)const
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
bool H3_POINT2D::fLoad(FILE *Stream)const
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
	if (nVersion!=H3_POINT2D_FILE_VERSION)
	{
		return false;
	}

	// Lire les donnees
	return fLoadRAW(Stream);
}

template <class TYPE> 
bool H3_POINT2D::fLoadRAW(FILE *Stream)const
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
// Retourne la distance au point
template <class TYPE> 
TYPE H3_POINT2D::DistanceTo(H3_POINT2D &pt)const
{
	TYPE dx=x-pt.x;
	TYPE dy=y-pt.y;
	return sqrt(dx*dx+dy*dy);
}

template <class TYPE> 
CH3Array< TYPE > H3_POINT2D::DistanceTo(CH3Array< H3_POINT2D >  &pt)const
{
	long Count=pt.GetSize();
	CH3Array< TYPE > Res(Count);
	TYPE dx,dy;

	for (long i=0;i<Count;i++){
		dx=x-pt[i].x;
		dy=y-pt[i].y;
		Res[i]=sqrt(dx*dx+dy*dy);
	}

	return Res;
}

//////////////////////////////////////////////////////////////////////////////////////
// Methodes et operateur arithmetiques
//////////////////////////////////////////////////////////////////////////////////////

template <class TYPE> 
H3_POINT2D H3_POINT2D::operator +(const H3_POINT2D & P)const
{
	H3_POINT2D Tmp( H3_DOUBLET(*this)+P);
	return Tmp;
}

template <class TYPE> 
H3_POINT2D H3_POINT2D::operator -(const H3_POINT2D & P)const
{
	H3_POINT2D Tmp( H3_DOUBLET(*this)-P);
	return Tmp;
}

template <class TYPE> 
H3_POINT2D H3_POINT2D::operator *(const H3_POINT2D & P)const
{
	H3_POINT2D Tmp( H3_DOUBLET(*this)*P);
	return Tmp;
}

template <class TYPE> 
H3_POINT2D H3_POINT2D::operator /(const H3_POINT2D & P)const
{
	H3_POINT2D Tmp( H3_DOUBLET(*this)/P);
	return Tmp;
}
//////////////////////////////////////////////////////////////////////////////////////
// Methodes et operateur arithmetiques avec un scalaire
//////////////////////////////////////////////////////////////////////////////////////
template <class TYPE> 
H3_POINT2D H3_POINT2D::operator +(const double value)const
{
	H3_POINT2D Tmp(*this);
	Tmp+=value;
	return Tmp;
}

template <class TYPE> 
H3_POINT2D H3_POINT2D::operator -(const double value)const
{
	H3_POINT2D Tmp(*this);
	Tmp-=value;
	return Tmp;
}

template <class TYPE> 
H3_POINT2D H3_POINT2D::operator *(const double value)const
{
	H3_POINT2D Tmp(*this);
	Tmp*=value;
	return Tmp;
}

template <class TYPE> 
H3_POINT2D H3_POINT2D::operator /(const double value)const
{
	H3_POINT2D Tmp(*this);
	Tmp/=value;
	return Tmp;
}

//////////////////////////////////////////////////////////////////////////////////////
// Methodes de conversion de type
//////////////////////////////////////////////////////////////////////////////////////
// Conversion vers H3_INT32
template <class TYPE> 
H3_POINT2D::operator H3_POINT2D_INT32()const
{
	return H3_POINT2D_INT32((H3_INT32)x,(H3_INT32)y);
}

// Conversion vers H3_FLT32
template <class TYPE> 
H3_POINT2D::operator H3_POINT2D_FLT32()const
{
	return H3_POINT2D_FLT32((float)x,(float)y);
}

// Conversion vers H3_FLT64
template <class TYPE> 
H3_POINT2D::operator H3_POINT2D_FLT64()const
{
	return H3_POINT2D_FLT64((double)x,(double)y);
}

//////////////////////////////////////////////////////////////////////////////////////
// Methodes diverses
//////////////////////////////////////////////////////////////////////////////////////

#endif