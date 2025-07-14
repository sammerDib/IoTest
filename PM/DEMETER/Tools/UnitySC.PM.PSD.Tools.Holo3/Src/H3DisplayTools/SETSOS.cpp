#define SET(Src,SrcType,SrcBaseType,Low,High)									\
{																				\
	long nSizeY=Src.GetLi();													\
	long nSizeX=Src.GetCo();													\
																				\
	if (ReAlloc(nSizeX,nSizeY))													\
	{																			\
  		float d=(float)fabs(High-Low);											\
		float Scale=d/255.0F;													\
		float Offset=(float)Low;												\
		SrcBaseType * pS=(SrcBaseType *)Src.GetData();							\
		if (Scale!=0)															\
		{																		\
			for (long y=0;y<nSizeY;y++)											\
			{																	\
				long Off=(nSizeY-1-y)*nSizeX;									\
				LPBYTE pDest=m_pBits+Off;										\
				for (long x=0;x<nSizeX;x++)										\
				{																\
					if (*pS>=Low && *pS<=High)									\
					{															\
						float val = (*pS - Offset)/Scale;						\
						*pDest=(BYTE)(val);										\
					}															\
					else														\
					{															\
						*pDest=0;												\
					}															\
					pDest++;pS++;												\
				}																\
			}																	\
		}																		\
																				\
		if (!(m_pPalette))														\
			CreatePalette();													\
	}																			\
}						

#define SET(Src,SrcType,SrcBaseType,Low,High)									\
{																				\
	long nSizeY=Src.GetLi();													\
	long nSizeX=Src.GetCo();													\
																				\
	if (ReAlloc(nSizeX,nSizeY))													\
	{																			\
  		float d=(float)fabs(High-Low);											\
		float Scale=d/255.0F;													\
		float Offset=(float)Low;												\
		SrcBaseType * pS=(SrcBaseType *)Src.GetData();							\
		FILE *Stream=fopen("t.txt","w+t");\
		if (Scale!=0)															\
		{																		\
			for (long y=0;y<nSizeY;y++)											\
			{																	\
				long Off=(nSizeY-1-y)*nSizeX;									\
				LPBYTE pDest=m_pBits+Off;										\
				for (long x=0;x<nSizeX;x++)										\
				{																\
					if (*pS>=Low && *pS<=High)									\
					{															\
						float val = (*pS - Offset)/Scale;						\
						*pDest=(BYTE)(val);										\
				fprintf(Stream,"%d %d %f\n",x,y,val);\
					}															\
					else														\
					{															\
						*pDest=0;												\
				fprintf(Stream,"%d %d %f\n",x,y,0.0F);\
					}															\
					pDest++;pS++;												\
				}																\
			}																	\
		}																		\
		fclose(Stream);
																				\
		if (!(m_pPalette))														\
			CreatePalette();													\
	}																			\
}																				