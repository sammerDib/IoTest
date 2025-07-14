#include "TypeAdapters.h"

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {

  using namespace System::Runtime::InteropServices;

  const char *string_to_char_array(String ^ string) {
    const char *str = (const char *)(Marshal::StringToHGlobalAnsi(string)).ToPointer();
    return str;
  }
} 