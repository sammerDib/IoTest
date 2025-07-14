

#include "SPG_General.h"

#ifdef SPG_General_USESGRAPH

#include "SPG_InhibitedFunctions.h"

#include "V_General.h"
#include "SPG_List.h"
#include "SPG_Graphics.h"
#include "SGRAPH.h"

#ifdef DebugFloat
#include <float.h>
#endif


#define AvgDiffX(Face) V_Operate4V(Face.NumP2->P,-Face.NumP1->P,+Face.NumP3->P,-Face.NumP4->P)
#define AvgDiffY(Face) V_Operate4V(Face.NumP4->P,-Face.NumP1->P,+Face.NumP3->P,-Face.NumP2->P)

void SPG_CONV SG_CalcNormales(SG_FullBloc& B)
{
#ifdef SGE_EMC
	for(int i=0;i<B.DB.NombreF;i++)
	{
#define Face B.DB.MemFaces[i]
				V_VECT DiffX=AvgDiffX(Face);
				V_VECT DiffY=AvgDiffY(Face);
				V_VectVect(DiffX,DiffY,Face.Normale);
				V_NormaliseSafe(Face.Normale);
#undef Face
	}
#endif
	return;
}

void SPG_CONV SG_CalcNormalesSpherical(SG_FullBloc& B,V_VECT Pos)
{
#ifdef SGE_EMC
	for(int i=0;i<B.DB.NombreF;i++)
	{
#define Face B.DB.MemFaces[i]
		V_VECT FCG;
		SG_FaceCG(FCG,Face);
		V_SetXYZ(Face.Normale,FCG.x-Pos.x,FCG.y-Pos.y,FCG.z-Pos.z);
		V_NormaliseSafe(Face.Normale);
#undef Face
	}
#endif
	return;
}

void SPG_CONV SG_CalcNormalesToExt(SG_FullBloc& B,V_VECT Pos)
{
#ifdef SGE_EMC
	for(int i=0;i<B.DB.NombreF;i++)
	{
#define Face B.DB.MemFaces[i]
				V_VECT DiffX=AvgDiffX(Face);
				V_VECT DiffY=AvgDiffY(Face);
				V_VectVect(DiffX,DiffY,Face.Normale);
				V_NormaliseSafe(Face.Normale);
				V_VECT DiffPos={Face.NumP1->P.x-Pos.x,Face.NumP1->P.y-Pos.y,Face.NumP1->P.z-Pos.z};
				float SigneProj;
				V_ScalVect(Face.Normale,DiffPos,SigneProj);
				if (SigneProj<0)
				{
					V_Operate2(Face.Normale,=-Face.Normale);
				}
#undef Face
	}
#endif
	return;
}

void SPG_CONV SG_CalcNormalesToDir(SG_FullBloc& B,V_VECT Dir)
{
#ifdef SGE_EMC
	for(int i=0;i<B.DB.NombreF;i++)
	{
#define Face B.DB.MemFaces[i]
				V_VECT DiffX=AvgDiffX(Face);
				V_VECT DiffY=AvgDiffY(Face);
				V_VectVect(DiffX,DiffY,Face.Normale);
				V_NormaliseSafe(Face.Normale);
				float SigneProj;
				V_ScalVect(Face.Normale,Dir,SigneProj);
				if (SigneProj<0)
				{
					V_Operate2(Face.Normale,=-Face.Normale);
				}
#undef Face
	}
#endif
	return;
}

void SPG_CONV SG_CalcNormalesPreExist(SG_FullBloc& B)
{
#ifdef SGE_EMC
	for(int i=0;i<B.DB.NombreF;i++)
	{
#define Face B.DB.MemFaces[i]
				V_VECT DiffX=AvgDiffX(Face);
				V_VECT DiffY=AvgDiffY(Face);
				V_VECT NewNormale;
				V_VectVect(DiffX,DiffY,NewNormale);
				V_NormaliseSafe(NewNormale);
				float SigneProj;
				V_ScalVect(NewNormale,Face.Normale,SigneProj);
				if (SigneProj<0)
				{
					V_Operate2(Face.Normale,=-NewNormale);
				}
				else
				{
					V_Operate2(Face.Normale,=NewNormale);
				}
#undef Face
	}
#endif
	return;
}

#endif



