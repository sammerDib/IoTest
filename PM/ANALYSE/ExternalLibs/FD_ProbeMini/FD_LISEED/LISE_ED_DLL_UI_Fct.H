/*
 * $Id: LISE_ED_DLL_UI_Fct.H 6640 2008-01-07 14:52:41Z y-randle $
 */

//Fonctions appelées de l'exterieur

int LEDIOpenSettingsWindow(void* s,int ShowGainSetting);
int LEDIUpdateSettingsWindow(void* s);
int LEDICloseSettingsWindow(void* s);

//Fonctions appelées par le système lui même
/*
void LEDUIUpdateOnInit(LISE_ED& LISE);
void LEDUIUpdateOnClose(LISE_ED& LISE);
void LEDUIUpdateOnAcqAndProcess(LISE_ED& LISE);
void LEDUIUpdateOnStart(LISE_ED& LISE);
void LEDUIUpdateOnStop(LISE_ED& LISE);
*/
