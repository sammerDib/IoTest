using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FormatAHM
{
    // Résultas des heightmeasurement pour 1 die
    public class DieHMResults : ICloneable
    {
        private static int FORMAT_VERSION = 1;

        public int indexX { get; set; }
        public int indexY { get; set; }

        public int positionX { get; set; }
        public int positionY { get; set; }

        public int imageSizeX { get; set; }
        public int imageSizeY { get; set; }

        public float MeanHeight_um { get; set; }
        public float MaxHeight { get; set; }
        public float MinHeight { get; set; }
        public float StdDev { get; set; }

        public float Coplanarity { get; set; }

        public float ComputeMax() { return m_HeightMeasuresList.Max(mes => mes.Height_um); }
        public float ComputeMin() { return m_HeightMeasuresList.Min(mes => mes.Height_um); }

        private bool m_StatsUpdated = false;
        public float SubstrateCoplanarity { get; set; }

        public int NbMeasures
        {
            get
            {
                if (m_HeightMeasuresList != null)
                    return m_HeightMeasuresList.Count;
                else
                    return 0;
            }
        }

        private List<HeightMeasure> m_HeightMeasuresList = null;

        public DieHMResults()
        {
            m_HeightMeasuresList = new List<HeightMeasure>();
        }

        public DieHMResults(int idxX, int idxY, int iPosX, int iPosY, int iImgSizeX, int iImgSizeY)
        {
            m_HeightMeasuresList = new List<HeightMeasure>();

            indexX = idxX;
            indexY = idxY;
            positionX = iPosX;
            positionY = iPosY;
            imageSizeX = iImgSizeX;
            imageSizeY = iImgSizeY;
        }

        public void AddHeightMeasure(int iLabelMeasure, float fHeightValue_um, int iPosCenterX, int iPosCenterY)
        {
            if (m_HeightMeasuresList == null)
                m_HeightMeasuresList = new List<HeightMeasure>();
            m_HeightMeasuresList.Add(new HeightMeasure(iLabelMeasure, fHeightValue_um, iPosCenterX, iPosCenterY));
        }

        public void UpdateStats()
        {
            if (!m_StatsUpdated)
            {
                m_StatsUpdated = true;
                if (m_HeightMeasuresList != null)
                {
                    if (NbMeasures > 0)
                    {
                        var NoNanList = m_HeightMeasuresList.Where(MyMes => !float.IsNaN(MyMes.Height_um));
                        if ((NoNanList != null) && (NoNanList.Count() > 0))
                        {
                            MaxHeight = NoNanList.Max(mes => mes.Height_um);
                            MinHeight = NoNanList.Min(mes => mes.Height_um);
                            MeanHeight_um = NoNanList.Average(mes => mes.Height_um);
                            StdDev = (float)Math.Sqrt(NoNanList.Average(mes => Math.Pow(mes.Height_um - MeanHeight_um, 2)));
                        }
                        else
                        {
                            MaxHeight = float.NaN;
                            MinHeight = float.NaN;
                            MeanHeight_um = 0.0f;
                            StdDev = 0.0f;
                        }
                    }
                    else
                    {
                        MaxHeight = 0;
                        MinHeight = 0;
                        MeanHeight_um = 0.0f;
                        StdDev = 0.0f;
                    }
                }
            }
        }

        public List<Tuple<int, float, int, int>> DieHeightMeasures()
        {
            List<Tuple<int, float, int, int>> mesList = new List<Tuple<int, float, int, int>>(NbMeasures);
            foreach (HeightMeasure hm in m_HeightMeasuresList)
                mesList.Add(new Tuple<int, float, int, int>(hm.Label, hm.Height_um, hm.PositionX_px, hm.PositionY_px));
            return mesList;
        }

        public double ComputeDieMeasuresVariance(double p_dMean_um)
        {
            return m_HeightMeasuresList.Sum(mes => Math.Pow(mes.Height_um - p_dMean_um, 2));
        }

        public void Read(BinaryReader br)
        {
            int lVersion = br.ReadInt32();
            if (lVersion < 0 || lVersion > FORMAT_VERSION)
                throw new Exception("Bad file format version number in DieHMResults. Reading failed.");
            switch (lVersion)
            {
                case 0:
                    {
                        indexX = br.ReadInt32();
                        indexY = br.ReadInt32();
                        positionX = br.ReadInt32();
                        positionY = br.ReadInt32();
                        imageSizeX = br.ReadInt32();
                        imageSizeY = br.ReadInt32();

                        MeanHeight_um = br.ReadSingle();
                        MaxHeight = br.ReadSingle();
                        MinHeight = br.ReadSingle();
                        StdDev = br.ReadSingle();

                        Coplanarity = br.ReadSingle();

                        int nNbMeasuresToRead = br.ReadInt32();
                        if (m_HeightMeasuresList != null)
                            m_HeightMeasuresList.Clear();
                        m_HeightMeasuresList = new List<HeightMeasure>(nNbMeasuresToRead);
                        for (int i = 0; i < nNbMeasuresToRead; i++)
                        {
                            HeightMeasure hm = new HeightMeasure();
                            hm.Read(br);
                            m_HeightMeasuresList.Add(hm);
                        }
                    }
                    break;

                case 1:
                    {
                        indexX = br.ReadInt32();
                        indexY = br.ReadInt32();
                        positionX = br.ReadInt32();
                        positionY = br.ReadInt32();
                        imageSizeX = br.ReadInt32();
                        imageSizeY = br.ReadInt32();

                        MeanHeight_um = br.ReadSingle();
                        MaxHeight = br.ReadSingle();
                        MinHeight = br.ReadSingle();
                        StdDev = br.ReadSingle();

                        Coplanarity = br.ReadSingle();

                        int nNbMeasuresToRead = br.ReadInt32();
                        if (m_HeightMeasuresList != null)
                            m_HeightMeasuresList.Clear();
                        m_HeightMeasuresList = new List<HeightMeasure>(nNbMeasuresToRead);
                        for (int i = 0; i < nNbMeasuresToRead; i++)
                        {
                            HeightMeasure hm = new HeightMeasure();
                            hm.Read(br);
                            m_HeightMeasuresList.Add(hm);
                        }

                        SubstrateCoplanarity = br.ReadSingle();
                    }
                    break;
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(DieHMResults.FORMAT_VERSION);
            bw.Write(indexX);
            bw.Write(indexY);
            bw.Write(positionX);
            bw.Write(positionY);
            bw.Write(imageSizeX);
            bw.Write(imageSizeY);

            bw.Write(MeanHeight_um);
            bw.Write(MaxHeight);
            bw.Write(MinHeight);
            bw.Write(StdDev);

            bw.Write(Coplanarity);

            bw.Write(NbMeasures);
            if (m_HeightMeasuresList != null)
            {
                foreach (HeightMeasure hm in m_HeightMeasuresList)
                {
                    hm.Write(bw);
                }
            }

            bw.Write(SubstrateCoplanarity);
        }

        public double ComputeDevMaxFromPlane(double a, double b, double c)
        {
            //Soit z=a.x+b.y+c l'équation du plan .
            double zi = 0.0;
            double deviation = 0.0;
            double dMax = -double.MaxValue;
            foreach (HeightMeasure hm in m_HeightMeasuresList)
            {
                //zi = -(a * bump.PositionX_um + b * bump.PositionY_um + d) / c; // ????
                zi = a * (double)hm.PositionX_px + b * (double)hm.PositionY_px + c;
                deviation = Math.Abs(zi - hm.Height_um);
                if (deviation > dMax)
                    dMax = deviation;
            }
            return dMax;
        }

        public double ComputeDistanceFromMinMaxPlane(double a, double b, double c)
        {
            //Soit z=a.x+b.y+c l'équation du plan de Regression LMS.
            // coplanarity is the distance between parallel plane passing via maximum height and
            // parallele plane passing via minimum height

            // retreive max and min
            HeightMeasure hMaxBump = new HeightMeasure(-666, float.MinValue, 0, 0);
            HeightMeasure hMinBump = new HeightMeasure(-666, float.MaxValue, 0, 0);
            foreach (HeightMeasure hm in m_HeightMeasuresList)
            {
                if (hMaxBump.Height_um < hm.Height_um)
                    hMaxBump = hm;
                if (hMinBump.Height_um > hm.Height_um)
                    hMinBump = hm;
            }

            // Nota Bene : here we dealing with deviation from maximum (respc. minimum) height measure. Depending of plane those could not be
            // THE Maximum deviation (respc. Minimum) toward reference regression plane
            double deviationBumpMax = (double)hMaxBump.Height_um - (a * (double)hMaxBump.PositionX_px + b * (double)hMaxBump.PositionY_px + c);
            double deviationBumpMin = (double)hMinBump.Height_um - (a * (double)hMinBump.PositionX_px + b * (double)hMinBump.PositionY_px + c);

            // plane ref max : a * x + b * y - 1 * z + c + deviationBumpMax = 0
            // plane ref min : a * x + b * y - 1 * z + c + deviationBumpMin = 0
            // Distance between those 2 parallele plane : copla = abs( deviationBumpMax - deviationBumpMin) / sqrt( a²+b²+(-1)² )
            double squaredNorm = a * a + b * b + 1.0; // a²+b²+(-1)²
            double coplanarity = Math.Abs(deviationBumpMax - deviationBumpMin) / Math.Sqrt(squaredNorm);

            //// Mathematics "Fun" , project maximum Height bump to Minimum ref plane

            //// coordinates of maximum height measure in max ref plane
            //double xa = (double)hMaxBump.PositionX_px;
            //double ya = (double)hMaxBump.PositionY_px;
            //double za = (a * (double)hMaxBump.PositionX_px + b * (double)hMaxBump.PositionY_px + c + deviationBumpMax);
            //double lambda = (a * xa + b * ya - 1.0 * za + c + deviationBumpMin) / squaredNorm;
            //// orthogonal projection onto minimum ref plane
            //double xb = xa - lambda * a;
            //double yb = ya - lambda * b;
            //double zb = za - lambda * -1.0;
            //// check if distance AB is the same than the copla
            //double dAB = Math.Sqrt((xb - xa) * (xb - xa) + (yb - ya) * (yb - ya) + (zb - za) * (zb - za));
            //if (Math.Abs(dAB - coplanarity) >= 0.0000001)
            //    throw new Exception($"coplanarity error calculation dAb={dAB} copla={coplanarity} " +
            //        $"DIFF = {dAB- coplanarity}\n" +
            //        $"l={lambda} sn={squaredNorm}\n" +
            //        $" A  = ({xa};\t{ya};\t{za})\n"+
            //        $" B  = ({xb};\t{yb};\t{zb})\n" +
            //        $" Hmax = {hMaxBump.Height_um} Hmin = {hMinBump.Height_um}\n" +
            //        $" a={a}, b={b} c={c}  dif={deviationBumpMax - deviationBumpMin} snorm ={squaredNorm}");

            return coplanarity;
        }


        #region ICloneable Members

        protected object DeepCopy()
        {
            DieHMResults cloned = MemberwiseClone() as DieHMResults;
            return cloned;
        }

        public virtual object Clone()
        {
            return MemberwiseClone(); // shallow copy
            //return DeepCopy();
        }

        #endregion


    }
}
