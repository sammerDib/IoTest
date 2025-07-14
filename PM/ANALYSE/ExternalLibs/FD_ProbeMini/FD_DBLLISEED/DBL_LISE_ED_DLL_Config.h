
#ifndef _DBL_LISE_ED_CONFIG_H_
#define _DBL_LISE_ED_CONFIG_H_

void ConfigSystemDefault(DBL_LISE_ED& DblLiseEd);
void CreateConfigFromFile(DBL_LISE_ED& DblLiseEd, char* ConfigFile);
void UpdateConfigFromHardwareConfig(DBL_LISE_ED& DblLiseEd, DBL_LISE_HCONFIG* HardwareConfigDual);
void SetTotalThicknessInCfgFile(DBL_LISE_ED& DblLiseEd, double CalibValue);
void SetZValueInCfgFile(DBL_LISE_ED& DblLiseEd, double ZValue);

#endif