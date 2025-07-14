
#ifdef SPG_General_USEFiles

#include "SPG_Files.agh"

#define SPG_ALL 0
#define SPG_TXT 1
#define SPG_BMP 2
#define SPG_AVI 3
#define SPG_WAV 4
#define SPG_RGR 5
#define SPG_TPO 6
#define SPG_RS  7
#define SPG_RAW 8
#define SPG_AI 9
#define SPG_DI 10

#define LOCALTIMEFORMATSTRING "y%0.4i_m%0.2i_d%0.2i_%0.2ih%0.2im%0.2is"
#define LOCALTIMEPARAMETERS(STime) (int)STime.wYear,(int)STime.wMonth,(int)STime.wDay,(int)STime.wHour,(int)STime.wMinute,(int)STime.wSecond

#endif

