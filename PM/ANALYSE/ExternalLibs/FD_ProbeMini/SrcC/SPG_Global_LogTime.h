
#ifdef SPG_General_USEWLTG_GLOBAL

void SPG_CONV SPG_GlobalLogTimeInit();
void SPG_CONV SPG_GlobalLogTimeClose();

#else

#define SPG_GlobalLogTimeInit();
#define SPG_GlobalLogTimeClose();

#endif

