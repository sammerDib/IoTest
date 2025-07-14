#define _USE_MATH_DEFINES
#include <math.h>
#include "FFT.h"

#ifdef __BORLANDC__
#include <conio.h>
typedef complex Complex;
#else
using namespace std;
typedef complex<double> Complex;
//const double M_PI = 3.14159265358979323846;
#endif

#include <iostream>

//***************************************************************************************

FFT::FFT(int pp, int nn) {
	p = pp; n = nn; m = 1;
	for (int i=0; i<n; i++) m *= p;
	w = new Complex[m]; j = new int [m];
	init(w,j,p,n);
}

FFT::~FFT() {delete [] j; delete [] w;}

FFT& FFT::operator = (const FFT& FT) {
	if (n!=FT.n || p!=FT.p) {
		delete [] j; delete [] w;
		n = FT.n; p = FT.p; m = FT.m;
		w = new Complex[m]; j = new int [m];
		init(w,j,p,n);
	}
	return *this;
}

void ft(Complex* a, Complex* f, int n) {
  int i, ii, NN = abs(n);
  Complex ww; ww = Complex(0,2.*M_PI/double(NN));
  if (n < 0) for (i=0; i<NN; i++) {f[i] = 0; for (ii=0; ii<NN; ii++) f[i] += a[ii]*exp(-ww*double(i*ii)); f[i] /= double(NN);}
  else for (i=0; i<NN; i++) {f[i] = 0; for (ii=0; ii<NN; ii++) f[i] += a[ii]*exp(ww*double(i*ii));}
}

int inv2(int j, int n) {
  int i = 0;
  while (n--) {i <<= 1; if (j%2) i += 1; j >>= 1;}
  return i;
}

int inv4(int j, int n) {
  int i = 0;
  while (n--) {i <<= 2; i += j%4; j >>= 2;}
  return i;
}

int inv(int j, int p, int n) {
  int i = 0;
  while (n--) {i *= p; i += j%p; j /= p;}
  return i;
}

void init2(Complex* w, int* j, int n) {
  int i, ii, iii, N, NN = 1, nn;
  Complex ww; NN <<= n; N = NN>>1;
  for (i=0; i<N; i++) {w[i] = 1; j[i] = 0;}
  ww = exp(Complex(0,M_PI/N)); nn = N;
  for (i=1; i<N; i+=i) {
    ii=0;
	  while (ii<N) for (ii+=i,iii=0; iii<i; iii++,ii++) {w[ii] *= ww; j[ii] += nn;}
	  ww *= ww; nn >>= 1;
  }
  for (i=0; i<N; i++) {j[i+N] = j[i]+1; w[i+N] = 1./w[i];}
}

void init3(Complex* w, int* j, int n) {
  int i, ii, iii, N, NN = 1, nn;
  Complex ww, tc;
  for (i=0; i<n; i++) NN *= 3; N = NN/3;
  for (i=0; i<N; i++) {w[i] = 1; j[i] = 0;}
  ww = exp(Complex(0,2*M_PI/NN)); nn = N;
  for (i=1; i<N; i+=i+i) {
    ii=0; tc = ww*ww;
	  while (ii<N) {
      for (ii+=i,iii=0; iii<i; iii++, ii++) {j[ii] += nn; w[ii] *= ww;}
      for (iii=0; iii<i; iii++, ii++) {j[ii] += nn+nn; w[ii] *= tc;}
    }
	  ww *= tc; nn /= 3;
  }
  for (i=0; i<N; i++) {j[i+N] = j[i]+1; j[i+2*N] = j[i]+2; w[i+N] = 1./w[i];}
}

void init4(Complex* w, int* j, int n) {
  int i, ii, iii, N, NN = 1, nn;
  Complex ww, tc, tcc; NN <<= n+n; N = NN>>2;
  for (i=0; i<N; i++) {w[i] = 1; j[i] = 0;}
  ww = exp(Complex(0,2*M_PI/NN)); nn = N;
  for (i=1; i<N; i<<=2) {
    ii=0; tc = ww*ww; tcc = tc*ww;
	  while (ii<N) {
      for (ii+=i,iii=0; iii<i; iii++, ii++) {j[ii] += nn; w[ii] *= ww;}
      for (iii=0; iii<i; iii++, ii++) {j[ii] += nn+nn; w[ii] *= tc;}
      for (iii=0; iii<i; iii++, ii++) {j[ii] += 3*nn; w[ii] *= tcc;}
    }
	  ww *= tcc; nn >>= 2;
  }
  for (i=0; i<N; i++) {j[i+N] = j[i]+1; j[i+2*N] = j[i]+2; j[i+3*N] = j[i]+3; w[i+N] = 1./w[i];}
}

void init5(Complex* w, int* j, int n) {
  int i, ii, iii, N, NN = 1, nn;
  Complex ww, tc, tcc, tccc;
  for (i=0; i<n; i++) NN *= 5; N = NN/5;
  for (i=0; i<N; i++) {w[i] = 1; j[i] = 0;}
  ww = exp(Complex(0,2*M_PI/NN)); nn = N;
  for (i=1; i<N; i+=4*i) {
    ii=0; tc = ww*ww; tcc = ww*tc; tccc = tc*tc;
	  while (ii<N) {
      for (ii+=i,iii=0; iii<i; iii++, ii++) {j[ii] += nn; w[ii] *= ww;}
      for (iii=0; iii<i; iii++, ii++) {j[ii] += nn+nn; w[ii] *= tc;}
      for (iii=0; iii<i; iii++, ii++) {j[ii] += 3*nn; w[ii] *= tcc;}
      for (iii=0; iii<i; iii++, ii++) {j[ii] += 4*nn; w[ii] *= tccc;}
    }
	  ww *= tccc; nn /= 5;
  }
  for (i=0; i<N; i++) {j[i+N] = j[i]+1; j[i+2*N] = j[i]+2; j[i+3*N] = j[i]+3; j[i+4*N] = j[i]+4; w[i+N] = 1./w[i];}
}

void init6(Complex* w, int* j, int n) {
  int i, ii, iii, N, NN = 1, nn;
  Complex ww, tc, tcc, tccc, tcw;
  for (i=0; i<n; i++) NN *= 6; N = NN/6;
  for (i=0; i<N; i++) {w[i] = 1; j[i] = 0;}
  ww = exp(Complex(0,2.*M_PI/NN)); nn = N;
  for (i=1; i<N; i*=6) {
    ii=0; tc = ww*ww; tcc = ww*tc; tccc = tc*tc; tcw = tccc*ww;
	  while (ii<N) {
      for (ii+=i,iii=0; iii<i; iii++, ii++) {j[ii] += nn; w[ii] *= ww;}
      for (iii=0; iii<i; iii++, ii++) {j[ii] += nn+nn; w[ii] *= tc;}
      for (iii=0; iii<i; iii++, ii++) {j[ii] += 3*nn; w[ii] *= tcc;}
      for (iii=0; iii<i; iii++, ii++) {j[ii] += 4*nn; w[ii] *= tccc;}
	  for (iii=0; iii<i; iii++, ii++) {j[ii] += 5*nn; w[ii] *= tcw;}
    }
	  ww *= tcw; nn /= 6;
  }
  for (i=0; i<N; i++) {j[i+N] = j[i]+1; j[i+2*N] = j[i]+2; j[i+3*N] = j[i]+3; j[i+4*N] = j[i]+4; j[i+5*N] = j[i]+5; w[i+N] = 1./w[i];}
}

void init(Complex* w, int* j, int p, int n) {
  int i, ii, iii, jj, N, NN = 1, nn;
  Complex ww, tc;
  for (i=0; i<n; i++) NN *= p; N = NN/p;
  for (i=0; i<N; i++) {w[i] = 1.; j[i] = 0;}
  ww = exp(Complex(0.,2.*M_PI/NN)); nn = N;
  for (i=1; i<N; i*=p) {
    ii=0;
	while (ii<N) {
      tc = ww;
      for (ii+=i,iii=0; iii<i; iii++, ii++) {j[ii] += nn; w[ii] *= ww;}
      for (jj=2; jj<p; jj++) for (tc*=ww,iii=0; iii<i; iii++, ii++) {j[ii] += jj*nn; w[ii] *= tc;}
    }
	ww *= tc; nn /= p;
  }
  for (i=0; i<N; i++) {for (jj=1; jj<p; jj++) j[i+jj*N] = j[i]+jj; w[i+N] = 1./w[i];}
}

void init(Complex* w, int* j, int pp, int p, int n) {
  int i, ii, iii, jj, N, NN = 1, nn;
  Complex ww, tc;
  for (i=0; i<n; i++) NN *= p; N = NN; NN *= pp;
  for (i=0; i<N; i++) {w[i] = 1; j[i] = 0;}
  ww = exp(Complex(0,2*M_PI/NN)); nn = N;
  for (i=1; i<N; i+=(p-1)*i) {
    ii=0;
	  while (ii<N) {
      tc = ww;
      for (ii+=i,iii=0; iii<i; iii++, ii++) {j[ii] += nn; w[ii] *= ww;}
      for (jj=2; jj<p; jj++) {tc *= ww; for (iii=0; iii<i; iii++, ii++) {j[ii] += jj*nn; w[ii] *= tc;}}
    }
	  ww *= tc; nn /= p;
  }
  for (i=0; i<N; i++) {for (jj=1; jj<pp; jj++) j[i+jj*N] = j[i]+jj; w[i+N] = 1./w[i];}
}

//
void fft2(Complex* a, Complex* f, Complex* w, int* j, int n) {
  int i, ii, iii, N, NN = 1, nn = abs(n);
  Complex *b, ww;
  double vv, vva, vvb, vvc, vvd, *va, *vaa, *vb, *vc;
  NN <<= nn; N = NN>>1; if (n<0) w += N;
  for (ii=0; ii<N; ii++) {b = a+ *(j++); ww = *(a+ *(j++)); *f++ = *b + ww; *f++ = *b - ww;}
  f -= NN; j -= NN; nn = N>>1; vaa = reinterpret_cast <double*>(w); 
  for (i=2; i<NN; i+=i) {
	  vb = reinterpret_cast <double*>(f); vc = vb+2*i; 
	  for (ii=0; ii<nn; ii++) {
        va = vaa;
        for (iii=0; iii<i; iii++) {
		  vva = *va; vvb = *(va+1); vvc = *vc++; vv = *vc;   	  
		  vvd = vva*vv+vvb*vvc; vvc = vva*vvc-vvb*vv;
		  *vc++ = *(vb+1)-vvd; *(vc-2) = *vb-vvc; *vb++ += vvc; *vb++ += vvd;  
		  va += 2*nn;
		} 
        vb += 2*i; vc += 2*i; 
	  }
    nn >>= 1;
  }
  if (n<0) {for (i=0; i<NN; i++) *(f+i) /= NN; w -= N;}
}

void fft3(Complex* a, Complex* f, Complex* w, int* j, int n) {
  int i, ii, iii, N, NN = 1, nn = abs(n);
  double vv, vva, vvb, vvc, vvd, vvv, *va, *vaa, *vb, *vc;
  double va0, vb0, va1, vb1, va2, vb2, rb1;
  for (i=0; i<nn; i++) NN *= 3; N = NN/3;
  if (n<0) {w += N; rb1 = -0.5*sqrt(3.);} else rb1 = 0.5*sqrt(3.);
  va = reinterpret_cast <double*>(a); vc = reinterpret_cast <double*>(f); 
  for (ii=0; ii<N; ii++) {
    i = *(j++)*2; va0 = *(va+i); vb0 = *(va+i+1);
	i = *(j++)*2; va1 = *(va+i); vb1 = *(va+i+1);
	i = *(j++)*2; vva = *(va+i); vvb = *(va+i+1); 
	va2 = va1-vva; vb2 = vb1-vvb; va1 += vva; vb1 += vvb; 
	*vc++ = va0+va1; *vc++ = vb0+vb1; va1 *= 0.5; vb1 *= 0.5; va2 *= rb1; vb2 *= rb1;
	*vc++ = va0-va1-vb2; *vc++ = vb0-vb1+va2; *vc++ = va0-va1+vb2; *vc++ = vb0-vb1-va2; 
  } 
  j -= NN; nn = N/3; vaa = reinterpret_cast <double*>(w); vb = reinterpret_cast <double*>(f); 
  for (i=3; i<NN; i+=i+i) {
	for (ii=0; ii<nn; ii++) {
      vc = vb+6*i*ii; va = vaa;
	  for (iii=0; iii<i; iii++) {
        vva = *va; vvb = *(va+1); vvc = vva*vva-vvb*vvb; vvd = 2.*vva*vvb; va0 = *vc; vb0 = *(vc+1); 
		vv = *(vc+2*i); vvv = *(vc+2*i+1); va1 = vv*vva-vvv*vvb; vb1 = vv*vvb+vvv*vva;
		vv = *(vc+4*i); vvv = *(vc+4*i+1); vva = vv*vvc-vvd*vvv; vvb = vvc*vvv+vvd*vv; 
		va2 = va1-vva; vb2 = vb1-vvb; va1 += vva; vb1 += vvb; 
        *vc++ = va0+va1; *vc = vb0+vb1; va1 *= 0.5; vb1 *= 0.5; va2 *= rb1; vb2 *= rb1;
	    *(vc+2*i-1) = va0-va1-vb2; *(vc+2*i) = vb0-vb1+va2; 
		*(vc+4*i-1) = va0-va1+vb2; *(vc+4*i) = vb0-vb1-va2;    
		vc++; va += 2*nn; 
	  }
    }
	nn /= 3;
  }
  if (n<0) {for (i=0; i<NN; i++) *(f+i) /= NN; w -= N;}
}

void fft4(Complex* a, Complex* f, Complex* w, int* j, int n) {
  int i, ii, iii, N, NN = 1, nn = abs(n);
  double vv, vvv, vva, vvb, vvc, vvd, *va, *vaa, *vb, *vc;
  double va0, vb0, va1, vb1, va2, vb2, va3, vb3;
  NN <<= 2*nn; N = NN>>2; if (n<0) w += N; 
  va = reinterpret_cast <double*>(a); vc = reinterpret_cast <double*>(f); 
  for (ii=0; ii<N; ii++) {
    i = *(j++)*2; va0 = *(va+i); vb0 = *(va+i+1);
	i = *(j++)*2; va1 = *(va+i); vb1 = *(va+i+1);
	i = *(j++)*2; va2 = *(va+i); vb2 = *(va+i+1);
	i = *(j++)*2; va3 = *(va+i); vb3 = *(va+i+1);
    vva = va0+va2; vvb = vb0+vb2; vvc = va1+va3; vvd = vb1+vb3;
	va0 -= va2; vb0 -= vb2; va1 -= va3; vb1 -= vb3; 
    *vc++ = vva+vvc; *vc++ = vvb+vvd; 
	if (n<0) {*vc++ = va0+vb1; *vc++ = vb0-va1;} else {*vc++ = va0-vb1; *vc++ = vb0+va1;} 
	*vc++ = vva-vvc; *vc++ = vvb-vvd;
    if (n<0) {*vc++ = va0-vb1; *vc++ = vb0+va1;} else {*vc++ = va0+vb1; *vc++ = vb0-va1;} 
  }
  nn = N >> 2; j -= NN; vaa = reinterpret_cast <double*>(w); vb = reinterpret_cast <double*>(f); 
  for (i=4; i<NN; i<<=2) {
	for(ii=0; ii<nn; ii++) {
      vc = vb+8*ii*i;
	  for (iii=0; iii<i; iii++) {
        va = vaa+2*iii*nn; vva = *va; vvb = *(va+1); vvc = vva*vva-vvb*vvb; vvd = 2.*vva*vvb;
		va0 = *vc; vb0 = *(vc+1); 
		vv = *(vc+2*i); vvv = *(vc+2*i+1); va1 = vv*vva-vvv*vvb; vb1 = vv*vvb+vvv*vva;
		vv = *(vc+4*i); vvv = *(vc+4*i+1); va2 = vv*vvc-vvv*vvd; vb2 = vv*vvd+vvv*vvc;
		vv = *(vc+6*i); vvv = *(vc+6*i+1); vb3 = vv*vvc-vvv*vvd; vvd = vv*vvd+vvv*vvc;
		va3 = vb3*vva-vvd*vvb; vb3 = vb3*vvb+vvd*vva;
        vva = va0+va2; vvb = vb0+vb2; vvc = va1+va3; vvd = vb1+vb3;
	    va0 -= va2; vb0 -= vb2; va1 -= va3; vb1 -= vb3; 
		if (n<0) {*(vc+2*i) = va0+vb1; *(vc+2*i+1) = vb0-va1; *(vc+6*i) = va0-vb1; *(vc+6*i+1) = vb0+va1;} 
		else {*(vc+2*i) = va0-vb1; *(vc+2*i+1) = vb0+va1; *(vc+6*i) = va0+vb1; *(vc+6*i+1) = vb0-va1;} 
	    *(vc+4*i) = vva-vvc; *(vc+4*i+1) = vvb-vvd; *vc++ = vva+vvc; *vc++ = vvb+vvd; 
      }
    }
	nn >>= 2;
  }
  if (n<0) {for (i=0; i<NN; i++) *(f+i) /= NN; w -= N;}
}

//
void fft5(Complex* a, Complex* f, Complex* w, int* j, int n) {
  int i, ii, iii, N, NN = 1, nn = abs(n);
  double vv, vva, vvb, vvc, vvd, vvv, *va, *vaa, *vb, *vc;
  double va0, vb0, va1, vb1, va2, vb2, va3, vb3, va4, vb4, ra1, ra2, rb1, rb2;
  for (i=0; i<nn; i++) NN *= 5; N = NN/5; 
  if (n<0) {w += N; ra1 = cos(M_PI*0.4); rb1 = -sin(M_PI*0.4); ra2 = cos(M_PI*0.8); rb2 = -sin(M_PI*0.8);} 
  else {ra1 = cos(M_PI*0.4); rb1 = sin(M_PI*0.4); ra2 = cos(M_PI*0.8); rb2 = sin(M_PI*0.8);}
  va = reinterpret_cast <double*>(a); vc = reinterpret_cast <double*>(f); 
  for (ii=0; ii<N; ii++) {
    i = *(j++)*2; va0 = *(va+i); vb0 = *(va+i+1);
	i = *(j++)*2; va1 = *(va+i); vb1 = *(va+i+1);
	i = *(j++)*2; va2 = *(va+i); vb2 = *(va+i+1);
	i = *(j++)*2; vva = *(va+i); vvb = *(va+i+1); va3 = va2-vva; vb3 = vb2-vvb; va2 += vva; vb2 += vvb;
	i = *(j++)*2; vva = *(va+i); vvb = *(va+i+1); va4 = va1-vva; vb4 = vb1-vvb; va1 += vva; vb1 += vvb;
    *vc++ = va0+va1+va2; *vc++ = vb0+vb1+vb2; 
	*vc++ = va0+va1*ra1-vb4*rb1+va2*ra2-vb3*rb2; 
	*vc++ = vb0+va4*rb1+vb1*ra1+va3*rb2+vb2*ra2; 
	*vc++ = va0+va1*ra2-vb4*rb2+va2*ra1+vb3*rb1; 
	*vc++ = vb0+va4*rb2+vb1*ra2-va3*rb1+vb2*ra1; 
	*vc++ = va0+va1*ra2+vb4*rb2+va2*ra1-vb3*rb1; 
	*vc++ = vb0-va4*rb2+vb1*ra2+va3*rb1+vb2*ra1; 
	*vc++ = va0+va1*ra1+vb4*rb1+va2*ra2+vb3*rb2; 
	*vc++ = vb0-va4*rb1+vb1*ra1-va3*rb2+vb2*ra2; 
  }
  nn = N/5; j -= NN; vaa = reinterpret_cast <double*>(w); vb = reinterpret_cast <double*>(f); 
  for (i=5; i<NN; i*=5) {
	for(ii=0; ii<nn; ii++) {
      vc = vb+10*ii*i;
	  for (iii=0; iii<i; iii++) {
        va = vaa+2*iii*nn; vva = *va++; vvb = *va; vvc = vva*vva-vvb*vvb; vvd = 2.*vva*vvb;
		va0 = *vc; vb0 = *(vc+1); 
		vv = *(vc+2*i); vvv = *(vc+2*i+1); va1 = vv*vva-vvv*vvb; vb1 = vv*vvb+vvv*vva;
		vv = *(vc+4*i); vvv = *(vc+4*i+1); va2 = vv*vvc-vvv*vvd; vb2 = vv*vvd+vvv*vvc;
		vv = *(vc+6*i); vvv = *(vc+6*i+1); vb3 = vv*vvc-vvv*vvd; vv = vv*vvd+vvv*vvc;
		vvv = vb3*vva-vv*vvb; vvb = vb3*vvb+vv*vva; va3 = va2-vvv; vb3 = vb2-vvb; va2 += vvv; vb2 += vvb;
		vv = *(vc+8*i); vvv = *(vc+8*i+1); vb4 = vv*vvc-vvv*vvd; vv = vv*vvd+vvv*vvc;
        vvv = vb4*vvc-vv*vvd; vvd = vb4*vvd+vv*vvc; va4 = va1-vvv; vb4 = vb1-vvd; va1 += vvv; vb1 += vvd;
	    *(vc+2*i) = va0+va1*ra1-vb4*rb1+va2*ra2-vb3*rb2; 
		*(vc+2*i+1) = vb0+va4*rb1+vb1*ra1+va3*rb2+vb2*ra2; 
		*(vc+4*i) = va0+va1*ra2-vb4*rb2+va2*ra1+vb3*rb1; 
		*(vc+4*i+1) = vb0+va4*rb2+vb1*ra2-va3*rb1+vb2*ra1; 
		*(vc+6*i) = va0+va1*ra2+vb4*rb2+va2*ra1-vb3*rb1; 
		*(vc+6*i+1) = vb0-va4*rb2+vb1*ra2+va3*rb1+vb2*ra1; 
		*(vc+8*i) = va0+va1*ra1+vb4*rb1+va2*ra2+vb3*rb2; 
		*(vc+8*i+1) = vb0-va4*rb1+vb1*ra1-va3*rb2+vb2*ra2; 
		*vc++ = va0+va1+va2; *vc++ = vb0+vb1+vb2; 
	  }
    }
	nn /= 5;
  }
  if (n<0) {for (i=0; i<NN; i++) *(f+i) /= NN; w -= N;}
}

void fft6(Complex* a, Complex* f, Complex* w, int* j, int n) {
  int i, ii, iii, N, NN = 1, nn = abs(n);
  double vv, vva, vvb, vvc, vvd, vvv, *va, *vaa, *vb, *vc;
  double va0, vb0, va1, vb1, va2, vb2, va3, vb3, va4, vb4, va5, vb5, rb1;
  for (i=0; i<nn; i++) NN *= 6; N = NN/6;
  if (n<0) {w += N; rb1 = -0.5*sqrt(3.);} else rb1 = 0.5*sqrt(3.);
  va = reinterpret_cast <double*>(a); vc = reinterpret_cast <double*>(f); 
  for (ii=0; ii<N; ii++) {
    i = *(j++)*2; va0 = *(va+i); vb0 = *(va+i+1);
	i = *(j++)*2; va1 = *(va+i); vb1 = *(va+i+1);
	i = *(j++)*2; va2 = *(va+i); vb2 = *(va+i+1);
	i = *(j++)*2; vva = *(va+i); vvb = *(va+i+1); va3 = va0-vva; vb3 = vb0-vvb; va0 += vva; vb0 += vvb;
	i = *(j++)*2; vva = *(va+i); vvb = *(va+i+1); va4 = va1-vva; vb4 = vb1-vvb; va1 += vva; vb1 += vvb;
	i = *(j++)*2; vva = *(va+i); vvb = *(va+i+1); vvc = va2-vva; vvd = vb2-vvb; vva += va2; vvb += vb2;
    va2 = va1-vva; vb2 = vb1-vvb; va1 += vva; vb1 += vvb;
	va5 = va4-vvc; vb5 = vb4-vvd; va4 += vvc; vb4 += vvd;
    *vc++ = va0+va1; *vc++ = vb0+vb1; 
	*vc++ = va3+0.5*va5-rb1*vb4; *vc++ = vb3+0.5*vb5+rb1*va4; 
	*vc++ = va0-0.5*va1-rb1*vb2; *vc++ = vb0-0.5*vb1+rb1*va2;
	*vc++ = va3-va5; *vc++ = vb3-vb5; 
	*vc++ = va0-0.5*va1+rb1*vb2; *vc++ = vb0-0.5*vb1-rb1*va2;
    *vc++ = va3+0.5*va5+rb1*vb4; *vc++ = vb3+0.5*vb5-rb1*va4; 
  }
  nn = N/6; j -= NN; vaa = reinterpret_cast <double*>(w); vb = reinterpret_cast <double*>(f); 
  for (i=6; i<NN; i*=6) {
	for(ii=0; ii<nn; ii++) {
      vc = vb+12*ii*i; va = vaa;
	  for (iii=0; iii<i; iii++) {
        vva = *va; vvb = *(va+1); vvc = vva*vva-vvb*vvb; vvd = 2.*vva*vvb;
		va5 = vvc*vvc-vvd*vvd; vb5 = 2.*vvc*vvd; va4 = vva*vvc-vvb*vvd; vb4 = vva*vvd+vvb*vvc;
		va0 = *vc; vb0 = *(vc+1); 
		vv = *(vc+2*i); vvv = *(vc+2*i+1); va1 = vv*vva-vvv*vvb; vb1 = vv*vvb+vvv*vva;
		vv = *(vc+4*i); vvv = *(vc+4*i+1); va2 = vv*vvc-vvv*vvd; vb2 = vv*vvd+vvv*vvc;
        vvc = vva*va5-vvb*vb5; vvd = vva*vb5+vvb*va5;
		vv = *(vc+6*i); vvv = *(vc+6*i+1); vva = vv*va4-vvv*vb4; vvb = vv*vb4+vvv*va4;
		va3 = va0-vva; vb3 = vb0-vvb; va0 += vva; vb0 += vvb;
        vv = *(vc+8*i); vvv = *(vc+8*i+1); vva = vv*va5-vvv*vb5; vvb = vv*vb5+vvv*va5;
		va4 = va1-vva; vb4 = vb1-vvb; va1 += vva; vb1 += vvb;
		vv = *(vc+10*i); vvv = *(vc+10*i+1); vva = vv*vvc-vvv*vvd; vvb = vv*vvd+vvv*vvc;
	    vvc = va2-vva; vvd = vb2-vvb; vva += va2; vvb += vb2;
		va2 = rb1*(va1-vva); vb2 = rb1*(vb1-vvb); va1 += vva; vb1 += vvb;
	    va5 = va4-vvc; vb5 = vb4-vvd; va4 = rb1*(va4+vvc); vb4 = rb1*(vb4+vvd);
		*(vc+2*i) = va3+0.5*va5-vb4; *(vc+4*i) = va0-0.5*va1-vb2; *(vc+6*i) = va3-va5; 
	    *(vc+8*i) = va0-0.5*va1+vb2; *(vc+10*i) = va3+0.5*va5+vb4; *vc++ = va0+va1; 
		*(vc+2*i) = vb3+0.5*vb5+va4; *(vc+4*i) = vb0-0.5*vb1+va2; *(vc+6*i) = vb3-vb5; 
		*(vc+8*i) = vb0-0.5*vb1-va2; *(vc+10*i) = vb3+0.5*vb5-va4; *vc++ = vb0+vb1; 
		va += 2*nn;
      }
    }
	nn /= 6;
  }
  if (n<0) {for (i=0; i<NN; i++) *(f+i) /= NN; w -= N;}
}

void fft8(Complex* a, Complex* f, Complex* w, int* j, int n) {
  int i, ii, iii, N, NN = 1, nn = abs(n);
  double vv, vva, vvb, vvc, vvd, vvv, *va, *vaa, *vb, *vc;
  double va0, vb0, va1, vb1, va2, vb2, va3, vb3, va4, vb4, va5, vb5, va6, vb6, va7, vb7, rb1;
  for (i=0; i<nn; i++) NN <<= 3; N = NN >> 3;
  if (n<0) w += N; rb1 = sqrt(0.5);
  va = reinterpret_cast <double*>(a); vc = reinterpret_cast <double*>(f); 
  for (ii=0; ii<N; ii++) {
    i = *(j++)*2; va0 = *(va+i); vb0 = *(va+i+1);
	i = *(j++)*2; va1 = *(va+i); vb1 = *(va+i+1);
	i = *(j++)*2; va2 = *(va+i); vb2 = *(va+i+1);
	i = *(j++)*2; va3 = *(va+i); vb3 = *(va+i+1);
	i = *(j++)*2; vva = *(va+i); vvb = *(va+i+1); va4 = va0-vva; vb4 = vb0-vvb; va0 += vva; vb0 += vvb;
	i = *(j++)*2; vva = *(va+i); vvb = *(va+i+1); va5 = va1-vva; vb5 = vb1-vvb; va1 += vva; vb1 += vvb;
	i = *(j++)*2; vva = *(va+i); vvb = *(va+i+1); va6 = va2-vva; vb6 = vb2-vvb; va2 += vva; vb2 += vvb;
	i = *(j++)*2; vva = *(va+i); vvb = *(va+i+1); va7 = va3-vva; vb7 = vb3-vvb; va3 += vva; vb3 += vvb;
	*vc++ = va0+va1+va2+va3; *vc++ = vb0+vb1+vb2+vb3; 
	if (n < 0) {
	  *vc++ = va4+vb6+rb1*(va5+vb5-va7+vb7); *vc++ = vb4-va6+rb1*(vb5-va5-vb7-va7);
	  *vc++ = va0+vb1-va2-vb3; *vc++ = vb0-va1-vb2+va3;
      *vc++ = va4-vb6+rb1*(-va5+vb5+va7+vb7); *vc++ = vb4+va6+rb1*(-vb5-va5+vb7-va7);
	}
	else {
	  *vc++ = va4-vb6+rb1*(va5-vb5-va7-vb7); *vc++ = vb4+va6+rb1*(vb5+va5-vb7+va7);
	  *vc++ = va0-vb1-va2+vb3; *vc++ = vb0+va1-vb2-va3;
      *vc++ = va4+vb6+rb1*(-va5-vb5+va7-vb7); *vc++ = vb4-va6+rb1*(-vb5+va5+vb7+va7);
	}
    *vc++ = va0-va1+va2-va3; *vc++ = vb0-vb1+vb2-vb3;
	if (n < 0) {
	  *vc++ = va4+vb6+rb1*(-va5-vb5+va7-vb7); *vc++ = vb4-va6+rb1*(-vb5+va5+vb7+va7);
	  *vc++ = va0-vb1-va2+vb3; *vc++ = vb0+va1-vb2-va3;
      *vc++ = va4-vb6+rb1*(va5-vb5-va7-vb7); *vc++ = vb4+va6+rb1*(vb5+va5-vb7+va7);
	}
	else {
	  *vc++ = va4-vb6+rb1*(-va5+vb5+va7+vb7); *vc++ = vb4+va6+rb1*(-vb5-va5+vb7-va7);
	  *vc++ = va0+vb1-va2-vb3; *vc++ = vb0-va1-vb2+va3;
      *vc++ = va4+vb6+rb1*(+va5+vb5-va7+vb7); *vc++ = vb4-va6+rb1*(vb5-va5-vb7-va7);
	}
  }
  nn = N >> 3; j -= NN; vaa = reinterpret_cast <double*>(w); vb = reinterpret_cast <double*>(f); 
  for (i=8; i<NN; i<<=3) {
	for(ii=0; ii<nn; ii++) {
      vc = vb+16*ii*i; va = vaa;
	  for (iii=0; iii<i; iii++) {
        vva = *va; vvb = *(va+1); vvc = vva*vva-vvb*vvb; vvd = 2.*vva*vvb; 
		vv = vvc*vvc-vvd*vvd; vvv += (vvv = vvc*vvd); va0 = *vc; vb0 = *(vc+1); 
		va2 = vv*vvc-vvv*vvd; vb2 = vv*vvd+vvv*vvc;	va7 = *(vc+14*i); vb7 = *(vc+14*i+1); 
		va1 = va7*va2-vb7*vb2; vb1 = va7*vb2+vb7*va2; va7 = va1*vva-vb1*vvb; vb7 = va1*vvb+vb1*vva;
		va1 = *(vc+12*i); vb1 = *(vc+12*i+1); va6 = va1*va2-vb1*vb2; vb6 = va1*vb2+vb1*va2; 
		va5 = *(vc+10*i); vb5 = *(vc+10*i+1); va1 = va5*vv-vb5*vvv; vb1 = va5*vvv+vb5*vv;
        va5 = va1*vva-vb1*vvb; vb5 = va1*vvb+vb1*vva; 
		va1 = *(vc+8*i); vb1 = *(vc+8*i+1); va2 = va1*vv-vb1*vvv; vb2 = va1*vvv+vb1*vv; 
		va4 = va0-va2; vb4 = vb0-vb2; va0 += va2; vb0 += vb2;
		va1 = *(vc+4*i); vb1 = *(vc+4*i+1); vv = va1*vvc-vb1*vvd; vvv = va1*vvd+vb1*vvc; 
		va2 = vv+va6; vb2 = vvv+vb6; va6 = vv-va6; vb6 = vvv-vb6;
		vv = *(vc+6*i); vvv = *(vc+6*i+1); va1 = vv*vvc-vvv*vvd; vb1 = vv*vvd+vvv*vvc; 
		vv = va1*vva-vb1*vvb; vvv = va1*vvb+vb1*vva; va3 = vv+va7; vb3 = vvv+vb7; vvc = vv-va7; vvd = vvv-vb7; 
		va1 = *(vc+2*i); vb1 = *(vc+2*i+1); vv = va1*vva-vb1*vvb; vvv = va1*vvb+vb1*vva; 
		va1 = vv+va5; vb1 = vvv+vb5; va5 = vv-va5; vb5 = vvv-vb5; 
		vva = va5-vvc; vvb = vb5-vvd; va5 += vvc; vb5 += vvd; 
		va7 = rb1*(va5-vvb); vb7 = rb1*(vb5-vva); va5 = rb1*(va5+vvb); vb5 = rb1*(vb5+vva); 
	    *vc = va0+va1+va2+va3; *(vc+1) = vb0+vb1+vb2+vb3; 
		*(vc+8*i) = va0-va1+va2-va3; *(vc+8*i+1) = vb0-vb1+vb2-vb3;
		if (n < 0) {
		  *(vc+2*i) = va4+vb6+vb5; *(vc+2*i+1) = vb4-va6-va7;
		  *(vc+4*i) = va0+vb1-va2-vb3; *(vc+4*i+1) = vb0-va1-vb2+va3;
		  *(vc+6*i) = va4-vb6+vb7; *(vc+6*i+1) = vb4+va6-va5;
		  *(vc+10*i) = va4+vb6-vb5; *(vc+10*i+1) = vb4-va6+va7;
		  *(vc+12*i) = va0-vb1-va2+vb3; *(vc+12*i+1) = vb0+va1-vb2-va3;
		  *(vc+14*i) = va4-vb6-vb7; *(vc+14*i+1) = vb4+va6+va5;
		}
		else {
		  *(vc+2*i) = va4-vb6-vb7; *(vc+2*i+1) = vb4+va6+va5;
		  *(vc+4*i) = va0-vb1-va2+vb3; *(vc+4*i+1) = vb0+va1-vb2-va3;
		  *(vc+6*i) = va4+vb6-vb5; *(vc+6*i+1) = vb4-va6+va7;
		  *(vc+10*i) = va4-vb6+vb7; *(vc+10*i+1) = vb4+va6-va5;
	  	  *(vc+12*i) = va0+vb1-va2-vb3; *(vc+12*i+1) = vb0-va1-vb2+va3;
		  *(vc+14*i) = va4+vb6+vb5; *(vc+14*i+1) = vb4-va6-va7;
		}
		va += 2*nn; vc += 2; 
      }
    }
	nn >>= 3;
  }
  if (n<0) {for (i=0; i<NN; i++) *(f+i) /= NN; w -= N;}
}

//
void fft(Complex* a, Complex* f, Complex* w, int* j, int p, int n) {
  switch (p) {
    case 2: {fft2(a,f,w,j,n); break;}
    case 3: {fft3(a,f,w,j,n); break;}
    case 4: {fft4(a,f,w,j,n); break;}
    case 5: {fft5(a,f,w,j,n); break;}
    case 6: {fft6(a,f,w,j,n); break;}
	case 8: {fft8(a,f,w,j,n); break;}
    default: {
      int i, ii, iii, jj, jjj, k, N, NN = 1, nn = abs(n);
      double vv, vva, vvb, vvc, vvd, vvv;
      Complex tc, tcc, *ww, *c, *b; c = new Complex[p]; b = new Complex[p]; ww = new Complex[p];
      for (i=0; i<nn; i++) NN *= p; N = NN/p;
      if (n<0) {w += N; tc = exp(Complex(0.,-2.*M_PI/p));} else tc = exp(Complex(0.,2.*M_PI/p)); 
	  vva = tc.real(); vvb = tc.imag(); ww[0] = Complex(vvc = 1.,vvd = 0.);
	  for (jj=1; jj<p; jj++) {vv = vvc; vvc = vv*vva-vvd*vvb; vvd = vv*vvb+vvd*vva; ww[jj] = Complex(vvc,vvd);}
      for (ii=0; ii<NN; ii+=p) {
        for (jj=0; jj<p; jj++) {b[jj] = *(a+j[ii+jj]); f[jj] = b[0];}
        for (jj=1; jj<p; jj++) f[0] += b[jj];
        for (jj=1; jj<p; jj++) for (jjj=1; jjj<p; jjj++) f[jj] += b[jjj]*ww[(jj*jjj)%p];
        f += p;
      }
      nn = N/p; f -= NN;
      for (i=p; i<NN; i+=(p-1)*i) {
       for(ii=0; ii<nn; ii++) {
        for (iii=0; iii<i; iii++) {
          k = ii*i*p+iii;
          vva = w[iii*nn].real(); vvb = w[iii*nn].imag(); vvc = 1.; vvd = 0.;
          for (jj=0; jj<p; jj++) {
            vv = f[k+i*jj].real(); vvv = f[k+i*jj].imag();    
			b[jj] = Complex(vv*vvc-vvv*vvd,vv*vvd+vvv*vvc); 
			c[jj] = b[0];
			vv = vvc; vvc = vva*vv-vvb*vvd; vvd = vv*vvb+vvd*vva; 
		  }
          for (jj=1; jj<p; jj++) c[0] += b[jj];
		  for (jj=1; jj<p; jj++) for (jjj=1; jjj<p; jjj++) {
		    vva = ww[(jj*jjj)%p].real(); vvb = ww[(jj*jjj)%p].imag(); 
			vvc = b[jjj].real(); vvd = b[jjj].imag();
			c[jj] += Complex(vva*vvc-vvb*vvd,vva*vvd+vvb*vvc); 
		  }
          for (jj=0; jj<p; jj++) f[k+i*jj] = c[jj];
        }
      }
        nn /= p;
      }
      if (n<0) {for (i=0; i<NN; i++) *(f+i) /= NN; w -= N;}
      delete[] ww; delete[] b; delete[] c;
    }
  };
}

int optim(int n, int& p, int& m) {
  if (n <= 6561) {
	if (n <= 216) {
      if (n <= 36) {
	    if (n <= 6) {p = n; m = 1; return n;} 
	    if (n <= 8) {p = 8; m = 1; return 8;}
	    if (n <= 16) {p = 4; m = 2; return 16;}
	    p = 6; m = 2; return 36;
	  }
	  if (n <= 64) {p = 4; m = 3; return 64;}
	  if (n <= 81) {p = 3; m = 4; return 81;}
	  if (n <= 128) {p = 2; m = 7; return 128;}
	  p = 6; m = 3; return 216;
	}
	if (n <= 1024) {
	  if (n <= 256) {p = 4; m = 4; return 256;}
	  if (n <= 512) {p = 8; m = 3; return 512;}
	  if (n <= 729) {p = 3; m = 6; return 729;}
	  p = 4; m = 5; return 1024;
	}
	if (n <= 1296) {p = 6; m = 4; return 1296;}
	if (n <= 2048) {p = 2; m = 11; return 2048;}
	if (n <= 4096) {p = 4; m = 6; return 4096;}
	p = 3; m = 8; return 6561;
  }
  if (n <= 1679616) {
    if (n <= 78125) {
	  if (n <= 19683) {
	    if (n <= 7776) {p = 6; m = 5; return 7776;}
	    if (n <= 8192) {p = 2; m = 13; return 8192;}
	    if (n <= 16384) {p = 4; m = 7; return 16384;}
	    p = 3; m = 9; return 19683;
	  }
	  if (n <= 32768) {p = 8; m = 5; return 32768;}
	  if (n <= 46656) {p = 6; m = 6; return 46656;}
	  if (n <= 65536) {p = 4; m = 8; return 65536;}
	  p = 5; m = 7; return 78125;
	}
	if (n <= 279936) {
	  if (n <= 131072) {p = 2; m = 17; return 131072;}
	  if (n <= 177147) {p = 3; m = 11; return 177147;}
	  if (n <= 262144) {p = 8; m = 6; return 262144;}
	  p = 6; m = 7; return 279936;
	}
	if (n <= 390625) {p = 5; m = 8; return 390625;}
	if (n <= 531441) {p = 3; m = 12; return 531441;}
	if (n <= 1048576) {p = 4; m = 10; return 1048576;}
	p = 6; m = 8; return 1679616;
  }
  if (n <= 4782969) {
	if (n <= 1953125) {p = 5; m = 9; return 1953125;}
	if (n <= 2097152) {p = 8; m = 7; return 2097152;}
	if (n <= 4194304) {p = 4; m = 11; return 4194304;}
	p = 3; m = 14; return 4782969;
  }
  if (n <= 10077696) {p = 2; m = 23; return 10077696;}
  if (n <= 14348907) {p = 3; m = 15; return 14348907;}
  if (n <= 16777216) {p = 8; m = 8; return 16777216;}
  p = 11; m = 7; return 19487171;
}