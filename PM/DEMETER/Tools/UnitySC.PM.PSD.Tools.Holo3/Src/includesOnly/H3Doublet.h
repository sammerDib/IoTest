/// 
///	\file    H3Doublet.h
///	\brief   Implementation de la classe CH3Doublet et des fonctions associees
///	\version 1.0.6.2
///	\author  E.COLON
///	\date    01/01/2002
///	\remarks 
/// 

// Modifications :
// EC 04/06/2008 - ajout de l'operateur <<


#ifndef CH3_DOUBLET__INCLUDED_
#define CH3_DOUBLET__INCLUDED_
#include <iostream>
#include <stdlib.h>

#pragma warning(disable: 4244)

#define H3_DOUBLET CH3Doublet< TYPE >

template <class TYPE> 
class CH3Doublet
{
public:
	// Constructeurs et destructeur
	H3_DOUBLET(TYPE val=0){x=y=val;};
    H3_DOUBLET(TYPE Coordx,TYPE Coordy){x=Coordx;y=Coordy;};
  
	~CH3Doublet< TYPE >(){};

	// Operateurs de copie
	H3_DOUBLET(const H3_DOUBLET & T);	
	
	// Methodes diverses
	H3_DOUBLET operator +(const double Value)const;
	H3_DOUBLET operator -(const double Value)const;
	H3_DOUBLET operator *(const double Value)const;
	H3_DOUBLET operator /(const double Value)const;

	H3_DOUBLET operator +(const H3_DOUBLET & P)const;
	H3_DOUBLET operator -(const H3_DOUBLET & P)const;
	H3_DOUBLET operator *(const H3_DOUBLET & P)const;
	H3_DOUBLET operator /(const H3_DOUBLET & P)const;
	H3_DOUBLET&operator =(const H3_DOUBLET & P)const;

	void operator +=(const double Value);
	void operator -=(const double Value);
	void operator *=(const double Value);
	void operator /=(const double Value);

	void operator +=(const H3_DOUBLET & T);
	void operator -=(const H3_DOUBLET & T);
	void operator *=(const H3_DOUBLET & T);
	void operator /=(const H3_DOUBLET & T);

	TYPE Sum()const{ return (x+y); }
	void rand();

	// Operateurs
	bool IsNull()const;
	bool IsNaN()const;

	bool SaveASCII(const CString& filename)const;

	friend ostream & operator<< ( ostream & os, H3_DOUBLET & P );

public:
	TYPE x,y;
};

template <class TYPE>
ostream& operator<< ( ostream& os, H3_DOUBLET & P )
{
   os << P.x << ' ' << P.y;
   return os;
}

template <class TYPE> 
H3_DOUBLET::CH3Doublet(const H3_DOUBLET &T)
{
	if (this==&T) return;

	x=T.x;
	y=T.y;
}
////////////////////////////////////////

template <class TYPE> 
void H3_DOUBLET::operator+=(const H3_DOUBLET & T)
{
	x+=T.x;
	y+=T.y;
}

template <class TYPE> 
void H3_DOUBLET::operator-=(const H3_DOUBLET & T)
{
	x-=T.x;
	y-=T.y;
}

template <class TYPE> 
void H3_DOUBLET::operator*=(const H3_DOUBLET & T)
{
	x*=T.x;
	y*=T.y;
}

template <class TYPE> 
void H3_DOUBLET::operator/=(const H3_DOUBLET & T)
{
	x/=T.x;
	y/=T.y;
}

////////////////////////////////////////
template <class TYPE> 
void H3_DOUBLET::operator +=(const double value)
{
	x+=value;
	y+=value;
}

template <class TYPE> 
void H3_DOUBLET::operator -=(const double value)
{
	x-=value;
	y-=value;
}

template <class TYPE> 
void H3_DOUBLET::operator *=(const double value)
{
	x*=value;
	y*=value;
}

template <class TYPE> 
void H3_DOUBLET::operator /=(const double value)
{
	x/=value;
	y/=value;
}

//////////////////////////////////////////////////////////////////////////////////////
// Methodes et operateur arithmetiques avec un scalaire
//////////////////////////////////////////////////////////////////////////////////////
template <class TYPE> 
H3_DOUBLET H3_DOUBLET::operator +(const double value)const
{
	H3_DOUBLET Tmp(*this);
	Tmp+=value;
	return Tmp;
}

template <class TYPE> 
H3_DOUBLET H3_DOUBLET::operator -(const double value)const
{
	H3_DOUBLET Tmp(*this);
	Tmp-=value;
	return Tmp;
}

template <class TYPE> 
H3_DOUBLET H3_DOUBLET::operator *(const double value)const
{
	H3_DOUBLET Tmp(*this);
	Tmp*=value;
	return Tmp;
}

template <class TYPE> 
H3_DOUBLET H3_DOUBLET::operator /(const double value)const
{
	H3_DOUBLET Tmp(*this);
	Tmp/=value;
	return Tmp;
}
////////////////////////////////////////////
template <class TYPE> 
H3_DOUBLET H3_DOUBLET::operator +(const H3_DOUBLET & T)const
{
	H3_DOUBLET Tmp(*this);
	Tmp+=T;
	return Tmp;
}

template <class TYPE> 
H3_DOUBLET H3_DOUBLET::operator -(const H3_DOUBLET & T)const
{
	H3_DOUBLET Tmp(*this);
	Tmp-=T;
	return Tmp;
}

template <class TYPE> 
H3_DOUBLET H3_DOUBLET::operator *(const H3_DOUBLET & T)const
{
	H3_DOUBLET Tmp(*this);
	Tmp*=T;
	return Tmp;
}

template <class TYPE> 
H3_DOUBLET H3_DOUBLET::operator /(const H3_DOUBLET & T)const
{
	H3_DOUBLET Tmp(*this);
	Tmp/=T;
	return Tmp;
}

//////////////////////////////////////////////////////////////////////////////////////
// Attributs
//////////////////////////////////////////////////////////////////////////////////////
// Retourne vrai si le point est nul
template <class TYPE> 
bool H3_DOUBLET::IsNull()const
{
	if (x==0 && y==0)
		return true;

	return false;
}

// Retourne vrai si le point est NaN
template <class TYPE> 
bool H3_DOUBLET::IsNaN()const
{
	if (_isnan(x+y))
		return true;

	return false;
}



template <class TYPE>
bool H3_DOUBLET::SaveASCII(const CString& filename)const
{
    CreateFolderForFile(filename);
    FILE *Stream=fopen(filename,"wt");
	if (!Stream) return false;

	fprintf(Stream,"%.10g %.10g",x,y);
	fclose(Stream);
	return true;
}

template <class TYPE> 
void H3_DOUBLET::rand()
{
	x=::rand();
	y=::rand();
}

#endif