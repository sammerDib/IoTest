/*
 * $Id: LISE_ED_DLL_Process.h 7662 2008-10-23 07:30:36Z m-abet $
 */

#ifndef LISE_ED_DLL_PROCESS_H
#define LISE_ED_DLL_PROCESS_H

#include "LISE_ED_DLL_Internal.h"

// fonction pour le process des deux différentes méthodes de GetThickness
int LEDGetContinousThickness(LISE_ED& LiseEd, double* Thickness, double* Quality,int NbThickness);
int LEDGetSingleShotThicknessWithTimeout(LISE_ED& LiseEd, double* Thickness, double* Quality,int NbThickness);
int LEDGetSingleShotThickness(LISE_ED& LiseEd, double* Thickness, double* Quality,int NbThickness);


// fonction pour le process et calcul dans la boucle d'acquisition
int LEDPulseDetection(LISE& Lise,LISE_ED& led,PICRESULT* BufferResultat,double* Buffer,RING_BUFFER_POS &WriteResultChannelProcess, int Voie);
int GetPic(LISE_ED& LiseEd);
int FindPicInBuffer(LISE_ED& LiseEd,int voie);

// Fonction pour l'autoGain
int ProcessAutoGain_SingleShot_Mode(LISE_ED* LiseEd);
int ProcessAutoGain_Continuous_Mode(LISE_ED* LiseEd);
double ProcessAutoGain(LISE_ED* LiseEd);

#endif