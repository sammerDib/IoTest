
#include "SPG_General.h"

#ifdef SPG_General_USERingBuffer

#include "SPG_Includes.h"
#include <memory.h>

//cree un ring buffer
int SPG_CONV RING_Create(PG_RINGBUFFER& R, int BufferLen)
{
	SPG_ZeroStruct(R);
	R.BufferLen=BufferLen;
	CHECK((R.M=SPG_TypeAlloc(BufferLen+4,BYTE,"RING Buffer"))==0,"RING_Create: Allocation echouee",return 0);
	return R.Etat=-1;
}

void SPG_CONV RING_Close(PG_RINGBUFFER& R)
{
	if (R.M) SPG_MemFree(R.M);
	memset(&R,0,sizeof(PG_RINGBUFFER));
	return;
}

int SPG_CONV RING_CanWrite(PG_RINGBUFFER& R)
{
	CHECK(R.Etat==0,"RING_CanWrite",return 0);
	int N=R.BufferLen-1-R.WBytePos+R.RBytePos;
	V_Wrap(N,R.BufferLen);
	return N;
}

int SPG_CONV RING_WriteBytes(PG_RINGBUFFER& R, BYTE* Mem, int Len)
{
	CHECK(R.Etat==0,"RING_WriteBytes",return 0);
	//CHECK(Len>RING_CanWrite(R),"Depassement RING Buffer",RING_WriteBytes(R,Mem,RING_CanWrite(R));return);
	R.WBytePos+=(R.WBitPos+7)>>3;
	R.WBitPos=0;

	CHECK(Len>RING_CanWrite(R),"RING_WriteBytes: Depassement RING Buffer",return 0);

	int LMax=(R.BufferLen-R.WBytePos);
	if(Len>LMax)
	{
		//SPG_Memcpy(R.M+R.WBytePos,Mem,LMax);
		memcpy(R.M+R.WBytePos,Mem,LMax);
		Mem+=LMax;
		Len-=LMax;
		R.WBytePos=0;
	}
	//SPG_Memcpy(R.M+R.WBytePos,Mem,Len);
	memcpy(R.M+R.WBytePos,Mem,Len);
	R.WBytePos+=Len;
	return Len;
}

int SPG_CONV RING_CanRead(PG_RINGBUFFER& R)
{
	CHECK(R.Etat==0,"RING_CanRead",return 0);
	int N=R.WBytePos-R.RBytePos;
	V_Wrap(N,R.BufferLen);
	return N;
}

int SPG_CONV RING_ReadBytes(PG_RINGBUFFER& R, BYTE* Mem, int Len)
{
	CHECK(R.Etat==0,"RING_ReadBytes",return 0);
	//CHECK(Len>RING_CanRead(R),"Depassement RING Buffer",memset(Mem,0,Len);RING_ReadBytes(R,Mem,RING_CanRead(R));return);
	R.RBytePos+=(R.RBitPos+7)>>3;
	R.RBitPos=0;

	CHECK(Len>RING_CanRead(R),"RING_ReadBytes: Depassement RING Buffer",return 0);

	int LMax=(R.BufferLen-R.RBytePos);
	if(Len>LMax)
	{
		//SPG_Memcpy(Mem,R.M+R.RBytePos,LMax);
		memcpy(Mem,R.M+R.RBytePos,LMax);
		Mem+=LMax;
		Len-=LMax;
		R.RBytePos=0;
	}
	//SPG_Memcpy(Mem,R.M+R.RBytePos,Len);
	memcpy(Mem,R.M+R.RBytePos,Len);
	R.RBytePos+=Len;
	return Len;
}

void SPG_FASTCONV RING_WriteBits(PG_RINGBUFFER& R, DWORD M,int NombreDeBits)
{
	CHECK(R.Etat==0,"RING_WriteBits",return);
	//CHECK(Len>RING_CanRead(R),"Depassement RING Buffer",memset(Mem,0,Len);RING_ReadBytes(R,Mem,RING_CanRead(R));return);
	CHECK(4>RING_CanWrite(R),"RING_WriteBits: Depassement RING Buffer",return);

	if(R.WBytePos<R.BufferLen-4)
	{
	*(DWORD*)(R.M+R.WBytePos)&=(1<<R.WBitPos)-1;//masque ce qui n'est pas encore ecrit
	*(DWORD*)(R.M+R.WBytePos)|=(M<<R.WBitPos);//ecrit plus qu'il n'en faut (normal)
	R.WBitPos+=NombreDeBits;
	R.WBytePos+=(R.WBitPos>>3);
	R.WBitPos&=7;
	}
	else
	{
		DWORD RW=0;
		int x;
		for(x=R.WBytePos;x<R.WBytePos+4;x++)
		{
			RW<<=8;
			RW|=R.M[V_WrapUp(x,R.BufferLen)];
		}
	RW&=(1<<R.WBitPos)-1;//masque ce qui n'est pas encore ecrit
	RW|=(M<<R.WBitPos);//ecrit plus qu'il n'en faut (normal)
		for(x=R.WBytePos;x<R.WBytePos+4;x++)
		{
			R.M[V_WrapUp(x,R.BufferLen)]=(BYTE)RW;
			RW>>=8;
		}
	R.WBitPos+=NombreDeBits;
	R.WBytePos+=(R.WBitPos>>3);
	R.WBitPos&=7;
	}

	return;
}

DWORD SPG_FASTCONV RING_ReadBits(PG_RINGBUFFER& R,int NombreDeBits)
{
	CHECK(R.Etat==0,"RING_ReadBits",return 0);
	CHECK(4>RING_CanRead(R),"RING_ReadBits: Depassement RING Buffer",return 0);
	DWORD BitsArray;
	if(R.WBytePos<R.BufferLen-4)
	{
	BitsArray=(*((DWORD*)(R.M+R.RBytePos)))>>R.RBitPos;
	R.RBitPos=R.RBitPos+NombreDeBits;
	R.RBytePos+=(R.RBitPos>>3);
	R.RBitPos&=7;
	}
	else
	{
		DWORD RW=0;
		for(int x=R.RBytePos;x<R.RBytePos+4;x++)
		{
			RW<<=8;
			RW|=R.M[V_WrapUp(x,R.BufferLen)];
		}
		BitsArray=RW>>R.RBitPos;
	}

	return (BitsArray&((((DWORD)1)<<NombreDeBits)-(DWORD)1));
}





#endif

