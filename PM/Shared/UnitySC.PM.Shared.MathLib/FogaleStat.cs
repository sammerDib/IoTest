using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitySC.PM.Shared.MathLib
{
    public class cStatResult
    {
        // variables membre
        private String m_Name;

        private double m_Average;
        private double m_Stdev;
        private double m_Min;
        private double m_Max;
        private double m_TTV;
        private double m_NbRepeatValid;
        private double _currentDeviation;

        // accesseur
        public String Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public double Average
        {
            get { return m_Average; }
            set { m_Average = value; }
        }

        public double Stdev
        {
            get { return m_Stdev; }
            set { m_Stdev = value; }
        }

        public double Min
        {
            get { return m_Min; }
            set { m_Min = value; }
        }

        public double Max
        {
            get { return m_Max; }
            set { m_Max = value; }
        }

        public double TTV
        {
            get { return m_TTV; }
            set { m_TTV = value; }
        }

        public double ValidRepeat
        {
            get { return m_NbRepeatValid; }
            set { m_NbRepeatValid = value; }
        }

        public double CurrentDeviation
        {
            get { return _currentDeviation; }
            set { _currentDeviation = value; }
        }

        // constructeur
        public cStatResult()
        {
            m_Name = "";
            m_Average = 0.0;
            m_Stdev = 0.0;
            m_Min = 0.0;
            m_Max = 0.0;
            m_TTV = 0.0;
            _currentDeviation = 0.0;
        }

        public cStatResult(String Name, double Ave, double stdev, double min, double max, double TTV, double ValidRepeat, double CurrentDeviation)
        {
            m_Name = Name;
            m_Average = Ave;
            m_Stdev = stdev;
            m_Min = min;
            m_Max = max;
            m_TTV = TTV;
            m_NbRepeatValid = ValidRepeat;
            _currentDeviation = CurrentDeviation;
        }
    }

    public class FogaleStat
    {
        public static double Average(double[] Echantillon)
        {
            double d_Ave = 0;
            for (int i = 0; i < Echantillon.Length; i += 1)
            {
                d_Ave += (Echantillon[i] / Echantillon.LongLength);
            }
            return d_Ave;
        }

        public static double Average(List<double> Echantillon)
        {
            double d_Ave = 0;
            for (int i = 0; i < Echantillon.Count; i += 1)
            {
                d_Ave += (Echantillon[i] / Echantillon.Count);
            }
            return d_Ave;
        }

        public static double Max(double[] Echantillon)
        { return Echantillon.Max(); }

        public static double Max(List<double> Echantillon)
        { return Echantillon.Max(); }

        public static double Min(double[] Echantillon)
        { return Echantillon.Min(); }

        public static double Min(List<double> Echantillon)
        { return Echantillon.Min(); }

        public static double TTV(double[] Echantillon)
        {
            return Echantillon.Max() - Echantillon.Min();
        }

        public static double TTV(List<double> Echantillon)
        {
            return Echantillon.Max() - Echantillon.Min();
        }

        public static double Stdev(double[] Echantillon)
        {
            if (Echantillon.LongLength <= 1)
            {
                return 0;
            }
            double EcartType = 0;
            double Moyenne = Average(Echantillon);
            double tmp;
            for (int i = 0; i < Echantillon.Length; i += 1)
            {
                tmp = Echantillon[i] - Moyenne;
                EcartType += (tmp * tmp);
            }
            EcartType = EcartType / (double)(Echantillon.LongLength - 1);
            return Math.Sqrt(EcartType);
        }

        public static double Stdev(List<double> Echantillon)
        {
            if (Echantillon.Count <= 1)
            {
                return 0;
            }
            double EcartType = 0;
            double Moyenne = Average(Echantillon);
            double tmp;
            for (int i = 0; i < Echantillon.Count; i += 1)
            {
                tmp = Echantillon[i] - Moyenne;
                EcartType += (tmp * tmp);
            }
            EcartType = EcartType / (double)(Echantillon.Count - 1);
            return Math.Sqrt(EcartType);
        }

        public static double CurrentDeviation(double[] Echantillon)
        {
            return 1;
        }

        public static double CurrentDeviation(List<double> Echantillon)
        {
            return 1;
        }
    }
}