using System;

using Matrox.MatroxImagingLibrary;

namespace PatternInspectionTools
{
    public sealed class PatternLocAdvSettings : ICloneable
    {
        public int m_nMethodType; // 0: MIL Align, 1: OPENCV Align (DEPRECATED !!)
        public string m_csLocImagePath;
        public int m_nRefPt_X;
        public int m_nRefPt_Y;
        public int m_nROIOrigin_X;
        public int m_nROIOrigin_Y;
        public int m_nROISize_sx;
        public int m_nROISize_sy;
        public int m_nUseMask;
        public string m_csLocMaskPath;

        public int m_nMilPrmSpeed;
        public int m_nMilPrmAccuracy;
        public int m_nMilPrmPolarity;
        public double m_dMilPrmAcceptance;
        public double m_dMilPrmCertainty;
        public double m_dMilPrmScaleFactor_Min;
        public double m_dMilPrmScaleFactor_Max;

        public int m_nOpencvPrmThreshold; //DEPRECATED 

        public PatternLocAdvSettings()
        {
            // All standard type no allocation needed hence no Disposable item needed
        }

        private object DeepCopy()
        {
            PatternLocAdvSettings cloned = MemberwiseClone() as PatternLocAdvSettings;
            return cloned;
        }

        public object Clone()
        {
            return DeepCopy();
        }

        public int SpeedToMil()
        {
            int nmilres = 0;
            switch (m_nMilPrmSpeed)
            {
                case 0:
                    nmilres = MIL.M_LOW; break;
                case 1:
                    nmilres = MIL.M_MEDIUM; break;
                case 2:
                    nmilres = MIL.M_HIGH; break;
                case 3:
                    nmilres = MIL.M_VERY_HIGH; break;
                default:
                    nmilres = MIL.M_DEFAULT; break;
            }
            return nmilres;
        }

        public int AccuracyToMil()
        {
            int nmilres = 0;
            switch (m_nMilPrmAccuracy)
            {
                case 0:
                    nmilres = MIL.M_MEDIUM; break;
                case 1:
                    nmilres = MIL.M_HIGH; break;
                default:
                    nmilres = MIL.M_DEFAULT; break;
            }
            return nmilres;
        }

        public int PolarityToMil()
        {
            int nmilres = 0;
            switch (m_nMilPrmPolarity)
            {
                case 0:
                    nmilres = MIL.M_SAME; break;
                case 1:
                    nmilres = MIL.M_SAME_OR_REVERSE; break;
                case 2:
                    nmilres = MIL.M_REVERSE; break;
                case 3:
                    nmilres = MIL.M_ANY; break;
                default:
                    nmilres = MIL.M_DEFAULT; break;
            }
            return nmilres;
        }
    }
}
