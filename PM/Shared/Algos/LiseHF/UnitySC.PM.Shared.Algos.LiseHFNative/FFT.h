#ifndef _FFT_H
#define _FFT_H

#ifdef __BORLANDC__
#include <complex.h>
typedef complex Complex;
#else
#include <complex>
using namespace std;
typedef complex<double> Complex;
#endif


void ft(Complex* a, Complex* f, int n);
int inv2(int j, int n); 
int inv4(int j, int n);
int inv(int j, int p, int n);
void init2(Complex* w, int* j, int n);
void init3(Complex* w, int* j, int n);
void init4(Complex* w, int* j, int n);
void init5(Complex* w, int* j, int n);
void init6(Complex* w, int* j, int n);
void init(Complex* w, int* j, int p, int n);
void init(Complex* w, int* j, int pp, int p, int n);
void init(Complex* w, Complex* w1, int* j1, Complex* w2, int* j2, int p1, int n1, int p2, int n2);
void fft2(Complex* a, Complex* f, Complex* w, int* j, int n);
void fft2(Complex* a, Complex* f, Complex* w, int* j, int p, int n);
void fft3(Complex* a, Complex* f, Complex* w, int* j, int n);
void fft4(Complex* a, Complex* f, Complex* w, int* j, int n);
void fft5(Complex* a, Complex* f, Complex* w, int* j, int n);
void fft6(Complex* a, Complex* f, Complex* w, int* j, int n);
void fft8(Complex* a, Complex* f, Complex* w, int* j, int n);
void fft(Complex* a, Complex* f, Complex* w, int* j, int p, int n);

int optim(int n, int& p, int& m);

class FFT {
public:
  int n;
  int *j;
  int p, m;
  Complex* w;

	FFT(int pp = 2, int nn = 1);
  ~FFT();
	FFT& operator = (const FFT&);
	void d(Complex* a, Complex* f) const {fft(a,f,w,j,p,n);}
	void i(Complex* a, Complex* f) const {fft(a,f,w,j,p,-n);} 
};

#endif

