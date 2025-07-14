#if !defined(SORT_FCT_H__INCLUDED_)
#define SORT_FCT_H__INCLUDED_

#include "h3matrix.h"
#include "FindSquare.h"

bool MySortFunc(H3_ARRAY_PT2DFLT32 &Pts,H3_ARRAY2D_PT2DFLT32 &Pts2,long Nx,long Ny,long maxx, long maxy);
bool MySortFunc3(H3_ARRAY_PT2DFLT32 &Pts,H3_ARRAY2D_FLT32 &ComputedPos,long Nx,long Ny,long maxx, long maxy);

#endif