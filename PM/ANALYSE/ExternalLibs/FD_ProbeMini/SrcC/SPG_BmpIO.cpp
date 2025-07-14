
#include "SPG_General.h"

#ifdef SPG_General_USEBMPIO

#include "SPG_Includes.h"

#include <memory.h>
#include <stdio.h>

BYTE* SPG_CONV BMP_ReadByte(char* FName,int& SizeX,int& SizeY)
{
	BYTE* Df=0;
	SizeX=SizeY=0;

	int SizeF;
	int From8;
	BYTE HEAD[54];
	
	FILE *F = fopen(FName,"rb");
	CHECKTWO(F==0,"B_Read: Impossible d'ouvrir le fichier",FName,return 0);
	
	fread(HEAD,54,1,F);
	
	SizeX = *(DWORD*)(HEAD+18);
	SizeY = *(DWORD*)(HEAD+22);

	/*
	*(LONG*)(HEAD+10)=54+((POCT==1)?1024:0);//+palette (1024);
	*(LONG*)(HEAD+14)=40;
	*(LONG*)(HEAD+18)=SizeX;
	*(LONG*)(HEAD+22)=SizeY;
	*(WORD*)(HEAD+26)=1;
	*(WORD*)(HEAD+28)=8*POCTF;
	*(LONG*)(HEAD+30)=0;
	*(LONG*)(HEAD+34)=SizeF*SizeY;
	*(LONG*)(HEAD+38)=0;
	*(LONG*)(HEAD+42)=0;
	*(LONG*)(HEAD+46)=((POCT==1)?256:0);//256;//si 256 coul
	*(LONG*)(HEAD+50)=((POCT==1)?256:0);//256;//si 256 coul
*/
	fseek(F,*(LONG*)(HEAD+10),SEEK_SET);

	if (*(WORD*)(HEAD+28)==8) 
		From8=1;
	else
		From8=0;

	if (From8==0) 
		SizeF = (SizeX*3+3)&0xfffffffc;
	else
		SizeF=SizeX;
	
	BYTE*UneLigne;
	UneLigne=(BYTE*)SPG_MemAlloc(SizeF,"BMP_ReadByte");
	SPG_CatMemName(UneLigne,FName);
	/*
	CHECK(G_InitEcran(E,G_ALLOC_MEMOIRE,
		0,0,POCT,SizeX,SizeY,
		0,0,0)==0,"SG_LoadTex: Creation ecran echouee",fclose(F);return 0);
	*/

	Df=SPG_TypeAlloc(SizeX*SizeY,BYTE,"2DByteProfilFromBMP");

	int y;
#ifdef P_ManagerRevBMP
	for(y=SizeY-1; y>=0; y--)
#else
	for(y=0; y<SizeY; y++)
#endif
	{
		fread(UneLigne,SizeF,1,F);
		//MT+=E.Pitch;
		if (From8)
		{
		int x;
		for(x=0; x<SizeX; x++)
		{
			Df[x+SizeX*y]=UneLigne[x];
		}
		}
		else
		{
		int x;
		for(x=0; x<SizeX; x++)
		{
			Df[x+SizeX*y]=UneLigne[3*x];//+(float)UneLigne[3*x+1]+(float)UneLigne[3*x+2])/3.0;
		}
		}
			/*
			G_DrawPixel(E,x,y,
			G_MakeCompatibleFrom24(E,*(DWORD*)(UneLigne+3*x)));
			*/
	}

	SPG_MemFree(UneLigne);
	
	fclose(F);
	
	return Df;
}

BYTE* SPG_CONV BMP_Read24(char* FName,int& Pitch,int& SizeX,int& SizeY)
{
	Pitch=SizeX=SizeY=0;

	BYTE HEAD[54];
	
	FILE *F = fopen(FName,"rb");
	CHECKTWO(F==0,"B_Read: Impossible d'ouvrir le fichier",FName,return 0);
	
	fread(HEAD,54,1,F);
	
	SizeX = *(DWORD*)(HEAD+18);
	SizeY = *(DWORD*)(HEAD+22);
	fseek(F,*(LONG*)(HEAD+10),SEEK_SET);

	CHECK((*(WORD*)(HEAD+28)!=24),"Format BMP RGB nécessaire",fclose(F);return 0);

	Pitch = (SizeX*3+3)&0xfffffffc;
	
	BYTE* Df=SPG_TypeAlloc(Pitch*SizeY,BYTE,"2DByteProfilFromBMP");

	int y;
#ifdef P_ManagerRevBMP
	for(y=SizeY-1; y>=0; y--)
#else
	for(y=0; y<SizeY; y++)
#endif
	{
		fread(Df+y*Pitch,Pitch,1,F);
	}
	fclose(F);
	
	return Df;
}

float* SPG_CONV BMP_ReadFloat(char* FName,int& SizeX,int& SizeY)
{
	float* Df=0;
	SizeX=SizeY=0;

	int SizeF;
	int From8;
	BYTE HEAD[54];
	
	FILE *F = fopen(FName,"rb");
	CHECKTWO(F==0,"B_Read: Impossible d'ouvrir le fichier",FName,return 0);
	
	fread(HEAD,54,1,F);
	
	SizeX = *(DWORD*)(HEAD+18);
	SizeY = *(DWORD*)(HEAD+22);

	/*
	*(LONG*)(HEAD+10)=54+((POCT==1)?1024:0);//+palette (1024);
	*(LONG*)(HEAD+14)=40;
	*(LONG*)(HEAD+18)=SizeX;
	*(LONG*)(HEAD+22)=SizeY;
	*(WORD*)(HEAD+26)=1;
	*(WORD*)(HEAD+28)=8*POCTF;
	*(LONG*)(HEAD+30)=0;
	*(LONG*)(HEAD+34)=SizeF*SizeY;
	*(LONG*)(HEAD+38)=0;
	*(LONG*)(HEAD+42)=0;
	*(LONG*)(HEAD+46)=((POCT==1)?256:0);//256;//si 256 coul
	*(LONG*)(HEAD+50)=((POCT==1)?256:0);//256;//si 256 coul
*/
	fseek(F,*(LONG*)(HEAD+10),SEEK_SET);

	if (*(WORD*)(HEAD+28)==8) 
		From8=1;
	else
		From8=0;

	if (From8==0) 
		SizeF = (SizeX*3+3)&0xfffffffc;
	else
		SizeF = (SizeX+3)&0xfffffffc;
		//SizeF=SizeX;
	
	BYTE*UneLigne;
	UneLigne=(BYTE*)SPG_MemAlloc(SizeF,"Ligne de bitmap");
	/*
	CHECK(G_InitEcran(E,G_ALLOC_MEMOIRE,
		0,0,POCT,SizeX,SizeY,
		0,0,0)==0,"SG_LoadTex: Creation ecran echouee",fclose(F);return 0);
	*/

	Df=(float*)SPG_MemAlloc(SizeX*SizeY*sizeof(float),"FIO_BMP");
	SPG_CatMemName(Df,SPG_NameOnly(FName));

	int y;
#ifdef P_ManagerRevBMP
	for(y=SizeY-1; y>=0; y--)
#else
	for(y=0; y<SizeY; y++)
#endif
	{
		fread(UneLigne,SizeF,1,F);
		//MT+=E.Pitch;
		if (From8)
		{
		int x;
		for(x=0; x<SizeX; x++)
		{
			Df[x+SizeX*y]=UneLigne[x];
		}
		}
		else
		{
		int x;
		for(x=0; x<SizeX; x++)
		{
			Df[x+SizeX*y]=((float)UneLigne[3*x]+(float)UneLigne[3*x+1]+(float)UneLigne[3*x+2])/3.0f;
		}
		}
			/*
			G_DrawPixel(E,x,y,
			G_MakeCompatibleFrom24(E,*(DWORD*)(UneLigne+3*x)));
			*/
	}

	SPG_MemFree(UneLigne);
	
	fclose(F);

	return Df;
}


int SPG_CONV BMP_WriteFloat(float* Df, int SizeX, int SizeY, float VMin, float VMax, char* FName)
{
	CHECK(Df==0,"BMP_Write: Profil vide",return 0);
	CHECK(VMin==VMax,"BMP_Write: Echelle nulle",return 0);
	CHECK(SizeX<=0,"BMP_Write: Taille X invalide",return 0);
	CHECK(SizeY<=0,"BMP_Write: Taille Y invalide",return 0);

	//int POCTF,SizeF;
	BYTE*ColorT;
	BYTE*MFF;

	BYTE HEAD[54];
/*
	if((POCTF=POCT)==4) 
		POCTF=3;
*/
	int POCT=1;
	int POCTF=1;

	int SizeF=(SizeX*POCTF+3)&0xfffffffc;
	//HEAD=SPG_MemAlloc(54,"Header Bitmap");
	
	memset(HEAD,0,54);
	HEAD[0]=0x42;
	HEAD[1]=0x4D;
	*(LONG*)(HEAD+2)=SizeF*SizeY+54+((POCT==1)?1024:0);//+Palette (1024);
	HEAD[6]=0;
	HEAD[7]=0;
	HEAD[8]=0;
	HEAD[9]=0;
	*(LONG*)(HEAD+10)=54+((POCT==1)?1024:0);//+palette (1024);
	*(LONG*)(HEAD+14)=40;
	*(LONG*)(HEAD+18)=SizeX;
	*(LONG*)(HEAD+22)=SizeY;
	*(WORD*)(HEAD+26)=1;
	*(WORD*)(HEAD+28)=(WORD)(8*POCTF);
	*(LONG*)(HEAD+30)=0;
	*(LONG*)(HEAD+34)=SizeF*SizeY;
	*(LONG*)(HEAD+38)=0;
	*(LONG*)(HEAD+42)=0;
	*(LONG*)(HEAD+46)=((POCT==1)?256:0);//256;//si 256 coul
	*(LONG*)(HEAD+50)=((POCT==1)?256:0);//256;//si 256 coul
	
	FILE*F=fopen(FName,"wb");
	CHECKTWO(F==0,"BMP_WriteFloat: Impossible d'ouvrir le fichier",FName,return 0);
	
	fwrite(HEAD,54,1,F);
	
	//SPG_MemFree(HEAD);
	
	if(POCT==1)
	{
		ColorT=(BYTE*)SPG_MemAlloc(1024,"ColorTable Bitmap");
		LONG *CT;
		CT=(LONG *)ColorT;
		int i;
		for(i=0;i<=0x00ffffff;i+=0x00010101)
		{
			*CT++=i;
		}
		fwrite(ColorT,1024,1,F);
		SPG_MemFree(ColorT);
	}
	
	MFF=(BYTE*)SPG_MemAlloc(SizeF,"Ligne Bitmap");
	int y;
	float * MT;
	BYTE * MFT;
#ifdef P_ManagerRevBMP
	for(y=SizeY-1; y>=0; y--)
#else
	for(y=0; y<SizeY; y++)
#endif
	{
		MT=Df+y*SizeX;
		MFT=MFF;
		int x;
		
		for(x=0;x<SizeX;x++)
		{
			*MFT=(BYTE)V_Sature(256.0*((*MT)-VMin)/(VMax-VMin),0,255);
			/*
			*(MFT+1)=*(MT+1);
			*(MFT+2)=*(MT+2);
			*/
			MFT+=POCTF;		
			MT++;
		}

		//MT+=Pitch-SizeX*POCT;
		
		fwrite(MFF,SizeF,1,F);
		
	}

	SPG_MemFree(MFF);
	fclose(F);
	return -1;
}

int SPG_CONV BMP_WriteByte(BYTE*Df,int SizeX,int SizeY,char*FName)
{
	CHECK(Df==0,"BMP_WriteByte: Profil vide",return 0);
	CHECK(SizeX<=0,"BMP_WriteByte: Taille X invalide",return 0);
	CHECK(SizeY<=0,"BMP_WriteByte: Taille Y invalide",return 0);

	//int POCTF,SizeF;
	BYTE*ColorT;
	BYTE*MFF;

	BYTE HEAD[54];
/*
	if((POCTF=POCT)==4) 
		POCTF=3;
*/
	int POCT=1;
	int POCTF=1;

	int SizeF=(SizeX*POCTF+3)&0xfffffffc;
	//HEAD=SPG_MemAlloc(54,"Header Bitmap");
	
	memset(HEAD,0,54);//inutile
	HEAD[0]=0x42;
	HEAD[1]=0x4D;
	*(LONG*)(HEAD+2)=SizeF*SizeY+54+((POCT==1)?1024:0);//+Palette (1024);
	HEAD[6]=0;
	HEAD[7]=0;
	HEAD[8]=0;
	HEAD[9]=0;
	*(LONG*)(HEAD+10)=54+((POCT==1)?1024:0);//+palette (1024);
	*(LONG*)(HEAD+14)=40;
	*(LONG*)(HEAD+18)=SizeX;
	*(LONG*)(HEAD+22)=SizeY;
	*(WORD*)(HEAD+26)=1;
	*(WORD*)(HEAD+28)=(WORD)(8*POCTF);
	*(LONG*)(HEAD+30)=0;
	*(LONG*)(HEAD+34)=SizeF*SizeY;
	*(LONG*)(HEAD+38)=0;
	*(LONG*)(HEAD+42)=0;
	*(LONG*)(HEAD+46)=((POCT==1)?256:0);//256;//si 256 coul
	*(LONG*)(HEAD+50)=((POCT==1)?256:0);//256;//si 256 coul
	
	FILE*F=fopen(FName,"wb");
	CHECKTWO(F==0,"BMP_WriteByte: Impossible d'ouvrir le fichier",FName,return 0);
	
	fwrite(HEAD,54,1,F);
	
	//SPG_MemFree(HEAD);
	
	if(POCT==1)
	{
		ColorT=(BYTE*)SPG_MemAlloc(1024,"ColorTable Bitmap");
		LONG *CT;
		CT=(LONG *)ColorT;
		int i;
		for(i=0;i<=0x00ffffff;i+=0x00010101)
		{
			*CT++=i;
		}
		fwrite(ColorT,1024,1,F);
		SPG_MemFree(ColorT);
	}
	
	MFF=(BYTE*)SPG_MemAlloc(SizeF,"Ligne Bitmap");
	int y;
	BYTE * MT;
	BYTE * MFT;
#ifdef P_ManagerRevBMP
	for(y=SizeY-1; y>=0; y--)
#else
	for(y=0; y<SizeY; y++)
#endif
	{
		MT=Df+y*SizeX;
		MFT=MFF;
		int x;
		
		for(x=0;x<SizeX;x++)
		{
			*MFT=*MT;
			/*
			*(MFT+1)=*(MT+1);
			*(MFT+2)=*(MT+2);
			*/
			MFT+=POCTF;		
			MT++;
		}

		//MT+=Pitch-SizeX*POCT;
		
		fwrite(MFF,SizeF,1,F);
		
	}

	SPG_MemFree(MFF);
	fclose(F);
	return -1;
}

//BMP_Write24 : voir SPG_BmpIO.agh
//define BMP_Write24(Name,M,POCT,Pitch,SizeX,SizeY) G_SaveToBMP(Name,M,POCT,Pitch,SizeX,SizeY)

#endif

