
#ifdef SPG_General_USEBRES

typedef struct
{
	union
	{
		struct
		{
		WORD R;
		SHORT Y;
		};
		LONG T;
	};
	LONG AddTo;
} BRES_Y;

typedef struct
{
	BRES_Y x;
	BRES_Y y;
} BRES_XY;

//#define BRES_START(BY,X0,Y0,DX,DY) BRES_Y BY; BY.R=0x8000; BY.Y=(Y0); BY.AddTo=((DY)<<16)/(DX); if (X0<0) BY.Y-=((X0)*(DY))/(DX);
#define BRES_START_CLIP(BY,X0,Y0,DX,DY) BRES_Y BY; BY.R=(WORD)0x8000; BY.Y=(SHORT)(Y0); BY.AddTo=(LONG)(((DY)<<16)/(DX)); if (X0<0) BY.Y-=(SHORT)(((X0)*(DY))/(DX));
#define BRES_CLIP(BY,Y0) if ((Y0)<0) BY.T-=BY.AddTo*Y0;
#define BRES_NEWADDTO(BY,DX,DY) BY.AddTo=(LONG)(((DY)<<16)/(DX));
#define BRES_SAFENEWADDTO(BY,DX,DY) if (DX) BY.AddTo=(LONG)(((DY)<<16)/(DX)); else BY.AddTo=0;
#define BRES_SET(BY,Y0) BY.R=(WORD)0x8000; BY.Y=(SHORT)(Y0);
#define BRES_GET(BY) BY.Y
#define BRES_NEXT(BY) BY.T+=BY.AddTo

#define BRESXY_NEWADDTO(BY,DX,DY,N) BY.x.AddTo=(LONG)((DX)<<16)/(N);BY.y.AddTo=(LONG)((DY)<<16)/(N)
#define BRESXY_NEWADDTOHiRES(BY,DX,DY,N) BY.x.AddTo=(DX)/(N);BY.y.AddTo=(DY)/(N)
#define BRESXY_SET(BY,TEXCOORD) BRES_SET(BY.x,TEXCOORD.x);BRES_SET(BY.y,TEXCOORD.y)
#define BRESXY_START(BY,BG,BD,N) BRES_XY BY;BY=BG; BRESXY_NEWADDTOHiRES(BY,BD.x.T-BG.x.T,BD.y.T-BG.y.T,N)
#define BRESXY_CLIP(BY,X0) if ((X0)<0) {BY.x.T-=BY.x.AddTo*X0;BY.y.T-=BY.y.AddTo*X0;}
//BRESXY_NEWADDTO(BY,BD.x.Y-BG.x.Y,BD.y.Y-BG.y.Y,N)
//BRESXY_NEWADDTOHiRES(BY,BD.x.T-BG.x.T,BD.y.T-BG.y.T,N)
#define BRESXY_GETX(BY) BRES_GET(BY.x)
#define BRESXY_GETY(BY) BRES_GET(BY.y)
#define BRESXY_NEXT(BY) BRES_NEXT(BY.x);BRES_NEXT(BY.y)


#endif

