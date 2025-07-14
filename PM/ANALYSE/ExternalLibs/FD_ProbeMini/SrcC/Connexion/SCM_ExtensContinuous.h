
#ifndef SPG_CONV
#error Convention d'appel non définie
#endif

//Fonctions d'extension apportées aux connexions 'acquisition device'
//en complément des fonctions déclarées dans "..\SrcC\Connexion\SCM_Connexion.h"

#define SCX_EXTSTARTCONTINUOUSREAD(f) int (SPG_CONV f)(SCX_CONNEXION* C)
#define SCX_EXTSTOPCONTINUOUSREAD(f) int (SPG_CONV f)(SCX_CONNEXION* C)
#define SCX_EXTSTARTCONTINUOUSWRITE(f) int (SPG_CONV f)(SCX_CONNEXION* C)
#define SCX_EXTSTOPCONTINUOUSWRITE(f) int (SPG_CONV f)(SCX_CONNEXION* C)
#define SCX_EXTGETCONTINUOUSFREQUENCY(f) int (SPG_CONV f)(double& Frequency, SCX_CONNEXION* C)

typedef SCX_EXTSTARTCONTINUOUSREAD(*SCX_STARTCONTINUOUSREAD);
typedef SCX_EXTSTOPCONTINUOUSREAD(*SCX_STOPCONTINUOUSREAD);
typedef SCX_EXTSTARTCONTINUOUSWRITE(*SCX_STARTCONTINUOUSWRITE);
typedef SCX_EXTSTOPCONTINUOUSWRITE(*SCX_STOPCONTINUOUSWRITE);
typedef SCX_EXTGETCONTINUOUSFREQUENCY(*SCX_GETCONTINUOUSFREQUENCY);

//declare les extensions pour pouvoir les appeler depuis le programme principal
SCX_EXTSTARTCONTINUOUSREAD(scxStartContinuousRead);
SCX_EXTSTOPCONTINUOUSREAD(scxStopContinuousRead);
SCX_EXTSTARTCONTINUOUSWRITE(scxStartContinuousWrite);
SCX_EXTSTOPCONTINUOUSWRITE(scxStopContinuousWrite);
SCX_EXTGETCONTINUOUSFREQUENCY(scxGetContinuousFrequency);
