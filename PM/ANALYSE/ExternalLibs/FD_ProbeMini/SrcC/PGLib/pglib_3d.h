typedef struct
{
	uint name;
	uint width;
	uint height;
	PGLFilterMode filter;
	PGLPixelFormat format;
} PGLTexture;

typedef struct
{
	PGLColor color;

	ulong nVertices;
	float* Vertices;
	ulong strideV;

	ulong nColors;
	PGLColor* Colors;
	ulong strideC;

	ulong nTexCoords;
	float* TexCoords;
	ulong strideTC;

	ulong nNormals;
	float* Normals;
	ulong strideN;

	ulong nIndexes;
	ushort* Indexes;

	ushort* stripIndexes;
	ushort strip_length[128];
	ushort rest_tri;
	ushort nr_strips;

	PGLTexture* texture;
	ubool linked;
	ubool compiled;
	ubool stripped;
	ubool modulate;

	PGLPrimitive p;
	PGLBlend blend;
} PGLBloc;

PGLTexture* PGL_CONV pglCreateTexture(PGLPixelFormat,PGLSurface*,PGLFilterMode);
void PGL_CONV pglUseTexture(PGLTexture*);
void PGL_CONV pglDestroyTexture(PGLTexture*);
PGLTexture* PGL_CONV pglLoadTexture(PGLDisplay*,PGLPixelFormat,char*,uchar,PGLFilterMode);

void PGL_CONV pglProjectionLoadIdentity(void);
void PGL_CONV pglModelLoadIdentity(void);
void PGL_CONV pglLookAt(double,double,double,double,double,double,double,double,double);
void PGL_CONV pglPerspective(double,double,double,double);
void PGL_CONV pglRotate(float angle,float x,float y,float z);
void PGL_CONV pglTranslate(float x,float y,float z);

PGLBloc* PGL_CONV pglCreateBloc(PGLPrimitive p);
PGLBloc* PGL_CONV pglCreateLinkedBloc(PGLPrimitive p);
void PGL_CONV pglBlocVertexPointer(PGLBloc* bloc,ulong nVertices,float* Vertices,ulong strideV);
void PGL_CONV pglBlocColorPointer(PGLBloc* bloc,ulong nColors,PGLColor* Colors,ulong strideC);
void PGL_CONV pglBlocTexCoordPointer(PGLBloc* bloc,ulong nTexCoords,float* TexCoords,ulong strideTC);
void PGL_CONV pglBlocPixTexCoordPointer(PGLBloc* bloc,ulong nTexCoords,ushort* TexCoords,ulong strideTC);
void PGL_CONV pglBlocNormalPointer(PGLBloc* bloc,ulong nNormals,float* Normals,ulong strideN);
void PGL_CONV pglBlocIndexPointer(PGLBloc* bloc,ulong nIndexes,ushort* Indexes);
void PGL_CONV pglDestroyBloc(PGLBloc*);
void PGL_CONV pglBlocTexture(PGLBloc*,PGLTexture*);
void PGL_CONV pglBlocColor(PGLBloc*,PGLColor);
void PGL_CONV pglRenderBlocs(ulong,PGLBloc**);
void PGL_CONV pglClear(ulong);
void PGL_CONV pglFlush(void);
void PGL_CONV pglTexturePriority(PGLTexture* texture,uchar prio);
void PGL_CONV pglBlocBlend(PGLBloc* bloc,PGLBlend b);
void PGL_CONV pglClearColor(PGLColor);
void PGL_CONV pglClearDepth(float);
void PGL_CONV pglBlocSort(PGLBloc* bloc);
void PGL_CONV pglBlocCompile(PGLBloc* bloc);
void PGL_CONV pglBlocTriangles(PGLBloc* bloc);
void PGL_CONV pglBlocModulate(PGLBloc* bloc,ubool modulate);
void PGL_CONV pglBackground(PGLTexture* texture);
void PGL_CONV pglDepthMode(ubool enable,ubool mask,PGLDepthFunc dtest);

//pglClear
const ulong PGL_COLOR_BUFFER_BIT=0x01;
const ulong PGL_DEPTH_BUFFER_BIT=0x02;
const ulong PGL_ACCUM_BUFFER_BIT=0x04;
const ulong PGL_STENCIL_BUFFER_BIT=0x08;
