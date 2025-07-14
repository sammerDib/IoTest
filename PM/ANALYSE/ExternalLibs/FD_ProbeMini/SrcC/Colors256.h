
#ifdef SPG_General_USEColors256
/*
PixCoul * C256_Init(int Gradient,int MA, int MB);
void C256_Close(PixCoul*BC);
int C256_FillTexture(SG_FullTexture& T, PixCoul*BC);
void C256_MakeFaceColor(SG_Bloc& B,int SizeX,int SizeY, int TSX, int TSY, V_VECT Light);
*/

#include "Colors256.agh"

//-1<L<1
//#define C256_CalcLight(L) ((L+1)*(L+1)*(L+1)*(L+1)*(L+1)*(L+1)*(L+1)/128)
#define C256_CalcLight(L) ((L+1)*(L+1)*(L+1)*(L+1)/16)
//#define C256_CalcLight(L) (L+1)/2

#endif

