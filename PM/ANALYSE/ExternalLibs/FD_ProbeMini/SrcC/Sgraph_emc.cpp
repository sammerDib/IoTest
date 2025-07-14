
#include "SPG_General.h"

#ifdef SGE_EMC
#ifdef SPG_General_USESGRAPH

#include "SPG_Includes.h"

//teste dfinition de SGE_DiscardFace
//seuil de (normale scalaire vue)>SGE_BACKSIDESELECT si SG_RENDER_NOBACKSIDE
#define SGE_BACKSIDESELECT 0.001
//#define SGE_HIRESNORMALSELECT
#define SGE_AVGRESNORMALSELECT

//#define SGE_HIRESNORMALLIGHT

#ifdef DebugFloat
#include <float.h>
#endif
#include <string.h>

#define SGTRICAT SG_TriCatNum

//determine si un bloc doit etre trace
bool SPG_CONV SGE_BlocVisible(const SG_PDV& Vue, SG_FullBloc& Bloc)
{
#ifdef DebugSGRAPH
	SG_CheckBloc(Bloc);
#endif
#ifdef SGE_TrieBlocs
#define Bloc_DB_Profondeur Bloc.DB.Profondeur
#define	SGE_BlocVisible_MACRO V_ScalVect(DiffPos,Vue.Rep.axex,Bloc_DB_Profondeur);
#else
#define Bloc_DB_Profondeur Profondeur
#define	SGE_BlocVisible_MACRO float Bloc_DB_Profondeur;V_ScalVect(DiffPos,Vue.Rep.axex,Bloc_DB_Profondeur);
#endif
	//Bloc.Rayon=0;
	V_VECT DiffPos={Bloc.BRef.x-Vue.Rep.pos.x,Bloc.BRef.y-Vue.Rep.pos.y,Bloc.BRef.z-Vue.Rep.pos.z};
	SGE_BlocVisible_MACRO;
	if ((Bloc_DB_Profondeur+Bloc.Rayon)<Vue.DEcr) return false;
	if ((Bloc_DB_Profondeur-Bloc.Rayon)>Vue.DMax) return false;
	
	float ConeDeVisionX=(float)Vue.Ecran.SizeX/(fabsf(Vue.PixlXPM)*2*Vue.DEcr);
	float x;
	V_ScalVect(DiffPos,Vue.Rep.axey,x);
	float xmax=ConeDeVisionX*(Bloc_DB_Profondeur+Bloc.Rayon);
	if ((x-Bloc.Rayon)>xmax) return false;
	if ((-Bloc.Rayon-x)>xmax) return false;
	
	float ConeDeVisionY=(float)Vue.Ecran.SizeY/(fabsf(Vue.PixlYPM)*2*Vue.DEcr);
	float y;
	V_ScalVect(DiffPos,Vue.Rep.axez,y);
	float ymax=ConeDeVisionY*(Bloc_DB_Profondeur+Bloc.Rayon);
	if ((y-Bloc.Rayon)>ymax) return false;
	if ((-Bloc.Rayon-y)>ymax) return false;
	return true;
#undef Bloc_DB_Profondeur
}

#ifdef SGE_DrawNormales
int SPG_CONV SGE_ProjOnScreen(V_VECT& VPROJ, int& X, int& Y, SG_PDV& V)
{
#ifdef G_SubPixelValue
	float PixlXPM=V.PixlXPM*G_SubPixelValue;
	float PixlYPM=V.PixlYPM*G_SubPixelValue;
	float ScreenSizeX=4*V.Ecran.SizeX;///V.PixlXPM;
#else
	//float PixlXPM=V.PixlXPM;
	//float PixlYPM=V.PixlYPM;
	//SHORT ScreenSizeX=8192;//PixlXPM;
#define ScreenSizeX 4096
#endif
	V_VECT const XEcran={V.PixlXPM*V.Rep.axey.x,V.PixlXPM*V.Rep.axey.y,V.PixlXPM*V.Rep.axey.z};
	V_VECT const YEcran={V.PixlYPM*V.Rep.axez.x,V.PixlYPM*V.Rep.axez.y,V.PixlYPM*V.Rep.axez.z};
	SHORT PixMX=(SHORT)G_SubPixel(V.Ecran.SizeX>>1);
	SHORT PixMY=(SHORT)G_SubPixel(V.Ecran.SizeY>>1);
	V_VECT T;
	V_Operate3(T,=VPROJ,-V.Rep.pos);
	float Prof;
	V_ScalVect(T,V.Rep.axex,Prof);
	if(Prof<=0)
	{
		return 0;
	}
	if(Prof>=V.DMax)
	{
		return 0;
	}
	float InvProf=V.DEcr/Prof;
	float ProjAxe;
	V_ScalVect(T,XEcran,ProjAxe);
	int ty=V_FloatToInt(InvProf*ProjAxe);//V.Rep.axey);
	V_ScalVect(T,YEcran,ProjAxe);
	int tz=V_FloatToInt(InvProf*ProjAxe);//V.Rep.axez);
	if ((ty<-ScreenSizeX)||(ty>ScreenSizeX))
	{
		return 0;
	}
	if ((tz<-ScreenSizeX)||(tz>ScreenSizeX))
	{
		return 0;
	}
	X=(SHORT)ty+PixMX;
	Y=(SHORT)tz+PixMY;
	
#ifdef SGE_ClipSafe
	if ((WORD)(np->XECR)>V.Ecran.SizeX) return 0;
	if ((WORD)(np->YECR)>V.Ecran.SizeY) return 0;
#endif
	
	return -1;
}
#endif

void SPG_CONV SGE_CalcPointsCoord(SG_PDV& V)
{
#ifdef G_SubPixelValue
	float PixlXPM=V.PixlXPM*G_SubPixelValue;
	float PixlYPM=V.PixlYPM*G_SubPixelValue;
	float ScreenSizeX=4*V.Ecran.SizeX;///V.PixlXPM;
#else
	//float PixlXPM=V.PixlXPM;
	//float PixlYPM=V.PixlYPM;
	//SHORT ScreenSizeX=8192;//PixlXPM;
#define ScreenSizeX 4096
#endif
	V_VECT XEcran={V.PixlXPM*V.Rep.axey.x,V.PixlXPM*V.Rep.axey.y,V.PixlXPM*V.Rep.axey.z};
	V_VECT YEcran={V.PixlYPM*V.Rep.axez.x,V.PixlYPM*V.Rep.axez.y,V.PixlYPM*V.Rep.axez.z};
	SHORT PixMX=(SHORT)G_SubPixel(V.Ecran.SizeX>>1);
	SHORT PixMY=(SHORT)G_SubPixel(V.Ecran.SizeY>>1);
	for (int nb=0;nb<V.NombreB;nb++)
	{
		for (SG_PNT3D* restrict np=V.MemBloc[nb].MemPoints;np<V.MemBloc[nb].MemPoints+V.MemBloc[nb].NombreP;np++)
		{
			V_VECT T;
			V_Operate3(T,=(*np).P,-V.Rep.pos);
			V_ScalVect(T,V.Rep.axex,np->Prof);
			//if (np->Prof<V.DEcr)
			if (np->Prof<=0)
			{
				np->XECR=-32768;
				continue;
			}
			if (np->Prof>=V.DMax)
			{
				np->XECR=-32768;
				continue;
			}
			float InvProf=V.DEcr/np->Prof;
			float ProjAxe;
			V_ScalVect(T,XEcran,ProjAxe);
			int ty=V_FloatToInt(InvProf*ProjAxe);//V.Rep.axey);
			V_ScalVect(T,YEcran,ProjAxe);
			int tz=V_FloatToInt(InvProf*ProjAxe);//V.Rep.axez);
			if ((ty<-ScreenSizeX)||(ty>ScreenSizeX))
			{
				np->XECR=-32768;
				continue;
			}
			if ((tz<-ScreenSizeX)||(tz>ScreenSizeX))
			{
				np->XECR=-32768;
				continue;
			}
			/*
			np->XECR=((SHORT)(ty*PixlXPM))+PixMX;
			np->YECR=((SHORT)(tz*PixlYPM))+PixMY;
			*/
			np->XECR=(SHORT)(ty+PixMX);
			np->YECR=(SHORT)(tz+PixMY);
			
#ifdef SGE_ClipSafe
			if ((WORD)(np->XECR)>V.Ecran.SizeX) np->XECR=-32768;
			if ((WORD)(np->YECR)>V.Ecran.SizeY) np->XECR=-32768;
#endif
		}
	}
	return;
}

void SPG_CONV SGE_RefreshTriBuff(SG_PDV& V)
{
	if (V.RenderMode&SGR_BLOC_TRI)
	{
		//float NProf=0x0ffffff/V.DMax;
		float NProfF=0x03fffff/V.DMax;
		SG_Tri* DestT=V.MemTri;
		SG_Tri* EndDestT=V.MemTri+V.NombreT;
		for (int nb=0;nb<V.NombreB;nb++)
		{
			//DWORD BlocProf=NProf*V.MemBloc[nb].Profondeur;
			for (SG_FACE* nf=V.MemBloc[nb].MemFaces;nf<V.MemBloc[nb].MemFaces+V.MemBloc[nb].NombreF;nf++)
			{
				if (nf->NumP1->XECR==-32768)
				{
					continue;
				}
				if (nf->NumP2->XECR==-32768)
				{
					continue;
				}
				if (nf->NumP3->XECR==-32768)
				{
					continue;
				}
				if (nf->NumP4->XECR==-32768)
				{
					continue;
				}
#ifdef SGE_DiscardFace
				if (
					((WORD)nf->NumP1->XECR>=(WORD)V.Ecran.SizeX)&&
					((WORD)nf->NumP2->XECR>=(WORD)V.Ecran.SizeX)&&
					((WORD)nf->NumP3->XECR>=(WORD)V.Ecran.SizeX)&&
					((WORD)nf->NumP4->XECR>=(WORD)V.Ecran.SizeX)
					)
				{
					continue;
				}
				if (
					((WORD)nf->NumP1->YECR>=(WORD)V.Ecran.SizeY)&&
					((WORD)nf->NumP2->YECR>=(WORD)V.Ecran.SizeY)&&
					((WORD)nf->NumP3->YECR>=(WORD)V.Ecran.SizeY)&&
					((WORD)nf->NumP4->YECR>=(WORD)V.Ecran.SizeY)
					)
				{
					continue;
				}
#endif				
				
				DestT->Face=nf;
				DestT->Prof=V_FloatToLong(NProfF*(nf->NumP1->Prof+nf->NumP2->Prof+nf->NumP3->Prof+nf->NumP4->Prof));
				//DestT->Prof=BlocProf;
				DestT++;
				if(DestT>EndDestT) break;
			}
			if(DestT>EndDestT) break;
		}
		V.TriFait=((int)DestT-(int)V.MemTri)/sizeof(SG_Tri);
		
		if((V.RenderMode&SGR_FACE_NOTRI)==0)
		{
			bool Permute=true;
			while(Permute)
			{
				Permute=false;
				//	for(SG_Tri* nt=V.MemTri;nt<V.MemTri+V.TriFait-1;nt++)
				
				for(SG_Tri* restrict nt=V.MemTri+V.TriFait-2;nt>V.MemTri;nt--)
				{
					if((nt->Prof)>((nt+1)->Prof))
					{
						Permute=true;
						SG_Tri PickFaceTri=*nt;
						SG_Tri* ntd;
						for(ntd=nt+2;ntd<V.MemTri+V.TriFait;ntd++)
						{
							if(ntd->Prof>=PickFaceTri.Prof)	break;
							*(ntd-2)=*(ntd-1);
						}
						*(ntd-2)=*(ntd-1);
						*(ntd-1)=PickFaceTri;
					}
				}
			}
		}
	}
	else
	{
		{for (int nb=0;nb<V.NombreB;nb++)
		{
			for (SG_FACE* restrict nf=V.MemBloc[nb].MemFaces;nf<V.MemBloc[nb].MemFaces+V.MemBloc[nb].NombreF;nf++)
			{
				if (nf->NumP1->XECR==-32768)
				{
					nf->TriState=0;
					continue;
				}
				if (nf->NumP2->XECR==-32768)
				{
					nf->TriState=0;
					continue;
				}
				if (nf->NumP3->XECR==-32768)
				{
					nf->TriState=0;
					continue;
				}
				if (nf->NumP4->XECR==-32768)
				{
					nf->TriState=0;
					continue;
				}
#ifdef SGE_DiscardFace
				if (
					((WORD)nf->NumP1->XECR>=(WORD)V.Ecran.SizeX)&&
					((WORD)nf->NumP2->XECR>=(WORD)V.Ecran.SizeX)&&
					((WORD)nf->NumP3->XECR>=(WORD)V.Ecran.SizeX)&&
					((WORD)nf->NumP4->XECR>=(WORD)V.Ecran.SizeX)
					)
				{
					nf->TriState=0;
					continue;
				}
				if (
					((WORD)nf->NumP1->YECR>=(WORD)V.Ecran.SizeY)&&
					((WORD)nf->NumP2->YECR>=(WORD)V.Ecran.SizeY)&&
					((WORD)nf->NumP3->YECR>=(WORD)V.Ecran.SizeY)&&
					((WORD)nf->NumP4->YECR>=(WORD)V.Ecran.SizeY)
					)
				{
					nf->TriState=0;
					continue;
				}
#endif				
				if (nf->Style&SG_RENDER_NOBACKSIDE)
				{
#ifdef SGE_HIRESNORMALSELECT
					V_VECT FCG;
					SG_FaceCG(FCG,(*nf));
					V_VECT DiffDir=V_Operate2V(FCG,-V.Rep.pos);
					if (V_ScalVect(nf->Normale,DiffDir)>SGE_BACKSIDESELECT) continue;
#else
#ifdef SGE_AVGRESNORMALSELECT
					V_VECT DiffDir=V_Operate2V(nf->NumP1->P,-V.Rep.pos);
					float BackSideVision;
					V_ScalVect(nf->Normale,DiffDir,BackSideVision);
					if (BackSideVision>SGE_BACKSIDESELECT) continue;
#else
					float BackSideVision;
					V_ScalVect(nf->Normale,V.Rep.axex,BackSideVision);
					if (BackSideVision>SGE_BACKSIDESELECT) continue;
#endif
#endif
				}
				nf->TriState=1;
			}
		}}
		
		float NProf=0x03fffff/V.DMax;
		SG_Tri* restrict DestT=V.MemTri+V.NombreT;
		memset(V.TriCat,0,SGTRICAT*sizeof(SG_TriCatType));
		

		{for(SG_Tri* restrict nt=V.MemTri;nt<V.MemTri+V.TriFait;nt++)
		{
			if (nt->Face->TriState==1)
			{
				SG_FACE* nf=DestT->Face=nt->Face;
				DestT->Prof=V_FloatToLong(NProf*(nf->NumP1->Prof+nf->NumP2->Prof+nf->NumP3->Prof+nf->NumP4->Prof));
				nf->TriState=0;
				V.TriCat[DestT->BProf]++;
				DestT++;
			}
		}}
		
		SG_Tri* restrict EndDestT=V.MemTri+(V.NombreT<<1);
		if(DestT<EndDestT)
		{
			{for (int nb=0;nb<V.NombreB;nb++)
			{
				for (SG_FACE* restrict nf=V.MemBloc[nb].MemFaces;nf<V.MemBloc[nb].MemFaces+V.MemBloc[nb].NombreF;nf++)
				{
					if (nf->TriState==1)
					{
						DestT->Face=nf;
						DestT->Prof=V_FloatToLong(NProf*(nf->NumP1->Prof+nf->NumP2->Prof+nf->NumP3->Prof+nf->NumP4->Prof));
						nf->TriState=0;
						V.TriCat[DestT->BProf]++;
						DestT++;
						if(DestT>=EndDestT) break;
					}
				}
				if(DestT>=EndDestT) break;
			}}
		}
		
		
		if((V.RenderMode&SGR_FACE_NOTRI)==0)
		{
			V.TriPtrCat[0]=V.MemTri;
			for(int nc=0;nc<SGTRICAT-1;nc++)
				V.TriPtrCat[nc+1]=V.TriPtrCat[nc]+V.TriCat[nc];
			V.TriFait=
				(
				(int)V.TriPtrCat[SGTRICAT-1]+V.TriCat[SGTRICAT-1]-(int)(V.MemTri)
				)/sizeof(SG_Tri);
			
			{for (SG_Tri* restrict nt=V.MemTri+V.NombreT;nt<DestT;nt++)
			{
				*(V.TriPtrCat[nt->BProf])=*nt;
				V.TriPtrCat[nt->BProf]++;
			}}
		}
		else
		{
			V.TriFait=((int)DestT-(int)(V.MemTri+V.NombreT))/sizeof(SG_Tri);
			memcpy(V.MemTri,V.MemTri+V.NombreT,V.TriFait*sizeof(SG_Tri));
		}	
}
return;
}

void SPG_CONV SGE_FinishRender(SG_PDV& V)
{
	PixCoul GlobalColor;
	{
		float Ix=V_Sature(V.DFog/V.DMax,0,1);
		G_Interpole(GlobalColor,V.SkyColor,V.FogColor,Ix);
	}
	
#ifdef DebugRenderTimer
	S_StartTimer(Global.T_SGE_G_ClearSky);
#endif
	if ((V.RenderMode&SGR_SKY_NOCLEAR)==0) G_DrawRect(V.Ecran,0,0,V.Ecran.SizeX,V.Ecran.SizeY,GlobalColor.Coul);
#ifdef DebugRenderTimer
	S_StopTimer(Global.T_SGE_G_ClearSky);
#endif
	
	if (V.RenderMode&SGR_NORMAL)
	{
		LONG FogPos;
		DWORD FogMul;
		
		if(V.DFog>=V.DMax) 
			FogPos=SG_OVERALL_DIST;
		else
		{
			FogPos=V_FloatToLong(0x0ffffff*V.DFog/V.DMax);
			if (FogPos==0x0ffffff) 
				FogPos=SG_OVERALL_DIST;
			else
				FogMul=0xffffffff/(0xffffff-FogPos);
		}
		
		LONG ProfTexMax=(LONG)(0x0ffffff*V.DTex/V.DMax);
#ifdef SGE_ReverseTri
		for (SG_Tri* restrict nt=V.MemTri;nt<V.MemTri+V.TriFait;nt++)
#else
			for (SG_Tri* restrict nt=V.MemTri+V.TriFait-1;nt>=V.MemTri;nt--)
#endif
			{
				G_PixCoord GPC[4];
				const SG_FACE* restrict Face=(nt->Face);
				GPC[0].xy=Face->NumP1->PECR.xy;
				GPC[1].xy=Face->NumP2->PECR.xy;
				GPC[2].xy=Face->NumP3->PECR.xy;
				GPC[3].xy=Face->NumP4->PECR.xy;
				
				PixCoul ResultColor;
				PixCoul LightColor;
				
				if(nt->Prof>FogPos)
					GlobalColor.A=(BYTE)(((DWORD)(nt->Prof-FogPos)*FogMul)>>24);
				else
					GlobalColor.A=0;
				if(V.NombreLight)
					SGE_CalcLight(V,*Face,LightColor);
				else
					LightColor.A=0;
				if((LightColor.A==0)&&(GlobalColor.A==0))
					ResultColor.A=0;
				//ResultColor=Face->Couleur;
				else if (LightColor.A==0)
					ResultColor=GlobalColor;
				else if (GlobalColor.A==0)
					ResultColor=LightColor;
				else
				{
					G_Combine(ResultColor,LightColor,GlobalColor);
				}
				
				if((Face->Style&SG_RENDER_TEX)&&(nt->Prof<ProfTexMax))
				{
					if(ResultColor.A==0)
					{
					/*
					TEXTURE SANS LUMIERE
						*/
#ifdef DebugRenderTimer
						S_StartTimer(Global.T_SGE_G_Draw);
#endif
						if (Face->Style&SG_RENDER_NORMAL)
						{
							G_DrawTexPoly4(V.Ecran,
								GPC,(G_TexCoord*)&Face->T,((G_Texture*)V.MemTex)[Face->NumTex]);
						}
						else if  (Face->Style&SG_RENDER_TRANSL)
						{
							G_DrawTexTranslPoly4(V.Ecran,
								GPC,(G_TexCoord*)&Face->T,((G_Texture*)V.MemTex)[Face->NumTex]);
						}
						else if  (Face->Style&SG_RENDER_TEXMSK)
						{
							PixCoul Transparency;
							Transparency.Coul=0;
							G_DrawTexMaskPoly4(V.Ecran,
								GPC,(G_TexCoord*)&Face->T,((G_Texture*)V.MemTex)[Face->NumTex],Transparency);
						}
#ifdef DebugRenderTimer
						S_StopTimer(Global.T_SGE_G_Draw);
#endif
					}
					else
					{
					/*
					TEXTURE AVEC LUMIERE
						*/
#ifdef DebugRenderTimer
						S_StartTimer(Global.T_SGE_G_Draw);
#endif
						G_DrawTexLightPoly4(V.Ecran,
							GPC,(G_TexCoord*)&Face->T,((G_Texture*)V.MemTex)[Face->NumTex],ResultColor);
#ifdef DebugRenderTimer
						S_StopTimer(Global.T_SGE_G_Draw);
#endif						
					}
				}
				else
				{
					if(ResultColor.A==0)
					{
					/*
					UNI SANS LUMIERE
						*/
#ifdef DebugRenderTimer
						S_StartTimer(Global.T_SGE_G_Draw);
#endif
						if (Face->Style&SG_RENDER_NORMAL)
						{
							G_DrawUniPoly4(V.Ecran,
								GPC,Face->Couleur);
						}
						else if  (Face->Style&SG_RENDER_TRANSL)
						{
							G_DrawUniTranslPoly4(V.Ecran,
								GPC,Face->Couleur);
						}
						else if (Face->Style&SG_RENDER_FIL)
						{
							SG_FACE* Face=(nt->Face);
							G_DrawLine(V.Ecran,
								Face->NumP1->XECR,Face->NumP1->YECR,
								Face->NumP2->XECR,Face->NumP2->YECR,
								Face->Couleur.Coul);
							G_DrawLine(V.Ecran,
								Face->NumP3->XECR,Face->NumP3->YECR,
								Face->NumP2->XECR,Face->NumP2->YECR,
								Face->Couleur.Coul);
							G_DrawLine(V.Ecran,
								Face->NumP3->XECR,Face->NumP3->YECR,
								Face->NumP4->XECR,Face->NumP4->YECR,
								Face->Couleur.Coul);
							G_DrawLine(V.Ecran,
								Face->NumP1->XECR,Face->NumP1->YECR,
								Face->NumP4->XECR,Face->NumP4->YECR,
								Face->Couleur.Coul);
						}
#ifdef DebugRenderTimer
						S_StopTimer(Global.T_SGE_G_Draw);
#endif
					}
					else
					{
					/*
					UNI AVEC LUMIERE
						*/
#ifdef DebugRenderTimer
						S_StartTimer(Global.T_SGE_G_Draw);
#endif
						if (Face->Style&SG_RENDER_NORMAL)
						{
							G_DrawUniLightPoly4(V.Ecran,
								GPC,Face->Couleur,ResultColor);
						}
						else if  (Face->Style&SG_RENDER_TRANSL)
						{
							G_DrawUniTranslLightPoly4(V.Ecran,
								GPC,Face->Couleur,ResultColor);
						}
						else if (Face->Style&SG_RENDER_FIL)
						{
							PixCoul FinalColor;
							G_CombineFaceAndLight(FinalColor,Face->Couleur,ResultColor);
							SG_FACE* Face=(nt->Face);
							G_DrawLine(V.Ecran,
								Face->NumP1->XECR,Face->NumP1->YECR,
								Face->NumP2->XECR,Face->NumP2->YECR,
								FinalColor.Coul);
							G_DrawLine(V.Ecran,
								Face->NumP3->XECR,Face->NumP3->YECR,
								Face->NumP2->XECR,Face->NumP2->YECR,
								FinalColor.Coul);
							G_DrawLine(V.Ecran,
								Face->NumP3->XECR,Face->NumP3->YECR,
								Face->NumP4->XECR,Face->NumP4->YECR,
								FinalColor.Coul);
							G_DrawLine(V.Ecran,
								Face->NumP1->XECR,Face->NumP1->YECR,
								Face->NumP4->XECR,Face->NumP4->YECR,
								FinalColor.Coul);
						}
#ifdef DebugRenderTimer
						S_StopTimer(Global.T_SGE_G_Draw);
#endif
					}
				}
#ifdef SGE_EMC
#ifdef SGE_DrawNormales
#pragma SPGMSG(__FILE__,__LINE__,"Dessin des normales")
				V_VECT fcg;
				SG_FaceCG(fcg,(*Face));
				float LenTot;
				{
					V_Mod2DiffVect_Unsafe(Face->NumP1->P,Face->NumP2->P,LenTot);
					float LenP;
					V_Mod2DiffVect_Unsafe(Face->NumP2->P,Face->NumP3->P,LenP);
					LenTot+=LenP;
					V_Mod2DiffVect_Unsafe(Face->NumP3->P,Face->NumP4->P,LenP);
					LenTot+=LenP;
					V_Mod2DiffVect_Unsafe(Face->NumP4->P,Face->NumP1->P,LenP);
					LenTot+=LenP;
				}
				LenTot=0.2f*sqrt(LenTot);
				V_VECT vxtreme;
				//attention bug du compilateur
				//version ok
				V_Operate3(vxtreme,=fcg,+LenTot*Face->Normale);
				//version buggee
				//V_Operate3(vxtreme,=fcg,+0.2*(LenX+LenY)*Face->Normale);
				int XB,YB;
				if(SGE_ProjOnScreen(fcg,XB,YB,V))
				{
					int XD,YD;
					if(SGE_ProjOnScreen(vxtreme,XD,YD,V))
					{
						G_DrawLine(V.Ecran,
							XB,YB,
							XD,YD,
							((Face->Couleur.Coul&0x00fefefe)>>1)|0x008000);
					}
					if(SG_CheckFceFnct((*Face)))
					{
						G_DrawRect(V.Ecran,
							XB-4,YB-4,
							XB+4,YB+4,
							((Face->Couleur.Coul&0x00fefefe)>>1)|0x00800000);
						
						G_DrawLine(V.Ecran,
							Face->NumP1->XECR,Face->NumP1->YECR,
							Face->NumP2->XECR,Face->NumP2->YECR,
							((Face->Couleur.Coul&0x00fefefe)>>1)|0x00800080);
						G_DrawLine(V.Ecran,
							Face->NumP3->XECR,Face->NumP3->YECR,
							Face->NumP2->XECR,Face->NumP2->YECR,
							((Face->Couleur.Coul&0x00fefefe)>>1)|0x00800080);
						G_DrawLine(V.Ecran,
							Face->NumP3->XECR,Face->NumP3->YECR,
							Face->NumP4->XECR,Face->NumP4->YECR,
							((Face->Couleur.Coul&0x00fefefe)>>1)|0x00800080);
						G_DrawLine(V.Ecran,
							Face->NumP1->XECR,Face->NumP1->YECR,
							Face->NumP4->XECR,Face->NumP4->YECR,
							((Face->Couleur.Coul&0x00fefefe)>>1)|0x00800080);
					}
				}
#endif
#endif
}
}
else if (V.RenderMode&SGR_UNI)
{
#ifdef SGE_ReverseTri
	for (SG_Tri* restrict nt=V.MemTri;nt<V.MemTri+V.TriFait;nt++)
#else
		for (SG_Tri* restrict nt=V.MemTri+V.TriFait-1;nt>=V.MemTri;nt--)
#endif
		{
			G_PixCoord GPC[4];
			const SG_FACE* restrict Face=(nt->Face);
			GPC[0].xy=Face->NumP1->PECR.xy;
			GPC[1].xy=Face->NumP2->PECR.xy;
			GPC[2].xy=Face->NumP3->PECR.xy;
			GPC[3].xy=Face->NumP4->PECR.xy;
			
#ifdef DebugRenderTimer
			S_StartTimer(Global.T_SGE_G_Draw);
#endif
			G_DrawUniPoly4(V.Ecran,
				GPC,Face->Couleur);
#ifdef DebugRenderTimer
			S_StopTimer(Global.T_SGE_G_Draw);
#endif
		}
}
else if (V.RenderMode&SGR_FILAIRE)
{
#ifdef SGE_ReverseTri
	for (SG_Tri* restrict nt=V.MemTri;nt<V.MemTri+V.TriFait;nt++)
#else
		for (SG_Tri* restrict nt=V.MemTri+V.TriFait-1;nt>=V.MemTri;nt--)
#endif
		{
			const SG_FACE* restrict Face=(nt->Face);
			G_DrawLine(V.Ecran,
				Face->NumP1->XECR,Face->NumP1->YECR,
				Face->NumP2->XECR,Face->NumP2->YECR,
				Face->Couleur.Coul);
			G_DrawLine(V.Ecran,
				Face->NumP3->XECR,Face->NumP3->YECR,
				Face->NumP2->XECR,Face->NumP2->YECR,
				Face->Couleur.Coul);
			G_DrawLine(V.Ecran,
				Face->NumP3->XECR,Face->NumP3->YECR,
				Face->NumP4->XECR,Face->NumP4->YECR,
				Face->Couleur.Coul);
			G_DrawLine(V.Ecran,
				Face->NumP1->XECR,Face->NumP1->YECR,
				Face->NumP4->XECR,Face->NumP4->YECR,
				Face->Couleur.Coul);
		}
}
/*
else
{
#ifndef REM_DEBILUS_BROUILLARDUS
BYTE CurrentFog=V.FogPos[0];
int FogNr=0;
#endif
if (V.Ecran.POCT!=4) return;
#ifdef SGE_ReverseTri
for (SG_Tri* nt=V.MemTri;nt<V.MemTri+V.TriFait;nt++)
#else
for (SG_Tri* nt=V.MemTri+V.TriFait-1;nt>=V.MemTri;nt--)
#endif
{
SG_FACEUnique F;
F.X1=G_TruePixel(nt->Face->NumP1->XECR);
F.Y1=G_TruePixel(nt->Face->NumP1->YECR);
F.X2=G_TruePixel(nt->Face->NumP2->XECR);
F.Y2=G_TruePixel(nt->Face->NumP2->YECR);
F.X3=G_TruePixel(nt->Face->NumP3->XECR);
F.Y3=G_TruePixel(nt->Face->NumP3->YECR);
F.X4=G_TruePixel(nt->Face->NumP4->XECR);
F.Y4=G_TruePixel(nt->Face->NumP4->YECR);
F.XT1=G_TrueTexel(nt->Face->XT1);
F.YT1=G_TrueTexel(nt->Face->YT1);
F.XT2=G_TrueTexel(nt->Face->XT2);
F.YT2=G_TrueTexel(nt->Face->YT2);
F.XT3=G_TrueTexel(nt->Face->XT3);
F.YT3=G_TrueTexel(nt->Face->YT3);
F.XT4=G_TrueTexel(nt->Face->XT4);
F.YT4=G_TrueTexel(nt->Face->YT4);
F.Coul=nt->Face->Couleur.Coul;
F.Blend=32;
F.Style=nt->Face->Style;
F.TriState=nt->Face->TriState;
F.NumTex=nt->Face->NumTex;
FACE(&F, V.Ecran.MECR, V.Ecran.Pitch, V.Ecran.SizeX, V.Ecran.SizeY, V.MemTex[nt->Face->NumTex].MemTex, V.MemTex[nt->Face->NumTex].LarT, 0);
#ifndef REM_DEBILUS_BROUILLARDUS
if(nt->BProf<CurrentFog)
{
G_DrawFog(V.Ecran,V.FogColor[FogNr],FogNr);
FogNr++;
if (FogNr==SG_MAX_FOG) 
{
CurrentFog=0;
//G_Soften(V.Ecran);
}
else
CurrentFog=V.FogPos[FogNr];
}
#endif
}

  }
*/
return;
}

void SPG_CONV SGE_TransformAndRender(SG_PDV& Vue)
{
	G_LogTimeEN(LT_RENDER);
	SG_VueCheck(Vue);
#ifdef DebugRenderTimer
	S_StartTimer(Global.T_SGE_TransformAndRender)
#endif
	IF_CD_G_CHECK(29,return);
#ifdef DebugRenderTimer
	S_StartTimer(Global.T_SGE_CalcPointsCoord)
#endif
	SGE_CalcPointsCoord(Vue);
#ifdef DebugRenderTimer
	S_StopTimer(Global.T_SGE_CalcPointsCoord)
	S_StartTimer(Global.T_SGE_RefreshTriBuff)
#endif
	SGE_RefreshTriBuff(Vue);
#ifdef DebugRenderTimer
	S_StopTimer(Global.T_SGE_RefreshTriBuff)
	S_StartTimer(Global.T_SGE_FinishRender)
#endif
	SGE_FinishRender(Vue);
#ifdef DebugRenderTimer
	S_StopTimer(Global.T_SGE_FinishRender)
#endif
	CD_G_CHECK_EXIT(1,5);
#ifdef DebugRenderTimer
	S_StopTimer(Global.T_SGE_TransformAndRender)
#endif
	G_LogTimeRN(LT_RENDER);
	return;
}

/*
(R*alpha+L*(1-alpha))*alpha'+L'*(1-alpha')=
R*alphaalpha'+L*alpha'-alphaalpha'L+L'-alpha'L'=
R*x+?*(1-x)

  L*alpha'-alphaalpha'L+L'-alpha'L'=
  
	
	  L'-alpha'L'
	  
		
		  R*alphaalpha'+(1-alphaalpha')*(L/alpha-
		  
			
*/

void SPG_CONV SGE_CalcLight(const SG_PDV& Vue, const SG_FACE& Face, PixCoul& ResultColor)
{
	ResultColor.A=0;
	if(Vue.NombreLight==0) return;
	
#ifdef DebugRenderTimer
	S_StartTimer(Global.T_SGE_CalcLight);
#endif
	
#ifdef SGE_HIRESNORMALLIGHT
	V_VECT FacePosition;
	SG_FaceCG(FacePosition,Face);
#else
	V_VECT FacePosition=Face.NumP1->P;
#endif
	
	int B=0;
	int V=0;
	int R=0;
	int A=0;
	int LIPonder=0;
	
	for(int NumLight=0;NumLight<Vue.NombreLight;NumLight++)
	{
		if(Vue.MemLight[NumLight].Etat&L_AMBIENT)
		{
			PixCoul LightColor=Vue.MemLight[NumLight].LightColor;
			
			int LightIntensity=LightColor.A;
			
			B+=LightIntensity*LightColor.B;
			V+=LightIntensity*LightColor.V;
			R+=LightIntensity*LightColor.R;
			A+=LightIntensity*LightColor.A;
			LIPonder+=LightIntensity;
		}
		else if (Vue.MemLight[NumLight].Etat&L_ISOTROPE)
		{
			float DistCarre=Vue.MemLight[NumLight].LightDistMax;
			DistCarre*=DistCarre;
			
			V_VECT DiffPos={
				Vue.MemLight[NumLight].LightPos.x-FacePosition.x,
					Vue.MemLight[NumLight].LightPos.y-FacePosition.y,
					Vue.MemLight[NumLight].LightPos.z-FacePosition.z};
				
				float DfN;
				V_Mod2Vect(DiffPos,DfN);
				if(DfN<DistCarre)
				{
					if(DfN>0)
					{
						float DL=sqrt(DfN);
						float S;
						V_ScalVect(DiffPos,Face.Normale,S)
							if(S>0)
							{
								int LightIntensity;
								if (
									(
									LightIntensity=
									V_FloatToInt(
									Vue.MemLight[NumLight].LightColor.A*
									((S/DL)-DL/Vue.MemLight[NumLight].LightDistMax)
									)
									)
									>0)
								{
									
									PixCoul LightColor=Vue.MemLight[NumLight].LightColor;
									
									B+=LightIntensity*LightColor.B;
									V+=LightIntensity*LightColor.V;
									R+=LightIntensity*LightColor.R;
									A+=LightIntensity*LightColor.A;
									LIPonder+=LightIntensity;
								}
							}
					}
				}
		}
	}
	
	if ((A==0)||(LIPonder==0))
		{
#ifdef DebugRenderTimer
		S_StopTimer(Global.T_SGE_CalcLight);
#endif
		return;
		}
	int InvFact=65535/LIPonder;
	
	{
		WORD NB=(WORD)((B*InvFact)>>16);
		ResultColor.B=(BYTE)V_SatureUP(NB,255);
	}
	{
		WORD NV=(WORD)((V*InvFact)>>16);
		ResultColor.V=(BYTE)V_SatureUP(NV,255);
	}
	{
		WORD NR=(WORD)((R*InvFact)>>16);
		ResultColor.R=(BYTE)V_SatureUP(NR,255);
	}
	ResultColor.A=(BYTE)V_SatureUP((A>>8),255);
	
#ifdef DebugRenderTimer
	S_StopTimer(Global.T_SGE_CalcLight);
#endif
	return;
}

#endif
#endif


