#include <string>
#include <codecvt>
#include <locale>
#include <vector>

#define MAXLOGMSG 512
#define MAXLOGNAME 512
#define MAXLOGDATE 64

typedef struct
{
	int Etat;
	int CurSize;
	int MaxSize;
	int Flag;

	void* hFile;

	int Count; //handles ouverts

#ifdef SPG_General_USESpinLock
	SPINLOCK L;
#endif

#ifdef SPG_General_USETimer
	S_TIMER T;
#endif

	int MaxCount;
	int MsgLen;
	char sDate[MAXLOGDATE];
	char sMsg[MAXLOGMSG];
	char sModuleName[MAXLOGNAME];
	char sLogName[MAXLOGNAME];

	void* SW;
	C_Lib CL;
	SPG_Console C;

} LOGFILE;

#define LOGWITHDATE 1
#define LOGWITHTHREADID 2
#define LOGWITHTIMER 4
#define LOGALWAYSCLOSE 8
#define LOGWINDOW 16
#define LOGCHECKCLBK 32

void SPG_CONV LogfileInit(LOGFILE& logfile, const char* sModuleName=0, int Flag=LOGWITHDATE|LOGWITHTHREADID, int MaxFileSize=1024*1024, int MaxFileCount=2, char* Path=0);
void SPG_CONV LogfileClose(LOGFILE& logfile);
void SPG_CONV Logfile(LOGFILE& logfile, const char* Msg);
void __cdecl LogfileF(LOGFILE &log, const char *format, ...);
void SPG_CONV LogfileEmptyLine(LOGFILE& logfile);

class fdwstring
{
	std::wstring ws;
	const wchar_t* data;
	//	LPCWSTR lpcstr;

public:
	fdwstring(const char* utf8)
	{
		// UTF-8 to wstring
		std::wstring_convert<std::codecvt_utf8<wchar_t>> wconv;
		ws = wconv.from_bytes(utf8);
		//// wstring to string
		//locale& loc = locale(".1252");
		//vector<char> buf(wstr.size());
		//use_facet<ctype<wchar_t>>(loc).narrow(wstr.data(), wstr.data() + wstr.size(), '?', buf.data());
		//return string(buf.data(), buf.size());
	}

	~fdwstring() 
	{
	}

	operator const wchar_t*()
	{
		data = ws.c_str();
		return data;
	}
	//operator const LPCWSTR()
	//{
	//	data = ws.c_str();
	//	lpcstr = data;
	//	return lpcstr;
	//}
};

class fdstring
{
	std::string s;
	const char* data;

public:
	fdstring(const wchar_t* wstr)
	{
		// wstring to string
		std::wstring ws(wstr);
		std::locale& loc = std::locale(".1252");
		std::vector<char> buf(ws.size());
		std::use_facet<std::ctype<wchar_t>>(loc).narrow(ws.data(), ws.data() + ws.size(), '?', buf.data());
		s = std::string(buf.data(), buf.size());
	}

	operator const char*()
	{
		data = s.c_str();
		return data;
	}
};
