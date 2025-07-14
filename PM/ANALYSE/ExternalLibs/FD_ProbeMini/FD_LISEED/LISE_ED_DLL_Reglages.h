/*
 * $Id: LISE_ED_DLL_Reglages.h 8257 2009-02-16 17:52:50Z S-PETITGRAND $
 */

#ifndef LISE_ED_DLL_REGLAGE_H
#define LISE_ED_DLL_REGLAGE_H

int SetAnalogOutputSource1(LISE_ED& LiseEd,float ValeurSource1);
int SetAnalogOutputSource2(LISE_ED& LiseEd,float ValeurSource2);
int SetAnalogOutput(LISE_ED& LiseEd,float ValeurSource1, float ValeurSource2);
int EnableScanOn(LISE_ED& LiseEd);
int EnableScanOff(LISE_ED& LiseEd);
int SwitchPRecoupOne(LISE_ED& LiseEd);
int SwitchPRecoupZero(LISE_ED& LiseEd);
int LRIsValid(LISE_ED& LiseEd);

#endif