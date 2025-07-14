
#define SPGSTR_OK			1
#define SPGSTR_ALLOC		2
#define SPGSTR_ALIAS		4
#define SPGSTR_Z			8
#define SPGSTR_DRIVE		16
#define SPGSTR_PATH			32
#define SPGSTR_FILENAME		64
#define SPGSTR_EXTENS		128

typedef struct
{
	int State;
	int MaxLen;
	int Len;
	BYTE* D;
} SPGSTR;

#define StrDeclare(S) SPGSTR S;StrInit(S);

void SPG_CONV StrInit(SPGSTR& S, int MaxLen=1024);
void SPG_CONV StrAliasInit(SPGSTR& SubString, BYTE* D, int Len, int MaxLen);
void SPG_CONV StrAliasInit(SPGSTR& S, BYTE* D, int Len);
void SPG_CONV StrSubInit(SPGSTR& SubString, SPGSTR& S, int Start, int Len);
void SPG_CONV StrClose(SPGSTR& S);

char* SPG_CONV StrZGet(SPGSTR& S);//internal use - ajoute le zero terminal ou raccourcit la chaine et indique si la chaine est valide - alternative à StrCpy
int SPG_CONV StrZLen(char* StrZ);//internal use
void SPG_CONV StrZtoStr(SPGSTR& S);//internal use

//string vers string

void SPG_CONV StrCpy(SPGSTR& D, SPGSTR& S);
void SPG_CONV StrCpy(SPGSTR& D, SPGSTR& S, int SrcStart, int Len);
void SPG_CONV StrCpy(SPGSTR& D, int DestStart, SPGSTR& S, int SrcStart, int Len);

void SPG_CONV StrCat(SPGSTR& D, SPGSTR& S);
void SPG_CONV StrCat(SPGSTR& D, SPGSTR& S, int SrcStart, int Len);

//C types vers string

void SPG_CONV StrCpy(SPGSTR& D, void* Data, int Len);
void SPG_CONV StrCpy(SPGSTR& D, char* SrcStrZ);
void SPG_CONV StrCpy(SPGSTR& S, BYTE b);
void SPG_CONV StrCpy(SPGSTR& S, WORD b);
void SPG_CONV StrCpy(SPGSTR& S, DWORD b);
void SPG_CONV StrCpy(SPGSTR& S, int v, int MinDigit=0);
void SPG_CONV StrCpy(SPGSTR& S, float v, int NumDigit=5, char Separateur='.');

// Constructeurs tous types

#define StrNew(S,MaxLen,Initializer) {StrInit(S,MaxLen);StrCpy(S,MaxLen,Initializer);}

//C types vers string

void SPG_CONV StrCat(SPGSTR& D, void* Data, int Len);
void SPG_CONV StrCat(SPGSTR& D, char* SrcStrZ);
void SPG_CONV StrCat(SPGSTR& D, BYTE b);
void SPG_CONV StrCat(SPGSTR& D, WORD b);
void SPG_CONV StrCat(SPGSTR& D, DWORD b);
void SPG_CONV StrCat(SPGSTR& S, int v, int MinDigit=0);
void SPG_CONV StrCat(SPGSTR& S, float v, int NumDigit=5, char Separateur='.');

//string vers C Type

void SPG_CONV StrToByte(void* Data, int MaxLen, SPGSTR& S);
int SPG_CONV StrToInt(int& v, SPGSTR& S, int Pos=0);
int SPG_CONV StrToFloat(float& v, SPGSTR& S, int Pos=0);

//divers

void SPG_CONV StrRemLeft(SPGSTR& S, int Count);
void SPG_CONV StrAddLeft(SPGSTR& S, char c);
void SPG_CONV StrRemRight(SPGSTR& S, int Count);
void SPG_CONV StrAddRight(SPGSTR& S, char c);

int SPG_CONV StrFind(SPGSTR& S, char c);//retourne la position dans S de la correspondance
int SPG_CONV StrFind(SPGSTR& S, SPGSTR& ID);//retourne la position dans S de la correspondance
int SPG_CONV StrFindEnd(SPGSTR& S, SPGSTR& ID);//retourne le prochain caractere de S apres la chaine trouvée

void SPG_CONV StrUp(SPGSTR& S);//uppercase
void SPG_CONV StrDn(SPGSTR& S);//lowercase

void SPG_CONV StrCatByteToAsciiHex(SPGSTR& D, SPGSTR& S);//chaine ascii de digits hexa vers données
void SPG_CONV StrCpyByteToAsciiHex(SPGSTR& D, SPGSTR& S);//chaine ascii de digits hexa vers données

void SPG_CONV StrCatWordToAsciiHex(SPGSTR& D, SPGSTR& S);//chaine ascii de digits hexa vers données
void SPG_CONV StrCpyWordToAsciiHex(SPGSTR& D, SPGSTR& S);//chaine ascii de digits hexa vers données

void SPG_CONV StrCatAsciiHexToByte(SPGSTR& D, SPGSTR& S);//chaine ascii de digits hexa vers données
void SPG_CONV StrCpyAsciiHexToByte(SPGSTR& D, SPGSTR& S);//chaine ascii de digits hexa vers données


void SPG_CONV StrSupprLeft(SPGSTR& S, int Count);//supprime les caracteres par la gauche


#define StrCpy2(S,Msg0,Msg1)						{StrCpy(S,Msg0);StrCat(S,Msg1);}
#define StrCpy3(S,Msg0,Msg1,Msg2)					{StrCpy(S,Msg0);StrCat(S,Msg1);StrCat(S,Msg2);}
#define StrCpy4(S,Msg0,Msg1,Msg2,Msg3)				{StrCpy(S,Msg0);StrCat(S,Msg1);StrCat(S,Msg2);StrCat(S,Msg3);}
#define StrCpy5(S,Msg0,Msg1,Msg2,Msg3,Msg4)			{StrCpy(S,Msg0);StrCat(S,Msg1);StrCat(S,Msg2);StrCat(S,Msg3);StrCat(S,Msg4);}

#define StrInit1(S,MaxLen,Msg)						{StrInit(S,MaxLen);StrCpy(S,Msg);}
#define StrInit2(S,MaxLen,Msg0,Msg1)				{StrInit(S,MaxLen);StrCpy2(S,Msg0,Msg1);}
#define StrInit3(S,MaxLen,Msg0,Msg1,Msg2)			{StrInit(S,MaxLen);StrCpy3(S,Msg0,Msg1,Msg2);}
#define StrInit4(S,MaxLen,Msg0,Msg1,Msg2,Msg3)		{StrInit(S,MaxLen);StrCpy4(S,Msg0,Msg1,Msg2,Msg3);}
#define StrInit5(S,MaxLen,Msg0,Msg1,Msg2,Msg3,Msg4) {StrInit(S,MaxLen);StrCpy5(S,Msg0,Msg1,Msg2,Msg3,Msg4);}
