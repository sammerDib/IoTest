using System;
using System.IO;

namespace FormatHAZE
{
    public class LSHazeRange
    {
        private static int FORMAT_VERSION = 0;

        public int nrank;
        public float area_pct;
        public float max_ppm;
        public float min_ppm;
        public ulong nbCount;

        public void Read(BinaryReader br)
        {
            int lVersion = br.ReadInt32();
            if (lVersion < 0 || lVersion > FORMAT_VERSION)
                throw new Exception("Bad file format version number in LSHazeRange. Reading failed.");
            switch (lVersion)
            {
                case 0:
                    {
                        nrank = br.ReadInt32();
                        area_pct = br.ReadSingle();
                        max_ppm = br.ReadSingle();
                        min_ppm = br.ReadSingle();
                        nbCount = br.ReadUInt64();
                    }
                    break;
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(LSHazeRange.FORMAT_VERSION);
            bw.Write(nrank);
            bw.Write(area_pct);
            bw.Write(max_ppm);
            bw.Write(min_ppm);
            bw.Write(nbCount);
        }

    }
}
