{
	complex b,c,d;//tmp[3];

	b.re = DFX[0].re - DFX[1].re + DFX[2].re - DFX[3].re;
	b.im = DFX[0].im - DFX[1].im + DFX[2].im - DFX[3].im;

	c.re = DFX[0].re + DFX[1].im - DFX[2].re - DFX[3].im;
	c.im = DFX[0].im - DFX[1].re - DFX[2].im + DFX[3].re;

	d.re = DFX[0].re - DFX[1].im - DFX[2].re + DFX[3].im;
	d.im = DFX[0].im + DFX[1].re - DFX[2].im - DFX[3].re;

	DFX[0].re = DFX[0].re + DFX[1].re + DFX[2].re + DFX[3].re;
	DFX[0].im = DFX[0].im + DFX[1].im + DFX[2].im + DFX[3].im;

	DFX[1] = b;
	DFX[2] = c;
	DFX[3] = d;
}