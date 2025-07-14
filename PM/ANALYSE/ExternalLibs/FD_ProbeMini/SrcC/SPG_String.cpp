

#include "SPG_General.h"
#include "SPG_Includes.h"

#include <memory.h>
#include <string.h>
#include <stdio.h>
//#include <stdlib.h>


void SPG_CONV StrInit(SPGSTR& S, int MaxLen)
{
	SPG_ZeroStruct(S);
	S.State=SPGSTR_OK|SPGSTR_ALLOC;
	S.MaxLen=MaxLen;
	S.Len=0;
	S.D=(BYTE*)SPG_MemAlloc(V_Max(S.MaxLen,1),"StrInit");
	return;
}

void SPG_CONV StrAliasInit(SPGSTR& SubString, BYTE* D, int Len, int MaxLen)
{
	SPG_ZeroStruct(SubString);
	CHECK(D==0,"StrAliasInit",return);
	SubString.State=SPGSTR_OK|SPGSTR_ALIAS;
	SubString.MaxLen=MaxLen;
	SubString.Len=V_Min(Len,MaxLen);
	SubString.D=D;
	return;
}

void SPG_CONV StrAliasInit(SPGSTR& SubString, BYTE* D, int Len)
{
	StrAliasInit(SubString,D,Len,Len);
	return;
}

void SPG_CONV StrSubInit(SPGSTR& SubString, SPGSTR& S, int Start, int Len)
{
	SPG_ZeroStruct(SubString);
	CHECK(S.State==0,"StrSubInit",return);
	CHECK(S.D==0,"StrSubInit",return);
	SubString.State=SPGSTR_OK|SPGSTR_ALIAS;
	SubString.MaxLen=S.MaxLen-Start;
	SubString.Len=Len;
	SubString.D=S.D+Start;
	return;
}

void SPG_CONV StrClose(SPGSTR& S)
{
	DbgCHECK(S.State==0,"StrClose");
	if(S.State&SPGSTR_ALLOC) SPG_MemFree(S.D);
	SPG_ZeroStruct(S);
	return;
}

char* SPG_CONV StrZGet(SPGSTR& S)//internal use - ajoute le zero terminal ou raccourcit la chaine et indique si la chaine est valide - alternative à StrCpy
{
	CHECK(S.State==0,"StrZGet",return 0);
	S.D[V_Min(S.Len,(S.MaxLen-1))]=0;
	return (char*)S.D;
}

int SPG_CONV StrZLen(char* StrZ)
{
	CHECK(StrZ==0,"StrZLen",return 0);
	return strlen(StrZ);
}

void SPG_CONV StrZtoStr(SPGSTR& S)//internal use
{
	S.Len=StrZLen((char*)S.D);
	CHECK(!V_IsBound(S.Len,0,S.MaxLen),"StrZtoStr",S.Len=S.MaxLen);
	return;
}

//string vers string

void SPG_CONV StrCpy(SPGSTR& D, SPGSTR& S)
{
	CHECK((D.State==0)||(D.D==0)||(S.State==0)||(S.D==0),"StrCpy",D.Len=0;return);
	D.Len=V_Min(D.MaxLen,S.Len);
	memcpy(D.D,S.D,D.Len);
	return;
}

void SPG_CONV StrCpy(SPGSTR& D, SPGSTR& S, int SrcStart, int Len)
{
	CHECK((D.State==0)||(D.D==0)||(S.State==0)||(S.D==0),"StrCpy",D.Len=0;return);
	D.Len=V_Min( V_Min(D.MaxLen,(S.Len-SrcStart)), Len);
	memcpy(D.D,S.D+SrcStart,D.Len);
	return;
}

void SPG_CONV StrCpy(SPGSTR& D, int DestStart, SPGSTR& S, int SrcStart, int Len)
{
	CHECK((D.State==0)||(D.D==0)||(S.State==0)||(S.D==0),"StrCpy",return);
	DbgCHECK(DestStart>D.Len,"StrCpy - non continuous string");
	D.Len=V_Min( V_Min(D.MaxLen-DestStart,(S.Len-SrcStart)), Len);
	memcpy(D.D+DestStart,S.D+SrcStart,D.Len);
	return;
}

void SPG_CONV StrCat(SPGSTR& D, SPGSTR& S)
{
	StrCpy(D,D.Len,S,0,S.Len);
	return;
}

void SPG_CONV StrCat(SPGSTR& D, SPGSTR& S, int SrcStart, int Len)
{
	StrCpy(D,D.Len,S,SrcStart,Len);
	return;
}

//C types vers string

void SPG_CONV StrCpy(SPGSTR& D, void* Data, int Len)
{
	CHECK((D.State==0)||(D.D==0)||(Data==0),"StrCpy",D.Len=0;return);
	D.Len=V_Min(D.MaxLen,Len);
	memcpy(D.D,Data,D.Len);
	return;
}

void SPG_CONV StrCpy(SPGSTR& D, char* MsgS)
{
	CHECK((D.State==0)||(D.D==0)||(MsgS==0),"StrCpy",D.Len=0;return);
	int L=strlen(MsgS);
	D.Len=V_Min(L,D.MaxLen);
	memcpy(D.D,MsgS,D.Len);
	return;
}

void SPG_CONV StrCpy(SPGSTR& D, BYTE b)
{
	CHECK((D.State==0)||(D.D==0),"StrCpy",D.Len=0;return);
	D.D[0]=b; D.Len=1;
	return;
}

void SPG_CONV StrCpy(SPGSTR& D, WORD b)
{
	CHECK((D.State==0)||(D.D==0),"StrCpy",D.Len=0;return);
	*(WORD*)D.D=b; D.Len=2;
	return;
}

void SPG_CONV StrCpy(SPGSTR& D, DWORD b)
{
	CHECK((D.State==0)||(D.D==0),"StrCpy",D.Len=0;return);
	*(DWORD*)D.D=b; D.Len=4;
	return;
}

void SPG_CONV StrCpy(SPGSTR& S, int v, int MinDigit)
{
	sprintf((char*)S.D,"%d",v);
	S.Len=strlen((char*)S.D);
	DbgCHECK(S.Len>S.MaxLen,"StrCat");
	return;
}

void SPG_CONV StrCpy(SPGSTR& S, float v, int NumDigit, char Separateur)
{
	CF_GetStringZ((char*)S.D,v,V_Min(NumDigit,(S.MaxLen-2)));
	S.Len=strlen((char*)S.D);
	DbgCHECK(S.Len>S.MaxLen,"StrCat");
	return;
}

//C types vers string

void SPG_CONV StrCat(SPGSTR& D, void* Data, int Len)
{
	CHECK((D.State==0)||(D.D==0)||(Data==0),"StrCat",D.Len=0;return);
	Len=V_Min(D.MaxLen-D.Len,Len);
	memcpy(D.D+D.Len,Data,Len);
	D.Len+=Len;
	DbgCHECK(D.Len>D.MaxLen,"StrCat");
	return;
}

void SPG_CONV StrCat(SPGSTR& D, char* MsgS)
{
	CHECK((D.State==0)||(D.D==0)||(MsgS==0),"StrCat",D.Len=0;return);
	int L=strlen(MsgS);
	L=V_Min(L,(D.MaxLen-D.Len));
	memcpy(D.D+D.Len,MsgS,L);
	D.Len+=L;
	DbgCHECK(D.Len>D.MaxLen,"StrCat");
	return;
}

void SPG_CONV StrCat(SPGSTR& D, BYTE b)
{
	CHECK((D.State==0)||(D.D==0),"StrCat",D.Len=0;return);
	D.D[D.Len]=b; D.Len++;
	DbgCHECK(D.Len>D.MaxLen,"StrCat");
	return;
}

void SPG_CONV StrCat(SPGSTR& D, WORD b)
{
	CHECK((D.State==0)||(D.D==0),"StrCat",D.Len=0;return);
	*(WORD*)(D.D+D.Len)=b; D.Len+=2;
	DbgCHECK(D.Len>D.MaxLen,"StrCat");
	return;
}

void SPG_CONV StrCat(SPGSTR& D, DWORD b)
{
	CHECK((D.State==0)||(D.D==0),"StrCat",D.Len=0;return);
	*(DWORD*)(D.D+D.Len)=b; D.Len+=4;
	DbgCHECK(D.Len>D.MaxLen,"StrCat");
	return;
}

void SPG_CONV StrCat(SPGSTR& S, int v, int MinDigit)
{
	sprintf((char*)S.D+S.Len,"%d",v);
	S.Len=strlen((char*)S.D);
	DbgCHECK(S.Len>S.MaxLen,"StrCat");
	return;
}

void SPG_CONV StrCat(SPGSTR& S, float v, int NumDigit, char Separateur)
{
	CF_GetStringZ((char*)S.D,v,V_Min(NumDigit,(S.MaxLen-2)));
	S.Len=strlen((char*)S.D);
	DbgCHECK(S.Len>S.MaxLen,"StrCat");
	return;
}

//string vers C Type

void SPG_CONV StrToByte(void* Data, int MaxLen, SPGSTR& S)
{
	if(MaxLen==0) return;
	int L=V_Min(MaxLen,S.Len);
	memcpy(Data,S.D,L);
	return;
}

int SPG_CONV StrToInt(int& v, SPGSTR& S, int Pos)
{
	v=0;
	bool Negative=false;
	CHECK(!V_IsBound(Pos,0,S.Len),"StrToInt",return Pos);
	if(S.D[Pos]==' ') Pos++;//efface le premier espace
	if(Pos>=S.Len) return Pos;

	if(S.D[Pos]=='+') 
	{
		Pos++;//efface le premier signe +
	}
	else if(S.D[Pos]=='-') 
	{
		Negative=true;
		Pos++;//efface le premier signe -
	}
	if(Pos>=S.Len) return Pos;
	while(V_InclusiveBound(S.D[Pos],(BYTE)'0',(BYTE)'9'))
	{
		v=10*v+S.D[Pos]-(BYTE)'0';
		Pos++;
		if(Pos>=S.Len) break;
	}


	if(Negative) v=-v;
	return Pos;
}

int SPG_CONV StrToFloat(float& v, SPGSTR& S, int Pos, char Separator='.')
{
	v=0.0f;
	bool Negative=false;

	CHECK(!V_IsBound(Pos,0,S.Len),"StrToInt",return Pos);
	if(S.D[Pos]==' ') Pos++;//efface le premier espace

	if(S.D[Pos]=='+') 
	{
		Pos++;//efface le premier signe +
	}
	else if(S.D[Pos]=='-') 
	{
		Negative=true;
		Pos++;//efface le premier signe -
	}
	if(Pos>=S.Len) return Pos;
	while(V_InclusiveBound(S.D[Pos],(BYTE)'0',(BYTE)'9'))
	{
		v=10*v+S.D[Pos]-(BYTE)'0';
		Pos++;
		if(Pos>=S.Len) goto StrToFloatFinal;
	}
	if(S.D[Pos]==Separator)
	{
		Pos++;
		if(Pos>=S.Len) goto StrToFloatFinal;
		float d=1.0f;
		while(V_InclusiveBound(S.D[Pos],(BYTE)'0',(BYTE)'9'))
		{
			d*=0.1f;
			v=v+d*(S.D[Pos]-(BYTE)'0');
			Pos++;
			if(Pos>=S.Len) goto StrToFloatFinal;
		}
	}
	if((S.D[Pos]=='e')||(S.D[Pos]=='E'))
	{
		int E=0;
		Pos++;
		if(Pos>=S.Len) goto StrToFloatFinal;
		Pos=StrToInt(E,S,Pos);
		v=v*powfInt(10,E);
	}


StrToFloatFinal:
	if(Negative) v=-v;
	return Pos;
}

//divers

int SPG_CONV StrFind(SPGSTR& S, char c)//retourne la position dans S de la correspondance
{
	for(int i=0;i<S.Len;i++)
	{
		if(S.D[i]==c) return i;
	}
	return -1;
}

/*
int SPG_CONV StrFind(SPGSTR& S, SPGSTR& ID)//retourne la position dans S de la correspondance
{
	return 0;
}

int SPG_CONV StrFindEnd(SPGSTR& S, SPGSTR& ID)//retourne le prochain caractere de S apres la chaine trouvée
{
	return StrFind(S,ID)+ID.Len;
}

void SPG_CONV StrInsert(SPGSTR& S, SPGSTR& I, int Pos)
{
}

void SPG_CONV StrCrop(SPGSTR& S, SPGSTR& I, int Pos)
{
}
*/

void SPG_CONV StrSupprLeft(SPGSTR& S, int Count)//supprime les caracteres par la gauche
{
	if(Count>=S.Len) {S.Len=0;return;}
	int i;
	for(i=Count;i<S.Len;i++)
	{
		S.D[i-Count]=S.D[i];
	}
	S.Len=i-Count;
	return;
}

/*
void SPG_CONV StrSupprRight(SPGSTR& S, int Count)//supprime les caracteres par la droite
{
}

void SPG_CONV StrUp(SPGSTR& S)//uppercase
{
}
*/

void SPG_CONV StrCatByteToAsciiHex(SPGSTR& D, SPGSTR& S)//chaine ascii de digits hexa vers données
{
	for(int i=0;i<S.Len;i++)
	{
		BYTE B=S.D[i]>>4;
		if(B<10) 
		{
			D.D[D.Len]=B+'0';
		}
		else
		{
			D.D[D.Len]=B+'A'-10;
		}
		D.Len++;
		CHECK(D.Len>=D.MaxLen,"StrCatByteToAsciiHex",return);

		B=(S.D[i]&15);
		if(B<10) 
		{
			D.D[D.Len]=B+'0';
		}
		else
		{
			D.D[D.Len]=B+'A'-10;
		}
		D.Len++;
		CHECK(D.Len>=D.MaxLen,"StrCatByteToAsciiHex",return);

		D.D[D.Len]=' ';
		D.Len++;
		CHECK(D.Len>=D.MaxLen,"StrCatByteToAsciiHex",return);
	}

	if(S.Len) 
	{
		D.D[D.Len]=0;//remplace le séparateuir surnuméraire par un zero
		D.Len--;//retire le séparateur surnuméraire
	}

	return;
}

void SPG_CONV StrCpyByteToAsciiHex(SPGSTR& D, SPGSTR& S)//chaine ascii de digits hexa vers données
{
	D.Len=0;
	StrCatByteToAsciiHex(D,S);
}

void SPG_CONV StrCatWordToAsciiHex(SPGSTR& D, SPGSTR& S)//chaine ascii de digits hexa vers données
{
	for(int i=0;i<S.Len;i+=2)
	{
		BYTE B=S.D[i+1]>>4;
		if(B<10) 
		{
			D.D[D.Len]=B+'0';
		}
		else
		{
			D.D[D.Len]=B+'A'-10;
		}
		D.Len++;
		CHECK(D.Len>=D.MaxLen,"StrCatWordToAsciiHex",return);

		B=(S.D[i+1]&15);
		if(B<10) 
		{
			D.D[D.Len]=B+'0';
		}
		else
		{
			D.D[D.Len]=B+'A'-10;
		}
		D.Len++;
		CHECK(D.Len>=D.MaxLen,"StrCatWordToAsciiHex",return);


		B=S.D[i]>>4;
		if(B<10) 
		{
			D.D[D.Len]=B+'0';
		}
		else
		{
			D.D[D.Len]=B+'A'-10;
		}
		D.Len++;
		CHECK(D.Len>=D.MaxLen,"StrCatWordToAsciiHex",return);

		B=(S.D[i]&15);
		if(B<10) 
		{
			D.D[D.Len]=B+'0';
		}
		else
		{
			D.D[D.Len]=B+'A'-10;
		}
		D.Len++;
		CHECK(D.Len>=D.MaxLen,"StrCatWordToAsciiHex",return);


		D.D[D.Len]=' ';
		D.Len++;
		CHECK(D.Len>=D.MaxLen,"StrCatWordToAsciiHex",return);
	}

	if(S.Len) 
	{
		D.D[D.Len]=0;//remplace le séparateur surnuméraire par un zero
		D.Len--;//retire le séparateur surnuméraire
	}

	return;
}

void SPG_CONV StrCpyWordToAsciiHex(SPGSTR& D, SPGSTR& S)//chaine ascii de digits hexa vers données
{
	D.Len=0;
	StrCatWordToAsciiHex(D,S);
}

void SPG_CONV StrCatAsciiHexToByte(SPGSTR& D, SPGSTR& S)
{
	int State=0;
	for(int i=0;i<S.Len;i++)
	{
		BYTE c=S.D[i];
		BYTE V;
		if(V_InclusiveBound(c,(BYTE)'0',(BYTE)'9'))
		{	//DbgCHECK(State==1,"Missing separator between bytes");
			V=c-'0';
		}
		else if(V_InclusiveBound(c,(BYTE)'A',(BYTE)'F'))
		{	//DbgCHECK(State==1,"Missing separator between bytes");
			V=c-'A'+10;
		}
		else
		{//autre caractère
			CHECKV(State!=0,"Found separator between hex digits at position:",i,State=0);//on est entre deux quartets
			continue;//tous les caracteres non ascii compris entre deux octets sont passés silencieusement
		}

		switch(State)
		{
		case 0://premier digit
			CHECK(D.Len>=D.MaxLen,"StrCatAsciiHexToByte",return);
			D.D[D.Len]=(V<<4);
			State++;
			break;
		case 1://second digit
			D.D[D.Len]|=V;
			State=0;
			D.Len++;
			break;
		default:
			break;
		}
	}
	return;
}

void SPG_CONV StrCpyAsciiHexToByte(SPGSTR& D, SPGSTR& S)//chaine ascii de digits hexa vers données
{
	D.Len=0;
	StrCatAsciiHexToByte(D,S);
	return;
}
