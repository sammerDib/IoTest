
#ifndef DebugNetwork

#define scxSendDebugConnexionAddress(FctName, Message, CI, Address)
#define scxSendDebugAddress(FctName, Message, CI, Address)
#define scxSendDebugConnexion(FctName, Message, C)
#define scxSendDebugDataRetval(FctName, Message, Data, DataLen, Retval, C)
#define scxSendDebugRetval(FctName, Message, Retval)
#define scxSendDebugConnexionRetval(FctName, Message, Retval, C)
#define scxSendDebugConnexionRetval(FctName, Message, Retval, C)
#define scxSendDebugData(FctName, Message, Data, DataLen, C)
#define scxSendDebugConnexionLen(FctName, Message, Data, DataLen, C)

#else

void SPG_CONV scxStartDebug(MSGCALLBACK MsgCallback, void* UserData);
void SPG_CONV scxSendDebug();
void SPG_CONV scxPartialFillAddress(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address);
void SPG_CONV scxSendDebugConnexionAddress(char* FctName, char* Message, SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address);
void SPG_CONV scxSendDebugConnexion(char* FctName, char* Message, SCX_CONNEXION* C);
void SPG_CONV scxSendDebugDataRetval(char* FctName, char* Message, void* Data, int DataLen, int Retval, SCX_CONNEXION* C);
void SPG_CONV scxSendDebugRetval(char* FctName, char* Message, int Retval);
void SPG_CONV scxSendDebugConnexionRetval(char* FctName, char* Message, int Retval, SCX_CONNEXION* C);
void SPG_CONV scxSendDebugData(char* FctName, char* Message, void* Data, int DataLen, SCX_CONNEXION* C);
void SPG_CONV scxSendDebugConnexionLen(char* FctName, char* Message, void* Data, int DataLen, SCX_CONNEXION* C);

#endif

