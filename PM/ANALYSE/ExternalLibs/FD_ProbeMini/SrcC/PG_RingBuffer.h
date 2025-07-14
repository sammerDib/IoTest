
#ifdef SPG_General_USERingBuffer

typedef struct
{
	int Etat;
	int BufferLen;
	int WBytePos;
	int WBitPos;
	int RBytePos;
	int RBitPos;
	BYTE* M;
} PG_RINGBUFFER;

int SPG_CONV RING_Create(PG_RINGBUFFER& R, int BufferLen);
void SPG_CONV RING_Close(PG_RINGBUFFER& R);
int SPG_CONV RING_CanWrite(PG_RINGBUFFER& R);
int SPG_CONV RING_WriteBytes(PG_RINGBUFFER& R, BYTE* Mem, int Len);
int SPG_CONV RING_CanRead(PG_RINGBUFFER& R);
int SPG_CONV RING_ReadBytes(PG_RINGBUFFER& R, BYTE* Mem, int Len);
void SPG_FASTCONV RING_WriteBits(PG_RINGBUFFER& R, DWORD M,int NombreDeBits);
DWORD SPG_FASTCONV RING_ReadBits(PG_RINGBUFFER& R,int NombreDeBits);

#endif

