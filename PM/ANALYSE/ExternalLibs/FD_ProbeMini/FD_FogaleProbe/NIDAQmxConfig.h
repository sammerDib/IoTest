#ifndef NOHARDWARE //define provenant des predefinitions du projet (project settings)

#include <windows.h>
#include "..\NIDAQ_SDK\DAQmx ANSI C Dev\include\NIDAQmx.h"
#ifdef _WIN64
#pragma comment(lib,"C:\\Program Files (x86)\\National Instruments\\Shared\\ExternalCompilerSupport\\C\\lib64\\msvc\\NIDAQmx.lib")
#else
#pragma comment(lib,"NIDAQ_SDK\\DAQmx ANSI C Dev\\lib\\msvc\\NIDAQmx.lib")
#endif


#define MAJOR_VERSION 8
#define MINOR_VERSION 5

//synonymes de ifndef(NOHARDWARE)
#define USE_NIDAQMX
#define DEVICECONNECTED

#else

#pragma message(__FILE__"(7): NIDAQmx inactif en configuration DebugNoHardware")
#include "..\\NIDAQ_SDK\\DAQmx ANSI C Dev\\include\\DummyNIDAQmx.h"

#endif

