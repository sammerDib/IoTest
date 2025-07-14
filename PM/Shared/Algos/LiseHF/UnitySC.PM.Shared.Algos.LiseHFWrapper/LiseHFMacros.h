#pragma once 
#pragma managed
#define SAFE_DELETE_GC(p) if(p != nullptr){ delete p; p=nullptr;}
#pragma managed(push, off)
#define SAFE_DELETE(p) if(p != nullptr){ delete p; p=nullptr;}
#define SAFE_DELETE_ARRAY(p) if(p != nullptr){ delete[] p; p=nullptr;}
#pragma managed(pop)