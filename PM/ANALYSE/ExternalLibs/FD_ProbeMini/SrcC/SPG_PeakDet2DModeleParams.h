#ifdef INC_SPG_PeakDet2DModeleParams_INC
#error SPG_PeakDet2DModeleParams Included twice
#endif
#define	INC_SPG_PeakDet2DModeleParams_INC

#define LASER_INCIDENCE_COUNT 3
#define LASER_ANGLE_COUNT 6

#define LASER_COUNT (LASER_INCIDENCE_COUNT*LASER_ANGLE_COUNT)

#define INCIDENCE_FROM_ID(ID) (ID%LASER_INCIDENCE_COUNT)
#define ANGLE_FROM_ID(ID) ((ID/LASER_INCIDENCE_COUNT)%LASER_ANGLE_COUNT)

#define MAKE_LASER_ID(Incidence,Angle) ((Incidence%LASER_INCIDENCE_COUNT)+LASER_INCIDENCE_COUNT*(Angle%LASER_ANGLE_COUNT))


typedef struct
{
	int Version;
	int SizeX;
	int SizeY;
	float YSearchWidthUp;
	float YSearchWidthDn;
	float radiusc;//Radius of the canon
	float camera_z;//z-Ofset of the camera virtual focus
	float dfocal; // distance of the focal to the main axis, positve value -> above
	float dccd;   // distance of the focal to the main axis, positve value -> above
	float CCDWidth;//largeur totale, ccd centré// These parameters are the dimension of the virtual CCD
	float CCDHeight;//hauteur totale, ccd centré
	int YReverse;//inversion Y de l'image, non implémenté
	float LaserIncidence[LASER_ANGLE_COUNT][LASER_INCIDENCE_COUNT];//angle par rapport à l'axe du tube
	float LaserOuverture[LASER_INCIDENCE_COUNT];//largeur de chacun des trois faisceaux
	float LaserLensloc[LASER_INCIDENCE_COUNT];
	float LaserRefAngle;//position du laser #0 par rapport au zéro de la caméra
	int LaserNumberingDirection;//0 or 1, sens de numération des lasers
	float LaserTilt[LASER_ANGLE_COUNT];//tilt du laser dans sa fixation
	float LaserAxisOffset;//effet du poids du systeme qui décale verticalement la souris 
	float LaserAxisGroundDirectionRad;//effet du poids du systeme qui décale verticalement la souris 
	//...
} LDMODELEPARAMS;

#define LDMODELEPARAMS_VERSION 2

/*

#define LDMODELEPARAMSDEF_YSearchWidthUp 50
#define LDMODELEPARAMSDEF_YSearchWidthDn 150
#define LDMODELEPARAMSDEF_radiusc		60
#define LDMODELEPARAMSDEF_camera_z		52
#define LDMODELEPARAMSDEF_dfocal		-200
#define LDMODELEPARAMSDEF_dccd			5
#define LDMODELEPARAMSDEF_CCDWidth		22.5
#define LDMODELEPARAMSDEF_CCDHeight		16.8677
#define LDMODELEPARAMSDEF_YReverse		1
float LDMODELEPARAMSDEF_LaserIncidence[LASER_ANGLE_COUNT][LASER_INCIDENCE_COUNT]={{2.01100,1.94600,1.88900},{2.01100,1.94600,1.88900},{2.01100,1.94600,1.88900},{2.01100,1.94600,1.88900},{2.01100,1.94600,1.88900},{2.01100,1.94600,1.88900},};
float LDMODELEPARAMSDEF_LaserOuverture[LASER_INCIDENCE_COUNT]={1.12900,1.25600,1.13500};
float LDMODELEPARAMSDEF_LaserLensloc[LASER_INCIDENCE_COUNT]={82.4700,65.8200,52.1900};
#define LDMODELEPARAMSDEF_LaserRefAngle		0
#define LDMODELEPARAMSDEF_LaserNumberingDirection		1
#define LDMODELEPARAMSDEF_LaserTilt(i)		0
#define LDMODELEPARAMSDEF_LaserAxisOffset	0
#define LDMODELEPARAMSDEF_LaserAxisGroundDirectionRad	0

*/
