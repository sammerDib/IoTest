using System;
using System.Collections.Generic;

namespace PatternInspectionTools
{
    public sealed class PatternInspectSettings : ICloneable
    {
        public int m_nType;			//0: dark; 1: bright; 2: both
        public int m_nNbROI;
        public int m_bInspectionByROI;
        public List<int> m_vnNormalizeMethod; // list of length m_nNbROI+1 which contains Normalize Methods (index 0, for global insp; index 1 to m_nNbROI+1, for each roi)
        public List<float>[] m_vfK = new List<float>[2]; // 2 lists (1 _DARK and 1 _BRIGHT of length m_nNbROI+1 which contains Sensitivity Prm (index 0, for global insp; index 1 to m_nNbROI+1, for each roi)
        public List<int>[] m_vnTh = new List<int>[2];// 2 lists (1 _DARK and 1 _BRIGHT of length m_nNbROI+1 which contains Threshold Prm (index 0, for global insp; index 1 to m_nNbROI+1, for each roi)
        public float[] m_fBlobExElongation = new float[2];
        public int[] m_nBlobExArea = new int[2];
        public int[] m_nBlobExBreadth = new int[2];

        public PatternInspectSettings()
        {
            m_nType = 0;
            m_nNbROI = 1;
            m_bInspectionByROI = 0;

            m_vnNormalizeMethod = new List<int>();
            for (int i = 0; i < 2; i++)
            {
                m_vfK[i] = new List<float>();
                m_vnTh[i] = new List<int>();
                m_fBlobExElongation[i] = 0.0F;
                m_nBlobExArea[i] = 0;
                m_nBlobExBreadth[i] = 0;
            }
        }

        private object DeepCopy()
        {
            PatternInspectSettings cloned = MemberwiseClone() as PatternInspectSettings;

            cloned.m_vnNormalizeMethod = new List<int>();
            cloned.m_vnNormalizeMethod.AddRange(m_vnNormalizeMethod);
            cloned.m_vfK = new List<float>[2];
            cloned.m_vnTh = new List<int>[2];
            cloned.m_fBlobExElongation = new float[2];
            cloned.m_nBlobExArea = new int[2];
            cloned.m_nBlobExBreadth = new int[2];
            for (int i = 0; i < 2; i++)
            {
                cloned.m_vfK[i] = new List<float>();
                cloned.m_vfK[i].AddRange(m_vfK[i]);
                cloned.m_vnTh[i] = new List<int>();
                cloned.m_vnTh[i].AddRange(m_vnTh[i]);
                cloned.m_fBlobExElongation[i] = m_fBlobExElongation[i];
                cloned.m_nBlobExArea[i] = m_nBlobExArea[i];
                cloned.m_nBlobExBreadth[i] = m_nBlobExBreadth[i];
            }
            return cloned;
        }

        public object Clone()
        {
            return DeepCopy();
        }
    }
}
