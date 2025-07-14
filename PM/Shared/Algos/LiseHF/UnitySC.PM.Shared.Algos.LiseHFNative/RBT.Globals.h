/* Globale Variablen für RBT */
#ifndef RBT_GLOBALS_H //W.Iff: um mehrmaliges Einbinden zu verhindern

#include "Nag_Compatibility.h" //definition of CComplex 
#include "AllWinHeaders.h"
#include "RBT.Typedefs.h" //wegen "pDataToReturn", das hier vorkommt, noetig.  W.Iff

//################################################################ Constants #######################################################################

extern cppc  cmOne, cZero, cOne, cHalf, cIm; 
extern CComplex CmOne, CZero, CHalf, COne, CTwo; //-1, 0,..., 2
void fInitConstants();

//################################################################ error handling ################################################################
extern BOOL bError_g; 


extern BOOL bRes; // Flag f. Ausgabe in .out Datei, Filepointer FRes (Globals.h)
/* FILE-Zeiger */
//extern FILE *Fres; // Ergebnis (res: result)


#define RBT_GLOBALS_H	//W.Iff
#endif //RBT_GLOBALS_H not defined