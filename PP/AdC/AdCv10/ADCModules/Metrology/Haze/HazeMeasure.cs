using System;
using System.Collections.Generic;
using System.Linq;

using ADCEngine;

using MergeContext.Context;

using UnitySC.Shared.Tools;

namespace HazeModule
{
    // DARkVIEW HAZE VERSION
    [Obsolete("Only For DarkviewModule - use Haze LS otherwise", false)]
    public class HazeMeasure : ObjectBase
    {
        public const int _R_ = 2;
        public const int _G_ = 1;
        public const int _B_ = 0;

        public class Data : DisposableObject
        {
            public byte[] lutR = new byte[256];
            public byte[] lutG = new byte[256];
            public byte[] lutB = new byte[256];

            public int[] Levels = new int[8];

            public byte[] Thumbbail = null; // packer en BGR
            public int ThumbSizeX = 0;
            public int ThumbSizeY = 0;

            public float[] HazeNbPixels = new float[9];     // 9 = 8 cores levels + 1 as all range Total ;
            public float[] HazeAeraPct = new float[9];      // 9 = 8 cores levels + 1 as all range Total ;

            public float Globalppm = 0.0f;

            public Data()
            {

            }
        }

        //=================================================================
        // Property
        //=================================================================
        public string RangeName = String.Empty;
        public float[] RangeScaleMax = new float[7];

        public int SizeWidth;
        public int SizeHeight;

        public byte[] InputImageGL = null;

        public byte[] InputWaferMask = null;

        #region WaferInfo
        public double PixelSizeX;
        public double PixelSizeY;
        public int EdgeExlusion_um;
        #endregion

        #region Data Haze 8 bit
        private Data Data_8bits = new Data();
        public Data D8
        {
            get { return Data_8bits; }
            private set { Data_8bits = value; }
        }
        #endregion

        #region Data Haze 256 bit
        private Data Data_256bits = new Data();
        public Data D256
        {
            get { return Data_256bits; }
            private set { Data_256bits = value; }
        }
        #endregion

        public HazeMeasure()
        {

        }

        public void Init(InputHazeConfiguration p_inputPrm)
        {
            RangeName = p_inputPrm.RangeName;
            int ncount = 0;
            foreach (var sclmax in p_inputPrm.RangeScaleMax)
            {
                RangeScaleMax[ncount] = sclmax;
                ncount++;
            }

            // 8 Bits Color maps
            ncount = 0;
            foreach (var colormap in p_inputPrm.ColorMap8)
            {
                Data_8bits.lutR[ncount] = colormap.R;
                Data_8bits.lutG[ncount] = colormap.G;
                Data_8bits.lutB[ncount] = colormap.B;
                ncount++;
            }
            if (ncount != 256)
                throw new ApplicationException("Haze ColorMap 8 : wrong number of elements (expected 256 values)");

            // compute level from Core values
            ncount = 0;
            int nSum = 0;
            List<int> ll = new List<int>(256);
            foreach (var Hazecore in p_inputPrm.CoreVal8)
            {
                Data_8bits.Levels[Hazecore.Index] = 0;
                ll.Clear();
                foreach (var Coreval in Hazecore.Cores.values)
                {
                    for (int i = 0; i < 256; i++)
                    {
                        if ((Data_8bits.lutR[i] == Coreval) || (Data_8bits.lutG[i] == Coreval) || (Data_8bits.lutB[i] == Coreval))
                        {
                            ll.Add(i);
                        }
                    }
                }
                Data_8bits.Levels[Hazecore.Index] = ll.Distinct().Count();
                nSum += Data_8bits.Levels[Hazecore.Index];
            }
            if (nSum != 256)
                throw new ApplicationException("Haze levels 8 total Error (!= 256) Check your color map  and core values configuration");



            // 256 Bits Color maps
            ncount = 0;
            foreach (var colormap in p_inputPrm.ColorMap256)
            {
                Data_256bits.lutR[ncount] = colormap.R;
                Data_256bits.lutG[ncount] = colormap.G;
                Data_256bits.lutB[ncount] = colormap.B;
                ncount++;
            }
            if (ncount != 256)
                throw new ApplicationException("Haze ColorMap 256 : wrong number of elements (expected 256 values)");

            // compute level from Core values

            Dictionary<int, int>[] dic = new Dictionary<int, int>[3]; //  <coreval, count> [Chan]
            dic[0] = new Dictionary<int, int>();// B
            dic[1] = new Dictionary<int, int>();// G
            dic[2] = new Dictionary<int, int>();// R
            for (int i = 0; i < 256; i++)
            {
                // red
                if (dic[2].ContainsKey(Data_256bits.lutR[i]))
                    dic[2][Data_256bits.lutR[i]] += 1;
                else
                    dic[2][Data_256bits.lutR[i]] = 1;

                // green
                if (dic[1].ContainsKey(Data_256bits.lutG[i]))
                    dic[1][Data_256bits.lutG[i]] += 1;
                else
                    dic[1][Data_256bits.lutG[i]] = 1;

                // blue
                if (dic[0].ContainsKey(Data_256bits.lutB[i]))
                    dic[0][Data_256bits.lutB[i]] += 1;
                else
                    dic[0][Data_256bits.lutB[i]] = 1;
            }

            for (int i = 0; i < 8; i++)
                Data_256bits.Levels[i] = 0;

            foreach (var Hazecore in p_inputPrm.CoreVal256)
            {
                foreach (var Coreval in Hazecore.Cores.values)
                {
                    if (dic[Hazecore.Chan].ContainsKey(Coreval)) // attention au haze core channel ici hsitoriquement R==2;G==1;B==0
                    {
                        Data_256bits.Levels[Hazecore.Index] += dic[Hazecore.Chan][Coreval];
                    }
                }
            }

            //  if(p_inputPrm.CoreVal256.Length < 256)
            //  {
            //      Data_256bits.Levels[7] += 256 - p_inputPrm.CoreVal256.Length;
            //  }

            nSum = 0;
            for (int i = 0; i < 8; i++)
                nSum += Data_256bits.Levels[i];
            if (nSum < 254 || nSum > 256)
                throw new ApplicationException("Haze levels 256 total Error (!= 256) Check your color map  and core values configuration");

        }

        public byte[] GetLut(int RGBIdx, bool bIs256)
        {
            byte[] ret = null;
            if (bIs256)
            {
                switch (RGBIdx)
                {
                    case _R_: // Red
                        ret = Data_256bits.lutR;
                        break;
                    case _G_: // Green
                        ret = Data_256bits.lutG;
                        break;
                    case _B_: // Blue
                        ret = Data_256bits.lutB;
                        break;
                    default: break;
                }
            }
            else
            {
                switch (RGBIdx)
                {
                    case _R_: // Red
                        ret = Data_8bits.lutR;
                        break;
                    case _G_: // Green
                        ret = Data_8bits.lutG;
                        break;
                    case _B_: // Blue
                        ret = Data_8bits.lutB;
                        break;
                    default: break;
                }
            }
            return ret;
        }
        //=================================================================
        // Dispose
        //=================================================================

        protected override void Dispose(bool disposing)
        {
            if (Data_8bits != null)
            {
                Data_8bits.Dispose();
                Data_8bits = null;
            }

            if (Data_256bits != null)
            {
                Data_256bits.Dispose();
                Data_256bits = null;
            }

            base.Dispose(disposing);
        }

        //=================================================================
        // Clonage
        //=================================================================
        protected override void CloneTo(DisposableObject obj)
        {
            //AcquisitionImageObject clone = (AcquisitionImageObject)obj;
            //clone.MilImage.AddRef();
        }
    }
}
