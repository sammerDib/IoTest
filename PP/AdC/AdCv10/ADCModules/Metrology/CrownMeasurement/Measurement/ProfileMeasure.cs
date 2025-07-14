using System;
using System.Collections.Generic;

using ADCEngine;

using CrownMeasurementModule.CrownProfile;

using LibProcessing;

namespace CrownMeasurementModule.Objects
{//-------------------------------------------------------------------------
    //-
    //-------------------------------------------------------------------------
    public class ProfileMeasure : ObjectBase
    {
        private List<CrownProfileModule_ProfileStat> profilelist = new List<CrownProfileModule_ProfileStat>();
        public List<CrownProfileModule_ProfileStat> _profile { get { return profilelist; } set { profilelist = value; } }

        public List<CrownProfileModule_CenteringValue> listCenteringCtrlValue = new List<CrownProfileModule_CenteringValue>();
        public bool glassDiameters { get; set; }
        public bool taikoRing { get; set; }
        public bool taikoShoulder { get; set; }
        public bool EBR { get; set; }
        public bool ringScan { get; set; }
        public int nbSampleForSmoothing { get; set; }
        public int nbSample { get; set; }
        public double averageGlassdiameters { get; set; }
        public double averageTaikoRing { get; set; }
        public double averageTaikoShoulder { get; set; }
        public double averageEBR { get; set; }
        public double averageRingScan { get; set; }
        //-----------------------------------------------------
        public void EdgeSmoothing()
        {
            try
            {
                double lfAverageEdge = 0;
                double lfAverageEBR = 0;
                double lfAverageTaikoRing = 0;
                double lfAverageTaikoShoulder = 0;

                int iValueCrownSizeEDG = 0;
                int iValueCrownSizeEBR = 0;

                int iValueShoulderSizeAverageTAIKO = 0;
                int iValueRingSizeAverageTAIKO = 0;

                for (int i = nbSampleForSmoothing; i < profilelist.Count + nbSampleForSmoothing; i++)
                {
                    CrownProfileModule_ProfileStat pProfile = null;
                    try
                    {
                        pProfile = profilelist[i % nbSample];
                    }
                    catch
                    {
                        pProfile = null;
                    }

                    // dans le cas d'un point hors PAD
                    if (!pProfile.m_isRejectPoint)
                    {
                        // moyenne glissante sur iNbSampleForSmoothing echantillon
                        // l'algo peut être optimisé en ajoutant le dernier élément à la moyenne courante et en soustrayant le premier. 
                        // Si on veut gagner quelques milisecondes, on fera ça ... 
                        lfAverageEdge = 0;
                        lfAverageEBR = 0;
                        lfAverageTaikoRing = 0;
                        lfAverageTaikoShoulder = 0;

                        int iSampleRealCount = 0;
                        for (int j = (int)(1 - nbSampleForSmoothing / 2); j < (int)(nbSampleForSmoothing / 2); j++)
                        {
                            try
                            {
                                pProfile = profilelist[(i + j) % nbSample];
                            }
                            catch
                            {
                                pProfile = null;
                            }

                            // ne sont prit en compte que les points hors pad
                            if (!pProfile.m_isRejectPoint)
                            {
                                iValueCrownSizeEDG = pProfile.GetValueCrownSize(CrownProfileModule_ProfileStat.enAskMeasuremnt.en_EdgeScan);
                                iValueCrownSizeEBR = pProfile.GetValueCrownSize(CrownProfileModule_ProfileStat.enAskMeasuremnt.en_EBR);
                                iValueRingSizeAverageTAIKO = pProfile.GetValueCrownSize(CrownProfileModule_ProfileStat.enAskMeasuremnt.en_TaikoRing);
                                iValueShoulderSizeAverageTAIKO = pProfile.GetValueCrownSize(CrownProfileModule_ProfileStat.enAskMeasuremnt.en_TaikoShoulder);

                                lfAverageEdge += (double)(iValueCrownSizeEDG);
                                lfAverageEBR += (double)(iValueCrownSizeEBR);
                                lfAverageTaikoRing += iValueRingSizeAverageTAIKO;
                                lfAverageTaikoShoulder += iValueShoulderSizeAverageTAIKO;

                                iSampleRealCount++;
                            }
                            else
                            {
                                // nothing
                                //int toto = 0;  // je laisse pour debugue pour le moment ... 
                            }
                        }

                        lfAverageEdge /= (double)iSampleRealCount;
                        lfAverageEBR /= (double)iSampleRealCount;
                        lfAverageTaikoRing /= (double)iSampleRealCount;
                        lfAverageTaikoShoulder /= (double)iSampleRealCount;

                        profilelist[i % nbSample].valueCrownSizeAverageEdg = lfAverageEdge;
                        profilelist[i % nbSample].valueCrownSizeAverageEBR = lfAverageEBR;
                        profilelist[i % nbSample].valueRingSizeAverageTaiko = lfAverageTaikoRing;
                        profilelist[i % nbSample].valueShoulderSizeAverageTaiko = lfAverageTaikoShoulder;
                    }
                    else
                    {
                        // on fixe une valeur connue du viewer qui sera référencé comme "non mesurée"
                        profilelist[i % nbSample].valueCrownSizeAverageEdg = Constants.constNaNValue;
                        profilelist[i % nbSample].valueCrownSizeAverageEBR = Constants.constNaNValue;
                        profilelist[i % nbSample].valueRingSizeAverageTaiko = Constants.constNaNValue;
                        profilelist[i % nbSample].valueShoulderSizeAverageTaiko = Constants.constNaNValue;
                    }
                }
            }
            catch
            {

            }
        }

        public class CrownProfileModule_ProfileStat
        {
            public enum rpRadialPosition
            {
                Degre_0 = 0,
                Degre_90 = 2,
                Degre_180 = 4,
                Degre_270 = 6,
                Quart_1 = 1,
                Quart_2 = 3,
                Quart_3 = 5,
                Quart_4 = 7
            }

            public enum enAskMeasuremnt
            {
                en_EBR = 0,
                en_TaikoRing,
                en_TaikoShoulder,
                en_GlassDiameter,
                en_EdgeScan
            }

            private enum enTransitionNumber
            {
                en_Beforelast = 8,
                en_Last = 9
            }

            #region EDGE_SCAN        

            public int m_iValueCrownSizeEDG;                // distance externe des anneaux pour le scan Edge
            public int valueStatusCrown;

            public double valueEdgeScanInternalRingPosX;      // position micron du ring extérieur de la mesure en X
            public double valueEdgeScanInternalRingPosY;      // position micron du ring extérieur de la mesure en Y

            public double valueCrownSizeAverageEdg;      // valeur de la taile du crown local post lissage glissant 

            #endregion


            // propriété et membres pour mesure EBR
            #region EBR
            public int m_iValueCrownSizeEBR;                // distance externe des anneaux pour le scan Edge

            public double valueEBRExternalRingPosX;     // position micron du ring exterieur de la mesure en X
            public double valueEBRExternalRingPosY;      // position micron du ring exterieur de la mesure en Y

            public double valueEBRInternalRingPosX;     // position micron du ring interieur de la mesure en X
            public double valueEBRInternalRingPosY;      // position micron du ring interieur de la mesure en Y

            public double valueCrownSizeAverageEBR;      // valeur de la taile du crown local post lissage glissant 
            #endregion

            // propriété et membres pour Taijo_ring
            #region TAIKO_RING
            public int m_iValueRingSizeTAIKO;
            public int m_iValueShoulderSizeTAIKO;

            public double valueRingSizeAverageTaiko;      // valeur de la taile du TaikoRing local post lissage glissant
            public double valueShoulderSizeAverageTaiko;      // valeur de la taile du TaikoShoulder local post lissage glissant
            #endregion

            // propriété et membres pour mesure GlassDiameter
            #region GLASS_DIAMETER
            public int m_iValueCrownPosXGD;             // position ring externe
            public int m_iValueCrownPosYGD;             // position ring externe
            public double valueCrownSizeAverageGD;       // valeur de la taile du crown local post lissage glissant 
            #endregion

            #region ALL_MEASUREMENT
            // toute variables membres
            private List<CrownProfileModule_TransitionInfo> m_pListOfTransition;
            //public List<double> m_pListOfMeasurement;

            // propriétes toutes mesures
            public int valueNumberOfDetected;           // nombre de transitions detecté dans le profil. 

            public int valuePixelPosX_Defect;      // position pixel en X de ce qu'on considère être le défaut pour la création du cluster
            public int valuePixelPosY_Defect;      // position pixel en Y de ce qu'on considère être le défaut pour la création du cluster

            #endregion

            public bool m_isRejectPoint;

            // CONSTRUCTION
            public CrownProfileModule_ProfileStat()
            {
                m_pListOfTransition = new List<CrownProfileModule_TransitionInfo>();

                valuePixelPosX_Defect = 0;
                m_iValueCrownSizeEDG = 0;
                valueStatusCrown = 0;

                valueEdgeScanInternalRingPosX = 0;
                valueEdgeScanInternalRingPosY = 0;

                valueCrownSizeAverageEdg = 0;

                valuePixelPosX_Defect = 0;
                m_iValueCrownSizeEBR = 0;               // distance externe des anneaux pour le scan Edge

                valueEBRExternalRingPosX = 0;
                valueEBRExternalRingPosY = 0;

                valueEBRInternalRingPosX = 0;
                valueEBRInternalRingPosY = 0;

                valueCrownSizeAverageEBR = 0;

                m_iValueRingSizeTAIKO = 0;
                m_iValueShoulderSizeTAIKO = 0;

                valueRingSizeAverageTaiko = 0;
                valueShoulderSizeAverageTaiko = 0;


                m_iValueCrownPosXGD = 0;
                m_iValueCrownPosYGD = 0;
                valueCrownSizeAverageGD = 0;


                valueNumberOfDetected = 0;

                valuePixelPosX_Defect = 0;
                valuePixelPosY_Defect = 0;

                m_isRejectPoint = false;
            }
            //----------------------------------------------
            public void SetMeasurementInfo(enAskMeasuremnt enMeasurement, CrownProfileModule_SettingEdge esEdgeSetting, ref List<CrownProfileModule_CenteringValue> listCenteringCtrlValue)
            {
                int iFirstTransition;
                int iLastTransition;
                int toto = listCenteringCtrlValue.Count;
                try
                {
                    switch (enMeasurement)
                    {
                        //-----------------------------------------
                        case enAskMeasuremnt.en_EBR:
                            {
                                double iOuterRingAveragePosX = 0;
                                double iOuterRingAveragePosY = 0;

                                double iInnerRingAveragePosX = 0;
                                double iInnerRingAveragePosY = 0;

                                int iRingSize = 0;

                                iFirstTransition = Math.Min(esEdgeSetting.m_lsTaikoRingSelectedTransition[0], m_pListOfTransition.Count - 2);
                                iLastTransition = Math.Min(esEdgeSetting.m_lsTaikoRingSelectedTransition[1], m_pListOfTransition.Count - 1);


                                if (m_pListOfTransition.Count >= 4)
                                {
                                    iOuterRingAveragePosX = (m_pListOfTransition[iFirstTransition].m_lfMicronPosX + m_pListOfTransition[iLastTransition].m_lfMicronPosX) / 2;
                                    iOuterRingAveragePosY = (m_pListOfTransition[iFirstTransition].m_lfMicronPosY + m_pListOfTransition[iLastTransition].m_lfMicronPosY) / 2;

                                    iInnerRingAveragePosX = (m_pListOfTransition[iFirstTransition].m_lfMicronPosX + m_pListOfTransition[iLastTransition].m_lfMicronPosX) / 2;
                                    iInnerRingAveragePosY = (m_pListOfTransition[iFirstTransition].m_lfMicronPosY + m_pListOfTransition[iLastTransition].m_lfMicronPosY) / 2;

                                    // Position du ring externe, moyenne des front montant et descendant pour récupérer le centre réel
                                    iRingSize = (int)(Math.Sqrt(Math.Pow((double)(iOuterRingAveragePosX - iInnerRingAveragePosX), 2) + Math.Pow((double)(iOuterRingAveragePosY - iInnerRingAveragePosY), 2)));
                                }
                                else
                                {
                                    // une des mesure est absente car transition non detectée
                                    iRingSize = 0;

                                    iOuterRingAveragePosX = 0;
                                    iOuterRingAveragePosY = 0;

                                    iInnerRingAveragePosX = 0;
                                    iInnerRingAveragePosY = 0;
                                }

                                m_iValueCrownSizeEBR = iRingSize;

                                valueEBRExternalRingPosX = iOuterRingAveragePosX;
                                valueEBRExternalRingPosY = iOuterRingAveragePosY;

                                valueEBRInternalRingPosX = iInnerRingAveragePosX;
                                valueEBRInternalRingPosY = iInnerRingAveragePosY;

                            }
                            break;
                        //-----------------------------------------
                        case enAskMeasuremnt.en_GlassDiameter:
                            {
                                if (m_pListOfTransition.Count >= 1)
                                {
                                    m_iValueCrownPosXGD = (int)m_pListOfTransition[0].m_lfMicronPosX;
                                    m_iValueCrownPosYGD = (int)m_pListOfTransition[0].m_lfMicronPosY;
                                }
                                else
                                {
                                    m_iValueCrownPosXGD = 0;
                                    m_iValueCrownPosYGD = 0;
                                }
                            }
                            break;
                        //-----------------------------------------
                        case enAskMeasuremnt.en_TaikoRing:
                            {
                                int iRingSize = 0;
                                iFirstTransition = Math.Min(esEdgeSetting.m_lsTaikoRingSelectedTransition[0], m_pListOfTransition.Count - 2);
                                iLastTransition = Math.Min(esEdgeSetting.m_lsTaikoRingSelectedTransition[1], m_pListOfTransition.Count - 1);


                                if (m_pListOfTransition.Count >= 2)
                                {
                                    // Ring externe
                                    double iOuterRingPosX = (m_pListOfTransition[iFirstTransition].m_lfMicronPosX);
                                    double iOuterRingPosY = (m_pListOfTransition[iFirstTransition].m_lfMicronPosY);

                                    double iInnerRingPosX = (m_pListOfTransition[iLastTransition].m_lfMicronPosX);
                                    double iInnerRingPosY = (m_pListOfTransition[iLastTransition].m_lfMicronPosY);

                                    CrownProfileModule_CenteringValue CenteringCtrlValue = new CrownProfileModule_CenteringValue();

                                    CenteringCtrlValue.AddPoint(iInnerRingPosX, iInnerRingPosY, iOuterRingPosX, iOuterRingPosY);

                                    listCenteringCtrlValue.Add(CenteringCtrlValue);

                                    // Position du ring externe, moyenne des front montant et descendant pour récupérer le centre réel
                                    iRingSize = (int)(Math.Sqrt(Math.Pow((double)(iOuterRingPosX - iInnerRingPosX), 2) + Math.Pow((double)(iOuterRingPosY - iInnerRingPosY), 2)));
                                }
                                else
                                {
                                    // une des mesure est absente car transition non detectée
                                    iRingSize = 0;
                                }

                                m_iValueRingSizeTAIKO = iRingSize;
                            }
                            break;
                        //-----------------------------------------
                        case enAskMeasuremnt.en_TaikoShoulder:
                            {
                                int iShoulderSize = 0;

                                iFirstTransition = Math.Min(esEdgeSetting.m_lsTaikoShoulderSelectedTransition[0], m_pListOfTransition.Count - 2);
                                iLastTransition = Math.Min(esEdgeSetting.m_lsTaikoShoulderSelectedTransition[1], m_pListOfTransition.Count - 1);

                                if (m_pListOfTransition.Count >= 2)
                                {
                                    //mesure du Shoulder
                                    double iOuterShoulderPosX = (m_pListOfTransition[iFirstTransition].m_lfMicronPosX);
                                    double iOuterShoulderPosY = (m_pListOfTransition[iFirstTransition].m_lfMicronPosY);

                                    double iInnerShoulderPosX = (m_pListOfTransition[iLastTransition].m_lfMicronPosX);
                                    double iInnerShoulderPosY = (m_pListOfTransition[iLastTransition].m_lfMicronPosY);

                                    // Position du ring externe, moyenne des front montant et descendant pour récupérer le centre réel
                                    iShoulderSize = (int)(Math.Sqrt(Math.Pow((double)(iOuterShoulderPosX - iInnerShoulderPosX), 2) + Math.Pow((double)(iOuterShoulderPosY - iInnerShoulderPosY), 2)));
                                }
                                else
                                {
                                    // une des mesure est absente car transition non detectée
                                    iShoulderSize = 0;
                                }

                                m_iValueShoulderSizeTAIKO = iShoulderSize;

                            }
                            break;
                        //-----------------------------------------
                        case enAskMeasuremnt.en_EdgeScan:
                            {
                                int iDistance = 0;
                                iFirstTransition = Math.Min(esEdgeSetting.m_lsEdgeScanSelectedTransition[0], m_pListOfTransition.Count - 2);
                                iLastTransition = Math.Min(esEdgeSetting.m_lsEdgeScanSelectedTransition[1], m_pListOfTransition.Count - 1);

                                if (m_pListOfTransition.Count >= 2)
                                {
                                    double lfRealDifX = m_pListOfTransition[iFirstTransition].m_lfMicronPosX - m_pListOfTransition[iLastTransition].m_lfMicronPosX;
                                    double lfRealDifY = m_pListOfTransition[iFirstTransition].m_lfMicronPosY - m_pListOfTransition[iLastTransition].m_lfMicronPosY;
                                    // distance réelle entre la première et la dernière transition = largeur de couronne
                                    iDistance = (int)(Math.Sqrt(Math.Pow((double)(lfRealDifX), 2) + Math.Pow((double)(lfRealDifY), 2)));

                                    m_iValueCrownSizeEDG = iDistance;

                                    valueEdgeScanInternalRingPosX = m_pListOfTransition[iFirstTransition].m_lfMicronPosX;
                                    valueEdgeScanInternalRingPosY = m_pListOfTransition[iFirstTransition].m_lfMicronPosY;

                                    double lfCX = m_pListOfTransition[iLastTransition].m_lfMicronPosX;
                                    double lfCY = m_pListOfTransition[iLastTransition].m_lfMicronPosY;

                                    CrownProfileModule_CenteringValue CenteringCtrlValue = new CrownProfileModule_CenteringValue();

                                    CenteringCtrlValue.AddPoint(lfCX, lfCY, 0, 0);

                                    listCenteringCtrlValue.Add(CenteringCtrlValue);

                                }
                                else
                                {
                                    // une des mesure est absente car transition non detectée
                                    iDistance = 0;
                                    valueEdgeScanInternalRingPosX = 0;
                                    valueEdgeScanInternalRingPosY = 0;
                                }

                                m_iValueCrownSizeEDG = iDistance;
                            }
                            break;
                    }
                }
                catch
                {
                }
            }

            public void AddMeasurementInfo(CrownProfileModule_TransitionInfo transition)
            {
                m_pListOfTransition.Add(transition);
            }


            //------------------------------------------
            private double GetAlphaValue(double lfPosX, double lfPosY)
            {
                // Position angulaire necessite les positions micron.             
                // Si x > 0 , Alpha = arcTang (Y/X)
                // Si x < 0 et y > 0 , Alpha = arcTang (Y/X) + PI
                // Si x < 0 et y < 0 , Alpha = arcTang (Y/X) - PI

                // valeur test en debug, pour info
                //lfAlpha = Math.Atan(0.5);
                //lfAlpha = Math.Atan(1);
                //lfAlpha = Math.Atan(-1);
                //lfAlpha = Math.Atan(-0.5); 

                double lfAlpha;

                // Calcul reel
                if ((lfPosX > 0.0) && (lfPosY > 0.0)) // 0 ==> PI/2
                {
                    lfAlpha = Math.Atan(lfPosY / lfPosX);

                }
                else if ((lfPosX > 0.0) && (lfPosY < 0.0)) // 3PI/2 ==> 2PI
                {
                    lfAlpha = Math.PI * 2 + Math.Atan(lfPosY / lfPosX);
                }
                else if ((lfPosX < 0.0) && (lfPosY > 0.0)) //PI/2 ==> PI
                {
                    lfAlpha = Math.Atan(lfPosY / lfPosX) + Math.PI;
                }
                else if ((lfPosX < 0.0) && (lfPosY < 0.0)) //PI ==> 3PI/2
                {
                    lfAlpha = Math.Atan(lfPosY / lfPosX) - Math.PI;
                }
                else // lfPosX == 0
                {
                    if (lfPosY > 0) lfAlpha = Math.PI / 2;
                    else lfAlpha = 3 * Math.PI / 2;
                }

                lfAlpha = lfAlpha * 180 / Math.PI; // conversion radian => degree

                // on recale le repère notch en bas, sens trigo
                lfAlpha = 90 + lfAlpha;

                lfAlpha = 360 - lfAlpha; // inversion du sens de report. 

                if (lfAlpha < 0)
                    lfAlpha += 360;

                if (lfAlpha > 360)
                    lfAlpha -= 360;

                return lfAlpha;
            }
            //----------------------------------------------
            public int GetValueCrownSize(enAskMeasuremnt enMeasurement)
            {
                int iValueCrownSize = 0;

                switch (enMeasurement)
                {
                    case enAskMeasuremnt.en_EBR:
                        iValueCrownSize = m_iValueCrownSizeEBR;
                        break;
                    case enAskMeasuremnt.en_GlassDiameter:
                        break;
                    case enAskMeasuremnt.en_TaikoRing:
                        iValueCrownSize = m_iValueRingSizeTAIKO;
                        break;
                    case enAskMeasuremnt.en_TaikoShoulder:
                        iValueCrownSize = m_iValueShoulderSizeTAIKO;
                        break;
                    case enAskMeasuremnt.en_EdgeScan:
                        iValueCrownSize = m_iValueCrownSizeEDG;
                        break;
                }

                return iValueCrownSize;
            }

        }
    }
}
