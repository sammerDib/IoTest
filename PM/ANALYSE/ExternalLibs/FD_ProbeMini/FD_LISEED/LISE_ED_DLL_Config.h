/*
 * $Id: LISE_ED_DLL_Config.h 7662 2008-10-23 07:30:36Z m-abet $
 */

#ifndef LISE_ED_DLL_CONFIG_H
#define LISE_ED_DLL_CONFIG_H

#include "LISE_ED_DLL_Internal.h"

int ConfigSystemDefault(LISE_ED& LiseEd);
int GetNidaqmxVersion(LISE_ED& LiseEd);
void ConfigInitFromFile(LISE_ED& LiseEd,char* ConfigFile);
void UpdateConfigFromHardwareConfig(LISE_ED& LiseEd, LISE_HCONFIG* HardwareConfig);

#endif