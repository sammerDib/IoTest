
#include "SPG_General.h"

#ifdef SPG_General_USESMTPPOP3

#include "SPG_Includes.h"

#include <stdlib.h>
#include <stdio.h>
#include <winsock2.h>

//SMTP RFC 821
//POP3 RFC 1939
//IMAP4 RFC 1730-2060

#define SMTP_PORT 25
#define POP3_PORT 110

#define MAXBUF 1024// Maximum buffer size

#define CLEANUP(s) shutdown(s,SD_BOTH);closesocket(s);WSACleanup();return 0
#define CHECK_SEND_AND_RECEIVE(s,req,buf) SPG_ArrayStackCheck(req);SPG_ArrayStackCheck(buf);memset(buf,0,MAXBUF);CHECKTWO((send(s,req,strlen(req),0)==SOCKET_ERROR)||(recv(s,buf,MAXBUF,0)==SOCKET_ERROR),"SPG_SMTP_SendMail",req,CLEANUP(s))
#define CHECK_SEND(s,req) CHECKTWO((send(s,req,strlen(req),0)==SOCKET_ERROR),"SPG_SMTP_SendMail",req,CLEANUP(s))
#define CHECK_RECEIVE(s,buf) SPG_ArrayStackCheck(buf);memset(buf,0,MAXBUF);CHECK((recv(s,buf,MAXBUF,0)==SOCKET_ERROR),"SPG_SMTP_SendMail",CLEANUP(s))
#define QUITCLEANUP(s) strcpy(req,"QUIT\r\n");CHECK_SEND_AND_RECEIVE(s,req,buf);CLEANUP(s)

int SPG_CONV SPG_SMTP_SendMail(char* remote_host,char* sender,char* receiver, char* subject,char* message,char* reply_to, char* from, char* to)
{
	
    WSADATA wsaData;
	CHECK(WSAStartup(0x202,&wsaData),"SPG_SMTP_SendMail",return 0);
	
	SOCKET s;
	SPG_ArrayStackAllocZ(char,buf,MAXBUF);
	SPG_ArrayStackAllocZ(char,req,MAXBUF);

	//
	// Creates and opens a socket with the SMTP Mail Server
	//
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
		addrServer.sin_port = htons(SMTP_PORT);
		LPHOSTENT      lpHostEntry;
		CHECKTWO((lpHostEntry=gethostbyname(remote_host))==0,"SPG_SMTP_SendMail:\nServeur de mail non trouvé",remote_host,closesocket(s);WSACleanup();return 0;);
		addrServer.sin_addr = *((LPIN_ADDR)*lpHostEntry->h_addr_list);
			
		CHECK(connect(s,(LPSOCKADDR)&addrServer,sizeof(addrServer))==SOCKET_ERROR,"SPG_SMTP_SendMail\nConnect failed",closesocket(s);WSACleanup();return 0);
	}
	//
	// Tells the SMTP Server that a mail operation is about to begin
	//
	CHECK_RECEIVE(s,buf);
	CHECKTWO(strncmp(buf,"220",3)!=0,"SPG_SMTP_SendMail:\nError in mail response",buf,CLEANUP(s));
	sprintf(req,"HELO %s\r\n",remote_host);
	CHECK_SEND_AND_RECEIVE(s,req,buf);
	CHECKTWO(strncmp(buf,"250",3)!=0,"SPG_SMTP_SendMail:\nError in mail response",buf,CLEANUP(s));
	sprintf(req,"MAIL FROM:<%s>\r\n",sender);
	CHECK_SEND_AND_RECEIVE(s,req,buf);
	CHECKTWO(strncmp(buf,"250",3)!=0,"SPG_SMTP_SendMail:\nError in mail response",buf,strcpy(req,"QUIT\r\n");CHECK_SEND_AND_RECEIVE(s,req,buf);CLEANUP(s));
	/*
	sprintf(req,"VRFY <%s>\r\n",receiver);
	CHECK_SEND_AND_RECEIVE(s,req,buf);
	*/
	CHECKTWO((strncmp(buf,"250",3)!=0)&&(strncmp(buf,"251",3)!=0)&&(strncmp(buf,"252",3)!=0)&&(strncmp(buf,"253",3)!=0),"SPG_SMTP_SendMail:\nError in mail response",buf,strcpy(req,"QUIT\r\n");CHECK_SEND_AND_RECEIVE(s,req,buf);CLEANUP(s));
	sprintf(req,"RCPT TO:<%s>\r\n",receiver);
	CHECK_SEND_AND_RECEIVE(s,req,buf);
	CHECKTWO((strncmp(buf,"250",3)!=0)&&(strncmp(buf,"251",3)!=0)&&(strncmp(buf,"252",3)!=0)&&(strncmp(buf,"253",3)!=0),"SPG_SMTP_SendMail:\nError in mail response",buf,strcpy(req,"QUIT\r\n");CHECK_SEND_AND_RECEIVE(s,req,buf);CLEANUP(s));
	strcpy(req,"DATA\r\n");
	CHECK_SEND_AND_RECEIVE(s,req,buf);
	CHECKTWO(strncmp(buf,"354",3)!=0,"SPG_SMTP_SendMail:\nError in mail response",buf,strcpy(req,"QUIT\r\n");CHECK_SEND_AND_RECEIVE(s,req,buf);CLEANUP(s));
	
	if(reply_to==0) reply_to=sender;
	if(from==0) from=sender;
	if(to==0) to=receiver;
	sprintf(req,"Reply-To: %s\r\nFrom: %s\r\nTo: %s\r\nSubject: %s\r\n",reply_to,from,to,subject);

	CHECK_SEND(s,req);
	//
	// Sends the actual text meesage to the SMTP Server
	//
	CHECK_SEND(s,message);

	strcpy(req,"\r\n.\r\n");
	CHECK_SEND_AND_RECEIVE(s,req,buf);
	CHECKTWO(strncmp(buf,"250",3)!=0,"SPG_SMTP_SendMail:\nError in mail response",buf,;);
	//
	// Closes the SMTP communication and tells the SMTP-Server to process mail
	//
	strcpy(req,"QUIT\r\n");
	CHECK_SEND_AND_RECEIVE(s,req,buf);
	CHECKTWO(strncmp(buf,"221",3)!=0,"SPG_SMTP_SendMail:\nError in mail response",buf,;);
	shutdown(s,SD_BOTH);
	closesocket(s);
	WSACleanup();
	return -1;
}

void SPG_CONV SPG_MailDecode(char* dest, int maxlendest, char* src, int maxlensrc, int& destlen, int& endofmsg)
{
	char* EndSeq="\r\n.\r\n";
	int EndSeqLen=5;
	int EndSeqPos=0;
	destlen=0;
	endofmsg=1;
	for(int i=0;i<maxlensrc;i++)
	{
		if(src[i]==0) 
			return;
		else
		if(src[i]==EndSeq[EndSeqPos])
		{
			EndSeqPos++;
			if(EndSeqPos==EndSeqLen) return;
		}
		else
		{
			if(EndSeqPos)
			{
				if(EndSeqPos==2)
					dest[destlen++]='\n';
				else
				{
					for(int skip=0;skip<V_Min(EndSeqPos,2);skip++)
					{
						dest[destlen++]=EndSeq[skip];
						CHECK(destlen>=maxlendest,"SPG_MailDecode:\nBuffer overflow",return);
					}
				}
				if(src[i]==EndSeq[0])
				{
					EndSeqPos=1;
				}
				else
				{
					EndSeqPos=0;
					dest[destlen++]=src[i];
				}
			}
			else
			{
				dest[destlen++]=src[i];
			}
			CHECK(destlen>=maxlendest,"SPG_MailDecode:\nBuffer overflow",return);
		}
	}
	endofmsg=0;
	return;
}

int SPG_CONV SPG_POP3_ReadMail(char* remote_host,char* user,char* pass,int& messagecount,char* &message,int& messagelen,bool remove)
{
	messagecount=0;
	message=0;
	messagelen=0;
	
    WSADATA wsaData;
	CHECK(WSAStartup(0x202,&wsaData),"SPG_POP3_ReadMail",return 0);
	
	SOCKET s;
	SPG_ArrayStackAllocZ(char,buf,MAXBUF);
	SPG_ArrayStackAllocZ(char,req,MAXBUF);

	//
	// Creates and opens a socket with the POP3 Mail Server
	//
	{
		PROTOENT *proto;
		proto = getprotobyname("tcp");
		CHECK((s=socket(PF_INET,SOCK_STREAM,proto->p_proto))==INVALID_SOCKET,"SPG_POP3_ReadMail",WSACleanup();return 0);
	}
	{
		SOCKADDR_IN addr;
		addr.sin_family = PF_INET;
		addr.sin_port = 0;
		addr.sin_addr.s_addr = htonl(INADDR_ANY);
		CHECK(bind(s,(LPSOCKADDR)&addr,sizeof(addr))==SOCKET_ERROR,"SPG_POP3_ReadMail",closesocket(s);WSACleanup();return 0);
	}
	{	
		SOCKADDR_IN addrServer;
		addrServer.sin_family = PF_INET;
		addrServer.sin_port = htons(POP3_PORT);
		LPHOSTENT      lpHostEntry;
		CHECKTWO((lpHostEntry=gethostbyname(remote_host))==0,"SPG_POP3_ReadMail:\nServeur de mail non trouvé",remote_host,closesocket(s);WSACleanup();return 0;);
		addrServer.sin_addr = *((LPIN_ADDR)*lpHostEntry->h_addr_list);
			
		CHECK(connect(s,(LPSOCKADDR)&addrServer,sizeof(addrServer))==SOCKET_ERROR,"SPG_POP3_ReadMail\nConnect failed",closesocket(s);WSACleanup();return 0);
	}
	//
	// Tells the POP3 Server that a mail operation is about to begin
	//
	CHECK_RECEIVE(s,buf);
	CHECKTWO(strncmp(buf,"+OK",3)!=0,"SPG_POP3_ReadMail:\nError in mail response",buf,CLEANUP(s));
	sprintf(req,"USER %s\r\n",user);
	CHECK_SEND_AND_RECEIVE(s,req,buf);
	CHECKTWO(strncmp(buf,"+OK",3)!=0,"SPG_POP3_ReadMail:\nError in server response",buf,CLEANUP(s));
	sprintf(req,"PASS %s\r\n",pass);
	CHECK_SEND_AND_RECEIVE(s,req,buf);
	CHECKTWO(strncmp(buf,"+OK",3)!=0,"SPG_POP3_ReadMail:\nError in server response",buf,CLEANUP(s));
	strcpy(req,"STAT\r\n");
	CHECK_SEND_AND_RECEIVE(s,req,buf);
	CHECKTWO(strncmp(buf,"+OK",3)!=0,"SPG_POP3_ReadMail:\nError in server response",buf,QUITCLEANUP(s));
	messagecount=atoi(buf+4);
	if(messagecount==0)
	{
		QUITCLEANUP(s);
	}
	strcpy(req,"RETR 1\r\n");
	CHECK_SEND_AND_RECEIVE(s,req,buf);
	CHECKTWO(strncmp(buf,"+OK",3)!=0,"SPG_POP3_ReadMail:\nError in server response",buf,QUITCLEANUP(s));
	int allocmessagelen=atoi(buf+4)+2;
	if(allocmessagelen)
	{
		message=SPG_TypeAlloc(allocmessagelen,char,"POP3_MSG");//pour le zero terminal

		char* startchar=SPG_StrFind(buf,"\r\n");
		if(startchar)
		{
			int endofmsg;
			int partlen;
			SPG_MailDecode(message,allocmessagelen,startchar+2,MAXBUF-2-(startchar-buf),partlen,endofmsg);
			messagelen+=partlen;
			while(endofmsg==0)
			{
				CHECK_RECEIVE(s,buf);
				SPG_MailDecode(message+messagelen,allocmessagelen-messagelen,buf,MAXBUF,partlen,endofmsg);
				messagelen+=partlen;
			}
		}
	}
	//
	// Closes the POP3 communication
	//
	
	if(messagelen<allocmessagelen)
	{
		message[messagelen]='\0';
	}
	
	strcpy(req,"QUIT\r\n");
	CHECK_SEND_AND_RECEIVE(s,req,buf);
	CHECKTWO(strncmp(buf,"+OK",3)!=0,"SPG_POP3_ReadMail:\nError in mail response",buf,;);
	shutdown(s,SD_BOTH);
	closesocket(s);
	WSACleanup();
	return -1;
}

#endif

