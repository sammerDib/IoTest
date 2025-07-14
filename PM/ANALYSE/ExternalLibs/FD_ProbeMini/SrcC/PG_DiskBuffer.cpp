
#include "SPG_General.h"

#ifdef SPG_General_USEDiskBuffer

#include "SPG_Includes.h"

#include <stdio.h>
#include <string.h>

//cree un acces fichier bufferise (ecriture sequentielle), Mode=PG_DB_READ, PG_DB_WRITE
int SPG_CONV PG_CreateDiskBuffer(PG_DISKBUFFER& B, int BufferLen, int Mode, char* FileName)
{
	CHECK(V_IsBound(BufferLen,16,(32768*1024))==0,"PG_CreateDiskBuffer: Taille invalide",return 0);
	CHECK((FileName==0)||((*FileName)==0),"PG_CreateDiskBuffer: Nom invalide",return 0);

	memset(&B,0,sizeof(PG_DISKBUFFER));

	if(Mode&PG_DB_READ)
	{

	CHECK((B.F=fopen(FileName,"rb"))==0,"PG_CreateDiskBuffer: Ouverture du fichier impossible",return 0);

	fseek((FILE*)B.F,0,SEEK_END);

	CHECK((B.FileLen=ftell((FILE*)B.F))==0,"PG_CreateDiskBuffer: Ouverture du fichier en lecture impossible",fclose((FILE*)B.F);return 0);

	B.Etat=PG_DB_READ;

	fseek((FILE*)B.F,0,SEEK_SET);

	if(B.FileLen<(BufferLen+(BufferLen>>1))) 
	{
		B.BufferLen=B.FileLen;
	}
	else
	{
		B.BufferLen=BufferLen;
	}

	}
	else if(Mode&PG_DB_WRITE)
	{
	CHECK((B.F=fopen(FileName,"wb"))==0,"PG_CreateDiskBuffer: Ouverture du fichier en ecriture impossible",return 0);
	B.Etat=PG_DB_WRITE;
	B.BufferLen=BufferLen;
	}
	else
		return 0;

	//+4 pour la lecture de bits au niveau de la fin du buffer
	//oct oct oct END END END ...
	//on lit
	//        oct oct oct
	//on masque
	//        msk 0   0

	CHECK((B.M=SPG_TypeAlloc(B.BufferLen+4,BYTE,"PG_DiskBuffer"))==0,"PG_CreateDiskBuffer: Allocation memoire echouee",fclose((FILE*)B.F);return 0);

	if(B.Etat&PG_DB_READ)
	{
	B.BytePos=B.BufferLen;//comme si on venait de lire le dernier octet
	PG_FillDiskBuffer(B);
	}

	return -1;
}

//ferme le buffer, ecrit les donnees qui doivent l'etre
void SPG_CONV PG_CloseDiskBuffer(PG_DISKBUFFER& B)
{
	if (B.Etat&PG_DB_WRITE) PG_FlushDiskBuffer(B);

	if (B.F) fclose((FILE*)B.F);
	if (B.M) SPG_MemFree(B.M);
	memset(&B,0,sizeof(PG_DISKBUFFER));
	return;
}

//fixe la position de lecture, provoque un rechargement disque
void SPG_CONV PG_SetPosition(PG_DISKBUFFER& B, int Position)
{
	CHECK((B.Etat&PG_DB_READ)==0,"PG_SetPosition: Ce buffer n'est pas en lecture",return);
	B.BytePos=B.BufferLen;//comme si on venait de lire/ecrire le dernier octet
	B.BufferPosInFile=Position;
	//if (B.Etat&PD_DB_READ)
		PG_FillDiskBuffer(B);
		/*
	else if (B.Etat&PD_DB_WRITE)
		PG_FlushDiskBuffer(B,0);
		*/
	//B.Etat&=~PG_DB_FULLFILE;
	return;
}

//usage interne: 
void SPG_CONV PG_FillDiskBuffer(PG_DISKBUFFER& B)
{
	//CHECK(ByteRecouvr>B.BufferLen,"PG_FillBuffer: Ne peut assurer un tel retour en arriere",return);
	//CHECK(B.Etat&PG_DB_FULLFILE,"PG_FillBuffer: Tout le fichier est deja charge",return);
	CHECK((B.Etat&PG_DB_READ)==0,"PG_FillDiskBuffer: Ce buffer n'est pas en lecture",return);
//	if (B.Etat&PG_DB_FULLFILE) return;

	int Raccord;
	if (B.BytePos<B.BufferLen)
	{
		Raccord=B.BufferLen-B.BytePos;
		SPG_Memmove(B.M,B.M+B.BytePos,Raccord);
	}
	else
		Raccord=0;

	B.BytePos=0;

	B.BufferPosInFile+=B.BufferLen-Raccord;


	int LenToReadInBuffer=B.BufferLen-Raccord;
	int LenToReadInFile=B.FileLen-B.BufferPosInFile;

	int LenToRead=V_Min(LenToReadInBuffer,LenToReadInFile);

	if (LenToRead) fread(B.M+Raccord,LenToRead,1,(FILE*)B.F);

	/*
	if(B.PosInFile>=B.FileLen)
	{
		fclose(B.F);
		B.F=0;
		B.Etat|=PG_DB_FULLFILE;
	}
	*/

	return;
}

//usage interne
void SPG_CONV PG_FlushDiskBuffer(PG_DISKBUFFER& B)
{
	//CHECK(ByteRecouvr>B.BufferLen,"PG_FillBuffer: Ne peut assurer un tel retour en arriere",return);
	//CHECK(B.Etat&PG_DB_FULLFILE,"PG_FillBuffer: Tout le fichier est deja charge",return);
	CHECK((B.Etat&PG_DB_WRITE)==0,"PG_FlushDiskBuffer: Ce buffer n'est pas en ecriture",return);
	if(B.BytePos)
	{
	fwrite(B.M,B.BytePos,1,(FILE*)B.F);
	B.BufferPosInFile+=B.BytePos;
	B.BytePos=0;
	}
	return;
}

//retourne le pointeur sur les donnees du buffer, avec la garantie de droit d'acces à LenToRead octets au moins
BYTE* SPG_CONV PG_ReadBytes(PG_DISKBUFFER& B, int LenToRead)
{
	CHECK((B.Etat&PG_DB_READ)==0,"PG_ReadBytes: Ce buffer n'est pas en lecture",return 0);
	CHECK(B.Etat==0,"PG_ReadBytes: Buffer invalide",return 0);
	B.BytePos+=((B.BitPos+7)>>3);
	B.BitPos=0;
	if(B.BytePos+LenToRead>B.BufferLen) PG_FillDiskBuffer(B);
	CHECK(B.BytePos+LenToRead>=B.BufferLen,"PG_ReadBytes: Lecture au dela du buffer",return 0);
	BYTE* ReadAtPos=B.M+B.BytePos;
	B.BytePos+=LenToRead;
	return ReadAtPos;
}

//ecris des donnees dans le buffer
void SPG_CONV PG_WriteBytes(PG_DISKBUFFER& B, BYTE* M, int LenToWrite)
{
	CHECK((B.Etat&PG_DB_WRITE)==0,"PG_WriteBytes: Ce buffer n'est pas en ecriture",return);
	CHECK(B.Etat==0,"PG_WriteBytes: Buffer invalide",return);
	B.BytePos+=((B.BitPos+7)>>3);
	B.BitPos=0;
	if(B.BytePos+LenToWrite>B.BufferLen) PG_FlushDiskBuffer(B);
	CHECK(B.BytePos+LenToWrite>B.BufferLen,"PG_WriteBytes: Ecriture au dela du buffer",return);
	SPG_Memcpy(B.M+B.BytePos,M,LenToWrite);
	B.BytePos+=LenToWrite;
	return;
}

//25 bits valides au minimum, ne fais pas avancer le pointeur
DWORD SPG_CONV PG_ConsulteBits(PG_DISKBUFFER& B)
{
	CHECK((B.Etat&PG_DB_READ)==0,"PG_FillBuffer: Ce buffer n'est pas en lecture",return 0);
	CHECK(B.Etat==0,"PG_ReadBytes: Buffer invalide",return 0);
	if(B.BytePos+4>B.BufferLen) PG_FillDiskBuffer(B);
	DWORD BitsArray=(*((DWORD*)(B.M+B.BytePos)))>>B.BitPos;
	return BitsArray;
}

DWORD SPG_CONV PG_ReadBits(PG_DISKBUFFER& B, int NombreDeBits)
{
	CHECK((B.Etat&PG_DB_READ)==0,"PG_FillBuffer: Ce buffer n'est pas en lecture",return 0);
	CHECK(B.Etat==0,"PG_ReadBytes: Buffer invalide",return 0);
	if(B.BytePos+4>B.BufferLen) PG_FillDiskBuffer(B);
	DWORD BitsArray=(*((DWORD*)(B.M+B.BytePos)))>>B.BitPos;
	B.BitPos=B.BitPos+NombreDeBits;
	B.BytePos+=(B.BitPos>>3);
	B.BitPos&=7;
	return (BitsArray&((((DWORD)1)<<NombreDeBits)-(DWORD)1));
}

void SPG_CONV PG_WriteBits(PG_DISKBUFFER& B, DWORD M, int NombreDeBits)
{
	CHECK((B.Etat&PG_DB_WRITE)==0,"PG_WriteBits: Ce buffer n'est pas en ecriture",return);
	CHECK(B.Etat==0,"PG_WriteBytes: Buffer invalide",return);
	if(B.BytePos+4>B.BufferLen) PG_FlushDiskBuffer(B);
	*(DWORD*)(B.M+B.BytePos)&=(1<<B.BitPos)-1;//masque ce qui n'est pas encore ecrit
	*(DWORD*)(B.M+B.BytePos)|=(M<<B.BitPos);//ecrit plus qu'il n'en faut (normal)
	B.BitPos+=NombreDeBits;
	B.BytePos+=(B.BitPos>>3);
	B.BitPos&=7;
	return;
}

#endif

