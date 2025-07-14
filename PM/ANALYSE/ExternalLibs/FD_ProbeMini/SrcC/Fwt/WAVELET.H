
#ifdef SPG_General_USEFWT

void SPG_CONV daub4(float* a,float* wksp,unsigned long n,int isign);
void SPG_CONV pwtset(int n);
void SPG_CONV pwt(float* a,float* wksp,unsigned long n,int isign);
/*
void wtransform(float* a,float* wksp,unsigned long n,int isign,
		 void (*wtstep)(float*,float*,unsigned long,int));
*/
#endif
