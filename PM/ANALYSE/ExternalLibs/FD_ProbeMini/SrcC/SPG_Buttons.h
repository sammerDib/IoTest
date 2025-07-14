
#ifdef SPG_General_USEButtons

#define B_MaxType 128
#define MaxButton 256
#define MaxJauge 64

typedef struct
{
	PixCoul Black;
	PixCoul White;
	PixCoul BlueColor;
	PixCoul GreenColor;
	PixCoul RedColor;
	PixCoul BackGround;
	PixCoul ForeGround;
	PixCoul FillColor;
} B_BaseColors;

typedef struct
{
	int Mode;//type de bouton

	G_Ecran* E;//sur quoi dessiner
	C_Lib* CL;//avec quelle police
//	B_Lib* BL;

	int PosX;
	int PosY;
	int SizeX;
	int SizeY;
	int Pitch;
	BYTE* TP;

	int Etat;//etat (clique,...) depend du type

	int NumDigits;

	//verifier la declaration
	//void*(B_Generic&) BCBK;

	union
	{
	void* UntypedToChange;
	float * VToChange;
	int * IntToChange;
	};
	union
	{
	DWORD UntypedIncrement;
	float Increment;
	int Flag;
	};
	float ValMin;
	float ValMax;
	union
	{
	DWORD OldUntyped;
	float OldV;
	int OldInt;
	};

	B_BaseColors BC;

} B_Generic;


typedef struct
{
	int Etat;

	DWORD ThreadId;

	int FocusButton;
	int SizeX[B_MaxType];
	int SizeY[B_MaxType];

	G_Ecran EIMG;//bitmap correspondant aux etats des bouton,
	//dans le BON FORMAT D'ECRAN
	//texturpos
	int Pitch;
	BYTE* TP[B_MaxType];
	//pointe de preference a
	//l'interieur de l'ecran

	B_BaseColors BC;


	B_Generic B[MaxButton];
} B_Lib;

#define NullButton 0
#define NullSprite 0

#define ClickButton 1
#define ClickSprite 1
//cliquable

#define CheckButton 2
#define CheckSprite 2

#define CheckIntButton 3
#define CheckIntSprite 2

#define AutoINCButton 4
#define AutoINCSprite 3
//incremente automatiquement une variable jusqua un max
#define AutoDECButton 5
#define AutoDECSprite 4
//decremente automatiquement une variable jusqua un min
#define AutoMultiINCButton 6
#define AutoMultiINCSprite 3
//incremente automatiquement une variable jusqua un max
//tant que le button est maintenu!
#define AutoMultiDECButton 7
#define AutoMultiDECSprite 4
//decremente automatiquement une variable jusqua un min
//tant que le button est maintenu!
#define ReglageHButton 8
#define ReglageHSprite 0

#define ReglageVButton 9
#define ReglageVSprite 0
/*
#define ReglageXYButton 9
#define ReglageXYSprite 0
*/
#define NumericButton 10
#define NumericSprite 0

#define CliquableNumericButton 11
#define CliquableNumericSprite 0
//MouseState
//#define MouseDN 1
#define NumericIntButton 12
#define NumericIntSprite 0

#define CliquableNumericIntButton 13
#define NumericIntSprite 0

//etat
#define B_Normal 0
#define B_Click 1
#define B_Change 2
#define B_Waiting 4

#ifdef IntelCompiler
#define B_ETAT_VOLATILE(BLib,NumBut) BLib.B[NumBut].Etat
#else
#define B_ETAT_VOLATILE(BLib,NumBut) BLib.B[NumBut].Etat
#endif

#define B_Set(BLib,NumBut,Flag) if (NumBut) BLib.B[NumBut].Etat=Flag
#define B_SetAndRedraw(BLib,NumBut,Flag) {B_Set(BLib,NumBut,Flag);B_RedrawButtonsLib(BLib,0);}

#define B_IsClick(BLib,NumBut) (B_ETAT_VOLATILE(BLib,NumBut)&B_Click)

#define B_IsNotClick(BLib,NumBut) ((B_ETAT_VOLATILE(BLib,NumBut)&B_Click)==0)
#define B_NotClick(BLib,NumBut) ((B_ETAT_VOLATILE(BLib,NumBut)&B_Click)==0)

#define B_IsChanged(BLib,NumBut) (B_ETAT_VOLATILE(BLib,NumBut)&B_Change)
#define B_Changed(BLib,NumBut) (B_ETAT_VOLATILE(BLib,NumBut)&B_Change)

#define B_IsChangedToClick(BLib,NumBut) ((B_ETAT_VOLATILE(BLib,NumBut)&(B_Click|B_Change))==(B_Click|B_Change))
#define B_ChangedToClick(BLib,NumBut) ((B_ETAT_VOLATILE(BLib,NumBut)&(B_Click|B_Change))==(B_Click|B_Change))
#define B_ClickDN(BLib,NumBut) ((B_ETAT_VOLATILE(BLib,NumBut)&(B_Click|B_Change))==(B_Click|B_Change))
#define B_ChangedAndClick(BLib,NumBut) ((B_ETAT_VOLATILE(BLib,NumBut)&(B_Click|B_Change))==(B_Click|B_Change))

#define B_IsChangedToNotClick(BLib,NumBut) ((B_ETAT_VOLATILE(BLib,NumBut)&(B_Click|B_Change))==(B_Change))
#define B_ClickUP(BLib,NumBut) ((B_ETAT_VOLATILE(BLib,NumBut)&(B_Click|B_Change))==(B_Change))

#define B_Exclude(BL,B0,B1)	if(B_IsChangedToClick(BL,B0)) B_SetAndRedraw(BL,B1,B_Normal);

#define B_HasFocus(BL) ((volatile int)BL.FocusButton>0)

#define BL_OK 1
#define BL_AUTOUPDATE 2

#include "SPG_Buttons.agh"

#endif

