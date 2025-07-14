
//DO NOT COMPILE: INCLUDE FILE ONLY


#ifndef NPACK

#error NPACK non defini

#endif

#ifdef NPACK

#define MELINK_UNPACK_C

//#define RGB_UNPACK_5_SN(B,N,P) B[0]+=(((P%5)-2)<<N);B[1]+=((((P/5)%5)-2)<<N);B[2]+=((((P/25)%5)-2)<<N);
/*
#define FOR_ALL_ELT_IN_SQUARE_FAST(MD,Size,x0,y0,xc,yc,PLine,PCol) GET_LINE_POINTER_FAST(MD,PLine,x0,y0) FOR_ALL_ELT_IN_SQUARE_Y_FAST(Size,yc,PLine,PCol) FOR_ALL_ELT_IN_SQUARE_X_FAST(Size,xc)
#define NEXT_ELT_IN_SQUARE_FAST(MD,Size,PLine,PCol) NEXT_ELT_IN_SQUARE_X_FAST(MD,PCol) NEXT_ELT_IN_SQUARE_Y_FAST(MD,PLine)
#define ELT_PTR_FAST(MD,PCol) (PCol)
*/
{
	//int Len=0;
#ifdef MELINK_UNPACK_C
	int Count=0;
#else
	int Count=1;
#endif
	BYTE*MsgM=Msg->M;
	BYTE CodingRP5;
	//GET_LINE_POINTER_FAST(MD0,PLine,x0,y0);
	//FOR_ALL_ELT_IN_SQUARE_Y_FAST(Size,yc,PLine,PCol);
	//FOR_ALL_ELT_IN_SQUARE_X_FAST(Size,xc);
	FOR_ALL_ELT_IN_SQUARE_FAST(MD0,Size,Msg->PosX*Size,Msg->PosY*Size,xc,yc,PLine,PCol)
#ifdef MELINK_UNPACK_C
	if(Count==0)
	{
		CodingRP5=*MsgM++;
		Count=(CodingRP5>>7)+1;
		CodingRP5&=0x7F;
	}
#else
	
	
	
	__asm
	{
		//push eax
		//push ebx
		//push ecx
		//push edx
		
		dec Count;
		mov ebx,MsgM;
#if NPACK==0
		jnz noread0
#elif NPACK==1
		jnz noread1
#elif NPACK==2
		jnz noread2
#elif NPACK==3
		jnz noread3
#elif NPACK==4
		jnz noread4
#elif NPACK==5
		jnz noread5
#elif NPACK==6
		jnz noread6
#endif
		xor ah,ah
		mov al,[ebx]
		add eax,(128+256)
		inc ebx
		mov BYTE PTR Count,ah
		and al,127
		mov MsgM,ebx
		mov CodingRP5,al

#if NPACK==0
noread0:
#elif NPACK==1
noread1:
#elif NPACK==2
noread2:
#elif NPACK==3
noread3:
#elif NPACK==4
noread4:
#elif NPACK==5
noread5:
#elif NPACK==6
noread6:
#endif


		mov ebx,PCol
		mov al,CodingRP5;
		mov cl,5
		xor ah,ah
		div cl
		mov dl,ah
		xor ah,ah
		div cl
		sub dl,2
#if NPACK!=0
		shl dl,NPACK
#endif
		sub al,2
		add [ebx],dl
#if NPACK==0
jnc nor0
#elif NPACK==1
jnc nor1
#elif NPACK==2
jnc nor2
#elif NPACK==3
jnc nor3
#elif NPACK==4
jnc nor4
#elif NPACK==5
jnc nor5
#elif NPACK==6
jnc nor6
#endif
		neg dl
		sar dl,7
		mov [ebx],dl
#if NPACK==0
nor0:
#elif NPACK==1
nor1:
#elif NPACK==2
nor2:
#elif NPACK==3
nor3:
#elif NPACK==4
nor4:
#elif NPACK==5
nor5:
#elif NPACK==6
nor6:
#endif
		mov dl,ah
		inc ebx
		sub dl,2
#if NPACK!=0
		shl dl,NPACK
#endif
#if NPACK!=0
		shl al,NPACK
#endif
		//mov dh,al
		add [ebx],dl
#if NPACK==0
jnc nov0
#elif NPACK==1
jnc nov1
#elif NPACK==2
jnc nov2
#elif NPACK==3
jnc nov3
#elif NPACK==4
jnc nov4
#elif NPACK==5
jnc nov5
#elif NPACK==6
jnc nov6
#endif
		neg dl
		sar dl,7
		mov [ebx],dl
#if NPACK==0
nov0:
#elif NPACK==1
nov1:
#elif NPACK==2
nov2:
#elif NPACK==3
nov3:
#elif NPACK==4
nov4:
#elif NPACK==5
nov5:
#elif NPACK==6
nov6:
#endif
		inc ebx
		add [ebx],al
#if NPACK==0
jnc nob0
#elif NPACK==1
jnc nob1
#elif NPACK==2
jnc nob2
#elif NPACK==3
jnc nob3
#elif NPACK==4
jnc nob4
#elif NPACK==5
jnc nob5
#elif NPACK==6
jnc nob6
#endif
		neg al
		sar al,7
		mov [ebx],al
#if NPACK==0
nob0:
#elif NPACK==1
nob1:
#elif NPACK==2
nob2:
#elif NPACK==3
nob3:
#elif NPACK==4
nob4:
#elif NPACK==5
nob5:
#elif NPACK==6
nob6:
#endif
		//pop edx;
		//pop ecx;
		//pop ebx;
		//pop eax;
	}
#endif
	
#ifdef MELINK_UNPACK_C
	RGB_UNPACK_5_SN(ELT_PTR_FAST(MD0,PCol),NPACK,CodingRP5);
	Count--;
#endif
	NEXT_ELT_IN_SQUARE_FAST(MD0,Size,PLine,PCol);
	//NEXT_ELT_IN_SQUARE_X_FAST(MD,PCol);
	//NEXT_ELT_IN_SQUARE_Y_FAST(MD,PLine);
	return MsgM-Msg->M+sizeof(MELINK_MSG);
}
#endif

#undef NPACK

