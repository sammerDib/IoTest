using System;
using System.IO;

namespace UnitySC.Shared.Format.HAZE
{
    public class HazeRange
    {
        private const int FORMAT_VERSION = 0;

        public int Nrank;
        public float Area_pct;
        public float Max_ppm;
        public float Min_ppm;
        public ulong NbCount;

        public void Read(BinaryReader br)
        {
            int lVersion = br.ReadInt32();
            if (lVersion < 0 || lVersion > FORMAT_VERSION)
                throw new Exception("Bad file format version number in HazeRange. Reading failed.");
            switch (lVersion)
            {
                case 0:
                    {
                        Nrank = br.ReadInt32();
                        Area_pct = br.ReadSingle();
                        Max_ppm = br.ReadSingle();
                        Min_ppm = br.ReadSingle();
                        NbCount = br.ReadUInt64();
                    }
                    break;
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(HazeRange.FORMAT_VERSION);
            bw.Write(Nrank);
            bw.Write(Area_pct);
            bw.Write(Max_ppm);
            bw.Write(Min_ppm);
            bw.Write(NbCount);
        }
    }
}
