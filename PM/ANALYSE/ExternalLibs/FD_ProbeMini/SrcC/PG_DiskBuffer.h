
#ifdef SPG_General_USEDiskBuffer

typedef struct
{

	int Etat;
	int FileLen;
	int BufferLen;
	int BufferPosInFile;
	int BytePos;
	int BitPos;
	BYTE *M;
	void *F;
} PG_DISKBUFFER;

#define PG_DB_READ 1
#define PG_DB_WRITE 2
//#define PG_DB_FULLFILE 4

#include "PG_DiskBuffer.agh"

#define PG_PosInFile(PGDKB) (PGDKB.BufferPosInFile+PGDKB.BytePos)

#endif

