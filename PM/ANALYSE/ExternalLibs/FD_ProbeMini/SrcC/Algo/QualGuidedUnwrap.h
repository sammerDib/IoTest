
#ifdef SPG_General_USEUnwrap

#include "GetQual.h"

void QualGuidedUnwrap(float*phase, float*soln, float*qual_map,unsigned char* bitflags, int xsize, int ysize, UnwrapMode mode);
/*
void GradientQualGuidedUnwrap(float*phase, float*soln, float*qual_map,unsigned char* bitflags, int xsize, int ysize);
void VarianceQualGuidedUnwrap(float*phase, float*soln, float*qual_map,unsigned char* bitflags, int xsize, int ysize);
*/
#define GradientQualGuidedUnwrap(phase,soln,qual_map,bitflags,xsize,ysize) QualGuidedUnwrap(phase,soln,qual_map,bitflags,xsize,ysize,gradient)
#define VarianceQualGuidedUnwrap(phase,soln,qual_map,bitflags,xsize,ysize) QualGuidedUnwrap(phase,soln,qual_map,bitflags,xsize,ysize,variance) 

#endif

