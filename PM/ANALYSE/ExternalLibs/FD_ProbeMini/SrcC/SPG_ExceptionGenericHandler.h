
#ifdef SPG_General_USEGENERICEXCEPTIONHANDLER

void SPG_CONV GenericSnapShotFct(LPEXCEPTION_POINTERS e, const char* fctname);
int SPG_CONV GenericExceptionFilter(LPEXCEPTION_POINTERS e, void SPG_CONV snapshotfct(LPEXCEPTION_POINTERS,const char*) , const char* fctname);
int SPG_CONV GenericExceptionHandlerRZ(); //return 0

/**
 * This macro starts a new try-catch block. Provide a name for the block.
 */
#define TRY_BEGIN			__try {

#define TRY_ENDGRZ(Msg) } __except ( GenericExceptionFilter(GetExceptionInformation(),GenericSnapShotFct,XCPTSTR(__FILE__,__FUNCTION__,__LINE__,Msg)) ) {	return GenericExceptionHandlerRZ(); }	
#define TRY_ENDG(Msg) } __except ( GenericExceptionFilter(GetExceptionInformation(),GenericSnapShotFct,XCPTSTR(__FILE__,__FUNCTION__,__LINE__,Msg)) ) {	GenericExceptionHandlerRZ(); }	
#define TRY_END(filterfct,snapshotfct,Msg,handlerinstr) } __except ( filterfct(GetExceptionInformation(),snapshotfct,XCPTSTR(__FILE__,__FUNCTION__,__LINE__,Msg)) ) {	handlerinstr; }	

#else

#define TRY_BEGIN						{

#define TRY_ENDGRZ(fctname)		 }	
#define TRY_ENDG(fctname)			 }
#define TRY_END(filterfct,snapshotfct,fctname,handlerinstr)	 }

#endif

