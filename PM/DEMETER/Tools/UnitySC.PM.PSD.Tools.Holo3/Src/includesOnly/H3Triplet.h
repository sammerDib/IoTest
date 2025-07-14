/// 
///	\file    H3Triplet.h
///	\brief   Implementation de la classe CH3Triplet et des fonctions associees
///	\version 1.0.6.2
///	\author  V.CHALVIDAN
///	\date    01/01/2002
///	\remarks 
/// 

// Modifications :
// EC 04/06/2008 - ajout operateur <<
//               - ajout fonction rand()


#ifndef CH3_TRIPLET__INCLUDED_
#define CH3_TRIPLET__INCLUDED_


#define H3_TRIPLET CH3Triplet< TYPE >

template <class TYPE> 
class CH3Triplet
{
public:
	// Constructeurs et destructeur
	H3_TRIPLET(TYPE val=0){x=y=z=val;};
    H3_TRIPLET(TYPE Coordx,TYPE Coordy,TYPE Coordz){x=Coordx;y=Coordy;z=Coordz;};
  
	~CH3Triplet< TYPE >(){};

	// Operateurs de copie
	H3_TRIPLET(const H3_TRIPLET & T);	
	
	// Methodes diverses
	H3_TRIPLET operator +(const TYPE Value)const;
	H3_TRIPLET operator -(const TYPE Value)const;
	H3_TRIPLET operator *(const TYPE Value)const;
	H3_TRIPLET operator /(const TYPE Value)const;

	H3_TRIPLET operator +(const H3_TRIPLET & P)const;
	H3_TRIPLET operator -(const H3_TRIPLET & P)const;
	H3_TRIPLET operator *(const H3_TRIPLET & P)const;
	H3_TRIPLET operator /(const H3_TRIPLET & P)const;
	H3_TRIPLET&operator =(const H3_TRIPLET & P)const;

	void operator +=(const TYPE Value);
	void operator -=(const TYPE Value);
	void operator *=(const TYPE Value);
	void operator /=(const TYPE Value);

	void operator +=(const H3_TRIPLET & T);
	void operator -=(const H3_TRIPLET & T);
	void operator *=(const H3_TRIPLET & T);
	void operator /=(const H3_TRIPLET & T);

	TYPE Sum() const{ return (x+y+z); }


	// Operateurs
	bool IsNull()const;
	bool IsNaN()const;
	void rand();

	friend ostream& operator<< ( iostream& os, H3_TRIPLET & T );

public:
	TYPE x,y,z;
};

template <class TYPE>
ostream& operator<< ( ostream& os, H3_TRIPLET & T )
{
   os << T.x << ' ' << T.y << ' ' << T.z;
   return os;
}

template <class TYPE> 
H3_TRIPLET::CH3Triplet(const H3_TRIPLET &T)
{
	if (this==&T) return;

	x=T.x;
	y=T.y;
	z=T.z;
}
////////////////////////////////////////

template <class TYPE> 
void H3_TRIPLET::operator+=(const H3_TRIPLET & T)
{
	x+=T.x;
	y+=T.y;
	z+=T.z;
}
template <class TYPE> 
void H3_TRIPLET::operator-=(const H3_TRIPLET & T)
{
	x-=T.x;
	y-=T.y;
	z-=T.z;
}

template <class TYPE> 
void H3_TRIPLET::operator*=(const H3_TRIPLET & T)
{
	x*=T.x;
	y*=T.y;
	z*=T.z;
}

template <class TYPE> 
void H3_TRIPLET::operator/=(const H3_TRIPLET & T)
{
	x/=T.x;
	y/=T.y;
	z/=T.z;
}

////////////////////////////////////////
template <class TYPE> 
void H3_TRIPLET::operator +=(const TYPE value)
{
	x+=value;
	y+=value;
	z+=value;
}

template <class TYPE> 
void H3_TRIPLET::operator -=(const TYPE value)
{
	x-=value;
	y-=value;
	z-=value;
}

template <class TYPE> 
void H3_TRIPLET::operator *=(const TYPE value)
{
	x*=value;
	y*=value;
	z*=value;
}

template <class TYPE> 
void H3_TRIPLET::operator /=(const TYPE value)
{
	x/=value;
	y/=value;
	z/=value;
}

//////////////////////////////////////////////////////////////////////////////////////
// Methodes et operateur arithmetiques avec un scalaire
//////////////////////////////////////////////////////////////////////////////////////
template <class TYPE> 
H3_TRIPLET & H3_TRIPLET::operator=(const H3_TRIPLET &T)const
{
	if (this==&T)
		return (*this);

	x=T.x;
	y=T.y;
	z=T.z;

	return (*this);
}
////////////////////////////////////////

template <class TYPE> 
H3_TRIPLET H3_TRIPLET::operator +(const TYPE value)const
{
	H3_TRIPLET Tmp(*this);
	Tmp+=value;
	return Tmp;
}

template <class TYPE> 
H3_TRIPLET H3_TRIPLET::operator -(const TYPE value)const
{
	H3_TRIPLET Tmp(*this);
	Tmp-=value;
	return Tmp;
}

template <class TYPE> 
H3_TRIPLET H3_TRIPLET::operator *(const TYPE value)const
{
	H3_TRIPLET Tmp(*this);
	Tmp*=value;
	return Tmp;
}

template <class TYPE> 
H3_TRIPLET H3_TRIPLET::operator /(const TYPE value)const
{
	H3_TRIPLET Tmp(*this);
	Tmp/=value;
	return Tmp;
}
////////////////////////////////////////////
template <class TYPE> 
H3_TRIPLET H3_TRIPLET::operator +(const H3_TRIPLET & T)const
{
	H3_TRIPLET Tmp(*this);
	Tmp+=T;
	return Tmp;
}

template <class TYPE> 
H3_TRIPLET H3_TRIPLET::operator -(const H3_TRIPLET & T)const
{
	H3_TRIPLET Tmp(*this);
	Tmp-=T;
	return Tmp;
}

template <class TYPE> 
H3_TRIPLET H3_TRIPLET::operator *(const H3_TRIPLET & T)const
{
	H3_TRIPLET Tmp(*this);
	Tmp*=T;
	return Tmp;
}

template <class TYPE> 
H3_TRIPLET H3_TRIPLET::operator /(const H3_TRIPLET & T)const
{
	H3_TRIPLET Tmp(*this);
	Tmp/=T;
	return Tmp;
}

///////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////
// Attributs
//////////////////////////////////////////////////////////////////////////////////////
// Retourne vrai si le point est nul
template <class TYPE> 
bool H3_TRIPLET::IsNull()const
{
	if (x==0 && y==0 && z==0)
		return true;

	return false;
}

// Retourne vrai si le point est NaN
template <class TYPE> 
bool H3_TRIPLET::IsNaN()const
{
	//if (_isnan(x) || _isnan(y) || _isnan(z))//cv250308
	if (_isnan(x+y+z))
		return true;

	return false;
}

template <class TYPE> 
void H3_TRIPLET::rand()
{
	x=::rand();
	y=::rand();
	z=::rand();
}


#endif