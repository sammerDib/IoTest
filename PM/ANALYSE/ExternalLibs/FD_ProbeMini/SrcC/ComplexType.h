#ifndef _SRRC_COMPLEXTYPE_H_
#define _SRRC_COMPLEXTYPE_H_


#ifdef SPG_COMPLEX_DEFINED
#error "Type SPG_COMPLEX defined twice"
#endif

#define SPG_COMPLEX_DEFINED

//ragma message( "Type SPG_COMPLEX defini dans "__FILE__ )

typedef struct {
	float re;
	float im;
} SPG_COMPLEX;

#define CX_MulR(C,a) C.re*=a;C.im*=a
#define CX_Mul(P1,P2,RES) RES.re=P1.re*P2.re-P1.im*P2.im;RES.im=P1.re*P2.im+P1.im*P2.re
#define CX_MulComplement(P1,P2,RES) RES.re=P1.re*P2.re+P1.im*P2.im;RES.im=P1.im*P2.re-P1.re*P2.im

#define CX_Module2(Z) (Z.re*Z.re+Z.im*Z.im)
#define CX_Module(Z) sqrtf(CX_Module2(Z))
#define CX_Arg(Z) atan2f(Z.re,Z.im)
#define CX_ModuleArg(Z) {SPG_COMPLEX MACRO_TMP_C;MACRO_TMP_C.re=CX_Module(Z);MACRO_TMP_C.im=CX_Arg(Z);Z=MACRO_TMP_C;}
#define CX_PolarToCart(ZDst,ZSrc) {ZDst.re=ZSrc.re*cos(ZSrc.im);ZDst.im=ZSrc.re*sin(ZSrc.im);}


#endif //_SRRC_COMPLEXTYPE_H_
