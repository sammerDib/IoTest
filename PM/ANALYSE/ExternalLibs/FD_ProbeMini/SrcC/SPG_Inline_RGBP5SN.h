
//DO NOT COMPILE: INCLUDE FILE ONLY


#ifndef NPACK

#error NPACK non defini

#endif

	{
		int Len=0;
		int Count=0;
		BYTE CodingRP5;
		//FOR_ALL_ELT_IN_SQUARE(MD0,Size,x,y,xc,yc,PLine,PCol)
		GET_LINE_POINTER(MD0,PLine,x,y);
		FOR_ALL_ELT_IN_SQUARE_Y(Size,yc,PLine,PCol)
MELINK_CheckForMsgLen(Len+Size,MaxMsgLen,AllowDecLen)
		FOR_ALL_ELT_IN_SQUARE_X(Size,xc)
		BYTE RP5;
		RGB_PACK_5_SN(ELT_PTR(MD0,PCol),ELT_PTR(MD1,PCol),NPACK,RP5);
		if(Count==0)
		{
			Count++;
			CodingRP5=RP5;
		}
		else if(RP5==CodingRP5)
		{
			if((++Count)==2)
			{
				CodingRP5|=((Count-1)<<7);
		//MELINK_CheckForMsgLen(Len+1,MaxMsgLen,AllowDecLen)
				Msg->M[Len++]=CodingRP5;
				Count=0;
			}
		}
		else
		{
			CodingRP5|=((Count-1)<<7);
		//MELINK_CheckForMsgLen(Len+1,MaxMsgLen,AllowDecLen)
			Msg->M[Len++]=CodingRP5;
			Count=1;//ATTENTION Count=1
			CodingRP5=RP5;
		}
		NEXT_ELT_IN_SQUARE(MD0,Size,PLine,PCol)
		if(Count)
		{
			CodingRP5|=((Count-1)<<7);
		//MELINK_CheckForMsgLen(Len+1,MaxMsgLen,AllowDecLen)
			Msg->M[Len++]=CodingRP5;
			Count=0;
		}
		SPG_MemFastCheck();
		return Len+sizeof(MELINK_MSG);
	}
#undef NPACK

