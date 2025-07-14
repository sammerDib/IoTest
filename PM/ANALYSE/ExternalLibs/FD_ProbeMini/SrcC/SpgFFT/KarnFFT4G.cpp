//FONCTION INITIALE
/*
void __inline
iFFT4G(complex *x,int astep,int tstep)
{
*/
	__asm
	{
	mov ebx,astep
	mov eax,x    	/* eax -> a */
	lea ecx,[eax+2*ebx]
	lea edx,[ebx+ecx]
	add ebx,eax

// Methode initiale
//					0				1		2		3		4			5
	fld Bre		//	Bre
	fsub Dre

	fld Bre
	fadd Dre
	fld Are
	fadd Cre
	fld Are
	fsub Cre

	fld	Bim		//	Bim				Are-Cre	Cre+Are	Bre+Dre	Bre-Dre
	fsub Dim	//	Bim-Dim			Are-Cre	Cre+Are	Bre+Dre	Bre-Dre
	fxch st(3)	//	Bre+Dre			Are-Cre	Cre+Are	Bim-Dim	Bre-Dre
	fld	st		//	Bre+Dre			Bre+Dre	Are-Cre	Cre+Are	Bim-Dim		Bre-Dre
	fadd st,st(3) //Br+Dr+Cr+ArX	Bre+Dre	Are-Cre	Cre+Are	Bim-Dim		Bre-Dre
	fxch st(3) 	//	Cre+Are			Bre+Dre	Are-Cre	X		Bim-Dim		Bre-Dre
	fsubr		//	Cr+Ar-(Br+Dr)Y	Are-Cre	X		Bim-Dim	Bre-Dre
	fxch st(1)	//	Are-Cre			Y		X		Bim-Dim	Bre-Dre
	fld	st		//	Are-Cre			Are-Cre	Y		X		Bim-Dim		Bre-Dre
	fadd st,st(4) //Ar-Cr+Bi-DiZ	Are-Cre	Y		X		Bim-Dim		Bre-Dre
	fxch st(4)	//	Bim-Dim			Are-Cre	Y		X		Z			Bre-Dre
	fsub		//	Bi-Di-(Ar-Cr)T	Y		X		Z		Bre-Dre
	fxch st(2)	//	X				Y		T		Z		Bre-Dre
	fstp Are	//	Y				T		Z		Bre-Dre
	fstp Bre	//	T				Z		Bre-Dre
	fstp Dre	//	Z				Bre-Dre
	fstp Cre	//	Bre-Dre
                      
    	/* Now do the imaginaries */
//					0				1		2		3		4			5
	fld	Aim
	fsub	Cim	/* Aim-Cim	Bre-Dre */
	fld	Aim
	fadd	Cim	/* Aim+Cim	Aim-Cim		Bre-Dre */

	fld	Bim		//	Bim				Aim+Cim	Aim-Cim	Bre-Dre
	fadd Dim	//	Bim+Dim			Aim+Cim	Aim-Cim	Bre-Dre
	fld	st(3)	//	Bre-Dre			Bim+Dim	Aim+Cim	Aim-Cim	Bre-Dre
	fadd st,st(3) //Br-Dr+Ai-CiX	Bim+Dim	Aim+Cim	Aim-Cim	Bre-Dre
	fxch st(2)	//	Aim+Cim			Bim+Dim	X		Aim-Cim	Bre-Dre
	fld	st		//	Aim+Cim			Aim+Cim	Bim+Dim	X		Aim-Cim		Bre-Dre
	fadd st,st(2) //Ai+Ci+Bi+DiY	Aim+Cim	Bim+Dim	X		Aim-Cim		Bre-Dre
	fxch st(2)	//	Bim+Dim			Aim+Cim	Y		X		Aim-Cim		Bre-Dre
	fsub	      //Bi+Di-(Ai+Ci)Z	Y		X		Aim-Cim	Bre-Dre
	fxch st(3)	//	Aim-Cim			Y		X		Z		Bre-Dre
	fsub st,st(4) //A FINIR Ai-Ci-(Br-Dr)T	Y		X		Z		Bre-Dre
	fxch st(2)	//	X				Y		T		Z		Bre-Dre
	fstp Dim	//	Y				T		Z		Bre-Dre
	fstp Aim	//	T				Z		Bre-Dre
	mov eax,tstep
	fstp Cim	//	Z				Bre-Dre
	cmp eax,0
	fstp Bim	//	Bre-Dre
	fstp st
	jz done	      		/* Twiddles are all unity, skip this stuff */

	
	//				0		1		2		3		4		5
/*
//2tstep
add eax,eax
fld Bre	//Bre
fld st	//Bre	Bre
fld Bim	//Bim	Bre		Bre
fld st	//Bim	Bim		Bre		Bre
fld COS(eax)	//cos	Bim		Bim		Bre		Bre
fmul st(4),st	//cos	Bim		Bim		Bre		Brecos
fld SIN(eax+MAXPOINTS_I)	//sin	cos	Bim	Bim	Bre	Brecos
fmul st(3),st	//sin	cos	Bim	Bimsin	Bre	Brecos
fxch st(2)		//Bim	cos	sin	Bimsin	Bre	Brecos
fmul			//Bimcos sin	Bimsin	Bre	Brecos
fxch st(2)		//Bimsin sin	Bimcos	Bre	Brecos
fsubp st(4),st //sin Bimcos Bre Brecos-Bimsin
fmulp st(2),st		//Bimcos Bresin Brecos-Bimsin
mov eax,tstep
fadd			//Bim Bre
fxch
fstp Bre
fstp Bim

//1tstep
fld Cre	//Bre
fld st	//Bre	Bre
fld Cim	//Bim	Bre		Bre
fld st	//Bim	Bim		Bre		Bre
fld COS(eax)	//cos	Bim		Bim		Bre		Bre
fmul st(4),st	//cos	Bim		Bim		Bre		Brecos
fld SIN(eax+MAXPOINTS_I)	//sin	cos	Bim	Bim	Bre	Brecos
fmul st(3),st	//sin	cos	Bim	Bimsin	Bre	Brecos
fxch st(2)		//Bim	cos	sin	Bimsin	Bre	Brecos
fmul			//Bimcos sin	Bimsin	Bre	Brecos
fxch st(2)		//Bimsin sin	Bimcos	Bre	Brecos
	add eax,eax
fsubp st(4),st //sin Bimcos Bre Brecos-Bimsin
fmulp st(2),st		//Bimcos Bresin Brecos-Bimsin
	add eax,tstep
fadd			//Bim Bre
fxch
fstp Cre
fstp Cim

//3tstep
fld Dre	//Bre
fld st	//Bre	Bre
fld Dim	//Bim	Bre		Bre
fld st	//Bim	Bim		Bre		Bre
fld COS(eax)	//cos	Bim		Bim		Bre		Bre
fmul st(4),st	//cos	Bim		Bim		Bre		Brecos
fld SIN(eax+MAXPOINTS_I)	//sin	cos	Bim	Bim	Bre	Brecos
fmul st(3),st	//sin	cos	Bim	Bimsin	Bre	Brecos
fxch st(2)		//Bim	cos	sin	Bimsin	Bre	Brecos
fmul			//Bimcos sin	Bimsin	Bre	Brecos
fxch st(2)		//Bimsin sin	Bimcos	Bre	Brecos
fsubp st(4),st //sin Bimcos Bre Brecos-Bimsin
fmulp st(2),st		//Bimcos Bresin Brecos-Bimsin
fadd			//Bim Bre
fxch
fstp Dre
fstp Dim
*/
	/* Multiply by twiddle factors */
	fld	Cre
	fmul	COS(eax)	/* Bre*cos */
	fld	Cim
	fmul	SIN(eax+MAXPOINTS_I)	/* Bim*sin	Bre*cos */
	fld	Cre
	fmul	SIN(eax+MAXPOINTS_I)	/* Bre*sin	Bim*sin		Bre*cos */
	fxch	st(2)		/* Bre*cos	Bim*sin		Bre*sin */
	fsubr			/* Bre'		Bre*sin */
	fxch	st(1)		/* Bre*sin	Bre' */
	fld	Cim
	fmul	COS(eax)	/* Bim*cos	Bre*sin		Bre' */
	fxch	st(2)		/* Bre'		Bre*sin		Bim*cos */
	fstp	Cre		/* Bre*sin	Bim*cos */
	fadd			/* Bim' */
	add eax,eax		/* 2*tstep*sizeof(float) */
	fld	Bre		/* Cre		Bim' */
	fxch	st(1)		/* Bim'		Cre */
	fstp	Cim		/* Cre */
	fmul	COS(eax)	/* Cre*cos */
	fld	Bim
	fmul	SIN(eax+MAXPOINTS_I)	/* Cim*sin	Cre*cos */
	fld	Bre
	fmul	SIN(eax+MAXPOINTS_I)	/* Cre*sin	Cim*sin		Cre*cos */
	fxch	st(2)		/* Cre*cos	Cim*sin		Cre*sin */
	fsubr			/* Cre'		Cre*sin */
	fxch	st(1)		/* Cre*sin	Cre' */
	fld	Bim
	fmul	COS(eax)	/* Cim*cos	Cre*sin		Cre' */
	fxch	st(2)		/* Cre'		Cre*sin		Cim*cos */
	fstp	Bre		/* Cre*sin	Cim*cos */
	fadd			/* Cim */
	add	eax,tstep	/* 3*tstep*sizeof(float) */
	fld	Dre		/* Dre		Cim */
	fxch	st(1)		/* Cim		Dre */
	fstp	Bim		/* Dre */
	fmul	COS(eax)	/* Dre*cos */
	fld	Dim
	fmul	SIN(eax+MAXPOINTS_I)	/* Dim*sin	Dre*cos */
	fld	Dre
	fmul	SIN(eax+MAXPOINTS_I)	/* Dre*sin	Dim*sin		Dre*cos */
	fxch	st(2)		/* Dre*cos	Dim*sin		Dre*sin */
	fsubr			/* Dre'		Dre*sin */
	fxch	st(1)		/* Dre*sin	Dre' */
	fld	Dim
	fmul	COS(eax)	/* Dim*cos	Dre*sin		Dre' */
	fxch	st(2)		/* Dre'		Dre*sin		Dim*cos */
	fstp	Dre		/* Dre*sin	Dim*cos */
	fadd			/* Dim */
	fstp	Dim
                           
done:
	}
	//return;
/*
}
*/