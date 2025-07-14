
// c:\sylvain\devc_svn\svn_root\capablademodule\connexion\..\..\NIDAQ_SDK\SPG_NIDAQmxConfig.h

#ifdef NOHARDWARE //  !defined(SPG_General_USENIDAQmx) && (!defined(SC_General_USENIDAQmx))
#include "DAQmx ANSI C Dev\include\DummyNIDAQmx.h"
#pragma SPGMSG(__FILE__,__LINE__,"NIDAQmx inactive")
#else
//ragma SPGMSG(__FILE__,__LINE__,"NIDAQmx active")
#include "DAQmx ANSI C Dev\include\NIDAQmx.h"
//http://pyrwi.googlecode.com/svn-history/r3/trunk/nidaqmx/nidaqmx.i
int32 __CFUNC     DAQmxGetNthTaskChannel   (TaskHandle taskHandle, uInt32 index, char buffer[], int32 bufferSize);
int32 __CFUNC     DAQmxGetNthTaskDevice    (TaskHandle taskHandle, uInt32 index, char buffer[], int32 bufferSize);
int32 __CFUNC DAQmxGetTaskNumDevices(TaskHandle taskHandle, uInt32 *data);
int32 __CFUNC DAQmxGetDevProductCategory(const char device[], int32 *data);
//http://www.pamguard.org/devDocs/constant-values.html#nidaqdev.NIConstants.DAQmx_Val_CSeriesModule
#define DAQmx_Val_CSeriesModule 14659
#define DAQmx_Val_SCXIModule 14660 

///int32 DAQmxGetTaskNumDevices(TaskHandle taskHandle, uInt32 index, char buffer[], int32 bufferSize); //cette fonction est utilisée par les exemples mais ni déclarée ni documentée dans le SDK //http://forums.ni.com/t5/Multifunction-DAQ/DAQmxGetNthTaskDevice-is-UNDOCUMENTED-but-used-in-an-example/m-p/504198
#ifdef SRCC_CAPABLADE
#pragma comment(lib,"FDE..\\..\\NIDAQ_SDK\\DAQmx ANSI C Dev\\lib\\msvc\\NIDAQmx.lib")
#else
#pragma comment(lib,"NIDAQ_SDK\\DAQmx ANSI C Dev\\lib\\msvc\\NIDAQmx.lib")
#endif

#endif // NOHARDWARE

#ifdef SC_General_USERefCount
#define SCM_NIDAQmxCreateTask(task) ( SC_ADDREF(task,#task), DAQmxCreateTask(task.getUniqueName(),&task) )
#define SCM_NIDAQmxDestroyTask(task) ( SC_REMREF(task), DAQmxClearTask(task) )
#else
#define SCM_NIDAQmxCreateTask(task) DAQmxCreateTask("",&task)
#define SCM_NIDAQmxDestroyTask(task) DAQmxClearTask(task)
#endif
