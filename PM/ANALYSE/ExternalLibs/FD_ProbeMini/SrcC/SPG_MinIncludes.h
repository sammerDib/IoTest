#ifdef SPG_MinIncludesIncluded
#error "SPG_MinIncludes Included twice"
#endif
#define	SPG_MinIncludesIncluded

#include "SPG_InhibitedFunctions.h"

#include "Config\SPG_DefineList.h"

#include "V_General.h"

#include "SPG_SpecMacro.h"

#include "SPG_List.h"

#include "SPG_StructVersionCheck.h"

#ifdef SPG_General_USEBRES
#include "Bres.h"
#endif

#ifdef SPG_General_USEGraphics
#include "SPG_Graphics.h"
#endif

#ifdef SPG_General_USEGEFFECT
#include "SPG_Graphics_Effect.h"
#endif

#ifdef SPG_General_USESGRAPH
#include "SGRAPH.h"
#include "V_ConvertToGraph.h"
#endif

#ifdef SPG_General_USECarac
#include "SPG_Carac.h"
#endif

#ifdef SPG_General_USECaracF
#include "SPG_CaracFormatter.h"
#endif

//fdef SPG_General_USETimer
#include "SPG_Timer.h"
//ndif

//fdef SPG_General_USESpinLock
#include "SpinLock.h"
//ndif

#ifdef SPG_General_USEConsole
#include "SPG_Console.h"
#endif

#ifdef SPG_General_USEScale
#include "SPG_Scale.h"
#endif

#include "SPG_Mem_Type.h"
#ifndef SPG_General_USEGlobal
#include "SPG_Mem.h"
#endif

#include "SPG_String.h"

