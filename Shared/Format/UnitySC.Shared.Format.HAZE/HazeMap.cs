using System;
using System.Collections.Generic;
using System.IO;


namespace UnitySC.Shared.Format.HAZE
{
    public class HazeMap
    {
        private const int FORMAT_VERSION = 1;

        // Source
        public int Id; // FW, BW, Tot
        public int Width;
        public int Heigth;
        public float PixelSize_um;
        public float[] HazeMeasures = null;

        // stats
        public float Max_ppm;
        public float Min_ppm;
        public float Mean_ppm;
        public float Stddev_ppm;
        public float Median_ppm;

        // ranges 
        public List<HazeRange> Ranges = null;

        // Histo
        public float HistLimitMax;
        public float HistLimitMin;
        public float HistNbStep;
        public UInt32[] Histo;
        public UInt32 HistMaxYBar;

        public void Read(BinaryReader br)
        {
            int lVersion = br.ReadInt32();
            if (lVersion < 0 || lVersion > FORMAT_VERSION)
                throw new Exception("Bad file format version number in LSHazeRange. Reading failed.");
            switch (lVersion)
            {
                case 0:
                    {
                        Id = br.ReadInt32();
                        Width = br.ReadInt32();
                        Heigth = br.ReadInt32();
                        PixelSize_um = br.ReadSingle();

                        int nBufSize = br.ReadInt32();
                        byte[] lDataConverted = br.ReadBytes(nBufSize);
                        float[] lDataFloat = null;
                        if ((lDataConverted.Length % 4) == 0)
                        {
                            uint lFloatDataSize = (uint)(lDataConverted.Length / sizeof(float));
                            // Convert byte to float
                            //System.Diagnostics.Debug.Assert(lFloatDataSize >= (uint.MaxValue / 4));
                            lDataFloat = new float[lFloatDataSize];
                            Buffer.BlockCopy(lDataConverted, 0, lDataFloat, 0, lDataConverted.Length);
                        }
                        HazeMeasures = lDataFloat;

                        Max_ppm = br.ReadSingle();
                        Min_ppm = br.ReadSingle();
                        Mean_ppm = br.ReadSingle();
                        Stddev_ppm = br.ReadSingle();
                        Median_ppm = br.ReadSingle();

                        //raneg
                        int nRangesNb = br.ReadInt32();
                        Ranges = new List<HazeRange>();
                        if (nRangesNb > 0)
                        {
                            for (int i = 0; i < nRangesNb; i++)
                            {
                                var rg = new HazeRange();
                                rg.Read(br);
                                Ranges.Add(rg);
                            }
                        }

                        //histo
                        HistLimitMin = br.ReadSingle();
                        HistLimitMax = br.ReadSingle();
                        HistNbStep = br.ReadSingle();
                        HistMaxYBar = (UInt32)br.ReadSingle();
                        int nHistoBufSize = br.ReadInt32();
                        byte[] lHistoConverted = br.ReadBytes(nHistoBufSize);
                        float[] lHistoFloat = null;
                        if ((lHistoConverted.Length % 4) == 0)
                        {
                            uint lHistFloatDataSize = (uint)(lHistoConverted.Length / sizeof(float));
                            // Convert byte to float
                            //System.Diagnostics.Debug.Assert(lFloatDataSize >= (uint.MaxValue / 4));
                            lHistoFloat = new float[lHistFloatDataSize];
                            Buffer.BlockCopy(lHistoConverted, 0, lHistoFloat, 0, lHistoConverted.Length);
                        }
                        UInt32[] UlHistoFlt = new UInt32[lHistoFloat.Length];
                        uint un = 0;
                        foreach (float fval in lHistoFloat)
                            UlHistoFlt[un++] = (UInt32)fval;
                        Histo = UlHistoFlt;
                    }
                    break;

                case 1:
                    {
                        Id = br.ReadInt32();
                        Width = br.ReadInt32();
                        Heigth = br.ReadInt32();
                        PixelSize_um = br.ReadSingle();

                        int nBufSize = br.ReadInt32();
                        byte[] lDataConverted = br.ReadBytes(nBufSize);
                        float[] lDataFloat = null;
                        if ((lDataConverted.Length % 4) == 0)
                        {
                            uint lFloatDataSize = (uint)(lDataConverted.Length / sizeof(float));
                            // Convert byte to float
                            //System.Diagnostics.Debug.Assert(lFloatDataSize >= (uint.MaxValue / 4));
                            lDataFloat = new float[lFloatDataSize];
                            Buffer.BlockCopy(lDataConverted, 0, lDataFloat, 0, lDataConverted.Length);
                        }
                        HazeMeasures = lDataFloat;

                        Max_ppm = br.ReadSingle();
                        Min_ppm = br.ReadSingle();
                        Mean_ppm = br.ReadSingle();
                        Stddev_ppm = br.ReadSingle();
                        Median_ppm = br.ReadSingle();

                        //raneg
                        int nRangesNb = br.ReadInt32();
                        Ranges = new List<HazeRange>();
                        if (nRangesNb > 0)
                        {
                            for (int i = 0; i < nRangesNb; i++)
                            {
                                var rg = new HazeRange();
                                rg.Read(br);
                                Ranges.Add(rg);
                            }
                        }

                        //histo
                        HistLimitMin = br.ReadSingle();
                        HistLimitMax = br.ReadSingle();
                        HistNbStep = br.ReadSingle();
                        HistMaxYBar = br.ReadUInt32();
                        int nHistoBufSize = br.ReadInt32();
                        byte[] lHistoConverted = br.ReadBytes(nHistoBufSize);
                        uint[] lHisto = null;
                        if ((lHistoConverted.Length % 4) == 0)
                        {
                            uint lHistDataSize = (uint)(lHistoConverted.Length / sizeof(UInt32));
                            // Convert byte to UInt32
                            lHisto = new UInt32[lHistDataSize];
                            Buffer.BlockCopy(lHistoConverted, 0, lHisto, 0, lHistoConverted.Length);
                        }
                        Histo = lHisto;
                    }
                    break;
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(HazeMap.FORMAT_VERSION);
            bw.Write(Id);
            bw.Write(Width);
            bw.Write(Heigth);
            bw.Write(PixelSize_um);

            byte[] lDataConverted = new byte[HazeMeasures.Length * sizeof(float)];
            bw.Write(lDataConverted.Length);
            Buffer.BlockCopy(HazeMeasures, 0, lDataConverted, 0, lDataConverted.Length);
            bw.Write(lDataConverted);

            bw.Write(Max_ppm);
            bw.Write(Min_ppm);
            bw.Write(Mean_ppm);
            bw.Write(Stddev_ppm);
            bw.Write(Median_ppm);

            //Range
            if (Ranges == null)
            {
                bw.Write(0);
            }
            else
            {
                bw.Write(Ranges.Count);
                foreach (var rg in Ranges)
                {
                    rg.Write(bw);
                }
            }

            //Histo
            bw.Write(HistLimitMin);
            bw.Write(HistLimitMax);
            bw.Write(HistNbStep);
            bw.Write(HistMaxYBar);
            byte[] lHistoConverted = new byte[Histo.Length * sizeof(float)];
            bw.Write(lHistoConverted.Length);
            Buffer.BlockCopy(Histo, 0, lHistoConverted, 0, lHistoConverted.Length);
            bw.Write(lHistoConverted);
        }
    }
}
