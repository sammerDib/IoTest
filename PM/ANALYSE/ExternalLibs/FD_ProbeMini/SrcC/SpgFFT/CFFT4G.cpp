{
	complex b,c,d;

	b.re = DFX[0].re + DFX[DFASTEP].im - DFX[2*DFASTEP].re - DFX[3*DFASTEP].im;
	b.im = DFX[0].im - DFX[DFASTEP].re - DFX[2*DFASTEP].im + DFX[3*DFASTEP].re;

	c.re = DFX[0].re - DFX[DFASTEP].re + DFX[2*DFASTEP].re - DFX[3*DFASTEP].re;
	c.im = DFX[0].im - DFX[DFASTEP].im + DFX[2*DFASTEP].im - DFX[3*DFASTEP].im;

	d.re = DFX[0].re - DFX[DFASTEP].im - DFX[2*DFASTEP].re + DFX[3*DFASTEP].im;
	d.im = DFX[0].im + DFX[DFASTEP].re - DFX[2*DFASTEP].im - DFX[3*DFASTEP].re;

	DFX[0].re = DFX[0].re + DFX[DFASTEP].re + DFX[2*DFASTEP].re + DFX[3*DFASTEP].re;
	DFX[0].im = DFX[0].im + DFX[DFASTEP].im + DFX[2*DFASTEP].im + DFX[3*DFASTEP].im;

	if(DFTSTEP == 0){
		DFX[2*DFASTEP] = b;
		DFX[DFASTEP] = c;
		DFX[3*DFASTEP] = d;
	} else {
		DFX[2*DFASTEP].re = b.re*COS(DFTSTEP) - b.im*SIN(DFTSTEP);
		DFX[2*DFASTEP].im = b.im*COS(DFTSTEP) + b.re*SIN(DFTSTEP);
		DFX[DFASTEP].re = c.re*COS(2*DFTSTEP) - c.im*SIN(2*DFTSTEP);
		DFX[DFASTEP].im = c.im*COS(2*DFTSTEP) + c.re*SIN(2*DFTSTEP);
		DFX[3*DFASTEP].re = d.re*COS(3*DFTSTEP) - d.im*SIN(3*DFTSTEP);
		DFX[3*DFASTEP].im = d.im*COS(3*DFTSTEP) + d.re*SIN(3*DFTSTEP);
	}
}