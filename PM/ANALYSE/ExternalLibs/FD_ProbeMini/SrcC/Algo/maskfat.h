
#ifdef SPG_General_USEUnwrap

#ifndef __MASKFAT
#define __MASKFAT
void FattenMask(unsigned char *mask, int mask_code, int thickness,
                int xsize, int ysize);
void FattenQual(float *qualmap, int thickness, int xsize,
                int ysize);
#endif

#endif

