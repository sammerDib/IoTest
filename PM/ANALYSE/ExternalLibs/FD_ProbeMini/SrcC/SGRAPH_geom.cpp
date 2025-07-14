
#include "SPG_General.h"

#ifdef SPG_General_USESGRAPH

#include "SPG_Includes.h"

#ifdef DebugFloat
#include <float.h>
#endif
#define MaxTex SG_MAX_TEX

int SPG_CONV SG_CreatePlan(SG_FullBloc &Bloc,int SubdivisionsX,int SubdivisionsY,
				  V_VECT v0,V_VECT v1,V_VECT v2,V_VECT v3,
				  DWORD coul,int style,SG_FullTexture& texture,
				  int x0,int y0,int x1,int y1,
				  int x2,int y2,int x3,int y3)
{
	CHECK(SubdivisionsX<=0,"SG_CreatePlan: 0 faces X",return 0);
	CHECK(SubdivisionsY<=0,"SG_CreatePlan: 0 faces Y",return 0);
#ifndef SPG_General_PGLib
	CHECK((x0|y0|x1|y1|x2|y2|x3|y3)&&(V_IsBound(texture.NumAttach,0,MaxTex)==0),"SG_CreatePlan: Mauvaise texture",style&=SG_MASKUNI);
#endif
	SG_CheckVDist(v0,v1,"SG_CreatePlan:",return 0);
	SG_CheckVDist(v1,v2,"SG_CreatePlan:",return 0);
	SG_CheckVDist(v2,v3,"SG_CreatePlan:",return 0);
	SG_CheckVDist(v3,v0,"SG_CreatePlan:",return 0);
	if(texture.NumAttach!=-1)
	{
		CHECK(!V_IsBound(x0,0,SG_TexSizeX(texture)),"SG_CreatePlan: Coordonnees de texture",style=SG_UNI);
		CHECK(!V_IsBound(x1,0,SG_TexSizeX(texture)),"SG_CreatePlan: Coordonnees de texture",style=SG_UNI);
		CHECK(!V_IsBound(x2,0,SG_TexSizeX(texture)),"SG_CreatePlan: Coordonnees de texture",style=SG_UNI);
		CHECK(!V_IsBound(x3,0,SG_TexSizeX(texture)),"SG_CreatePlan: Coordonnees de texture",style=SG_UNI);
		CHECK(!V_IsBound(y0,0,SG_TexSizeY(texture)),"SG_CreatePlan: Coordonnees de texture",style=SG_UNI);
		CHECK(!V_IsBound(y1,0,SG_TexSizeY(texture)),"SG_CreatePlan: Coordonnees de texture",style=SG_UNI);
		CHECK(!V_IsBound(y2,0,SG_TexSizeY(texture)),"SG_CreatePlan: Coordonnees de texture",style=SG_UNI);
		CHECK(!V_IsBound(y3,0,SG_TexSizeY(texture)),"SG_CreatePlan: Coordonnees de texture",style=SG_UNI);
	}
	
	CHECK(SG_CreateBloc(Bloc,(SubdivisionsX+1)*(SubdivisionsY+1),SubdivisionsX*SubdivisionsY,SG_WithPOINTS|SG_WithFACES)==0,"SG_CreatePlan: SG_CreateBloc echoue",return 0);
	
	int u;
	for(u=0; u<=SubdivisionsY; u++)
	{
		int t;
		V_VECT vs,vd;
		V_Operate3(vs,=((SubdivisionsY-u)/(float)(SubdivisionsY))*v0,+(u/(float)(SubdivisionsY))*v3);
		V_Operate3(vd,=((SubdivisionsY-u)/(float)(SubdivisionsY))*v1,+(u/(float)(SubdivisionsY))*v2);
		float xs0=x0+(x3-x0)*u/(float)SubdivisionsY;
		float ys0=y0+(y3-y0)*u/(float)SubdivisionsY;
		float xd0=x1+(x2-x1)*u/(float)SubdivisionsY;
		float yd0=y1+(y2-y1)*u/(float)SubdivisionsY;
		float xs1=x0+(x3-x0)*(u+1)/(float)SubdivisionsY;
		float ys1=y0+(y3-y0)*(u+1)/(float)SubdivisionsY;
		float xd1=x1+(x2-x1)*(u+1)/(float)SubdivisionsY;
		float yd1=y1+(y2-y1)*(u+1)/(float)SubdivisionsY;
		
		for(t=0;t<=SubdivisionsX;t++)
		{
			V_VECT Pos;
			V_Operate3(Pos,=(SubdivisionsX-t)/(float)(SubdivisionsX)*vs,+t/(float)(SubdivisionsX)*vd);
			
			SG_DefPnt(Bloc.DB.MemPoints[t+(1+SubdivisionsX)*u],Pos);

			if ((u!=SubdivisionsY)&&(t!=SubdivisionsX))
			{
				
				int xcoin0=V_FloatToInt(xs0+(xd0-xs0)*t/(float)SubdivisionsX);
				int ycoin0=V_FloatToInt(ys0+(yd0-ys0)*t/(float)SubdivisionsX);
				int xcoin1=V_FloatToInt(xs0+(xd0-xs0)*(t+1)/(float)SubdivisionsX);
				int ycoin1=V_FloatToInt(ys0+(yd0-ys0)*(t+1)/(float)SubdivisionsX);
				int xcoin2=V_FloatToInt(xs1+(xd1-xs1)*(t+1)/(float)SubdivisionsX);
				int ycoin2=V_FloatToInt(ys1+(yd1-ys1)*(t+1)/(float)SubdivisionsX);
				int xcoin3=V_FloatToInt(xs1+(xd1-xs1)*t/(float)SubdivisionsX);
				int ycoin3=V_FloatToInt(ys1+(yd1-ys1)*t/(float)SubdivisionsX);
				
				SG_DefTexFceI_Unsafe(Bloc,t+SubdivisionsX*u,
					t+(1+SubdivisionsX)*u,
					t+1+(1+SubdivisionsX)*u,
					t+1+(1+SubdivisionsX)*(u+1),
					t+(1+SubdivisionsX)*(u+1),
					coul,style,texture,
					xcoin0,ycoin0,xcoin1,ycoin1,
					xcoin2,ycoin2,xcoin3,ycoin3);
			}
		}
	}
	SG_MakeBRef(Bloc);
	SG_CalcNormales(Bloc);
	
	return -1;
}


int SPG_CONV SG_CreateCylinder(SG_FullBloc &Bloc,
					  int SubdivisionsC,int SubdivisionsH,
					  V_VECT Base,V_VECT RX,V_VECT RY,V_VECT RH,
					  DWORD coul,int style,SG_FullTexture& texture,
					  int x0,int y0,int x1,int y1,
					  int x2,int y2,int x3,int y3)
{
	CHECK(SubdivisionsC<=3,"SG_CreateCylinder: moins de 3 faces",return 0);
	CHECK(SubdivisionsH<=3,"SG_CreateCylinder: 0 face",return 0);
#ifndef SPG_General_PGLib
	CHECK((x0|y0|x1|y1|x2|y2|x3|y3)&&(V_IsBound(texture.NumAttach,0,MaxTex)==0),"SG_CreatePlan: Mauvaise texture",style&=SG_MASKUNI);
#endif	
	CHECK(SG_CreateBloc(Bloc,(SubdivisionsC)*(SubdivisionsH+1),SubdivisionsC*SubdivisionsH,SG_WithPOINTS|SG_WithFACES)==0,"SG_CreateCylinder: SG_CreateBloc echoue",return 0);
	
	int u;
	for(u=0; u<=SubdivisionsC; u++)
	{
		int t;
		float alpha=(float)V_2PI*u/(float)(SubdivisionsC);
		V_VECT vs,vd;
		V_Operate4(vs,=Base,+cos(alpha)*RX,+sin(alpha)*RY);
		V_Operate3(vd,=vs,+RH);
		
		float xs0=x0+(x3-x0)*u/(float)SubdivisionsC;
		float ys0=y0+(y3-y0)*u/(float)SubdivisionsC;
		float xd0=x1+(x2-x1)*u/(float)SubdivisionsC;
		float yd0=y1+(y2-y1)*u/(float)SubdivisionsC;
		
		float xs1=x0+(x3-x0)*(u+1)/(float)SubdivisionsC;
		float ys1=y0+(y3-y0)*(u+1)/(float)SubdivisionsC;
		float xd1=x1+(x2-x1)*(u+1)/(float)SubdivisionsC;
		float yd1=y1+(y2-y1)*(u+1)/(float)SubdivisionsC;
		
		//V_Operate3(vd,=((Subdivisions-u)/(float)(Subdivisions))*v1,+(u/(float)(Subdivisions))*v2);
		for(t=0;t<=SubdivisionsH;t++)
		{
			V_VECT Pos;
			V_Operate3(Pos,=(SubdivisionsH-t)/(float)(SubdivisionsH)*vs,+t/(float)(SubdivisionsH)*vd);
			
			if (u!=SubdivisionsC) 
				SG_DefPnt(Bloc.DB.MemPoints[t+(SubdivisionsH+1)*u],Pos);
			
			if ((u!=SubdivisionsC)&&(t!=SubdivisionsH))
			{
				
				int xcoin0=V_FloatToInt(xs0+(xd0-xs0)*t/(float)SubdivisionsH);
				int ycoin0=V_FloatToInt(ys0+(yd0-ys0)*t/(float)SubdivisionsH);
				int xcoin1=V_FloatToInt(xs0+(xd0-xs0)*(t+1)/(float)SubdivisionsH);
				int ycoin1=V_FloatToInt(ys0+(yd0-ys0)*(t+1)/(float)SubdivisionsH);
				int xcoin2=V_FloatToInt(xs1+(xd1-xs1)*(t+1)/(float)SubdivisionsH);
				int ycoin2=V_FloatToInt(ys1+(yd1-ys1)*(t+1)/(float)SubdivisionsH);
				int xcoin3=V_FloatToInt(xs1+(xd1-xs1)*t/(float)SubdivisionsH);
				int ycoin3=V_FloatToInt(ys1+(yd1-ys1)*t/(float)SubdivisionsH);
				
				if (u!=(SubdivisionsC-1))
				{
					SG_DefTexFceI_Unsafe(Bloc,t+SubdivisionsH*u,
						t+(SubdivisionsH+1)*u,
						t+1+(SubdivisionsH+1)*u,
						t+1+(SubdivisionsH+1)*(u+1),
						t+(SubdivisionsH+1)*(u+1),
						coul,style,texture,
						xcoin0,ycoin0,xcoin1,ycoin1,
						xcoin2,ycoin2,xcoin3,ycoin3);
				}
				else
				{
					SG_DefTexFceI_Unsafe(Bloc,t+SubdivisionsH*u,
						t+(SubdivisionsH+1)*u,
						t+1+(SubdivisionsH+1)*u,
						t+1+0,
						t+0,
						coul,style,texture,
						xcoin0,ycoin0,xcoin1,ycoin1,
						xcoin2,ycoin2,xcoin3,ycoin3);
				}
			}
		}
	}

	SG_MakeBRef(Bloc);
	SG_CalcNormalesToExt(Bloc,Bloc.BRef);
	
	return -1;
}

int SPG_CONV SG_CreateCone(SG_FullBloc &Bloc,
					  int SubdivisionsC,int SubdivisionsH,
					  V_VECT Base,V_VECT RX,V_VECT RY,V_VECT RH,
					  DWORD coul,int style,SG_FullTexture& texture,
					  int x0,int y0,int x1,int y1,
					  int x2,int y2,int x3,int y3)
{
	CHECK(SubdivisionsC<=3,"SG_CreateCylinder: moins de 3 faces",return 0);
	CHECK(SubdivisionsH<=3,"SG_CreateCylinder: 0 face",return 0);
#ifndef SPG_General_PGLib
	CHECK((x0|y0|x1|y1|x2|y2|x3|y3)&&(V_IsBound(texture.NumAttach,0,MaxTex)==0),"SG_CreatePlan: Mauvaise texture",style&=SG_MASKUNI);
#endif
	
	CHECK(SG_CreateBloc(Bloc,(SubdivisionsC)*(SubdivisionsH+1),SubdivisionsC*SubdivisionsH,SG_WithPOINTS|SG_WithFACES)==0,"SG_CreateCylinder: SG_CreateBloc echoue",return 0);
	
	int u;
	for(u=0; u<=SubdivisionsC; u++)
	{
		int t;
		float alpha=(float)(V_2PI*u/(float)(SubdivisionsC));
		V_VECT vs,vd;
		V_Operate4(vs,=Base,+cos(alpha)*RX,+sin(alpha)*RY);
		//V_Operate3(vd,=vs,+RH);
		V_Operate3(vd,=Base,+RH);
		
		float xs0=x0+(x3-x0)*u/(float)SubdivisionsC;
		float ys0=y0+(y3-y0)*u/(float)SubdivisionsC;
		float xd0=x1+(x2-x1)*u/(float)SubdivisionsC;
		float yd0=y1+(y2-y1)*u/(float)SubdivisionsC;
		
		float xs1=x0+(x3-x0)*(u+1)/(float)SubdivisionsC;
		float ys1=y0+(y3-y0)*(u+1)/(float)SubdivisionsC;
		float xd1=x1+(x2-x1)*(u+1)/(float)SubdivisionsC;
		float yd1=y1+(y2-y1)*(u+1)/(float)SubdivisionsC;
		
		//V_Operate3(vd,=((Subdivisions-u)/(float)(Subdivisions))*v1,+(u/(float)(Subdivisions))*v2);
		for(t=0;t<=SubdivisionsH;t++)
		{
			V_VECT Pos;
			V_Operate3(Pos,=(SubdivisionsH-t)/(float)(SubdivisionsH)*vs,+t/(float)(SubdivisionsH)*vd);
			
			if (u!=SubdivisionsC) 
				SG_DefPnt(Bloc.DB.MemPoints[t+(SubdivisionsH+1)*u],Pos);
			
			if ((u!=SubdivisionsC)&&(t!=SubdivisionsH))
			{
				
				int xcoin0=V_FloatToInt(xs0+(xd0-xs0)*t/(float)SubdivisionsH);
				int ycoin0=V_FloatToInt(ys0+(yd0-ys0)*t/(float)SubdivisionsH);
				int xcoin1=V_FloatToInt(xs0+(xd0-xs0)*(t+1)/(float)SubdivisionsH);
				int ycoin1=V_FloatToInt(ys0+(yd0-ys0)*(t+1)/(float)SubdivisionsH);
				int xcoin2=V_FloatToInt(xs1+(xd1-xs1)*(t+1)/(float)SubdivisionsH);
				int ycoin2=V_FloatToInt(ys1+(yd1-ys1)*(t+1)/(float)SubdivisionsH);
				int xcoin3=V_FloatToInt(xs1+(xd1-xs1)*t/(float)SubdivisionsH);
				int ycoin3=V_FloatToInt(ys1+(yd1-ys1)*t/(float)SubdivisionsH);
				
				if (u!=(SubdivisionsC-1))
				{
					SG_DefTexFceI_Unsafe(Bloc,t+SubdivisionsH*u,
						t+(SubdivisionsH+1)*u,
						t+1+(SubdivisionsH+1)*u,
						t+1+(SubdivisionsH+1)*(u+1),
						t+(SubdivisionsH+1)*(u+1),
						coul,style,texture,
						xcoin0,ycoin0,xcoin1,ycoin1,
						xcoin2,ycoin2,xcoin3,ycoin3);
				}
				else
				{
					SG_DefTexFceI_Unsafe(Bloc,t+SubdivisionsH*u,
						t+(SubdivisionsH+1)*u,
						t+1+(SubdivisionsH+1)*u,
						t+1+0,
						t+0,
						coul,style,texture,
						xcoin0,ycoin0,xcoin1,ycoin1,
						xcoin2,ycoin2,xcoin3,ycoin3);
				}
			}
		}
	}

	SG_MakeBRef(Bloc);
	SG_CalcNormalesToDir(Bloc,RH);
	
	return -1;
}

int SPG_CONV SG_CreateSphere(SG_FullBloc &Bloc,
					  int Subdivisions,
					  V_VECT Centre,V_VECT RX,V_VECT RY,V_VECT RZ,
					  DWORD coul,int style)
{
	CHECK(Subdivisions<=0,"SG_CreateSphere: moins de 6 faces",return 0);
	//CHECK(SG_CreateBloc(Bloc,4*6*Subdivisions,6*Subdivisions,SG_WithPOINTS|SG_WithFACES)==0,"SG_CreateSphere: SG_CreateBloc echoue",return 0);
	
	V_VECT Coin1,Coin2,Coin3,Coin4,Coin5,Coin6,Coin7,Coin8;
	V_Operate4(Coin1,=+RX,+RY,+RZ);
	V_Operate4(Coin2,=-RX,+RY,+RZ);
	V_Operate4(Coin3,=+RX,-RY,+RZ);
	V_Operate4(Coin4,=-RX,-RY,+RZ);
	V_Operate4(Coin5,=+RX,+RY,-RZ);
	V_Operate4(Coin6,=-RX,+RY,-RZ);
	V_Operate4(Coin7,=+RX,-RY,-RZ);
	V_Operate4(Coin8,=-RX,-RY,-RZ);

	SG_FullBloc P1,P2,P3,P4,P5,P6;
	SG_FullTexture NTX;
	SG_SetToNullTex(NTX);

	SG_CreatePlan(P1,Subdivisions,Subdivisions,
		Coin1,Coin2,Coin4,Coin3,
		coul,style,NTX,
		0,0,0,0,0,0,0,0);
	SG_CreatePlan(P2,Subdivisions,Subdivisions,
		Coin5,Coin6,Coin8,Coin7,
		coul,style,NTX,
		0,0,0,0,0,0,0,0);
	SG_CreatePlan(P3,Subdivisions,Subdivisions,
		Coin1,Coin2,Coin6,Coin5,
		coul,style,NTX,
		0,0,0,0,0,0,0,0);
	SG_CreatePlan(P4,Subdivisions,Subdivisions,
		Coin3,Coin4,Coin8,Coin7,
		coul,style,NTX,
		0,0,0,0,0,0,0,0);
	SG_CreatePlan(P5,Subdivisions,Subdivisions,
		Coin1,Coin3,Coin7,Coin5,
		coul,style,NTX,
		0,0,0,0,0,0,0,0);
	SG_CreatePlan(P6,Subdivisions,Subdivisions,
		Coin2,Coin4,Coin8,Coin6,
		coul,style,NTX,
		0,0,0,0,0,0,0,0);

	SG_DispatchDescr BlocDescr;
	SG_ConcatAndDispatchBloc6(BlocDescr,1,1,
		(1.0f)/(4.0f*(1+Subdivisions)),//precision relative
		P1,P2,P3,P4,P5,P6);
	Bloc=BlocDescr.Bloc[0];
	SPG_MemFree(BlocDescr.Bloc);

	SG_CloseBloc(P1);
	SG_CloseBloc(P2);
	SG_CloseBloc(P3);
	SG_CloseBloc(P4);
	SG_CloseBloc(P5);
	SG_CloseBloc(P6);

	V_VECT AxeVraiX=RX;
	V_VECT AxeVraiY=RY;
	V_VECT AxeVraiZ=RZ;
	V_NormalizeSafe(AxeVraiX);
	V_NormalizeSafe(AxeVraiY);
	V_NormalizeSafe(AxeVraiZ);
	for(int i=0;i<Bloc.DB.NombreP;i++)
	{
		V_VECT R;
		V_ScalVect(Bloc.DB.MemPoints[i].P,AxeVraiX,R.x);
		V_ScalVect(Bloc.DB.MemPoints[i].P,AxeVraiY,R.y);
		V_ScalVect(Bloc.DB.MemPoints[i].P,AxeVraiZ,R.z);
		V_NormaliseSafe(R);
		V_Operate5(Bloc.DB.MemPoints[i].P,=Centre,+R.x*RX,+R.y*RY,+R.z*RZ);
	}

	SG_MakeBRef(Bloc);
	SG_CalcNormalesSpherical(Bloc,Bloc.BRef);
	
	return -1;
}

#ifdef SGE_EMC
#ifdef DebugSGRAPH
void SPG_CONV SG_CreateDebugBounds(SG_FullBloc &Bloc)
{
	if (Bloc.Etat==0) return;
	CHECK((Bloc.Etat&(SG_WithFACES|SG_WithPOINTS))!=(SG_WithFACES|SG_WithPOINTS),"SG_CreateDebugBounds: Ne pas utiliser sur des blocs non proprietaires de leur memoire qui est peut etre desallouee par le traitement du premier bloc. Le programme devrait planter sous peu.",return);
	SG_CheckBloc(Bloc);
	SG_FullBloc BBloc;
	V_VECT RX={Bloc.Rayon,0,0};
	V_VECT RY={0,Bloc.Rayon,0};
	V_VECT RZ={0,0,Bloc.Rayon};
	SG_CreateSphere(BBloc,5,Bloc.BRef,RX,RY,RZ,0x800000,SG_RENDER_FIL);
	SG_FullBloc FinalBloc;
	SG_ConcatBloc2(FinalBloc,Bloc,BBloc);
	SG_CloseBloc(BBloc);
	SG_CloseBloc(Bloc);
	FinalBloc.BRef=Bloc.BRef;
	FinalBloc.Rayon=Bloc.Rayon;
	Bloc=FinalBloc;
	return;
}
#endif
#endif
#endif


