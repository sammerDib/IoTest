

#include "SPG_General.h"

#ifdef SPG_General_USESGRAPH
#ifdef SPG_General_PGLib

#include <string.h>
#include <stdio.h>
#ifdef DebugFloat
#include <float.h>
#endif

#include "SPG_Includes.h"
#define MaxTex SG_MAX_TEX

//determine si un bloc doit etre trace
bool SPG_CONV SG_PGLBlocVisible(SG_PDV& Vue, SG_FullBloc& Bloc)
{
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
	float ConeDeVisionX=1;//(float)Vue.Ecran.SizeX/(fabs(Vue.PixlXPM)*2*Vue.DEcr);
	float x;
	V_ScalVect(DiffPos,Vue.Rep.axey,x);
	float xmax=ConeDeVisionX*(Bloc_DB_Profondeur+Bloc.Rayon);
	if ((x-Bloc.Rayon)>xmax) return false;
	if ((-Bloc.Rayon-x)>xmax) return false;
	
	float ConeDeVisionY=1;//(float)Vue.Ecran.SizeY/(fabs(Vue.PixlYPM)*2*Vue.DEcr);
	float y;
	V_ScalVect(DiffPos,Vue.Rep.axez,y);
	float ymax=ConeDeVisionY*(Bloc_DB_Profondeur+Bloc.Rayon);
	if ((y-Bloc.Rayon)>ymax) return false;
	if ((-Bloc.Rayon-y)>ymax) return false;
	return true;
#undef Bloc_DB_Profondeur
}

void SPG_CONV SG_PGLCreateQuad(SG_PNT3D*OpenGLpnt,PGLColor*OpenGLcolor,PGLTexCoord*OpenGLTexCoord,PGLTexture*Texture,SG_FACE& Face)
{
	PixCoul CTEMP;
	CTEMP=Face.Couleur;
	V_Swap(BYTE,CTEMP.R,CTEMP.B);
	OpenGLcolor[3].c=
		OpenGLcolor[2].c=
		OpenGLcolor[1].c=
		OpenGLcolor[0].c=
		CTEMP.Coul;
	OpenGLpnt[0]=*(Face.NumP1);
	OpenGLpnt[1]=*(Face.NumP2);
	OpenGLpnt[2]=*(Face.NumP3);
	OpenGLpnt[3]=*(Face.NumP4);
	if(OpenGLTexCoord)
	{
		OpenGLTexCoord[0].u=((float)Face.XT1/(Texture->width));
		OpenGLTexCoord[0].v=((float)Face.YT1/(Texture->height));
		OpenGLTexCoord[1].u=((float)Face.XT2/(Texture->width));
		OpenGLTexCoord[1].v=((float)Face.YT2/(Texture->height));
		OpenGLTexCoord[2].u=((float)Face.XT3/(Texture->width));
		OpenGLTexCoord[2].v=((float)Face.YT3/(Texture->height));
		OpenGLTexCoord[3].u=((float)Face.XT4/(Texture->width));
		OpenGLTexCoord[3].v=((float)Face.YT4/(Texture->height));
	}
	return;
}

void SPG_CONV SG_PGLCreateIQuad(SG_INDEX*OpenGLindex,PGLColor*OpenGLcolor,PGLTexCoord*OpenGLTexCoord,PGLTexture*Texture,SG_FACE& Face,SG_PNT3D*MemPoints)
{
	//attention le c divise automatiquement les (int)(soustraction de pointeurs) par sizeof(*pointeur)
	OpenGLindex[0]=(Face.NumP1-MemPoints);//sizeof(SG_PNT3D);
	OpenGLindex[1]=(Face.NumP2-MemPoints);//sizeof(SG_PNT3D);
	OpenGLindex[2]=(Face.NumP3-MemPoints);//sizeof(SG_PNT3D);
	OpenGLindex[3]=(Face.NumP4-MemPoints);//sizeof(SG_PNT3D);
	PixCoul CTEMP;
	CTEMP=Face.Couleur;
	V_Swap(BYTE,CTEMP.R,CTEMP.B);
	OpenGLcolor[OpenGLindex[3]].c=
		OpenGLcolor[OpenGLindex[2]].c=
		OpenGLcolor[OpenGLindex[1]].c=
		OpenGLcolor[OpenGLindex[0]].c=
		CTEMP.Coul;
	if(OpenGLTexCoord)
	{
		OpenGLTexCoord[OpenGLindex[0]].u=((float)Face.XT1/(Texture->width));
		OpenGLTexCoord[OpenGLindex[0]].v=((float)Face.YT1/(Texture->height));
		OpenGLTexCoord[OpenGLindex[1]].u=((float)Face.XT2/(Texture->width));
		OpenGLTexCoord[OpenGLindex[1]].v=((float)Face.YT2/(Texture->height));
		OpenGLTexCoord[OpenGLindex[2]].u=((float)Face.XT3/(Texture->width));
		OpenGLTexCoord[OpenGLindex[2]].v=((float)Face.YT3/(Texture->height));
		OpenGLTexCoord[OpenGLindex[3]].u=((float)Face.XT4/(Texture->width));
		OpenGLTexCoord[OpenGLindex[3]].v=((float)Face.YT4/(Texture->height));
	}
	return;
}
//lors du dispatch les textures
//existent deja
//en consequence elle ne doivent pas etre desallouees dans sg_closebloc
//en effet la meme texture peut apparaitre dans plusieurs blocs et
//certaines eventuellement ne pas apparaitre
int SPG_CONV SG_PGLDispatchByTex(SG_FullBloc& B,PGLPrimitive PGL_FaceType,int BlocIndexe, int BlocLinke)
{
/*
CHECK(B.Etat==0,"SG_PGLDispatchByTex: Bloc vide",return);
CHECK(B.DB.MemFaces==0,"SG_PGLDispatchByTex: Bloc vide",return);
	*/
	
	//les blocs indexes marchent moyennement
	//CHECK(BlocIndexe,"SG_PGLDispatchByTex: Cette fonctionnalite n'est pas implementee",return 0);
	
	//le resultat d'un concatanddispatch peut avoir beaucoup de blocs nuls
	CHECK(B.Etat==0,"SG_PGLDispatchByTex: Bloc nul",return 0);
	//if(B.Etat==0) return 0;
	CHECK(B.DB.MemFaces==0,"SG_PGLDispatchByTex: Bloc vide",return 0);
	int IncVertex=0;
	if(PGL_FaceType==PGL_QUADS) IncVertex=4;
	if(PGL_FaceType==PGL_TRIANGLES) IncVertex=6;
	CHECK(IncVertex==0,"SG_PGLDispatchByTexQuads: PGL_FaceType doit etre PGL_QUADS ou PGL_TRI",return 0);
	PGLTexture* ValTex[MaxTex];
	memset(ValTex,0,MaxTex*sizeof(int));
	int NFacesByTex[MaxTex];
	memset(NFacesByTex,0,MaxTex*sizeof(int));
	int NReg=0;
	for(int i=0;i<B.DB.NombreF;i++)
	{
		for(int r=0;r<NReg;r++)
		{
			if(ValTex[r]==SG_GetFceTex(B.DB.MemFaces[i])) break;
		}
		if(r==NReg)
		{
			ValTex[NReg]=SG_GetFceTex(B.DB.MemFaces[i]);
			NReg++;
		}
		NFacesByTex[r]++;
	}
	B.NombreTex=NReg;
	B.BlocByTex=SPG_TypeAlloc(NReg,PGLBloc*,"Tableau de pointeurs de PGLBloc");
	B.Texture=SPG_TypeAlloc(NReg,PGLTexture*,"Tableau de pointeurs de PGLTexture");
	if(BlocLinke)
	{
		if(BlocIndexe==0) 
			B.OpenGLpnt=SPG_TypeAlloc(NReg,SG_PNT3D*,"Tableau de Tableaux de vertices");
		else
			B.OpenGLindex=SPG_TypeAlloc(NReg,SG_INDEX*,"Tableau de Tableaux d'index");
		B.OpenGLcolor=SPG_TypeAlloc(NReg,PGLColor*,"Tableau de Tableaux de couleurs");
		B.OpenGLTexCoord=SPG_TypeAlloc(NReg,PGLTexCoord*,"Tableau de Tableaux de TexCoord");
	}
	
	for(int r=0;r<NReg;r++)
	{
		int NombreF=NFacesByTex[r];
		
		if(BlocLinke)
		{
			B.BlocByTex[r]=pglCreateLinkedBloc(PGL_FaceType);
		}
		else
		{
			B.BlocByTex[r]=pglCreateBloc(PGL_FaceType);
		}
		B.Texture[r]=ValTex[r];
		
		SG_PNT3D* OpenGLpnt=0;
		SG_INDEX* OpenGLindex=0;
		PGLColor* OpenGLcolor=0;
		PGLTexCoord* OpenGLTexCoord=0;
		
		if(BlocIndexe==0) 
			OpenGLpnt=SPG_TypeAlloc(IncVertex*NombreF, SG_PNT3D, "Points OpenGL");
		else
			OpenGLindex=SPG_TypeAlloc(IncVertex*NombreF, SG_INDEX, "Index OpenGL");
		
		if(BlocIndexe==0) 
		{
			OpenGLcolor=SPG_TypeAlloc(IncVertex*NombreF, PGLColor, "Couleurs OpenGL");
			if (B.Texture[r]) OpenGLTexCoord=SPG_TypeAlloc(IncVertex*NombreF,PGLTexCoord,"TexCoord OpenGL");
		}
		else
		{//c'est non optimal puisque par texture on n'accede pas a tous les points
			OpenGLcolor=SPG_TypeAlloc(B.DB.NombreP, PGLColor, "Couleurs OpenGL");
			if (B.Texture[r]) OpenGLTexCoord=SPG_TypeAlloc(B.DB.NombreP,PGLTexCoord,"TexCoord OpenGL");
		}
		
		
		
		int CurrentFace=0;
		
		for(int i=0;i<B.DB.NombreF;i++)
		{
			if(ValTex[r]==SG_GetFceTex(B.DB.MemFaces[i]))
			{
				CHECK(CurrentFace>NombreF,"SG_PGLDispatchByTex: Erreur de nombre de faces",break);
				if(PGL_FaceType==PGL_QUADS)
				{
					if(BlocIndexe==0)
					{
						SG_PGLCreateQuad(OpenGLpnt+4*CurrentFace,OpenGLcolor+4*CurrentFace,OpenGLTexCoord+4*CurrentFace,B.Texture[r],B.DB.MemFaces[i]);
					}
					else
					{
						SG_PGLCreateIQuad(OpenGLindex+4*CurrentFace,OpenGLcolor,OpenGLTexCoord,B.Texture[r],B.DB.MemFaces[i],B.DB.MemPoints);
					}
				}
				else
				{
	PixCoul CTEMP;
	CTEMP=B.DB.MemFaces[i].Couleur;
	V_Swap(BYTE,CTEMP.R,CTEMP.B);
					
					//a faire: couper sur la plus petite diagonale
					if(BlocIndexe==0)
					{
						OpenGLpnt[6*CurrentFace  ]=*(B.DB.MemFaces[i].NumP1);
						OpenGLpnt[6*CurrentFace+1]=*(B.DB.MemFaces[i].NumP2);
						OpenGLpnt[6*CurrentFace+2]=*(B.DB.MemFaces[i].NumP3);
						OpenGLpnt[6*CurrentFace+3]=*(B.DB.MemFaces[i].NumP2);
						OpenGLpnt[6*CurrentFace+4]=*(B.DB.MemFaces[i].NumP3);
						OpenGLpnt[6*CurrentFace+5]=*(B.DB.MemFaces[i].NumP4);
						OpenGLcolor[6*CurrentFace+5].c=
							OpenGLcolor[6*CurrentFace+4].c=
							OpenGLcolor[6*CurrentFace+3].c=
							OpenGLcolor[6*CurrentFace+2].c=
							OpenGLcolor[6*CurrentFace+1].c=
							OpenGLcolor[6*CurrentFace].c=
							CTEMP.Coul;
						if(OpenGLTexCoord)
						{
							OpenGLTexCoord[6*CurrentFace  ].u=((float)B.DB.MemFaces[i].XT1/(B.Texture[r]->width));
							OpenGLTexCoord[6*CurrentFace  ].v=((float)B.DB.MemFaces[i].YT1/(B.Texture[r]->height));
							OpenGLTexCoord[6*CurrentFace+1].u=((float)B.DB.MemFaces[i].XT2/(B.Texture[r]->width));
							OpenGLTexCoord[6*CurrentFace+1].v=((float)B.DB.MemFaces[i].YT2/(B.Texture[r]->height));
							OpenGLTexCoord[6*CurrentFace+2].u=((float)B.DB.MemFaces[i].XT3/(B.Texture[r]->width));
							OpenGLTexCoord[6*CurrentFace+2].v=((float)B.DB.MemFaces[i].YT3/(B.Texture[r]->height));
							OpenGLTexCoord[6*CurrentFace+3].u=((float)B.DB.MemFaces[i].XT2/(B.Texture[r]->width));
							OpenGLTexCoord[6*CurrentFace+3].v=((float)B.DB.MemFaces[i].YT2/(B.Texture[r]->height));
							OpenGLTexCoord[6*CurrentFace+4].u=((float)B.DB.MemFaces[i].XT3/(B.Texture[r]->width));
							OpenGLTexCoord[6*CurrentFace+4].v=((float)B.DB.MemFaces[i].YT3/(B.Texture[r]->height));
							OpenGLTexCoord[6*CurrentFace+5].u=((float)B.DB.MemFaces[i].XT4/(B.Texture[r]->width));
							OpenGLTexCoord[6*CurrentFace+5].v=((float)B.DB.MemFaces[i].YT4/(B.Texture[r]->height));
						}
					}
					else
					{
						OpenGLindex[6*CurrentFace  ]=(B.DB.MemFaces[i].NumP1-B.DB.MemPoints);//sizeof(SG_PNT3D);
						OpenGLindex[6*CurrentFace+1]=(B.DB.MemFaces[i].NumP2-B.DB.MemPoints);//sizeof(SG_PNT3D);
						OpenGLindex[6*CurrentFace+2]=(B.DB.MemFaces[i].NumP3-B.DB.MemPoints);//sizeof(SG_PNT3D);
						OpenGLindex[6*CurrentFace+3]=(B.DB.MemFaces[i].NumP2-B.DB.MemPoints);//sizeof(SG_PNT3D);
						OpenGLindex[6*CurrentFace+4]=(B.DB.MemFaces[i].NumP3-B.DB.MemPoints);//sizeof(SG_PNT3D);
						OpenGLindex[6*CurrentFace+5]=(B.DB.MemFaces[i].NumP4-B.DB.MemPoints);//sizeof(SG_PNT3D);
						OpenGLcolor[OpenGLindex[6*CurrentFace+5]].c=
							OpenGLcolor[OpenGLindex[6*CurrentFace+4]].c=
							OpenGLcolor[OpenGLindex[6*CurrentFace+3]].c=
							OpenGLcolor[OpenGLindex[6*CurrentFace+2]].c=
							OpenGLcolor[OpenGLindex[6*CurrentFace+1]].c=
							OpenGLcolor[OpenGLindex[6*CurrentFace  ]].c=
							CTEMP.Coul;
						if(OpenGLTexCoord)
						{
							OpenGLTexCoord[OpenGLindex[6*CurrentFace  ]].u=((float)B.DB.MemFaces[i].XT1/(B.Texture[r]->width));
							OpenGLTexCoord[OpenGLindex[6*CurrentFace  ]].v=((float)B.DB.MemFaces[i].YT1/(B.Texture[r]->height));
							OpenGLTexCoord[OpenGLindex[6*CurrentFace+1]].u=((float)B.DB.MemFaces[i].XT2/(B.Texture[r]->width));
							OpenGLTexCoord[OpenGLindex[6*CurrentFace+1]].v=((float)B.DB.MemFaces[i].YT2/(B.Texture[r]->height));
							OpenGLTexCoord[OpenGLindex[6*CurrentFace+2]].u=((float)B.DB.MemFaces[i].XT3/(B.Texture[r]->width));
							OpenGLTexCoord[OpenGLindex[6*CurrentFace+2]].v=((float)B.DB.MemFaces[i].YT3/(B.Texture[r]->height));
							OpenGLTexCoord[OpenGLindex[6*CurrentFace+3]].u=((float)B.DB.MemFaces[i].XT2/(B.Texture[r]->width));
							OpenGLTexCoord[OpenGLindex[6*CurrentFace+3]].v=((float)B.DB.MemFaces[i].YT2/(B.Texture[r]->height));
							OpenGLTexCoord[OpenGLindex[6*CurrentFace+4]].u=((float)B.DB.MemFaces[i].XT3/(B.Texture[r]->width));
							OpenGLTexCoord[OpenGLindex[6*CurrentFace+4]].v=((float)B.DB.MemFaces[i].YT3/(B.Texture[r]->height));
							OpenGLTexCoord[OpenGLindex[6*CurrentFace+5]].u=((float)B.DB.MemFaces[i].XT4/(B.Texture[r]->width));
							OpenGLTexCoord[OpenGLindex[6*CurrentFace+5]].v=((float)B.DB.MemFaces[i].YT4/(B.Texture[r]->height));
						}
					}
				}
				
				SPG_MemFastCheck();
				CurrentFace++;
			}//fin du test de texture

		}//fin du for i

		if(BlocIndexe==0)
		{
			pglBlocVertexPointer(B.BlocByTex[r],IncVertex*NombreF,(float*)OpenGLpnt,0);
		}
		else
		{
			//DbgCHECK(B.BlocByTex[r]->linked==0,"SG_PGLDispatchByTex: Le bloc n'est pas linked");
			pglBlocVertexPointer(B.BlocByTex[r],B.DB.NombreP,(float*)B.DB.MemPoints,sizeof(SG_PNT3D));
			pglBlocIndexPointer(B.BlocByTex[r],IncVertex*NombreF,(SG_INDEX*)OpenGLindex);
			//DbgCHECK(D.Objet.BlocByTex->Vertices!=D.Objet.DB.MemFaces,"SG_PGLDispatchByTex: Le bloc possede\nen fait une copie\nlocale des donnees");
		}

		if (OpenGLTexCoord)
		{
			pglBlocTexCoordPointer(B.BlocByTex[r],IncVertex*NombreF,(float*)OpenGLTexCoord,sizeof(PGLTexCoord));
			pglBlocTexture(B.BlocByTex[r],B.Texture[r]);
			pglBlocModulate(B.BlocByTex[r],ufalse);
		}
		else
		{
			pglBlocColorPointer(B.BlocByTex[r],IncVertex*NombreF,(PGLColor*)OpenGLcolor,0);
			pglBlocBlend(B.BlocByTex[r],PGLBLEND_NONE);
		}
		if(BlocLinke)
		{
			//attention si le bloc est indexe OpenGLpnt=0
			//car on a passe directement B.DB.MemFaces
			if(B.OpenGLpnt) B.OpenGLpnt[r]=OpenGLpnt;
			//attention si le bloc n'est pas indexe OpenGLindex=0
			if(B.OpenGLindex) B.OpenGLindex[r]=OpenGLindex;
			if(B.OpenGLcolor) B.OpenGLcolor[r]=OpenGLcolor;
			if(B.OpenGLTexCoord) B.OpenGLTexCoord[r]=OpenGLTexCoord;
		}
		else
		{
			SPG_MemFastCheck();
			if (OpenGLpnt) SPG_MemFree(OpenGLpnt);
			if (OpenGLindex) SPG_MemFree(OpenGLindex);
			if (OpenGLcolor) SPG_MemFree(OpenGLcolor);
			if (OpenGLTexCoord)	SPG_MemFree(OpenGLTexCoord);
		}
	//pglBlocTriangles(B.BlocByTex[r]);
	}//fin du for r
	return -1;
}

void SPG_CONV SG_PGLCloseDispatch(SG_FullBloc& Bloc)
{
	if (Bloc.BlocByTex)
	{
		for(int nt=0;nt<Bloc.NombreTex;nt++)
		{
			pglDestroyBloc(Bloc.BlocByTex[nt]);
		}
		SPG_MemFree(Bloc.BlocByTex);
		Bloc.BlocByTex=0;
	}
	
	if(Bloc.OpenGLpnt)
	{
		for(int nt=0;nt<Bloc.NombreTex;nt++)
		{
			if (Bloc.OpenGLpnt[nt]) 
			{
				SPG_MemFree(Bloc.OpenGLpnt[nt]);
				Bloc.OpenGLpnt[nt]=0;
			}
		}
		SPG_MemFree(Bloc.OpenGLpnt);
		Bloc.OpenGLpnt=0;
	}
	if(Bloc.OpenGLindex)
	{
		for(int nt=0;nt<Bloc.NombreTex;nt++)
		{
			if (Bloc.OpenGLindex[nt]) 
			{
				SPG_MemFree(Bloc.OpenGLindex[nt]);
				Bloc.OpenGLindex[nt]=0;
			}
		}
		SPG_MemFree(Bloc.OpenGLindex);
		Bloc.OpenGLindex=0;
	}
	if(Bloc.OpenGLcolor)
	{
		for(int nt=0;nt<Bloc.NombreTex;nt++)
		{
			if (Bloc.OpenGLcolor[nt]) 
			{
				SPG_MemFree(Bloc.OpenGLcolor[nt]);
				Bloc.OpenGLcolor[nt]=0;
			}
		}
		SPG_MemFree(Bloc.OpenGLcolor);
		Bloc.OpenGLcolor=0;
	}
	if(Bloc.OpenGLTexCoord)
	{
		for(int nt=0;nt<Bloc.NombreTex;nt++)
		{
			if (Bloc.OpenGLTexCoord[nt]) 
			{
				SPG_MemFree(Bloc.OpenGLTexCoord[nt]);
				Bloc.OpenGLTexCoord[nt]=0;
			}
		}
		SPG_MemFree(Bloc.OpenGLTexCoord);
		Bloc.OpenGLTexCoord=0;
	}
	
	Bloc.NombreTex=0;
	
	if(Bloc.Texture)
	{
		SPG_MemFree(Bloc.Texture);
		Bloc.Texture=0;
	}
	return;
}

#endif
#endif

