
/*

typedef struct
{
	int Version;
	int param1;
	float angle;
	...
} PARAMSSTRUCTNAME

#define PARAMSSTRUCTNAME_VERSION 1

#define PARAMSSTRUCTNAMEDEF_param1 4
#define PARAMSSTRUCTNAMEDEF_angle 0.5f

*/

#define SPG_STVIsValidVersion(StructName,StructType) (StructName.Version==StructType##_VERSION)
#define SPG_STVIsValidSize(StructType,sizeofStruct) (sizeof(StructTypeName)==sizeofStruct)
#define SPG_STVGetParam(Dest,ParamName,StructType,StructName,sizeofStruct) {if(((BYTE*)&(StructName.ParamName)-(BYTE*)&StructName<sizeofStruct)&&SPG_STVIsValidVersion(StructName,StructType)){Dest##ParamName=StructName.ParamName;}else{Dest##ParamName=StructType##DEF_##ParamName;}}
#define SPG_STVGetArray_static(Dest,ArrayName,ArraySize,StructType,StructName,sizeofStruct) {for(int i_LOCAL_MACRO=0;i_LOCAL_MACRO<ArraySize;i_LOCAL_MACRO++){if(((BYTE*)&(StructName.ArrayName[i_LOCAL_MACRO])-(BYTE*)&StructName<sizeofStruct)&&SPG_STVIsValidVersion(StructName,StructType)){Dest##ArrayName[i_LOCAL_MACRO]=StructName.ArrayName[i_LOCAL_MACRO];}else{Dest##ArrayName[i_LOCAL_MACRO]=StructType##DEF_##ArrayName[i_LOCAL_MACRO];}}}
#define SPG_STVGetArray_inline(Dest,ArrayName,ArraySize,StructType,StructName,sizeofStruct) {for(int i_LOCAL_MACRO=0;i_LOCAL_MACRO<ArraySize;i_LOCAL_MACRO++){if(((BYTE*)&(StructName.ArrayName[i_LOCAL_MACRO])-(BYTE*)&StructName<sizeofStruct)&&SPG_STVIsValidVersion(StructName,StructType)){Dest##ArrayName[i_LOCAL_MACRO]=StructName.ArrayName[i_LOCAL_MACRO];}else{Dest##ArrayName[i_LOCAL_MACRO]=StructType##DEF_##ArrayName(i_LOCAL_MACRO);}}}
#define SPG_STVGet2DArray_static(Dest,ArrayName,ArraySizeY,ArraySizeX,StructType,StructName,sizeofStruct) {for(int j_LOCAL_MACRO=0;j_LOCAL_MACRO<ArraySizeY;j_LOCAL_MACRO++){for(int i_LOCAL_MACRO=0;i_LOCAL_MACRO<ArraySizeX;i_LOCAL_MACRO++){if(((BYTE*)&(StructName.ArrayName[j_LOCAL_MACRO][i_LOCAL_MACRO])-(BYTE*)&StructName<sizeofStruct)&&SPG_STVIsValidVersion(StructName,StructType)){Dest##ArrayName[j_LOCAL_MACRO][i_LOCAL_MACRO]=StructName.ArrayName[j_LOCAL_MACRO][i_LOCAL_MACRO];}else{Dest##ArrayName[j_LOCAL_MACRO][i_LOCAL_MACRO]=StructType##DEF_##ArrayName[j_LOCAL_MACRO][i_LOCAL_MACRO];}}}}
//#define SPG_STVGet2DArray_inline(Dest,ArrayName,ArraySizeY,ArraySizeX,StructType,StructName,sizeofStruct) {for(int j_LOCAL_MACRO=0;j_LOCAL_MACRO<ArraySizeY;j_LOCAL_MACRO++){for(int i_LOCAL_MACRO=0;i_LOCAL_MACRO<ArraySizeX;i_LOCAL_MACRO++){if(((BYTE*)&(StructName.ArrayName[j_LOCAL_MACRO][i_LOCAL_MACRO])-(BYTE*)&StructName<sizeofStruct)&&SPG_STVIsValidVersion(StructName,StructType)){Dest##ArrayName[j_LOCAL_MACRO][i_LOCAL_MACRO]=StructName.ArrayName[j_LOCAL_MACRO][i_LOCAL_MACRO];}else{Dest##ArrayName[j_LOCAL_MACRO][i_LOCAL_MACRO]=StructType##DEF_##ArrayName(j_LOCAL_MACRO,i_LOCAL_MACRO);}}}
