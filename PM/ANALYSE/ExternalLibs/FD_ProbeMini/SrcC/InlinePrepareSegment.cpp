
//DO NOT COMPILE: INCLUDE FILE ONLY
#ifdef SPG_General_USEGraphicsRenderPoly


#ifdef DebugGraphicsTimer
	S_StartTimer(Global.T_InlinePrepareSegment);
#endif

	int NYMin=0;
	int NYMax=0;
	
	if (P[0].y>P[1].y) 
		NYMin=1;
	else if (P[0].y<P[1].y) 
		NYMax=1;
	
#if (NPoints>=3)
	if (P[NYMin].y>P[2].y) NYMin=2;
	if (P[NYMax].y<P[2].y) NYMax=2;
#endif
	
#if (NPoints>=4)
	if (P[NYMin].y>P[3].y) NYMin=3;
	if (P[NYMax].y<P[3].y) NYMax=3;
#endif
	
#if (NPoints>=5)
	if (P[NYMin].y>P[4].y) NYMin=4;
	if (P[NYMax].y<P[4].y) NYMax=4;
#endif
	
	if (V_Max(P[NYMin].y,0)>=V_Min(P[NYMax].y,E.SizeY)) 
		{
#ifdef DebugGraphicsTimer
			S_StopTimer(Global.T_InlinePrepareSegment);
#endif
#ifdef DebugGraphicsTimer
			S_StopTimer(Global.T_GraphicsRender);
#endif
			return;
		}
	
	int sens;
	{
		int LP=PrevPoint(0);
		int LLP=PrevPoint(LP);
		int D=0;
#pragma ivdep
		for(int NP=0;NP<NPoints;NP++)
		{
			D+=(P[NP].y-P[LP].y)*(P[LLP].x-P[LP].x)-(P[NP].x-P[LP].x)*(P[LLP].y-P[LP].y);
			LLP=LP;
			LP=NP;
		}
		sens=V_Signe(D);
		if (sens==0) sens=1;
		/*
		{
			G_DrawRect(E,
				V_Min(V_Min(P[0].x,P[1].x),V_Min(P[2].x,P[3].x)),
				V_Min(V_Min(P[0].y,P[1].y),V_Min(P[2].y,P[3].y)),
				V_Max(V_Max(P[0].x,P[1].x),V_Min(P[2].x,P[3].x)),
				V_Max(V_Max(P[0].y,P[1].y),V_Min(P[2].y,P[3].y)),
				0);
			return;
		}
		*/
		/*
		int NSPrev=PrevPoint(NYMin);
		int NSNext=NextPoint(NYMin);
		int D=((P[NSPrev].x-P[NYMin].x)*(P[NSNext].y-P[NYMin].y))-((P[NSPrev].y-P[NYMin].y)*(P[NSNext].x-P[NYMin].x));
		sens=V_Signe(D);
		if (sens==0)
		{
		NSPrev=PrevPoint(NYMax);
		NSNext=NextPoint(NYMax);
		D=((P[NSPrev].x-P[NYMax].x)*(P[NSNext].y-P[NYMax].y))-((P[NSPrev].y-P[NYMax].y)*(P[NSNext].x-P[NYMax].x));
		sens=V_Signe(D);
		if (sens==0)
		{
		NSPrev=NYMax;
		int T=NSNext;
		NSNext=NextPoint(T);
		D=((P[NSPrev].x-P[T].x)*(P[NSNext].y-P[T].y))-((P[NSPrev].y-P[T].y)*(P[NSNext].x-P[T].x));
		sens=V_Signe(D);
		if (sens==0)
		{
		G_DrawRect(E,
		V_Min(V_Min(P[0].x,P[1].x),V_Min(P[2].x,P[3].x)),
		V_Min(V_Min(P[0].y,P[1].y),V_Min(P[2].y,P[3].y)),
		V_Max(V_Max(P[0].x,P[1].x),V_Min(P[2].x,P[3].x)),
		V_Max(V_Max(P[0].y,P[1].y),V_Min(P[2].y,P[3].y)),
		0);
		return;
		}
		
		  //sens=1;//return;
		  if (sens==0)
		  {
		  NSPrev=T;
		  T=NSNext;
		  NSNext=NextPoint(T);
		  D=((P[NSPrev].x-P[T].x)*(P[NSNext].y-P[T].y))-((P[NSPrev].y-P[T].y)*(P[NSNext].x-P[T].x));
		  sens=V_Signe(D);
		  if (sens==0)
		  {
		  NSPrev=T;
		  T=NSNext;
		  NSNext=NextPoint(T);
		  D=((P[NSPrev].x-P[T].x)*(P[NSNext].y-P[T].y))-((P[NSPrev].y-P[T].y)*(P[NSNext].x-P[T].x));
		  sens=V_Signe(D);
		  if (sens==0)
		  {
		  sens=1;
		  }
		  }
		  }
		}
	}
		*/
	
	}
	
	int PntG[NPoints];//liste ds points gauches
	int CurWPointG=0;//position courante dans la liste
	{
		int CurGPoint;//point gauche courant
		if (P[NYMin].y==P[ToPoint(NYMin,-sens)].y)
		{
			if (P[NYMin].x>P[ToPoint(NYMin,-sens)].x) 
				CurGPoint=ToPoint(NYMin,-sens);//point gauche courant
			else
				CurGPoint=NYMin;//point gauche courant
		}
		else
		{
			CurGPoint=NYMin;//point gauche courant
		}
		PntG[0]=CurGPoint;//le premier point gauche
		
		CurGPoint=ToPoint(CurGPoint,sens);//prochain point gauche
		do
		{
			while(P[CurGPoint].y==P[PntG[CurWPointG]].y)
			{
				if ((P[CurGPoint].x)<(P[PntG[CurWPointG]].x)) PntG[CurWPointG]=CurGPoint;
				if ((CurGPoint=ToPoint(CurGPoint,sens))==(NYMin)) break;
			}
			CurWPointG++;
			PntG[CurWPointG]=CurGPoint;
		} while (P[CurGPoint].y<P[NYMax].y);
		while(P[CurGPoint].y==P[PntG[CurWPointG]].y)
		{
			if ((P[CurGPoint].x)<(P[PntG[CurWPointG]].x)) PntG[CurWPointG]=CurGPoint;
			if ((CurGPoint=ToPoint(CurGPoint,sens))==(NYMin)) break;
		}
	}
	
	int PntD[NPoints];//liste ds points gauches
	int CurWPointD=0;//position courante dans la liste
	sens=-sens;
	
	{
		int CurDPoint;//point droit courant
		if (P[NYMin].y==P[ToPoint(NYMin,-sens)].y)
		{
			if (P[NYMin].x<P[ToPoint(NYMin,-sens)].x) 
				CurDPoint=ToPoint(NYMin,-sens);//point droit courant
			else
				CurDPoint=NYMin;//point droit courant
		}
		else
		{
			CurDPoint=NYMin;//point droit courant
		}
		PntD[0]=CurDPoint;//le premier point droit
		
		CurDPoint=ToPoint(CurDPoint,sens);//prochain point droit
		do
		{
			while(P[CurDPoint].y==P[PntD[CurWPointD]].y)
			{
				if ((P[CurDPoint].x)>(P[PntD[CurWPointD]].x)) PntD[CurWPointD]=CurDPoint;
				if ((CurDPoint=ToPoint(CurDPoint,sens))==(NYMin)) break;
			}
			CurWPointD++;
			PntD[CurWPointD]=CurDPoint;
		} while (P[CurDPoint].y<P[NYMax].y);
		while(P[CurDPoint].y==P[PntD[CurWPointD]].y)
		{
			if ((P[CurDPoint].x)>(P[PntD[CurWPointD]].x)) PntD[CurWPointD]=CurDPoint;
			if ((CurDPoint=ToPoint(CurDPoint,sens))==(NYMin)) break;
		}
	}
#undef NextPoint
#undef PrevPoint
#undef ToPoint
	
	bool FastCopy;
	{
		int iout;
		for(iout=0;iout<NPoints;iout++)
		{
			if(!(G_InEcr(P[iout],E))) break;
		}
		FastCopy=(iout==NPoints);
		//if (iout==NPoints) Coul=Coul^0xffffff;
	}
#undef NPoints

#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_InlinePrepareSegment);
#endif

#else
#error Configuration error
#endif

