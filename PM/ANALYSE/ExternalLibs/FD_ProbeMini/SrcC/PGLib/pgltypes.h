typedef unsigned long ulong;
typedef unsigned int uint;
typedef unsigned short ushort;
typedef unsigned char uchar;
typedef uchar ubool;

const ubool utrue=1;
const ubool ufalse=0;

typedef union
{
	struct
	{
		uchar r;
		uchar g;
		uchar b;
		uchar a;
	};
	ulong c;
} PGLColor;

typedef union
{
	struct
	{
		float x;
		float y;
		float z;
	};
	float v[3];
} PGLVector;

typedef struct
{
	float u;
	float v;
} PGLTexCoord;

typedef union
{
	float ml[16];
	float mb[4][4];
	struct
	{
		float _11;float _21;float _31;float _41;
		float _12;float _22;float _32;float _42;
		float _13;float _23;float _33;float _43;
		float _14;float _24;float _34;float _44;
	};
} PGLMatrix;

typedef struct
{
	PGLVector pos;
	PGLColor c;
	PGLTexCoord tc;
} PGLVertex;

typedef enum {PGLCLEAR_STD,PGLCLEAR_QUAD} PGLClearMode;
typedef enum {PGL_NEVER,PGL_LESS,PGL_EQUAL,PGL_LEQUAL,PGL_GREATER,PGL_NOTEQUAL,PGL_GEQUAL,PGL_ALWAYS} PGLDepthFunc;
typedef enum {PGL_POINTS,PGL_LINES,PGL_LINE_LOOP,PGL_QUADS,PGL_QUAD_STRIP,PGL_TRIANGLES,PGL_TRIANGLE_FAN,PGL_TRIANGLE_STRIP} PGLPrimitive;
typedef enum {PGL_LUMINANCE,PGL_INTENSITY,PGL_RGB,PGL_RGBA,PGL_RGBA4,PGL_RGB5_A1,PGL_RGB8,PGL_RGBA8,PGL_RGB10_A2} PGLPixelFormat;
typedef enum {PGLBLEND_NONE,PGLBLEND_ALPHA,PGLBLEND_TRANSPARENT} PGLBlend;
typedef enum {PGL_NEAREST,PGL_LINEAR,PGL_NEAREST_MIPMAP_NEAREST,PGL_NEAREST_MIPMAP_LINEAR,PGL_LINEAR_MIPMAP_NEAREST,PGL_LINEAR_MIPMAP_LINEAR} PGLFilterMode;
