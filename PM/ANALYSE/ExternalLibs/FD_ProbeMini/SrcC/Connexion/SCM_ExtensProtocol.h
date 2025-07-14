
static int SPG_CONV scxOpenProtocol(SCX_STATE& State, int scxOpenFlag)
{
	SPG_MemFastCheck();
	//CHECK(State.Address.W.Address==0,"scxOpenProtocol " sci_NAME ": Null write connexion address",return 0);
	//CHECK(State.Address.R.Address==0,"scxOpenProtocol " sci_NAME ": Null read connexion address",return 0);
	//CHECK(State.Address.W.CI==0,"scxOpenProtocol " sci_NAME ": Null write connexion interface",return 0);
	//CHECK(State.Address.R.CI==0,"scxOpenProtocol " sci_NAME ": Null read connexion interface",return 0);
	if(scxAddressCompare(State.Address.R.Address,State.Address.W.Address)==0)
	{//lecture et écriture sont sur le même support physique
		if(((scxOpenFlag&SCXOPENPRESERVEADDRESS)==0)&&(State.Address.R.Address!=State.Address.W.Address)) scxDestroyAddress(State.Address.R.Address); //on free manuellement l'adresse R
		State.R.C=State.W.C=scxOpen(State.Address.W.CI,State.Address.W.Address, scxOpenFlag); //open free l'address W
		CHECK(State.W.C==0,"scxOpenProtocol R=W=0  " sci_NAME,return 0);
	}
	else
	{
		if(State.Address.W.Address)
		{
			State.W.C=scxOpen(State.Address.W.CI,State.Address.W.Address, scxOpenFlag); //open free l'address W
		}
		if(State.Address.R.Address)
		{
			State.R.C=scxOpen(State.Address.R.CI,State.Address.R.Address, scxOpenFlag); //open free l'address R
		}
	}
	SPG_MemFastCheck();

	CHECK((State.W.C==0)&&(State.R.C==0),"scxOpenProtocol " sci_NAME,return 0);

	DbgCHECK(State.R.C==0,"scxOpenProtocol " sci_NAME); //si seulement l'un des deux est OK on considère qu'on peut continuer pour faciliter la mise au point
	DbgCHECK(State.W.C==0,"scxOpenProtocol " sci_NAME);

	return -1;
}

/*
static SCX_EXTWRITETHROUGH(scxProtocolWriteThrough)
{
	scxCHECK(C, "scxReadThrough");
	SCX_STATE& State=*C->State;
	CHECK(State.W.C==0,"scxWriteThrough",return 0);
	return scxWrite(Data,DataLen,State.W.C);
}

static SCX_EXTREADTHROUGH(scxProtocolReadThrough)
{
	scxCHECK(C, "scxReadThrough");
	SCX_STATE& State=*C->State;
	CHECK(State.R.C==0,"scxReadThrough",return 0);
	return scxRead(Data,DataLen,State.R.C);
}
*/

static void SPG_CONV scxProtocolInheritUserExtension(SCX_CONNEXION* C, SCX_CONNEXION* W, SCX_CONNEXION* R)
{
	SPG_MemFastCheck();
	{for(int i=0;i<SCX_MAXUSEREXTENSION;i++)
	{
		if(C->UserFctPtr[i]==0)
		{
			CHECKTWO(R && R->UserFctPtr[i] && W && W->UserFctPtr[i] && (W!=R),"scxProtocolInheritUserExtension",C->CI?C->CI->Name:C->Address->H.Name,continue);
			if(W && W->UserFctPtr[i])
			{
				C->UserFctPtr[i]=W->UserFctPtr[i]; C->UserFctData[i]=W;
			}
			else if(R)
			{
				C->UserFctPtr[i]=R->UserFctPtr[i]; C->UserFctData[i]=R;
			}
		}
	}}
	if(W && (C->UserFctPtr[sci_EXT_WRITETHROUGH]==0) )
	{//l'extension writethrough est créée automatiquement sur le write de la connexion W si elle même ne possedais pas cette extension
		C->UserFctPtr[sci_EXT_WRITETHROUGH]=(SCX_USEREXTENSION)W->CI->scxWrite; C->UserFctData[sci_EXT_WRITETHROUGH]=W;
	}
	if(R && (C->UserFctPtr[sci_EXT_READTHROUGH]==0) )
	{
		C->UserFctPtr[sci_EXT_READTHROUGH]=(SCX_USEREXTENSION)R->CI->scxWrite; C->UserFctData[sci_EXT_READTHROUGH]=R;
	}
	SPG_MemFastCheck();
	return;
}

static void SPG_CONV scxCloseProtocol(SCX_STATE& State)
{
	//desallocation CONNEXIONS
	if(State.W.C==State.R.C)
	{
		if(State.R.C) scxClose(State.R.C);
		State.W.C=State.R.C=0;
	}
	else
	{
		if(State.R.C) scxClose(State.R.C);
		if(State.W.C) scxClose(State.W.C);
		State.W.C=State.R.C=0;
	}

/*
	//desallocation ADDRESS de STATE
	if(State.Address.R.Address==State.Address.W.Address)
	{
		if(State.Address.R.Address) scxDestroyAddress(State.Address.R.Address);
	}
	else
	{
		if(State.Address.R.Address) scxDestroyAddress(State.Address.R.Address);
		if(State.Address.W.Address) scxDestroyAddress(State.Address.W.Address);
	}
*/
	return;
}
