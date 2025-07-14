
#ifdef SPG_General_USESerialComm

typedef struct
{
	int Etat;
	void* hCom;
#ifdef DebugSerialComm
	void* F;
#endif
} SPG_SERIALCOMM;

#include "SPG_SerialComm.agh"
#define DropDTR(SSC) CommSetDTR(SSC,0)

#endif

