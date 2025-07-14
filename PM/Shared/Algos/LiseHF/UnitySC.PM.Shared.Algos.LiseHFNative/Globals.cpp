////W. 15.7.12: I have introduced this file, in order to set some global variables here.
//
#include "RBT.h" //contains all remaining headers
//
//
////################################################################ Constants #######################################################################
//
//
cppc  cmOne(-1.0, 0),  cZero(0, 0),  cOne(1, 0),  cHalf(0.5, 0),  cIm(0, 1); 
CComplex CmOne, CZero, CHalf, COne, CTwo; //-1, 0,..., 2
void fInitConstants()
{
	CmOne = fc(-1,0), CZero = fc(0,0), CHalf = fc(0.5,0), COne = fc(1,0), CTwo = fc(2,0);
}

//
//
////################################################################ error handling ################################################################
//
//
BOOL bError_g = FALSE; 

BOOL bRes; // Flag f. Ausgabe in .out Datei, Filepointer FRes (Globals.h)
///* FILE-Zeiger */
//FILE* Fres = NULL; // Ergebnis (res: result)

