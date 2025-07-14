
typedef struct
{
	BYTE* Tag;
	float* Input;
	float* Output;
} LEARNINGSET;

typedef struct
{
	int szInputSet;
	int szOutputSet;
	int szTag;
	int Size;
	int MaxSet;
	int NumSet;
	LEARNINGSET* S;

	float* M;
} LEARNINGSTATE;

//callback tagdistance(Tag,Tag) pour condenser le set d'apprentissage
int SPG_CONV LearningSet_Init(LEARNINGSTATE& L, int szTag, int szInputSet, int szOutputSet, int MaxSet);
void SPG_CONV LearningSet_Close(LEARNINGSTATE& L);
int SPG_CONV LearningSet_Add(LEARNINGSTATE& L, BYTE* Tag, float* InputSet, float* OutputSet);//desired output set
int SPG_CONV LearningSet_Apply(LEARNINGSTATE& L, float* InputSet, float* OutputSet);//computed output set
int SPG_CONV LearningSet_Save(LEARNINGSTATE& L, char* FName);
int SPG_CONV LearningSet_Load(LEARNINGSTATE& L, char* FName);

int SPG_CONV LearningSet_Invert(LEARNINGSTATE& L);
