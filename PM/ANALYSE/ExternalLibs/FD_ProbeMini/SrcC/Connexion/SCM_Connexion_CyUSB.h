
#ifdef SPG_General_USECyUSB
SCI_CONNEXIONINTERFACE* SPG_CONV sciCyUSBCreateConnexionInterface();
int SPG_CONV scxCyUSBRead(void* Data, int& DataLen, SCX_CONNEXION* C, int EndPointNr);
#endif
