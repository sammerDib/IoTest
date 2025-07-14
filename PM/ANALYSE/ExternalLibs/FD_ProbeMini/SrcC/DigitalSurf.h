

typedef struct{
	char code[12];		// 1
	short format;		// 2
	short nb_obj;		// 3
	short nb_ver;		//4
	short type_etud;	//5 
	char nom_obj[30];	//6
	char nom_oper[30];	//7
	short nu_1;			//8
	short nu_2;			//9
	short nu_3;			//10
	BYTE res_1[12];		//11
	short nb_bit_pt;	//12
	long pt_min;		//13
	long pt_max;		//14
	long nb_pt_ligne;	//15
	long nb_ligne;		//16
	long nb_total_pt;	//17
	float pas_x;		//18
	float pas_y;		//19
	float pas_z;		//20
	char nom_axe_x[16];	//21
	char nom_axe_y[16];	//22
	char nom_axe_z[16];	//23
	char unite_pas_x[16];	//24
	char unite_pas_y[16];	//25
	char unite_pas_z[16];	//26
	char unite_lg_axe_x[16];//27
	char unite_lg_axe_y[16];//28
	char unite_lg_axe_z[16];//29
	float rap_unite_x;		//30
	float rap_unite_y;		//31
	float rap_unite_z;		//32
	short replique;			//33
	short inverse;			//34
	short redresse;			//35
	float nu_4;				//36
	float nu_5;				//37
	float nu_6;				//38
	short secondes;			//39
	short minutes;			//40
	short heures;			//41
	short jour_mois;		//42
	short mois;				//43
	short annee;			//44
	short jour_semaine;		//45
	float duree_mesure;     //46
	short nu_7;				//47
	short nu_8;				//48
	float nu_9;				//49
	short nu_10;			//50
	short lg_zone_comment;	//51
	short lg_zone_privee;    //52
	BYTE  zone_libre_client[128];	//53
	BYTE  nu_11[42];				//54
	
}SURF;


