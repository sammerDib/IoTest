
// ###################################################

#define sci_UID sci_UID_PACKET
#define sci_NAME sci_NAME_PACKET

// ###################################################

typedef struct SCI_PARAMETERS
{
	//parametres de l'interface
	int DummyInterfaceParam;
} SCI_PARAMETERS; //parametres de l'interface

// ###################################################

typedef struct
{
	SCX_ADDRESS* Address;
	SCI_CONNEXIONINTERFACE* CI;
	int BufferSize;
	int EnablePacketSize;
	int EnableChecksum;
	//int EnableRPacketSeqID;
	int EnableSTXETX;
	int Transcode;
	int MaxRetry;
} PACKETW_CONNEXION_ADDRESS;

typedef struct
{
	SCX_ADDRESS* Address;
	SCI_CONNEXIONINTERFACE* CI;
	int BufferSize;
	int EnablePacketSize;
	int EnableChecksum;
	//int EnableRPacketSeqID;
	int EnableSTXETX;
	int Transcode;
	int ChecksumOkAutoReply;
	int ChecksumErrorAutoReply;
	int EnableMultipleRead;
} PACKETR_CONNEXION_ADDRESS;

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire

	PACKETW_CONNEXION_ADDRESS W;
	PACKETR_CONNEXION_ADDRESS R;
} SCX_ADDRESS;

typedef struct
{
	SCX_CONNEXION* C;//connexion physique en lecture
	PACKET_BUFFER PB;
	BYTE* TranscodeBuffer;
	int TranscodeLen;
} PACKET_CONNEXION_STATE;

#define CHAR_STX 0x02
#define STR_STX "\x02"

#define CHAR_ETX 0x03
#define STR_ETX "\x03"

#define CHAR_ACK 0x06
#define STR_ACK "\x06"

#define CHAR_NAK 0x15
#define CHAR_NACK CHAR_NAK.
#define STR_NAK "\x15"
#define STR_NACK STR_NAK

#define CHAR_ACKR 0x05
#define STR_ACKR "\x05"

#define CHAR_NAKR 0x07
#define CHAR_NACKR CHAR_NAKR
#define STR_NAKR "\x07"
#define STR_NACKR STR_NAKR


// ###################################################

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;
	PACKET_CONNEXION_STATE W;
	PACKET_CONNEXION_STATE R;
//	PACKET_PROTOCOL_STATE P;
	SCX_CONNEXION* CTrash;//packet invalides
} SCX_STATE; //parametres d'une connexion en particulier

// ###################################################

int SPG_CONV scxTestPACKETRead(SCX_CONNEXION* C); //-1:un packet valide est en attente, 0:pas de packet valide, mais un packet en cours de décodage peut exister
int SPG_CONV scxFlushPACKETRead(SCX_CONNEXION* C);//vide tous les packets en attente y compris ceux en cours de décodage

