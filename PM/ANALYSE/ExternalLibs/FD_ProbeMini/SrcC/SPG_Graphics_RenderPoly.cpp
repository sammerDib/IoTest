
#include "SPG_General.h"

#ifdef SPG_General_USEGraphics
#ifdef SPG_General_USEGraphicsRenderPoly

#include "SPG_Includes.h"
#ifdef SPG_General_USEWindows
#include "SPG_SysInc.h"
#endif

#include <string.h>


//#define DebugRenderSegment
//#undef DebugRenderSegment


#pragma SPGMSG(__FILE__,__LINE__,"G_DrawVoidPoly4")
void SPG_CONV G_DrawVoidPoly4(const G_Ecran& E, const G_PixCoord P[4], const PixCoul Coul)
{
#ifdef DebugGraphics
	CHECK(E.MECR==0,"G_DrawPoly: G_Ecran vide",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"G_DrawPoly: G_Ecran inaccessible",return);
#endif
#ifdef DebugGraphicsTimer
	S_StartTimer(Global.T_GraphicsRender);
#endif
	
#define NPoints 4
#define NextPoint(x) (x+1)&3
#define PrevPoint(x) (x-1)&3
#define ToPoint(x,n) (x+n)&3
	
#include "InlinePrepareSegment.cpp"
switch(E.POCT)
{
case 4:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 0
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 0
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 3:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 0
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 0
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 2:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 0
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 0
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 1:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 0
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 0
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}

#pragma SPGMSG(__FILE__,__LINE__,"G_DrawUniPoly4")
void SPG_CONV G_DrawUniPoly4(const G_Ecran& E, const G_PixCoord P[4], const PixCoul Coul)
{
#ifdef DebugGraphics
	CHECK(E.MECR==0,"G_DrawPoly: G_Ecran vide",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"G_DrawPoly: G_Ecran inaccessible",return);
#endif
#ifdef DebugGraphicsTimer
	S_StartTimer(Global.T_GraphicsRender);
#endif
	
#define NPoints 4
#define NextPoint(x) (x+1)&3
#define PrevPoint(x) (x-1)&3
#define ToPoint(x,n) (x+n)&3
	
#include "InlinePrepareSegment.cpp"
switch(E.POCT)
{
case 4:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 1
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 1
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 3:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 1
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 1
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 2:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 1
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 1
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 1:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 1
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 1
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}

#pragma SPGMSG(__FILE__,__LINE__,"G_DrawUniTranslPoly4")
void SPG_CONV G_DrawUniTranslPoly4(const G_Ecran& E, const G_PixCoord P[4], const PixCoul Coul)
{
#ifdef DebugGraphics
	CHECK(E.MECR==0,"G_DrawPoly: G_Ecran vide",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"G_DrawPoly: G_Ecran inaccessible",return);
#endif
#ifdef DebugGraphicsTimer
	S_StartTimer(Global.T_GraphicsRender);
#endif
	
#define NPoints 4
#define NextPoint(x) (x+1)&3
#define PrevPoint(x) (x-1)&3
#define ToPoint(x,n) (x+n)&3
	
#include "InlinePrepareSegment.cpp"
switch(E.POCT)
{
case 4:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 2
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 2
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 3:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 2
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 2
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 2:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 2
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 2
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 1:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 2
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 2
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}

#pragma SPGMSG(__FILE__,__LINE__,"G_DrawUniLightPoly4")
void SPG_CONV G_DrawUniLightPoly4(const G_Ecran& E, const G_PixCoord P[4], const PixCoul CoulFace, const PixCoul CoulLight)
{
#ifdef DebugGraphics
	CHECK(E.MECR==0,"G_DrawPoly: G_Ecran vide",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"G_DrawPoly: G_Ecran inaccessible",return);
#endif
#ifdef DebugGraphicsTimer
	S_StartTimer(Global.T_GraphicsRender);
#endif
	
#define NPoints 4
#define NextPoint(x) (x+1)&3
#define PrevPoint(x) (x-1)&3
#define ToPoint(x,n) (x+n)&3
	
#include "InlinePrepareSegment.cpp"

	PixCoul Coul;
	G_CombineFaceAndLight(Coul,CoulFace,CoulLight);
switch(E.POCT)
{
case 4:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 1
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 1
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 3:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 1
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 1
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 2:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 1
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 1
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 1:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 1
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 1
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}

#pragma SPGMSG(__FILE__,__LINE__,"G_DrawUniTranslLightPoly4")
void SPG_CONV G_DrawUniTranslLightPoly4(const G_Ecran& E, const G_PixCoord P[4], const PixCoul CoulFace, const PixCoul CoulLight)
{
#ifdef DebugGraphics
	CHECK(E.MECR==0,"G_DrawPoly: G_Ecran vide",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"G_DrawPoly: G_Ecran inaccessible",return);
#endif
#ifdef DebugGraphicsTimer
	S_StartTimer(Global.T_GraphicsRender);
#endif
	
#define NPoints 4
#define NextPoint(x) (x+1)&3
#define PrevPoint(x) (x-1)&3
#define ToPoint(x,n) (x+n)&3
	
#include "InlinePrepareSegment.cpp"

	PixCoul Coul;
	G_CombineFaceAndLight(Coul,CoulFace,CoulLight);
switch(E.POCT)
{
case 4:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 2
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 2
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 3:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 2
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 2
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 2:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 2
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 2
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 1:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 2
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 2
#undef DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}

#pragma SPGMSG(__FILE__,__LINE__,"G_DrawVoidTexPoly4")
void SPG_CONV G_DrawVoidTexPoly4(const G_Ecran& E, const G_PixCoord P[4], const G_TexCoord T[4], const G_Texture& DescrTex)
{
#ifdef DebugGraphics
	CHECK(E.MECR==0,"G_DrawPoly: G_Ecran vide",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"G_DrawPoly: G_Ecran inaccessible",return);
	CHECKPOINTER(DescrTex.MemTex,"G_DrawVoidTexPoly4: G_Texture invalide",PixCoul C;C.Coul=0x008000;G_DrawUniPoly4(E,P,C);return);
#endif
#ifdef DebugGraphicsTimer
	S_StartTimer(Global.T_GraphicsRender);
#endif
	
#define NPoints 4
#define NextPoint(x) (x+1)&3
#define PrevPoint(x) (x-1)&3
#define ToPoint(x,n) (x+n)&3
	
#include "InlinePrepareSegment.cpp"
switch(E.POCT)
{
case 4:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 0
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 0
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 3:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 0
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 0
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 2:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 0
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 0
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 1:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 0
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 0
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}

#pragma SPGMSG(__FILE__,__LINE__,"G_DrawTexPoly4")
void SPG_CONV G_DrawTexPoly4(const G_Ecran& E, const G_PixCoord P[4], const G_TexCoord T[4], const G_Texture& DescrTex)
{
#ifdef DebugGraphics
	CHECK(E.MECR==0,"G_DrawPoly: G_Ecran vide",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"G_DrawPoly: G_Ecran inaccessible",return);
	CHECKPOINTER(DescrTex.MemTex,"G_DrawVoidTexPoly4: G_Texture invalide",PixCoul C;C.Coul=0x008000;G_DrawUniPoly4(E,P,C);return);
#endif
#ifdef DebugGraphicsTimer
	S_StartTimer(Global.T_GraphicsRender);
#endif
	
#define NPoints 4
#define NextPoint(x) (x+1)&3
#define PrevPoint(x) (x-1)&3
#define ToPoint(x,n) (x+n)&3
	
#include "InlinePrepareSegment.cpp"
switch(E.POCT)
{
case 4:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 1
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 1
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 3:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 1
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 1
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 2:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 1
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 1
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 1:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 1
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 1
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}

#pragma SPGMSG(__FILE__,__LINE__,"G_DrawTexMaskPoly4")
void SPG_CONV G_DrawTexMaskPoly4(const G_Ecran& E, const G_PixCoord P[4], const G_TexCoord T[4], const G_Texture& DescrTex, const PixCoul Mask)
{
#ifdef DebugGraphics
	CHECK(E.MECR==0,"G_DrawPoly: G_Ecran vide",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"G_DrawPoly: G_Ecran inaccessible",return);
	CHECKPOINTER(DescrTex.MemTex,"G_DrawVoidTexPoly4: G_Texture invalide",PixCoul C;C.Coul=0x008000;G_DrawUniPoly4(E,P,C);return);
#endif
#ifdef DebugGraphicsTimer
	S_StartTimer(Global.T_GraphicsRender);
#endif
	
#define NPoints 4
#define NextPoint(x) (x+1)&3
#define PrevPoint(x) (x-1)&3
#define ToPoint(x,n) (x+n)&3
	
#include "InlinePrepareSegment.cpp"
switch(E.POCT)
{
case 4:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 5
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 5
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 3:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 5
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 5
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 2:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 5
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 5
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 1:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 5
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 5
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}

#pragma SPGMSG(__FILE__,__LINE__,"G_DrawTexTranslPoly4")
void SPG_CONV G_DrawTexTranslPoly4(const G_Ecran& E, const G_PixCoord P[4], const G_TexCoord T[4], const G_Texture& DescrTex)
{
#ifdef DebugGraphics
	CHECK(E.MECR==0,"G_DrawPoly: G_Ecran vide",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"G_DrawPoly: G_Ecran inaccessible",return);
	CHECKPOINTER(DescrTex.MemTex,"G_DrawVoidTexPoly4: G_Texture invalide",PixCoul C;C.Coul=0x008000;G_DrawUniPoly4(E,P,C);return);
#endif
#ifdef DebugGraphicsTimer
	S_StartTimer(Global.T_GraphicsRender);
#endif
	
#define NPoints 4
#define NextPoint(x) (x+1)&3
#define PrevPoint(x) (x-1)&3
#define ToPoint(x,n) (x+n)&3
	
#include "InlinePrepareSegment.cpp"
switch(E.POCT)
{
case 4:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 2
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 2
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 3:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 2
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 2
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 2:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 2
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 2
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 1:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 2
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 2
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}

#pragma SPGMSG(__FILE__,__LINE__,"G_DrawTexLightPoly4")
#ifdef UseAlphaPixCoul
void SPG_CONV G_DrawTexLightPoly4(const G_Ecran& E, const G_PixCoord P[4], const G_TexCoord T[4], const G_Texture& DescrTex, const PixCoul PreAlphaCouleurFaceLight)
{
#if 0
}
#endif
#else
void SPG_CONV G_DrawTexLightPoly4(const G_Ecran& E, const G_PixCoord P[4], const G_TexCoord T[4], const G_Texture& DescrTex, const PixCoul CouleurFaceLight)
{
#endif
#ifdef DebugGraphics
	CHECK(E.MECR==0,"G_DrawPoly: G_Ecran vide",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"G_DrawPoly: G_Ecran inaccessible",return);
	CHECKPOINTER(DescrTex.MemTex,"G_DrawVoidTexPoly4: G_Texture invalide",G_DrawUniPoly4(E,P,CouleurFaceLight);return);
#endif
#ifdef DebugGraphicsTimer
	S_StartTimer(Global.T_GraphicsRender);
#endif

#define NPoints 4
#define NextPoint(x) (x+1)&3
#define PrevPoint(x) (x-1)&3
#define ToPoint(x,n) (x+n)&3
	
#include "InlinePrepareSegment.cpp"

#ifdef UseAlphaPixCoul
	AlphaPixCoul CouleurFaceLight;
	G_ComputeAlphaPixCoul(CouleurFaceLight,PreAlphaCouleurFaceLight);
#endif
switch(E.POCT)
{
case 4:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 3
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 3
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 3:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 3
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 3
#define DEF_G_RENDERMODE 3
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 2:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 3
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 2
#define DEF_G_RENDERMODE 3
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
case 1:
	if (FastCopy)
	{
#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 3
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
	else
	{
#undef G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 1
#define DEF_G_RENDERMODE 3
#define DEF_G_TEXTURE
#include "InlineRenderSegmentX.cpp"
	}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}
#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_GraphicsRender);
#endif
	return;
}

#endif
#endif



