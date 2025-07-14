using System;
using System.IO;

namespace FormatAHM
{
    public class HeightMeasure : ICloneable
    {
        private static int FORMAT_VERSION = 0;

        public int Label { set; get; }
        public float Height_um { set; get; }
        public int PositionX_px { set; get; }
        public int PositionY_px { set; get; }

        public HeightMeasure()
        {
            Label = -666;
            Height_um = 0.0f;
            PositionX_px = 0;
            PositionY_px = 0;
        }
        public HeightMeasure(int iLabel, float fHeight_um, int iPosX, int iPosY)
        {
            Label = iLabel;
            Height_um = fHeight_um;
            PositionX_px = iPosX;
            PositionY_px = iPosY;
        }

        public void Read(BinaryReader br)
        {
            int lVersion = br.ReadInt32();
            if (lVersion < 0 || lVersion > FORMAT_VERSION)
                throw new Exception("Bad file format version number in HeightMeasure. Reading failed.");
            switch (lVersion)
            {
                case 0:
                    {
                        Label = br.ReadInt32();
                        Height_um = br.ReadSingle();
                        PositionX_px = br.ReadInt32();
                        PositionY_px = br.ReadInt32();
                    }
                    break;
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(HeightMeasure.FORMAT_VERSION);
            bw.Write(Label);
            bw.Write(Height_um);
            bw.Write(PositionX_px);
            bw.Write(PositionY_px);
        }

        #region ICloneable Members

        protected object DeepCopy()
        {
            HeightMeasure cloned = MemberwiseClone() as HeightMeasure;
            return cloned;
        }

        public virtual object Clone()
        {
            return DeepCopy();
        }

        #endregion
    }
}
