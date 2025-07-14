
#ifdef SPG_General_USESMTPPOP3

int SPG_CONV SPG_SMTP_SendMail(char* remote_host,char* sender,char* receiver, char* subject,char* message,char* reply_to=0, char* from=0, char* to=0);
int SPG_CONV SPG_POP3_ReadMail(char* remote_host,char* user,char* pass,int& messagecount,char* &message,int& messagelen,bool remove=0);

#endif

