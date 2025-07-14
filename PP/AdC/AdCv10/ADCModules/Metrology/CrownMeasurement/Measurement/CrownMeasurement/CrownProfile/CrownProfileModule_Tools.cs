using System;
using System.Collections.Generic;
using System.Drawing;

using ADCEngine;

using LibProcessing;

namespace CrownMeasurementModule.CrownProfile
{
    //-------------------------------------------------------------------------
    //-
    //-------------------------------------------------------------------------
    public class CrownProfileModule_Edgeinfo
    {
        public CrownProfileModule_Edgeinfo() { }

        public int GreyLevel;
        public int PosX;
        public int PosY;
    }
    //-------------------------------------------------------------------------
    //-
    //-------------------------------------------------------------------------
    public class CrownProfileModule_TransitionInfo
    {
        public CrownProfileModule_TransitionInfo(int iPixelPosX, int iPixelPosY)
        {
            m_iPixelPosX = iPixelPosX;
            m_iPixelPosY = iPixelPosY;
        }

        public int m_iPixelPosX;
        public int m_iPixelPosY;

        public double m_lfMicronPosX;
        public double m_lfMicronPosY;

        public enTypeOfTransition TypeOfTransition;
    }
    //-------------------------------------------------------------------------
    //-
    //-------------------------------------------------------------------------
    public class CrownProfileModule_CenteringValue
    {
        public double cx_inner_px;
        public double cy_inner_px;
        public double cx_outter_px;
        public double cy_outter_px;
        public double radialValue_px;
        public double angleValue_dg;

        public void AddPoint(double cxi, double cyi, double cxo, double cyo)
        {
            cx_inner_px = cxi;
            cy_inner_px = cyi;
            cx_outter_px = cxo;
            cy_outter_px = cyo;
        }
    }

    #region EXCLUDE_EBR_CLASS
    public class CrownProfileModule_ExcludeZone
    {
        public enum enWaferSizeExisting
        {
            en_4_Inches = 100,
            en_6_Inches = 150,
            en_8_Inches = 200,
            en_12_Inches = 300
        }

        public CrownProfileModule_ExcludeZone()
        {
        }

        public CrownProfileModule_ExcludeZone(int iP1_X, int iP1_Y, int iP2_X, int iP2_Y)
        {
            pixelPosX_first = iP1_X;
            pixelPosY_first = iP1_Y;

            pixelPosX_second = iP2_X;
            pixelPosY_second = iP2_Y;
        }

        public int pixelPosX_first;
        public int pixelPosY_first;

        public int pixelPosX_second;
        public int pixelPosY_second;

        public double m_lfFirstValueAngle;
        public double m_lfLastValueAngle;
    }
    #endregion

    public class CrownProfileModule_SettingEdge
    {

        public int valuePictureCenterX_Px { get; set; }         // Point central en X du profils radial 
        public int valuePictureCenterY_Px { get; set; }         // Point central en Y du profils radial 
        public int valueRadius_Px { get; set; }                 // Nomre de pixel definissant le rayon de mesure 

        public String valueEBRColor { get; set; }               // couleur de report dans le viewer EBR
        public String valueTaikoRingColor { get; set; }         // couleur de report dans le viewer TaikoRing
        public String valueTaikoShoulderColor { get; set; }     // couleur de report dans le viewer TaikoShoulder
        public String valueEdgeScanColor { get; set; }          // couleur de report dans le viewer EdgeScan

        public int valueEBRVID { get; set; }                    // numero VID EBR
        public int valueTaikoRingVID { get; set; }            // numero VID TaikoRing
        public int valueTaikoShoulderVID { get; set; }        // numero VID TaikoShoulder
        public int valueEdgeScanVID { get; set; }             // numero VID EdgeScan
        public int valueGlassDiameterVID { get; set; }             // numero VID diametre


        public List<int> m_lsEBRSelectedTransition;             // liste des index des transitions choisit EBR
        public List<int> m_lsTaikoRingSelectedTransition;       // liste des index des transitions choisit TaikoRing
        public List<int> m_lsTaikoShoulderSelectedTransition;   // liste des index des transitions choisit TaikoShoulder
        public List<int> m_lsEdgeScanSelectedTransition;        // liste des index des transitions choisit EdgeScan

        private List<CrownProfileModule_ExcludeZone> m_lsExcludePadCoordinate_px;

        public CrownProfileModule_SettingEdge()
        {
            m_lsEBRSelectedTransition = new List<int>();
            m_lsTaikoRingSelectedTransition = new List<int>();
            m_lsTaikoShoulderSelectedTransition = new List<int>();
            m_lsEdgeScanSelectedTransition = new List<int>();

            m_lsExcludePadCoordinate_px = new List<CrownProfileModule_ExcludeZone>();
        }

        public void AddExcludePad(CrownProfileModule_ExcludeZone pExcludeZone)
        {
            m_lsExcludePadCoordinate_px.Add(pExcludeZone);
        }

        public void ClearExcludePad()
        {
            m_lsExcludePadCoordinate_px.Clear();
        }

        public void SetExcludeAngle(MatrixBase matrix, int WaferDiameters_µ)
        {
            //bool bOverRound = false;

            List<CrownProfileModule_ExcludeZone> lsNewExcludePadCoordinate_px = new List<CrownProfileModule_ExcludeZone>();

            foreach (CrownProfileModule_ExcludeZone pExcludeZone in m_lsExcludePadCoordinate_px)
            {
                double lfMicronPosX = 0.0;
                double lfMicronPosY = 0.0;

                double lfPosX = 0.0;
                double lfPosY = 0.0;

                //==========
                // le premier point de rejet du PAD courant
                PointF firstRejectPoint = matrix.pixelToMicron(new Point(pExcludeZone.pixelPosX_first, pExcludeZone.pixelPosY_first));

                lfMicronPosY = WaferDiameters_µ - lfMicronPosY;

                // on repositinne le repère au centre du wafer
                lfPosX = lfMicronPosX - (WaferDiameters_µ / 2);
                lfPosY = lfMicronPosY - (WaferDiameters_µ / 2);

                // angle corespondant au premier point
                pExcludeZone.m_lfFirstValueAngle = GetAngle(lfPosX, lfPosY);
                //==========

                //==========
                // le second point de rejet du PAD courant
                PointF secondRejectPoint = matrix.pixelToMicron(new Point(pExcludeZone.pixelPosX_second, pExcludeZone.pixelPosY_second));

                lfMicronPosY = WaferDiameters_µ - lfMicronPosY;

                // on repositionne le repère au centre du wafer
                lfPosX = lfMicronPosX - (WaferDiameters_µ / 2);
                lfPosY = lfMicronPosY - (WaferDiameters_µ / 2);

                // angle corespondant au second point
                pExcludeZone.m_lfLastValueAngle = GetAngle(lfPosX, lfPosY);
                //==========

                // on ne peut predire l'ordre de setting des points pour la config.
                // le first angle doit être plus petit que le last angle, si l'utilisateur a renseigné les points 
                // dans l'autre sens, on inverse les valeurs d'angles. 
                if (pExcludeZone.m_lfLastValueAngle < pExcludeZone.m_lfFirstValueAngle)
                {
                    double lfAngleSave = pExcludeZone.m_lfLastValueAngle;
                    pExcludeZone.m_lfLastValueAngle = pExcludeZone.m_lfFirstValueAngle;
                    pExcludeZone.m_lfFirstValueAngle = lfAngleSave;
                }

                // ATTENTION : Cas d'un PAD se trouvant "à cheval" sur un tour
                if (pExcludeZone.m_lfFirstValueAngle + 90 < pExcludeZone.m_lfLastValueAngle)
                {
                    // il faut alors creer deux zone d'exclusion
                    CrownProfileModule_ExcludeZone pNewExcludeZone = new CrownProfileModule_ExcludeZone(pExcludeZone.pixelPosX_first, pExcludeZone.pixelPosY_first, pExcludeZone.pixelPosX_second, pExcludeZone.pixelPosY_second);

                    pNewExcludeZone.m_lfFirstValueAngle = pExcludeZone.m_lfLastValueAngle;
                    pNewExcludeZone.m_lfLastValueAngle = 360;

                    pExcludeZone.m_lfLastValueAngle = pExcludeZone.m_lfFirstValueAngle;
                    pExcludeZone.m_lfFirstValueAngle = 0;
                    // on stock les nouvelle zone
                    lsNewExcludePadCoordinate_px.Add(pNewExcludeZone);
                }
            }
            //a la fin, on ajoute les nouvelles zones aux anciennes
            m_lsExcludePadCoordinate_px.AddRange(lsNewExcludePadCoordinate_px);
        }
        //---------------------------------------------------------------------
        private double GetAngle(double lfPosX, double lfPosY)
        {
            double lfAlpha = 0;

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
                lfAlpha = Math.Atan(lfPosY / lfPosX) + Math.PI;
            }
            else // lfPosX == 0
            {
                if (lfPosY > 0) lfAlpha = Math.PI / 2;
                else lfAlpha = 3 * Math.PI / 2;
            }

            //lfAlpha = 360 - (lfAlpha * 180 / Math.PI); // conversion radian => pixel et inversion du sens de rotation
            lfAlpha = (lfAlpha * 180 / Math.PI); // conversion radian => pixel et inversion du sens de rotation

            return lfAlpha;

        }

        //------------------------------------------------------
        public bool CheckForExcludePad(double lfPointAngle)
        {
            bool isRejectPoint = false;

            foreach (CrownProfileModule_ExcludeZone pExcludeZone in m_lsExcludePadCoordinate_px)
            {
                if ((lfPointAngle > pExcludeZone.m_lfFirstValueAngle) && (lfPointAngle < pExcludeZone.m_lfLastValueAngle))
                {
                    isRejectPoint = true;
                    break;
                }
            }

            return isRejectPoint;
        }
    }
}
