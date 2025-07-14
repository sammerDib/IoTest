enum
{
	PGL_OK=0,
	PGLERR_BADPARAMS,
	PGLERR_NULLPTR,
	PGLERR_CANTCREATE,
	PGLERR_CANTLOAD,
	PGLERR_LOCKED,
	PGLERR_FAILED
};

extern const char* PGLError[];

#define pglFree(ptr) {if(ptr) _pglFree(ptr);ptr=NULL;}

void PGL_CONV pglSetLastError(ulong);
ulong PGL_CONV pglGetLastError(void);
void PGL_CONV pglMessage(PGLCommon* common,char* m,char* t);
void* PGL_CONV pglAlloc(char* descr,ulong size);
void PGL_CONV _pglFree(void* ptr);
void PGL_CONV pglMemCheck(uchar*);
void PGL_CONV pglErrorCheck(ubool t);
void PGL_CONV pglAllFreed(void);
ubool PGL_CONV pglAskMessage(PGLCommon* common,char* m,char* t);
