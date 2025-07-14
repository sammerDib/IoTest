
#ifdef SPG_General_USECDCHECK

typedef struct
{
	union
	{
		struct
		{
			BYTE B[32];
		};
		struct
		{
			BYTE B0;
			BYTE B1;
			BYTE B2;
			BYTE B3;
			BYTE B4;
			BYTE B5;
			BYTE B6;
			BYTE B7;
			BYTE B8;
			BYTE B9;
			BYTE B10;
			BYTE B11;
			BYTE B12;
			BYTE B13;
			BYTE B14;
			BYTE B15;
			BYTE B16;
			BYTE B17;
			BYTE B18;
			BYTE B19;
			BYTE B20;
			BYTE B21;
			BYTE B22;
			BYTE B23;
			BYTE B24;
			BYTE B25;
			BYTE B26;
			BYTE B27;
			BYTE B28;
			BYTE B29;
			BYTE B30;
			BYTE B31;
		};
	};
} CRYPTED_ARRAY_32;


#define IF_CD_CHECK(IMMED_v_0_31,ThisVal_CA32,Instruction) if(CD_CHECK_KEY_B##IMMED_v_0_31!=ThisVal_CA32.B##IMMED_v_0_31) {Instruction;};

#define IF_CD_G_CHECK(IMMED_v_0_31,Instruction) IF_CD_CHECK(IMMED_v_0_31,Global.CD_UID,Instruction)

#define CD_CHECK_EXIT(IMMED_v_0_31,IMMED_x_0_31,ThisVal_CA32) IF_CD_CHECK(IMMED_v_0_31,ThisVal_CA32,CD_Exit##IMMED_x_0_31())

#define CD_G_CHECK_EXIT(IMMED_v_0_31,IMMED_x_0_31) CD_CHECK_EXIT(IMMED_v_0_31,IMMED_x_0_31,Global.CD_UID)

/*
Ceci est la clef a changer au cas par cas
*/
#include "CD_CHECK_KEY.h"

void SPG_CONV CD_CheckFile(CRYPTED_ARRAY_32& CD_UID, char* S);

void __stdcall  CD_Exit0(void);
void __stdcall  CD_Exit1(void);
void __stdcall  CD_Exit2(void);
void __stdcall  CD_Exit3(void);
void __stdcall  CD_Exit4(void);
void __stdcall  CD_Exit5(void);
void __stdcall  CD_Exit6(void);
void __stdcall  CD_Exit7(void);
void __stdcall  CD_Exit8(void);
void __stdcall  CD_Exit9(void);

void __stdcall  CD_Exit10(void);
void __stdcall  CD_Exit11(void);
void __stdcall  CD_Exit12(void);
void __stdcall  CD_Exit13(void);
void __stdcall  CD_Exit14(void);
void __stdcall  CD_Exit15(void);
void __stdcall  CD_Exit16(void);
void __stdcall  CD_Exit17(void);
void __stdcall  CD_Exit18(void);
void __stdcall  CD_Exit19(void);

void __stdcall  CD_Exit20(void);
void __stdcall  CD_Exit21(void);
void __stdcall  CD_Exit22(void);
void __stdcall  CD_Exit23(void);
void __stdcall  CD_Exit24(void);
void __stdcall  CD_Exit25(void);
void __stdcall  CD_Exit26(void);
void __stdcall  CD_Exit27(void);
void __stdcall  CD_Exit28(void);
void __stdcall  CD_Exit29(void);


void __stdcall  CD_Exit30(void);
void __stdcall  CD_Exit31(void);

#endif



