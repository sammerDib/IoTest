#ifdef INC_SPG_PeakDet2DModeleMatlab_INC
#error SPG_SPG_PeakDet2DModeleMatlab Included twice
#endif
#define	INC_SPG_PeakDet2DModeleMatlab_INC


typedef struct
{
	float xmin;
	float ymin;
	float xmax;
	float ymax;
	float dx;
	int noflip;
	float pixelsize;
	int nx;
	int ny;
} TypCCDBox;

typedef struct
{//parametres sortants
	TypCCDBox ccdbox;
	float* Y;
	BYTE* Yvalid;
	int NumY;
} MATLAB_VAR;

typedef struct
{//parametres entrants dans les algos de bruno
	//parametres geometriques fixes servant à initialiser les routines de calcul de Bruno
	//ils sont lus à partir du fichier de paramètres "LaserParams.txt" qui se créée 
	//dans le même dossier que la dll
	//ils sont copiés dans les variables locales dans la partie 
	//transcrite des fichiers matlab (vers les lignes 150)
	float radiusc;//Radius of the canon
	float camera_z;//z-Ofset of the camera virtual focus
	float dfocal; // distance of the focal to the main axis, positve value -> above
	float dccd;   // distance of the focal to the main axis, positve value -> above
	float CCDWidth;//largeur totale, ccd centré// These parameters are the dimension of the virtual CCD
	float CCDHeight;//hauteur totale, ccd centré
	float YReverse;//inversion Y de l'image, non implémenté
	float LaserIncidence[LASER_ANGLE_COUNT][LASER_INCIDENCE_COUNT];//angle par rapport à l'axe du tube
	float LaserOuverture[LASER_INCIDENCE_COUNT];//largeur de chcun des trois faisceaux
	float LaserLensloc[LASER_INCIDENCE_COUNT];//largeur de chcun des trois faisceaux
	//float LaserAngle[LASER_ANGLE_COUNT];//rotation par rapport au laser #0
	float LaserRefAngle;
	int LaserNumberingDirection;
	float LaserTilt[LASER_ANGLE_COUNT];//tilt du laser dans sa fixation

	//paramètres variables à l'execution
	float CameraAngleRotationRad;//rotation par rapport au laser #0
	float CameraAngleTiltRad;//normalement 90° (à convertir en radians)
	int LaserID;//=MAKE_LASER_ID(Incidence 0 1 ou 2, Angle 0 à 5)
	int CCDSizeX;//nombre de pixels
	int CCDSizeY;
} MATLAB_PARAM;

void SPG_CONV PrintMatrix(char* Msg, Matrix& M, char* MatrixName);
//void SPG_CONV SPG_PeakModeleLoadMatlabParams(SPG_CONFIGFILE& CFG, MATLAB_PARAM& p);

void simurays(MATLAB_VAR& m, MATLAB_PARAM& p);
void constraintfcn(MATLAB_VAR& m, Matrix& X, Matrix& NCONSTRAINT, RowVector& psconstraint, RowVector& Valid);
void prjcam(MATLAB_VAR& m,  
			float& fx, float& fy, float& fz, float& nx, float& ny, float& nz, ColumnVector& N, ColumnVector& P, ColumnVector& Q, TypCCDBox& ccdbox,
			Matrix& NCONSTRAINT, RowVector& psconstraint);
void cambasis(MATLAB_VAR& m,  
			  Matrix& Pcam, float& nnorm, float& nx, float& ny, float& nz, 
			  float& noflipx);
