using System;
using System.Collections.Generic;
using System.IO;

namespace FormatHAZE
{
    public class LSHazeData
    {
        private static int FORMAT_VERSION = 1;

        // Source
        public int nId; // FW, BW, Tot
        public int nWidth;
        public int nHeigth;
        public float fPixelSize_um;
        public float[] HazeMeasures = null;

        // stats
        public float max_ppm;
        public float min_ppm;
        public float mean_ppm;
        public float stddev_ppm;
        public float median_ppm;

        // ranges 
        public List<LSHazeRange> Ranges = null;

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
                        nId = br.ReadInt32();
                        nWidth = br.ReadInt32();
                        nHeigth = br.ReadInt32();
                        fPixelSize_um = br.ReadSingle();

                        int nBufSize = br.ReadInt32();
                        Byte[] lDataConverted = br.ReadBytes(nBufSize);
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

                        max_ppm = br.ReadSingle();
                        min_ppm = br.ReadSingle();
                        mean_ppm = br.ReadSingle();
                        stddev_ppm = br.ReadSingle();
                        median_ppm = br.ReadSingle();

                        //raneg
                        int nRangesNb = br.ReadInt32();
                        Ranges = new List<LSHazeRange>();
                        if (nRangesNb > 0)
                        {
                            for (int i = 0; i < nRangesNb; i++)
                            {
                                LSHazeRange rg = new LSHazeRange();
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
                        Byte[] lHistoConverted = br.ReadBytes(nHistoBufSize);
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
                        nId = br.ReadInt32();
                        nWidth = br.ReadInt32();
                        nHeigth = br.ReadInt32();
                        fPixelSize_um = br.ReadSingle();

                        int nBufSize = br.ReadInt32();
                        Byte[] lDataConverted = br.ReadBytes(nBufSize);
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

                        max_ppm = br.ReadSingle();
                        min_ppm = br.ReadSingle();
                        mean_ppm = br.ReadSingle();
                        stddev_ppm = br.ReadSingle();
                        median_ppm = br.ReadSingle();

                        //raneg
                        int nRangesNb = br.ReadInt32();
                        Ranges = new List<LSHazeRange>();
                        if (nRangesNb > 0)
                        {
                            for (int i = 0; i < nRangesNb; i++)
                            {
                                LSHazeRange rg = new LSHazeRange();
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
                        Byte[] lHistoConverted = br.ReadBytes(nHistoBufSize);
                        UInt32[] lHisto = null;
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
            bw.Write(LSHazeData.FORMAT_VERSION);
            bw.Write(nId);
            bw.Write(nWidth);
            bw.Write(nHeigth);
            bw.Write(fPixelSize_um);

            Byte[] lDataConverted = new Byte[HazeMeasures.Length * sizeof(float)];
            bw.Write(lDataConverted.Length);
            Buffer.BlockCopy(HazeMeasures, 0, lDataConverted, 0, lDataConverted.Length);
            bw.Write(lDataConverted);

            bw.Write(max_ppm);
            bw.Write(min_ppm);
            bw.Write(mean_ppm);
            bw.Write(stddev_ppm);
            bw.Write(median_ppm);

            //Range
            if (Ranges == null)
            {
                bw.Write(0);
            }
            else
            {
                bw.Write(Ranges.Count);
                foreach (LSHazeRange rg in Ranges)
                {
                    rg.Write(bw);
                }
            }

            //Histo
            bw.Write(HistLimitMin);
            bw.Write(HistLimitMax);
            bw.Write(HistNbStep);
            bw.Write(HistMaxYBar);
            Byte[] lHistoConverted = new Byte[Histo.Length * sizeof(float)];
            bw.Write(lHistoConverted.Length);
            Buffer.BlockCopy(Histo, 0, lHistoConverted, 0, lHistoConverted.Length);
            bw.Write(lHistoConverted);
        }
    }
}
