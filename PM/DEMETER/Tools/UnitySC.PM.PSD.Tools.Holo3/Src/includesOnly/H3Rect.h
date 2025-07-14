#ifndef CH3RECT__INCLUDED_
#define CH3RECT__INCLUDED_

#include <iostream>
#include <algorithm>

using namespace std;

#define H3_RECT_INT32	CH3Rect< long >
#define H3_RECT_UINT32	CH3Rect< unsigned long >
#define H3_RECT_FLT32	CH3Rect< float >
#define H3_RECT_FLT64	CH3Rect< double >


#define H3_RECT_FILE_VERSION	100

template <class TYPE> 
class CH3Rect
{
public:

	// Methodes E/S
	bool fSave(FILE *Stream);
	bool fLoad(FILE *Stream);
	bool fSaveHeader(FILE *Stream);
	bool fLoadHeader(FILE *Stream);
	bool fSaveRAW(FILE *Stream);
	bool fLoadRAW(FILE *Stream);

	// Methodes
	TYPE Width();
	TYPE Height();
	bool IsEmpty();
	void SetEmpty();
	bool IsNull();
	void SetNull();

	// Methodes de conversion de type
	operator H3_RECT_INT32();
	operator H3_RECT_FLT32();
	operator H3_RECT_FLT64();

	// Operateurs de copie
	CH3Rect & operator =(CH3Rect &rc);
	void Copy(CH3Rect &rc);

	bool IntersectRect(CH3Rect &rc1,CH3Rect &rc2);
	bool UnionRect(CH3Rect &rc1, CH3Rect &rc2);
	bool PtInRect(CH3Point2D<TYPE> pt);

	CH3Point2D<TYPE> BottomLeft();
	CH3Point2D<TYPE> BottomRight();
	CH3Point2D<TYPE> TopLeft();
	CH3Point2D<TYPE> TopRight();

    void Set(TYPE l=0,TYPE t=0,TYPE r=0,TYPE b=0);


	// Constructeurs et destructeur
	CH3Rect(const CH3Rect & rc);
    CH3Rect(CH3Point2D<TYPE> ptTopLeft,CH3Point2D<TYPE> ptBottomRight);
    CH3Rect(TYPE l=0,TYPE t=0,TYPE r=0,TYPE b=0);
	~CH3Rect();

public:
	TYPE top;
	TYPE left;
	TYPE bottom;
	TYPE right;
protected:
};

//////////////////////////////////////////////////////////////////////////////////////
// Groupe des constructeurs/Destructeurs
//////////////////////////////////////////////////////////////////////////////////////
// Constructeur par defaut
template <class TYPE> 
CH3Rect<TYPE>::CH3Rect(TYPE l,TYPE t,TYPE r,TYPE b)
{
	Set(l,t,r,b);
}

template <class TYPE> 
CH3Rect<TYPE>::CH3Rect(CH3Point2D<TYPE> ptTopLeft, CH3Point2D<TYPE> ptBottomRight)
{
	Set(ptTopLeft.x,ptTopLeft.y,ptBottomRight.x,ptBottomRight.y);
}

// Constructeur de copie
template <class TYPE> 
CH3Rect<TYPE>::CH3Rect(const CH3Rect & rc)
{
	if (this!=&rc)
	{
		top=rc.top;
		left=rc.left;
		right=rc.right;
		bottom=rc.bottom;
	}
}

// Destructeur
template <class TYPE> 
CH3Rect<TYPE>::~CH3Rect()
{
}

template <class TYPE> 
void CH3Rect<TYPE>::Set(TYPE l,TYPE t,TYPE r,TYPE b)
{
	top=__min(t,b);
	bottom=__max(t,b);
	right=__max(l,r);
	left=__min(l,r);
}

//////////////////////////////////////////////////////////////////////////////////////
// Methodes et operateurs de copie
//////////////////////////////////////////////////////////////////////////////////////
// Operateur de copie
template <class TYPE> 
CH3Rect<TYPE> & CH3Rect<TYPE>::operator =(CH3Rect &rc)
{
	Copy(rc);
	return *this;
}

template <class TYPE> 
void CH3Rect<TYPE>::Copy(CH3Rect &rc)
{
	if (this==&rc)
		return;

	top=rc.top;
	left=rc.left;
	right=rc.right;
	bottom=rc.bottom;
}

template <class TYPE > 
bool CH3Rect <TYPE>::fSave(FILE *Stream)
{
	if (!fSaveHeader(Stream))
		return false;

	// Ecrire donnees
	return fSaveRAW(Stream);
}

template <class TYPE > 
bool CH3Rect <TYPE>::fSaveRAW(FILE *Stream)
{
	::fwrite(&left,sizeof(left),1,Stream);
	::fwrite(&top,sizeof(top),1,Stream);
	::fwrite(&right,sizeof(right),1,Stream);
	::fwrite(&bottom,sizeof(bottom),1,Stream);
	return true;
}


template <class TYPE > 
bool CH3Rect <TYPE>::fSaveHeader(FILE *Stream)
{
	if (Stream==NULL)
		return false;
/*
	// Ecrire type
	const type_info& t = typeid(this);   
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

	// Ecrire la version
	unsigned long nVersion=H3_RECT_FILE_VERSION;
	::fwrite(&nVersion,sizeof(nVersion),1,Stream);

	return true;
}

template <class TYPE > 
bool CH3Rect <TYPE>::fLoad(FILE *Stream)
{
	if (!fLoadHeader(Stream))
		return false;

	return fLoadRAW(Stream);
}

template <class TYPE > 
bool CH3Rect <TYPE>::fLoadRAW(FILE *Stream)
{
	if (fread(&left,sizeof(left),1,Stream)!=1)
		return false;
	if (fread(&top,sizeof(top),1,Stream)!=1)
		return false;
	if (fread(&right,sizeof(right),1,Stream)!=1)
		return false;
	if (fread(&bottom,sizeof(bottom),1,Stream)!=1)
		return false;

	return true;
}

template <class TYPE > 
bool CH3Rect <TYPE>::fLoadHeader(FILE *Stream)
{
	if (!Stream)
		return NULL;

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
		return false;
	if (nVersion!=H3_RECT_FILE_VERSION)
		return false;

	return true;
}


//////////////////////////////////////////////////////////////////////////////////////
// Attributs
//////////////////////////////////////////////////////////////////////////////////////
template <class TYPE> 
TYPE CH3Rect<TYPE>::Width()
{
	return right-left+1;
}

template <class TYPE> 
TYPE CH3Rect<TYPE>::Height()
{
	return bottom-top+1;
}

//////////////////////////////////////////////////////////////////////////////////////
// Methodes de conversion de type
//////////////////////////////////////////////////////////////////////////////////////
// Conversion vers H3_INT32
template <class TYPE> 
CH3Rect<TYPE>::operator H3_RECT_INT32()
{
	return H3_RECT_INT32(top,left,bottom,right);
}

// Conversion vers H3_FLT32
template <class TYPE> 
CH3Rect<TYPE>::operator H3_RECT_FLT32()
{
	return H3_RECT_FLT32(top,left,bottom,right);
}

// Conversion vers H3_FLT64
template <class TYPE> 
CH3Rect<TYPE>::operator H3_RECT_FLT64()
{
	return H3_RECT_FLT32(top,left,bottom,right);
}

//////////////////////////////////////////////////////////////////////////////////////
// Attributs
//////////////////////////////////////////////////////////////////////////////////////
template <class TYPE> 
bool CH3Rect<TYPE>::IsEmpty()
{
	if (((bottom-top)==0) || 
		((right-left)==0))
		return true;
	return false;
}

template <class TYPE> 
void CH3Rect<TYPE>::SetEmpty()
{
	SetNull();
}

template <class TYPE> 
bool CH3Rect<TYPE>::IsNull()
{
	if (top==0 && left==0 && 
		bottom==0 && right==0)
		return true;

	return false;
}

template <class TYPE> 
void CH3Rect<TYPE>::SetNull()
{
	top=bottom=left=right=0;
}


/*
// Cette fct modifie les coordonnees des points de sorte que  
// largeur et hauteur soient positives. Une comparaison entre 
// l et r, les valeurs sont echangees si l>r. Le meme processus
// est applique sur t et b
// Le sens positif est donne par la repere ecran
template <class TYPE> 
void CH3Rect<TYPE>::NormalizeRect()
{
	if (top>bottom)
	{
		TYPE tmp=top;
		top=bottom;bottom=tmp;
	}

	if (right<left)
	{
		TYPE tmp=right;
		right=left;left=tmp;
	}
}
*/

// Cree le rectangle représentant l'intersection des rectangles
// rc1 et rc2. Si aucune interection existe alors le rectangle
// cree prend l'etat "vide" et la fonction retourne false.
template <class TYPE> 
bool CH3Rect<TYPE>::IntersectRect(CH3Rect &rc1, CH3Rect &rc2)
{
	if (rc1.IsEmpty() && rc2.IsEmpty())
	{
		SetEmpty();
		return false;
	}

	if (rc1.IsEmpty())
	{
		Copy(rc2);
		return true;
	}

	if (rc2.IsEmpty())
	{
		Copy(rc1);
		return true;
	}

	top=__max(rc1.top,rc2.top);
	bottom=__min(rc1.bottom,rc2.bottom);

	right=__min(rc1.right,rc2.right);
	left=__max(rc1.left,rc2.left);


	if (right<left ||
		top > bottom)
	{
		SetEmpty();
		return false;
	}


	return true;
}

// Makes the dimensions of CH3Rect equal to the union of the two 
// source rectangles. The union is the smallest rectangle that 
// contains both source rectangles. 
template <class TYPE> 
bool CH3Rect<TYPE>::UnionRect(CH3Rect &rc1, CH3Rect &rc2)
{
	if (rc1.IsEmpty() && rc2.IsEmpty())
	{
		SetEmpty();
		return false;
	}

	if (rc1.IsEmpty())
	{
		Copy(rc2);
		return true;
	}

	if (rc2.IsEmpty())
	{
		Copy(rc1);
		return true;
	}

	top=__min(rc1.top,rc2.top);
	bottom=__max(rc1.bottom,rc2.bottom);
	right=__max(rc1.right,rc2.right);
	left=__min(rc1.left,rc2.left);
		
	return true;
}


template <class TYPE> 
CH3Point2D<TYPE> CH3Rect<TYPE>::BottomLeft()
{
	return CH3Point2D<TYPE>(left,bottom);
}

template <class TYPE> 
CH3Point2D<TYPE> CH3Rect<TYPE>::BottomRight()
{
	return CH3Point2D<TYPE>(right,bottom);
}

template <class TYPE> 
CH3Point2D<TYPE> CH3Rect<TYPE>::TopRight()
{
	return CH3Point2D<TYPE>(right,top);
}

template <class TYPE> 
CH3Point2D<TYPE> CH3Rect<TYPE>::TopLeft()
{
	return CH3Point2D<TYPE>(left,top);
}

// Retourne vrai si le point est dans le rectangle
template <class TYPE> 
bool CH3Rect<TYPE>::PtInRect(CH3Point2D<TYPE> pt)
{
	if (pt.x < left || pt.x > right ||
		pt.y < top || pt.y > bottom)
		return false;
	return true;
}

#endif