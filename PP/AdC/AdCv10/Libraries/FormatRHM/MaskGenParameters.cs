using System;
using System.Collections.Generic;
using System.IO;

namespace FormatRHM
{
    public class MaskGenParameters : ICloneable
    {
        private static int FORMAT_VERSION = 0;

        public float ThresholdMin { set; get; }
        public float ThresholdMax { set; get; }
        public int SampleStep { set; get; }
        public int Order { set; get; }
        public List<Tuple<int, int>> m_MorphoMask = null;
        public int Method { set; get; } // reserved for later used

        public MaskGenParameters()
        {
            ThresholdMin = 0.0f;
            ThresholdMax = 0.0f;
            SampleStep = 1;
            Order = 1;
            Method = 0;
            m_MorphoMask = new List<Tuple<int, int>>();


        }

        public MaskGenParameters(float fThMin, float fThMax, int NbSpl, int nOrder)
        {
            ThresholdMin = fThMin;
            ThresholdMax = fThMax;
            SampleStep = NbSpl;
            Order = nOrder;
            Method = 0;
            m_MorphoMask = new List<Tuple<int, int>>();

        }

        public void Read(BinaryReader br)
        {
            int lVersion = br.ReadInt32();
            if (lVersion < 0 || lVersion > FORMAT_VERSION)
                throw new Exception("Bad file format version number in MaskGenParameters. Reading failed.");
            switch (lVersion)
            {
                case 0:
                    {
                        ThresholdMin = br.ReadSingle();
                        ThresholdMax = br.ReadSingle();
                        SampleStep = br.ReadInt32();
                        Order = br.ReadInt32();
                        Method = br.ReadInt32();

                        int bnMorphoCount = br.ReadInt32();
                        m_MorphoMask = new List<Tuple<int, int>>(bnMorphoCount);
                        for (int i = 0; i < bnMorphoCount; i++)
                        {
                            int nT1 = br.ReadInt32();
                            int nT2 = br.ReadInt32();
                            m_MorphoMask.Add(new Tuple<int, int>(nT1, nT2));
                        }
                    }
                    break;
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(MaskGenParameters.FORMAT_VERSION);

            bw.Write(ThresholdMin);
            bw.Write(ThresholdMax);
            bw.Write(SampleStep);
            bw.Write(Order);
            bw.Write(Method);

            bw.Write(m_MorphoMask.Count);
            for (int i = 0; i < m_MorphoMask.Count; i++)
            {
                bw.Write(m_MorphoMask[i].Item1);
                bw.Write(m_MorphoMask[i].Item2);
            }
        }

        #region ICloneable Members

        protected object DeepCopy()
        {
            MaskGenParameters cloned = MemberwiseClone() as MaskGenParameters;
            if (m_MorphoMask != null)
            {
                cloned.m_MorphoMask = new List<Tuple<int, int>>(m_MorphoMask.Count);
                foreach (Tuple<int, int> tp in m_MorphoMask)
                {
                    cloned.m_MorphoMask.Add(new Tuple<int, int>(tp.Item1, tp.Item2));
                }
            }
            return cloned;
        }

        public virtual object Clone()
        {
            return DeepCopy();
        }

        #endregion
    }
}
