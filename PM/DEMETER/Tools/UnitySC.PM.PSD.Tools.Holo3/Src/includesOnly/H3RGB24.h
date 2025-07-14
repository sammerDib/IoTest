/// 
///	\file    H3RGB24.h
///	\brief   Implementation de la classe CH3RGB24 et des fonctions associees
///	\version 1.0.6.0
///	\author  E.COLON
///	\date    01/01/2002
///	\remarks 
/// 

#ifndef CH3_RGB24__INCLUDED_
#define CH3_RGB24__INCLUDED_

#define H3_RGB24 CH3RGB24

class CH3RGB24
{
public:
	// Constructeurs et destructeur
	inline H3_RGB24(H3_UINT8 crgb=0){b=g=r=crgb;};
    inline H3_RGB24(H3_UINT8 cr,H3_UINT8 cg,H3_UINT8 cb){r=cr;g=cg;b=cb;};
  	inline H3_RGB24(H3_RGB24 & T) {if (this==&T) return;r=T.r;g=T.g;b=T.b;};	

	inline ~H3_RGB24(){};

	// Operateurs de copie
	inline H3_RGB24 & operator =(H3_RGB24 & T) {if (this==&T) return *this;r=T.r;g=T.g;b=T.b;return *this;};

public:
	H3_UINT8 r,g,b;
};

#endif