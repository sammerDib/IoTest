
typedef void(SPG_CONV * SPG_DEFINELIST_CALLBACK)(void* DMV, char* Msg, int Defined, char* Value);

void SPG_CONV SPG_Config_Save(char* Filename);
void SPG_CONV SPG_Config_WriteToString(char* Msg, int MaxLen);

void SPG_CONV SPG_Config_EnumDefines(SPG_DEFINELIST_CALLBACK SPG_DisplayMacroValue, void* DMV);


