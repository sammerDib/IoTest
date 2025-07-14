
#pragma message("PGLib include")

#ifdef _WIN32
#include <windows.h>
#endif
#include <GL/gl.h>

#include <stdlib.h>
#include <stdio.h>

#ifdef _DEBUG
#include <crtdbg.h>
#endif

#define PGL_CONV __cdecl

#include "pgltypes.h"
#include "pglib_display.h"
#include "pglib_wm.h"
#include "pglib_sound.h"
#include "pglib_input.h"
#include "pglib_3d.h"
#include "pglib_std.h"
#include "pglib_math.h"
