
#ifndef _WCHAR_T_DEFINED
typedef unsigned short wchar_t; //missing ';' before identifier 'wchar_t' -> verifier le ';' du header précédent dans le cpp qui cause l'erreur
#endif

#ifndef _WCTYPE_T_DEFINED
typedef wchar_t wint_t;
typedef wchar_t wctype_t;
#endif

#ifndef _VA_LIST_DEFINED
typedef char *  va_list;
#endif


