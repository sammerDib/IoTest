typedef struct
{
	ubool quit;
	ulong state;
	char appname[64];
	ubool checkrestore;
	PGLDisplay* display;
	struct
	{
		ulong x;
		ulong y;
		ulong z;
		ulong b;
	} joy;
	uchar kb[256];
	struct
	{
		int x;
		int y;
		uchar b[3];
	} mouse;
} PGLCommon;

const ulong PGLSTATE_WAIT=0;
const ulong PGLSTATE_RUN=1;
const ulong PGLSTATE_PAUSE=2;

ulong PGL_CONV pglDoEvents(PGLCommon*);
PGLCommon* PGL_CONV pglCreateCommon(char*);
PGLDisplay* PGL_CONV pglCreateDisplay(PGLCommon*,PGLDisplayParams*);
PGLDisplay* PGL_CONV pglAttachDisplayToCommon(PGLCommon*,PGLDisplay*);
void PGL_CONV pglDestroyCommon(PGLCommon*);
void PGL_CONV pglDestroyDisplay(PGLDisplay*);
ulong PGL_CONV pglTickCount(void);

ulong PGL_CONV pglGetLastError(void);
void PGL_CONV pglMessage(PGLCommon* common,char* m,char* t);

#ifdef PGL_USEEXTENSIONS
extern PFNGLDRAWARRAYSEXTPROC glDrawArraysEXT;
extern PFNGLVERTEXPOINTEREXTPROC glVertexPointerEXT;
extern PFNGLCOLORPOINTEREXTPROC glColorPointerEXT;
extern PFNGLTEXCOORDPOINTEREXTPROC glTexCoordPointerEXT;
extern PFNGLNORMALPOINTEREXTPROC glNormalPointerEXT;

#ifdef WIN32
	typedef void (APIENTRY * PFNGLLOCKARRAYSPROC) (GLint first, GLsizei count);
	typedef void (APIENTRY * PFNGLUNLOCKARRAYSPROC) (void);
#else
	typedef void (* PFNGLLOCKARRAYSPROC) (GLint first, GLsizei count);
	typedef void (* PFNGLUNLOCKARRAYSPROC) (void);
#endif

extern PFNGLLOCKARRAYSPROC glLockArraysEXT;
extern PFNGLUNLOCKARRAYSPROC glUnlockArraysEXT;
extern PFNGLARRAYELEMENTEXTPROC glArrayElementEXT;
#endif

extern ubool _PGLUseExtensions;
