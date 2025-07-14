
#include "SPG_General.h"

#ifdef SPG_General_USEFileList

#include "SPG_Includes.h"

#include <memory.h>
/////////////////////////////////////////////////

//implementation de la liste de type SPG_FL_FILE

#define SPG_SpecType SPG_FL_FILE
#define SPG_SpecBlockSize FileListFilePerBlock
#define SPG_SpecMaxBlock FileListMaxBlock

#include "SpecList.cpp"

/////////////////////////////////////////////////

#endif
