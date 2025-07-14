
#include "SPG_General.h"

#ifdef SPG_General_USEFTP

#include "SPG_Includes.h"

#include <stdio.h>
#include <stdlib.h>
#include <winsock2.h>

//RFC 959

#define FTP_PORT 21

#define MAXBUF 512

#define CLEANUP(s) shutdown(s,SD_BOTH);closesocket(s);WSACleanup();return 0
#define CHECK_SEND_AND_RECEIVE(s,req,buf) SPG_ArrayStackCheck(req);SPG_ArrayStackCheck(buf);memset(buf,0,MAXBUF);CHECKTWO((send(s,req,strlen(req),0)==SOCKET_ERROR)||(recv(s,buf,MAXBUF,0)==SOCKET_ERROR),"SPG_FTP_SendFile",req,CLEANUP(s))
#define CHECK_SEND_AND_RECEIVE_PASV(s,pasvSocket,req,buf) SPG_ArrayStackCheck(req);SPG_ArrayStackCheck(buf);memset(buf,0,MAXBUF);CHECKTWO((send(s,req,strlen(req),0)==SOCKET_ERROR)||(recv(s,buf,MAXBUF,0)==SOCKET_ERROR),"SPG_FTP_SendFile",req,closesocket(pasvSocket);CLEANUP(s))
#define CHECK_SEND(s,req) CHECKTWO((send(s,req,strlen(req),0)==SOCKET_ERROR),"SPG_SMTP_SendMail",req,CLEANUP(s))
#define CHECK_RECEIVE(s,buf) SPG_ArrayStackCheck(buf);memset(buf,0,MAXBUF);CHECK((recv(s,buf,MAXBUF,0)==SOCKET_ERROR),"SPG_SMTP_SendMail",CLEANUP(s))
#define QUITCLEANUP(s) strcpy(req,"QUIT\r\n");CHECK_SEND_AND_RECEIVE(s,req,buf);CLEANUP(s)

int SPG_CONV SPG_FTP_ForceDir(SOCKET& s, char* single_directory)
{
	SPG_ArrayStackAllocZ(char,buf,MAXBUF);
	SPG_ArrayStackAllocZ(char,req,MAXBUF);

	sprintf(req, "CWD %s\r\n", single_directory);
	CHECK_SEND_AND_RECEIVE(s,req,buf);
	if(strncmp(buf,"250",3)!=0)
	{
		sprintf(req, "MKD %s\r\n", single_directory);
		CHECK_SEND_AND_RECEIVE(s,req,buf);
		CHECKTWO(strncmp(buf,"257",3)!=0,"SPG_FTP_ForceDir:\nError in response",buf,return 0;);
		sprintf(req, "CWD %s\r\n", single_directory);
		CHECK_SEND_AND_RECEIVE(s,req,buf);
		CHECKTWO(strncmp(buf,"250",3)!=0,"SPG_FTP_ForceDir:\nError in response",buf,return 0;);
	}
	return -1;
}

int SPG_CONV SPG_FTP_SendFile(char* remote_host, char* user, char* pass, char* file_path, char* file_name, BYTE* data, int Len)
{
    WSADATA wsaData;
	CHECK(WSAStartup(0x202,&wsaData),"SPG_FTP_SendFile",return 0);

	//struct hostent *p_host = NULL;					// pointer to a host structure for dns-resolving

	SOCKET s;
	SPG_ArrayStackAllocZ(char,buf,MAXBUF);
	SPG_ArrayStackAllocZ(char,req,MAXBUF);
	// Creating Socket
	{
		PROTOENT *proto;
		proto = getprotobyname("tcp");
		CHECK((s=socket(PF_INET,SOCK_STREAM,proto->p_proto))==INVALID_SOCKET,"SPG_SMTP_SendMail",WSACleanup();return 0);
	}
	{
		SOCKADDR_IN addr;
		addr.sin_family = PF_INET;
		addr.sin_port = 0;
		addr.sin_addr.s_addr = htonl(INADDR_ANY);
		CHECK(bind(s,(LPSOCKADDR)&addr,sizeof(addr))==SOCKET_ERROR,"SPG_SMTP_SendMail",closesocket(s);WSACleanup();return 0);
	}
	{	
		SOCKADDR_IN addrServer;
		addrServer.sin_family = PF_INET;
		addrServer.sin_port = htons(FTP_PORT);
		LPHOSTENT      lpHostEntry;
		CHECKTWO((lpHostEntry=gethostbyname(remote_host))==0,"SPG_SMTP_SendMail:\nServeur de mail non trouvé",remote_host,closesocket(s);WSACleanup();return 0;);
		addrServer.sin_addr = *((LPIN_ADDR)*lpHostEntry->h_addr_list);
			
		CHECK(connect(s,(LPSOCKADDR)&addrServer,sizeof(addrServer))==SOCKET_ERROR,"SPG_SMTP_SendMail\nConnect failed",closesocket(s);WSACleanup();return 0);
	}
	CHECK_RECEIVE(s,buf);
	CHECKTWO(strncmp(buf,"220",3)!=0,"SPG_FTP_SendFile:\nError in server response",buf,CLEANUP(s));
	sprintf(req,"USER %s\r\n",user);
	CHECK_SEND_AND_RECEIVE(s,req,buf);
	CHECKTWO(strncmp(buf,"331",3)!=0,"SPG_FTP_SendFile:\nError in server response",buf,CLEANUP(s));
	sprintf(req,"PASS %s\r\n",pass);
	CHECK_SEND_AND_RECEIVE(s,req,buf);
	CHECKTWO(strncmp(buf,"230",3)!=0,"SPG_FTP_SendFile:\nError in server response",buf,CLEANUP(s));
//strcpy(req,"QUIT\r\n");CHECK_SEND_AND_RECEIVE(s,req,buf);
	if(file_path&&file_path[0])
	{
		int istart=0;
		for(int istop=0;istop<strlen(file_path);istop++)
		{
			char c=file_path[istop];
			if(c=='\\'||c=='/')
			{
				if(istart<istop)
				{
					SPG_ArrayStackAllocZ(char,single_directory,MAXBUF);
					strncpy(single_directory,file_path+istart,istop-istart);
					SPG_ArrayStackCheck(single_directory);
					if(SPG_FTP_ForceDir(s,single_directory)==0)
					{
						QUITCLEANUP(s);
					}
					istart=istop+1;
				}
			}
		}
		if(istart<istop)
		{
			if(SPG_FTP_ForceDir(s,file_path+istart)==0)
			{
				QUITCLEANUP(s);
			}
		}
	}

	strcpy(req,"PASV\r\n");
	CHECK_SEND_AND_RECEIVE(s,req,buf);
	CHECKTWO(strncmp(buf,"227",3)!=0,"SPG_FTP_SendFile:\nError in server response",buf,QUITCLEANUP(s));
	// status code 227: Entering passive mode.
	// this needs to be parsed for the ip/port data channel data
	// lets make a seperate scope for this task
	SOCKADDR_IN server_pasv;
	{
		server_pasv.sin_family = AF_INET;
		BYTE Addr[6];
		int istart=0;
		int istop=0;
		int AddrC=0;
		for(istop=0;(buf[istop]!=0)&&(buf[istop]!='\r')&&(buf[istop]!='\n')&&(istop<MAXBUF);istop++)
		{
			if(buf[istop]=='(')
			{
				istart=istop+1;
			}
			if((buf[istop]==',')||(buf[istop]==')'))
			{
				buf[istop]=0;
				Addr[AddrC++]=atoi(buf+istart);
				istart=istop+1;
			}
		}
		memcpy(&server_pasv.sin_addr.S_un.S_addr,Addr,4);
		server_pasv.sin_port=*(WORD*)(Addr+4);
	}
	// create pasvSocket
	SOCKET pasvSocket;
	CHECK((pasvSocket = socket(AF_INET,SOCK_STREAM,IPPROTO_TCP))==INVALID_SOCKET,"SPG_SMTP_SendMail",CLEANUP(s));

	// sync the information with the PASV socket and open the PASV data connection
	CHECK(connect(pasvSocket,(LPSOCKADDR)&server_pasv,sizeof(server_pasv))==SOCKET_ERROR,"SPG_SMTP_SendMail\nConnect failed",closesocket(pasvSocket);QUITCLEANUP(s));

	// this is put, not get... so we dont need to check if data is send through this
	// instead we need to announce on the ftpSocket that we want to store a file 
	// all commands on ftpSocket will only work after you established the pasvSocket
	// otherwise your commands will be buffered till sunrise. but we took care of it already.

	// remember? we already CWD'ed to the directory, so we only need the filename
	
	sprintf(req,"STOR %s\r\n",file_name);
	CHECK_SEND_AND_RECEIVE_PASV(s,pasvSocket,req,buf);
	CHECKTWO(strncmp(buf,"150",3)!=0,"SPG_FTP_SendFile:\nError in server response",buf,closesocket(pasvSocket);QUITCLEANUP(s));

	int Start=0;
	while(Len>Start+MAXBUF)
	{
		CHECK(send(pasvSocket,(const char*)data+Start,MAXBUF,0)==SOCKET_ERROR,"SPG_FTP_SendFile",Start=Len;break);
		Start+=MAXBUF;
	}
	if(Len>Start) CHECK(send(pasvSocket,(const char*)data+Start,Len-Start,0)==SOCKET_ERROR,"SPG_FTP_SendFile",;);
	
	
	// be sure to close the pasv port! otherwise the transfer will not be finished
	closesocket(pasvSocket);

	CHECK_RECEIVE(s,buf);
	CHECKTWO(strncmp(buf,"226",3)!=0,"SPG_FTP_SendFile:\nError in server response",buf,;);

	strcpy(req,"QUIT\r\n");
	CHECK_SEND_AND_RECEIVE(s,req,buf);
	shutdown(s,SD_BOTH);
	closesocket(s);
	WSACleanup();
	return -1;
}

#endif

