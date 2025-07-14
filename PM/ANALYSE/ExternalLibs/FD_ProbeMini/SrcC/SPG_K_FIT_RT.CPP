
#include "SPG_General.h"

#if defined(SPG_General_USEPARAFIT)||defined(SPG_General_USEWEIGHTEDPARAFIT)

#include "SPG_Includes.h"

#include <memory.h>

#define AvanceLeftPtr(P1,L1,P2,L2,av) {if(av<L1) {P1+=av;L1-=av;} else {int av2=av-L1; P1=P2+av2; L1=L2-av2; P2=0; L2=0;}}
#define ReculeRightPtr(P1,L1,P2,L2,av) {if(av<L2) {L2-=av;} else {int av2=av-L2; L1=L1-av2; P2=0; L2=0;}}

#define S2A(D,R) R._a+=D[i]*E->_a
#define S2(D,R) R._c+=D[i]*E->_c; R._b+=D[i]*E->_b; R._a+=D[i]*E->_a
#define SINV2(D,R) {kfloat iD=(kfloat)1/(D[i]-Vinf); R._c+=iD*E->_c; R._b+=iD*E->_b; R._a+=iD*E->_a;}
#define SSQR2(D,R) {kfloat iD=(kfloat)1/(kfloat)sqrtf(D[i]-Vinf); R._c+=iD*E->_c; R._b+=iD*E->_b; R._a+=iD*E->_a;}
#define S3(D,R) R._d+=D[i]*E->_d; R._c+=D[i]*E->_c; R._b+=D[i]*E->_b; R._a+=D[i]*E->_a
#define S4(D,R) R._e+=D[i]*E->_e; R._d+=D[i]*E->_d; R._c+=D[i]*E->_c; R._b+=D[i]*E->_b; R._a+=D[i]*E->_a
#define SINV4(D,R) {kfloat iD=(kfloat)1/(D[i]-Vinf); R._e+=iD*E->_e; R._d+=iD*E->_d; R._c+=iD*E->_c; R._b+=iD*E->_b; R._a+=iD*E->_a;}
#define SSQR4(D,R) {kfloat iD=(kfloat)1/(kfloat)sqrtf(D[i]-Vinf); R._e+=iD*E->_e; R._d+=iD*E->_d; R._c+=iD*E->_c; R._b+=iD*E->_b; R._a+=iD*E->_a;}

#define SDMono(S,R) int i; for(i=0;i<Len;i++) { S(D,R); E++; }
#define SD(S,R) int i; for(i=0;i<Len1;i+=Step) { S(D1,R); E++; } i-=Len1; for(;i<Len2;i+=Step) { S(D2,R); E++; }
#define SV(S,R) int i; int id=Step*Divisor; for(i=0;i<Len1;i+=Step) { if(i==id){id+=Step*Divisor; E++;}; S(D1,R); } i-=Len1; id-=Len1; for(;i<Len2;i+=Step) { if(i==id){id+=Step*Divisor; E++;} S(D2,R); }


void SPG_CONV K_FIT2_Short(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; CHECK(Len>K.Len,"K_FIT2_Short",return); K_FIT2_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT2_Short",return);
	SD(S2,R);
	return;
}

void SPG_CONV K_FIT2_Short_Divided(short int* D1, int Len1, short int* D2, int Len2, int Step, int Divisor, K_FIT2_ELT& R, K_FIT2& K)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/(Divisor*Step); CHECK(Len>K.Len,"K_FIT2_Short",return); K_FIT2_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT2_Short_Divided",return);
	//DbgCHECK((Len1+Len2)%(Divisor*Step),"K_FIT2_Short_Divided");
	SV(S2,R);
	return;
}

void SPG_CONV K_FIT2_Short_Unlimited(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; 
	if(Len<=K.Len) 
	{
		K_FIT2_Short(D1,Len1,D2,Len2,Step,R,K);
	}
	else
	{
		int Divisor=(Len+K.Len-1)/(K.Len);
		if((Divisor&1)==0) Divisor+=1;//diviseur impair pour que l'on puisse facilement trouver un reste pair (divisible en deux pour centrer l'intervalle de fit)

		int QLen=Len/Divisor;

		int Reste=Len-QLen*Divisor;
		if(Reste&1)// si le reste est impair
		{
			QLen--;//on prend un groupe de moins pour que le reste devienne pair (car si Len-QLen*Diviseur est pair Len-(QLen-1)*Diviseur est impair
			Reste+=Divisor;//impair+impair=pair
		}
		int nSkip=(Reste/2)*Step;
		AvanceLeftPtr(D1,Len1,D2,Len2,nSkip);
		ReculeRightPtr(D1,Len1,D2,Len2,nSkip);

		//fit en x'
		K_FIT2_Short_Divided(D1,Len1,D2,Len2,Step,Divisor,R,K);

		//changement de variable
		kfloat iDiv=1/((kfloat)Divisor);//car on somme diviseur fois chaque coefficient
		R._c*=iDiv; R._b*=iDiv*iDiv; R._a*=iDiv*iDiv*iDiv;
		R._c-=R._a*(Divisor*Divisor-1)/12.0;
	}
	return;
}


void SPG_CONV K_FIT2_FloatMonoA(float* D, int Len, K_FIT2_ELT& R, K_FIT2& K)
{
	SPG_ZeroStruct(R); CHECK(Len>K.Len,"K_FIT2_Float",return); K_FIT2_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT2_FloatMonoA",return);
	SDMono(S2A,R);
	return;
}

void SPG_CONV K_FIT2_FloatMono(float* D, int Len, K_FIT2_ELT& R, K_FIT2& K)
{
	SPG_ZeroStruct(R); CHECK(Len>K.Len,"K_FIT2_Float",return); K_FIT2_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT2_FloatMono",return);;
	SDMono(S2,R);
	return;
}

void SPG_CONV K_FIT2_Float(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; CHECK(Len>K.Len,"K_FIT2_Float",return); K_FIT2_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT2_Float",return);;
	SD(S2,R);
	return;
}

void SPG_CONV K_FIT2_Float_Divided(float* D1, int Len1, float* D2, int Len2, int Step, int Divisor, K_FIT2_ELT& R, K_FIT2& K)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/(Divisor*Step); CHECK(Len>K.Len,"K_FIT2_Float",return); K_FIT2_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT2_Float_Divided",return);;
	//DbgCHECK((Len1+Len2)%(Divisor*Step),"K_FIT2_Float_Divided");
	SV(S2,R);
	return;
}

void SPG_CONV K_FIT2_Float_Unlimited(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; 
	if(Len<=K.Len) 
	{
		K_FIT2_Float(D1,Len1,D2,Len2,Step,R,K);
	}
	else
	{
		int Divisor=(Len+K.Len-1)/(K.Len);
		if((Divisor&1)==0) Divisor+=1;//diviseur impair pour que l'on puisse facilement trouver un reste pair (divisible en deux pour centrer l'intervalle de fit)

		int QLen=Len/Divisor;

		int Reste=Len-QLen*Divisor;
		if(Reste&1)// si le reste est impair
		{
			QLen--;//on prend un groupe de moins pour que le reste devienne pair (car si Len-QLen*Diviseur est pair Len-(QLen-1)*Diviseur est impair
			Reste+=Divisor;//impair+impair=pair
		}
		int nSkip=(Reste/2)*Step;
		AvanceLeftPtr(D1,Len1,D2,Len2,nSkip);
		ReculeRightPtr(D1,Len1,D2,Len2,nSkip);

		//fit en x'
		K_FIT2_Float_Divided(D1,Len1,D2,Len2,Step,Divisor,R,K);

		//changement de variable
		kfloat iDiv=1/((kfloat)Divisor);//car on somme diviseur fois chaque coefficient
		R._c*=iDiv; R._b*=iDiv*iDiv; R._a*=iDiv*iDiv*iDiv;
		R._c-=R._a*(Divisor*Divisor-1)/12.0;
	}
	return;
}

void SPG_CONV K_FIT2_INVShort(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K, kfloat Vinf)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; CHECK(Len>K.Len,"K_FIT2_Short",return); K_FIT2_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT2_INVShort",return);;
	SD(SINV2,R);
	return;
}

void SPG_CONV K_FIT2_INVShort_Divided(short int* D1, int Len1, short int* D2, int Len2, int Step, int Divisor, K_FIT2_ELT& R, K_FIT2& K, kfloat Vinf)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/(Divisor*Step); CHECK(Len>K.Len,"K_FIT2_Short",return); K_FIT2_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT2_INVShort_Divided",return);
	//DbgCHECK((Len1+Len2)%(Divisor*Step),"K_FIT2_Short_Divided");
	SV(SINV2,R);
	return;
}

void SPG_CONV K_FIT2_INVShort_Unlimited(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K, kfloat Vinf)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; 
	if(Len<=K.Len) 
	{
		K_FIT2_INVShort(D1,Len1,D2,Len2,Step,R,K,Vinf);
	}
	else
	{
		int Divisor=(Len+K.Len-1)/(K.Len);
		if((Divisor&1)==0) Divisor+=1;//diviseur impair pour que l'on puisse facilement trouver un reste pair (divisible en deux pour centrer l'intervalle de fit)

		int QLen=Len/Divisor;

		int Reste=Len-QLen*Divisor;
		if(Reste&1)// si le reste est impair
		{
			QLen--;//on prend un groupe de moins pour que le reste devienne pair (car si Len-QLen*Diviseur est pair Len-(QLen-1)*Diviseur est impair
			Reste+=Divisor;//impair+impair=pair
		}
		int nSkip=(Reste/2)*Step;
		AvanceLeftPtr(D1,Len1,D2,Len2,nSkip);
		ReculeRightPtr(D1,Len1,D2,Len2,nSkip);

		//fit en x'
		K_FIT2_INVShort_Divided(D1,Len1,D2,Len2,Step,Divisor,R,K,Vinf);

		//changement de variable
		kfloat iDiv=1/((kfloat)Divisor);//car on somme diviseur fois chaque coefficient
		R._c*=iDiv; R._b*=iDiv*iDiv; R._a*=iDiv*iDiv*iDiv;
		R._c-=R._a*(Divisor*Divisor-1)/12.0;
	}
	return;
}

void SPG_CONV K_FIT2_SQRShort(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K, kfloat Vinf)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; CHECK(Len>K.Len,"K_FIT2_Short",return); K_FIT2_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT2_SQRShort",return);;
	SD(SSQR2,R);
	return;
}

void SPG_CONV K_FIT2_SQRShort_Divided(short int* D1, int Len1, short int* D2, int Len2, int Step, int Divisor, K_FIT2_ELT& R, K_FIT2& K, kfloat Vinf)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/(Divisor*Step); CHECK(Len>K.Len,"K_FIT2_Short",return); K_FIT2_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT2_SQRShort_Divided",return);
	//DbgCHECK((Len1+Len2)%(Divisor*Step),"K_FIT2_Short_Divided");
	SV(SSQR2,R);
	return;
}

void SPG_CONV K_FIT2_SQRShort_Unlimited(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K, kfloat Vinf)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; 
	if(Len<=K.Len) 
	{
		K_FIT2_SQRShort(D1,Len1,D2,Len2,Step,R,K,Vinf);
	}
	else
	{
		int Divisor=(Len+K.Len-1)/(K.Len);
		if((Divisor&1)==0) Divisor+=1;//diviseur impair pour que l'on puisse facilement trouver un reste pair (divisible en deux pour centrer l'intervalle de fit)

		int QLen=Len/Divisor;

		int Reste=Len-QLen*Divisor;
		if(Reste&1)// si le reste est impair
		{
			QLen--;//on prend un groupe de moins pour que le reste devienne pair (car si Len-QLen*Diviseur est pair Len-(QLen-1)*Diviseur est impair
			Reste+=Divisor;//impair+impair=pair
		}
		int nSkip=(Reste/2)*Step;
		AvanceLeftPtr(D1,Len1,D2,Len2,nSkip);
		ReculeRightPtr(D1,Len1,D2,Len2,nSkip);

		//fit en x'
		K_FIT2_SQRShort_Divided(D1,Len1,D2,Len2,Step,Divisor,R,K,Vinf);

		//changement de variable
		kfloat iDiv=1/((kfloat)Divisor);//car on somme diviseur fois chaque coefficient
		R._c*=iDiv; R._b*=iDiv*iDiv; R._a*=iDiv*iDiv*iDiv;
		R._c-=R._a*(Divisor*Divisor-1)/12.0;
	}
	return;
}

void SPG_CONV K_FIT2_INVFloat(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K, float Vinf)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; CHECK(Len>K.Len,"K_FIT2_Float",return); K_FIT2_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT2_INVFloat",return);
	SD(SINV2,R);
	return;
}

void SPG_CONV K_FIT2_INVFloat_Divided(float* D1, int Len1, float* D2, int Len2, int Step, int Divisor, K_FIT2_ELT& R, K_FIT2& K, float Vinf)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/(Divisor*Step); CHECK(Len>K.Len,"K_FIT2_Float",return); K_FIT2_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT2_INVFloat_Divided",return);
	//DbgCHECK((Len1+Len2)%(Divisor*Step),"K_FIT2_Float_Divided");
	SV(SINV2,R);
	return;
}

void SPG_CONV K_FIT2_INVFloat_Unlimited(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K, float Vinf)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; 
	if(Len<=K.Len) 
	{
		K_FIT2_INVFloat(D1,Len1,D2,Len2,Step,R,K,Vinf);
	}
	else
	{
		int Divisor=(Len+K.Len-1)/(K.Len);
		if((Divisor&1)==0) Divisor+=1;//diviseur impair pour que l'on puisse facilement trouver un reste pair (divisible en deux pour centrer l'intervalle de fit)

		int QLen=Len/Divisor;

		int Reste=Len-QLen*Divisor;
		if(Reste&1)// si le reste est impair
		{
			QLen--;//on prend un groupe de moins pour que le reste devienne pair (car si Len-QLen*Diviseur est pair Len-(QLen-1)*Diviseur est impair
			Reste+=Divisor;//impair+impair=pair
		}
		int nSkip=(Reste/2)*Step;
		AvanceLeftPtr(D1,Len1,D2,Len2,nSkip);
		ReculeRightPtr(D1,Len1,D2,Len2,nSkip);

		//fit en x'
		K_FIT2_INVFloat_Divided(D1,Len1,D2,Len2,Step,Divisor,R,K,Vinf);

		//changement de variable
		kfloat iDiv=1/((kfloat)Divisor);//car on somme diviseur fois chaque coefficient
		R._c*=iDiv; R._b*=iDiv*iDiv; R._a*=iDiv*iDiv*iDiv;
		R._c-=R._a*(Divisor*Divisor-1)/12.0;
	}
	return;
}

void SPG_CONV K_FIT2_Double(double* D1, int Len1, double* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; CHECK(Len>K.Len,"K_FIT2_Double",return); K_FIT2_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT2_Double",return);
	SD(S2,R);
	return;
}

/*
void SPG_CONV K_FIT2C_Short(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K)
{//calcule seulement la hauteur de la parabole des moindres carrés au centre de l'intervalle du fit
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; CHECK(Len>K.Len,"K_FIT2C_Short",return); K_FIT2_ELT* E = K.e[Len]; if(E==0) return;
	int i;
	for(i=0;i<Len1;i+=Step) { R._c+=D1[i]*E->_c; E++; }
	i-=Len1;
	for(;i<Len2;i+=Step) { R._c+=D2[i]*E->_c; E++; }
	return;
}

void SPG_CONV K_FIT2C_Float(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; CHECK(Len>K.Len,"K_FIT2C_Float",return); K_FIT2_ELT* E = K.e[Len]; if(E==0) return;
	int i;
	for(i=0;i<Len1;i+=Step) { R._c+=D1[i]*E->_c; E++; }
	i-=Len1;
	for(;i<Len2;i+=Step) { R._c+=D2[i]*E->_c; E++; }
	return;
}
*/

void SPG_CONV K_FIT3_Short(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT3_ELT& R, K_FIT3& K)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; CHECK(Len>K.Len,"K_FIT3_Short",return); K_FIT3_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT3_Short",return);
	SD(S3,R);
	return;
}

void SPG_CONV K_FIT3_Float(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT3_ELT& R, K_FIT3& K)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; CHECK(Len>K.Len,"K_FIT3_Float",return); K_FIT3_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT3_Float",return);
	SD(S3,R);
	return;
}

void SPG_CONV K_FIT4_Short(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; CHECK(Len>K.Len,"K_FIT4_Short",return); K_FIT4_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT4_Short",return);
	SD(S4,R);
	return;
}

void SPG_CONV K_FIT4_Short_Divided(short int* D1, int Len1, short int* D2, int Len2, int Step, int Divisor, K_FIT4_ELT& R, K_FIT4& K)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/(Divisor*Step); CHECK(Len>K.Len,"K_FIT4_Short",return); K_FIT4_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT4_Short_Divided",return);
	DbgCHECK((Len1+Len2)%(Divisor*Step),"K_FIT4_Short_Divided");
	SV(S4,R);
	return;
}

void SPG_CONV K_FIT4_Short_Unlimited(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; 
	if(Len<=K.Len) 
	{
		K_FIT4_Short(D1,Len1,D2,Len2,Step,R,K);
	}
	else
	{
		int Divisor=(Len+K.Len-1)/(K.Len);
		if((Divisor&1)==0) Divisor+=1;//diviseur impair pour que l'on puisse facilement trouver un reste pair (divisible en deux pour centrer l'intervalle de fit)

		int QLen=Len/Divisor;//longueur du fit (nombre de bins)

		int Reste=Len-QLen*Divisor;
		if(Reste&1)// si le reste est impair
		{
			QLen--;//on prend un groupe de moins pour que le reste devienne pair (car si Len-QLen*Diviseur est pair Len-(QLen-1)*Diviseur est impair
			Reste+=Divisor;//impair+impair=pair
		}
		int nSkip=(Reste/2)*Step;
		AvanceLeftPtr(D1,Len1,D2,Len2,nSkip);
		ReculeRightPtr(D1,Len1,D2,Len2,nSkip);

		//fit en x'
		K_FIT4_Short_Divided(D1,Len1,D2,Len2,Step,Divisor,R,K);

		//changement de variable
		kfloat iDiv=1/((kfloat)Divisor);//car on somme diviseur fois chaque coefficient
		R._e*=iDiv; R._d*=iDiv*iDiv; R._c*=iDiv*iDiv*iDiv; R._b*=iDiv*iDiv*iDiv*iDiv; R._a*=iDiv*iDiv*iDiv*iDiv*iDiv;
		R._c-=R._a*(Divisor*Divisor-1)/2.0;
		R._d-=R._b*(Divisor*Divisor-1)/4.0;
		R._e-=R._a*(Divisor*Divisor-1)*(Divisor*Divisor-1)/80.0 + R._c*(Divisor*Divisor-1)/12.0;
	}
	return;
}

void SPG_CONV K_FIT4_Float(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; CHECK(Len>K.Len,"K_FIT4_Float",return); K_FIT4_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT4_Float",return);
	SD(S4,R);
	return;
}

void SPG_CONV K_FIT4_Float_Divided(float* D1, int Len1, float* D2, int Len2, int Step, int Divisor, K_FIT4_ELT& R, K_FIT4& K)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/(Divisor*Step); CHECK(Len>K.Len,"K_FIT4_Float",return); K_FIT4_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT4_Float_Divided",return);
	//DbgCHECK((Len1+Len2)%(Divisor*Step),"K_FIT4_Float_Divided");
	SV(S4,R);
	return;
}

void SPG_CONV K_FIT4_Float_Unlimited(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; 
	if(Len<=K.Len) 
	{
		K_FIT4_Float(D1,Len1,D2,Len2,Step,R,K);
	}
	else
	{
		int Divisor=(Len+K.Len-1)/(K.Len);
		if((Divisor&1)==0) Divisor+=1;//diviseur impair pour que l'on puisse facilement trouver un reste pair (divisible en deux pour centrer l'intervalle de fit)

		int QLen=Len/Divisor;//longueur du fit (nombre de bins)

		int Reste=Len-QLen*Divisor;
		if(Reste&1)// si le reste est impair
		{
			QLen--;//on prend un groupe de moins pour que le reste devienne pair (car si Len-QLen*Diviseur est pair Len-(QLen-1)*Diviseur est impair
			Reste+=Divisor;//impair+impair=pair
		}
		int nSkip=(Reste/2)*Step;
		AvanceLeftPtr(D1,Len1,D2,Len2,nSkip);
		ReculeRightPtr(D1,Len1,D2,Len2,nSkip);

		//fit en x'
		K_FIT4_Float_Divided(D1,Len1,D2,Len2,Step,Divisor,R,K);

		//changement de variable
		kfloat iDiv=1/((kfloat)Divisor);//car on somme diviseur fois chaque coefficient
		R._e*=iDiv; R._d*=iDiv*iDiv; R._c*=iDiv*iDiv*iDiv; R._b*=iDiv*iDiv*iDiv*iDiv; R._a*=iDiv*iDiv*iDiv*iDiv*iDiv;
		R._c-=R._a*(Divisor*Divisor-1)/2.0;
		R._d-=R._b*(Divisor*Divisor-1)/4.0;
		R._e-=R._a*(Divisor*Divisor-1)*(Divisor*Divisor-1)/80.0 + R._c*(Divisor*Divisor-1)/12.0;
	}
	return;
}


void SPG_CONV K_FIT4_INVShort(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K, kfloat Vinf)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; CHECK(Len>K.Len,"K_FIT4_Short",return); K_FIT4_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT4_INVShort",return);
	SD(SINV4,R);
	return;
}

void SPG_CONV K_FIT4_INVShort_Divided(short int* D1, int Len1, short int* D2, int Len2, int Step, int Divisor, K_FIT4_ELT& R, K_FIT4& K, kfloat Vinf)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/(Divisor*Step); CHECK(Len>K.Len,"K_FIT4_Short",return); K_FIT4_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT4_INVShort_Divided",return);
	//DbgCHECK((Len1+Len2)%(Divisor*Step),"K_FIT4_Short_Divided");
	SV(SINV4,R);
	return;
}

void SPG_CONV K_FIT4_INVShort_Unlimited(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K, kfloat Vinf)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; 
	if(Len<=K.Len) 
	{
		K_FIT4_INVShort(D1,Len1,D2,Len2,Step,R,K,Vinf);
	}
	else
	{
		int Divisor=(Len+K.Len-1)/(K.Len);
		if((Divisor&1)==0) Divisor+=1;//diviseur impair pour que l'on puisse facilement trouver un reste pair (divisible en deux pour centrer l'intervalle de fit)

		int QLen=Len/Divisor;//longueur du fit (nombre de bins)

		int Reste=Len-QLen*Divisor;
		if(Reste&1)// si le reste est impair
		{
			QLen--;//on prend un groupe de moins pour que le reste devienne pair (car si Len-QLen*Diviseur est pair Len-(QLen-1)*Diviseur est impair
			Reste+=Divisor;//impair+impair=pair
		}
		int nSkip=(Reste/2)*Step;
		AvanceLeftPtr(D1,Len1,D2,Len2,nSkip);
		ReculeRightPtr(D1,Len1,D2,Len2,nSkip);

		//fit en x'
		K_FIT4_INVShort_Divided(D1,Len1,D2,Len2,Step,Divisor,R,K,Vinf);

		//changement de variable
		kfloat iDiv=1/((kfloat)Divisor);//car on somme diviseur fois chaque coefficient
		R._e*=iDiv; R._d*=iDiv*iDiv; R._c*=iDiv*iDiv*iDiv; R._b*=iDiv*iDiv*iDiv*iDiv; R._a*=iDiv*iDiv*iDiv*iDiv*iDiv;
		R._c-=R._a*(Divisor*Divisor-1)/2.0;
		R._d-=R._b*(Divisor*Divisor-1)/4.0;
		R._e-=R._a*(Divisor*Divisor-1)*(Divisor*Divisor-1)/80.0 + R._c*(Divisor*Divisor-1)/12.0;
	}
	return;
}

void SPG_CONV K_FIT4_SQRShort(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K, kfloat Vinf)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; CHECK(Len>K.Len,"K_FIT4_Short",return); K_FIT4_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT4_SQRShort",return);
	SD(SSQR4,R);
	return;
}

void SPG_CONV K_FIT4_SQRShort_Divided(short int* D1, int Len1, short int* D2, int Len2, int Step, int Divisor, K_FIT4_ELT& R, K_FIT4& K, kfloat Vinf)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/(Divisor*Step); CHECK(Len>K.Len,"K_FIT4_Short",return); K_FIT4_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT4_SQRShort_Divided",return);
	//DbgCHECK((Len1+Len2)%(Divisor*Step),"K_FIT4_Short_Divided");
	SV(SSQR4,R);
	return;
}

void SPG_CONV K_FIT4_SQRShort_Unlimited(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K, kfloat Vinf)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; 
	if(Len<=K.Len) 
	{
		K_FIT4_SQRShort(D1,Len1,D2,Len2,Step,R,K,Vinf);
	}
	else
	{
		int Divisor=(Len+K.Len-1)/(K.Len);
		if((Divisor&1)==0) Divisor+=1;//diviseur impair pour que l'on puisse facilement trouver un reste pair (divisible en deux pour centrer l'intervalle de fit)

		int QLen=Len/Divisor;//longueur du fit (nombre de bins)

		int Reste=Len-QLen*Divisor;
		if(Reste&1)// si le reste est impair
		{
			QLen--;//on prend un groupe de moins pour que le reste devienne pair (car si Len-QLen*Diviseur est pair Len-(QLen-1)*Diviseur est impair
			Reste+=Divisor;//impair+impair=pair
		}
		int nSkip=(Reste/2)*Step;
		AvanceLeftPtr(D1,Len1,D2,Len2,nSkip);
		ReculeRightPtr(D1,Len1,D2,Len2,nSkip);

		//fit en x'
		K_FIT4_SQRShort_Divided(D1,Len1,D2,Len2,Step,Divisor,R,K,Vinf);

		//changement de variable
		kfloat iDiv=1/((kfloat)Divisor);//car on somme diviseur fois chaque coefficient
		R._e*=iDiv; R._d*=iDiv*iDiv; R._c*=iDiv*iDiv*iDiv; R._b*=iDiv*iDiv*iDiv*iDiv; R._a*=iDiv*iDiv*iDiv*iDiv*iDiv;
		R._c-=R._a*(Divisor*Divisor-1)/2.0;
		R._d-=R._b*(Divisor*Divisor-1)/4.0;
		R._e-=R._a*(Divisor*Divisor-1)*(Divisor*Divisor-1)/80.0 + R._c*(Divisor*Divisor-1)/12.0;
	}
	return;
}

void SPG_CONV K_FIT4_INVFloat(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K, float Vinf)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; CHECK(Len>K.Len,"K_FIT4_Float",return); K_FIT4_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT4_INVFloat",return);
	SD(SINV4,R);
	return;
}

void SPG_CONV K_FIT4_INVFloat_Divided(float* D1, int Len1, float* D2, int Len2, int Step, int Divisor, K_FIT4_ELT& R, K_FIT4& K, float Vinf)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/(Divisor*Step); CHECK(Len>K.Len,"K_FIT4_Float",return); K_FIT4_ELT* E = K.e[Len]; CHECK(E==0,"K_FIT4_INVFloat_Divided",return);
	DbgCHECK((Len1+Len2)%(Divisor*Step),"K_FIT4_Float_Divided");
	SV(SINV4,R);
	return;
}

void SPG_CONV K_FIT4_INVFloat_Unlimited(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K, float Vinf)
{
	SPG_ZeroStruct(R); int Len=(Len1+Len2)/Step; 
	if(Len<=K.Len) 
	{
		K_FIT4_INVFloat(D1,Len1,D2,Len2,Step,R,K,Vinf);
	}
	else
	{
		int Divisor=(Len+K.Len-1)/(K.Len);
		if((Divisor&1)==0) Divisor+=1;//diviseur impair pour que l'on puisse facilement trouver un reste pair (divisible en deux pour centrer l'intervalle de fit)

		int QLen=Len/Divisor;//longueur du fit (nombre de bins)

		int Reste=Len-QLen*Divisor;
		if(Reste&1)// si le reste est impair
		{
			QLen--;//on prend un groupe de moins pour que le reste devienne pair (car si Len-QLen*Diviseur est pair Len-(QLen-1)*Diviseur est impair
			Reste+=Divisor;//impair+impair=pair
		}
		int nSkip=(Reste/2)*Step;
		AvanceLeftPtr(D1,Len1,D2,Len2,nSkip);
		ReculeRightPtr(D1,Len1,D2,Len2,nSkip);

		//fit en x'
		K_FIT4_INVFloat_Divided(D1,Len1,D2,Len2,Step,Divisor,R,K,Vinf);

		//changement de variable
		kfloat iDiv=1/((kfloat)Divisor);//car on somme diviseur fois chaque coefficient
		R._e*=iDiv; R._d*=iDiv*iDiv; R._c*=iDiv*iDiv*iDiv; R._b*=iDiv*iDiv*iDiv*iDiv; R._a*=iDiv*iDiv*iDiv*iDiv*iDiv;
		R._c-=R._a*(Divisor*Divisor-1)/2.0;
		R._d-=R._b*(Divisor*Divisor-1)/4.0;
		R._e-=R._a*(Divisor*Divisor-1)*(Divisor*Divisor-1)/80.0 + R._c*(Divisor*Divisor-1)/12.0;
	}
	return;
}
#endif


/*
//import de fichiers de coefficients mathematica

#include "..\SrcC\SPG.h"
#include "..\SrcC\SPG_SysInc.h"

#include <stdio.h>


int SPG_CONV CallBack(DWORD hwnd, DWORD uMsg, DWORD wParam, DWORD lParam)
{
	return 0;
}

#define SKIP(p) {while((*p==' ')||(*p=='\r')||(*p=='\n')) p++;}
#define PASS(p,c,M) {SKIP(p);DbgCHECK(*p!=c,M);p++;SKIP(p);}

int WINAPI WinMain( HINSTANCE hInstance, 
				   // handle to current instance 
				   
				   HINSTANCE hPrevInstance, 
				   // handle to previous instance 
				   
				   LPSTR lpCmdLine, 
				   // pointer to command line 
				   
				   int nCmdShow 
				   // show state of window 
				   
				   )
{
	SPG_WinMainStart((int)hInstance,0,0,0,SPG_SM_UserFriendly,G_ECRAN_DIBSECT,"ProgPrincipal","SrcC",CallBack);

	int NFit=2;
	int Len=128;

#define NAMELEN 1024

	char Filename[NAMELEN];
	sprintf(Filename,"C:\\Sylvain\\Developpement C - Docs and ZIP\\Documentation\\K_FIT%i_%i_%i.txt",NFit,NFit+1,Len);

	if(SPG_GetLoadName(SPG_TXT,Filename,NAMELEN))
	{
		int TxtLen=0;
		BYTE* TxtMem=SPG_LoadFile(Filename,TxtLen,1);
		BYTE* TxtCur=TxtMem;
		sprintf(Filename,"C:\\Sylvain\\Developpement C\\SrcC\\K_FIT%i_%i_%i.cpp",NFit,NFit+1,Len);
		FILE* F=fopen(Filename,"wb+");
		PASS(TxtCur,'{',"Accolade ouvrante recherchée en fichier");
		for(int L=NFit+1;L<=Len;L++)//pour chaque longueur de fit possible
		{
			fprintf(F,"\r\n\r\n\r\n// ##### FIT DEGRE %i LONGUEUR %i #####\r\n",NFit,L);
			PASS(TxtCur,'{',"Accolade ouvrante recherchée coefficients de polynome");
			for(int n=0;n<=NFit;n++)//pour chaque coefficient (a,b,c,...) du polynome
			{
				PASS(TxtCur,'{',"Accolade ouvrante recherchée en liste poids echantillons");

				fprintf(F,"\r\n// ----- Calcul du coefficient %c . x^%i  - somme de %i termes -----\r\n",'a'+NFit-n,n,L);

				for(int i=0;i<L;i++)//pour chaque echantillon du fit de longueur L
				{
					for(int T=0;T<40;T++)
					{
						if((TxtCur[T]=='}')||(TxtCur[T]==','))
						{
							DbgCHECK(T==0,"Nombre attendu");
							if(T)
							{
								BYTE endOfNumber=0;
								V_SWAP(BYTE,endOfNumber,TxtCur[T]);
								char ProgLine[512];
								sprintf(ProgLine,"KF%i[%i].e[%i].a[%i]=%s;\r\n",NFit,L,  i,  n,TxtCur);
								//SPG_List(ProgLine);
								fwrite(ProgLine,strlen(ProgLine),1,F);
								V_SWAP(BYTE,endOfNumber,TxtCur[T]);
								TxtCur+=T;
								break;
							}
						}
					}

					if(i!=L-1)
					{
						PASS(TxtCur,',',"Séparateur recherchée en liste poids echantillons");
					}
				}
				PASS(TxtCur,'}',"Terminateur recherchée en liste poids echantillons");
				if(n!=NFit)
				{
					PASS(TxtCur,',',"Séparateur recherchée coefficients de polynome");
				}
			}
			PASS(TxtCur,'}',"Terminateur recherchée coefficients de polynome");
			if(L!=Len)
			{
				PASS(TxtCur,',',"Séparateur recherchée en fichier");
			}
		}
		PASS(TxtCur,'}',"Terminateur recherchée en fichier");
		SPG_MemFree(TxtMem);
		fclose(F);
	}
	SPG_WinMainClose();
	return 0;
}

//script mathematica
Clear[x, X, XZ, Y, FP, f, MXP, NFIT, NP, i, TC]
TC[NFIT_, NP_] := Module[{XZ, X, MXP, i},
    XZ = Table[1, {i, NP}];
    X = Table[(2*i - 1 - NP)/2, {i, NP}];
    MXP = Table[If[i > 1, X^(i - 1), XZ], {i, NFIT + 1}];
    Inverse[MXP.Transpose[MXP]].MXP]

Table[N[TC[4, i]], {i, 5, 128}] >> "C:\Sylvain\Mathematica\K_FIT4.txt"
(* avant utilisation faire 'remplacer tout' *^ par e *)

*/
