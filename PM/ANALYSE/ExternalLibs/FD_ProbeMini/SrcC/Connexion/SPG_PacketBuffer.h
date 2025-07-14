
//PACKET_BUFFER_ELT.State, pkBufferFillElt(Flag)

#define PACKET_EMPTY		0
#define PACKET_PARTIAL		1
#define PACKET_FULL			2
#define PACKET_FILLSTATE_MASK (1+2)

#define PACKET_INVALIDSTX 8
#define PACKET_INVALIDETX 16
#define PACKET_INVALIDSIZE 32
#define PACKET_INVALIDCHECKSUM 64
//#define PACKET_INVALIDSEQID	32
#define PACKET_ERRORSTATE_MASK (8+16+32+64)

#define PACKET_OK(State) ((State&(PACKET_FILLSTATE_MASK|PACKET_ERRORSTATE_MASK))==PACKET_FULL)

#define PACKET_HASDATA		128

#define PACKET_CMD_CREATE	256
#define PACKET_CMD_DATA		512
#define PACKET_CMD_CLOSE	1024

typedef struct
{
	int State;
	int Start;//en comptant header - trailer
	int Len;//en comptant header - trailer
	int DataStart;//payload
	int DataLen;//payload
	S_TimerCountType StartTransmit;
	S_TimerCountType EndTransmit;
} PACKET_BUFFER_ELT;

typedef struct
{
	int Size;
	void* B;
	int MaxPacket;
	int NumPacket;
	PACKET_BUFFER_ELT* P;
	S_TIMER T;
	//S_TimerCountType StartTime;
	//int SeqID;
} PACKET_BUFFER;

int SPG_CONV pkBufferCreate(PACKET_BUFFER& P, int Size);
void SPG_CONV pkBufferDestroy(PACKET_BUFFER& P);
int SPG_CONV pkBufferFillLastPacket(PACKET_BUFFER& P, void* Data, int Size, int Flag);
int SPG_CONV pkBufferCutLastPacket(PACKET_BUFFER& P, int FlagLeft, int CutOffset, int FlagRight);
int SPG_CONV pkBufferGetLastPacketState(PACKET_BUFFER& P);
PACKET_BUFFER_ELT* SPG_CONV pkBufferGetLastPacket(PACKET_BUFFER& P);
int SPG_CONV pkBufferGetFirstPacketState(PACKET_BUFFER& P);
int SPG_CONV pkBufferHasFirstPacket(PACKET_BUFFER& P);
int SPG_CONV pkBufferGetFirstPacketRRaw(PACKET_BUFFER& P, void* &PacketData, int &PacketDataSize);
int SPG_CONV pkBufferGetFirstPacketRData(PACKET_BUFFER& P, void* &PacketData, int &PacketDataSize);
int SPG_CONV pkBufferGetFirstPacketWData(PACKET_BUFFER& P, void* &PacketHeaderAndData, int &PacketHeaderAndDataSize);
void SPG_CONV pkBufferGetFirstPacket(PACKET_BUFFER& P, PACKET_BUFFER_ELT& PELT);
int SPG_CONV pkBufferRemoveFirstPacket(PACKET_BUFFER& P);
