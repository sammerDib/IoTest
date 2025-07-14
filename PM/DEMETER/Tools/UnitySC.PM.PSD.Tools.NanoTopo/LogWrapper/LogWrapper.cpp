#include "LogWrapper.h"
#include "LogWrapperInterface.h"

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace LogWorker;

namespace Wrapper
{
#ifdef __cplusplus
extern "C"
{
#endif

	__declspec(dllexport) void LogThis(int nSrc, int nTypeEvt, char* pMsg)
	{
		LogObsWorker ^ oLogObsWorker = LogWrapper::Instance->GetLogObsWorker();
		String^ SMsg = gcnew String(pMsg);
		oLogObsWorker->WriteEntry(nSrc, nTypeEvt, SMsg);
	}

// 	void copyManagedStringToCharPointer(char target[],System::String ^ inputString);
// 	void copyManagedStringToCharPointer(char target[],System::String ^ inputString)
//       {
//          int maxSize = inputString->Length;
//          if ( maxSize > 0) 
//          {
//             for (int index = 0; index < maxSize; ++index ) 
//             {
//                target[index] = (char)inputString->default[index];
//             }
// 				target[maxSize] = '\0';
//          }
//       }
	
#ifdef __cplusplus
}
#endif
}