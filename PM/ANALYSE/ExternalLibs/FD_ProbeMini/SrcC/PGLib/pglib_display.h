typedef struct
{
	ulong width;
	ulong height;
	uchar bpp;
	uchar zbpp;
	uchar abpp;
	uchar sbpp;
	uchar accbpp;
	ulong flags;
	ulong x;
	ulong y;
	void* internal;
	PGLClearMode cmode;
} PGLDisplay;

typedef struct
{
	ulong width;
	ulong height;
	ulong bpp;
	ulong pitch;
	ulong flags;
	uchar* bits;
	ubool lock;
	void* internal;
} PGLSurface;

typedef struct
{
	ulong width;
	ulong height;
	uchar bpp;
	uchar zbpp;
	uchar abpp;
	uchar sbpp;
	uchar accbpp;
	ulong flags;
	char title[128];
} PGLDisplayParams;

const ulong PGL_OPENGL=1;
const ulong PGL_2D=2;
const ulong PGL_3D=4;
const ulong PGL_SIMPLE=8;
const ulong PGL_FULLSCREEN=16;

PGLSurface* PGL_CONV pglCreateSurface(PGLDisplay*,ulong,ulong,ushort,ulong);
void PGL_CONV pglDestroySurface(PGLSurface*);
void PGL_CONV pglBlitSurface(PGLDisplay*,PGLSurface*,PGLSurface*,ulong,ulong,ulong,ulong,ulong,ulong,ulong);
ubool PGL_CONV pglReloadBitmap(PGLDisplay*,PGLSurface*,char*);
PGLSurface* PGL_CONV pglLoadBitmap(PGLDisplay*,ushort,char*);
void PGL_CONV pglSwapBuffers(PGLDisplay*);
uchar* PGL_CONV pglLockSurface(PGLSurface*);
void PGL_CONV pglUnlockSurface(PGLSurface*);

const ulong PSURF_ANYFORMAT=1;
