
#define M_Start					0x00000001
#define M_Stop					0x00000002

#define M_RAW					0x00000004 //format RAW
#define M_BinaryI16				0x00000008 //format I16
#define M_AnalogF64				0x00000010 //format floating point (double)
#define M_DigitalLines			0x00000020 //format BYTE

#define M_Read					0x00000100
#define M_Write					0x00000200
#define M_CreateAIVoltageChan	0x00000400
#define M_CreateAOVoltageChan	0x00000800
#define M_CreateDIChan			0x00001000
#define M_CreateDOChan			0x00002000
#define M_GetAIConvMaxRate		0x00004000
#define M_CfgSampClkTiming		0x00008000
#define M_CounterClkTiming		0x00010000

/*
#define M_READ_FREQ_I16 (M_Read |M_Start|M_Stop|M_CreateAIVoltageChan|M_CfgSampClkTiming|M_BinaryI16)
#define M_WRITE_F64     (M_Write|M_Start|M_Stop|M_CreateAOVoltageChan|         0        |M_AnalogF64)
#define M_READ_DIO      (M_Read |M_Start|M_Stop|M_CreateDIChan       |         0        |M_DigitalLines)
#define M_WRITE_DIO     (M_Write|M_Start|M_Stop|M_CreateDOChan       |         0        |M_DigitalLines)
*/

#define ONBOARDCLOCK "OnboardClock"
#define AICLOCK "ai/SampleClock"
#define COUNTER0CLOCK "Ctr0InternalOutput"


/*

//20+200+2000+8000 = 41504
//efine M_WRITE_FREQ_RAWDIO  M_RAW|M_CfgSampClkTiming

  
	//2 + 4+8 + 20 + 200+8000 = 33326
#define M_WRITE_FREQ_RAWAIO  (M_Write|M_Start|M_Stop|M_CreateAOChan       |M_CfgSampClkTiming|M_RAW)

//1 + 4+8 + 10 + 200+8000 = 33309
#define M_READ_FREQ_RAWAIO   (M_Write|M_Start|M_Stop|M_CreateAIChan       |M_CfgSampClkTiming|M_RAW)

//2 + 4+8 + 80 + 200+8000 = 33422
//efine M_WRITE_FREQ_RAWDIO  (M_Write|M_Start|M_Stop|M_CreateDOChan       |M_CfgSampClkTiming|M_RAW)

//2 + 8 + 80 + 200+8000 = 33418
#define M_WRITE_FREQ_RAWDIO  (M_Write|   0   |M_Stop|M_CreateDOChan       |M_CfgSampClkTiming|M_RAW)

//1 + 4+8 + 40 + 200+8000 = 33357
#define M_READ_FREQ_RAWDIO   (M_Write|M_Start|M_Stop|M_CreateDIChan       |M_CfgSampClkTiming|M_RAW)
*/
