// H3Complex standard header

// E.COLON 07/09/04
// Classe definissant les nombres complexes, cette classe est absolument identique
// à la classe complex définie en standard dans MSVC6 créée par P.J. Plauger.
// La seule difference réside dans les parties Re et im qui sont de type public et
// non protected.
// E.COLON 22/06/07 
// Ajout directives pour rendre la classe compatible avec VC7 et VC8. La compatibilite
// VC6 est conservee.

#if     _MSC_VER > 1000
#pragma once
#endif

#ifndef CCH3Complex__INCLUDED_
#define CCH3Complex__INCLUDED_

#include <ymath.h>
#include <cmath>
#include <sstream>
#include <xutility>

#ifdef  _MSC_VER
#pragma pack(push,8)
#endif  /* _MSC_VER */

#if _MSC_VER > 1200			// Déclarations à changer pour les versions > VC6
	#define _H3COMPLEX_DECL	template<> class
	#define _H3COMPLEX_LD	_Long_double
	#define _H3COMPLEX_D	_Double
	#define _H3COMPLEX_F	_Float
#else
	#define _H3COMPLEX_DECL	class _CRTIMP
	#define _H3COMPLEX_LD	_L
	#define _H3COMPLEX_D	_D
	#define _H3COMPLEX_F	_F
#endif

_STD_BEGIN
//#define __STD_COMPLEX
// TEMPLATE CLASS _H3Ctr
template<class _Ty> class _H3Ctr 
{
public:
	static _Ty _Cosh(_Ty _X, _Ty _Y)
		{return (::_Cosh((double)_X, (double)_Y)); }
	static short _Exp(_Ty *_P, _Ty _Y, short _E)
		{double _W = (double)*_P;
		short _Ans = ::_Exp(&_W, (double)_Y, _E);
		*_P = (_Ty)_W;
		return (_Ans); }
	static _Ty _Infv(_Ty)
		{return (_Inf._D); }
	static bool _Isinf(_Ty _X)
		{double _W = (double)_X;
		return (_Dtest(&_W) == _INFCODE); }
	static bool _Isnan(_Ty _X)
		{double _W = (double)_X;
		return (_Dtest(&_W) == _NANCODE); }
	static _Ty _Nanv(_Ty)
		{return (_Nan._D); }
	static _Ty _Sinh(_Ty _X, _Ty _Y)
		{return (::_Sinh((double)_X, (double)_Y)); }
	static _Ty atan2(_Ty _Y, _Ty _X)
		{return (::atan2((double)_Y, (double)_X)); }
	static _Ty cos(_Ty _X)
		{return (::cos((double)_X)); }
	static _Ty exp(_Ty _X)
		{return (::exp((double)_X)); }
	static _Ty ldexp(_Ty _R, int _E)
		{return (::ldexp((double)_R, _E)); }
	static _Ty log(_Ty _X)
		{return (::log((double)_X)); }
	static _Ty pow(_Ty _X, _Ty _Y)
		{return (::pow((double)_X, (double)_Y)); }
	static _Ty sin(_Ty _X)
		{return (::sin((double)_X)); }
	static _Ty sqrt(_Ty _X)
		{return (::sqrt((double)_X)); }
	};
		// CLASS _H3Ctr<long double>
_H3COMPLEX_DECL _H3Ctr<long double> {
public:
	typedef long double _Ty;
	static _Ty _Cosh(_Ty _X, _Ty _Y)
		{return (_LCosh(_X, _Y)); }
	static short _Exp(_Ty *_P, _Ty _Y, short _E)
		{return (_LExp(_P, _Y, _E)); }
	static _Ty _Infv(_Ty)
		{return (_LInf._H3COMPLEX_LD); }
	static bool _Isinf(_Ty _X)
		{return (_LDtest(&_X) == _INFCODE); }
	static bool _Isnan(_Ty _X)
		{return (_LDtest(&_X) == _NANCODE); }
	static _Ty _Nanv(_Ty)
		{return (_LNan._H3COMPLEX_LD); }
	static _Ty _Sinh(_Ty _X, _Ty _Y)
		{return (_LSinh(_X, _Y)); }
	static _Ty atan2(_Ty _Y, _Ty _X)
		{return (atan2l(_Y, _X)); }
	static _Ty cos(_Ty _X)
		{return (cosl(_X)); }
	static _Ty exp(_Ty _X)
		{return (expl(_X)); }
	static _Ty ldexp(_Ty _R, int _E)
		{return (ldexpl(_R, _E)); }
	static _Ty log(_Ty _X)
		{return (logl(_X)); }
	static _Ty pow(_Ty _X, _Ty _Y)
		{return (powl(_X, _Y)); }
	static _Ty sin(_Ty _X)
		{return (sinl(_X)); }
	static _Ty sqrt(_Ty _X)
		{return (sqrtl(_X)); }
	};
		// CLASS _H3Ctr<double>
_H3COMPLEX_DECL _H3Ctr<double> {
public:
	typedef double _Ty;
	static _Ty _Cosh(_Ty _X, _Ty _Y)
		{return (::_Cosh(_X, _Y)); }
	static short _Exp(_Ty *_P, _Ty _Y, short _E)
		{return (::_Exp(_P, _Y, _E)); }
	static _Ty _Infv(_Ty)
		{return (_Inf._H3COMPLEX_D); }
	static bool _Isinf(_Ty _X)
		{return (_Dtest(&_X) == _INFCODE); }
	static bool _Isnan(_Ty _X)
		{return (_Dtest(&_X) == _NANCODE); }
	static _Ty _Nanv(_Ty)
		{return (_Nan._H3COMPLEX_D); }
	static _Ty _Sinh(_Ty _X, _Ty _Y)
		{return (::_Sinh(_X, _Y)); }
	static _Ty atan2(_Ty _Y, _Ty _X)
		{return (::atan2(_Y, _X)); }
	static _Ty cos(_Ty _X)
		{return (::cos(_X)); }
	static _Ty exp(_Ty _X)
		{return (::exp(_X)); }
	static _Ty ldexp(_Ty _R, int _E)
		{return (::ldexp(_R, _E)); }
	static _Ty log(_Ty _X)
		{return (::log(_X)); }
	static _Ty pow(_Ty _X, _Ty _Y)
		{return (::pow(_X, _Y)); }
	static _Ty sin(_Ty _X)
		{return (::sin(_X)); }
	static _Ty sqrt(_Ty _X)
		{return (::sqrt(_X)); }
	};

_H3COMPLEX_DECL _H3Ctr<float> {
public:
	typedef float _Ty;
	static _Ty _Cosh(_Ty _X, _Ty _Y)
		{return (_FCosh(_X, _Y)); }
	static short _Exp(_Ty *_P, _Ty _Y, short _E)
		{return (_FExp(_P, _Y, _E)); }
	static _Ty _Infv(_Ty)
		{return (_FInf._H3COMPLEX_F); }
	static bool _Isinf(_Ty _X)
		{return (_FDtest(&_X) == _INFCODE); }
	static bool _Isnan(_Ty _X)
		{return (_FDtest(&_X) == _NANCODE); }
	static _Ty _Nanv(_Ty)
		{return (_FNan._H3COMPLEX_F); }
	static _Ty _Sinh(_Ty _X, _Ty _Y)
		{return (_FSinh(_X, _Y)); }
	static _Ty atan2(_Ty _Y, _Ty _X)
		{return (atan2f(_Y, _X)); }
	static _Ty cos(_Ty _X)
		{return (cosf(_X)); }
	static _Ty exp(_Ty _X)
		{return (expf(_X)); }
	static _Ty ldexp(_Ty _R, int _E)
		{return (ldexpf(_R, _E)); }
	static _Ty log(_Ty _X)
		{return (logf(_X)); }
	static _Ty pow(_Ty _X, _Ty _Y)
		{return (powf(_X, _Y)); }
	static _Ty sin(_Ty _X)
		{return (sinf(_X)); }
	static _Ty sqrt(_Ty _X)
		{return (sqrtf(_X)); }
	};
		// TEMPLATE CLASS _Complex_base

// TEMPLATE CLASS _CH3Complex_base
template<class _Ty> class CH3Complex;
//class CH3Complex<float>;
//class CH3Complex<double>;
//class CH3Complex<long double>;


template<class _Ty>
	class CH3Complex_base {
public:
	typedef CH3Complex_base<_Ty> _Myt;
	typedef _H3Ctr<_Ty> _Myctr;
	typedef _Ty value_type;
	CH3Complex_base(const _Ty& _R, const _Ty& _I)
		: re(_R), im(_I) {}
	_Myt& operator+=(const _Ty& _X)
		{re = re + _X;
		return (*this); }
	_Myt& operator-=(const _Ty& _X)
		{re = re - _X;
		return (*this); }
	_Myt& operator*=(const _Ty& _X)
		{re = re * _X;
		im = im * _X;
		return (*this); }
	_Myt& operator/=(const _Ty& _X)
		{re = re / _X;
		im = im / _X;
		return (*this); }
	_Ty real(const _Ty& _X)
		{return (re = _X); }
	_Ty imag(const _Ty& _X)
		{return (im = _X); }
	_Ty real() const
		{return (re); }
	_Ty imag() const
		{return (im); }
public:
	_Ty re, im;
	};

// CLASS CH3Complex<float>
template<> class CH3Complex<float> : public CH3Complex_base<float> {
//class _CRTIMP CH3Complex<float> : public CH3Complex_base<float> {
public:
	typedef float _Ty;
	explicit CH3Complex(const CH3Complex<double>&);
	explicit CH3Complex(const CH3Complex<long double>&);
	CH3Complex(const _Ty& _R = 0, const _Ty& _I = 0)
		: CH3Complex_base<_Ty>(_R, _I) {}
	CH3Complex<_Ty>& operator=(const _Ty& _X)
		{re = _X;
		im = 0;
		return (*this); }
	};
		// CLASS complex<double>
template<> class CH3Complex<double> : public CH3Complex_base<double> {
//class _CRTIMP CH3Complex<double> : public CH3Complex_base<double> {
public:
	typedef double _Ty;
	CH3Complex(const CH3Complex<float>&);
	explicit CH3Complex(const CH3Complex<long double>&);
	CH3Complex(const _Ty& _R = 0, const _Ty& _I = 0)
		: CH3Complex_base<_Ty>(_R, _I) {}
	CH3Complex<_Ty>& operator=(const _Ty& _X)
		{re = _X;
		im = 0;
		return (*this); }
	};
		// CLASS complex<long double>
template<> class CH3Complex<long double> : public CH3Complex_base<long double> {
//class _CRTIMP CH3Complex<long double> : public CH3Complex_base<long double> {
public:
	typedef long double _Ty;
	CH3Complex(const CH3Complex<float>&);
	CH3Complex(const CH3Complex<double>&);
	CH3Complex(const _Ty& _R = 0, const _Ty& _I = 0)
		: CH3Complex_base<_Ty>(_R, _I) {}
	CH3Complex<_Ty>& operator=(const _Ty& _X)
		{re = _X;
		im = 0;
		return (*this); }
	};
		// CONSTRUCTORS FOR complex SPECIALIZATIONS
inline CH3Complex<float>::CH3Complex(const CH3Complex<double>& _X)
	: CH3Complex_base<float>((_Ty)_X.real(), (_Ty)_X.imag()) {}
inline CH3Complex<float>::CH3Complex(const CH3Complex<long double>& _X)
	: CH3Complex_base<float>((_Ty)_X.real(), (_Ty)_X.imag()) {}
inline CH3Complex<double>::CH3Complex(const CH3Complex<float>& _X)
	: CH3Complex_base<double>((_Ty)_X.real(), (_Ty)_X.imag()) {}
inline CH3Complex<double>::CH3Complex(const CH3Complex<long double>& _X)
	: CH3Complex_base<double>((_Ty)_X.real(), (_Ty)_X.imag()) {}
inline CH3Complex<long double>::CH3Complex(const CH3Complex<float>& _X)
	: CH3Complex_base<long double>((_Ty)_X.real(), (_Ty)_X.imag()) {}
inline CH3Complex<long double>::CH3Complex(const CH3Complex<double>& _X)
	: CH3Complex_base<long double>((_Ty)_X.real(), (_Ty)_X.imag()) {}
		// TEMPLATE CLASS complex
template<class _Ty>
	class CH3Complex : public CH3Complex_base<_Ty> {
public:
	CH3Complex(const _Ty& _R = 0, const _Ty& _I = 0)
		: CH3Complex_base<_Ty>(_R, _I) {}
	typedef _Ty _U;
	CH3Complex(const CH3Complex<_U>& _X)
		: CH3Complex_base<_Ty>((_Ty)_X.real(), (_Ty)_X.imag()) {}
	CH3Complex<_Ty>& operator=(const CH3Complex<_U>& _X)
		{re = (_Ty)_X.real();
		im = (_Ty)_X.imag();
		return (*this); }
	};
		// TEMPLATE complex OPERATORS
template<class _Ty, class _U> inline
	CH3Complex<_Ty>& __cdecl operator+=(
	CH3Complex<_Ty>& _X,
	const CH3Complex<_U>& _Y)
	{_X.real(_X.real() + (_Ty)_Y.real());
	_X.imag(_X.imag() + (_Ty)_Y.imag());
	return (_X); }
template<class _Ty, class _U> inline
	 CH3Complex<_Ty>& __cdecl operator-=(
	CH3Complex<_Ty>& _X,
	const CH3Complex<_U>& _Y)
	{_X.real(_X.real() - (_Ty)_Y.real());
	_X.imag(_X.imag() - (_Ty)_Y.imag());
	return (_X); }
template<class _Ty, class _U> inline
	 CH3Complex<_Ty>& __cdecl operator*=(
	CH3Complex<_Ty>& _X,
	const CH3Complex<_U>& _Y)
	{_Ty _Yre = (_Ty)_Y.real();
	_Ty _Yim = (_Ty)_Y.imag();
	_Ty _W = _X.real() * _Yre - _X.imag() * _Yim;
	_X.imag(_X.real() * _Yim + _X.imag() * _Yre);
	_X.real(_W);
	return (_X); }
template<class _Ty, class _U> inline
	 CH3Complex<_Ty>& __cdecl operator/=(
	CH3Complex<_Ty>& _X,
	const CH3Complex<_U>& _Y)
	{typedef _H3Ctr<_Ty> _Myctr;
	_Ty _Yre = (_Ty)_Y.real();
	_Ty _Yim = (_Ty)_Y.imag();
	if (_Myctr::_Isnan(_Yre) || _Myctr::_Isnan(_Yim))
		_X.real(_Myctr::_Nanv(_Yre)), _X.imag(_X.real());
	else if ((_Yim < 0 ? -_Yim : +_Yim)
		< (_Yre < 0 ? -_Yre : +_Yre))
		{_Ty _Wr = _Yim / _Yre;
		_Ty _Wd = _Yre + _Wr * _Yim;
		if (_Myctr::_Isnan(_Wd) || _Wd == 0)
			_X.real(_Myctr::_Nanv(_Yre)), _X.imag(_X.real());
		else
			{_Ty _W = (_X.real() + _X.imag() * _Wr) / _Wd;
			_X.imag((_X.imag() - _X.real() * _Wr) / _Wd);
			_X.real(_W); }}
	else if (_Yim == 0)
		_X.real(_Myctr::_Nanv(_Yre)), _X.imag(_X.real());
	else
		{_Ty _Wr = _Yre / _Yim;
		_Ty _Wd = _Yim + _Wr * _Yre;
		if (_Myctr::_Isnan(_Wd) || _Wd == 0)
			_X.real(_Myctr::_Nanv(_Yre)), _X.imag(_X.real());
		else
			{_Ty _W = (_X.real() * _Wr + _X.imag()) / _Wd;
			_X.imag((_X.imag() * _Wr - _X.real()) / _Wd);
			_X.real(_W); }}
	return (_X); }
		// TEMPLATE FUNCTION operator>>
template<class _E, class _Tr, class _U> inline
	basic_istream<_E, _Tr>& __cdecl operator>>(
		basic_istream<_E, _Tr>& _I, CH3Complex<_U>& _X)
	{typedef CH3Complex<_U> _Myt;
	_E _Ch;
	long double re, im;
	if (_I >> _Ch && _Ch != '(')
		_I.putback(_Ch), _I >> re, im = 0;
	else if (_I >> re >> _Ch && _Ch != ',')
		if (_Ch == ')')
			im = 0;
		else
			_I.putback(_Ch), _I.setstate(ios_base::failbit);
	else if (_I >> im >> _Ch && _Ch != ')')
			_I.putback(_Ch), _I.setstate(ios_base::failbit);
	if (!_I.fail())
		_X = _Myt((_U)re, (_U)im);
	return (_I); }
		// TEMPLATE FUNCTION operator<<
template<class _E, class _Tr, class _U> inline
	basic_ostream<_E, _Tr>& __cdecl operator<<(
		basic_ostream<_E, _Tr>& _O, const CH3Complex<_U>& _X)
	{basic_ostringstream<_E, _Tr, allocator<_E> > _S;
	_S.flags(_O.flags());
	_S.imbue(_O.getloc());
	_S.precision(_O.precision());
	_S << '(' << real(_X) << ',' << imag(_X) << ')';
	return (_O << _S.str().c_str()); }

 #define _H3CMPLX(T)	CH3Complex<T >
 #define _H3CTR(T)		_H3Ctr<T >
 #define _TMPLT(T)	template<class T >

////////////
		// TEMPLATE FUNCTION imag
_TMPLT(_Ty) inline
	_Ty __cdecl imag(const _H3CMPLX(_Ty)& _X)
	{return (_X.imag()); }
		// TEMPLATE FUNCTION real
_TMPLT(_Ty) inline
	_Ty __cdecl real(const _H3CMPLX(_Ty)& _X)
	{return (_X.real()); }
		// TEMPLATE FUNCTION _Fabs
_TMPLT(_Ty) inline
	_Ty __cdecl _Fabs(const _H3CMPLX(_Ty)& _X, int *_Pexp)
	{*_Pexp = 0;
	_Ty _A = real(_X);
	_Ty _B = imag(_X);
	if (_H3CTR(_Ty)::_Isnan(_A))
		return (_A);
	else if (_H3CTR(_Ty)::_Isnan(_B))
		return (_B);
	else
		{if (_A < 0)
			_A = -_A;
		if (_B < 0)
			_B = -_B;
		if (_A < _B)
			{_Ty _W = _A;
			_A = _B, _B = _W; }
		if (_A == 0 || _H3CTR(_Ty)::_Isinf(_A))
			return (_A);
		if (1 <= _A)
			*_Pexp = 2, _A = _A * 0.25, _B = _B * 0.25;
		else
			*_Pexp = -2, _A = _A * 4, _B = _B * 4;
		_Ty _W = _A - _B;
		if (_W == _A)
			return (_A);
		else if (_B < _W)
			{const _Ty _Q = _A / _B;
			return (_A + _B
				/ (_Q + _H3CTR(_Ty)::sqrt(_Q * _Q + 1))); }
		else
			{static const _Ty _R2 = (const _Ty)1.4142135623730950488L;
			static const _Ty _Xh = (const _Ty)2.4142L;
			static const _Ty _Xl = (const _Ty)0.0000135623730950488016887L;
			const _Ty _Q = _W / _B;
			const _Ty _R = (_Q + 2) * _Q;
			const _Ty _S = _R / (_R2 + _H3CTR(_Ty)::sqrt(_R + 2))
				+ _Xl + _Q + _Xh;
			return (_A + _B / _S); }}}
		// TEMPLATE FUNCTION operator+
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl operator+(const _H3CMPLX(_Ty)& _L,
		const _H3CMPLX(_Ty)& _R)
	{_H3CMPLX(_Ty) _W(_L);
	return (_W += _R); }
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl operator+(const _H3CMPLX(_Ty)& _L, const _Ty& _R)
	{_H3CMPLX(_Ty) _W(_L);
	_W.real(_W.real() + _R);
	return (_W); }
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl operator+(const _Ty& _L, const _H3CMPLX(_Ty)& _R)
	{_H3CMPLX(_Ty) _W(_L);
	return (_W += _R); }
		// TEMPLATE FUNCTION operator-
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl operator-(const _H3CMPLX(_Ty)& _L,
		const _H3CMPLX(_Ty)& _R)
	{_H3CMPLX(_Ty) _W(_L);
	return (_W -= _R); }
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl operator-(const _H3CMPLX(_Ty)& _L, const _Ty& _R)
	{_H3CMPLX(_Ty) _W(_L);
	_W.real(_W.real() - _R);
	return (_W); }
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl operator-(const _Ty& _L, const _H3CMPLX(_Ty)& _R)
	{_H3CMPLX(_Ty) _W(_L);
	return (_W -= _R); }
		// TEMPLATE FUNCTION operator*
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl operator*(const _H3CMPLX(_Ty)& _L,
		const _H3CMPLX(_Ty)& _R)
	{_H3CMPLX(_Ty) _W(_L);
	return (_W *= _R); }
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl operator*(const _H3CMPLX(_Ty)& _L, const _Ty& _R)
	{_H3CMPLX(_Ty) _W(_L);
	_W.real(_W.real() * _R);
	_W.imag(_W.imag() * _R);
	return (_W); }
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl operator*(const _Ty& _L, const _H3CMPLX(_Ty)& _R)
	{_H3CMPLX(_Ty) _W(_L);
	return (_W *= _R); }
		// TEMPLATE FUNCTION operator/
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl operator/(const _H3CMPLX(_Ty)& _L,
		const _H3CMPLX(_Ty)& _R)
	{_H3CMPLX(_Ty) _W(_L);
	return (_W /= _R); }
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl operator/(const _H3CMPLX(_Ty)& _L, const _Ty& _R)
	{_H3CMPLX(_Ty) _W(_L);
	_W.real(_W.real() / _R);
	_W.imag(_W.imag() / _R);
	return (_W); }
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl operator/(const _Ty& _L, const _H3CMPLX(_Ty)& _R)
	{_H3CMPLX(_Ty) _W(_L);
	return (_W /= _R); }
		// TEMPLATE FUNCTION UNARY operator+
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl operator+(const _H3CMPLX(_Ty)& _L)
	{return (_H3CMPLX(_Ty)(_L)); }
		// TEMPLATE FUNCTION UNARY operator-
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl operator-(const _H3CMPLX(_Ty)& _L)
	{return (_H3CMPLX(_Ty)(-real(_L), -imag(_L))); }
		// TEMPLATE FUNCTION operator==
_TMPLT(_Ty) inline
	bool __cdecl operator==(const _H3CMPLX(_Ty)& _L, const _H3CMPLX(_Ty)& _R)
	{return (real(_L) == real(_R) && imag(_L) == imag(_R)); }
_TMPLT(_Ty) inline
	bool __cdecl operator==(const _H3CMPLX(_Ty)& _L, const _Ty& _R)
	{return (real(_L) == _R && imag(_L) == 0); }
_TMPLT(_Ty) inline
	bool __cdecl operator==(const _Ty& _L, const _H3CMPLX(_Ty)& _R)
	{return (_L == real(_R) && 0 == imag(_R)); }
_TMPLT(_Ty) inline
	bool __cdecl operator!=(const _H3CMPLX(_Ty)& _L, const _H3CMPLX(_Ty)& _R)
	{return (!(_L == _R)); }
_TMPLT(_Ty) inline
	bool __cdecl operator!=(const _H3CMPLX(_Ty)& _L, const _Ty& _R)
	{return (!(_L == _R)); }
_TMPLT(_Ty) inline
	bool __cdecl operator!=(const _Ty& _L, const _H3CMPLX(_Ty)& _R)
	{return (!(_L == _R)); }
		// TEMPLATE FUNCTION abs
_TMPLT(_Ty) inline
	_Ty __cdecl abs(const _H3CMPLX(_Ty)& _X)
	{int _Xexp;
	_Ty _Rho = _Fabs(_X, &_Xexp);
	if (_Xexp == 0)
		return (_Rho);
	else
		return (_H3CTR(_Ty)::ldexp(_Rho, _Xexp)); }
		// TEMPLATE FUNCTION arg
_TMPLT(_Ty) inline
	_Ty __cdecl arg(const _H3CMPLX(_Ty)& _X)
	{return (_H3CTR(_Ty)::atan2(imag(_X), real(_X))); }
		// TEMPLATE FUNCTION conjg
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl conj(const _H3CMPLX(_Ty)& _X)
	{return (_H3CMPLX(_Ty)(real(_X), -imag(_X))); }
		// TEMPLATE FUNCTION cos
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl cos(const _H3CMPLX(_Ty)& _X)
	{return (_H3CMPLX(_Ty)(
		_H3CTR(_Ty)::_Cosh(imag(_X), _CTR(_Ty)::cos(real(_X))),
		-_H3CTR(_Ty)::_Sinh(imag(_X),
			_H3CTR(_Ty)::sin(real(_X))))); }
		// TEMPLATE FUNCTION cosh
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl cosh(const _H3CMPLX(_Ty)& _X)
	{return (_H3CMPLX(_Ty)(
		_H3CTR(_Ty)::_Cosh(real(_X), _H3CTR(_Ty)::cos(imag(_X))),
		_H3CTR(_Ty)::_Sinh(real(_X), _H3CTR(_Ty)::sin(imag(_X))))); }
		// TEMPLATE FUNCTION exp
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl exp(const _H3CMPLX(_Ty)& _X)
	{_Ty re(real(_X)), im(real(_X));
	_H3CTR(_Ty)::_Exp(&re, _H3CTR(_Ty)::cos(imag(_X)), 0);
	_H3CTR(_Ty)::_Exp(&im, _H3CTR(_Ty)::sin(imag(_X)), 0);
	return (_H3CMPLX(_Ty)(re, im)); }
		// TEMPLATE FUNCTION log
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl log(const _H3CMPLX(_Ty)& _X)
	{int _Xexp;
	_Ty _Rho = _Fabs(_X, &_Xexp);
	if (_CTR(_Ty)::_Isnan(_Rho))
		return (_H3CMPLX(_Ty)(_Rho, _Rho));
	else
		{static const _Ty _Cm = 22713.0 / 32768.0;
		static const _Ty _Cl = (const _Ty)1.428606820309417232e-6L;
		_Ty _Xn = _Xexp;
		_H3CMPLX(_Ty) _W(_Rho == 0 ? -_CTR(_Ty)::_Infv(_Rho)
			: _CTR(_Ty)::_Isinf(_Rho) ? _Rho
			: _CTR(_Ty)::log(_Rho) + _Xn * _Cl + _Xn * _Cm,
				_CTR(_Ty)::atan2(imag(_X), real(_X)));
		return (_W); }}
		// TEMPLATE FUNCTION log10
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl log10(const _H3CMPLX(_Ty)& _X)
	{return (log(_X) * (_Ty)0.4342944819032518276511289L); }
		// TEMPLATE FUNCTION norm
_TMPLT(_Ty) inline
	_Ty __cdecl norm(const _H3CMPLX(_Ty)& _X)
	{return (real(_X) * real(_X) + imag(_X) * imag(_X)); }
		// TEMPLATE FUNCTION polar
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl polar(const _Ty& _Rho, const _Ty& _Theta)
	{return (_H3CMPLX(_Ty)(_Rho * _CTR(_Ty)::cos(_Theta),
		_Rho * _CTR(_Ty)::sin(_Theta))); }
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl polar(const _Ty& _Rho)
	{return (polar(_Rho, (_Ty)0)); }
		// TEMPLATE FUNCTION pow
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl pow(const _H3CMPLX(_Ty)& _X,
		const _H3CMPLX(_Ty)& _Y)
	{if (imag(_Y) == 0)
		return (pow(_X, real(_Y)));
	else if (imag(_X) == 0)
		return (_H3CMPLX(_Ty)(pow(real(_X), _Y)));
	else
		return (exp(_Y * log(_X))); }
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl pow(const _H3CMPLX(_Ty)& _X, const _Ty& _Y)
	{if (imag(_X) == 0)
		return (_H3CMPLX(_Ty)(_CTR(_Ty)::pow(real(_X), _Y)));
	else
		return (exp(_Y * log(_X))); }
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl pow(const _H3CMPLX(_Ty)& _X, int _Y)
	{if (imag(_X) == 0)
		return (_H3CMPLX(_Ty)(_CTR(_Ty)::pow(real(_X), _Y)));
	else
		return (_Pow_int(_H3CMPLX(_Ty)(_X), _Y)); }
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl pow(const _Ty& _X, const _H3CMPLX(_Ty)& _Y)
	{if (imag(_Y) == 0)
		return (_H3CMPLX(_Ty)(_CTR(_Ty)::pow(_X, real(_Y))));
	else
		return (exp(_Y * _CTR(_Ty)::log(_X))); }
		// TEMPLATE FUNCTION sin
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl sin(const _H3CMPLX(_Ty)& _X)
	{return (_H3CMPLX(_Ty)(
		_CTR(_Ty)::_Cosh(imag(_X), _CTR(_Ty)::sin(real(_X))),
		_CTR(_Ty)::_Sinh(imag(_X), _CTR(_Ty)::cos(real(_X))))); }
		// TEMPLATE FUNCTION sinh
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl sinh(const _H3CMPLX(_Ty)& _X)
	{return (_H3CMPLX(_Ty)(
		_CTR(_Ty)::_Sinh(real(_X), _CTR(_Ty)::cos(imag(_X))),
		_CTR(_Ty)::_Cosh(real(_X), _CTR(_Ty)::sin(imag(_X))))); }
		// TEMPLATE FUNCTION sqrt
_TMPLT(_Ty) inline
	_H3CMPLX(_Ty) __cdecl sqrt(const _H3CMPLX(_Ty)& _X)
	{int _Xexp;
	_Ty _Rho = _Fabs(_X, &_Xexp);
	if (_Xexp == 0)
		return (_H3CMPLX(_Ty)(_Rho, _Rho));
	else
		{_Ty Remag = _CTR(_Ty)::ldexp(real(_X) < 0
			? - real(_X) : real(_X), -_Xexp);
		_Rho = _CTR(_Ty)::ldexp(_CTR(_Ty)::sqrt(
			2 * (Remag + _Rho)), _Xexp / 2 - 1);
		if (0 <= real(_X))
			return (_H3CMPLX(_Ty)(_Rho, imag(_X) / (2 * _Rho)));
		else if (imag(_X) < 0)
			return (_H3CMPLX(_Ty)(-imag(_X) / (2 * _Rho), -_Rho));
		else
			return (_H3CMPLX(_Ty)(imag(_X) / (2 * _Rho),
				_Rho)); }}


_STD_END
#ifdef  _MSC_VER
#pragma pack(pop)
#endif  /* _MSC_VER */

#endif /* _COMPLEX_ */

/*
 * Copyright (c) 1994 by P.J. Plauger.  ALL RIGHTS RESERVED. 
 * Consult your license regarding permissions and restrictions.
 */
