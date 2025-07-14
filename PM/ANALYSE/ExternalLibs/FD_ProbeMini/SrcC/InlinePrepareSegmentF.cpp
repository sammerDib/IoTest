
//DO NOT COMPILE: INCLUDE FILE ONLY

	int PntG[NPoints];//liste ds points gauches
	int PntD[NPoints];//liste ds points gauches
	int CurWPointG=1;//position courante dans la liste
	int CurWPointD=1;//position courante dans la liste
	int NYMin,NYMax;

{
	int YMin=P[0].y;
	int HiPG=0;
	int HiPD=0;
	for(int i=1;i<NPoints;i++)
	{
		if (P[i].y<YMin) 
		{
			P[i].y=YMin;
			HiPG=HiPD=i;
		}
		else if (P[i].y==YMin)
		{
			if (P[i].x<P[HiPG].x) HiPG=i;
			if (P[i].x>P[HiPD].x) HiPD=i;
		}
	}

	PntG[0]=HiPG;
	PntD[0]=HiPD;

	int BarriereG=YMin;
	int BarriereD=YMin;
	NYMin=YMin;

	int C_DXG=1;
	int C_DYG=0;
	int C_DXD=-1;
	int C_DYD=0;
	int XG=P[PntG[0]].x;
	int XD=P[PntD[0]].x;

	do
	{
		int NewG=-1;
		int NewD=-1;
	for(int i=0;i<NPoints;i++)
	{
		if (P[i].y>BarriereG)
		{
			if ((P[i].x-XG)*C_DYG<(P[i].y-BarriereG)*C_DXG)
			{
				C_DXG=P[i].x-XG;
				C_DYG=P[i].y-BarriereG;
				NewG=i;
			}
		}

		if (P[i].y>BarriereD)
		{
			if ((P[i].x-XD)*C_DYD>(P[i].y-BarriereD)*C_DXD)
			{
				C_DXD=P[i].x-XD;
				C_DYD=P[i].y-BarriereD;
				NewD=i;
			}
		}

	}
	if ((NewG==-1)&&(NewD==-1)) break;
	if(NewG!=-1)
	{

		PntG[CurWPointG]=NewG;
		CurWPointG++;
		XG=P[NewG].x;
		BarriereG=P[NewG].y;
		NYMax=BarriereG;
	}
	if(NewD!=-1)
	{

		PntD[CurWPointD]=NewD;
		CurWPointD++;
		XD=P[NewD].x;
		BarriereD=P[NewD].y;
		NYMax=BarriereD;
	}
	} while (1);

}
	if ((CurWPointG==1)||(CurWPointD==1)) return;

	bool FastCopy;
	{
		for(int iout=0;iout<NPoints;iout++)
		{
			if(!(G_InEcr(P[iout],E))) break;
		}
		FastCopy=(iout==NPoints);
		//if (iout==NPoints) Coul=Coul^0xffffff;
	}

#undef NPoints

