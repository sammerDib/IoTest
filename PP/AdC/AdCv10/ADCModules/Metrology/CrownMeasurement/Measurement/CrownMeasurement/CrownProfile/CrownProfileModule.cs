using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using CrownMeasurementModule.Objects;

using LibProcessing;

using MergeContext.Context;

using static CrownMeasurementModule.Objects.ProfileMeasure;

namespace CrownMeasurementModule.CrownProfile
{
    public class CrownProfileModule : ModuleBase
    {
        //-------------------------------------------------------------------------
        public class CrownProfileModule_Edgeinfo
        {
            public CrownProfileModule_Edgeinfo() { }

            public int GreyLevel;
            public int PosX;
            public int PosY;
        }
        //-------------------------------------------------------------------------

        //public class CrownProfileModule_TransitionInfo
        //{
        //    public CrownProfileModule_TransitionInfo(int iPixelPosX, int iPixelPosY)
        //    {
        //        m_iPixelPosX = iPixelPosX;
        //        m_iPixelPosY = iPixelPosY;
        //    }

        //    public int m_iPixelPosX;
        //    public int m_iPixelPosY;

        //    public double m_lfMicronPosX;
        //    public double m_lfMicronPosY;

        //    public enTypeOfTransition TypeOfTransition;
        //}

        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly IntParameter paramNbSampleByDegrees;
        public readonly IntParameter paramNbSampleForSmoothing;
        public readonly IntParameter paramMeasurementVectorSize;
        public readonly EnumParameter<enTypeOfTransition> parmaTypeOfTransition;




        //=================================================================
        // Constructeur
        //=================================================================
        public CrownProfileModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramNbSampleByDegrees = new IntParameter(this, "NbSampleByDegrees");
            paramNbSampleForSmoothing = new IntParameter(this, "NbSampleForSmoothing");
            paramMeasurementVectorSize = new IntParameter(this, "MeasurementVectorSize");

            parmaTypeOfTransition = new EnumParameter<enTypeOfTransition>(this, "TypeOfTransition");
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            ImageBase image = (ImageBase)obj;

            ProfileMeasure Measure = new ProfileMeasure();

            Measure.nbSampleForSmoothing = paramNbSampleForSmoothing.Value;
            Measure.nbSample = paramNbSampleByDegrees.Value * 360;
            Measure._profile = GenerateProfile(image);

            ProcessChildren(Measure);
        }
        //=================================================================
        private List<CrownProfileModule_ProfileStat> GenerateProfile(ImageBase image)
        {
            List<CrownProfileModule_ProfileStat> _profile = new List<CrownProfileModule_ProfileStat>();

            WaferBase wafer = image.Layer.Wafer;
            MatrixBase matrix = image.Layer.Matrix;

            int PosX, PosY;
            double CoefA, CoefB;

            ChamberSettings chamberSettings = null;
            try
            {
                chamberSettings = image.Layer.GetContextMachine<ChamberSettings>(ConfigurationType.Chamber.ToString());
            }
            catch (Exception ex)
            {
                logWarning("Get chamber settings error " + ex.ToString());
            }


            // OPF_DBG begin ==>
            // PointF WaferCenterInPicture = matrix.micronToPixel(new PointF(0,0));
            // Rectangle Pixrect = matrix.micronToPixel(wafer.SurroundingRectangle);
            // int Radius = Pixrect.Width / 2; // via le surrounding rectangle, on récupère le "diamètre" du wafer, puis le rayon
            PointF WaferCenterInPicture = new PointF((float)1515.0, (float)1514.0);
            int Radius = 1398;
            // OPF_DBG end <==


            int NumberOfSample = paramNbSampleByDegrees.Value * 360;
            double degreeBySample = 360.0 / NumberOfSample;

            CSharpImage csharpImage = image.CurrentProcessingImage.GetCSharpImage();

            for (int Sample = 0; Sample < NumberOfSample; Sample++)
            {
                // pour chaque point d'echantillon du cercle, on trace la droite reliant le point au centre. 
                // la variable echantillon représente ici une position radial. 
                PosX = (int)(WaferCenterInPicture.X + Radius * Math.Cos(Math.PI * 2 * (double)Sample / (double)NumberOfSample));
                PosY = (int)(WaferCenterInPicture.Y + Radius * Math.Sin(Math.PI * 2 * (double)Sample / (double)NumberOfSample));

                bool bRejectPoint = chamberSettings != null ? CheckForExcludePad(Sample * degreeBySample, chamberSettings) : false;

                // y = ax + b
                // c(Cx,Cy) = centre
                // p(x1,y1) = point du cercle

                // a = (Cy - y1)/(Cx - x1)
                // b = Cy - aCx		
                if (WaferCenterInPicture.X != PosX)
                {
                    CoefA = (double)((WaferCenterInPicture.Y - (double)PosY) / (WaferCenterInPicture.X - (double)PosX));
                    CoefB = (double)WaferCenterInPicture.Y - CoefA * (double)WaferCenterInPicture.X;
                }
                else
                {
                    // les droites verticales sont a traiter explicitement car leur pentes est infinies
                    CoefA = 0;
                    CoefB = 0;
                }

                List<CrownProfileModule_Edgeinfo> pVector = new List<CrownProfileModule_Edgeinfo>();
                GetVectorValue(ref pVector, CoefA, CoefB, PosX, PosY, WaferCenterInPicture.X, WaferCenterInPicture.Y, Radius, paramMeasurementVectorSize.Value, csharpImage);

                // byValue contient un vecteur de point noir ou blanc.
                // il faut maintenant etudier le profils. 
                CrownProfileModule_ProfileStat pProfile = GetProfileEdge(ref pVector, paramMeasurementVectorSize.Value, parmaTypeOfTransition.Value, matrix, (int)(wafer.SurroundingRectangle.Width / 2));

                pProfile.m_isRejectPoint = bRejectPoint;

                _profile.Add(pProfile);


            }

            return _profile;
        }


        public bool CheckForExcludePad(double lfPointAngle, ChamberSettings chamberSettings)
        {
            bool isRejectPoint = false;

            foreach (ExclusionArea exclusion in chamberSettings.Exclusions)
            {
                // On reposition endAngle et pointToCheck dans un repére où startAngle est à 0°
                double endAngle = exclusion.EndExclusionAngleDegree;
                double startAngle = exclusion.StartExclusionAngleDegree;
                double pointToCheck = lfPointAngle;
                endAngle = (endAngle - startAngle) < 0 ? endAngle - startAngle + 360 : endAngle - startAngle;
                pointToCheck = (lfPointAngle - startAngle) < 0 ? lfPointAngle - startAngle + 360 : lfPointAngle - startAngle;

                if (pointToCheck < endAngle)
                {
                    isRejectPoint = true;
                    break;
                }
            }

            return isRejectPoint;
        }
        private CrownProfileModule_ProfileStat GetProfileEdge(ref List<CrownProfileModule_Edgeinfo> pVector, int MeasurementVectorSize, enTypeOfTransition TypeOfTransition, MatrixBase matrix, int WaferRadius)
        {

            CrownProfileModule_ProfileStat pProfile = new CrownProfileModule_ProfileStat();
            List<int> eListIndexOfEdge = GetAllTransition(MeasurementVectorSize, TypeOfTransition, ref pVector);

            // Analyse des transitions. La première sera un flanc montant.
            int inNbTransition = eListIndexOfEdge.Count;

            if (inNbTransition >= 2) // au moins deux front
            {
                pProfile.valueNumberOfDetected = inNbTransition;

                // pour toute les nouvelles mesures, on récupère l'ensemble des transition. Elles sont marqué à 
                // la mesure comme montante ou descendante, et la suite nous dira ce que l'on en fait. 
                //-----------                        
                // par defaut, les nouvelles mesures se feront avec une detection de toutes les transitions.
                // si celà doit être autrement, il faudra implementer un petit objet qui trace le type de transition dans 
                // la methode GetAllTransition. Le premier front est un front montant, et on bascule à chaque transition. 
                int iTypeOfEdge = 0;
                for (int i = 0; i < eListIndexOfEdge.Count; i++)
                {
                    CrownProfileModule_TransitionInfo pTransition = new CrownProfileModule_TransitionInfo(pVector[eListIndexOfEdge[i]].PosX, pVector[eListIndexOfEdge[i]].PosY);

                    PointF position_µ = matrix.pixelToMicron(new Point(pTransition.m_iPixelPosX, pTransition.m_iPixelPosY));

                    pTransition.m_lfMicronPosX = position_µ.X;
                    pTransition.m_lfMicronPosY = position_µ.Y;

                    if (iTypeOfEdge == 0)
                        pTransition.TypeOfTransition = enTypeOfTransition.en_RisingEdge;
                    else pTransition.TypeOfTransition = enTypeOfTransition.en_FallingEdge;

                    pProfile.AddMeasurementInfo(pTransition);

                    iTypeOfEdge = 1 - iTypeOfEdge; // basculement du type de front
                }
            }
            else    // cas batard d'une non detection, on doit éviter que cela arrive si possible via le filtrage.
                    // la mise à zero des données genérera une non conformité de ring
            {
                CrownProfileModule_TransitionInfo pTransition_0 = new CrownProfileModule_TransitionInfo(WaferRadius, WaferRadius);
                CrownProfileModule_TransitionInfo pTransition_1 = new CrownProfileModule_TransitionInfo(0, 0);

                pProfile.AddMeasurementInfo(pTransition_0);
                pProfile.AddMeasurementInfo(pTransition_1);
            }

            return pProfile;

        }

        private List<int> GetAllTransition(int measurementVectorSize, enTypeOfTransition typeOfTransition, ref List<CrownProfileModule_Edgeinfo> pVector)
        {
            List<int> eListOfEdge = new List<int>();
            eListOfEdge.Clear();
            // on force tous les niveaux non noir à la valeur maximum. 
            for (int i = 0; i < measurementVectorSize; i++)
            {
                if (pVector[i].GreyLevel != Constants.constBlackGreyLevel)
                    pVector[i].GreyLevel = Constants.constWhiteGreyLevel;
            }
            // recupération des basculement de front
            switch (typeOfTransition)
            {
                case enTypeOfTransition.en_AllTransition: // tous les fronts montant et descendant
                    //
                    for (int i = 1; i < measurementVectorSize; i++)
                    {
                        if (pVector[i].GreyLevel != pVector[i - 1].GreyLevel)	// on cherche les transitions
                            eListOfEdge.Add(i);
                    }
                    break;
                //--------------
                case enTypeOfTransition.en_RisingEdge: // Front montant uniquement
                    //
                    for (int i = 1; i < measurementVectorSize; i++)
                    {
                        if ((pVector[i].GreyLevel == Constants.constWhiteGreyLevel) & (pVector[i - 1].GreyLevel == Constants.constBlackGreyLevel))	// on cherche les transitions positive
                            eListOfEdge.Add(i);
                    }
                    break;
                //--------------
                case enTypeOfTransition.en_FallingEdge: // Front descendant uniquement
                    //
                    for (int i = 1; i < measurementVectorSize; i++)
                    {
                        if ((pVector[i].GreyLevel == Constants.constBlackGreyLevel) & (pVector[i - 1].GreyLevel == Constants.constWhiteGreyLevel))	// on cherche les transitions negative
                            eListOfEdge.Add(i);
                    }
                    break;
            }

            return eListOfEdge;
        }

        private void GetVectorValue(ref List<CrownProfileModule_Edgeinfo> pVector, double coefA, double coefB, int posX, int posY, float waferCenterX, float waferCenterY, int radius, int MeasurementVectorSize, CSharpImage csharpImage)
        {
            CrownProfileModule_ProfileStat.rpRadialPosition RadialPosition = GetRadialPosition((int)waferCenterX, (int)waferCenterY, posX, posY);

            double lfStepX;
            int byValue;

            int inPosX, inPosY;
            double lfPosX, lfPosY;

            switch (RadialPosition)
            {
                ////////////
                case CrownProfileModule_ProfileStat.rpRadialPosition.Quart_1:
                case CrownProfileModule_ProfileStat.rpRadialPosition.Quart_2:
                case CrownProfileModule_ProfileStat.rpRadialPosition.Quart_3:
                case CrownProfileModule_ProfileStat.rpRadialPosition.Quart_4:
                    lfStepX = ((double)waferCenterX - (double)posX) / (double)radius;
                    break;
                ////////////
                case CrownProfileModule_ProfileStat.rpRadialPosition.Degre_0:
                    lfStepX = -1;
                    break;
                ////////////
                case CrownProfileModule_ProfileStat.rpRadialPosition.Degre_90:
                    lfStepX = 0;
                    break;
                ////////////
                case CrownProfileModule_ProfileStat.rpRadialPosition.Degre_180:
                    lfStepX = 1;
                    break;
                ////////////
                case CrownProfileModule_ProfileStat.rpRadialPosition.Degre_270:
                    lfStepX = 0;
                    break;
                default:
                    lfStepX = 1;
                    break;
            }

            // Sur les droites verticales
            if ((coefA == 0) && (coefB == 0))
            {
                lfStepX = 0;
                lfPosX = posX;
                // lfPosY explore la verticale dans un sens ou dans l'autre selon l'angle courant
                // son point de départ est le bord de l'image. 
                if (RadialPosition == CrownProfileModule_ProfileStat.rpRadialPosition.Degre_90)
                    lfPosY = 0;
                else lfPosY = csharpImage.height - 1; // Degre_270
            }
            else
            {
                lfPosX = posX;
                lfPosY = coefA * lfPosX + coefB;
            }


            for (int i = 0; i < MeasurementVectorSize; i++)
            {
                CrownProfileModule_Edgeinfo pEdgeInfo = new CrownProfileModule_Edgeinfo();

                inPosX = (int)Math.Min(csharpImage.width - 1, Math.Max(0, lfPosX)); // On cadre entre 0 et la taille max de l'image
                inPosY = (int)Math.Min(csharpImage.height - 1, Math.Max(0, lfPosY));

                // recupération de la valeur dans l'image
                byValue = csharpImage.uint8(inPosX, inPosY);

                pEdgeInfo.PosX = inPosX;
                pEdgeInfo.PosY = inPosY;
                pEdgeInfo.GreyLevel = byValue;

                pVector.Add(pEdgeInfo);

                if ((coefA == 0) && (coefB == 0))
                {
                    // lfPosX ne change pas de valeur;
                    // lfPosY explore la verticale dans un sens ou dans l'autre selon l'angle courant
                    if (RadialPosition == CrownProfileModule_ProfileStat.rpRadialPosition.Degre_90)
                        lfPosY += 1;
                    else lfPosY -= 1; // Degre_270
                }
                else
                {
                    lfPosX += lfStepX;
                    lfPosY = (coefA * lfPosX + coefB);
                }
            }
        }
        protected CrownProfileModule_ProfileStat.rpRadialPosition GetRadialPosition(int Cx, int Cy, int Px, int Py)
        {
            CrownProfileModule_ProfileStat.rpRadialPosition RadialPosition;

            if (Cx > Px)        // PI/2 à 3PI/2 : demi gauche
            {
                if (Cy > Py)        // de PI/2 à PI : quart haut gauche
                {
                    RadialPosition = CrownProfileModule_ProfileStat.rpRadialPosition.Quart_2;
                }
                else if (Cy < Py)   // de PI à 3PI/2 : quart bas gauche
                {
                    RadialPosition = CrownProfileModule_ProfileStat.rpRadialPosition.Quart_3;
                }
                else                // PI : horizontale du centre
                {
                    RadialPosition = CrownProfileModule_ProfileStat.rpRadialPosition.Degre_180;
                }
            }
            else if (Cx < Px)   // de 3PI/2 à PI/2 : demi droit
            {
                if (Cy > Py)        // de 0 à PI/2 : quart haut droit
                {
                    RadialPosition = CrownProfileModule_ProfileStat.rpRadialPosition.Quart_1;
                }
                else if (Cy < Py)   // de PI à 3PI/2 : quart bas droit
                {
                    RadialPosition = CrownProfileModule_ProfileStat.rpRadialPosition.Quart_4;
                }
                else                // 0 : horizontale du centre
                {
                    RadialPosition = CrownProfileModule_ProfileStat.rpRadialPosition.Degre_0;
                }
            }
            else                // PI/2 ou 3PI/2 : vertical du centre
            {
                if (Cy < Py)        // 3PI/2
                {
                    RadialPosition = CrownProfileModule_ProfileStat.rpRadialPosition.Degre_270;
                }
                else                // PI/2
                {
                    RadialPosition = CrownProfileModule_ProfileStat.rpRadialPosition.Degre_90;
                }
            }

            return RadialPosition;
        }
    }
}
