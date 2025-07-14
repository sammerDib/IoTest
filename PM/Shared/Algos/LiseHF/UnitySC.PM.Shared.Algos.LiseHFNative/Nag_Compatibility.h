#ifndef NAG_COMPATIBILITY_H

#include <stdio.h>
#include <float.h>

#define PI 3.141592653589793238462643383279503
#define NUM_NOISE			1.0e-12   //"NUMERICAL_NOISE"; supposed least numerical accuracy - e.g. in some security queries; W.Iff, 1.3.2012
#define NUM_NOISE_FLOAT		1.0e-5 


//From Internals.h : 


//Makros: TESTSUCCESS   "{" muss hier in selber Zeile wie #define stehen!! 
#define TESTSUCCES_NAG_COMPATIBILITY(Text) {						\
	if (ireturn_status != 0)										\
	{																\
		sprintf_s( szText, iNoChar*sizeof(char), #Text );     		\
		iexit_status = 1;											\
		goto END;													\
	}																\
}



//From Compatibility.h : 


#define FALSE 0
#define TRUE 1

/* Typedefs usw */

/* BOOL */
typedef int BOOL;  //is already defined in windows.h

/* CComplex */
typedef struct {
	double re;
	double im;
} CComplex;

/* Integer */
typedef int Integer;

#define max(a,b)    (((a) > (b)) ? (a) : (b)) //since inclusion of <stdlib.h> failed
#define min(a,b)    (((a) < (b)) ? (a) : (b))


#define NAG_COMPATIBILITY_H
#endif